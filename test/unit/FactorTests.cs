namespace KtEquationTreeTest
{
    [TestClass]
    public class FactorTests
    {
        [TestMethod]
        public void LiteralFactor()
        {
            var newlit = ExprParser.ParseOrThrow("2069366877425482173897306373270574575520870902460429264486400000");
            var expected = newlit.Factor();
            var actual = new[] { PowerOp.Create(2, 77), PowerOp.Create(3, 52), PowerOp.Create(5, 5), PowerOp.Create(7, 14) };
            Assert.IsTrue(expected.ContentEquals(actual));
        }
    }
}