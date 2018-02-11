using System.Linq.Expressions;
using System.Text;

namespace ITI.NoMonkeyTree
{
    public class VisitorReversePolishNotation : ExpressionVisitor
    {
        public VisitorReversePolishNotation()
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
