using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.RepositoryContracts
{
    public interface IScheduleRepository
    {
        public Task AddNewTimeSlot(TimeSlotInSchedule timeSlot);
        public Task CleanTimeSlotInScheduleTable();
        Task<List<TimeSlotInSchedule>> GetTimeSlotsList();
        Task<List<SingleTask>> GetTasksNotSetInSchedule();

        Task UpdateTimeSlot(TimeSlotInSchedule timeSlotInSchedule);
        Task<TimeSlotInSchedule?> GetTimeSlot(Guid slotId);
        Task UpdateScheduleSettings(ScheduleSettings scheduleSettingsForDb);

        Task UpdateDaySchedule(DaySchedule scheduleSettingsForDb);
        Task RemoveTimeSlot(TimeSlotInSchedule scheduleSettingsForDb);
        Task AddDayScheduleAndRemoveOld(DaySchedule scheduleSettingsForDb);
        

        Task<DaySchedule> GetDaySchedule();
        Task<ScheduleSettings> GetScheduleSettings();
        Task<List<TimeSlotInSchedule>> GetActiveSlots();
    }
}
