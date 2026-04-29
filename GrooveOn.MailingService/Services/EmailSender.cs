using GrooveOn.MailingService.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GrooveOn.MailingService.Services;

public class EmailSender
{
    private readonly SmtpSettings _settings;

    public EmailSender(SmtpSettings settings)
    {
        _settings = settings;
    }

    public async Task SendResetPasswordEmailAsync(
    string to,
    string name,
    string username,
    string newPassword)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(_settings.FromName, _settings.FromEmail)
        );
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = "Nova lozinka – GrooveOn";

        message.Body = new TextPart("html")
{
    Text = $"""
        <div style="font-family: Arial, sans-serif;">
            <h2>Zdravo {name},</h2>

            <p>Vaši podaci za prijavu:</p>

            <p><strong>Username:</strong> {username}</p>
            <p><strong>Nova lozinka:</strong></p>

            <div style="background:#f4f4f4;padding:10px;border-radius:5px;">
                <h3 style="margin:0;">{newPassword}</h3>
            </div>

            <p>Preporučujemo da se odmah prijavite i promijenite lozinku.</p>

            <br/>
            <small>GrooveOn tim</small>
        </div>
    """
};

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            _settings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _settings.User,
            _settings.Password
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}