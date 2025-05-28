// UsersController manages user operations
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Data;
using dotnetapp.Models;

namespace dotnetapp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly MockDbContext _db;

        public UsersController(MockDbContext db) {
            _db = db;
        }

        [HttpGet]
        // [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_db.Users);

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(User user) {
            if (_db.Users.Any(u => u.Username == user.Username))
                return BadRequest("Username already exists");
            user.Role = "Editor";
            _db.Users.Add(user);
            return Ok(user);
        }
    }
}