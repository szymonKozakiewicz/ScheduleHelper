using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.ServiceContracts
{
    public interface IScheduleService
    {
        
        public Task GenerateSchedule(ScheduleSettingsDTO scheduleSettings);
        public Task<int> GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus status);
        Task<List<TaskForEditListDTO>> GetTasksNotSetInSchedule();
        public Task<List<TimeSlotInScheduleDTO>> GetTimeSlotsList();

    }
}
