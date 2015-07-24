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
    public class Tests_ComponentStatusService
    {
        [TestMethod]
        public void ComponentStatusService_GetAsync_ReturnsComponentStatuss()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var resultComponentStatuss = compStatusService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultComponentStatuss.Count == 1);
            Assert.IsTrue(resultComponentStatuss[0].CompStatusAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void ComponentStatusService_GetAsync_ReturnsComponentStatussByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentStatus { Id = "dummyEntryId3", CompStatusName = "Name3", CompStatusAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].CompStatusAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void ComponentStatusService_GetAsync_DoesNotReturnComponentStatussByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentStatus { Id = "dummyEntryId3", CompStatusName = "Name3", CompStatusAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentStatusService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentStatus { Id = "dummyEntryId3", CompStatusName = "Name3", CompStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentStatuss = compStatusService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentStatuss.Count == 2);
            Assert.IsTrue(returnedComponentStatuss[1].CompStatusAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void ComponentStatusService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentStatus { Id = "dummyEntryId3", CompStatusName = "Name3", CompStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentStatuss = compStatusService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentStatuss.Count == 1);
            Assert.IsTrue(returnedComponentStatuss[0].CompStatusName.Contains("Name2"));
        }

        [TestMethod]
        public void ComponentStatusService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentStatus { Id = "dummyEntryId3", CompStatusName = "Name3", CompStatusAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentStatus> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentStatus>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentStatus>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentStatus>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentStatus>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentStatus>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentStatuss).Returns(mockDbSet.Object);

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentStatuss = compStatusService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentStatuss.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentStatusService_EditAsync_DoesNothingIfNoComponentStatus()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compStatuss = new ComponentStatus[] { };

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.EditAsync(compStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentStatuss.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentStatusService_EditAsync_CreatesComponentStatusIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var compStatus1 = new ComponentStatus { Id = initialId, CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false };
            var compStatuss = new ComponentStatus[] { compStatus1 };

            mockEfDbContext.Setup(x => x.ComponentStatuss.FindAsync(compStatus1.Id)).Returns(Task.FromResult<ComponentStatus>(null));
            mockEfDbContext.Setup(x => x.ComponentStatuss.Add(compStatus1)).Returns(compStatus1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.EditAsync(compStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(compStatus1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentStatuss.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentStatuss.Add(compStatus1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentStatusService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compStatus1 = new ComponentStatus { Id = "dummyComponentStatusId1", CompStatusName = "Name1", CompStatusAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "CompStatusName", "CompStatusAltName" }};
            var compStatus2 = new ComponentStatus { Id = "dummyComponentStatusId2", CompStatusName = "Name2", CompStatusAltName = "NameAlt2", IsActive_bl = true };
            var compStatuss = new ComponentStatus[] { compStatus1, compStatus2 };

            var dbEntry1 = new ComponentStatus { Id = "dummyEntryId1", CompStatusName = "DbEntry1", CompStatusAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new ComponentStatus { Id = "dummyEntryId2", CompStatusName = "DbEntry2", CompStatusAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.ComponentStatuss.FindAsync(compStatus1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentStatuss.FindAsync(compStatus2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ComponentStatuss.Add(compStatus1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.EditAsync(compStatuss).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(compStatus1.CompStatusName == dbEntry1.CompStatusName); Assert.IsTrue(compStatus1.CompStatusAltName == dbEntry1.CompStatusAltName);
            Assert.IsTrue(compStatus1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(compStatus2.CompStatusName != dbEntry2.CompStatusName); Assert.IsTrue(compStatus2.CompStatusAltName != dbEntry2.CompStatusAltName);
            Assert.IsTrue(compStatus2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentStatuss.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.ComponentStatuss.Add(It.IsAny<ComponentStatus>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentStatusService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new ComponentStatus { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.ComponentStatuss.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.ComponentStatuss.FindAsync("dummyId2")).Returns(Task.FromResult<ComponentStatus>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compStatusService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentStatuss.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        

    }
}
