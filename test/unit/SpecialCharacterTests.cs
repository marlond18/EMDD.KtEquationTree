using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace KtEquationTreeTest;

[TestClass]
public class SpecialCharacterTests
{

    [TestMethod]
    public void Beta1()
    {
        var actual = ExprParser.ParseOrThrow("β_1");
        var expected = new Identifier("β_1");
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Pi()
    {
        var actual = ExprParser.ParseOrThrow("2×π");
        var expected = Literal.Create(2) * new Identifier("π");
        Assert.AreEqual(expected, actual);
    }
}