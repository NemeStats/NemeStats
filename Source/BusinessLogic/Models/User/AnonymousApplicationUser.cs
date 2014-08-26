using BusinessLogic.EventTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.User
{
    [NotMapped]
    public class AnonymousApplicationUser : ApplicationUser
    {
        public const string USER_NAME_ANONYMOUS = "Anonymous User";

        public AnonymousApplicationUser()
        {

        }

        public override string AnonymousClientId
        {
            get
            {
                return UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID;
            }
        }

        public override string UserName
        {
            get
            {
                return USER_NAME_ANONYMOUS;
            }
        }
    }
}
