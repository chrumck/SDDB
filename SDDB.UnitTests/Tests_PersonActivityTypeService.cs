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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

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

            var prsActivityTypeService = new PersonActivityTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedActivityTypes = prsActivityTypeService.LookupAsync("Name2", true).Result;

            //Assert
            Assert.IsTrue(returnedActivityTypes.Count == 0);
        }

    }
}
