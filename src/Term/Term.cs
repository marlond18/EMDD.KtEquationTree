using System;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using KtExtensions;

using Parser.Expression.Var;

namespace Parser.Expression
{
    internal class Term : TermBase, IEquatable<Term>
    {
        protected Term(Constant coefficient, Variables variables)
        {
            Coeff = coefficient;
            Var = variables;
        }

        public Constant Coeff { get; }
        public Variables Var { get; }

        public static TermBase Create(Constant coeff, Variables variables)
        {
            if (coeff is ConstantZero) return new TermZero();
            if (variables.IsEmpty()) return TermConstant.Create(coeff);
            if (coeff is ConstantOne) return TermVariables.Create(variables);
            return new Term(coeff, variables);
        }

        public override TermBase Div(TermBase b) => b switch
        {
            TermZero _ => throw new DivideByZeroException($"{this}/{b}"),
            TermOne _ => this,
            TermConstant c => Create(Coeff / c.Coeff, Var),
            TermVariables v => Create(Coeff, Var * v.Variables.Invert()),
            Term t => Create(Coeff / t.Coeff, Var * t.Var.Invert()),
            _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
        };

        public override bool Equals(TermBase other) => Equals(other as Term);

        public override TermBase Mult(TermBase b) => b switch
        {
            TermZero _ => new TermZero(),
            TermOne _ => this,
            TermConstant c => Create(Coeff * c.Coeff, Var),
            TermVariables v => Create(Coeff, Var * v.Variables),
            Term t => Create(Coeff * t.Coeff, Var * t.Var),
            _ => b.Mult(this),
        };

        public override TermBase Negate() => Create(-Coeff, Var);

        public override TermBase Raise(TermBase power) => power switch
        {
            TermZero _ => new TermOne(),
            TermOne _ => this,
            TermConstant c => Coeff.Raise(c.Coeff) * TermVariables.Create(Var.Raise(c.ToExpr())),
            TermVariables v => Coeff.Raise(v.Variables) * TermVariables.Create(Var.Raise(v.ToExpr())),
            Term t => Coeff.Raise(t.Coeff).Raise(TermVariables.Create(t.Var)) * TermVariables.Create(Var.Raise(power.ToExpr())),
            _ => throw new InvalidOperationException($"Cannot determine the type of {power}")
        };

        public override Expr ToExpr() => MethodShortcuts.SeedProcess(() => (Coeff.ToExpr(), Var.ToExpr()), InternalToExpr);

        public override string ToString() => $"{Coeff}*{Var}";

        private Expr InternalToExpr(Expr c, Expr v) => (c, v) switch
        {
            (_, Literal l) when l.Value == 1 => c,
            (Literal l1, Literal l2) => Literal.Create(l1.Value * l2.Value),
            (_, Literal _) => InternalToExpr(v, c),
            (Literal l, _) when l.Value == 1 => v,
            (Literal l, _) when l.Value == 0 => new Zero(),
            (NegativeOp n, _) => NegativeOp.Create(InternalToExpr(n.Expr, v)),
            (DivideOp d1, DivideOp d2) => DivideOp.Create(d1.Left * d2.Left, d1.Right * d2.Right),
            (DivideOp d, _) => DivideOp.Create(d.Left * v, d.Right),
            (_, DivideOp d) => DivideOp.Create(d.Left * c, d.Right),
            _ => MultiplyOp.Create(c, v)
        };

        public bool Equals(Term other) => this.TestNullBeforeEquals(other, () => Coeff.Equals(other.Coeff) && Var.Equals(other.Var));

        public override int GetHashCode() => unchecked(464943524.ChainHashCode(Coeff).ChainHashCode(Var));

        public static bool operator ==(Term term1, Term term2) => term1.DefaultEquals(term2);

        public static bool operator !=(Term term1, Term term2) => !(term1 == term2);
    }
}