using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Data;
using dotnetapp.Dtos;
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
        [Authorize]
        public IActionResult GetAllDocuments() => Ok(_db.Documents);

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetDocumentById(string id) {
            var doc = _db.Documents.FirstOrDefault(d => d.Id == id);
            return doc == null ? NotFound() : Ok(doc);
        }

        [HttpPost]
        [Authorize(Roles = "Editor")]
        public IActionResult CreateDocument(Document doc) {
            Console.WriteLine("Creating document");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_db.Documents.Any(d => d.Title == doc.Title))
                return BadRequest("Document title must be unique");
            doc.Id = Guid.NewGuid().ToString();
            doc.OwnerId = userId;
            _db.Documents.Add(doc);
            return Ok(doc);
        }

//         [HttpPost]
// [Authorize(Roles = "Editor")]
// public async Task<IActionResult> CreateDocument([FromForm] string title, [FromForm] IFormFile file)
// {
//     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//     if (_db.Documents.Any(d => d.Title == title))
//         return BadRequest("Document title must be unique");

//     if (file == null || file.Length == 0)
//         return BadRequest("Uploaded file is empty");

//     string content;
//     using (var reader = new StreamReader(file.OpenReadStream()))
//     {
//         content = await reader.ReadToEndAsync();
//     }

//     var doc = new Document
//     {
//         Id = Guid.NewGuid().ToString(),
//         Title = title,
//         Content = content,
//         OwnerId = userId
//     };

//     _db.Documents.Add(doc);
//     // await _db.SaveChangesAsync();

//     return Ok(doc);
// }


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
            Console.WriteLine($"Evaluating rule: {rule.Expression}");
            var allowed = evaluator.Evaluate(rule.Expression, user, doc);
            Console.WriteLine($"Access allowed: {allowed}");
            if (!allowed)
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied: not the owner");

            doc.Title = updatedDoc.Title;
            doc.Content = updatedDoc.Content;
            return Ok(doc);
        }
    }
}
