namespace GrooveOn.MailingService.Configuration
{
    public class AppConfig
    {
        public string ResetPasswordQueue { get; set; } = "email.reset-password";
        public string PasswordChangedQueue { get; set; } = "email.password-changed";
        public string PaymentCurrency { get; set; } = "eur";
    }
}