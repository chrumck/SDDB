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
    public class Tests_ComponentLogEntryService
    {
        [TestMethod]
        public void ComponentLogEntryService_GetAsync_ReturnsComponentLogEntrysByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new ComponentLogEntry { Id = "dummyEntryId2", Component_Id = "dummyCompId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new ComponentLogEntry { Id = "dummyEntryId3", Component_Id = "dummyCompId3", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments3", AssignedToProject = project1 };
            var dbEntries = (new List<ComponentLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentLogEntrys).Returns(mockDbSet.Object);

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = compLogEntryService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].Comments.Contains("DummyComments3"));

        }

        [TestMethod]
        public void ComponentLogEntryService_GetAsync_ReturnsFromNotManagedProject()
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

            var dbEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new ComponentLogEntry { Id = "dummyEntryId2", Component_Id = "dummyCompId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project2 };
            var dbEntries = (new List<ComponentLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ComponentLogEntrys).Returns(mockDbSet.Object);

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultComponentLogEntrys = compLogEntryService.GetAsync(new string[] { "dummyEntryId1", "dummyEntryId2" }).Result;
            
            //Assert
            Assert.IsTrue(resultComponentLogEntrys.Count == 1);
        }

        [TestMethod]
        public void ComponentLogEntryService_GetAsync_DoesNotReturnComponentLogEntrysByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new ComponentLogEntry { Id = "dummyEntryId2", Component_Id = "dummyCompId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new ComponentLogEntry { Id = "dummyEntryId3", Component_Id = "dummyCompId3", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments3", AssignedToProject = project1 };
            var dbEntries = (new List<ComponentLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.ComponentLogEntrys).Returns(mockDbSet.Object);

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = compLogEntryService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }
                                
        //-----------------------------------------------------------------------------------------------------------------------

        
    }
}
