using dotnetapp.Data;
using dotnetapp.Models;

namespace dotnetapp.Services {
    public class AccessService {
        private readonly MockDbContext _db;
        private readonly RuleEvaluator _evaluator;

        public AccessService(MockDbContext db, RuleEvaluator evaluator) {
            _db = db;
            _evaluator = evaluator;
        }

        public (bool allowed, string reason) EvaluateAccess(string userId, string action, string resourceId) {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            var resource = _db.Documents.FirstOrDefault(d => d.Id == resourceId);

            if (user == null || resource == null)
                return (false, "User or Resource not found");

            if (user.Role == "Admin")
            
                return (true, "Admin bypass access");

            var rule = _db.Rules.FirstOrDefault(r => r.Action == action);
            if (rule == null)
                return (false, "No rule for action");
            Console.WriteLine(rule.Expression);

            var result = _evaluator.Evaluate(rule.Expression, user, resource);
            return result ? (true, "User is the owner") : (false, "Access denied by rule");
        }
    }
}