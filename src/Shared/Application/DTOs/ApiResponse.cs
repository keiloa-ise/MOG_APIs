using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MOJ.Shared.Application.DTOs
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Status = "success",
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> Error(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Status = "error",
                Message = message,
                Data = default,
                Errors = errors
            };
        }

        public static ApiResponse<T> Created(T data, string message = "Created successfully")
        {
            return new ApiResponse<T>
            {
                Status = "created",
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Status = "not_found",
                Message = message,
                Data = default,
                Errors = null
            };
        }
    }

    // For responses without data
    public class ApiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; }

        public static ApiResponse Success(string message = "Success")
        {
            return new ApiResponse
            {
                Status = "success",
                Message = message,
                Errors = null
            };
        }

        public static ApiResponse Error(string message, List<string> errors = null)
        {
            return new ApiResponse
            {
                Status = "error",
                Message = message,
                Errors = errors
            };
        }
    }
}
