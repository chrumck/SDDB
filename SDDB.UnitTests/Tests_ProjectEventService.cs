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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = false, Comments = "DummyComments1",
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true, Comments = "DummyComments2",
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var resultProjectEvents = eventService.GetAsync(projectPerson1.Id).Result;
            
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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = false, Comments = "DummyComments1",
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true, Comments = "DummyComments2",
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var resultProjectEvents = eventService.GetAsync(projectPerson1.Id).Result;
            
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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = false, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive = true, 
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = false, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive = false, 
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive = false, 
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var returnedEvents = eventService.LookupAsync(projectPerson1.Id,"", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive = false, 
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var returnedEvents = eventService.LookupAsync(projectPerson1.Id,"Event1", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = true, 
                 AssignedToProject = project1 };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,
                 AssignedToProject = project1 };
            var dbEntry3 = new ProjectEvent { Id = "dummyEntryId3", EventName = "Event3", EventAltName = "EventAlt3", IsActive = false, 
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

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var returnedEvents = eventService.LookupAsync(projectPerson1.Id,"Event3", true).Result;

            //Assert
            Assert.IsTrue(returnedEvents.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void ProjectEventService_EditAsync_DoesNothingIfNoProjectEvent()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var events = new ProjectEvent[] { };

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.EditAsync(events).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ProjectEventService_EditAsync_CreatesProjectEventIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var event1 = new ProjectEvent { Id = initialId, EventName = "Event1", EventAltName = "EventAlt1", IsActive = false,  };
            var events = new ProjectEvent[] { event1 };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(event1.Id)).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.ProjectEvents.Add(event1)).Returns(event1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.EditAsync(events).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(event1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.Add(event1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectEventService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var event1 = new ProjectEvent { Id = "dummyProjectEventId1", EventName = "Event1", EventAltName = "EventAlt1", IsActive = false, 
                                      ModifiedProperties = new string[] { "EventName", "EventAltName" }};
            var event2 = new ProjectEvent { Id = "dummyProjectEventId2", EventName = "Event2", EventAltName = "EventAlt2", IsActive = true,  };
            var events = new ProjectEvent[] { event1, event2 };

            var dbEntry1 = new ProjectEvent { Id = "dummyEntryId1", EventName = "DbEntry1", EventAltName = "DbEntryAlt1", IsActive = true };
            var dbEntry2 = new ProjectEvent { Id = "dummyEntryId2", EventName = "DbEntry2", EventAltName = "DbEntryAlt2", IsActive = false };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(event1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(event2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.Add(event1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.EditAsync(events).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(event1.EventName == dbEntry1.EventName); Assert.IsTrue(event1.EventAltName == dbEntry1.EventAltName);
            Assert.IsTrue(event1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(event2.EventName != dbEntry2.EventName); Assert.IsTrue(event2.EventAltName != dbEntry2.EventAltName);
            Assert.IsTrue(event2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.ProjectEvents.Add(It.IsAny<ProjectEvent>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectEventService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var event1 = new ProjectEvent { Id = initialId, EventName = "Event1", EventAltName = "EventAlt1", IsActive = false,  };
            var events = new ProjectEvent[] { event1 };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.ProjectEvents.Add(event1)).Returns(event1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.EditAsync(events).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(event1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ProjectEventService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var eventIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new ProjectEvent { IsActive = true, EventClosed = "2015-10-10", ClosedByPerson_Id = "DummyId" };
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync("dummyId2")).Returns(Task.FromResult<ProjectEvent>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.DeleteAsync(eventIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectEventService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var eventIds = new string[] { "dummyId1" };

            var dbEntry = new ProjectEvent { IsActive = true, EventClosed = "2015-10-10", ClosedByPerson_Id = "DummyId" };
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.DeleteAsync(eventIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectEventService_DeleteAsync_DoeNotDeleteIfNotClosed()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var eventIds = new string[] { "dummyId1" };

            var dbEntry = new ProjectEvent { IsActive = true, EventName="Name", EventClosed = "2015-10-10" };
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var eventService = new ProjectEventService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = eventService.DeleteAsync(eventIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Event Name not deleted. Please close before deleting\n"));
            Assert.IsTrue(dbEntry.IsActive == true);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ProjectEvents.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

    }
}
