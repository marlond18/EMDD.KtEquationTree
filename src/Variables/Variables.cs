using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;

using KtExtensions;

namespace Parser.Expression.Var
{
    internal class Variables : IEnumerable<Variable>, IEquatable<Variables>
    {
        private readonly IEnumerable<Variable> _variables;

        private Variables(IEnumerable<Variable> variables)
        {
            _variables = variables;
        }

        public static Variables Create(IEnumerable<Variable> variables) =>
            new Variables(variables.Select(v => v.Factor()).SelectMany(fs => fs).Select(f => f.Pair())
                .ToLookup(f => f.Base, f => f.Expo).Select(f => VarPowN.Create(f.Key, f.Sum())));

        public static Variables Create(Variable variable) => Create(new[] { variable });

        public static Variables operator *(Variables a, Variables b) => a.Concat(b);

        public IEnumerator<Variable> GetEnumerator() => _variables.GetEnumerator();

        public Variables Empty() => new Variables(new Variable[] { });

        IEnumerator IEnumerable.GetEnumerator() => _variables.GetEnumerator();

        public override string ToString() => _variables.BuildString("");

        public override bool Equals(object obj) => Equals(obj as Variables);

        public bool Equals(Variables other) => this.TestNullBeforeEquals(other, () => this.ContentEquals(other));

        public override int GetHashCode() => unchecked(-548317341.ChainHashCode(_variables));

        public static bool operator ==(Variables variables1, Variables variables2) => variables1.DefaultEquals(variables2);

        public static bool operator !=(Variables variables1, Variables variables2) => !(variables1 == variables2);

        public Variables Concat(IEnumerable<Variable> variables) =>
            Create(_variables.Concat(variables));

        public Variables Raise(Expr e) => Create(this.Select(v => v.Raise(e)));

        public Variables Invert() => Create(_variables.Select(v => v.Invert()));

        public Expr ToExpr()
        {
            var (VN, VSN, VRN) = _variables.OfType<VarPowN, VarN, VarRootN>();
            var numerators = VN.Concat<Variable>(VSN).Concat(VRN).Select(f => f.ToExpr());
            var (VD, VSD, VRD) = _variables.OfType<VarPowD, VarD, VarRootD>();
            var denominators = VD.Concat<Variable>(VSD).Concat(VRD).Select(f => f.ToExpr());
            var numerator = numerators.IsEmpty() ? new One() : numerators.Aggregate((n1, n2) => MultiplyOp.Create(n1, n2));
            if (denominators.IsEmpty()) return numerator;
            return DivideOp.Create(numerator, denominators.Aggregate((n1, n2) => MultiplyOp.Create(n1, n2)));
        }
    }
}