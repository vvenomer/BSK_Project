using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity;
using BSK_proj2.Data;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            if (userManager.GetUserId(HttpContext.User) == null)
                return Redirect("/Identity/Account/Login");

            var model = new UploadedImage() { access = "public" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadedImage model)
        {
            string error = null;
            if (ModelState.IsValid)
            {
                Image image = new Image();

                image.LinkType = model.image_choice;
                image.Name = model.name;
                if (model.image_choice == "upload")
                {
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
                    var ownerPermission = new Permission<Image>(true);
                    ownerPermission.User = await userManager.GetUserAsync(HttpContext.User);
                    ownerPermission.Object = image;

                    dBContext.Images.Add(image);
                    dBContext.ImagePermissions.Add(ownerPermission);
                    await dBContext.SaveChangesAsync();
                    ViewData["Message"] = "Image saved succesfully";
                }
            }
            else
            {
                error = "Wrong form model";
            }
            if (error != null)
                ViewData["Message"] = "<span>Something went wrong :( Error message is: " + error + "</span>";
            return View(model);
        }

        public IActionResult Browse()
        {
            var model = dBContext.Images.Where(x => x.Access == "public").ToList();

            return View(model);
        }

        public IActionResult Collection()
        {
            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            if (user == null)
                return Redirect("/Identity/Account/Login");

            var model = dBContext.ImagePermissions
                .Where(x => x.User == user && x.read)
                .Join(dBContext.Images, p => p.Object, i => i, (p, i) => i).ToList();

            return View("Browse", model);
        }
        [HttpGet]
        [Route("Home/Image/{id:int}-{name}")]
        public IActionResult ViewImage(int id, string name)
        {
            var model = dBContext.Images.First(x => (x.ID == id) && (x.Name == name));
            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            if (user != null)
            {
                var permission = dBContext.ImagePermissions.FirstOrDefault(x => x.User == user && x.Object == model);
                bool read_or_write_perm = GetImagePerm(() => permission, x => x.read || x.write);
                if (read_or_write_perm)
                {
                    ViewBag.Perm = permission;
                    if (permission.give)
                    {
                        var usersPermissions = dBContext.ImagePermissions.Where(x => x.Object.ID == id && !x.owner).Include(x => x.User).ToList();
                        ViewBag.Permissions = usersPermissions;
                        if (!permission.owner)
                            ViewBag.UsersThatCanTake = usersPermissions.Where(x => x.take).Select(x => x.User).ToList();
                        else
                            ViewBag.UsersThatCanTake = dBContext.Users.Where(x => x != user).ToList();
                    }
                }
                else if (model.Access == "public")
                {
                    ViewBag.Perm = new Permission<Image>(true, false, false, false, false, false);
                }
                var comments = dBContext.Comments.Where(x => x.Image == model).Include(x => x.Owner)
                    .GroupJoin(dBContext.CommentPermissions, c => c, p => p.Object, (c, p) => new CommentWithPerm
                    {
                        c = c,
                        p = GetCommentPerm(() => p.FirstOrDefault(x => x.User == user), x => x.write || x.delete || x.give || x.owner),
                    }).ToList();
                ViewBag.Comments = comments;
                ViewData["LoggedIn"] = true;
            }
            else if (model.Access == "public")
            {
                ViewBag.Perm = new Permission<Image>(true, false, false, false, false, false);
                var comments = dBContext.Comments.Where(x => x.Image == model).Include(x => x.Owner)
                    .GroupJoin(dBContext.CommentPermissions, c => c, p => p.Object, (c, p) => new CommentWithPerm
                    {
                        c = c,
                        p = false,
                    }).ToList();
                ViewData["LoggedIn"] = false;
                ViewBag.Comments = comments;
            }
            else
                return RedirectToAction(nameof(Error));

            ViewData["Owner"] = dBContext.ImagePermissions.Include(x => x.User).First(x => x.Object==model && x.owner).User.UserName;



            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/Image/{id:int}-{name}")]
        public async Task<IActionResult> ViewImage(int id, string name, Image image)
        {
            var model = dBContext.Images.First(x => (x.ID == id) && (x.Name == name));
            var user = await userManager.GetUserAsync(HttpContext.User);
            bool write_perm = GetImagePerm(
                () => dBContext.ImagePermissions.FirstOrDefault(x => x.Object == image && x.User == user),
                x => x.write
                ); ;
            if (!write_perm)
                return RedirectToAction(nameof(ViewImage));
            model.Access = image.Access;
            model.Description = image.Description;
            model.Comment = image.Comment;
            model.Like = image.Like;
            dBContext.Update(model);
            await dBContext.SaveChangesAsync();

            return RedirectToAction(nameof(ViewImage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/Image/{id:int}-{name}/Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, string name)
        {
            var image = await dBContext.Images.FindAsync(id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            bool del_perm = GetImagePerm(
                () => dBContext.ImagePermissions.FirstOrDefault(x => x.Object == image && x.User == user),
                x => x.delete
                );
            if (!del_perm)
                return RedirectToAction(nameof(Error));

            dBContext.Entry(image).Collection(x => x.ImagePermissions).Load();
            image.ImagePermissions.ToList().ForEach(x => dBContext.Remove(x));
            
            dBContext.Images.Remove(image);
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/Image/{id:int}-{name}/Comment/{idc:int}/Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, string name, int idc)
        {
            var comment = await dBContext.Comments.FindAsync(idc);
            var user = await userManager.GetUserAsync(HttpContext.User);
            bool del_perm = GetCommentPerm(
                () => dBContext.CommentPermissions.FirstOrDefault(x => x.Object == comment && x.User == user),
                x => x.delete
                );
            if (!del_perm)
                return RedirectToAction(nameof(Error));

            dBContext.Entry(comment).Collection(x => x.CommentPermissions).Load();
            comment.CommentPermissions.ToList().ForEach(x => dBContext.Remove(x));

            dBContext.Remove(comment);
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewImage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/Image/{id:int}-{name}/UpdatePerm")]
        public async Task<IActionResult> UpdatePerm(int id, string name)
        {
            var image = await dBContext.Images.FindAsync(id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            var permission = dBContext.ImagePermissions.FirstOrDefault(x => x.Object == image && x.User == user);
            bool read_perm = GetImagePerm(() => permission, p => p.read);
            bool write_perm = GetImagePerm(() => permission, p => p.write);
            bool delete_perm = GetImagePerm(() => permission, p => p.delete);
            bool give_perm = GetImagePerm(() => permission, p => p.give);
            bool owner_perm = GetImagePerm(() => permission, p => p.owner);
            if (!give_perm)
                return RedirectToAction(nameof(Error));

            var formArgs = Request.Form;
            string userName = formArgs["selectedUser"];
            if (userName == null)
                return RedirectToAction(nameof(Error));
            bool read = formArgs["read"] == "read" && read_perm;
            bool write = formArgs["write"] == "write" && write_perm;
            bool delete = formArgs["delete"] == "delete" && delete_perm;
            bool give = formArgs["give"] == "give" && give_perm;
            bool take = formArgs["take"] == "take";

            if (!owner_perm && take)
                return RedirectToAction(nameof(Error));

            var newUser = dBContext.Users.First(x => x.UserName == userName);
            var newPermission = dBContext.ImagePermissions.FirstOrDefault(x => x.Object == image && x.User == newUser);
            if (newPermission == null)
            {
                newPermission = new Permission<Image>(read, write, delete, give, take, false);
                newPermission.User = newUser;
                newPermission.Object = image;
                dBContext.Add(newPermission);
            }
            else
            {
                if (newPermission.owner == true)
                    return RedirectToAction(nameof(Error));

                newPermission.read = read;
                newPermission.write = write;
                newPermission.delete = delete;
                newPermission.give = give;

                if (!owner_perm)
                {
                    newPermission.take = take;
                    permission.read = permission.write = permission.delete = permission.give = permission.take = false;
                    dBContext.Update(permission);
                }
                else newPermission.take = false;

                dBContext.Update(newPermission);
            }
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewImage));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Home/Image/{id:int}-{name}/Comment/{idc:int}/UpdatePerm")]
        public async Task<IActionResult> UpdatePerm(int id, string name, int idc)
        {
            var comment = await dBContext.Comments.FindAsync(idc);
            var user = await userManager.GetUserAsync(HttpContext.User);
            var permission = dBContext.CommentPermissions.FirstOrDefault(x => x.Object == comment && x.User == user);
            bool read_perm = GetCommentPerm(() => permission, p => p.read);
            bool write_perm = GetCommentPerm(() => permission, p => p.write);
            bool delete_perm = GetCommentPerm(() => permission, p => p.delete);
            bool give_perm = GetCommentPerm(() => permission, p => p.give);
            bool owner_perm = GetCommentPerm(() => permission, p => p.owner);
            if (!give_perm)
                return RedirectToAction(nameof(Error));

            var formArgs = Request.Form;
            string userName = formArgs["selectedUser"];
            if (userName == null)
                return RedirectToAction(nameof(Error));
            bool read = true;
            bool write = formArgs["write"] == "write" && write_perm;
            bool delete = formArgs["delete"] == "delete" && delete_perm;
            bool give = formArgs["give"] == "give" && give_perm;
            bool take = formArgs["take"] == "take";

            if (!owner_perm && take)
                return RedirectToAction(nameof(Error));

            var newUser = dBContext.Users.First(x => x.UserName == userName);
            var newPermission = dBContext.CommentPermissions.FirstOrDefault(x => x.Object == comment && x.User == newUser);
            if (newPermission == null)
            {
                newPermission = new Permission<Comment>(read, write, delete, give, take, false);
                newPermission.User = newUser;
                newPermission.Object = comment;
                dBContext.Add(newPermission);
            }
            else
            {
                if (newPermission.owner == true)
                    return RedirectToAction(nameof(Error));

                newPermission.read = read;
                newPermission.write = write;
                newPermission.delete = delete;
                newPermission.give = give;

                if (!owner_perm)
                {
                    newPermission.take = take;
                    permission.read = permission.write = permission.delete = permission.give = permission.take = false;
                    dBContext.Update(permission);
                }
                else newPermission.take = false;

                dBContext.Update(newPermission);
            }
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewImage));
        }

        [HttpPost]
        [Route("Home/Image/{id:int}-{name}/Comment")]
        public async Task<IActionResult> Comment(int id, string name, Comment comment)
        {
            //create comment
            var user = await userManager.GetUserAsync(HttpContext.User);
            comment.Owner = user;
            comment.Image = dBContext.Images.Find(id);
            comment.ID = 0;
            var permission = new Permission<Comment>(true);
            permission.Object = comment;
            permission.User = user;
            dBContext.Add(comment);
            dBContext.Add(permission);
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewImage));
        }
        [HttpGet]
        [Route("Home/Image/{id:int}-{name}/Comment/{idc:int}")]
        public IActionResult Comment(int id, string name, int idc)
        {
            var comm = dBContext.Comments.Find(idc);
            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            if (user != null)
            {
                var permission = dBContext.CommentPermissions.FirstOrDefault(x => x.User == user && x.Object == comm);
                //bool read_or_write_perm = GetCommentPerm(() => permission, x => x.read);
                if (permission != null)
                {
                    ViewBag.Perm = permission;
                    if (permission.give)
                    {
                        var usersPermissions = dBContext.CommentPermissions.Where(x => x.Object.ID == id && !x.owner).Include(x => x.User).ToList();
                        ViewBag.Permissions = usersPermissions;
                        if (!permission.owner)
                            ViewBag.UsersThatCanTake = usersPermissions.Where(x => x.take).Select(x => x.User).ToList();
                        else
                            ViewBag.UsersThatCanTake = dBContext.Users.Where(x => x != user).ToList();
                    }
                }
            }
            else
                return RedirectToAction(nameof(Error));

            return View(comm);
        }
        [HttpPost]
        [Route("Home/Image/{id:int}-{name}/Comment/{idc:int}")]
        public async Task<IActionResult> Comment(int id, string name, int idc, Comment comment)
        {
            //update
            var comm = await dBContext.Comments.FindAsync(idc);
            var user = await userManager.GetUserAsync(HttpContext.User);
            var permission = dBContext.CommentPermissions.FirstOrDefault(x => x.Object == comm && x.User == user);
            bool write_perm = GetCommentPerm(() => permission, p => p.write);
            if (!write_perm)
                return RedirectToAction(nameof(Error));
            dBContext.Entry(comm).State = EntityState.Detached;
            dBContext.Update(comment);
            await dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewImage));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        bool GetCommentPerm(Func<Permission<Comment>> func, Func<Permission<Comment>, bool> perm)
        {
            var res = func();
            return res == null ? false : perm(res);
        }
        bool GetImagePerm(Func<Permission<Image>> func, Func<Permission<Image>, bool> perm)
        {
            var res = func();
            return res == null ? false : perm(res);
        }
        public class CommentWithPerm
        {
            public Comment c;
            public bool p;
        }
    }
}
