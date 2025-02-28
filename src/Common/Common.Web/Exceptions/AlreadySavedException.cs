using Common.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Exceptions;

public class AlreadySavedException : ExceptionBase
{
    public AlreadySavedException() : base(StatusCodes.Status409Conflict, "Already saved")
    {
    }

    public AlreadySavedException(string message) : base(StatusCodes.Status409Conflict, message)
    {
    }

    public static readonly Func<Exception> Creator = () => new AlreadySavedException();
}