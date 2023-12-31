﻿using FluentAssertions;
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
        DbContextOptionsBuilder<MyDbContext> builder;
        public TaskRepositoryTests()
        {
            builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseInMemoryDatabase("ScheduleHelperDb");
        }

        [Fact]
        public async Task AddNewTask_ForValidTask_ExpectThatTaskWillBeAddedToDatabase()
        {
            
            using (var dbcontext=new MyDbContext(builder.Options))
            {
                clearDatabase(dbcontext);
                ITaskRespository taskRespository = new TaskRepository(dbcontext);
                var newTask = new SingleTask("Test", 23);


                //act
                taskRespository.AddNewTask(newTask);
                var addedTask = dbcontext.SingleTask.FirstOrDefault(t => t.Name == "Test");

                //assert
                addedTask.Should().NotBeNull();

            }

        }


        [Fact]
        public async Task GetTasks_ForGivenTasksInDb_ShouldReturnsTasksFromDb()
        {

            using (var dbcontext = new MyDbContext(builder.Options))
            {
                // Clear the database before executing the test
                clearDatabase(dbcontext);
                ITaskRespository taskRespository = new TaskRepository(dbcontext);
                var task1 = new SingleTask("test1", 15);
                SingleTask task2 = new SingleTask("test2", 14);
                SingleTask task3 = new SingleTask("test3", 12.3);
                dbcontext.Add(task1);
                dbcontext.Add(task2);
                dbcontext.Add(task3);
                dbcontext.SaveChanges();



                //act
                var resultList = await taskRespository.GetTasks();


                //assert
                resultList.Should().HaveCount(3);
                resultList.Should().Contain(task1);
                resultList.Should().Contain(task2);
                resultList.Should().Contain(task3);

            }

        }

        [Fact]
        public async Task RemoveTaskWithId_ForValidId_shouldRemoveTaskFromDb()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                // Clear the database before executing the test
                clearDatabase(dbcontext);
                ITaskRespository taskRespository = new TaskRepository(dbcontext);
                var task1 = new SingleTask("test1", 15);
                dbcontext.Add(task1);
                dbcontext.SaveChanges();



                //act
                await taskRespository.RemoveTaskWithId(task1.Id);


                //assert

                dbcontext.SingleTask.ToList().Should().BeEmpty();


            }
        }


        [Fact]
        public async Task RemoveTaskWithId_ForIdWhichIsNotInDb_shouldRiseArgumentException()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                // Clear the database before executing the test
                clearDatabase(dbcontext);
                ITaskRespository taskRespository = new TaskRepository(dbcontext);
                var task1 = new SingleTask("test1", 15);
                dbcontext.Add(task1);
                dbcontext.SaveChanges();
                Guid badId= Guid.NewGuid();



                //act
               var action=async()=> await taskRespository.RemoveTaskWithId(badId);


                //assert

                await Assert.ThrowsAsync<ArgumentException>(action);


            }
        }

        private static void clearDatabase(MyDbContext dbcontext)
        {
            dbcontext.Database.EnsureDeleted();
            dbcontext.Database.EnsureCreated();
        }
    }
}
