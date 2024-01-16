using Microsoft.AspNetCore.Mvc;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using System.Net;

namespace ScheduleHelper.UI.Controllers
{
    public class ScheduleController : Controller
    {


        IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [Route(RouteConstants.ShowSchedule)]
        public async Task<IActionResult> ShowSchedule()
        {

            ViewBag.Title = "Schedule";
            List<TimeSlotInScheduleDTO> tasks = await _scheduleService.GetTimeSlotsList();
            return View("Schedule",tasks);
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
                return View("OperationStatus");

            }
            try
            {
                await _scheduleService.GenerateSchedule(scheduleSettings.ConvertToScheduleSettingsDTO());
            }
            catch (Exception ex) {
                ViewBag.OperationStatus = "Operation failed!";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return View("OperationStatus");
            }


            Response.StatusCode=(int)HttpStatusCode.Created;
            ViewBag.OperationStatus = "Schedule created!";
            return View("OperationStatus");
        }

    }
}
