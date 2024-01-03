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
            

            try
            {
                await _taskService.AddNewTask(newTask);
                Response.StatusCode = (int)HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                //TODO handle expection and make test for it
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
        public async Task<IActionResult> DeleteTask(Guid taskToDeleteId)
        {
            ViewBag.Title = _taskslistTitle;
            try
            {
                //TODO validation of ID?
                await _taskService.RemoveTaskWithId(taskToDeleteId);
                Response.StatusCode = (int)HttpStatusCode.OK;
                ViewBag.OperationStatus = "Task deleted!";
                
            }
            catch (Exception ex)
            {
                ViewBag.OperationStatus = "Operation failed!";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                

            }
            return View("OperationStatus");


        }
    }
}
