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
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");

        if (exception is FluentValidation.ValidationException fluentException)
        {
            problemDetails.Title = "one or more validation errors occurred.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            List<string> validationErrors = new List<string>();
            foreach (var error in fluentException.Errors)
            {
                validationErrors.Add(error.ErrorMessage);
            }

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
}