using Microsoft.AspNetCore.Mvc.ModelBinding;
using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.UI.CustomBinders
{
    public class TaskUpdateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var name = bindingContext.ValueProvider.GetValue("Name").FirstValue;
            var timeStr = bindingContext.ValueProvider.GetValue("Time").FirstValue;
            var startTimeStr = bindingContext.ValueProvider.GetValue("StartTime").FirstValue;
            var hasStartTimeStr = bindingContext.ValueProvider.GetValue("HasStartTime").FirstValue;
            var idStr = bindingContext.ValueProvider.GetValue("Id").FirstValue; 

            TaskCreateDTO? result = null;
            try
            {
                TimeOnly startTime;
                Guid id;
                bool hasStartTime = bool.Parse(hasStartTimeStr);

                if (hasStartTime)
                    startTime = TimeOnly.Parse(startTimeStr);
                else
                    startTime = new TimeOnly(8, 0);
                id = Guid.Parse(idStr);



                double time = double.Parse(timeStr);

                result = new TaskCreateDTO()
                {
                    HasStartTime = hasStartTime,
                    StartTime = startTime,
                    Time = time,
                    Name = name,
                    Id = id,
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
