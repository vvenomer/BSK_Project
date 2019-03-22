using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity;
using BSK_proj2.Data;
using System.Linq;
using System.IO;
using System;
using System.Net;
using System.Globalization;
using System.Collections.Generic;

namespace BSK_proj2.Controllers
{
    public class HomeController : Controller
    {
        UserManager<ApplicationUser> userManager;
        ApplicationDbContext dBContext;
        Random random;

        private bool IsImageUrl(string URL)
        {
            var req = HttpWebRequest.Create(URL);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("image/");
            }
        }

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
                if (model.image_choice == "upload")
                {
                    //seperate folder for specific users
                    if (model.uploaded_img == null)
                        error = "No file sent";
                    else
                    {
                        var fileExt = Path.GetExtension(model.uploaded_img.FileName);
                        var fileName = Path.GetFileNameWithoutExtension(model.uploaded_img.FileName);
                        var filePath = "images/uploaded/" + fileName + "-" + random.Next().ToString("X4") + fileExt;
                        image.Link = filePath;
                        if (model.name == "")
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
                    if (model.linked_img == null)
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
            return View( new object[]{ model, error==null});
        }

        public  IActionResult Browse()
        {
            var model = dBContext.Images.Where(x => x.Access == "public").ToList();


            return View(model);
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
