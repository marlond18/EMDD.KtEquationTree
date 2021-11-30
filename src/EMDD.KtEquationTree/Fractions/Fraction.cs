using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using Parser.Expression;

namespace EMDD.KtEquationTree.Fractions;
public struct Fraction
{
    public BigInteger num;
    public BigInteger den;

    public Fraction(BigInteger num, BigInteger den)
    {
        this.num = num;
        this.den = den;
    }

    public override bool Equals(object obj)
    {
        return obj is Fraction other &&
               num.Equals(other.num) &&
               den.Equals(other.den);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(num, den);
    }

    public void Deconstruct(out BigInteger num, out BigInteger den)
    {
        num = this.num;
        den = this.den;
    }

    public static implicit operator (BigInteger num, BigInteger den)(Fraction value)
    {
        return (value.num, value.den);
    }

    public static implicit operator Fraction((BigInteger num, BigInteger den) value)
    {
        return new Fraction(value.num, value.den);
    }

    public static Fraction operator *(Fraction a, Fraction b) =>
        new Fraction(a.num * b.num, a.den * b.den).Reduce();

    public static Fraction operator *(Fraction a, BigInteger b) =>
        new Fraction(a.num * b, a.den).Reduce();

    public static Fraction operator /(Fraction a, Fraction b) =>
        new Fraction(a.num * b.den, a.den * b.num).Reduce();

    public static Fraction operator /(BigInteger a, Fraction b) =>
        new Fraction(a * b.den, b.num).Reduce();

    public static Fraction operator /(Fraction a, BigInteger b) =>
        new Fraction(a.num, a.den * b).Reduce();

    public static Fraction operator -(Fraction a) =>
        new Fraction(-a.num, a.den).Reduce();

    public static Fraction operator +(Fraction arg1, Fraction arg2) =>
        new Fraction(arg1.num * arg2.den + arg2.num * arg1.den, arg1.den * arg2.den).Reduce();

    public static Fraction operator +(Fraction arg1, BigInteger arg2) =>
        new Fraction(arg1.num + arg2 * arg1.den, arg1.den).Reduce();

    public static Fraction operator +(BigInteger arg2, Fraction arg1) =>
        new Fraction(arg1.num + arg2 * arg1.den, arg1.den).Reduce();

    public static Fraction Subtract(Fraction arg1, Fraction arg2) =>
        new Fraction(arg1.num * arg2.den - arg2.num * arg1.den, arg1.den * arg2.den).Reduce();

    public override string ToString() => $"{num.ToString("R")}/{den.ToString("R")}";

    public Fraction Reduce() =>
        DivBoth(this, BigInteger.GreatestCommonDivisor(num, den));

    private static Fraction DivBoth(Fraction number, BigInteger div) =>
        (number.num / div, number.den / div);

    internal TermBase ToTerm() => TermConstant.Create(num, den);

    public Fraction Raise(BigInteger p) => num.Raise(p) / den.Raise(p);

    public Expr ToExprDiv() => this switch
    {
        (_, { IsOne: true }) => num.ToExpr(),
        (_, { IsZero: true }) => throw new DivideByZeroException($"{num.ToString("R")} cannot be divided by zero"),
        _ when num < 0 => NegativeOp.Create(DivideOp.Create(BigInteger.Abs(num), den)),
        _ => DivideOp.Create(num, den)
    };

    public Expr ToExpr() => this switch
    {
        (_, { IsOne: true }) => num.ToExpr(),
        ({ IsZero: true }, _) => new Zero(),
        _ when num < 0 => NegativeOp.Create(DivideOp.Create(BigInteger.Abs(num), den)),
        _ => DivideOp.Create(num, den)
    };
}