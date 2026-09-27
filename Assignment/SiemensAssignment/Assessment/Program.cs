// This top level class is added just for the local testing purpose before unit testing. Keeping the same as of now.

using ExpressionEvaluator;

Console.WriteLine("Start => Main Program Started");

// Testing Add Function
var addResult1 = Evaluator.Evaluate(new Function("add", [
    new Literal(new Value(3)),
    new Literal(new Value(6))
]));
Console.WriteLine($"Add Result 1: {addResult1.Get<int>()}");

var addResult2 = Evaluator.Evaluate(new Literal(new Value(2)));
Console.WriteLine($"Add Result 2: {addResult2.Get<int>()}");

var addResult3 = Evaluator.Evaluate(new Literal(new Value(3)));
Console.WriteLine($"Add Result 3: {addResult3.Get<int>()}");

var addResult4 = Evaluator.Evaluate(new Literal(new Value(4)));
Console.WriteLine($"Add Result 4: {addResult4.Get<int>()}");

var addResult5 = Evaluator.Evaluate(new Function("add", [
    new Literal(new Value(3.5)),
    new Literal(new Value(6))
]));
Console.WriteLine($"Add Result 5: {addResult5.Get<double>()}");

// Testing Equals Function

var equalsResult1 = Evaluator.Evaluate(new Function("equals", [
    new Literal(new Value(3)),
    new Literal(new Value(6))
]));
Console.WriteLine($"Equals Result 1: {equalsResult1.Get<bool>()}");


var equalsResult2 = Evaluator.Evaluate(new Function("equals", [
    new Literal(new Value(3.0)),
    new Literal(new Value(3))
]));
Console.WriteLine($"Equals Result 2: {equalsResult2.Get<bool>()}");

// Testing Not Equals Function

var notEqualsResult1 = Evaluator.Evaluate(new Function("not", [
    new Function("equals",[
    new Literal(new Value(3.0)),
    new Literal(new Value(3))
    ])
]));
Console.WriteLine($"Not Equals Result 1: {notEqualsResult1.Get<bool>()}");

// clearer errors (should print EvaluationException messages) ----------
Console.WriteLine();
Console.WriteLine("Step 4 — clearer errors");

Try("Get<int> on a string value", () =>
{
    var literal = Evaluator.Evaluate(new Literal(new Value("hello")));
    literal.Get<int>();
});

Try("unknown function toString()", () =>
{
    Evaluator.Evaluate(new Function("toString", []));
});

Try("add(true, 1) — invalid types", () =>
{
    Evaluator.Evaluate(new Function("add", [
        new Literal(new Value(true)),
        new Literal(new Value(1))
    ]));
});

Try("not(3) — not a bool", () =>
{
    Evaluator.Evaluate(new Function("not", [
        new Literal(new Value(3))
    ]));
});



static void Try(string label, Action action)
{
    Console.WriteLine();
    Console.WriteLine($"> {label}");
    try
    {
        action();
        Console.WriteLine("  (no error — unexpected)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
    }
}


// Registery implementation
Console.WriteLine();
 var result=Evaluator.Evaluate(new Function("add", [
        new Literal(new Value(3)),new Function("add", [
        new Literal(new Value(4)),new Function("add", [
        new Literal(new Value(5)), new Literal(new Value(6))
    ])
    ])
    ]));
    System.Console.WriteLine($"Step 5 After Registery Implementation. Result is = {result.Get<int>()}");


// Contains Function Implementation

Console.WriteLine();
 var containsResult=Evaluator.Evaluate(new Function("contains", [new Literal(new Value("wqe")),new Literal(new Value("q"))]));
    System.Console.WriteLine($"Contains Result = {containsResult.Get<bool>()}");

