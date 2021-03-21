using System;

using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

using KtExtensions;

namespace Parser.Expression
{
    internal class TermZero : TermBase, IEquatable<TermZero>
    {
        public override TermBase Div(TermBase b) => b is TermZero _ ? throw new DivideByZeroException($"{this}/{b}") : new TermZero();

        public override bool Equals(TermBase other) => other is TermZero;

        public override TermBase Mult(TermBase b) => new TermZero();

        public override TermBase Negate() => this;

        public override TermBase Raise(TermBase power) => power switch
        {
            TermZero _ => throw new InvalidOperationException($"Cannot raise Zero With Zero {this}^{power}"),
            _ => new TermZero()
        };

        public override Expr ToExpr() => new Zero();

        public override string ToString() => "0";

        public bool Equals(TermZero other) => this.TestNullBeforeEquals(other, () => true);

        public override int GetHashCode() => -1853421131;
    }
}