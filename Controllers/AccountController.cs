using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ShopX.Data;
using ShopX.Models;
using System.Security.Claims;

namespace ShopX.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) { _db = db; }

        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string email)
        {
            var user = new User { Username = username, Password = password, Email = email };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        // Optional: Trigger Entra ID sign-in
        public IActionResult ExternalLogin(string provider = "AzureAD")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            // After Entra ID login, you can read claims and create local user record if needed.
            var userName = User.Identity?.Name ?? "aduser";
            // ...create user in DB if needed...
            return RedirectToAction("Index", "Home");
        }
    }
}
