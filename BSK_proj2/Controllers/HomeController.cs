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

        [HttpGet]
        public IActionResult Upload()
        {
            //var users = dBContext.Users.ToList();
            
            ViewData["Message"] = userManager.GetUserId(HttpContext.User) /*+ users[0].Id*/;

            return View();
        }

        [HttpPost]
        public IActionResult Upload(UploadedImage image)
        {
            //process image
            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Yay";
                return View();
            }
            else
            {
                ViewData["Message"] = "Nay";
                return View();
            }
        }

        public  IActionResult Browse()
        {
            /*Photo photo = new Photo();
            photo.Link = "google.com";
            photo.User = await userManager.GetUserAsync(HttpContext.User);
            dBContext.Photos.Add(photo);
            dBContext.SaveChanges();*/
            //ViewData["Message"] = "Your contact page.";

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
