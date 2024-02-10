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
            _scheduleSettings = scheduleSettings;
            await _scheduleRepository.CleanTimeSlotInScheduleTable();
            var tasksList = await _taskRepository.GetTasks();
            await updateScheduleSettings(scheduleSettings);

            await addEachTaskToSchedule(scheduleSettings, tasksList);

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




        private TimeSlotInSchedule makeTimeSlot(IterationStateForGenerateSchedule iterationState,double taskDuration, SingleTask? task)
        {




            double slotDurationMin;
            bool isItBreak = task == null;
            if (isItBreak)
            {
                slotDurationMin = _scheduleSettings.breakLenghtMin;
            }
            else
            {
                slotDurationMin = taskDuration;
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

        public async Task<int> GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus status)
        {
            var slots= await _scheduleRepository.GetTimeSlotsList();
            var slotsWithStatus = slots.Where(slot =>slot.Status==status).ToList();
            double sumOfTimeForSlotsWithStatus = 0;
            foreach(var slot in slotsWithStatus)
            {
                sumOfTimeForSlotsWithStatus += slot.getDurationOfSlotInMin();
            }
            double sumOfTimeForAllSlots = 0;
            foreach (var slot in slots)
            {
                sumOfTimeForAllSlots += slot.getDurationOfSlotInMin();
            }
            if (sumOfTimeForAllSlots > 0)
            {
                int result = (int)Math.Round(100* sumOfTimeForSlotsWithStatus / sumOfTimeForAllSlots);
                return result;
            }

            return 0;
        }

        private async Task addEachTaskToSchedule(ScheduleSettingsDTO scheduleSettings, List<SingleTask> tasksList)
        {
            var iterationState = new IterationStateForGenerateSchedule(scheduleSettings.startTime);
            TimeSlotInSchedule? timeSlotForBreak = null;
            bool shouldBeBreak = false;
            List<SingleTask> tasksListForLoop = new List<SingleTask>();
            foreach (var task in tasksList)
            {
                
                tasksListForLoop.Add(task.Copy());
            }
            for (int i = 0; i < tasksListForLoop.Count; i++)
            {
                var task = tasksListForLoop[i];
                SingleTask orginalTask = tasksList[i];

                bool taskShouldBeSplitted = iterationState.TimeOfWorkFromLastBreak + task.TimeMin > scheduleSettings.MaxWorkTimeBeforeBreakMin;
                if (taskShouldBeSplitted)
                {
                    task=splitTask(scheduleSettings, iterationState.TimeOfWorkFromLastBreak,  task);
                    i--;
                }

               
                TimeSlotInSchedule timeSlotForTask = makeTimeSlot(iterationState,task.TimeMin, orginalTask);
                bool timeSlotNotCreated = timeSlotForTask == null;
                if (timeSlotNotCreated)
                    break;
                iterationState.UpdateState(timeSlotForTask.FinishTime,task.TimeMin);


                
                if (shouldBeBreak)
                    await _scheduleRepository.AddNewTimeSlot(timeSlotForBreak);
                await _scheduleRepository.AddNewTimeSlot(timeSlotForTask);

                shouldBeBreak = iterationState.TimeOfWorkFromLastBreak > scheduleSettings.MinWorkTimeBeforeBreakMin;
                if (scheduleSettings.hasScheduledBreaks && shouldBeBreak)
                {

                    timeSlotForBreak = makeTimeSlot(iterationState,20, null);
                    timeSlotNotCreated = timeSlotForBreak == null;
                    if (timeSlotNotCreated)
                    {
                        break;
                    }
                    iterationState.UpdateStateAfterBreak(timeSlotForBreak.FinishTime);


                }

            }
        }

        private SingleTask splitTask(ScheduleSettingsDTO scheduleSettings, double timeOfWorkFromLastBreak, SingleTask task)
        {
            var avaiableTimeToBreak = scheduleSettings.MaxWorkTimeBeforeBreakMin - timeOfWorkFromLastBreak;
            var newTask = task.Copy();
            newTask.TimeMin = avaiableTimeToBreak;
            task.TimeMin = task.TimeMin - avaiableTimeToBreak;
            return newTask;
            
        }

        private async Task updateScheduleSettings(ScheduleSettingsDTO scheduleSettings)
        {
            var scheduleSettingsForDb = new ScheduleSettings()
            {
                breakDurationMin = scheduleSettings.breakLenghtMin,
                FinishTime = scheduleSettings.finishTime,
                StartTime = scheduleSettings.startTime,
                MaxWorkTimeBeforeBreakMin = scheduleSettings.MaxWorkTimeBeforeBreakMin,
                MinWorkTimeBeforeBreakMin = scheduleSettings.MinWorkTimeBeforeBreakMin,

            };

            await _scheduleRepository.UpdateScheduleSettings(scheduleSettingsForDb);
        }
    }
}
