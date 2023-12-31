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

        public async Task<List<SingleTask>> GetTasks()
        {
            return await _dbContext.SingleTask.ToListAsync();
        }


        public async Task RemoveTaskWithId(Guid id)
        {

            var taskToRemove=await _dbContext.SingleTask.FindAsync(id);
            if(taskToRemove == null)
            {
                throw new ArgumentException("There is no task with such id in Db");
            }
            _dbContext.SingleTask.Remove(taskToRemove);
            await _dbContext.SaveChangesAsync();
        }
    }
}
