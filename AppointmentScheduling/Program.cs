using Common;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repo;
using Services.AuthModels;
using Services.AuthServices;
using System.Net;
using System.Text;
using common;
using AppointmentScheduling.Hubs;

namespace AppointmentScheduling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppointmentSchedulingContext>(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));
            builder.Services.AddTransient<UserRepo>();
            builder.Services.AddTransient<RoleRepo>();
            builder.Services.AddTransient<Encryption>();
            builder.Services.AddTransient<RegisterModel>();
            builder.Services.AddTransient<Authentication>();
            builder.Services.AddTransient<JwtToken>();
            builder.Services.AddTransient<DoctorServices>();
            builder.Services.AddTransient<Validation>();
            builder.Services.AddTransient<Uploads>();
            builder.Services.AddTransient<Email>();
            builder.Services.AddTransient<SpecializationRepo>();
            builder.Services.AddTransient<AvailabilityRepo>();
            builder.Services.AddTransient<AppointmentRepo>();
            builder.Services.AddTransient<WaitListRepo>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<PatientServices>();
            builder.Services.AddTransient<ChatRepo>();
            builder.Services.AddTransient<ChatServices>(); 
            builder.Services.AddTransient<DoctorVerificationRepo>();
            builder.Services.AddTransient<DocumentRepo>();
            builder.Services.AddTransient<AdminServices>();
            builder.Services.AddSignalR();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            builder.Services.AddMvc(options =>
            {  
                options.Filters.Add(new NoCacheAttribute());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    var loginUrl = "/Auth/Login?unauthorized=true&returnUrl=" + returnUrl;
                    context.Response.Redirect(loginUrl);
                }
            });
            app.Use(async (context, next) =>
            {
                var JWTokenCookie = context.Request.Cookies["JWTToken"];
                if (!string.IsNullOrEmpty(JWTokenCookie))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWTokenCookie);
                }
                await next();
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");
            app.MapHub<ChatHub>("/chathub");
            app.Run();
        }
    }
}
