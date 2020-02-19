using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizationCodeClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace AuthorizationCodeClient.Controllers
{

    [Authorize()]
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            var access_token = await HttpContext.GetTokenAsync("access_token");
            var id_token = await HttpContext.GetTokenAsync("id_token");
            var refresh = await HttpContext.GetTokenAsync("refresh_token");
            return View(new TokenModel { AccessToken = access_token, IdToken = id_token });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
