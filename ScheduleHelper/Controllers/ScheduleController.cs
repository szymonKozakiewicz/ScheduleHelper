using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Constants;
using ScheduleHelper.UI.CustomBinders;
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
            if(slots.Count>0)
            {
                ViewBag.ShareOfCanceledSlots = await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Canceled);
                ViewBag.ShareOfFinishedSlots = await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Finished);
                ViewBag.ShareOfActiveSlots = await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Active);
            }
           
            ViewBag.tasksNotInSchedule = tasksNotInSchedule;
            
            return View("Schedule",slots);
        }

        [Route(RouteConstants.GenerateScheduleSettings)]
        [HttpGet]
        public async Task<IActionResult> GenerateScheduleSettings()
        {

            ViewBag.Title = "Schedule settings";
            return View("GenerateScheduleSettings", new ScheduleSettingsDTO()

            {
                hasScheduledBreaks = true,
                breakLenghtMin = 20,
                startTime = new TimeOnly(8,0),
                finishTime= new TimeOnly(22,0),
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin=45

            }); 
        }

        [Route(RouteConstants.ShowScheduleSettings)]
        [HttpGet]
        public async Task<IActionResult> ShowScheduleSettings()
        {
            
            ScheduleSettingsDTO scheduleSettingsDto = await _scheduleService.GetScheduleSettings();
            

            return View("UpdateScheduleSettings",scheduleSettingsDto);
        }

        [Route(RouteConstants.ShowScheduleSettings)]
        [HttpPost]
        public async Task<IActionResult> UpdateScheduleSettings(ScheduleSettingsDTO scheduleSettingsDto)
        {

            ViewBag.OperationStatus = "Settings updated!";
            return View("OperationStatusSchedule");
        }

        [Route(RouteConstants.GenerateScheduleSettings)]
        [HttpPost]
        public async Task<IActionResult> GenerateScheduleSettings([ModelBinder(typeof(ScheduleSettingsBinder))] ScheduleSettingsDTO scheduleSettings)
        {

            

            if(ValidationHelper.HasObjectGotValidationErrors(scheduleSettings))
            {
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = ValidationHelper.GetErrorsList(ModelState);
                ViewBag.ErrorsList = errors;
                return View("OperationStatusSchedule");

            }
            try
            {
                await _scheduleService.GenerateSchedule(scheduleSettings);
            }
            catch (Exception ex) {
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
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

                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
                Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                return View("OperationStatusSchedule");
            }
            
            ViewBag.OperationStatus = "Time slot finished!";
            return View("OperationStatusSchedule");
        }



    }
}
