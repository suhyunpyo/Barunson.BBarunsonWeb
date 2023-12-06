using Barunson.DbContext;
using Barunson.DbContext.DbModels.DearDeer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class AccountController : Controller
    {
        private BarShopContext BarShopContext { get; set; }

        public AccountController(BarShopContext barShopContext)
        {
            this.BarShopContext = barShopContext;
        }

        public async Task<IActionResult> SignIn(string userid, string pwd)
        {
            var query = from m in BarShopContext.ADMIN_LST //어드민TB
                        where m.ADMIN_ID == userid && m.ADMIN_PASSWD == pwd
                        && m.NState == "1" && m.ADMIN_LEVEL <= 3
                        select new
                        {
                            m.ADMIN_ID,
                            m.ADMIN_NAME,
                            m.CMS_ID,
                            m.CMS_NUM,
                            m.isDesigner,
                            m.isCS,
                            m.isAlba,
                            m.isDeveloper
                        };

            var item = await query.FirstOrDefaultAsync();
            if (item != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, item.ADMIN_ID),
                    new Claim(ClaimTypes.NameIdentifier, item.ADMIN_ID),
                    new Claim(ClaimTypes.Name, item.ADMIN_NAME),
                    new Claim("CMS_ID", item.CMS_ID),
                    new Claim("CMS_NUM", item.CMS_NUM),
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
