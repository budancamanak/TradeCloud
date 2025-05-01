using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks;

public abstract class BaseCheckRequest<T> : IRequest<T>
{
    public required string Token { get; set; }
    public required string ClientIp { get; set; }
}