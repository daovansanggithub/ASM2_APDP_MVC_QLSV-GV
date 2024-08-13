using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApplication10.Controllers;
using WebApplication10.Models;
using Xunit;

namespace WebApplication10.Tests.Controllers
{
    public class CRUD_UserControllerTests
    {
        private readonly CRUD_UserController _controller;
        private readonly Mock<QlsvMvc2Context> _mockContext;
        private readonly Mock<DbSet<User>> _mockSet;

        public CRUD_UserControllerTests()
        {
            _mockContext = new Mock<QlsvMvc2Context>();
            _mockSet = new Mock<DbSet<User>>();

            _controller = new CRUD_UserController(_mockContext.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, UserName = "user1" },
                new User { UserId = 2, UserName = "user2" }
            }.AsQueryable();

            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Users).Returns(_mockSet.Object);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenUserNotFound()
        {
            // Arrange
            _mockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithUser()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "user1" };
            _mockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<User>(viewResult.ViewData.Model);
            Assert.Equal("user1", model.UserName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "user1", Password = "password", Role = "role" };
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Create(user);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model state is invalid");
            var user = new User { UserId = 1, UserName = "user1" };

            // Act
            var result = await _controller.Create(user);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<User>(viewResult.ViewData.Model);
            Assert.Equal(user, model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFoundResult_WhenUserNotFound()
        {
            // Arrange
            _mockContext.Setup(c => c.Users.Find(It.IsAny<int>())).Returns((User)null);

            // Act
            var result = _controller.Edit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithUser()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "user1" };
            _mockContext.Setup(c => c.Users.Find(It.IsAny<int>())).Returns(user);

            // Act
            var result = _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<User>(viewResult.ViewData.Model);
            Assert.Equal("user1", model.UserName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "user1" };
            _controller.ModelState.Clear();
            _mockContext.Setup(c => c.Users.Update(It.IsAny<User>()));
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.Edit(user);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenUserNotFound()
        {
            // Arrange
            _mockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithUser()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "admin" };
            _mockContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<User>(viewResult.ViewData.Model);
            Assert.Equal("admin", model.UserName);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenUserIsDeleted()
        {
            // Arrange
            var user = new User { UserId = 1 };
            _mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mockContext.Setup(c => c.Users.Remove(It.IsAny<User>()));
          /*  _mockContext.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);*/

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void UserExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            _mockContext.Setup(c => c.Users.Any(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>())).Returns(true);

            // Act
            var result = _controller.UserExists(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UserExists_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.Users.Any(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>())).Returns(false);

            // Act
            var result = _controller.UserExists(1);

            // Assert
            Assert.False(result);
        }
    }
}
