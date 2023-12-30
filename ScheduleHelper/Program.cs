using Infrastructure.Repositories;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<ITaskRespository, TaskRepository>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program
{

}