using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Mehdime.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Threading.Tasks;
using Moq;

using SDDB.Domain.Services;
using SDDB.Domain.Entities;
using SDDB.Domain.Abstract;
using SDDB.Domain.DbContexts;
using System.Data.Entity.Infrastructure;

namespace SDDB.UnitTests
{
    [TestClass]
    public class Tests_DBUserService
    {

        [TestMethod]
        public void UserService_EditAsync_CreatesUserIfNotInDbWithGUID()
        {
            // Arrange
            var dbUser = new DBUser { Id = "dummyId", UserName = "UserName2",  Email = "Email2",  LDAPAuthenticated_bl = true, Password = "NewPassword" };
            
            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult<DBUser>(null));
            mockAppUserManager.Setup(x => x.CreateAsync(dbUser, It.IsNotIn(new[] { dbUser.Password }))).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>())).Verifiable();

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(dbUser.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Once);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
            Assert.IsTrue(dbUser.Password != "NewPassword");
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*"); Assert.IsTrue(regex.IsMatch(dbUser.Password));

        }

        [TestMethod]
        public void UserService_EditAsync_CreatesUserIfNotInDbWithPassword()
        {
            // Arrange
            var dbUser = new DBUser { Id = "dummyId", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = false, Password = "NewPassword" };
            
            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult<DBUser>(null));
            mockAppUserManager.Setup(x => x.CreateAsync(dbUser, It.IsIn(new[] { dbUser.Password }))).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>())).Verifiable();

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Once);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
            Assert.IsTrue(dbUser.Password == "NewPassword");
            
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserwithGUID()
        {
            // Arrange
            var dbEntry = new DBUser { Id = "dummyId1", UserName = "UserName", Email = "Email", LDAPAuthenticated_bl = false };
            var dbUser = new DBUser { Id = "dummyId2", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = true, 
                Password = "NewPassword", ModifiedProperties = new[] {"Password"} };
            
            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult(dbEntry));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            Assert.IsTrue(dbEntry.Id != dbUser.Id);
            Assert.IsTrue(dbEntry.UserName != dbUser.UserName);
            Assert.IsTrue(dbEntry.Email != dbUser.Email);
            Assert.IsTrue(dbEntry.LDAPAuthenticated_bl != dbUser.LDAPAuthenticated_bl);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*out\b"); Assert.IsTrue(regex.IsMatch(dbEntry.PasswordHash));
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserwithPassword()
        {
            // Arrange
            var dbEntry = new DBUser { Id = "dummyId1", UserName = "UserName", Email = "Email", LDAPAuthenticated_bl = true };
            var dbUser = new DBUser { Id = "dummyId2", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = false,
                Password = "NewPassword", ModifiedProperties = new[] {"Password", "UserName"} };
            
            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult(dbEntry));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            Assert.IsTrue(dbEntry.Id != dbUser.Id);
            Assert.IsTrue(dbEntry.UserName == dbUser.UserName);
            Assert.IsTrue(dbEntry.Email != dbUser.Email);
            Assert.IsTrue(dbEntry.LDAPAuthenticated_bl != dbUser.LDAPAuthenticated_bl);
            Assert.IsTrue(dbEntry.PasswordHash == dbUser.Password + "out");
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserLeavePasswordIfPropNotModified()
        {
            // Arrange
            var dbEntry = new DBUser { Id = "dummyId1", UserName = "UserName", Email = "Email", LDAPAuthenticated_bl = true };
            var dbUser = new DBUser { Id = "dummyId2", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = false, Password = "D$mmyPasww0rd",
                ModifiedProperties = new[] {"Password"} };
            
            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult(dbEntry));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            Assert.IsTrue(dbEntry.Id != dbUser.Id);
            Assert.IsTrue(dbEntry.UserName != dbUser.UserName);
            Assert.IsTrue(dbEntry.Email != dbUser.Email);
            Assert.IsTrue(dbEntry.LDAPAuthenticated_bl != dbUser.LDAPAuthenticated_bl);
            Assert.IsTrue(dbEntry.PasswordHash == dbUser.Password + "out");
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(dbEntry), Times.Once);
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateManyUsersDoUpdateModified()
        {
            // Arrange
            var dbEntry1 = new DBUser { Id = "dummyEntryId1", UserName = "EntryName1", Email = "EntryEmail1", LDAPAuthenticated_bl = true };
            var dbEntry2 = new DBUser { Id = "dummyEntryId2", UserName = "EntryName2", Email = "EntryEmail2", LDAPAuthenticated_bl = false };
            var dbUser1 = new DBUser { Id = "dummyUserId1", UserName = "UserName1", Email = "UserEmail1", LDAPAuthenticated_bl = false, Password = "NewPassword1", 
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl", } };
            var dbUser2 = new DBUser { Id = "dummyUserId2", UserName = "UserName2", Email = "UserEmail2", LDAPAuthenticated_bl = true, Password = "NewPassword2" };

            var dbUsers = new DBUser[] { dbUser1, dbUser2 };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult(dbEntry1));
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser2.Id)).Returns(Task.FromResult(dbEntry2));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry1)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry2)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);
            
            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            Assert.IsTrue(dbEntry1.Id != dbUser1.Id);
            Assert.IsTrue(dbEntry1.UserName == dbUser1.UserName);
            Assert.IsTrue(dbEntry1.Email == dbUser1.Email);
            Assert.IsTrue(dbEntry1.LDAPAuthenticated_bl == dbUser1.LDAPAuthenticated_bl);
            Assert.IsTrue(dbEntry1.PasswordHash == null);
            Assert.IsTrue(dbEntry2.Id != dbUser2.Id);
            Assert.IsTrue(dbEntry2.UserName != dbUser2.UserName);
            Assert.IsTrue(dbEntry2.Email != dbUser2.Email);
            Assert.IsTrue(dbEntry2.LDAPAuthenticated_bl != dbUser2.LDAPAuthenticated_bl);
            Assert.IsTrue(dbEntry2.PasswordHash == null);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error editing user UserName2:Error1; Error2;")]
        public void UserService_EditAsync_UpdateManyUsersReportErrorsFromIdentityResult()
        {
            // Arrange
            var dbEntry1 = new DBUser { Id = "dummyEntryId1", UserName = "EntryName1", Email = "EntryEmail1", LDAPAuthenticated_bl = true };
            var dbEntry2 = new DBUser { Id = "dummyEntryId2", UserName = "EntryName2", Email = "EntryEmail2", LDAPAuthenticated_bl = false };
            var dbUser1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "UserEmail1",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword1",
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl" }
            };
            var dbUser2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "UserEmail2",
                LDAPAuthenticated_bl = true,
                Password = "NewPassword2",
                ModifiedProperties = new[] { "Password" }
            };

            var dbUsers = new DBUser[] { dbUser1, dbUser2 };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult(dbEntry1));
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser2.Id)).Returns(Task.FromResult(dbEntry2));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry1)).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry2)).Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error1", "Error2" })));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error editing user UserName2:Error1; Error2;")]
        public void UserService_EditAsync_UpdateSingleUserReportErrorsFromIdentityResult()
        {
            // Arrange
            var dbEntry1 = new DBUser { Id = "dummyEntryId1", UserName = "EntryName1", Email = "EntryEmail1", LDAPAuthenticated_bl = true };
            var dbUser1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "UserEmail1",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword1",
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl", "Password" }
            };

            var dbUsers = new DBUser[] { dbUser1 };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult(dbEntry1));
            mockAppUserManager.Setup(x => x.UpdateAsync(dbEntry1)).Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error1", "Error2" })));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error creating user UserName1:no password")]
        public void UserService_EditAsync_ReturnsErrorifNewUserwithNoPasswordAndLDAPFalse()
        {
            // Arrange
            var dbUser1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "UserEmail1",
                LDAPAuthenticated_bl = false,
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl" }
            };

            var dbUsers = new DBUser[] { dbUser1 };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult((DBUser)null));
            mockAppUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>())).Verifiable();
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("no password")));
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Password is required if not LDAP authenticated")]
        public void UserService_EditAsync_DoesNotUpdateUserifPasswordChangedAndNoPassword()
        {
            // Arrange
            var dbEntry = new DBUser { Id = "dummyEntryId1", UserName = "UserName", Email = "Email", LDAPAuthenticated_bl = true };
            var dbUser = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = false,
                Password = "",
                ModifiedProperties = new[] { "Password" },
            };

            var dbUsers = new DBUser[] { dbUser };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser.Id)).Returns(Task.FromResult(dbEntry));
            mockAppUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>())).Verifiable();
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>())).Verifiable();

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.EditAsync(dbUsers).Wait();

            //Assert
            Assert.IsTrue(dbEntry.Id != dbUser.Id);
            Assert.IsTrue(dbEntry.UserName != dbUser.UserName);
            Assert.IsTrue(dbEntry.Email != dbUser.Email);
            Assert.IsTrue(dbEntry.LDAPAuthenticated_bl != dbUser.LDAPAuthenticated_bl);
            Assert.IsTrue(dbEntry.PasswordHash == null);
            mockAppUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never());
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never());
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void UserService_GetAsync_ReturnsResultWithUsers()
        {
            // Arrange
            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1",
                LDAPAuthenticated_bl = false
            };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = true
            };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            var dbUserList = dbUserService.GetAsync().Result;

            //Assert
            Assert.IsTrue(dbUserList.Count() == dbEntries.Count());
            Assert.IsTrue(dbUserList[0].Email == (dbEntries.ToList())[0].Email);
            mockAppUserManager.Verify(x => x.Users, Times.Once);
        }
        
        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error Deleting User UserName2: Error deleting dbentry;")]
        public void UserService_DeleteAsync_ReturnsDeleteError()
        {
            // Arrange
            var ids = new string[] { "dummyEntryId1", "dummyEntryId11" };

            var dbEntry = new DBUser { Id = "dummyEntryId1", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = true, Password = "NewPassword" };


            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync("dummyEntryId11")).Returns(Task.FromResult<DBUser>(null));
            mockAppUserManager.Setup(x => x.FindByIdAsync("dummyEntryId1")).Returns(Task.FromResult<DBUser>(dbEntry));
            mockAppUserManager.Setup(x => x.DeleteAsync(dbEntry)).Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error deleting dbentry" })));

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.DeleteAsync(ids).Wait();

            //Assert
            
        }

        [TestMethod]
        public void UserService_DeleteAsync_DeletesAndReturnsOK()
        {
            // Arrange
            var ids = new string[] { "dummyEntryId1" };

            var dbEntry = new DBUser { Id = "dummyEntryId1", UserName = "UserName2", Email = "Email2", LDAPAuthenticated_bl = true,
                Password = "NewPassword" };

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync("dummyEntryId1")).Returns(Task.FromResult<DBUser>(dbEntry));
            mockAppUserManager.Setup(x => x.DeleteAsync(dbEntry)).Returns(Task.FromResult(IdentityResult.Success));

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.DeleteAsync(ids).Wait();

            //Assert
            mockAppUserManager.Verify(m => m.DeleteAsync(It.IsIn(new DBUser[] { dbEntry })), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error adding/removing roles. User(s) not found.")]
        public void UserService_EditRolesAsync_ReturnErrorIfUserNotFound()
        {
            // Arrange
            var userIds = new string[] { "dummyUserId1", "dummyUserId2", "dummyUserId3" };
            var dbRoles = new string[] { "dummyRole1", "dummyRole2" };

            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1",
                LDAPAuthenticated_bl = false
            };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = true
            };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.AddRemoveRolesAsync(userIds, dbRoles, true).Wait();

            //Assert
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Add_skipsRolesAlreadyHad()
        {
            // Arrange
            var userIds = new string[] { "dummyUserId1", "dummyUserId2" };
            var dbRoles = new string[] { "dummyRole1", "dummyRole2" };

            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1",
                LDAPAuthenticated_bl = false
            };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = true
            };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockRolesList = new Mock<IList<string>>();
            mockRolesList.Setup(x => x.Contains(dbRoles[0])).Returns(true);
            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new DBUser { }));
            mockAppUserManager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).Returns(Task.FromResult(mockRolesList.Object));
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.AddRemoveRolesAsync(userIds, dbRoles, true).Wait();


            //Assert
            mockAppUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Add_AddsRoles()
        {
            // Arrange
            var userIds = new string[] { "dummyUserId1", "dummyUserId2" };
            var dbRoles = new string[] { "dummyRole1", "dummyRole2" };

            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1", 
                LDAPAuthenticated_bl = false };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2", 
                LDAPAuthenticated_bl = true };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2}).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockRolesList = new Mock<IList<string>>();
            mockRolesList.Setup(x => x.Contains(dbRoles[0])).Returns(false);
            mockRolesList.Setup(x => x.Contains(dbRoles[1])).Returns(true);
            
            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new DBUser { }));
            mockAppUserManager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).Returns(Task.FromResult(mockRolesList.Object));
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.AddRemoveRolesAsync(userIds, dbRoles, true).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "Error adding role dummyRole1 to user UserName1: Error1;")]
        public void UserService_EditRolesAsync_Add_AddsRolesReturnsIdentityErr()
        {
            // Arrange
            var userIds = new string[] { "dummyUserId1", "dummyUserId2" };
            var dbRoles = new string[] { "dummyRole1", "dummyRole2" };

            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1",
                LDAPAuthenticated_bl = false
            };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = true
            };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockRolesList = new Mock<IList<string>>();
            mockRolesList.Setup(x => x.Contains(dbRoles[0])).Returns(false);
            mockRolesList.Setup(x => x.Contains(dbRoles[1])).Returns(true);
            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new DBUser { }));
            mockAppUserManager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).Returns(Task.FromResult(mockRolesList.Object));
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed("Error1")));
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);

            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.AddRemoveRolesAsync(userIds, dbRoles, true).Wait();

            //Assert
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Remove_RemovesRoles()
        {
            // Arrange
            var userIds = new string[] { "dummyUserId1", "dummyUserId2" };
            var dbRoles = new string[] { "dummyRole1", "dummyRole2" };

            var dbEntry1 = new DBUser
            {
                Id = "dummyUserId1",
                UserName = "UserName1",
                Email = "Email1",
                LDAPAuthenticated_bl = false
            };
            var dbEntry2 = new DBUser
            {
                Id = "dummyUserId2",
                UserName = "UserName2",
                Email = "Email2",
                LDAPAuthenticated_bl = true
            };
            var dbEntries = (new List<DBUser> { dbEntry1, dbEntry2 }).AsQueryable();

            var mockDbSet = new Mock<DbSet<DBUser>>();
            mockDbSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<PersonLogEntry>(dbEntries.Provider));
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            var mockRolesList = new Mock<IList<string>>();
            mockRolesList.Setup(x => x.Contains(dbRoles[0])).Returns(true);
            mockRolesList.Setup(x => x.Contains(dbRoles[1])).Returns(true);
            var mockAppUserManager = new Mock<IAppUserManager>();
            mockAppUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new DBUser { }));
            mockAppUserManager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).Returns(Task.FromResult(mockRolesList.Object));
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);
            
            var mockDbContextScopeFac = new Mock<IDbContextScopeFactory>();

            var dbUserService = new DBUserService(mockDbContextScopeFac.Object, mockAppUserManager.Object, true);

            //Act   
            dbUserService.AddRemoveRolesAsync(userIds, dbRoles, false).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
        }
    }

}
