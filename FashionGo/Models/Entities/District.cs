namespace FashionGo.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class District
    {
        [Key]
        [StringLength(255)]
        public string DistrictId { get; set; }

        [StringLength(255)]
        [Display(Name = "Quận/Huyện")]
        public string Name { get; set; }

        [StringLength(255)]
        [Display(Name = "Loại quận/huyện")]
        public string Type { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa điểm")]
        public string Location { get; set; }

        [StringLength(255)]
        [Display(Name = "Mã tỉnh/thành")]
        public string ProvinceId { get; set; }

        public virtual Province Province { get; set; }

        public virtual List<Transport> Transports { get; set; }
    }
}
