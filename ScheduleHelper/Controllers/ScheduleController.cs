using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.Domain.Entities;
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
            var settingsDTO=await _scheduleService.GetScheduleSettings();
            ViewBag.actionForForm = RouteConstants.GenerateScheduleSettings;
            return View("GenerateScheduleSettings", settingsDTO); 
        }

        [Route(RouteConstants.ShowScheduleSettings)]
        [HttpGet]
        public async Task<IActionResult> ShowScheduleSettings()
        {
            
            ScheduleSettingsDTO scheduleSettingsDto = await _scheduleService.GetScheduleSettings();
            ViewBag.actionForForm = RouteConstants.UpdateScheduleSettings;

            return View("UpdateScheduleSettings",scheduleSettingsDto);
        }



        [Route(RouteConstants.UpdateScheduleSettings)]
        [HttpPost]
        public async Task<IActionResult> UpdateScheduleSettings([ModelBinder(typeof(ScheduleSettingsBinder))] ScheduleSettingsDTO scheduleSettingsDto)
        {
            ViewBag.ShowBtnBackToSchedule = false;
            if (ValidationHelper.HasObjectGotValidationErrors(scheduleSettingsDto))
            {
                setFailedStatusAddErrorsToViewBag();
                return View("OperationStatusSchedule");
            }
            try
            {
                await _updateService.UpdateSettings(scheduleSettingsDto);
            }
            catch (Exception ex)
            {
                ViewBag.OperationStatus = ConstantsValues.FailedOperation;
                
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return View("OperationStatusSchedule");
            }
            ViewBag.OperationStatus = "Settings updated!";
            return View("OperationStatusSchedule");
        }



        [Route(RouteConstants.GenerateScheduleSettings)]
        [HttpPost]
        public async Task<IActionResult> GenerateSchedule([ModelBinder(typeof(ScheduleSettingsBinder))] ScheduleSettingsDTO scheduleSettings)
        {


            ViewBag.ShowBtnBackToSchedule = true;
            if (ValidationHelper.HasObjectGotValidationErrors(scheduleSettings))
            {
                setFailedStatusAddErrorsToViewBag();
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
            ViewBag.ShowBtnBackToSchedule = true;
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
        private void setFailedStatusAddErrorsToViewBag()
        {
            ViewBag.OperationStatus = ConstantsValues.FailedOperation;
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errors = ValidationHelper.GetErrorsList(ModelState);
            ViewBag.ErrorsList = errors;
        }



    }
}
