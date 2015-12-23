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

            

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, "dummyUserId");

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
            var mockAppRoleManager = new Mock<IAppRoleManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppRoleManager.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var dbEntry3 = new Project { Id = "dummyEntryId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive_bl = true, ProjectCode = "FLA3" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, "dummyUserId");

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
                        
            var dbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var dbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var dbEntry3 = new Project { Id = "dummyEntryId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive_bl = false, ProjectCode = "FLA3" };
            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var serviceResult = projectService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }
                        
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
            var mockAppRoleManager = new Mock<IAppRoleManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppRoleManager.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var projectPerson2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive_bl = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry4 = new Project { Id = "dummyId4", ProjectName = "Project4", ProjectAltName = "ProjectAlt4", IsActive_bl = true, ProjectCode = "CODE4", 
                ProjectPersons = new List<Person> { } };
            var dbEntry5 = new Project { Id = "dummyId5", ProjectName = "Project5", ProjectAltName = "ProjectAlt5", IsActive_bl = true, ProjectCode = "CODE5", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3, dbEntry4, dbEntry5 }).AsQueryable();
            
            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedProjects = projectService.LookupAsync("", true).Result;

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
            var mockAppRoleManager = new Mock<IAppRoleManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppRoleManager.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive_bl = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedProjects = projectService.LookupAsync("Project1", true).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].ProjectCode.Contains("CODE1"));
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
            var mockAppRoleManager = new Mock<IAppRoleManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppRoleManager.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var dbEntry3 = new Project { Id = "dummyId3", ProjectName = "Project3", ProjectAltName = "ProjectAlt3", IsActive_bl = true, ProjectCode = "CODE3", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntries = (new List<Project> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Project>>();
            mockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet.Object);

            var projectService = new ProjectService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedProjects = projectService.LookupAsync("CODE2", true).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 0);
        }

    }
}
