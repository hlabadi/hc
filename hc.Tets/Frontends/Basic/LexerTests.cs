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

    [Fact]
    public void Tokenize_StringContainingSingleQuote_NotTreatedAsComment()
    {
        var lexer = new BasicLexer("\"Hello ' World\"");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.String, tokens[0].Type);
        Assert.Equal("Hello ' World", tokens[0].Value);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[1].Type);
    }

    [Fact]
    public void Tokenize_StringWithSpacesAndPunctuation_ReturnsFullValue()
    {
        var lexer = new BasicLexer("\"Enter a value: \"");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.String, tokens[0].Type);
        Assert.Equal("Enter a value: ", tokens[0].Value);
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
    [InlineData("ELSE", BasicTokenType.Else)]
    [InlineData("FOR", BasicTokenType.For)]
    [InlineData("TO", BasicTokenType.To)]
    [InlineData("STEP", BasicTokenType.Step)]
    [InlineData("NEXT", BasicTokenType.Next)]
    [InlineData("WHILE", BasicTokenType.While)]
    [InlineData("WEND", BasicTokenType.Wend)]
    [InlineData("LET", BasicTokenType.Let)]
    [InlineData("GOTO", BasicTokenType.Goto)]
    [InlineData("GOSUB", BasicTokenType.Gosub)]
    [InlineData("RETURN", BasicTokenType.Return)]
    [InlineData("END", BasicTokenType.End)]
    [InlineData("DIM", BasicTokenType.Dim)]
    [InlineData("AS", BasicTokenType.As)]
    [InlineData("INTEGER", BasicTokenType.Integer)]
    [InlineData("SINGLE", BasicTokenType.Single)]
    [InlineData("DOUBLE", BasicTokenType.Double)]
    [InlineData("STRING", BasicTokenType.String_Type)]
    [InlineData("AND", BasicTokenType.And)]
    [InlineData("OR", BasicTokenType.Or)]
    [InlineData("NOT", BasicTokenType.Not)]
    [InlineData("MOD", BasicTokenType.Mod)]
    [InlineData("INPUT", BasicTokenType.Input)]
    public void Tokenize_Keyword_IsCaseInsensitive(string input, BasicTokenType expected)
    {
        var lexer = new BasicLexer(input);
        var tokens = lexer.Tokenize();

        Assert.Equal(expected, tokens[0].Type);
    }

    [Theory]
    [InlineData("and", BasicTokenType.And)]
    [InlineData("And", BasicTokenType.And)]
    [InlineData("or", BasicTokenType.Or)]
    [InlineData("Or", BasicTokenType.Or)]
    [InlineData("not", BasicTokenType.Not)]
    [InlineData("Not", BasicTokenType.Not)]
    [InlineData("mod", BasicTokenType.Mod)]
    [InlineData("Mod", BasicTokenType.Mod)]
    public void Tokenize_LogicalAndArithmeticKeywords_AreCaseInsensitive(string input, BasicTokenType expected)
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

    [Fact]
    public void Tokenize_MODOperator_ReturnsModToken()
    {
        var lexer = new BasicLexer("x MOD y");
        var tokens = lexer.Tokenize();

        Assert.Equal(4, tokens.Count); // Identifier, Mod, Identifier, EOF
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal("x", tokens[0].Value);
        Assert.Equal(BasicTokenType.Mod, tokens[1].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[2].Type);
        Assert.Equal("y", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_IntegerDivision_ReturnsBackslashToken()
    {
        // \ is integer division in BASIC
        var lexer = new BasicLexer("10 \\ 3");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal(BasicTokenType.Backslash, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
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

    [Fact]
    public void Tokenize_SingleQuoteCommentAtStartOfLine_SkipsLine()
    {
        var lexer = new BasicLexer("PRINT\n' This whole line is a comment\nLET");
        var tokens = lexer.Tokenize();

        // PRINT, NewLine, NewLine (the \n after the comment), LET, EOF
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[1].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[2].Type);
        Assert.Equal(BasicTokenType.Let, tokens[3].Type);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[4].Type);
    }

    [Fact]
    public void Tokenize_REMWithNoFollowingContent_OnlyProducesNewLine()
    {
        // REM at end of a line — next token is the newline
        var lexer = new BasicLexer("10 REM\n20 PRINT");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.NewLine, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal("20", tokens[2].Value);
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

    [Fact]
    public void Tokenize_EmptyInput_ReturnsOnlyEOF()
    {
        var lexer = new BasicLexer("");
        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_WhitespaceOnly_ReturnsOnlyEOF()
    {
        var lexer = new BasicLexer("   \t  ");
        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_CRLFLineEnding_TreatedSameAsLF()
    {
        var lexer = new BasicLexer("10 PRINT\r\n20 LET");
        var tokens = lexer.Tokenize();

        // \r is skipped as whitespace, \n becomes the NewLine token
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Print, tokens[1].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal("20", tokens[3].Value);
        Assert.Equal(BasicTokenType.Let, tokens[4].Type);
    }

    #endregion

    #region Logical Operator Tests

    [Fact]
    public void Tokenize_ANDExpression_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("x AND y");
        var tokens = lexer.Tokenize();

        Assert.Equal(4, tokens.Count); // Identifier, And, Identifier, EOF
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal("x", tokens[0].Value);
        Assert.Equal(BasicTokenType.And, tokens[1].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[2].Type);
        Assert.Equal("y", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_ORExpression_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("a OR b");
        var tokens = lexer.Tokenize();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal(BasicTokenType.Or, tokens[1].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[2].Type);
    }

    [Fact]
    public void Tokenize_NOTExpression_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("NOT flag");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count); // Not, Identifier, EOF
        Assert.Equal(BasicTokenType.Not, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("flag", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_CompoundBooleanExpression_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("a > 0 AND b < 10 OR NOT c");
        var tokens = lexer.Tokenize();

        // a, >, 0, AND, b, <, 10, OR, NOT, c, EOF
        Assert.Equal(11, tokens.Count);
        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal(BasicTokenType.GreaterThan, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal(BasicTokenType.And, tokens[3].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[4].Type);
        Assert.Equal(BasicTokenType.LessThan, tokens[5].Type);
        Assert.Equal(BasicTokenType.Number, tokens[6].Type);
        Assert.Equal(BasicTokenType.Or, tokens[7].Type);
        Assert.Equal(BasicTokenType.Not, tokens[8].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[9].Type);
    }

    #endregion

    #region Arithmetic Expression Tests

    [Fact]
    public void Tokenize_ParenthesizedExpression_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("(A + B) * C");
        var tokens = lexer.Tokenize();

        // (, A, +, B, ), *, C, EOF
        Assert.Equal(8, tokens.Count);
        Assert.Equal(BasicTokenType.LeftParen, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal(BasicTokenType.Plus, tokens[2].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[3].Type);
        Assert.Equal(BasicTokenType.RightParen, tokens[4].Type);
        Assert.Equal(BasicTokenType.Star, tokens[5].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[6].Type);
    }

    [Fact]
    public void Tokenize_NegationViaMinusOperator_ReturnsSeparateTokens()
    {
        // BASIC has no unary minus literal; -5 is Minus + Number
        var lexer = new BasicLexer("LET x = -5");
        var tokens = lexer.Tokenize();

        // LET, x, =, -, 5, EOF
        Assert.Equal(6, tokens.Count);
        Assert.Equal(BasicTokenType.Let, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal(BasicTokenType.Equals, tokens[2].Type);
        Assert.Equal(BasicTokenType.Minus, tokens[3].Type);
        Assert.Equal(BasicTokenType.Number, tokens[4].Type);
        Assert.Equal("5", tokens[4].Value);
    }

    [Fact]
    public void Tokenize_ExponentiationOperator_ReturnsCaret()
    {
        var lexer = new BasicLexer("x ^ 2");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Identifier, tokens[0].Type);
        Assert.Equal(BasicTokenType.Caret, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
    }

    #endregion

    #region FOR Loop Tests

    [Fact]
    public void Tokenize_ForLoopWithStep_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("FOR i% = 1 TO 10 STEP 2");
        var tokens = lexer.Tokenize();

        // FOR, i%, =, 1, TO, 10, STEP, 2, EOF
        Assert.Equal(9, tokens.Count);
        Assert.Equal(BasicTokenType.For, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("i%", tokens[1].Value);
        Assert.Equal(BasicTokenType.Equals, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal("1", tokens[3].Value);
        Assert.Equal(BasicTokenType.To, tokens[4].Type);
        Assert.Equal(BasicTokenType.Number, tokens[5].Type);
        Assert.Equal("10", tokens[5].Value);
        Assert.Equal(BasicTokenType.Step, tokens[6].Type);
        Assert.Equal(BasicTokenType.Number, tokens[7].Type);
        Assert.Equal("2", tokens[7].Value);
    }

    [Fact]
    public void Tokenize_ForLoopWithoutStep_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("FOR i = 1 TO 5");
        var tokens = lexer.Tokenize();

        // FOR, i, =, 1, TO, 5, EOF
        Assert.Equal(7, tokens.Count);
        Assert.Equal(BasicTokenType.For, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("i", tokens[1].Value);
        Assert.Equal(BasicTokenType.Equals, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal(BasicTokenType.To, tokens[4].Type);
        Assert.Equal(BasicTokenType.Number, tokens[5].Type);
        Assert.Equal("5", tokens[5].Value);
    }

    [Fact]
    public void Tokenize_NextWithVariable_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("NEXT i%");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count); // NEXT, i%, EOF
        Assert.Equal(BasicTokenType.Next, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("i%", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_CompleteForLoop_TokenizesAllLines()
    {
        var source = "FOR i = 1 TO 3\nPRINT i\nNEXT i";
        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.For, tokens[0].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[6].Type);
        Assert.Equal(BasicTokenType.Print, tokens[7].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[9].Type);
        Assert.Equal(BasicTokenType.Next, tokens[10].Type);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[tokens.Count - 1].Type);
    }

    #endregion

    #region WHILE Loop Tests

    [Fact]
    public void Tokenize_WhileCondition_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("WHILE x > 0");
        var tokens = lexer.Tokenize();

        // WHILE, x, >, 0, EOF
        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.While, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(BasicTokenType.GreaterThan, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal("0", tokens[3].Value);
    }

    [Fact]
    public void Tokenize_Wend_ReturnsWendToken()
    {
        var lexer = new BasicLexer("WEND");
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count); // WEND, EOF
        Assert.Equal(BasicTokenType.Wend, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_CompleteWhileLoop_TokenizesAllLines()
    {
        var source = "WHILE x > 0\nLET x = x - 1\nWEND";
        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.While, tokens[0].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[4].Type);
        Assert.Equal(BasicTokenType.Let, tokens[5].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[11].Type);
        Assert.Equal(BasicTokenType.Wend, tokens[12].Type);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[tokens.Count - 1].Type);
    }

    #endregion

    #region GOSUB and RETURN Tests

    [Fact]
    public void Tokenize_GosubWithLineNumber_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("GOSUB 1000");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count); // GOSUB, 1000, EOF
        Assert.Equal(BasicTokenType.Gosub, tokens[0].Type);
        Assert.Equal(BasicTokenType.Number, tokens[1].Type);
        Assert.Equal("1000", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_Return_ReturnsReturnToken()
    {
        var lexer = new BasicLexer("RETURN");
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count); // RETURN, EOF
        Assert.Equal(BasicTokenType.Return, tokens[0].Type);
    }

    [Fact]
    public void Tokenize_GosubCallAndReturn_TokenizesCorrectly()
    {
        var source = "10 GOSUB 100\n20 END\n100 PRINT\n110 RETURN";
        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Gosub, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal("100", tokens[2].Value);
    }

    #endregion

    #region GOTO Tests

    [Fact]
    public void Tokenize_GotoWithLineNumber_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("GOTO 200");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count); // GOTO, 200, EOF
        Assert.Equal(BasicTokenType.Goto, tokens[0].Type);
        Assert.Equal(BasicTokenType.Number, tokens[1].Type);
        Assert.Equal("200", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_GotoOnNumberedLine_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("30 GOTO 10");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("30", tokens[0].Value);
        Assert.Equal(BasicTokenType.Goto, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal("10", tokens[2].Value);
    }

    #endregion

    #region IF/THEN/ELSE Tests

    [Fact]
    public void Tokenize_IfThen_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("IF x > 0 THEN PRINT x");
        var tokens = lexer.Tokenize();

        // IF, x, >, 0, THEN, PRINT, x, EOF
        Assert.Equal(8, tokens.Count);
        Assert.Equal(BasicTokenType.If, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(BasicTokenType.GreaterThan, tokens[2].Type);
        Assert.Equal(BasicTokenType.Number, tokens[3].Type);
        Assert.Equal("0", tokens[3].Value);
        Assert.Equal(BasicTokenType.Then, tokens[4].Type);
        Assert.Equal(BasicTokenType.Print, tokens[5].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[6].Type);
    }

    [Fact]
    public void Tokenize_IfThenElse_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("IF x > 0 THEN PRINT \"Pos\" ELSE PRINT \"Neg\"");
        var tokens = lexer.Tokenize();

        // IF, x, >, 0, THEN, PRINT, "Pos", ELSE, PRINT, "Neg", EOF
        Assert.Equal(11, tokens.Count);
        Assert.Equal(BasicTokenType.If, tokens[0].Type);
        Assert.Equal(BasicTokenType.Then, tokens[4].Type);
        Assert.Equal(BasicTokenType.Print, tokens[5].Type);
        Assert.Equal(BasicTokenType.String, tokens[6].Type);
        Assert.Equal("Pos", tokens[6].Value);
        Assert.Equal(BasicTokenType.Else, tokens[7].Type);
        Assert.Equal(BasicTokenType.Print, tokens[8].Type);
        Assert.Equal(BasicTokenType.String, tokens[9].Type);
        Assert.Equal("Neg", tokens[9].Value);
    }

    [Fact]
    public void Tokenize_IfWithCompoundCondition_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("IF x > 0 AND y < 10 THEN PRINT x");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.If, tokens[0].Type);
        Assert.Equal(BasicTokenType.And, tokens[4].Type);
        Assert.Equal(BasicTokenType.Then, tokens[8].Type);
    }

    #endregion

    #region DIM Statement Tests

    [Fact]
    public void Tokenize_DimAsInteger_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("DIM x AS INTEGER");
        var tokens = lexer.Tokenize();

        // DIM, x, AS, INTEGER, EOF
        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Dim, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
        Assert.Equal(BasicTokenType.As, tokens[2].Type);
        Assert.Equal(BasicTokenType.Integer, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_DimAsString_ReturnsStringTypeToken()
    {
        var lexer = new BasicLexer("DIM s AS STRING");
        var tokens = lexer.Tokenize();

        // DIM, s, AS, STRING (String_Type), EOF
        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Dim, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("s", tokens[1].Value);
        Assert.Equal(BasicTokenType.As, tokens[2].Type);
        Assert.Equal(BasicTokenType.String_Type, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_DimAsDouble_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("DIM d AS DOUBLE");
        var tokens = lexer.Tokenize();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Dim, tokens[0].Type);
        Assert.Equal(BasicTokenType.Double, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_DimAsSingle_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("DIM f AS SINGLE");
        var tokens = lexer.Tokenize();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Dim, tokens[0].Type);
        Assert.Equal(BasicTokenType.Single, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_DimWithTypeSuffixedVariable_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("DIM x% AS INTEGER");
        var tokens = lexer.Tokenize();

        Assert.Equal(BasicTokenType.Dim, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("x%", tokens[1].Value);
        Assert.Equal(BasicTokenType.As, tokens[2].Type);
        Assert.Equal(BasicTokenType.Integer, tokens[3].Type);
    }

    #endregion

    #region INPUT Statement Tests

    [Fact]
    public void Tokenize_InputSimple_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("INPUT x");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count); // INPUT, x, EOF
        Assert.Equal(BasicTokenType.Input, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
    }

    [Fact]
    public void Tokenize_InputWithPrompt_ReturnsCorrectTokenSequence()
    {
        var lexer = new BasicLexer("INPUT \"Enter value: \", x%");
        var tokens = lexer.Tokenize();

        // INPUT, "Enter value: ", ,, x%, EOF
        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Input, tokens[0].Type);
        Assert.Equal(BasicTokenType.String, tokens[1].Type);
        Assert.Equal("Enter value: ", tokens[1].Value);
        Assert.Equal(BasicTokenType.Comma, tokens[2].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[3].Type);
        Assert.Equal("x%", tokens[3].Value);
    }

    [Fact]
    public void Tokenize_InputWithTypedVariable_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("INPUT name$");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(BasicTokenType.Input, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal("name$", tokens[1].Value);
    }

    #endregion

    #region PRINT Formatting Tests

    [Fact]
    public void Tokenize_PrintWithSemicolons_ReturnsSemicolonTokens()
    {
        // Semicolon in PRINT suppresses newline between values
        var lexer = new BasicLexer("PRINT a; b; c");
        var tokens = lexer.Tokenize();

        // PRINT, a, ;, b, ;, c, EOF
        Assert.Equal(7, tokens.Count);
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[1].Type);
        Assert.Equal(BasicTokenType.Semicolon, tokens[2].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[3].Type);
        Assert.Equal(BasicTokenType.Semicolon, tokens[4].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[5].Type);
    }

    [Fact]
    public void Tokenize_PrintWithCommas_ReturnsCommaTokens()
    {
        // Comma in PRINT advances to next tab stop
        var lexer = new BasicLexer("PRINT a, b, c");
        var tokens = lexer.Tokenize();

        // PRINT, a, ,, b, ,, c, EOF
        Assert.Equal(7, tokens.Count);
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.Comma, tokens[2].Type);
        Assert.Equal(BasicTokenType.Comma, tokens[4].Type);
    }

    [Fact]
    public void Tokenize_PrintMixedStringsAndVariables_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("PRINT \"Value:\"; x%");
        var tokens = lexer.Tokenize();

        // PRINT, "Value:", ;, x%, EOF
        Assert.Equal(5, tokens.Count);
        Assert.Equal(BasicTokenType.Print, tokens[0].Type);
        Assert.Equal(BasicTokenType.String, tokens[1].Type);
        Assert.Equal("Value:", tokens[1].Value);
        Assert.Equal(BasicTokenType.Semicolon, tokens[2].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[3].Type);
        Assert.Equal("x%", tokens[3].Value);
    }

    #endregion

    #region Multiple Statements on One Line Tests

    [Fact]
    public void Tokenize_ColonSeparatedStatements_ReturnsColonToken()
    {
        var lexer = new BasicLexer("LET x = 1 : PRINT x");
        var tokens = lexer.Tokenize();

        // LET, x, =, 1, :, PRINT, x, EOF
        Assert.Equal(8, tokens.Count);
        Assert.Equal(BasicTokenType.Let, tokens[0].Type);
        Assert.Equal(BasicTokenType.Colon, tokens[4].Type);
        Assert.Equal(BasicTokenType.Print, tokens[5].Type);
    }

    [Fact]
    public void Tokenize_MultipleStatementsNoSpacesAroundColon_ReturnsCorrectTokens()
    {
        var lexer = new BasicLexer("LET x=1:PRINT x");
        var tokens = lexer.Tokenize();

        // LET, x, =, 1, :, PRINT, x, EOF
        Assert.Equal(8, tokens.Count);
        Assert.Equal(BasicTokenType.Let, tokens[0].Type);
        Assert.Equal(BasicTokenType.Colon, tokens[4].Type);
        Assert.Equal(BasicTokenType.Print, tokens[5].Type);
    }

    #endregion

    #region Token Position Tests

    [Fact]
    public void Tokenize_FirstToken_StartsAtLine1Column1()
    {
        var lexer = new BasicLexer("PRINT");
        var tokens = lexer.Tokenize();

        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);
    }

    [Fact]
    public void Tokenize_SecondTokenAfterSpace_HasCorrectColumn()
    {
        var lexer = new BasicLexer("10 PRINT");
        var tokens = lexer.Tokenize();

        // "10" starts at column 1
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);
        // "PRINT" starts at column 4 (after "10 ")
        Assert.Equal(1, tokens[1].Line);
        Assert.Equal(4, tokens[1].Column);
    }

    [Fact]
    public void Tokenize_SecondLine_HasCorrectLineNumber()
    {
        var lexer = new BasicLexer("PRINT\nLET");
        var tokens = lexer.Tokenize();

        // PRINT at line 1
        Assert.Equal(1, tokens[0].Line);
        // NewLine token at line 1
        Assert.Equal(1, tokens[1].Line);
        // LET at line 2
        Assert.Equal(2, tokens[2].Line);
        Assert.Equal(1, tokens[2].Column);
    }

    [Fact]
    public void Tokenize_ThirdLine_HasCorrectLineNumber()
    {
        var lexer = new BasicLexer("10 PRINT\n20 LET\n30 END");
        var tokens = lexer.Tokenize();

        // "10" on line 1
        Assert.Equal(1, tokens[0].Line);
        // "30" on line 3
        var endLineToken = tokens.First(t => t.Value == "30");
        Assert.Equal(3, endLineToken.Line);
        Assert.Equal(1, endLineToken.Column);
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

    [Fact]
    public void Tokenize_FullProgram_CounterLoop()
    {
        var source = """
            10 DIM i AS INTEGER
            20 FOR i = 1 TO 5
            30 PRINT i
            40 NEXT i
            50 END
            """;

        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();

        // Line 10: DIM, i, AS, INTEGER, NewLine
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Dim, tokens[1].Type);
        Assert.Equal(BasicTokenType.Identifier, tokens[2].Type);
        Assert.Equal(BasicTokenType.As, tokens[3].Type);
        Assert.Equal(BasicTokenType.Integer, tokens[4].Type);
        Assert.Equal(BasicTokenType.NewLine, tokens[5].Type);

        // Line 20: 20, FOR, i, =, 1, TO, 5, NewLine
        Assert.Equal(BasicTokenType.Number, tokens[6].Type);
        Assert.Equal("20", tokens[6].Value);
        Assert.Equal(BasicTokenType.For, tokens[7].Type);

        Assert.Equal(BasicTokenType.EndOfFile, tokens[tokens.Count - 1].Type);
    }

    [Fact]
    public void Tokenize_FullProgram_SubroutineWithGosubReturn()
    {
        var source = """
            10 GOSUB 100
            20 END
            100 PRINT "In subroutine"
            110 RETURN
            """;

        var lexer = new BasicLexer(source);
        var tokens = lexer.Tokenize();

        // 10, GOSUB, 100, NewLine
        Assert.Equal(BasicTokenType.Number, tokens[0].Type);
        Assert.Equal("10", tokens[0].Value);
        Assert.Equal(BasicTokenType.Gosub, tokens[1].Type);
        Assert.Equal(BasicTokenType.Number, tokens[2].Type);
        Assert.Equal("100", tokens[2].Value);
        Assert.Equal(BasicTokenType.NewLine, tokens[3].Type);

        // 20, END, NewLine
        Assert.Equal(BasicTokenType.Number, tokens[4].Type);
        Assert.Equal(BasicTokenType.End, tokens[5].Type);

        // Verify RETURN appears in the token stream
        Assert.Contains(tokens, t => t.Type == BasicTokenType.Return);
        Assert.Equal(BasicTokenType.EndOfFile, tokens[tokens.Count - 1].Type);
    }

    #endregion
}
