using Calculator;
using Calculator.Core;
using Calculator.Core.Expr;

Helper.PrintHello();
string? input;

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(">>> ");
    Console.ForegroundColor = ConsoleColor.White;

    input = Console.ReadLine();
    if (input is null)
        continue;

    if (input == "help")
    {
        Helper.PrintHelper();
        continue;
    }

    if (input == "exit")
    {
        Console.WriteLine("Bye bye~");
        break;
    }

    if (input == "clear")
    {
        VarExpr.ClearVars();
        continue;
    }

    try
    {
        if (input.Contains('='))
        {
            var pos = input.IndexOf('=');
            var varName = input[..pos].Trim().ToLower();
            var content = input[(pos + 1)..].Trim();
            VarExpr.Define(varName, Parser.Parse(content));
        }
        else if(input.StartsWith('!'))
        {
            var varName = input[1..];
            VarExpr.Undefine(varName);
        }
        else if(input.StartsWith("latex "))
        {
            var content = input[6..].Trim();
            var res = Parser.Parse(content).WithVar().ToLatex();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.Write("[+] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(res);
        }
        else if(input.StartsWith("show "))
        {
            var content = input[5..].Trim();
            Utils.PrettyPrintExprTree(Parser.Parse(content).WithVar());
        }
        else if (input.StartsWith("eval "))
        {
            var content = input[5..].Trim();
            var res = Parser.Parse(content).WithVar().Eval();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.Write("[+] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(res);
        }
        else
        {
            var res = Parser.Parse(input).WithVar();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.Write("[+] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(res);
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("[!] An exception has occurred!");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Error.Write("[-] ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Error.WriteLine(ex.Message);
    }
}