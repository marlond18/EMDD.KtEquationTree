
using EMDD.KtEquationTree.Exprs;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtEquationTree.Constant;
internal abstract class Constant : IEquatable<Constant>
{
    public abstract override string ToString();

    public static Constant operator +(Constant a, Constant b) => a is null && b is null ? null : a is null ? b : b is null ? a : a.Add(b);

    protected abstract Constant Add(Constant b);

    public static Constant operator -(Constant a) => a?.Negate();

    protected abstract Constant Negate();

    public static Constant operator *(Constant a, Constant b) => a is null || b is null ? null : a.Mult(b);

    protected abstract Constant Mult(Constant b);

    public static Constant operator /(Constant a, Constant b) => b is null ? throw new DivideByZeroException($"{a}/null") : (a?.Div(b));

    protected abstract Constant Div(Constant b);

    public abstract TermBase Raise(Constant coefficient);

    public static bool operator ==(Constant a, Constant b) => a.DefaultEquals(b);

    public static bool operator !=(Constant a, Constant b) => !(a == b);

    public abstract Expr ToExpr();

    public abstract bool Equals(Constant other);

    public override bool Equals(object obj) => Equals(obj as Constant);

    public TermBase Raise(Variables expo) => TermVariables.Create(VarPowN.Create(ToExpr(), expo.ToExpr()));

    public abstract override int GetHashCode();
}
