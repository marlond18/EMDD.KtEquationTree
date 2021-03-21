using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

using Parser.Expression;

using System;

namespace EMDD.KtEquationTree.Constant
{
    internal class ConstantZero : Constant, IEquatable<ConstantZero>
    {
        protected override Constant Add(Constant b) => b;

        protected override Constant Div(Constant b) => new ConstantZero();

        public override bool Equals(Constant other) => Equals(other as ConstantZero);

        protected override Constant Negate() => new ConstantZero();

        public override Expr ToExpr() => new Zero();

        public override string ToString() => "0";

        protected override Constant Mult(Constant b) => new ConstantZero();

        public override TermBase Raise(Constant coefficient) => new TermZero();

        public bool Equals(ConstantZero other) => true;

        public override int GetHashCode() => -1937169417;
    }
}