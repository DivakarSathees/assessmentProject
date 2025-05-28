// AuthController handles login
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Dtos;
using dotnetapp.Services;
using dotnetapp.Data;
using dotnetapp.Models;

namespace dotnetapp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly AuthService _authService;
        private readonly MockDbContext _db;

        public AuthController(AuthService authService, MockDbContext db) {
            _authService = authService;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login(AuthRequestDto dto) {
            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username && u.PasswordHash == dto.Password);
            if (user == null) return Unauthorized();
            var token = _authService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register(AuthRequestDto dto) {
            // password should be 8 characters minimum
            
            if (_db.Users.Any(u => u.Username == dto.Username)) {
                return BadRequest("Username already exists");
            }

            var user = new User {
                Id = Guid.NewGuid().ToString(),
                Username = dto.Username,
                PasswordHash = dto.Password,
                Role = "Editor"
            };

            _db.Users.Add(user);
            // _db.SaveChanges();

            return CreatedAtAction(nameof(Login), new { username = user.Username }, user);
        }
    }
}
