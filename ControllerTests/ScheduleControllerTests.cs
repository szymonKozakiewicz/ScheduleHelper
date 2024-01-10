using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using ScheduleHelper.ControllerTests.Helpers;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.ControllerTests
{
    public class ScheduleControllerTests
    {
        private ScheduleController _controller;
        private Mock<IScheduleService>_mockScheduleService;

        public ScheduleControllerTests()
        {
            _mockScheduleService = new Mock<IScheduleService>();
            _controller=new ScheduleController(_mockScheduleService.Object);
        }

 








    }
}
