using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Factors;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtMathStatement.Exprs.Singles;
public class Identifier : Single
{
    public Identifier(string definition)
    {
        Definition = definition;
        IsSimple = true;
    }

    public string Definition { get; }

    public override int GetHashCode() => unchecked(539060726.ChainHashCode(Definition));

    public override string ToString() => Definition;

    public override bool Equals(Expr other) =>
        ReferenceEquals(this, other) ? true :
        this is null ? false :
        other is null ? false : other switch
        {
            { IsSimple: false } => Equals(other.Simplify()),
            Identifier l => Definition.Equals(l.Definition, StringComparison.Ordinal),
            _ => false
        };

    internal override TermsBase Terms() => TermsSingle.Create(TermVariables.Create(VarN.Create(this)));

    public static bool operator ==(Identifier a, Identifier b) => a.DefaultEquals(b);

    public static bool operator !=(Identifier a, Identifier b) => !(a == b);

    public override Expr Simplify() => this;

    public override IEnumerable<Expr> Factor() => new[] { this };

    internal override FactorsBase InnerFactor() => FactorsSingle.Create(FactorSingleN.Create(this));

    public override bool TryToDouble(out double value)
    {
        value = 0;
        return false;
    }

    public override Expr Subtitute(Expr current, Expr replacement)
    {
        return this == current ? replacement : this;
    }
}