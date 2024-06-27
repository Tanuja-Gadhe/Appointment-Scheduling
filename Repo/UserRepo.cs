
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repo
{
    public class UserRepo
    { 
        private readonly AppointmentSchedulingContext _context;
        private readonly RoleRepo _roleRepo;

        public UserRepo(AppointmentSchedulingContext context,RoleRepo roleRepo) {
           _context = context;
            _roleRepo = roleRepo;
        }
        public async Task CreateUser(TblUser user)
        {
            await _context.TblUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public TblUser GetUser(string usernameOrEmail)
        {
            TblUser user =_context.TblUsers.FirstOrDefault(user => user.UserName == usernameOrEmail || user.Email == usernameOrEmail );
            return user;
        }
        public bool IsEmailValid(string email)
        {
            TblUser user = _context.TblUsers.FirstOrDefault(user => user.Email == email);
            if (user == null)
                return false;
            return true;

        }

        public List<TblUser> GetByRole(string role)
        {

            Guid roleId = _roleRepo.GetRoleId(role);
            Console.WriteLine(roleId);
            var result = _context.TblUsers.Where(user => user.RoleId == roleId).ToList();                                                                            
            return result;

        }

        public TblUser GetUserById(Guid userId)
        {
            return _context.TblUsers.FirstOrDefault(user => user.UserId == userId);
        }

        public void UpdateUser()
        {
            _context.SaveChanges();
        }
    }
}
  