using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class SingleTaskDTO
    {

        public string Name { get; set; }
        public int Time { get; set; }
        public SingleTaskDTO() {
            Name = "";
            Time = 1;
        }
    }
}
