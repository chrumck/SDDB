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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };
            var project2 = new Project { Id = "dummyId2", ProjectName = "Project2", ProjectAltName = "ProjectAlt2", IsActive_bl = false, ProjectCode = "CODE2", 
                ProjectPersons = new List<Person> { projectPerson2 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = false, Comments = "DummyComments1",
                DocFilePath = "DummyPath1", AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true, Comments = "DummyComments2",
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var resultDocuments = docService.GetAsync(new[] { dbEntry1.Id, dbEntry2.Id }).Result;
            
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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = false, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive_bl = true, 
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = docService.GetAsync(new[] { dbEntry3.Id }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = false, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive_bl = false, 
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var serviceResult = docService.GetAsync(new[] { dbEntry3.Id }).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive_bl = false, 
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedDocs = docService.LookupAsync("", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive_bl = false, 
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedDocs = docService.LookupAsync("Doc1", true).Result;

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

            var project1 = new Project { Id = "dummyId1", ProjectName = "Project1", ProjectAltName = "ProjectAlt1", IsActive_bl = true, ProjectCode = "CODE1", 
                ProjectPersons = new List<Person> { projectPerson1 } };

            var dbEntry1 = new Document { Id = "dummyEntryId1", DocName = "Doc1", DocAltName = "DocAlt1", IsActive_bl = true, 
                DocFilePath = "DummyPath1" , AssignedToProject = project1 };
            var dbEntry2 = new Document { Id = "dummyEntryId2", DocName = "Doc2", DocAltName = "DocAlt2", IsActive_bl = true,
                DocFilePath = "DummyPath2" , AssignedToProject = project1 };
            var dbEntry3 = new Document { Id = "dummyEntryId3", DocName = "Doc3", DocAltName = "DocAlt3", IsActive_bl = false, 
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

            var docService = new DocumentService(mockDbContextScopeFac.Object, projectPerson1.Id);

            //Act
            var returnedDocs = docService.LookupAsync("Doc3", true).Result;

            //Assert
            Assert.IsTrue(returnedDocs.Count == 0);
        }

        
        

    }
}
