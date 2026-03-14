using hc.Frontends.Basic;
using Xunit;

namespace hc.Tests.Frontends.Basic;

public class LexerTests
{
    #region Basic Token Tests
    
    [Fact]
    public void Tokenize_SimplePrint_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("PRINT \"Hello\"");
        var tokens = lexer.Tokenize();
    
        Assert.Equal(3, tokens.Count);  // PRINT, String, EOF (no NewLine!)
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.String, tokens[1].Type);
        Assert.Equal("Hello", tokens[1].Value);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[2].Type);
    }
    
    #endregion
    
    #region Number Tests
    
    [Theory]
    [InlineData("123", "123")]
    [InlineData("0", "0")]
    [InlineData("999", "999")]
    public void Tokenize_Integer_ReturnsNumberToken(string input, string expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(expected, tokens[0].Value);
    }
    
    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("0.5", "0.5")]
    [InlineData("99.99", "99.99")]
    public void Tokenize_FloatingPoint_ReturnsNumberToken(string input, string expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(expected, tokens[0].Value);
    }
    
    [Theory]
    [InlineData("1.5E10", "1.5E10")]
    [InlineData("2.5E-3", "2.5E-3")]
    [InlineData("3E+5", "3E+5")]
    [InlineData("1e10", "1e10")]
    public void Tokenize_ScientificNotation_ReturnsNumberToken(string input, string expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(expected, tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_NumberWithInvalidDecimal_ThrowsException()
    {
        var lexer = new BasicLexer("3.");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    [Fact]
    public void Tokenize_NumberWithInvalidExponent_ThrowsException()
    {
        var lexer = new BasicLexer("3E");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    #endregion
    
    #region String Tests
    
    [Fact]
    public void Tokenize_SimpleString_ReturnsStringToken()
    {
        var lexer = new BasicLexer("\"Hello\"");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.String, tokens[0].Type);
        Assert.Equal("Hello", tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_EmptyString_ReturnsEmptyStringToken()
    {
        var lexer = new BasicLexer("\"\"");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.String, tokens[0].Type);
        Assert.Equal("", tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_StringWithEscapedQuotes_ReturnsCorrectValue()
    {
        var lexer = new BasicLexer("\"Say \"\"Hi\"\"\"");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.String, tokens[0].Type);
        Assert.Equal("Say \"Hi\"", tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_UnterminatedString_ThrowsException()
    {
        var lexer = new BasicLexer("\"Hello");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    [Fact]
    public void Tokenize_StringWithNewline_ThrowsException()
    {
        var lexer = new BasicLexer("\"Hello\nWorld\"");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    #endregion
    
    #region Keyword Tests
    
    [Theory]
    [InlineData("PRINT", BasicTokenType.Print)]
    [InlineData("print", BasicTokenType.Print)]
    [InlineData("Print", BasicTokenType.Print)]
    [InlineData("IF", BasicTokenType.If)]
    [InlineData("if", BasicTokenType.If)]
    [InlineData("THEN", BasicTokenType.Then)]
    [InlineData("FOR", BasicTokenType.For)]
    [InlineData("TO", BasicTokenType.To)]
    [InlineData("NEXT", BasicTokenType.Next)]
    [InlineData("WHILE", BasicTokenType.While)]
    [InlineData("WEND", BasicTokenType.Wend)]
    [InlineData("LET", BasicTokenType.Let)]
    [InlineData("GOTO", BasicTokenType.Goto)]
    [InlineData("END", BasicTokenType.End)]
    public void Tokenize_Keyword_IsCaseInsensitive(string input, BasicTokenType expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(expected, tokens[0].Type);
    }
    
    #endregion
    
    #region Identifier Tests
    
    [Theory]
    [InlineData("x", "x")]
    [InlineData("name", "name")]
    [InlineData("variable123", "variable123")]
    [InlineData("X123Y", "X123Y")]
    public void Tokenize_SimpleIdentifier_ReturnsIdentifier(string input, string expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal(expected, tokens[0].Value);
    }
    
    [Theory]
    [InlineData("x%", "x%")]
    [InlineData("name$", "name$")]
    [InlineData("value#", "value#")]
    [InlineData("count!", "count!")]
    public void Tokenize_IdentifierWithTypeSuffix_ReturnsIdentifier(string input, string expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal(expected, tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_IdentifierWithSuffixFollowedByLetter_ThrowsException()
    {
        // Edge case: x$s should error (suffix must be at end)
        var lexer = new BasicLexer("x$s");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    [Fact]
    public void Tokenize_IdentifierWithSuffixFollowedByDigit_ThrowsException()
    {
        // Edge case: x%123 should error (suffix must be at end)
        var lexer = new BasicLexer("x%123");
        
        Assert.Throws<Exception>(() => lexer.Tokenize());
    }
    
    [Fact]
    public void Tokenize_PRINT10_ReturnsIdentifier()
    {
        // Edge case: PRINT10 is identifier, not PRINT + 10
        var lexer = new BasicLexer("PRINT10");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal("PRINT10", tokens[0].Value);
    }
    
    [Fact]
    public void Tokenize_10PRINT_ReturnsNumberThenKeyword()
    {
        // Edge case: 10PRINT splits correctly
        var lexer = new BasicLexer("10PRINT");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(3, tokens.Count); // Number, Print, EOF
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
    }
    
    #endregion
    
    #region Operator Tests
    
    [Theory]
    [InlineData("+", BasicTokenType.Plus)]
    [InlineData("-", BasicTokenType.Minus)]
    [InlineData("*", BasicTokenType.Star)]
    [InlineData("/", BasicTokenType.Slash)]
    [InlineData("^", BasicTokenType.Caret)]
    [InlineData("\\", BasicTokenType.Backslash)]
    [InlineData("=", BasicTokenType.Equals)]
    [InlineData("<", BasicTokenType.LessThan)]
    [InlineData(">", BasicTokenType.GreaterThan)]
    public void Tokenize_SingleCharOperator_ReturnsCorrectToken(string input, BasicTokenType expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(expected, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("<=", BasicTokenType.LessEqual)]
    [InlineData(">=", BasicTokenType.GreaterEqual)]
    [InlineData("<>", BasicTokenType.NotEqual)]
    public void Tokenize_MultiCharOperator_ReturnsCorrectToken(string input, BasicTokenType expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(expected, tokens[0].Type);
    }
    
    #endregion
    
    #region Punctuation Tests
    
    [Theory]
    [InlineData("(", BasicTokenType.LeftParen)]
    [InlineData(")", BasicTokenType.RightParen)]
    [InlineData(",", BasicTokenType.Comma)]
    [InlineData(";", BasicTokenType.Semicolon)]
    [InlineData(":", BasicTokenType.Colon)]
    public void Tokenize_Punctuation_ReturnsCorrectToken(string input, BasicTokenType expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();
        
        Assert.Equal(expected, tokens[0].Type);
    }
    
    #endregion
    
    #region Line Number Tests
    
    [Fact]
    public void Tokenize_LineNumber_ReturnsLineNumberToken()
    {
        var lexer = new BasicLexer("10 PRINT");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
    }
    
    [Fact]
    public void Tokenize_MultipleLines_HandlesLineNumbersCorrectly()
    {
        var lexer = new BasicLexer("10 PRINT\n20 LET");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal("20", tokens[3].Value);
        Assert.Equal(BasicTokenType.Let, tokens[4].Type);
    }
    
    #endregion
    
    #region Comment Tests
    
    [Fact]
    public void Tokenize_REMComment_SkipsRestOfLine()
    {
        var lexer = new BasicLexer("10 REM This is a comment\n20 PRINT");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.NewLine, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal("20", tokens[2].Value);
        Assert.Equal(BasicTokenType.Print, tokens[3].Type);
    }
    
    [Fact]
    public void Tokenize_SingleQuoteComment_SkipsRestOfLine()
    {
        var lexer = new BasicLexer("10 PRINT ' This is a comment\n20 LET");
        var tokens = lexer.Tokenize();
        
        // Should have: 10, PRINT, NewLine, 20, LET, EOF
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal(BasicTokenType.Let, tokens[4].Type);
    }
    
    #endregion
    
    #region Whitespace Tests
    
    [Fact]
    public void Tokenize_MultipleSpaces_AreSkipped()
    {
        var lexer = new BasicLexer("PRINT     123");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(3, tokens.Count); // PRINT, 123, EOF
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.Number, tokens[1].Type);
    }
    
    [Fact]
    public void Tokenize_NoSpaces_WorksCorrectly()
    {
        var lexer = new BasicLexer("PRINT\"Hello\"");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.String, tokens[1].Type);
        Assert.Equal("Hello", tokens[1].Value);
    }
    
    [Fact]
    public void Tokenize_TabsAndSpaces_AreSkipped()
    {
        var lexer = new BasicLexer("PRINT\t\t  123");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(3, tokens.Count);
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.Number, tokens[1].Type);
    }
    
    #endregion
    
    #region Complex Integration Tests
    
    [Fact]
    public void Tokenize_CompleteBasicProgram_TokenizesCorrectly()
    {
        var source = """
            10 PRINT "Hello, World!"
            20 LET x% = 5
            30 IF x% > 3 THEN PRINT "Big"
            """;
        
        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();
        
        // Line 1
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
        Assert.Equal(BasicTokenType.String, tokens[2].Type);
        Assert.Equal("Hello, World!", tokens[2].Value);
        Assert.Equal(BasicTokenType.NewLine, tokens[3].Type);
        
        // Line 2
        Assert.Equal(BasicTokenType.Number, tokens[4].Type);
        Assert.Equal("20", tokens[4].Value);
        Assert.Equal(BasicTokenType.Let, tokens[5].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[6].Type);
        Assert.Equal("x%", tokens[6].Value);
        Assert.Equal(BasicTokenType.Equals, tokens[7].Type);
        Assert.Equal(BasicTokenType.Number, tokens[8].Type);
        Assert.Equal("5", tokens[8].Value);
        
        // Ends with EOF
        Assert.Equal(BasicTokenType.EndOfFile, tokens[tokens.Count - 1].Type);
    }
    
    [Fact]
    public void Tokenize_CompactCode_TokenizesCorrectly()
    {
        // Edge case: Compact code with proper spacing
        var lexer = new BasicLexer("10A=5:IF A>3THEN PRINT A");  // Added spaces!
        var tokens = lexer.Tokenize();
    
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("A", tokens[1].Value);
        Assert.Equal(BasicTokenType.Equals, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal(BasicTokenType.Colon, tokens[4].Type);
        Assert.Equal(BasicTokenType.If, tokens[5].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[6].Type);
        Assert.Equal("A", tokens[6].Value);
        Assert.Equal(BasicTokenType.GreaterThan, tokens[7].Type);
        Assert.Equal(BasicTokenType.Number, tokens[8].Type);
        Assert.Equal(BasicTokenType.Then, tokens[9].Type);
        Assert.Equal(BasicTokenType.Print, tokens[10].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[11].Type);
        Assert.Equal("A", tokens[11].Value);
    }
    
    #endregion
}