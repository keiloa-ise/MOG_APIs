using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Common.Services
{
    public interface IPasswordPolicyService
    {
        bool IsStrongPassword(string password);
        List<string> ValidatePasswordStrength(string password);
        string GenerateStrongPassword();
    }

    public class PasswordPolicyService : IPasswordPolicyService
    {
        private readonly int _minimumLength = 8;
        private readonly bool _requireUppercase = true;
        private readonly bool _requireLowercase = true;
        private readonly bool _requireDigit = true;
        private readonly bool _requireSpecialChar = true;
        private readonly string _specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        public bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < _minimumLength)
                return false;

            if (_requireUppercase && !password.Any(char.IsUpper))
                return false;

            if (_requireLowercase && !password.Any(char.IsLower))
                return false;

            if (_requireDigit && !password.Any(char.IsDigit))
                return false;

            if (_requireSpecialChar && !password.Any(ch => _specialCharacters.Contains(ch)))
                return false;

            // Check for common weak patterns
            if (IsCommonPassword(password))
                return false;

            return true;
        }

        public List<string> ValidatePasswordStrength(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password cannot be empty");
                return errors;
            }

            if (password.Length < _minimumLength)
                errors.Add($"Password must be at least {_minimumLength} characters long");

            if (_requireUppercase && !password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter");

            if (_requireLowercase && !password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter");

            if (_requireDigit && !password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit");

            if (_requireSpecialChar && !password.Any(ch => _specialCharacters.Contains(ch)))
                errors.Add($"Password must contain at least one special character ({_specialCharacters})");

            if (IsCommonPassword(password))
                errors.Add("Password is too common or weak");

            return errors;
        }

        public string GenerateStrongPassword()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var random = new Random();
            var passwordChars = new List<char>();

            // Ensure at least one of each required character type
            passwordChars.Add(upperCase[random.Next(upperCase.Length)]);
            passwordChars.Add(lowerCase[random.Next(lowerCase.Length)]);
            passwordChars.Add(digits[random.Next(digits.Length)]);
            passwordChars.Add(specialChars[random.Next(specialChars.Length)]);

            // Fill remaining characters randomly
            var allChars = upperCase + lowerCase + digits + specialChars;
            for (int i = 4; i < 12; i++) // Generate 12-character password
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters
            return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
        }

        private bool IsCommonPassword(string password)
        {
            var commonPasswords = new List<string>
            {
                "password", "123456", "12345678", "1234", "qwerty",
                "password123", "admin", "welcome", "monkey", "sunshine"
            };

            return commonPasswords.Contains(password.ToLower());
        }
    }
}
