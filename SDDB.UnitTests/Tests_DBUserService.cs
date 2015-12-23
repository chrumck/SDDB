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
        Mock<IAppRoleManager> mockAppRoleManager = new Mock<IAppRoleManager>();
        Mock<IAppUserManager> mockAppUserManager = new Mock<IAppUserManager>();
        Mock<DbSet<DBUser>> mockDbUserSet = new Mock<DbSet<DBUser>>();
             
        DBUserService dbUserService;

        DBUser dbUser1 = new DBUser {
            Id = "dummyId1",
            UserName = "UserName1",
            Email = "Email1",
            LDAPAuthenticated_bl = true,
            Password = "NewPassword1"
        };

        DBUser dbUser2 = new DBUser
        {
            Id = "dummyId2",
            UserName = "UserName2",
            Email = "Email2",
            LDAPAuthenticated_bl = false,
            Password = "NewPassword2"
        };


        public Tests_DBUserService()
        {
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult(dbUser1));
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser2.Id)).Returns(Task.FromResult(dbUser2));
            mockAppUserManager.Setup(x => x.FindByIdAsync(It.IsNotIn(new[] {dbUser1.Id, dbUser2.Id}))).Returns(Task.FromResult<DBUser>(null));
            mockAppUserManager.Setup(x => x.CreateAsync(It.IsAny<DBUser>(),It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.UpdateAsync(It.IsAny<DBUser>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string s) => s + "out");
            mockAppUserManager.Setup(x => x.DeleteAsync(It.IsAny<DBUser>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            mockAppUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

            var dbEntries = (new List<DBUser> { dbUser1, dbUser2 }).AsQueryable();
            mockDbUserSet.As<IDbAsyncEnumerable<DBUser>>().Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<DBUser>(dbEntries.GetEnumerator()));
            mockDbUserSet.As<IQueryable<DBUser>>().Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<DBUser>(dbEntries.Provider));
            mockDbUserSet.As<IQueryable<DBUser>>().Setup(m => m.Expression).Returns(dbEntries.Expression);
            mockDbUserSet.As<IQueryable<DBUser>>().Setup(m => m.ElementType).Returns(dbEntries.ElementType);
            mockDbUserSet.As<IQueryable<DBUser>>().Setup(m => m.GetEnumerator()).Returns(dbEntries.GetEnumerator());
            mockDbUserSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockDbUserSet.Object);
            mockAppUserManager.Setup(x => x.Users).Returns(mockDbUserSet.Object);

            dbUserService = new DBUserService(mockAppRoleManager.Object, mockAppUserManager.Object, true);
        }

        //-----------------------------------------------------------------------------------------

        [TestMethod]
        public void UserService_EditAsync_CreatesUserIfNotInDbWithGUIDPassword()
        {
            //Arrange
            DBUser record3 = new DBUser
            {
                Id = "dummyId3",
                UserName = "UserName3",
                Email = "Email1",
                LDAPAuthenticated_bl = true,
                Password = "NewPassword3"
            };

            //Act   
            dbUserService.EditAsync(new[] {record3}).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(record3.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Once);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*");
            Assert.IsTrue(regex.IsMatch(record3.Password));
        }

        [TestMethod]
        public void UserService_EditAsync_CreatesUserIfNotInDbWithGivenPassword()
        {
            //Arrange
            DBUser record3 = new DBUser
            {
                Id = "dummyId3",
                UserName = "UserName3",
                Email = "Email1",
                LDAPAuthenticated_bl = false,
            };

            //Act   
            dbUserService.EditAsync(new[] {record3}).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(record3.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Once);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
            Assert.IsTrue(String.IsNullOrEmpty(record3.Password));
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserwithGUIDPassword()
        {
            //Arrange
            DBUser record2 = new DBUser
            {
                Id = dbUser2.Id,
                UserName = "UserName10",
                Email = "Email10",
                LDAPAuthenticated_bl = true,
                Password = "NewPassword10",
                ModifiedProperties = new[] { "Password" }
            };

            //Act   
            dbUserService.EditAsync(new[] {record2}).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(record2.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Once);
            mockAppUserManager.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(record2.UserName != dbUser1.UserName);
            Assert.IsTrue(record2.Email != dbUser1.Email);
            Assert.IsTrue(record2.LDAPAuthenticated_bl != dbUser2.LDAPAuthenticated_bl);
            var regex = new Regex(@"\w*-\w*-\w*-\w*-\w*out\b");
            Assert.IsTrue(regex.IsMatch(dbUser2.PasswordHash));
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserwithPassword()
        {
            //Arrange
            DBUser record1 = new DBUser
            {
                Id = dbUser1.Id,
                UserName = "UserName10",
                Email = "Email10",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword10",
                ModifiedProperties = new[] { "Password" }
            };

            //Act   
            dbUserService.EditAsync(new[] { record1 }).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(dbUser1.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Once);
            mockAppUserManager.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(record1.UserName != dbUser1.UserName);
            Assert.IsTrue(record1.Email != dbUser1.Email);
            Assert.IsTrue(record1.LDAPAuthenticated_bl != dbUser1.LDAPAuthenticated_bl);
            Assert.IsTrue(dbUser1.PasswordHash == record1.Password + "out");
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateSingleUserLeavePasswordIfPropNotModified()
        {
            //Arrange
            DBUser record1 = new DBUser
            {
                Id = dbUser1.Id,
                UserName = "UserName10",
                Email = "Email10",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword10",
                ModifiedProperties = new[] { "LDAPAuthenticated_bl" }
            };

            //Act   
            dbUserService.EditAsync(new[] { record1 }).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.FindByIdAsync(dbUser1.Id), Times.Once);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Once);
            mockAppUserManager.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
            Assert.IsTrue(record1.UserName != dbUser1.UserName);
            Assert.IsTrue(record1.Email != dbUser1.Email);
            Assert.IsTrue(record1.LDAPAuthenticated_bl == dbUser1.LDAPAuthenticated_bl);
            Assert.IsTrue(String.IsNullOrEmpty(dbUser1.PasswordHash));
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateManyUsersDoUpdateModified()
        {
            // Arrange
            DBUser record1 = new DBUser
            {
                Id = dbUser1.Id,
                UserName = "UserName10",
                Email = "Email10",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword10",
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl", }
            };
            DBUser record2 = new DBUser
            {
                Id = dbUser2.Id,
                UserName = "UserName20",
                Email = "Email20",
                LDAPAuthenticated_bl = true,
                Password = "NewPassword20",
            };

            //Act   
            dbUserService.EditAsync(new[] { record1, record2 }).Wait();

            //Assert
            Assert.IsTrue(record1.UserName == dbUser1.UserName);
            Assert.IsTrue(record1.Email == dbUser1.Email);
            Assert.IsTrue(record1.LDAPAuthenticated_bl == dbUser1.LDAPAuthenticated_bl);
            Assert.IsTrue(dbUser1.PasswordHash == null);
            Assert.IsTrue(record2.UserName != dbUser2.UserName);
            Assert.IsTrue(record2.Email != dbUser2.Email);
            Assert.IsTrue(record2.LDAPAuthenticated_bl != dbUser2.LDAPAuthenticated_bl);
            Assert.IsTrue(record2.PasswordHash == null);
            mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
            mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Exactly(2));
        }

        [TestMethod]
        public void UserService_EditAsync_UpdateManyUsersReportErrorsFromIdentityResult()
        {
            // Arrange
            DBUser record1 = new DBUser
            {
                Id = dbUser1.Id,
                UserName = "UserName10",
                Email = "Email10",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword10",
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl", }
            };
            DBUser record2 = new DBUser
            {
                Id = dbUser2.Id,
                UserName = "UserName20",
                Email = "Email20",
                LDAPAuthenticated_bl = true,
                Password = "NewPassword20",
            };

            mockAppUserManager.Setup(x => x.UpdateAsync(dbUser2)).Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error1", "Error2" })));

            try
            {
                //Act   
                dbUserService.EditAsync(new[] { record1, record2 }).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("Error editing user UserName20:Error1; Error2;"));

                Assert.IsTrue(record1.UserName == dbUser1.UserName);
                Assert.IsTrue(record1.Email == dbUser1.Email);
                Assert.IsTrue(record1.LDAPAuthenticated_bl == dbUser1.LDAPAuthenticated_bl);
                Assert.IsTrue(dbUser1.PasswordHash == null);
                Assert.IsTrue(record2.UserName != dbUser2.UserName);
                Assert.IsTrue(record2.Email != dbUser2.Email);
                Assert.IsTrue(record2.LDAPAuthenticated_bl != dbUser2.LDAPAuthenticated_bl);
                Assert.IsTrue(record2.PasswordHash == null);
                mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
                mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Exactly(2));
                return;
            }
            Assert.IsFalse(true);

        }

        [TestMethod]
        public void UserService_EditAsync_CreateSingleUserReportErrorsFromIdentityResult()
        {
            //Arrange
            DBUser record3 = new DBUser
            {
                Id = "dummyId3",
                UserName = "UserName3",
                Email = "Email1",
                LDAPAuthenticated_bl = false,
                Password = "NewPassword3"
            };

            mockAppUserManager.Setup(x => x.CreateAsync(record3,record3.Password))
                .Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error1", "Error2" })));

            try
            {
                //Act   
                dbUserService.EditAsync(new[] { record3 }).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("Error creating user UserName3:Error1; Error2;"));

                mockAppUserManager.Verify(x => x.FindByIdAsync(record3.Id), Times.Once);
                mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Once);
                mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
                Assert.IsTrue(record3.Password == "NewPassword3");
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UserService_EditAsync_ReturnsErrorifNewUserwithNoPasswordAndLDAPFalse()
        {
            //Arrange
            DBUser record3 = new DBUser
            {
                Id = "dummyId3",
                UserName = "UserName3",
                Email = "Email1",
                LDAPAuthenticated_bl = false,
                ModifiedProperties = new[] { "UserName", "Email", "LDAPAuthenticated_bl" }
            };

            try
            {
                //Act   
                dbUserService.EditAsync(new[] { record3 }).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("User UserName3: Password is required if not LDAP authenticated"));

                mockAppUserManager.Verify(x => x.FindByIdAsync(record3.Id), Times.Never);
                mockAppUserManager.Verify(x => x.CreateAsync(It.IsAny<DBUser>(), It.IsAny<string>()), Times.Never);
                mockAppUserManager.Verify(x => x.UpdateAsync(It.IsAny<DBUser>()), Times.Never);
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UserService_GetAsync_ReturnsResultWithUsers()
        {
            //Act   
            var dbUserList = dbUserService.GetAsync().Result;

            //Assert
            Assert.IsTrue(dbUserList.Count() == 2);
            Assert.IsTrue(dbUserList[0].Email == dbUser1.Email );
            mockAppUserManager.Verify(x => x.Users, Times.Once);
        }

        [TestMethod]
        public void UserService_DeleteAsync_DeletesAndReturnsOK()
        {
            //Act   
            dbUserService.DeleteAsync(new[] {dbUser1.Id}).Wait();

            //Assert
            mockAppUserManager.Verify(m => m.FindByIdAsync(dbUser1.Id), Times.Once());
            mockAppUserManager.Verify(m => m.DeleteAsync(dbUser1), Times.Once());
        }

        [TestMethod]
        public void UserService_DeleteAsync_ReturnsDeleteErrorNotFound()
        {
            //Arrange
            mockAppUserManager.Setup(x => x.FindByIdAsync(dbUser1.Id)).Returns(Task.FromResult<DBUser>(null));
            
            try
            {
                //Act   
                dbUserService.DeleteAsync(new[] { dbUser1.Id }).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains(String.Format("DB User with Id={0} not found",dbUser1.Id)));

                mockAppUserManager.Verify(m => m.FindByIdAsync(dbUser1.Id), Times.Once());
                mockAppUserManager.Verify(m => m.DeleteAsync(dbUser1), Times.Never());
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UserService_EditRolesAsync_ReturnErrorIfUserNotFound()
        {
            try
            {
                //Act   
                dbUserService.AddRemoveRolesAsync(new[] { "dummyId999" }, new[] { "role1", "role2" }, true).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains("Error adding/removing roles. User(s) not found."));
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Add_AddsRoles()
        {
            //Act   
            dbUserService.AddRemoveRolesAsync(new[] {dbUser1.Id, dbUser2.Id}, new[] {"roleName1","roleName2"}, true).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Add_AddsRolesReturnsIdentityErr()
        {
            // Arrange
            mockAppUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed(new[] { "Error1" })));
            
            try
            {
                //Act   
                dbUserService.AddRemoveRolesAsync(new[] { dbUser1.Id, dbUser2.Id }, new[] { "roleName1", "roleName2" }, true).Wait();
            }
            catch (Exception exception)
            {
                //Assert
                Assert.IsTrue(exception.GetBaseException().GetType() == typeof(DbBadRequestException));
                Assert.IsTrue(exception.GetBaseException().ToString().Contains(
                    string.Format("Error adding role {0} to user {1}: {2}", "roleName1", dbUser1.UserName,"Error1")));
                return;
            }
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UserService_EditRolesAsync_Remove_RemovesRoles()
        {
            //Arrange
            mockAppUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act   
            dbUserService.AddRemoveRolesAsync(new[] { dbUser1.Id, dbUser2.Id }, new[] { "roleName1", "roleName2" }, false).Wait();

            //Assert
            mockAppUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockAppUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
        }

        
    }

}
