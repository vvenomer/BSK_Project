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

        public DbSet<Photo> Photos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
