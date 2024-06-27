using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.AuthModels
{
    public class ResetPasswordModel:IValidatableObject
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Password) });
            }
            else
            {
                if (!IsValidPassword(Password))
                {
                    yield return new ValidationResult("Password should have atleast a capital , small and digit and should " +
                        "be greater than 8 and less than 20 letters.", new[] { nameof(Password) });
                }
            }
        }

        private bool IsValidPassword(string password)
        {

            if (password.Length < 8 || password.Length > 20)
            {
                return false;
            }
            var hasUpperCase = new Regex(@"[A-Z]+").IsMatch(password);
            var hasLowerCase = new Regex(@"[a-z]+").IsMatch(password);
            var hasDigit = new Regex(@"[0-9]+").IsMatch(password);
            var hasSpecialChar = new Regex(@"[!@#$%^&*()_+}{:;',.?\|]").IsMatch(password);

            if (!(hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar))
            {
                return false;
            }
            var commonPatterns = new string[] { "password", "12345678", "abcd1234" };
            if (commonPatterns.Contains(password.ToLower()))
            {
                return false;
            }

            return true;
        }
    }
}
