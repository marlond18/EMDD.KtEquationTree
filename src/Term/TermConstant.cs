using System;
using System.Numerics;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Fractions;

using KtExtensions;

namespace Parser.Expression
{
    internal class TermConstant : TermBase, IEquatable<TermConstant>
    {
        protected TermConstant(Constant coeff)
        {
            Coeff = coeff;
        }

        protected TermConstant(BigInteger val) : this(ConstantWhole.Create(val))
        {
        }

        protected TermConstant(BigInteger num, BigInteger den) : this(ConstantFraction.Create(new Fraction(num, den)))
        {
        }

        public Constant Coeff { get; }

        public static TermBase Create(BigInteger val) => val switch
        {
            { IsZero: true } => new TermZero(),
            { IsOne: true } => new TermOne(),
            _ => new TermConstant(val)
        };

        public static TermBase Create(BigInteger num, BigInteger den) => (num, den) switch
        {
            ({ IsZero: true }, _) => new TermZero(),
            ({ IsOne: true }, { IsOne: true }) => new TermOne(),
            _ => new TermConstant(num, den)
        };

        public static TermBase Create(Constant constant) => constant switch
        {
            ConstantZero _ => new TermZero(),
            ConstantOne _ => new TermOne(),
            _ => new TermConstant(constant)
        };

        public override TermBase Div(TermBase b) => this is null ? null :
            b switch
        {
            TermZero _ => throw new DivideByZeroException($"{this}/{b}"),
            TermOne _ => this,
            TermConstant c => Create(Coeff / c.Coeff),
            TermVariables v => Term.Create(Coeff, v.Variables.Invert()),
            Term t => Term.Create(Coeff / t.Coeff, t.Var.Invert()),
            _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
        };

        public override bool Equals(TermBase other) => Equals(other as TermConstant);

        public override TermBase Mult(TermBase b) => b switch
        {
            TermZero z => z,
            TermOne _ => this,
            TermConstant c => Create(Coeff * c.Coeff),
            TermVariables v => Term.Create(Coeff, v.Variables),
            Term t => Term.Create(Coeff * t.Coeff, t.Var),
            _ => b.Mult(this)
        };

        public override TermBase Negate() => Create(-Coeff);

        public override TermBase Raise(TermBase power) => power switch
        {
            TermZero _ => new TermOne(),
            TermOne _ => this,
            TermConstant c => Coeff.Raise(c.Coeff),
            TermVariables v => Coeff.Raise(v.Variables),
            Term t => Coeff.Raise(t.Coeff).Raise(TermVariables.Create(t.Var)),
            _ => throw new InvalidOperationException($"Cannot Determine the type of {power}")
        };

        public override Expr ToExpr() => Coeff.ToExpr();

        public override string ToString() => Coeff.ToString();

        public bool Equals(TermConstant other) => this.TestNullBeforeEquals(other, () => Coeff.Equals(other.Coeff));

        public override int GetHashCode() => unchecked(-1574991550.ChainHashCode(Coeff));

        public static bool operator ==(TermConstant constant1, TermConstant constant2) => constant1.DefaultEquals(constant2);

        public static bool operator !=(TermConstant constant1, TermConstant constant2) => !(constant1 == constant2);
    }
}