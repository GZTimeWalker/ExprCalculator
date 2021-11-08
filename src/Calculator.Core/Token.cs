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
    Simplify = 0x8000000,
    Functions = Sin | Cos | Tan | Log | Sqrt | ArcSin |
                ArcCos | ArcTan | Exp | Ln | Cot | Csc |
                Sec | Abs | Floor | Ceil | Round | Sign | Simplify,
    Derivative = 0x10000000,
    Comma = 0x20000000,
    EOF = 0x10000000
}

public record struct Token(TokenType Type, string? Value = null)
{
    public static readonly Dictionary<TokenType, string> TokenName = new()
    {
        [TokenType.Number] = nameof(TokenType.Number),
        [TokenType.Name] = nameof(TokenType.Name),
        [TokenType.Plus] = nameof(TokenType.Plus),
        [TokenType.Minus] = nameof(TokenType.Minus),
        [TokenType.Multiply] = nameof(TokenType.Multiply),
        [TokenType.Divide] = nameof(TokenType.Divide),
        [TokenType.Power] = nameof(TokenType.Power),
        [TokenType.LParen] = nameof(TokenType.LParen),
        [TokenType.RParen] = nameof(TokenType.RParen),
        [TokenType.Sin] = nameof(TokenType.Sin),
        [TokenType.Cos] = nameof(TokenType.Cos),
        [TokenType.Tan] = nameof(TokenType.Tan),
        [TokenType.Log] = nameof(TokenType.Log),
        [TokenType.ArcSin] = nameof(TokenType.ArcSin),
        [TokenType.ArcCos] = nameof(TokenType.ArcCos),
        [TokenType.ArcTan] = nameof(TokenType.ArcTan),
        [TokenType.Sqrt] = nameof(TokenType.Sqrt),
        [TokenType.Exp] = nameof(TokenType.Exp),
        [TokenType.Ln] = nameof(TokenType.Ln),
        [TokenType.Cot] = nameof(TokenType.Cot),
        [TokenType.Csc] = nameof(TokenType.Csc),
        [TokenType.Sec] = nameof(TokenType.Sec),
        [TokenType.Abs] = nameof(TokenType.Abs),
        [TokenType.Floor] = nameof(TokenType.Floor),
        [TokenType.Ceil] = nameof(TokenType.Ceil),
        [TokenType.Round] = nameof(TokenType.Round),
        [TokenType.Sign] = nameof(TokenType.Sign),
        [TokenType.Simplify] = nameof(TokenType.Simplify),
        [TokenType.Derivative] = nameof(TokenType.Derivative),
        [TokenType.Comma] = nameof(TokenType.Comma),
        [TokenType.EOF] = nameof(TokenType.EOF)
    };
    public override string ToString() => $"{TokenName[Type]} ({Value})";
}
