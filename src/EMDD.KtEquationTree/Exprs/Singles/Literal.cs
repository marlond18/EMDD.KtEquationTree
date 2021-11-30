using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.Fractions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Singles;
public class One : Literal
{
    public One() : base(1)
    {

    }
}

public class Zero : Literal
{
    public Zero() : base(0)
    {
    }
}

public class Two : Literal
{
    public Two() : base(2)
    {
    }
}

public class Literal : Single
{
    protected Literal(BigInteger value)
    {
        IsSimple = true;
        Value = value;
    }

    public static Expr Create(BigInteger value)
    {
        if (value == 0) return new Zero();
        if (value == 1) return new One();
        if (value < 0) NegativeOp.Create(Create(-value));
        return new Literal(value);
    }

    public bool IsZero => Value == 0;

    public override bool Equals(Expr other) =>
        ReferenceEquals(this, other) ? true :
        this is null ? false :
        other is null ? false :
        other switch
        {
            { IsSimple: false } => Equals(other.Simplify()),
            Literal l => Value == l.Value,
            _ => false
        };

    public BigInteger Value { get; }

    public override int GetHashCode() => unchecked(739060726.ChainHashCode(Value));

    public override string ToString() => Value.ToString("R");

    internal override TermsBase Terms() => TermsSingle.Create(TermConstant.Create(Value));

    public static bool operator ==(Literal a, Literal b) => a.DefaultEquals(b);

    public static bool operator !=(Literal a, Literal b) => !(a == b);

    public double ToDouble() => (double)Value;

    public override Expr Simplify() => this;

    public static Expr operator +(Literal l1, Literal l2) => Create(l1.Value + l2.Value);

    public override IEnumerable<Expr> Factor()
        => FactorAndGroup()
        .Select(g => (b: g.Key, e: g.Count()))
        .Select(pair => pair.e > 1 ? PowerOp.Create(pair.b, pair.e) : Create(pair.b));

    internal override FactorsBase InnerFactor() =>
        Factors.Factors.Create(FactorAndGroup().Select(g => (b: g.Key, e: (BigInteger)g.Count())).Select(CreateFactorFromPair));

    private ILookup<BigInteger, BigInteger> FactorAndGroup()
        => Value.Factor().ToLookup(f => f);

    private FactorBase CreateFactorFromPair((BigInteger b, BigInteger e) pair)
    {
        if (pair.e > 1) return FactorPowN.Create(pair.b.ToExpr(), pair.e.ToExpr());
        return FactorSingleN.Create(pair.b.ToExpr());
    }

    public override bool TryToDouble(out double value)
    {
        value = ToDouble();
        return true;
    }

    public override Expr Subtitute(Expr current, Expr replacement) => this;
}