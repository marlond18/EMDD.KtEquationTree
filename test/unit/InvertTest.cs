namespace KtEquationTreeTest
{
    [TestClass]
    public class InvertTest
    {
        [TestMethod]
        public void InvertLiteralTest()
        {
            var one = new One();
            Assert.AreEqual(one.Invert(), PowerOp.Create(new One(), new NegativeOne()));
            var two = new Two();
            Assert.AreEqual(two.Invert(), PowerOp.Create(new Two(), new NegativeOne()));
        }

        [TestMethod]
        public void InvertIdenterfierTest()
        {
            var a = new Identifier("a");
            Assert.AreEqual(a.Invert(), PowerOp.Create(a, -1));
        }

        [TestMethod]
        public void InvertNegativeValueTest()
        {
            var a = -new Identifier("a");
            Assert.AreEqual(a.Invert(), PowerOp.Create(a, -1));
        }

        [TestMethod]
        public void InvertPowerOpTest()
        {
            var a = new Identifier("a");
            var neg = a.Raise(-new One());
            Assert.AreEqual(neg.Invert(), a);
            var b = new Identifier("b").Raise(new Two());
            Assert.AreEqual(b.Invert(), PowerOp.Create(new Identifier(nameof(b)), -2));
            var negB = new Identifier(nameof(b)).Raise(-new Two());
            Assert.AreEqual(negB.Invert(), PowerOp.Create(new Identifier(nameof(b)), 2));
        }

        [TestMethod]
        public void InvertAddOpTest()
        {
            var aPb = new Identifier("a") + new Identifier("b");
            Assert.AreEqual(aPb.Invert(), PowerOp.Create(aPb, -1));
        }

        [TestMethod]
        public void InvertSubtractOpTest()
        {
            var aNb = new Identifier("a") - new Identifier("b");
            Assert.AreEqual(aNb.Invert(), PowerOp.Create(aNb, -1));
        }

        [TestMethod]
        public void InvertDivisionOpTest()
        {
            var a = new Identifier("a");
            var b = new Identifier("b");
            var actual1 = new One() / a;
            Assert.AreEqual(actual1.Invert(), a);
            var actual2 = DivideOp.Create(-new One(), a);
            Assert.AreEqual(actual2.Invert(), -a);
            var actual3 = DivideOp.Create(a, b);
            Assert.AreEqual(actual3.Invert(), DivideOp.Create(b, a));
            var actual4 = DivideOp.Create(NegativeOp.Create(a), b);
            Assert.AreEqual(actual4.Invert(), DivideOp.Create(-b, a));
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var a = new Identifier("a");
            var b = new Identifier("b");
            Assert.AreEqual((a * b).Invert(), DivideOp.Create(1, a * b));
            Assert.AreEqual(a * b.Invert(), DivideOp.Create(a, b));
            Assert.AreEqual(a.Invert() * b, DivideOp.Create(b, a));
            Assert.AreEqual((a / b).Invert(), DivideOp.Create(b, a));
        }

        [TestMethod]
        public void InvertPowerOfOneTest()
        {
            var a = new Identifier("a^1");
            Assert.AreEqual(a.Invert(), DivideOp.Create(1, a));
        }

        [TestMethod]
        public void InvertPowerOfNegativeOneTest()
        {
            var a = new Identifier("a");
            var negativeOne = new NegativeOne();
            var aPnOne = a.Raise(negativeOne);
            var inv = aPnOne.Invert();
            Assert.AreEqual(inv, DivideOp.Create(1, aPnOne));
            Assert.AreEqual(inv, a);
        }
    }
}