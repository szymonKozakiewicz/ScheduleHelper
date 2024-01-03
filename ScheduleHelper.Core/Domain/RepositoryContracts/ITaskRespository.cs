using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.RepositoryContracts
{
    public interface ITaskRespository
    {
        public Task AddNewTask(SingleTask task);
        public Task<List<SingleTask>> GetTasks();
    }
}
