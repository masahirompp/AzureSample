using System.Web.Mvc;
using System.Web.Security;

namespace jp.ne.ghopper.echo.azuredemo.Controllers
{
    public class LogoutController : Controller
    {
        //
        // GET: /Logout/

        public ActionResult Index()
        {
            //認証クッキー破棄
            FormsAuthentication.SignOut();

            //セッション破棄
            Session.Abandon();

            return Redirect("/");
        }

    }
}
