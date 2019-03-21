using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity;
using BSK_proj2.Data;
using System.Linq;

namespace BSK_proj2.Controllers
{
    public class HomeController : Controller
    {
        UserManager<ApplicationUser> userManager;
        ApplicationDbContext dBContext;
        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext dBContext)
        {
            this.userManager = userManager;
            this.dBContext = dBContext;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Upload()
        {
            //var users = dBContext.Users.ToList();
            
            ViewData["Message"] = userManager.GetUserId(HttpContext.User) /*+ users[0].Id*/;

            return View();
        }

        public async System.Threading.Tasks.Task<IActionResult> Browse()
        {
            Photo photo = new Photo();
            photo.Link = "google.com";
            photo.User = await userManager.GetUserAsync(HttpContext.User);
            dBContext.Photos.Add(photo);
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Collection()
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
