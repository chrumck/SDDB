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
    public class Tests_AssemblyTypeService
    {
        [TestMethod]
        public void AssemblyTypeService_GetAsync_ReturnsAssemblyTypes()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false, Comments = "DummyComments1" };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true, Comments = "DummyComments2" };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyTypes = assyTypeService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyTypes.Count == 1);
            Assert.IsTrue(resultAssemblyTypes[0].AssyTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void AssemblyTypeService_GetAsync_ReturnsAssemblyTypesByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive = true };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].AssyTypeAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void AssemblyTypeService_GetAsync_DoesNotReturnAssemblyTypesByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive = false };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyTypeService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyTypes = assyTypeService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyTypes.Count == 2);
            Assert.IsTrue(returnedAssemblyTypes[1].AssyTypeAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void AssemblyTypeService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyTypes = assyTypeService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyTypes.Count == 1);
            Assert.IsTrue(returnedAssemblyTypes[0].AssyTypeName.Contains("Name2"));
        }

        [TestMethod]
        public void AssemblyTypeService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssemblyTypes = assyTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyTypeService_EditAsync_DoesNothingIfNoAssemblyType()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyTypes = new AssemblyType[] { };

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.EditAsync(assyTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void AssemblyTypeService_EditAsync_CreatesAssemblyTypeIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyType1 = new AssemblyType { Id = initialId, AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var assyTypes = new AssemblyType[] { assyType1 };

            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync(assyType1.Id)).Returns(Task.FromResult<AssemblyType>(null));
            mockEfDbContext.Setup(x => x.AssemblyTypes.Add(assyType1)).Returns(assyType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.EditAsync(assyTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyType1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.Add(assyType1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyTypeService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyType1 = new AssemblyType { Id = "dummyAssemblyTypeId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false,
                                      ModifiedProperties = new string[] { "AssyTypeName", "AssyTypeAltName" }};
            var assyType2 = new AssemblyType { Id = "dummyAssemblyTypeId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive = true };
            var assyTypes = new AssemblyType[] { assyType1, assyType2 };

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "DbEntry1", AssyTypeAltName = "DbEntryAlt1", IsActive = true };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "DbEntry2", AssyTypeAltName = "DbEntryAlt2", IsActive = false };

            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync(assyType1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync(assyType2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyTypes.Add(assyType1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.EditAsync(assyTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assyType1.AssyTypeName == dbEntry1.AssyTypeName); Assert.IsTrue(assyType1.AssyTypeAltName == dbEntry1.AssyTypeAltName);
            Assert.IsTrue(assyType1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(assyType2.AssyTypeName != dbEntry2.AssyTypeName); Assert.IsTrue(assyType2.AssyTypeAltName != dbEntry2.AssyTypeAltName);
            Assert.IsTrue(assyType2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyTypes.Add(It.IsAny<AssemblyType>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyTypeService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyType1 = new AssemblyType { Id = initialId, AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive = false };
            var assyTypes = new AssemblyType[] { assyType1 };

            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyType>(null));
            mockEfDbContext.Setup(x => x.AssemblyTypes.Add(assyType1)).Returns(assyType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.EditAsync(assyTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyType1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyTypeService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new AssemblyType { IsActive = true };
            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync("dummyId2")).Returns(Task.FromResult<AssemblyType>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyTypeService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1" };

            var dbEntry = new AssemblyType { IsActive = true };
            mockEfDbContext.Setup(x => x.AssemblyTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyTypeService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyTypes.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
