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
    public class Tests_PersonLogEntryService
    {
        [TestMethod]
        public void PersonLogEntryService_GetAsync_ReturnsPersonLogEntrys()
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1), IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, Comments = "DummyComments2",
                 AssignedToProject = project1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultPersonLogEntrys = prsLogEntryService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultPersonLogEntrys.Count == 1);
            Assert.IsTrue(resultPersonLogEntrys[0].LogEntryDateTime == DateTime.Parse("2000-01-02"));
        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_DoeNotReturnPersonLogEntrysFromWrongProject()
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project2 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultPersonLogEntrys = prsLogEntryService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultPersonLogEntrys.Count == 0);
        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_ReturnsPersonLogEntrysByIds()
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false,  AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive_bl = true, AssignedToProject = project1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LogEntryDateTime == DateTime.Parse("2000-01-03"));

        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_DoesNotReturnPersonLogEntrysByIdsIfNotActive()
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false, AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive_bl = false, AssignedToProject = project1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonLogEntryService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1", Initials = "Initials1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = true, Comments="DummyComments1",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, Comments = "DummyComments2",
                AssignedToProject = project1, EnteredByPerson = projectPerson1  };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3), IsActive_bl = false, Comments = "DummyComments3", 
                AssignedToProject = project1, EnteredByPerson = projectPerson1  };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var returnedPrsLogEntrys = prsLogEntryService.LookupAsync(projectPerson1.Id,"", true).Result;

            //Assert
            Assert.IsTrue(returnedPrsLogEntrys.Count == 2);
            Assert.IsTrue(returnedPrsLogEntrys[1].Comments.Contains("DummyComments2"));
        }

        [TestMethod]
        public void PersonLogEntryService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1", Initials = "Initials1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = true, Comments="DummyComments1",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, Comments="DummyComments2",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive_bl = false,  Comments="DummyComments3",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var returnedPrsLogEntrys = prsLogEntryService.LookupAsync(projectPerson1.Id, "Comments1", true).Result;

            //Assert
            Assert.IsTrue(returnedPrsLogEntrys.Count == 1);
            Assert.IsTrue(returnedPrsLogEntrys[0].LogEntryDateTime == DateTime.Parse("2000-01-01"));
        }

        [TestMethod]
        public void PersonLogEntryService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1", Initials = "Initials1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = true, Comments="DummyComments1",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true, Comments="DummyComments2",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive_bl = false, Comments="DummyComments3",
                AssignedToProject = project1, EnteredByPerson = projectPerson1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var returnedPrsLogEntrys = prsLogEntryService.LookupAsync(projectPerson1.Id, "DummyComments3", true).Result;

            //Assert
            Assert.IsTrue(returnedPrsLogEntrys.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void PersonLogEntryService_EditAsync_DoesNothingIfNoPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntrys = new PersonLogEntry[] { };

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void PersonLogEntryService_EditAsync_CreatesPersonLogEntryIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var prsLogEntry1 = new PersonLogEntry { Id = initialId, LogEntryDateTime = new DateTime(2000,1,1), 
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", IsActive_bl = false,  };
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId" };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult<PersonLogEntry>(null));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Returns(prsLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(prsLogEntry1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.Add(prsLogEntry1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntry1 = new PersonLogEntry { Id = "dummyPersonLogEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false, 
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", ModifiedProperties = new string[] { "LogEntryDateTime" }};
            var prsLogEntry2 = new PersonLogEntry { Id = "dummyPersonLogEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID"};
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1, prsLogEntry2 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime); Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime); Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.PersonLogEntrys.Add(It.IsAny<PersonLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditAsync_DoesNotUpdateIfProjIdsNotMatching()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntry1 = new PersonLogEntry { Id = "dummyPersonLogEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false, 
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", ModifiedProperties = new[] { "LogEntryDateTime" }};
            var prsLogEntry2 = new PersonLogEntry { Id = "dummyPersonLogEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId2", AssignedToProjectEvent_Id = "DummyProjEntryID", 
                ModifiedProperties = new[] { "LogEntryDateTime","AssignedToProjectEvent_Id" }};
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1, prsLogEntry2 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(It.IsNotIn<string>(new[] { projEventDbEntry.Id }))).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Log Entry and Project Event do not belong to the same project. Entry not saved."));
            Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime); Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime); Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.Add(It.IsAny<PersonLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditAsync_DoesNotUpdateIfProjLocIdsNotMatching()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntry1 = new PersonLogEntry { Id = "dummyPersonLogEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive_bl = false, 
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", ModifiedProperties = new[] { "LogEntryDateTime" }};
            var prsLogEntry2 = new PersonLogEntry { Id = "dummyPersonLogEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", AssignedToLocation_Id = "DummyLocId", 
                ModifiedProperties = new[] { "LogEntryDateTime","AssignedToProjectEvent_Id","AssignedToLocation_Id" }};
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1, prsLogEntry2 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId2" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(It.IsNotIn<string>(new[] { projEventDbEntry.Id }))).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Log Entry and Location do not belong to the same project. Entry not saved."));
            Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime); Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime); Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.Add(It.IsAny<PersonLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var prsLogEntry1 = new PersonLogEntry { Id = initialId, LogEntryDateTime = new DateTime(2000,1,1), 
                AssignedToProject_Id = "DummyProjectId", AssignedToProjectEvent_Id = "DummyProjEntryID", IsActive_bl = false,  };
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId" };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<PersonLogEntry>(null));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Returns(prsLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync("DummyUserId", prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(prsLogEntry1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonLogEntryService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntryIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new PersonLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync("dummyId2")).Returns(Task.FromResult<PersonLogEntry>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.DeleteAsync(prsLogEntryIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogEntryIds = new string[] { "dummyId1" };

            var dbEntry = new PersonLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.DeleteAsync(prsLogEntryIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesNothingIfNoPersonLogEntrys()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { };

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true};
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesNothingIfPersonLogEntryNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId2" };

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesNothingIfNoAssemblyDbs()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "EntryId1" };

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesNothingIfAssemblyDbNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var prsLogEntryIds = new string[] { "EntryId1" };
            var assyIds = new string[] { "AssemblyDbId2" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_AddsAssemblyDbToPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { "AssemblyDbId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesntAddProjectToPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { "AssemblyDbId1" };

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10,
            PrsLogEntryAssemblyDbs = new List<AssemblyDb> {assyDbEntry1} };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_RemovesAssemblyDbFromPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { "AssemblyDbId1" };

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10,
                PrsLogEntryAssemblyDbs = new List<AssemblyDb> { assyDbEntry1 } };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_DoesntRemoveAssemblyDbFromPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { "AssemblyDbId1" };

            var prsLogDbEntry1 = new PersonLogEntry
            { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }


        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryAssysAsync_ReturnsExceptionMessageFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "AssemblyDbId1", AssyName = "AssemblyDb1", AssyAltName = "AssemblyDbAlt1", IsActive = true };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<AssemblyDb>>();
            mockDbSet2.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockDbSet2.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet2.Object);

            var assyIds = new string[] { "AssemblyDbId1" };

            var prsLogDbEntry1 = new PersonLogEntry
            { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10,
                PrsLogEntryAssemblyDbs = new List<AssemblyDb> { assyDbEntry1 } };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyExceptionMessage"));
            //Returns(Task.FromResult<int>(throw new ArgumentException("DummyExceptionMessage"); ));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryAssysAsync(prsLogEntryIds, assyIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryAssemblyDbs.Contains(assyDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyExceptionMessage"));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesNothingIfNoPersonLogEntrys()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { };

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesNothingIfPersonLogEntryNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId2" };

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesNothingIfNoPersons()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "EntryId1" };

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesNothingIfPersonNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var prsLogEntryIds = new string[] { "EntryId1" };
            var personIds = new string[] { "PersonId2" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_AddsPersonToPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { "PersonId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesntAddProjectToPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { "PersonId1" };

            var prsLogDbEntry1 = new PersonLogEntry
            {
                Id = "dummyEntryId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                ManHours = 10,
                PrsLogEntryPersons = new List<Person> { personDbEntry1 }
            };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_RemovesPersonFromPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { "PersonId1" };

            var prsLogDbEntry1 = new PersonLogEntry
            {
                Id = "dummyEntryId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                ManHours = 10,
                PrsLogEntryPersons = new List<Person> { personDbEntry1 }
            };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_DoesntRemovePersonFromPersonLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { "PersonId1" };

            var prsLogDbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, ManHours = 10 };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }


        [TestMethod]
        public void PersonLogEntryService_EditPrsLogEntryPersonsAsync_ReturnsExceptionMessageFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var personDbEntry1 = new Person { Id = "PersonId1", FirstName = "Person1", LastName = "PersonAlt1", IsActive = true };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Person>>();
            mockDbSet2.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet2.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet2.Object);

            var personIds = new string[] { "PersonId1" };

            var prsLogDbEntry1 = new PersonLogEntry
            {
                Id = "dummyEntryId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                ManHours = 10,
                PrsLogEntryPersons = new List<Person> { personDbEntry1 }
            };
            var prsLogDbEntries = (new List<PersonLogEntry> { prsLogDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryIds = new string[] { "dummyEntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyExceptionMessage"));
            //Returns(Task.FromResult<int>(throw new ArgumentException("DummyExceptionMessage"); ));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditPrsLogEntryPersonsAsync(prsLogEntryIds, personIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys, Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            Assert.IsTrue(!prsLogDbEntry1.PrsLogEntryPersons.Contains(personDbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyExceptionMessage"));
        }


    }
}
