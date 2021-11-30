namespace KtEquationTreeTest;



[TestClass]
public class CreateNegativeTest
{
    [TestMethod]
    public void CreateNegativeZeroTest()
    {
        var zero = new Zero();
        var negativezero = NegativeOp.Create(0);
        Assert.AreEqual(zero, negativezero);
    }

    [TestMethod]
    public void CreateNegativeOneTest()
    {
        var negativeone = new NegativeOne();
        Assert.AreEqual(negativeone.GetType(), typeof(NegativeOne));
    }
}
