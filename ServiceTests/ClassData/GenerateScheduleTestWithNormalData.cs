using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.ClassData
{
    public class GenerateScheduleTestWithNormalData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            ScheduleSettingsDTO testScheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings();

            List<SingleTask> tasksListsInMemory = GenerateDataHelper.GetNormalValidListOfTasks();
            List<TimeSlotInSchedule> expectedTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(tasksListsInMemory[0], tasksListsInMemory[1]);
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
