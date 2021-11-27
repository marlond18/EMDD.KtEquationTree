namespace KtEquationTreeTest;

[TestClass]
public class TryToDoubleTests
{
    [DataTestMethod]
    [DataRow("1+3.2", true, 4.2)]
    [DataRow("1-3.2", true, -2.2)]
    [DataRow("2^4", true, 16)]
    [DataRow("√4", true, 2)]
    [DataRow("3.2*5", true, 16)]
    [DataRow("5/2", true, 2.5)]
    public void ToDoubleTest(string input, bool expected, double expectedValue)
    {
        var expr = ExprParser.ParseOrThrow(input);
        var actual = expr.TryToDouble(out double actualValue);
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expectedValue, actualValue);
    }
}
