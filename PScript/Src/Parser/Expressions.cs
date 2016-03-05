/*
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

using System;

namespace PScript.Parser
{
    internal enum ExpressionType
    {
        Condition,
        Call,
        ArrayAccess,
        MemberAccess,
        Array,

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
        Bin_And,
        Bin_AndAssign,
        Bin_Or,
        Bin_OrAssign,
        Log_And,
        Log_Or,
        Plus,
        PlusAssign,
        Minus,
        MinusAssign,
        IncrementPre,
        DecrementPre,
        IncrementPost,
        DecrementPost,
    }

    internal class Expression
    {
        public ExpressionType Type;
        public Expression[] Expressions;
        public Literal Literal;

        public Expression(ExpressionType type, Literal literal)
        {
            Type = type;
            Literal = literal;
        }

        public Expression(ExpressionType type, Expression[] expressions)
        {
            Type = type;
            Expressions = expressions;
        }
    }
    
    internal class DeclarationExpression
    {
        public string Name;
        public Expression Expression;

        public DeclarationExpression(string name, Expression expr)
        {
            Name = name;
            Expression = expr;
        }
    }

    internal enum LiteralType
    {
        Integer,
        Float,
        String,
        Identifier,
        Boolean,
        Null,
    }

    internal class Literal
    {
        public LiteralType Type;

        public int Integer;
        public float Float;
        public string String;
        public Boolean Bool;

        public Literal(LiteralType type)
        {
            Type = type;
        }

        public Literal(int val)
        {
            Type = LiteralType.Integer;
            Integer = val;
        }

        public Literal(float f)
        {
            Type = LiteralType.Float;
            Float = f;
        }

        public Literal(LiteralType type, string str)
        {
            Type = type;
            String = str;
        }
    }
}
