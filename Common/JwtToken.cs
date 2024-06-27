using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class JwtToken
    {
        private readonly IConfiguration _config;

        private readonly  RoleRepo _roleRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtToken(IConfiguration config,RoleRepo roleRepo, IHttpContextAccessor httpContextAccessor) {
            _config = config;
            _roleRepo = roleRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GenerateToken(TblUser user,out string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            string Role = _roleRepo.GetRoleName(user.RoleId);
            role = Role;
    
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Role,Role),
                new Claim("username",user.UserName),
                
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken GetToken(string token)
        {
            string jwtToken = _httpContextAccessor.HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            return parsedToken;
        }

    }
    public static class TokenHelper
    {
        public static string GetUserRole(IHttpContextAccessor httpContextAccessor)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var roleClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

            return roleClaim;
        }
    }
}
