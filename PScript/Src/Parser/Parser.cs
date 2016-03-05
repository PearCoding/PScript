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
using System.Globalization;

namespace PScript.Parser
{
    internal class Parser
    {
        Lexer _Lexer;
        Logger _Logger;

        public Parser(string source, Logger logger)
        {
            _Lexer = new Lexer(source, logger);
            _Logger = logger;
        }

        public SyntaxTree Parse()
        {
            return gr_tr_unit();
        }

        // Internal
        SyntaxTree gr_tr_unit()
        {
            SyntaxTree tree = new SyntaxTree();
            tree.Statements = gr_stmt_list();
            return tree;
        }

       
        // Utils
        Token Match(TokenType type)
        {
            Token token = _Lexer.Next();
            if (token.Type != type)
            {
                throw new Error(ErrorType.Parser_WrongToken, _Lexer.CurrentLine, _Lexer.CurrentColumn, type, token.Type);
            }
            return token;
        }

        bool Lookahead(TokenType type)
        {
            Token token = _Lexer.Look();
            return token.Type == type;
        }       
    }
}
