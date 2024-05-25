using System.Web.Mvc;

namespace FashionGo.Controllers
{
    public class BannerAdsController : Controller
    {
        // GET: BannerAds
        public ActionResult _HomeBanner()
        {
            return PartialView();
        }

    }
}