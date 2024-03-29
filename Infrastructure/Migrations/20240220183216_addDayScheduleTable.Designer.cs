﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScheduleHelper.Infrastructure;

#nullable disable

namespace ScheduleHelper.Infrastructure.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20240220183216_addDayScheduleTable")]
    partial class addDayScheduleTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.DaySchedule", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("settingsId")
                        .HasColumnType("int");

                    b.Property<double>("timeFromLastBreakMin")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.HasIndex("settingsId");

                    b.ToTable("DaySchedule", (string)null);
                });

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.ScheduleSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("MaxWorkTimeBeforeBreakMin")
                        .HasColumnType("float");

                    b.Property<double>("MinWorkTimeBeforeBreakMin")
                        .HasColumnType("float");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("breakDurationMin")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("ScheduleSettings", (string)null);
                });

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.SingleTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasStartTime")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("TimeMin")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Tasks", (string)null);
                });

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsItBreak")
                        .HasColumnType("bit");

                    b.Property<int>("OrdinalNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("taskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("taskId");

                    b.ToTable("TimeSlots", (string)null);
                });

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.DaySchedule", b =>
                {
                    b.HasOne("ScheduleHelper.Core.Domain.Entities.ScheduleSettings", "settings")
                        .WithMany()
                        .HasForeignKey("settingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("settings");
                });

            modelBuilder.Entity("ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule", b =>
                {
                    b.HasOne("ScheduleHelper.Core.Domain.Entities.SingleTask", "task")
                        .WithMany()
                        .HasForeignKey("taskId");

                    b.Navigation("task");
                });
#pragma warning restore 612, 618
        }
    }
}
