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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultLocations = locationService.GetAsync(new[] {dbEntry1.Id, dbEntry2.Id}).Result;
            
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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultLocations = locationService.GetAsync(new[] { dbEntry1.Id, dbEntry2.Id }).Result;
            
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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = locationService.GetAsync(new string[] { "dummyEntryId3" }).Result;

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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = locationService.GetAsync(new[] { "dummyEntryId3" }).Result;

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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedLocs = locationService.LookupAsync("", true).Result;

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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedLocs = locationService.LookupAsync("Loc1", true).Result;

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

            var locationService = new LocationService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedLocs = locationService.LookupAsync("Loc3", true).Result;

            //Assert
            Assert.IsTrue(returnedLocs.Count == 0);
        }




    }
}
