using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.UI.Controllers
{
    public class TasksController : Controller
    {

        string taskslistTitle = "Tasks list";
        
        [Route("/")]
        public async Task<IActionResult> TasksList()
        {
            ViewBag.Title = taskslistTitle;
            return View();
        }


        [Route("/addNewTask")]
        public async Task<IActionResult> AddNewTask()
        {
            ViewBag.Title = "New task";
            return View("EditTask",new SingleTaskDTO());
        }


        [Route("/EditTask")]
        public async Task<IActionResult> EditTask(SingleTaskDTO taskToEdit)
        {
            ViewBag.Title = taskslistTitle;
            return RedirectToAction("TasksList");
        }
    }
}
