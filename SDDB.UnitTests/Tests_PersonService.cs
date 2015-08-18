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
    public class Tests_PersonService
    {
        [TestMethod]
        public void PersonService_GetAsync_ReturnsPersons()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntries = (new List<Person> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));

            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var resultPersons = personService.GetAllAsync().Result;
            
            //Assert
            Assert.IsTrue(resultPersons.Count == 1);
            Assert.IsTrue(resultPersons[0].LastName.Contains("Last2"));
        }

        [TestMethod]
        public void PersonService_GetAsync_ReturnsPersonsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };
            var dbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();
            
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = personService.GetAllAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LastName.Contains("Last3"));

        }

        [TestMethod]
        public void PersonService_GetAsync_DoesNotReturnPersonsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = false, Initials = "FLA3" };
            var dbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = personService.GetAllAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_GetWoDBUserAsync_ReturnsPersonsWoUser()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockDbContextScopeFac.Object, mockAppUserManager.Object,  true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3", DBUser = new DBUser() };
            var dbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();
            
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, "DummyUserId");

            //Act
            var serviceResult = personService.GetWoDBUserAsync().Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LastName.Contains("Last2"));
        }

        [TestMethod]
        public void PersonService_LookupAsync_ReturnsAllPersons()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "FirstManager1", LastName = "LastManager1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "FirstManager2", LastName = "LastManager2" };

            var personGroupDbEntry1 = new PersonGroup { GroupManagers = new List<Person> { groupManager1 }, };
            var personGroupDbEntry2 = new PersonGroup { GroupManagers = new List<Person> { groupManager2 }, };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry2 }};

            var personDbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedProjects = personService.LookupAsync().Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].LastName.Contains("Last2"));
        }

        [TestMethod]
        public void PersonService_LookupAsync_ReturnsMatchingPersons()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "FirstManager1", LastName = "LastManager1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "FirstManager2", LastName = "LastManager2" };

            var personGroupDbEntry1 = new PersonGroup { GroupManagers = new List<Person> { groupManager1 }, };
            var personGroupDbEntry2 = new PersonGroup { GroupManagers = new List<Person> { groupManager2 }, };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry1 }};

            var personDbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedProjects = personService.LookupAsync("FLA3").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].LastName.Contains("Last3"));
        }
        
        [TestMethod]
        public void PersonService_LookupAsync_ReturnsMatchingPersons2()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "FirstManager1", LastName = "LastManager1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "FirstManager2", LastName = "LastManager2" };

            var personGroupDbEntry1 = new PersonGroup { GroupManagers = new List<Person> { groupManager1 }, };
            var personGroupDbEntry2 = new PersonGroup { GroupManagers = new List<Person> { groupManager2 }, };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry1 }};
            var dbEntry4 = new Person { Id = "dummyEntryId4", FirstName = "First4", LastName = "Last4", IsActive_bl = true, Initials = "FLA4",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry2 }};

            var personDbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3, dbEntry4 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedProjects = personService.LookupAsync("FLA").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 2);
            Assert.IsTrue(returnedProjects[1].LastName.Contains("Last3"));
        }

        [TestMethod]
        public void PersonService_LookupAsync_ReturnsNoPersons()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "FirstManager1", LastName = "LastManager1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "FirstManager2", LastName = "LastManager2" };

            var personGroupDbEntry1 = new PersonGroup { GroupManagers = new List<Person> { groupManager1 }, };
            var personGroupDbEntry2 = new PersonGroup { GroupManagers = new List<Person> { groupManager2 }, };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2",
                                        PersonGroups = new List<PersonGroup> {personGroupDbEntry1}};
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry1 }};
            var dbEntry4 = new Person { Id = "dummyEntryId4", FirstName = "First4", LastName = "Last4", IsActive_bl = true, Initials = "FLA4",
                                        PersonGroups = new List<PersonGroup> { personGroupDbEntry2 }};

            var personDbEntries = (new List<Person> { dbEntry1, dbEntry2, dbEntry3, dbEntry4 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, groupManager1.Id);

            //Act
            var returnedProjects = personService.LookupAsync("FLA1").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 0);
        }


    }
}
