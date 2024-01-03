

using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.Core.ServiceContracts
{
    public interface ITaskService
    {
        public Task AddNewTask(SingleTaskDTO taskDTO);

    }
}
