namespace ExpressionEvaluator
{
    /// <summary>
    /// Walks an AST. Uses a FunctionRegistry (Strategy lookup by name).
    /// Pass a custom registry to inject a fake HttpClient for fetchGet tests.
    /// </summary>
    public sealed class Evaluator
    {
        private readonly FunctionRegistry _registry;

        public Evaluator(FunctionRegistry? registry = null)
        {
            _registry = registry ?? FunctionRegistry.CreateDefault();
        }

        // Convenience for Program.cs / simple calls (default registry + real HttpClient)
        public static Value Evaluate(Node expression) =>
            new Evaluator().EvaluateNode(expression);

        public Value EvaluateNode(Node expression)
        {
            if (expression.GetType() == typeof(Literal))
            {
                var literal = (Literal)expression;
                return literal.Value;
            }

            if (expression is not Function function)
            {
                throw new EvaluationException(
                    $"Unsupported node type: {expression.GetType().Name}");
            }

            // Original if/else chain —
            // if (function.Name == "add") { ... Functions.Add ... }
            // else if (function.Name == "equals") { ... }
            // else if (function.Name == "not") { ... }
            // else throw unknown

            // evaluate all args, then dispatch by name via registry
            if (!_registry.TryGet(function.Name, out var impl))
            {
                throw new EvaluationException($"Unknown function: '{function.Name}'");
            }

            // Recursion: evaluate children first → values
            var args = new List<Value>();
            foreach (var parameter in function.Parameters)
            {
                args.Add(EvaluateNode(parameter));
            }

            return impl.Invoke(args);
        }
    }
}
