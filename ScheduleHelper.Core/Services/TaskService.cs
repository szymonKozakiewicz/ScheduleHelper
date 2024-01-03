using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async void AddNewTask(SingleTaskDTO taskDTO)
        {
            SingleTask newTask = covertSingleTaskDtoToSingleTask(taskDTO);
            //TODO: validation
            _taskRepository.AddNewTask(newTask);
        }

        private SingleTask covertSingleTaskDtoToSingleTask(SingleTaskDTO taskDTO)
        {
            return new SingleTask()
            {
                Name = taskDTO.Name,
                TimeMin = taskDTO.Time
            };
        }
    }
}
