namespace KtEquationTreeTest;
[TestClass]
public class FactorTests
{
    [TestMethod]
    public void LiteralFactorOfBigNumber()
    {
        var newlit = ExprParser.ParseOrThrow("2069366877425482173897306373270574575520870902460429264486400000");
        if (newlit is Literal lit)
        {
            var expected = lit.Factor();
            var actual = new[] { PowerOp.Create(2, 77), PowerOp.Create(3, 52), PowerOp.Create(5, 5), PowerOp.Create(7, 14) };
            Assert.IsTrue(expected.ContentEquals(actual));
        }
        else
        {
            Assert.Fail($"Type is not Literal");
        }
    }

    [DataTestMethod]
    [ExprTestData]
    public void LiteralFactor(string name, IEnumerable<Expr> actual, IEnumerable<Expr> expected)
    {
        Console.WriteLine($"actual factor of {name} is {actual.BuildString("*")}");
        Assert.IsTrue(expected.ContentEquals(actual));
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ExprTestDataAttribute : Attribute, ITestDataSource
    {
        private static readonly int[] data = new[] { 2, 3, 5, 7 };
        private readonly Random randomizer = new Random();
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            for (int j = 0; j < 100; j++)
            {
                var d = randomizer.Next(10, 50);
                var tempArr = new Expr[d];
                BigInteger total = 1;
                for (int i = 0; i < d; i++)
                {
                    var val = data[randomizer.Next(4)];
                    total = total * val;
                    tempArr[i] = Literal.Create(val);
                }
                var actual = tempArr.GroupBy((e) => e).Select((g) => g.Count() == 1 ? g.Key : PowerOp.Create(g.Key, g.Count()));
                var expected = Literal.Create(total).Factor();
                yield return new object[] { total.ToString(), actual, expected };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return $"factor of {data[0]}";
        }
    }
}