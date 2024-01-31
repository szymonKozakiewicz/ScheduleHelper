using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Infrastructure
{
    public class MyDbContext:DbContext
    {
        public DbSet<SingleTask> SingleTask { get; set; }
        public DbSet<TimeSlotInSchedule> TimeSlotsInSchedule { get; set; }
        public DbSet<ScheduleSettings> ScheduleSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SingleTask>()
                .ToTable("Tasks");

            modelBuilder.Entity<ScheduleSettings>()
                .ToTable("ScheduleSettings");



            modelBuilder.Entity<ScheduleSettings>()
                .Property(settings => settings.FinishTime)
                .HasConversion(
                    v => new DateTime(1, 1, 1, v.Hour, v.Minute, v.Second),
                    v => new TimeOnly(v.TimeOfDay.Hours, v.TimeOfDay.Minutes, v.TimeOfDay.Seconds)
                );

            modelBuilder.Entity<ScheduleSettings>()
                .Property(settings => settings.StartTime)
                .HasConversion(
                    v => new DateTime(1, 1, 1, v.Hour, v.Minute, v.Second),
                    v => new TimeOnly(v.TimeOfDay.Hours, v.TimeOfDay.Minutes, v.TimeOfDay.Seconds)
                );

            modelBuilder.Entity<TimeSlotInSchedule>()
                .Property(ts => ts.FinishTime)
                .HasConversion(
                    v => new DateTime(1,1,1,v.Hour,v.Minute,v.Second),
                    v => new TimeOnly(v.TimeOfDay.Hours, v.TimeOfDay.Minutes, v.TimeOfDay.Seconds)
                );

            modelBuilder.Entity<TimeSlotInSchedule>()
                .Property(ts => ts.StartTime)
                .HasConversion(
                    v => new DateTime(1, 1, 1, v.Hour, v.Minute, v.Second),
                    v => new TimeOnly(v.TimeOfDay.Hours, v.TimeOfDay.Minutes, v.TimeOfDay.Seconds)
                );
            modelBuilder.Entity<TimeSlotInSchedule>()
                .ToTable("TimeSlots");
        }
        public MyDbContext(DbContextOptions options) :base(options)
        {
            
        }
    }
}
