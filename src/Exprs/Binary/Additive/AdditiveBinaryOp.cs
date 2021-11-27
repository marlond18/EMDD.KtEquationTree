using EMDD.KtEquationTree.Exprs.Binary.Main;

namespace EMDD.KtEquationTree.Exprs.Binary.Additive;
public abstract class AdditiveBinaryOp : BinaryOp, IEquatable<Expr>
{
    protected AdditiveBinaryOp(Expr left, Expr right) : base(left, right)
    {
    }

    protected AdditiveBinaryOp(BigInteger i, Expr right) : base(i, right)
    {
    }

    protected AdditiveBinaryOp(Expr left, BigInteger i) : base(left, i)
    {
    }

    protected AdditiveBinaryOp(BigInteger i, BigInteger j) : base(i, j)
    {
    }

    protected override bool Equals(BinaryOp other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || this is null) return false;
        return (IsSimple ? this : Simplify()).Terms().Equals((other.IsSimple ? other : other.Simplify()).Terms());
    }

    public override Expr Invert() => this.RaiseToNegativeOne();
}