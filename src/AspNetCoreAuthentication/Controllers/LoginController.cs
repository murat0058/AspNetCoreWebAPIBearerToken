using AspNetCoreAuthentication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    LoginModel loginModel = new LoginModel();
                    loginModel.username = model.Username;
                    loginModel.password = model.Password;

                    client.BaseAddress = new Uri("http://localhost:64295");

                    HttpResponseMessage response = new HttpResponseMessage();
                    response = client.PostAsync("/api/account", loginModel, new JsonMediaTypeFormatter()).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var token = response.Content.ReadAsAsync<UserToken>().Result;
                        LoginContext.UserToken = token;
                        return RedirectToAction("Index", "Home");
                    }
                }

            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
