namespace FashionGo.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }


        [Display(Name = "Giá bán")]
        public double PriceAfter { get; set; }

        [Display(Name = "Giảm giá")]
        public double Discount { get; set; }

        [Display(Name = "Số lượng")]
        public int? Amount { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        [Display(Name = "Color")]

        public string Color { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
