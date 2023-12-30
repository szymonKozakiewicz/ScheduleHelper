

using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.Core.ServiceContracts
{
    public interface ITaskService
    {
        public void AddNewTask(SingleTaskDTO taskDTO);

    }
}
