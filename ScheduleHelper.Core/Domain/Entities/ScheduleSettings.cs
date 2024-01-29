﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities
{
    public class ScheduleSettings
    {
        [Key]
        public int Id { get; set; }
        public TimeOnly FinishTime { get; set; }
        public double breakDurationMin { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ScheduleSettings settings &&
                   FinishTime.Equals(settings.FinishTime) &&
                   breakDurationMin == settings.breakDurationMin;
        }
    }
}
