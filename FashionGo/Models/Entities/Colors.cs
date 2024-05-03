using System.ComponentModel.DataAnnotations;

namespace FashionGo.Models.Entities
{
    public partial class Colors
    {
        [Key]
        public int ColorID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(50)]
        public string ColorName { get; set; }

        public virtual Product Product { get; set; }

    }
}
