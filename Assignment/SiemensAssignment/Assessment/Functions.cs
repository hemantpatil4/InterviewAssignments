namespace ExpressionEvaluator
{
    //shared arg-count checks (clearer than IndexOutOfRange)
    internal static class ArgCount
    {
        public static void Exact(IReadOnlyList<Value> args, int expected, string functionName)
        {
            if (args.Count != expected)
            {
                throw new EvaluationException(
                    $"Function '{functionName}' expects {expected} argument(s), got {args.Count}.");
            }
        }

        public static void AtLeast(IReadOnlyList<Value> args, int minimum, string functionName)
        {
            if (args.Count < minimum)
            {
                throw new EvaluationException(
                    $"Function '{functionName}' expects at least {minimum} argument(s), got {args.Count}.");
            }
        }
    }

    // ----------  functions as IFunction classes ----------
    // Original static Functions.Add / Equals / Not replaced by these registered types.

    public sealed class AddFunction : IFunction
    {
        public string Name => "add";

        public Value Invoke(IReadOnlyList<Value> arguments)
        {
            ArgCount.AtLeast(arguments, 1, Name);

            // Original (starter bug):
            // return new Value(param1.Get<double>() + param2.Get<double>());

            // All strings → concat
            if (arguments.All(a => a.Get<object>() is string))
            {
                return new Value(string.Concat(arguments.Select(a => (string)a.Get<object>())));
            }

            var allInts = true;
            double sum = 0;

            foreach (var arg in arguments)
            {
                var raw = arg.Get<object>();
                if (!TryAsNumber(raw, out var number))
                {
                    throw new EvaluationException(
                        $"Function 'add' expects numbers (or all strings); got {TypeName(raw)}.");
                }

                if (raw is not int)
                {
                    allInts = false;
                }

                sum += number;
            }

            return allInts ? new Value((int)sum) : new Value(sum);
        }

        private static bool TryAsNumber(object value, out double number) =>
            NumberHelpers.TryAsNumber(value, out number);

        private static string TypeName(object? value) => NumberHelpers.TypeName(value);
    }

    public sealed class EqualsFunction : IFunction
    {
        public string Name => "equals";

        public Value Invoke(IReadOnlyList<Value> arguments)
        {
            ArgCount.Exact(arguments, 2, Name);

            // Original:
            // return new Value(param1.Get<bool>() == param2.Get<bool>());

            var a = arguments[0].Get<object>();
            var b = arguments[1].Get<object>();

            if (a is null && b is null)
            {
                return new Value(true);
            }

            if (a is null || b is null)
            {
                return new Value(false);
            }

            if (NumberHelpers.TryAsNumber(a, out var n1) && NumberHelpers.TryAsNumber(b, out var n2))
            {
                return new Value(n1.Equals(n2));
            }

            return new Value(object.Equals(a, b));
        }
    }

    public sealed class NotFunction : IFunction
    {
        public string Name => "not";

        public Value Invoke(IReadOnlyList<Value> arguments)
        {
            ArgCount.Exact(arguments, 1, Name);

            // Original : Not(Node) assumed Literal
            // Not takes evaluated Value; Get<bool> gives clear Step 4 errors
            return new Value(!arguments[0].Get<bool>());
        }
    }

    public sealed class ContainsFunction : IFunction
    {
        public string Name => "contains";

        public Value Invoke(IReadOnlyList<Value> arguments)
        {
            ArgCount.Exact(arguments, 2, Name);
            var haystack = arguments[0].Get<object>();
            var needle = arguments[1].Get<object>();

            if (haystack is string s1 && needle is string s2)
            {
                return new Value(s1.Contains(s2));
            }

            throw new EvaluationException(
                $"Function 'contains' expects two strings; got {NumberHelpers.TypeName(haystack)} and {NumberHelpers.TypeName(needle)}.");
        }
    }

    /// <summary>
    /// Step 6: HTTP GET → response body as string.
    /// HttpClient is injected so tests can pass a fake handler (no real network).
    /// </summary>
    public sealed class FetchGetFunction(HttpClient httpClient) : IFunction
    {
        public string Name => "fetchGet";

        public Value Invoke(IReadOnlyList<Value> arguments)
        {
            ArgCount.Exact(arguments, 1, Name);

            var urlObj = arguments[0].Get<object>();
            if (urlObj is not string url)
            {
                throw new EvaluationException(
                    $"Function 'fetchGet' expects a string URL; got {NumberHelpers.TypeName(urlObj)}.");
            }

            try
            {
                var body = httpClient.GetStringAsync(url).GetAwaiter().GetResult();
                return new Value(body);
            }
            catch (Exception ex) when (ex is not EvaluationException)
            {
                throw new EvaluationException($"Function 'fetchGet' failed for URL '{url}'.", ex);
            }
        }
    }

    internal static class NumberHelpers
    {
        // widen int → double (conversion, not unboxing-as-double)
        public static bool TryAsNumber(object value, out double number)
        {
            switch (value)
            {
                case int i:
                    number = i;
                    return true;
                case double d:
                    number = d;
                    return true;
                default:
                    number = default;
                    return false;
            }
        }

        public static string TypeName(object? value) =>
            value?.GetType().Name ?? "null";
    }

    /*
    // ============================================================
    // ORIGINAL STARTER — Functions.cs before any step fixes
    // ============================================================
    //
    // namespace ExpressionEvaluator
    // {
    //     internal static class Functions
    //     {
    //         public static Value Add(Value param1, Value param2)
    //         {
    //             return new Value(param1.Get<double>() + param2.Get<double>());
    //         }
    //
    //         public static Value Equals(Value param1, Value param2)
    //         {
    //             return new Value(param1.Get<bool>() == param2.Get<bool>());
    //         }
    //
    //         public static Value Not(Node node)
    //         {
    //             var literal = (Literal)node;
    //             return new Value(!literal.Value.Get<bool>());
    //         }
    //     }
    // }
    */
}
