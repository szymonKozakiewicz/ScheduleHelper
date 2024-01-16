using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities.Builders
{
    public class TimeSlotInScheduleBuilder
    {
        private Guid? _id;
        private TimeOnly _finishTime;
        private TimeOnly _startTime;
        private SingleTask? _task;
        private bool _isItBreak;
        private int _ordinalNumber;

        public TimeSlotInScheduleBuilder()
        {
            _id = null;
            // Inicjalizacja domyślnych wartości, jeśli to konieczne
        }

        public TimeSlotInScheduleBuilder SetId(Guid id)
        {
            _id = id;
            return this;
        }

        public TimeSlotInScheduleBuilder SetFinishTime(TimeOnly finishTime)
        {
            _finishTime = finishTime;
            return this;
        }

        public TimeSlotInScheduleBuilder SetStartTime(TimeOnly startTime)
        {
            _startTime = startTime;
            return this;
        }

        public TimeSlotInScheduleBuilder SetTask(SingleTask? task)
        {
            _task = task;
            return this;
        }

        public TimeSlotInScheduleBuilder SetIsItBreak(bool isItBreak)
        {
            _isItBreak = isItBreak;
            return this;
        }

        public TimeSlotInScheduleBuilder SetOrdinalNumber(int ordinalNumber)
        {
            _ordinalNumber = ordinalNumber;
            return this;
        }

        public TimeSlotInSchedule Build()
        {
            return new TimeSlotInSchedule(_finishTime,_startTime,_task,_isItBreak, _ordinalNumber,_id);
        }
    }
}
