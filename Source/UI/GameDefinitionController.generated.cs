// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace UI.Controllers
{
    public partial class GameDefinitionController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected GameDefinitionController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Details()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Details);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Create()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Create);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult AjaxCreate()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AjaxCreate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ImportFromBGG()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ImportFromBGG);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Edit()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Edit);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CreatePartial()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CreatePartial);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SearchBoardGameGeekHttpGet()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchBoardGameGeekHttpGet);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SearchGameDefinition()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchGameDefinition);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public GameDefinitionController Actions { get { return MVC.GameDefinition; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "GameDefinition";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "GameDefinition";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Details = "Details";
            public readonly string Create = "Create";
            public readonly string AjaxCreate = "AjaxCreate";
            public readonly string ImportFromBGG = "ImportFromBGG";
            public readonly string Edit = "Edit";
            public readonly string CreatePartial = "CreatePartial";
            public readonly string SearchBoardGameGeekHttpGet = "SearchBoardGameGeekHttpGet";
            public readonly string SearchGameDefinition = "SearchGameDefinition";
            public readonly string ShowTrendingGames = "ShowTrendingGames";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Details = "Details";
            public const string Create = "Create";
            public const string AjaxCreate = "AjaxCreate";
            public const string ImportFromBGG = "ImportFromBGG";
            public const string Edit = "Edit";
            public const string CreatePartial = "CreatePartial";
            public const string SearchBoardGameGeekHttpGet = "SearchBoardGameGeekHttpGet";
            public const string SearchGameDefinition = "SearchGameDefinition";
            public const string ShowTrendingGames = "ShowTrendingGames";
        }


        static readonly ActionParamsClass_Details s_params_Details = new ActionParamsClass_Details();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Details DetailsParams { get { return s_params_Details; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Details
        {
            public readonly string id = "id";
            public readonly string currentUser = "currentUser";
        }
        static readonly ActionParamsClass_Create s_params_Create = new ActionParamsClass_Create();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Create CreateParams { get { return s_params_Create; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Create
        {
            public readonly string returnUrl = "returnUrl";
            public readonly string createGameDefinitionViewModel = "createGameDefinitionViewModel";
            public readonly string currentUser = "currentUser";
        }
        static readonly ActionParamsClass_AjaxCreate s_params_AjaxCreate = new ActionParamsClass_AjaxCreate();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AjaxCreate AjaxCreateParams { get { return s_params_AjaxCreate; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AjaxCreate
        {
            public readonly string createGameDefinitionViewModel = "createGameDefinitionViewModel";
            public readonly string currentUser = "currentUser";
        }
        static readonly ActionParamsClass_ImportFromBGG s_params_ImportFromBGG = new ActionParamsClass_ImportFromBGG();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ImportFromBGG ImportFromBGGParams { get { return s_params_ImportFromBGG; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ImportFromBGG
        {
            public readonly string currentUser = "currentUser";
        }
        static readonly ActionParamsClass_Edit s_params_Edit = new ActionParamsClass_Edit();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Edit EditParams { get { return s_params_Edit; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Edit
        {
            public readonly string id = "id";
            public readonly string currentUser = "currentUser";
            public readonly string viewModel = "viewModel";
        }
        static readonly ActionParamsClass_CreatePartial s_params_CreatePartial = new ActionParamsClass_CreatePartial();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CreatePartial CreatePartialParams { get { return s_params_CreatePartial; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CreatePartial
        {
            public readonly string currentUser = "currentUser";
        }
        static readonly ActionParamsClass_SearchBoardGameGeekHttpGet s_params_SearchBoardGameGeekHttpGet = new ActionParamsClass_SearchBoardGameGeekHttpGet();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SearchBoardGameGeekHttpGet SearchBoardGameGeekHttpGetParams { get { return s_params_SearchBoardGameGeekHttpGet; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SearchBoardGameGeekHttpGet
        {
            public readonly string searchText = "searchText";
        }
        static readonly ActionParamsClass_SearchGameDefinition s_params_SearchGameDefinition = new ActionParamsClass_SearchGameDefinition();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SearchGameDefinition SearchGameDefinitionParams { get { return s_params_SearchGameDefinition; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SearchGameDefinition
        {
            public readonly string q = "q";
            public readonly string currentUser = "currentUser";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _CreatePartial = "_CreatePartial";
                public readonly string _GameDefinitionPlayedGamesPartial = "_GameDefinitionPlayedGamesPartial";
                public readonly string _GameDefinitionsPartial = "_GameDefinitionsPartial";
                public readonly string _GameDefinitionsTablePartial = "_GameDefinitionsTablePartial";
                public readonly string _GamingGroupGameDefinitionStatsPartial = "_GamingGroupGameDefinitionStatsPartial";
                public readonly string _TrendingGamesPartial = "_TrendingGamesPartial";
                public readonly string Create = "Create";
                public readonly string Details = "Details";
                public readonly string Edit = "Edit";
                public readonly string TrendingGames = "TrendingGames";
            }
            public readonly string _CreatePartial = "~/Views/GameDefinition/_CreatePartial.cshtml";
            public readonly string _GameDefinitionPlayedGamesPartial = "~/Views/GameDefinition/_GameDefinitionPlayedGamesPartial.cshtml";
            public readonly string _GameDefinitionsPartial = "~/Views/GameDefinition/_GameDefinitionsPartial.cshtml";
            public readonly string _GameDefinitionsTablePartial = "~/Views/GameDefinition/_GameDefinitionsTablePartial.cshtml";
            public readonly string _GamingGroupGameDefinitionStatsPartial = "~/Views/GameDefinition/_GamingGroupGameDefinitionStatsPartial.cshtml";
            public readonly string _TrendingGamesPartial = "~/Views/GameDefinition/_TrendingGamesPartial.cshtml";
            public readonly string Create = "~/Views/GameDefinition/Create.cshtml";
            public readonly string Details = "~/Views/GameDefinition/Details.cshtml";
            public readonly string Edit = "~/Views/GameDefinition/Edit.cshtml";
            public readonly string TrendingGames = "~/Views/GameDefinition/TrendingGames.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_GameDefinitionController : UI.Controllers.GameDefinitionController
    {
        public T4MVC_GameDefinitionController() : base(Dummy.Instance) { }

        [NonAction]
        partial void DetailsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int? id, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult Details(int? id, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Details);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            DetailsOverride(callInfo, id, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void CreateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string returnUrl);

        [NonAction]
        public override System.Web.Mvc.ActionResult Create(string returnUrl)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Create);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "returnUrl", returnUrl);
            CreateOverride(callInfo, returnUrl);
            return callInfo;
        }

        [NonAction]
        partial void CreateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, UI.Models.GameDefinitionModels.CreateGameDefinitionViewModel createGameDefinitionViewModel, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult Create(UI.Models.GameDefinitionModels.CreateGameDefinitionViewModel createGameDefinitionViewModel, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Create);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "createGameDefinitionViewModel", createGameDefinitionViewModel);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            CreateOverride(callInfo, createGameDefinitionViewModel, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void AjaxCreateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, UI.Models.GameDefinitionModels.CreateGameDefinitionViewModel createGameDefinitionViewModel, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult AjaxCreate(UI.Models.GameDefinitionModels.CreateGameDefinitionViewModel createGameDefinitionViewModel, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.AjaxCreate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "createGameDefinitionViewModel", createGameDefinitionViewModel);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            AjaxCreateOverride(callInfo, createGameDefinitionViewModel, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void ImportFromBGGOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult ImportFromBGG(BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ImportFromBGG);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            ImportFromBGGOverride(callInfo, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void EditOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int? id, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult Edit(int? id, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Edit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            EditOverride(callInfo, id, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void EditOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, UI.Models.GameDefinitionModels.GameDefinitionEditViewModel viewModel, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult Edit(UI.Models.GameDefinitionModels.GameDefinitionEditViewModel viewModel, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Edit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "viewModel", viewModel);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            EditOverride(callInfo, viewModel, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void CreatePartialOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult CreatePartial(BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CreatePartial);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            CreatePartialOverride(callInfo, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void SearchBoardGameGeekHttpGetOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string searchText);

        [NonAction]
        public override System.Web.Mvc.ActionResult SearchBoardGameGeekHttpGet(string searchText)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchBoardGameGeekHttpGet);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "searchText", searchText);
            SearchBoardGameGeekHttpGetOverride(callInfo, searchText);
            return callInfo;
        }

        [NonAction]
        partial void SearchGameDefinitionOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string q, BusinessLogic.Models.User.ApplicationUser currentUser);

        [NonAction]
        public override System.Web.Mvc.ActionResult SearchGameDefinition(string q, BusinessLogic.Models.User.ApplicationUser currentUser)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SearchGameDefinition);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "q", q);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "currentUser", currentUser);
            SearchGameDefinitionOverride(callInfo, q, currentUser);
            return callInfo;
        }

        [NonAction]
        partial void ShowTrendingGamesOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult ShowTrendingGames()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ShowTrendingGames);
            ShowTrendingGamesOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
