namespace Calculator.Core.Expr;

/// <summary>
/// The Interface for Expressions.
/// </summary>
public interface IExpr : ICloneable {
    /// <summary>
    /// Evaluates the expression.
    /// </summary>
    /// <returns></returns>
    public double Eval();
    /// <summary>
    /// Differentiates the expression.
    /// </summary>
    IExpr D(string byVar);
    /// <summary>
    /// Simplifies the expression.
    /// </summary>
    IExpr Simplify();
    /// <summary>
    /// Converts the expression to a latex string.
    /// </summary>
    /// <returns></returns>
    string ToLatex();
    /// <summary>
    /// Get the width of this expr.
    /// </summary>
    /// <returns>the width of this expr</returns>
    int Width();
    /// <summary>
    /// Get the left width of the node.
    /// </summary>
    /// <returns>the left width of the node</returns>
    int LeftWidth();
    /// <summary>
    /// Get the right width of the node.
    /// </summary>
    /// <returns>the right width of the node</returns>
    int RightWidth();
    /// <summary>
    /// Get the depth of the node.
    /// </summary>
    int Depth();
}

/// <summary>
/// Constant Expression.
/// </summary>
public sealed class ConstExpr : IExpr {
    public double Value { get; private set; }
    public ConstExpr(double val) {
        Value = val;
    }
    public ConstExpr(string? val) {
        if (val is null)
            throw new ArgumentNullException(nameof(val));
        Value = double.Parse(val);
    }
    public double Eval() => Value;
    public IExpr D(string byVar) => new ConstExpr(0);
    public IExpr Simplify() => new ConstExpr(Value);

    public override string ToString() => Math.Round(Value, 4).ToString();
    public string ToLatex() => Math.Round(Value, 4).ToString();
    public object Clone() => new ConstExpr(Value);
    public int Width()
    {
        var len = Math.Round(Value, 2).ToString().Length;
        return len % 2 == 0 ? len + 1 : len;
    }

    public int LeftWidth() => Math.Round(Value, 2).ToString().Length / 2;
    public int RightWidth() => Math.Round(Value, 2).ToString().Length / 2;
    public int Depth() => 1;
}

/// <summary>
/// Variable Expression.
/// </summary>
public sealed class VarExpr : IExpr {

    private static SortedList<string, double> Environment { get; set; }
    /// <summary>
    /// Identifier of the variable.
    /// </summary>
    public string Identifier { get; private set; }
    /// <summary>
    /// Is the variable in the environment?
    /// </summary>
    public bool HasVar { get { return Environment.ContainsKey(Identifier); } }
    /// <summary>
    /// Get the value of the variable.
    /// </summary>
    public double Value {
        get {
            if (HasVar) {
                return (double) Environment[Identifier];
            }
            throw new UndefinedVariableException(Identifier + " is not defined.");
        }
    }

    public VarExpr(string? id) {
        if (id is null)
            throw new ArgumentNullException(nameof(id));
        Identifier = id;
    }

    static VarExpr() {
        Environment = new SortedList<string, double>();
        Environment.Add("pi", Math.PI);
        Environment.Add("e", Math.E);
    }

    /// <summary>
    /// Defines a variable in the environment.
    /// </summary>
    /// <param name="varName"> Variable Name </param>
    /// <param name="value"> Variable Value </param>
    public static void Define(string varName, double value) {
        varName = varName.ToLower();
        if (Environment.ContainsKey(varName)) {
            Environment[varName] = value;
        } else {
            Environment.Add(varName, value);
        }
    }

    /// <summary>
    /// Undefines a variable in the environment.
    /// </summary>
    /// <param name="varName"> Variable Name </param>
    public static void Undefine(string varName) {
        varName = varName.ToLower();
        if (Environment.ContainsKey(varName)) {
            Environment.Remove(varName);
        }
    }

    public double Eval() => Value;
    public IExpr D(string byVar) => new ConstExpr(Identifier == byVar.ToLower() ? 1 : 0);
    public IExpr Simplify() => HasVar ? new ConstExpr(Value) : new VarExpr(Identifier);
    public override string ToString() => Identifier;
    public string ToLatex() => Identifier;
    public object Clone() => new VarExpr(Identifier);
    public int Width()
    {
        var len = Identifier.Length;
        return len % 2 == 0 ? len + 1 : len;
    }

    public int LeftWidth() => Identifier.Length / 2;
    public int RightWidth() => Identifier.Length / 2;

    public int Depth() => 1;
}

/// <summary>
/// Binary Expression.
/// </summary>
public abstract class BinaryExpr : IExpr {
    public IExpr Left { get; private set; }
    public IExpr Right { get; private set; }
    protected abstract double Operate(double left, double right);

    public BinaryExpr(IExpr left, IExpr right) {
        Left = left;
        Right = right;
    }

    #region Monomial record
    protected record Monomial(double Coefficient, double Exponent, string? Identifier) {
        public IExpr AsExpr() {
            if (Identifier is null)
                return new ConstExpr(Coefficient);

            if (Exponent == 0)
                return new ConstExpr(Coefficient);

            if (Coefficient == 0)
                return new ConstExpr(0);

            if(Exponent == 1)
            {
                if(Coefficient == 1)
                    return new VarExpr(Identifier);
                return new MulExpr(new ConstExpr(Coefficient), new VarExpr(Identifier));
            }

            if(Coefficient == 1)
                return new PowExpr(new VarExpr(Identifier), new ConstExpr(Exponent));

            return new MulExpr(new ConstExpr(Coefficient), new PowExpr(new VarExpr(Identifier), new ConstExpr(Exponent)));
        }
    }

    protected static Monomial? AsMonomial(IExpr expr) {
        if (expr is ConstExpr constExpr)
            return new Monomial(constExpr.Value, 0, null);

        if (expr is VarExpr varExpr)
            return new Monomial(1, 1, varExpr.Identifier);

        if (expr is PowExpr powExpr && powExpr.Left is VarExpr powBase && powExpr.Right is ConstExpr powExp)
            return new Monomial(1, powExp.Value, powBase.Identifier);

        if (expr is MulExpr mulExpr) {
            var left = AsMonomial(mulExpr.Left);
            var right = AsMonomial(mulExpr.Right);

            if (left is null || right is null)
                return null;

            if (left.Identifier is not null && right.Identifier is not null && left.Identifier != right.Identifier)
                return null;

            return new Monomial(
                left.Coefficient * right.Coefficient,
                left.Exponent + right.Exponent,
                left.Identifier ?? right.Identifier);
        }

        return null;
    }
    #endregion
    public double Eval() => Operate(Left.Eval(), Right.Eval());
    public abstract IExpr D(string byVar);
    public abstract IExpr Simplify();
    public abstract string ToLatex();
    public abstract object Clone();
    // for all binary expr only have a one char operator
    public int Width() => Left.Width() + Right.Width() + 1;
    public int LeftWidth() => Left.Width();
    public int RightWidth() => Right.Width();

    public int Depth() => Math.Max(Left.Depth(), Right.Depth()) + 1;
}

/// <summary>
/// Unary Expression.
/// </summary>
public abstract class UnaryExpr : IExpr {
    public IExpr Operand { get; private set; }
    protected abstract double Operate(double operand);

    public UnaryExpr(IExpr operand) {
        Operand = operand;
    }

    public double Eval() => Operate(Operand.Eval());
    public abstract IExpr D(string byVar);
    public abstract IExpr Simplify();
    public abstract string ToLatex();
    public abstract object Clone();
    public abstract int Width();
    public abstract int LeftWidth();
    public abstract int RightWidth();
    public int Depth() => Operand.Depth() + 1;
}
