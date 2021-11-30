
using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

namespace Parser.Expression
{
    internal class TermOne : TermBase, IEquatable<TermOne>
    {
        public override TermBase Div(TermBase b) => b switch
        {
            TermZero _ => throw new DivideByZeroException($"{this}/{b}"),
            TermOne _ => this,
            TermConstant c => TermConstant.Create(new ConstantOne() / c.Coeff),
            TermVariables v => Term.Create(new ConstantOne(), v.Variables.Invert()),
            Term t => Term.Create(new ConstantOne() / t.Coeff, t.Var.Invert()),
            _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
        };

        public TermBase Wrap() => this;

        public override bool Equals(TermBase other) => Equals(other as TermOne);

        public override TermBase Mult(TermBase b) => b;

        public override TermBase Negate() => TermConstant.Create(ConstantWhole.Create(-1));

        public override TermBase Raise(TermBase power) => new TermOne();

        public override Expr ToExpr() => new One();

        public override string ToString() => "1";

        public bool Equals(TermOne other) => this.TestNullBeforeEquals(other, () => true);

        public override int GetHashCode() => 1853421131;
    }
}