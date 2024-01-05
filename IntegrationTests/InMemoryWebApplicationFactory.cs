using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScheduleHelper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.IntegrationTests
{
    public class InMemoryWebApplicationFactory:WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(RemoveOldDbAndAddInMemoryDb);
        }

 
        private void RemoveOldDbAndAddInMemoryDb(IServiceCollection services)
        {
            var dbContextOptions = this.GetDbContext(services);
            services.Remove(dbContextOptions);

            services.AddDbContext<MyDbContext>(options => options.UseInMemoryDatabase("ScheduleHelperDb"));
        }
        private ServiceDescriptor GetDbContext(IServiceCollection services)
        {
            return services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<MyDbContext>));
        }
    }
}
