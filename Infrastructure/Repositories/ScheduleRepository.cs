using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private MyDbContext _dbContext;
        public ScheduleRepository(MyDbContext myDbContext)
        {
            _dbContext = myDbContext;
        }
        public async Task AddNewTimeSlot(TimeSlotInSchedule timeSlot)
        {
            _dbContext.TimeSlotsInSchedule.Add(timeSlot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CleanTimeSlotInScheduleTable()
        {
            _dbContext.TimeSlotsInSchedule.RemoveRange(_dbContext.TimeSlotsInSchedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TimeSlotInSchedule>> GetActiveSlots()
        {
            return await _dbContext.TimeSlotsInSchedule
                .Where(slot=>slot.Status==TimeSlotStatus.Active)
                .ToListAsync();
        }

        public async Task<ScheduleSettings> GetScheduleSettings()
        {
            return await _dbContext.ScheduleSettings.FindAsync(1);
        }

        public async Task<List<SingleTask>> GetTasksNotSetInSchedule()
        {

            var resultOfQuery= from task in _dbContext.SingleTask
                   where !_dbContext.TimeSlotsInSchedule.Any(slot => slot.task == task)
                   select task;
            var result= await resultOfQuery.ToListAsync();
            return result;
        }

        public Task<TimeSlotInSchedule?> GetTimeSlot(Guid slotId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TimeSlotInSchedule>> GetTimeSlotsList()
        {
            return await _dbContext.TimeSlotsInSchedule.
                Include(slot=>slot.task).ToListAsync();
        }

        public async Task UpdateScheduleSettings(ScheduleSettings scheduleSettingsForDb)
        {
            _dbContext.Update(scheduleSettingsForDb);
            await _dbContext.SaveChangesAsync();
        }

        public Task UpdateTimeSlot(TimeSlotInSchedule timeSlotInSchedule)
        {
            throw new NotImplementedException();
        }
    }
}
