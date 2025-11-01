using Authentication.Controllers;
using Authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;

namespace Authentication.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            // สร้าง DI Provider
            var provider = DependencyResolver.BuildServiceProvider();
            _authController = provider.GetRequiredService<AuthController>();
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsValid()
        {
            var model = new AuthInputModel
            {
                Username = "unit_test_user",
                Password = "password123"
            };

            dynamic response = await SignupUser(model);

            Assert.NotNull(response);
            Assert.Equal("User registered successfully", (string)response.Message);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenUserIsValid()
        {
            var model = new AuthInputModel
            {
                Username = "unit_test_user",
                Password = "password123"
            };

            dynamic responseOne = await SignupUser(model);

            var result = await _authController.Login(model);

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as LoginResponeModel;
            Assert.NotNull(response);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserIsInvalid()
        {

            var model = new AuthInputModel
            {
                Username = "unit_test_user",
                Password = "password123"
            };

            await SignupUser(model);

            model.Password = "newPassword";

            var result = await _authController.Login(model);

            Assert.NotNull(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            dynamic response = unauthorizedResult.Value;

            Assert.Equal("Invalid username or password.", (string)response);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task Profile_ShouldReturnOk_WhenUserIsNotLogin()
        {
            try
            {
                var result = await _authController.Profile();
            }
            catch (Exception ex)
            {
                Assert.Equal("Object reference not set to an instance of an object.", ex.Message);
            }

        }

        [Fact]
        public async Task Profile_ShouldReturnOk_WhenGetProfileSuccess()
        {
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "unit_test_user")
            }, "mock"));

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userClaims
                }
            };

            var model = new AuthInputModel
            {
                Username = "unit_test_user",
                Password = "password123"
            };

            await SignupUser(model);
            await _authController.Login(model);

            var result = await _authController.Profile();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as ProfileResponeModel;

            Assert.Equal(model.Username, response.Username);
        }

        private async Task<dynamic> SignupUser(AuthInputModel input)
        {
            var model = new AuthInputModel
            {
                Username = input.Username,
                Password = input.Password
            };

            var result = await _authController.SignUp(model);

            var okResult = Assert.IsType<OkObjectResult>(result); // check 200 OK
            var response = okResult.Value as SignUpResponeModel;
            return response;
        }
    }
}
