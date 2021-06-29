using BLL.BLL;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class UserInfoController : UserInfoBLL
    {
        // GET: UserInfo
        public ActionResult UserInfo()
        {
            return View();
        }
    }
}