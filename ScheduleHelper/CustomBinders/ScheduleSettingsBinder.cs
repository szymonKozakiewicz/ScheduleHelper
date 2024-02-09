using Microsoft.AspNetCore.Mvc.ModelBinding;
using ScheduleHelper.Core.CustomException;
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
            string minWorkingTimeStr = bindingContext.ValueProvider.GetValue("MinWorkTimeBeforeBreakMin").FirstValue;
            string maxWorkingTimeStr = bindingContext.ValueProvider.GetValue("MaxWorkTimeBeforeBreakMin").FirstValue;
            
            ScheduleSettingsDTO? scheduleSettingsDTO = null;
            try
            {
                TimeOnly startTime = TimeOnly.Parse(startTimeStr);
                TimeOnly finishTime = TimeOnly.Parse(finishTimeStr);
                bool hasBreaks = bool.Parse(hasBreaksStr);
                double breakLenght = double.Parse(breakLenghtMinStr);
                double minWorkingTime = double.Parse(minWorkingTimeStr);
                double maxWorkingTime = double.Parse(maxWorkingTimeStr);
                validateMinMaxWorkingTime(minWorkingTime, maxWorkingTime);
                scheduleSettingsDTO = new ScheduleSettingsDTO
                {
                    startTime = startTime,
                    finishTime = finishTime,
                    breakLenghtMin = breakLenght,
                    hasScheduledBreaks = hasBreaks,
                    MinWorkTimeBeforeBreakMin = minWorkingTime,
                    MaxWorkTimeBeforeBreakMin = maxWorkingTime
                };



            }
            catch(MinGreaterThanMax ex)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError("BadRequest", "Minimal working time have to be lower than max working time!");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError("BadRequest","Received wrong data");
                return Task.CompletedTask;
            }





            bindingContext.Result = ModelBindingResult.Success(scheduleSettingsDTO);

            return Task.CompletedTask;

        }

        private static void validateMinMaxWorkingTime(double minWorkingTime, double maxWorkingTime)
        {
            if (minWorkingTime >= maxWorkingTime)
            {
                throw new MinGreaterThanMax();
            }
        }
    }
}
