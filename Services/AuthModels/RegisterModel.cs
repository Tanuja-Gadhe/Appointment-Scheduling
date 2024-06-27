using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.AuthModels
{
    public class RegisterModel:IValidatableObject
    {
       

        
        public string UserName { get; set; }
        public string Email { set; get; }
        public string Password { set; get; }
        public string Role { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public IFormFile? ImageFile { get; set; } = null;
        public RegisterModel()
        {
           
        }
        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           //Username
            if (string.IsNullOrEmpty(UserName)||string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult("Username is required.", new[] { nameof(UserName) });
            }
            else
            {
                if(!IsValidUsername(UserName))
                {
                    yield return new ValidationResult("Username is should contain alphabets, numbers and underscore only.Length should between" +
                        "3 to 16 characters", new[] { nameof(UserName) });
                }
            }

            //Email
            if(string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
            }
            else
            {
                if (!IsValidEmail(Email))
                {
                    yield return new ValidationResult("Invalid Email Address", new[] { nameof(Email) });
                }

            }

            // Password
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Password) });
            }
            else
            {
                if(!IsValidPassword(Password))
                {
                    yield return new ValidationResult("Password should have atleast a capital , small and digit and should " +
                        "be greater than 8 and less than 20 letters.", new[] { nameof(Password) });
                }
            }


            //Role
            if (string.IsNullOrEmpty(Role) || string.IsNullOrWhiteSpace(Role))
            {
                yield return new ValidationResult("Role is required.", new[] { nameof(Role) });
            }
            //Firsname
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("First Name is required.", new[] { nameof(FirstName) });
            }
            else
            {
                if(!IsValidName(FirstName))
                {
                    yield return new ValidationResult("First Name is inavlid.", new[] { nameof(FirstName) });
                }
            }
            //LastName
            if (string.IsNullOrEmpty(LastName) || string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Last Name  is required.", new[] { nameof(LastName) });
            }
            else
            {
                if (!IsValidName(LastName))
                {
                    yield return new ValidationResult("Last Name is inavlid.", new[] { nameof(FirstName) });
                }
            }
            //PhoneNumber
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Phone Number is required.", new[] { nameof(PhoneNumber) });
            }
            else
            {
                if(!IsValidPhoneNumber(PhoneNumber))
                {
                    yield return new ValidationResult("Phone Number is Invalid", new[] { nameof(PhoneNumber) });
                }
            }

            //Address
            if (string.IsNullOrEmpty(Address) || string.IsNullOrWhiteSpace(Address))
            {
                yield return new ValidationResult("Address is required.", new[] { nameof(Address) });
            }
            else
            {
                if(!IsValidAddress(Address))
                {
                    yield return new ValidationResult("Address is Invalid.", new[] { nameof(Address) });
                }
            }
            
        }
        
       
        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
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
            var commonPatterns = new string[] { "password", "12345678",  "abcd1234" };
            if (commonPatterns.Contains(password.ToLower()))
            {
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            
            var isValid = new Regex(@"^\d{10}$").IsMatch(phoneNumber);
            if (phoneNumber == "0000000000")
                isValid = false;
            return isValid;
        }

        private bool IsValidUsername(string userName)
        {
            var isValid = new Regex(@"^[a-zA-Z0-9_]{3,16}$").IsMatch(userName);
            return isValid;
        }

        private bool IsValidName(string name)
        {
            var isValid= new Regex(@"^[A-Za-z]+(?:[\s-][A-Za-z]+)*$").IsMatch(name);
            return isValid;
        }

        private bool IsValidAddress(string address)
        {
            var isValid = new Regex(@"^[A-Za-z0-9\s,'-.]*$").IsMatch(address);
            return isValid;
        }
    }
}
