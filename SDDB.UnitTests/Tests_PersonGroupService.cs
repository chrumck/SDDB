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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var serviceResult = PersonGroupService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync("", true).Result;

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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync("PersonGroup1", true).Result;

            //Assert
            Assert.IsTrue(returnedPersonGroups.Count == 1);
            Assert.IsTrue(returnedPersonGroups[0].PrsGroupAltName.Contains("PersonGroupAlt1"));
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

            var PersonGroupService = new PersonGroupService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedPersonGroups = PersonGroupService.LookupAsync("website2", true).Result;

            //Assert
            Assert.IsTrue(returnedPersonGroups.Count == 0);
        }

    }
}
