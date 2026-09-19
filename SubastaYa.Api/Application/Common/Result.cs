using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public ErrorType ErrorType { get; }
        public string? ErrorMessage { get; }

        protected Result(bool isSuccess, ErrorType errorType, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true, ErrorType.None, null);

        public static Result Failure(ErrorType errorType, string message) =>
            new(false, errorType, message);
    }
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, ErrorType.None, null) => Value = value;

        private Result(ErrorType errorType, string message)
            : base(false, errorType, message) => Value = default;

        public static Result<T> Success(T value) => new(value);

        public static new Result<T> Failure(ErrorType errorType, string message) =>
            new(errorType, message);
    }
}
