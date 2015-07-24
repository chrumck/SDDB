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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

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

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentModels = compModelService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentModels.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentModelService_EditAsync_DoesNothingIfNoComponentModel()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compModels = new ComponentModel[] { };

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compModelService.EditAsync(compModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentModels.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentModelService_EditAsync_CreatesComponentModelIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var compModel1 = new ComponentModel { Id = initialId, CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false };
            var compModels = new ComponentModel[] { compModel1 };

            mockEfDbContext.Setup(x => x.ComponentModels.FindAsync(compModel1.Id)).Returns(Task.FromResult<ComponentModel>(null));
            mockEfDbContext.Setup(x => x.ComponentModels.Add(compModel1)).Returns(compModel1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compModelService.EditAsync(compModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(compModel1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentModels.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentModels.Add(compModel1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentModelService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compModel1 = new ComponentModel { Id = "dummyComponentModelId1", CompModelName = "Name1", CompModelAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "CompModelName", "CompModelAltName" }};
            var compModel2 = new ComponentModel { Id = "dummyComponentModelId2", CompModelName = "Name2", CompModelAltName = "NameAlt2", IsActive_bl = true };
            var compModels = new ComponentModel[] { compModel1, compModel2 };

            var dbEntry1 = new ComponentModel { Id = "dummyEntryId1", CompModelName = "DbEntry1", CompModelAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new ComponentModel { Id = "dummyEntryId2", CompModelName = "DbEntry2", CompModelAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.ComponentModels.FindAsync(compModel1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentModels.FindAsync(compModel2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ComponentModels.Add(compModel1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compModelService.EditAsync(compModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(compModel1.CompModelName == dbEntry1.CompModelName); Assert.IsTrue(compModel1.CompModelAltName == dbEntry1.CompModelAltName);
            Assert.IsTrue(compModel1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(compModel2.CompModelName != dbEntry2.CompModelName); Assert.IsTrue(compModel2.CompModelAltName != dbEntry2.CompModelAltName);
            Assert.IsTrue(compModel2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentModels.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.ComponentModels.Add(It.IsAny<ComponentModel>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentModelService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new ComponentModel { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.ComponentModels.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.ComponentModels.FindAsync("dummyId2")).Returns(Task.FromResult<ComponentModel>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compModelService = new ComponentModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compModelService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentModels.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


    }
}
