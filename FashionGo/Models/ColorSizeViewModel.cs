namespace FashionGo.Models
{
    using global::FashionGo.Models.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ColorSizeViewModel
    {
        public Colors Color { get; set; }
        public Sizes Size { get; set; }
    }
}