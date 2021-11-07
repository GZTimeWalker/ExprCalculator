using Calculator.Core.Expr;

namespace Calculator.Core;

public static class Utils {
    private record ExprItem(string Content, int Width, int Depth, ConsoleColor Color, bool IsBinary = false);

    private static List<ExprItem> InOrderExprList { get; set; } = new();
    private static Queue<bool> IsBinary { get; set; } = new();
    private static int MaxDepth { get; set; } = 0;
    private static int State { get; set; } = 0;

    private static void Fill(int length, string content = " ")
    {
        if(length == 0)
            return;
        for (var i = 0; i < length; i++)
            Console.Write(content);
    }

    private static void GetInOrder(IExpr expr, int depth = 0, int totWidth = 0) {
        MaxDepth = Math.Max(MaxDepth, depth);
        switch (expr) {
            case VarExpr varExpr:
                InOrderExprList.Add(new ExprItem(varExpr.Identifier,
                    totWidth == 0 ? varExpr.Width() : totWidth,
                    depth, ConsoleColor.Yellow));
                break;
            case ConstExpr constExpr:
                InOrderExprList.Add(new ExprItem(Math.Round(constExpr.Value, 2).ToString(),
                    totWidth == 0 ? constExpr.Width() : totWidth,
                    depth, ConsoleColor.Green));
                break;
            case NegExpr negExpr:
                InOrderExprList.Add(new ExprItem("-",
                    totWidth == 0 ? negExpr.Width() : totWidth,
                    depth, ConsoleColor.Cyan));
                GetInOrder(negExpr.Operand, depth + 1,
                    totWidth == 0 ? negExpr.TotalWidth() : totWidth);
                break;
            case FuncExpr funcExpr:
                InOrderExprList.Add(new ExprItem(funcExpr.FuncName,
                    totWidth == 0 ? funcExpr.Width() : totWidth, depth, ConsoleColor.Blue));
                GetInOrder(funcExpr.Operand, depth + 1,
                    totWidth == 0 ? funcExpr.TotalWidth() : totWidth);
                break;
            case BinaryExpr binaryExpr:
                GetInOrder(binaryExpr.Left, depth + 1);
                string sign = binaryExpr switch {
                    AddExpr _ => "+",
                    SubExpr _ => "-",
                    MulExpr _ => "*",
                    DivExpr _ => "/",
                    PowExpr _ => "^",
                    _ => throw new Exception("Unknown binary expression")
                };
                InOrderExprList.Add(new ExprItem(sign, binaryExpr.Width(), depth, ConsoleColor.Cyan, true));
                GetInOrder(binaryExpr.Right, depth + 1);
                break;
            default:
                throw new Exception("Unknown expression");
        }
    }

    private static void PrintTree() {
        for (var i = 0; i <= MaxDepth; ++i) {
            IsBinary.Clear();
            foreach (var item in InOrderExprList) {
                if (item.Depth == i) {
                    Console.ForegroundColor = item.Color;
                    int paddingLength = item.Width - item.Content.Length;
                    if(item.Content.Length % 2 == 0)
                    {
                        paddingLength = (paddingLength + 1) / 2;
                        Fill(paddingLength);
                        Console.Write(item.Content);
                        Fill(paddingLength + 1);
                    }
                    else
                    {
                        paddingLength /= 2;
                        Fill(paddingLength);
                        Console.Write(item.Content);
                        Fill(paddingLength);
                    }
                    IsBinary.Enqueue(item.IsBinary);
                }
                else if(item.IsBinary)
                {
                    Fill(item.Width);
                }
            }
            Console.WriteLine();
            State = 0;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var item in InOrderExprList)
            {
                // print blank
                if (State == 0)
                {
                    if (item.Depth == i + 1)
                    {
                        if (IsBinary.Dequeue())
                        {
                            Fill(item.Width / 2);
                            Console.Write("┌");
                            Fill(item.Width / 2, "─");
                            State = 1;
                        }
                        else
                        {
                            Fill(item.Width / 2);
                            Console.Write("│");
                            Fill(item.Width / 2);
                        }
                    }
                    else if(item.Depth == i && item.IsBinary)
                    {
                        Fill(item.Width);
                    }
                }
                // after left bound
                else if (State == 1)
                {
                    if(item.Depth == i)
                    {
                        Fill(item.Width / 2, "─");
                        Console.Write("┴");
                        Fill(item.Width / 2, "─");
                        State = 2;
                    }
                    else
                    {
                        Fill(item.Width, "─");
                    }
                }
                // after middle
                else if(State == 2)
                {
                    if(item.Depth == i + 1)
                    {
                        Fill(item.Width / 2, "─");
                        Console.Write("┐");
                        Fill(item.Width / 2);
                        State = 0;
                    }
                    else
                    {
                        Fill(item.Width, "─");
                    }
                }
            }
            Console.WriteLine();
        }
    }

    public static void PrettyPrintExprTree(IExpr expr) {
        InOrderExprList.Clear();
        IsBinary.Clear();
        GetInOrder(expr);
        PrintTree();
        Console.ForegroundColor = ConsoleColor.White;
    }
}
