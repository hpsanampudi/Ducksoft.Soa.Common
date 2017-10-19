using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ducksoft.Soa.Common.ObjectComparer
{
    /// <summary>
    /// Abstract class which is used to store object comparer model data.
    /// </summary>
    /// <typeparam name="TObjectType">The type of the object type.</typeparam>
    /// <typeparam name="TCompareObjType">The type of the compare object type.</typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.ObjectComparer.IObjectComparerModel{TObjectType, TCompareObjType}" />
    public abstract class ObjectComparerModel<TObjectType, TCompareObjType>
        : IObjectComparerModel<TObjectType, TCompareObjType>
    {
        /// <summary>
        /// Gets the source object.
        /// </summary>
        /// <value>
        /// The source object.
        /// </value>
        public IEnumerable<TObjectType> Source { get; private set; }

        /// <summary>
        /// Gets the target object.
        /// </summary>
        /// <value>
        /// The target object.
        /// </value>
        public IEnumerable<TObjectType> Target { get; private set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public IList<TCompareObjType> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        /// <value>
        /// The name of the primary key.
        /// </value>
        public string PrimaryKeyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the compare property.
        /// </summary>
        /// <value>
        /// The name of the compare property.
        /// </value>
        public string ComparePropName { get; set; }

        /// <summary>
        /// Gets the display type.
        /// </summary>
        /// <value>
        /// The display type.
        /// </value>
        public ObjectComparDisplayTypes DisplayType
        {
            get
            {
                return ((string.IsNullOrWhiteSpace(ComparePropName)) ?
                    ObjectComparDisplayTypes.Horizontal :
                    ObjectComparDisplayTypes.Vertical);
            }
        }

        /// <summary>
        /// Executes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public IEnumerable<TCompareObjType> Execute(IEnumerable<TObjectType> source,
            IEnumerable<TObjectType> target)
        {
            Results = default(IList<TCompareObjType>);
            Source = new List<TObjectType>(source ?? new List<TObjectType>());
            Target = new List<TObjectType>(target ?? new List<TObjectType>());

            var displayType = DisplayType;
            if ((!Source.Any()) && (!Target.Any()))
            {
                //Hp --> Logic: But are empty collection objects
                Results = new List<TCompareObjType>();
            }
            else if ((!Source.Any()) && (Target.Any()))
            {
                //Hp --> Logic: If source is empty collection and target is having values then
                //mark all target items as newly added.
                Results = Target.Select(T => ConvertTo(T,
                    ObjectCompareStatusTypes.Added, displayType, null, null)).ToList();
            }
            else if ((Source.Any()) && (!Target.Any()))
            {
                //Hp --> Logic: If source is having values and target is empty collection then
                //mark all source items as deleted.
                Results = Source.Select(T => ConvertTo(T,
                    ObjectCompareStatusTypes.Deleted, displayType, null, null)).ToList();
            }
            else
            {
                //Hp --> Logic: If source and target is having values then perform custom execute.
                Results = Compare().ToList();
            }

            return (Results);
        }

        /// <summary>
        /// The convert to
        /// </summary>
        Func<TObjectType, ObjectCompareStatusTypes, ObjectComparDisplayTypes,
            object, int?, TCompareObjType> ConvertTo =
            (srcObject, status, displayType, targetValue, oId) =>
            {
                return ((TCompareObjType)Activator.CreateInstance(typeof(TCompareObjType),
                    srcObject, status, displayType, targetValue, oId));
            };

        /// <summary>
        /// Compares this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual IEnumerable<TCompareObjType> Compare()
        {
            var equalItems = new Dictionary<TObjectType, TObjectType>();
            var srcDeletedItems = new List<TObjectType>();
            var trgtAddedItems = new List<TObjectType>();
            var modifiedItems = new Dictionary<TObjectType, TObjectType>();

            //Hp --> Logic: Compare whether source item exists in target.
            foreach (var item in Source)
            {
                var compareObject = Target.SingleOrDefault(T => T.Equals(item));
                if (null == compareObject)
                {
                    //Hp --> Logic: Means source item may be deleted (or) modified.
                    //So, check whether any item in target has same primary key.
                    var pkValue = GetPropValue(item, PrimaryKeyName);
                    var isExists = Target.SingleOrDefault(GetPKExpression(pkValue));
                    if (null == isExists)
                    {
                        srcDeletedItems.Add(item);
                    }
                    else
                    {
                        modifiedItems.Add(item, isExists);
                    }
                }
                else
                {
                    equalItems.Add(item, compareObject);
                }
            }

            var comparer = (IEqualityComparer<TObjectType>)Activator.CreateInstance<TObjectType>();
            var targetExceptEqual = Target.Except(equalItems.Values, comparer);
            var targetModified = modifiedItems.Values;
            trgtAddedItems = targetExceptEqual.Except(targetModified, comparer).ToList();

            var results = new List<TCompareObjType>();
            switch (DisplayType)
            {
                case ObjectComparDisplayTypes.Vertical:
                    {
                        results.AddRange(PrepareVerticalResults(equalItems, srcDeletedItems,
                            trgtAddedItems, modifiedItems));
                    }
                    break;

                case ObjectComparDisplayTypes.Horizontal:
                    {
                        results.AddRange(PrepareHorizontalResults(equalItems, srcDeletedItems,
                            trgtAddedItems, modifiedItems));
                    }
                    break;

                case ObjectComparDisplayTypes.None:
                default:
                    {
                        var errMessage = $"The given display type {DisplayType} is not handled!";
                        throw (new NotImplementedException(errMessage));
                    }
            }

            return (results);
        }

        /// <summary>
        /// Prepares the vertical results.
        /// </summary>
        /// <param name="equalItems">The equal items.</param>
        /// <param name="srcDeletedItems">The source deleted items.</param>
        /// <param name="trgtAddedItems">The TRGT added items.</param>
        /// <param name="modifiedItems">The modified items.</param>
        /// <returns></returns>
        private IEnumerable<TCompareObjType> PrepareVerticalResults(
           Dictionary<TObjectType, TObjectType> equalItems, List<TObjectType> srcDeletedItems,
            List<TObjectType> trgtAddedItems, Dictionary<TObjectType, TObjectType> modifiedItems)
        {
            var results = new List<TCompareObjType>();
            var displayType = DisplayType;

            //Hp --> Logic: Get all equal items.
            results.AddRange(equalItems.Keys.Select(I => ConvertTo(I,
                ObjectCompareStatusTypes.Equal, displayType, GetPropValue(I, ComparePropName),
                null)));

            //Hp --> Logic: Get all deleted items.
            results.AddRange(srcDeletedItems.Select(I => ConvertTo(I,
                ObjectCompareStatusTypes.Deleted, displayType, null, null)));

            //Hp --> Logic: Get all added items.
            var oIdIndex = Source.Count();
            results.AddRange(trgtAddedItems.Select(I => ConvertTo(I,
                ObjectCompareStatusTypes.Added, displayType, null, ++oIdIndex)));

            //Hp --> Logic: Get all modified items.
            results.AddRange(modifiedItems.Select(I => ConvertTo(I.Key,
                ObjectCompareStatusTypes.ModifiedAtBoth, displayType,
                GetPropValue(I.Value, ComparePropName), null)));

            return (results);
        }

        /// <summary>
        /// Prepares the horizontal results.
        /// </summary>
        /// <param name="equalItems">The equal items.</param>
        /// <param name="srcDeletedItems">The source deleted items.</param>
        /// <param name="trgtAddedItems">The TRGT added items.</param>
        /// <param name="modifiedItems">The modified items.</param>
        /// <returns></returns>
        private IEnumerable<TCompareObjType> PrepareHorizontalResults(
            Dictionary<TObjectType, TObjectType> equalItems, List<TObjectType> srcDeletedItems,
            List<TObjectType> trgtAddedItems, Dictionary<TObjectType, TObjectType> modifiedItems)
        {
            var results = new List<TCompareObjType>();
            var displayType = DisplayType;
            var oIdIndex = 0;

            //Hp --> Logic: Get all deleted items.
            results.AddRange(srcDeletedItems.Select(I => ConvertTo(I,
                ObjectCompareStatusTypes.Deleted, displayType, null, oIdIndex++)));

            //Hp --> Logic: Get all added items.
            results.AddRange(trgtAddedItems.Select(I => ConvertTo(I,
                ObjectCompareStatusTypes.Added, displayType, null, oIdIndex++)));

            //Hp --> Logic: Get all modified items.
            foreach (var item in modifiedItems)
            {
                results.Add(ConvertTo(item.Key, ObjectCompareStatusTypes.ModifiedAtSource,
                    displayType, null, oIdIndex++));

                results.Add(ConvertTo(item.Value, ObjectCompareStatusTypes.ModifiedAtTarget,
                    displayType, null, oIdIndex++));
            }

            //Hp --> Logic: Get all equal items.
            foreach (var item in equalItems)
            {
                results.Add(ConvertTo(item.Key, ObjectCompareStatusTypes.Equal, displayType,
                    null, oIdIndex++));

                results.Add(ConvertTo(item.Value, ObjectCompareStatusTypes.Equal, displayType,
                    null, oIdIndex++));
            }

            return (results);
        }

        /// <summary>
        /// Gets the pk expression.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object type.</typeparam>
        /// <param name="pkValue">The pk value.</param>
        /// <returns></returns>
        protected virtual Func<TObjectType, bool> GetPKExpression(object pkValue)
        {
            var parameter = Expression.Parameter(typeof(TObjectType), "T");
            var property = Expression.Property(parameter, PrimaryKeyName); //T.<PrimaryKeyName>
            var equalsTo = Expression.Constant(pkValue);
            var body = Expression.Equal(property, equalsTo); //T.<PrimaryKeyName> == <pkValue>

            // Hp --> Finally create entire expression - T => T.<PrimaryKeyName> == <pkValue>
            var result = Expression.Lambda<Func<TObjectType, bool>>(body, new[] { parameter });

            return (result.Compile());
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        protected object GetPropValue(TObjectType targetObject, string propName) =>
            targetObject.GetType().GetProperty(propName).GetValue(targetObject, null);
    }
}
