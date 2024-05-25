using FashionGo.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FashionGo.Controllers
{
    public class CartController : BaseController
    {
        public ActionResult Index()
        {
            if (ShoppingCart.Cart.Count == 0)
            {
                Warning(string.Format("<b><h5>{0}</h4></b>", "Bạn chưa có sản phẩm nào trong giỏ hàng, Vui lòng chọn sản phẩm trước khi thanh toán."), true);
                return RedirectToAction("Index", "Home");
            }

            var cart = ShoppingCart.Cart;
            return View(cart.Items);

        }
        public ActionResult OrderDetail()
        {
            var cart = ShoppingCart.Cart;
            return PartialView("Partials/_OrderDetail", cart.Items);
        }


        public ActionResult _PartialCart()
        {
            var cart = ShoppingCart.Cart;
            return PartialView(cart.Items);
        }

        public ActionResult Add(int id, int quatity, string color, string size)
        {
            var cart = ShoppingCart.Cart;

            cart.Add(id, quatity, color, size);

            var info = new { Count = cart.Count, Total = cart.Total };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddCartInDetail(int id, int quantity, string color, string size)
        {
            try
            {
                var cart = ShoppingCart.Cart;
                cart.Add(id, quantity, color, size);

                return Json(new { success = true, Count = cart.Count, Total = cart.Total });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public ActionResult Remove(int id)
        {
            var cart = ShoppingCart.Cart;
            cart.Remove(id);
            if (cart.Total == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var info = new { Count = cart.Count, Total = cart.Total };
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int id, int quantity)
        {
            var cart = ShoppingCart.Cart;
            cart.Update(id, quantity);

            var p = cart.Items.Single(i => i.Id == id);
            var info = new
            {
                Count = cart.Count,
                Total = cart.Total,
                quantity = quantity,
                Amount = p.PriceAfter.Value * p.Amount
            };
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateColor(int id, string color)
        {
            var cart = ShoppingCart.Cart;
            cart.UpdateColor(id, color);

            var p = cart.Items.Single(i => i.Id == id);
            var info = new
            {
                Count = cart.Count,
                Total = cart.Total,
                Colors = color,
                Amount = p.PriceAfter.Value * p.Amount
            };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateSize(int id, string size)
        {
            var cart = ShoppingCart.Cart;
            cart.UpdateSize(id, size);

            var p = cart.Items.Single(i => i.Id == id);
            var info = new
            {
                Count = cart.Count,
                Total = cart.Total,
                Sizes = size,
                Amount = p.PriceAfter.Value * p.Amount
            };
            return Json(info, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Clear()
        {

            var cart = ShoppingCart.Cart;
            cart.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult _MiniCart()
        {
            return PartialView();
        }
        public ActionResult _MiniCartMobile()
        {
            return PartialView();
        }
    }
}