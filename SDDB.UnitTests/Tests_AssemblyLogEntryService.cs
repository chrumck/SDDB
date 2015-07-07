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
        public void AssemblyLogEntryService_GetAsync_ReturnsAssemblyLogEntrys()
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

            var dbEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyLogEntry { Id = "dummyEntryId2", AssemblyDb_Id = "dummyAssyId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyLogEntrys = assyLogEntryService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyLogEntrys.Count == 1);
            Assert.IsTrue(resultAssemblyLogEntrys[0].AssemblyDb_Id.Contains("dummyAssyId2"));
        }

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

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyLogEntry { Id = "dummyEntryId2", AssemblyDb_Id = "dummyAssyId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new AssemblyLogEntry { Id = "dummyEntryId3", AssemblyDb_Id = "dummyAssyId3", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments3", AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyLogEntry { Id = "dummyEntryId2", AssemblyDb_Id = "dummyAssyId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project2 };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyLogEntrys = assyLogEntryService.GetAsync(projectPerson1.Id).Result;
            
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

            var project1 = new Project { Id = "dummyProjId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyLogEntry { Id = "dummyEntryId2", AssemblyDb_Id = "dummyAssyId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments2", AssignedToProject = project1 };
            var dbEntry3 = new AssemblyLogEntry { Id = "dummyEntryId3", AssemblyDb_Id = "dummyAssyId3", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments3", AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyLogEntry> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyLogEntry>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys).Returns(mockDbSet.Object);

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }
                                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyLogEntryService_EditAsync_DoesNothingIfNoAssemblyLogEntry()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyLogEntrys = new AssemblyLogEntry[] { };

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.EditAsync(assyLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void AssemblyLogEntryService_EditAsync_CreatesAssemblyLogEntryIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyLogEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1", IsActive_bl = false, Comments = "DummyComments1" };
            var assyLogEntrys = new AssemblyLogEntry[] { assyLogEntry1 };

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync(assyLogEntry1.Id)).Returns(Task.FromResult<AssemblyLogEntry>(null));
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(assyLogEntry1)).Returns(assyLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.EditAsync(assyLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyLogEntry1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.Add(assyLogEntry1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyLogEntryService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assyLogEntry1 = new AssemblyLogEntry { Id = "CLEntryId1", AssemblyDb_Id = "CLAssyId1", AssignedToProject_Id = "CLProjId1",
                IsActive_bl = false, Comments = "CLComments1",ModifiedProperties = new string[] { "AssemblyDb_Id", "AssignedToProject_Id" } };
            var assyLogEntry2 = new AssemblyLogEntry { Id = "CLEntryId2", AssemblyDb_Id = "CLAssyId2", AssignedToProject_Id = "CLProjId2",
                IsActive_bl = true, Comments = "CLComments2"};
            var assyLogEntrys = new AssemblyLogEntry[] { assyLogEntry1, assyLogEntry2 };

            var dbEntry1 = new AssemblyLogEntry { Id = "dummyEntryId1", AssemblyDb_Id = "dummyAssyId1", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = true, Comments = "DummyComments1" };
            var dbEntry2 = new AssemblyLogEntry { Id = "dummyEntryId2", AssemblyDb_Id = "dummyAssyId2", AssignedToProject_Id = "dummyProjId1",
                IsActive_bl = false, Comments = "DummyComments2" };

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync(assyLogEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync(assyLogEntry2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(assyLogEntry1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.EditAsync(assyLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assyLogEntry1.AssemblyDb_Id == dbEntry1.AssemblyDb_Id); Assert.IsTrue(assyLogEntry1.AssignedToProject_Id == dbEntry1.AssignedToProject_Id);
            Assert.IsTrue(assyLogEntry1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assyLogEntry2.AssemblyDb_Id != dbEntry2.AssemblyDb_Id); Assert.IsTrue(assyLogEntry2.AssignedToProject_Id != dbEntry2.AssignedToProject_Id);
            Assert.IsTrue(assyLogEntry2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyLogEntryService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assyLogEntry1 = new AssemblyLogEntry { Id = initialId, AssemblyDb_Id = "CLAssyId1", AssignedToProject_Id = "CLProjId1", IsActive_bl = false, Comments = "CLComments1" };
            var assyLogEntrys = new AssemblyLogEntry[] { assyLogEntry1 };

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyLogEntry>(null));
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(assyLogEntry1)).Returns(assyLogEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.EditAsync(assyLogEntrys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assyLogEntry1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyLogEntryService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new AssemblyLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync("dummyId2")).Returns(Task.FromResult<AssemblyLogEntry>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyLogEntryService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new string[] { "dummyId1" };

            var dbEntry = new AssemblyLogEntry { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assyLogEntryService = new AssemblyLogEntryService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assyLogEntryService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
