namespace FashionGo.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Sizes
    {
        [Key]
        public int SizeID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(50)]
        public string SizeName { get; set; }
    }
}
