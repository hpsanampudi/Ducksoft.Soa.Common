using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.Soa.Common.Filters
{
    /// <summary>
    /// Class which is used to build Linq filter expression by groups. 
    /// </summary>
    public class FilterGroup
    {
        /// <summary>
        /// Gets or sets the type of the operator.
        /// </summary>
        /// <value>
        /// The type of the operator.
        /// </value>
        public FilterLogicalOperatorTypes OperatorType { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<FilterBuilder> Filters { get; set; }

        /// <summary>
        /// Gets or sets the sub groups.
        /// </summary>
        /// <value>
        /// The sub groups.
        /// </value>
        public List<FilterGroup> SubGroups { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterGroup"/> class.
        /// </summary>
        public FilterGroup()
        {
            OperatorType = FilterLogicalOperatorTypes.None;
            Filters = new List<FilterBuilder>();
            SubGroups = new List<FilterGroup>();
        }

        /// <summary>
        /// Creates the linq expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public Expression CreateLinqExpression<T>(ParameterExpression parameter) where T : class
        {
            ErrorBase.CheckArgIsNull(parameter, () => parameter);
            var linqExp = default(Expression);
            var leftExp = default(Expression);
            var rightExp = default(Expression);

            var filterExpList = Filters.Select(item => item.CreateLinqExpression<T>(parameter));
            if (filterExpList.Any())
            {
                leftExp = filterExpList.Aggregate((E1, E2) => CombineExpression(E1, E2));
            }

            var subGrpExpList = SubGroups.Select(item => item.CreateLinqExpression<T>(parameter));
            if (subGrpExpList.Any())
            {
                rightExp = subGrpExpList.Aggregate((E1, E2) => CombineExpression(E1, E2));
            }

            if ((null != leftExp) && (null != rightExp))
            {
                linqExp = CombineExpression(leftExp, rightExp);
            }
            else if (null != leftExp)
            {
                linqExp = leftExp;
            }
            else if (null != rightExp)
            {
                linqExp = rightExp;
            }

            return (linqExp);
        }

        /// <summary>
        /// Combines the expression.
        /// </summary>
        /// <param name="exp1">The exp1.</param>
        /// <param name="exp2">The exp2.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private BinaryExpression CombineExpression(Expression exp1, Expression exp2)
        {
            var mergeExp = default(BinaryExpression);
            switch (OperatorType)
            {
                case FilterLogicalOperatorTypes.None:
                    break;

                case FilterLogicalOperatorTypes.And:
                    {
                        mergeExp = Expression.AndAlso(exp1, exp2);
                    }
                    break;

                case FilterLogicalOperatorTypes.Or:
                    {
                        mergeExp = Expression.OrElse(exp1, exp2);
                    }
                    break;

                default:
                    {
                        throw (new NotImplementedException(
                            $"The given {OperatorType} is not handled!"));
                    }
            }

            return (mergeExp);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var trgtObject = obj as FilterGroup;
            if (null == trgtObject)
            {
                return (false);
            }

            var fComparer = new CustomEqualityComparer<FilterBuilder>();
            var gComparer = new CustomEqualityComparer<FilterGroup>();
            return (OperatorType.Equals(trgtObject.OperatorType) &&
                (Filters?.TrueForAll(F => trgtObject.Filters?.Contains(F, fComparer) ?? false)
                ?? false) &&
                (SubGroups?.TrueForAll(G => trgtObject.SubGroups?.Contains(G, gComparer) ?? false)
                ?? false));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
