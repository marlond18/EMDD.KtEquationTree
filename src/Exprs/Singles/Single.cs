
using System;

namespace EMDD.KtEquationTree.Exprs.Singles
{
    public abstract class Single : Expr, IEquatable<Expr>
    {
        public override Expr Invert() => this.RaiseToNegativeOne();

    }
}
