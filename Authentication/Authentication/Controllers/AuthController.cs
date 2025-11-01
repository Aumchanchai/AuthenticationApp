using Authentication.Models;
using Authentication.Services;
using DAL;
using DAL.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(SignUpResponeModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUp([FromBody] AuthInputModel request)
        {
            if (await _context.tbUsers.AnyAsync(u => u.Username == request.Username && u.IsActive))
                return BadRequest("Username already exists.");

            _jwtService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new tbUser { 
                Username = request.Username, 
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _context.tbUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new SignUpResponeModel { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponeModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] AuthInputModel request)
        {
            var user = await _context.tbUsers.FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);
            if (user == null) return Unauthorized("Invalid credentials");

            var isCorrectPassword = _jwtService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

            if (!isCorrectPassword)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtService.GenerateToken(user.Id, user.Username);
            return Ok(new LoginResponeModel { Token = token });
        }

        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileResponeModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            return Ok(new ProfileResponeModel { Username = username ?? "" });
        }
    }

}
