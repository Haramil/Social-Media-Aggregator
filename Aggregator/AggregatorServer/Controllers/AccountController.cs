using AggregatorServer.Cash;
using AggregatorServer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AggregatorServerv.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Users(string id)
        {
            DBWorker db = new DBWorker();
            return View(db.GetAllUsers());
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            await UserManager.DeleteAsync(user);
            return RedirectToAction("Users", "Account");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Login,
                    Login = model.Login,
                };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // если создание прошло успешно, то добавляем роль пользователя
                    await UserManager.AddToRoleAsync(user.Id, "user");

                    return RedirectToAction("Users", "Account");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,

                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Home");
                    return RedirectToAction("AddHashTag", "Account");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Tags()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public string AddHashTag(string query, int countofpages)
        {
            try
            {
                DBWorker dbworker = new DBWorker();
                Pagination p = dbworker.GetPaginations(query);
                if (p != null) { return "tagexists"; }
                return JsonConvert.SerializeObject(Cashing.Cash(query, countofpages));
            }
            catch
            {
                return "badbd";
            }
        }

        [Authorize]
        [HttpPost]
        public string DeleteHashTag(string query)
        {
            try
            {
                return JsonConvert.SerializeObject(Cashing.DeleteHashTag(query));
            }
            catch
            {
                return "badbd";
            }
        }

        [Authorize]
        [HttpPost]
        public string GetAllTags()
        {
            DBWorker dbworker = new DBWorker();
            return JsonConvert.SerializeObject(dbworker.GetAllPagination());
        }
    }
}
