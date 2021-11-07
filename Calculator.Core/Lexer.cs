namespace Calculator.Core;
public class Lexer {
    private readonly string Input;
    private int Position { get; set; } = 0;

    public Lexer(string input) {
        Input = input.ToLower() ?? string.Empty;
    }

    public List<Token> GetTokens() {
        var tokens = new List<Token>();
        do {
            var token = Advance();
            if (token.Type == TokenType.None)
                throw new UnknownTokenException(Position);
            tokens.Add(token);
        } while (!Finished());
        return tokens;
    }

    private bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private bool IsSpace(char c) {
        return " \n\t\r".Contains(c);
    }

    private bool IsDigit(char c) {
        return (c >= '0' && c <= '9') || c == '.';
    }

    private bool Finished() {
        return Position >= Input.Length;
    }

    private bool Match(Func<char, bool> cond) {
        if (Finished())
            return false;

        if (cond(Input[Position])) {
            Position++;
            return true;
        }

        return false;
    }

    private bool Match(char c) {
        return Match(x => x == c);
    }

    private Token Advance() {
        while (Match(IsSpace));

        if (Match('+'))
            return new Token(TokenType.Plus, "+");

        if (Match('-'))
            return new Token(TokenType.Minus, "-");

        if (Match('*'))
            return new Token(TokenType.Multiply, "*");

        if (Match('/'))
            return new Token(TokenType.Divide, "/");

        if (Match('('))
            return new Token(TokenType.LParen, "(");

        if (Match(')'))
            return new Token(TokenType.RParen, ")");

        if (Match('^'))
            return new Token(TokenType.Power, "^");

        if (Match(IsDigit)) {
            var start = Position - 1;
            while (Match(IsDigit));
            return new Token(TokenType.Number, Input.Substring(start, Position - start));
        }

        if (Match(IsAlpha)) {
            var start = Position - 1;
            while (Match(IsAlpha));
            var name = Input.Substring(start, Position - start);
            return name
            // Sin | Cos | Tan | Log | Sqrt | ArcSin | ArcCos | ArcTan | Exp | Ln | Cot | Csc | Sec | Abs | Floor | Ceil | Round | Sign
            switch {
                "sin" => new Token(TokenType.Sin, "sin"),
                "cos" => new Token(TokenType.Cos, "cos"),
                "tan" => new Token(TokenType.Tan, "tan"),
                "log" => new Token(TokenType.Log, "log"),
                "sqrt" => new Token(TokenType.Sqrt, "sqrt"),
                "arcsin" => new Token(TokenType.ArcSin, "arcsin"),
                "arccos" => new Token(TokenType.ArcCos, "arccos"),
                "arctan" => new Token(TokenType.ArcTan, "arctan"),
                "exp" => new Token(TokenType.Exp, "exp"),
                "ln" => new Token(TokenType.Ln, "ln"),
                "cot" => new Token(TokenType.Cot, "cot"),
                "csc" => new Token(TokenType.Csc, "csc"),
                "sec" => new Token(TokenType.Sec, "sec"),
                "abs" => new Token(TokenType.Abs, "abs"),
                "floor" => new Token(TokenType.Floor, "floor"),
                "ceil" => new Token(TokenType.Ceil, "ceil"),
                "round" => new Token(TokenType.Round, "round"),
                "sign" => new Token(TokenType.Sign, "sign"),
                _ => new Token(TokenType.Name, name)
            };
        }

        return new Token(TokenType.None);
    }
}
