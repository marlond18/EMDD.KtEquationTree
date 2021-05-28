&nbsp; [![Nuget](https://img.shields.io/nuget/v/EMDD.KtEquationTree)](https://www.nuget.org/packages/EMDD.KtEquationTree/)[![Nuget](https://img.shields.io/nuget/dt/EMDD.KtEquationTree)](https://www.nuget.org/stats/packages/EMDD.KtEquationTree?groupby=Version&groupby=ClientName&groupby=ClientVersion)[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/marlond18/EMDD.KtEquationTree/Run%20Tests)](https://github.com/marlond18/EMDD.KtEquationTree/actions/workflows/runTest.yml)
&nbsp; 
----------------
# EMDD.KtEquationTree
C# .Net implementation of Symbolic Mathematics

## Requirements

[.Net 5.0.102 sdk](https://dotnet.microsoft.com/download/dotnet/5.0) or greater

## Nuget Package Usage

https://www.nuget.org/packages/EMDD.KtEquationTree/

`<PackageReference Include="EMDD.KtEquationTree" Version="*.*.*" />`

## Dependencies

- EMDD.Kt.Extensions

`<PackageReference Include="EMDD.Kt.Extensions" Version="1.0.0.1" />`

- Pidgin, Parser

`<PackageReference Include="Pidgin" Version="2.5.0" />`

## Motivation
I really wanted to at-least mimic the basic functionality of [wxmaxima](http://wxmaxima-developers.github.io/wxmaxima/index.html) in .Net. Here are some of the things I wanted to see:
    - parsing of text to symbolic math expressions
    - simplication of math expressions
To cut it short, this is my take on this task

## Usage
- strings can be parsed into numbers or math expression using `EMDD.KtEquationTree.Parsers.ExprParser.ParseOrThrow();`, see examples below.
- Actual `Expr` can be instantiated as:
    - Literal

```c#
var number = EMDD.KtEquationTree.Exprs.Singles.Literal.Create(20);
```

    - Variables

```c#
var variableA = new EMDD.KtEquationTree.Exprs.Singles.Identifier("A");
```

    - Other `Expr`

```c#
//lambda functions (x, y) => x + y
var x = new EMDD.KtEquationTree.Exprs.Singles.Identifier("x");
var y = new EMDD.KtEquationTree.Exprs.Singles.Identifier("y");
var mathfunction = EMDD.KtEquationTree.Exprs.Unary.Call(x + y, (x, y));


// square root of an Expr
var x = new EMDD.KtEquationTree.Exprs.Singles.Identifier("x");
var sqrtOfX = new  EMDD.KtEquationTree.Exprs.Unary.SqrtOp(x);

//
```

### Parsing of string to number `Expr`
```c#
using System.Linq;
using EMDD.KtEquationTree.Parsers;
using EMDD.KtEquationTree.Exprs.Binary.Additive;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Exprs;

namespace Samples
{
    public class NumbersTests
    {
        public void BasicFactor()
        {
            var str= "12";
            var twelve = ExprParser.ParseOrThrow(str);
            
            // this may look a bit excessiv, since we can just simply parse the string 12 into double
            var number = double.Parse(str);

            // but parsing 12 into an Expr enables us to factor it like:
            var factorsOfTwelve = twelve.Factor();
            
            var twoRaisedToTwo =  PowerOp.Create(2, 2);
            var three = Literal.Create(3);
            
            // factorsOfTwelve will be (2^2)*3
            Console.WriteLine(factorsOfTwelve.Contains(twoRaisedToTwo));
            Console.WriteLine(factorsOfTwelve.Contains(three));
        }
    
        // quite simple for small integers, but it is also able to perform parsing and factorization of large numbers
        public void LargeNumber()
        {
            var newlit = ExprParser.ParseOrThrow("2069366877425482173897306373270574575520870902460429264486400000");
            var expected = newlit.Factor();
            var twoRaisedToSeventySeven = PowerOp.Create(2, 77);
            var threeRaisedToFiftyTwo = PowerOp.Create(3, 52);
            var fiveRaisedToFive = PowerOp.Create(5, 5);
            var sevenPowOf14 = PowerOp.Create(7, 14) };

            Console.WriteLine(expected.Contains(twoRaisedToSeventySeven));
            Console.WriteLine(expected.Contains(threeRaisedToFiftyTwo));
            Console.WriteLine(expected.Contains(fiveRaisedToFive));
            Console.WriteLine(expected.Contains(sevenPowOf14));
        }

        // it is also able to recognize floating point values
        public void Decimal()
        {
            var f = ExprParser.ParseOrThrow("1.2340000");

            //1.234 == 1 + (234/1000)
            Console.WriteLine(f == new One() + (Literal.Create(234) / Literal.Create(1000)));
            
            Console.WriteLine(f == (Literal.Create(234) / Literal.Create(1000)) + new One());
        }

        //Including scientific notations
        public void ScientificNotation()
        {
            var f = ExprParser.ParseOrThrow("1.2340000E(4)");
            // 1.234E4 == 1 * 10000 + 2340
            Console.WriteLine(f == (new One() * Literal.Create(10000)) + Literal.Create(2340));
            Console.WriteLine(f == Literal.Create(2340) + (Literal.Create(10000) * new One()));
        }
    }
}

```

### Parsing of string to math expression `Expr`
```c#
using System.Linq;
using EMDD.KtEquationTree.Parsers;
using EMDD.KtEquationTree.Exprs.Binary.Additive;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Exprs;
namespace Samples
{
    public class ExpressionTests
    {
        public void AdditionOfExpression()
        {
            var d = ExprParser.ParseOrThrow("2*x");
            var e = ExprParser.ParseOrThrow("1*x");
            var f = ExprParser.ParseOrThrow("2");
            var h = ExprParser.ParseOrThrow("2");

            ///2x + x + 2 + 2 == 3x + 4 
            Console.WriteLine(Assert.AreEqual(d + e + f + h, AddOp.Create(MultiplyOp.Create(3, new Identifier("x")), 4));
        }

        public void PowerOfExpressions()
        {
            var a = new Identifier("a^1");

            // a^1 == a
            // invert of a == 1 / a
            Console.WriteLine(a.Invert(), DivideOp.Create(1, a));
        }
    }
}

```
### TODO
- More Examples
