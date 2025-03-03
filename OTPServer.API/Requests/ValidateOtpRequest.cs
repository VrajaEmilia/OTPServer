namespace OtpNet.Api.Requests;

public class ValidateOtpRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}
