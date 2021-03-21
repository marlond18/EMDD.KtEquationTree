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

    internal class VarPowD : Variable, IEquatable<VarPowD>
    {
        public VarPowD(Expr @base, Expr exponent)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Expr Base { get; }
        public Expr Exponent { get; }

        internal static Variable Create(Expr @base, Expr exponent) => @base switch
        {
            PowerOp pb => VarPowN.Create(pb.Left, -pb.Right * exponent),
            _ => exponent switch
            {
                NegativeOp n => Create(@base, n.Expr).Invert(),
                Literal l when l.Value == 0 => VarUnit.Create(),
                Literal s when s.Value == 1 => VarD.Create(@base),
                DivideOp(Expr l, Expr r) when l == new One() => VarRootD.Create(@base, r),
                _ => new VarPowD(@base, exponent)
            }
        };

        public override bool Equals(Variable other) => Equals(other as VarPowD);

        public override IEnumerable<Variable> Factor() => Base.Factor().Select(f => VarPowN.Create(f, Exponent).Invert());

        public override Variable Invert() => VarPowN.Create(Base, Exponent);

        public override (Expr Base, Expr Expo) Pair() => (Base, -Exponent);

        public override Variable Raise(Expr c) => VarPowN.Create(Base, -Exponent * c);

        public override Expr ToExpr() => (Base.Terms(), Exponent.Terms()) switch
        {
            (TermsSingle { Term: TermConstant { Coeff: Constant c } },
            TermsSingle { Term: Term { Coeff: Constant c2, Var: Variables v } }) =>
              PowerOp.Create(c.Raise(c2).ToExpr(), v.ToExpr()),
            _ => PowerOp.Create(Base, Exponent)
        };

        public override string ToString() => $"{Base}^{Exponent}";

        public bool Equals(VarPowD other) => this.TestNullBeforeEquals(other, () => Base.Equals(other.Base) && Exponent.Equals(other.Exponent));

        public override int GetHashCode() => unchecked(-801863140.ChainHashCode(Base).ChainHashCode(Exponent));

        public static bool operator ==(VarPowD vD1, VarPowD vD2) => vD1.DefaultEquals(vD2);

        public static bool operator !=(VarPowD vD1, VarPowD vD2) => !(vD1 == vD2);
    }
}