﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Db;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public AppDbContext dbContext = null;
        public HomeController(AppDbContext _dbContext) {
            this.dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Loginup(String name,String password) {
            User _user = new User(name,password);
            //query this user from database
            User DbRes = dbContext.users.Where(u =>
                             _user.name.Equals(u.name)
                             && _user.passwordHash.Equals(u.passwordHash)
                         ).FirstOrDefault();
            //check is if this user exists
            if (DbRes != null)
            {
                //login in, if user exists
                var userClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, DbRes.name),
                    new Claim("passwordhash",DbRes.passwordHash),
                    new Claim(ClaimTypes.Role,"Admin"),
                    new Claim("Gmail",DbRes.Gmail),
                    new Claim("Outlook",DbRes.Outlook),
                    new Claim("QQ",DbRes.QQ)
                };
                var userIdentity = new ClaimsIdentity(userClaim, CookieAuthenticationDefaults.AuthenticationScheme);
                var userPrinciple = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(userPrinciple);
                return RedirectToAction("Userprofile");
            }
            //redirects to login page, if user doesn't exist
            return RedirectToAction("login");
        }

        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registerup(String name,String password,String Gmail,String Outlook,String QQ) {
            //query is if exists the same user in database
            User _user = new User(name, password,Gmail,Outlook,QQ);
            User DbRes = dbContext.users.Where(u => u.name.Equals(_user.name)).FirstOrDefault();
            if (DbRes == null) {
                //it doesn't exist same user,so save this user into database
                dbContext.users.Add(_user);
                int RCount = await dbContext.SaveChangesAsync();
                //check is if insert success
                if (RCount > 0)
                {
                    //log in in HttpContext
                    var userClaim = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, _user.name),
                        new Claim("passwordhash",_user.passwordHash),
                        new Claim(ClaimTypes.Role,"Admin"),
                        new Claim("Gmail",_user.Gmail),
                        new Claim("Outlook",_user.Outlook),
                        new Claim("QQ",_user.QQ)
                    };
                    var userIdentity = new ClaimsIdentity(userClaim, CookieAuthenticationDefaults.AuthenticationScheme);
                    var userPrinciple = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(userPrinciple);
                    return RedirectToAction("Userprofile");
                }
                //insert data failed
                return RedirectToAction("register");
            }
            //this name has already registered by others
            return RedirectToAction("register");
        }

        public async Task<IActionResult> Loginout() {
            await HttpContext.SignOutAsync();
            return RedirectToAction("login");
        }

        [Authorize(Policy = "emailsPolicy",Roles = "Admin")]
        public IActionResult Userprofile() {
            ClaimsPrincipal _claimsPrincipal = HttpContext.User;
            List<String> roles = _claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(s => s.Value).ToList();
            ClaimsIdentity claimIdentity = _claimsPrincipal.Identities.Where(ct => ct.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme).FirstOrDefault();
            ViewData["name"] = claimIdentity.Claims.Where(cm => cm.Type == ClaimTypes.Name).FirstOrDefault().Value;
            ViewData["passwordhash"] = claimIdentity.Claims.Where(cm => cm.Type == "passwordhash").FirstOrDefault().Value;
            ViewData["Outlook"] = claimIdentity.Claims.Where(cm => cm.Type == "Outlook").FirstOrDefault().Value;
            ViewData["Gmail"] = claimIdentity.Claims.Where(cm => cm.Type == "Gmail").FirstOrDefault().Value;
            ViewData["QQ"] = claimIdentity.Claims.Where(cm => cm.Type == "QQ").FirstOrDefault().Value;
            ViewData["roles"] = roles;
            return View();
        }
    }
}
