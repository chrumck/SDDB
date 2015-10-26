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
    public class Tests_AssemblyLogEntryService
    {
        [TestMethod]
        public void AssemblyLogEntryService_GetAsync_ReturnsAssemblyLogEntrysByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project
            {
                Id = "dummyProjId1",
               ProjectName = "Project1",
                ProjectAltName = "ProjectAlt1",
                IsActive_bl = true,
                ProjectCode = "CODE1",
                ProjectPersons = new List<Person> { projectPerson1 }
            };

            var location1 = new Location { Id = "dummyLocId1", LocName = "Loc1", AssignedToProject_Id = project1.Id, AssignedToProject = project1 };

            var dbEntry1 = new AssemblyLogEntry
            {
                Id = "dummyEntryId1",
                AssemblyDb_Id = "dummyAssyId1",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = false,
                Comments = "DummyComments1",
                AssignedToLocation = location1
            };
            var dbEntry2 = new AssemblyLogEntry
            {
                Id = "dummyEntryId2",
                AssemblyDb_Id = "dummyAssyId2",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = true,
                Comments = "DummyComments2",
                AssignedToLocation = location1
            };
            var dbEntry3 = new AssemblyLogEntry
            {
                Id = "dummyEntryId3",
                AssemblyDb_Id = "dummyAssyId3",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = true,
                Comments = "DummyComments3",
                AssignedToLocation = location1
            };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = assyLogEntryService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].Comments.Contains("DummyComments3"));

        }

        [TestMethod]
        public void AssemblyLogEntryService_GetAsync_DoeNotReturnFromWrongProject()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var projectPerson2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var project1 = new Project
            {
                Id = "dummyId1",
                ProjectName = "Project1",
                ProjectAltName = "ProjectAlt1",
                IsActive_bl = true,
                ProjectCode = "CODE1",
                ProjectPersons = new List<Person> { projectPerson1 }
            };

            var location1 = new Location { Id = "dummyLocId1", LocName = "Loc1", AssignedToProject_Id = project1.Id, AssignedToProject = project1 };

            var project2 = new Project
            {
                Id = "dummyId2",
                ProjectName = "Project2",
                ProjectAltName = "ProjectAlt2",
                IsActive_bl = false,
                ProjectCode = "CODE2",
                ProjectPersons = new List<Person> { projectPerson2 }
            };

            var location2 = new Location { Id = "dummyLocId2", LocName = "Loc2", AssignedToProject_Id = project2.Id, AssignedToProject = project2 };

            var dbEntry1 = new AssemblyLogEntry
            {
                Id = "dummyEntryId1",
                AssemblyDb_Id = "dummyAssyId1",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = false,
                Comments = "DummyComments1",
                AssignedToLocation = location1
            };
            var dbEntry2 = new AssemblyLogEntry
            {
                Id = "dummyEntryId2",
                AssemblyDb_Id = "dummyAssyId2",
                AssignedToLocation_Id = location2.Id,
                IsActive_bl = true,
                Comments = "DummyComments2",
                AssignedToLocation = location2
            };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultAssemblyLogEntrys = assyLogEntryService.GetAsync(new[] { "dummyEntryId1", "dummyEntryId2" }).Result;

            //Assert
            Assert.IsTrue(resultAssemblyLogEntrys.Count == 0);
        }

        [TestMethod]
        public void AssemblyLogEntryService_GetAsync_DoesNotReturnAssemblyLogEntrysByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project
            {
                Id = "dummyProjId1",
                ProjectName = "Project1",
                ProjectAltName = "ProjectAlt1",
                IsActive_bl = true,
                ProjectCode = "CODE1",
                ProjectPersons = new List<Person> { projectPerson1 }
            };

            var location1 = new Location { Id = "dummyLocId1", LocName = "Loc1", AssignedToProject_Id = project1.Id, AssignedToProject = project1 };

            var dbEntry1 = new AssemblyLogEntry
            {
                Id = "dummyEntryId1",
                AssemblyDb_Id = "dummyAssyId1",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = false,
                Comments = "DummyComments1",
                AssignedToLocation = location1
            };
            var dbEntry2 = new AssemblyLogEntry
            {
                Id = "dummyEntryId2",
                AssemblyDb_Id = "dummyAssyId2",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = true,
                Comments = "DummyComments2",
                AssignedToLocation = location1
            };
            var dbEntry3 = new AssemblyLogEntry
            {
                Id = "dummyEntryId3",
                AssemblyDb_Id = "dummyAssyId3",
                AssignedToLocation_Id = location1.Id,
                IsActive_bl = false,
                Comments = "DummyComments3",
                AssignedToLocation = location1
            };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = assyLogEntryService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

    }
}