using Domain.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthServices
{
    public class Validation
    {
        private readonly UserRepo _userRepo;

        public Validation(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public bool IsUsernameUnique(string username)
        {
            TblUser user = _userRepo.GetUser(username);
            if (user == null)
                return true;
            return false;
        }

        public bool IsEmailUnique(string email)
        {
            bool isPresent = _userRepo.IsEmailValid(email);
            return !isPresent;
        }

    }
}
