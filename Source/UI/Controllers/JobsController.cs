using System.Web.Mvc;
using BusinessLogic.Jobs.BoardGameGeekCleanUpService;

namespace UI.Controllers
{
    public partial class JobsController : Controller
    {
        private readonly IBoardGameGeekCleanUpService _boardGameGeekCleanUpService;

        public JobsController(IBoardGameGeekCleanUpService boardGameGeekCleanUpService)
        {
            _boardGameGeekCleanUpService = boardGameGeekCleanUpService;
        }

        // GET: Jobs
        public virtual ActionResult LinkOrphanGames()
        {
            var result = _boardGameGeekCleanUpService.LinkOrphanGames();

            return Content(result.ToString().Replace("\r\n", "<br/>"));
        }
    }
}