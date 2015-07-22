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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedLocationTypes = locationTypeService.LookupAsync("NameAlt2", true).Result;

            //Assert
            Assert.IsTrue(returnedLocationTypes.Count == 1);
            Assert.IsTrue(returnedLocationTypes[0].LocTypeName.Contains("Name2"));
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

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedLocationTypes = locationTypeService.LookupAsync("NameAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedLocationTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void LocationTypeService_EditAsync_DoesNothingIfNoLocationType()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var locationTypes = new LocationType[] { };

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.EditAsync(locationTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void LocationTypeService_EditAsync_CreatesLocationTypeIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var locationType1 = new LocationType { Id = initialId, LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var locationTypes = new LocationType[] { locationType1 };

            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync(locationType1.Id)).Returns(Task.FromResult<LocationType>(null));
            mockEfDbContext.Setup(x => x.LocationTypes.Add(locationType1)).Returns(locationType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.EditAsync(locationTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(locationType1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.Add(locationType1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void LocationTypeService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var locationType1 = new LocationType { Id = "dummyLocationTypeId1", LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "LocTypeName", "LocTypeAltName" }};
            var locationType2 = new LocationType { Id = "dummyLocationTypeId2", LocTypeName = "Name2", LocTypeAltName = "NameAlt2", IsActive_bl = true };
            var locationTypes = new LocationType[] { locationType1, locationType2 };

            var dbEntry1 = new LocationType { Id = "dummyEntryId1", LocTypeName = "DbEntry1", LocTypeAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new LocationType { Id = "dummyEntryId2", LocTypeName = "DbEntry2", LocTypeAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync(locationType1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync(locationType2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.LocationTypes.Add(locationType1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.EditAsync(locationTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(locationType1.LocTypeName == dbEntry1.LocTypeName); Assert.IsTrue(locationType1.LocTypeAltName == dbEntry1.LocTypeAltName);
            Assert.IsTrue(locationType1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(locationType2.LocTypeName != dbEntry2.LocTypeName); Assert.IsTrue(locationType2.LocTypeAltName != dbEntry2.LocTypeAltName);
            Assert.IsTrue(locationType2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.LocationTypes.Add(It.IsAny<LocationType>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void LocationTypeService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var locationType1 = new LocationType { Id = initialId, LocTypeName = "Name1", LocTypeAltName = "NameAlt1", IsActive_bl = false };
            var locationTypes = new LocationType[] { locationType1 };

            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<LocationType>(null));
            mockEfDbContext.Setup(x => x.LocationTypes.Add(locationType1)).Returns(locationType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.EditAsync(locationTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(locationType1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void LocationTypeService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new LocationType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync("dummyId2")).Returns(Task.FromResult<LocationType>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void LocationTypeService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1" };

            var dbEntry = new LocationType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.LocationTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var locationTypeService = new LocationTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationTypeService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.LocationTypes.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
