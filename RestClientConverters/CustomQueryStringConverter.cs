using Ducksoft.Soa.Common.DataContracts;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Dispatcher;

namespace Ducksoft.Soa.Common.RestClientConverters
{
    /// <summary>
    /// Class which is used to convert WCF rest url parameters to string (or) vice versa.
    /// </summary>
    /// <seealso cref="System.ServiceModel.Dispatcher.QueryStringConverter" />
    public class CustomQueryStringConverter : QueryStringConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override bool CanConvert(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            return ((typeof(DynamicLinqFilter) == type) ||
                (typeof(IList<QueryOption>) == type) ||
                (typeof(int[]) == type) ||
                (typeof(double[]) == type) ||
                (typeof(float[]) == type) ||
                (typeof(string[]) == type) ||
                (typeof(DateTime) == type) ||
                (type.IsEnum) ||
                (base.CanConvert(nullableType ?? type)));
        }

        /// <summary>
        /// Converts the string to value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns></returns>
        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (typeof(DynamicLinqFilter) == parameterType)
            {
                // Hp --> Logic: Check whether null value is passed or not?
                return ((string.IsNullOrEmpty(parameter) ||
                    string.Equals(parameter.ToLower().Trim(), "null")) ? null :
                    Utility.DeserializeFromJson<DynamicLinqFilter>(parameter));
            }
            else if (typeof(IList<QueryOption>) == parameterType)
            {
                #region Hp --> Example: Json string format to be passed in URL
                //[
                //  {
                //    "Option": "$filter",
                //    "Query": "startswith(CompanyName, 'A')"
                //  },
                //  {
                //    "Option": "$orderby",
                //    "Query": "CompanyName asc,CompanyId asc"
                //  },
                //  {
                //    "Option": "$skip",
                //    "Query": "0"
                //  },
                //  {
                //    "Option": "$top",
                //    "Query": "20"
                //  }
                //] 
                #endregion
                // Hp --> Logic: Check whether null value is passed or not?
                return ((string.IsNullOrEmpty(parameter) ||
                    string.Equals(parameter.ToLower().Trim(), "null")) ? null :
                    Utility.DeserializeFromJson<IList<QueryOption>>(parameter));
            }
            else if (typeof(DateTime) == parameterType)
            {
                return (parameter.ToDateTime());
            }
            else if (parameterType.IsArray)
            {
                var elementType = parameterType.GetElementType();
                if ((elementType.IsValueType) || (typeof(string) == elementType.GetType()))
                {
                    var parameterList = parameter.Split(',');
                    var result = Array.CreateInstance(elementType, parameterList.Length);
                    for (int index = 0; index < parameterList.Length; index++)
                    {
                        result.SetValue(
                            base.ConvertStringToValue(parameterList[index], elementType), index);
                    }

                    return (result);
                }
                else
                {
                    throw (new ExceptionBase(string.Format(CultureInfo.CurrentUICulture,
                        "Custom query string converter for concreate array datatypes not handled!"),
                        new NotImplementedException()));
                }
            }
            else if ((parameterType.IsEnum))
            {
                var enumValue = Utility.GetEnumFrom(parameterType, parameter, false);
                if (null == enumValue)
                {
                    enumValue = base.ConvertStringToValue(parameter, parameterType) as Enum;
                }

                return (enumValue);
            }
            else
            {
                var srcValue = parameter?.Trim() ?? string.Empty;
                if (srcValue.IsEqualTo("null"))
                {
                    srcValue = null;
                }

                var nullableType = Nullable.GetUnderlyingType(parameterType);
                if ((nullableType != null) && (string.IsNullOrWhiteSpace(srcValue)))
                {
                    return (null);
                }

                return (base.ConvertStringToValue(srcValue, (nullableType ?? parameterType)));
            }
        }

        /// <summary>
        /// Converts the value to string.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns></returns>
        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            //Hp --> Logic: If object value is null then return empty string.
            if (typeof(DynamicLinqFilter) == parameterType)
            {
                //Hp --> Logic: If object value is null then return empty string.
                return ((null != parameter) ? Utility.SerializeToJson(parameter) : string.Empty);
            }
            else if (typeof(IList<QueryOption>) == parameterType)
            {
                //Hp --> Logic: If object value is null then return empty string.
                return ((null != parameter) ? Utility.SerializeToJson(parameter) : string.Empty);
            }
            else if (typeof(DateTime) == parameterType)
            {
                //Hp --> Logic: If object value is null then return default value.
                return ((null != parameter) ? parameter.ToString() : default(DateTime).ToString());
            }
            else if (parameterType.IsArray)
            {
                //Hp --> Logic: If object value is null then return empty string.
                return ((null != parameter) ? string.Join(",", parameter) : string.Empty);
            }
            else if ((parameterType.IsEnum))
            {
                Func<Enum, Type, string> GetEnumDescription = (E, T) =>
                {
                    var description = Utility.GetEnumDescription(E, false);
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        description = base.ConvertValueToString(E, T);
                    }

                    return (description);
                };

                return ((null != parameter) ?
                    GetEnumDescription(parameter as Enum, parameterType) : string.Empty);
            }
            else
            {
                var nullableType = Nullable.GetUnderlyingType(parameterType);
                if ((nullableType != null) && (parameter == null))
                {
                    //Hp --> Logic: If object value is null then return empty string.
                    return (string.Empty);
                }

                return (base.ConvertValueToString(parameter, (nullableType ?? parameterType)));
            }
        }
    }
}
