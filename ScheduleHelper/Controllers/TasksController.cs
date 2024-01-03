using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using System.Net;

namespace ScheduleHelper.UI.Controllers
{
    public class TasksController : Controller
    {

        string _taskslistTitle = "Tasks list";
        ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Route(RouteConstants.ShowTasksList)]
        public async Task<IActionResult> TasksList()
        {
            List<SingleTaskDTO> tasks=await _taskService.GetTasksList();


            ViewBag.Title = _taskslistTitle;
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
            ViewBag.Title = "Operation status";
            Response.StatusCode = (int)HttpStatusCode.Created;

            try
            {
                await _taskService.AddNewTask(newTask);
            }
            catch (Exception ex)
            {
                //TODO handle expection and ,ake test for it
            }


            ViewBag.OperationStatus = "Task created!";
            return View("OperationStatus");
        }


        [Route(RouteConstants.UpdateTask)]
        public async Task<IActionResult> EditTask(SingleTaskDTO taskToUpdate)
        {
            ViewBag.Title = _taskslistTitle;

           
            return RedirectToAction(nameof(TasksController.TasksList));
        }
    }
}
