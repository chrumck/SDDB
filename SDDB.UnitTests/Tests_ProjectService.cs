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
using SDDB.Domain.Abstract;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace SDDB.UnitTests
{
    [TestClass]
    public class Tests_ProjectService
    {
        [TestMethod]
        public void ProjectService_GetAsync_ReturnsProjects()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockAppUserManager.Object, mockDbContextScopeFac.Object, true});
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = true, ProjectCode = "CODE2" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var resultProjects = projectService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultProjects.Count == 1);
            Assert.IsTrue(resultProjects[0].ProjectAltName.Contains("ProjectAlt2"));
        }

        [TestMethod]
        public void ProjectService_GetAsync_ReturnsProjectsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = true, ProjectCode = "CODE2" };
            var dbEntry3 = new Project { Id = "dummyEntryId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive = true, ProjectCode = "FLA3" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].ProjectAltName.Contains("ProjectAlt3"));

        }

        [TestMethod]
        public void ProjectService_GetAsync_DoesNotReturnProjectsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = true, ProjectCode = "CODE2" };
            var dbEntry3 = new Project { Id = "dummyEntryId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive = false, ProjectCode = "FLA3" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ProjectService_EditAsync_DoesNothingIfNoProject()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projects = new Project[] { };

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.EditAsync(projects).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ProjectService_EditAsync_CreatesProjectIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var initialId = "dummyEntryId1";
            var project1 = new Project { Id = initialId, ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = false, ProjectCode = "CODE1" };
            var projects = new Project[] { project1 };

            mockEfDbContext.Setup(x => x.Projects.FindAsync(project1.Id)).Returns(Task.FromResult<Project>(null));
            mockEfDbContext.Setup(x => x.Projects.Add(project1)).Returns(project1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.EditAsync(projects).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(project1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.Add(project1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var project1 = new Project {Id = "dummyProjectId1", ProjectName = "dummyProject1", ProjectAltName = "DummyProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ModifiedProperties = new string[] { "ProjectName", "ProjectAltName" } };
            var project2 = new Project { Id = "dummyProjectId2", ProjectName = "dummyProject2", ProjectAltName = "DummyProjectAlt2", IsActive = false, ProjectCode = "CODE2" };

            var projects = new Project[] { project1, project2 };

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "entryProject1", ProjectAltName = "entryProjectAlt1", IsActive = false, ProjectCode = "EntryCODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "entryProject2", ProjectAltName = "entryProjectAlt2", IsActive = true, ProjectCode = "EntryCODE2" };

            mockEfDbContext.Setup(x => x.Projects.FindAsync(project1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Projects.FindAsync(project2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Projects.Add(project1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.EditAsync(projects).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(project1.ProjectName == dbEntry1.ProjectName); Assert.IsTrue(project1.ProjectAltName == dbEntry1.ProjectAltName);
            Assert.IsTrue(project1.ProjectCode != dbEntry1.ProjectCode); Assert.IsTrue(project1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(project2.ProjectName != dbEntry2.ProjectName); Assert.IsTrue(project2.ProjectAltName != dbEntry2.ProjectAltName);
            Assert.IsTrue(project2.ProjectCode != dbEntry2.ProjectCode); Assert.IsTrue(project2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Projects.Add(It.IsAny<Project>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var initialId = "dummyEntryId1";
            var dbEntry1 = new Project { Id = initialId, ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = false, ProjectCode = "CODE1" };
            var projects = new Project[] { dbEntry1 };

            mockEfDbContext.Setup(x => x.Projects.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<Project>(null));
            mockEfDbContext.Setup(x => x.Projects.Add(dbEntry1)).Returns(dbEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.EditAsync(projects).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(dbEntry1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ProjectService_DeleteAsync_DoesNothingIfNoProjectIds()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectIds = new string[] { };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.DeleteAsync(projectIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_DeleteAsync_CallsUserServiceEditManagedProjectsAsync()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectIds = new string[] { "dummyId1" };
            
            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var docDbEntry1 = new Document { Id = "docDummyId1", DocName = "Name1", DocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var docDbEntries = (new List<Document> { docDbEntry1 }).AsQueryable();
            var mockDocDbSet = new Mock<DbSet<Document>>();
            mockDocDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(docDbEntries.GetEnumerator()));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(docDbEntries.Provider));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(docDbEntries.Expression);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(docDbEntries.ElementType);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(docDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDocDbSet.Object);

            var locDbEntry1 = new Location { Id = "locDummyId1", LocName = "Name1", LocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var locDbEntries = (new List<Location> { locDbEntry1 }).AsQueryable();
            var mockLocDbSet = new Mock<DbSet<Location>>();
            mockLocDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(locDbEntries.GetEnumerator()));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(locDbEntries.Provider));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(locDbEntries.Expression);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(locDbEntries.ElementType);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(locDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Locations).Returns(mockLocDbSet.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "assyDummyId1", AssyName = "Name1", AssyAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockAssyDbSet = new Mock<DbSet<AssemblyDb>>();
            mockAssyDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockAssyDbSet.Object);

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry = new Project { IsActive = true };
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockPersonService.Setup(x => x.EditPersonProjectsAsync(new string[] {dbEntry1.Id}, projectIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.DeleteAsync(projectIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Once);
            mockPersonService.Verify(x => x.EditPersonProjectsAsync(new string[] { dbEntry1.Id }, projectIds, false), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_DeleteAsync_CallsUserServiceReturnsError()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectIds = new string[] { "dummyId1" };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var docDbEntry1 = new Document { Id = "docDummyId1", DocName = "Name1", DocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var docDbEntries = (new List<Document> { docDbEntry1 }).AsQueryable();
            var mockDocDbSet = new Mock<DbSet<Document>>();
            mockDocDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(docDbEntries.GetEnumerator()));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(docDbEntries.Provider));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(docDbEntries.Expression);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(docDbEntries.ElementType);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(docDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDocDbSet.Object);

            var locDbEntry1 = new Location { Id = "locDummyId1", LocName = "Name1", LocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var locDbEntries = (new List<Location> { locDbEntry1 }).AsQueryable();
            var mockLocDbSet = new Mock<DbSet<Location>>();
            mockLocDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(locDbEntries.GetEnumerator()));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(locDbEntries.Provider));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(locDbEntries.Expression);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(locDbEntries.ElementType);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(locDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Locations).Returns(mockLocDbSet.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "assyDummyId1", AssyName = "Name1", AssyAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockAssyDbSet = new Mock<DbSet<AssemblyDb>>();
            mockAssyDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockAssyDbSet.Object);

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry = new Project { IsActive = true };
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockPersonService.Setup(x => x.EditPersonProjectsAsync(new string[] { dbEntry1.Id }, projectIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "DummyUserError" }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.DeleteAsync(projectIds).Result;
            
            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyUserError"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Once);
            mockPersonService.Verify(x => x.EditPersonProjectsAsync(new string[] { dbEntry1.Id }, projectIds, false), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var docDbEntry1 = new Document { Id = "docDummyId1", DocName = "Name1", DocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var docDbEntries = (new List<Document> { docDbEntry1 }).AsQueryable();
            var mockDocDbSet = new Mock<DbSet<Document>>();
            mockDocDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(docDbEntries.GetEnumerator()));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(docDbEntries.Provider));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(docDbEntries.Expression);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(docDbEntries.ElementType);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(docDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDocDbSet.Object);

            var locDbEntry1 = new Location { Id = "locDummyId1", LocName = "Name1", LocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var locDbEntries = (new List<Location> { locDbEntry1 }).AsQueryable();
            var mockLocDbSet = new Mock<DbSet<Location>>();
            mockLocDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(locDbEntries.GetEnumerator()));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(locDbEntries.Provider));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(locDbEntries.Expression);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(locDbEntries.ElementType);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(locDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Locations).Returns(mockLocDbSet.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "assyDummyId1", AssyName = "Name1", AssyAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockAssyDbSet = new Mock<DbSet<AssemblyDb>>();
            mockAssyDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockAssyDbSet.Object);

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry = new Project { IsActive = true };
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId2")).Returns(Task.FromResult<Project>(null));

            mockPersonService.Setup(x => x.EditPersonProjectsAsync(It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.DeleteAsync(projectIds).Result;
            
            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockPersonService.Verify(x => x.EditPersonProjectsAsync(It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<bool>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ProjectService_DeleteAsync_ReturnsErrorIfDocumentsAssigned()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectIds = new string[] { "dummyId1", "dummyId2" };

            var docDbEntry1 = new Document { Id = "docDummyId1", DocName = "Name1", DocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "dummyId2" };
            var docDbEntries = (new List<Document> { docDbEntry1 }).AsQueryable();
            var mockDocDbSet = new Mock<DbSet<Document>>();
            mockDocDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(docDbEntries.GetEnumerator()));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(docDbEntries.Provider));
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(docDbEntries.Expression);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(docDbEntries.ElementType);
            mockDocDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(docDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDocDbSet.Object);

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var locDbEntry1 = new Location { Id = "locDummyId1", LocName = "Name1", LocAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var locDbEntries = (new List<Location> { locDbEntry1 }).AsQueryable();
            var mockLocDbSet = new Mock<DbSet<Location>>();
            mockLocDbSet.As<IDbAsyncEnumerable<Location>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Location>(locDbEntries.GetEnumerator()));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Location>(locDbEntries.Provider));
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(locDbEntries.Expression);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(locDbEntries.ElementType);
            mockLocDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(locDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Locations).Returns(mockLocDbSet.Object);

            var assyDbEntry1 = new AssemblyDb { Id = "assyDummyId1", AssyName = "Name1", AssyAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var assyDbEntries = (new List<AssemblyDb> { assyDbEntry1 }).AsQueryable();
            var mockAssyDbSet = new Mock<DbSet<AssemblyDb>>();
            mockAssyDbSet.As<IDbAsyncEnumerable<AssemblyDb>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<AssemblyDb>(assyDbEntries.GetEnumerator()));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<AssemblyDb>(assyDbEntries.Provider));
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.Expression).Returns(assyDbEntries.Expression);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.ElementType).Returns(assyDbEntries.ElementType);
            mockAssyDbSet.As<IQueryable<AssemblyDb>>().Setup(m => m.GetEnumerator()).Returns(assyDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.AssemblyDbs).Returns(mockAssyDbSet.Object);

            var compDbEntry1 = new Component { Id = "compDummyId1", CompName = "Name1", CompAltName = "AltName1", IsActive = true, AssignedToProject_Id = "assignedProjId" };
            var compDbEntries = (new List<Component> { compDbEntry1 }).AsQueryable();
            var mockCompDbSet = new Mock<DbSet<Component>>();
            mockCompDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(compDbEntries.GetEnumerator()));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(compDbEntries.Provider));
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(compDbEntries.Expression);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(compDbEntries.ElementType);
            mockCompDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(compDbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Components).Returns(mockCompDbSet.Object);

            var dbEntry1 = new Project { IsActive = true };
            var dbEntry2 = new Project { IsActive = true };
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Projects.FindAsync("dummyId2")).Returns(Task.FromResult(dbEntry2));

            mockPersonService.Setup(x => x.EditPersonProjectsAsync(It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = projectService.DeleteAsync(projectIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("not deleted, it has documents assigned to it"));
            Assert.IsTrue(dbEntry1.IsActive == false);
            Assert.IsTrue(dbEntry2.IsActive == true);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Projects.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockPersonService.Verify(x => x.EditPersonProjectsAsync(It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<bool>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ProjectService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var projectPerson2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry4 = new Project { Id = "dummyId4", ProjectName = "Project4", ProjectAltName = "ProjectAlt4", IsActive = true, ProjectCode = "CODE4", 
                ProjectPersons = new List<Person> { } };
            var dbEntry5 = new Project { Id = "dummyId5", ProjectName = "Project5", ProjectAltName = "ProjectAlt5", IsActive = true, ProjectCode = "CODE5", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3, dbEntry4, dbEntry5 }).AsQueryable();
            
            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedProjects = projectService.LookupAsync(projectPerson1.Id,"", true).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 2);
            Assert.IsTrue(returnedProjects[1].ProjectAltName.Contains("ProjectAlt3"));
        }

        [TestMethod]
        public void ProjectService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedProjects = projectService.LookupAsync(projectPerson1.Id,"ProjectAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].ProjectName.Contains("Project1"));
        }

        [TestMethod]
        public void ProjectService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedProjects = projectService.LookupAsync(projectPerson1.Id,"CODE2", true).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 0);
        }

    }
}
