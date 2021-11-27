namespace KtEquationTreeTest;

[TestClass]
public class MultipleOperationTest
{
    [TestMethod]
    public void Test2()
    {
        var d = ExprParser.ParseOrThrow("2*4/3").Simplify();
        Assert.AreEqual(d, DivideOp.Create(8, 3));
    }

    [TestMethod]
    public void Test3()
    {
        var d = ExprParser.ParseOrThrow("1+2*4/3").Simplify();
        Assert.AreEqual(d, DivideOp.Create(11, 3));
    }

    [TestMethod]
    public void Test4()
    {
        var d = ExprParser.ParseOrThrow("2*4/3-4");
        var dsimped = d.Simplify();
        Assert.AreEqual(dsimped, NegativeOp.Create(DivideOp.Create(4, 3)));
    }

    [TestMethod]
    public void Test1()
    {
        var d = ExprParser.ParseOrThrow("1+2*4/3-4").Simplify();
        Assert.AreEqual(d, -DivideOp.Create(1, 3));
    }

    [TestMethod]
    public void MultiplyBinomials()
    {
        var d = ExprParser.ParseOrThrow("x+2").Simplify();
        var e = ExprParser.ParseOrThrow("x-3").Simplify();
        var actual = ExprParser.ParseOrThrow("x^2-x-6").Simplify();
        Assert.AreEqual(d * e, actual);
    }

    [TestMethod]
    public void DifferenceOfTwoSquares()
    {
        var d = ExprParser.ParseOrThrow("x+3").Simplify();
        var e = ExprParser.ParseOrThrow("x-3").Simplify();
        var actual = ExprParser.ParseOrThrow("x^2-9").Simplify();
        Assert.AreEqual(d * e, actual);
    }

    [TestMethod]
    public void BinomialSquared()
    {
        var d = ExprParser.ParseOrThrow("x+2").Simplify();
        var expected = d.Raise(new Two());
        var actual = ExprParser.ParseOrThrow("x^2+4*x+4").Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void BinomialRaisedToNegative()
    {
        var d = ExprParser.ParseOrThrow("x+2").Simplify();
        var expected = d.Raise(-new Two());
        var actual = ExprParser.ParseOrThrow("1/(x^2+4*x+4)").Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void BinomialRaisedToFraction()
    {
        var d = ExprParser.ParseOrThrow("x+2").Simplify();
        var expected = d.Raise(DivideOp.Create(2, 3));
        var actual = ExprParser.ParseOrThrow("(x^2+4*x+4)^(1/3)").Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void BinomialRaisedToNegativeFraction()
    {
        var d = ExprParser.ParseOrThrow("x+2").Simplify();
        var expected = d.Raise(-DivideOp.Create(2, 3));
        var actual = ExprParser.ParseOrThrow("1/(x^2+4*x+4)^(1/3)");
        actual = actual.Simplify();
        Assert.AreEqual(expected, actual);
    }
}
