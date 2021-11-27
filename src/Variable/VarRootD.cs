
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Fractions;

namespace Parser.Expression.Var
{

    internal class VarRootD : VariableRoot, IEquatable<VarRootD>
    {
        protected VarRootD(Expr @base, Expr exponent) : base(@base, exponent)
        {
        }

        internal static Variable Create(Expr @base, Expr right) => @base switch
        {
            PowerOp(Expr l, Expr r) => VarPowN.Create(l, -r / right),
            _ => right switch
            {
                DivideOp(Expr l, Expr r) when l == new One() => VarPowD.Create(@base, r),
                _ => new VarRootD(@base, right)
            }
        };

        public override bool Equals(Variable other) => Equals(other as VarRootD);

        internal static Variable Create(Expr @base, BigInteger right) => Create(@base, right.ToExpr());

        public override IEnumerable<Variable> Factor() => Base.Factor().Select(f => VarRootN.Create(f, Exponent).Invert());

        public override Variable Invert() => VarRootN.Create(Base, Exponent);

        public override (Expr Base, Expr Expo) Pair() => (Base, -DivideOp.Create(1, Exponent));

        public override Variable Raise(Expr c) => VarPowN.Create(Base, -c / Exponent);

        public override int GetHashCode() => unchecked(-801863139.ChainHashCode(Base).ChainHashCode(Exponent));

        public bool Equals(VarRootD other) => this.TestNullBeforeEquals(other, () => Base.Equals(other.Base) && Exponent.Equals(other.Exponent));
    }
}