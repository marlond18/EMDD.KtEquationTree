using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtMathStatement.Exprs.Singles;
using EMDD.KtMathStatement.Exprs.Unary;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs;
public abstract class Expr : IEquatable<Expr>
{
    protected Expr()
    {
    }

    public abstract bool Equals(Expr other);

    public static Expr operator -(Expr a) => a.Negate();

    public static Expr operator -(Expr a, Expr b) => a+(-b);

    public static Expr operator *(Expr a, Expr b) => (a.Terms() * b.Terms()).ToExpr();

    public static Expr operator +(Expr a, Expr b) => a.Add(b);

    public static Expr operator /(Expr a, Expr b) => (a.Terms() / b.Terms()).ToExpr();

    public Expr Raise(Expr expr)
    {
        return expr switch
        {
            _ => throw new NotImplementedException()
        };
    }

    protected Expr Add(Expr other)
    {
        if (other is null) return this;
        return other switch
        {
            Literal l => Add(l),
            Identifier i => Add(i),
            Call c => Add(c),
            ComplementOp co => Add(co),
            NegativeOp n => Add(n),
            SqrtOp s => Add(s),
            _ => throw new NotImplementedException()
        };
    }

    public abstract Expr Add(Literal other);
    public abstract Expr Add(Identifier other);
    public abstract Expr Add(Call other);
    public abstract Expr Add(ComplementOp other);
    public abstract Expr Add(NegativeOp other);
    public abstract Expr Add(SqrtOp other);

    protected Expr Mult(Expr other)
    {
        if (other is null) return this;
        return other switch
        {
            Literal l       => Mult(l),
            Identifier i    => Mult(i),
            Call c          => Mult(c),
            ComplementOp co => Mult(co),
            NegativeOp n    => Mult(n),
            SqrtOp s        => Mult(s),
            _               => throw new NotImplementedException()
        };
    }

    public abstract Expr Mult(Literal other);
    public abstract Expr Mult(Identifier other);
    public abstract Expr Mult(Call other);
    public abstract Expr Mult(ComplementOp other);
    public abstract Expr Mult(NegativeOp other);
    public abstract Expr Mult(SqrtOp other);


    public abstract Expr Negate();

    public abstract Expr Invert();

    public abstract Expr Sqrt();

    public static bool operator ==(Expr a, Expr b) => a.DefaultEquals(b);

    public static bool operator !=(Expr a, Expr b) => !(a == b);

    /// <summary>
    /// Try to Convert Expression to Decimal Value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract bool TryToDouble(out double value);

    public override bool Equals(object obj) =>
        ReferenceEquals(this, obj) ||
        this is null ? false :
        obj is null ? false :
        (IsSimple ? this : Simplify()).Equals(obj as Expr);

    public bool IsSimple { get; internal set; }

    public abstract override int GetHashCode();

    public abstract Expr Simplify();

    public abstract Expr Subtitute(Expr current, Expr replacement);
}