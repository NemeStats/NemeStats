using System.Web.Mvc;

namespace UI.Controllers.Helpers
{

    public static class TempMessageKeys
    {
        public const string CREATE_GAMEDEFITION_RESULT_TEMPMESSAGE = "creategamedefinitionresult";
        public const string MANAGE_ACCOUNT_RESULT_TEMPMESSAGE = "manageaccountresult";
    }

    public class BaseController : Controller
    {     

        public void SetTempMessage(string key, string message, string kind = "success")
        {
            if (!string.IsNullOrEmpty(message))
            {
                TempData[key] = message;
                TempData[key + "_kind"] = kind;
            }
        }
    }
}