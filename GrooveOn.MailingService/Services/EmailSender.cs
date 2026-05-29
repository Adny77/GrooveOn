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
    string resetToken)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(_settings.FromName, _settings.FromEmail)
        );
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = "Reset lozinke – GrooveOn";

        message.Body = new TextPart("html")
{
    Text = $"""
        <div style="font-family: Arial, sans-serif;">
            <h2>Zdravo {name},</h2>

            <p>Primili smo zahtjev za reset lozinke za Vaš GrooveOn nalog <strong>{username}</strong>.</p>

            <p>Unesite sljedeći kod u aplikaciju kako biste postavili novu lozinku:</p>

            <div style="background:#f4f4f4;padding:14px;border-radius:6px;margin:16px 0;text-align:center;">
                <span style="font-size:18px;letter-spacing:3px;font-weight:bold;color:#1a1a1a;font-family:monospace;">{resetToken}</span>
            </div>

            <p>Kod je važeći <strong>1 sat</strong>. Ako niste tražili reset lozinke, ignorišite ovaj email.</p>

            <br/>
            <small>GrooveOn tim</small>
        </div>
    """
};

        await SendAsync(message);
    }

    public async Task SendPasswordChangedEmailAsync(
        string to,
        string name,
        string username,
        DateTime changedAt)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = "Lozinka promijenjena – GrooveOn";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <div style="font-family: Arial, sans-serif;">
                    <h2>Zdravo {name},</h2>

                    <p>Vaša lozinka na GrooveOn nalogu <strong>{username}</strong> je uspješno promijenjena.</p>

                    <p><strong>Datum i vrijeme:</strong> {changedAt:dd.MM.yyyy HH:mm} UTC</p>

                    <p>Ako niste Vi napravili ovu promjenu, odmah kontaktirajte podršku ili koristite opciju "Zaboravljena lozinka".</p>

                    <br/>
                    <small>GrooveOn tim</small>
                </div>
            """
        };

        await SendAsync(message);
    }

    private async Task SendAsync(MimeMessage message)
    {
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