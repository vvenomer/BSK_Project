using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BSK_proj2.Models
{
    public class UploadedImage
    {
        public string image_choice { get; set; }
        
        public IFormFile uploaded_img {get; set; }
        [Url]
        public string linked_img { get; set; }
        public List<SelectListItem> access { get; } = new List<SelectListItem>
        {
            new SelectListItem {Value = "public", Text="Public"},
            new SelectListItem {Value = "private", Text="Private"}
        };
        public bool comment { get; set; }
        public bool like { get; set; }
        
    }
}
