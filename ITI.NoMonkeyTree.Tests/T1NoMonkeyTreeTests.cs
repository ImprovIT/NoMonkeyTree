using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ITI.NoMonkeyTree.Tests
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
    public class T1NoMonkeyTreeTests
    {
        [Test]
        public void ast_simple_operator_should_works()
        {
            BinaryExpression ast = NoMonkeyTree.AstSimpleOperator();


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
            BinaryExpression stringExpr = NoMonkeyTree.AstStringOperator();

            ((ConstantExpression) stringExpr.Left).Value.Should().Be("toto");
            ((ConstantExpression) stringExpr.Right).Value.Should().Be("tata");

            Expression.Lambda<Func<string>>(NoMonkeyTree.AstStringOperator()).Compile()().Should().Be("tototata");
        }

        [Test]
        public void ast_string_and_datetime()
        {
            Console.WriteLine(Expression.Lambda<Func<string>>(NoMonkeyTree.AstStringAndDateTime()).Compile()());
        }

        [Test]
        public void ast_usage_of_func()
        {
            Expression<Func<int, int, int>> expr = (Expression<Func<int, int, int>>) NoMonkeyTree.AstFunc();

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
            List<int> constValue = new List<int>();
            Expression expr = CreateE1(new Random(), constValue);

            var visitor = new VisitorReversePolishNotation();
            visitor.Visit(expr);
            string result = visitor.GetResult().Trim();

            //Console.WriteLine(result);
            //Console.WriteLine("C1 C2 C3 + +"
            //    .Replace("C1", constValue.ElementAt(0).ToString())
            //    .Replace("C2", constValue.ElementAt(1).ToString())
            //    .Replace("C3", constValue.ElementAt(2).ToString())
            //);

            result.Should().Be("C1 C2 C3 + +"
                .Replace("C1", constValue.ElementAt(0).ToString())
                .Replace("C2", constValue.ElementAt(1).ToString())
                .Replace("C3", constValue.ElementAt(2).ToString())
            );

            constValue.Clear();

            Expression expr2 = CreateE2(new Random(), constValue);

            var visitor2 = new VisitorReversePolishNotation();
            visitor2.Visit(expr2);
            string result2 = visitor2.GetResult().Trim();

            result2.Should().Be("C1 C2 C3 C4 C5 + + - *"
                .Replace("C1", constValue.ElementAt(0).ToString())
                .Replace("C2", constValue.ElementAt(1).ToString())
                .Replace("C3", constValue.ElementAt(2).ToString())
                .Replace("C4", constValue.ElementAt(3).ToString())
                .Replace("C5", constValue.ElementAt(4).ToString())
            );
        }

        public Expression CreateE1(Random r, List<int> constValue)
        {
            int const1 = r.Next(Int32.MinValue, Int32.MaxValue);
            int const2 = r.Next(Int32.MinValue, Int32.MaxValue);
            int const3 = r.Next(Int32.MinValue, Int32.MaxValue);

            constValue.Add(const1);
            constValue.Add(const2);
            constValue.Add(const3);

            var a = Expression.Add(Expression.Constant(const2), Expression.Constant(const3));
            return Expression.Add(Expression.Constant(const1), a);

        }

        public Expression CreateE2(Random r, List<int> constValue)
        {
            int const1 = r.Next(Int32.MinValue, Int32.MaxValue);
            int const2 = r.Next(Int32.MinValue, Int32.MaxValue);

            constValue.Add(const1);
            constValue.Add(const2);

            Expression createE1 = CreateE1(new Random(), constValue);

            var a = Expression.Subtract(Expression.Constant(const2), createE1);
            return Expression.Multiply(Expression.Constant(const1), a);

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
                (Expression<Func<Queue, int>>) NoMonkeyTree.CalculSimpleReversePolishNotation();
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
            var visitor = new VisitorParenthesis1();
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
            var visitor = new VisitorParenthesis2();
            visitor.Visit( expr );
            var result = visitor.GetResult();

            result.Should().Be( "( ( ( 3 + 5 ) * 3 ) / 4 )" );
            Console.WriteLine( result );
        }

        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation3_t1()
        {
            // ( 3 + 5 ) * 3 / 4 => ( 3 + 5 ) * 3 ) / 4

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add(Expression.Constant(3),
                    Expression.Constant(5)),
                    Expression.Constant(3)),
                Expression.Constant(4));



            var visitor = new VisitorParenthesis3();
            visitor.Visit( expr );
            var result = visitor.GetResult();

            result.Should().Be( "( 3 + 5 ) * 3 / 4" );
            Console.WriteLine( result );
        }

        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation3_t2()
        {
            // ( 3 + 5 * ( 4 + 7 ) )



            var expr = Expression.Add(Expression.Constant(3),
                Expression.Multiply(Expression.Constant(5),
                Expression.Add(Expression.Constant(4), Expression.Constant(7))
                ));

            var visitor = new VisitorParenthesis3();
            visitor.Visit(expr);
            var result = visitor.GetResult();

            Console.WriteLine(result);
            result.Should().Be("3 + 5 * ( 4 + 7 )");
            
        }

        public static bool CheckEven(ref int val)
        {
            val++;
            Console.WriteLine("call check : " + val);
            return (val & 1) != 1;
        }

        [Test]
        public void ast_loop_with_block_expression()
        {
            int val1 = new Random().Next(0, 10);
            Console.WriteLine("val1 : " + val1);
            int val2 = new Random().Next(11, 20);
            Console.WriteLine("val2 : " + val2);

            Expression<Func<bool>> evenExpr = () => CheckEven(ref val1);

            // Creating a parameter expression.
            ParameterExpression startValue = Expression.Parameter(typeof(int), "startValue");
            ParameterExpression endValue = Expression.Parameter(typeof(int), "endValue");

            // Creating an expression to hold a local variable. 
            ParameterExpression evenResult = Expression.Parameter(typeof(int), "evenResult");

            // Creating a label to jump to from a loop.
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.
            BlockExpression block = Expression.Block(
                new[] {evenResult},
                Expression.Assign(evenResult, Expression.Constant(0, typeof(int))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThanOrEqual(startValue, endValue),
                        Expression.Break(label, evenResult),
                        Expression.IfThenElse(
                            Expression.Invoke(evenExpr),
                            Expression.Block(
                                Expression.PostIncrementAssign(startValue),
                                //Expression.Call(
                                //    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),
                                //    startValue
                                //),
                                Expression.PostIncrementAssign(evenResult)
                            ),
                            Expression.PostIncrementAssign(startValue)
                        )                              
                    ),
                    label
                )
            );

            // Compile and run an expression tree.
            int result = Expression.Lambda<Func<int, int, int>>(block, startValue, endValue).Compile()(val1, val2);
            Console.WriteLine("val1 after increment : " + val1);
            Console.WriteLine(result);

        }

    }
}
