using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities.Helpers;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities
{
    public class TimeSlotInSchedule
    {
        
        [Key]
        public Guid? Id { get; set; }
        public TimeOnly FinishTime { get; set; }
        public TimeOnly StartTime { get; set; }

        public SingleTask? task { get; set; }
        public bool IsItBreak { get; set; }
        public int OrdinalNumber { get; set; }
        public TimeSlotStatus Status { get; set; }

        public TimeSlotInSchedule(TimeOnly finishTime, TimeOnly startTime, SingleTask? task, bool isItBreak, int ordinalNumber,Guid? id)
        {
            FinishTime = finishTime;
            StartTime = startTime;
            this.task = task;
            IsItBreak = isItBreak;
            OrdinalNumber = ordinalNumber;
            Id = id;
        }

        public TimeSlotInSchedule()
        {
            
        }


        public String GetTaskName()
        {
            return task.Name;
        }
       

        public double getDurationOfSlotInMin()
        {
            return (FinishTime - StartTime).TotalMinutes;
        }

        public void updateStartAndFinishWithDely(double delyMin)
        {
            FinishTime=FinishTime.AddMinutes(delyMin);
            StartTime=StartTime.AddMinutes(delyMin);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimeSlotInSchedule schedule &&
                   FinishTime.Equals(schedule.FinishTime) &&
                   StartTime.Equals(schedule.StartTime) &&
                   EqualityComparer<SingleTask?>.Default.Equals(task, schedule.task) &&
                   IsItBreak == schedule.IsItBreak &&
                   Status == schedule.Status;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FinishTime, StartTime, task, IsItBreak, Status);
        }

        public bool TestedTimeSlotIsInsideOfTimeSlot(TimeSlotInSchedule testedTimeslot)
        {
            return testedTimeslot.StartTime.IsBetweenOpenBrackets(StartTime, FinishTime) || testedTimeslot.FinishTime.IsBetweenOpenBrackets(StartTime, FinishTime);
        }
    }
}
