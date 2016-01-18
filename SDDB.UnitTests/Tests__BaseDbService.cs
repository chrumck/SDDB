using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mehdime.Entity;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace SDDB.UnitTests
{
    public class TestBaseDbService : BaseDbService<PersonLogEntry>
    {
        public TestBaseDbService(IDbContextScopeFactory contextScopeFac, string userId) : base(contextScopeFac, userId) { }
       
    }
    
    [TestClass]
    public class Tests__BaseDbService
    {
        [TestMethod]
        public void BaseDbService_EditAsync_CreatesDbEntryIfNotInDb()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";

            var dbEntry1 = new PersonLogEntry
            {
                Id = userId,
                LogEntryDateTime = new DateTime(2000, 1, 1),
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
                IsActive_bl = false,
            };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockDbSet.Setup(x => x.FindAsync(dbEntry1.Id)).Returns(Task.FromResult<PersonLogEntry>(null));
            mockDbSet.Setup(x => x.Add(dbEntry1)).Returns(dbEntry1);

            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>().FindAsync(userId)).Returns(Task.FromResult<PersonLogEntry>(null));
            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>().Add(dbEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var derivedTestService = new TestBaseDbService(mockDbContextScopeFac.Object, userId);

            //Act
            var newIdsList = derivedTestService.EditAsync(dbEntries.ToArray()).Result;

            //Assert
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*");
            Assert.IsTrue(regex.IsMatch(dbEntry1.Id));
            Assert.IsTrue(dbEntry1.LastSavedByPerson_Id == userId);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<PersonLogEntry>().FindAsync(userId), Times.Once);
            mockEfDbContext.Verify(x => x.Set<PersonLogEntry>().Add(dbEntry1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void BaseDbService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";
            
            var record1 = new PersonLogEntry
            {
                Id = "dummyRecordId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
                ModifiedProperties = new[] { "LogEntryDateTime", "IsActive_bl" }
            };
            var record2 = new PersonLogEntry
            {
                Id = "dummyRecordId2",
                LogEntryDateTime = new DateTime(2000, 1, 2),
                IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID"
            };
            var records = new PersonLogEntry[] { record1, record2 };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>().FindAsync(record1.Id)).Returns(Task.FromResult((PersonLogEntry)dbEntry1));
            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>().FindAsync(record2.Id)).Returns(Task.FromResult((PersonLogEntry)dbEntry2));
            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>().Add(It.IsAny<PersonLogEntry>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var derivedTestService = new TestBaseDbService(mockDbContextScopeFac.Object, userId);

            //Act
            var newIdsList = derivedTestService.EditAsync(records).Result;

            //Assert
            Assert.IsTrue(record1.LogEntryDateTime == dbEntry1.LogEntryDateTime);
            Assert.IsTrue(record1.IsActive_bl == dbEntry1.IsActive_bl);
            Assert.IsTrue(record2.LogEntryDateTime != dbEntry2.LogEntryDateTime);
            Assert.IsTrue(record2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<PersonLogEntry>().FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Set<PersonLogEntry>().Add(It.IsAny<PersonLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        

        ////-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void BaseDbService_DeleteAsync_DeletesRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";

            var recordIds = new string[] { "dummyId1", "dummyId2" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyId1", IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyId2", IsActive_bl = false };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Set<PersonLogEntry>()).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var derivedTestService = new TestBaseDbService(mockDbContextScopeFac.Object, userId);

            //Act
            derivedTestService.DeleteAsync(recordIds).Wait();

            //Assert
            Assert.IsTrue(dbEntry1.IsActive_bl == false);
            Assert.IsTrue(dbEntry2.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        // not able to unit test BaseDbService_AddRemoveRelated because of Moq limitation:
        // NotSupportedException: Conversion between generic and non-generic DbSet objects is not supported for test doubles.



    }
}

        