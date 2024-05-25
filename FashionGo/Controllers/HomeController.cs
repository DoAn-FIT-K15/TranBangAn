using FashionGo.Models;
using FashionGo.Models.Entities;
using FashionGo.Others;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FashionGo.Controllers
{
    public class HomeController : BaseController
    {
        public DateTime now = new DateTime();

        public ActionResult Index()
        {
            //Saleoff product
            var saleOffProducts = db.Products.Where(p => p.Actived == true).Where(p => p.IsSpecial == true).Take(20);
            ViewBag.saleOffProducts = saleOffProducts;

            //Lastest Product
            var lastProducts = db.Products.Where(p => p.Actived == true).OrderByDescending(p => p.IsNew).OrderBy(p => p.CreateDate).Take(20);
            ViewBag.lastProducts = lastProducts;

            var model = db.ProductCategories.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder);

            return View(model);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }


        public ActionResult _AboutUs()
        {
            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Liên hệ FashionGo";

            return View();
        }

        public ActionResult Category()
        {
            var model = db.ProductCategories.OrderBy(p => p.DisplayOrder);
            return PartialView("Partials/_Category", model);
        }

        public ActionResult Header()
        {
            var today = new DateTime();
            var cart = ShoppingCart.Cart;
            try
            {
                var cats = db.ProductCategories.OrderBy(p => p.DisplayOrder).ToList();
                ViewBag.ProductCategories = cats;
            }
            catch
            {

            }

            try
            {
                // Lấy cookie cũ tên views
                var wishlist = Request.Cookies["wishlist"];
                // Nếu chưa có cookie cũ -> tạo mới
                if (wishlist == null)
                {
                    wishlist = new HttpCookie("wishlist");
                }
                ViewBag.Count = wishlist.Values.Count;
            }
            catch
            {

            }

            ViewBag.Provinces = db.Provinces.OrderBy(p => p.Name).ToList();

            ViewBag.BannerTop = db.Banners.Where(b => b.BannerTypeId == 1)
                                        .Where(b => b.BannerPositionId == 1)
                                        .Where(b => b.Active == true)
                                        .Where(b => b.StartDate >= today)
                                        .Where(b => b.EndDate > today).FirstOrDefault();

            return PartialView("Partials/_Header", cart.Items);
        }

        public ActionResult _SliderShow()
        {
            var model = db.Sliders.OrderByDescending(s => s.Id).Take(8).ToList();
            return PartialView(model);
        }

        public ActionResult _Testimonials()
        {
            var testimonials = db.Testimonials.Take(6).ToList();
            return PartialView(testimonials);
        }

        public ActionResult _AllShopCategory()
        {
            var productCategories = db.ProductCategories.Where(c => c.ParentId == null).Take(5).ToList();
            return PartialView(productCategories);
        }

        public ActionResult _TosWidget()
        {
            return PartialView();
        }


        public ActionResult Footer()
        {
            ViewBag.MenuGioiThieu = db.Menus.Where(m => m.LocationId == 1).ToList();
            ViewBag.MenuTroGiup = db.Menus.Where(m => m.LocationId == 2).ToList();

            return PartialView("Partials/_Footer");
        }


        public ActionResult Culture(string id)
        {
            HttpCookie cookie = Request.Cookies["Culture"];
            if (cookie == null)
            {
                cookie = new HttpCookie("Culture", id);
            }
            cookie.Value = id;
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.SetCookie(cookie);

            return Redirect("/");
        }

        public ActionResult PopoupBanner()
        {
            //var today = new DateTime();
            Banner popup = db.Banners.Where(b => b.BannerTypeId == 2)
                            .Where(b => b.Active == true).FirstOrDefault();

            return PartialView("Partials/_PopupBaner", popup);
        }


        public ActionResult NotFound()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AjaxGetDistrictByProvice(string provinceId)
        {
            //var m = db.Provinces.Find(provinceId);
            return PartialView(db.Districts.Where(d => d.ProvinceId == provinceId).ToList());
        }

    }
}