namespace KtEquationTreeTest;

[TestClass]
public class ExprMethodTests
{
    [DataTestMethod]
    [DataRow("a_a+b_b", "1.2", "3.2", 4.4)]
    [DataRow("a_a-b_b", "1.2", "3.2", -2.0)]
    [DataRow("a_a*b_b", "-1.2", "3.2", -3.84)]
    [DataRow("a_a/b_b", "1.2", "-3.2", -0.375)]
    [DataRow("a_a^b_b", "1.2", "2", 1.44)]
    public void BasicSubstitutionTest(string expr, string a, string b, double expected)
    {
        var e = ExprParser.ParseOrThrow(expr);
        var aa = ExprParser.ParseOrThrow("a_a") as Expr;
        var bb = ExprParser.ParseOrThrow("b_b") as Expr;
        var result1 = e.Substitute(aa, ExprParser.ParseOrThrow(a) as Expr);
        var result2 = result1.Substitute(bb, ExprParser.ParseOrThrow(b) as Expr);
        if (result2.TryToDouble(out double actual))
        {
            Assert.AreEqual(expected, actual, 0.00000001);
        }
        else
        {
            Assert.Fail($"{e} should be double.");
        }
    }

    [DataTestMethod]
    [DataRow("1+3.2", true, 4.2)]
    [DataRow("1-3.2", true, -2.2)]
    [DataRow("2^4", true, 16)]
    [DataRow("√4", true, 2)]
    [DataRow("3.2*5", true, 16)]
    [DataRow("5/2", true, 2.5)]
    public void TryToDoubleTest(string input, bool expected, double expectedValue)
    {
        var expr = ExprParser.ParseOrThrow(input);
        var actual = expr.TryToDouble(out double actualValue);
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expectedValue, actualValue);
    }
}