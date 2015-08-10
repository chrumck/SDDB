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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var compTypeService = new ComponentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedComponentTypes = compTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedComponentTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        

    }
}
