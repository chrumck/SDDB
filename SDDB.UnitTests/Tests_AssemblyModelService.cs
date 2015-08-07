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
    public class Tests_AssemblyModelService
    {
        [TestMethod]
        public void AssemblyModelService_GetAsync_ReturnsAssemblyModels()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var resultAssemblyModels = assyModelService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyModels.Count == 1);
            Assert.IsTrue(resultAssemblyModels[0].AssyModelAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void AssemblyModelService_GetAsync_ReturnsAssemblyModelsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyModel { Id = "dummyEntryId3", AssyModelName = "Name3", AssyModelAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = assyModelService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].AssyModelAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void AssemblyModelService_GetAsync_DoesNotReturnAssemblyModelsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyModel { Id = "dummyEntryId3", AssyModelName = "Name3", AssyModelAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = assyModelService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyModelService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyModel { Id = "dummyEntryId3", AssyModelName = "Name3", AssyModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedAssemblyModels = assyModelService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyModels.Count == 2);
            Assert.IsTrue(returnedAssemblyModels[1].AssyModelAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void AssemblyModelService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyModel { Id = "dummyEntryId3", AssyModelName = "Name3", AssyModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedAssemblyModels = assyModelService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyModels.Count == 1);
            Assert.IsTrue(returnedAssemblyModels[0].AssyModelName.Contains("Name2"));
        }

        [TestMethod]
        public void AssemblyModelService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyModel { Id = "dummyEntryId3", AssyModelName = "Name3", AssyModelAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyModel> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyModel>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyModel>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyModel>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyModel>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyModel>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyModels).Returns(mockDbSet.Object);

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var returnedAssemblyModels = assyModelService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyModels.Count == 0);
        }

                       

                
    }
}
