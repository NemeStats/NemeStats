using System.Web.Mvc;

namespace UI.Controllers.Helpers
{

    public static class TempMessageKeys
    {
        public const string CREATE_GAMEDEFITION_RESULT_TEMPMESSAGE = "creategamedefinitionresult";
        public const string MANAGE_ACCOUNT_RESULT_TEMPMESSAGE = "manageaccountresult";
        public const string TEMP_MESSAGE_KEY_PLAYED_GAME_RECORDED = "playedgamerecorded";
        public const string TEMP_MESSAGE_KEY_PLAYER_INVITED = "playerinvitedresult";
        public const string TEMP_MESSAGE_KEY_PLAYER_DELETED = "playerdeleted";
    }

    public partial class BaseController : Controller
    {

        public virtual void SetToastMessage(string key, string message, string kind = "success")
        {
            if (!string.IsNullOrEmpty(message))
            {
                TempData[key] = message;
                TempData[key + "_kind"] = kind;
            }
        }
    }
}