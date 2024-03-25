using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public class EntityToDtoConverter
    {
        public static ScheduleSettingsDTO ConvertScheduleSettingsToDto(ScheduleSettings settings)
        {
            var scheduleSettingsDto = new ScheduleSettingsDTO()
            {
                breakLenghtMin = settings.breakDurationMin,
                finishTime = settings.FinishTime,
                hasScheduledBreaks = settings.HasScheduleBreaks,
                MaxWorkTimeBeforeBreakMin = settings.MaxWorkTimeBeforeBreakMin,
                MinWorkTimeBeforeBreakMin = settings.MinWorkTimeBeforeBreakMin,
                startTime = settings.StartTime


            };
            return scheduleSettingsDto;
        }
    }
}
