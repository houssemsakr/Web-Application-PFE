namespace Web_Application_PFE.Models
{
    public class AuthMessageSenderOptions
    {
        public string? FromEmail { get; set; }
        public string? Password { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
    }
}
