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

            await addElasticAndFixedTasksToSchedule(scheduleSettings, tasksList);

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

        private async Task addElasticAndFixedTasksToSchedule(ScheduleSettingsDTO scheduleSettings, List<SingleTask> tasksList)
        {

            List<TimeSlotInSchedule> fixedTimeSlots = await addFixedTasksToScheduleAndGetListOfCreatedSlots(scheduleSettings, tasksList);
            var flexibleTasks = tasksList.FindAll(task => !task.HasStartTime);
            await addElasticTasksToSchedule(scheduleSettings, flexibleTasks, fixedTimeSlots);
        }

        private TimeSlotInSchedule makeTimeSlot(IterationStateForGenerateSchedule iterationState, double taskDuration, SingleTask? task)
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
            bool wasTaskSplitted = false;
            if(!isItBreak && task.HasStartTime)
            {
                wasTaskSplitted = iterationState.CurrentTime>task.StartTime;
            }
            
            if (!isItBreak && task.HasStartTime && !wasTaskSplitted)
            {
                slotStartTime = task.StartTime;
            }

            TimeOnly slotFinishTime = slotStartTime.AddMinutes(slotDurationMin);
            if (slotOverlapsWithExistingFixedTimeSlot(slotStartTime, slotFinishTime, iterationState.FixedTimeSlots))
            {
                TimeSlotInSchedule OverlapsingFixedTimeSlot = getOverlapsingFixedTimeSlot(slotStartTime, slotFinishTime, iterationState.FixedTimeSlots);
                slotFinishTime = OverlapsingFixedTimeSlot.StartTime;
                
  
            }

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

        private TimeSlotInSchedule getOverlapsingFixedTimeSlot(TimeOnly slotStartTime, TimeOnly slotFinishTime, List<TimeSlotInSchedule> fixedTimeSlots)
        {

            foreach (var fixedTimeSlot in fixedTimeSlots)
            {
                if (slotOverlapsWithExistingFixedTimeSlot(slotStartTime,slotFinishTime,fixedTimeSlots))
                    return fixedTimeSlot;
            }
            return null;
        }

        private bool slotOverlapsWithExistingFixedTimeSlot(TimeOnly slotStartTime, TimeOnly slotFinishTime, List<TimeSlotInSchedule> fixedTimeSlots)
        {
            TimeSlotInSchedule tempTimeSlot = new TimeSlotInSchedule()
            {
                StartTime = slotStartTime,
                FinishTime = slotFinishTime,
            };
            foreach (var fixedTimeSlot in fixedTimeSlots)
            {
                if (fixedTimeSlot.TestedTimeSlotIsInsideOfTimeSlot(tempTimeSlot) || tempTimeSlot.TestedTimeSlotIsInsideOfTimeSlot(fixedTimeSlot))
                    return true;
            }
            return false;

        }


        private async Task<List<TimeSlotInSchedule>> addFixedTasksToScheduleAndGetListOfCreatedSlots(ScheduleSettingsDTO scheduleSettings, List<SingleTask> tasksList)
        {
            List<SingleTask> fixedTasksList = tasksList.FindAll(task => task.HasStartTime).ToList();
            
            List<TimeSlotInSchedule>fixedTimeSlots=await addEachTaskToScheduleAndReturnListOfCreatedSlots(scheduleSettings,fixedTasksList, new List<TimeSlotInSchedule>());
            return fixedTimeSlots;

        }

        private async Task<List<TimeSlotInSchedule>> addEachTaskToScheduleAndReturnListOfCreatedSlots(ScheduleSettingsDTO scheduleSettings, List<SingleTask> tasksList,List<TimeSlotInSchedule>fixedTimeSlots)
        {
            var iterationState = new IterationStateForGenerateSchedule(scheduleSettings.startTime,fixedTimeSlots);
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
                    task = splitTask(scheduleSettings, iterationState.TimeOfWorkFromLastBreak, task);
                    i--;
                }


                TimeSlotInSchedule timeSlotForTask = makeTimeSlot(iterationState, task.TimeMin, orginalTask);
                bool timeSlotNotCreated = timeSlotForTask == null;
                if (timeSlotNotCreated)
                {
                    break;
                }
                bool taskSplittedByFixedTimeSlot = timeSlotForTask.getDurationOfSlotInMin() < task.TimeMin;
                if (taskSplittedByFixedTimeSlot)
                {
                    i = setTaskToBeContinuedAfterFixedSlot(tasksListForLoop, i, task, timeSlotForTask);
                }
                if (taskSplittedByFixedTimeSlot)
                {

                    updateIterationStateAfterFixedTask(fixedTimeSlots, iterationState, timeSlotForTask.FinishTime);
                }
                else
                {
                    iterationState.UpdateState(timeSlotForTask.FinishTime, task.TimeMin);
                }




                if (shouldBeBreak)
                {
                    bool timeSlotAdded = await addNewTimeSlot(timeSlotForBreak);
                    if (timeSlotAdded)
                        iterationState.AddedSlots.Add(timeSlotForBreak);
                    if (task.HasStartTime)
                    {
                        iterationState.AddFixedTimeSlot(timeSlotForBreak);
                    }
                }
                if(await addNewTimeSlot(timeSlotForTask))
                    iterationState.AddedSlots.Add(timeSlotForTask);
                if (task.HasStartTime)
                {
                    iterationState.AddFixedTimeSlot(timeSlotForTask);
                }

                shouldBeBreak = isTimeForBreak(scheduleSettings, iterationState);
                if (scheduleSettings.hasScheduledBreaks && shouldBeBreak)
                {
                    
                    timeSlotForBreak =  await makeBreakAndUpdateIterationState(scheduleSettings, fixedTimeSlots, iterationState);
                    timeSlotNotCreated = timeSlotForBreak == null;
                    if (timeSlotNotCreated)
                    {
                        break;
                    }
                }

            }
            return iterationState.AddedSlots;
        }

        private async Task<bool> addNewTimeSlot(TimeSlotInSchedule? timeSlot)
        {
            if (timeSlot.getDurationOfSlotInMin() > 0.1)
            {
                await _scheduleRepository.AddNewTimeSlot(timeSlot);
                return true;
            }
            return false;
        }

        private async Task<TimeSlotInSchedule> makeBreakAndUpdateIterationState(ScheduleSettingsDTO scheduleSettings, List<TimeSlotInSchedule> fixedTimeSlots, IterationStateForGenerateSchedule iterationState)
        {
            TimeSlotInSchedule? timeSlotForBreak = makeTimeSlot(iterationState, scheduleSettings.breakLenghtMin, null);
            var timeSlotNotCreated = timeSlotForBreak == null;
            if (timeSlotNotCreated)
            {
                return null;
            }
            bool breakSplittedByFixedTask = timeSlotForBreak.getDurationOfSlotInMin() < scheduleSettings.breakLenghtMin;
            if (breakSplittedByFixedTask)
            {
                updateIterationStateAfterFixedTask(fixedTimeSlots, iterationState, timeSlotForBreak.FinishTime);

                if (isTimeForBreak(scheduleSettings, iterationState))
                {


                    bool timeSlotAdded = await addNewTimeSlot(timeSlotForBreak);
                    if (timeSlotAdded)
                        iterationState.AddedSlots.Add(timeSlotForBreak);
                    timeSlotForBreak = await makeBreakAndUpdateIterationState(scheduleSettings, fixedTimeSlots, iterationState);

                    timeSlotNotCreated = timeSlotForBreak == null;
                    if (timeSlotNotCreated)
                    {
                        return null;
                    }
                }
            }
            else
            {
                iterationState.UpdateStateAfterBreak(timeSlotForBreak.FinishTime);
            }

            return timeSlotForBreak;
        }

        private static bool isTimeForBreak(ScheduleSettingsDTO scheduleSettings, IterationStateForGenerateSchedule iterationState)
        {
            return iterationState.TimeOfWorkFromLastBreak > scheduleSettings.MinWorkTimeBeforeBreakMin;
        }

        private void updateIterationStateAfterFixedTask(List<TimeSlotInSchedule> fixedTimeSlots, IterationStateForGenerateSchedule iterationState, TimeOnly lastTimeSlotFinishTime)
        {
            TimeOnly fixedTaskFinishTime = getfixedTaskFinishTime(fixedTimeSlots, lastTimeSlotFinishTime);
            TimeOnly? fixedTaskLastBreakFinish = getfixedTaskLastBreakFinish(fixedTimeSlots, lastTimeSlotFinishTime);
            TimeOnly timeFormLastBreakOrStartOfFixedTask = lastTimeSlotFinishTime;
            if (fixedTaskLastBreakFinish != null)
            {
                timeFormLastBreakOrStartOfFixedTask = (TimeOnly)fixedTaskLastBreakFinish;
                iterationState.UpdateStateAfterBreak((TimeOnly)fixedTaskLastBreakFinish);
            }
            double durationFixedTaskPlusLastTimeSlot = (fixedTaskFinishTime - timeFormLastBreakOrStartOfFixedTask).TotalMinutes;

            iterationState.UpdateState(fixedTaskFinishTime, durationFixedTaskPlusLastTimeSlot);
        }

        private TimeOnly? getfixedTaskLastBreakFinish(List<TimeSlotInSchedule> fixedTimeSlots, TimeOnly finishTimeOfLastTask)
        {

            TimeSlotInSchedule lastOverlapsingTimeSlot = getLastOverlapsingFixedTimeSlot(fixedTimeSlots,  finishTimeOfLastTask);

            TimeOnly lastBreakFinishTime = lastOverlapsingTimeSlot.StartTime;
            return (TimeOnly)lastBreakFinishTime;
        }

        private TimeOnly getfixedTaskFinishTime(List<TimeSlotInSchedule> fixedTimeSlots, TimeOnly finishTimeOfLastTask)
        {
            TimeOnly? fixedTaskFinishTime;
            TimeSlotInSchedule lastOverlapsingTimeSlot = getLastOverlapsingFixedTimeSlot(fixedTimeSlots, finishTimeOfLastTask);

            fixedTaskFinishTime = lastOverlapsingTimeSlot.FinishTime;
            return (TimeOnly)fixedTaskFinishTime;
        }

        private TimeSlotInSchedule getLastOverlapsingFixedTimeSlot(List<TimeSlotInSchedule> fixedTimeSlots, TimeOnly finishTimeOfLastTask)
        {
            TimeOnly overlapsingFinish = finishTimeOfLastTask.AddMinutes(1);
            TimeSlotInSchedule overlapsingFixedTimeSlot = getOverlapsingFixedTimeSlot(finishTimeOfLastTask, overlapsingFinish, fixedTimeSlots);
            List<TimeSlotInSchedule> overlapsingFixedTimeSlots = fixedTimeSlots.FindAll(slot => !slot.IsItBreak && slot.task.Id == overlapsingFixedTimeSlot.task.Id).ToList();
            TimeSlotInSchedule lastOverlapsingTimeSlot = overlapsingFixedTimeSlots[0];
            foreach (var slot in overlapsingFixedTimeSlots)
            {
                if (slot.FinishTime > lastOverlapsingTimeSlot.FinishTime)
                    lastOverlapsingTimeSlot = slot;
            }

            return lastOverlapsingTimeSlot;
        }

        private static int setTaskToBeContinuedAfterFixedSlot(List<SingleTask> tasksListForLoop, int i, SingleTask task, TimeSlotInSchedule timeSlotForTask)
        {
            bool taskWasSplittedBefore = tasksListForLoop[i].Id != task.Id;
            if (taskWasSplittedBefore)
            {
                double resultDuration = timeSlotForTask.getDurationOfSlotInMin();
                tasksListForLoop[i + 1].TimeMin += task.TimeMin - resultDuration;

            }
            else
            {
                double resultDuration = timeSlotForTask.getDurationOfSlotInMin();
                tasksListForLoop[i].TimeMin -= resultDuration;
                i--;
            }

            return i;
        }

        private async Task addElasticTasksToSchedule(ScheduleSettingsDTO scheduleSettings, List<SingleTask> tasksList, List<TimeSlotInSchedule> fixedTimeSlots)
        {
            await addEachTaskToScheduleAndReturnListOfCreatedSlots(scheduleSettings, tasksList, fixedTimeSlots);
        }

        private SingleTask splitTask(ScheduleSettingsDTO scheduleSettings, double timeOfWorkFromLastBreak, SingleTask task)
        {
            var avaiableTimeUntilBreak = Math.Max(scheduleSettings.MaxWorkTimeBeforeBreakMin - timeOfWorkFromLastBreak,0);
            var newTask = task.Copy();
            newTask.TimeMin = avaiableTimeUntilBreak;
            task.TimeMin = task.TimeMin - avaiableTimeUntilBreak;
            
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
