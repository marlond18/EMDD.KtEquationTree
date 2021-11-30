using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.Fractions;
using EMDD.KtEquationTree.Parsers;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Singles;
public class Dec : Single
{
    protected Dec(decimal value)
    {
        Value = value;
        _rational = value.ToFraction(0.0000001m).ToExpr();
        IsSimple = true;
    }

    public decimal Value { get; }

    private readonly Expr _rational;

    public static Expr Create(decimal value)
    {
        if (value == 0) return new Zero();
        if (value == 1) return new One();
        if (value < 0) NegativeOp.Create(Create(-value));
        return new Dec(value);
    }

    public bool IsZero => Value == 0;

    public override int GetHashCode() => unchecked(HashCode.Combine(typeof(Dec), Value));

    public override bool Equals(Expr other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (this is null || other is null) return false;
        return _rational.Equals(other);
    }

    public Expr Rationalize()
    {
        return _rational;
    }

    public override IEnumerable<Expr> Factor()
    {
        return _rational.Factor();
    }

    public override Expr Simplify()
    {
        return _rational.Simplify();
    }

    public override Expr Subtitute(Expr current, Expr replacement)
    {
        return this;
    }

    public override bool TryToDouble(out double value)
    {
        value = (double)Value;
        return true;
    }

    internal override FactorsBase InnerFactor()
    {
        return _rational.InnerFactor();
    }

    public override string ToString() => $"{Value}";

    internal override TermsBase Terms()
    {
        return _rational.Terms();
    }
}

