using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public class IterationStateForGenerateSchedule
    {
        public IterationStateForGenerateSchedule(TimeOnly currentTime)
        {
            _currentOrdinalNumber = 1;
            _currentTime = currentTime;
            TimeOfWorkFromLastBreak = 0;
        }
        private int _currentOrdinalNumber;
        private TimeOnly _currentTime;
        public double TimeOfWorkFromLastBreak;
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


        public void UpdateStateAfterBreak(TimeOnly newCurrentTime)
        {
            CurrentTime = newCurrentTime;
            CurrentOrdinalNumber++;
           
            
            TimeOfWorkFromLastBreak =0 ;
            

        }
    }
}
