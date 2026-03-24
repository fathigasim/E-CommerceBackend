using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Common.Settings
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }
        
        public IReadOnlyList<string> Errors { get; } = new List<string>();
        private Result(bool isSuccess, T? data, string? errorMessage, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            if (errors != null) Errors = errors;
        }

        public static Result<T> Success(T data) 
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return new(true, data, null);
        }
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
        public static Result<T> Failure(List<string> errors)
            => new(false, default, string.Join("; ", errors), errors);
    }
}