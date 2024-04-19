
namespace FashionGo.Models
{
    using global::FashionGo.Models.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class MenuCategoryViewModel
    {
        public List<ProductCategory> procate { get; set; }
        public List<Menu> menu { get; set; }
        public List<Product> pro { get; set; }


    }
}