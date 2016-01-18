using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mehdime.Entity;

using SDDB.Domain.DbContexts;
using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.Domain.Abstract;

namespace SDDB.UnitTests
{
    [TestClass]
    public class Tests_PersonLogEntryService
    {
        Mock<IDbContextScopeFactory> mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
        Mock<IDbContextScope> mockDbContextScope = new Mock<IDbContextScope>();
        Mock<IDbContextReadOnlyScope> mockDbContextScopeRo = new Mock<IDbContextReadOnlyScope>();
        Mock<EFDbContext> mockEfDbContext = new Mock<EFDbContext>();
        Mock<IAppUserManager> mockAppUserManager = new Mock<IAppUserManager>();
        Mock<DbSet<PersonLogEntry>> prsLogEntriersMock = new Mock<DbSet<PersonLogEntry>>();
        Mock<DbSet<Project>> projectsMock= new Mock<DbSet<Project>>();
        

        string userId = "PersonId1";

        Person person1 = new Person { Id = "PersonId1", FirstName = "First1", LastName = "Last1" };
        Person person2 = new Person { Id = "PersonId2", FirstName = "First2", LastName = "Last2" };
        Person person3 = new Person { Id = "PersonId3", FirstName = "First3", LastName = "Last3" };
        Person person4 = new Person { Id = "PersonId4", FirstName = "First4", LastName = "Last4" };

        PersonActivityType activityType1 = new PersonActivityType { Id = "ActivityTypeId1", ActivityTypeName = "ActivityTypeName1" };

        Project project1 = new Project
        {
            Id = "projectId1",
            ProjectName = "Project1",
            ProjectAltName = "ProjectAlt1",
            IsActive_bl = true,
            ProjectCode = "CODE1",
            ProjectPersons = new List<Person>()
        };

        Project project2 = new Project
        {
            Id = "projectId2",
            ProjectName = "Project2",
            ProjectAltName = "ProjectAlt2",
            IsActive_bl = true,
            ProjectCode = "CODE2",
            ProjectPersons = new List<Person>()
        };

        Location location1 = new Location { Id = "LocId1", LocName = "LocName1", AssignedToProject_Id = "projectId1" };
        Location location2 = new Location { Id = "LocId1", LocName = "LocName1", AssignedToProject_Id = "projectId2" };

        ProjectEvent projEvent1 = new ProjectEvent { Id = "ProjEventId1", EventName = "EventName1", AssignedToProject_Id = "projectId1" };
        ProjectEvent projEvent2 = new ProjectEvent { Id = "ProjEventId2", EventName = "EventName2", AssignedToProject_Id = "projectId2" };

        PersonLogEntry prsLogEntry1 = new PersonLogEntry
        {
            Id = "prsLogEntryId1",
            LogEntryDateTime = new DateTime(2000, 1, 1),
            IsActive_bl = false,
            PersonLogEntryFiles = new List<PersonLogEntryFile>(),
            PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
            PrsLogEntryPersons = new List<Person>()
        };
        PersonLogEntry prsLogEntry2 = new PersonLogEntry
        {
            Id = "prsLogEntryId2",
            LogEntryDateTime = new DateTime(2000, 1, 2),
            IsActive_bl = true,
            PersonLogEntryFiles = new List<PersonLogEntryFile>(),
            PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
            PrsLogEntryPersons = new List<Person>()
        };
        PersonLogEntry prsLogEntry3 = new PersonLogEntry
        {
            Id = "prsLogEntryId3",
            LogEntryDateTime = new DateTime(2000, 1, 3),
            IsActive_bl = true,
            PersonLogEntryFiles = new List<PersonLogEntryFile>(),
            PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
            PrsLogEntryPersons = new List<Person>()
        };
        PersonLogEntry prsLogEntry4 = new PersonLogEntry
        {
            Id = "prsLogEntryId4",
            LogEntryDateTime = new DateTime(2000, 1, 4),
            IsActive_bl = true,
            PersonLogEntryFiles = new List<PersonLogEntryFile>(),
            PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
            PrsLogEntryPersons = new List<Person>()
        };
        
        PersonLogEntryService prsLogEntryService;

        //-----------------------------------------------------------------------------------------------------------------------


        public Tests_PersonLogEntryService()
        {
            //Arrange
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScopeRo.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);
            mockDbContextScopeRo.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            mockAppUserManager.Setup(x => x.IsInRoleAsync(userId, It.IsAny<string>())).Returns(Task.FromResult(true));
                        
            project1.ProjectPersons.Add(person1);
            project2.ProjectPersons.Add(person2);

            prsLogEntry1.AssignedToProject = project1;
            prsLogEntry1.EnteredByPerson = person2;
            prsLogEntry1.PersonActivityType = activityType1;
            prsLogEntry1.AssignedToLocation = location1;
            prsLogEntry1.AssignedToProjectEvent = projEvent1;
            prsLogEntry1.QcdByPerson = person1;
            prsLogEntry1.PrsLogEntryPersons.Add(person3);

            prsLogEntry2.AssignedToProject = project2;
            prsLogEntry2.EnteredByPerson = person3;
            prsLogEntry2.PersonActivityType = activityType1;
            prsLogEntry2.AssignedToLocation = location1;
            prsLogEntry2.AssignedToProjectEvent = projEvent2;
            prsLogEntry2.QcdByPerson = person1;
            prsLogEntry2.PrsLogEntryPersons.Add(person1);

            prsLogEntry3.AssignedToProject = project1;
            prsLogEntry3.EnteredByPerson = person1;
            prsLogEntry3.PersonActivityType = activityType1;
            prsLogEntry3.AssignedToLocation = location1;
            prsLogEntry3.AssignedToProjectEvent = projEvent1;
            prsLogEntry3.QcdByPerson = person1;
            prsLogEntry3.PrsLogEntryPersons.Add(person1);

            prsLogEntry4.AssignedToProject = project1;
            prsLogEntry4.EnteredByPerson = person2;
            prsLogEntry4.PersonActivityType = activityType1;
            prsLogEntry4.AssignedToLocation = location1;
            prsLogEntry4.AssignedToProjectEvent = projEvent1;
            prsLogEntry4.QcdByPerson = person3;
            prsLogEntry4.PrsLogEntryPersons.Add(person3);

            var prsLogEntrys = (new List<PersonLogEntry> { prsLogEntry1, prsLogEntry2, prsLogEntry3, prsLogEntry4 }).AsQueryable();
            prsLogEntriersMock.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(prsLogEntrys.GetEnumerator()));
            prsLogEntriersMock.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(prsLogEntrys.Provider));
            prsLogEntriersMock.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(prsLogEntrys.Expression);
            prsLogEntriersMock.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(prsLogEntrys.ElementType);
            prsLogEntriersMock.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(prsLogEntrys.GetEnumerator());
            prsLogEntriersMock.Setup(x => x.Include(It.IsAny<string>())).Returns(prsLogEntriersMock.Object);
            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(prsLogEntriersMock.Object);

            var projects = (new List<Project> { project1, project2 }).AsQueryable();
            projectsMock.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(projects.GetEnumerator()));
            projectsMock.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(projects.Provider));
            projectsMock.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projects.Expression);
            projectsMock.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projects.ElementType);
            projectsMock.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projects.GetEnumerator());
            projectsMock.Setup(x => x.Include(It.IsAny<string>())).Returns(projectsMock.Object);
            mockEfDbContext.Setup(x => x.Projects).Returns(projectsMock.Object);

            prsLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, userId, mockAppUserManager.Object);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        
        [TestMethod]
        public void PersonLogEntryService_GetAsync_ReturnsPersonLogEntrysByIds()
        {
            //Act
            var serviceResult = prsLogEntryService.GetAsync( new string[] { prsLogEntry3.Id }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LogEntryDateTime == prsLogEntry3.LogEntryDateTime);

        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_DoesNotReturnPersonLogEntrysByIdsIfNotActive()
        {
            //Act
            var serviceResult = prsLogEntryService.GetAsync(new string[] { prsLogEntry1.Id }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_ReturnsByIdsIfNotActive()
        {
            //Act
            var serviceResult = prsLogEntryService.GetAsync(new string[] { prsLogEntry1.Id }, false).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LogEntryDateTime == prsLogEntry1.LogEntryDateTime);
        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_FiltersOtherProjectsForPLEView()
        {
            //Act
            var serviceResult = prsLogEntryService.GetAsync(new string[] { prsLogEntry2.Id }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        [TestMethod]
        public void PersonLogEntryService_GetAsync_FiltersOtherPersonsForNonPLEView()
        {
            //Act
            var serviceResult = prsLogEntryService.GetAsync(new string[] { prsLogEntry2.Id }, true, false).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LogEntryDateTime == prsLogEntry2.LogEntryDateTime);
        }

        [TestMethod]
        public void PersonLogEntryService_GetByAltIdsAsync_GetsProjectPLEs()
        {
            //Act
            var serviceResult = prsLogEntryService.GetByAltIdsAsync(null, null, null, null, null, null).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 2);
            Assert.IsTrue(serviceResult[1].LogEntryDateTime == prsLogEntry4.LogEntryDateTime);
        }


                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void BaseDbService_EditAsync_DoesNotQcIfNotAuthorized()
        {
            //arrange
            var record3 = new PersonLogEntry
            {
                Id = "prsLogEntryId3",
                LogEntryDateTime = new DateTime(2000, 1, 3),
                IsActive_bl = true,
                PersonLogEntryFiles = new List<PersonLogEntryFile>(),
                PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
                PrsLogEntryPersons = new List<Person>(),
                AssignedToProject_Id = project2.Id,
                AssignedToProjectEvent_Id = projEvent1.Id,
                ModifiedProperties = new[] {"QcdByPerson_Id"}
            };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEvent1.Id)).Returns(Task.FromResult(projEvent1));
            mockAppUserManager.Setup(x => x.IsInRoleAsync(userId, "PersonLogEntry_Qc")).Returns(Task.FromResult(false));

            try
            {
                //Act
                var serviceResult = prsLogEntryService.EditAsync(new[] { record3 }).Result;
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("You do not have sufficient rights to QC Person Entries."));
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void BaseDbService_EditAsync_DoesNotQcIfYourEntry()
        {
            //arrange
            var record3 = new PersonLogEntry
            {
                Id = "prsLogEntryId3",
                LogEntryDateTime = new DateTime(2000, 1, 3),
                EnteredByPerson_Id = userId,
                IsActive_bl = true,
                PersonLogEntryFiles = new List<PersonLogEntryFile>(),
                PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
                PrsLogEntryPersons = new List<Person>(),
                AssignedToProject_Id = project2.Id,
                AssignedToProjectEvent_Id = projEvent1.Id,
                ModifiedProperties = new[] { "QcdByPerson_Id" }
            };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEvent1.Id)).Returns(Task.FromResult(projEvent1));
            mockAppUserManager.Setup(x => x.IsInRoleAsync(userId, "PersonLogEntry_Qc")).Returns(Task.FromResult(true));

            try
            {
                //Act
                var serviceResult = prsLogEntryService.EditAsync(new[] { record3 }).Result;
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("You cannot QC your own entry."));
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void BaseDbService_EditAsync_DoesNotUpdateIfProjEventIdsNotMatching()
        {
            //arrange
            var record3 = new PersonLogEntry
            {
                Id = "prsLogEntryId3",
                LogEntryDateTime = new DateTime(2000, 1, 3),
                IsActive_bl = true,
                PersonLogEntryFiles = new List<PersonLogEntryFile>(),
                PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
                PrsLogEntryPersons = new List<Person>(),
                AssignedToProject_Id = project2.Id,
                AssignedToProjectEvent_Id = projEvent1.Id
            };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEvent1.Id)).Returns(Task.FromResult<ProjectEvent>(projEvent1));

            try
            {
                //Act
                var serviceResult = prsLogEntryService.EditAsync(new[] { record3 }).Result;
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("Log Entry and Project Event do not belong to the same project"));
                return;
            }
            Assert.IsFalse(true);

        }

        [TestMethod]
        public void BaseDbService_EditAsync_DoesNotUpdateIfProjLocIdsNotMatching()
        {
            //arrange
            var record3 = new PersonLogEntry
            {
                Id = prsLogEntry3.Id,
                LogEntryDateTime = new DateTime(2000, 1, 3),
                IsActive_bl = true,
                PersonLogEntryFiles = new List<PersonLogEntryFile>(),
                PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
                PrsLogEntryPersons = new List<Person>(),
                AssignedToProject_Id = project1.Id,
                AssignedToProjectEvent_Id = projEvent1.Id,
                AssignedToLocation_Id = location2.Id
            };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEvent1.Id)).Returns(Task.FromResult<ProjectEvent>(projEvent1));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(location2.Id)).Returns(Task.FromResult<Location>(location2));

            try
            {
                //Act
                var serviceResult = prsLogEntryService.EditAsync(new[] { record3 }).Result;
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("Log Entry and Location do not belong to the same project"));
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void BaseDbService_EditAsync_DoesNotUpdateIfDbEntryProjNotManaged()
        {
            //arrange
            var record3 = new PersonLogEntry
            {
                Id = prsLogEntry2.Id,
                LogEntryDateTime = new DateTime(2000, 1, 3),
                IsActive_bl = true,
                PersonLogEntryFiles = new List<PersonLogEntryFile>(),
                PrsLogEntryAssemblyDbs = new List<AssemblyDb>(),
                PrsLogEntryPersons = new List<Person>(),
                AssignedToProject_Id = project1.Id,
                AssignedToProjectEvent_Id = projEvent1.Id,
                AssignedToLocation_Id = location1.Id
            };

            mockEfDbContext.Setup(x => x.ProjectEvents.FindAsync(projEvent1.Id)).Returns(Task.FromResult<ProjectEvent>(projEvent1));
            mockEfDbContext.Setup(x => x.Locations.FindAsync(location1.Id)).Returns(Task.FromResult<Location>(location1));

            try
            {
                //Act
                var serviceResult = prsLogEntryService.EditAsync(new[] { record3 }).Result;
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString()
                    .Contains("Log Entry you try to modify is assigned to project which is not managed by you."));
                return;
            }
            Assert.IsFalse(true);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void BaseDbService_GetActivitySummariesAsync_ReturnsProperGroups()
        {
            // Arrange
            var personId = "DummyUserId";
            var startDate = new DateTime(2000,1,1);
            var endDate = new DateTime(2000,1,3);

            var person = new Person { Id = personId };

            var project1 = new Project {Id = "ProjectId1", ProjectName = "ProjectName1", ProjectCode = "ProjectCode1" };
            var project2 = new Project { Id = "ProjectId2", ProjectName = "ProjectName2", ProjectCode = "ProjectCode2" };

            var dbEntry1 = new PersonLogEntry { Id = "Id1", LogEntryDateTime = new DateTime(2000, 1, 1, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = "otherPersonId",
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 3, PrsLogEntryPersons = new List<Person> {person}};
            var dbEntry2 = new PersonLogEntry { Id = "Id2", LogEntryDateTime = new DateTime(2000, 1, 1, 11, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 4};
            var dbEntry3 = new PersonLogEntry { Id = "Id3", LogEntryDateTime = new DateTime(2000, 1, 1, 12, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 5};
            var dbEntry4 = new PersonLogEntry { Id = "Id4", LogEntryDateTime = new DateTime(2000, 1, 1, 13, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 6};
            
            var dbEntry5 = new PersonLogEntry { Id = "Id5", LogEntryDateTime = new DateTime(2000, 1, 2, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 7};
            var dbEntry6 = new PersonLogEntry { Id = "Id6", LogEntryDateTime = new DateTime(2000, 1, 2, 11, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 8 };

            var dbEntry7 = new PersonLogEntry { Id = "Id7", LogEntryDateTime = new DateTime(2000, 1, 1, 9, 0, 0), IsActive_bl = true, EnteredByPerson_Id = "otherPersonId",
                                                AssignedToProject_Id = "ProjectId2", AssignedToProject = project2, ManHours = 100 };
            var dbEntry8 = new PersonLogEntry { Id = "Id8", LogEntryDateTime = new DateTime(2000, 1, 4, 10, 0, 0), IsActive_bl = true, EnteredByPerson_Id = personId,
                                                AssignedToProject_Id = "ProjectId1", AssignedToProject = project1, ManHours = 1000 };

            var dbEntrys = new PersonLogEntry[] { dbEntry1, dbEntry2, dbEntry3, dbEntry4, dbEntry5, dbEntry6, dbEntry7, dbEntry8 }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonLogEntry>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonLogEntry>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonLogEntry>(dbEntrys.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntrys.Provider));
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.Expression).Returns(dbEntrys.Expression);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.ElementType).Returns(dbEntrys.ElementType);
            mockDbSet.As<IQueryable<PersonLogEntry>>().Setup(m => m.GetEnumerator()).Returns(dbEntrys.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonLogEntrys).Returns(mockDbSet.Object);

            var personLogEntryService = new PersonLogEntryService(mockDbContextScopeFac.Object, personId, mockAppUserManager.Object);

            //Act
            var serviceResult = personLogEntryService.GetActivitySummariesAsync(personId, startDate, endDate).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 2);
            Assert.IsTrue(serviceResult[0].TotalManHours == dbEntry1.ManHours + dbEntry2.ManHours + dbEntry3.ManHours + dbEntry4.ManHours);
            Assert.IsTrue(serviceResult[0].SummaryDay == dbEntry4.LogEntryDateTime.Date);
            Assert.IsTrue(serviceResult[1].SummaryDetails.Count() == 2);
            Assert.IsTrue(serviceResult[1].TotalManHours == dbEntry5.ManHours + dbEntry6.ManHours);
            Assert.IsTrue(serviceResult.All(x => x.PersonId == personId));

        }

    }
}
