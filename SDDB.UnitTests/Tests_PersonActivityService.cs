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
    public class Tests_PersonActivityTypeService
    {
        [TestMethod]
        public void PersonActivityTypeService_GetAsync_ReturnsPersonActivityTypes()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var resultPersonActivityTypes = prsActivityTypeService.GetAsync().Result;
            
            //Assert
            Assert.IsTrue(resultPersonActivityTypes.Count == 1);
            Assert.IsTrue(resultPersonActivityTypes[0].ActivityTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void PersonActivityTypeService_GetAsync_ReturnsPersonActivityTypesByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new PersonActivityType { Id = "dummyEntryId3", ActivityTypeName = "Name3", ActivityTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].ActivityTypeAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void PersonActivityTypeService_GetAsync_DoesNotReturnPersonActivityTypesByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new PersonActivityType { Id = "dummyEntryId3", ActivityTypeName = "Name3", ActivityTypeAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonActivityTypeService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new PersonActivityType { Id = "dummyEntryId3", ActivityTypeName = "Name3", ActivityTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedActivityTypes = prsActivityTypeService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedActivityTypes.Count == 2);
            Assert.IsTrue(returnedActivityTypes[1].ActivityTypeAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void PersonActivityTypeService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new PersonActivityType { Id = "dummyEntryId3", ActivityTypeName = "Name3", ActivityTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedActivityTypes = prsActivityTypeService.LookupAsync("Name1", true).Result;

            //Assert
            Assert.IsTrue(returnedActivityTypes.Count == 1);
            Assert.IsTrue(returnedActivityTypes[0].ActivityTypeAltName.Contains("NameAlt1"));
        }

        [TestMethod]
        public void PersonActivityTypeService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new PersonActivityType { Id = "dummyEntryId3", ActivityTypeName = "Name3", ActivityTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<PersonActivityType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonActivityType>>();
            mockDbSet.As<IDbAsyncEnumerable<PersonActivityType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<PersonActivityType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<PersonActivityType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<PersonActivityType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.PersonActivityTypes).Returns(mockDbSet.Object);

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedActivityTypes = prsActivityTypeService.LookupAsync("Name2", true).Result;

            //Assert
            Assert.IsTrue(returnedActivityTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonActivityTypeService_EditAsync_DoesNothingIfNoPersonActivityType()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsActivityTypes = new PersonActivityType[] { };

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.EditAsync(prsActivityTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void PersonActivityTypeService_EditAsync_CreatesPersonActivityTypeIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var prsActivityType1 = new PersonActivityType { Id = initialId, ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false };
            var prsActivityTypes = new PersonActivityType[] { prsActivityType1 };

            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync(prsActivityType1.Id)).Returns(Task.FromResult<PersonActivityType>(null));
            mockEfDbContext.Setup(x => x.PersonActivityTypes.Add(prsActivityType1)).Returns(prsActivityType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.EditAsync(prsActivityTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(prsActivityType1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.Add(prsActivityType1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonActivityTypeService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsActivityType1 = new PersonActivityType { Id = "dummyPersonActivityTypeId1", ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "ActivityTypeName", "ActivityTypeAltName" }};
            var prsActivityType2 = new PersonActivityType { Id = "dummyPersonActivityTypeId2", ActivityTypeName = "Name2", ActivityTypeAltName = "NameAlt2", IsActive_bl = true };
            var prsActivityTypes = new PersonActivityType[] { prsActivityType1, prsActivityType2 };

            var dbEntry1 = new PersonActivityType { Id = "dummyEntryId1", ActivityTypeName = "DbEntry1", ActivityTypeAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new PersonActivityType { Id = "dummyEntryId2", ActivityTypeName = "DbEntry2", ActivityTypeAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync(prsActivityType1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync(prsActivityType2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.PersonActivityTypes.Add(prsActivityType1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.EditAsync(prsActivityTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(prsActivityType1.ActivityTypeName == dbEntry1.ActivityTypeName); Assert.IsTrue(prsActivityType1.ActivityTypeAltName == dbEntry1.ActivityTypeAltName);
            Assert.IsTrue(prsActivityType1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(prsActivityType2.ActivityTypeName != dbEntry2.ActivityTypeName); Assert.IsTrue(prsActivityType2.ActivityTypeAltName != dbEntry2.ActivityTypeAltName);
            Assert.IsTrue(prsActivityType2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.PersonActivityTypes.Add(It.IsAny<PersonActivityType>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonActivityTypeService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var prsActivityType1 = new PersonActivityType { Id = initialId, ActivityTypeName = "Name1", ActivityTypeAltName = "NameAlt1", IsActive_bl = false };
            var prsActivityTypes = new PersonActivityType[] { prsActivityType1 };

            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<PersonActivityType>(null));
            mockEfDbContext.Setup(x => x.PersonActivityTypes.Add(prsActivityType1)).Returns(prsActivityType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.EditAsync(prsActivityTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(prsActivityType1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void PersonActivityTypeService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsActivityTypeIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new PersonActivityType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync("dummyId2")).Returns(Task.FromResult<PersonActivityType>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.DeleteAsync(prsActivityTypeIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void PersonActivityTypeService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var prsActivityTypeIds = new string[] { "dummyId1" };

            var dbEntry = new PersonActivityType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.PersonActivityTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = prsActivityTypeService.DeleteAsync(prsActivityTypeIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.PersonActivityTypes.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
