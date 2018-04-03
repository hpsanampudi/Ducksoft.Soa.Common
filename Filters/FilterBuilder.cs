using Ducksoft.Soa.Common.Utilities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.Filters
{
    /// <summary>
    /// Class which is used to build Linq filter expression. 
    /// </summary>
    [DataContract(Name = "FilterBuilder",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class FilterBuilder
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the operator.
        /// </summary>
        /// <value>
        /// The type of the operator.
        /// </value>
        [DataMember]
        public FilterCompareOperatorTypes OperatorType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [IgnoreDataMember]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the value as string.
        /// </summary>
        /// <value>
        /// The value as string.
        /// </value>
        [DataMember(Name = "Value", IsRequired = true)]
        public string ValueAsStr { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterBuilder"/> class.
        /// </summary>
        public FilterBuilder()
        {
            PropertyName = string.Empty;
            OperatorType = FilterCompareOperatorTypes.None;
            Value = null;
        }

        #region SerializeToXml/DeserializeFromXml
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            //Hp --> Logic: Converts value to string.
            var propType = Value?.GetType();
            ValueAsStr = $"{propType}::{Value}";
        }

        /// <summary>
        /// Called when [deserializing].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserialized]
        void OnDeserializing(StreamingContext context)
        {
            //Hp --> Logic: Converts string value to corresponding object type.
            var delimeter = new string[] { "::" };
            var data = ValueAsStr?.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
            if ((data?.Length ?? -1) == 2)
            {
                Value = Utility.ConvertTo(data[1], data[0]);
            }
            else
            {
                Value = null;
            }
        }
        #endregion

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
            var rightExp = (null != Value) ? (Expression.Constant(
                ((typeof(string) != propType) ? Value : Value.ToString().Trim()), propType)) :
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

                case FilterCompareOperatorTypes.NotContains:
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
        /// Gets the OData filter expression.
        /// </summary>
        /// <value>
        /// The o data filter expression.
        /// </value>
        public string ODataFilterExpression
        {
            get
            {
                var result = string.Empty;
                switch (OperatorType)
                {
                    case FilterCompareOperatorTypes.EqualTo:
                        {
                            result = $"{PropertyName} eq {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.NotEqualTo:
                        {
                            result = $"{PropertyName} ne {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.LessThan:
                        {
                            result = $"{PropertyName} lt {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.LessThanOrEqualTo:
                        {
                            result = $"{PropertyName} le {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.GreaterThan:
                        {
                            result = $"{PropertyName} gt {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.GreaterThanOrEqualTo:
                        {
                            result = $"{PropertyName} ge {ODataFilterValue}";
                        }
                        break;

                    case FilterCompareOperatorTypes.StartsWith:
                        {
                            result = $"startswith({PropertyName}, {ODataFilterValue})";
                        }
                        break;

                    case FilterCompareOperatorTypes.EndsWith:
                        {
                            result = $"endswith({PropertyName}, {ODataFilterValue})";
                        }
                        break;

                    case FilterCompareOperatorTypes.Contains:
                        {
                            result = $"substringof({ODataFilterValue}, {PropertyName})";
                        }
                        break;

                    case FilterCompareOperatorTypes.NotContains:
                        {
                            result = $"not(substringof({ODataFilterValue}, {PropertyName})";
                        }
                        break;

                    case FilterCompareOperatorTypes.IsNull:
                        {
                            result = $"{PropertyName} eq null";
                        }
                        break;

                    case FilterCompareOperatorTypes.IsNotNull:
                        {
                            result = $"not({PropertyName} eq null)";
                        }
                        break;

                    case FilterCompareOperatorTypes.IsEmpty:
                        {
                            result = $"{PropertyName} eq '{string.Empty}'";
                        }
                        break;

                    case FilterCompareOperatorTypes.IsNotEmpty:
                        {
                            result = $"not({PropertyName} eq '{string.Empty}')";
                        }
                        break;
                }

                return (result);
            }
        }

        /// <summary>
        /// To the OData filter value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual string ODataFilterValue
        {
            get
            {
                var oDataStr = string.Empty;
                if (typeof(string) == Value.GetType())
                {
                    oDataStr = Value.ToString().ToODataStr();
                }
                else if (typeof(DateTime) == Value.GetType())
                {
                    oDataStr = ((DateTime)Value).ToODataDate();
                }
                else
                {
                    oDataStr = $"{Value}";
                }

                return (oDataStr);
            }
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ($"({ODataFilterExpression})");
        }
    }
}
