using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public class SingleTaskConvertHelper
    {
        public static SingleTask covertSingleTaskDtoToSingleTask(SingleTaskDTO taskDTO)
        {
            return new SingleTask(taskDTO.Name, taskDTO.Time);
     
        }

        public static SingleTaskDTO covertSingleTaskToSingleTaskDTO(SingleTask task)
        {
            return new SingleTaskDTO()
            {
                Name = task.Name,
                Time = task.TimeMin
            };
        }
    }
}
