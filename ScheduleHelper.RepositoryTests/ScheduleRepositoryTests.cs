using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Infrastructure;
using ScheduleHelper.Infrastructure.Repositories;
using ScheduleHelper.RepositoryTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.RepositoryTests
{
    public class ScheduleRepositoryTests
    {
        
        DbContextOptionsBuilder<MyDbContext> builder;
            
        public ScheduleRepositoryTests()
        {
            builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseInMemoryDatabase("ScheduleHelperDb");
        }

        [Fact]
        public async Task AddNewTimeSlot_ForValidData_ShouldAddEntityToDB()
        {
            var task = new SingleTask("testTask", 20);
            var entity = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(22,0))
                .SetTask(task)
                .SetStartTime(new TimeOnly(6,0))
                .SetOrdinalNumber(1)
                .SetIsItBreak(false)
                .Build();
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);
                await dbcontext.AddAsync(task);
                await dbcontext.SaveChangesAsync();
                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);


                //act
                await scheduleRepository.AddNewTimeSlot(entity);


                //assert
                List<TimeSlotInSchedule> listOfSlots = dbcontext.TimeSlotsInSchedule.ToList();
                
                listOfSlots.Should().HaveCount(1);
                listOfSlots.Should().Contain(entity);


            }
        }

        [Fact]
        public async Task CleanTimeSlotInScheduleTable_forDbWhichHasSomeTimeSlots_expectedEmptyTable()
        {
            var task = new SingleTask("testTask", 20);
            List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = GetListOfTimeSlotsForTest(task);
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);
                await dbcontext.SingleTask.AddAsync(task);
                await dbcontext.SaveChangesAsync();
                await dbcontext.TimeSlotsInSchedule.AddAsync(listOfTimeSlotsInSchedule[0]);
                await dbcontext.TimeSlotsInSchedule.AddAsync(listOfTimeSlotsInSchedule[0]);
                await dbcontext.SaveChangesAsync();
                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);


                //act
                await scheduleRepository.CleanTimeSlotInScheduleTable();


                //assert
                List<TimeSlotInSchedule> listOfSlots = dbcontext.TimeSlotsInSchedule.ToList();
                listOfSlots.Should().HaveCount(0);



            }
        }



        [Fact]
        public async Task GetTimeSlotsList_forDbWhichHasSomeTimeSlots_expectThatThisTimeSlotsWillBeReturned()
        {
            var task = new SingleTask("testTask", 20);
            List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = GetListOfTimeSlotsForTest(task);
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);
                await AddTimeSlotsAndTaskToDb(task, listOfTimeSlotsInSchedule, dbcontext);
                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);


                //act
                var resultTimeSlotsList=await scheduleRepository.GetTimeSlotsList();


                //assert
                resultTimeSlotsList.Should().HaveCount(2);
                resultTimeSlotsList.Should().BeEquivalentTo(listOfTimeSlotsInSchedule);



            }

        }

        [Fact]
        public async Task GetTasksNotSetInSchedule_2TaksFor4AreNotSetInSchedule_Return2NotSeTask()
        {

            SingleTask notPresentTask1=new SingleTask("testTask2", 20);
            SingleTask notPresentTask2=new SingleTask("testTask4", 20);
            //arrange
            List<SingleTask> listOfTasks = new List<SingleTask>()
            {
                new SingleTask("testTask1", 20),
                notPresentTask1,
                new SingleTask("testTask3", 20),
                notPresentTask2

            };

            var entity1 = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(22, 0))
                .SetTask(listOfTasks[0])
                .SetStartTime(new TimeOnly(6, 0))
                .SetOrdinalNumber(1)
                .SetIsItBreak(false)
                .Build();
            var entity2 = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(22, 0))
                .SetTask(listOfTasks[2])
                .SetStartTime(new TimeOnly(6, 0))
                .SetOrdinalNumber(1)
                .SetIsItBreak(false)
                .Build();

            List<TimeSlotInSchedule> timeSlotsList = new List<TimeSlotInSchedule>()
            {
                entity1,entity2
            };
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                foreach (var task in listOfTasks)
                {
                    dbcontext.SingleTask.Add(task);
                    await dbcontext.SaveChangesAsync();
                }


                foreach (var timeSlot in timeSlotsList)
                {
                    dbcontext.TimeSlotsInSchedule.Add(timeSlot);
                    await dbcontext.SaveChangesAsync();
                }
                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);


                //act
                var resultList=await scheduleRepository.GetTasksNotSetInSchedule();

                //assert
                resultList.Should().Contain(notPresentTask2);
                resultList.Should().Contain(notPresentTask1);

            }

            
        }





        private static async Task AddTimeSlotsAndTaskToDb(SingleTask task, List<TimeSlotInSchedule> listOfTimeSlotsInSchedule, MyDbContext dbcontext)
        {
            await dbcontext.SingleTask.AddAsync(task);
            await dbcontext.SaveChangesAsync();
            await dbcontext.TimeSlotsInSchedule.AddAsync(listOfTimeSlotsInSchedule[0]);
            await dbcontext.TimeSlotsInSchedule.AddAsync(listOfTimeSlotsInSchedule[1]);
            await dbcontext.SaveChangesAsync();
        }
        private static List<TimeSlotInSchedule> GetListOfTimeSlotsForTest(SingleTask task)
        {
            var entity1 = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(22, 0))
                .SetTask(task)
                .SetStartTime(new TimeOnly(6, 0))
                .SetOrdinalNumber(1)
                .SetIsItBreak(false)
                .Build();

            var entity2 = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(21, 0))
                .SetTask(task)
                .SetStartTime(new TimeOnly(9, 0))
                .SetOrdinalNumber(2)
                .SetIsItBreak(false)
                .Build();
            List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = new()
            {
                entity1,entity2
            };
            return listOfTimeSlotsInSchedule;
        }
    }
}
