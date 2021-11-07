namespace Calculator.Core.Expr;

public sealed class AddExpr : BinaryExpr {
    public AddExpr(IExpr left, IExpr right) : base(left, right) { }

    protected override double Operate(double left, double right) => left + right;

    // d(f(x) + g(x)) = f'(x) + g'(x)
    public override IExpr D(string byVar) => new AddExpr(Left.D(byVar), Right.D(byVar));

    public override IExpr Simplify() {
        var left = Left.Simplify();
        var right = Right.Simplify();

        // const + const
        if (left is ConstExpr && right is ConstExpr)
            return new ConstExpr(Operate(((ConstExpr)left).Value, ((ConstExpr)right).Value));

        // x + x -> 2*x
        if(left is VarExpr && right is VarExpr)
        {
            if(((VarExpr)left).Identifier == ((VarExpr)right).Identifier)
                return new MulExpr(new ConstExpr(2), left);
        }

        // 0 + x -> x
        if (left is ConstExpr && ((ConstExpr)left).Value == 0)
            return right;

        // x + 0 -> x
        if (right is ConstExpr && ((ConstExpr)right).Value == 0)
            return left;

        // -x + y -> y - x
        if (left is NegExpr)
            return new SubExpr(right, ((NegExpr)left).Operand).Simplify();

        // x + -y -> x - y
        if (right is NegExpr)
            return new SubExpr(left, ((NegExpr)right).Operand).Simplify();

        if(left is AddExpr addLeft && right is ConstExpr)
        {
            var constRight = (ConstExpr)right;
            // (x + const) + const -> x + const
            if(addLeft.Right is ConstExpr constLeftRight)
            {
                return new AddExpr(addLeft.Left, new ConstExpr(constLeftRight.Value + constRight.Value)).Simplify();
            }
            // (const + x) + const -> x + const
            if(addLeft.Left is ConstExpr constLeftLeft)
            {
                return new AddExpr(addLeft.Right, new ConstExpr(constLeftLeft.Value + constRight.Value)).Simplify();
            }
            return new AddExpr(new AddExpr(right, addLeft.Left), addLeft.Right).Simplify();
        }

        if(right is AddExpr addRight && left is ConstExpr)
        {
            var constLeft = (ConstExpr)left;
            // const + (x + const) -> x + const
            if(addRight.Right is ConstExpr constRightRight)
            {
                return new AddExpr(addRight.Left, new ConstExpr(constLeft.Value + constRightRight.Value)).Simplify();
            }
            // const + (const + x) -> x + const
            if(addRight.Left is ConstExpr constRightLeft)
            {
                return new AddExpr(addRight.Right, new ConstExpr(constLeft.Value + constRightLeft.Value)).Simplify();
            }
            return new AddExpr(new AddExpr(left, addRight.Left), addRight.Right).Simplify();
        }

        if(left is SubExpr subLeft && right is ConstExpr)
        {
            var constRight = (ConstExpr)right;
            // (x - const) + const -> x + const
            if(subLeft.Right is ConstExpr constLeftRight)
            {
                return new AddExpr(subLeft.Left, new ConstExpr(constRight.Value - constLeftRight.Value)).Simplify();
            }
            // (const - x) + const -> const - x
            if(subLeft.Left is ConstExpr constLeftLeft)
            {
                return new SubExpr(new ConstExpr(constLeftLeft.Value + constRight.Value), subLeft.Right).Simplify();
            }
        }

        if(right is SubExpr subRight && left is ConstExpr)
        {
            var constLeft = (ConstExpr)left;
            // const + (x - const) -> x + const
            if(subRight.Right is ConstExpr constRightRight)
            {
                return new AddExpr(subRight.Left, new ConstExpr(constLeft.Value - constRightRight.Value)).Simplify();
            }
            // const + (const - x) -> const - x
            if(subRight.Left is ConstExpr constRightLeft)
            {
                return new SubExpr(new ConstExpr(constRightLeft.Value + constLeft.Value), subRight.Right).Simplify();
            }
        }

        #region Try Monomial
        var leftMonomial = AsMonomial(left);
        var rightMonomial = AsMonomial(right);

        if(leftMonomial is not null && rightMonomial is not null)
        {
            if (leftMonomial.Identifier == rightMonomial.Identifier && leftMonomial.Exponent == rightMonomial.Exponent)
                return new Monomial(leftMonomial.Coefficient + rightMonomial.Coefficient, leftMonomial.Exponent, leftMonomial.Identifier).AsExpr();
        }
        #endregion

        return new AddExpr(left, right);
    }

    public override string ToString() => $"({Left} + {Right})";
    public override string ToLatex() => $"\\left({Left.ToLatex()} + {Right.ToLatex()}\\right)";

    public override object Clone() => new AddExpr((IExpr)Left.Clone(), (IExpr)Right.Clone());
}

public sealed class SubExpr : BinaryExpr {
    public SubExpr(IExpr left, IExpr right) : base(left, right) { }

    protected override double Operate(double left, double right) => left - right;

    // d(f(x) - g(x)) = f'(x) - g'(x)
    public override IExpr D(string byVar) => new SubExpr(Left.D(byVar), Right.D(byVar));

    public override IExpr Simplify() {
        var left = Left.Simplify();
        var right = Right.Simplify();

        // const - const
        if (left is ConstExpr && right is ConstExpr)
            return new ConstExpr(((ConstExpr)left).Value - ((ConstExpr)right).Value);

        // x - x -> 0
        if(left is VarExpr && right is VarExpr)
        {
            if(((VarExpr)left).Identifier == ((VarExpr)right).Identifier)
                return new ConstExpr(0);
        }

        // 0 - x -> -x
        if (left is ConstExpr && ((ConstExpr)left).Value == 0)
            return new NegExpr(right).Simplify();

        // x - 0 -> x
        if (right is ConstExpr && ((ConstExpr)right).Value == 0)
            return left;

        // x - -(y) -> x + y
        if (right is NegExpr)
            return new AddExpr(left, ((NegExpr)right).Operand).Simplify();

        // -(x) - y -> -(x + y)
        if(left is NegExpr)
            return new NegExpr(new AddExpr(((NegExpr)left).Operand, right)).Simplify();

        if(left is SubExpr subLeft && right is ConstExpr)
        {
            var constRight = (ConstExpr)right;
            // (x - const) - const -> x - const
            if(subLeft.Right is ConstExpr constLeftRight)
            {
                return new SubExpr(subLeft.Left, new ConstExpr(constLeftRight.Value + constRight.Value)).Simplify();
            }
            // (const - x) - const -> const - x
            if(subLeft.Left is ConstExpr constLeftLeft)
            {
                return new SubExpr(new ConstExpr(constLeftLeft.Value - constRight.Value), subLeft.Right).Simplify();
            }
        }

        if(right is SubExpr subRight && left is ConstExpr)
        {
            var constLeft = (ConstExpr)left;
            // const - (x - const) -> const - x
            if(subRight.Right is ConstExpr constRightRight)
            {
                return new SubExpr(new ConstExpr(constLeft.Value + constRightRight.Value), subRight.Left).Simplify();
            }
            // const - (const - x) -> x + const
            if(subRight.Left is ConstExpr constRightLeft)
            {
                return new AddExpr(subRight.Right, new ConstExpr(constLeft.Value - constRightLeft.Value)).Simplify();
            }
        }


        #region Try Monomial
        var leftMonomial = AsMonomial(left);
        var rightMonomial = AsMonomial(right);

        if(leftMonomial is not null && rightMonomial is not null)
        {
            if (leftMonomial.Identifier == rightMonomial.Identifier && leftMonomial.Exponent == rightMonomial.Exponent)
                return new Monomial(leftMonomial.Coefficient - rightMonomial.Coefficient, leftMonomial.Exponent, leftMonomial.Identifier).AsExpr();
        }
        #endregion

        return new SubExpr(left, right);
    }

    public override string ToString() => $"({Left} - {Right})";

    public override string ToLatex() => $"\\left({Left.ToLatex()} - {Right.ToLatex()}\\right)";

    public override object Clone() => new SubExpr((IExpr)Left.Clone(), (IExpr)Right.Clone());
}

public sealed class MulExpr : BinaryExpr {
    public MulExpr(IExpr left, IExpr right) : base(left, right) { }

    protected override double Operate(double left, double right) => left * right;

    // d(f(x)g(x)) -> f(x)g'(x) + f'(x)g(x)
    public override IExpr D(string byVar) => new AddExpr(new MulExpr(Left.D(byVar), Right), new MulExpr(Left, Right.D(byVar)));

    public override IExpr Simplify() {
        var left = Left.Simplify();
        var right = Right.Simplify();

        // const * const
        if (left is ConstExpr && right is ConstExpr)
            return new ConstExpr(((ConstExpr)left).Value * ((ConstExpr)right).Value);


        if(right is ConstExpr)
        {
            var constRight = (ConstExpr)right;
            switch(constRight.Value)
            {
                case 0:
                    return new ConstExpr(0);
                case 1:
                    return left;
                case -1:
                    return new NegExpr(left).Simplify();
            }
            return new MulExpr(right, left).Simplify();
        }

        if(left is ConstExpr)
        {
            var constLeft = (ConstExpr)left;
            switch(constLeft.Value)
            {
                case 0:
                    return new ConstExpr(0);
                case 1:
                    return right;
                case -1:
                    return new NegExpr(right).Simplify();
            }
        }

        // (-x) * y -> -(x * y)
        if (left is NegExpr)
            return new NegExpr(new MulExpr(((NegExpr)left).Operand, right)).Simplify();

        // x * (-y) -> -(x * y)
        if (right is NegExpr)
            return new NegExpr(new MulExpr(left, ((NegExpr)right).Operand)).Simplify();

        if(left is MulExpr && right is ConstExpr)
        {
            var mulLeft = (MulExpr)left;
            var constRight = (ConstExpr)right;
            // (x * const) * const -> const * x
            if(mulLeft.Right is ConstExpr constLeftRight)
            {
                return new MulExpr(new ConstExpr(constLeftRight.Value * constRight.Value), mulLeft.Left).Simplify();
            }
            // (const * x) * const -> const * x
            if(mulLeft.Left is ConstExpr constLeftLeft)
            {
                return new MulExpr(new ConstExpr(constLeftLeft.Value * constRight.Value), mulLeft.Right).Simplify();
            }
        }

        if(right is MulExpr && left is ConstExpr)
        {
            var mulRight = (MulExpr)right;
            var constLeft = (ConstExpr)left;
            // const * (x * const) -> const * x
            if(mulRight.Right is ConstExpr constRightRight)
            {
                return new MulExpr(new ConstExpr(constLeft.Value * constRightRight.Value), mulRight.Left).Simplify();
            }
            // const * (const * x) -> const * x
            if(mulRight.Left is ConstExpr constRightLeft)
            {
                return new MulExpr(new ConstExpr(constLeft.Value * constRightLeft.Value), mulRight.Right).Simplify();
            }
        }

        #region Try Monomial
        var leftMonomial = AsMonomial(left);
        var rightMonomial = AsMonomial(right);

        if(leftMonomial is not null && rightMonomial is not null)
        {
            if (leftMonomial.Identifier == rightMonomial.Identifier)
                return new Monomial(
                    leftMonomial.Coefficient * rightMonomial.Coefficient,
                    leftMonomial.Exponent + rightMonomial.Exponent,
                    leftMonomial.Identifier).AsExpr();

            if(leftMonomial.Identifier is null && rightMonomial.Identifier is not null)
                return new Monomial(
                    leftMonomial.Coefficient * rightMonomial.Coefficient,
                    rightMonomial.Exponent,
                    rightMonomial.Identifier).AsExpr();

            if(leftMonomial.Identifier is not null && rightMonomial.Identifier is null)
                return new Monomial(
                    leftMonomial.Coefficient * rightMonomial.Coefficient,
                    leftMonomial.Exponent,
                    leftMonomial.Identifier).AsExpr();

            leftMonomial = leftMonomial with {Coefficient = leftMonomial.Coefficient * rightMonomial.Coefficient};
            rightMonomial = rightMonomial with {Coefficient = 1};

            return new MulExpr(leftMonomial.AsExpr(), rightMonomial.AsExpr());
        }
        #endregion

        return new MulExpr(left, right);
    }

    public override string ToString() => $"({Left} * {Right})";

    public override string ToLatex()
    {
        if(Left is ConstExpr && Right is VarExpr)
            return $"{((ConstExpr)Left).Value}{((VarExpr)Right).Identifier}";

        return $"\\left({Left.ToLatex()} \\cdot {Right.ToLatex()}\\right)";
    }

    public override object Clone() => new MulExpr((IExpr)Left.Clone(), (IExpr)Right.Clone());
}

public sealed class DivExpr : BinaryExpr {
    public DivExpr(IExpr left, IExpr right) : base(left, right) { }

    protected override double Operate(double left, double right) => left / right;

    public override IExpr D(string byVar) => new DivExpr(new SubExpr(new MulExpr(Left.D(byVar), Right), new MulExpr(Left, Right.D(byVar))), new PowExpr(Right, new ConstExpr(2)));

    public override IExpr Simplify() {
        var left = Left.Simplify();
        var right = Right.Simplify();

        // const / const -> const
        if (left is ConstExpr && right is ConstExpr)
            return new ConstExpr(((ConstExpr)left).Value / ((ConstExpr)right).Value);

        // 0 / x -> 0
        if (left is ConstExpr && ((ConstExpr)left).Value == 0)
            return new ConstExpr(0);

        if(right is ConstExpr constRight)
        {
            switch(constRight.Value)
            {
                case 0:
                    throw new DivideByZeroException();
                case 1:
                    return left;
                case -1:
                    return new NegExpr(left).Simplify();
            }
        }

        // (-x) / (-y) -> x / y
        if(left is NegExpr && right is NegExpr)
            return new DivExpr(((NegExpr)left).Operand, ((NegExpr)right).Operand).Simplify();

        // (-x) / y -> -(x / y)
        if (left is NegExpr)
            return new NegExpr(new DivExpr(((NegExpr)left).Operand, right)).Simplify();

        // x / (-y) -> -(x / y)
        if (right is NegExpr)
            return new NegExpr(new DivExpr(left, ((NegExpr)right).Operand)).Simplify();

        // (x / y) / (z / w) -> (x * w) / (y * z)
        if (right is DivExpr && left is DivExpr)
            return new DivExpr(new MulExpr(((DivExpr)left).Left, ((DivExpr)right).Right), new MulExpr(((DivExpr)left).Right, ((DivExpr)right).Left)).Simplify();

        // w / (x / y) -> (w * y) / x
        if (right is DivExpr)
            return new DivExpr(new MulExpr(left, ((DivExpr)right).Right), ((DivExpr)right).Left).Simplify();

        #region Try Monomial
        var leftMonomial = AsMonomial(left);
        var rightMonomial = AsMonomial(right);

        if(leftMonomial is not null && rightMonomial is not null)
        {
            if (leftMonomial.Identifier == rightMonomial.Identifier)
                return new Monomial(
                    leftMonomial.Coefficient / rightMonomial.Coefficient,
                    leftMonomial.Exponent - rightMonomial.Exponent,
                    leftMonomial.Identifier).AsExpr();

            if(leftMonomial.Identifier is not null && rightMonomial.Identifier is null)
                return new Monomial(
                    leftMonomial.Coefficient / rightMonomial.Coefficient,
                    leftMonomial.Exponent,
                    leftMonomial.Identifier).AsExpr();

            if(leftMonomial.Identifier is null && rightMonomial.Identifier is not null)
                return new DivExpr(leftMonomial.AsExpr(), rightMonomial.AsExpr());

            leftMonomial = leftMonomial with {Coefficient = leftMonomial.Coefficient / rightMonomial.Coefficient};
            rightMonomial = rightMonomial with {Coefficient = 1};

            return new DivExpr(leftMonomial.AsExpr(), rightMonomial.AsExpr());
        }
        #endregion

        return new DivExpr(left, right);
    }

    public override string ToString() => $"({Left} / {Right})";

    public override string ToLatex() => $"\\frac{{{Left.ToLatex()}}}{{{Right.ToLatex()}}}";

    public override object Clone() => new DivExpr((IExpr)Left.Clone(), (IExpr)Right.Clone());
}

public sealed class PowExpr : BinaryExpr {
    public PowExpr(IExpr left, IExpr right) : base(left, right) { }

    protected override double Operate(double left, double right) => Math.Pow(left, right);

    public override IExpr D(string byVar)
    {
        if(Right.Simplify() is ConstExpr rConst)
        {
            return new MulExpr(new PowExpr(Left, new ConstExpr(rConst.Value - 1)), new MulExpr(Left.D(byVar), new ConstExpr(rConst.Value)));
        }
        else if(Left.Simplify() is ConstExpr lConst)
        {
            return new MulExpr(new MulExpr(new ConstExpr(Math.Log(lConst.Value)), Right.D(byVar)), new PowExpr(lConst, Right));
        }
        throw new UnsupportedOperationException("Unsupported operation: complex exponent differentiation.");
    }

    public override IExpr Simplify() {
        var left = Left.Simplify();
        var right = Right.Simplify();

        if (left is ConstExpr && right is ConstExpr)
            return new ConstExpr(Math.Pow(((ConstExpr)left).Value, ((ConstExpr)right).Value));

        if(left is ConstExpr constLeft)
        {
            switch(constLeft.Value)
            {
                case 0:
                    return new ConstExpr(0);
                case 1:
                    return new ConstExpr(1);
            }
        }

        if(right is ConstExpr constRight)
        {
            switch(constRight.Value)
            {
                case 0:
                    return new ConstExpr(1);
                case 1:
                    return left;
                case -1:
                    return new DivExpr(new ConstExpr(1), left).Simplify();
            }
        }

        return new PowExpr(left, right);
    }

    public override string ToString() => $"({Left} ^ {Right})";

    public override string ToLatex()
    {
        if(Right is ConstExpr constRight && constRight.Value < 0)
        {
            if(constRight.Value == -1)
                return $"\\frac{{1}}{{{Left.ToLatex()}}}";
            return $"\\frac{{1}}{{{Left.ToLatex()}^{{{-constRight.Value}}}}}";
        }
        return $"{Left.ToLatex()}^{{{Right.ToLatex()}}}";
    }

    public override object Clone() => new PowExpr((IExpr)Left.Clone(), (IExpr)Right.Clone());
}
