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
    public class Tests_DocumentTypeService
    {
        [TestMethod]
        public void DocumentTypeService_GetAsync_ReturnsDocumentTypes()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = false, Comments = "DummyComments1" };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = true, Comments = "DummyComments2" };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var resultDocumentTypes = docTypeService.GetAsync(new[] {dbEntry1.Id, dbEntry2.Id}).Result;
            
            //Assert
            Assert.IsTrue(resultDocumentTypes.Count == 1);
            Assert.IsTrue(resultDocumentTypes[0].DocTypeAltName.Contains("NameAlt2"));
        }

        [TestMethod]
        public void DocumentTypeService_GetAsync_ReturnsDocumentTypesByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new DocumentType { Id = "dummyEntryId3", DocTypeName = "Name3", DocTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var serviceResult = docTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].DocTypeAltName.Contains("NameAlt3"));

        }

        [TestMethod]
        public void DocumentTypeService_GetAsync_DoesNotReturnDocumentTypesByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = false };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = true };
            var dbEntry3 = new DocumentType { Id = "dummyEntryId3", DocTypeName = "Name3", DocTypeAltName = "NameAlt3", IsActive_bl = false };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var serviceResult = docTypeService.GetAsync(new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void DocumentTypeService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new DocumentType { Id = "dummyEntryId3", DocTypeName = "Name3", DocTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedDocTypes = docTypeService.LookupAsync("", true).Result;

            //Assert
            Assert.IsTrue(returnedDocTypes.Count == 2);
            Assert.IsTrue(returnedDocTypes[1].DocTypeAltName.Contains("NameAlt3"));
        }

        [TestMethod]
        public void DocumentTypeService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new DocumentType { Id = "dummyEntryId3", DocTypeName = "Name3", DocTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedDocTypes = docTypeService.LookupAsync("Name1", true).Result;

            //Assert
            Assert.IsTrue(returnedDocTypes.Count == 1);
            Assert.IsTrue(returnedDocTypes[0].DocTypeAltName.Contains("NameAlt1"));
        }

        [TestMethod]
        public void DocumentTypeService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = true };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = false };
            var dbEntry3 = new DocumentType { Id = "dummyEntryId3", DocTypeName = "Name3", DocTypeAltName = "NameAlt3", IsActive_bl = true };
            var dbEntries = (new List<DocumentType> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DocumentType>>();
            mockDbSet.As<IDbAsyncEnumerable<DocumentType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<DocumentType>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<DocumentType>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DocumentType>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.DocumentTypes).Returns(mockDbSet.Object);

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object, "dummyUserId");

            //Act
            var returnedDocTypes = docTypeService.LookupAsync("Name2", true).Result;

            //Assert
            Assert.IsTrue(returnedDocTypes.Count == 0);
        }

              

       


    }
}
