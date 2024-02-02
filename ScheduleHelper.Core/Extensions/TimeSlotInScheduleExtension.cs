using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Extensions
{
    public static class TimeSlotInScheduleExtension
    {
        public static TimeSlotInScheduleDTO ConvertToTimeSlotInScheduleDTO(this TimeSlotInSchedule timeSlot)
        {

            string name;
            if(timeSlot.IsItBreak)
            {
                name = "Break";
            }
            else { 
                name=timeSlot.GetTaskName();
            }

            return new TimeSlotInScheduleDTO()
            {
                StartTime = timeSlot.StartTime,
                FinishTime = timeSlot.FinishTime,
                Id = (Guid)timeSlot.Id,
                Name = name,
                OrdinalNumber = timeSlot.OrdinalNumber,
                Status = timeSlot.Status



            };
        }
    }
}
