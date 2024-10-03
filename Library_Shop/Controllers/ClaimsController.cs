using Library_Shop.Data;
using Library_Shop.Models.DTOs.Claim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_Shop.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<Library_User> userManager;
        public ClaimsController(UserManager<Library_User> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index() => View(User.Claims);
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateClaimDTO claimDTO)
        {
            Claim claim = new Claim(claimDTO.ClaimType, claimDTO.ClaimValue, ClaimValueTypes.String);
            Library_User? library_User = await userManager.GetUserAsync(User);
            if (library_User != null)
            {
                var indentityResult = await userManager.AddClaimAsync(library_User, claim);
                if (indentityResult.Succeeded) { return RedirectToAction("Index"); }
                Errors(indentityResult);
            }
            return View(claimDTO);
        }
        private void Errors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        public async Task<IActionResult> Delete(string claimInfo)
        {
            string[] claimsData = claimInfo.Split(';');
            string claimType = claimsData[0];
            string claimValueType = claimsData[1];
            string claimValue = claimsData[2];
            Claim? deleteClaim = User.Claims.Where(c => c.Value == claimValue && c.Type == claimType && c.ValueType == claimValueType).FirstOrDefault();
            Library_User? library_User = await userManager.GetUserAsync(User);
            if (library_User != null && deleteClaim is not null)
            {
                var result = await userManager.RemoveClaimAsync(library_User, deleteClaim);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                Errors(result);
            }
            return RedirectToAction("Index");
        }

    }
}
