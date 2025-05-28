// In-memory lists of users, documents, and rules
using dotnetapp.Models;

namespace dotnetapp.Data {
    public class MockDbContext {
        public List<User> Users = new List<User>();
        public List<Document> Documents = new List<Document>();
        public List<Rule> Rules = new List<Rule>();

        public MockDbContext() {
            SeedData.Seed(this);
        }
    }
}