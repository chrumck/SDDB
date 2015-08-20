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
    public class Tests_ProjectEventService
    {
        [TestMethod]
        public void ProjectEventService_GetAsync_ReturnsProjectEvents()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = false, Comments = "DummyComments1",
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true, Comments = "DummyComments2",
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultProjectEvents = eventService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultProjectEvents.Count == 1);
            Assert.IsTrue(resultProjectEvents[0].EventAltName.Contains("EventAlt2"));
        }

        [TestMethod]
        public void ProjectEventService_GetAsync_DoeNotReturnProjectEventsFromWrongProject()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = false, Comments = "DummyComments1",
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true, Comments = "DummyComments2",
                 AssignedToProject = project2 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultProjectEvents = eventService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultProjectEvents.Count == 0);
        }

        [TestMethod]
        public void ProjectEventService_GetAsync_ReturnsProjectEventsByIds()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive_bl = true, 
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = eventService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].EventAltName.Contains("EventAlt3"));

        }

        [TestMethod]
        public void ProjectEventService_GetAsync_DoesNotReturnProjectEventsByIdsIfNotActive()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = eventService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }
                
        [TestMethod]
        public void ProjectEventService_LookupAsync_ReturnsAllRecords()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedEvents = eventService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedEvents.Count == 2);
            Assert.IsTrue(returnedEvents[1].EventAltName.Contains("EventAlt2"));
        }

        [TestMethod]
        public void ProjectEventService_LookupAsync_ReturnsMatchingRecords()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedEvents = eventService.LookupAsync("Event1", true).Result;

            //Assert
            Assert.IsTrue(returnedEvents.Count == 1);
            Assert.IsTrue(returnedEvents[0].EventAltName.Contains("EventAlt1"));
        }

        [TestMethod]
        public void ProjectEventService_LookupAsync_ReturnsNoRecordsIfNotActive()
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

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive_bl = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive_bl = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive_bl = false, 
                 AssignedToProject = project1 };
            var dbEntries = (new List<ProjectEvent> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ProjectEvent>>();
            mockDbSet.As<IDbAsyncEnumerable<ProjectEvent>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ProjectEvent>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ProjectEvent>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ProjectEvent>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.ProjectEvents).Returns(mockDbSet.Object);

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedEvents = eventService.LookupAsync("Event3", true).Result;

            //Assert
            Assert.IsTrue(returnedEvents.Count == 0);
        }

        

    }
}
