using EMDD.KtEquationTree.Exprs.Singles;

using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;

namespace Parser.Expression.Var
{

    internal class VarN : Variable, IEquatable<VarN>
    {
        protected VarN(Expr expr)
        {
            Expr = expr;
        }

        public Expr Expr { get; }

        internal static Variable Create(Expr @base) => @base switch
        {
            PowerOp(Expr l, Expr r) => VarPowN.Create(l, r),
            _ => new VarN(@base)
        };

        public override bool Equals(Variable other) => Equals(other as VarN);

        public override IEnumerable<Variable> Factor() => Expr.Factor().Select(f => Create(f));

        public override Variable Invert() => VarD.Create(Expr);

        public override (Expr Base, Expr Expo) Pair() => (Expr, new One());

        public override Variable Raise(Expr c) => VarPowN.Create(Expr, c);

        public override Expr ToExpr() => Expr;

        public override string ToString() => Expr.ToString();

        public bool Equals(VarN other) => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr));

        public override int GetHashCode() => unchecked(601397246.ChainHashCode(Expr));
    }
}