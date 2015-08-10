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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object,"DummyUserId");

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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object, "DummyUserId");

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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object, "DummyUserId");

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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object, "DummyUserId");

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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object, "DummyUserId");

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

            var compStatusService = new ComponentStatusService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedComponentStatuss = compStatusService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentStatuss.Count == 0);
        }


        

    }
}
