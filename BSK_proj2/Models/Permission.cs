using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSK_proj2.Models
{
    public class Permission<T>
    {
        public Permission() { }
        public Permission(bool owner)
        {
            read = write = delete = give = take = this.owner = owner;
        }
        public Permission(bool read, bool write, bool delete, bool give, bool take, bool owner)
        {
            this.read = read;
            this.write = write;
            this.delete = delete;
            this.give = give;
            this.take = take;
            this.owner = owner;
        }

        [Key]
        public int ID { get; set; }

        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
        public bool give { get; set; }
        public bool take { get; set; }
        public bool owner { get; set; }
        
        public virtual ApplicationUser User { get; set; }
        public virtual T Object { get; set; }
    }
}
