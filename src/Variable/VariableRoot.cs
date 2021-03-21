using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

namespace Parser.Expression.Var
{

    internal abstract class VariableRoot : Variable
    {
        protected VariableRoot(Expr @base, Expr exponent)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Expr Base { get; }
        public Expr Exponent { get; }

        public override Expr ToExpr() => Exponent switch
        {
            Literal { Value: var v } when v == 1 => Base,
            Literal { Value: var v } when v == 2 => SqrtOp.Create(Base),
            _ => PowerOp.Create(Base,DivideOp.Create(1, Exponent))
        };

        public override string ToString() => ToExpr().ToString();
    }
}