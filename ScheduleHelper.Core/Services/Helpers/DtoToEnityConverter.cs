using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public class DtoToEnityConverter
    {
        public static SingleTask covertSingleTaskDtoToSingleTask(TaskCreateDTO taskDTO)
        {
            return new SingleTask
            {
                Name = taskDTO.Name,
                TimeMin = taskDTO.Time,
                HasStartTime = taskDTO.HasStartTime,
                StartTime = taskDTO.StartTime
            };
     
        }

        public static TaskCreateDTO covertSingleTaskToTaskCreateDTO(SingleTask task)
        {
            return new TaskCreateDTO()
            {
                Name = task.Name,
                Time = task.TimeMin
            };
        }

        public static TaskForEditListDTO covertSingleTaskToTaskForEditListDTO(SingleTask task)
        {
            return new TaskForEditListDTO()
            {
                Name = task.Name,
                Time = task.TimeMin,
                Id=task.Id
            };
        }

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
