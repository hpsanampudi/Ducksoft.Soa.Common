﻿using Ducksoft.Soa.Common.Utilities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ducksoft.Soa.Common.Filters
{
    /// <summary>
    /// Class which is used to build Linq filter expression. 
    /// </summary>
    public class FilterBuilder
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the operator.
        /// </summary>
        /// <value>
        /// The type of the operator.
        /// </value>
        public FilterCompareOperatorTypes OperatorType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterBuilder"/> class.
        /// </summary>
        public FilterBuilder()
        {
            PropertyName = string.Empty;
            OperatorType = FilterCompareOperatorTypes.None;
            Value = null;
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
            var srcType = typeof(T);

            var propInfo = srcType.GetProperties()
                .SingleOrDefault(P => P.Name.IsEqualTo(PropertyName));

            if (null == propInfo)
            {
                //Hp --> If given column does not exists then throw exception.
                throw (new NullReferenceException(string.Join(Environment.NewLine,
                    $"{nameof(propInfo)} instance is null.",
                    $"[Reason]: Cannot find column with name \"{PropertyName}\".")));
            }

            var propType = propInfo.PropertyType;
            if (!PropertyComparer<T>.CanSort(propType))
            {
                //Hp --> Logic: Check property type implements IComparable (or) not?
                throw (new NotSupportedException(
                    $"{propType} does not implement {nameof(IComparable)}."));
            }

            var expComparer = Expression.Constant(StringComparison.CurrentCultureIgnoreCase);
            var expNullObject = Expression.Constant(null);
            if ((null != (Nullable.GetUnderlyingType(propType))) || (!propType.IsValueType))
            {
                expNullObject = Expression.Constant(null, propType);
            }

            Func<string, MethodInfo> GetStrComparerMethod = (mName) =>
            typeof(string).GetMethod(mName, new[] { typeof(string), typeof(StringComparison) });

            Func<Expression, Expression> expGetStrIsNullOrEmpty = (exp) =>
                Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, exp);

            var GetStrTrimMethod = typeof(string).GetMethod(nameof(string.Trim), new Type[0]);

            Func<Expression, string, Expression, Expression> expGetStrComparer =
                (left, mName, right) => Expression.AndAlso(
                    Expression.Not(expGetStrIsNullOrEmpty(left)),
                    Expression.Call(Expression.Call(left, GetStrTrimMethod),
                    GetStrComparerMethod(mName), right, expComparer));

            Func<Expression, string, Expression, Expression> expGetStrContains =
                (left, mName, right) => Expression.AndAlso(
                    Expression.Not(expGetStrIsNullOrEmpty(left)),
                    Expression.GreaterThanOrEqual(Expression.Call(
                        Expression.Call(left, GetStrTrimMethod),
                        GetStrComparerMethod(mName), right, expComparer), Expression.Constant(0)));

            Action<string, Type> throwIfObjectType = (isEqual, TObj) =>
            {
                var condition = isEqual.IsEqualTo(nameof(Expression.Equal)) ?
                (TObj == propType) : (TObj != propType);

                if (condition)
                {
                    var errMessage = $"The given operator {OperatorType} is not supported " +
                    $"on object type {propType}";

                    throw (new NotSupportedException(errMessage));
                }
            };

            //Hp --> Logic: Create E.Id portion of lambda expression (left)
            var leftExp = Expression.Property(parameter, propInfo.Name);

            //Hp --> Logic: Create 'id' portion of lambda expression (right)
            var rightExp = (null != Value) ? Expression.Constant(
               ((typeof(string) != propType) ? Value : Value.ToString().Trim()), propType) :
               expNullObject;

            // create E.Id == 'id' portion of lambda expression    
            switch (OperatorType)
            {
                case FilterCompareOperatorTypes.None:
                    break;

                case FilterCompareOperatorTypes.EqualTo:
                    {
                        if (typeof(string) == propType)
                        {
                            linqExp = expGetStrComparer(leftExp, nameof(string.Equals), rightExp);
                        }
                        else
                        {
                            linqExp = Expression.Equal(leftExp, rightExp);
                        }
                    }
                    break;

                case FilterCompareOperatorTypes.NotEqualTo:
                    {
                        if (typeof(string) == propType)
                        {
                            linqExp = Expression.Not(
                                expGetStrComparer(leftExp, nameof(string.Equals), rightExp));
                        }
                        else
                        {
                            linqExp = Expression.NotEqual(leftExp, rightExp);
                        }
                    }
                    break;

                case FilterCompareOperatorTypes.LessThan:
                    {
                        throwIfObjectType(nameof(Expression.Equal), typeof(string));
                        linqExp = Expression.LessThan(leftExp, rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.LessThanOrEqualTo:
                    {
                        throwIfObjectType(nameof(Expression.Equal), typeof(string));
                        linqExp = Expression.LessThanOrEqual(leftExp, rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.GreaterThan:
                    {
                        throwIfObjectType(nameof(Expression.Equal), typeof(string));
                        linqExp = Expression.GreaterThan(leftExp, rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.GreaterThanOrEqualTo:
                    {
                        throwIfObjectType(nameof(Expression.Equal), typeof(string));
                        linqExp = Expression.GreaterThanOrEqual(leftExp, rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.StartsWith:
                    {
                        throwIfObjectType(nameof(Expression.NotEqual), typeof(string));
                        linqExp = expGetStrComparer(leftExp, nameof(string.StartsWith), rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.EndsWith:
                    {
                        throwIfObjectType(nameof(Expression.NotEqual), typeof(string));
                        linqExp = expGetStrComparer(leftExp, nameof(string.EndsWith), rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.Contains:
                    {
                        //Hp --> Note: Contains doesnot have overload to take string comparer.
                        //In this case, call string.IndexOf method and check its value ge -1
                        throwIfObjectType(nameof(Expression.NotEqual), typeof(string));
                        linqExp = expGetStrContains(leftExp, nameof(string.IndexOf), rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.DoesNotContain:
                    {
                        //Hp --> Note: Contains doesnot have overload to take string comparer.
                        //In this case, call string.IndexOf method and check its value ge -1
                        throwIfObjectType(nameof(Expression.NotEqual), typeof(string));
                        linqExp = Expression.Not(
                            expGetStrContains(leftExp, nameof(string.IndexOf), rightExp));
                    }
                    break;

                case FilterCompareOperatorTypes.IsNull:
                    {
                        linqExp = Expression.Equal(leftExp, rightExp);
                    }
                    break;
                case FilterCompareOperatorTypes.IsNotNull:
                    {
                        linqExp = Expression.NotEqual(leftExp, rightExp);
                    }
                    break;

                case FilterCompareOperatorTypes.IsEmpty:
                    {
                        if (typeof(string) == propType)
                        {
                            linqExp = expGetStrIsNullOrEmpty(leftExp);
                        }
                        else
                        {
                            linqExp = Expression.Equal(leftExp, expNullObject);
                        }
                    }
                    break;

                case FilterCompareOperatorTypes.IsNotEmpty:
                    {
                        if (typeof(string) == propType)
                        {
                            linqExp = Expression.Not(expGetStrIsNullOrEmpty(leftExp));
                        }
                        else
                        {
                            linqExp = Expression.NotEqual(leftExp, expNullObject);
                        }
                    }
                    break;

                default:
                    {
                        throw (new NotImplementedException(
                            $"The given {OperatorType} is not handled!"));
                    }

            }

            return (linqExp);
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
            var trgtObject = obj as FilterBuilder;
            if (null == trgtObject)
            {
                return (false);
            }

            return (OperatorType.Equals(trgtObject.OperatorType) &&
                (PropertyName.IsEqualTo(trgtObject.PropertyName)) &&
                (Value?.Equals(trgtObject.Value) ?? false));
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
