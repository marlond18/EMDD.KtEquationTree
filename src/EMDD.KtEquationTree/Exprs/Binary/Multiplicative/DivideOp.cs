using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.MathStatements;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
public class FractOp : DivideOp
{
    internal FractOp(Expr left, Expr right) : base(left, right)
    {
        _num = (Literal)Left;
        _den = (Literal)Right;
    }

    private readonly Literal _num;
    
    private readonly Literal _den;

    public BigInteger Numerator => _num.Value;

    public BigInteger Denomerator => _den.Value;
}

public class DivideOp : MultiplicativeBinaryOp
{
    protected DivideOp(Expr left, Expr right) : base(left, right)
    {
    }

    public static Expr Create(Expr left, Expr right) => (left, right) switch
    {
        (_, null) => throw new DivideByZeroException($"{left}/0"),
        (_, Zero _) => throw new DivideByZeroException($"{left}/0"),
        (_, One _) => left,
        (null, _) => new Zero(),
        (Zero _, _) => new Zero(),
        (Literal, Literal)=> new FractOp(left,right),
        _ => new DivideOp(left, right)
    };

    public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);

    public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));

    public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

    public static MathStatement Create(MathStatement left, MathStatement right) => (left, right) switch
    {
        (Expr el, Expr er) => Create(el, er),
        (Expr el, EqualsOp eor) => EqualsOp.Create(Create(el, eor.Left), Create(el, eor.Right)),
        (EqualsOp eol, EqualsOp eor) => EqualsOp.Create(Create(eol.Left, eor.Left), Create(eol.Right, eor.Right)),
        (EqualsOp eol, Expr er) => EqualsOp.Create(Create(eol.Left, er), Create(eol.Right, er)),
        _ => throw new NotImplementedException()
    };

    protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

    internal override string Op => "/";

    internal override TermsBase Terms() => Left.Terms() / Right.Terms();

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Left.Simplify() / Right.Simplify();
        v.IsSimple = true;
        return v;
    }

    public override IEnumerable<Expr> Factor() => Left.Factor().Concat(Right.Factor().Select(f => f.Invert()));

    public override Expr Invert() => Left switch
    {
        Literal l when l.Value == 1 => Right,
        NegativeOp { Expr: Literal l } when l.Value == 1 => -Right,
        NegativeOp n => Create(-Right, n.Expr),
        _ => Create(Right, Left)
    };

    public override int GetHashCode() => unchecked(539060727 * HashCode.Combine(typeof(DivideOp), Left, '/', Right));

    internal override FactorsBase InnerFactor() => IsSimple ? Left.InnerFactor().Concat(Right.InnerFactor().Invert()) : Simplify().InnerFactor();

    public override bool TryToDouble(out double value)
    {
        if (Left.TryToDouble(out double valLeft) && Right.TryToDouble(out double valRight))
        {
            value = valLeft / valRight;
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