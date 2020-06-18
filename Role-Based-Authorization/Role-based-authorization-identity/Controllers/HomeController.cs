using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<IdentityUser> UserManager, SignInManager<IdentityUser> SignInManager,RoleManager<IdentityRole> RoleManager)
        {
            _userManager = UserManager;
            _signInManager = SignInManager;
            _roleManager = RoleManager;
        }
        public IActionResult Index() {
            return View();
        }
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Loginin(String name, String password)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user != null)
            {
                //if the role doesn't exist,create it
                if (!await _roleManager.RoleExistsAsync("admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                }
                //add the role to the current user
                await _userManager.AddToRoleAsync(user,"admin");


                //sign in
                var signResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signResult.Succeeded)
                {
                    return RedirectToAction("Userinfo");
                }
            }
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Loginout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registerup(String name,String email, String password)
        {
            var user = new IdentityUser
            {
                UserName = name,
                Email = email
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var res = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (res.Succeeded)
                    return RedirectToAction("Userinfo");
            }
            return RedirectToAction("Login");
        }
        //[Authorize(AuthenticationSchemes= "Identity.Application")]
        [Authorize]
        public async Task<IActionResult> UserinfoAsync() {
            var user = await _userManager.GetUserAsync(User);
            IList<String> roles = await _userManager.GetRolesAsync(user);

            ViewData["roles"] = roles;
            return View();
        }
    }
}
