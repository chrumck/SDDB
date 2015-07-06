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
        public void ComponentLogEntryService_GetAsync_ReturnsComponentLogEntrys()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new ComponentLogEntry { Id = "dummyEntryId2", Component_Id = "dummyCompId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntries = (new List<ComponentLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<ComponentLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<ComponentLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<ComponentLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<ComponentLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<ComponentLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.ComponentLogEntrys).Returns(mockDbSet.Object);

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultComponentLogEntrys = compLogEntryService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultComponentLogEntrys.Count == 1);
            Assert.IsTrue(resultComponentLogEntrys[0].Component_Id.Contains("dummyCompId2"));
        }

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

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
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

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].Comments.Contains("DummyComments3"));

        }

        [TestMethod]
        public void ComponentLogEntryService_GetAsync_DoeNotReturnFromWrongProject()
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

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultComponentLogEntrys = compLogEntryService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultComponentLogEntrys.Count == 0);
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

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
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

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }
                                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentLogEntryService_EditAsync_DoesNothingIfNoComponentLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compLogEntrys = new ComponentLogEntry[] { };

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.EditAsync(compLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentLogEntryService_EditAsync_CreatesComponentLogEntryIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var compLogEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1", IsActive_bl = false, Comments = "DummyComments1" };
            var compLogEntrys = new ComponentLogEntry[] { compLogEntry1 };

            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync(compLogEntry1.Id)).Returns(Task.FromResult<ComponentLogEntry>(null));
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.Add(compLogEntry1)).Returns(compLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.EditAsync(compLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(compLogEntry1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.Add(compLogEntry1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentLogEntryService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var compLogEntry1 = new ComponentLogEntry { Id = "CLEntryId1", Component_Id = "CLCompId1", AssignedToProject_Id = "CLProjId1",
                IsActive_bl = false, Comments = "CLComments1",ModifiedProperties = new string[] { "Component_Id", "AssignedToProject_Id" } };
            var compLogEntry2 = new ComponentLogEntry { Id = "CLEntryId2", Component_Id = "CLCompId2", AssignedToProject_Id = "CLProjId2",
                IsActive_bl = true, Comments = "CLComments2"};
            var compLogEntrys = new ComponentLogEntry[] { compLogEntry1, compLogEntry2 };

            var dbEntry1 = new ComponentLogEntry { Id = "dummyEntryId1", Component_Id = "dummyCompId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments1" };
            var dbEntry2 = new ComponentLogEntry { Id = "dummyEntryId2", Component_Id = "dummyCompId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments2" };

            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync(compLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync(compLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.Add(compLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.EditAsync(compLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(compLogEntry1.Component_Id == dbEntry1.Component_Id); Assert.IsTrue(compLogEntry1.AssignedToProject_Id == dbEntry1.AssignedToProject_Id);
            Assert.IsTrue(compLogEntry1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(compLogEntry2.Component_Id != dbEntry2.Component_Id); Assert.IsTrue(compLogEntry2.AssignedToProject_Id != dbEntry2.AssignedToProject_Id);
            Assert.IsTrue(compLogEntry2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.Add(It.IsAny<ComponentLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentLogEntryService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var compLogEntry1 = new ComponentLogEntry { Id = initialId, Component_Id = "CLCompId1", AssignedToProject_Id = "CLProjId1", IsActive_bl = false, Comments = "CLComments1" };
            var compLogEntrys = new ComponentLogEntry[] { compLogEntry1 };

            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<ComponentLogEntry>(null));
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.Add(compLogEntry1)).Returns(compLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.EditAsync(compLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(compLogEntry1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentLogEntryService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new ComponentLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync("dummyId2")).Returns(Task.FromResult<ComponentLogEntry>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentLogEntryService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1" };

            var dbEntry = new ComponentLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.ComponentLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var compLogEntryService = new ComponentLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = compLogEntryService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
