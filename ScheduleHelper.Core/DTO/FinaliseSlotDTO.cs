using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class FinaliseSlotDTO
    {
        public Guid SlotId { get; set; }
        public string FinishTime { get; set; }
    }
}
