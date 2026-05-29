namespace GrooveOn.MailingService.Messages
{
    public class PasswordChangedEmailMessage
    {
        public string To { get; set; } = "";
        public string Name { get; set; } = "";
        public string UserName { get; set; } = "";
        public DateTime ChangedAt { get; set; }
    }
}
