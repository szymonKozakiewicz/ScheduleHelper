using ScheduleHelper.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.IntegrationTests.Helpers
{
    public static class DbTestHelper
    {
        public static void clearDatabase(MyDbContext dbcontext)
        {
            dbcontext.Database.EnsureDeleted();
            dbcontext.Database.EnsureCreated();
        }
    }
}
