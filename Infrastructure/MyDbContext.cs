using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Infrastructure
{
    public class MyDbContext:DbContext
    {
        DbSet<SingleTask> singleTask;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SingleTask>().ToTable("Tasks");
        }
        public MyDbContext(DbContextOptions options) :base(options)
        {
            
        }
    }
}
