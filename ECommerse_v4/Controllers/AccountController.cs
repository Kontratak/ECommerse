using ECommerse_v4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<AppRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<AppRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            if (_roleManager.Roles.ToList().Where(item => item.Name == "User").ToList().Count == 0)
            {
                Task.Run(() =>
                {
                    _roleManager.CreateAsync(new AppRole() { Name = "User" });
                }).Wait();
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                await _userManager.AddToRoleAsync(appUser, "User");


                if (result.Succeeded)
                    ViewBag.Message = "User Created";
                else
                    foreach (IdentityError err in result.Errors)
                        ModelState.AddModelError("", err.Description);
            }
            return View(user);
        }

        [HttpPost] 
        public async Task<IActionResult> Redirect()
        {
            AppUser appUser = await _userManager.GetUserAsync(User);
            if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "Admin").First().Id))
            {
                return RedirectToAction("Index", "Secured");

            }
            else if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "Company").First().Id))
            {
                return RedirectToAction("Index", "Company");
            }
            else if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "CompanyEmployee").First().Id))
            {
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Required][EmailAddress] string email,[Required] string password,string returnurl)
        {

            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByEmailAsync(email);
                if(appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, password, false, false);
                    if (result.Succeeded)
                    {
                        if (_roleManager.Roles.ToList().Where(item => item.Name == "Admin").ToList().Count != 0)
                        {
                            if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "Admin").First().Id))
                            {
                                return Redirect(returnurl ?? "/");
                            }
                            else if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "Company").First().Id))
                            {
                                return RedirectToAction("Index", "Company");
                            }

                            else if (appUser.Roles.Contains(_roleManager.Roles.ToList().Where(item => item.Name == "CompanyEmployee").First().Id))
                            {
                                return RedirectToAction("Index", "Company");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError(nameof(email), "Login Failed : Invalid Email or Password");
            }
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
    }
}
