using Common.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Exceptions;

public class RequestValidationException(string message) : ExceptionBase(StatusCodes.Status400BadRequest, message)
{
}