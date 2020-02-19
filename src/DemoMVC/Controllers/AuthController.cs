using DemoMVC.Models;
using System.Security.Claims;
using System.Web.Mvc;

namespace DemoMVC.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {
            var user = User.Identity as ClaimsIdentity;
            var access_token = user.FindFirst("access_token").Value;
            var id_token = user.FindFirst("id_token").Value;
            var refresh = user.FindFirst("refresh_token").Value;

            return View(new TokenModel { AccessToken = access_token, IdToken = id_token });
        }
    }
}