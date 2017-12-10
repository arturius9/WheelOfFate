using Moq;
using SupportWheelOfFateWebApi.Business_Logic;
using SupportWheelOfFateWebApi.Controllers;
using SupportWheelOfFateWebApi.Data;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SupportWheelOfFate.WebApi.UnitTest
{
    public class BAUsControllerUnitTests
    {
        [Fact]
        public async void GetBAU_ShouldReturnAllBAUs()
        {
            //arrange
            var mockBusinessService = new Mock<IBusinessService>();
            var listOfBAUs = CreateBAUs();
            mockBusinessService.Setup(x => x.GetAllBAUs()).Returns(listOfBAUs);

            var controller = new BAUsController(mockBusinessService.Object);

            //act
            var actionResult = await controller.GetBAU();

            //assert
            var result = actionResult as OkObjectResult;
            var resultBAUs = result?.Value as IEnumerable<BAU>;

            Assert.NotNull(resultBAUs);
            Assert.Equal(listOfBAUs.Count(), resultBAUs.Count());
            mockBusinessService.Verify(x => x.GetAllBAUs(), Times.Once);
        }

        [Theory]
        [ClassData(typeof(DateData))]
        public async void GetBAUWithSpecificDateShouldReturnCorrectBAUs(DateTime date)
        {
            //arrange
            var mockBusinessService = new Mock<IBusinessService>();
            var listOfBAUs = CreateBAUs();
            var expectedBAUs = listOfBAUs.Where(x => x.Date == date);
            mockBusinessService.Setup(x => x.GetBAU(date)).Returns(expectedBAUs);

            var controller = new BAUsController(mockBusinessService.Object);

            //act
            var actionResult = await controller.GetBAU(date);

            //assert
            var result = actionResult as OkObjectResult;
            var resultBAUs = result?.Value as IEnumerable<BAU>;
            Assert.NotNull(resultBAUs);
            Assert.Equal(expectedBAUs.Count(), resultBAUs.Count());
            foreach (var bau in expectedBAUs)
            {
                Assert.True(resultBAUs.Contains(bau,new BAUEqualityComparer()));
            }
            mockBusinessService.Verify(x => x.GetBAU(date), Times.Once);
        }

        private IEnumerable<BAU> CreateBAUs()
        {
            var person1 = new Person();
            var person2 = new Person();

            yield return new BAU() { Date = new DateTime(2017, 11, 22), HalfOfTheDay = 1, Person = person1 };
            yield return new BAU() { Date = new DateTime(2017, 11, 22), HalfOfTheDay = 2, Person = person2 };

            yield return new BAU() { Date = new DateTime(2017,11,23), HalfOfTheDay = 1, Person = new Person() };
            yield return new BAU() { Date = new DateTime(2017, 11, 23), HalfOfTheDay = 2, Person = new Person() };

            yield return new BAU() { Date = new DateTime(2017, 11, 24), HalfOfTheDay = 1, Person = new Person() };
            yield return new BAU() { Date = new DateTime(2017, 11, 24), HalfOfTheDay = 2, Person = new Person() };
        }
    }

}
