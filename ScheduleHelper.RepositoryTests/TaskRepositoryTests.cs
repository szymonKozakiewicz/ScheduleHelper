using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.RepositoryTests
{
    public class TaskRepositoryTests
    {
        [Fact]
        public async Task AddNewTask_ForValidTask_ExpectThatTaskWillBeAddedToDatabase()
        {
            var builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseInMemoryDatabase("ScheduleHelperDb");
            using (var dbcontext=new MyDbContext(builder.Options))
            {
                ITaskRespository taskRespository = new TaskRepository(dbcontext);
                var newTask = new SingleTask()
                {
                    Name = "Test",
                    TimeMin = 23
                };


                //act
                taskRespository.AddNewTask(newTask);
                var addedTask = dbcontext.SingleTask.FirstOrDefault(t => t.Name == "Test");

                //assert
                addedTask.Should().NotBeNull();

            }

        }

    }
}
