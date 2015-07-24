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
    public class Tests_ComponentTypeService
    {
        [TestMethod]
        public void ComponentTypeService_GetAsync_ReturnsComponentTypes()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var resultComponentTypes = compTypeService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultComponentTypes.Count == 1);
            Assert.IsTrue(resultComponentTypes[0].CompTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void ComponentTypeService_GetAsync_ReturnsComponentTypesByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentType { Id = "dummyEntryId3", CompTypeName = "Name3", CompTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].CompTypeAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void ComponentTypeService_GetAsync_DoesNotReturnComponentTypesByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentType { Id = "dummyEntryId3", CompTypeName = "Name3", CompTypeAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentTypeService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentType { Id = "dummyEntryId3", CompTypeName = "Name3", CompTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentTypes = compTypeService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentTypes.Count == 2);
            Assert.IsTrue(returnedComponentTypes[1].CompTypeAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void ComponentTypeService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentType { Id = "dummyEntryId3", CompTypeName = "Name3", CompTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentTypes = compTypeService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentTypes.Count == 1);
            Assert.IsTrue(returnedComponentTypes[0].CompTypeName.Contains("Name2"));
        }

        [TestMethod]
        public void ComponentTypeService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new ComponentType { Id = "dummyEntryId3", CompTypeName = "Name3", CompTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<ComponentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentType>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.ComponentTypes).Returns(mockDbSet.Object);

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedComponentTypes = compTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentTypeService_EditAsync_DoesNothingIfNoComponentType()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compTypes = new ComponentType[] { };

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.EditAsync(compTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentTypes.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentTypeService_EditAsync_CreatesComponentTypeIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var compType1 = new ComponentType { Id = initialId, CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false };
            var compTypes = new ComponentType[] { compType1 };

            mockEfDbContext.Setup(x => x.ComponentTypes.FindAsync(compType1.Id)).Returns(Task.FromResult<ComponentType>(null));
            mockEfDbContext.Setup(x => x.ComponentTypes.Add(compType1)).Returns(compType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.EditAsync(compTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(compType1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentTypes.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentTypes.Add(compType1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentTypeService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compType1 = new ComponentType { Id = "dummyRecordTypeId1", CompTypeName = "Name1", CompTypeAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "CompTypeName", "CompTypeAltName" }};
            var compType2 = new ComponentType { Id = "dummyRecordTypeId2", CompTypeName = "Name2", CompTypeAltName = "NameAlt2", IsActive_bl = true };
            var compTypes = new ComponentType[] { compType1, compType2 };

            var dbEntry1 = new ComponentType { Id = "dummyEntryId1", CompTypeName = "DbEntry1", CompTypeAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new ComponentType { Id = "dummyEntryId2", CompTypeName = "DbEntry2", CompTypeAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.ComponentTypes.FindAsync(compType1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentTypes.FindAsync(compType2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ComponentTypes.Add(compType1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.EditAsync(compTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(compType1.CompTypeName == dbEntry1.CompTypeName); Assert.IsTrue(compType1.CompTypeAltName == dbEntry1.CompTypeAltName);
            Assert.IsTrue(compType1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(compType2.CompTypeName != dbEntry2.CompTypeName); Assert.IsTrue(compType2.CompTypeAltName != dbEntry2.CompTypeAltName);
            Assert.IsTrue(compType2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.ComponentTypes.Add(It.IsAny<ComponentType>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentTypeService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new ComponentType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.ComponentTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.ComponentTypes.FindAsync("dummyId2")).Returns(Task.FromResult<ComponentType>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compTypeService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        

    }
}
