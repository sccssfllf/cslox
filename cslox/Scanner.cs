using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static cslox.TokenType;

namespace cslox;

public class Scanner
{
    private string Source { get; }
    private readonly List<Token> _tokens = new List<Token>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
    {
        { "and",    AND },
        { "class",  CLASS },
        { "else",   ELSE },
        { "false",  FALSE },
        { "fun",    FUN },
        { "for",    FOR },
        { "if",     IF },
        { "nil",    NIL },
        { "or",     OR },
        { "print",  PRINT },
        { "return", RETURN },
        { "super",  SUPER },
        { "this",   THIS },
        { "true",   TRUE },
        { "var",    VAR },
        { "while",  WHILE }
    };
    
    
    
    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }
        
        _tokens.Add(new Token(EOF, "", null, _line));
        return _tokens;
    }

    private bool IsAtEnd()
    {
        return _current >= Source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break;
            case '!': AddToken(MatchToken('=') ? BANG_EQUAL : BANG); break;
            case '=': AddToken(MatchToken('=') ? EQUAL_EQUAL : EQUAL); break;
            case '<': AddToken(MatchToken('=') ? LESS_EQUAL : LESS); break;
            case '>': AddToken(MatchToken('=') ? GREATER_EQUAL: GREATER); break;
            case '/':
                if (MatchToken('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(SLASH);
                }
                break;
            
            case ' ':
            case '\r':
            case '\t':
                break;
            
            case '\n':
                _line++;
                break;
            
            case '"': StringLiteral(); break;
            
            default:
                if (IsDigit(c))
                {
                    NumberLiteral();
                } 
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Lox.Error(_line, "Unexpected character.");
                }
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = Source.Substring(_start, _current - _start);
        TokenType type;
        if (!Keywords.TryGetValue(text, out type))
        {
            type = IDENTIFIER;
        }
        
        AddToken(type);
    }

    private static bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsDigit(c) || IsAlpha(c);
    }

    private void StringLiteral()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Undetermined string.");
        }

        Advance();

        string value = Source.Substring(_start + 1, _current - _start - 2);
        AddToken(STRING, value);
    }

    private void NumberLiteral()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        string text = Source.Substring(_start, _current - _start);
        AddToken(NUMBER, double.Parse(text, CultureInfo.InvariantCulture));
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
    
    private bool MatchToken(char expected)
    {
        if (IsAtEnd()) return false;
        if (Source[_current] != expected) return false;

        _current++;
        return true;
    }
    
    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return Source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= Source.Length) return '\0';
        return Source[_current + 1];
    }

    private char Advance()
    {
        _current++;
        return Source[_current - 1];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        string text = Source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }
}