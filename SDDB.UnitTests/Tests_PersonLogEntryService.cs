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
        public void PersonLogEntryService_GetAsync_ReturnsPersonLogEntrysByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";

            var projectPerson1 = new Person { Id = userId, FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
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

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, userId);

            //Act
            var serviceResult = prsLogEntryService.GetAsync( new string[] { "dummyEntryId3" }).Result;

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

            var userId = "DummyUserId";

            var projectPerson1 = new Person { Id = userId, FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project
            {
                Id = "dummyId1",
                ProjectName = "Project1",
                ProjectAltName = "ProjectAlt1",
                IsActive_bl = true,
                ProjectCode = "CODE1",
                ProjectPersons = new List<Person> { projectPerson1 }
            };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2000, 1, 1), IsActive_bl = false, AssignedToProject = project1 };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2000, 1, 2), IsActive_bl = true, AssignedToProject = project1 };
            var dbEntry3 = new PersonLogEntry { Id = "dummyEntryId3", LogEntryDateTime = new DateTime(2000, 1, 3), IsActive_bl = false, AssignedToProject = project1 };
            var dbEntries = (new List<PersonLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, userId);

            //Act
            var serviceResult = prsLogEntryService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Log Entry and Project Event do not belong to the same project. Entry(ies) not saved.")]
        public void BaseDbService_EditAsync_DoesNotUpdateIfProjIdsNotMatching()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";

            var record1 = new PersonLogEntry
            {
                Id = "dummyRecordId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
            };
            var record2 = new PersonLogEntry
            {
                Id = "dummyRecordId2",
                LogEntryDateTime = new DateTime(2000, 1, 2),
                IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId2",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
            };
            var records = new PersonLogEntry[] { record1, record2 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(record1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(record2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(It.IsNotIn<string>(new[] { projEventDbEntry.Id }))).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(record1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, userId);

            //Act
            var serviceResult = personLogEntryService.EditAsync(records).Result;

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Log Entry and Location do not belong to the same project. Entry(ies) not saved.")]
        public void BaseDbService_EditAsync_DoesNotUpdateIfProjLocIdsNotMatching()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var userId = "DummyUserId";

            var record1 = new PersonLogEntry
            {
                Id = "dummyRecordId1",
                LogEntryDateTime = new DateTime(2000, 1, 1),
                IsActive_bl = false,
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
                ModifiedProperties = new[] { "LogEntryDateTime" }
            };
            var record2 = new PersonLogEntry
            {
                Id = "dummyRecordId2",
                LogEntryDateTime = new DateTime(2000, 1, 2),
                IsActive_bl = true,
                AssignedToProject_Id = "DummyProjectId",
                AssignedToProjectEvent_Id = "DummyProjEntryID",
                AssignedToLocation_Id = "DummyLocId",
                ModifiedProperties = new[] { "LogEntryDateTime", "AssignedToProjectEvent_Id", "AssignedToLocation_Id" }
            };
            var records = new PersonLogEntry[] { record1, record2 };

            var projEventDbEntry = new ProjectEvent { Id = "DummyProjEntryID", AssignedToProject_Id = "DummyProjectId" };

            var locDbEntry = new Location { Id = "DummyLocId", AssignedToProject_Id = "DummyProjectId2" };

            var dbEntry1 = new PersonLogEntry { Id = "dummyEntryId1", LogEntryDateTime = new DateTime(2001, 1, 1), IsActive_bl = true };
            var dbEntry2 = new PersonLogEntry { Id = "dummyEntryId2", LogEntryDateTime = new DateTime(2001, 1, 2), IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(record1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.FindAsync(record2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEventDbEntry.Id)).Returns(Task.FromResult<ProjectEvent>(projEventDbEntry));
            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(It.IsNotIn<string>(new[] { projEventDbEntry.Id }))).Returns(Task.FromResult<ProjectEvent>(null));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(locDbEntry.Id)).Returns(Task.FromResult<Location>(locDbEntry));
            mockEfDbContext.Setup(x => x.PersonLogEntrys.Add(record1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, userId);

            //Act
            var serviceResult = personLogEntryService.EditAsync(records).Result;

            //Assert
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void BaseDbService_GetActivitySummariesAsync_ReturnsProperGroups()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var personId = "DummyUserId";
            var startDate = new DateTime(2000,1,1);
            var endDate = new DateTime(2000,1,3);

            var person = new Person { Id = personId };

            var project1 = new Project {Id = "ProjectId1", ProjectName = "ProjectName1", ProjectCode = "ProjectCode1" };
            var project2 = new Project { Id = "ProjectId2", ProjectName = "ProjectName2", ProjectCode = "ProjectCode2" };

            var dbEntry1 = new PersonLogEntry { Id = "Id1", LogEntryDateTime = new DateTime(2000, 1, 1, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = "otherPersonId",
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 3, PrsLogEntryPersons = new List<Person> {person}};
            var dbEntry2 = new PersonLogEntry { Id = "Id2", LogEntryDateTime = new DateTime(2000, 1, 1, 11, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 4};
            var dbEntry3 = new PersonLogEntry { Id = "Id3", LogEntryDateTime = new DateTime(2000, 1, 1, 12, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 5};
            var dbEntry4 = new PersonLogEntry { Id = "Id4", LogEntryDateTime = new DateTime(2000, 1, 1, 13, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 6};
            
            var dbEntry5 = new PersonLogEntry { Id = "Id5", LogEntryDateTime = new DateTime(2000, 1, 2, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 7};
            var dbEntry6 = new PersonLogEntry { Id = "Id6", LogEntryDateTime = new DateTime(2000, 1, 2, 11, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 8 };

            var dbEntry7 = new PersonLogEntry { Id = "Id7", LogEntryDateTime = new DateTime(2000, 1, 1, 9, 0, 0), IsActive_bl = true, EnteredByPerson_Id = "otherPersonId",
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 100 };
            var dbEntry8 = new PersonLogEntry { Id = "Id8", LogEntryDateTime = new DateTime(2000, 1, 4, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 1000 };

            var dbEntrys = new PersonLogEntry[] { dbEntry1, dbEntry2, dbEntry3, dbEntry4, dbEntry5, dbEntry6, dbEntry7, dbEntry8 }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntrys.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntrys.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntrys.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntrys.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntrys.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var personLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, personId);

            //Act
            var serviceResult = personLogEntryService.GetActivitySummariesAsync(personId, startDate, endDate).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 2);
            Assert.IsTrue(serviceResult[0].TotalManHours == dbEntry1.ManHours + dbEntry2.ManHours + dbEntry3.ManHours + dbEntry4.ManHours);
            Assert.IsTrue(serviceResult[0].SummaryDay == dbEntry4.LogEntryDateTime.Date);
            Assert.IsTrue(serviceResult[1].SummaryDetails.Count() == 2);
            Assert.IsTrue(serviceResult[1].TotalManHours == dbEntry5.ManHours + dbEntry6.ManHours);
            Assert.IsTrue(serviceResult.All(x => x.PersonId == personId));

        }

    }
}
