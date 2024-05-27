using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class FinaliseSlotDTO
    {
        public Guid SlotId { get; set; }
        public string FinishTime { get; set; }
        [Range(0, 100, ErrorMessage = "Value have to be between 0-100")]
        public int ComplitedShareOfTask { get; set; }
    }
}
