using System.Linq.Expressions;
using System.Text;

namespace ITI.NoMonkeyTree
{

    /// <summary>
    /// ( 3 + 5 ) * 3 / 4 =>  ( ( ( 3 + 5 ) * 3 ) / 4 )
    /// </summary>
    public class VisitorParenthesis2 : ExpressionVisitor
    {
        public VisitorParenthesis2()
        {
            throw new System.NotImplementedException();
        }

        public string GetResult() => throw new System.NotImplementedException();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            throw new System.NotImplementedException();
        }
    }

}
