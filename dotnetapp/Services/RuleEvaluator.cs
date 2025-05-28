// RuleEvaluator interprets dynamic expressions
using dotnetapp.Models;
using DynamicExpresso;

namespace dotnetapp.Services {
    public class RuleEvaluator {
        public bool Evaluate(string expression, User user, Document resource) {
            var interpreter = new Interpreter();
            interpreter.SetVariable("user", user);
            interpreter.SetVariable("resource", resource);
            Console.WriteLine(user.Id);
            Console.WriteLine(resource.Id);
            return interpreter.Eval<bool>(expression);
        }
    }
}