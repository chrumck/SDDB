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
    public class Tests_LocationTypeService
    {
        [TestMethod]
        public void LocationTypeService_GetAsync_ReturnsLocationTypes()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var resultLocationTypes = locationTypeService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultLocationTypes.Count == 1);
            Assert.IsTrue(resultLocationTypes[0].LocTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void LocationTypeService_GetAsync_ReturnsLocationTypesByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new LocationType { Id = "dummyEntryId3", LocTypeName = "Name3", LocTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var serviceResult = locationTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LocTypeAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void LocationTypeService_GetAsync_DoesNotReturnLocationTypesByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new LocationType { Id = "dummyEntryId3", LocTypeName = "Name3", LocTypeAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var serviceResult = locationTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void LocationTypeService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new LocationType { Id = "dummyEntryId3", LocTypeName = "Name3", LocTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var returnedLocationTypes = locationTypeService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedLocationTypes.Count == 2);
            Assert.IsTrue(returnedLocationTypes[1].LocTypeAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void LocationTypeService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new LocationType { Id = "dummyEntryId3", LocTypeName = "Name3", LocTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var returnedLocationTypes = locationTypeService.LookupAsync("Name2", true).Result;

            //Assert
            Assert.IsTrue(returnedLocationTypes.Count == 1);
            Assert.IsTrue(returnedLocationTypes[0].LocTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void LocationTypeService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new LocationType { Id = "dummyEntryId3", LocTypeName = "Name3", LocTypeAltName = "NameAlt3", IsActive_bl = true };

            var dbEntries = (new List<LocationType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<LocationType>>();
            mockDbSet.As<IDbAsyncEnumerable<LocationType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<LocationType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<LocationType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<LocationType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.LocationTypes).Returns(mockDbSet.Object);

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object, "dummyuserId");

            //Act
            var returnedLocationTypes = locationTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedLocationTypes.Count == 0);
        }

        

        
        
    }
}
