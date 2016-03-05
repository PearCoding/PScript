using System;
using System.Collections.Generic;

namespace PScript.Parser
{
    internal class Lexer
    {
        struct ChoiceTypeToken
        {
            public char Character;
            public TokenType[] Choices;

            public ChoiceTypeToken(char c, TokenType[] choices)
            {
                Character = c;
                Choices = choices;
            }
        }
        
        static KeyValuePair<Char, TokenType>[] singleTokens = new KeyValuePair<char, TokenType>[]
        {
            new KeyValuePair<char, TokenType>('(', TokenType.OpenParanthese),
            new KeyValuePair<char, TokenType>(')', TokenType.CloseParanthese),
            new KeyValuePair<char, TokenType>('[', TokenType.OpenSquareBracket),
            new KeyValuePair<char, TokenType>(']', TokenType.CloseSquareBracket),
            new KeyValuePair<char, TokenType>('{', TokenType.OpenCurlyBracket),
            new KeyValuePair<char, TokenType>('}', TokenType.CloseCurlyBracket),
            new KeyValuePair<char, TokenType>(',', TokenType.Comma),
            new KeyValuePair<char, TokenType>(':', TokenType.Colon),
            new KeyValuePair<char, TokenType>(';', TokenType.Semicolon),
            new KeyValuePair<char, TokenType>('?', TokenType.QuestionMark),
            new KeyValuePair<char, TokenType>('^', TokenType.Bin_Xor),
            new KeyValuePair<char, TokenType>('~', TokenType.Bin_Not),
        };

        static KeyValuePair<string, TokenType>[] identifiers = new KeyValuePair<string, TokenType>[]
        {
            new KeyValuePair<string, TokenType>("true", TokenType.True),
            new KeyValuePair<string, TokenType>("false", TokenType.False),
            new KeyValuePair<string, TokenType>("import", TokenType.Import),
            new KeyValuePair<string, TokenType>("function", TokenType.Function),
            new KeyValuePair<string, TokenType>("return", TokenType.Return),
            new KeyValuePair<string, TokenType>("const", TokenType.Const),
            new KeyValuePair<string, TokenType>("for", TokenType.For),
            new KeyValuePair<string, TokenType>("while", TokenType.While),
            new KeyValuePair<string, TokenType>("do", TokenType.Do),
            new KeyValuePair<string, TokenType>("break", TokenType.Break),
            new KeyValuePair<string, TokenType>("continue", TokenType.Continue),
            new KeyValuePair<string, TokenType>("switch", TokenType.Switch),
            new KeyValuePair<string, TokenType>("case", TokenType.Case),
            new KeyValuePair<string, TokenType>("default", TokenType.Default),
            new KeyValuePair<string, TokenType>("if", TokenType.If),
            new KeyValuePair<string, TokenType>("else", TokenType.Else),
            new KeyValuePair<string, TokenType>("has", TokenType.Has),
            new KeyValuePair<string, TokenType>("is", TokenType.Is),
            new KeyValuePair<string, TokenType>("in", TokenType.In),
            new KeyValuePair<string, TokenType>("null", TokenType.Null),
            new KeyValuePair<string, TokenType>("and", TokenType.Log_And),
            new KeyValuePair<string, TokenType>("or", TokenType.Log_Or),
            new KeyValuePair<string, TokenType>("not", TokenType.Log_Not),
        };

        static ChoiceTypeToken[] repeatTypeTokens = new ChoiceTypeToken[]
        {
            new ChoiceTypeToken('.', new TokenType[]{TokenType.Period, TokenType.Ellipses}),
        };

        static ChoiceTypeToken[] equalTypeTokens = new ChoiceTypeToken[]
        {
            new ChoiceTypeToken('=', new TokenType[]{TokenType.Assign, TokenType.Equal }),
            new ChoiceTypeToken('!', new TokenType[]{TokenType.Log_Not, TokenType.NotEqual }),
            new ChoiceTypeToken('/', new TokenType[]{TokenType.Divide, TokenType.DivideAssign }),
            new ChoiceTypeToken('%', new TokenType[]{TokenType.Modulo, TokenType.ModuloAssign }),
            new ChoiceTypeToken('^', new TokenType[]{TokenType.Bin_Xor, TokenType.Bin_XorAssign }),
            new ChoiceTypeToken('~', new TokenType[]{TokenType.Bin_Not, TokenType.Bin_NotAssign }),
        };

        static ChoiceTypeToken[] combinationTypeTokens = new ChoiceTypeToken[]
        {
            new ChoiceTypeToken('*', new TokenType[] {TokenType.Multiply, TokenType.MultiplyAssign, TokenType.Pow, TokenType.PowAssign}),
            new ChoiceTypeToken('<', new TokenType[] {TokenType.Lesser, TokenType.LesserEqual, TokenType.LeftShift, TokenType.LeftShiftAssign}),
            new ChoiceTypeToken('>', new TokenType[] {TokenType.Greater, TokenType.GreaterEqual, TokenType.RightShift, TokenType.RightShiftAssign}),
        };

        static ChoiceTypeToken[] threeTypeTokens = new ChoiceTypeToken[]
        {
            new ChoiceTypeToken('&', new TokenType[] {TokenType.Bin_And, TokenType.Bin_AndAssign, TokenType.Log_And}),
            new ChoiceTypeToken('|', new TokenType[] {TokenType.Bin_Or, TokenType.Bin_OrAssign, TokenType.Log_Or}),
            new ChoiceTypeToken('+', new TokenType[]{TokenType.Plus, TokenType.PlusAssign, TokenType.Increment }),
            new ChoiceTypeToken('-', new TokenType[]{TokenType.Minus, TokenType.MinusAssign, TokenType.Decrement })
        };

        string _Source;
        Logger _Logger;
        int _Position = 0;

        int _Line = 1;
        public int CurrentLine
        {
            get { return _Line; }
        }

        int _Column = 1;
        public int CurrentColumn
        {
            get { return _Column; }
        }

        public Lexer(string source, Logger logger)
        {
            _Source = source;
            _Logger = logger;
        }

        public Token Next()
        {
            if (_Position >= _Source.Length)
            {
                return new Token(TokenType.EOF);
            }

            foreach (var pair in singleTokens)
            {
                if (CurrentChar == pair.Key)
                {
                    ++_Position;
                    ++_Column;

                    return new Token(pair.Value);
                }
            }

            foreach(var entry in repeatTypeTokens)
            {
                if(CurrentChar == entry.Character)
                {
                    ++_Position;
                    ++_Column;

                    if(_Position < _Source.Length &&
                        CurrentChar == entry.Character)
                    {
                        ++_Position;
                        ++_Column;
                        if (_Position < _Source.Length &&
                            CurrentChar == entry.Character)
                        {
                            ++_Position;
                            ++_Column;
                            return new Token(entry.Choices[1]);
                        }
                        else
                        {//Back to normal
                            _Position -= 2;
                            _Column -= 2;
                        }
                    }
                    else
                    {
                        return new Token(entry.Choices[0]);
                    }
                }
            }

            foreach(var entry in equalTypeTokens)
            {
                if (CurrentChar == entry.Character)
                {
                    ++_Position;
                    ++_Column;

                    if (_Position < _Source.Length &&
                        CurrentChar == '=')
                    {
                        ++_Position;
                        ++_Column;
                        return new Token(entry.Choices[1]);
                    }
                    else
                    {
                        return new Token(entry.Choices[0]);
                    }
                }
            }

            foreach (var entry in combinationTypeTokens)
            {
                if (CurrentChar == entry.Character)
                {
                    ++_Position;
                    ++_Column;

                    if (_Position < _Source.Length &&
                        CurrentChar == '=')
                    {
                        ++_Position;
                        ++_Column;
                        return new Token(entry.Choices[1]);
                    }
                    else if(_Position < _Source.Length &&
                        CurrentChar == entry.Character)
                    {
                        ++_Position;
                        ++_Column;

                        if (_Position < _Source.Length &&
                            CurrentChar == '=')
                        {
                            ++_Position;
                            ++_Column;
                            return new Token(entry.Choices[3]);
                        }
                        else
                        {
                            return new Token(entry.Choices[2]);
                        }
                    }
                    else
                    {
                        return new Token(entry.Choices[0]);
                    }
                }
            }

            foreach (var entry in threeTypeTokens)
            {
                if (CurrentChar == entry.Character)
                {
                    ++_Position;
                    ++_Column;

                    if (_Position < _Source.Length &&
                        CurrentChar == '=')
                    {
                        ++_Position;
                        ++_Column;
                        return new Token(entry.Choices[1]);
                    }
                    else if (_Position < _Source.Length &&
                        CurrentChar == entry.Character)
                    {
                        ++_Position;
                        ++_Column;
                        return new Token(entry.Choices[2]);
                    }
                    else
                    {
                        return new Token(entry.Choices[0]);
                    }
                }
            }

            if (CurrentChar == '#')//Comment
            {
                while (_Position < _Source.Length && CurrentChar != '\n')
                {
                    ++_Position;
                    ++_Column;
                }

                return Next();
            }
            else if (CurrentChar == '"' || CurrentChar == '\'')//String
            {
                char start = CurrentChar;
                string str = "";
                while (true)
                {
                    ++_Position;
                    ++_Column;

                    if (_Position >= _Source.Length ||
                        CurrentChar == '\n')
                    {
                        throw new Error(ErrorType.Lexer_StringNotClosed,
                            _Column, _Line, str);
                    }
                    else if (CurrentChar == '\\')
                    {
                        ++_Position;
                        ++_Column;

                        if (_Position >= _Source.Length ||
                            CurrentChar == '\n')
                        {
                            throw new Error(ErrorType.Lexer_InvalidOperator, _Column, _Line,
                                "\\");
                        }
                        str += CurrentChar;
                    }
                    else if (CurrentChar == start)
                    {
                        ++_Position;
                        ++_Column;
                        break;
                    }
                    else
                    {
                        str += CurrentChar;
                    }
                }

                return new Token(TokenType.String, str);
            }
            else if (IsDigit(CurrentChar))
            {
                string identifier = "";
                identifier += CurrentChar;

                ++_Position;
                ++_Column;
                while (_Position < _Source.Length)
                {
                    if (IsDigit(CurrentChar) || CurrentChar == '.')
                    {
                        identifier += CurrentChar;
                        ++_Position;
                        ++_Column;
                    }
                    else
                    {
                        break;
                    }
                }

                if (IsInteger(identifier))
                {
                    return new Token(TokenType.Integer, identifier);
                }
                else if (IsFloat(identifier))
                {
                    return new Token(TokenType.Float, identifier);
                }
                else
                {
                    throw new Error(ErrorType.Lexer_UnknownIdentifier, _Column, _Line, identifier);
                }
            }
            else if (IsAlpha(CurrentChar))//Identifier
            {
                string identifier = "";
                identifier += CurrentChar;

                ++_Position;
                ++_Column;
                while (_Position < _Source.Length)
                {
                    if (IsAscii(CurrentChar))
                    {
                        identifier += CurrentChar;
                        ++_Position;
                        ++_Column;
                    }
                    else
                    {
                        break;
                    }
                }

                
                foreach(var pair in identifiers)
                {
                    if(pair.Key == identifier)
                    {
                        return new Token(pair.Value);
                    }
                }

                return new Token(TokenType.Identifier, identifier);
            }
            else if (IsWhitespace(CurrentChar))
            {
                if (CurrentChar == '\n')
                {
                    ++_Line;
                    _Column = 0;
                }

                ++_Position;
                ++_Column;
                return Next();
            }
            else
            {
                throw new Error(ErrorType.Lexer_UnknownCharacter, _Column, _Line, CurrentChar);
            }
        }

        public Token Look(int i = 1)
        {
            if (i < 1)
                throw new Error(ErrorType.Internal_LookParamLessOne);

            int pos = _Position;
            int line = _Line;
            int col = _Column;

            while(--i > 0)
                Next();

            Token t = Next();

            _Position = pos;
            _Line = line;
            _Column = col;

            return t;
        }

        static private bool IsWhitespace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\r' ||
                c == '\n' || c == '\v' || c == '\f')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsAscii(char c)
        {
            if (IsDigit(c) || IsAlpha(c))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static private bool IsDigit(char c)
        {
            if (c == '1' || c == '2' || c == '3' ||
            c == '4' || c == '5' || c == '6' ||
            c == '7' || c == '8' || c == '9' ||
            c == '0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsAlpha(char c)
        {
            if (c == 'a' || c == 'b' ||
            c == 'c' || c == 'd' || c == 'e' ||
            c == 'f' || c == 'g' || c == 'h' ||
            c == 'i' || c == 'j' || c == 'k' ||
            c == 'l' || c == 'm' || c == 'n' ||
            c == 'o' || c == 'p' || c == 'q' ||
            c == 'r' || c == 's' || c == 't' ||
            c == 'u' || c == 'v' || c == 'w' ||
            c == 'x' || c == 'y' || c == 'z' ||
            c == 'A' || c == 'B' || c == 'C' ||
            c == 'D' || c == 'E' || c == 'F' ||
            c == 'G' || c == 'H' || c == 'I' ||
            c == 'J' || c == 'K' || c == 'L' ||
            c == 'M' || c == 'N' || c == 'O' ||
            c == 'P' || c == 'Q' || c == 'R' ||
            c == 'S' || c == 'T' || c == 'U' ||
            c == 'V' || c == 'W' || c == 'X' ||
            c == 'Y' || c == 'Z' || c == '_')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsInteger(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        static private bool IsFloat(string str)
        {
            bool point = false;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];

                if (c != '.' &&
                !IsDigit(c))
                {
                    return false;
                }
                else if (c == '.')
                {
                    if (point)
                        return false;

                    point = true;
                }
            }

            return true;
        }

        private char CurrentChar
        {
            get { return _Source[_Position]; }
        }
    }
}
