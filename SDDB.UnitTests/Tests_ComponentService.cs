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
    public class Tests_ComponentService
    {
        [TestMethod]
        public void ComponentService_GetAsync_ReturnsComponents()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, Comments = "DummyComments1",
                ProgramAddress = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true, Comments = "DummyComments2",
                ProgramAddress = "DummyInfo2", AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var resultComponents = componentService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultComponents.Count == 1);
            Assert.IsTrue(resultComponents[0].CompAltName.Contains("CompAlt2"));
        }

        [TestMethod]
        public void ComponentService_GetAsync_DoeNotReturnComponentsFromWrongProject()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, Comments = "DummyComments1",
                ProgramAddress = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true, Comments = "DummyComments2",
                ProgramAddress = "DummyInfo2", AssignedToProject = project2 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var resultComponents = componentService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultComponents.Count == 0);
        }

        [TestMethod]
        public void ComponentService_GetAsync_ReturnsComponentsByIds()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive = true, 
                ProgramAddress = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].CompAltName.Contains("CompAlt3"));

        }

        [TestMethod]
        public void ComponentService_GetAsync_DoesNotReturnComponentsByIdsIfNotActive()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive = false, 
                ProgramAddress = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentService_LookupAsync_ReturnsAllRecords()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive = false, 
                ProgramAddress = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var returnedComps = componentService.LookupAsync(projectPerson1.Id,"", true).Result;

            //Assert
            Assert.IsTrue(returnedComps.Count == 2);
            Assert.IsTrue(returnedComps[1].CompAltName.Contains("CompAlt2"));
        }

        [TestMethod]
        public void ComponentService_LookupAsync_ReturnsMatchingRecords()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", CompAltName2 = "CompAlt1", IsActive = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", CompAltName2 = "CompAlt2", IsActive = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", CompAltName2 = "CompAlt3", IsActive = false, 
                ProgramAddress = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var returnedComps = componentService.LookupAsync(projectPerson1.Id,"Comp1", true).Result;

            //Assert
            Assert.IsTrue(returnedComps.Count == 1);
            Assert.IsTrue(returnedComps[0].CompAltName.Contains("CompAlt1"));
        }

        [TestMethod]
        public void ComponentService_LookupAsync_ReturnsNoRecordsIfNotActive()
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

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", CompAltName2 = "CompAlt1", IsActive = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", CompAltName2 = "CompAlt2", IsActive = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", CompAltName2 = "CompAlt3", IsActive = false, 
                ProgramAddress = "DummyInfo3" , AssignedToProject = project1 };
            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var returnedComps = componentService.LookupAsync(projectPerson1.Id,"Comp3", true).Result;

            //Assert
            Assert.IsTrue(returnedComps.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void ComponentService_EditAsync_DoesNothingIfNoComponent()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var components = new Component[] { };

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentService_EditAsync_CreatesComponentIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var component1 = new Component { Id = initialId, CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, ProgramAddress = "DummyInfo1" };
            var components = new Component[] { component1 };

            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult<Component>(null));
            mockEfDbContext.Setup(x => x.Components.Add(component1)).Returns(component1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(component1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Components.Add(component1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_EditAsync_CreatesManyComponentsIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "newEntryId1";
            var component1 = new Component { Id = initialId, CompName = "Comp", IsActive = true, ProgramAddress = "DummyInfo" };
            var component2 = new Component { Id = initialId, CompName = "Comp", IsActive = false, ProgramAddress = "DummyInfo" };
            var component3 = new Component { Id = initialId, CompName = "Comp", IsActive = true, ProgramAddress = "DummyInfo" };
            var components = new Component[] { component1, component2, component3 };

            mockEfDbContext.Setup(x => x.Components.FindAsync(It.IsAny<Component>())).Returns(Task.FromResult<Component>(null));
            mockEfDbContext.Setup(x => x.Components.Add(It.IsAny<Component>())).Returns((Component x) => x);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); 
            Assert.IsTrue(regex.IsMatch(component1.Id)); Assert.IsTrue(regex.IsMatch(component2.Id)); Assert.IsTrue(regex.IsMatch(component3.Id));
            Assert.IsTrue(component2.CompName.Contains("002")); Assert.IsTrue(component3.CompAltName.Contains("003"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(initialId), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.Components.Add(It.IsAny<Component>()), Times.Exactly(3));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        [TestMethod]
        public void ComponentService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var component1 = new Component { Id = "dummyComponentId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, ProgramAddress = "DummyInfo1",
                                      ModifiedProperties = new string[] { "CompName", "CompAltName" }};
            var component2 = new Component { Id = "dummyComponentId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive = true, ProgramAddress = "DummyInfo2" };
            var components = new Component[] { component1, component2 };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "DbEntry1", CompAltName = "DbEntryAlt1", IsActive = true, ProgramAddress = "DbEntryInfo1" };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "DbEntry2", CompAltName = "DbEntryAlt2", IsActive = false, ProgramAddress = "DbEntryInfo2" };

            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Components.Add(component1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(component1.CompName == dbEntry1.CompName); Assert.IsTrue(component1.CompAltName == dbEntry1.CompAltName);
            Assert.IsTrue(component1.ProgramAddress != dbEntry1.ProgramAddress); Assert.IsTrue(component1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(component2.CompName != dbEntry2.CompName); Assert.IsTrue(component2.CompAltName != dbEntry2.CompAltName);
            Assert.IsTrue(component2.ProgramAddress != dbEntry2.ProgramAddress); Assert.IsTrue(component2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Components.Add(It.IsAny<Component>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var component1 = new Component { Id = initialId, CompName = "Comp1", CompAltName = "CompAlt1", IsActive = false, ProgramAddress = "DummyInfo1" };
            var components = new Component[] { component1 };

            mockEfDbContext.Setup(x => x.Components.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<Component>(null));
            mockEfDbContext.Setup(x => x.Components.Add(component1)).Returns(component1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(component1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void ComponentService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var componentIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new Component { IsActive = true };
            mockEfDbContext.Setup(x => x.Components.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.Components.FindAsync("dummyId2")).Returns(Task.FromResult<Component>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.DeleteAsync(componentIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var componentIds = new string[] { "dummyId1" };

            var dbEntry = new Component { IsActive = true };
            mockEfDbContext.Setup(x => x.Components.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.DeleteAsync(componentIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void ComponentService_EditExtAsync_DoesNothingIfNoComponentExt()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var components = new ComponentExt[] { };

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditExtAsync(components).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<object[]>()), Times.Never);
            mockEfDbContext.Verify(x => x.ComponentExts.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void ComponentService_EditExtAsync_CreatesComponentExtIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var componentExt1 = new ComponentExt { Id = initialId, Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04" };
            var componentExts = new ComponentExt[] { componentExt1 };

            var component1 = new Component { Id = initialId};
            var components = new Component[] { component1 };

            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(component1.Id)).Returns(Task.FromResult<ComponentExt>(null));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult(component1));
            mockEfDbContext.Setup(x => x.ComponentExts.Add(componentExt1)).Returns(componentExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditExtAsync(componentExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.Add(componentExt1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_EditExtAsync_ReturnsErrorDbIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var componentExt1 = new ComponentExt { Id = initialId, Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04" };
            var componentExts = new ComponentExt[] { componentExt1 };

            var component1 = new Component { Id = initialId };
            var components = new Component[] { component1 };

            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(component1.Id)).Returns(Task.FromResult<ComponentExt>(null));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult<Component>(null));
            mockEfDbContext.Setup(x => x.ComponentExts.Add(componentExt1)).Returns(componentExt1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditExtAsync(componentExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Component with id=dummyEntryId1 not found.\n"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Components.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.Add(componentExt1), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_EditExtAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var componentExt1 = new ComponentExt { Id = "dummyEntryId1", Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04",
                ModifiedProperties = new string[] { "Attr01", "Attr02" } };
            var componentExt2 = new ComponentExt { Id = "dummyEntryId2", Attr01 = "Attr05", Attr02 = "Attr06", Attr03 = "Attr07", Attr04 = "Attr08" };
            var componentExts = new ComponentExt[] { componentExt1, componentExt2 };

            var component1 = new Component { Id = "dummyEntryId1" };
            var component2 = new Component { Id = "dummyEntryId2" };
            var components = new Component[] { component1, component2 };

            var dbEntry1 = new ComponentExt { Id = "dummyEntryId1", Attr01 = "Attr09", Attr02 = "Attr10", Attr03 = "Attr11", Attr04 = "Attr12" };
            var dbEntry2 = new ComponentExt { Id = "dummyEntryId2", Attr01 = "Attr13", Attr02 = "Attr14", Attr03 = "Attr15", Attr04 = "Attr16" };
            
            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(componentExt1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(componentExt2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult(component1));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component2.Id)).Returns(Task.FromResult(component2));
            mockEfDbContext.Setup(x => x.ComponentExts.Add(It.IsAny<ComponentExt>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditExtAsync(componentExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(componentExt1.Attr01 == dbEntry1.Attr01); Assert.IsTrue(componentExt1.Attr02 == dbEntry1.Attr02);
            Assert.IsTrue(componentExt1.Attr03 != dbEntry1.Attr03); Assert.IsTrue(componentExt1.Attr04 != dbEntry1.Attr04);
            Assert.IsTrue(componentExt2.Attr01 != dbEntry2.Attr01); Assert.IsTrue(componentExt2.Attr02 != dbEntry2.Attr02);
            Assert.IsTrue(componentExt2.Attr03 != dbEntry2.Attr03); Assert.IsTrue(componentExt2.Attr04 != dbEntry2.Attr04);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.ComponentExts.Add(It.IsAny<ComponentExt>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ComponentService_EditExtAsync_ReturnsErrorFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var componentExt1 = new ComponentExt { Id = "dummyEntryId1", Attr01 = "Attr01", Attr02 = "Attr02", Attr03 = "Attr03", Attr04 = "Attr04",
                ModifiedProperties = new string[] { "Attr01", "Attr02" } };
            var componentExt2 = new ComponentExt { Id = "dummyEntryId2", Attr01 = "Attr05", Attr02 = "Attr06", Attr03 = "Attr07", Attr04 = "Attr08" };
            var componentExts = new ComponentExt[] { componentExt1, componentExt2 };

            var component1 = new Component { Id = "dummyEntryId1" };
            var component2 = new Component { Id = "dummyEntryId2" };
            var components = new Component[] { component1, component2 };

            var dbEntry1 = new ComponentExt { Id = "dummyEntryId1", Attr01 = "Attr09", Attr02 = "Attr10", Attr03 = "Attr11", Attr04 = "Attr12" };
            var dbEntry2 = new ComponentExt { Id = "dummyEntryId2", Attr01 = "Attr13", Attr02 = "Attr14", Attr03 = "Attr15", Attr04 = "Attr16" };
            
            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(componentExt1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.ComponentExts.FindAsync(componentExt2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component1.Id)).Returns(Task.FromResult(component1));
            mockEfDbContext.Setup(x => x.Components.FindAsync(component2.Id)).Returns(Task.FromResult(component2));
            mockEfDbContext.Setup(x => x.ComponentExts.Add(It.IsAny<ComponentExt>())).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var componentService = new ComponentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = componentService.EditExtAsync(componentExts).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(componentExt1.Attr01 == dbEntry1.Attr01); Assert.IsTrue(componentExt1.Attr02 == dbEntry1.Attr02);
            Assert.IsTrue(componentExt1.Attr03 != dbEntry1.Attr03); Assert.IsTrue(componentExt1.Attr04 != dbEntry1.Attr04);
            Assert.IsTrue(componentExt2.Attr01 != dbEntry2.Attr01); Assert.IsTrue(componentExt2.Attr02 != dbEntry2.Attr02);
            Assert.IsTrue(componentExt2.Attr03 != dbEntry2.Attr03); Assert.IsTrue(componentExt2.Attr04 != dbEntry2.Attr04);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.ComponentExts.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Components.FindAsync(It.IsAny<string>()), Times.Never);
            mockEfDbContext.Verify(x => x.ComponentExts.Add(It.IsAny<ComponentExt>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

    }
}
