using System.Web.Mvc;
using BusinessLogic.Jobs.BoardGameGeekCleanUpService;

namespace UI.Controllers
{
    public partial class JobsController : Controller
    {
        private readonly IBoardGameGeekBatchUpdateService _boardGameGeekBatchUpdateService;

        public JobsController(IBoardGameGeekBatchUpdateService boardGameGeekBatchUpdateService)
        {
            _boardGameGeekBatchUpdateService = boardGameGeekBatchUpdateService;
        }

        // GET: Jobs
        public virtual ActionResult LinkOrphanGames()
        {
            var result = _boardGameGeekBatchUpdateService.LinkOrphanGames();

            return Content(result.ToString().Replace("\r\n", "<br/>"));
        }
    }
}