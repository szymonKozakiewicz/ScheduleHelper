﻿using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Helpers;
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
            List<TaskForEditListDTO> tasks=await _taskService.GetTasksList();


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

            int minimalTimeValue = 0;
            if (ValidationHelper.HasObjectGotValidationErrors(newTask))
            {
                ViewBag.OperationStatus = "Operation Failed!";
                var errors = ValidationHelper.GetErrorsList(ModelState);
                ViewBag.ErrorsList = errors;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("OperationStatus");
            }

            try
            {
                await _taskService.AddNewTask(newTask);
                Response.StatusCode = (int)HttpStatusCode.Created;
                ViewBag.OperationStatus = "Task created!";
            }
            catch (Exception ex)
            {
                ViewBag.OperationStatus = "Operation failed!";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            

            
            return View("OperationStatus");
        }

        private void addErrorsToViewBag()
        {
            List<string> errors = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);

                }

            }
            
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
