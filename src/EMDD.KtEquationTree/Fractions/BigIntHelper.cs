using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtEquationTree.Fractions;
public static class BigIntHelper
{
    internal static Fraction ToFraction(this decimal value, decimal accuracy)
    {
        int sign = value < 0 ? -1 : 1;
        value = value < 0 ? -value : value;
        int integerpart = (int)value;
        value -= integerpart;
        var minimalvalue = value - accuracy;
        if (minimalvalue < 0.0m) return new Fraction(sign * integerpart, 1);
        var maximumvalue = value + accuracy;
        if (maximumvalue > 1.0m) return new Fraction(sign * (integerpart + 1), 1);
        int a = 0;
        int b = 1;
        int c = 1;
        int d = (int)(1 / maximumvalue);
        while (true)
        {
            int n = (int)((b * minimalvalue - a) / (c - d * minimalvalue));
            if (n == 0) break;
            a += n * c;
            b += n * d;
            n = (int)((c - d * maximumvalue) / (b * maximumvalue - a));
            if (n == 0) break;
            c += n * a;
            d += n * b;
        }
        int denominator = b + d;
        return new Fraction(sign * (integerpart * denominator + (a + c)), denominator);
    }

    public static Expr ToExpr(this BigInteger i)
    {
        if (i < 0) return NegativeOp.Create(Literal.Create(BigInteger.Abs(i)));
        return Literal.Create(i);
    }

    public static Fraction Raise(this BigInteger a, BigInteger b)
    {
        if (a == 1 || b == 0) return (1, 1);
        if (a == 0) return (0, 1);
        if (b == 1) return (a, 1);
        if (BigInteger.Abs(b) > int.MaxValue) throw new OverflowException($"cannot do this {a}^{b}");
        if (b < 0) return (1, BigInteger.Pow(a, (int)BigInteger.Abs(b)));
        return (BigInteger.Pow(a, (int)b), 1);
    }

    public static IEnumerable<(BigInteger b, Fraction e)> Raise(this BigInteger b, Fraction e) =>
        b.Factor().ToLookup(ff => ff).Select(g => (b: g.Key, e: g.Count())).Select(f => (f.b, e: new Fraction(f.e * e.num, e.den).Reduce()));

    public static bool ExceedsIntMax(this BigInteger i) => BigInteger.Abs(i) > int.MaxValue;

    internal static TermBase ConvertBigIntRaiseToTerm(this IEnumerable<(BigInteger b, Fraction e)> r) =>
        CreateFromFork(r.Fork(f => f.e.den == 1));

    private static TermBase CreateFromFork((IEnumerable<(BigInteger b, Fraction e)> matches, IEnumerable<(BigInteger b, Fraction e)> nonMatches) newF)
    {
        var constant = newF.matches.Aggregate(ConstantWhole.Create(1), (total, f) => total * ConstantFraction.Create(f.b.Raise(f.e.num)));
        var variables = Variables.Create(newF.nonMatches.WhereNot(f => f.b == 1).Select(f => VarRootN.Create(f.b.Raise(f.e.num).ToExprDiv(), f.e.den)));
        return Term.Create(constant, variables);
    }
}