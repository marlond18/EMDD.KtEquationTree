using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.MathStatements;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs;
public abstract class Expr : MathStatement, IEquatable<Expr>
{
    protected Expr()
    {
    }

    public abstract bool Equals(Expr other);

    public static Expr operator -(Expr a) => a == null ? null : (-a.Terms()).ToExpr();

    public static Expr operator -(Expr a, Expr b) => (a.Terms() - b.Terms()).ToExpr();

    public static Expr operator *(Expr a, Expr b) => (a.Terms() * b.Terms()).ToExpr();

    public static Expr operator +(Expr a, Expr b) => (a.Terms() + b.Terms()).ToExpr();

    public static Expr operator /(Expr a, Expr b) => (a.Terms() / b.Terms()).ToExpr();

    public abstract IEnumerable<Expr> Factor();

    internal abstract FactorsBase InnerFactor();

    public override Expr Raise(Expr expr) => Terms().Raise(expr.Terms()).ToExpr();

    public override Expr Sqrt() => Raise(DivideOp.Create(1, 2));

    internal abstract TermsBase Terms();

    public static bool operator ==(Expr a, Expr b) => a.DefaultEquals(b);

    public static bool operator !=(Expr a, Expr b) => !(a == b);

    public override bool Equals(object obj) =>
        ReferenceEquals(this, obj) ||
        this is null ? false :
        obj is null ? false :
        (IsSimple ? this : Simplify()).Equals(obj as Expr);

    public abstract override int GetHashCode();

    public abstract override Expr Invert();

    public abstract override Expr Simplify();

    public abstract override Expr Substitute(Expr current, Expr replacement);

    protected override MathStatement Mult(Relation other)
    {
        return other switch
        {
            EqualsOp eo => EqualsOp.Create(this * eo.Left, this * eo.Right),
            _ => throw new NotImplementedException()
        };
    }

    protected override MathStatement Div(Relation other)
    {
        return other switch
        {
            EqualsOp eo => EqualsOp.Create(this / eo.Left, this / eo.Right),
            _ => throw new NotImplementedException()
        };
    }

    protected override MathStatement Add(Relation other)
    {
        return other switch
        {
            EqualsOp eo => EqualsOp.Create(this + eo.Left, this + eo.Right),
            _ => throw new NotImplementedException()
        };
    }

    protected override MathStatement Mult(Expr other)
    {
        return this * other;
    }

    protected override MathStatement Div(Expr other)
    {
        return this / other;
    }

    protected override MathStatement Add(Expr other)
    {
        return this + other;
    }

    public override bool Equals(MathStatement other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (this is null || other is null) return false;
        return other is Expr e && Equals(e);
    }

    public override MathStatement Raise(Relation r)
    {
        return r switch
        {
            EqualsOp eo => EqualsOp.Create(Raise(eo.Left), Raise(eo.Right)),
            _ => throw new NotImplementedException()
        };
    }
}