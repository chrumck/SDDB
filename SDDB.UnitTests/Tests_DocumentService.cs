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
    public class Tests_DocumentService
    {
        [TestMethod]
        public void DocumentService_GetAsync_ReturnsDocuments()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, Comments = "DummyComments1",
                DocFilePath = "DummyPath1", AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true, Comments = "DummyComments2",
                DocFilePath = "DummyPath2", AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var resultDocuments = docService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultDocuments.Count == 1);
            Assert.IsTrue(resultDocuments[0].DocAltName.Contains("DocAlt2"));
        }

        [TestMethod]
        public void DocumentService_GetAsync_DoeNotReturnDocumentsFromWrongProject()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };
            var projectPerson2 = new Person { Id = "dummyUserId2", FirstName = "Firs2", LastName = "Last2" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, Comments = "DummyComments1",
                DocFilePath = "DummyPath1", AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true, Comments = "DummyComments2",
                DocFilePath = "DummyPath2", AssignedToProject = project2 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator())); 
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var resultDocuments = docService.GetAsync(projectPerson1.Id).Result;
            
            //Assert
            Assert.IsTrue(resultDocuments.Count == 0);
        }

        [TestMethod]
        public void DocumentService_GetAsync_ReturnsDocumentsByIds()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive = true, 
                DocFilePath = "DummyPath3" , AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.GetAsync(projectPerson1.Id, new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 1);
            Assert.IsTrue(serviceResult[0].DocAltName.Contains("DocAlt3"));

        }

        [TestMethod]
        public void DocumentService_GetAsync_DoesNotReturnDocumentsByIdsIfNotActive()
        {
            //Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive = false, 
                DocFilePath = "DummyPath3" , AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.GetAsync(projectPerson1.Id,new string[] { "dummyEntryId3" }).Result;

            //Assert
            Assert.IsTrue(serviceResult.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void DocumentService_LookupAsync_ReturnsAllRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive = false, 
                DocFilePath = "DummyPath3" , AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var returnedDocs = docService.LookupAsync(projectPerson1.Id,"", true).Result;

            //Assert
            Assert.IsTrue(returnedDocs.Count == 2);
            Assert.IsTrue(returnedDocs[1].DocAltName.Contains("DocAlt2"));
        }

        [TestMethod]
        public void DocumentService_LookupAsync_ReturnsMatchingRecords()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive = false, 
                DocFilePath = "DummyPath3" , AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var returnedDocs = docService.LookupAsync(projectPerson1.Id,"Doc1", true).Result;

            //Assert
            Assert.IsTrue(returnedDocs.Count == 1);
            Assert.IsTrue(returnedDocs[0].DocAltName.Contains("DocAlt1"));
        }

        [TestMethod]
        public void DocumentService_LookupAsync_ReturnsNoRecordsIfNotActive()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextReadOnlyScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.CreateReadOnly(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

                        var projectPerson1 = new Person { Id = "dummyUserId1", FirstName = "Firs1", LastName = "Last1" };

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive = false, 
                DocFilePath = "DummyPath3" , AssignedToProject = project1 };
            var dbEntries = (new List<Document> { dbEntry1, dbEntry2, dbEntry3 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<Document>>();
            mockDbSet.As<IDbAsyncEnumerable<Document>>().Setup(m => m.GetAsyncEnumerator()).Returns(new MockDbAsyncEnumerator<Document>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(new MockDbAsyncQueryProvider<Document>(dbEntries.Provider));
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            mockEfDbContext.Setup(x => x.Documents).Returns(mockDbSet.Object);

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var returnedDocs = docService.LookupAsync(projectPerson1.Id,"Doc3", true).Result;

            //Assert
            Assert.IsTrue(returnedDocs.Count == 0);
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [TestMethod]
        public void DocumentService_EditAsync_DoesNothingIfNoDocument()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docs = new Document[] { };

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.EditAsync(docs).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.FindAsync(It.IsAny<object[]>()), Times.Never);
        }


        [TestMethod]
        public void DocumentService_EditAsync_CreatesDocumentIfNotInDB()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var doc1 = new Document { Id = initialId, DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, DocFilePath = "DummyPath1" };
            var docs = new Document[] { doc1 };

            mockEfDbContext.Setup(x => x.Documents.FindAsync(doc1.Id)).Returns(Task.FromResult<Document>(null));
            mockEfDbContext.Setup(x => x.Documents.Add(doc1)).Returns(doc1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.EditAsync(docs).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(doc1.Id));
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.FindAsync(initialId), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.Add(doc1), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void DocumentService_EditAsync_UpdatesExistingEntries()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var doc1 = new Document { Id = "dummyDocumentId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, DocFilePath = "DummyPath1",
                                      ModifiedProperties = new string[] { "DocName", "DocAltName" }};
            var doc2 = new Document { Id = "dummyDocumentId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive = true, DocFilePath = "DummyPath2" };
            var docs = new Document[] { doc1, doc2 };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "DbEntry1", DocAltName = "DbEntryAlt1", IsActive = true, DocFilePath = "DbEntryPath1" };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "DbEntry2", DocAltName = "DbEntryAlt2", IsActive = false, DocFilePath = "DbEntryPath2" };

            mockEfDbContext.Setup(x => x.Documents.FindAsync(doc1.Id)).Returns(Task.FromResult(dbEntry1));
            mockEfDbContext.Setup(x => x.Documents.FindAsync(doc2.Id)).Returns(Task.FromResult(dbEntry2));
            mockEfDbContext.Setup(x => x.Documents.Add(doc1)).Verifiable();
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.EditAsync(docs).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(doc1.DocName == dbEntry1.DocName); Assert.IsTrue(doc1.DocAltName == dbEntry1.DocAltName);
            Assert.IsTrue(doc1.DocFilePath != dbEntry1.DocFilePath); Assert.IsTrue(doc1.IsActive != dbEntry1.IsActive);
            Assert.IsTrue(doc2.DocName != dbEntry2.DocName); Assert.IsTrue(doc2.DocAltName != dbEntry2.DocAltName);
            Assert.IsTrue(doc2.DocFilePath != dbEntry2.DocFilePath); Assert.IsTrue(doc2.IsActive != dbEntry2.IsActive);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.Documents.Add(It.IsAny<Document>()), Times.Never);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void DocumentService_EditAsync_ReturnsExceptionFromSaveChanges()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var initialId = "dummyEntryId1";
            var doc1 = new Document { Id = initialId, DocName = "Doc1", DocAltName = "DocAlt1", IsActive = false, DocFilePath = "DummyPath1" };
            var docs = new Document[] { doc1 };

            mockEfDbContext.Setup(x => x.Documents.FindAsync(It.IsAny<string>())).Returns(Task.FromResult<Document>(null));
            mockEfDbContext.Setup(x => x.Documents.Add(doc1)).Returns(doc1);
            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.EditAsync(docs).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(doc1.Id));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

        //-----------------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void DocumentService_DeleteAsync_DeletesAndReturnsErrorIfNotFound()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docIds = new string[] { "dummyId1", "DummyId2" };

            var dbEntry = new Document { IsActive = true };
            mockEfDbContext.Setup(x => x.Documents.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));
            mockEfDbContext.Setup(x => x.Documents.FindAsync("dummyId2")).Returns(Task.FromResult<Document>(null));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult<int>(1));

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.DeleteAsync(docIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Record with Id=DummyId2 not found\n"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.FindAsync(It.IsAny<string>()), Times.Exactly(2));
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void DocumentService_DeleteAsync_DeletesAndReturnsErrorifSaveException()
        {
            // Arrange
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();
            var mockDbContextScope = new Mock<IDbContextScope>();
            var mockEfDbContext = new Mock<EFDbContext>();
            mockDbContextScopeFac.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(mockDbContextScope.Object);
            mockDbContextScope.Setup(x => x.DbContexts.Get<EFDbContext>()).Returns(mockEfDbContext.Object);

            var docIds = new string[] { "dummyId1" };

            var dbEntry = new Document { IsActive = true };
            mockEfDbContext.Setup(x => x.Documents.FindAsync("dummyId1")).Returns(Task.FromResult(dbEntry));

            mockEfDbContext.Setup(x => x.SaveChangesAsync()).Throws(new ArgumentException("DummyMessage"));

            var docService = new DocumentService(mockDbContextScopeFac.Object);

            //Act
            var serviceResult = docService.DeleteAsync(docIds).Result;

            //Assert
            Assert.IsTrue(serviceResult.StatusCode == HttpStatusCode.Conflict);
            Assert.IsTrue(serviceResult.StatusDescription.Contains("Errors deleting records:\n"));
            Assert.IsTrue(serviceResult.StatusDescription.Contains("DummyMessage"));
            Assert.IsTrue(dbEntry.IsActive == false);
            mockDbContextScopeFac.Verify(x => x.Create(DbContextScopeOption.JoinExisting), Times.Once);
            mockDbContextScope.Verify(x => x.DbContexts.Get<EFDbContext>(), Times.Once);
            mockEfDbContext.Verify(x => x.Documents.FindAsync(It.IsAny<string>()), Times.Once);
            mockEfDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }



    }
}
