using Microsoft.AspNetCore.Mvc;

namespace ScheduleHelper.UI.Controllers
{
    public class TasksListController : Controller
    {
        [Route("/")]
        public async Task<IActionResult> TasksList()
        {
            return View();
        }
    }
}
