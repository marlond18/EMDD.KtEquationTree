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
        var aa = ExprParser.ParseOrThrow("a_a");
        var bb = ExprParser.ParseOrThrow("b_b");
        var result1 = e.Subtitute(aa, ExprParser.ParseOrThrow(a));
        var result2 = result1.Subtitute(bb, ExprParser.ParseOrThrow(b));
        if (result2.TryToDouble(out double actual))
        {
            Assert.AreEqual(expected, actual,0.00000001);
        }
        else
        {
            Assert.Fail($"{e} should be double.");
        }
    }
}