

using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.Core.ServiceContracts
{
    public interface ITaskService
    {
        public Task AddNewTask(TaskCreateDTO taskDTO);
        Task<List<TaskForSheduleDTO>> GetTasksForSchedule();
        public Task<List<TaskForEditListDTO>> GetTasksList();
        Task RemoveTaskWithId(Guid taskToDeleteId);
    }
}
