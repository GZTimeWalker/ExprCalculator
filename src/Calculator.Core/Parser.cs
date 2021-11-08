using Calculator.Core.Expr;
namespace Calculator.Core;
public class Parser {
    private static List<Token> Tokens { get; set; } = new List<Token>();
    private static int Current { get; set; } = 0;

    public static IExpr Parse(string expression) {
        Tokens = new Lexer(expression).GetTokens();
        Current = 0;
        var ret = ParseAddExpr();
        if (Current != Tokens.Count)
            throw new UnexpectedTokenException(TokenType.EOF, Tokens[Current]);
        return ret;
    }

    private static bool Peek(TokenType type) {
        if (Current < Tokens.Count)
            return type.HasFlag(Tokens[Current].Type);
        return false;
    }

    private static Token Match() {
        if (Current < Tokens.Count)
            return Tokens[Current++];
        throw new UnexpectedEOFException();
    }
    private static Token Match(TokenType type) {
        if (Peek(type))
            return Tokens[Current++];
        if (Current >= Tokens.Count)
            throw new UnexpectedEOFException();
        throw new UnexpectedTokenException(type, Tokens[Current]);
    }

    private static IExpr ParseAddExpr() {
        IExpr ret = ParseMulExpr();
        while (Peek(TokenType.Plus | TokenType.Minus)) {
            Token op = Match();
            IExpr right = ParseMulExpr();
            ret = op.Type
            switch {
                TokenType.Plus => new AddExpr(ret, right),
                TokenType.Minus => new SubExpr(ret, right),
                _ =>
                throw new InvalidTokenException(op)
            };
        }
        return ret;
    }

    private static IExpr ParseMulExpr() {
        IExpr ret = ParsePowExpr();
        while (Peek(TokenType.Multiply | TokenType.Divide | TokenType.Name | TokenType.LParen)) {
            if(Peek(TokenType.Name | TokenType.LParen))
            {
                ret = new MulExpr(ret, ParsePowExpr());
                continue;
            }
            Token op = Match();
            IExpr right = ParsePowExpr();
            ret = op.Type
            switch {
                TokenType.Multiply => new MulExpr(ret, right),
                TokenType.Divide => new DivExpr(ret, right),
                _ =>
                throw new InvalidTokenException(op)
            };
        }
        return ret;
    }

    private static IExpr ParsePowExpr() {
        IExpr ret = ParseUnaryExpr();
        while (Peek(TokenType.Power)) {
            Token op = Match();
            IExpr right = ParseUnaryExpr();
            ret = op.Type
            switch {
                TokenType.Power => new PowExpr(ret, right),
                _ =>
                throw new InvalidTokenException(op)
            };
        }
        return ret;
    }

    private static IExpr ParseUnaryExpr() {
        bool neg = false;
        if (Peek(TokenType.Minus)) {
            neg = true;
            Match();
        }

        IExpr ret = ParsrPrimary();

        if (neg)
            ret = new NegExpr(ret);
        return ret;
    }

    private static IExpr ParsrPrimary() {
        if (Peek(TokenType.Number))
            return new ConstExpr(Match().Value);

        if (Peek(TokenType.LParen)) {
            Match(TokenType.LParen);
            IExpr ret = ParseAddExpr();
            Match(TokenType.RParen);
            return ret;
        }

        if(Peek(TokenType.Functions))
        {
            var func = Match();
            Match(TokenType.LParen);
            IExpr ret = ParseAddExpr();
            Match(TokenType.RParen);
            // Sin | Cos | Tan | Log | Sqrt | ArcSin | ArcCos | ArcTan | Exp | Ln | Cot | Csc | Sec | Abs | Floor | Ceil | Round | Sign
            return func.Type switch
            {
                TokenType.Sin => new SinExpr(ret),
                TokenType.Cos => new CosExpr(ret),
                TokenType.Tan => new TanExpr(ret),
                TokenType.Log => new LogExpr(ret),
                TokenType.Sqrt => new SqrtExpr(ret),
                TokenType.ArcSin => new ArcSinExpr(ret),
                TokenType.ArcCos => new ArcCosExpr(ret),
                TokenType.ArcTan => new ArcTanExpr(ret),
                TokenType.Exp => new ExpExpr(ret),
                TokenType.Ln => new LnExpr(ret),
                TokenType.Cot => new CotExpr(ret),
                TokenType.Csc => new CscExpr(ret),
                TokenType.Sec => new SecExpr(ret),
                TokenType.Abs => new AbsExpr(ret),
                TokenType.Floor => new FloorExpr(ret),
                TokenType.Ceil => new CeilExpr(ret),
                TokenType.Round => new RoundExpr(ret),
                TokenType.Sign => new SignExpr(ret),
                TokenType.Simplify => ret.WithVar().Simplify(),
                _ => throw new InvalidTokenException(func)
            };
        }

        if(Peek(TokenType.Derivative))
        {
            Match(TokenType.Derivative);
            Match(TokenType.LParen);
            string name = Match(TokenType.Name).Value!;
            Match(TokenType.Comma);
            IExpr ret = ParseAddExpr().WithVar();
            Match(TokenType.RParen);
            return ret.D(name);
        }

        if(Peek(TokenType.Name))
        {
            var name = Match();
            return new VarExpr(name.Value);
        }

        throw new InvalidTokenException(Tokens[Current]);
    }
}
