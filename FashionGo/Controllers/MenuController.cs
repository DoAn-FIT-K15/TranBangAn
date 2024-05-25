using FashionGo.Models;
using System.Linq;
using System.Web.Mvc;

namespace FashionGo.Controllers
{
    public class MenuController : BaseController
    {
        // GET: Menu
        public ActionResult _MainMenu()
        {
            var data = new MenuCategoryViewModel
            {
                //Mainmenu Location = 8
                menu = db.Menus.Where(m => m.LocationId == 8).ToList(),
                procate = db.ProductCategories.Where(pc => pc.ParentId == null).ToList(),
                pro = db.Products.Where(m => m.Discount >= 50).ToList(),

            };
            return PartialView(data);
        }


        public ActionResult _MobileMenu()
        {
            //Mainmenu Location = 8
            var menus = db.Menus.Where(m => m.LocationId == 8).ToList();
            return PartialView(menus);
        }
    }
}