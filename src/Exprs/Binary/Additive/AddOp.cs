using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Additive;
public class AddOp : AdditiveBinaryOp
{
    protected AddOp(Expr left, Expr right) : base(left, right)
    {
    }

    public static Expr Create(Expr left, Expr right) => (left, right) switch
    {
        (Zero _, _) => right,
        (_, Zero _) => left,
        (_, NegativeOp { Expr: Literal l }) => SubtractOp.Create(left, l),
        _ => new AddOp(left, right)
    };

    public static Expr Create(BigInteger left, Expr right) => (left, right) switch
    {
        ({ IsZero: true }, _) => right,
        (_, Zero _) => Literal.Create(left),
        (_, NegativeOp { Expr: Literal l }) => SubtractOp.Create(left, l),
        _ => Create(Literal.Create(left), right)
    };

    public static Expr Create(Expr left, BigInteger right) => (left, right) switch
    {
        (Zero _, _) => Literal.Create(right),
        (_, { IsZero: true }) => left,
        _ => Create(left, Literal.Create(right))
    };

    public static Expr Create(BigInteger left, BigInteger right) => (left, right) switch
    {
        ({ IsZero: true }, _) => Literal.Create(right),
        (_, { IsZero: true }) => Literal.Create(left),
        _ => Create(Literal.Create(left), Literal.Create(right))
    };


    protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

    public override string ToString() => $"{Left} + {Right}";

    internal override TermsBase Terms() => Left.Simplify().Terms() + Right.Simplify().Terms();

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Left.Simplify() + Right.Simplify();
        v.IsSimple = true;
        return v;
    }

    public override IEnumerable<Expr> Factor() => MethodShortcuts.SeedProcess(
        () => Left.Factor().Venn(Right.Factor()),
        d => d.AandB.Concat(new[] { d.OnlyA.Product() + d.OnlyB.Product() }));

    public override int GetHashCode() => unchecked(539060726.ChainHashCode(Left).ChainHashCode(Right));

    public override FactorsBase InnerFactor()
    {
        return CreateFactor(Left.InnerFactor().Venn(Right.InnerFactor()));
        FactorsBase CreateFactor((FactorsBase OnlyA, FactorsBase AandB, FactorsBase OnlyB) venn) => venn.AandB.Concat(FactorsSingle.Create(FactorSingleN.Create(venn.OnlyA.ToExpr() + venn.OnlyB.ToExpr())));
    }

    public override bool TryToDouble(out double value)
    {
        if (Left.TryToDouble(out double valLeft) && Right.TryToDouble(out double valRight))
        {
            value = valLeft + valRight;
            return true;
        }
        value = 0;
        return false;
    }

    public override Expr Subtitute(Expr current, Expr replacement)
    {
        var newLeft = Left == current ? replacement : Left.Subtitute(current, replacement);
        var newRight = Right== current ? replacement : Right.Subtitute(current, replacement);
        return Create(newLeft, newRight);
    }
}