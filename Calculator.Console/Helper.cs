using Calculator.Core;
using Calculator.Core.Expr;
using System;

namespace Calculator;

public static class Helper
{
    public static void PrintHello()
    {
        string bound = "===============================================================================";
        string[] content =
        {
            "",
            " ██████╗██╗  ██╗██╗      ██████╗██╗   ██╗ ██╗ █████╗ ████████╗ ██████╗ ██████╗ ",
            "██╔════╝██║  ██║██║     ██╔════╝██║   ██║███║██╔══██╗╚══██╔══╝██╔═████╗██╔══██╗",
            "██║     ███████║██║     ██║     ██║   ██║╚██║███████║   ██║   ██║██╔██║██████╔╝",
            "██║     ╚════██║██║     ██║     ██║   ██║ ██║██╔══██║   ██║   ████╔╝██║██╔══██╗",
            "╚██████╗     ██║███████╗╚██████╗╚██████╔╝ ██║██║  ██║   ██║   ╚██████╔╝██║  ██║",
            " ╚═════╝     ╚═╝╚══════╝ ╚═════╝ ╚═════╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝",
            "                                                                               ",
        };

        string sign = " By GZTime 2021/11/7";
        sign = new String(' ', bound.Length - sign.Length) + sign;

        string help = " Type 'help' for help.";
        help = new String(' ', bound.Length - help.Length) + help;

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(bound);

        Console.ForegroundColor = ConsoleColor.Green;
        foreach (string line in content)
            Console.WriteLine(line);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(bound + '\n');

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(sign + '\n');
        Console.WriteLine(help);

        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void PrintHelper()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n    C4LCU1AT0R Version 1.0");
        Console.WriteLine("     By GZTime 2021/11/07");
        Console.WriteLine("\n Usage: \n");
        string[] content =
        {
            "- [Expression]",
            "    Print a expression in pure text.\n",
            "- [Variable] = [Expression]",
            "    Define a variable with the expression.\n",
            "- eval [Expression]",
            "    Evaluate the expression with known values.\n",
            "- latex [Expression]",
            "    Show the expression in LaTeX format.\n",
            "- show [Expression]",
            "    Show the expression in a tree format.\n",
            "- ![Variable]",
            "    Undefine a variable.\n",
            "- help",
            "    Show this help.\n",
            "- exit",
            "    Exit the program.\n",
            "- clear",
            "    Undefine all variables.\n",
            "- #([Expression])",
            "    Simplify the expression.\n",
            "- d([Variable],[Expression])",
            "    Take the partial derivative of an expression with respect to a variable.\n",
        };

        Console.ForegroundColor = ConsoleColor.Cyan;
        for(var i = 0; i < content.Length; i++)
        {
            Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
            Console.WriteLine(content[i]);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n Supported Functions: \n");

        string[] funcs =
        {
            "    sin | cos | tan | log | sqrt | arcsin | arccos | arctan | exp | ln",
            "    cot | csc | sec | abs | floor | ceil | round | sign"
        };

        Console.ForegroundColor = ConsoleColor.Cyan;
        foreach (string line in funcs)
            Console.WriteLine(line);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Try these Examples:\n");

        string[] examples =
        {
            ">>> a = 1+2-(3*4-6/sin(x))",
            ">>> show a",
            ">>> a = #(a)",
            ">>> latex a",
            ">>> b = #(d(x,a))",
            ">>> c = x^x",
            ">>> c = #(d(x,c))",
            ">>> show c",
            ">>> x = 1/e",
            ">>> eval c",
            ">>> x = e",
            ">>> eval c",
            ""
        };

        Console.ForegroundColor = ConsoleColor.Gray;
        foreach (string line in examples)
            Console.WriteLine(line);
    }
}
