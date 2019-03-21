using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSK_proj2.Models
{
    public class Photo
    {
        [Key]
        public int ID { get; set; }
        public string Link { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
