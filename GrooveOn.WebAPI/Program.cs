using DotNetEnv;
using GrooveOn.MailingService.Configuration;
using GrooveOn.Services;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.Services;
using GrooveOn.Services.PaymentStateMachine;
using GrooveOn.WebAPI.Authentication;
using GrooveOn.WebAPI.Configuration;
using GrooveOn.WebAPI.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Stripe;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (System.IO.File.Exists(envPath))
{
    Env.Load(envPath);
}

string? GetOptionalEnvEarly(string key, string? fallback = null)
    => Environment.GetEnvironmentVariable(key) ?? fallback;

int GetIntEnvEarly(string key, int defaultValue)
    => int.TryParse(Environment.GetEnvironmentVariable(key), out var v) ? v : defaultValue;

var rabbitConnection = await new ConnectionFactory
{
    HostName = GetOptionalEnvEarly("RABBITMQ_HOST", "localhost")!,
    Port = GetIntEnvEarly("RABBITMQ_PORT", 5672),
    UserName = GetOptionalEnvEarly("RABBITMQ_USERNAME", "guest")!,
    Password = GetOptionalEnvEarly("RABBITMQ_PASSWORD", "guest")!,
    VirtualHost = GetOptionalEnvEarly("RABBITMQ_VIRTUALHOST", "/")!
}.CreateConnectionAsync();

var builder = WebApplication.CreateBuilder(args);

string GetRequiredEnv(string key)
{
    var value = Environment.GetEnvironmentVariable(key) ?? builder.Configuration[key];

    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException($"Missing env var: {key}");

    return value;
}

string? GetOptionalEnv(string key)
{
    return Environment.GetEnvironmentVariable(key) ?? builder.Configuration[key];
}

var connectionString = GetRequiredEnv("CONNECTION_STRING");

builder.Services.AddDbContext<GrooveOnDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("GrooveOn.Services")
    ));

builder.Services.Configure<AppConfig>(options =>
{
    options.ResetPasswordQueue = "email.reset-password";
    options.PaymentCurrency = GetOptionalEnv("PAYMENT_CURRENCY") ?? "eur";
});

builder.Services.AddSingleton<IConnection>(rabbitConnection);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GrooveOn API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

TypeAdapterConfig.GlobalSettings.Default
    .IgnoreNullValues(true)
    .PreserveReference(true)
    .ShallowCopyForSameType(true);

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAlbumGenreService, AlbumGenreService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IPlayHistoryService, PlayHistoryService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMusicSearchEngineService, MusicSearchEngineService>();
builder.Services.AddScoped<IMusicResolveService, MusicResolveService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IPlaylistSongService, PlaylistSongService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddScoped<ISubscriptionService, GrooveOn.Services.Services.SubscriptionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<BasePaymentState>();
builder.Services.AddScoped<PendingPaymentState>();
builder.Services.AddScoped<ProcessingPaymentState>();
builder.Services.AddScoped<PaidPaymentState>();
builder.Services.AddScoped<FailedPaymentState>();
builder.Services.AddScoped<CancelledPaymentState>();

var stripeSecretKey = GetRequiredEnv("STRIPE_SECRET_KEY");
var stripeWebhookSecret = GetRequiredEnv("STRIPE_WEBHOOK_SECRET");

StripeConfiguration.ApiKey = stripeSecretKey;

builder.Services.AddSingleton(new StripeSettings
{
    SecretKey = stripeSecretKey,
    WebhookSecret = stripeWebhookSecret
});

builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        switch (error)
        {
            case UserException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogWarning(ex, "Business rule violation: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case ForbiddenException ex:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                logger.LogWarning(ex, "Forbidden access: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case InvalidOperationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogWarning(ex, "Invalid operation: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case NotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                logger.LogInformation(ex, "Not found: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case UnauthorizedAccessException ex:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                logger.LogWarning(ex, "Unauthorized: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case ArgumentException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogWarning(ex, "Argument error: {Message} | Path: {Path}",
                    ex.Message, context.Request.Path);
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case DbUpdateException dbEx
                when dbEx.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true
                  || dbEx.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                logger.LogWarning(dbEx, "Unique constraint violation | Path: {Path}", context.Request.Path);
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "A record with the same data already exists."
                });
                break;

            case DbUpdateException dbEx
                when dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true
                  || dbEx.InnerException?.Message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase) == true:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogWarning(dbEx, "FK constraint violation | Path: {Path}", context.Request.Path);
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Cannot delete this record because it is referenced by other data in the system."
                });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(error, "Unhandled exception | Path: {Path} | Method: {Method} | User: {User}",
                    context.Request.Path,
                    context.Request.Method,
                    context.User?.Identity?.Name ?? "anonymous");
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred."
                });
                break;
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GrooveOnDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var retries = 10;
    var delay = TimeSpan.FromSeconds(5);
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (retries > 1)
        {
            retries--;
            logger.LogWarning("Migration failed ({Remaining} retries left): {Msg}", retries, ex.Message);
            Thread.Sleep(delay);
            delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 30));
        }
    }
}

app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
