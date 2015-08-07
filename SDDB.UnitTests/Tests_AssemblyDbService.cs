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
    public class Tests_AssemblyDbService
    {
        [TestMethod]
        public void AssemblyDbService_GetAsync_ReturnsAssemblyDbsByIds()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(() => dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(() => dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = assemblyService.GetAsync( new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].AssyAltName.Contains("AssyAlt3"));

        }

        [TestMethod]
        public void AssemblyDbService_GetAsync_DoesNotReturnAssemblyDbsByIdsIfNotActive()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = assemblyService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyDbService_LookupAsync_ReturnsAllRecords()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedAssys = assemblyService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedAssys.Count == 2);
            Assert.IsTrue(returnedAssys[1].AssyAltName.Contains("AssyAlt2"));
        }

        [TestMethod]
        public void AssemblyDbService_LookupAsync_ReturnsMatchingRecords()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", AssyAltName2 = "AssyAlt1", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", AssyAltName2 = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", AssyAltName2 = "AssyAlt3", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedAssys = assemblyService.LookupAsync("Assy1", true).Result;

            //Assert
            Assert.IsTrue(returnedAssys.Count == 1);
            Assert.IsTrue(returnedAssys[0].AssyAltName.Contains("AssyAlt1"));
        }

        [TestMethod]
        public void AssemblyDbService_LookupAsync_ReturnsNoRecordsIfNotActive()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", AssyAltName2 = "AssyAlt1", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", AssyAltName2 = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", AssyAltName2 = "AssyAlt3", IsActive_bl = false, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedAssys = assemblyService.LookupAsync("Assy3", true).Result;

            //Assert
            Assert.IsTrue(returnedAssys.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        
        [TestMethod]
        public void AssemblyDbService_EditAsync_AddsLogEntryWhenEdit()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assembly1 = new AssemblyDb { Id = "dummyAssemblyDbId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = false, TechnicalDetails = "DummyInfo1",
                                        AssignedToLocation_Id = "dummyLocId1", ModifiedProperties = new string[] { "AssyName", "AssyAltName", "AssignedToLocation_Id" }};
            var assembly2 = new AssemblyDb { Id = "dummyAssemblyDbId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true, TechnicalDetails = "DummyInfo2",
                                        AssignedToLocation_Id = "dummyLocId2"};
            var assemblys = new AssemblyDb[] { assembly1, assembly2 };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "DbEntry1", AssyAltName = "DbEntryAlt1", IsActive_bl = true, TechnicalDetails = "DbEntryInfo1",
                                        AssignedToLocation_Id = "dummyLocId3" };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "DbEntry2", AssyAltName = "DbEntryAlt2", IsActive_bl = false, TechnicalDetails = "DbEntryInfo2",
                                        AssignedToLocation_Id = "dummyLocId4" };

            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().Add(assembly1)).Verifiable();
            var logEntry = new AssemblyLogEntry();
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>())).Callback<AssemblyLogEntry>( x => logEntry = x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var newEntriesList = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(assembly1.AssyName == dbEntry1.AssyName); Assert.IsTrue(assembly1.AssyAltName == dbEntry1.AssyAltName);
            Assert.IsTrue(assembly1.TechnicalDetails != dbEntry1.TechnicalDetails); Assert.IsTrue(assembly1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assembly1.AssignedToLocation_Id == dbEntry1.AssignedToLocation_Id);
            Assert.IsTrue(assembly1.AssyName == dbEntry1.AssyName); Assert.IsTrue(assembly1.AssyAltName == dbEntry1.AssyAltName);
            Assert.IsTrue(assembly1.TechnicalDetails != dbEntry1.TechnicalDetails); Assert.IsTrue(assembly1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assembly2.AssignedToLocation_Id != dbEntry2.AssignedToLocation_Id);
            Assert.IsTrue(logEntry.AssemblyDb_Id == dbEntry1.Id); Assert.IsTrue(logEntry.AssignedToLocation_Id == dbEntry1.AssignedToLocation_Id);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().Add(It.IsAny<AssemblyDb>()), Times.Never);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
           
        [TestMethod]
        public void AssemblyDbService_EditAsync_AddsLogEntryWhenCreate()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assembly1 = new AssemblyDb { Id = "dummyAssemblyDbId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = false, TechnicalDetails = "DummyInfo1",
                                        AssignedToLocation_Id = "dummyLocId1", ModifiedProperties = new string[] { "AssyName", "AssyAltName", "AssignedToLocation_Id" }};
            var assembly2 = new AssemblyDb { Id = "dummyAssemblyDbId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true, TechnicalDetails = "DummyInfo2",
                                        AssignedToLocation_Id = "dummyLocId2"};
            var assemblys = new AssemblyDb[] { assembly1, assembly2 };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "DbEntry1", AssyAltName = "DbEntryAlt1", IsActive_bl = true, TechnicalDetails = "DbEntryInfo1",
                                        AssignedToLocation_Id = "dummyLocId3" };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "DbEntry2", AssyAltName = "DbEntryAlt2", IsActive_bl = false, TechnicalDetails = "DbEntryInfo2",
                                        AssignedToLocation_Id = "dummyLocId4" };

            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly2.Id)).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().Add(It.IsAny<AssemblyDb>())).Verifiable();
            var logEntry = new AssemblyLogEntry();
            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>())).Callback<AssemblyLogEntry>( x => logEntry = x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var newEntriesList = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(assembly1.AssyName == dbEntry1.AssyName);
            Assert.IsTrue(assembly1.AssyAltName == dbEntry1.AssyAltName);
            Assert.IsTrue(assembly1.TechnicalDetails != dbEntry1.TechnicalDetails);
            Assert.IsTrue(assembly1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assembly1.AssignedToLocation_Id == dbEntry1.AssignedToLocation_Id);
            Assert.IsTrue(assembly1.AssyName == dbEntry1.AssyName);
            Assert.IsTrue(assembly1.AssyAltName == dbEntry1.AssyAltName);
            Assert.IsTrue(assembly1.TechnicalDetails != dbEntry1.TechnicalDetails);
            Assert.IsTrue(assembly1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(assembly2.AssignedToLocation_Id != dbEntry2.AssignedToLocation_Id);
            Assert.IsTrue(logEntry.AssignedToLocation_Id == assembly2.AssignedToLocation_Id);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().Add(assembly2), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Delete failed Assembly Assy2 has components assigned to it.")]
        public void AssemblyDbService_DeleteAsync_ThrowsIfComponentsAssignedToAssy()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new[] { "dummyAssemblyDbId2", "dummyEntryId2" };

            var assyDbEntry1 = new AssemblyDb { Id = "dummyAssemblyDbId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true, TechnicalDetails = "DummyInfo2",
                                        AssignedToLocation_Id = "dummyLocId2"};

            var dbEntry = new Component { Id = "dummyEntryId1", CompName = "DbEntry1", CompAltName = "DbEntryAlt1", IsActive_bl = true, AssignedToAssemblyDb_Id = "dummyAssemblyDbId2" };
            var dbEntries = (new List<Component> { dbEntry }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(() => dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(() => dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assyDbEntry1.Id)).Returns(Task.FromResult(assyDbEntry1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            assemblyService.DeleteAsync(ids).Wait();

            //Assert

        }

        ////-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyDbService_EditStatusAsync_EditsStatusIfDifferent()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockDbContextScopeReadOnly = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScopeReadOnly.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);
            mockDbContextScopeReadOnly.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var ids = new[] { "dummyEntryId1", "dummyEntryId2"};
            var statusId = "newStatusId";

            var userId = "dummyUserId1";
            var projectPerson1 = new Person { Id = userId, FirstName = "Firs1", LastName = "Last1" };
            
            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive_bl = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1, AssemblyStatus_Id = statusId };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive_bl = true, 
                TechnicalDetails = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();
            
            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(() => dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(() => dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockDbSet.Setup(x => x.FindAsync(dbEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockDbSet.Setup(x => x.FindAsync(dbEntry2.Id)).Returns(Task.FromResult(dbEntry2));

            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var logEntry = new AssemblyLogEntry();

            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(dbEntry1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(dbEntry2.Id)).Returns(Task.FromResult(dbEntry1));

            mockEfDbContext.Setup(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>())).Callback<AssemblyLogEntry>( x => logEntry = x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, userId);

            //Act
            assemblyService.EditStatusAsync(ids,statusId).Wait();

            //Assert
            Assert.IsTrue(dbEntry1.AssemblyStatus_Id == statusId);
            Assert.IsTrue(dbEntry2.AssemblyStatus_Id == statusId);
            Assert.IsTrue(logEntry.AssignedToLocation_Id == dbEntry2.AssignedToLocation_Id);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyLogEntrys.Add(It.IsAny<AssemblyLogEntry>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        ////-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyDbService_EditExtAsync_CreatesAssemblyExtIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assemblyExt1 = new AssemblyExt { Id = initialId, Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04" };
            var assemblyExts = new AssemblyExt[] { assemblyExt1 };

            var assembly1 = new AssemblyDb { Id = initialId };
            var assemblys = new AssemblyDb[] { assembly1 };

            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyExt>(null));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly1.Id)).Returns(Task.FromResult(assembly1));
            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().Add(assemblyExt1)).Returns(assemblyExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object,"dummyUserId");

            //Act
            assemblyService.EditExtendedAsync(assemblyExts).Wait();

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyExt>().FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyExt>().Add(assemblyExt1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Entity AssemblyExt with id=dummyEntryId1 not found.")]
        public void AssemblyDbService_EditExtAsync_ReturnsErrorDbIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assemblyExt1 = new AssemblyExt { Id = initialId, Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04" };
            var assemblyExts = new AssemblyExt[] { assemblyExt1 };

            var assembly1 = new AssemblyDb { Id = initialId };
            var assemblys = new AssemblyDb[] { assembly1 };

            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyExt>(null));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly1.Id)).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().Add(assemblyExt1)).Returns(assemblyExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            assemblyService.EditExtendedAsync(assemblyExts).Wait();

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.Add(assemblyExt1), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyDbService_EditExtAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assemblyExt1 = new AssemblyExt { Id = "dummyEntryId1", Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04",
                ModifiedProperties = new string[] { "Attr01", "Attr02" } };
            var assemblyExt2 = new AssemblyExt { Id = "dummyEntryId2", Attr01 = "Attr05", Attr02 = "Attr06", Attr03 = "Attr07", Attr04 = "Attr08" };
            var assemblyExts = new AssemblyExt[] { assemblyExt1, assemblyExt2 };

            var assembly1 = new AssemblyDb { Id = "dummyEntryId1" };
            var assembly2 = new AssemblyDb { Id = "dummyEntryId2" };
            var assemblys = new AssemblyDb[] { assembly1, assembly2 };

            var dbEntry1 = new AssemblyExt { Id = "dummyEntryId1", Attr01 = "Attr09", Attr02 = "Attr10", Attr03 = "Attr11", Attr04 = "Attr12" };
            var dbEntry2 = new AssemblyExt { Id = "dummyEntryId2", Attr01 = "Attr13", Attr02 = "Attr14", Attr03 = "Attr15", Attr04 = "Attr16" };

            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().FindAsync(assemblyExt1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().FindAsync(assemblyExt2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly1.Id)).Returns(Task.FromResult(assembly1));
            mockEfDbContext.Setup(x => x.Set<AssemblyDb>().FindAsync(assembly2.Id)).Returns(Task.FromResult(assembly2));
            mockEfDbContext.Setup(x => x.Set<AssemblyExt>().Add(It.IsAny<AssemblyExt>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            assemblyService.EditExtendedAsync(assemblyExts).Wait();

            //Assert
            Assert.IsTrue(assemblyExt1.Attr01 == dbEntry1.Attr01); Assert.IsTrue(assemblyExt1.Attr02 == dbEntry1.Attr02);
            Assert.IsTrue(assemblyExt1.Attr03 != dbEntry1.Attr03); Assert.IsTrue(assemblyExt1.Attr04 != dbEntry1.Attr04);
            Assert.IsTrue(assemblyExt2.Attr01 != dbEntry2.Attr01); Assert.IsTrue(assemblyExt2.Attr02 != dbEntry2.Attr02);
            Assert.IsTrue(assemblyExt2.Attr03 != dbEntry2.Attr03); Assert.IsTrue(assemblyExt2.Attr04 != dbEntry2.Attr04);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Set<AssemblyExt>().FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Set<AssemblyDb>().FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.Set<AssemblyExt>().Add(It.IsAny<AssemblyExt>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        

    }
}
