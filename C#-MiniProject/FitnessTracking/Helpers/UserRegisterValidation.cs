using FitnessTracking.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Helpers
{
    public class MininumLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public MininumLengthAttribute(int minLength)
        {
            _minLength = minLength;
            ErrorMessage = $"Minium length should be {_minLength} characters";
        }
        public override bool IsValid(object? value)
        {
            if (value is string str)
            {
                return str.Length >= _minLength;
            }
            return false;
        }
    }
}