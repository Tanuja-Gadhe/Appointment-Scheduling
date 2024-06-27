using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class SearchModel
    {

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;   

        public string UserImage { get; set; } = null!;

        public string Specialization { get;set; } = null!;

        public string Degree { get; set; } = null!; 

        public SearchModel(string firstname , string lastname , string email, string phoneNumber, string address, string userImage, string specialiazation,string degree)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            UserImage = userImage;
            Specialization = specialiazation;
            Degree = degree;
        }

    }
}
