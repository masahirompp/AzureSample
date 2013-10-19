
namespace jp.ne.ghopper.echo.azuredemo.Helper
{
    public class CommonHelper
    {
        /// <summary>
        /// パスワードのハッシュ値を求める。
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string HashPassword(string plainText)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(plainText, "md5");
        }


    }
}