using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScheduleHelper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ScheduleHelper.IntegrationTests
{
    public class InMemoryWebApplicationFactory : WebApplicationFactory<Program>
    {

        private string _dbName;
        public InMemoryWebApplicationFactory()
        {
            _dbName = Guid.NewGuid().ToString();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(RemoveOldDbAndAddInMemoryDb);
        }

        
        private void RemoveOldDbAndAddInMemoryDb(IServiceCollection services)
        {
            var dbContextOptions = this.GetDbContext(services);
            services.Remove(dbContextOptions);

            services.AddDbContext<MyDbContext>(optionsForDBcOntext);
        }

        private void optionsForDBcOntext(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase(_dbName);
        }


        public MyDbContext GetDbContextInstance()
        {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<MyDbContext>();
        }
        private ServiceDescriptor GetDbContext(IServiceCollection services)
        {
            return services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<MyDbContext>));
        }
    }
}
