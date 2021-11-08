using Calculator.Core.Expr;

namespace Calculator.Core;

public static class Utils {
    private record struct PrintItem(string Content = " ", int Offset = 0, ConsoleColor Color = ConsoleColor.White);
    private static List<Queue<PrintItem>> PrintItems {get; set;} = new();
    private static int Width {get; set;} = 0;
    private static int Depth {get; set;} = 0;

    private static void ShowPrintItems()
    {
        for(var d = 0; d < Depth * 2; ++d)
        {
            var line = PrintItems[d];
            if (line.Count == 0)
                continue;
            var now = line.Dequeue();
            for (var i = 0; i < Width;)
            {
                if (i == now.Offset)
                {
                    Console.ForegroundColor = now.Color;
                    Console.Write(now.Content);
                    i += now.Content.Length;
                    if (line.Count == 0)
                        break;
                    now = line.Dequeue();
                }
                else
                {
                    Console.Write(' ');
                    ++i;
                }
            }
            Console.WriteLine();
        }
    }

    private static void SolveBinaryBranch(int depth, int start, int middle, int end)
    {
        PrintItems[depth].Enqueue(new("┌", start, ConsoleColor.DarkGray));
        if (middle - start > 1)
            PrintItems[depth].Enqueue(new(new('─', middle - start - 1), start + 1, ConsoleColor.DarkGray));
        PrintItems[depth].Enqueue(new("┴", middle, ConsoleColor.DarkGray));
        if (end - middle > 1)
            PrintItems[depth].Enqueue(new(new('─', end - middle - 1), middle + 1, ConsoleColor.DarkGray));
        PrintItems[depth].Enqueue(new("┐", end, ConsoleColor.DarkGray));
        return;
    }

    private static void SolvePrintItem(IExpr expr, int depth = 0, int offset = 0)
    {
        if(expr is BinaryExpr binaryExpr)
        {
            var leftwidth = binaryExpr.LeftWidth();

            string op = binaryExpr switch {
                AddExpr _ => "+",
                SubExpr _ => "-",
                MulExpr _ => "*",
                DivExpr _ => "/",
                PowExpr _ => "^",
                _ => throw new Exception("Unknown operator")
            };

            if (offset == 0)
                offset = leftwidth + 4;

            PrintItems[depth].Enqueue(new PrintItem(op, offset, ConsoleColor.Cyan));

            var leftchild = offset - binaryExpr.Left.RightWidth() - 1;
            var rightchild = offset + binaryExpr.Right.LeftWidth() + 1;

            SolveBinaryBranch(depth + 1, leftchild, offset, rightchild);

            // left branch
            // [        ] + [-]
            SolvePrintItem(binaryExpr.Left, depth + 2, leftchild);
            // right branch
            // [--------] + [ ]
            SolvePrintItem(binaryExpr.Right, depth + 2, rightchild);
        }
        else if(expr is UnaryExpr unaryExpr)
        {
            var leftwidth = unaryExpr.LeftWidth();

            var showText = unaryExpr switch {
                NegExpr _ => "-",
                FuncExpr f => f.FuncName,
                _ => throw new Exception("Unknown expression")
            };

            if(offset == 0)
                offset = leftwidth + 4;


            PrintItems[depth].Enqueue(new PrintItem(showText,
                offset - showText.Length / 2,
                ConsoleColor.Blue));

            PrintItems[depth + 1].Enqueue(new PrintItem("│", offset, ConsoleColor.DarkGray));

            SolvePrintItem(unaryExpr.Operand, depth + 2, offset);
        }
        else if(expr is ConstExpr constExpr)
        {
            var showText = Math.Round(constExpr.Value, 2).ToString();
            PrintItems[depth].Enqueue(new PrintItem(showText,
                offset - showText.Length / 2,
                ConsoleColor.Green));
        }
        else if(expr is VarExpr varExpr)
        {
            PrintItems[depth].Enqueue(new PrintItem(varExpr.Identifier,
                offset - varExpr.Identifier.Length / 2,
                ConsoleColor.Yellow));
        }
        else throw new ArgumentException("Unknown expression type: " + expr.GetType().Name);
    }

    public static void PrettyPrintExprTree(IExpr expr) {
        Depth = expr.Depth();
        Width = Console.WindowWidth;
        PrintItems.Clear();
        for (int depth = 0; depth < Depth * 2; ++depth)
            PrintItems.Add(new());
        SolvePrintItem(expr);
        ShowPrintItems();
        Console.ForegroundColor = ConsoleColor.White;
    }
}
