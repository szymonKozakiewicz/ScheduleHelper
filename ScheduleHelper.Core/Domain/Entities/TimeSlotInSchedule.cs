using ScheduleHelper.Core.Domain.Entities.Helpers;
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
        public Guid Id { get; set; }
        public TimeOnly FinishTime { get; set; }
        public TimeOnly StartTime { get; set; }

        private SingleTask task;
        public bool IsItBreak { get; set; }
        public int OrdinalNumber;

        public TimeSlotInSchedule(TimeOnly finishTime, TimeOnly startTime, SingleTask? task, bool isItBreak, int ordinalNumber)
        {
            FinishTime = finishTime;
            StartTime = startTime;
            this.task = task;
            IsItBreak = isItBreak;
            OrdinalNumber = ordinalNumber;
        }

        public override bool Equals(object? obj)
        {
            return obj is TimeSlotInSchedule schedule &&
                   FinishTime.AreTimesEqualWithTolerance(schedule.FinishTime) &&
                   StartTime.AreTimesEqualWithTolerance(schedule.StartTime) &&
                   EqualityComparer<SingleTask>.Default.Equals(task, schedule.task) &&
                   IsItBreak == schedule.IsItBreak &&
                   OrdinalNumber == schedule.OrdinalNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FinishTime, StartTime, task, IsItBreak, OrdinalNumber);
        }
    }
}
