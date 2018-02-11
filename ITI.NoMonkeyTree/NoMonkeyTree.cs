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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// "toto" + "tata"
        /// </summary>
        public static BinaryExpression AstStringOperator()
        {
            throw new System.NotImplementedException();
        }

        public static BinaryExpression AstStringAndDateTime(int date)
        {
            throw new System.NotImplementedException();
        }

        public static Expression AstFuncMultilplication()
        {
            throw new System.NotImplementedException();
        }

        public static Expression AstCallCustomFunctionSubstract(int c1, int c2)
        {
            throw new System.NotImplementedException();
        }

        public static int Substract(int c1, int c2) => throw new System.NotImplementedException();

        public static Expression AstLoopWithBlock(int val1, ParameterExpression startValue, ParameterExpression endValue)
        {
            throw new System.NotImplementedException();
        }

        internal static bool CheckEven(ref int val)
        {
            throw new System.NotImplementedException();
        }

    }
}