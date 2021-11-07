namespace Calculator.Core;

[Flags]
public enum TokenType
{
    None = 0x0,
    Number = 0x1,
    Name = 0x2,
    Plus = 0x4,
    Minus = 0x8,
    Multiply = 0x10,
    Divide = 0x20,
    Power = 0x40,
    LParen = 0x80,
    RParen = 0x100,
    Sin = 0x200,
    Cos = 0x400,
    Tan = 0x800,
    Log = 0x1000,
    ArcSin = 0x2000,
    ArcCos = 0x4000,
    ArcTan = 0x8000,
    Sqrt = 0x10000,
    Exp = 0x20000,
    Ln = 0x40000,
    Cot = 0x80000,
    Csc = 0x100000,
    Sec = 0x200000,
    Abs = 0x400000,
    Floor = 0x800000,
    Ceil = 0x1000000,
    Round = 0x2000000,
    Sign = 0x4000000,
    Functions = Sin | Cos | Tan | Log | Sqrt | ArcSin | ArcCos | ArcTan | Exp | Ln | Cot | Csc | Sec | Abs | Floor | Ceil | Round | Sign,
    EOF = 0x10000000
}

public record struct Token(TokenType Type, string? Value = null)
{
    public override string ToString() => $"{Type} ({Value})";
}
