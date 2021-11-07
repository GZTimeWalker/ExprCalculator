namespace Calculator.Core.Expr;

public sealed class NegExpr : UnaryExpr {
    public NegExpr(IExpr expr) : base(expr) { }

    protected override double Operate(double operand) => -operand;

    public override IExpr D(string byVar) => new NegExpr(Operand.D(byVar));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        // -const -> const
        if (operand is ConstExpr constop)
            return new ConstExpr(-constop.Value);

        // -(-(x)) -> x
        if (operand is NegExpr negop)
            return negop.Operand;

        // -(x-y) -> y - x
        if (operand is SubExpr subop)
            return new SubExpr(subop.Right, subop.Left);

        return new NegExpr(operand);
    }

    public override string ToString() => $"(-{Operand})";

    public override string ToLatex() => $"-{{{Operand.ToLatex()}}}";

    public override int Width() => Math.Max(Operand.Width(), 1);
    public override object Clone() => new NegExpr((IExpr) Operand.Clone());

    public override int LeftWidth() => Math.Max(Operand.LeftWidth(), 0);

    public override int RightWidth() => Math.Max(Operand.RightWidth(), 0);
}

public abstract class FuncExpr : UnaryExpr {
    public FuncExpr(IExpr expr) : base(expr) { }
    public virtual string FuncName { get { return "Unknown"; } }

    public override string ToString() => $"{FuncName}({Operand})";
    public override int Width() => Math.Max(Operand.Width(), FuncName.Length % 2 == 0 ? FuncName.Length + 1 : FuncName.Length);
    public override int LeftWidth() => Math.Max(Operand.LeftWidth(), FuncName.Length / 2);
    public override int RightWidth() => Math.Max(Operand.RightWidth(), FuncName.Length / 2);
}

public sealed class SinExpr : FuncExpr {
    public override string FuncName { get { return "sin"; } }
    public SinExpr(IExpr expr) : base(expr) { }

    protected override double Operate(double operand) => Math.Sin(operand);

    public override IExpr D(string byVar) => new MulExpr(new CosExpr(Operand), Operand.D(byVar));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Sin(s.Value));
        }
        return new SinExpr(operand);
    }
    public override string ToLatex() => $"\\sin\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new SinExpr((IExpr) Operand.Clone());
}

public sealed class CosExpr : FuncExpr {
    public CosExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "cos"; } }

    protected override double Operate(double operand) => Math.Cos(operand);

    public override IExpr D(string byVar) => new MulExpr(new NegExpr(new SinExpr(Operand)), Operand.D(byVar));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Cos(s.Value));
        }
        if (operand is NegExpr neg) {
            return new CosExpr(neg.Operand);
        }
        return new CosExpr(operand);
    }
    public override string ToLatex() => $"\\cos\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new CosExpr((IExpr) Operand.Clone());
}

public sealed class TanExpr : FuncExpr {
    public TanExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "tan"; } }
    protected override double Operate(double operand) => Math.Tan(operand);

    public override IExpr D(string byVar) => new MulExpr(new PowExpr(new CosExpr(Operand), new ConstExpr(-2)), Operand.D(byVar));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Tan(s.Value));
        }
        return new TanExpr(operand);
    }

    public override string ToLatex() => $"\\tan\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new TanExpr((IExpr) Operand.Clone());
}

public sealed class SqrtExpr : FuncExpr {
    public SqrtExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "sqrt"; } }
    protected override double Operate(double operand) => Math.Sqrt(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), new MulExpr(new ConstExpr(2), new SqrtExpr(Operand)));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Sqrt(s.Value));
        }
        return new SqrtExpr(operand);
    }

    public override string ToLatex() => $"\\sqrt{{{Operand.ToLatex()}}}";

    public override object Clone() => new SqrtExpr((IExpr) Operand.Clone());
}

public sealed class ArcSinExpr : FuncExpr {
    public ArcSinExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "arcsin"; } }
    protected override double Operate(double operand) => Math.Asin(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), new SqrtExpr(new SubExpr(new ConstExpr(1), new PowExpr(Operand, new ConstExpr(2)))));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Asin(s.Value));
        }
        return new ArcSinExpr(operand);
    }

    public override string ToLatex() => $"\\arcsin\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new ArcSinExpr((IExpr) Operand.Clone());
}

public sealed class ArcCosExpr : FuncExpr {
    public ArcCosExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "arccos"; } }
    protected override double Operate(double operand) => Math.Acos(operand);

    public override IExpr D(string byVar) => new NegExpr(new DivExpr(Operand.D(byVar), new SqrtExpr(new SubExpr(new ConstExpr(1), new PowExpr(Operand, new ConstExpr(2))))));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Acos(s.Value));
        }
        return new ArcCosExpr(operand);
    }

    public override string ToLatex() => $"\\arccos\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new ArcCosExpr((IExpr) Operand.Clone());
}

public sealed class ArcTanExpr : FuncExpr {
    public ArcTanExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "arctan"; } }
    protected override double Operate(double operand) => Math.Atan(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), new AddExpr(new ConstExpr(1), new PowExpr(Operand, new ConstExpr(2))));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Atan(s.Value));
        }
        return new ArcTanExpr(operand);
    }

    public override string ToLatex() => $"\\arctan\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new ArcTanExpr((IExpr) Operand.Clone());
}

public sealed class LogExpr : FuncExpr {
    public LogExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "log"; } }
    protected override double Operate(double operand) => Math.Log10(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), new MulExpr(Operand, new ConstExpr(Math.Log10(Math.E))));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Log10(s.Value));
        }
        return new LogExpr(operand);
    }

    public override string ToLatex() => $"\\log\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new LogExpr((IExpr) Operand.Clone());
}

public sealed class LnExpr : FuncExpr {
    public LnExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "ln"; } }
    protected override double Operate(double operand) => Math.Log(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), Operand);

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Log(s.Value));
        }
        if (operand is ExpExpr opexp) {
            return (IExpr) opexp.Operand.Clone();
        }
        return new LnExpr(operand);
    }

    public override string ToLatex() => $"\\ln\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new LnExpr((IExpr) Operand.Clone());
}

public sealed class ExpExpr : FuncExpr {
    public ExpExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "exp"; } }
    protected override double Operate(double operand) => Math.Exp(operand);

    public override IExpr D(string byVar) => new MulExpr(Operand.D(byVar), new ExpExpr(Operand));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Exp(s.Value));
        }
        return new ExpExpr(operand);
    }

    public override string ToLatex() => $"\\exp\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new ExpExpr((IExpr) Operand.Clone());
}

public sealed class CotExpr : FuncExpr {
    public CotExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "cot"; } }
    protected override double Operate(double operand) => 1 / Math.Tan(operand);

    public override IExpr D(string byVar) => new DivExpr(Operand.D(byVar), new NegExpr(new PowExpr(new SinExpr(Operand), new ConstExpr(2))));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(1 / Math.Tan(s.Value));
        }
        return new CotExpr(operand);
    }

    public override string ToLatex() => $"\\cot\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new CotExpr((IExpr) Operand.Clone());
}

public sealed class SecExpr : FuncExpr {
    public SecExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "sec"; } }

    protected override double Operate(double operand) => 1 / Math.Cos(operand);

    public override IExpr D(string byVar) => new DivExpr(new MulExpr(Operand.D(byVar), new SinExpr(Operand)), new PowExpr(new CosExpr(Operand), new ConstExpr(2)));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(1 / Math.Cos(s.Value));
        }
        return new SecExpr(operand);
    }

    public override string ToLatex() => $"\\sec\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new SecExpr((IExpr) Operand.Clone());
}

public sealed class CscExpr : FuncExpr {
    public CscExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "csc"; } }

    protected override double Operate(double operand) => 1 / Math.Sin(operand);

    public override IExpr D(string byVar) => new DivExpr(new MulExpr(new NegExpr(Operand.D(byVar)), new CosExpr(Operand)), new PowExpr(new SinExpr(Operand), new ConstExpr(2)));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(1 / Math.Sin(s.Value));
        }
        return new CscExpr(operand);
    }

    public override string ToLatex() => $"\\csc\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new CscExpr((IExpr) Operand.Clone());
}

public sealed class AbsExpr : FuncExpr {
    public AbsExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "abs"; } }

    protected override double Operate(double operand) => Math.Abs(operand);

    public override IExpr D(string byVar) => new SignExpr(Operand.D(byVar));

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Abs(s.Value));
        }
        if (operand is NegExpr neg) {
            return new AbsExpr(neg.Operand);
        }
        return new AbsExpr(operand);
    }

    public override string ToLatex() => $"\\left|{Operand.ToLatex()}\\right|";

    public override object Clone() => new AbsExpr((IExpr) Operand.Clone());
}

public sealed class SignExpr : FuncExpr {
    public SignExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "sign"; } }
    protected override double Operate(double operand) => Math.Sign(operand);

    public override IExpr D(string byVar) => new ConstExpr(0);

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Sign(s.Value));
        }
        return new SignExpr(operand);
    }

    public override string ToLatex() => $"\\left\\langle{Operand.ToLatex()}\\right\\rangle";

    public override object Clone() => new SignExpr((IExpr) Operand.Clone());
}

public sealed class FloorExpr : FuncExpr {
    public FloorExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "floor"; } }
    protected override double Operate(double operand) => Math.Floor(operand);

    public override IExpr D(string byVar) => new ConstExpr(0);

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Floor(s.Value));
        }
        return new FloorExpr(operand);
    }

    public override string ToLatex() => $"\\left\\lfloor{Operand.ToLatex()}\\right\\rfloor";

    public override object Clone() => new FloorExpr((IExpr) Operand.Clone());
}

public sealed class CeilExpr : FuncExpr {
    public CeilExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "ceil"; } }
    protected override double Operate(double operand) => Math.Ceiling(operand);

    public override IExpr D(string byVar) => new ConstExpr(0);

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Ceiling(s.Value));
        }
        return new CeilExpr(operand);
    }

    public override string ToLatex() => $"\\left\\lceil{Operand.ToLatex()}\\right\\rceil";

    public override object Clone() => new CeilExpr((IExpr) Operand.Clone());
}

public sealed class RoundExpr : FuncExpr {
    public RoundExpr(IExpr expr) : base(expr) { }

    public override string FuncName { get { return "round"; } }
    protected override double Operate(double operand) => Math.Round(operand);

    public override IExpr D(string byVar) => new ConstExpr(0);

    public override IExpr Simplify() {
        var operand = Operand.Simplify();
        if (operand is ConstExpr s) {
            return new ConstExpr(Math.Round(s.Value));
        }
        return new RoundExpr(operand);
    }

    public override string ToLatex() => $"round\\left({Operand.ToLatex()}\\right)";

    public override object Clone() => new RoundExpr((IExpr) Operand.Clone());
}
