namespace Security.Application.Features.Checks;

public abstract class BaseCheckRequest
{
    public string Token { get; set; }
    public string ClientIp { get; set; }
}