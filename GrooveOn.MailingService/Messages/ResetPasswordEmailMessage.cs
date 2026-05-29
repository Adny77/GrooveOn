namespace GrooveOn.MailingService.Messages
{
    public class ResetPasswordEmailMessage
    {
        public string To { get; set; } = "";
        public string Name { get; set; } = "";
        public string UserName { get; set; } = "";
        public string ResetToken { get; set; } = "";
    }
}