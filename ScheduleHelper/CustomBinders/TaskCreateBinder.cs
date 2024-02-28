using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using ScheduleHelper.Core.CustomException;
using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.UI.CustomBinders
{
    public class TaskCreateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var name= bindingContext.ValueProvider.GetValue("Name").FirstValue;
            var timeStr= bindingContext.ValueProvider.GetValue("Time").FirstValue;
            var startTimeStr= bindingContext.ValueProvider.GetValue("StartTime").FirstValue;
            var hasStartTimeStr = bindingContext.ValueProvider.GetValue("HasStartTime").FirstValue;


            TaskCreateDTO? result = null;
            try
            {
                TimeOnly startTime;
                bool hasStartTime = bool.Parse(hasStartTimeStr);
                
                if (hasStartTime)
                    startTime = TimeOnly.Parse(startTimeStr);
                else
                    startTime = new TimeOnly(8, 0);




                double time = double.Parse(timeStr);

                result = new TaskCreateDTO()
                {
                    HasStartTime = hasStartTime,
                    StartTime = startTime,
                    Time = time,
                    Name=name
                };


            }
            catch (Exception ex)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError("BadRequest", "Received wrong data");
                return Task.CompletedTask;
            }





            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
