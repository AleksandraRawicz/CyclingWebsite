using CyclingWebsite.Models;
using CyclingWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Controllers
{
    //[ApiController]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;
        public AccountController(IAccountService accountService, IEmailSender emailSender)
        {
            _accountService = accountService;
            _emailSender = emailSender;
        }       

        [HttpGet]  
        [Route("Index")]
        public IActionResult Index()
        {
            var tours = RedirectToAction("GetAllForUser", "Tour");
            return View("MyAccount");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult LogIn([FromForm] UserLoginDto dto)
        {
            bool logged = _accountService.LoginAsync(dto).Result;
            if(logged)
                return RedirectToAction("Index", "Home");
            ViewBag.Info = "wrong email or password";
            return View("~/Views/Home/Index.cshtml");

        }

        [HttpGet]
        [Route("LoginAndRegister")]
        public IActionResult LoginAndRegister()
        {
            return View("LoginAndRegister");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto dto)
        {
            if (ModelState.IsValid)
            {
                string token =_accountService.Register(dto);
                string confirmationLink = Url.Action("ConfirmEmail", "Account", new
                {
                    token = token
                },
               protocol: HttpContext.Request.Scheme) ;
                await _emailSender.SendEmailAsync(dto.Email, "Email Confirmation", confirmationLink);
                ViewBag.Info = "Confirmation link was sended. Check your email";
                return View("~/Views/Home/Index.cshtml");
            }
            ViewBag.Info = "Something went wrong";
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpGet]
        [Route("LogOut")]
        public IActionResult LogOut()
        {
            _accountService.LogOutAsync();
            ViewBag.info = "logged out";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        [Route("EditView")]
        public IActionResult Edit()
        {
            return PartialView("_EditUser", new UserEditDto());
        }

        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromForm] UserEditDto dto)
        {            
            if (ModelState.IsValid)
            {
                bool result = _accountService.Edit(dto);
                if (result)
                    ViewBag.Info = "Your password has been changed";
                else
                    ViewBag.Info = "Bad password";
            }
            return View("MyAccount");
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public IActionResult ConfirmEmail([FromQuery] string token)
        {
            bool ok = _accountService.ConfirmEmail(token);
            if(ok)
                return View("LoginAndRegister");
            ViewBag.Info = "Something went wrong with your confirmation link";
            return View("~/Views/Home/Index.cshtml");
        }
        
    }
}

