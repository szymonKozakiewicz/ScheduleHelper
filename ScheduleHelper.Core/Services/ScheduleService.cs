using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleHelper.Core.Services.Helpers;
using ScheduleHelper.Core.Domain.Entities.Builders;

namespace ScheduleHelper.Core.Services
{
    public class ScheduleService : IScheduleService
    {
        ITaskRespository _taskRepository;
        IScheduleRepository _scheduleRepository;
        ScheduleSettingsDTO _scheduleSettings;
        public ScheduleService(ITaskRespository taskRepository, IScheduleRepository scheduleRepository)
        {

            _taskRepository = taskRepository;
            _scheduleRepository = scheduleRepository;

        }
        public async Task GenerateSchedule(ScheduleSettingsDTO scheduleSettings)
        {
            _scheduleSettings= scheduleSettings;
            await _scheduleRepository.CleanTimeSlotInScheduleTable();
            var tasksList= await _taskRepository.GetTasks();

            var iterationState = new IterationStateForGenerateSchedule(scheduleSettings.startTime);
            TimeSlotInSchedule? timeSlotForBreak= null;
            for (int i=0;i<tasksList.Count;i++)
            {
                var task = tasksList[i];
                TimeSlotInSchedule timeSlotForTask = makeTimeSlot(iterationState, task);
                bool timeSlotNotCreated = timeSlotForTask == null;
                if (timeSlotNotCreated)
                    break;
                iterationState.UpdateState(timeSlotForTask.FinishTime);
                

                bool NotFirstIteration = timeSlotForBreak != null;
                if (NotFirstIteration)
                    await _scheduleRepository.AddNewTimeSlot(timeSlotForBreak);
                await _scheduleRepository.AddNewTimeSlot(timeSlotForTask); 
                

                if (scheduleSettings.hasScheduledBreaks)
                {

                    timeSlotForBreak = makeTimeSlot(iterationState, null);
                    timeSlotNotCreated = timeSlotForBreak == null;
                    if (timeSlotNotCreated)
                    {
                        break;
                    }
                    iterationState.UpdateState(timeSlotForBreak.FinishTime);
                    
          
                }

            }

        }

    

        private TimeSlotInSchedule makeTimeSlot(IterationStateForGenerateSchedule iterationState, SingleTask? task)
        {
            
           
            
            
            double slotDurationMin;
            bool isItBreak = task == null;
            if (isItBreak)
            {
               slotDurationMin = _scheduleSettings.breakLenghtMin;
            }
            else
            {
                slotDurationMin = task.TimeMin;
            }



            
            var currentTime = iterationState.CurrentTime;
            TimeOnly slotStartTime = currentTime;
            TimeOnly slotFinishTime = currentTime.AddMinutes(slotDurationMin);
            bool enoughTimeForTask = slotFinishTime.
                CompareTo(_scheduleSettings.finishTime)<0;
            if (enoughTimeForTask)
            {
                var newTimeSlot = new TimeSlotInScheduleBuilder()
                    .SetFinishTime(slotFinishTime)
                    .SetStartTime(slotStartTime)
                    .SetTask(task)
                    .SetIsItBreak(isItBreak)
                    .SetOrdinalNumber(iterationState.CurrentOrdinalNumber)
                    .Build();
                return newTimeSlot;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<TimeSlotInScheduleDTO>> GetTasksForSchedule()
        {
            var tasksList = await _taskRepository.GetTasks();
            var tasksDTOList = tasksList.Select(
                task => SingleTaskConvertHelper.covertSingleTaskToTimeSlotInScheduleDTO(task))
                .ToList();
            return tasksDTOList;

        }
    }
}
