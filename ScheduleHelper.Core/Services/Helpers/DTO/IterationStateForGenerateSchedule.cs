using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public class IterationStateForGenerateSchedule
    {
        public IterationStateForGenerateSchedule(TimeOnly currentTime,List<TimeSlotInSchedule> fixedTimeSlots)
        {
            _currentOrdinalNumber = 1;
            _currentTime = currentTime;
            TimeOfWorkFromLastBreak = 0;
            FixedTimeSlots = fixedTimeSlots;
            AddedSlots=new List<TimeSlotInSchedule>();
        }

        public IterationStateForGenerateSchedule(TimeOnly currentTime,double timeFromLastBreak, List<TimeSlotInSchedule> fixedTimeSlots)
        {
            _currentOrdinalNumber = 1;
            _currentTime = currentTime;
            TimeOfWorkFromLastBreak = timeFromLastBreak;
            FixedTimeSlots = fixedTimeSlots;
            AddedSlots = new List<TimeSlotInSchedule>();
        }
        private int _currentOrdinalNumber;
        private TimeOnly _currentTime;
        public double TimeOfWorkFromLastBreak;
        public List<TimeSlotInSchedule> FixedTimeSlots;
        public List<TimeSlotInSchedule> AddedSlots;
        public int CurrentOrdinalNumber
        { 
            
            get => _currentOrdinalNumber; 
            private set { _currentOrdinalNumber = value; } 
        }

        public TimeOnly CurrentTime { 
            get => _currentTime; 
            private set => _currentTime = value; 
        
        }

        public void UpdateState(TimeOnly newCurrentTime,double lastTimeSlotDuration)
        {
            CurrentTime = newCurrentTime;
            CurrentOrdinalNumber++;
           
            TimeOfWorkFromLastBreak+= lastTimeSlotDuration;
            
            
        }

        public void UpdateState(TimeSlotInSchedule slot)
        {
            _currentTime = slot.FinishTime;
            if (slot.IsItBreak)
            {
                TimeOfWorkFromLastBreak = 0;
            }
            else
            {
                TimeOfWorkFromLastBreak += slot.getDurationOfSlotInMin();
            }


        }
        public void UpdateStateAfterBreak(TimeOnly newCurrentTime)
        {
            CurrentTime = newCurrentTime;
            CurrentOrdinalNumber++;
           
            
            TimeOfWorkFromLastBreak =0 ;
            

        }

        public void AddFixedTimeSlot(TimeSlotInSchedule timeSlotForBreak)
        {
            FixedTimeSlots.Add(timeSlotForBreak);
        }
    }
}
