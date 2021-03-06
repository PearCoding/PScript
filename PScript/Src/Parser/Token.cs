﻿/*
 * Copyright (c) 2016, Ömercan Yazici <omercan AT pearcoding.eu>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 *    1. Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *
 *    2. Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *
 *    3. Neither the name of the copyright owner may be used
 *       to endorse or promote products derived from this software without
 *       specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE
 */

namespace PScript.Parser
{
    internal enum TokenType
    {
        Identifier,
        String,
        Integer,
        Float,
        //Identifiers
        True,
        False,
        Const,
        Import,
        Function,
        Return,
        Var,
        Ref,
        For,
        While,
        Do,
        Break,
        Continue,
        Switch,
        Case,
        Default,
        If,
        Else,
        Has,
        Is,
        In,
        Null,
        //Brackets
        OpenParanthese,
        CloseParanthese,
        OpenSquareBracket,
        CloseSquareBracket,
        OpenCurlyBracket,
        CloseCurlyBracket,
        // Single tokens
        Comma,
        Colon,
        Semicolon,
        QuestionMark,
        // Repeat-type tokens
        Period,
        Ellipses,
        // Two-Choice-Equal-type tokens
        Equal,
        Assign,
        Log_Not,
        NotEqual,
        Divide,
        DivideAssign,
        Modulo,
        ModuloAssign,
        Bin_Xor,
        Bin_XorAssign,
        Bin_Not,
        Bin_NotAssign,
        // Combination tokens
        Multiply,
        MultiplyAssign,
        Pow,
        PowAssign,
        Lesser,
        Greater,
        LesserEqual,
        GreaterEqual,
        LeftShift,
        LeftShiftAssign,
        RightShift,
        RightShiftAssign,
        //Three type tokens
        Bin_And,
        Bin_AndAssign,
        Bin_Or,
        Bin_OrAssign,
        Log_And,
        Log_Or,
        Plus,
        PlusAssign,
        Increment,
        Minus,
        MinusAssign,
        Decrement,
        //Special
        EOF
    }

    internal class Token
    {
        public TokenType Type;
        public string Value;

        public Token(TokenType type)
        {
            Type = type;
        }
        public Token(TokenType type, string val)
        {
            Type = type;
            Value = val;
        }
    }
}
