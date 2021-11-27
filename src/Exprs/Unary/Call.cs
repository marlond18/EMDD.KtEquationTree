using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtEquationTree.Exprs.Unary;
public class Call : UnaryOp
{
    public ImmutableArray<Expr> Arguments { get; }

    public Call(Expr expr, params Expr[] arguments) : base(expr)
    {
        Arguments = arguments == default ?
            throw new ArgumentNullException(nameof(arguments)) : arguments.ToImmutableArray();
        IsSimple = true;
    }

    public Call(Expr expr, IEnumerable<Expr> arguments) : base(expr)
    {
        Arguments = arguments == default ?
            throw new ArgumentNullException(nameof(arguments)) : arguments.ToImmutableArray();
        IsSimple = true;
    }


    protected override bool Equals(UnaryOp other) => Equals(other.Simplify() as Call);

    private bool Equals(Call other) => other switch
    {
        { IsSimple: false } => Equals(other.Simplify()),
        _ => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr) && Arguments.ContentEquals(other.Arguments))
    };

    public override int GetHashCode() =>
        HashCode.Combine(Expr, Arguments.GetHashCodeOfEnumerable());

    public override bool Equals(object obj) => Equals(obj as Expr);

    public override string ToString() => $"{Expr}({Arguments.BuildString(", ")})";

    internal override TermsBase Terms() => TermsSingle.Create(TermVariables.Create(VarN.Create(this)));

    public override Expr Simplify() => this;

    public override IEnumerable<Expr> Factor() => new[] { this };

    public override Expr Invert() => PowerOp.Create(this, -new One());

    public override FactorsBase InnerFactor() => FactorsSingle.Create(FactorSingleN.Create(this));

    public override bool TryToDouble(out double value)
    {
        value = 0;
        return false;
    }

    public override Expr Subtitute(Expr current, Expr replacement)
    {
        if (Expr == current) return new Call(replacement, Arguments.ReplaceAll(current, replacement));
        return new Call(Expr.Subtitute(current, replacement), Arguments.ReplaceAll(current, replacement));
    }
}