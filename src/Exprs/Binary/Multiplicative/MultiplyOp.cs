using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
public class MultiplyOp : MultiplicativeBinaryOp
{
    protected MultiplyOp(Expr left, Expr right) : base(left, right)
    {
    }

    public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);
    public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));
    public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

    public static Expr Create(Expr left, Expr right) => (left, right) switch
    {
        (null, _) => new Zero(),
        (_, null) => new Zero(),
        (_, Zero z) => z,
        (Zero z, _) => z,
        (One _, _) => right,
        (_, One _) => left,
        (Literal l, Literal r) => Literal.Create(l.Value * r.Value),
        (DivideOp d1, DivideOp d2) => DivideOp.Create(d1.Left * d2.Left, d1.Right * d2.Right),
        (DivideOp d, _) => DivideOp.Create(right * d.Left, d.Right),
        (_, DivideOp d) => DivideOp.Create(left * d.Left, d.Right),
        _ => new MultiplyOp(left, right)
    };

    internal override string Op => "*";

    protected override (Expr left, Expr right) Arrange(Expr a, Expr b)
    {
        if (a is not Literal && b is Literal) return (b, a);
        return (a, b);
    }

    internal override TermsBase Terms() => Left.Simplify().Terms() * Right.Simplify().Terms();

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Left.Simplify() * Right.Simplify();
        v.IsSimple = true;
        return v;

    }

    public override IEnumerable<Expr> Factor() =>
        Left.Factor().Concat(Right.Factor()).ToLookup(f => f).Select(g => (b: g.Key, e: g.Count())).Select(pair => pair.e > 1 ? PowerOp.Create(pair.b, pair.e) : pair.b);

    public override Expr Invert() => Left.Invert() * Right.Invert();

    public override int GetHashCode() => unchecked(539060728.ChainHashCode(Left).ChainHashCode(Right));

    public override FactorsBase InnerFactor() => IsSimple ? Left.InnerFactor().Concat(Right.InnerFactor()) : Simplify().InnerFactor();

    public override bool TryToDouble(out double value)
    {
        if (Left.TryToDouble(out double valLeft) && Right.TryToDouble(out double valRight))
        {
            value = valLeft * valRight;
            return true;
        }
        value = 0;
        return false;
    }

    public override Expr Subtitute(Expr current, Expr replacement)
    {
        var newLeft = Left == current ? replacement : Left.Subtitute(current, replacement);
        var newRight = Right == current ? replacement : Right.Subtitute(current, replacement);
        return Create(newLeft, newRight);
    }
}