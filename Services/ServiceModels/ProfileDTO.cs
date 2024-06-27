using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public  class ProfileDTO:IValidatableObject
    {
        public string UserName { get; set; }
        public string Email { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? ImagePath { get; set; } 

        public IFormFile? ImageFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Username
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult("Username is required.", new[] { nameof(UserName) });
            }
            else
            {
                if (!IsValidUsername(UserName))
                {
                    yield return new ValidationResult("Username is should contain alphabets, numbers and underscore only.Length should between" +
                        "3 to 16 characters", new[] { nameof(UserName) });
                }
            }

            //Email
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
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

           
            //Firsname
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("First Name is required.", new[] { nameof(FirstName) });
            }
            else
            {
                if (!IsValidName(FirstName))
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
                if (!IsValidPhoneNumber(PhoneNumber))
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
                if (!IsValidAddress(Address))
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
            var isValid = new Regex(@"^[A-Za-z]+(?:[\s-][A-Za-z]+)*$").IsMatch(name);
            return isValid;
        }

        private bool IsValidAddress(string address)
        {
            var isValid = new Regex(@"^[A-Za-z0-9\s,'-.]*$").IsMatch(address);
            return isValid;
        }
    }
}
