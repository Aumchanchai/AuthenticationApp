using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly string _key;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly int _expireMinutes;
        private readonly int _iterations;
        private readonly int _saltSize;
        private readonly int _keySize;

        public JwtService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
            _issuer = _config["Jwt:Issuer"];
            _audience = _config["Jwt:Audience"];
            _expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var em) ? em : 60;
            _iterations = int.TryParse(_config["Jwt:Iterations"], out var it) ? it : 10000;
            _saltSize = int.TryParse(_config["Jwt:SaltSize"], out var ss) ? ss : 16;
            _keySize = int.TryParse(_config["Jwt:KeySize"], out var ks) ? ks : 32;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using var rdb = new Rfc2898DeriveBytes(password, _saltSize, _iterations, HashAlgorithmName.SHA512);
            passwordSalt = rdb.Salt;
            passwordHash = rdb.GetBytes(_keySize);
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));
            if (passwordSalt == null) throw new ArgumentNullException(nameof(passwordSalt));

            using var rdb = new Rfc2898DeriveBytes(password, passwordSalt, _iterations, HashAlgorithmName.SHA512);
            var computed = rdb.GetBytes(passwordHash.Length);
            return CryptographicOperations.FixedTimeEquals(computed, passwordHash);
        }

        public string GenerateToken(int userId, string username, string[]? roles = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null)
            {
                foreach (var r in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
