using Commons.Libs;
using System.Web.Mvc;

namespace FashionGo.Controllers
{
    public class MailController : BaseController
    {
        // GET: Mail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Send(string name, string email, string message)

        {
            try
            {
                var from = name + "<" + email + ">";
                XMail.Send(from, "ahann4960@gmail.com", "test", message);
            }
            catch
            {

            }
            return View();

        }
    }
}