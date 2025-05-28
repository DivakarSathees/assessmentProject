using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Data;
using dotnetapp.Models;
using System.Security.Claims;

namespace dotnetapp.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase {
        private readonly MockDbContext _db;

        public DocumentController(MockDbContext db) {
            _db = db;
        }

        [HttpGet]
        // [Authorize]
        public IActionResult GetAllDocuments() => Ok(_db.Documents);

        [HttpGet("{id}")]
        // [Authorize]
        public IActionResult GetDocumentById(string id) {
            var doc = _db.Documents.FirstOrDefault(d => d.Id == id);
            return doc == null ? NotFound() : Ok(doc);
        }

        [HttpPost]
        [Authorize(Roles = "Editor")]
        public IActionResult CreateDocument(Document doc) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_db.Documents.Any(d => d.Title == doc.Title))
                return BadRequest("Document title must be unique");
            doc.Id = Guid.NewGuid().ToString();
            doc.OwnerId = userId;
            _db.Documents.Add(doc);
            return Ok(doc);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Editor")]
        public IActionResult EditDocument(string id, Document updatedDoc) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            var doc = _db.Documents.FirstOrDefault(d => d.Id == id);
            var rule = _db.Rules.FirstOrDefault(r => r.Action == "Edit");

            if (doc == null || user == null || rule == null)
                return NotFound("Document, User, or Rule not found");

            var evaluator = new Services.RuleEvaluator();
            var allowed = evaluator.Evaluate(rule.Expression, user, doc);
            if (!allowed)
                return Forbid("Access denied: not the owner");

            doc.Title = updatedDoc.Title;
            doc.Content = updatedDoc.Content;
            return Ok(doc);
        }
    }
}
