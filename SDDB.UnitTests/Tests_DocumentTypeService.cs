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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var resultDocumentTypes = docTypeService.GetAsync().Result;
            
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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

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

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var returnedDocTypes = docTypeService.LookupAsync("Name2", true).Result;

            //Assert
            Assert.IsTrue(returnedDocTypes.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void DocumentTypeService_EditAsync_DoesNothingIfNoDocumentType()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docTypes = new DocumentType[] { };

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docTypeService.EditAsync(docTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.DocumentTypes.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void DocumentTypeService_EditAsync_CreatesDocumentTypeIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var docType1 = new DocumentType { Id = initialId, DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = false };
            var docTypes = new DocumentType[] { docType1 };

            mockEfDbContext.Setup(x => x.DocumentTypes.FindAsync(docType1.Id)).Returns(Task.FromResult<DocumentType>(null));
            mockEfDbContext.Setup(x => x.DocumentTypes.Add(docType1)).Returns(docType1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docTypeService.EditAsync(docTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(docType1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.DocumentTypes.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.DocumentTypes.Add(docType1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void DocumentTypeService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docType1 = new DocumentType { Id = "dummyDocumentTypeId1", DocTypeName = "Name1", DocTypeAltName = "NameAlt1", IsActive_bl = false,
                                      ModifiedProperties = new string[] { "DocTypeName", "DocTypeAltName" }};
            var docType2 = new DocumentType { Id = "dummyDocumentTypeId2", DocTypeName = "Name2", DocTypeAltName = "NameAlt2", IsActive_bl = true };
            var docTypes = new DocumentType[] { docType1, docType2 };

            var dbEntry1 = new DocumentType { Id = "dummyEntryId1", DocTypeName = "DbEntry1", DocTypeAltName = "DbEntryAlt1", IsActive_bl = true };
            var dbEntry2 = new DocumentType { Id = "dummyEntryId2", DocTypeName = "DbEntry2", DocTypeAltName = "DbEntryAlt2", IsActive_bl = false };

            mockEfDbContext.Setup(x => x.DocumentTypes.FindAsync(docType1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.DocumentTypes.FindAsync(docType2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.DocumentTypes.Add(docType1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docTypeService.EditAsync(docTypes).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(docType1.DocTypeName == dbEntry1.DocTypeName); Assert.IsTrue(docType1.DocTypeAltName == dbEntry1.DocTypeAltName);
            Assert.IsTrue(docType1.IsActive_bl != dbEntry1.IsActive_bl);
            Assert.IsTrue(docType2.DocTypeName != dbEntry2.DocTypeName); Assert.IsTrue(docType2.DocTypeAltName != dbEntry2.DocTypeAltName);
            Assert.IsTrue(docType2.IsActive_bl != dbEntry2.IsActive_bl);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.DocumentTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.DocumentTypes.Add(It.IsAny<DocumentType>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
                
        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void DocumentTypeService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docTypeIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new DocumentType { IsActive_bl = true };
            mockEfDbContext.Setup(x => x.DocumentTypes.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.DocumentTypes.FindAsync("dummyId2")).Returns(Task.FromResult<DocumentType>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docTypeService = new DocumentTypeService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docTypeService.DeleteAsync(docTypeIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive_bl == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.DocumentTypes.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

       


    }
}
