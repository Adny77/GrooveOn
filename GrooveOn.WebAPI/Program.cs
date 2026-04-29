using DotNetEnv;
using GrooveOn.MailingService.Configuration;
using GrooveOn.Services;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.PaymentStateMachine;
using GrooveOn.Services.Services;
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

int GetIntEnv(string key, int defaultValue)
{
    var raw = Environment.GetEnvironmentVariable(key) ?? builder.Configuration[key];
    return int.TryParse(raw, out var value) ? value : defaultValue;
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

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = GetOptionalEnv("RABBITMQ_HOST") ?? "localhost",
        Port = GetIntEnv("RABBITMQ_PORT", 5672),
        UserName = GetOptionalEnv("RABBITMQ_USERNAME") ?? "guest",
        Password = GetOptionalEnv("RABBITMQ_PASSWORD") ?? "guest",
        VirtualHost = GetOptionalEnv("RABBITMQ_VIRTUALHOST") ?? "/"
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

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

// SERVICES
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
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddScoped<ISubscriptionService, GrooveOn.Services.Services.SubscriptionService>();

// STRIPE
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

// PAYMENT STATE MACHINE
builder.Services.AddTransient<BasePaymentState, PendingPaymentState>();
builder.Services.AddTransient<PendingPaymentState>();
builder.Services.AddTransient<ProcessingPaymentState>();
builder.Services.AddTransient<PaidPaymentState>();
builder.Services.AddTransient<FailedPaymentState>();
builder.Services.AddTransient<CancelledPaymentState>();

// AUTH
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

        switch (error)
        {
            case UserException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case ForbiddenException ex:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case InvalidOperationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            case NotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
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

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<GrooveOnDbContext>();
//     db.Database.Migrate();
// }

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();