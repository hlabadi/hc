namespace hc.Frontends.Basic;

public class BasicToken
{
    public BasicTokenType Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
        
    public BasicToken(BasicTokenType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }
        
    public override string ToString() => $"{Type}: '{Value}' at ({Line},{Column})";
}