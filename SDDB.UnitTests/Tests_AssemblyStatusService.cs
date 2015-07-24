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
    [TestClass]
    public class Tests_AssemblyStatusService
    {
        [TestMethod]
        public void AssemblyStatusService_GetAsync_ReturnsAssemblyStatuss()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyStatuss = assyStatusService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyStatuss.Count == 1);
            Assert.IsTrue(resultAssemblyStatuss[0].AssyStatusAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void AssemblyStatusService_GetAsync_ReturnsAssemblyStatussByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyStatus { Id = "dummyEntryId3", AssyStatusName = "Name3", AssyStatusAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].AssyStatusAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void AssemblyStatusService_GetAsync_DoesNotReturnAssemblyStatussByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyStatus { Id = "dummyEntryId3", AssyStatusName = "Name3", AssyStatusAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyStatusService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyStatus { Id = "dummyEntryId3", AssyStatusName = "Name3", AssyStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyStatuss = assyStatusService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyStatuss.Count == 2);
            Assert.IsTrue(returnedAssemblyStatuss[1].AssyStatusAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void AssemblyStatusService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyStatus { Id = "dummyEntryId3", AssyStatusName = "Name3", AssyStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyStatuss = assyStatusService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyStatuss.Count == 1);
            Assert.IsTrue(returnedAssemblyStatuss[0].AssyStatusName.Contains("Name2"));
        }

        [TestMethod]
        public void AssemblyStatusService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyStatus { Id = "dummyEntryId3", AssyStatusName = "Name3", AssyStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyStatuss).Returns(mockDbSet.Object);

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyStatuss = assyStatusService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyStatuss.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyStatusService_EditAsync_DoesNothingIfNoAssemblyStatus()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyStatuss = new AssemblyStatus[] { };

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.EditAsync(assyStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyStatuss.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void AssemblyStatusService_EditAsync_CreatesAssemblyStatusIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyStatus1 = new AssemblyStatus { Id = initialId, AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false };
            var assyStatuss = new AssemblyStatus[] { assyStatus1 };

            mockEfDbContext.Setup(x => x.AssemblyStatuss.FindAsync(assyStatus1.Id)).Returns(Task.FromResult<AssemblyStatus>(null));
            mockEfDbContext.Setup(x => x.AssemblyStatuss.Add(assyStatus1)).Returns(assyStatus1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.EditAsync(assyStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyStatus1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyStatuss.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyStatuss.Add(assyStatus1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyStatusService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyStatus1 = new AssemblyStatus { Id = "dummyAssemblyStatusId1", AssyStatusName = "Name1", AssyStatusAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "AssyStatusName", "AssyStatusAltName" }};
            var assyStatus2 = new AssemblyStatus { Id = "dummyAssemblyStatusId2", AssyStatusName = "Name2", AssyStatusAltName = "NameAlt2", IsActive_bl = true };
            var assyStatuss = new AssemblyStatus[] { assyStatus1, assyStatus2 };

            var dbEntry1 = new AssemblyStatus { Id = "dummyEntryId1", AssyStatusName = "DbEntry1", AssyStatusAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new AssemblyStatus { Id = "dummyEntryId2", AssyStatusName = "DbEntry2", AssyStatusAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.AssemblyStatuss.FindAsync(assyStatus1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyStatuss.FindAsync(assyStatus2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyStatuss.Add(assyStatus1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.EditAsync(assyStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assyStatus1.AssyStatusName == dbEntry1.AssyStatusName); Assert.IsTrue(assyStatus1.AssyStatusAltName == dbEntry1.AssyStatusAltName);
            Assert.IsTrue(assyStatus1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assyStatus2.AssyStatusName != dbEntry2.AssyStatusName); Assert.IsTrue(assyStatus2.AssyStatusAltName != dbEntry2.AssyStatusAltName);
            Assert.IsTrue(assyStatus2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyStatuss.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyStatuss.Add(It.IsAny<AssemblyStatus>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyStatusService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new AssemblyStatus { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.AssemblyStatuss.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.AssemblyStatuss.FindAsync("dummyId2")).Returns(Task.FromResult<AssemblyStatus>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyStatusService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyStatuss.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        
    }
}
