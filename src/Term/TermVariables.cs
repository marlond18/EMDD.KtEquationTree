using System;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;

using KtExtensions;

using Parser.Expression.Var;

namespace Parser.Expression
{
    internal class TermVariables : TermBase, IEquatable<TermVariables>
    {
        protected TermVariables(Variables variables)
        {
            Variables = variables;
        }

        public Variables Variables { get; }

        public override TermBase Raise(TermBase power) => power switch
        {
            TermZero _ => new TermOne(),
            TermOne _ => this,
            _ => Create(Variables.Raise(power.ToExpr()))
        };

        public override Expr ToExpr() => Variables.ToExpr();

        internal static TermBase Create(Variables variables) => variables.IsEmpty() ? (TermBase)new TermOne() : new TermVariables(variables);

        internal static TermBase Create(Variable variable) => Create(Variables.Create(variable));

        internal static TermBase Create(Variable[] variables) => Create(Variables.Create(variables));

        public override string ToString() => Variables.ToString();

        public override TermBase Negate() => Term.Create(ConstantWhole.Create(-1), Variables);

        public override TermBase Mult(TermBase b) => b switch
        {
            TermZero _ => new TermZero(),
            TermOne _ => this,
            TermConstant c => Term.Create(c.Coeff, Variables),
            TermVariables v => Term.Create(new ConstantOne(), Variables * v.Variables),
            Term t => Term.Create(t.Coeff, Variables * t.Var),
            _ => b.Mult(this)
        };

        public override TermBase Div(TermBase b) => b switch
        {
            TermZero _ => throw new DivideByZeroException($"{this}/{b}"),
            TermOne _ => this,
            TermConstant c => Term.Create(new ConstantOne() / c.Coeff, Variables),
            TermVariables v => Term.Create(new ConstantOne(), Variables * v.Variables.Invert()),
            Term t => Term.Create(t.Coeff, Variables * t.Var.Invert()),
            _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
        };


        public override bool Equals(TermBase other) => Equals(other as TermVariables);

        public bool Equals(TermVariables other) => this.TestNullBeforeEquals(other, () => Variables.Equals(other.Variables));

        public override int GetHashCode() => unchecked(-1853421134.ChainHashCode(Variables));

        public static bool operator ==(TermVariables variables1, TermVariables variables2) => variables1.DefaultEquals(variables2);

        public static bool operator !=(TermVariables variables1, TermVariables variables2) => !(variables1 == variables2);
    }
}