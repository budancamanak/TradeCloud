using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequest : BaseCheckRequest, IRequest<MethodResponse>
{
    public string Permission { get; set; }
}