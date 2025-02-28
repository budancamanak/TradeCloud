using Common.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Exceptions;

public class IllegalStateException(string message) : ExceptionBase(StatusCodes.Status409Conflict, message)
{
}