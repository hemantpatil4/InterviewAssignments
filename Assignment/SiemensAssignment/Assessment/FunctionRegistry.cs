namespace ExpressionEvaluator
{
    // Step 5: name → implementation map (replaces Evaluator if/else chain)
    public sealed class FunctionRegistry
    {
        private readonly Dictionary<string, IFunction> _functions =
            new(StringComparer.OrdinalIgnoreCase);

        // Works as Builder also.
        public FunctionRegistry Register(IFunction function)
        {
            _functions[function.Name] = function;
            return this;
        }

        public bool TryGet(string name, out IFunction function) =>
            _functions.TryGetValue(name, out function!);

        /// <summary>
        /// Optional HttpClient so FetchGet can be tested with a fake handler
        /// instead of a real network call.
        /// </summary>
        public static FunctionRegistry CreateDefault(HttpClient? httpClient = null)
        {
            return new FunctionRegistry()
                .Register(new AddFunction())
                .Register(new EqualsFunction())
                .Register(new NotFunction())
                .Register(new ContainsFunction())
                .Register(new FetchGetFunction(httpClient ?? new HttpClient()));
        }
    }
}
