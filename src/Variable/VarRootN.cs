using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Fractions;
using EMDD.KtEquationTree.Exprs.Singles;

using KtExtensions;

namespace Parser.Expression.Var
{

    internal class VarRootN : VariableRoot, IEquatable<VarRootN>
    {
        protected VarRootN(Expr @base, Expr exponent) : base(@base, exponent)
        {
        }

        internal static Variable Create(Expr @base, Expr right) => @base switch
        {
            PowerOp(Expr l, Expr r) => VarPowN.Create(l, r / right),
            _ => right switch
            {
                DivideOp(Expr l, Expr r) when l == new One() => VarPowN.Create(@base, r),
                _ => new VarRootN(@base, right)
            }
        };

        internal static Variable Create(Expr @base, BigInteger right) => Create(@base, right.ToExpr());

        public override bool Equals(Variable other) => Equals(other as VarRootN);

        public override int GetHashCode() => unchecked(HashCode.Combine(-801863141, Base, Exponent));

        public override IEnumerable<Variable> Factor() => Base.Factor().Select(f => Create(f, Exponent));

        public override Variable Invert() => VarRootD.Create(Base, Exponent);

        public override (Expr Base, Expr Expo) Pair() => (Base, DivideOp.Create(1, Exponent));

        public override Variable Raise(Expr c) => VarPowN.Create(Base, c / Exponent);

        public bool Equals(VarRootN other) => this.TestNullBeforeEquals(other, () => Base.Equals(other.Base) && Exponent.Equals(other.Exponent));
    }
}