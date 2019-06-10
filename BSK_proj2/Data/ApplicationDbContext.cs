using System;
using System.Collections.Generic;
using System.Text;
using BSK_proj2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BSK_proj2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Image> Images { get; set; }
        public DbSet<Permission<Image>> ImagePermissions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Permission<Comment>> CommentPermissions { get; set; }

        public new DbSet<ApplicationUser> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
