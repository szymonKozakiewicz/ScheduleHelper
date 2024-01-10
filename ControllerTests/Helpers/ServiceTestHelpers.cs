using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleHelper.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ControllerTests.Helpers
{
    public static class ServiceTestHelpers
    {
        public static void setMockForResponseStatus(TasksController controller, HttpStatusCode status)
        {
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupSet(r => r.StatusCode = (int)status);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(mock => mock.Response).Returns(httpResponseMock.Object);


            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            };
            controller.ControllerContext = controllerContext;
        }
    }
}
