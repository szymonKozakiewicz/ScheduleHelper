using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities
{


    public class SingleTask
    {
        public string Name { get; set; }
        public double TimeMin { get; set; }
        Guid Id { get; set; }

    }
}
