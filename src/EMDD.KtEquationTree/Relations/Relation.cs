using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.MathStatements;

namespace EMDD.KtEquationTree.Relations;
public abstract class Relation : MathStatement
{
    protected Relation(MathStatement left, MathStatement right)
    {
        Left = left;
        Right = right;
    }

    public static Relation operator -(Relation a) => a.Negate();

    public static MathStatement operator -(Relation a, MathStatement b) => a + (-b);

    public static MathStatement operator *(Relation a, MathStatement b)
    {
        return b switch
        {
            Relation r => a.Mult(r),
            Expr e => a.Mult(e),
            _ => throw new NotImplementedException($"type of {b} not recornized")
        };
    }

    public static MathStatement operator +(Relation a, MathStatement b)
    {
        return b switch
        {
            Relation r => a.Add(r),
            Expr e => a.Add(e),
            _ => throw new NotImplementedException($"type of {b} not recornized")
        };
    }

    public static MathStatement operator /(Relation a, MathStatement b)
    {
        return b switch
        {
            Relation r => a.Div(r),
            Expr e => a.Div(e),
            _ => throw new NotImplementedException($"type of {b} not recornized")
        };
    }

    protected abstract Relation Negate();

    public MathStatement Left { get; }

    public MathStatement Right { get; }

    public abstract string Operator { get; }

    public override string ToString() => $"{Left}{Operator}{Right}";

    public abstract override int GetHashCode();

    protected abstract bool Equals(Relation other);
}
