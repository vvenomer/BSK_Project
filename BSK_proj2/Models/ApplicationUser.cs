using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BSK_proj2
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment> CommentsOwner { get; set; }
        public virtual ICollection<Permission<Comment>> CommentPermissions { get; set; }
        public virtual ICollection<Permission<Image>> ImagePermissions { get; set; }
    }
}
