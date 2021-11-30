using EMDD.KtEquationTree.Exprs.Binary.Additive;
using EMDD.KtEquationTree.Fractions;

namespace EMDD.KtEquationTree.Exprs.Binary.Main;
public abstract class BinaryOp : Expr
{
    public Expr Left { get; }
    public Expr Right { get; }

    protected BinaryOp(Expr left, Expr right)
    {
        if (left is null) throw new ArgumentNullException(nameof(left));
        if (right is null) throw new ArgumentNullException(nameof(right));
        (Left, Right) = Arrange(left, right);
    }

    protected abstract (Expr left, Expr right) Arrange(Expr a, Expr b);

    protected BinaryOp(BigInteger i, Expr right) : this(i.ToExpr(), right)
    {
    }

    protected BinaryOp(Expr left, BigInteger i) : this(left, i.ToExpr())
    {
    }

    protected BinaryOp(BigInteger i, BigInteger j) : this(i.ToExpr(), j.ToExpr())
    {
    }

    public override bool Equals(Expr other) =>
        ReferenceEquals(this, other) ? true :
        this is null ? false :
        other is null ? false :
        !IsSimple ? Simplify().Equals(other) :
        other switch
        {
            { IsSimple: false } => Equals(other.Simplify()),
            BinaryOp b => Equals(b),
            _ => false
        };

    protected abstract bool Equals(BinaryOp other);

    private bool RightExprShouldBeParenthesised() => !(this is AddOp) && !(Right is Singles.Single);

    protected string RightExprToString() => RightExprShouldBeParenthesised() ? Right.Parenthesize() : Right.ToString();

    public abstract override string ToString();

    public abstract override int GetHashCode();

    public static bool operator ==(BinaryOp a, BinaryOp b) => a.DefaultEquals(b);

    public static bool operator !=(BinaryOp a, BinaryOp b) => !(a == b);

    public void Deconstruct(out Expr left, out Expr right) =>
        (left, right) = (Left, Right);

}