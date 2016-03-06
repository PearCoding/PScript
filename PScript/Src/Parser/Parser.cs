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

        Compound gr_stmt_list()
        {
            var list = new List<Statement>();
            while(true)
            {
                Statement statement = gr_stmt();

                if (statement != null)
                    list.Add(statement);
                else
                    break;
            };

            Compound compound = new Compound();
            compound.Statements = list;
            return compound;
        }

        Compound gr_compound()
        {
            Match(TokenType.OpenCurlyBracket);
            var r = gr_stmt_list();
            Match(TokenType.CloseCurlyBracket);
            return r;
        }

        Statement gr_stmt()
        {
            if (IsEOF())
                return null;

            if (Lookahead(TokenType.Function))
                return gr_func_stmt();
            else if (Lookahead(TokenType.Import))
                return gr_import_stmt();
            else if (Lookahead(TokenType.Break))
            {
                Match(TokenType.Break);
                return new BreakStatement();
            }
            else if (Lookahead(TokenType.Continue))
            {
                Match(TokenType.Continue);
                return new ContinueStatement();
            }
            else if (Lookahead(TokenType.Return))
                return gr_return_stmt();
            else if (Lookahead(TokenType.Var) || Lookahead(TokenType.Const))
                return gr_decl_stmt();
            else if (Lookahead(TokenType.For))
                return gr_for_stmt();
            else if (Lookahead(TokenType.While))
                return gr_while_stmt();
            else if (Lookahead(TokenType.Do))
                return gr_dowhile_stmt();
            else if (Lookahead(TokenType.If))
                return gr_if_stmt();
            else if (Lookahead(TokenType.Switch))
                return gr_switch_stmt();
            else
                return gr_expr_stmt();
        }

        //Functions
        Statement gr_func_stmt()
        {
            Match(TokenType.Function);
            Token identifier = Match(TokenType.Identifier);
            Match(TokenType.OpenParanthese);
            List<FunctionParameter> parameters = gr_func_params();
            Match(TokenType.CloseParanthese);

            if(Lookahead(TokenType.Semicolon))//Only header
            {
                Match(TokenType.Semicolon);
                FunctionStatement statement = new FunctionStatement(identifier.Value, null);
                statement.Parameters = parameters;
                return statement;
            }
            else
            {
                FunctionStatement statement = new FunctionStatement(identifier.Value, gr_compound());
                statement.Parameters = parameters;
                return statement;
            }
        }

        List<FunctionParameter> gr_func_params()
        {
            var list = new List<FunctionParameter>();
            bool hadEllipses = false;
            bool hadDefaults = false;

            while(true)
            {
                if (Lookahead(TokenType.CloseParanthese))
                    break;
                else if (Lookahead(TokenType.Ellipses))
                {
                    Match(TokenType.Ellipses);
                    if (!hadEllipses)
                    {
                        hadEllipses = true;
                        list.Add(null);//Special
                    }
                    else
                        throw new Error(ErrorType.Parser_EllipsesInvalidCount, _Lexer.CurrentLine, _Lexer.CurrentColumn);
                }
                else if (hadEllipses)
                    throw new Error(ErrorType.Parser_EllipsesNotLast, _Lexer.CurrentLine, _Lexer.CurrentColumn);
                else
                {
                    Token identifier = Match(TokenType.Identifier);

                    if (Lookahead(TokenType.Assign))
                    {
                        Match(TokenType.Assign);
                        hadDefaults = true;
                        list.Add(new FunctionParameter(identifier.Value, gr_const_expr()));
                    }
                    else if (hadDefaults)
                        throw new Error(ErrorType.Parser_NonDefaultAfterDefault, _Lexer.CurrentLine, _Lexer.CurrentColumn);
                    else
                        list.Add(new FunctionParameter(identifier.Value, null));
                }
            }

            return list;
        }

        // Imports
        Statement gr_import_stmt()
        {
            List<string> path = new List<string>();

            Match(TokenType.Import);
            path.Add(Match(TokenType.Identifier).Value);

            bool hadMultiply = false;
            while(Lookahead(TokenType.Period))
            {
                if (hadMultiply)
                    throw new Error(ErrorType.Parser_EntryAfterAllAccess, _Lexer.CurrentLine, _Lexer.CurrentColumn);

                Match(TokenType.Period);

                if(Lookahead(TokenType.Multiply))
                {
                    Match(TokenType.Multiply);
                    hadMultiply = true;
                    path.Add(null);
                }
                else
                {
                    path.Add(Match(TokenType.Identifier).Value);
                }
            }

            ImportStatement stmt = new ImportStatement();
            stmt.Namespace = path;
            return stmt;
        }

        Statement gr_decl_stmt()
        {
            Statement stmt = new DeclarationStatement(gr_decl_expr());
            Match(TokenType.Semicolon);
            return stmt;
        }

        // Jump Statements
        Statement gr_return_stmt()
        {
            Match(TokenType.Return);
            ReturnStatement stmt = new ReturnStatement();
            stmt.Expressions = gr_expr_list();
            return stmt;
        }

        // Iterative Statements
        Statement gr_for_stmt()
        {
            Match(TokenType.For);
            Match(TokenType.OpenParanthese);

            if(Lookahead(TokenType.Identifier) && Lookahead(TokenType.In, 2))
            {
                Token var = Match(TokenType.Identifier);
                Match(TokenType.In);
                Expression expr = gr_expr();
                Match(TokenType.CloseParanthese);
                return new ForEachStatement(var.Value, expr, gr_compound());
            }
            else
            {
                DeclarationExpression decl = null;
                if (!Lookahead(TokenType.Semicolon))
                    decl = gr_decl_expr();
                Match(TokenType.Semicolon);

                Expression cond = null;
                if (!Lookahead(TokenType.Semicolon))
                    cond = gr_cond_expr();
                Match(TokenType.Semicolon);

                Expression next = null;
                if (!Lookahead(TokenType.OpenParanthese))
                    next = gr_expr();
                Match(TokenType.CloseParanthese);

                return new ForStatement(decl, cond, next, gr_compound());
            }
        }

        Statement gr_while_stmt()
        {
            Match(TokenType.While);
            Match(TokenType.OpenParanthese);
            Expression cond = gr_cond_expr();
            Match(TokenType.CloseParanthese);
            return new WhileStatement(cond, gr_compound());
        }

        Statement gr_dowhile_stmt()
        {
            Match(TokenType.Do);
            Compound body = gr_compound();
            Match(TokenType.While);
            Match(TokenType.OpenParanthese);
            Expression cond = gr_cond_expr();
            Match(TokenType.CloseParanthese);
            Match(TokenType.Semicolon);
            return new DoWhileStatement(cond, body);
        }

        // Conditional Statements
        IfStatement gr_if_stmt()
        {
            Match(TokenType.If);
            Match(TokenType.OpenParanthese);
            Expression cond = gr_cond_expr();
            Match(TokenType.CloseParanthese);
            Compound ifBody = gr_compound();

            IfStatement stmt = new IfStatement(cond, ifBody);
            if (Lookahead(TokenType.Else))
            {
                Match(TokenType.Else);
                if (Lookahead(TokenType.If))
                {
                    stmt.ElseIf = gr_if_stmt();
                }
                else
                {
                    stmt.ElseBody = gr_compound();
                }
            }
            return stmt;
        }

        Statement gr_switch_stmt()
        {
            Match(TokenType.Switch);
            Match(TokenType.OpenParanthese);
            Expression expr = gr_expr();
            Match(TokenType.CloseParanthese);

            SwitchStatement stmt = new SwitchStatement(expr);
            stmt.Cases = new List<CaseLine>();

            Match(TokenType.OpenCurlyBracket);
            while(!Lookahead(TokenType.CloseCurlyBracket) && !IsEOF())
            {
                if(Lookahead(TokenType.Default))
                {
                    Match(TokenType.Default);
                    Match(TokenType.Colon);

                    if (stmt.DefaultCase != null)
                        throw new Error(ErrorType.Parser_MultipleDefaults, _Lexer.CurrentLine, _Lexer.CurrentColumn);

                    stmt.DefaultCase = gr_stmt_list();
                }
                else
                {
                    Match(TokenType.Case);
                    Expression constexpr = gr_const_expr();
                    Match(TokenType.Colon);
                    stmt.Cases.Add(new CaseLine(constexpr, gr_stmt_list()));
                }
            }
            Match(TokenType.CloseCurlyBracket);

            return stmt;
        }

        Statement gr_expr_stmt()
        {
            if(Lookahead(TokenType.Semicolon))//Empty expression
            {
                Match(TokenType.Semicolon);
                return new ExpressionStatement(new Expression());
            }
            else
            {
                return new ExpressionStatement(gr_expr());
            }
        }

        // Expressions
        Expression gr_const_expr()//Alias
        {
            return gr_cond_expr();
        }

        List<Expression> gr_expr_list()
        {
            var list = new List<Expression>();

            while(true)
            {
                list.Add(gr_expr());

                if (Lookahead(TokenType.Colon))
                    Match(TokenType.Colon);
                else
                    break;
            };

            return list;
        }

        Expression gr_expr()
        {
            return gr_ass_expr();
        }

        DeclarationExpression gr_decl_expr()
        {
            bool isConst = false;
            if (Lookahead(TokenType.Const))
            {
                Match(TokenType.Const);
                isConst = true;
            }
            Match(TokenType.Var);
            Token name = Match(TokenType.String);
            Match(TokenType.Assign);
            return new DeclarationExpression(name.Value, gr_expr(), isConst);
        }

        Expression gr_ass_expr()
        {
            Expression first = gr_cond_expr();

            TokenType t = Lookahead_AssOp();

            if (t != TokenType.EOF)
            {
                Match(t);
                if (first.Type == ExpressionType.Condition)
                    throw new Error(ErrorType.Parser_ConditionAssignmentStatementMixed, _Lexer.CurrentLine, _Lexer.CurrentColumn);
                else
                {
                    Expression second = gr_cond_expr();
                    switch (t)
                    {
                        case TokenType.Assign:
                            return new Expression(ExpressionType.Assign, new Expression[] { first, second });
                        case TokenType.LeftShiftAssign:
                            return new Expression(ExpressionType.LeftShiftAssign, new Expression[] { first, second });
                        case TokenType.RightShiftAssign:
                            return new Expression(ExpressionType.RightShiftAssign, new Expression[] { first, second });
                        case TokenType.PlusAssign:
                            return new Expression(ExpressionType.PlusAssign, new Expression[] { first, second });
                        case TokenType.MinusAssign:
                            return new Expression(ExpressionType.MinusAssign, new Expression[] { first, second });
                        case TokenType.MultiplyAssign:
                            return new Expression(ExpressionType.MultiplyAssign, new Expression[] { first, second });
                        case TokenType.PowAssign:
                            return new Expression(ExpressionType.PowAssign, new Expression[] { first, second });
                        case TokenType.DivideAssign:
                            return new Expression(ExpressionType.DivideAssign, new Expression[] { first, second });
                        case TokenType.ModuloAssign:
                            return new Expression(ExpressionType.ModuloAssign, new Expression[] { first, second });
                        case TokenType.Bin_AndAssign:
                            return new Expression(ExpressionType.Bin_AndAssign, new Expression[] { first, second });
                        case TokenType.Bin_OrAssign:
                            return new Expression(ExpressionType.Bin_OrAssign, new Expression[] { first, second });
                        case TokenType.Bin_XorAssign:
                            return new Expression(ExpressionType.Bin_XorAssign, new Expression[] { first, second });
                        case TokenType.Bin_NotAssign:
                            return new Expression(ExpressionType.Bin_NotAssign, new Expression[] { first, second });
                    }
                }
                return null;//FATAL
            }
            else
                return first;
        }

        Expression gr_cond_expr()
        {
            Expression first = gr_log_or_expr();
            if (Lookahead(TokenType.QuestionMark))
            {
                Match(TokenType.QuestionMark);
                Expression second = gr_expr();
                Match(TokenType.Colon);
                Expression third = gr_ass_expr();
                return new Expression(ExpressionType.Condition, new Expression[] { first, second, third });
            }
            else
                return first;
        }
        
        Expression gr_log_or_expr()
        {
            //TODO
            return null;
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

        bool Lookahead(TokenType type, int i = 1)
        {
            Token token = _Lexer.Look(i);
            return token.Type == type;
        }

        TokenType Lookahead_AssOp(int i = 1)
        {
            if (Lookahead(TokenType.Assign, i))
                return TokenType.Assign;
            else if (Lookahead(TokenType.LeftShiftAssign, i))
                return TokenType.LeftShiftAssign;
            else if (Lookahead(TokenType.RightShiftAssign, i))
                return TokenType.RightShiftAssign;
            else if (Lookahead(TokenType.PlusAssign, i))
                return TokenType.PlusAssign;
            else if (Lookahead(TokenType.MinusAssign, i))
                return TokenType.MinusAssign;
            else if (Lookahead(TokenType.MultiplyAssign, i))
                return TokenType.MultiplyAssign;
            else if (Lookahead(TokenType.PowAssign, i))
                return TokenType.PowAssign;
            else if (Lookahead(TokenType.DivideAssign, i))
                return TokenType.DivideAssign;
            else if (Lookahead(TokenType.ModuloAssign, i))
                return TokenType.ModuloAssign;
            else if (Lookahead(TokenType.Bin_AndAssign, i))
                return TokenType.Bin_AndAssign;
            else if (Lookahead(TokenType.Bin_OrAssign, i))
                return TokenType.Bin_OrAssign;
            else if (Lookahead(TokenType.Bin_XorAssign, i))
                return TokenType.Bin_XorAssign;
            else if (Lookahead(TokenType.Bin_NotAssign, i))
                return TokenType.Bin_NotAssign;
            else
                return TokenType.EOF;
        }

        bool IsEOF()
        {
            return _Lexer.Look().Type == TokenType.EOF;
        }      
    }
}
