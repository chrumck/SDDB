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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

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

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyModels = assyModelService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyModels.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyModelService_EditAsync_DoesNothingIfNoAssemblyModel()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyModels = new AssemblyModel[] { };

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.EditAsync(assyModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void AssemblyModelService_EditAsync_CreatesAssemblyModelIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyModel1 = new AssemblyModel { Id = initialId, AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var assyModels = new AssemblyModel[] { assyModel1 };

            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync(assyModel1.Id)).Returns(Task.FromResult<AssemblyModel>(null));
            mockEfDbContext.Setup(x => x.AssemblyModels.Add(assyModel1)).Returns(assyModel1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.EditAsync(assyModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyModel1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.Add(assyModel1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyModelService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyModel1 = new AssemblyModel { Id = "dummyAssemblyModelId1", AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "AssyModelName", "AssyModelAltName" }};
            var assyModel2 = new AssemblyModel { Id = "dummyAssemblyModelId2", AssyModelName = "Name2", AssyModelAltName = "NameAlt2", IsActive_bl = true };
            var assyModels = new AssemblyModel[] { assyModel1, assyModel2 };

            var dbEntry1 = new AssemblyModel { Id = "dummyEntryId1", AssyModelName = "DbEntry1", AssyModelAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new AssemblyModel { Id = "dummyEntryId2", AssyModelName = "DbEntry2", AssyModelAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync(assyModel1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync(assyModel2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyModels.Add(assyModel1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.EditAsync(assyModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assyModel1.AssyModelName == dbEntry1.AssyModelName); Assert.IsTrue(assyModel1.AssyModelAltName == dbEntry1.AssyModelAltName);
            Assert.IsTrue(assyModel1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assyModel2.AssyModelName != dbEntry2.AssyModelName); Assert.IsTrue(assyModel2.AssyModelAltName != dbEntry2.AssyModelAltName);
            Assert.IsTrue(assyModel2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyModels.Add(It.IsAny<AssemblyModel>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyModelService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyModel1 = new AssemblyModel { Id = initialId, AssyModelName = "Name1", AssyModelAltName = "NameAlt1", IsActive_bl = false };
            var assyModels = new AssemblyModel[] { assyModel1 };

            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyModel>(null));
            mockEfDbContext.Setup(x => x.AssemblyModels.Add(assyModel1)).Returns(assyModel1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.EditAsync(assyModels).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyModel1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyModelService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new AssemblyModel { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync("dummyId2")).Returns(Task.FromResult<AssemblyModel>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyModelService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1" };

            var dbEntry = new AssemblyModel { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.AssemblyModels.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyModelService = new AssemblyModelService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyModelService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyModels.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
