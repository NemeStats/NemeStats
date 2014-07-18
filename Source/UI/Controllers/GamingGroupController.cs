using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Filters;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Controllers
{
    [Authorize]
    public partial class GamingGroupController : Controller
    {
        internal NemeStatsDbContext db;
        internal GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation;
        internal GamingGroupRepository gamingGroupRepository;
        internal GamingGroupAccessGranter gamingGroupAccessGranter;

        public GamingGroupController(
            NemeStatsDbContext dbContext,
            GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation,
            GamingGroupRepository gamingGroupRespository,
            GamingGroupAccessGranter gamingGroupAccessGranter)
        {
            this.db = dbContext;
            this.gamingGroupToGamingGroupViewModelTransformation = gamingGroupToGamingGroupViewModelTransformation;
            this.gamingGroupRepository = gamingGroupRespository;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
        }

        //
        // GET: /GamingGroup
        [UserContextAttribute]
        public virtual ActionResult Index(UserContext userContext)
        {
            GamingGroup gamingGroup = gamingGroupRepository.GetGamingGroupDetails(userContext.GamingGroupId.Value, userContext);
            GamingGroupViewModel viewModel = gamingGroupToGamingGroupViewModelTransformation.Build(gamingGroup);

            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        [HttpGet]
        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Create(string gamingGroupName, UserContext userContext)
        {
            return View();
        }


        //
        // POST: /GamingGroup/Delete/5
        [HttpPost]
        [UserContextAttribute]
        public virtual ActionResult GrantAccess(string inviteeEmail, UserContext userContext)
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);
            return RedirectToAction(MVC.GamingGroup.ActionNames.Index);
        }
    }
}
