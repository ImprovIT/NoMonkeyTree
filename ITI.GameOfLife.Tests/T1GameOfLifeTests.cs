using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace ITI.GameOfLife.Tests
{

    class MyExpressionVisitor : ExpressionVisitor
    {

        protected override Expression VisitBinary( BinaryExpression node )
        {
            Console.WriteLine( "Visit Binary" );
            return node;
        }
    }


    [TestFixture]
    public class T1GameOfLifeTests
    {
        [Test]
        public void ast_simple_operator_should_works()
        {
            BinaryExpression ast = Game.AstSimpleOperator();


            ast.NodeType.Should().Be(ExpressionType.Divide);

            BinaryExpression astMultiply;
            ConstantExpression const1;

            if (ast.Left.NodeType == ExpressionType.Multiply)
            {
                astMultiply = (BinaryExpression) ast.Left;
                const1 = (ConstantExpression) ast.Right;
            }
            else
            {
                astMultiply = (BinaryExpression) ast.Right;
                const1 = (ConstantExpression) ast.Left;
            }

            astMultiply.NodeType.Should().Be(ExpressionType.Multiply);
            //const1.NodeType.Should().Be( ExpressionType.Constant );
            const1.Value.Should().Be(4);


            BinaryExpression astAddition;
            ConstantExpression const2;

            if (astMultiply.Left.NodeType == ExpressionType.Add)
            {
                astAddition = (BinaryExpression) astMultiply.Left;
                const2 = (ConstantExpression) astMultiply.Right;
            }
            else
            {
                astAddition = (BinaryExpression) astMultiply.Right;
                const2 = (ConstantExpression) astMultiply.Left;
            }

            astAddition.NodeType.Should().Be(ExpressionType.Add);
            //const2.NodeType.Should().Be( ExpressionType.Constant );


            const2.Value.Should().Be(3);


            ConstantExpression const3 = (ConstantExpression) astAddition.Left;
            ConstantExpression const4 = (ConstantExpression) astAddition.Right;
            const3.NodeType.Should().Be(ExpressionType.Constant);
            const4.NodeType.Should().Be(ExpressionType.Constant);

            if ((int) const3.Value == 3)
            {
                const4.Value.Should().Be(5);
            }
            else
            {
                const4.Value.Should().Be(3);
                const3.Value.Should().Be(5);
            }



            var result = Expression.Lambda<Func<int>>(ast).Compile()();

            result.Should().Be(6);


        }

        [Test]
        public void ast_play_with_string()
        {
            BinaryExpression stringExpr = Game.AstStringOperator();

            ((ConstantExpression) stringExpr.Left).Value.Should().Be("toto");
            ((ConstantExpression) stringExpr.Right).Value.Should().Be("tata");

            Expression.Lambda<Func<string>>(Game.AstStringOperator()).Compile()().Should().Be("tototata");
        }

        [Test]
        public void ast_string_and_datetime()
        {
            Console.WriteLine(Expression.Lambda<Func<string>>(Game.AstStringAndDateTime()).Compile()());
        }

        [Test]
        public void ast_usage_of_func()
        {
            Expression<Func<int, int, int>> expr = (Expression<Func<int, int, int>>) Game.AstFunc();

            expr.NodeType.Should().Be(ExpressionType.Lambda);

            expr.Parameters[0].NodeType.Should().Be(ExpressionType.Parameter);
            expr.Parameters[1].NodeType.Should().Be(ExpressionType.Parameter);

            expr.Compile()(3, 5).Should().Be(15);
            expr.Compile()(7, 5).Should().Be(35);
            expr.Compile()(9, 9).Should().Be(81);
            expr.Compile()(10, 5).Should().Be(50);
        }

        [Test]
        public void ast_reverse_polish_notation_principle()
        {
            Expression expr = createE1(new Random());
            
            Console.WriteLine(Expression.Lambda<Func<int>>(expr).Compile()());


            //Expression<Func<Expression, string>> expression = (Expression<Func<Expression, string>>)Game.ReversePolishNotationPrinciple();
            //expression.NodeType.Should().Be(ExpressionType.Lambda);

            //expression.Parameters[0].NodeType.Should().Be(ExpressionType.Parameter);

            //var a = Expression.Add(Expression.Constant(3), Expression.Constant(5));
            //var b = Expression.Multiply(Expression.Constant(3), a);

            //Console.WriteLine(expression.Compile()(b));

        }

        public Expression createE1(Random r)
        {
            int const1 = new Random().Next(Int32.MinValue, Int32.MaxValue);
            int const2 = new Random().Next(Int32.MinValue, Int32.MaxValue);
            int const3 = new Random().Next(Int32.MinValue, Int32.MaxValue);

            var a = Expression.Add(Expression.Constant(const2), Expression.Constant(const3));
            return Expression.Add(Expression.Constant(const1), a);

        }

        [Test]
        public void ast_reverse_polish_notation_simple_calcul()
        {
            Queue reversePolishNotation = new Queue();
            reversePolishNotation.Enqueue(1);
            reversePolishNotation.Enqueue(2);
            reversePolishNotation.Enqueue(3);
            reversePolishNotation.Enqueue("+");
            reversePolishNotation.Enqueue("*");

            Expression<Func<Queue, int>> expression =
                (Expression<Func<Queue, int>>) Game.CalculSimpleReversePolishNotation();
            Console.WriteLine(expression.Compile()(reversePolishNotation));
        }

        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation1()
        {
            // ( 3 + 5 ) * 3 / 4 =>  ( ( ( (3) + (5) ) * (3) ) / (4) )

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add( Expression.Constant( 3 ),
                    Expression.Constant( 5 ) ),
                    Expression.Constant( 3 ) ),
                Expression.Constant( 4 ) );
            var visitor = new Game.VisitorParenthesis1();
            visitor.Visit( expr );
            var result = visitor.GetResult();

            result.Should().Be( "( ( ( (3) + (5) ) * (3) ) / (4) )" );
            Console.WriteLine( result );
        }


        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation2()
        {
            // ( 3 + 5 ) * 3 / 4 =>  ( ( ( 3 + 5 ) * 3 ) / 4 )

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add( Expression.Constant( 3 ),
                    Expression.Constant( 5 ) ),
                    Expression.Constant( 3 ) ),
                Expression.Constant( 4 ) );
            var visitor = new Game.VisitorParenthesis2();
            visitor.Visit( expr );
            var result = visitor.GetResult();

            result.Should().Be( "( ( ( 3 + 5 ) * 3 ) / 4 )" );
            Console.WriteLine( result );
        }

        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation3()
        {
            // ( 3 + 5 ) * 3 / 4 => ( 3 + 5 ) * 3 ) / 4

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add( Expression.Constant( 3 ),
                    Expression.Constant( 5 ) ),
                    Expression.Constant( 3 ) ),
                Expression.Constant( 4 ) );
            var visitor = new Game.VisitorParenthesis3();
            visitor.Visit( expr );
            var result = visitor.GetResult();

            result.Should().Be( "( 3 + 5 ) * 3 / 4" );
            Console.WriteLine( result );
        }

    }
}
