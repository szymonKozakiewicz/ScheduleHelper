using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
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
    }
}
