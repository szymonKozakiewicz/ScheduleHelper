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
            List<TaskGetDTO> tasks=await _taskService.GetTasksList();


            ViewBag.Title = _taskslistTitle;
            return View(tasks);
        }


        [Route(RouteConstants.AddNewTask)]
        [HttpGet]
        public async Task<IActionResult> AddNewTask()
        {
            ViewBag.Title = "New task";
            ViewBag.formHref = RouteConstants.AddNewTask;
            return View("EditTask",new TaskCreateDTO());
        }

        [Route(RouteConstants.AddNewTask)]
        [HttpPost]
        public async Task<IActionResult> AddNewTask(TaskCreateDTO newTask)
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
        public async Task<IActionResult> EditTask(TaskCreateDTO taskToUpdate)
        {
            ViewBag.Title = _taskslistTitle;

           
            return RedirectToAction(nameof(TasksController.TasksList));
        }


        [Route(RouteConstants.DeleteTask)]
        public async Task<IActionResult> DeleteTask(TaskCreateDTO taskToUpdate)
        {
            ViewBag.Title = _taskslistTitle;


            return RedirectToAction(nameof(TasksController.TasksList));
        }
    }
}
