using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities.Helpers;
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

        [Fact]
        public async Task UpdateScheduleSettings()
        {
            var model = new ScheduleSettings()
            {

                FinishTime = new TimeOnly(6, 23),
                breakDurationMin=20
            };

            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);
                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);
                
                //act
                await scheduleRepository.UpdateScheduleSettings(model);

                //assert
                var scheduleSettingsInDB= await dbcontext.ScheduleSettings.FindAsync(1);

                Assert.True(scheduleSettingsInDB.FinishTime.AreTimesEqualWithTolerance(model.FinishTime));

            }
        }

        [Fact]
        public async Task GetScheduleSettings_ReturnsDefaultsSettings()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);

                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);
                var expectedResult = new ScheduleSettings()
                {
                    FinishTime = new TimeOnly(1, 1),
                    breakDurationMin = 20
                };
                dbcontext.ScheduleSettings.Update(expectedResult);
                await dbcontext.SaveChangesAsync();


                //act
                var result = await scheduleRepository.GetScheduleSettings();

                //assert
                result.Should().Be(expectedResult);
                

            }

        }


        [Fact]
        public async Task GetActiveSlots_ReturnsAllActiveSlots()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);

                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);

                var task = new SingleTask("testTask", 20);
                List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = GetListOfTimeSlotsForTest(task);
                List<TimeSlotInSchedule>expectedResult=new List<TimeSlotInSchedule>(listOfTimeSlotsInSchedule);  
               var finishedTimeSlot = new TimeSlotInScheduleBuilder()
                    .SetFinishTime(new TimeOnly(22, 0))
                    .SetTask(task)
                    .SetStartTime(new TimeOnly(6, 0))
                    .SetTimeSlotStatus(TimeSlotStatus.Finished)
                    .SetOrdinalNumber(1)
                    .SetIsItBreak(false)
                    .Build();
                var canceledTimeSlot = new TimeSlotInScheduleBuilder()
                    .SetFinishTime(new TimeOnly(22, 0))
                    .SetTask(task)
                    .SetStartTime(new TimeOnly(6, 0))
                    .SetTimeSlotStatus(TimeSlotStatus.Canceled)
                    .SetOrdinalNumber(1)
                    .SetIsItBreak(false)
                    .Build();
                listOfTimeSlotsInSchedule.Add(finishedTimeSlot);
                listOfTimeSlotsInSchedule.Add(canceledTimeSlot);
                await AddTimeSlotsAndTaskToDb(task, listOfTimeSlotsInSchedule,dbcontext);



                //act
                var resultList = await scheduleRepository.GetActiveSlots();

                //assert
                resultList.Should().HaveCount(2);
                resultList.Should().Contain(expectedResult);


            }

        }


        [Fact]
        public async Task GetTimeSlot_ReturnsSlotWithRightId()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);

                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);

                var task = new SingleTask("testTask", 20);
                List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = GetListOfTimeSlotsForTest(task);
                List<TimeSlotInSchedule> expectedResult = new List<TimeSlotInSchedule>(listOfTimeSlotsInSchedule);
    
                await AddTimeSlotsAndTaskToDb(task, listOfTimeSlotsInSchedule, dbcontext);



                //act
                var result = await scheduleRepository.GetTimeSlot((Guid)listOfTimeSlotsInSchedule[0].Id);

                //assert
                result.Should().Be(listOfTimeSlotsInSchedule[0]);
                


            }

        }



        [Fact]
        public async Task UpdateTimeSlot_UpdateSlotInDb()
        {
            using (var dbcontext = new MyDbContext(builder.Options))
            {
                DbTestHelper.clearDatabase(dbcontext);

                IScheduleRepository scheduleRepository = new ScheduleRepository(dbcontext);

                var task = new SingleTask("testTask", 20);
                List<TimeSlotInSchedule> listOfTimeSlotsInSchedule = GetListOfTimeSlotsForTest(task);
                List<TimeSlotInSchedule> expectedResult = new List<TimeSlotInSchedule>(listOfTimeSlotsInSchedule);
       
                await AddTimeSlotsAndTaskToDb(task, listOfTimeSlotsInSchedule, dbcontext);

                listOfTimeSlotsInSchedule[0].Status = TimeSlotStatus.Canceled;
                listOfTimeSlotsInSchedule[0].StartTime = new TimeOnly(20, 20);
                listOfTimeSlotsInSchedule[0].FinishTime = new TimeOnly(10, 20);
                
                //act
                await scheduleRepository.UpdateTimeSlot(listOfTimeSlotsInSchedule[0]);

                //assert
                var result = await dbcontext.TimeSlotsInSchedule.FindAsync(listOfTimeSlotsInSchedule[0].Id);
                result.Status.Should().Be(TimeSlotStatus.Canceled);
                Assert.True(result.FinishTime.AreTimesEqualWithTolerance(new TimeOnly(10, 20)));
                Assert.True(result.StartTime.AreTimesEqualWithTolerance(new TimeOnly(20, 20)));



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
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetTask(task)
                .SetStartTime(new TimeOnly(6, 0))
                .SetOrdinalNumber(1)
                .SetIsItBreak(false)
                .Build();

            var entity2 = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(21, 0))
                .SetTask(task)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
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
