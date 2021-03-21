using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Parsers;
using EMDD.KtEquationTree.Exprs.Singles;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KtEquationTreeTest
{
    [TestClass]
    public class ExprEqualityTest
    {
        [TestMethod]
        public void OnePlusZeroEqualsOne()
        {
            var one = new One();
            var zero = new Zero();
            var onepluszero = one + zero;
            Assert.AreEqual(one, onepluszero);
        }

        [TestMethod]
        public void OnePlusZeroEqualsOneV2()
        {
            var one = new One();
            var onepluszero = ExprParser.ParseOrThrow("1+0");
            Assert.AreEqual(one, onepluszero);
        }

        [TestMethod]
        public void IPlusZeroEqualsOneV2()
        {
            var one = new Identifier("i");
            var onepluszero = ExprParser.ParseOrThrow("i+0");
            Assert.AreEqual(one, onepluszero);
        }

        [TestMethod]
        public void MixedEqualityTest()
        {
            var expected = ExprParser.ParseOrThrow("2*(x^2-2)");
            var actual = ExprParser.ParseOrThrow("-4+2*x^2");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ArrangementEqualityTest()
        {
            var s = SqrtOp.Create(2);
            var d = new One();
            Assert.AreEqual(s + d, d + s);
            Assert.AreEqual(s * d, d * s);
        }

        [TestMethod]
        public void MultOpArrangementEqualityTest()
        {
            var a = new Identifier("a");
            var b = new Identifier("b");
            var c = new Identifier("c");
            var d = new Identifier("d");
            var e = new Identifier("e");
            var a1 = a * b * c * d * e;
            var a2 = e * b * c * d * a;
            var a3 = c * d * a * b * e;
            var a4 = d * e * c * b * a;
            var a5 = d * e * a * c * b;
            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a1, a3);
            Assert.AreEqual(a1, a4);
            Assert.AreEqual(a1, a5);
            Assert.AreEqual(a2, a3);
            Assert.AreEqual(a2, a4);
            Assert.AreEqual(a2, a5);
            Assert.AreEqual(a3, a4);
            Assert.AreEqual(a3, a5);
            Assert.AreEqual(a4, a5);
        }

        [TestMethod]
        public void AddOpArrangementEqualityTest()
        {
            var a = new Identifier("a");
            var b = new Identifier("b");
            var c = new Identifier("c");
            var d = new Identifier("d");
            var e = new Identifier("e");
            var a1 = a + b + c + d + e;
            var a2 = e + b + c + d + a;
            var a3 = c + d + a + b + e;
            var a4 = d + e + c + b + a;
            var a5 = d + e + a + c + b;
            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a1, a3);
            Assert.AreEqual(a1, a4);
            Assert.AreEqual(a1, a5);
            Assert.AreEqual(a2, a3);
            Assert.AreEqual(a2, a4);
            Assert.AreEqual(a2, a5);
            Assert.AreEqual(a3, a4);
            Assert.AreEqual(a3, a5);
            Assert.AreEqual(a4, a5);
        }
    }
}