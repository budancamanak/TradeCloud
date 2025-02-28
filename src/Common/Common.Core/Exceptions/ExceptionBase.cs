namespace Common.Core.Exceptions;

public class ExceptionBase(int statusCode,string message) : Exception(message)
{
    public int StatusCode => statusCode;
}