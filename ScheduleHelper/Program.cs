global using TimeSlotList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using ScheduleHelper.Infrastructure;
using ScheduleHelper.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<IScheduleService, ScheduleService>();
builder.Services.AddTransient<IScheduleUpdateService, ScheduleUpdateService>();
builder.Services.AddTransient<ITaskRespository, TaskRepository>();
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IDbInMemory, InMemoryScheduleRepository>();
var connectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyDbContext>(
options => {
    options.UseSqlServer(connectionStrings);
}
);
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program
{

}