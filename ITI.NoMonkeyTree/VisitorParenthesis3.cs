using System.Linq.Expressions;
using System.Text;

namespace ITI.NoMonkeyTree
{

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
