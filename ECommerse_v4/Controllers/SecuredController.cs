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
    [Authorize]
    public class SecuredController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        private IConfiguration Configuration { get; }
        public SecuredController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager, IConfiguration configuration)
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

        [Authorize(Roles = "Admin,Company")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            List<AppUser> users = _userManager.GetUsersInRoleAsync("Admin").Result.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateSuperUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new AppRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                    ViewBag.Message = "Role Created";
                else
                    foreach (IdentityError err in result.Errors)
                        ModelState.AddModelError("", err.Description);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateSuperUser(User user,string Role,string CompanyName)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = user.Name;
                appUser.Email = user.Email;
                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                await _userManager.AddToRoleAsync(appUser, Role);
                CompanyMiddleware.Add(CompanyName,appUser, Configuration);

                if (result.Succeeded)
                    ViewBag.Message = $"{Role} Created";
                else
                    foreach (IdentityError err in result.Errors)
                        ModelState.AddModelError("", err.Description);
            }
            return View(user);
        }

        

    }
}
