﻿using Microsoft.EntityFrameworkCore;
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

        public async Task AddDayScheduleAndRemoveOld(DaySchedule daySchedule)
        {
           var listOfDaySchedule= _dbContext.DaySchedule.ToList();
            foreach(var oldDaySchedule in listOfDaySchedule) { 
            
                _dbContext.Remove(oldDaySchedule);
            }
            _dbContext.DaySchedule.Add(daySchedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddNewTimeSlot(TimeSlotInSchedule timeSlot)
        {
            _dbContext.TimeSlotsInSchedule.Add(timeSlot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddScheduleSettings(ScheduleSettings scheduleSettings)
        {
            await _dbContext.ScheduleSettings.AddAsync(scheduleSettings);
            _dbContext.SaveChanges();
        }

        public async Task CleanTimeSlotInScheduleTable()
        {
            _dbContext.TimeSlotsInSchedule.RemoveRange(_dbContext.TimeSlotsInSchedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TimeSlotInSchedule>> GetActiveSlots()
        {
            return await _dbContext.TimeSlotsInSchedule
                .Where(slot=>slot.Status==TimeSlotStatus.Active).Include(m=>m.task)
                .ToListAsync();
        }

        public async Task<List<TimeSlotInSchedule>> GetCanceledSlots()
        {
            return await _dbContext.TimeSlotsInSchedule
                .Where(slot => slot.Status == TimeSlotStatus.Canceled).Include(m => m.task)
                .ToListAsync();
        }

        public async Task<DaySchedule> GetDaySchedule()
        {
            if (_dbContext.DaySchedule.ToList().Count > 0)
                return await _dbContext.DaySchedule.FirstAsync();
            else
            {
                return null;
            }
        }

        public async Task<ScheduleSettings> GetScheduleSettings()
        {
            if (_dbContext.ScheduleSettings.Count() == 0)
                return null;
            return await _dbContext.ScheduleSettings.FirstAsync();
        }

        public async Task<List<SingleTask>> GetTasksNotSetInSchedule()
        {

            var resultOfQuery= from task in _dbContext.SingleTask
                   where !_dbContext.TimeSlotsInSchedule.Any(slot => slot.task == task)
                   select task;
            var result= await resultOfQuery.ToListAsync();
            return result;
        }

        public async Task<TimeSlotInSchedule?> GetTimeSlot(Guid slotId)
        {

            var timeSlot= await _dbContext.TimeSlotsInSchedule.FindAsync(slotId);
            if (timeSlot != null)
            {
                await _dbContext.Entry(timeSlot).Reference(ts => ts.task).LoadAsync();
            }
            return timeSlot;
        }

        public async Task<List<TimeSlotInSchedule>> GetTimeSlotsList()
        {
            return await _dbContext.TimeSlotsInSchedule.
                Include(slot=>slot.task).ToListAsync();
        }

        public async Task RemoveTimeSlot(TimeSlotInSchedule timeSlot)
        {
            _dbContext.TimeSlotsInSchedule.Remove(timeSlot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDaySchedule(DaySchedule daySchedule)
        {
            _dbContext.DaySchedule.Update(daySchedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateScheduleSettings(ScheduleSettings scheduleSettingsForDb)
        {
            var oldSettings = _dbContext.ScheduleSettings.ToList()[0];
            _dbContext.Entry(oldSettings).State = EntityState.Detached;
            scheduleSettingsForDb.Id = oldSettings.Id;
            _dbContext.ScheduleSettings.Update(scheduleSettingsForDb);
            _dbContext.SaveChanges();

        }

        public async Task UpdateTimeSlot(TimeSlotInSchedule timeSlotInSchedule)
        {
            _dbContext.TimeSlotsInSchedule.Update(timeSlotInSchedule);
            await _dbContext.SaveChangesAsync();
        }
    }
}
