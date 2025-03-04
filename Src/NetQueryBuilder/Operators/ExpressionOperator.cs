using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;


        public class ExpressionOperator
        {
            public virtual ExpressionType ExpressionType { get; set; }
            public virtual string DisplayText { get; set; }

            public override bool Equals(object obj)
            {
                return obj is ExpressionOperator op
                    && ExpressionType == op.ExpressionType;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ExpressionType);
            }

            public static bool operator ==(ExpressionOperator left, ExpressionOperator right)
            {
                return EqualityComparer<ExpressionOperator>.Default.Equals(left, right);
            }

            public static bool operator !=(ExpressionOperator left, ExpressionOperator right)
            {
                return !(left == right);
            }
        }