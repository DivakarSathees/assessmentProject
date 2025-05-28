// AccessController handles access evaluation and rule creation
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Data;
using dotnetapp.Dtos;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace dotnetapp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase {
        private readonly AccessService _accessService;
        private readonly MockDbContext _db;

        public AccessController(AccessService accessService, MockDbContext db) {
            _accessService = accessService;
            _db = db;
        }

        [HttpPost("evaluate")]
        public IActionResult EvaluateAccess(AccessRequestDto dto) {
            Console.WriteLine(dto.UserId);
            var (allowed, reason) = _accessService.EvaluateAccess(dto.UserId, dto.Action, dto.ResourceId);
            return Ok(new { allowed, reason });
        }

        [HttpPost("rules")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRule(Rule rule) {
            if (string.IsNullOrWhiteSpace(rule.Expression))
                return BadRequest("Expression is required");
            _db.Rules.Add(rule);
            return Ok(rule);
        }
    }
}