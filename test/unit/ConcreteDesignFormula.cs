namespace KtEquationTreeTest;

[TestClass]
public class ConcreteDesignFormula
{
    [TestMethod]
    public void Beta1Formula()
    {
        var actual = ExprParser.ParseOrThrow("0.85-(0.05/7)*(fᐠ_c-28)");
        var expected = Dec.Create(0.85m) - (Dec.Create(0.05m) / Literal.Create(7)) * (new Identifier("fᐠ_c") - Literal.Create(28));
        Assert.AreEqual(expected, actual);
        Assert.AreEqual("0.85-((0.05/7)×(fᐠ_c-28))", actual.ToString());
        var substiResult = actual.Subtitute(new Identifier("fᐠ_c"), 32).Simplify();
        Assert.AreEqual(substiResult, DivideOp.Create(23, 28));
        if (substiResult.TryToDouble(out var val))
        {
            Assert.AreEqual(0.8214, val, 0.0001);
        }
        else
        {
            Assert.Fail($"{substiResult} is not a double");
        }
    }
}