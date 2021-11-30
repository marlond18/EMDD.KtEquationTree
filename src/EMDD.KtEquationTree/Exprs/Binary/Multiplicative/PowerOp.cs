using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.MathStatements;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
public class PowerOp : MultiplicativeBinaryOp
{
    internal override string Op => "^";

    protected PowerOp(Expr left, Expr right) : base(left, right)
    {
    }

    public static Expr Create(Expr left, Expr right) => (left, right) switch
    {
        (null, null) => throw new InvalidOperationException($"0^0"),
        (_, null) => new One(),
        (null, _) => new Zero(),
        (Zero _, Zero _) => throw new InvalidOperationException($"0^0"),
        (_, Zero _) => new One(),
        (Zero z, _) => z,
        (_, One _) => left,
        (One o, _) => o,
        (DivideOp d, NegativeOp n) => new PowerOp(DivideOp.Create(d.Right, d.Left), n.Expr),
        (_, NegativeOp n) => DivideOp.Create(1, Create(left, n.Expr)),
        _ => new PowerOp(left, right)
    };

    public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);
    
    public static MathStatement Create(BigInteger left, MathStatement right) => Create(Literal.Create(left), right);

    public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));

    public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

    public static MathStatement Create(MathStatement left, MathStatement right) => (left, right) switch
    {
        (Expr el, Expr er) => Create(el, er),
        (Expr el, EqualsOp eor) => throw new InvalidOperationException($"expression raised to Equality operator."),
        (EqualsOp eol, EqualsOp eor) => EqualsOp.Create(Create(eol.Left, eor.Left), Create(eol.Right, eor.Right)),
        (EqualsOp eol, Expr er) => EqualsOp.Create(Create(eol.Left, er), Create(eol.Right, er)),
        _ => throw new NotImplementedException()
    };

    protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

    internal override TermsBase Terms() => Left.Simplify().Terms().Raise(Right.Simplify().Terms());

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Left.Simplify().Raise(Right.Simplify());
        v.IsSimple = true;
        return v;
    }

    public override IEnumerable<Expr> Factor() =>
        Left.Factor().Select(f => f is PowerOp p ? Create(p.Left, p.Right * Right) : Create(f, Right));

    public override Expr Invert()
    {
        var simp = Simplify();
        return simp switch
        {
            PowerOp(Expr left, NegativeOp { Expr: Expr e }) => e switch
            {
                Literal l when l.Value == 1 => left,
                _ => Create(left, e)
            },
            PowerOp(Expr l, Expr r) => Create(l, -r),
            _ => simp.Invert()
        };
    }

    public override int GetHashCode() => unchecked(HashCode.Combine(539060729, Left, Right));

    internal override FactorsBase InnerFactor() => IsSimple ? Factors.Factors.Create(Left.InnerFactor().Raise(Right)) : Simplify().InnerFactor();

    public override bool TryToDouble(out double value)
    {
        if (Left.TryToDouble(out double valLeft) && Right.TryToDouble(out double valRight))
        {
            value = Math.Pow(valLeft, valRight);
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