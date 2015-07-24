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
    public class Tests_LocationService
    {
        [TestMethod]
        public void LocationService_GetAsync_ReturnsLocations()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, Comments = "DummyComments1",
                AccessInfo = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true, Comments = "DummyComments2",
                AccessInfo = "DummyInfo2", AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var resultLocations = locationService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultLocations.Count == 1);
            Assert.IsTrue(resultLocations[0].LocAltName.Contains("LocAlt2"));
        }

        [TestMethod]
        public void LocationService_GetAsync_DoeNotReturnLocationsFromWrongProject()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var projectPerson2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, Comments = "DummyComments1",
                AccessInfo = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true, Comments = "DummyComments2",
                AccessInfo = "DummyInfo2", AssignedToProject = project2 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var resultLocations = locationService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultLocations.Count == 0);
        }

        [TestMethod]
        public void LocationService_GetAsync_ReturnsLocationsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, 
                AccessInfo = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true,
                AccessInfo = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Location { Id = "dummyEntryId3", LocName = "Loc3", LocAltName = "LocAlt3", IsActive_bl = true, 
                AccessInfo = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LocAltName.Contains("LocAlt3"));

        }

        [TestMethod]
        public void LocationService_GetAsync_DoesNotReturnLocationsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, 
                AccessInfo = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true,
                AccessInfo = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Location { Id = "dummyEntryId3", LocName = "Loc3", LocAltName = "LocAlt3", IsActive_bl = false, 
                AccessInfo = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void LocationService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = true, 
                AccessInfo = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true,
                AccessInfo = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Location { Id = "dummyEntryId3", LocName = "Loc3", LocAltName = "LocAlt3", IsActive_bl = false, 
                AccessInfo = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var returnedLocs = locationService.LookupAsync(projectPerson1.Id,"", true).Result;

            //Assert
            Assert.IsTrue(returnedLocs.Count == 2);
            Assert.IsTrue(returnedLocs[1].LocAltName.Contains("LocAlt2"));
        }

        [TestMethod]
        public void LocationService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = true, 
                AccessInfo = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true,
                AccessInfo = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Location { Id = "dummyEntryId3", LocName = "Loc3", LocAltName = "LocAlt3", IsActive_bl = false, 
                AccessInfo = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var returnedLocs = locationService.LookupAsync(projectPerson1.Id,"Loc1", true).Result;

            //Assert
            Assert.IsTrue(returnedLocs.Count == 1);
            Assert.IsTrue(returnedLocs[0].LocAltName.Contains("LocAlt1"));
        }

        [TestMethod]
        public void LocationService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

                        var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = true, 
                AccessInfo = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true,
                AccessInfo = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Location { Id = "dummyEntryId3", LocName = "Loc3", LocAltName = "LocAlt3", IsActive_bl = false, 
                AccessInfo = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Location> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Location>>();
            mockDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Locations).Returns(mockDbSet.Object);

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var returnedLocs = locationService.LookupAsync(projectPerson1.Id,"Loc3", true).Result;

            //Assert
            Assert.IsTrue(returnedLocs.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void LocationService_EditAsync_DoesNothingIfNoLocation()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var locations = new Location[] { };

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.EditAsync(locations).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void LocationService_EditAsync_CreatesLocationIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var location1 = new Location { Id = initialId, LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, AccessInfo = "DummyInfo1" };
            var locations = new Location[] { location1 };

            mockEfDbContext.Setup(x => x.Locations.FindAsync(location1.Id)).Returns(Task.FromResult<Location>(null));
            mockEfDbContext.Setup(x => x.Locations.Add(location1)).Returns(location1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.EditAsync(locations).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(location1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.Add(location1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void LocationService_EditAsync_CreatesManyLocationsIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "newEntryId1";
            var location1 = new Location { Id = initialId, LocName = "Loc", IsActive_bl = true, AccessInfo = "DummyInfo" };
            var location2 = new Location { Id = initialId, LocName = "Loc", IsActive_bl = false, AccessInfo = "DummyInfo" };
            var location3 = new Location { Id = initialId, LocName = "Loc", IsActive_bl = true, AccessInfo = "DummyInfo" };
            var locations = new Location[] { location1, location2, location3 };

            mockEfDbContext.Setup(x => x.Locations.FindAsync(It.IsAny<Location>())).Returns(Task.FromResult<Location>(null));
            mockEfDbContext.Setup(x => x.Locations.Add(It.IsAny<Location>())).Returns((Location x) => x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.EditAsync(locations).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); 
            Assert.IsTrue(regex.IsMatch(location1.Id)); Assert.IsTrue(regex.IsMatch(location2.Id)); Assert.IsTrue(regex.IsMatch(location3.Id));
            Assert.IsTrue(location2.LocName.Contains("002")); Assert.IsTrue(location3.LocAltName.Contains("003"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.FindAsync(initialId), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.Locations.Add(It.IsAny<Location>()), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        [TestMethod]
        public void LocationService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var location1 = new Location { Id = "dummyLocationId1", LocName = "Loc1", LocAltName = "LocAlt1", IsActive_bl = false, AccessInfo = "DummyInfo1",
                                      ModifiedProperties = new string[] { "LocName", "LocAltName" }};
            var location2 = new Location { Id = "dummyLocationId2", LocName = "Loc2", LocAltName = "LocAlt2", IsActive_bl = true, AccessInfo = "DummyInfo2" };
            var locations = new Location[] { location1, location2 };

            var dbEntry1 = new Location { Id = "dummyEntryId1", LocName = "DbEntry1", LocAltName = "DbEntryAlt1", IsActive_bl = true, AccessInfo = "DbEntryInfo1" };
            var dbEntry2 = new Location { Id = "dummyEntryId2", LocName = "DbEntry2", LocAltName = "DbEntryAlt2", IsActive_bl = false, AccessInfo = "DbEntryInfo2" };

            mockEfDbContext.Setup(x => x.Locations.FindAsync(location1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(location2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Locations.Add(location1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.EditAsync(locations).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(location1.LocName == dbEntry1.LocName); Assert.IsTrue(location1.LocAltName == dbEntry1.LocAltName);
            Assert.IsTrue(location1.AccessInfo != dbEntry1.AccessInfo); Assert.IsTrue(location1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(location2.LocName != dbEntry2.LocName); Assert.IsTrue(location2.LocAltName != dbEntry2.LocAltName);
            Assert.IsTrue(location2.AccessInfo != dbEntry2.AccessInfo); Assert.IsTrue(location2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Locations.Add(It.IsAny<Location>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
               

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void LocationService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var locationIds = new string[] { "dummyId1", "DummyId2" };

            var assyDbEntry1 = new AssemblyDb { Id = "assyDummyId1", AssyName = "Name1", AssyAltName = "AltName1", IsActive_bl = true, AssignedToProject_Id = "assignedProjId" };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockAssyDbSet = new Mock<DbSet<AssemblyDb>>();
            mockAssyDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockAssyDbSet.Object);

            var dbEntry = new Location { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.Locations.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.Locations.FindAsync("dummyId2")).Returns(Task.FromResult<Location>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var locationService = new LocationService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = locationService.DeleteAsync(locationIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Locations.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

    }
}
