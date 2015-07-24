using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Infrastructure;

namespace SDDB.UnitTests
{
    [TestClass]
    public class Tests___EFDbContextExtensions
    {
        [TestMethod]
        public void EFDBContext_SaveChangesWithRetryAsync_RunsSaveChangesAsync()
        {
            //Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            //Act
            mockEfDbContext.Object.SaveChangesWithRetryAsync().Wait();

            //Assert
            
        }

        [TestMethod]
        public void EFDBContext_SaveChangesWithRetryAsync_RetriesIfGetsDeadlockError()
        {
            //Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            var callCounter = 0;
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(() => 
            {
                callCounter++;
                if (callCounter == 1) { throw new Exception("Deadlock found when trying to get lock"); }
                return Task.FromResult<int>(1);
            });
            
            //Act
            mockEfDbContext.Object.SaveChangesWithRetryAsync().Wait();
            
            //Assert
            Assert.IsTrue(callCounter == 2);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Deadlock found when trying to get lock")]
        public void EFDBContext_SaveChangesWithRetryAsync_Retries10TimesAndThrows()
        {
            //Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new Exception("Deadlock found when trying to get lock"));

            //Act
            mockEfDbContext.Object.SaveChangesWithRetryAsync().Wait();

            //Assert
        }
    }
}

        