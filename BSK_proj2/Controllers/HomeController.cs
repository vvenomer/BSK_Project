using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity;
using BSK_proj2.Data;
using System.Linq;
using System.IO;
using System;

namespace BSK_proj2.Controllers
{
    public struct ImagePlusUser
    {
        public Image Image;
        public string user;
    }
    public class HomeController : Controller
    {
        UserManager<ApplicationUser> userManager;
        ApplicationDbContext dBContext;
        Random random;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext dBContext)
        {
            this.userManager = userManager;
            this.dBContext = dBContext;
            random = new Random();
        }
        public IActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            //if not logged in -> redirect
            //doTo: add message, that would explain redirection
            if (userManager.GetUserId(HttpContext.User) == null)
                return RedirectToAction("Account/Login", "Identity");

            var model = new UploadedImage() { access = "public" };
            return View(model);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Upload(UploadedImage model)
        {
            string error = null;
            //process image
            if (ModelState.IsValid)
            {

                Image image = new Image();

                image.LinkType = model.image_choice;
                image.Name = model.name;
                if (model.image_choice == "upload")
                {
                    //seperate folder for specific users
                    if (model.uploaded_img == null)
                        error = "No file sent";
                    else
                    {
                        var fileExt = Path.GetExtension(model.uploaded_img.FileName);
                        var fileName = Path.GetFileNameWithoutExtension(model.uploaded_img.FileName);
                        var filePath = "/images/uploaded/" + fileName + "-" + random.Next().ToString("X4") + fileExt;
                        image.Link = filePath;
                        if (model.name == null)
                            image.Name = fileName;
                        filePath = "wwwroot/" + filePath;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.uploaded_img.CopyToAsync(stream);
                        }

                    }
                }
                else if (model.image_choice == "link")
                {
                    //check if it is actually image - will do in js
                    if (model.linked_img == null) //can that even be null
                        error = "No image linked";
                    else
                    {
                        if (model.name == null)
                            image.Name = Path.GetFileNameWithoutExtension(model.linked_img);
                        
                        image.Link = model.linked_img;
                    }
                }
                else
                {
                    error = "Wierd option selected.";
                }
                if (error == null)
                {
                    image.Description = model.desctiption;
                    image.Access = model.access;
                    image.Comment = model.comment;
                    image.Like = model.like;
                    image.User = await userManager.GetUserAsync(HttpContext.User);
                    dBContext.Images.Add(image);
                    await dBContext.SaveChangesAsync();
                    ViewData["Message"] = "Image saved succesfully";
                }
            }
            else
            {
                error = "Wrong form model";
            }
            if(error != null)
                ViewData["Message"] = "<span>Something went wrong :( Error message is: " + error + "</span>";
            return View( /*new object[]{*/ model/*, error==null}*/);
        }

        public  IActionResult Browse()
        {
            var model = dBContext.Images.Where(x => x.Access == "public").ToList();

            return View(model);
        }

        public IActionResult Collection()
        {
            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter();
            var model = dBContext.Images.Where(x => x.User == user.GetResult()).ToList();

            return View("Browse", model);
        }

        [Route("Home/Image/{id:int}-{name}")]
        public IActionResult ViewImage(int id, string name)
        {
            var model = dBContext.Images.First( x => (x.ID == id) && (x.Name == name) );
            dBContext.Entry(model).Reference(x => x.User).Load();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
