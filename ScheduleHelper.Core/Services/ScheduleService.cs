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
using ScheduleHelper.Core.Extensions;
using ScheduleHelper.Core.Domain.Entities.Helpers;
using ScheduleHelper.Core.Domain.Entities.Enums;

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
            var scheduleSettingsForDb = new ScheduleSettings()
            {
                breakDurationMin=scheduleSettings.breakLenghtMin,
                FinishTime=scheduleSettings.finishTime

            };
            
            await _scheduleRepository.UpdateScheduleSettings(scheduleSettingsForDb);

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

    

 


        public async Task<List<TimeSlotInScheduleDTO>> GetTimeSlotsList()
        {
            var timeSlotsList = await _scheduleRepository.GetTimeSlotsList();
            var timeSlotsDTOList = timeSlotsList.Select(
                timeSlot => timeSlot.ConvertToTimeSlotInScheduleDTO())
                .ToList();
            timeSlotsDTOList.Sort((slot1, slot2) => slot1.StartTime.CompareTo(slot2.StartTime));
            return timeSlotsDTOList;

        }

        public async Task<List<TaskForEditListDTO>> GetTasksNotSetInSchedule()
        {
            var listOfNotSettasks= await _scheduleRepository.GetTasksNotSetInSchedule();
            return listOfNotSettasks.Select(task=>SingleTaskConvertHelper.covertSingleTaskToTaskForEditListDTO(task))
                .ToList();
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
                CompareTo(_scheduleSettings.finishTime) < 0;
            if (enoughTimeForTask)
            {
                var newTimeSlot = new TimeSlotInScheduleBuilder()
                    .SetFinishTime(slotFinishTime)
                    .SetStartTime(slotStartTime)
                    .SetTimeSlotStatus(TimeSlotStatus.Active)
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



    }
}
