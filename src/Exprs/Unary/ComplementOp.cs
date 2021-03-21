﻿using System;
using System.Collections.Generic;
using System.Numerics;

using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;

using KtExtensions;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtEquationTree.Exprs.Unary
{
    public class ComplementOp : UnaryOp
    {
        protected ComplementOp(Expr expr) : base(expr)
        {
            IsSimple = true;
        }

        protected ComplementOp(BigInteger i) : base(i)
        {
            IsSimple = true;
        }

        public static Expr Create(Expr expr) => expr switch
        {
            _ => new ComplementOp(expr)
        };

        public static Expr Create(BigInteger i) => i switch
        {
            _ => new ComplementOp(i)
        };

        private bool Equals(ComplementOp other) => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr));

        protected override bool Equals(UnaryOp other) => Equals(other.Simplify() as ComplementOp);

        public override string ToString() => $"~{ExprToString()}";

        internal override TermsBase Terms() => TermsSingle.Create(TermVariables.Create(VarN.Create(this)));

        public override Expr Simplify()
        {
            if (IsSimple) return this;
            var v = Expr switch
            {
                Literal l => Literal.Create(~l.Value),
                NegativeOp { Expr: Literal l } => Literal.Create(~-l.Value),
                _ => new ComplementOp(this)
            };
            v.IsSimple = true;
            return v;
        }

        public override int GetHashCode() => HashCode.Combine(539060726, Expr);

        public override IEnumerable<Expr> Factor() => new[] { this };

        public override Expr Invert() => this.RaiseToNegativeOne();

        public override FactorsBase InnerFactor() => FactorsSingle.Create(FactorSingleN.Create(this));
    }
}