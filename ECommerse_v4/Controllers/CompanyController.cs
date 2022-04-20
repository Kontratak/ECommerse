using ECommerse_v4.Middleware;
using ECommerse_v4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Controllers
{
    public class CompanyController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        private IConfiguration Configuration { get; }
        public CompanyController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            Configuration = configuration;
        }

        [Authorize(Roles = "Admin,Company")]
        public IActionResult CreateEmployee()
        {
            return View();
        }


        [Authorize(Roles = "Company")]
        public async Task<IActionResult> ListUsers()
        {
            AppUser appUser = await _userManager.GetUserAsync(User);
            List<AppUser> users = CompanyMiddleware.GetEmployees(appUser.Id,Configuration);
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = user.Name;
                appUser.Email = user.Email;
                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                AppUser companyUser = await _userManager.GetUserAsync(User);
                await _userManager.AddToRoleAsync(appUser, "CompanyEmployee");
                CompanyMiddleware.AddEmployee(companyUser, appUser, Configuration);

                if (result.Succeeded)
                    ViewBag.Message = "Company Employee Created";
                else
                    foreach (IdentityError err in result.Errors)
                        ModelState.AddModelError("", err.Description);
            }
            return View(user);
        }


        public IActionResult Index()
        {
            return View();
        }



    }
}
