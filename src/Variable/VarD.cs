
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;

namespace Parser.Expression.Var
{

    internal class VarD : Variable, IEquatable<VarD>
    {
        protected VarD(Expr expr)
        {
            Expr = expr;
        }

        public Expr Expr { get; }

        internal static Variable Create(Expr @base) => @base switch
        {
            PowerOp(Expr l, Expr r) => VarPowN.Create(l, -r),
            _ => new VarD(@base)
        };

        public override IEnumerable<Variable> Factor() => Expr.Factor().Select(f => VarN.Create(f).Invert());

        public override Variable Invert() => VarN.Create(Expr);

        public override Variable Raise(Expr c) => VarPowN.Create(Expr, -c);

        public override (Expr Base, Expr Expo) Pair() => (Expr, -new One());

        public override Expr ToExpr() => Expr;

        public override string ToString() => Expr.ToString();

        public override bool Equals(Variable other) => Equals(other as VarD);

        public bool Equals(VarD other) => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr));

        public override int GetHashCode() => unchecked(-601397246.ChainHashCode(Expr));
    }
}