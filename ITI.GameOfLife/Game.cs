using System;
using System.Linq.Expressions;
using System.Text;

namespace ITI.GameOfLife
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
            return Expression.Divide( b, Expression.Constant( 4 ) );
        }

        /// <summary>
        /// "toto" + "tata"
        /// </summary>
        public static BinaryExpression AstStringOperator()
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            return Expression.Add( Expression.Constant( "toto" ), Expression.Constant( "tata" ), addStringMethod );
        }

        public static BinaryExpression AstStringAndDateTime()
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            return Expression.Add( Expression.Constant( "toto" ), Expression.Constant( (DateTime.UtcNow.Millisecond & 1) == 1 ? "You" : "Me" ), addStringMethod );
        }

        public static Expression AstFunc()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x * y;
            return expression;
        }

    }

    /// <summary>
    ///( 3 + 5 ) * 3 / 4 =>  ( ( ( (3) + (5) ) * (3) ) / (4) )
    /// </summary>
    public class VisitorParenthesis1 : ExpressionVisitor
    {
        StringBuilder _sb;

        public VisitorParenthesis1()
        {
            _sb = new StringBuilder();
        }

        public string GetResult() => _sb.ToString();

        protected override Expression VisitBinary( BinaryExpression node )
        {
            _sb.Append( "( " );
            var leftNode = this.Visit(node.Left);


            switch( node.NodeType )
            {
                case ExpressionType.Add:
                    _sb.Append( " + " );
                    break;
                case ExpressionType.Subtract:
                    _sb.Append( " - " );
                    break;
                case ExpressionType.Multiply:
                    _sb.Append( " * " );
                    break;
                case ExpressionType.Divide:
                    _sb.Append( " / " );
                    break;
            }


            var rightNode = this.Visit(node.Right);

            _sb.Append( " )" );
            return node;
        }

        protected override Expression VisitConstant( ConstantExpression node )
        {
            _sb.Append( "(" + node.Value + ")" );
            return node;
        }
    }


}