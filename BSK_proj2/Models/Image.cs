using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSK_proj2.Models
{
    public class Image
    {
        [Key]
        public int ID { get; set; }

        public string LinkType { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Access { get; set; }
        public bool Comment { get; set; }
        public bool Like { get; set; }

        public virtual ICollection<Permission<Image>> ImagePermissions { get; set; }
    }
}
