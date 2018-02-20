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
        /// <summary>
        /// Create an expression tree that respect the syntax : ( 3 + 5 ) * 3 / 4
        /// The expression have to chain addition, multiplication and division.
        /// </summary>
        [Test]
        public void ast_create_simple_expression_with_basic_operators_and_return_expression_that_do_the_calcul()

        {
            BinaryExpression ast = NoMonkeyTree.AstSimpleOperator();

            ast.NodeType.Should().Be(ExpressionType.Divide);


            BinaryExpression astMultiply = (BinaryExpression)ast.Left;
            ConstantExpression const1 = (ConstantExpression)ast.Right;

            astMultiply.NodeType.Should().Be(ExpressionType.Multiply);
            const1.Value.Should().Be(4);


            BinaryExpression astAddition = (BinaryExpression)astMultiply.Left;
            ConstantExpression const2 = (ConstantExpression)astMultiply.Right;

            astAddition.NodeType.Should().Be(ExpressionType.Add);
            const2.Value.Should().Be(3);


            ConstantExpression const3 = (ConstantExpression)astAddition.Left;
            ConstantExpression const4 = (ConstantExpression)astAddition.Right;

            const4.Value.Should().Be(3);
            const3.Value.Should().Be(5);


            ast.ToString().Should().Be("(3+5)*3/4");


            var result = Expression.Lambda<Func<int>>(ast).Compile()();
            result.Should().Be(6);


        }

        [Test]
        public void ast_play_with_string_should_return_expression_concat_of_two_strings()
        {
            BinaryExpression stringExpr = NoMonkeyTree.AstStringOperator();

            ((ConstantExpression)stringExpr.Left).Value.Should().Be("toto");
            ((ConstantExpression)stringExpr.Right).Value.Should().Be("tata");

            Expression.Lambda<Func<string>>(stringExpr).Compile()().Should().Be("tototata");
        }

        /// <summary>
        /// Check parity of date and display totoYou or totoMe.
        /// </summary>
        [Test]
        public void ast_string_and_datetime_loop_should_return_toto_and_Me_for_even_date_else_You()
        {
            for (int i = 0; i < 500; i++)
            {
                var date = DateTime.UtcNow.Millisecond;
                var expression = NoMonkeyTree.AstStringAndDateTime(date);
                var shouldResult = ("toto" + ((date & 1) == 1 ? "You" : "Me"));

                expression.Left.NodeType.Should().Be(ExpressionType.Constant);
                expression.Right.NodeType.Should().Be(ExpressionType.Constant);

                var result = Expression.Lambda<Func<string>>(expression).Compile()();
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
        public void ast_return_expression_that_call_our_own_method_substract()
        {
            Random rand = new Random();
            int c1 = rand.Next(Int32.MinValue, Int32.MaxValue);
            int c2 = rand.Next(Int32.MinValue, Int32.MaxValue);
            Expression expr = NoMonkeyTree.AstCallCustomFunctionSubstract(c1, c2);
            var func = Expression.Lambda<Func<int>>(expr).Compile();
            func.Invoke().Should().Be(c1 - c2);
        }

        /// <summary>
        /// Check doc for more information
        /// </summary>
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

        /// <summary>
        /// GetResult should return  “( ( ( (3) + (5) ) * (3) ) / (4) )”.
        /// It's the representation of an expression tree with full parenthesis.
        /// Use the visitor pattern on the given expression to create the string.
        /// There's no priority in operator.Even const are in parenthesis.
        /// </summary>
        [Test]
        public void ast_should_display_non_optimised_at_all_explicit_to_string_representation_of_the_whole_expression_tree_from_given_expr()
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

        /// <summary>
        /// The same as the precedent TU but with little optimisation.
        /// Result should be “( ( ( 3 + 5 ) * 3 ) / 4 )”. There's no parenthesis between a const.
        /// </summary>
        [Test]
        public void ast_should_display_non_optimised_but_smarter_explicit_to_string_representation_of_the_whole_expression_tree_from_given_expr()
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



        /// <summary>
        /// Use an Expression Block to loop between a start to end value.
        /// When you loop you should count even occurences.
        /// Should return an Expression Block that display the number of even occurences when looping.
        /// </summary>
        [Test]
        public void ast_loop_with_block_expression_return_block_expression_that_calcul_even_occurence_between_start_value_and_end_value()
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
