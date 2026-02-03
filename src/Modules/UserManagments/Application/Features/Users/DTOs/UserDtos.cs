using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.DTOs
{
    public class UserSignupRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserSignupResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
    }

    public class CheckAvailabilityRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class CheckAvailabilityResponse
    {
        public bool Available { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
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
    }
}
