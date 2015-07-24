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
    public class Tests_PersonGroupService
    {
        [TestMethod]
        public void PersonGroupService_GetAsync_ReturnsPersonGroups()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false  };
            var dbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true  };
            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var resultPersonGroups = PersonGroupService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultPersonGroups.Count == 1);
            Assert.IsTrue(resultPersonGroups[0].PrsGroupAltName.Contains("PersonGroupAlt2"));
        }

        [TestMethod]
        public void PersonGroupService_GetAsync_ReturnsPersonGroupsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var dbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true };
            var dbEntry3 = new PersonGroup { Id = "dummyEntryId3", PrsGroupName = "PersonGroup3", PrsGroupAltName = "PersonGroupAlt3", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].PrsGroupAltName.Contains("PersonGroupAlt3"));

        }

        [TestMethod]
        public void PersonGroupService_GetAsync_DoesNotReturnPersonGroupsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var dbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var dbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true };
            var dbEntry3 = new PersonGroup { Id = "dummyEntryId3", PrsGroupName = "PersonGroup3", PrsGroupAltName = "PersonGroupAlt3", IsActive_bl = false };
            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonGroupService_EditAsync_DoesNothingIfNoPersonGroup()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroups = new PersonGroup[] { };

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.EditAsync(PersonGroups).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void PersonGroupService_EditAsync_CreatesPersonGroupIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var initialId = "dummyEntryId1";
            var PersonGroup1 = new PersonGroup { Id = initialId, PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var PersonGroups = new PersonGroup[] { PersonGroup1 };

            mockEfDbContext.Setup(x => x.PersonGroups.FindAsync(PersonGroup1.Id)).Returns(Task.FromResult<PersonGroup>(null));
            mockEfDbContext.Setup(x => x.PersonGroups.Add(PersonGroup1)).Returns(PersonGroup1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.EditAsync(PersonGroups).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(PersonGroup1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.Add(PersonGroup1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonGroupService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroup1 = new PersonGroup {Id = "dummyPersonGroupId1", PrsGroupName = "dummyPersonGroup1", PrsGroupAltName = "DummyPersonGroupAlt1", IsActive_bl = true, 
                ModifiedProperties = new string[] { "PrsGroupName", "PrsGroupAltName" } };
            var PersonGroup2 = new PersonGroup { Id = "dummyPersonGroupId2", PrsGroupName = "dummyPersonGroup2", PrsGroupAltName = "DummyPersonGroupAlt2", IsActive_bl = false };

            var PersonGroups = new PersonGroup[] { PersonGroup1, PersonGroup2 };

            var dbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "entryPersonGroup1", PrsGroupAltName = "entryPersonGroupAlt1", IsActive_bl = false };
            var dbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "entryPersonGroup2", PrsGroupAltName = "entryPersonGroupAlt2", IsActive_bl = true };

            mockEfDbContext.Setup(x => x.PersonGroups.FindAsync(PersonGroup1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonGroups.FindAsync(PersonGroup2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.PersonGroups.Add(PersonGroup1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.EditAsync(PersonGroups).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(PersonGroup1.PrsGroupName == dbEntry1.PrsGroupName); Assert.IsTrue(PersonGroup1.PrsGroupAltName == dbEntry1.PrsGroupAltName);
            Assert.IsTrue(PersonGroup1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(PersonGroup2.PrsGroupName != dbEntry2.PrsGroupName); Assert.IsTrue(PersonGroup2.PrsGroupAltName != dbEntry2.PrsGroupAltName);
            Assert.IsTrue(PersonGroup2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.PersonGroups.Add(It.IsAny<PersonGroup>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonGroupService_DeleteAsync_CallsUserServiceEditManagedPersonGroupsAsync()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroupIds = new string[] { };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            mockPersonService.Setup(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockPersonService.Setup(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.DeleteAsync(PersonGroupIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<string>()), Times.Never);
            mockPersonService.Verify(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockPersonService.Verify(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonGroupService_DeleteAsync_CallsUserServiceReturnsError()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroupIds = new string[] { };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            mockPersonService.Setup(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockPersonService.Setup(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "DummyUserError" }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.DeleteAsync(PersonGroupIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyUserError"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<string>()), Times.Never);
            mockPersonService.Verify(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockPersonService.Verify(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonGroupService_DeleteAsync_CallsUserServiceReturnsError2()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroupIds = new string[] { };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);
            
            mockPersonService.Setup(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "DummyUserError" }));

            mockPersonService.Setup(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));
            
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.DeleteAsync(PersonGroupIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyUserError"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<string>()), Times.Never);
            mockPersonService.Verify(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockPersonService.Verify(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonGroupService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockDbContextScopeFac.Object, mockAppUserManager.Object, true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var PersonGroupIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntries = (new List<Person> { dbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry = new PersonGroup { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.PersonGroups.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.PersonGroups.FindAsync("dummyId2")).Returns(Task.FromResult<PersonGroup>(null));

            mockPersonService.Setup(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockPersonService.Setup(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false))
                .Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var serviceResult = PersonGroupService.DeleteAsync(PersonGroupIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockPersonService.Verify(x => x.EditPersonGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockPersonService.Verify(x => x.EditManagedGroupsAsync(new string[] { dbEntry1.Id }, PersonGroupIds, false), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonGroupService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var dbEntry1 = new PersonGroup { Id = "dummyId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = true,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry2 = new PersonGroup { Id = "dummyId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = false,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry3 = new PersonGroup { Id = "dummyId3", PrsGroupName = "PersonGroup3", PrsGroupAltName = "PersonGroupAlt3", IsActive_bl = true,
                GroupManagers = new List<Person> { groupManager1 } };
            var dbEntry4 = new PersonGroup { Id = "dummyId4", PrsGroupName = "PersonGroup4", PrsGroupAltName = "PersonGroupAlt4", IsActive_bl = true, 
                GroupManagers = new List<Person> { } };
            var dbEntry5 = new PersonGroup { Id = "dummyId5", PrsGroupName = "PersonGroup5", PrsGroupAltName = "PersonGroupAlt5", IsActive_bl = true, 
                GroupManagers = new List<Person> { groupManager2 } };

            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2, dbEntry3, dbEntry4, dbEntry5 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync(groupManager1.Id, "", true).Result;

            //Assert
            Assert.IsTrue(returnedPersonGroups.Count == 2);
            Assert.IsTrue(returnedPersonGroups[0].PrsGroupAltName.Contains("PersonGroupAlt1"));
            Assert.IsTrue(returnedPersonGroups[1].PrsGroupAltName.Contains("PersonGroupAlt3"));
        }

        [TestMethod]
        public void PersonGroupService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new PersonGroup { Id = "dummyId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = true,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry2 = new PersonGroup { Id = "dummyId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = false,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry3 = new PersonGroup { Id = "dummyId3", PrsGroupName = "PersonGroup3", PrsGroupAltName = "PersonGroupAlt3", IsActive_bl = true, 
                GroupManagers = new List<Person>{groupManager1} };

            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync(groupManager1.Id, "PersonGroupAlt1", true).Result;

            //Assert
            Assert.IsTrue(returnedPersonGroups.Count == 1);
            Assert.IsTrue(returnedPersonGroups[0].PrsGroupName.Contains("PersonGroup1"));
        }

        [TestMethod]
        public void PersonGroupService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });
            var mockPersonService = new Mock<PersonService>(new object[] { mockDbContextScopeFac.Object, mockDbUserService.Object });

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var dbEntry1 = new PersonGroup { Id = "dummyId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = true,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry2 = new PersonGroup { Id = "dummyId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = false,
                GroupManagers = new List<Person>{groupManager1} };
            var dbEntry3 = new PersonGroup { Id = "dummyId3", PrsGroupName = "PersonGroup3", PrsGroupAltName = "PersonGroupAlt3", IsActive_bl = true,
                GroupManagers = new List<Person>{groupManager1} };

            var dbEntries = (new List<PersonGroup> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, mockPersonService.Object);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync(groupManager1.Id,"website2", true).Result;

            //Assert
            Assert.IsTrue(returnedPersonGroups.Count == 0);
        }

    }
}
