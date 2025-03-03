namespace OTPServer.Business.Dtos
{
    public class OtpDto
    {
        public string EncryptedCode { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
