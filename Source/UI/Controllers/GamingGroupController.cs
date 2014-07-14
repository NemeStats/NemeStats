using BusinessLogic.DataAccess;
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
    public partial class GamingGroupController : Controller
    {
        internal NemeStatsDbContext db;
        internal GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation;
        internal GamingGroupRepository gamingGroupRepository;

        public GamingGroupController(
            NemeStatsDbContext dbContext,
            GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation,
            GamingGroupRepository gamingGroupRespository)
        {
            this.db = dbContext;
            this.gamingGroupToGamingGroupViewModelTransformation = gamingGroupToGamingGroupViewModelTransformation;
            this.gamingGroupRepository = gamingGroupRespository;
        }

        //
        // GET: /GamingGroup
        [UserContextActionFilter]
        public virtual ActionResult Index(UserContext userContext)
        {
            //TODO should redirect to some other action if the user doesn't have a gaming group (rather than NPE here)
            GamingGroup gamingGroup = gamingGroupRepository.GetGamingGroupDetails(userContext.GamingGroupId.Value, userContext);
            GamingGroupViewModel viewModel = gamingGroupToGamingGroupViewModelTransformation.Build(gamingGroup);
            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        // POST: /GamingGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Edit([Bind(Include = "Id,Name")] GamingGroup player, UserContext userContext)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(MVC.Player.ActionNames.Index);
            }
            return View(MVC.Player.Views.Edit, player);
        }

        //
        // POST: /GamingGroup/Delete/5
        [HttpPost]
        public virtual ActionResult GrantAccess(string email, UserContext userContext)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
