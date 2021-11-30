namespace KtEquationTreeTest;

[TestClass]
public class DecimalTests
{

    [TestMethod]
    public void CreateDecimalTest()
    {
        var d = Dec.Create(0.5m);
        Assert.AreEqual(d, Literal.Create(1) / Literal.Create(2));
        Assert.AreEqual(d.ToString(), "0.5");
    }

    [TestMethod]
    public void CreateDecimalTest2()
    {
        var d = Dec.Create(0.33333333333333m);
        Assert.AreEqual(d, Literal.Create(1) / Literal.Create(3));
        Assert.AreEqual(d.ToString(), "0.33333333333333");
    }
}
