using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ITI.NoMonkeyTree.Tests
{

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
                astMultiply = (BinaryExpression)ast.Left;
                const1 = (ConstantExpression)ast.Right;
            }
            else
            {
                astMultiply = (BinaryExpression)ast.Right;
                const1 = (ConstantExpression)ast.Left;
            }

            astMultiply.NodeType.Should().Be(ExpressionType.Multiply);
            //const1.NodeType.Should().Be( ExpressionType.Constant );
            const1.Value.Should().Be(4);


            BinaryExpression astAddition;
            ConstantExpression const2;

            if (astMultiply.Left.NodeType == ExpressionType.Add)
            {
                astAddition = (BinaryExpression)astMultiply.Left;
                const2 = (ConstantExpression)astMultiply.Right;
            }
            else
            {
                astAddition = (BinaryExpression)astMultiply.Right;
                const2 = (ConstantExpression)astMultiply.Left;
            }

            astAddition.NodeType.Should().Be(ExpressionType.Add);
            //const2.NodeType.Should().Be( ExpressionType.Constant );


            const2.Value.Should().Be(3);


            ConstantExpression const3 = (ConstantExpression)astAddition.Left;
            ConstantExpression const4 = (ConstantExpression)astAddition.Right;
            const3.NodeType.Should().Be(ExpressionType.Constant);
            const4.NodeType.Should().Be(ExpressionType.Constant);

            if ((int)const3.Value == 3)
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
        public void ast_play_with_string_should_return_concat_of_two_strings()
        {
            BinaryExpression stringExpr = NoMonkeyTree.AstStringOperator();

            ((ConstantExpression)stringExpr.Left).Value.Should().Be("toto");
            ((ConstantExpression)stringExpr.Right).Value.Should().Be("tata");

            Expression.Lambda<Func<string>>(stringExpr).Compile()().Should().Be("tototata");
        }

        [Test]
        public void ast_string_and_datetime()
        {
            // It may fail even if implementation is correct. I have no better idea. Low CPU can't run it properly.

            BinaryExpression expression;


            for (int i = 0; i < 100; i++)
            {
                expression = NoMonkeyTree.AstStringAndDateTime();
                var shouldResult = ("toto" + ((DateTime.UtcNow.Millisecond & 1) == 1 ? "You" : "Me"));
                expression.Left.NodeType.Should().Be(ExpressionType.Constant);
                expression.Right.NodeType.Should().Be(ExpressionType.Constant);
                var result = Expression.Lambda<Func<string>>(expression).Compile()();
                Console.WriteLine(result);
                result.Should().Be(shouldResult);
            }
        }

        [Test]
        public void ast_usage_of_func_should_return_result_multiplication_of_two_numbers()
        {
            Expression<Func<int, int, int>> expr = (Expression<Func<int, int, int>>)NoMonkeyTree.AstFuncMultilplication();

            expr.NodeType.Should().Be(ExpressionType.Lambda);

            expr.Parameters[0].NodeType.Should().Be(ExpressionType.Parameter);
            expr.Parameters[1].NodeType.Should().Be(ExpressionType.Parameter);

            expr.Compile()(3, 5).Should().Be(15);
            expr.Compile()(7, 5).Should().Be(35);
            expr.Compile()(9, 9).Should().Be(81);
            expr.Compile()(10, 5).Should().Be(50);
        }

        [Test]
        public void ast_expression_that_call_our_own_method_substract()
        {
            Random rand = new Random();
            int c1 = rand.Next(Int32.MinValue, Int32.MaxValue);
            int c2 = rand.Next(Int32.MinValue, Int32.MaxValue);
            Expression expr = NoMonkeyTree.AstCallCustomFunction(c1, c2);
            var func = Expression.Lambda<Func<int>>(expr).Compile();
            func.Invoke().Should().Be(c1 - c2);
        }

        [Test]
        public void ast_reverse_polish_notation_principle()
        {
            List<int> constValue = new List<int>();
            Expression expr = CreateE1(new Random(), constValue);

            var visitor = new VisitorReversePolishNotation();
            visitor.Visit(expr);
            string result = visitor.GetResult().Trim();

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
        public void ast_full_explicit_string_representation_of_simple_operation1()
        {
            // ( 3 + 5 ) * 3 / 4 =>  ( ( ( (3) + (5) ) * (3) ) / (4) )

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add(Expression.Constant(3),
                    Expression.Constant(5)),
                    Expression.Constant(3)),
                Expression.Constant(4));
            var visitor = new VisitorParenthesis1();
            visitor.Visit(expr);
            var result = visitor.GetResult();

            result.Should().Be("( ( ( (3) + (5) ) * (3) ) / (4) )");
        }


        [Test]
        public void ast_full_explicit_string_representation_of_simple_operation2()
        {
            // ( 3 + 5 ) * 3 / 4 =>  ( ( ( 3 + 5 ) * 3 ) / 4 )

            var expr = Expression.Divide(
                Expression.Multiply(
                    Expression.Add(Expression.Constant(3),
                    Expression.Constant(5)),
                    Expression.Constant(3)),
                Expression.Constant(4));
            var visitor = new VisitorParenthesis2();
            visitor.Visit(expr);
            var result = visitor.GetResult();

            result.Should().Be("( ( ( 3 + 5 ) * 3 ) / 4 )");
        }

     

        [Test]
        public void ast_loop_with_block_expression_return_even_occurence_between_start_value_and_end_value()
        {

            int val1 = new Random().Next(0, 1000);
            int val2;
            do
            {
                val2 = new Random().Next(0, 5000);
            } while (val1 > val2);
 
            // Creating a parameter expression.
            ParameterExpression startValue = Expression.Parameter(typeof(int), "startValue");
            ParameterExpression endValue = Expression.Parameter(typeof(int), "endValue");

            // Compile and run an expression tree.
            BlockExpression block = (BlockExpression)NoMonkeyTree.AstLoopWithBlock(val1, startValue, endValue);

            ExpressionType.Loop.Should().Be(block.Result.NodeType);

            int result = Expression.Lambda<Func<int, int, int>>(block, startValue, endValue).Compile()(val1, val2);

            int evenOccurence = 0;

            for (int i = ++val1; i <= val2; i++)
            {
                if ((i & 1) != 1)
                {
                    evenOccurence++;
                }
            }

            evenOccurence.Should().Be(result);

        }

    }
}
