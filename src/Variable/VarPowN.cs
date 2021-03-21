using System;
using System.Collections.Generic;
using System.Linq;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using KtExtensions;

namespace Parser.Expression.Var
{

    internal class VarPowN : Variable, IEquatable<VarPowN>
    {
        protected VarPowN(Expr @base, Expr exponent)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Expr Base { get; }
        public Expr Exponent { get; }

        public static Variable Create(Expr @base, Expr exponent) => @base switch
        {
            PowerOp { Left: Expr l, Right: Expr r } => Create(l, r * exponent),
            _ => exponent switch
            {
                NegativeOp { Expr: Expr e } => Create(@base, e).Invert(),
                Literal { Value: var v } when v == 0 => VarUnit.Create(),
                Literal { Value: var v } when v == 1 => VarN.Create(@base),
                DivideOp d when d.Left == new One() => VarRootN.Create(@base, d.Right),
                _ => new VarPowN(@base, exponent)
            }
        };

        public override bool Equals(Variable other) => Equals(other as VarPowN);

        public override IEnumerable<Variable> Factor() => Base.Factor().Select(f => Create(f, Exponent));

        public override Variable Invert() => VarPowD.Create(Base, Exponent);

        public override (Expr Base, Expr Expo) Pair() => (Base, Exponent);

        public override Variable Raise(Expr c) => Create(Base, Exponent * c);

        public override Expr ToExpr() => (Base.Terms(), Exponent.Terms()) switch
        {
            (TermsSingle { Term: TermConstant { Coeff: Constant c1 } },
            TermsSingle { Term: Term { Coeff: Constant c2, Var: Variables v } }) => PowerOp.Create(c1.Raise(c2).ToExpr(), v.ToExpr()),
            _ => PowerOp.Create(Base, Exponent)
        };

        public override string ToString() => $"{Base}^{Exponent}";

        public bool Equals(VarPowN other) => this.TestNullBeforeEquals(other, () => Base.Equals(other.Base) && Exponent.Equals(other.Exponent));

        public override int GetHashCode() => unchecked(-801863138.ChainHashCode(Base).ChainHashCode(Exponent));

        public static bool operator ==(VarPowN vN1, VarPowN vN2) => vN1.DefaultEquals(vN2);

        public static bool operator !=(VarPowN vN1, VarPowN vN2) => !(vN1 == vN2);
    }
}