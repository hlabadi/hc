using System.Text;

namespace hc.Frontends.Basic;

public class BasicLexer
{
    private static readonly Dictionary<string, BasicTokenType> _keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        { "PRINT", BasicTokenType.Print },
        { "INPUT", BasicTokenType.Input },
        { "LET", BasicTokenType.Let },
        { "IF", BasicTokenType.If },
        { "THEN", BasicTokenType.Then },
        { "ELSE", BasicTokenType.Else },
        { "FOR", BasicTokenType.For },
        { "TO", BasicTokenType.To },
        { "STEP", BasicTokenType.Step },
        { "NEXT", BasicTokenType.Next },
        { "WHILE", BasicTokenType.While },
        { "WEND", BasicTokenType.Wend },
        { "GOTO", BasicTokenType.Goto },
        { "GOSUB", BasicTokenType.Gosub },
        { "RETURN", BasicTokenType.Return },
        { "END", BasicTokenType.End },
        { "DIM", BasicTokenType.Dim },
        { "AS", BasicTokenType.As },
        { "INTEGER", BasicTokenType.Integer },
        { "SINGLE", BasicTokenType.Single },
        { "DOUBLE", BasicTokenType.Double },
        { "STRING", BasicTokenType.String_Type },
        { "AND", BasicTokenType.And },
        { "OR", BasicTokenType.Or },
        { "NOT", BasicTokenType.Not },
        { "REM", BasicTokenType.Rem },
        { "MOD", BasicTokenType.Mod }
    };

    private int _column;
    private int _line;
    private int _position;
    private string _source;

    public BasicLexer(string source)
    {
        _source = source;
        _position = 0;
        _line = 1;
        _column = 1;
    }
    
    public List<BasicToken> Tokenize()
    {
        var tokens = new List<BasicToken>();

        while (!IsAtEnd())
        {
            SkipWhitespace();
            if (IsAtEnd()) break;

            Console.WriteLine($"About to tokenize: '{Peek()}' (char code: {(int)Peek()}) at position {_position}");
            
            tokens.Add(NextToken());
        }

        tokens.Add(new BasicToken(BasicTokenType.EndOfFile, "", _line, _column));
        return tokens;
    }

    private BasicToken NextToken()
    {
        var startLine = _line;
        var startColumn = _column;

        var c = Advance();
        
        switch (c)
        {
            case '(': return new BasicToken(BasicTokenType.LeftParen, "(", startLine, startColumn);
            case ')': return new BasicToken(BasicTokenType.RightParen, ")", startLine, startColumn);
            case ';': return new BasicToken(BasicTokenType.Semicolon, ";", startLine, startColumn);
            case ',': return new BasicToken(BasicTokenType.Comma, ",", startLine, startColumn);
            case ':': return new BasicToken(BasicTokenType.Colon, ":", startLine, startColumn);
            case '+': return new BasicToken(BasicTokenType.Plus, "+", startLine, startColumn);
            case '-': return new BasicToken(BasicTokenType.Minus, "-", startLine, startColumn);
            case '*': return new BasicToken(BasicTokenType.Star, "*", startLine, startColumn);
            case '/': return new BasicToken(BasicTokenType.Slash, "/", startLine, startColumn);
            case '^': return new BasicToken(BasicTokenType.Caret, "^", startLine, startColumn);
            case '\\': return new BasicToken(BasicTokenType.Backslash, "\\", startLine, startColumn);
            case '\n': return new BasicToken(BasicTokenType.NewLine, "\\n", startLine, startColumn);

            case '=':
                return new BasicToken(BasicTokenType.Equals, "=", startLine, startColumn);

            case '<':
                if (Peek() == '=')
                {
                    Advance();
                    return new BasicToken(BasicTokenType.LessEqual, "<=", startLine, startColumn);
                }

                if (Peek() == '>')
                {
                    Advance();
                    return new BasicToken(BasicTokenType.NotEqual, "<>", startLine, startColumn);
                }

                return new BasicToken(BasicTokenType.LessThan, "<", startLine, startColumn);

            case '>':
                if (Peek() == '=')
                {
                    Advance();
                    return new BasicToken(BasicTokenType.GreaterEqual, ">=", startLine, startColumn);
                }

                return new BasicToken(BasicTokenType.GreaterThan, ">", startLine, startColumn);

            case '"':
                return ScanString(startLine, startColumn);
        }

        if (char.IsDigit(c))
            return ScanNumber(c, startLine, startColumn);

        if (char.IsLetter(c) || c == '_')
            return ScanIdentifierOrKeyword(c, startLine, startColumn);

        throw new Exception($"Unexpected character: {c} at line {startLine}, column {startColumn}");
    }

    private BasicToken ScanNumber(char c, int startLine, int startColumn)
    {
        var num = new StringBuilder();
        num.Append(c);

        while (char.IsDigit(Peek()))
            num.Append(Advance());

        var next = Peek();

        if (next == '.')
        {
            if (!char.IsDigit(PeekNext()))
                throw new Exception($"Invalid number at {startLine}:{startColumn}");

            num.Append(Advance());

            while (char.IsDigit(Peek()))
                num.Append(Advance());
        }

        if (next is 'e' or 'E')
        {
            num.Append(Advance());
            
            if (Peek() == '+' || Peek() == '-')
                num.Append(Advance());
            
            if (!char.IsDigit(Peek()))
                throw new Exception($"Invalid exponent at {startLine}:{startColumn}");
            
            while (char.IsDigit(Peek()))
                num.Append(Advance());
        }

        return new BasicToken(BasicTokenType.Number, num.ToString(), startLine, startColumn);
    }

    private BasicToken ScanString(int startLine, int startColumn)
    {
        var sb = new StringBuilder();

        while (!IsAtEnd() && Peek() != '\n')
            if (Peek() == '"' && PeekNext() == '"')
            {
                sb.Append('"');
                Advance();
                Advance();
            }
            else if (Peek() == '"')
            {
                break;
            }
            else
            {
                sb.Append(Advance());
            }
        
        if (Peek() != '"')
            throw new Exception($"Unterminated string at {startLine}:{startColumn}");

        Advance();

        return new BasicToken(BasicTokenType.String, sb.ToString(), startLine, startColumn);
    }

    private BasicToken ScanIdentifierOrKeyword(char firstChar, int startLine, int startColumn)
    {
        var sb = new StringBuilder();
        sb.Append(firstChar);
        
        while (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
            sb.Append(Advance());
        
        if (!IsAtEnd() && (Peek() == '%' || Peek() == '$' || Peek() == '#' || Peek() == '!'))
            sb.Append(Advance());
        
        if (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
            throw new Exception(
                $"Invalid identifier at {startLine}:{startColumn}: type suffix must be at end of identifier");

        var value = sb.ToString();
        
        if (_keywords.TryGetValue(value.ToUpper(), out BasicTokenType keywordType))
        {
            if (keywordType == BasicTokenType.Rem)
            {
                while (!IsAtEnd() && Peek() != '\n')
                    Advance();
        
                return NextToken();
            }
    
            return new BasicToken(keywordType, value, startLine, startColumn);
        }
        
        return new BasicToken(BasicTokenType.Identifier, value, startLine, startColumn);
    }
    
    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_position];
    }

    private char PeekNext()
    {
        return _position + 1 >= _source.Length ? '\0' : _source[_position + 1];
    }

    private char Advance()
    {
        var c = _source[_position++];
        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return c;
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd())
        {
            char c = Peek();
        
            if (c is ' ' or '\t' or '\r')
                Advance();
            else if (c == '\'')
            {
                while (!IsAtEnd() && Peek() != '\n')
                    Advance();
            }
            else
                break;
        }
    }
}