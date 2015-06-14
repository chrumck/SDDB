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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1), IsActive = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, Comments = "DummyComments2",
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, Comments = "DummyComments2", AssignedToProject = project2 };
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false,  AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive = true, AssignedToProject = project1 };
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

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false, AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive = false, AssignedToProject = project1 };
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

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = true, Comments="DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3), IsActive = false, Comments = "DummyComments3", AssignedToProject = project1 };
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

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = true, Comments="DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, Comments="DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive = false,  Comments="DummyComments3", AssignedToProject = project1 };
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

                        var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = true, Comments="DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true, Comments="DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000,1,3),  IsActive = false, Comments="DummyComments3", AssignedToProject = project1 };
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
            var serviceResult = prsLogEntryService.EditAsync(prsLogEntrys).Result;

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
            var prsLogEntry1 = new PersonLogEntry { Id = initialId, LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false,  };
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1 };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult<PersonLogEntry>(null));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Returns(prsLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync(prsLogEntrys).Result;

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

            var prsLogEntry1 = new PersonLogEntry { Id = "dummyPersonLogEntryId1", LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false, 
                                      ModifiedProperties = new string[] { "LogEntryDateTime" }};
            var prsLogEntry2 = new PersonLogEntry { Id = "dummyPersonLogEntryId2", LogEntryDateTime = new DateTime(2000,1,2), IsActive = true};
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1, prsLogEntry2 };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(prsLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync(prsLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime); Assert.IsTrue(prsLogEntry1.LogEntryDateTime == dbEntry1.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime); Assert.IsTrue(prsLogEntry2.LogEntryDateTime != dbEntry2.LogEntryDateTime);
            Assert.IsTrue(prsLogEntry2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
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
            var prsLogEntry1 = new PersonLogEntry { Id = initialId, LogEntryDateTime = new DateTime(2000,1,1),  IsActive = false };
            var prsLogEntrys = new PersonLogEntry[] { prsLogEntry1 };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<PersonLogEntry>(null));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(prsLogEntry1)).Returns(prsLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.EditAsync(prsLogEntrys).Result;

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

            var dbEntry = new PersonLogEntry { IsActive = true };
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
            Assert.IsTrue(dbEntry.IsActive == false);
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

            var dbEntry = new PersonLogEntry { IsActive = true };
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsLogEntryService.DeleteAsync(prsLogEntryIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
