namespace Calculator.Core;

public class UndefinedVariableException : Exception {
    public UndefinedVariableException(string message) : base(message) { }
}

public class UnsupportedOperationException : Exception {
    public UnsupportedOperationException(string message) : base(message) { }
}

public class UnknownTokenException : Exception {
    public UnknownTokenException(int position) : base($"Unknown Token at {position}") { }
}

public class InvalidTokenException : Exception {
    public InvalidTokenException(Token token) : base($"Invalid Token {token}") { }
}

public class UnexpectedEOFException : Exception {
    public UnexpectedEOFException() : base("Unexpected EOF") { }
}

public class UnexpectedTokenException : Exception {
    public UnexpectedTokenException(TokenType expected, Token actual) : base($"Expected Token {Token.TokenName[expected]} but got Token {actual}.") { }
}
