using System.Net;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using SDDB.Domain.Abstract;
using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.Domain.DbContexts;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SDDB.UnitTests
{
    [TestClass]
    public class Tests_DBLogger
    {
        [TestMethod]
        public void Logger_LogServiceResult_doesNotSaveIfLogLeve0()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                UserName = "dummyUser",
                StatusDescription = "test descr"
            };
            var logger = new DBLogger(0, 0, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            mockEfDbContext.Verify(m => m.DBResults.Add(It.IsAny<DBResult>()), Times.Never());
        }

        [TestMethod]
        public void Logger_LogServiceResult_DoesNotSaveIfResultOk()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.OK,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                UserName = "dummyUser",
                StatusDescription = "test descr"
            };
            var logger = new DBLogger(1, 0, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            mockEfDbContext.Verify(m => m.DBResults.Add(It.IsAny<DBResult>()), Times.Never());
        }

        [TestMethod]
        public void Logger_LogServiceResult_DoesNotSaveOKIfLoggingLevel2()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.DBResults.Add(It.IsAny<DBResult>())).Returns(new DBResult());
            mockEfDbContext.Setup(x => x.SaveChanges()).Verifiable();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.OK,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                UserName = "dummyUser",
                StatusDescription = "test descr"
            };
            var logger = new DBLogger(2, 0, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            mockEfDbContext.Verify(m => m.DBResults.Add(It.IsAny<DBResult>()), Times.Exactly(1));
            mockEfDbContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [TestMethod]
        public void Logger_LogServiceResult_SavesIfLoggingLevel1AndError()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.DBResults.Add(It.IsAny<DBResult>())).Returns(new DBResult());
            mockEfDbContext.Setup(x => x.SaveChanges()).Verifiable();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                UserName = "dummyUser",
                StatusDescription = "test descr"
            };
            var logger = new DBLogger(1, 0, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            mockEfDbContext.Verify(m => m.DBResults.Add(It.IsAny<DBResult>()), Times.Exactly(1));
            mockEfDbContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [TestMethod]
        public void Logger_LogServiceResult_AddsDTStartDTEndUserNameHostAddresAndGuid()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.DBResults.Add(It.IsAny<DBResult>())).Returns(new DBResult());
            mockEfDbContext.Setup(x => x.SaveChanges()).Verifiable();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                StatusDescription = "test descr"
            };
            var logger = new DBLogger(1, 0, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            Assert.IsTrue(result.UserName.Contains("_unknown_"));
            Assert.IsTrue(result.UserHostAddress.Contains("_unknown_"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(result.Id));
            Assert.IsTrue(result.DtStart > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(result.DtEnd > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Logger_LogServiceResult_SavesIfProcTooLong()
        {
            // Arrange
            var mockEfDbContext = new Mock<EFDbContext>();
            mockEfDbContext.Setup(x => x.DBResults.Add(It.IsAny<DBResult>())).Returns(new DBResult());
            mockEfDbContext.Setup(x => x.SaveChanges()).Verifiable();

            var result = new DBResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                ServiceName = "dummyService",
                ActionName = "dummyAction",
                ControllerName = "dummyController",
                StatusDescription = "test descr",
                DtStart = DateTime.Parse("2015-05-05 00:00:00"),
                DtEnd = DateTime.Parse("2015-05-05 00:00:01")
            };

            var logger = new DBLogger(1, 500, mockEfDbContext.Object);

            //Act
            logger.LogResult(result);

            // Assert
            Assert.IsTrue(result.StatusCode == HttpStatusCode.InternalServerError);
            Assert.IsTrue(result.StatusDescription.Contains("Process ran 1.000 seconds."));
        }
    }
}
