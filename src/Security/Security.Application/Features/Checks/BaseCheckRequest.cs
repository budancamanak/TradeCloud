namespace Security.Application.Features.Checks;

public abstract class BaseCheckRequest
{
    public required string Token { get; set; }
    public required string ClientIp { get; set; }
}