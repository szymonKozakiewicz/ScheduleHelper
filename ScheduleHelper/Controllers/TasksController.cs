using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;


namespace ScheduleHelper.UI.Controllers
{
    public class TasksController : Controller
    {

        string taskslistTitle = "Tasks list";
        
        [Route(RouteConstants.ShowTasksList)]
        public async Task<IActionResult> TasksList()
        {
            ViewBag.Title = taskslistTitle;
            return View();
        }


        [Route(RouteConstants.AddNewTask)]
        [HttpGet]
        public async Task<IActionResult> AddNewTask()
        {
            ViewBag.Title = "New task";
            ViewBag.formHref = RouteConstants.AddNewTask;
            return View("EditTask",new SingleTaskDTO());
        }

        [Route(RouteConstants.AddNewTask)]
        [HttpPost]
        public async Task<IActionResult> AddNewTask(SingleTaskDTO newTask)
        {
            ViewBag.Title = taskslistTitle;

            return RedirectToAction(nameof(TasksController.TasksList));
        }


        [Route(RouteConstants.UpdateTask)]
        public async Task<IActionResult> EditTask(SingleTaskDTO taskToUpdate)
        {
            ViewBag.Title = taskslistTitle;
            return RedirectToAction(nameof(TasksController.TasksList));
        }
    }
}
