using GrooveOn.MailingService.Configuration;
using GrooveOn.MailingService.Messages;
using GrooveOn.MailingService.Services;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GrooveOn.MailingService.Consumers;

public class EmailQueueConsumer
{
    private IChannel _channel = null!;
    private readonly IConnection _connection;
    private readonly EmailSender _emailSender;
    private readonly AppConfig _config;
    private readonly ILogger<EmailQueueConsumer> _logger;

    private const int MaxRetries = 3;

    public EmailQueueConsumer(
        IConnection connection,
        EmailSender emailSender,
        IOptions<AppConfig> config,
        ILogger<EmailQueueConsumer> logger)
    {
        _connection = connection;
        _emailSender = emailSender;
        _config = config.Value;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: _config.ResetPasswordQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await _channel.QueueDeclareAsync(
            queue: _config.PasswordChangedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await _channel.BasicQosAsync(0, 1, false);

        _logger.LogInformation("EmailQueueConsumer initialised. Queues: {Q1}, {Q2}",
            _config.ResetPasswordQueue, _config.PasswordChangedQueue);
    }

    public async Task StartAsync()
    {
        if (_channel == null)
            throw new InvalidOperationException("Channel is not initialized. Call InitializeAsync first.");

        var resetConsumer = new AsyncEventingBasicConsumer(_channel);

        resetConsumer.ReceivedAsync += async (sender, e) =>
        {
            var deliveryTag = e.DeliveryTag;

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var json = Encoding.UTF8.GetString(e.Body.ToArray());

                    var message =
                        JsonSerializer.Deserialize<ResetPasswordEmailMessage>(json)
                        ?? throw new InvalidOperationException("Invalid message payload.");

                    await _emailSender.SendResetPasswordEmailAsync(
                        message.To,
                        message.Name,
                        message.UserName,
                        message.ResetToken
                    );

                    await _channel.BasicAckAsync(deliveryTag, false);

                    _logger.LogInformation("Reset-password email sent to {Email} (attempt {Attempt})", message.To, attempt);
                    return;
                }
                catch (Exception ex) when (attempt < MaxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                    _logger.LogWarning(ex, "Email send failed (attempt {Attempt}/{Max}). Retrying in {Delay}s...", attempt, MaxRetries, delay.TotalSeconds);
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Email send failed after {Max} attempts. Message discarded.", MaxRetries);

                    await _channel.BasicNackAsync(deliveryTag, false, requeue: false);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _config.ResetPasswordQueue,
            autoAck: false,
            consumer: resetConsumer
        );

        var changedConsumer = new AsyncEventingBasicConsumer(_channel);

        changedConsumer.ReceivedAsync += async (sender, e) =>
        {
            var deliveryTag = e.DeliveryTag;

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var json = Encoding.UTF8.GetString(e.Body.ToArray());

                    var message =
                        JsonSerializer.Deserialize<PasswordChangedEmailMessage>(json)
                        ?? throw new InvalidOperationException("Invalid message payload.");

                    await _emailSender.SendPasswordChangedEmailAsync(
                        message.To,
                        message.Name,
                        message.UserName,
                        message.ChangedAt
                    );

                    await _channel.BasicAckAsync(deliveryTag, false);

                    _logger.LogInformation("Password-changed email sent to {Email} (attempt {Attempt})", message.To, attempt);
                    return;
                }
                catch (Exception ex) when (attempt < MaxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                    _logger.LogWarning(ex, "Email send failed (attempt {Attempt}/{Max}). Retrying in {Delay}s...", attempt, MaxRetries, delay.TotalSeconds);
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Password-changed email failed after {Max} attempts. Message discarded.", MaxRetries);
                    await _channel.BasicNackAsync(deliveryTag, false, requeue: false);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _config.PasswordChangedQueue,
            autoAck: false,
            consumer: changedConsumer
        );

        _logger.LogInformation("EmailQueueConsumer listening on queues: {Q1}, {Q2}",
            _config.ResetPasswordQueue, _config.PasswordChangedQueue);
    }
}
