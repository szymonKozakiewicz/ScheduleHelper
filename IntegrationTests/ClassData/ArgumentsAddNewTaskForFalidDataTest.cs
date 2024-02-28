using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.IntegrationTests.ClassData
{
    public class ArgumentsAddNewTaskForFalidDataTest : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = 30,
                HasStartTime = true,
                StartTime = new TimeOnly(5, 25)

            };
            yield return new object[] { model };
            model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = 30,
                HasStartTime = false

            };
            yield return new object[] { model };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
