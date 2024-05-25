using System.ComponentModel.DataAnnotations.Schema;

namespace FashionGo.Models.Entities
{
    public partial class Testimonial
    {
        public int Id { get; set; }

        [Column(TypeName = "nText")]
        public string Message { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatar { get; set; }
    }
}