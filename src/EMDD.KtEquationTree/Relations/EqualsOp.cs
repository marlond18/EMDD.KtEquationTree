using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.MathStatements;

namespace EMDD.KtEquationTree.Relations;
public sealed class EqualsOp : Relation, IEquatable<EqualsOp>
{
    private EqualsOp(MathStatement left, MathStatement right) : base(left, right)
    {
    }

    public static EqualsOp Create(MathStatement left, MathStatement right)
    {
        return new EqualsOp(left, right);
    }

    public override int GetHashCode() => HashCode.Combine(typeof(EqualsOp), Left, '=', Right);

    public override string Operator => "=";

    public override MathStatement Invert()
    {
        return Create(Left.Invert(), Right.Invert());
    }

    public override MathStatement Simplify()
    {
        if (IsSimple) return this;
        var v = Create(Left.Simplify(), Right.Simplify());
        v.IsSimple = true;
        return v;
    }

    public override MathStatement Substitute(Expr current, Expr replacement)
    {
        return (Left, Right) switch
        {
            (Expr el, Expr er) => Create(el.Substitute(current, replacement), er.Substitute(current, replacement)),
            (Expr el, MathStatement er) => Create(el.Substitute(current, replacement), er.Substitute(current, replacement)),
            (MathStatement el, Expr er) => Create(el.Substitute(current, replacement), er.Substitute(current, replacement)),
            (MathStatement el, MathStatement er) => Create(el.Substitute(current, replacement), er.Substitute(current, replacement)),
            _ => throw new NotImplementedException()
        };
    }

    protected override bool Equals(Relation other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null || this is null) return false;
        if (other is EqualsOp eo)
        {
            return Equals(eo);
        }
        return false;
    }

    public bool Evaluate()
    {
        return Left == Right;
    }

    protected override Relation Negate()
    {
        return Create(-Left, -Right);
    }

    protected override Relation Mult(Relation other)
    {
        if (other is null) return null;
        return Create(Left * other.Left, Right * other.Right);
    }

    protected override Relation Div(Relation other)
    {
        if (other is null) throw new ArgumentNullException("Division by null value.");
        return Create(Left / other.Left, Right / other.Right);
    }

    protected override Relation Add(Relation other)
    {
        if (other is null) return this;
        return Create(Left + other.Left, Right + other.Right);
    }

    protected override Relation Mult(Expr other)
    {
        if (other is null) return null;
        return Create(Left * other, Right * other);
    }

    protected override Relation Div(Expr other)
    {
        if (other is null) throw new ArgumentNullException("Division by null value.");
        return Create(Left / other, Right / other);
    }

    protected override Relation Add(Expr other)
    {
        if (other is null) return null;
        return Create(Left + other, Right + other);
    }

    public override MathStatement Raise(Expr expr) => Create(Left.Raise(expr), Right.Raise(expr));

    public override MathStatement Raise(Relation r)
    {
        if (r is EqualsOp eo)
        {
            return Create(Left.Raise(eo.Left), Right.Raise(eo.Right));
        }
        throw new InvalidOperationException();
    }

    public override bool TryToDouble(out double value)
    {
        value = 0;
        return false;
    }

    public override bool Equals(MathStatement other)
    {
        if (ReferenceEquals(other, this)) return true;
        if (other is null || this is null) return false;
        if (other is EqualsOp eo)
        {
            return Equals(eo);
        }
        return false;
    }

    public bool Equals(EqualsOp other)
    {
        if (ReferenceEquals(other, this)) return true;
        if (other is null || this is null) return false;
        return Left == other.Left && Right == other.Right;
    }

    public override MathStatement Sqrt()
    {
        return Create(Left.Sqrt(), Right.Sqrt());
    }
}