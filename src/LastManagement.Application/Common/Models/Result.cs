using LastManagement.Utilities.Constants.Global;

namespace LastManagement.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException(ResultMessages.Common.SUCCESS_FAILURE_ERROR_MISMATCH);

        if (!isSuccess && error == null)
            throw new InvalidOperationException(ResultMessages.Common.FAILURE_REQUIRES_ERROR);

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
}