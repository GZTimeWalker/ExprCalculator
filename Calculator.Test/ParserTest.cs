using Xunit;
using Calculator.Core;
using Calculator.Core.Expr;
using Xunit.Abstractions;

namespace Calculator.Test
{
    public class ParserTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public ParserTest(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }

        void DoParserTest(string input)
        {
            var result = Parser.Parse(input);
            Assert.True(result is not null);
            _outputHelper.WriteLine($"f(a) = {result}");
            var ans = result!.Simplify();
            _outputHelper.WriteLine($"f(a) = {ans}");
            var dres = ans.D("a");
            _outputHelper.WriteLine($"f'(a) = {dres}");
            var dans = dres.Simplify();
            _outputHelper.WriteLine($"f'(a) = {dans}");

            var value = System.Random.Shared.Next(0, 20);
            VarExpr.Define("a", value);

            _outputHelper.WriteLine($"Preview: ");
            _outputHelper.WriteLine(@"\begin{align}");
            _outputHelper.WriteLine($"f(a) &= {ans.ToLatex()} \\\\");
            _outputHelper.WriteLine($"f'(a) &= {dans.ToLatex()} \\\\");
            _outputHelper.WriteLine($"define \\ a &= {value} \\\\");
            _outputHelper.WriteLine($"f(a) &= {ans.Simplify().ToLatex()} \\\\");
            _outputHelper.WriteLine($"f'(a) &= {dans.Simplify().ToLatex()}");
            _outputHelper.WriteLine(@"\end{align}");
            VarExpr.Undefine("a");
        }

        [Fact]
        public void ExprParseTest()
        {
            string[] funcs =
            {
                "(a^2/a^3) / (2*a/(3*a))",
                "sin(a) + cos(a)",
                "ln(a) / (2 * ln(3*a))",
                "a^2*a^5*8*3^2*a^3"
            };
            foreach(var func in funcs)
            {
                _outputHelper.WriteLine($"===================================");
                _outputHelper.WriteLine($"f(a) := {func} ");
                DoParserTest(func);
                _outputHelper.WriteLine($"===================================");
            }
        }

        [Fact]
        public void DExprTest()
        {
            string[] funcs =
            {
                "sin(a) + cos(a)",
                "arctan(a) + arccos(a)",
                "(2*a+3)/(a^3-b)",
                "a*b*c*3",
                "(a/(b/a))/(a^2/b)"
            };
            foreach (var func in funcs)
            {
                _outputHelper.WriteLine($"===================================");
                _outputHelper.WriteLine($"f(a) := {func} ");
                DoParserTest(func);
                _outputHelper.WriteLine($"===================================");
            }
        }

        [Fact]
        public void FuncExprTest()
        {
            string[] funcs =
            {
                "floor((5*a)/(4*b))",
                "abs(2a)+abs(-2a)",
                "(2*a+3)/(a^3-b)",
                "a*b*c*3",
                "(a/(b/a))/(a^2/b)"
            };
            foreach (var func in funcs)
            {
                _outputHelper.WriteLine($"===================================");
                _outputHelper.WriteLine($"f(a) := {func} ");
                DoParserTest(func);
                _outputHelper.WriteLine($"===================================");
            }
        }
    }
}