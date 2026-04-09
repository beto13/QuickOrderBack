namespace QuickOrder.Application.Common;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string message) => new("NOT_FOUND", message);
    public static Error Business(string message) => new("BUSINESS_ERROR", message);
    public static Error Validation(string message) => new("VALIDATION_ERROR", message);
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = Error.None;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Error error) => Fail(error);
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    private Result()
    {
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Ok() => new();
    public static Result Fail(Error error) => new(error);

    public static implicit operator Result(Error error) => Fail(error);
}
