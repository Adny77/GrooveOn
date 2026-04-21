namespace GrooveOn.MailingService.Configuration
{
    public class AppConfig
    {
        public string ResetPasswordQueue { get; set; } = "email.reset-password";
    }
}