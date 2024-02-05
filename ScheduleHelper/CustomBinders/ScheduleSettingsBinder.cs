using Microsoft.AspNetCore.Mvc.ModelBinding;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;

namespace ScheduleHelper.UI.CustomBinders
{
    public class ScheduleSettingsBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string startTimeStr = bindingContext.ValueProvider.GetValue("startTime").FirstValue;
            string finishTimeStr = bindingContext.ValueProvider.GetValue("finishTime").FirstValue;
            string hasBreaksStr = bindingContext.ValueProvider.GetValue("hasScheduledBreaks").FirstValue;
            string breakLenghtMinStr = bindingContext.ValueProvider.GetValue("breakLenghtMin").FirstValue;
            
            ScheduleSettingsDTO? timeSlotInScheduleDTO = null;
            try
            {
                TimeOnly startTime = TimeOnly.Parse(startTimeStr);
                TimeOnly finishTime = TimeOnly.Parse(finishTimeStr);
                bool hasBreaks = bool.Parse(hasBreaksStr);
                double breakLenght = double.Parse(breakLenghtMinStr);
                timeSlotInScheduleDTO = new ScheduleSettingsDTO
                {
                    startTime = startTime,
                    finishTime = finishTime,
                    breakLenghtMin = breakLenght,
                    hasScheduledBreaks = hasBreaks
                };

                

            }catch (Exception ex)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError("BadRequest","Received wrong data");
                return Task.CompletedTask;
            }





            bindingContext.Result = ModelBindingResult.Success(timeSlotInScheduleDTO);

            return Task.CompletedTask;

        }
    }
}
