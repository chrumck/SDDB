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
        public void AssemblyDbService_GetAsync_ReturnsAssemblyDbs()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, Comments = "DummyComments1",
                TechnicalDetails = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true, Comments = "DummyComments2",
                TechnicalDetails = "DummyInfo2", AssignedToProject = project1 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyDbs = assemblyService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyDbs.Count == 1);
            Assert.IsTrue(resultAssemblyDbs[0].AssyAltName.Contains("AssyAlt2"));
        }

        [TestMethod]
        public void AssemblyDbService_GetAsync_DoeNotReturnAssemblyDbsFromWrongProject()
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

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, Comments = "DummyComments1",
                TechnicalDetails = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true, Comments = "DummyComments2",
                TechnicalDetails = "DummyInfo2", AssignedToProject = project2 };
            var dbEntries = (new List<AssemblyDb> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<AssemblyDb>>();
            mockDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(dbEntries.Provider));
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockDbSet.Object);

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var resultAssemblyDbs = assemblyService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultAssemblyDbs.Count == 0);
        }

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive = true, 
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

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive = false, 
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

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", IsActive = false, 
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

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssys = assemblyService.LookupAsync(projectPerson1.Id,"", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", AssyAltName2 = "AssyAlt1", IsActive = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", AssyAltName2 = "AssyAlt2", IsActive = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", AssyAltName2 = "AssyAlt3", IsActive = false, 
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

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssys = assemblyService.LookupAsync(projectPerson1.Id,"Assy1", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", AssyAltName2 = "AssyAlt1", IsActive = true, 
                TechnicalDetails = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", AssyAltName2 = "AssyAlt2", IsActive = true,
                TechnicalDetails = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new AssemblyDb { Id = "dummyEntryId3", AssyName = "Assy3", AssyAltName = "AssyAlt3", AssyAltName2 = "AssyAlt3", IsActive = false, 
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

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var returnedAssys = assemblyService.LookupAsync(projectPerson1.Id,"Assy3", true).Result;

            //Assert
            Assert.IsTrue(returnedAssys.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void AssemblyDbService_EditAsync_DoesNothingIfNoAssemblyDb()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assemblys = new AssemblyDb[] { };

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void AssemblyDbService_EditAsync_CreatesAssemblyDbIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assembly1 = new AssemblyDb { Id = initialId, AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, TechnicalDetails = "DummyInfo1" };
            var assemblys = new AssemblyDb[] { assembly1 };

            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.AssemblyDbs.Add(assembly1)).Returns(assembly1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assembly1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.Add(assembly1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyDbService_EditAsync_CreatesManyAssemblyDbsIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "newEntryId1";
            var assembly1 = new AssemblyDb { Id = initialId, AssyName = "Assy", IsActive = true, TechnicalDetails = "DummyInfo" };
            var assembly2 = new AssemblyDb { Id = initialId, AssyName = "Assy", IsActive = false, TechnicalDetails = "DummyInfo" };
            var assembly3 = new AssemblyDb { Id = initialId, AssyName = "Assy", IsActive = true, TechnicalDetails = "DummyInfo" };
            var assemblys = new AssemblyDb[] { assembly1, assembly2, assembly3 };

            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(It.IsAny<AssemblyDb>())).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.AssemblyDbs.Add(It.IsAny<AssemblyDb>())).Returns((AssemblyDb x) => x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); 
            Assert.IsTrue(regex.IsMatch(assembly1.Id)); Assert.IsTrue(regex.IsMatch(assembly2.Id)); Assert.IsTrue(regex.IsMatch(assembly3.Id));
            Assert.IsTrue(assembly2.AssyName.Contains("002")); Assert.IsTrue(assembly3.AssyAltName.Contains("003"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(initialId), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.AssemblyDbs.Add(It.IsAny<AssemblyDb>()), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        [TestMethod]
        public void AssemblyDbService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assembly1 = new AssemblyDb { Id = "dummyAssemblyDbId1", AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, TechnicalDetails = "DummyInfo1",
                                      ModifiedProperties = new string[] { "AssyName", "AssyAltName" }};
            var assembly2 = new AssemblyDb { Id = "dummyAssemblyDbId2", AssyName = "Assy2", AssyAltName = "AssyAlt2", IsActive = true, TechnicalDetails = "DummyInfo2" };
            var assemblys = new AssemblyDb[] { assembly1, assembly2 };

            var dbEntry1 = new AssemblyDb { Id = "dummyEntryId1", AssyName = "DbEntry1", AssyAltName = "DbEntryAlt1", IsActive = true, TechnicalDetails = "DbEntryInfo1" };
            var dbEntry2 = new AssemblyDb { Id = "dummyEntryId2", AssyName = "DbEntry2", AssyAltName = "DbEntryAlt2", IsActive = false, TechnicalDetails = "DbEntryInfo2" };

            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyDbs.Add(assembly1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assembly1.AssyName == dbEntry1.AssyName); Assert.IsTrue(assembly1.AssyAltName == dbEntry1.AssyAltName);
            Assert.IsTrue(assembly1.TechnicalDetails != dbEntry1.TechnicalDetails); Assert.IsTrue(assembly1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(assembly2.AssyName != dbEntry2.AssyName); Assert.IsTrue(assembly2.AssyAltName != dbEntry2.AssyAltName);
            Assert.IsTrue(assembly2.TechnicalDetails != dbEntry2.TechnicalDetails); Assert.IsTrue(assembly2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyDbs.Add(It.IsAny<AssemblyDb>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyDbService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var assembly1 = new AssemblyDb { Id = initialId, AssyName = "Assy1", AssyAltName = "AssyAlt1", IsActive = false, TechnicalDetails = "DummyInfo1" };
            var assemblys = new AssemblyDb[] { assembly1 };

            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.AssemblyDbs.Add(assembly1)).Returns(assembly1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(assembly1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void AssemblyDbService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assemblyIds = new string[] { "dummyId1", "DummyId2" };

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry = new AssemblyDb { IsActive = true };
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync("dummyId2")).Returns(Task.FromResult<AssemblyDb>(null));
            
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.DeleteAsync(assemblyIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyDbService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assemblyIds = new string[] { "dummyId1" };

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry = new AssemblyDb { IsActive = true };
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.DeleteAsync(assemblyIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void AssemblyDbService_EditExtAsync_DoesNothingIfNoAssemblyExt()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var assemblys = new AssemblyExt[] { };

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditExtAsync(assemblys).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<object[]>()), Times.Never);
            mockEfDbContext.Verify(x => x.AssemblyExts.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


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

            var assembly1 = new AssemblyDb { Id = initialId};
            var assemblys = new AssemblyDb[] { assembly1 };

            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assembly1.Id)).Returns(Task.FromResult<AssemblyExt>(null));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult(assembly1));
            mockEfDbContext.Setup(x => x.AssemblyExts.Add(assemblyExt1)).Returns(assemblyExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditExtAsync(assemblyExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.Add(assemblyExt1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
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

            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assembly1.Id)).Returns(Task.FromResult<AssemblyExt>(null));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult<AssemblyDb>(null));
            mockEfDbContext.Setup(x => x.AssemblyExts.Add(assemblyExt1)).Returns(assemblyExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditExtAsync(assemblyExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Assembly with id=dummyEntryId1 not found.\n"));
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
            
            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assemblyExt1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assemblyExt2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult(assembly1));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly2.Id)).Returns(Task.FromResult(assembly2));
            mockEfDbContext.Setup(x => x.AssemblyExts.Add(It.IsAny<AssemblyExt>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditExtAsync(assemblyExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(assemblyExt1.Attr01 == dbEntry1.Attr01); Assert.IsTrue(assemblyExt1.Attr02 == dbEntry1.Attr02);
            Assert.IsTrue(assemblyExt1.Attr03 != dbEntry1.Attr03); Assert.IsTrue(assemblyExt1.Attr04 != dbEntry1.Attr04);
            Assert.IsTrue(assemblyExt2.Attr01 != dbEntry2.Attr01); Assert.IsTrue(assemblyExt2.Attr02 != dbEntry2.Attr02);
            Assert.IsTrue(assemblyExt2.Attr03 != dbEntry2.Attr03); Assert.IsTrue(assemblyExt2.Attr04 != dbEntry2.Attr04);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.AssemblyExts.Add(It.IsAny<AssemblyExt>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void AssemblyDbService_EditExtAsync_ReturnsErrorFromSaveChanges()
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
            
            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assemblyExt1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.AssemblyExts.FindAsync(assemblyExt2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly1.Id)).Returns(Task.FromResult(assembly1));
            mockEfDbContext.Setup(x => x.AssemblyDbs.FindAsync(assembly2.Id)).Returns(Task.FromResult(assembly2));
            mockEfDbContext.Setup(x => x.AssemblyExts.Add(It.IsAny<AssemblyExt>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var assemblyService = new AssemblyDbService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = assemblyService.EditExtAsync(assemblyExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(assemblyExt1.Attr01 == dbEntry1.Attr01); Assert.IsTrue(assemblyExt1.Attr02 == dbEntry1.Attr02);
            Assert.IsTrue(assemblyExt1.Attr03 != dbEntry1.Attr03); Assert.IsTrue(assemblyExt1.Attr04 != dbEntry1.Attr04);
            Assert.IsTrue(assemblyExt2.Attr01 != dbEntry2.Attr01); Assert.IsTrue(assemblyExt2.Attr02 != dbEntry2.Attr02);
            Assert.IsTrue(assemblyExt2.Attr03 != dbEntry2.Attr03); Assert.IsTrue(assemblyExt2.Attr04 != dbEntry2.Attr04);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.AssemblyExts.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.AssemblyDbs.FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.AssemblyExts.Add(It.IsAny<AssemblyExt>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

    }
}
