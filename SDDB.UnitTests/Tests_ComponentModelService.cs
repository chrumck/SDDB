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
    public class Tests_ComponentModelService
    {
        [TestMethod]
        public void ComponentModelService_GetAsync_ReturnsComponentModels()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var resultComponentModels = compModelService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultComponentModels.Count == 1);
            Assert.IsTrue(resultComponentModels[0].CompModelAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void ComponentModelService_GetAsync_ReturnsComponentModelsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentModel { Id = "dummyEntryId3", CompModelName = "Name3", CompModelAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = compModelService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].CompModelAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void ComponentModelService_GetAsync_DoesNotReturnComponentModelsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentModel { Id = "dummyEntryId3", CompModelName = "Name3", CompModelAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = compModelService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentModelService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentModel { Id = "dummyEntryId3", CompModelName = "Name3", CompModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedComponentModels = compModelService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentModels.Count == 2);
            Assert.IsTrue(returnedComponentModels[1].CompModelAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void ComponentModelService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentModel { Id = "dummyEntryId3", CompModelName = "Name3", CompModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedComponentModels = compModelService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentModels.Count == 1);
            Assert.IsTrue(returnedComponentModels[0].CompModelName.Contains("Name2"));
        }

        [TestMethod]
        public void ComponentModelService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentModel { Id = "dummyEntryId3", CompModelName = "Name3", CompModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentModel>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentModels).Returns(mockDbSet.Object);

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedComponentModels = compModelService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentModels.Count == 0);
        }

                


    }
}
