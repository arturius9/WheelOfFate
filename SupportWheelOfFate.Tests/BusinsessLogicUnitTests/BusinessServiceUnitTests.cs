using Moq;
using Xunit;
using System;
using System.Linq;
using SupportWheelOfFateWebApi.Data;
using SupportWheelOfFateWebApi.Business_Logic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace SupportWheelOfFate.WebApi.UnitTest.BusinsessLogicUnitTests
{
    public class BusinessServiceUnitTests
    {
        [Fact]
        public void GetBAU_ShouldGetExistingBaus()
        {
            //arrange
            var mockBAUs = GetQueryableMockDbSet(CreateBAUs().ToList());
            var mockContext = new Mock<IWheelOfFateContext>();

            mockContext.Setup(x => x.BAU).Returns(mockBAUs.Object);
            BusinessService business = new BusinessService(mockContext.Object);

            //act
            var result = business.GetBAU(new DateTime(2017, 11, 23));

            //assert
            Assert.Equal(result.Count(), 2);
            mockBAUs.Verify(x => x.Add(It.IsAny<BAU>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DateData))]
        public void GetBAU_ShouldGenerateNewBaus(DateTime date)
        {
            //arrange
            var mockBAUs = GetQueryableMockDbSet(new List<BAU>());
            var mockPeople = GetQueryableMockDbSet(CreatePeople().ToList());
            var mockContext = new Mock<IWheelOfFateContext>();

            mockContext.Setup(x => x.BAU).Returns(mockBAUs.Object);
            mockContext.Setup(x => x.People).Returns(mockPeople.Object);

            BusinessService business = new BusinessService(mockContext.Object);

            //act
            var result = business.GetBAU(date);

            //assert
            Assert.Equal(result.Count(), 2);
            mockBAUs.Verify(x => x.Add(It.IsAny<BAU>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData(14)]
        [InlineData(30)]
        public void GetBAU_VerifyCorrectnessOfGeneratedBAUFor(int numberOfDays)
        {
            //arrange
            var mockBAUs = GetQueryableMockDbSet(new List<BAU>());
            var mockPeople = GetQueryableMockDbSet(CreatePeople().ToList());
            var mockContext = new Mock<IWheelOfFateContext>();

            mockContext.Setup(x => x.BAU).Returns(mockBAUs.Object);
            mockContext.Setup(x => x.People).Returns(mockPeople.Object);

            BusinessService business = new BusinessService(mockContext.Object);
            var date = new DateTime(2017, 11, 23);

            for (int i = 0; i < numberOfDays; i++)
            {
                date = date.AddDays(1);

                //act
                var result = business.GetBAU(date);
                //assert
                Assert.Equal(2, result.Count());
            }

            Assert.Equal(mockPeople.Object.Count(), mockBAUs.Object.GroupBy(x => x.Person).Distinct().Count());

            foreach (var person in mockPeople.Object)
            {
                Assert.True(mockBAUs.Object.Where(x => x.Person.Equals(person) && x.HalfOfTheDay == 1).Any());
                Assert.True(mockBAUs.Object.Where(x => x.Person.Equals(person) && x.HalfOfTheDay == 2).Any());
            }
        }

        private static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet;
        }

        [Fact]
        public void GetBAU_VerifyCorrectnessOfGeneratedBAUWhenCreationIsInStrangeOrder()
        {
            //arrange
            var mockBAUs = GetQueryableMockDbSet(new List<BAU>());
            var mockPeople = GetQueryableMockDbSet(CreatePeople().ToList());
            var mockContext = new Mock<IWheelOfFateContext>();

            mockContext.Setup(x => x.BAU).Returns(mockBAUs.Object);
            mockContext.Setup(x => x.People).Returns(mockPeople.Object);

            BusinessService business = new BusinessService(mockContext.Object);
            var date = new DateTime(2017, 11, 23);

            for (int i = 0; i < 5; i++)
            {
                date = date.AddDays(1);

                //act
                var result = business.GetBAU(date);
                //assert
                Assert.Equal(2, result.Count());
            }

            //act
            var res = business.GetBAU(new DateTime(2017, 11, 21));
            //assert
            Assert.Equal(2, res.Count());

            //act
            res = business.GetBAU(new DateTime(2017, 11, 18));
            //assert
            Assert.Equal(2, res.Count());

            //act
            res = business.GetBAU(new DateTime(2017, 11, 19));
            //assert
            Assert.Equal(2, res.Count());

            //act
            res = business.GetBAU(new DateTime(2017, 11, 20));
            //assert
            Assert.Equal(2, res.Count());

            //act
            res = business.GetBAU(new DateTime(2017, 11, 22));
            //assert
            Assert.Equal(2, res.Count());

            Assert.Equal(mockPeople.Object.Count(), mockBAUs.Object.GroupBy(x => x.Person).Distinct().Count());

            foreach (var person in mockPeople.Object)
            {
                Assert.Equal(1, mockBAUs.Object.Where(x => x.Person.Equals(person) && x.HalfOfTheDay == 1).Count());
                Assert.Equal(1, mockBAUs.Object.Where(x => x.Person.Equals(person) && x.HalfOfTheDay == 2).Count());
            }
        }

        private IEnumerable<BAU> CreateBAUs()
        {
            var person1 = new Person();
            var person2 = new Person();

            yield return new BAU() { Date = new DateTime(2017, 11, 22), HalfOfTheDay = 1, Person = person1 };
            yield return new BAU() { Date = new DateTime(2017, 11, 22), HalfOfTheDay = 2, Person = person2 };

            yield return new BAU() { Date = new DateTime(2017, 11, 23), HalfOfTheDay = 1, Person = new Person() };
            yield return new BAU() { Date = new DateTime(2017, 11, 23), HalfOfTheDay = 2, Person = new Person() };

            yield return new BAU() { Date = new DateTime(2017, 11, 24), HalfOfTheDay = 1, Person = new Person() };
            yield return new BAU() { Date = new DateTime(2017, 11, 24), HalfOfTheDay = 2, Person = new Person() };
        }

        private IEnumerable<Person> CreatePeople()
        {
            yield return new Person() { Id = 1, FirstName = "Alice", Surname = "A." };
            yield return new Person() { Id = 2, FirstName = "Bob", Surname = "B." };
            yield return new Person() { Id = 3, FirstName = "Celine", Surname = "C." };
            yield return new Person() { Id = 4, FirstName = "Damian", Surname = "D." };
            yield return new Person() { Id = 5, FirstName = "Eliza", Surname = "E." };
            yield return new Person() { Id = 6, FirstName = "Frank", Surname = "F." };
            yield return new Person() { Id = 7, FirstName = "Gregory", Surname = "G." };
            yield return new Person() { Id = 8, FirstName = "Henry", Surname = "H." };
            yield return new Person() { Id = 9, FirstName = "Ingrid", Surname = "I." };
            yield return new Person() { Id = 10, FirstName = "Jack", Surname = "J." };
        }
    }
}
