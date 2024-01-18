namespace DTOs.MainApp.BL
{
    public class SendMailModelDTO
    {
        public string FromUsername { get; set; }
        public string FromEmail { get; set; }
        public string ToUsername { get; set; }
        public string ToEmail { get; set; }
        public string MessageSubject { get; set; }
        public string MessageBodyPlain { get; set; }
        public string MessageBody { get; set; }
        public SmtpClientModelDTO SmtpClientModel { get; set; }
    }
}
