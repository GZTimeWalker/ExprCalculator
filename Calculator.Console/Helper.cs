using Calculator.Core;
using Calculator.Core.Expr;

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

        string sign = " By 20308010 2021/11/7";
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
        string[] content =
        {

        };
    }
}
