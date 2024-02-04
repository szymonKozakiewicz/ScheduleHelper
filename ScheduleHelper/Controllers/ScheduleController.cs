using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Helpers;
using System.Buffers;
using System.Net;

namespace ScheduleHelper.UI.Controllers
{
    public class ScheduleController : Controller
    {


        IScheduleService _scheduleService;
        IScheduleUpdateService _updateService;


        public ScheduleController(IScheduleService scheduleService, IScheduleUpdateService updateService)
        {
            _scheduleService = scheduleService;
            _updateService = updateService;
        }

        [Route(RouteConstants.ShowSchedule)]
        public async Task<IActionResult> ShowSchedule()
        {

            ViewBag.Title = "Schedule";
            List<TimeSlotInScheduleDTO> slots = await _scheduleService.GetTimeSlotsList();
            List<TaskForEditListDTO> tasksNotInSchedule = await _scheduleService.GetTasksNotSetInSchedule();
            ViewBag.tasksNotInSchedule = tasksNotInSchedule;
            
            return View("Schedule",slots);
        }

        [Route(RouteConstants.GenerateScheduleSettings)]
        [HttpGet]
        public async Task<IActionResult> GenerateScheduleSettings()
        {

            ViewBag.Title = "Schedule settings";
            return View("GenerateScheduleSettings", new ScheduleSettingsPostDTO());
        }


        [Route(RouteConstants.GenerateScheduleSettings)]
        [HttpPost]
        public async Task<IActionResult> GenerateScheduleSettings(ScheduleSettingsPostDTO scheduleSettings)
        {

            

            if(ValidationHelper.HasObjectGotValidationErrors(scheduleSettings))
            {
                ViewBag.OperationStatus = "Operation failed!";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = ValidationHelper.GetErrorsList(ModelState);
                ViewBag.ErrorsList = errors;
                return View("OperationStatusSchedule");

            }
            try
            {
                await _scheduleService.GenerateSchedule(scheduleSettings.ConvertToScheduleSettingsDTO());
            }
            catch (Exception ex) {
                ViewBag.OperationStatus = "Operation failed!";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return View("OperationStatusSchedule");
            }


            Response.StatusCode=(int)HttpStatusCode.Created;
            ViewBag.OperationStatus = "Schedule created!";
            return View("OperationStatusSchedule");
        }

        [Route(RouteConstants.FinishTimeSlot)]
        [HttpGet]
        public async Task<IActionResult> GetTimeSlotFinalisePage(FinaliseSlotDTO model)
        {
            
            return View("SlotFinalise",model);
        }

        [Route(RouteConstants.FinishTimeSlot)]
        [HttpPost]
        public async Task<IActionResult> FinaliseTimeSlot(FinaliseSlotDTO model)
        {
            try
            {
                await _updateService.FinaliseTimeSlot(model);
            }catch (Exception ex) {

                ViewBag.OperationStatus = ex.Message;
                Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                return View("OperationStatusSchedule");
            }
            
            ViewBag.OperationStatus = "Time slot finished!";
            return View("OperationStatusSchedule");
        }



    }
}
