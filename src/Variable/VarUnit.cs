using System;
using System.Collections.Generic;

using EMDD.KtEquationTree.Exprs;

using KtExtensions;
using EMDD.KtEquationTree.Exprs.Singles;


namespace Parser.Expression.Var
{

    internal class VarUnit : Variable, IEquatable<VarUnit>
    {
        protected VarUnit()
        {
        }

        internal static Variable Create() => new VarUnit();

        public override bool Equals(Variable other) => Equals(other as VarUnit);

        public override IEnumerable<Variable> Factor() => new[] { new VarUnit() };

        public override Variable Invert() => new VarUnit();

        public override (Expr Base, Expr Expo) Pair() => (new One(), new One());

        public override Variable Raise(Expr c) => new VarUnit();

        public override Expr ToExpr() => new One();

        public override string ToString() => "1";

        public bool Equals(VarUnit other) => this.TestNullBeforeEquals(other, () => true);

        public override int GetHashCode() => 801863149;
    }
}