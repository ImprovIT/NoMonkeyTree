using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITI.NoMonkeyTree
{
    public class VisitorReversePolishNotation : ExpressionVisitor
    {
        private readonly StringBuilder _sb;

        public VisitorReversePolishNotation()
        {
            _sb = new StringBuilder();
        }

        public string GetResult() => _sb.ToString();

        protected override Expression VisitBinary(BinaryExpression node)
        {

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    Visit(node.Left);
                    Visit(node.Right);
                    _sb.Append("+ ");
                    break;
                case ExpressionType.Subtract:
                    Visit(node.Left);
                    Visit(node.Right);
                    _sb.Append("- ");
                    break;
                case ExpressionType.Multiply:
                    Visit(node.Left);
                    Visit(node.Right);
                    _sb.Append("* ");
                    break;
                case ExpressionType.Divide:
                    Visit(node.Left);
                    Visit(node.Right);
                    _sb.Append("/ ");
                    break;
                case ExpressionType.Modulo:
                    Visit(node.Left);
                    Visit(node.Right);
                    _sb.Append("% ");
                    break;
                default:
                    _sb.Append("? ");
                    break;
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _sb.Append(node.Value + " ");
            return node;
        }
    }
}
