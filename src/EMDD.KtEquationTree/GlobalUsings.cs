global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.Numerics;
global using System.Collections.Immutable;
global using KtExtensions;
global using ParserChar = Pidgin.Parser<char, char>;
global using ParserExpr = Pidgin.Parser<char, EMDD.KtEquationTree.MathStatements.MathStatement>;
global using ParserStr = Pidgin.Parser<char, string>;