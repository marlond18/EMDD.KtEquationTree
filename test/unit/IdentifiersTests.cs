namespace KtEquationTreeTest;

[TestClass]
public class IdentifiersTests
{
    [DataTestMethod]
    [DataRow("a_a")]
    [DataRow("a_")]
    [DataRow("b_a")]
    [DataRow("b_adaa1")]
    [DataRow("b1_adaa")]
    [DataRow("b1_1adaa")]
    [DataRow("b1_1")]
    public void WithSubscriptTest(string expr)
    {
        var e = ExprParser.ParseOrThrow(expr);
        if (e is Identifier id)
        {
            Assert.AreEqual(id.Definition, expr);
        }
        else
        {
            Assert.Fail($"{e} from {expr} is not an identifier");
        }
    }
}