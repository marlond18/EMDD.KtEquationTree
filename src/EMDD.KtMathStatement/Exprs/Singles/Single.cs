using EMDD.KtEquationTree.Exprs;

namespace EMDD.KtMathStatement.Exprs.Singles;
public abstract class Single : Expr, IEquatable<Expr>
{
    public override Expr Invert() => this.RaiseToNegativeOne();

}