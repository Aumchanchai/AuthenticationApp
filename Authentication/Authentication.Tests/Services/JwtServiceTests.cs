using Authentication.Controllers;
using Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            // สร้าง DI Provider
            var provider = DependencyResolver.BuildServiceProvider();
            _jwtService = provider.GetRequiredService<JwtService>();
        }

        #region CreatePasswordHash Tests

        [Fact]
        public void CreatePasswordHash_ShouldGenerateHashAndSalt()
        {
            _jwtService.CreatePasswordHash("mypassword", out var hash, out var salt);

            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.Equal(64, hash.Length); // SHA512 key size
        }

        [Fact]
        public void CreatePasswordHash_ShouldGenerateDifferentHashOnSamePassword()
        {
            _jwtService.CreatePasswordHash("mypassword", out var hash1, out var salt1);
            _jwtService.CreatePasswordHash("mypassword", out var hash2, out var salt2);

            Assert.NotEqual(hash1, hash2);
            Assert.NotEqual(salt1, salt2);
        }

        [Fact]
        public void CreatePasswordHash_NullPassword_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _jwtService.CreatePasswordHash(null!, out _, out _));
        }

        #endregion

        #region VerifyPasswordHash Tests

        [Fact]
        public void VerifyPasswordHash_CorrectPassword_ShouldReturnTrue()
        {
            _jwtService.CreatePasswordHash("mypassword", out var hash, out var salt);
            bool result = _jwtService.VerifyPasswordHash("mypassword", hash, salt);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordHash_WrongPassword_ShouldReturnFalse()
        {
            _jwtService.CreatePasswordHash("mypassword", out var hash, out var salt);
            bool result = _jwtService.VerifyPasswordHash("wrongpassword", hash, salt);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPasswordHash_NullArguments_ShouldThrow()
        {
            _jwtService.CreatePasswordHash("mypassword", out var hash, out var salt);

            Assert.Throws<ArgumentNullException>(() => _jwtService.VerifyPasswordHash(null!, hash, salt));
            Assert.Throws<ArgumentNullException>(() => _jwtService.VerifyPasswordHash("mypassword", null!, salt));
            Assert.Throws<ArgumentNullException>(() => _jwtService.VerifyPasswordHash("mypassword", hash, null!));
        }

        #endregion

        #region GenerateToken Tests

        [Fact]
        public void GenerateToken_ShouldReturnValidTokenWithClaims()
        {
            var token = _jwtService.GenerateToken(1, "unit_test_user");
            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("unit_test_user", jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal("1", jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.True(jwt.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateToken_WithRoles_ShouldContainRoleClaims()
        {
            string[] roles = new[] { "Admin", "User" };
            var token = _jwtService.GenerateToken(1, "unit_test_user", roles);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            foreach (var role in roles)
            {
                Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
            }
        }

        #endregion

    }
}
