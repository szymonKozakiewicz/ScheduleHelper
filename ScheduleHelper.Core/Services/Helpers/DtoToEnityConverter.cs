﻿using ScheduleHelper.Core.Domain.Entities;
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
                StartTime = taskDTO.StartTime,
                Id = taskDTO.Id
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



    }
}
