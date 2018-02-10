using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;

namespace ITI.NoMonkeyTree
{
    public static class NoMonkeyTree
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
            // Visitor ? Need to know Expression.Constant(whatishere)
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


        public static Expression AstLoopWithBlock()
        {
            throw new System.NotImplementedException();
        }
    }
}