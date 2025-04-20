using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Common.Web.Attributes.Security;

public class HasPermissionAttribute(Permission permission) : AuthorizeAttribute(policy: permission.ToString())
{
}