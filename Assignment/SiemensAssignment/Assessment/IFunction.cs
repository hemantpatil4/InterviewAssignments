namespace ExpressionEvaluator
{
    // Each function is a named implementation the registry can look up
    public interface IFunction
    {
        string Name { get; }
        Value Invoke(IReadOnlyList<Value> arguments);
    }
}
