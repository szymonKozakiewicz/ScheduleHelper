

using ScheduleHelper.Core.DTO;

namespace ScheduleHelper.Core.ServiceContracts
{
    public interface ITaskService
    {
        public Task AddNewTask(TaskCreateDTO taskDTO);
        Task<TaskCreateDTO> GetTaskCreateDTOWithId(Guid taskToDeleteId);
        public Task<List<TaskCreateDTO>> GetTasksList();
        Task RemoveTaskWithId(Guid taskToDeleteId);
        Task UpdateTask(TaskCreateDTO newVersionOfTask);
    }
}
