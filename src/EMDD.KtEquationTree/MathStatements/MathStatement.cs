using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Relations;

namespace EMDD.KtEquationTree.MathStatements;

public abstract class MathStatement : IEquatable<MathStatement>
{
    public bool IsSimple { get; internal set; }

    public abstract override int GetHashCode();

    public abstract MathStatement Invert();

    public abstract MathStatement Simplify();

    public abstract MathStatement Subtitute(Expr current, Expr replacement);

    public MathStatement Subtitute(Expr current, int replacement)
    {
        return Subtitute(current, Literal.Create(replacement));
    }

    public MathStatement Subtitute(Expr current, decimal replacement)
    {
        return Subtitute(current, Dec.Create(replacement));
    }

    public static MathStatement operator -(MathStatement a)
    {
        return a switch
        {
            null => null,
            Relation r => -r,
            Expr e => -e,
            _ => throw new NotImplementedException()
        };
    }
    public static MathStatement operator -(MathStatement a, MathStatement b) => a + (-b);

    public static MathStatement operator *(MathStatement a, MathStatement b)
    {
        return b switch
        {
            Relation r => a * r,
            Expr e => a * e,
            _ => throw new NotImplementedException()
        };
    }

    public static MathStatement operator +(MathStatement a, MathStatement b)
    {
        return b switch
        {
            Relation r => a + r,
            Expr e => a + e,
            _ => throw new NotImplementedException()
        };
    }

    public static MathStatement operator /(MathStatement a, MathStatement b)
    {
        return b switch
        {
            Relation r => a / r,
            Expr e => a / e,
            _ => throw new NotImplementedException()
        };
    }

    public static bool operator ==(MathStatement a, MathStatement b) => a.Equals(b);

    public static bool operator !=(MathStatement a, MathStatement b) => !(a == b);

    public static MathStatement operator -(MathStatement a, Relation b) => a + (-b);

    public static MathStatement operator *(MathStatement a, Relation b) => a.Mult(b);

    public static MathStatement operator +(MathStatement a, Relation b) => a.Add(b);

    public static MathStatement operator /(MathStatement a, Relation b) => a.Div(b);

    public static MathStatement operator -(MathStatement a, Expr b) => a + (-b);

    public static MathStatement operator *(MathStatement a, Expr b) => a.Mult(b);

    public static MathStatement operator +(MathStatement a, Expr b) => a.Add(b);

    public static MathStatement operator /(MathStatement a, Expr b) => a.Div(b);

    protected abstract MathStatement Mult(Relation other);

    protected abstract MathStatement Div(Relation other);

    protected abstract MathStatement Add(Relation other);

    protected abstract MathStatement Mult(Expr other);

    protected abstract MathStatement Div(Expr other);

    protected abstract MathStatement Add(Expr other);

    public abstract MathStatement Raise(Expr expr);

    public abstract MathStatement Raise(Relation r);

    public MathStatement Raise(MathStatement ms)
    {
        if (ms is Expr e) return Raise(e);
        if (ms is Relation r) return Raise(r);
        throw new InvalidOperationException("raise a value with an equation is invalid");
    }

    public abstract bool TryToDouble(out double value);

    public abstract bool Equals(MathStatement other);

    public abstract MathStatement Sqrt();

    public override bool Equals(object obj)
        => Equals(obj as MathStatement);
}