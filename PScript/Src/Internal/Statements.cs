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

using System.Collections.Generic;

namespace PScript.Internal
{
    internal enum StatementType
    {
        Expression,
        Declaration,
        If,
        Switch,
        ForEach,
        For,
        While,
        DoWhile,
        Break,
        Continue,
        Return,
        Function,
        Import,
    }

    internal class Statement
    {
        protected StatementType _Type;
        public StatementType Type { get { return _Type; } }
    }

    internal class ExpressionStatement : Statement
    {
        public Expression Expression;

        public ExpressionStatement(Expression expr)
        {
            _Type = StatementType.Expression;
            Expression = expr;
        }
    }

    internal class DeclarationStatement : Statement
    {
        public DeclarationExpression Expression;

        public DeclarationStatement(DeclarationExpression expr)
        {
            _Type = StatementType.Declaration;
            Expression = expr;
        }
    }

    internal class IfStatement : Statement
    {
        public ConditionalExpression Condition;

        // Never both
        public IfStatement ElseIf;
        public Compound Else;

        public IfStatement(ConditionalExpression cond)
        {
            _Type = StatementType.If;
            Condition = cond;
        }
    }

    internal class SwitchStatement : Statement
    {
        public Expression Expression;
        public List<CaseLine> Cases;
        public Compound DefaultCase;

        public SwitchStatement(Expression expr)
        {
            _Type = StatementType.Switch;
            Expression = expr;
        }
    }


}
