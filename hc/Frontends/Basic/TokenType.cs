namespace hc.Frontends.Basic;

public enum BasicTokenType
{
    // Literals
    Number, String,
        
    // Keywords
    Print, Input, Let, If, Then, Else, For, To, Step, Next, While, Wend,
    Goto, Gosub, Return, End, Dim, As, Integer, Single, Double, String_Type,
    And, Or, Not, Rem,
        
    // Operators
    Plus, Minus, Star, Slash, Equals, LessThan, GreaterThan,
    LessEqual, GreaterEqual, NotEqual, Caret, Backslash, Mod,
        
    // Punctuation
    LeftParen, RightParen, Comma, Semicolon, Colon,
        
    // Line number
    LineNumber,
        
    // Identifier
    Identifier,
        
    // Special
    NewLine, EndOfFile
}