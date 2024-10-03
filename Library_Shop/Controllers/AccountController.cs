using Library_Shop.Data;
using Library_Shop.Models.ViewModel.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Library_User> userManager;
        private readonly SignInManager<Library_User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<Library_User> userManager, SignInManager<Library_User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await userManager.Users.ToListAsync());
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                Library_User user = new Library_User()
                {
                    UserName = rvm.Login,
                    Email = rvm.Email,
                    DateOfBirth = rvm.DateOfBirth,
                };
                IdentityResult result = await userManager.CreateAsync(user, rvm.Password);
                if (result.Succeeded)
                {
                    // Переконайтеся, що роль існує
                    if (!await roleManager.RoleExistsAsync(rvm.Role))
                    {
                        IdentityResult roleResult = await roleManager.CreateAsync(new IdentityRole(rvm.Role));
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(rvm);
                        }
                    }

                    // Призначте роль користувачу
                    await userManager.AddToRoleAsync(user, rvm.Role);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(rvm);
        }

        public IActionResult Login(string? returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel() { ReturnUrl = returnUrl };
            return View(loginViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(lvm.Email, lvm.Password, lvm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(lvm.Email);
                    if (user != null)
                    {
                        var userRoles = await userManager.GetRolesAsync(user);
  
                        if (userRoles.Contains("Admin"))
                        {
                            return RedirectToAction("AdminDashboard", "Home");
                        }
                        else if (userRoles.Contains("Manager"))
                        {
                            return RedirectToAction("ManagerDashboard", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(lvm);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl)
        {
            await signInManager.SignOutAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
