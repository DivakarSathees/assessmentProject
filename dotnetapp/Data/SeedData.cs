// Seeds mock data into MockDbContext
using dotnetapp.Models;

namespace dotnetapp.Data {
    public static class SeedData {
        public static void Seed(MockDbContext db) {
            db.Users.AddRange(new[] {
                new User { Id = "u1", Username = "admin", PasswordHash = "hashed_admin_pass", Role = "Admin" },
                new User { Id = "u2", Username = "editor_jane", PasswordHash = "hashed_editor_pass", Role = "Editor" }
            });

            db.Documents.AddRange(new[] {
                new Document { Id = "d1", Title = "System Design Spec", Content = "Initial draft.", OwnerId = "u2" },
                new Document { Id = "d2", Title = "Editor Notes", Content = "Training material.", OwnerId = "u2" }
            });

            db.Rules.Add(new Rule {
                Id = "r1",
                Action = "Edit",
                Expression = "user.Id == resource.OwnerId"
            });
        }
    }
}