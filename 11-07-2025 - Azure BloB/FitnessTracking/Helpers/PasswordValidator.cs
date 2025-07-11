using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Helpers
{
    public class PasswordValidator : ValidationAttribute
    {
        public PasswordValidator()
        {
            ErrorMessage = "Password must contain at least 8 characters, 1 Upper case letter, 1 lower case letter, 1 digit and 1 special characters";
        }

        public override bool IsValid(object? value)
        {
            if (value is string password)
            {
                return password.Length >= 8 && Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"[\a-z]") && Regex.IsMatch(password, @"[0-9]") && Regex.IsMatch(password, @"[\W_]");
            }
            return false;
        }
    }
}