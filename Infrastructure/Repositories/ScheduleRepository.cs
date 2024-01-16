using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
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

        public async Task<List<TimeSlotInSchedule>> GetTimeSlotsList()
        {
            return await _dbContext.TimeSlotsInSchedule.
                Include(slot=>slot.task).ToListAsync();
        }
    }
}
