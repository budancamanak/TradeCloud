using Common.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Market.Application.Exceptions;

public class NoPageCalculatedException() : ExceptionBase(StatusCodes.Status400BadRequest,"No page was calculated for given data");