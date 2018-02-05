using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;

namespace ITI.NoMonkeyTree
{
    public static class Game
    {
        /// <summary>
        /// ( 3 + 5 ) * 3 / 4
        /// </summary>
        public static BinaryExpression AstSimpleOperator()
        {
            var a = Expression.Add(Expression.Constant(3), Expression.Constant(5));
            var b = Expression.Multiply(Expression.Constant(3), a);
            return Expression.Divide(b, Expression.Constant(4));
        }

        /// <summary>
        /// "toto" + "tata"
        /// </summary>
        public static BinaryExpression AstStringOperator()
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)});
            return Expression.Add(Expression.Constant("toto"), Expression.Constant("tata"), addStringMethod);
        }

        public static BinaryExpression AstStringAndDateTime()
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)});
            return Expression.Add(Expression.Constant("toto"),
                Expression.Constant((DateTime.UtcNow.Millisecond & 1) == 1 ? "You" : "Me"), addStringMethod);
        }

        public static Expression AstFunc()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x * y;
            return expression;
        }

        public static Expression CalculSimpleReversePolishNotation()
        {
            Expression<Func<Queue, int>> expression = (reversePolishNotation) => TestCalcul(reversePolishNotation);
            return expression;
        }

        public static int TestCalcul(Queue reversePolishNotation)
        {
            Console.WriteLine(reversePolishNotation);
            return reversePolishNotation.Count;
        }

        /*
        public Expression AstAddition(int a, int b)
        {
            return Expression.Add(Expression.Constant(a), Expression.Constant(b));
        }

        public void CreateCharacter(string Name, int hp)
        {

        }*/

        /// <summary>
        ///( 3 + 5 ) * 3 / 4 =>  ( ( ( (3) + (5) ) * (3) ) / (4) )
        /// </summary>
        public class VisitorParenthesis1 : ExpressionVisitor
        {
            readonly StringBuilder _sb;
            public VisitorParenthesis1()
            {
                _sb = new StringBuilder();
            }

            public string GetResult() => _sb.ToString();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                _sb.Append("( ");
                var leftNode = this.Visit(node.Left);


                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        _sb.Append(" + ");
                        break;
                    case ExpressionType.Subtract:
                        _sb.Append(" - ");
                        break;
                    case ExpressionType.Multiply:
                        _sb.Append(" * ");
                        break;
                    case ExpressionType.Divide:
                        _sb.Append(" / ");
                        break;
                    case ExpressionType.Modulo:
                        _sb.Append(" % ");
                        break;
                    default:
                        _sb.Append(" ? ");
                        break;
                }


            var rightNode = this.Visit(node.Right);

                _sb.Append(" )");
                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                _sb.Append("(" + node.Value + ")");
                return node;
            }
        }

        /// <summary>
        /// ( 3 + 5 ) * 3 / 4 =>  ( ( ( 3 + 5 ) * 3 ) / 4 )
        /// </summary>
        public class VisitorParenthesis2 : ExpressionVisitor
        {
            readonly StringBuilder _sb;

            public VisitorParenthesis2()
            {
                _sb = new StringBuilder();
            }

            public string GetResult() => _sb.ToString();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                _sb.Append("( ");
                var leftNode = this.Visit(node.Left);


                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        _sb.Append(" + ");
                        break;
                    case ExpressionType.Subtract:
                        _sb.Append(" - ");
                        break;
                    case ExpressionType.Multiply:
                        _sb.Append(" * ");
                        break;
                    case ExpressionType.Divide:
                        _sb.Append(" / ");
                        break;
                    case ExpressionType.Modulo:
                        _sb.Append(" % ");
                        break;
                    default:
                        _sb.Append(" ? ");
                        break;
                }


                var rightNode = this.Visit(node.Right);

                _sb.Append(" )");
                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                _sb.Append(node.Value);
                return node;
            }
        }


        /// <summary>
        /// ( 3 + 5 ) * 3 / 4 => ( 3 + 5 ) * 3 / 4
        /// </summary>
        public class VisitorParenthesis3 : ExpressionVisitor
        {
            readonly StringBuilder _sb;

            public VisitorParenthesis3()
            {
                _sb = new StringBuilder();
            }

            public string GetResult() => _sb.ToString();

            protected override Expression VisitBinary(BinaryExpression node)
            {
                bool priorOp;
                //TODO: Remove parenthesis from multiple non prior operations
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        priorOp = false;
                        break;
                    case ExpressionType.Subtract:
                        priorOp = false;
                        break;
                    case ExpressionType.Multiply:
                        priorOp = true;
                        break;
                    case ExpressionType.Divide:
                        priorOp = true;
                        break;
                    case ExpressionType.Modulo:
                        priorOp = true;
                        break;
                    default:
                        priorOp = false;
                        break;
                }

                if (!priorOp)
                    _sb.Append("( ");

                var leftNode = this.Visit(node.Left);


                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        _sb.Append(" + ");
                        break;
                    case ExpressionType.Subtract:
                        _sb.Append(" - ");
                        break;
                    case ExpressionType.Multiply:
                        _sb.Append(" * ");
                        break;
                    case ExpressionType.Divide:
                        _sb.Append(" / ");
                        break;
                    case ExpressionType.Modulo:
                        _sb.Append(" % ");
                        break;
                    default:
                        _sb.Append(" ? ");
                        break;
                }


                var rightNode = this.Visit(node.Right);

                if (!priorOp)
                    _sb.Append(" )");

                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                _sb.Append(node.Value);
                return node;
            }
        }
    }
}