using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;

namespace ScheduleHelper.UI.Controllers
{
    public class ScheduleController : Controller
    {

        string _taskslistTitle = "Tasks list";
        ITaskService _taskService;

        public ScheduleController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Route(RouteConstants.ShowSchedule)]
        public async Task<IActionResult> ShowSchedule()
        {
          
            List<TaskForSheduleDTO> tasks = await _taskService.GetTasksForSchedule();
            return View("Schedule",tasks);
        }

        [Route(RouteConstants.GenerateScheduleSettings)]
        public async Task<IActionResult> GenerateScheduleSettings()
        {

            
            return View("GenerateScheduleSettings");
        }

    }
}
