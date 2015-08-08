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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var assyStatusService = new AssemblyStatusService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedAssemblyStatuss = assyStatusService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyStatuss.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------
                
        
    }
}
