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
            var mockDbUserService = new Mock<DBUserService>(new object[] {mockAppUserManager.Object, mockDbContextScopeFac.Object, true});

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

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

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
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

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

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

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
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

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

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

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
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

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

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.GetWoDBUserAsync().Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].LastName.Contains("Last2"));
        }

        
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_EditAsync_ReturnsNothingifNoUser()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var persons = new Person[] { };

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditAsync(persons).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void PersonService_EditAsync_CreatesPersonIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var initialId = "dummyEntryId1";
            var person1 = new Person { Id = initialId, FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var persons = new Person[] { person1 };

            mockEfDbContext.Setup(x => x.Persons.FindAsync(person1.Id)).Returns(Task.FromResult<Person>(null));
            mockEfDbContext.Setup(x => x.Persons.Add(person1)).Returns(person1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditAsync(persons).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(person1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Persons.Add(person1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var person1 = new Person { Id = "dummyPersonId1", FirstName = "PersonFirst1", LastName = "PersonLast1", IsActive_bl = true, Initials = "PersonFLA1", 
                                        ModifiedProperties = new string[] {"FirstName", "LastName"} };
            var person2 = new Person { Id = "dummyPersonId2", FirstName = "PersonFirst2", LastName = "PersonLast2", IsActive_bl = false, Initials = "PersonFLA2" };
            
            var persons = new Person[] { person1, person2 };

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };

            mockEfDbContext.Setup(x => x.Persons.FindAsync(person1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Persons.FindAsync(person2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Persons.Add(person1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditAsync(persons).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(person1.FirstName == dbEntry1.FirstName); Assert.IsTrue(person1.LastName == dbEntry1.LastName);
            Assert.IsTrue(person1.Initials != dbEntry1.Initials); Assert.IsTrue(person1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(person2.FirstName != dbEntry2.FirstName); Assert.IsTrue(person2.LastName != dbEntry2.LastName);
            Assert.IsTrue(person2.Initials != dbEntry2.Initials); Assert.IsTrue(person2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once );
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Persons.Add(It.IsAny<Person>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var initialId = "dummyEntryId1";
            var debEntry1 = new Person { Id = initialId, FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var persons = new Person[] { debEntry1 };

            mockEfDbContext.Setup(x => x.Persons.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<Person>(null));
            mockEfDbContext.Setup(x => x.Persons.Add(debEntry1)).Returns(debEntry1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditAsync(persons).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(debEntry1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_DeleteAsync_CallsOtherServices()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var personMockDbSet = new Mock<DbSet<Person>>();
            personMockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(() => personDbEntries.GetEnumerator()));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => personDbEntries.GetEnumerator());
            personMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personMockDbSet.Object);
            personMockDbSet.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(personDbEntry1));

            var personGroupDbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var personGroupDbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true };
            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();
            var personGroupMockDbSet = new Mock<DbSet<PersonGroup>>();
            personGroupMockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());
            personGroupMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personGroupMockDbSet.Object);

            var projectDbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var projectDbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var projectDbEntries = (new List<Project> { projectDbEntry1, projectDbEntry2 }).AsQueryable();
            var projectMockDbSet = new Mock<DbSet<Project>>();
            projectMockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(projectDbEntries.GetEnumerator()));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(projectDbEntries.Provider));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectDbEntries.Expression);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectDbEntries.ElementType);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectDbEntries.GetEnumerator());
            projectMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(projectMockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(personMockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(personGroupMockDbSet.Object);
            mockEfDbContext.Setup(x => x.Projects).Returns(projectMockDbSet.Object);

            var ids = new string[] { "dummyEntryId1" };

            mockDbUserService.Setup(x => x.DeleteAsync(ids)).Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.AtLeastOnce);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.AtLeastOnce);
            mockEfDbContext.Verify(x => x.Persons.FindAsync(It.IsAny<string>()), Times.Never);
            mockDbUserService.Verify(x => x.DeleteAsync(It.IsAny<string[]>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void PersonService_DeleteAsync_CallsUserServiceReturnsError()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personGroupDbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var personGroupDbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true };
            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();
            var personGroupMockDbSet = new Mock<DbSet<PersonGroup>>();
            personGroupMockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());
            personGroupMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personGroupMockDbSet.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var personMockDbSet = new Mock<DbSet<Person>>();
            personMockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            personMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personMockDbSet.Object);
            
            var projectDbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var projectDbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var projectDbEntries = (new List<Project> { projectDbEntry1, projectDbEntry2 }).AsQueryable();
            var projectMockDbSet = new Mock<DbSet<Project>>();
            projectMockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(projectDbEntries.GetEnumerator()));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(projectDbEntries.Provider));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectDbEntries.Expression);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectDbEntries.ElementType);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectDbEntries.GetEnumerator());
            projectMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(projectMockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(personMockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(personGroupMockDbSet.Object);
            mockEfDbContext.Setup(x => x.Projects).Returns(projectMockDbSet.Object);
            
            var ids = new string[] { };

            mockDbUserService.Setup(x => x.DeleteAsync(ids)).Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.Conflict, StatusDescription = "DummyUserError" }));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyUserError"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.AtLeastOnce);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.AtLeastOnce);
            mockEfDbContext.Verify(x => x.Persons.FindAsync(It.IsAny<string>()), Times.Never);
            mockDbUserService.Verify(x => x.DeleteAsync(It.IsAny<string[]>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });
            
            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntry2 = new Person { Id = "dummyEntryId1", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var personDbEntries = (new List<Person> { personDbEntry1, personDbEntry2 }).AsQueryable();
            var personMockDbSet = new Mock<DbSet<Person>>();
            personMockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            personMockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            personMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personMockDbSet.Object);

            var personGroupDbEntry1 = new PersonGroup { Id = "dummyEntryId1", PrsGroupName = "PersonGroup1", PrsGroupAltName = "PersonGroupAlt1", IsActive_bl = false };
            var personGroupDbEntry2 = new PersonGroup { Id = "dummyEntryId2", PrsGroupName = "PersonGroup2", PrsGroupAltName = "PersonGroupAlt2", IsActive_bl = true };
            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();
            var personGroupMockDbSet = new Mock<DbSet<PersonGroup>>();
            personGroupMockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            personGroupMockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());
            personGroupMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(personGroupMockDbSet.Object);

            var projectDbEntry1 = new Project { Id = "dummyEntryId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = false, ProjectCode = "CODE1" };
            var projectDbEntry2 = new Project { Id = "dummyEntryId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = true, ProjectCode = "CODE2" };
            var projectDbEntries = (new List<Project> { projectDbEntry1, projectDbEntry2 }).AsQueryable();
            var projectMockDbSet = new Mock<DbSet<Project>>();
            projectMockDbSet.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(projectDbEntries.GetEnumerator()));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(projectDbEntries.Provider));
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectDbEntries.Expression);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectDbEntries.ElementType);
            projectMockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectDbEntries.GetEnumerator());
            projectMockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(projectMockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(personMockDbSet.Object);
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(personGroupMockDbSet.Object);
            mockEfDbContext.Setup(x => x.Projects).Returns(projectMockDbSet.Object);


            var ids = new string[] { "dummyId1", "DummyId2" };
            
            var dbEntry = new Person { IsActive_bl = true };
            personMockDbSet.Setup(x => x.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            personMockDbSet.Setup(x => x.FindAsync("dummyId2")).Returns(Task.FromResult<Person>(null));

            mockDbUserService.Setup(x => x.DeleteAsync(ids)).Returns(Task.FromResult(new DBResult { StatusCode = HttpStatusCode.OK}));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.DeleteAsync(ids).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.AtLeastOnce);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.AtLeastOnce);
            personMockDbSet.Verify(x => x.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesNothingIfNoPersons()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { };

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var projectIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesNothingIfPersonNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "dummyEntryId2" };

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var projectIds = new string[] {"notExistingId" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesNothingIfNoProjects()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);
            
            var projectIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesNothingIfProjectNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var personIds = new string[] { "EntryId1" };
            var projectIds = new string[] { "ProjectId2" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_AddsProjectToPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
                        
            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);
            

            var projectIds = new string[] {"ProjectId1"};

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesntAddProjectToPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", 
                PersonProjects = new List<Project> {dbEntry1} };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var projectIds = new string[] { "ProjectId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_RemovesProjectFromPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", PersonProjects = new List<Project> { dbEntry1 } };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var projectIds = new string[] { "ProjectId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_DoesntRemoveProjectFromPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            
            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var projectIds = new string[] { "ProjectId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPrsProjectsAsync_ReturnsExceptionMessageFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new Project { Id = "ProjectId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1" };
            var dbEntries = (new List<Project> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<Project>>();
            mockDbSet2.As<IDbAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Project>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Project>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.Projects).Returns(mockDbSet2.Object);

            var projectIds = new string[] { "" };
            var personIds = new string[] { "" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyExceptionMessage"));
            //Returns(Task.FromResult<int>(throw new ArgumentException("DummyExceptionMessage"); ));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonProjectsAsync(personIds, projectIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.Projects, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonProjects.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyExceptionMessage"));
        }
        
        
        //-----------------------------------------------------------------------------------------------------------------------
        
        [TestMethod]
        public void PersonService_LookupAsync_ReturnsAllPersons()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var personGroupDbEntry1 = new PersonGroup { IsActive_bl = true, GroupManagers = new List<Person> { groupManager1 },
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3} };
            var personGroupDbEntry2 = new PersonGroup { Id = "dummyId2", GroupManagers = new List<Person> { groupManager1 }, 
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3 } };

            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var returnedProjects = personService.LookupAsync(groupManager1.Id).Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 2);
            Assert.IsTrue(returnedProjects[1].LastName.Contains("Last3"));
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

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var personGroupDbEntry1 = new PersonGroup { IsActive_bl = true, GroupManagers = new List<Person> { groupManager1 },
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3} };
            var personGroupDbEntry2 = new PersonGroup { Id = "dummyId2", GroupManagers = new List<Person> { groupManager1 }, 
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3 } };

            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var returnedProjects = personService.LookupAsync(groupManager1.Id,"FLA2").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].LastName.Contains("Last2"));
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

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var personGroupDbEntry1 = new PersonGroup
            {
                IsActive_bl = true,
                GroupManagers = new List<Person> { groupManager1 },
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3 }
            };
            var personGroupDbEntry2 = new PersonGroup
            {
                Id = "dummyId2",
                GroupManagers = new List<Person> { groupManager1 },
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3 }
            };

            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var returnedProjects = personService.LookupAsync(groupManager1.Id, "Last3").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 1);
            Assert.IsTrue(returnedProjects[0].LastName.Contains("Last3"));
        }

        [TestMethod]
        public void PersonService_LookupAsync_ReturnsMatchingPersons3()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };
            var dbEntry4 = new Person { Id = "dummyEntryId4", FirstName = "First4", LastName = "Last4", IsActive_bl = false, Initials = "FLA4" };

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var personGroupDbEntry1 = new PersonGroup
            {
                IsActive_bl = true,
                GroupManagers = new List<Person> { groupManager1 },
                GroupPersons = new List<Person> { dbEntry1, dbEntry2, dbEntry3 }
            };
            var personGroupDbEntry2 = new PersonGroup
            {
                Id = "dummyId2",
                GroupManagers = new List<Person> { groupManager2 },
                GroupPersons = new List<Person> { dbEntry2, dbEntry3, dbEntry4 }
            };

            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var returnedProjects = personService.LookupAsync(groupManager2.Id, "FLA").Result;

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

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var dbEntry2 = new Person { Id = "dummyEntryId2", FirstName = "First2", LastName = "Last2", IsActive_bl = true, Initials = "FLA2" };
            var dbEntry3 = new Person { Id = "dummyEntryId3", FirstName = "First3", LastName = "Last3", IsActive_bl = true, Initials = "FLA3" };
            var dbEntry4 = new Person { Id = "dummyEntryId4", FirstName = "First4", LastName = "Last4", IsActive_bl = false, Initials = "FLA4" };

            var groupManager1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var groupManager2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var personGroupDbEntry1 = new PersonGroup {IsActive_bl = true, 
                GroupManagers = new List<Person> { groupManager1 },GroupPersons = new List<Person> { dbEntry1, dbEntry2,dbEntry3  }};
            var personGroupDbEntry2 = new PersonGroup {Id = "dummyId2", 
                GroupManagers = new List<Person> { groupManager2 }, GroupPersons = new List<Person> { dbEntry2, dbEntry3, dbEntry4 }};

            var personGroupDbEntries = (new List<PersonGroup> { personGroupDbEntry1, personGroupDbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonGroup>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(personGroupDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(personGroupDbEntries.Provider));
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(personGroupDbEntries.Expression);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(personGroupDbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(personGroupDbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet.Object);

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var returnedProjects = personService.LookupAsync(groupManager2.Id,"FLA1").Result;

            //Assert
            Assert.IsTrue(returnedProjects.Count == 0);
        }


        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesNothingIfNoPersons()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesNothingIfPersonNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "dummyEntryId2" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] {"notExistingId" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesNothingIfNoGroups()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));

        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesNothingIfGroupNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personIds = new string[] { "EntryId1" };
            var personGroupIds = new string[] { "DummyId2" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_AddsGroupToPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true};
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);


            var personGroupIds = new string[] { "DummyId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesntAddGroupToPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", PersonGroups = new List<PersonGroup> { dbEntry1 } };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_RemovesGroupFromPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", PersonGroups = new List<PersonGroup> { dbEntry1 } };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_DoesntRemoveGroupFromPerson()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditPersonGroupsAsync_ReturnsExceptionMessageFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { "" };
            var personIds = new string[] { "" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyExceptionMessage"));
            //Returns(Task.FromResult<int>(throw new ArgumentException("DummyExceptionMessage"); ));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditPersonGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.PersonGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyExceptionMessage"));
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesNothingIfNoPersons()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesNothingIfPersonNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "dummyEntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "dummyEntryId2" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] {"notExistingId"};

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesNothingIfNoGroups()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Never);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Never);
            mockEfDbContext.Verify(x => x.Persons, Times.Never);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.BadRequest);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("arguments missing"));

        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesNothingIfGroupNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personIds = new string[] { "EntryId1" };
            var personGroupIds = new string[] { "DummyId2" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_AddsGroupToManaged()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personIds = new string[] { "EntryId1" };

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);


            var personGroupIds = new string[] { "DummyId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesntAddGroupToManaged()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", PersonGroups = new List<PersonGroup> { dbEntry1 } };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, true).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_RemovesGroupFromManaged()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1", PersonGroups = new List<PersonGroup> { dbEntry1 } };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_DoesntRemoveGroupFromManaged()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { "DummyId1" };
            var personIds = new string[] { "EntryId1" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void PersonService_EditManagedGroupsAsync_ReturnsExceptionMessageFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            var mockDbUserService = new Mock<DBUserService>(new object[] { mockAppUserManager.Object, mockDbContextScopeFac.Object, true });

            var personDbEntry1 = new Person { Id = "EntryId1", FirstName = "First1", LastName = "Last1", IsActive_bl = false, Initials = "FLA1" };
            var personDbEntries = (new List<Person> { personDbEntry1 }).AsQueryable();
            var mockDbSet = new Mock<DbSet<Person>>();
            mockDbSet.As<IDbAsyncEnumerable<Person>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Person>(personDbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Person>(personDbEntries.Provider));
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personDbEntries.Expression);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personDbEntries.ElementType);
            mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personDbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Persons).Returns(mockDbSet.Object);

            var dbEntry1 = new PersonGroup { Id = "DummyId1", PrsGroupName = "Dummy1", PrsGroupAltName = "DummyAlt1", IsActive_bl = true };
            var dbEntries = (new List<PersonGroup> { dbEntry1 }).AsQueryable();
            var mockDbSet2 = new Mock<DbSet<PersonGroup>>();
            mockDbSet2.As<IDbAsyncEnumerable<PersonGroup>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonGroup>(dbEntries.GetEnumerator()));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonGroup>(dbEntries.Provider));
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet2.As<IQueryable<PersonGroup>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());

            mockEfDbContext.Setup(x => x.PersonGroups).Returns(mockDbSet2.Object);

            var personGroupIds = new string[] { "" };
            var personIds = new string[] { "" };

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyExceptionMessage"));
            //Returns(Task.FromResult<int>(throw new ArgumentException("DummyExceptionMessage"); ));

            var personService = new PersonService(mockDbContextScopeFac.Object, mockDbUserService.Object);

            //Act
            var serviceResult = personService.EditManagedGroupsAsync(personIds, personGroupIds, false).Result;

            //Assert
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Persons, Times.Once);
            mockEfDbContext.Verify(x => x.PersonGroups, Times.Once);
            Assert.IsTrue(!personDbEntry1.ManagedGroups.Contains(dbEntry1));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyExceptionMessage"));
        }

    }
}
