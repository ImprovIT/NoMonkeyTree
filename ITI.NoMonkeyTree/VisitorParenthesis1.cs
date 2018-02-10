using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITI.NoMonkeyTree
{

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

}
