using DotNetEnv;
using GrooveOn.MailingService.Configuration;
using GrooveOn.MailingService.Consumers;
using GrooveOn.MailingService.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

var rabbitSettings = new RabbitMqSettings
{
    Host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
    Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
    User = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
    VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VIRTUALHOST") ?? "/"
};

var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST")
    ?? throw new InvalidOperationException("Missing required env var: SMTP_HOST");
var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")
    ?? throw new InvalidOperationException("Missing required env var: SMTP_USER");
var fromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL")
    ?? throw new InvalidOperationException("Missing required env var: FROM_EMAIL");

var smtpSettings = new SmtpSettings
{
    Host = smtpHost,
    Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587"),
    User = smtpUser,
    Password = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "",
    FromEmail = fromEmail,
    FromName = Environment.GetEnvironmentVariable("FROM_NAME") ?? "GrooveOn",
    UseSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_SSL") ?? "false")
};

builder.Services.AddEmailConsumer(rabbitSettings, smtpSettings);

builder.Services.Configure<AppConfig>(options =>
{
    options.ResetPasswordQueue = "email.reset-password";
    options.PasswordChangedQueue = "email.password-changed";
});


var host = builder.Build();

var consumer = host.Services.GetRequiredService<EmailQueueConsumer>();
await consumer.InitializeAsync();
await consumer.StartAsync();

await host.RunAsync();