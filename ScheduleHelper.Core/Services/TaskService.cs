using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScheduleHelper.Core.Services.Helpers.SingleTaskConvertHelper;

namespace ScheduleHelper.Core.Services
{
    public class TaskService : ITaskService
    {
        private ITaskRespository _taskRepository;
        public TaskService()
        {
            
        }
        public TaskService(ITaskRespository taskRespository)
        {
            _taskRepository = taskRespository;
        }
        public async Task AddNewTask(TaskCreateDTO taskDTO)
        {
            SingleTask newTask = covertSingleTaskDtoToSingleTask(taskDTO);
            //TODO: validation
            await _taskRepository.AddNewTask(newTask);
        }

        public async Task<List<TaskGetDTO>> GetTasksList()
        {
           var tasksList =await _taskRepository.GetTasks();
           var tasksDTOList=tasksList.Select(task=>covertSingleTaskToTaskGetDTO(task))
                .ToList();
            return tasksDTOList;
        }

        public async Task RemoveTaskWithId(Guid taskToDeleteId)
        {
            await _taskRepository.RemoveTaskWithId(taskToDeleteId);
        }
    }
}
