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
using static ScheduleHelper.Core.Services.Helpers.GenerateScheduleTools;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleHelper.Core.Services
{
    public class ScheduleService : ScheduleServiceBase, IScheduleService
    {
        ITaskRespository _taskRepository;

        ScheduleSettings _scheduleSettings;
        IDbInMemory _inMemoryScheduleRepository;


        public ScheduleService(ITaskRespository taskRepository, IScheduleRepository scheduleRepository, IDbInMemory inMemoryScheduleRepository) : base(scheduleRepository)
        {

            _taskRepository = taskRepository;
            _scheduleRepository = scheduleRepository;
            _inMemoryScheduleRepository = inMemoryScheduleRepository;


        }
        public async Task GenerateSchedule(ScheduleSettingsDTO scheduleSettings)
        {

            await _scheduleRepository.CleanTimeSlotInScheduleTable();
            var tasksList = await _taskRepository.GetTasks();

            await updateScheduleSettings(scheduleSettings);
            _scheduleSettings = await _scheduleRepository.GetScheduleSettings();
            await updateDaySchedule();

            await addElasticAndFixedTasksToSchedule(_scheduleSettings, tasksList);

        }

        private async Task updateDaySchedule()
        {
            var settings = await _scheduleRepository.GetScheduleSettings();
            var daySchedule = new DaySchedule()
            {
                Settings = settings,
                TimeFromLastBreakMin = 0

            };
            await _scheduleRepository.AddDayScheduleAndRemoveOld(daySchedule);
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
            var listOfNotSettasks = await _scheduleRepository.GetTasksNotSetInSchedule();
            return listOfNotSettasks.Select(task => DtoToEnityConverter.covertSingleTaskToTaskForEditListDTO(task))
                .ToList();
        }






        public async Task<int> GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus status)
        {
            var slots = await _scheduleRepository.GetTimeSlotsList();
            var slotsWithStatus = slots.Where(slot => slot.Status == status).ToList();
            double sumOfTimeForSlotsWithStatus = 0;
            foreach (var slot in slotsWithStatus)
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
                int result = (int)Math.Round(100 * sumOfTimeForSlotsWithStatus / sumOfTimeForAllSlots);
                return result;
            }

            return 0;
        }


        public async Task<ScheduleSettingsDTO> GetScheduleSettings()
        {
            ScheduleSettings settings = await _scheduleRepository.GetScheduleSettings();
            if (settings != null)
                return EntityToDtoConverter.ConvertScheduleSettingsToDto(settings);
            else
                return new ScheduleSettingsDTO()
                {
                    breakLenghtMin = 20,
                    finishTime = new TimeOnly(12, 0),
                    startTime = new TimeOnly(8, 0),
                    MaxWorkTimeBeforeBreakMin = 60,
                    MinWorkTimeBeforeBreakMin = 45

                };
        }

        public async Task<double> CalculateAvaiableFreeTimeBasedOnExistingTasks()
        {
            IScheduleRepository sqlServerRepository = _scheduleRepository;
            await replaceSqlScheduleRepositoryWithInMemoryRepository(_inMemoryScheduleRepository);

            await generateScheduleForInMemoryDb();

            List<TimeSlotInSchedule> tempSlotsList = await _scheduleRepository.GetActiveSlots();

            double timeRequiredForTasks = getSumOfDurationOfAllSlots(tempSlotsList);

            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();
            var maxTimeOfSchedule = GetMaxTimeOfSchedule(scheduleSettings);

            double avaiableFreeTime = maxTimeOfSchedule - timeRequiredForTasks;

            setSqlRepositoryAsRepository(sqlServerRepository);
            return avaiableFreeTime;
        }

        private void setSqlRepositoryAsRepository(IScheduleRepository sqlServerRepository)
        {
            _scheduleRepository = sqlServerRepository;
        }

        private static double getSumOfDurationOfAllSlots(List<TimeSlotInSchedule> tempSlotsList)
        {
            double timeRequiredForSlots = 0;
            foreach (var slot in tempSlotsList)
            {
                timeRequiredForSlots += slot.getDurationOfSlotInMin();
            }

            return timeRequiredForSlots;
        }

        private static double GetMaxTimeOfSchedule(ScheduleSettings scheduleSettings)
        {
            return (scheduleSettings.FinishTime - scheduleSettings.StartTime).TotalMinutes;
        }

        private async Task generateScheduleForInMemoryDb()
        {
            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();
            ScheduleSettingsDTO settingsDto = EntityToDtoConverter.ConvertScheduleSettingsToDto(scheduleSettings);
            await GenerateSchedule(settingsDto);
        }

        private async Task replaceSqlScheduleRepositoryWithInMemoryRepository(IDbInMemory inMemoryScheduleRepository)
        {

            await inMemoryScheduleRepository.CleanTimeSlotInScheduleTable();
            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();
            if (scheduleSettings == null)
            {
                scheduleSettings = new ScheduleSettings()
                {
                    breakDurationMin = 20,
                    MaxWorkTimeBeforeBreakMin = 60,
                    MinWorkTimeBeforeBreakMin = 45,
                    HasScheduleBreaks = false,
                    FinishTime = new TimeOnly(21, 0),
                    StartTime = new TimeOnly(9, 0)

                };

            }
            await inMemoryScheduleRepository.UpdateScheduleSettings(scheduleSettings);
            _scheduleRepository = inMemoryScheduleRepository;
        }

        private async Task addElasticAndFixedTasksToSchedule(ScheduleSettings scheduleSettings, List<SingleTask> tasksList)
        {

            List<TimeSlotInSchedule> fixedTimeSlots = await addFixedTasksToScheduleAndGetListOfCreatedSlots(scheduleSettings, tasksList);
            var flexibleTasks = tasksList.FindAll(task => !task.HasStartTime);
            List<TimeSlotInSchedule> flexibleSlotsList = getTimeSlotsForTasks(flexibleTasks, scheduleSettings);
            List<TimeSlotInSchedule> allTimeSlots = new List<TimeSlotInSchedule>(flexibleSlotsList);
            allTimeSlots.AddRange(fixedTimeSlots);

            await loopWhichBuildScheduleByAdjustingSlots(scheduleSettings.StartTime, scheduleSettings, allTimeSlots);

        }

        private List<TimeSlotInSchedule> getTimeSlotsForTasks(List<SingleTask> flexibleTasks, ScheduleSettings settings)
        {


            var listOfSlots = new List<TimeSlotInSchedule>();
            var maxSlotDuration = getDuartionBetweenTimes(settings.StartTime, settings.FinishTime);

            foreach (var task in flexibleTasks)
            {
                convertTaskIntoTimeSlotsAndAddItToList(settings, listOfSlots, task);
            }
            return listOfSlots;
        }

        private static void convertTaskIntoTimeSlotsAndAddItToList(ScheduleSettings settings, List<TimeSlotInSchedule> listOfSlots, SingleTask task)
        {
            var maxSlotDuration = getDuartionBetweenTimes(settings.StartTime, settings.FinishTime);
 

            var finishTime = settings.StartTime.AddMinutes(task.TimeMin);
            if (maxSlotDuration < task.TimeMin)
            {
                finishTime = settings.FinishTime;
            }
               




            var slot = new TimeSlotInSchedule()
            {
                StartTime = settings.StartTime,
                FinishTime = finishTime,
                Status = TimeSlotStatus.Active,
                IsItBreak = false,
                task = task,
                OrdinalNumber = 1
            };
            listOfSlots.Add(slot);

        }

        private static void copyListOfTasks(List<SingleTask> flexibleTasks, List<SingleTask> copyOfListOfTasks)
        {
            foreach (var task in flexibleTasks)
            {
                copyOfListOfTasks.Add(task.Copy());
            }
        }

        private async Task<List<TimeSlotInSchedule>> addFixedTasksToScheduleAndGetListOfCreatedSlots(ScheduleSettings scheduleSettings, List<SingleTask> tasksList)
        {
            List<SingleTask> fixedTasksList = tasksList.FindAll(task => task.HasStartTime).ToList();
            List<TimeSlotInSchedule> fixedTimeSlots = new List<TimeSlotInSchedule>();
            foreach (var task in fixedTasksList)
            {
                if (!isInRangeOfSchedule(task.StartTime, task.TimeMin, scheduleSettings))
                    continue;
                var slotFinishTime = task.StartTime.AddMinutes(task.TimeMin);
                if (SlotOverlapsWithExistingFixedTimeSlot(task.StartTime, slotFinishTime, fixedTimeSlots))
                    continue;

                var newTimeSlot = new TimeSlotInSchedule()
                {
                    FinishTime = slotFinishTime,
                    StartTime = task.StartTime,
                    IsItBreak = false,
                    task = task,
                    OrdinalNumber = 1,
                    Status = TimeSlotStatus.Active
                };
                await addNewTimeSlot(newTimeSlot);
                fixedTimeSlots.Add(newTimeSlot);
            }

            return fixedTimeSlots;

        }

        private bool isInRangeOfSchedule(TimeOnly startTime, double timeMin, ScheduleSettings scheduleSettings)
        {
            if (startTime < scheduleSettings.StartTime || startTime > scheduleSettings.FinishTime)
            {
                return false;
            }
            double maxDuration = getDuartionBetweenTimes(startTime, scheduleSettings.FinishTime);
            if (maxDuration < timeMin)
            {
                return false;
            }
            return true;
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
            TimeSlotInSchedule? timeSlotForBreak = makeTimeSlot(_scheduleSettings, iterationState, scheduleSettings.breakLenghtMin, null);
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

            TimeSlotInSchedule lastOverlapsingTimeSlot = GetLastOverlapsingFixedTimeSlot(fixedTimeSlots, finishTimeOfLastTask);

            TimeOnly lastBreakFinishTime = lastOverlapsingTimeSlot.StartTime;
            return (TimeOnly)lastBreakFinishTime;
        }

        private TimeOnly getfixedTaskFinishTime(List<TimeSlotInSchedule> fixedTimeSlots, TimeOnly finishTimeOfLastTask)
        {
            TimeOnly? fixedTaskFinishTime;
            TimeSlotInSchedule lastOverlapsingTimeSlot = GetLastOverlapsingFixedTimeSlot(fixedTimeSlots, finishTimeOfLastTask);

            fixedTaskFinishTime = lastOverlapsingTimeSlot.FinishTime;
            return (TimeOnly)fixedTaskFinishTime;
        }




    }
}
