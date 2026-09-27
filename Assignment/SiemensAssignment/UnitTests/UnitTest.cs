using ExpressionEvaluator;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void It_Evaluates_A_Literal()
        {
            var r = Evaluator.Evaluate(new Literal(new Value(13)));

            Assert.AreEqual(r.Get<int>(), 13);
        }

        [TestMethod]
        public void It_Evaluates_A_Not_Function()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("not", [new Literal(new Value(true))])).Get<bool>(), false);
        }

        [TestMethod]
        public void It_Evaluates_A_Add_Function_For_Ints()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("add", [new Literal(new Value(3)), new Literal(new Value(6))])).Get<int>(), 9);
        }

        [TestMethod]
        public void It_Evaluates_A_Add_Function_For_Doubles()
        {
            Assert.AreEqual(Evaluator.Evaluate(new Function("add", [new Literal(new Value(0.3)), new Literal(new Value(0.6))])).Get<double>(), 0.9, 0.0000001);
        }

        [TestMethod]
        public void It_Evaluates_Add_For_Mixed_Int_And_Double()
        {
            var result = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(3)),
                new Literal(new Value(6.8))
            ]));
            Assert.AreEqual(9.8, result.Get<double>(), 0.0000001);
        }

        [TestMethod]
        public void It_Evaluates_Add_For_Many_Args()
        {
            var result = Evaluator.Evaluate(new Function("add", [
                new Literal(new Value(3)),
                new Literal(new Value(4)),
                new Literal(new Value(5)),
                new Literal(new Value(6))
            ]));
            Assert.AreEqual(18, result.Get<int>());
        }

        [TestMethod]
        public void It_Evaluates_Equals_For_Numbers()
        {
            Assert.IsTrue(Evaluator.Evaluate(new Function("equals", [
                new Literal(new Value(3)),
                new Literal(new Value(3))
            ])).Get<bool>());

            Assert.IsTrue(Evaluator.Evaluate(new Function("equals", [
                new Literal(new Value(3)),
                new Literal(new Value(3.0))
            ])).Get<bool>());
        }

        [TestMethod]
        public void It_Evaluates_Readme_Nested_Not_Equals_Add()
        {
            // Expression: 1 + 2 != 3  →  not(equals(add(1, 2), 3)) → false
            var add = new Function("add", [
                new Literal(new Value(1)),
                new Literal(new Value(2))
            ]);
            var equals = new Function("equals", [add, new Literal(new Value(3))]);
            var ast = new Function("not", [equals]);

            Assert.AreEqual(false, Evaluator.Evaluate(ast).Get<bool>());
        }

        [TestMethod]
        public void It_throws_for_invalid_Expression()
        {
            var literal = Evaluator.Evaluate(new Literal(new Value("")));
            Assert.ThrowsException<EvaluationException>(() => literal.Get<int>());
        }

        [TestMethod]
        public void It_throws_for_invalid_function_expression()
        {
            Assert.ThrowsException<EvaluationException>(() => Evaluator.Evaluate(new Function("toString", [])));
        }

        [TestMethod]
        public void It_Evaluates_Contains()
        {
            var result = Evaluator.Evaluate(new Function("contains", [
                new Literal(new Value("hello Bing")),
                new Literal(new Value("Bing"))
            ]));
            Assert.IsTrue(result.Get<bool>());
        }

        [TestMethod]
        public void It_Throws_For_Contains_Non_Strings()
        {
            Assert.ThrowsException<EvaluationException>(() =>
                Evaluator.Evaluate(new Function("contains", [
                    new Literal(new Value(1)),
                    new Literal(new Value("x"))
                ])));
        }

        [TestMethod]
        public void It_Evaluates_FetchGet_With_Injected_HttpClient()
        {
            // Fake HTTP — no real network
            var handler = new FakeHandler("page about Bing search");
            var http = new HttpClient(handler) { BaseAddress = new Uri("https://example.test") };
            var registry = FunctionRegistry.CreateDefault(http);
            var evaluator = new Evaluator(registry);

            // contains(fetchGet(url), "Bing")
            var ast = new Function("contains", [
                new Function("fetchGet", [new Literal(new Value("https://example.test/"))]),
                new Literal(new Value("Bing"))
            ]);

            Assert.IsTrue(evaluator.EvaluateNode(ast).Get<bool>());
        }

        private sealed class FakeHandler(string body) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(body)
                });
            }
        }
    }
}
