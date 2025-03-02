using Common.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.Web;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = SafeRequestPath(httpContext.Request.Path)
        };

        if (exception is FluentValidation.ValidationException fluentException)
        {
            problemDetails.Title = "one or more validation errors occurred.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var validationErrors = fluentException.Errors.Select(error => error.ErrorMessage).ToList();

            problemDetails.Extensions.Add("errors", validationErrors);
            problemDetails.Extensions.Add("errorsString", string.Join(',', validationErrors));
            problemDetails.Status = StatusCodes.Status400BadRequest;
        }
        else
        {
            if (exception is ExceptionBase baseException)
            {
                problemDetails.Status = baseException.StatusCode;
            }
            else
            {
                problemDetails.Status = StatusCodes.Status500InternalServerError;
            }

            problemDetails.Title = exception.Message;
        }

        logger.LogError("{Path}:[{ErrorCode}]>{ProblemDetailsTitle} : {Extensions}", problemDetails.Instance,
            problemDetails.Status,
            problemDetails.Title, problemDetails.Extensions);

        httpContext.Response.StatusCode = problemDetails.Status.GetValueOrDefault(httpContext.Response.StatusCode);
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private static string SafeRequestPath(PathString requestPath) => requestPath.HasValue
        ? requestPath.Value.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "")
        : "";
}