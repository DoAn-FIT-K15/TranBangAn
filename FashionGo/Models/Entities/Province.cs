namespace FashionGo.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Province
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Province()
        {
            Districts = new HashSet<District>();
        }
        [Key]
        [StringLength(255)]
        public string ProvinceId { get; set; }

        [StringLength(255)]
        [Display(Name = "Tỉnh thành")]
        public string Name { get; set; }

        [StringLength(255)]
        [Display(Name = "Loại tỉnh/thành")]
        public string Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<District> Districts { get; set; }

    }
}
