using FashionGo.Models;
using System.Web.Mvc;

namespace FashionGo.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public abstract class AdminController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

    }
}