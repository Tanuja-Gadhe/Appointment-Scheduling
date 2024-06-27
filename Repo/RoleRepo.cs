using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class RoleRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public RoleRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }
        public async Task Create(TblRole role)
        {
            await _context.TblRoles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public Guid GetRoleId(string roleName)
        {
            TblRole role = _context.TblRoles.FirstOrDefault(role => role.RoleName == roleName);
            return role.RoleId;
        }

        public string GetRoleName(Guid roleId)
        {
            TblRole role = _context.TblRoles.FirstOrDefault(role => role.RoleId == roleId);
            return role.RoleName;
        }
    }
}
