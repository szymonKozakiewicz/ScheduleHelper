﻿using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Constants;
using ScheduleHelper.UI.CustomBinders;
using ScheduleHelper.UI.Helpers;
using System.Net;

namespace ScheduleHelper.UI.Controllers
{
    public class TasksController : Controller
    {

        string _taskslistTitle = "Tasks list";
        ITaskService _taskService;
        IScheduleService _scheduleService;

        public TasksController(ITaskService taskService, IScheduleService scheduleService)
        {
            _taskService = taskService;
            _scheduleService = scheduleService;
        }

        [Route(RouteConstants.ShowTasksList)]
        public async Task<IActionResult> TasksList()
        {
            List<TaskCreateDTO> tasks=await _taskService.GetTasksList();

            double freeTime=await _scheduleService.CalculateAvaiableFreeTimeBasedOnExistingTasks();
            ViewBag.EstimatedFreeTime = freeTime;
            ViewBag.Title = _taskslistTitle;
            return View(tasks);
        }


        [Route(RouteConstants.AddNewTask)]
        [HttpGet]
        public async Task<IActionResult> AddNewTask()
        {
            ViewBag.Title = "New task";
            ViewBag.formHref = RouteConstants.AddNewTask;
            var newTask = new TaskCreateDTO()
            {
                HasStartTime = false,
                StartTime = new TimeOnly(8, 0),
                Time=60
            };
            return View("EditTask",newTask);
        }

        [Route(RouteConstants.AddNewTask)]
        [HttpPost]
        public async Task<IActionResult> AddNewTask([ModelBinder(typeof(TaskCreateBinder))]TaskCreateDTO newTask)
        {
            ViewBag.Title = "Operation status";

            int minimalTimeValue = 0;
            if (ValidationHelper.HasObjectGotValidationErrors(newTask))
            {
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
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
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
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
        [HttpGet]
        public async Task<IActionResult> EditTask(Guid taskToEditId)
        {
            ViewBag.Title = _taskslistTitle;
            ViewBag.formHref = RouteConstants.UpdateTask;
            TaskCreateDTO taskToEdit;
            try
            {
                taskToEdit = await _taskService.GetTaskCreateDTOWithId(taskToEditId);
            }
            catch(Exception ex) {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("OperationStatus");
            }

            return View("EditTask", taskToEdit);
        }


        [Route(RouteConstants.UpdateTask)]
        [HttpPost]
        public async Task<IActionResult> EditTask([ModelBinder(typeof(TaskUpdateBinder))] TaskCreateDTO newVersionOfTask)
        {
            ViewBag.Title = _taskslistTitle;
            ViewBag.formHref = RouteConstants.UpdateTask;
            TaskCreateDTO taskToEdit;
            if (ValidationHelper.HasObjectGotValidationErrors(newVersionOfTask))
            {
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
                var errors = ValidationHelper.GetErrorsList(ModelState);
                ViewBag.ErrorsList = errors;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("OperationStatus");
            }

            try
            {
                await _taskService.UpdateTask(newVersionOfTask);
                Response.StatusCode = (int)HttpStatusCode.OK;
                ViewBag.OperationStatus = "Task updated!";
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ViewBag.OperationStatus = ex.Message;
                return View("OperationStatus");
            }

            return View("OperationStatus");
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
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                

            }
            return View("OperationStatus");


        }


    }
}
