using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDDDNetCore.Models;

namespace WebDDDNetCore.Controllers
{
    public class HomeController : Controller
    {
        //UserManager<IdentityUser> _userManager;
        //AuthorizationHandlerContext context;
        //SignInManager<IdentityUser> _signInManager;

        //public HomeController(UserManager<IdentityUser>
        //    userManager, SignInManager<IdentityUser> signInManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //}
        public IActionResult Index()
        {
            return View();
         //  return RedirectToAction("Index", "Personas");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
