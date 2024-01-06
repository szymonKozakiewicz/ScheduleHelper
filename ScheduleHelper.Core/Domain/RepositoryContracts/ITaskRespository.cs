using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
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
        public Task RemoveTaskWithId(Guid id);
    }
}
