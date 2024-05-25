namespace FashionGo.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    public partial class Sizes
    {
        [Key]
        public int SizeID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(50)]
        public string SizeName { get; set; }
        public virtual Product Product { get; set; }

    }
}
