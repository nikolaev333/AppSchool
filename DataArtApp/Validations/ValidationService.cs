namespace DataArtApp.Validations
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using DataArtApp.DTOs;

    public static class ValidationService
    {
        public static List<string> ValidatePersonDTO(PersonDTO dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length < 4 || dto.Name.Length > 30)
            {
                errors.Add("Name must be between 4 and 30 characters long.\n");
            }

            // Validate DateOfBirth 
            if (dto.DateOfBirth.Year < 1900 || dto.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            {
                errors.Add("Invalid Date of Birth.\n");
            }

            // Validate PIN 10 digits
            if (!Regex.IsMatch(dto.PIN, @"^\d{10}$"))
            {
                errors.Add("PIN must consist of exactly 10 digits.\n");
            }

            // Validate Email
            if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add("Email is not in a valid format.\n");
            }
            return errors;
        }
    }

}
