using System;
using System.Web.Mvc;
using System.Web.Security;
using jp.ne.ghopper.echo.azuredemo.Dto;
using jp.ne.ghopper.echo.azuredemo.Services;

namespace jp.ne.ghopper.echo.azuredemo.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _us;
        public LoginController() { _us = new UserService(); }

        //
        // GET: /Login/

        public ActionResult Index()
        {
            ViewBag.Title = "ログイン";
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginDto m)
        {
            try
            {
                //ログオン検証（エラーの場合はException）
                var user = _us.Login(1, m.userid, m.password);

                //認証クッキー発行
                FormsAuthentication.SetAuthCookie(m.userid, false);

                //セッションにログオン情報格納
                Session.Add(SessionKey.LogonUser, user);

                return Redirect("/User");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
