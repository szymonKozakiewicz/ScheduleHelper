using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TaskRepository : ITaskRespository
    {
        public TaskRepository()
        {
            
        }
        public Task AddNewTask(SingleTask task)
        {
            throw new NotImplementedException();
        }
    }
}
