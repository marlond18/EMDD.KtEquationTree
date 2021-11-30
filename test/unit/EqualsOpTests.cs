using EMDD.KtEquationTree.Relations;

namespace KtEquationTreeTest;

[TestClass]
public class EqualsOpTests
{
    [DataTestMethod]
    [EqualsOpCreateTestData]
    public void SimpleEqualsOpTest(string ex)
    {
        var a = ExprParser.ParseOrThrow(ex);
        Assert.IsTrue(a is EqualsOp);
    }

    [TestMethod]
    [DataRow("a=b", "2", "a+2=b+2")]
    [DataRow("2", "a=b", "a+2=b+2")]
    [DataRow("a=b", "c=d", "a+c=b+d")]
    [DataRow("c=d", "a=b", "a+c=b+d")]
    public void AddEqualsOpTest(string aa, string bb, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var b = ExprParser.ParseOrThrow(bb);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a + b);
    }

    [TestMethod]
    [DataRow("a=b", "2", "a-2=b-2")]
    [DataRow("2", "a=b", "2-a=2-b")]
    [DataRow("a=b", "c=d", "a-c=b-d")]
    [DataRow("c=d", "a=b", "c-a=d-b")]
    public void SubtractEqualsOpTest(string aa, string bb, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var b = ExprParser.ParseOrThrow(bb);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a - b);
    }
    
    [TestMethod]
    [DataRow("a=b", "2", "a*2=b*2")]
    [DataRow("2", "a=b", "2*a=2*b")]
    [DataRow("a=b", "c=d", "a*c=b*d")]
    [DataRow("c=d", "a=b", "c*a=d*b")]
    public void MultEqualsOpTest(string aa, string bb, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var b = ExprParser.ParseOrThrow(bb);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a * b);
    }

    [TestMethod]
    [DataRow("a=b", "2", "a/2=b/2")]
    [DataRow("2", "a=b", "2/a=2/b")]
    [DataRow("a=b", "c=d", "a/c=b/d")]
    [DataRow("c=d", "a=b", "c/a=d/b")]
    public void DivEqualsOpTest(string aa, string bb, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var b = ExprParser.ParseOrThrow(bb);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a / b);
    }

    [TestMethod]
    [DataRow("a=b", "2", "a^2=b^2")]
    [DataRow("2", "a=b", "2^a=2^b")]
    [DataRow("a=b", "c=d", "a^c=b^d")]
    [DataRow("c=d", "a=b", "c^a=d^b")]
    public void PowerEqualsOpTest(string aa, string bb, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var b = ExprParser.ParseOrThrow(bb);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a.Raise(b));
        //todo: test failed power operation of other relation type
    }

    [TestMethod]
    [DataRow("a=b", "√a=√b")]
    [DataRow("a+2=b+2", "√(a+2)=√(b+2)")]
    public void SqrtEqualsOpTest(string aa, string cc)
    {
        var a = ExprParser.ParseOrThrow(aa);
        var c = ExprParser.ParseOrThrow(cc);
        Assert.AreEqual(c, a.Sqrt());
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class EqualsOpCreateTestDataAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new[] { "a=b" };
            foreach (var item in new[] { '+', '-', '*', '/', '^' })
            {
                yield return new[] { $"a=b{item}2" };
                yield return new[] { $"a{item}c=b" };
                yield return new[] { $"a{item}c=b{item}2" };
            }
            yield return new[] { "a=√b" };
            yield return new[] { "a=~b" };
            yield return new[] { "√a=b" };
            yield return new[] { "~a=b" };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return $"Creating an EqualsOp: {data[0]}";
        }
    }
}
