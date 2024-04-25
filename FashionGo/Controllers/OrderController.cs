using FashionGo.Models.Entities;
using FashionGo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Commons.Libs;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Configuration;
using Newtonsoft.Json.Linq;
using MoMo;

using CKFinder.Connector;
using FashionGo.Others;
using Twilio.TwiML.Voice;
using System.Threading.Tasks;

namespace FashionGo.Controllers
{
    public class OrderController : BaseController
    {

        public ShoppingCart cart = ShoppingCart.Cart;

        [HttpGet]
        public ActionResult Checkout()
        {
            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
            {
                Warning(string.Format("<b><h5>{0}</h4></b>", "Vui lòng đăng nhập!"), true);
                return RedirectToAction("Index", "Cart");
            }
            if (cart.Count == 0)
            {
                Warning(string.Format("<b><h5>{0}</h4></b>", "Bạn chưa có sản phẩm nào trong giỏ hàng, Vui lòng chọn sản phẩm trước khi thanh toán."), true);
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.ProvinceId = new SelectList(db.Provinces.Select(x => new { ProvinceId = x.ProvinceId, NameFull = x.Type + " " + x.Name }), "ProvinceId", "NameFull");
            ViewBag.DistrictId = new SelectList(db.Districts.Where(d => d.ProvinceId == "-1").Select(x => new { DistrictId = x.DistrictId, NameFull = x.Type + " " + x.Name }), "DistrictId", "NameFull");

           
            var model = new Order();

            if (ModelState.IsValid && Request.IsAuthenticated)
            {
                model.UserId = User.Identity.GetUserId();
                var user = db.Users.Find(User.Identity.GetUserId());
                model.ReceiveName = user.FullName;
                model.ReceivePhone = user.PhoneNumber;
                model.ReceiveAddress = user.Address;
                
                ViewBag.Email = user.UserName;

                if (user.District != null)//second
                {
                    ViewBag.ProvinceId = new SelectList(db.Provinces.Select(x => new { ProvinceId = x.ProvinceId, NameFull = x.Type + " " +  x.Name }), "ProvinceId", "NameFull", user.District.ProvinceId);
                    ViewBag.DistrictId = new SelectList(db.Districts.Where(d => d.ProvinceId == user.District.ProvinceId).Select(x => new { DistrictId = x.DistrictId, NameFull = x.Type + " " + x.Name }), "DistrictId", "NameFull", user.DistrictId);
                }
            }

            return View(model);
        }

        public ActionResult OderPurchased()
        {
            var userId = User.Identity.GetUserId();
            var orders = db.Orders.Where(x => x.UserId == userId).ToList();
            return View(orders);
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Checkout(Order model, FormCollection form)
        {
            var sms = new SpeedSMSAPI();
            //Validate Cart
            if (cart.Count == 0)
            {
                Warning(string.Format("<h5>{0}</h4>", "Bạn chưa có sản phẩm nào trong giỏ hàng, Vui lòng chọn sản phẩm trước khi thanh toán."), true);
                return RedirectToAction("Index", "Home");
            }
            // Validate Email
            if (! Request.IsAuthenticated && String.IsNullOrEmpty(form["Email"]))
                ModelState.AddModelError("", "-Bạn chưa nhập email nhận đơn hàng!");

            //Check quận huyện
            if (String.IsNullOrEmpty(form["DistrictId"]))
                ModelState.AddModelError("", "-Bạn chưa chọn quận huyện nơi chuyển hàng tới!");

            //Check phuong thuc van chuyen
            if (String.IsNullOrEmpty(form["TransportId"]))
                ModelState.AddModelError("", "-Bạn chưa chọn nhà vận chuyển trước khi đặt hàng!");
            //Check phuong thuc thanh toán
            if (String.IsNullOrEmpty(form["PaymentMethodId"]))
                ModelState.AddModelError("", "-Bạn chưa chọn phương thức thanh toán!");
            
                int paymentMethodId = int.Parse(form["PaymentMethodId"]);



            var user = db.Users.Find(model.UserId);
            //Kiểm tra nếu là người dùng mới thì tạo tài khoản
            if (string.IsNullOrEmpty(model.UserId))
            {
                var password = Xstring.GeneratePassword();
                var newUser = new ApplicationUser
                {
                    UserName = form["Email"],
                    Email = form["Email"],
                    PhoneNumber = model.ReceivePhone,
                    PasswordHash = password,
                };

                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var result = await UserManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    var SignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    await SignInManager.SignInAsync(newUser, isPersistent: false, rememberBrowser: false);
                    model.UserId = newUser.Id;

                    //Gửi sms
                    string smsAcc = "QFashion: Tai khoan quan ly don hang cua ban tren QFashion la: Email:" + form["Email"] + ", mat khau:" + password;
                    string sent = sms.sendSMS(model.ReceivePhone, smsAcc, 2, "");

                    //Gửi tin nhắn tài khoản cho người dùng.
                    var subject = "Tài khoản quản lý đơn hàng tại ZdealVN.!";
                    var msg = "Xin chào, " + model.ReceiveName;
                    msg += "<br>Tài khoản quản lý đơn hàng của bạn tại <a href='http://QFashion.vn'>QFashion.vn</a> là:";
                    msg += "<br>-Tên đăng nhập: " + form["Email"];
                    msg += "<br>-Mật khẩu của bạn: " + password;
                    msg += "<br>Bạn có thể sử dụng tài khoản này đăng nhập trên QFashion.vn để quản lý đơn hàng và sử dụng các dịch vụ khác do QFashion cung cấp.!";
                    msg += "<br>Cảm ơn bạn đã quan tâm sử dụng dịch vụ của QFashion. mọi thắc mắc xin liên hệ hotline: 0901.002.822-0965.002.822.";
                    msg += "<br>QFashion Hân hạnh được phục vụ bạn.";
                    msg += "<br>Chúc bạn một ngày tốt lành.";
                    msg += "<p></p><p></p>-BQT ZdealVN!.</p>";

                    XMail.Send(newUser.Email, subject, msg);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", "-" + error);
                    }
                }

                ViewBag.Email = form["Email"];
            }
            else if (user != null)
            {
                //update FullName & Phone
                if (string.IsNullOrEmpty(user.FullName))
                {
                    user.FullName = model.ReceiveName;
                }
                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    user.PhoneNumber = model.ReceivePhone;
                }

                if (string.IsNullOrEmpty(user.Address))
                {
                    user.Address = model.ReceiveAddress;
                }

                if (string.IsNullOrEmpty(user.DistrictId))
                {
                    user.DistrictId = cart.Transport.DistrictId;
                }

                // Xử lý phương thức thanh toán
                switch (paymentMethodId)
                {
                    case 1: // Thanh toán tiền mặt
                            // Thêm logic xử lý thanh toán tiền mặt ở đây
                        break;

                    case 2:
                        string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
                        string partnerCode = "MOMOOJOI20210710";
                        string accessKey = "iPXneGmrJH0G8FOP";
                        string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
                        string orderInfo = "test";
                        string returnUrl = "https://localhost:57502/Order/ConfirmPaymentClient";
                        string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

                        string amount = "100000";
                        string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
                        string requestId = DateTime.Now.Ticks.ToString();
                        string extraData = "";

                        //Before sign HMAC SHA256 signature
                        string rawHash = "partnerCode=" +
                            partnerCode + "&accessKey=" +
                            accessKey + "&requestId=" +
                            requestId + "&amount=" +
                            amount + "&orderId=" +
                            orderid + "&orderInfo=" +
                            orderInfo + "&returnUrl=" +
                            returnUrl + "&notifyUrl=" +
                            notifyurl + "&extraData=" +
                            extraData;

                        MoMoSecurity crypto = new MoMoSecurity();
                        //sign signature SHA256
                        string signature = crypto.signSHA256(rawHash, serectkey);

                        //build body json request
                        JObject message = new JObject
                        {
                            { "partnerCode", partnerCode },
                            { "accessKey", accessKey },
                            { "requestId", requestId },
                            { "amount", amount },
                            { "orderId", orderid },
                            { "orderInfo", orderInfo },
                            { "returnUrl", returnUrl },
                            { "notifyUrl", notifyurl },
                            { "extraData", extraData },
                            { "requestType", "captureMoMoWallet" },
                            { "signature", signature }

                        };

                        string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

                        JObject jmessage = JObject.Parse(responseFromMomo);

                        return Redirect(jmessage.GetValue("payUrl").ToString());

                    case 3:
                        string url = ConfigurationManager.AppSettings["Url"];
                        string ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
                        string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
                        string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

                        PayLib pay = new PayLib();

                        pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
                        pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                        pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                        pay.AddRequestData("vnp_Amount", (cart.OrderTotal * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                        pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
                        pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                        pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                        pay.AddRequestData("vnp_IpAddr", Others.Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
                        pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                        pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
                        pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                        pay.AddRequestData("vnp_ReturnUrl", ReturnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                        pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

                        string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
                        TempData["model"] = model;
                        TempData["form"] = form;

                        return Redirect(paymentUrl);
                    default:
                        ModelState.AddModelError("", "-Phương thức thanh toán không hợp lệ!");
                        break;
                }

                SaveData(model, form);
            }   
            return View(model);
        }

        public ActionResult SaveData(Order model , FormCollection form)
        {
            var sms = new SpeedSMSAPI();
            //Update order info 
            model.TotalAmount = cart.Total;
            model.TotalOrder = cart.OrderTotal;
            if (cart.Transport != null) { model.TransportId = cart.Transport.Id; }
            model.PaymentMethodId = model.PaymentMethodId;
            model.Coupon = cart.CouponCode;
            model.Discount = cart.Discount;
            model.OrderDate = DateTime.Now;
            model.StatusId = model.StatusId != null ? model.StatusId : 1;

            db.Orders.Add(model);
            try
            {
                foreach (var p in cart.Items)
                {
                    var d = new OrderDetail
                    {
                        OrderId = model.Id,
                        ProductId = p.Id,
                        PriceAfter = p.PriceAfter.Value,
                        Discount = p.Discount.Value,
                        Amount = p.Amount,
                        Size = p.SizeDefaut,
                        Color = p.ColorDefaut

                    };
                    //ViewBag.ProductDetail = cart.Items;
                    db.OrderDetails.Add(d);
                }
                if (db.SaveChanges() > 0)
                {
                    var data = db.Users.Find(model.UserId);
                    cart.Clear();
                    Success(string.Format("<b><h5>{0}</h4></b>", "Đặt hàng thành công, chúng tôi sẽ liên hệ lại với bạn để xác nhận đơn hàng trước khi tiến hành giao hàng."), true);
                    //Gửi SMS xác nhận và báo tin cho Sale
                    var customerMsg = "QFashion: Dat hang thanh cong don hang:#" + model.Id + ", Voi so tien: " + string.Format("{0:0,0}vnđ", model.TotalAmount);
                    var saleSMS = "QFashion: Don hang moi #" + model.Id + " tu KH: " + model.ReceiveName + " - " + model.ReceivePhone;
                    string response = sms.sendSMS(model.ReceivePhone, customerMsg, 2, "");
                    response = sms.sendSMS("0334460843", saleSMS, 2, "");


                    //string accountSid = Convert.ToString(ConfigurationManager.AppSettings["config:AccountSID"]);
                    //string authToken = Convert.ToString(ConfigurationManager.AppSettings["config:AuthToken"]);
                    //string phone = Convert.ToString(ConfigurationManager.AppSettings["config:TwilioPhoneNum"]);
                    //TwilioClient.Init(accountSid, authToken);

                    //var message = MessageResource.Create(
                    //    body: "Hello",
                    //    from: new Twilio.Types.PhoneNumber(phone),
                    //    to: new Twilio.Types.PhoneNumber("+84334460843")
                    //);

                    //Gửi tin nhắn tài khoản cho người dùng.
                    var subject = "Đơn Hàng Tại QFASHION";
                    var msg = "Xin chào, " + model.ReceiveName;
                    msg += "<br>Chúng tôi xin gửi lời cảm ơn chân thành vì đã lựa chọn sản phẩm/dịch vụ của chúng tôi. ";
                    msg += "<br>Đơn Hàng Đã Được Đặt";
                    msg += "<br>Mã Đơn Hàng :#" + model.Id + ", Với Số Tiền: " + string.Format("{0:0,0}vnđ", model.TotalOrder);
                    msg += "<br>Chúng tôi sẽ tiến hành xử lý đơn hàng của bạn và sẽ thông báo cho bạn khi đơn hàng được vận chuyển";
                    msg += "<br>Nếu bạn có bất kỳ câu hỏi hoặc yêu cầu đặc biệt nào, xin vui lòng liên hệ với chúng tôi qua địa chỉ email <a href='mailto:ahan4960@gmail.com'>ahan4960@gmail.com</a> hoặc số điện thoại <a href='tel:0375021901'>0375021901</a>.";
                    msg += "<br>Chúc bạn một ngày tốt lành.";
                    msg += "<p></p><p></p>-BQT QFASHION!.</p>";

                    XMail.Send(data.Email, subject, msg);
                }

            }
            catch (Exception ex)
            {
                Danger(string.Format("-{0}<br>", ex.Message), true);
                ModelState.AddModelError("", ex.InnerException);
            }
            
            var provinceId = form["ProvinceId"];
            ViewBag.ProvinceId = new SelectList(db.Provinces.Select(x => new { ProvinceId = x.ProvinceId, NameFull = x.Type + " " + x.Name }), "ProvinceId", "NameFull", provinceId);
            ViewBag.DistrictId = new SelectList(db.Districts.Where(d => d.ProvinceId == provinceId).Select(x => new { DistrictId = x.DistrictId, NameFull = x.Type + " " + x.Name }), "DistrictId", "NameFull", form["DistrictId"].ToString());
            return View();
        }


        public async Task<ActionResult> PaymentConfirm()
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

                        var models = TempData["model"] as Order;
                        var forms = TempData["form"] as FormCollection;
                        models.StatusId = 8;
                        
                        SaveData(models, forms);
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

        public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
            String a = "";
        }


        public ActionResult Detail(int id)
        {
            var order = db.Orders.Find(id);
            ViewBag.Total = order.StatusId;
            return View(order);
        }
        public ActionResult Delete(int id)
        {
            var orderDetails = db.OrderDetails.Where(x=>x.OrderId == id);
            db.OrderDetails.RemoveRange(orderDetails);
            db.SaveChanges();

            var order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("OderPurchased");
        }
        public ActionResult List()
        {
            string currentUserId = User.Identity.GetUserId();
            var orders = db.Orders.Where(o => o.UserId == currentUserId).ToList();
            return View(orders);
        }

        
        public bool UpdateTransport(int transportId)
        {
            var transport = db.Transports.Find(transportId);
            if (transport == null)
            {
                return false;
            }
            
            //Update cart Transport
            cart.UpdateTransport(transport);

            return true;
        }

        
        public bool UpdateCoupon(string code)
        {
            var coupon = db.Coupons.Find(code);
            if (coupon == null)
            {
                return false;
            }
            //Update cart Transport
            cart.UpdateCoupon(coupon);

            return true;
        }

        
        public ActionResult AjaxGetTransport(string districtId)
        {
            var transports = db.Transports.Where(t => t.DistrictId == districtId).ToList();
            if (transports.Count() > 0)
            {
                UpdateTransport(transports.First().Id);
            }
            return PartialView(transports);
        }

        [HttpPost]
        
        public ActionResult AjaxUpdateCoupon(string couponCode)
        {
            var info = new
            {
                Status = 0,
                Msg = "Coupon không tồn tại hoặc đã hết hạn dùng.!"
            };

            if (UpdateCoupon(couponCode))
            {
                info = new
                {
                    Status = 1,
                    Msg = "Update thành công!"
                };
            }

            return Json(info, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult getOrderInfo()
        {
            var info = new
            {
                TransportCost = cart.TransportCost,
                Discount = cart.Discount,
                DiscountDescription = cart.discountDescription,
                OrderTotal = cart.OrderTotal
            };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
    }
}