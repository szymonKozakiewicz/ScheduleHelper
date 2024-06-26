﻿using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TaskRepository : ITaskRespository
    {
        private MyDbContext _dbContext;
        public TaskRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddNewTask(SingleTask task)
        {
            await _dbContext.AddAsync(task);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SingleTask> GetTask(Guid id)
        {
            return await _dbContext.SingleTask.AsNoTracking().FirstAsync(x=>x.Id==id);
        }

        public async Task<List<SingleTask>> GetTasks()
        {
            return await _dbContext.SingleTask.ToListAsync();
        }


        public async Task RemoveTaskWithId(Guid id)
        {

            var taskToRemove = await _dbContext.SingleTask.Where(a => a.Id == id)
            .FirstOrDefaultAsync();
            var list = _dbContext.SingleTask.ToList();
            if (taskToRemove == null)
            {
                throw new ArgumentException("There is no task with such id in Db");
            }

            await removeTimeSlotsRelatedToTask(id);

            _dbContext.SingleTask.Remove(taskToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTask(SingleTask singleTask)
        {

            var taskToUpdate = await _dbContext.SingleTask.AsNoTracking().Where(a => a.Id == singleTask.Id)
            .FirstAsync();
            if (taskToUpdate != null)
            {
                _dbContext.SingleTask.Update(singleTask);
                await _dbContext.SaveChangesAsync();

            }
            else
            {
                throw new ArgumentException("Task not exists in db");
            }
        }

        private async Task removeTimeSlotsRelatedToTask(Guid id)
        {
            var relatedTimeSlots = _dbContext.TimeSlotsInSchedule.Where(ts => ts.task.Id == id).ToList();
            if (relatedTimeSlots.Any())
            {

                _dbContext.TimeSlotsInSchedule.RemoveRange(relatedTimeSlots);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
