using System;
using System.Linq.Expressions;

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

        public static BinaryExpression AstStringAndDateTime(int date)
        {
            var addStringMethod = typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)});
            return Expression.Add(Expression.Constant("toto"),
                Expression.Constant((date & 1) == 1 ? "You" : "Me"), addStringMethod);
        }

        public static Expression AstFuncMultilplication()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x * y;
            return expression;
        }

        public static Expression AstCallCustomFunction(int c1, int c2)
        {
            var exprC1 = Expression.Constant(c1);
            var exprC2 = Expression.Constant(c2);
            return Expression.Call(typeof(NoMonkeyTree).GetMethod("Substract"), exprC1, exprC2);
        }

        public static int Substract(int c1, int c2) => c1 - c2;

        public static Expression AstLoopWithBlock(int val1, ParameterExpression startValue, ParameterExpression endValue)
        {
            Expression<Func<bool>> evenExpr = () => CheckEven(ref val1);

            // Creating an expression to hold a local variable. 
            ParameterExpression evenResult = Expression.Parameter(typeof(int), "evenResult");

            // Creating a label to jump to from a loop.
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.
            BlockExpression block = Expression.Block(
                new[] { evenResult },
                Expression.Assign(evenResult, Expression.Constant(0, typeof(int))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThanOrEqual(startValue, endValue),
                        Expression.Break(label, evenResult),
                        Expression.IfThenElse(
                            Expression.Invoke(evenExpr),
                            Expression.Block(
                                Expression.PostIncrementAssign(startValue),
                                Expression.PostIncrementAssign(evenResult)
                            ),
                            Expression.PostIncrementAssign(startValue)
                        )
                    ),
                    label
                )
            );

            return block;
        }

        public static bool CheckEven(ref int val)
        {
            val++;
            return (val & 1) != 1;
        }

    }
}