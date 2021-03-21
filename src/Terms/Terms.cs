using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Additive;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Fractions;

using KtExtensions;

using Parser.Expression.Var;

namespace Parser.Expression
{
    internal class Terms : TermsBase, IEnumerable<TermBase>
    {
        private readonly IEnumerable<TermBase> _terms;

        private Terms(IEnumerable<TermBase> terms)
        {
            _terms = terms;
        }

        public override IEnumerator<TermBase> GetEnumerator() => _terms.GetEnumerator();

        public static TermsBase Create(IEnumerable<TermBase> terms)
        {
            if (terms == null) throw new NullReferenceException(nameof(terms));
            var (constants, ones, variables, ts) = terms.OfType<TermConstant, TermOne, TermVariables, Term>();
            var constant = TermConstant.Create(constants.Aggregate((Constant)new ConstantZero(), (c1, c2) => c1 + c2.Coeff) + ConstantWhole.Create(ones.Count()));
            var variable = variables.Select(t => (c: (Constant)new ConstantOne(), v: t.Variables)).Concat(ts.Select(t => (c: t.Coeff, v: t.Var))).ToLookup(t => t.v, t => t.c).Select(SumGroup);
            var lookup = variable.Concat(new[] { constant });
            if (lookup.Count() == 1) return TermsSingle.Create(lookup.First());
            if (lookup.Count() < 1) return new TermsZero();
            return new Terms(lookup);
        }

        public override TermsBase Concat(IEnumerable<TermBase> terms) => Create(_terms.Concat(terms));

        protected override TermsBase Negate() => Create(this.Select(t => -t));

        protected override TermsBase Mult(TermsBase b) =>
            Create(Terms.Create(this.Cross(b, (t1, t2) => t1 * t2)));

        public override TermsBase Raise(TermsBase b) => b switch
        {
            TermsZero _ => TermsSingle.Create(new TermOne()),
            TermsSingle { Term: TermConstant { Coeff: Constant c } } => c switch
            {
                ConstantOne _ => this,
                ConstantWhole { Value: var v } => Raise(v),
                ConstantFraction f => Raise(f),
                _ => throw new InvalidOperationException("Tang ina ano to?")
            },
            _ => TermsSingle.Create(TermVariables.Create(VarPowN.Create(ToExpr(), b.ToExpr())))
        };

        internal TermsBase Raise(ConstantFraction c) =>
            c.Value.num.ExceedsIntMax() ? throw new OverflowException($"{this}^{c}")
            : c.Value.num < 0 ? DivideOp.Create(1, Raise(BigInteger.Abs(c.Value.num)).ToExpr().Root(Literal.Create(c.Value.den))).Terms()
            : Root(InnerRaiseToPositive(c.Value.num).ToExpr(), c.Value.den);

        internal TermsBase Raise(BigInteger i) =>
            i.ExceedsIntMax() ? throw new OverflowException($"{this}^{i}")
            : i < 0 ? DivideOp.Create(1, Raise(BigInteger.Abs(i)).ToExpr()).Terms()
            : InnerRaiseToPositive(i);

        private TermsBase InnerRaiseToPositive(BigInteger i) =>
            Enumerable.Repeat<TermsBase>(this, (int)i).Aggregate((t1, t2) => (t1 * t2));

        public override Expr ToExpr() => this.Select(t => t.ToExpr()).Aggregate<Expr, Expr>(new Zero(), AddOp.Create);

        public override TermsBase Div(TermsBase s) => s switch
        {
            TermsZero _ => throw new DivideByZeroException($"{this}/{s}"),
            TermsSingle ss => Create(_terms.Select(t => t / ss.Term)),
            _ => TermsSingle.Create(TermVariables.Create(Variables.Create(VarN.Create(DivideOp.Create(ToExpr(), s.ToExpr())))))
        };

        private static TermBase SumGroup(IGrouping<Variables, Constant> ts) => Term.Create(ts.Sum(), Variables.Create(ts.Key));
    }
}