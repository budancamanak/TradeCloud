using Common.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Market.Application.Exceptions;

public class NoPriceFetchedException() : ExceptionBase(StatusCodes.Status500InternalServerError,
    "Failed to fetch price information from exchange");