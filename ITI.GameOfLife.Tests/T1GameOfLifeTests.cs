﻿using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq.Expressions;
using System.Xml;

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
        public void init_ast()
        {
            var a = Expression.Constant(2);
            var b = Expression.Constant(3);
            var c = Expression.Constant(1);

            var sumExpr = Expression.Add(a, b);
            var sumExpr2 = Expression.Add(sumExpr, c);

            var visitor = new MyExpressionVisitor();
            //visitor.VisitBinary

            Console.WriteLine( visitor.Visit( sumExpr2 ) );


            //Console.WriteLine(sumExpr.ToString());
            var sumResult = Expression.Lambda<Func<int>>(sumExpr).Compile()();

            //Console.WriteLine(sumResult);
        }

        [Test]
        public void ast_addition()
        {
            /*Game game = new Game();
            int a = new Random().Next(Int32.MinValue, Int32.MaxValue);
            int b = new Random().Next(Int32.MinValue, Int32.MaxValue);

            Expression expression = game.AstAddition(a, b);
            var sumResult = Expression.Lambda<Func<int>>(expression).Compile()();

            sumResult.Should().Be(a + b);
        */
        }

        [Test]
        public void ast_rpg_story()
        {
            /*   Game game = new Game();

               var exp1 = game.Damaged(10, 5);
             //  var exp2 = game.Healed(9, 2);

               var res1 = Expression.Lambda<Func<int>>(exp1).Compile()();
           //    var res2 = Expression.Lambda<Func<int>>(exp2).Compile()();

               var visitor = new RpgExpressionVisitor();

               //Console.WriteLine(res1.ToString());

               Console.WriteLine("visitor : " + visitor.Visit(exp1));
               Console.WriteLine("exp1 : " + exp1);
               Console.WriteLine("res 1 : " + res1);

               visitor.Visit(exp1).ToString().Should().Be("10 damaged 5");
               res1.Should().Be(5);
               */
        }

        [Test]
        public void ast_simple_operator_should_works()
        {
            BinaryExpression ast = Game.AstSimpleOperator();


            ast.NodeType.Should().Be( ExpressionType.Divide );

            BinaryExpression astMultiply;
            ConstantExpression const1;

            if( ast.Left.NodeType == ExpressionType.Multiply )
            {
                astMultiply = (BinaryExpression)ast.Left;
                const1 = (ConstantExpression)ast.Right;
            }
            else
            {
                astMultiply = (BinaryExpression)ast.Right;
                const1 = (ConstantExpression)ast.Left;
            }

            astMultiply.NodeType.Should().Be( ExpressionType.Multiply );
            //const1.NodeType.Should().Be( ExpressionType.Constant );
            const1.Value.Should().Be( 4 );


            BinaryExpression astAddition;
            ConstantExpression const2;

            if( astMultiply.Left.NodeType == ExpressionType.Add )
            {
                astAddition = (BinaryExpression)astMultiply.Left;
                const2 = (ConstantExpression)astMultiply.Right;
            }
            else
            {
                astAddition = (BinaryExpression)astMultiply.Right;
                const2 = (ConstantExpression)astMultiply.Left;
            }

            astAddition.NodeType.Should().Be( ExpressionType.Add );
            //const2.NodeType.Should().Be( ExpressionType.Constant );


            const2.Value.Should().Be( 3 );


            ConstantExpression const3 = (ConstantExpression)astAddition.Left;
            ConstantExpression const4 = (ConstantExpression)astAddition.Right;
            const3.NodeType.Should().Be( ExpressionType.Constant );
            const4.NodeType.Should().Be( ExpressionType.Constant );

            if( (int)const3.Value == 3 )
            {
                const4.Value.Should().Be( 5 );
            }
            else
            {
                const4.Value.Should().Be( 3 );
                const3.Value.Should().Be( 5 );
            }
            

        }

    }
}