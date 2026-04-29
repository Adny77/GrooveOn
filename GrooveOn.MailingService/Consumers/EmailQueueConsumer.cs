using GrooveOn.MailingService.Configuration;
using GrooveOn.MailingService.Messages;
using GrooveOn.MailingService.Services;
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

    public EmailQueueConsumer(
        IConnection connection,
        EmailSender emailSender,
        IOptions<AppConfig> config)
    {
        _connection = connection;
        _emailSender = emailSender;
        _config = config.Value;
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

        await _channel.BasicQosAsync(0, 1, false);
    }

    public async Task StartAsync()
    {
        if (_channel == null)
            throw new InvalidOperationException("Channel is not initialized. Call InitializeAsync first.");

        var resetConsumer = new AsyncEventingBasicConsumer(_channel);

        resetConsumer.ReceivedAsync += async (sender, e) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<ResetPasswordEmailMessage>(json)
                    ?? throw new InvalidOperationException("Invalid message.");

                await _emailSender.SendResetPasswordEmailAsync(
    message.To,
    message.Name,
    message.UserName,
    message.NewPassword
);

                await _channel.BasicAckAsync(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RESET EMAIL ERROR] {ex.Message}");

                await _channel.BasicNackAsync(
                    e.DeliveryTag,
                    false,
                    requeue: false
                );
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _config.ResetPasswordQueue,
            autoAck: false,
            consumer: resetConsumer
        );
    }
}