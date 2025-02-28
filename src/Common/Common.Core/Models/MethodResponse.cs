namespace Common.Core.Models;

public class MethodResponse
{
    public bool IsSuccess { get; set; }
    public int Id { get; set; }
    public string Message { get; set; } = "";


    private MethodResponse()
    {
    }

    public static MethodResponse Success(string message)
    {
        return Success(-1, message);
    }

    public static MethodResponse Success(int id, string message)
    {
        return new MethodResponse
        {
            Id = id,
            Message = message,
            IsSuccess = true
        };
    }

    public static MethodResponse Error(Exception exception)
    {
        return Error(-1, exception.Message);
    }

    public static MethodResponse Error(string message)
    {
        return Error(-1, message);
    }

    public static MethodResponse Error(int id, string message)
    {
        return new MethodResponse
        {
            Id = id,
            Message = message,
            IsSuccess = false
        };
    }

    public override string ToString()
    {
        return $"MethodResponse[{IsSuccess}]:{Id} - {Message}";
    }
}