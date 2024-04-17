using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
namespace ScheduleHelper.Infrastructure.Repositories
{
    public class InMemoryScheduleRepository : IDbInMemory
    {
        DaySchedule _daySchedule;
        TimeSlotsList _timeSlotsList;
        ScheduleSettings _scheduleSettings;

        public InMemoryScheduleRepository()
        {
            _scheduleSettings = new ScheduleSettings();
            _daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = 0
            };
            _timeSlotsList = new TimeSlotsList();
        }
        public async Task AddDayScheduleAndRemoveOld(DaySchedule newDaySchedule)
        {
            _daySchedule = newDaySchedule;
        }

        public async Task AddNewTimeSlot(TimeSlotInSchedule timeSlot)
        {
            _timeSlotsList.Add(timeSlot);
        }

        public async Task AddScheduleSettings(ScheduleSettings scheduleSettings)
        {
            _scheduleSettings=scheduleSettings;
        }

        public async Task CleanTimeSlotInScheduleTable()
        {
            _timeSlotsList=new TimeSlotsList();
        }

        public async Task<List<TimeSlotInSchedule>> GetActiveSlots()
        {
            return _timeSlotsList.FindAll(a => a.Status == TimeSlotStatus.Active)
                .ToList();

        }

        public async Task<List<TimeSlotInSchedule>> GetCanceledSlots()
        {
            return _timeSlotsList.FindAll(a => a.Status == TimeSlotStatus.Canceled)
                .ToList();
        }

        public async Task<DaySchedule> GetDaySchedule()
        {
            return _daySchedule;
        }

        public async Task<ScheduleSettings> GetScheduleSettings()
        {
            return _scheduleSettings;
        }

        public async Task<List<SingleTask>> GetTasksNotSetInSchedule()
        {
            throw new NotImplementedException();
        }

        public async Task<TimeSlotInSchedule?> GetTimeSlot(Guid slotId)
        {
            return _timeSlotsList.Find(m => m.Id == slotId);
        }

        public async Task<List<TimeSlotInSchedule>> GetTimeSlotsList()
        {
            return _timeSlotsList;
        }

        public async Task RemoveTimeSlot(TimeSlotInSchedule timeSlotToRemove)
        {
            _timeSlotsList.Remove(timeSlotToRemove);
        }

        public async Task UpdateDaySchedule(DaySchedule newDaySchedule)
        {
            _daySchedule = newDaySchedule;
        }

        public async Task UpdateScheduleSettings(ScheduleSettings scheduleSettingsForDb)
        {
            _scheduleSettings = scheduleSettingsForDb;
        }

        public async Task UpdateTimeSlot(TimeSlotInSchedule timeSlotToUpdate)
        {
            var timeSlotToRemove=_timeSlotsList.Find(a=>a.Id == timeSlotToUpdate.Id);
            _timeSlotsList.Remove(timeSlotToRemove);
            _timeSlotsList.Add(timeSlotToUpdate);
        }
    }
}
