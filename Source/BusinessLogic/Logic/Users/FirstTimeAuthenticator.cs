using System.Configuration;
using System.Configuration.Abstractions;
using System.Web;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public class FirstTimeAuthenticator : IFirstTimeAuthenticator
    {
        internal const string APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL = "emailConfirmationCallbackUrl";
        internal const string CONFIRMATION_EMAIL_CALLBACK_URL_SUFFIX = "?userId={0}&code={1}";
        internal const string CONFIRMATION_EMAIL_BODY = "Please confirm your account by clicking this <a href=\"{0}\">link</a>";
        internal const string EMAIL_SUBJECT = "Confirm Your NemeStats Account";

        private readonly IGamingGroupSaver gamingGroupSaver;
        private readonly IConfigurationManager configurationManager;
        private readonly ApplicationUserManager applicationUserManager;
        private readonly IDataContext dataContext;

        public FirstTimeAuthenticator(
            IGamingGroupSaver gamingGroupSaver,
            ApplicationUserManager applicationUserManager, 
            IConfigurationManager configurationManager, 
            IDataContext dataContext)
        {
            this.gamingGroupSaver = gamingGroupSaver;
            this.applicationUserManager = applicationUserManager;
            this.configurationManager = configurationManager;
            this.dataContext = dataContext;
        }

        public async Task<object> CreateGamingGroupAndSendEmailConfirmation(ApplicationUser applicationUser)
        {
            //fetch this first since we want to fail as early as possible if the config entry is missing
            var callbackUrl = this.GetCallbackUrlFromConfig();

            await this.gamingGroupSaver.CreateNewGamingGroup(
                applicationUser.UserName + "'s Gaming Group", applicationUser);

            await this.SendConfirmationEmail(applicationUser, callbackUrl);

            return new object();
        }

        private string GetCallbackUrlFromConfig()
        {
            string callbackUrl;
            try
            {
                callbackUrl = this.configurationManager.AppSettings.Get(APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL);
            }
            catch (Exception)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Missing app setting with key: {0}", APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL));
            }
            return callbackUrl;
        }

        private async Task SendConfirmationEmail(ApplicationUser applicationUser, string callbackActionUrl)
        {
            var code = await this.applicationUserManager.GenerateEmailConfirmationTokenAsync(applicationUser.Id);

            var callbackUrl = callbackActionUrl + string.Format(CONFIRMATION_EMAIL_CALLBACK_URL_SUFFIX, applicationUser.Id, HttpUtility.UrlEncode(code));
            var emailBody = string.Format(CONFIRMATION_EMAIL_BODY, callbackUrl);
            await this.applicationUserManager.SendEmailAsync(applicationUser.Id, EMAIL_SUBJECT, emailBody);
        }
    }
}
