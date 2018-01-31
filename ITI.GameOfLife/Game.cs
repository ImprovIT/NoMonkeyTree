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
            return Expression.Divide(b, Expression.Constant(4));
        }

        /// <summary>
        /// "toto" + "tata"
        /// </summary>
        public static Expression AstStringOperator()
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            return Expression.Add(Expression.Constant("toto"), Expression.Constant("tata"), addStringMethod);
        }

        /*
        public Expression AstAddition(int a, int b)
        {
            return Expression.Add(Expression.Constant(a), Expression.Constant(b));
        }

        public void CreateCharacter(string Name, int hp)
        {

        }

        public Expression Damaged(int charHp, int hp)
        {
            return Expression.Subtract(Expression.Constant(charHp), Expression.Constant(hp));
        }

        public Expression Healed(int charHp, int hp)
        {
            return Expression.Add(Expression.Constant(charHp), Expression.Constant(hp));
        }

    }
    public class RpgExpressionVisitor : ExpressionVisitor
    {

        protected override Expression VisitBinary(BinaryExpression node)
        {

            StringBuilder sb = new StringBuilder();
            // base hp
            var leftNode = this.Visit(node.Left);

            // hp modificator
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    sb.Append(leftNode);
                    sb.Append(" healed ");
                    //Console.Write(" healed ");
                    break;
                case ExpressionType.Subtract:
                    sb.Append(leftNode);
                    sb.Append(" damaged ");
                    //Console.Write(leftNode.ToString());
                    //Console.Write(" damaged ");
                    break;

            }

            // quantity of hp affected
            var rightNode = this.Visit(node.Right);
            sb.Append(rightNode);
            //Console.Write(rightNode.ToString());
            //Console.WriteLine(" HP.");

            return Expression.Constant(sb);
        }
    */
    }
}
