using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;

using Parser.Expression;
using Parser.Expression.Var;
using EMDD.KtEquationTree.MathStatements;

namespace EMDD.KtEquationTree.Exprs.Unary;
public sealed class Call : UnaryOp
{
    public ImmutableArray<Expr> Arguments { get; }

    private Call(Expr expr, params Expr[] arguments) : base(expr)
    {
        Arguments = arguments.ToImmutableArray();
        IsSimple = true;
    }

    private Call(Expr expr, IEnumerable<Expr> arguments) : base(expr)
    {
        Arguments = arguments == default ?
            throw new ArgumentNullException(nameof(arguments)) : arguments.ToImmutableArray();
        IsSimple = true;
    }

    public static Expr Create(Expr expr, params Expr[] arguments)
    {
        if (arguments == default) throw new ArgumentNullException(nameof(arguments));
        return new Call(expr, arguments);
    }

    public static Expr Create(Expr expr, IEnumerable<Expr> arguments)
    {
        if (arguments == default) throw new ArgumentNullException(nameof(arguments));
        return new Call(expr, arguments);
    }

    public static MathStatement Create(MathStatement expr, params MathStatement[] arguments)
    {
        if (expr is Expr e && arguments.All(aa => aa is Expr))
        {
            if (arguments == default) throw new ArgumentNullException(nameof(arguments));
            return new Call(e, arguments.Where(aa=> aa is Expr).Select(aa=>aa as Expr));
        }
        throw new NotImplementedException();
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

    internal override FactorsBase InnerFactor() => FactorsSingle.Create(FactorSingleN.Create(this));

    public override bool TryToDouble(out double value)
    {
        value = 0;
        return false;
    }

    public override Expr Substitute(Expr current, Expr replacement)
    {
        if (Expr == current) return new Call(replacement, Arguments.ReplaceAll(current, replacement));
        return Call.Create(Expr.Substitute(current, replacement), Arguments.ReplaceAll(current, replacement));
    }
}