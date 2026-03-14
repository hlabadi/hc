// See https://aka.ms/new-console-template for more information

using hc.Frontends.Basic;

var source = """
             10 PRINT "Hello, World!"
             20 LET x% = 5
             30 IF x% > 3 THEN PRINT "Big"
             """;

var lexer = new BasicLexer(source);
var tokens = lexer.Tokenize();

foreach (var token in tokens)
{
    Console.WriteLine(token);
}