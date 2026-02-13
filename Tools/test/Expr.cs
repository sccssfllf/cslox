using System.Collections.Generic;

namespace cslox;

public abstract class Expr
{
      public interface IVisitor<R>
      {
         R visitBinaryExpr(Binary expr);
         R visitGroupingExpr(Grouping expr);
         R visitLiteralExpr(Literal expr);
         R visitUnaryExpr(Unary expr);
      }

      public abstract R Accept<R>(IVisitor<R> visitor);

     public class Binary : Expr
      {
         public Binary(Expr left, Token operator, Expr right)
              {
                 this.Left = left;
                 this.Operator = operator;
                 this.Right = right;
              }

          public Expr Left { get; }
          public Token Operator { get; }
          public Expr Right { get; }

          public override R Accept<R>(IVisitor<R> visitor)
          {
              return visitor.VisitBinaryExpr(this);
          }
      }

     public class Grouping : Expr
      {
         public Grouping(Expr expression)
              {
                 this.Expression = expression;
              }

          public Expr Expression { get; }

          public override R Accept<R>(IVisitor<R> visitor)
          {
              return visitor.VisitGroupingExpr(this);
          }
      }

     public class Literal : Expr
      {
         public Literal(object value)
              {
                 this.Value = value;
              }

          public object Value { get; }

          public override R Accept<R>(IVisitor<R> visitor)
          {
              return visitor.VisitLiteralExpr(this);
          }
      }

     public class Unary : Expr
      {
         public Unary(Token operator, Expr right)
              {
                 this.Operator = operator;
                 this.Right = right;
              }

          public Token Operator { get; }
          public Expr Right { get; }

          public override R Accept<R>(IVisitor<R> visitor)
          {
              return visitor.VisitUnaryExpr(this);
          }
      }
}
