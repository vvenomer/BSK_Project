using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSK_proj2.Models
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }

        public virtual ApplicationUser Owner { get; set; }
        public virtual Image Image { get; set; }
        public virtual ICollection<Permission<Comment>> CommentPermissions { get; set; }
    }
}
