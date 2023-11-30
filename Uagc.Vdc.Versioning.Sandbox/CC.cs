namespace Uagc.Vdc.Versioning.Sandbox;

/// <summary>
/// Why, yes, CC does stand for Color Console.  Thanks for asking.
/// </summary>
static public class CC
{
    public const string INFO = "Info";
    public const string HEADER = "Header";
    public const string PASS = "Pass Test";
    public const string FAIL = "Fail Test";
    public const string ERROR = "Error";

    static private Dictionary<string, ConsoleColor[]> _named = new();

    static CC()
    {
        _named = new();
        CC.RegisterNamedColorSet(INFO, new[] { ConsoleColor.Black, ConsoleColor.White });          // info
        CC.RegisterNamedColorSet(HEADER, new[] { ConsoleColor.DarkBlue, ConsoleColor.Yellow });    // header
        CC.RegisterNamedColorSet(PASS, new[] { ConsoleColor.Black, ConsoleColor.Green });          // pass
        CC.RegisterNamedColorSet(FAIL, new[] { ConsoleColor.Black, ConsoleColor.Red });            // fail
        CC.RegisterNamedColorSet(ERROR, new[] { ConsoleColor.DarkRed, ConsoleColor.White });       // error
    }

    #region Color Management
    static public void RegisterNamedColorSet(string name, ConsoleColor bg, ConsoleColor fg) =>
        RegisterNamedColorSet(name, new[] { bg, fg });

    static public void RegisterNamedColorSet(string name, ConsoleColor[] bgfg)   // why not? => _named.ContainsKey(name) ? _named[name] = bgfg : _named.Add(name, bgfg);
    {
        if (_named.ContainsKey(name)) _named[name] = bgfg; else _named.Add(name, bgfg);
    }

    static public ConsoleColor[] GetColors(string name) => _named.ContainsKey(name) ? _named[name] : _named[INFO];
    #endregion

    static public void Write(string msg, string clrs = INFO)
    {
        display(msg, clrs, terminateLine: false);
    }

    static public void WriteLine(string msg, string clrs = INFO)
    {
        display(msg, clrs, terminateLine: true);
    }

    static public void Indent(int indents, string msg, string clrs = INFO) => display(string.Concat(new string(' ', indents * 4), msg), clrs);

    static public void IndentLine(int indents, string msg, string clrs = INFO) => display(string.Concat(new string(' ', indents * 4), msg), clrs, terminateLine: true);

    static public void RightJustifyAndFill(string msg, string clrs = INFO)
    {
        int offset = Console.WindowWidth - msg.Length;
        display(new string(' ', offset), clrs, terminateLine: true);
        WriteLine(msg, clrs);
    }

    static public void LeftJustifyAndFill(string msg, string clrs = INFO)
    {
        Write(msg, clrs);
        int filler = Console.WindowWidth - msg.Length;
        WriteLine(new string(' ', filler), clrs);
    }

    static public void Center(string msg, string clrs = INFO)
    {
        int offset = (Console.WindowWidth - msg.Length) / 2;
        display(new string(' ', offset), clrs, terminateLine: false);
        Write(msg, clrs);
        display(new string(' ', offset), clrs, terminateLine: true);
    }

    static public void Stripe(string clrs = INFO) => Stripe('=', clrs);

    static public void Stripe(char? c, string clrs = INFO) => display(new string(c ?? '=', Console.WindowWidth), clrs, terminateLine: true);

    static public void display(string msg, string clrs, bool terminateLine = true)
    {
        Console.BackgroundColor = GetColors(clrs)[0];
        Console.ForegroundColor = GetColors(clrs)[1];

        if (terminateLine) Console.WriteLine(msg); else Console.Write(msg);

        Console.ResetColor();
    }
}