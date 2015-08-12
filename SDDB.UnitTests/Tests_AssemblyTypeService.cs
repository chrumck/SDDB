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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

            //Act
            var returnedAssemblyTypes = assyTypeService.LookupAsync("Name2", true).Result;

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

            var dbEntry1 = new AssemblyType { Id = "dummyEntryId1", AssyTypeName = "Name1", AssyTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new AssemblyType { Id = "dummyEntryId2", AssyTypeName = "Name2", AssyTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new AssemblyType { Id = "dummyEntryId3", AssyTypeName = "Name3", AssyTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<AssemblyType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyType>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyTypes).Returns(mockDbSet.Object);

            var assyTypeService = new AssemblyTypeService(mockDbContextScopeFac.Object,"dummyUserId");

            //Act
            var returnedAssemblyTypes = assyTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssemblyTypes.Count == 0);
        }


        
    }
}
