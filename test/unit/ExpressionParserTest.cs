namespace KtEquationTreeTest;

[TestClass]
public class AddTest
{
    [TestMethod]
    public void AddTest1()
    {
        var d = ExprParser.ParseOrThrow("2*x");
        var e = ExprParser.ParseOrThrow("1*x");
        var f = ExprParser.ParseOrThrow("2");
        var h = ExprParser.ParseOrThrow("2");
        Assert.AreEqual(d + e + f + h, AddOp.Create(MultiplyOp.Create(3, new Identifier("x")), 4));
    }
}

[TestClass]
public class ParseTextToExpTest
{
    [TestMethod]
    public void TrailingZerosOnDecimal()
    {
        var f = ExprParser.ParseOrThrow("1.2340000");
        Assert.AreEqual(f, new One() + (Literal.Create(234) / Literal.Create(1000)));
        Assert.AreEqual(f, (Literal.Create(234) / Literal.Create(1000)) + new One());
    }

    [TestMethod]
    public void TrailingZerosOnSciNo()
    {
        var f = ExprParser.ParseOrThrow("1.2340000E4");
        Assert.AreEqual(f, (new One() * Literal.Create(10000)) + Literal.Create(2340));
        Assert.AreEqual(f, Literal.Create(2340) + (Literal.Create(10000) * new One()));
    }

    [TestMethod]
    public void ParseSciNotWithParenTest()
    {
        var f = ExprParser.ParseOrThrow("1.2340000E(4)");
        Assert.AreEqual(f, (new One() * Literal.Create(10000)) + Literal.Create(2340));
        Assert.AreEqual(f, Literal.Create(2340) + (Literal.Create(10000) * new One()));
    }

    [TestMethod]
    public void ParseSciNotWithParenTest2()
    {
        var f = ExprParser.ParseOrThrow("-(1.2340000)E(-14)");
        Assert.AreEqual(f, (new NegativeOne() - (Literal.Create(234) / Literal.Create(1000))) * PowerOp.Create(10, -14));
    }

    [TestMethod]
    public void SciNotSmallE()
    {
        var f = ExprParser.ParseOrThrow("-(1.2340000)e(-14)");
        Assert.AreEqual(f, (new NegativeOne() - (Literal.Create(234) / Literal.Create(1000))) * PowerOp.Create(10, -14));
    }

    [TestMethod]
    public void ParseCallTest()
    {
        var f = ExprParser.ParseOrThrow("f(x)");
        Assert.AreEqual(f, new Call(new Identifier("f"), new Identifier("x")));
    }

    [TestMethod]
    public void BracketsTest()
    {
        var actual = ExprParser.ParseOrThrow("1+[2+y]/{x*(y+4)}");
        var expected = new One() + (AddOp.Create(new Two(), new Identifier("y")) / MultiplyOp.Create(new Identifier("x"), AddOp.Create(4, new Identifier("y"))));
        Assert.AreEqual(expected, actual);
    }
}

[TestClass]
public class IdentifierAndLiteralTest
{
    private static Identifier X => new Identifier("x");
    private static Expr Two => new Two();
    private static Literal One => new One();
    private static Literal Zero => new Zero();

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void DividedByZero()
    {
        _ = X / Zero;
    }

    [TestMethod]
    public void DividedByOne()
    {
        Assert.AreEqual(X / One, X);
    }

    [TestMethod]
    public void RaiseToZero()
    {
        Assert.AreEqual(X.Raise(Zero).Simplify(), One);
    }

    [TestMethod]
    public void RaiseToOne()
    {
        Assert.AreEqual(X.Raise(One).Simplify(), X);
    }

    [TestMethod]
    public void RaiseToANumber()
    {
        Assert.AreEqual(X.Raise(new Two()).Simplify(), PowerOp.Create(X, 2));
    }

    [TestMethod]
    public void MultipliedByZero()
    {
        Assert.AreEqual(X * Zero, Zero);
        Assert.AreEqual(Zero * X, Zero);
    }

    [TestMethod]
    public void MultipliedBy1()
    {
        Assert.AreEqual(X * One, X);
        Assert.AreEqual(One * X, X);
    }

    [TestMethod]
    public void MultipliedByAnyNumber()
    {
        Assert.AreEqual(X * Two, MultiplyOp.Create(2, X));
        Assert.AreEqual(Two * X, MultiplyOp.Create(2, X));
    }

    [TestMethod]
    public void DivideByAnyNumber()
    {
        Assert.AreEqual(X / Two, DivideOp.Create(X, 2));
        Assert.AreEqual(Two / X, DivideOp.Create(2, X));
    }

    [TestMethod]
    public void AddSubtractToItself()
    {
        Assert.AreEqual(X + X, MultiplyOp.Create(2, X));
        Assert.AreEqual(X - X, Zero);
    }

    [TestMethod]
    public void MultiplyWithZero()
    {
        Assert.AreEqual(X * Zero, new Zero());
        Assert.AreEqual(Zero * X, new Zero());
    }

    [TestMethod]
    public void MultiplyWithOne()
    {
        Assert.AreEqual(X * One, new Identifier("x"));
        Assert.AreEqual(One * X, new Identifier("x"));
    }

    [TestMethod]
    public void AddToZero()
    {
        Assert.AreEqual(X + Zero, new Identifier("x"));
        Assert.AreEqual(Zero + X, new Identifier("x"));
    }

    [TestMethod]
    public void SubtractedByZero()
    {
        Assert.AreEqual(X - Zero, new Identifier("x"));
        Assert.AreEqual(Zero - X, -new Identifier("x"));
    }

    [TestMethod]
    public void AddToLiteralTest()
    {
        var actual1 = AddOp.Create(new Identifier("x"), 1);
        var actual2 = AddOp.Create(1, new Identifier("x"));
        Assert.AreEqual(X + One, actual1);
        Assert.AreEqual(One + X, actual2);
    }

    [TestMethod]
    public void AddToNegativeLiteralTest()
    {
        var actual1 = SubtractOp.Create(new Identifier("x"), 1);
        var actual2 = SubtractOp.Create(1, new Identifier("x"));
        Assert.AreEqual(X + new NegativeOne(), actual1);
        Assert.AreEqual(One + NegativeOp.Create(X), actual2);
    }

    [TestMethod]
    public void SubtractToLiteralTest()
    {
        var actual1 = SubtractOp.Create(new Identifier("x"), 1);
        var actual2 = SubtractOp.Create(1, new Identifier("x"));
        var testing = One - X;
        Assert.AreEqual(X - One, actual1);
        Assert.AreEqual(testing, actual2);
    }

    [TestMethod]
    public void SubtractToNegativeLiteralTest()
    {
        var num = new One();
        var actual1 = AddOp.Create(new Identifier("x"), 1);
        var actual2 = AddOp.Create(1, new Identifier("x"));
        Assert.AreEqual(X - NegativeOp.Create(num), actual1);
        Assert.AreEqual(num - NegativeOp.Create(X), actual2);
    }
}

[TestClass]
public class IdentifierIdentifierTest
{
    private readonly Identifier i1 = new Identifier("x");
    private readonly Identifier i2 = new Identifier("x");
    private readonly Identifier i3 = new Identifier("y");

    [TestMethod]
    public void DivideToAny()
    {
        Assert.AreEqual(i1 / i3, DivideOp.Create(i1, i3));
    }

    [TestMethod]
    public void Power()
    {
        Assert.AreEqual(i1.Raise(i3), PowerOp.Create(i1, i3));
    }

    [TestMethod]
    public void DivideByItself()
    {
        var actual = i2 / i2;
        Assert.AreEqual(actual, new One());
    }

    [TestMethod]
    public void MutiplyToSelf()
    {
        Assert.AreEqual(i1 * i1, PowerOp.Create(i1, 2));
    }

    [TestMethod]
    public void MultiplyToOther()
    {
        Assert.AreEqual(i1 * i3, MultiplyOp.Create(i1, i3));
        Assert.AreEqual(i3 * i1, MultiplyOp.Create(i3, i1));
    }

    [TestMethod]
    public void AddToIdentifierSame()
    {
        Assert.AreEqual(i1 + i2, MultiplyOp.Create(2, new Identifier("x")));
        Assert.AreEqual(i2 + i1, MultiplyOp.Create(2, new Identifier("x")));
    }

    [TestMethod]
    public void AddToIdentifierDifferent()
    {
        Assert.AreEqual(i1 + i3, AddOp.Create(new Identifier("x"), new Identifier("y")));
        Assert.AreEqual(i3 + i1, AddOp.Create(new Identifier("y"), new Identifier("x")));
    }

    [TestMethod]
    public void SubtractToIdentifierSame()
    {
        Assert.AreEqual(i1 - i2, new Zero());
        Assert.AreEqual(i2 - i1, new Zero());
    }

    [TestMethod]
    public void SubtractToIdentifierDifferent()
    {
        Assert.AreEqual(i1 - i3, SubtractOp.Create(new Identifier("x"), new Identifier("y")));
        Assert.AreEqual(i3 - i1, SubtractOp.Create(new Identifier("y"), new Identifier("x")));
    }
}

[TestClass]
public class ExpressionParserTest
{
    public static Expr A => new Identifier("a");
    public static Expr D => new Identifier("d");
    public static Expr De => new Identifier("de");
    public static Expr Two => new Two();
    public static Expr One => new One();

    [TestMethod]
    public void PowerOpTest()
    {
        var actual = PowerOp.Create(Two, D);
        Assert.AreEqual(ExprParser.ParseOrThrow("2^d"), actual);
        Assert.AreEqual(Two.Raise(D), actual);
    }

    [TestMethod]
    public void RaisedToNegativeTest()
    {
        var d = -De;
        var actual = PowerOp.Create(Two, d);
        Assert.AreEqual(ExprParser.ParseOrThrow("2 ^-de"), actual);
        var inf = Two.Raise(d).InnerFactor();
        Console.WriteLine(Two.Raise(d).InnerFactor().Select(f => f.ToExpr()).BuildString("\n"));
        Console.WriteLine("\n" + actual.Simplify().InnerFactor().Select(f => f.ToExpr()).BuildString("\n"));
        Assert.AreEqual(-De, -De.Simplify());
        Assert.AreEqual(Two.Raise(d), actual.Simplify());
    }

    [TestMethod]
    public void RaisedToNumberThenAddTest()
    {
        var actual = AddOp.Create(PowerOp.Create(A, One), Two);
        Assert.AreEqual(ExprParser.ParseOrThrow("a^1+2"), actual);
        Assert.AreEqual(A.Raise(One) + Two, actual.Simplify());
    }

    [TestMethod]
    public void DecimalTest()
    {
        var onePthree = ExprParser.ParseOrThrow("1.3");
        Assert.AreEqual(onePthree, DivideOp.Create(13, 10));
        var actual = AddOp.Create(PowerOp.Create(A, onePthree), Two);
        Assert.AreEqual(ExprParser.ParseOrThrow("a^1.3+2"), actual);
        Assert.AreEqual(A.Raise(onePthree) + Two, actual.Simplify());
    }

    [TestMethod]
    public void FractionRaisedToNegative()
    {
        var expected = ExprParser.ParseOrThrow("(a/b)^-1");
        var actual = DivideOp.Create(new Identifier("b"), new Identifier("a"));
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void A_RaiseToQuantity1Plus2Test()
    {
        var actual = new Identifier("a").Raise(new One() + new Two()).Simplify();
        var expected = ExprParser.ParseOrThrow("a^(1+2)").Simplify();
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void TwoMinus_a_PowerOf1Test()
    {
        var actual = SubtractOp.Create(new Two(), PowerOp.Create(new Identifier("a"), new One()));
        var expected = ExprParser.ParseOrThrow("2-a^1");
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void QuantityTwoMinus_a_raisedTo1Test()
    {
        var actual = PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), new One()).Simplify();
        var expected = ExprParser.ParseOrThrow("(2-a)^1").Simplify();
        Assert.AreEqual(expected, actual.Simplify());
    }

    [TestMethod]
    public void QuantityTwoMinus_a_raisedTo1DividedBy3Test()
    {
        var actual = DivideOp.Create(PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), new One()), Literal.Create(3)).Simplify();
        var expected = ExprParser.ParseOrThrow("(2-a)^1/3").Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void QuantityTwoMinus_a_raisedToOneThirdTest()
    {
        const string input = "(2-a)^(1/3)";
        var actual = PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), DivideOp.Create(new One(), Literal.Create(3))).Simplify();
        var expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void QuantityTwoMinus_a_raisedTo1Plus3Test()
    {
        const string input = "(2-a)^1+3";
        var actual = AddOp.Create(PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), new One()), Literal.Create(3)).Simplify();
        var expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test1()
    {
        const string input = "-(2-a)^1+3";
        var actual = AddOp.Create(NegativeOp.Create(PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), new One())), Literal.Create(3)).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void Test2()
    {
        const string input = "-a/b^1-3+a/b^1";
        var term1 = DivideOp.Create(NegativeOp.Create(new Identifier("a")), PowerOp.Create(new Identifier("b"), new One()));
        var term2 = Literal.Create(3);
        var term3 = DivideOp.Create(new Identifier("a"), PowerOp.Create(new Identifier("b"), new One()));
        var actual = AddOp.Create(SubtractOp.Create(term1, term2), term3).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void Test3()
    {
        const string input = "-a/b^1+3-a/b^1";
        var term1 = DivideOp.Create(NegativeOp.Create(new Identifier("a")), PowerOp.Create(new Identifier("b"), new One()));
        var term2 = Literal.Create(3);
        var term3 = DivideOp.Create(new Identifier("a"), PowerOp.Create(new Identifier("b"), new One()));
        var actual = SubtractOp.Create(AddOp.Create(term1, term2), term3).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void Test4()
    {
        const string input = "1-2/3+1-3+3*2";
        var a = new One();
        var b = DivideOp.Create(new Two(), Literal.Create(3));
        var c = new One();
        var d = Literal.Create(3);
        var e = MultiplyOp.Create(Literal.Create(3), new Two());
        var actual = AddOp.Create(SubtractOp.Create(AddOp.Create(SubtractOp.Create(a, b), c), d), e).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void Test5()
    {
        const string input = "a*b/c";
        var actual = DivideOp.Create(MultiplyOp.Create(new Identifier("a"), new Identifier("b")), new Identifier("c")).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void QuantityTwoMinus_a_raisedToQuantity1Plus3Test()
    {
        const string input = "(2-a)^(1+3)";
        var actual = PowerOp.Create(SubtractOp.Create(new Two(), new Identifier("a")), AddOp.Create(new One(), Literal.Create(3))).Simplify();
        var expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FunctionMethodNegateParamTest()
    {
        const string input = "foo(-3, x)()";
        var actual =
                new Call(
                    new Call(
                        new Identifier("foo"),
                            NegativeOp.Create(Literal.Create(3)),
                            new Identifier("x")
                    )).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SubtractBeforeMultTest()
    {
        const string input = "d-12 * 3";
        var actual = SubtractOp.Create(
            new Identifier("d"),
            MultiplyOp.Create(Literal.Create(12), Literal.Create(3))
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SubtractThenMultTest()
    {
        var input = "(d-12) * 3";
        var actual = MultiplyOp.Create(
            SubtractOp.Create(new Identifier("d"), Literal.Create(12)),
            Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.ToString(), "3*(d-12)");
        Assert.AreEqual(expected.Simplify(), actual);
        input = "(d+12) * 3";
        actual = MultiplyOp.Create(
            AddOp.Create(new Identifier("d"), Literal.Create(12)),
            Literal.Create(3)
            ).Simplify();
        expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ExprTest()
    {
        const string input = "12 * 3 + foo(-3, x)() * (2 + 1)";
        var actual = AddOp.Create(
            MultiplyOp.Create(
                Literal.Create(12),
                Literal.Create(3)
            ),
            MultiplyOp.Create(
                new Call(
                    new Call(
                        new Identifier("foo"),
                            NegativeOp.Create(Literal.Create(3)),
                            new Identifier("x")
                    )
                ),
                AddOp.Create(
                    new Two(),
                    new One()
                )
            )
        ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void NegativeBeforeAddTest()
    {
        const string input = "-12 + 3";
        var actual = AddOp.Create(
            NegativeOp.Create(Literal.Create(12)),
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void NegativeBeforeSubtractTest()
    {
        const string input = "-12 - 3";
        var actual = SubtractOp.Create(
            NegativeOp.Create(Literal.Create(12)),
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input).Simplify();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void NegativeBeforeMultiplyTest()
    {
        const string input = "-12*3";
        var actual = MultiplyOp.Create(
            NegativeOp.Create(Literal.Create(12)),
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void EqualityTest()
    {
        const string input = "12 * 3";
        var actual = MultiplyOp.Create(
                Literal.Create(12),
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void SubtractionTest()
    {
        const string input = "12 - 3";
        var actual = SubtractOp.Create(
                Literal.Create(12),
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected.Simplify(), actual);
    }

    [TestMethod]
    public void NegateTest()
    {
        const string input = "- 3";
        var actual = NegativeOp.Create(
                Literal.Create(3)
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void NegateDivideTest()
    {
        const string input = "- (1/3)";
        var actual = NegativeOp.Create(
                DivideOp.Create(new One(), Literal.Create(3))
            ).Simplify();
        var expected = ExprParser.ParseOrThrow(input);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test6()
    {
        const string input = "(1-a*(3-4)*x/21)^3-(1/2)^5+11-(16.31^3-a)*r*f(x)";
        var ex2 = ExprParser.ParseOrThrow("16.31^3");
        ex2 = ex2.Simplify();
        Assert.AreEqual(ex2, DivideOp.Create(4338722591, 1000000));
        var a = new Identifier("a");
        var one = new One();
        var three = Literal.Create(3);
        var four = Literal.Create(4);
        var x = new Identifier("x");
        var twentyone = Literal.Create(21);
        var two = new Two();
        var five = Literal.Create(5);
        var eleven = Literal.Create(11);
        var sixteenPthirtyone = ExprParser.ParseOrThrow("16.31");
        var r = new Identifier("r");
        var fOfx = new Call(new Identifier("f"), x);
        var expected = ExprParser.ParseOrThrow(input);
        var actual = (one - (a * (three - four) * x / twentyone)).Raise(three) - (one / two).Raise(five) + eleven - ((sixteenPthirtyone.Raise(three) - a) * r * fOfx).Simplify();
        Assert.AreEqual(expected.Simplify(), actual);
        Assert.AreEqual((a * r * fOfx) - (Literal.Create(4338722591) * r * fOfx / Literal.Create(1000000)) + (a.Raise(three) * x.Raise(three) / Literal.Create(9261)) + (a.Raise(two) * x.Raise(two) / Literal.Create(147)) + (a * x / Literal.Create(7)) + (Literal.Create(383) / Literal.Create(32)), actual);
    }

    [TestMethod]
    public void NegativeFractionRootOfFive()
    {
        var d = ExprParser.ParseOrThrow("-(1/2)^5").Simplify();
        Assert.AreEqual(d, NegativeOp.Create(DivideOp.Create(1, 32)));
    }

    [TestMethod]
    public void TwoxRaiseTo3()
    {
        var d = ExprParser.ParseOrThrow("(2*x)^3").Simplify();
        Assert.AreEqual(d, MultiplyOp.Create(8, PowerOp.Create(new Identifier("x"), 3)));
    }

    [TestMethod]
    public void TwoXRaiseTox()
    {
        var x = new Identifier("x");
        Assert.AreEqual(MultiplyOp.Create(2, x).Raise(x), MultiplyOp.Create(PowerOp.Create(2, x), PowerOp.Create(x, x)));
    }

    [TestMethod]
    public void TwoXRaiseToTwoX()
    {
        var x = new Identifier("x");
        var TwoX = MultiplyOp.Create(2, x);
        var test = TwoX.Raise(TwoX);
        Assert.IsTrue(test is MultiplyOp m && m.Left == PowerOp.Create(4, x));
        Assert.AreEqual(test, MultiplyOp.Create(PowerOp.Create(4, x), PowerOp.Create(x, TwoX)));
    }

    [TestMethod]
    public void SqrtOf8()
    {
        var sqrtOf8 = SqrtOp.Create(Literal.Create(8)).Simplify();
        var two = new Two();
        var three = Literal.Create(3);
        var actual = two.Raise(three / two);
        Assert.AreEqual(sqrtOf8, actual);
    }

    [TestMethod]
    public void SqrtN16()
    {
        var NegSixteen = NegativeOp.Create(Literal.Create(16));
        var sqrt = NegSixteen.Sqrt();
        var four = Literal.Create(4);
        var sqrtOfNegOne = SqrtOp.Create(-1).Simplify();
        var expected = four * sqrtOfNegOne;
        Assert.AreEqual(sqrt, expected);
    }

    [TestMethod]
    public void FractionRaiseToFraction()
    {
        var twentyFive = Literal.Create(25);
        var thirtyTwo = Literal.Create(32);
        var oneTenth = DivideOp.Create(1, 10);
        var result = (thirtyTwo / twentyFive).Raise(oneTenth);
        var actual = SqrtOp.Create(2) / PowerOp.Create(5, DivideOp.Create(1, 5));
        Assert.AreEqual(result, actual);
    }

    [TestMethod]
    public void XRaiseToXTimesXRaiseTo2()
    {
        var x = new Identifier("x");
        var xToX = x.Raise(x);
        var xTo2 = x.Raise(new Two());
        var expected = x.Raise(AddOp.Create(2, x));
        Assert.AreEqual(xToX * xTo2, expected);
    }

    [TestMethod]
    public void SqrtOf4()
    {
        var sqrt4 = Literal.Create(4).Raise(DivideOp.Create(1, 2)).Simplify();
        Assert.AreEqual(sqrt4, new Two());
    }

    [TestMethod]
    public void SixTo15()
    {
        var sixTo15 = DivideOp.Create(6, 15).Simplify();
        Assert.AreEqual(sixTo15, DivideOp.Create(2, 5));
    }
}

[TestClass]
public class ExprDivisionTests
{
    public static Expr A => new Identifier("a");
    public static Expr D => new Identifier("d");
    public static Expr De => new Identifier("de");
    public static Expr Two => new Two();
    public static Expr One => new One();
    public static Expr Zero => new Zero();

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void ExprDivisionByNull()
    {
        var _ = A / null;
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void ExprDivisionByZero()
    {
        var _ = A / Zero;
    }

    [TestMethod]
    public void ExprDivisionByOne()
    {
        var result = A / One;
        Assert.AreEqual(A, result);
    }
}

[TestClass]
public class ExprNegateTests
{
    public static Expr A => new Identifier("a");
    public static Expr D => new Identifier("d");
    public static Expr De => new Identifier("de");
    public static Expr Two => new Two();
    public static Expr One => new One();
    public static Expr Zero => new Zero();

    [TestMethod]
    public void ExprNegateNullTest()
    {
        Expr B = null;
        Assert.IsNull(-B);
    }

    [TestMethod]
    public void ExprNegateZeroTest()
    {
        Assert.AreEqual(-Zero, Zero);
    }
}