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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = false, Comments = "DummyComments1",
                ProgramAddress = "DummyInfo1", AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true, Comments = "DummyComments2",
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultComponents = componentService.GetAsync(new[] { "dummyEntryId1", "dummyEntryId2" }).Result;
            
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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var location1 = new Location { Id = "dummyLocId1", LocName = "Loc1", AssignedToProject_Id = project1.Id, AssignedToProject = project1 };
            var location2 = new Location { Id = "dummyLocId2", LocName = "Loc2", AssignedToProject_Id = project2.Id, AssignedToProject = project2 };

            var assembly1 = new AssemblyDb { Id = "DummyAssyId1", AssyName = "AssyName1", AssignedToLocation_Id = location1.Id, AssignedToLocation = location1 };
            var assembly2 = new AssemblyDb { Id = "DummyAssyId2", AssyName = "AssyName2", AssignedToLocation_Id = location2.Id, AssignedToLocation = location2 };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = false, Comments = "DummyComments1",
                ProgramAddress = "DummyInfo1", AssignedToProject = project1, AssignedToAssemblyDb_Id = assembly1.Id, AssignedToAssemblyDb = assembly1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true, Comments = "DummyComments2",
                ProgramAddress = "DummyInfo2", AssignedToProject = project2 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive_bl = true, Comments = "DummyComments3",
                ProgramAddress = "DummyInfo3", AssignedToProject = project2, AssignedToAssemblyDb_Id = assembly2.Id, AssignedToAssemblyDb = assembly2 };
            var dbEntry4 = new Component { Id = "dummyEntryId4", CompName = "Comp4", CompAltName = "CompAlt4", IsActive_bl = true, Comments = "DummyComments4",
                ProgramAddress = "DummyInfo4", AssignedToProject = project2, AssignedToAssemblyDb_Id = assembly1.Id, AssignedToAssemblyDb = assembly1 };

            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3, dbEntry4 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultComponents = componentService.GetAsync(new[] { dbEntry1.Id, dbEntry2.Id, dbEntry3.Id, dbEntry4.Id}).Result;
            
            //Assert
            Assert.IsTrue(resultComponents.Count == 1);
            Assert.IsTrue(resultComponents[0].CompName == dbEntry4.CompName);
        }

        [TestMethod]
        public void ComponentService_GetByAltIdsAsync_DoeNotReturnComponentsFromWrongProject()
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

            var location1 = new Location { Id = "dummyLocId1", LocName = "Loc1", AssignedToProject_Id = project1.Id, AssignedToProject = project1 };
            var location2 = new Location { Id = "dummyLocId2", LocName = "Loc2", AssignedToProject_Id = project2.Id, AssignedToProject = project2 };

            var assembly1 = new AssemblyDb { Id = "DummyAssyId1", AssyName = "AssyName1", AssignedToLocation_Id = location1.Id, AssignedToLocation = location1 };
            var assembly2 = new AssemblyDb { Id = "DummyAssyId2", AssyName = "AssyName2", AssignedToLocation_Id = location2.Id, AssignedToLocation = location2 };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = true, Comments = "DummyComments1",
                ProgramAddress = "DummyInfo1", AssignedToProject_Id = project1.Id , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true, Comments = "DummyComments2",
                ProgramAddress = "DummyInfo2", AssignedToProject_Id = project2.Id, AssignedToProject = project2 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive_bl = true, Comments = "DummyComments3",
                ProgramAddress = "DummyInfo3", AssignedToProject_Id = project2.Id, AssignedToProject = project2, AssignedToAssemblyDb_Id = assembly2.Id, AssignedToAssemblyDb = assembly2 };
            var dbEntry4 = new Component { Id = "dummyEntryId4", CompName = "Comp4", CompAltName = "CompAlt4", IsActive_bl = true, Comments = "DummyComments4",
                ProgramAddress = "DummyInfo4", AssignedToProject_Id = project2.Id, AssignedToProject = project2, AssignedToAssemblyDb_Id = assembly1.Id, AssignedToAssemblyDb = assembly1 };

            var dbEntries = (new List<Component> { dbEntry1, dbEntry2, dbEntry3, dbEntry4 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Component>>();
            mockDbSet.As<IDbAsyncEnumerable<Component>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Component>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Component>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Component>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Components).Returns(mockDbSet.Object);

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultComponents = componentService.GetByAltIdsAsync(new[] { project1.Id }, null, null).Result;
            
            //Assert
            Assert.IsTrue(resultComponents.Count == 2);
            Assert.IsTrue(resultComponents[0].CompName == dbEntry1.CompName);
            Assert.IsTrue(resultComponents[1].CompName == dbEntry4.CompName);
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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = false, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive_bl = true, 
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = componentService.GetAsync(new string[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = false, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive_bl = false, 
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = componentService.GetAsync(new[] { "dummyEntryId3" }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", IsActive_bl = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", IsActive_bl = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", IsActive_bl = false, 
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedComps = componentService.LookupAsync("", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", CompAltName2 = "CompAlt1", IsActive_bl = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", CompAltName2 = "CompAlt2", IsActive_bl = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", CompAltName2 = "CompAlt3", IsActive_bl = false, 
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedComps = componentService.LookupAsync("Comp1", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Component { Id = "dummyEntryId1", CompName = "Comp1", CompAltName = "CompAlt1", CompAltName2 = "CompAlt1", IsActive_bl = true, 
                ProgramAddress = "DummyInfo1" , AssignedToProject = project1 };
            var dbEntry2 = new Component { Id = "dummyEntryId2", CompName = "Comp2", CompAltName = "CompAlt2", CompAltName2 = "CompAlt2", IsActive_bl = true,
                ProgramAddress = "DummyInfo2" , AssignedToProject = project1 };
            var dbEntry3 = new Component { Id = "dummyEntryId3", CompName = "Comp3", CompAltName = "CompAlt3", CompAltName2 = "CompAlt3", IsActive_bl = false, 
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

            var componentService = new ComponentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedComps = componentService.LookupAsync("Comp3", true).Result;

            //Assert
            Assert.IsTrue(returnedComps.Count == 0);
        }

        

    }
}
