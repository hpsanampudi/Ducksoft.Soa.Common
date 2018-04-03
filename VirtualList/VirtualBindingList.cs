using Ducksoft.Soa.Common.Filters;
using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ducksoft.Soa.Common.VirtualList
{
    /// <summary>
    /// Class which inherits binding list and supports sorting and filtering of its items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.ComponentModel.BindingList{T}" />
    /// <seealso cref="System.ComponentModel.IBindingListView" />
    public class VirtualBindingList<T> : BindingList<T>, IBindingListView where T : class
    {
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        protected List<T> DataSource { get; private set; }

        /// <summary>
        /// Gets the orignal data source.
        /// </summary>
        /// <value>
        /// The orignal data source.
        /// </value>
        public IReadOnlyList<T> OrignalDataSource
        {
            get
            {
                return ((DataSource ?? new List<T>()).AsReadOnly());
            }
        }

        /// <summary>
        /// The is filter duplicates
        /// </summary>
        private bool isFilterDuplicates;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter duplicates.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is filter duplicates; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterDuplicates
        {
            get
            {
                return (isFilterDuplicates);
            }
            set
            {
                isFilterDuplicates = value;
                RemoveWithFilter();
            }
        }

        /// <summary>
        /// The is sorted
        /// </summary>
        private bool isSorted;

        /// <summary>
        /// Gets whether the items in the list are sorted.
        /// </summary>
        public bool IsSorted
        {
            get
            {
                return (isSorted);
            }
            protected set
            {
                isSorted = value;
                if (!isSorted)
                {
                    sortDescriptions = new List<ListSortDescription>();
                }
            }
        }

        /// <summary>
        /// Gets or sets the linq filter.
        /// </summary>
        /// <value>
        /// The linq filter.
        /// </value>
        public FilterGroup Filter { get; protected set; }

        /// <summary>
        /// The sort list descriptions
        /// </summary>
        private IEnumerable<ListSortDescription> sortDescriptions;

        /// <summary>
        /// The synchronize object
        /// </summary>
        private ISynchronizeInvoke syncObject;

        /// <summary>
        /// The on filter changed
        /// </summary>
        public event EventHandler<FilterChangedEventArgs> FilterChanged;

        /// <summary>
        /// Occurs when [sort changed].
        /// </summary>
        public event EventHandler<SortChangedEventArgs> SortChanged;

        /// <summary>
        /// Occurs when [notify list changed].
        /// </summary>
        public event EventHandler<NotifyListChangedEventArgs<T>> NotifyListChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualBindingList{T}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isFilterDuplicates">if set to <c>true</c> [is filter duplicates].</param>
        /// <param name="syncObject">The synchronize object.</param>
        public VirtualBindingList(IEnumerable<T> source = null, bool isFilterDuplicates = false,
            ISynchronizeInvoke syncObject = null) : base(source?.ToList() ?? new List<T>())
        {
            DataSource = new List<T>();
            this.isFilterDuplicates = isFilterDuplicates;
            IsSorted = false;
            Filter = null;

            this.syncObject = syncObject;
            if (source?.Any() ?? false)
            {
                AddRange(source);
            }
        }

        #region BindingList: Override methods/properties
        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        protected override bool IsSortedCore => IsSorted;

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        protected override ListSortDirection SortDirectionCore =>
            sortDescriptions?.FirstOrDefault()?.SortDirection ?? default(ListSortDirection);

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null.
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore =>
            sortDescriptions?.FirstOrDefault()?.PropertyDescriptor;

        /// <summary>
        /// Gets a value indicating whether the list supports searching.
        /// </summary>
        protected override bool SupportsSearchingCore => true;

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        protected override bool SupportsSortingCore => true;

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.BindingList`1.ListChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs" /> that contains the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            //Hp --> Logic: To handle cross thread exception, marshal the event invocations 
            //onto the UI thread.
            if (null == syncObject)
            {
                RaiseListChanged(e);
            }
            else
            {
                Action<ListChangedEventArgs> raiseOnListChanged = RaiseListChanged;
                syncObject?.Invoke(raiseOnListChanged, new object[] { e });
            }
        }

        /// <summary>
        /// Sorts the items if overridden in a derived class; otherwise, throws a <see cref="T:System.NotSupportedException" />.
        /// </summary>
        /// <param name="prop">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that specifies the property to sort on.</param>
        /// <param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection" />  values.</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            ApplySortCore(new List<ListSortDescription>
            {
                new ListSortDescription(prop, direction)
            });
        }

        /// <summary>
        /// Commits a pending new item to the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the new item to be added.</param>
        public override void EndNew(int itemIndex)
        {
            try
            {
                //TODO: Hp --> This logic needs to be tested once again.
                //Hp --> Logic: Check to see if the item is added to the end of the list,and if so, 
                //re-apply filter by sorting the list.
                if (IsSorted & ((Count - 1) == itemIndex))
                {
                    //Hp --> Logic: Do not notify user, because this would reset the position.
                    AddWithFilter();
                }
            }
            finally
            {
                base.EndNew(itemIndex);
            }
        }

        /// <summary>
        /// Removes any sort applied with <see cref="M:System.ComponentModel.BindingList`1.ApplySortCore(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" /> if sorting is implemented in a derived class; otherwise, raises <see cref="T:System.NotSupportedException" />.
        /// </summary>
        protected override void RemoveSortCore()
        {
            if (!IsSorted)
            {
                return;
            }

            //Hp --> Logic: Reset the list with original data source.
            IsSorted = false;
            AddWithFilter();
            OnSortChanged(new SortChangedEventArgs(true));
        }

        /// <summary>
        /// Searches for the index of the item that has the specified property descriptor with the specified value, if searching is implemented in a derived class; otherwise, a <see cref="T:System.NotSupportedException" />.
        /// </summary>
        /// <param name="prop">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to search for.</param>
        /// <param name="key">The value of <paramref name="property" /> to match.</param>
        /// <returns>
        /// The zero-based index of the item that matches the property descriptor and contains the specified value.
        /// </returns>
        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            var index = -1;
            if ((null == prop) || (null == key))
            {
                //Hp --> If there is not a match, return -1 otherwise pass search to FindCore method.
                return (index);
            }

            //Hp -->Logic: Get the property info for the specified property.
            var propInfo = typeof(T).GetProperty(prop.Name);

            //Hp --> Logic: Loop through the items to see if the key value matches the property value.
            var findItem = Items.FirstOrDefault(I =>
            {
                ++index;
                return (propInfo.GetValue(I, null).Equals(key));
            });

            return ((null == findItem) ? index : -1);
        }

        #endregion

        #region Interface: IBindingListView implementation
        /// <summary>
        ///  This property is depriciated, Please use concreate class Filter property(Linq)
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// This property is depriciated, Please use concreate class Filter property(Linq)
        /// </exception>
        [Obsolete]
        string IBindingListView.Filter
        {
            get
            {
                throw new NotSupportedException($"[Info]:  This property is depriciated, " +
                    $"Please use concreate class \"{nameof(Filter)}\" property.");
            }
            set
            {
                throw new NotSupportedException($"[Info]:  This property is depriciated, " +
                    $"Please use concreate class \"{nameof(Filter)}\" property.");
            }
        }

        /// <summary>
        /// Gets the collection of sort descriptions currently applied to the data source.
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions =>
            new ListSortDescriptionCollection(sortDescriptions.ToArray());

        /// <summary>
        /// Gets a value indicating whether the data source supports advanced sorting.
        /// </summary>
        public bool SupportsAdvancedSorting => true;

        /// <summary>
        /// Gets a value indicating whether the data source supports filtering.
        /// </summary>
        public bool SupportsFiltering => true;

        /// <summary>
        /// Sorts the data source based on the given <see cref="T:System.ComponentModel.ListSortDescriptionCollection" />.
        /// </summary>
        /// <param name="sorts">The <see cref="T:System.ComponentModel.ListSortDescriptionCollection" /> containing the sorts to apply to the data source.</param>
        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            ErrorBase.CheckArgIsNull(sorts, () => sorts);
            ApplySortCore(sorts.Cast<ListSortDescription>());
        }

        /// <summary>
        /// Removes the current filter applied to the data source.
        /// </summary>
        public void RemoveFilter()
        {
            ExecuteFilter(null);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="T:System.Collections.ObjectModel.Collection`1" />. The value can be null for reference types.</param>
        public new void Add(T item)
        {
            ErrorBase.CheckArgIsNull(item, () => item);
            AddRange(new List<T> { item });
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (!collection?.Any() ?? true)
            {
                return;
            }

            DataSource = DataSource ?? new List<T>();
            DataSource.AddRange(collection);
            AddWithFilter(isNotifyUser: true);

            //Hp --> Note: Do not delete below commented code as we may need to get all duplicates.
            //if (!IsFilterDuplicates)
            //{
            //    var srcCollection = collection.ToList();
            //    var comparer = new CustomEqualityComparer<T>(C => C);
            //    var itemsToAdd = srcCollection.Except(DataSource, comparer).ToList();
            //    var duplicates = srcCollection.Except(itemsToAdd, comparer).ToList();

            //    if (duplicates?.Any() ?? false)
            //    {
            //        OnNotifyListChanged(new NotifyListChangedEventArgs<T>
            //        {
            //            ListChangedType = ListChangedNotifyTypes.Duplicates,
            //            Items = duplicates
            //        });
            //    }

            //    if (itemsToAdd?.Any() ?? false)
            //    {
            //        DataSource.AddRange(itemsToAdd);
            //        AddWithFilter(isNotifyUser: true);
            //    }

            //    return;
            //}
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public new void Remove(T item)
        {
            ErrorBase.CheckArgIsNull(item, () => item);
            RemoveRange(item);
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="isBackup">if set to <c>true</c> [is backup].</param>
        /// <param name="collection">The items.</param>
        public void RemoveRange(params T[] collection)
        {
            ErrorBase.CheckArgIsNull(collection, () => collection);

            var comparer = new CustomEqualityComparer<T>();
            var modifiedDataSource = DataSource?.Except(collection, comparer);
            var itemsToDelete = DataSource?.Except(modifiedDataSource, comparer).ToList();
            if (itemsToDelete?.Any() ?? false)
            {
                DataSource = modifiedDataSource.ToList();
                OnNotifyListChanged(new NotifyListChangedEventArgs<T>
                {
                    ListChangedType = ListChangedNotifyTypes.Deleted,
                    Items = itemsToDelete
                });
            }

            //Hp --> Logic: Pass removeFilter value as null to notify items already delete.
            RemoveWithFilter();
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="grpFilter">The remove group filter.</param>
        public void RemoveAll(FilterGroup grpFilter = null)
        {
            //Hp --> Logic: As removeFilter value null represents items already delete, 
            //create new instance of filter group to remove all items.
            RemoveWithFilter(filter: grpFilter ?? new FilterGroup());
        }

        /// <summary>
        /// Gets all duplicates.
        /// </summary>
        public IEnumerable<T> GetAllDuplicates()
        {
            var srcData = DataSource ?? new List<T>();
            return (srcData.GetDuplicates().ToList());
        }

        /// <summary>
        /// Removes any sort applied using <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" />.
        /// </summary>
        public void RemoveSort()
        {
            RemoveSortCore();
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="grpFilter">The group filter.</param>
        public void ApplyFilter(FilterGroup grpFilter)
        {
            ExecuteFilter(grpFilter);
        }

        /// <summary>
        /// Finds the specified property.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public int Find<TKey>(string property, TKey key)
        {
            //Hp --> Logic: Check the properties for a property with the specified name.
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var prop = properties.Find(property, true);
            return (FindCore(prop, key));
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Applies the sort core.
        /// </summary>
        /// <param name="sortCollection">The sort collection.</param>
        protected void ApplySortCore(IEnumerable<ListSortDescription> sortCollection)
        {
            ErrorBase.CheckArgIsNull(sortCollection, () => sortCollection);

            try
            {
                sortCollection.ForEach(item =>
                {
                    var prop = item.PropertyDescriptor;
                    var direction = item.SortDirection;
                    if (!PropertyComparer<T>.CanSort(prop.PropertyType))
                    {
                        //Hp --> Logic: Check property type implements IComparable (or) not?
                        throw (new NotSupportedException($"Cannot sort by {prop.Name}." +
                                $"{prop.PropertyType} does not implement {nameof(IComparable)}."));
                    }
                }).ToList();

                //Hp --> Logic: Sort the list using LINQ (OrderBy/ThenBy).
                IsSorted = true;
                sortDescriptions = sortCollection;
            }
            catch
            {
                //Hp --> Logic: Reset the list with original data source.
                IsSorted = false;
                throw;
            }
            finally
            {
                //Hp --> Note: Most of the times, isNotifyUser will be true. In rare cases, 
                //this function is called from EndNew, and then the OnListChanged event should 
                //not be fired.
                AddWithFilter();
                OnSortChanged(new SortChangedEventArgs(true));
            }
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="isNotifyUser">if set to <c>true</c> [is notify user].</param>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        /// <param name="notifyListType">Type of the notify list.</param>
        protected void AddItems(IEnumerable<T> collection, bool isNotifyUser = true,
            bool isReset = true, ListChangedType notifyListType = ListChangedType.ItemAdded)
        {
            ErrorBase.CheckArgIsNull(collection, () => collection);

            //Hp --> Logic: Suppress the list changed event while addding items in bulk.
            RaiseListChangedEvents = false;
            if (isReset)
            {
                Items.Clear();
            }

            collection.ForEach(item => Items.Add(item)).ToList();
            RaiseListChangedEvents = isNotifyUser;

            if (isNotifyUser)
            {
                //Hp --> Logic: Raise the ListChanged event so bound controls refresh their values.
                OnListChanged(new ListChangedEventArgs(notifyListType, -1, -1));
            }
        }

        /// <summary>
        /// Prepares the sort filter query.
        /// </summary>
        /// <param name="srcCollection">The source collection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        protected virtual IQueryable<T> PrepareSortFilterQuery(IEnumerable<T> srcCollection = null,
            FilterGroup filter = null)
        {
            var srcData = (srcCollection ?? DataSource) ?? new List<T>();

            //Hp --> Logic: Check whether do we need to filter duplicates?
            var comparer = new CustomEqualityComparer<T>();
            var filteredItems = (IsFilterDuplicates ? srcData.Distinct(comparer) : srcData)
                .AsQueryable();

            var query = filter ?? Filter;

            try
            {
                if (IsValidFilter(query))
                {
                    //Hp --> Using Linq, filter the original data source.
                    filteredItems = filteredItems.FilterBy(query);
                }
            }
            catch
            {
                //Hp --> Logic: Reset the list with original data source.
                filteredItems = DataSource?.AsQueryable();
                throw;
            }
            finally
            {
                //Hp --> Logic: Check whether sorting applied on any column
                if (IsSorted)
                {
                    var sortColumns = sortDescriptions.ToDictionary(
                        S => S.PropertyDescriptor.Name, S => S.SortDirection);

                    //Hp --> Logic: Sort the list using LINQ (OrderBy/ThenBy).
                    //Remember if the sort operation is the first sort or not (OrderBy vs ThenBy).
                    filteredItems = filteredItems.SortBy(sortColumns);
                }
            }

            return (filteredItems);
        }

        /// <summary>
        /// Add items with filter.
        /// </summary>
        /// <param name="isNotifyUser">if set to <c>true</c> [is notify user].</param>
        protected virtual void AddWithFilter(bool isNotifyUser = false)
        {
            var filteredItems = PrepareSortFilterQuery();

            //Hp --> Logic: Updated list with filtered items without notifying the user.
            AddItems(filteredItems, isNotifyUser);
        }

        /// <summary>
        /// Removes item with filter.
        /// </summary>
        /// <param name="isNotifyUser">if set to <c>true</c> [is notify user].</param>
        /// <param name="filter">The remove filter.</param>
        protected virtual void RemoveWithFilter(bool isNotifyUser = true, FilterGroup filter = null)
        {
            var filteredItems = default(IQueryable<T>);

            try
            {
                var itemsToDelete = new List<T>();
                if (null == filter)
                {
                    //Hp --> Logic: Do nothing as we have already deleted items from original data source.
                }
                else if (!IsValidFilter(filter))
                {
                    itemsToDelete = new List<T>(DataSource);
                    DataSource?.Clear();
                }
                else
                {
                    //Hp --> Using Linq, remove items from original data source.
                    var deleteExpression = filter.GetFilterExpression<T>();
                    itemsToDelete = DataSource?.AsQueryable().FilterBy(deleteExpression).ToList();
                    DataSource?.RemoveAll(deleteExpression);
                }

                if (itemsToDelete?.Any() ?? false)
                {
                    OnNotifyListChanged(new NotifyListChangedEventArgs<T>
                    {
                        ListChangedType = ListChangedNotifyTypes.Deleted,
                        Items = itemsToDelete
                    });
                }
            }
            catch
            {
                //Hp --> Logic: Reset the list with original data source.
                throw;
            }
            finally
            {
                //Hp --> Logic: While removing items, always clear all items and add filtered items.
                filteredItems = PrepareSortFilterQuery();

                //Hp --> Logic: Updated list with filtered items without notifying the user.
                AddItems(filteredItems, isNotifyUser, notifyListType: ListChangedType.ItemDeleted);
            }
        }

        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="grpFilter">The GRP filter.</param>
        protected void ExecuteFilter(FilterGroup grpFilter)
        {
            //Hp --> Logic: Check whether given filter mactches with existing one?
            if (Filter?.Equals(grpFilter) ?? false)
            {
                return;
            }

            Filter = grpFilter;
            AddWithFilter();
            OnFilterChanged(new FilterChangedEventArgs(true));
        }

        /// <summary>
        /// Raises the <see cref="E:FilterChanged" /> event.
        /// </summary>
        /// <param name="eArgs">The <see cref="FilterChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFilterChanged(FilterChangedEventArgs eArgs)
        {
            //Hp --> Logic: To handle cross thread exception, marshal the event invocations 
            //onto the UI thread.
            if (null == syncObject)
            {
                RaiseFilterChanged(eArgs);
            }
            else
            {
                Action<FilterChangedEventArgs> raiseOnFilterChanged = RaiseFilterChanged;
                syncObject?.Invoke(raiseOnFilterChanged, new object[] { eArgs });
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SortChanged" /> event.
        /// </summary>
        /// <param name="eArgs">The <see cref="SortChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSortChanged(SortChangedEventArgs eArgs)
        {
            //Hp --> Logic: To handle cross thread exception, marshal the event invocations 
            //onto the UI thread.
            if (null == syncObject)
            {
                RaiseSortChanged(eArgs);
            }
            else
            {
                Action<SortChangedEventArgs> raiseOnSortChanged = RaiseSortChanged;
                syncObject?.Invoke(raiseOnSortChanged, new object[] { eArgs });
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NotifyListChanged" /> event.
        /// </summary>
        /// <param name="eArgs">The <see cref="NotifyListChangedEventArgs{T}"/> instance containing the event data.</param>
        protected virtual void OnNotifyListChanged(NotifyListChangedEventArgs<T> eArgs)
        {
            //Hp --> Logic: To handle cross thread exception, marshal the event invocations 
            //onto the UI thread.
            if (null == syncObject)
            {
                RaiseNotifyListChanged(eArgs);
            }
            else
            {
                Action<NotifyListChangedEventArgs<T>> raiseOnNotifyListChanged
                    = RaiseNotifyListChanged;

                syncObject?.Invoke(raiseOnNotifyListChanged, new object[] { eArgs });
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Determines whether [is valid filter] [the specified filter].
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///   <c>true</c> if [is valid filter] [the specified filter]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidFilter(FilterGroup filter)
        {
            return ((null != filter) &&
                (GetAllFlatternFilters(filter).Any(G => G.Filters?.Any() ?? false)));
        }

        /// <summary>
        /// Gets all flattern filters.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        private IEnumerable<FilterGroup> GetAllFlatternFilters(FilterGroup filter)
        {
            var children = filter.SubGroups ?? new List<FilterGroup>();
            return (children.SelectMany(GetAllFlatternFilters).Concat(new[] { filter }));
        }

        /// <summary>
        /// Raises the list changed.
        /// </summary>
        /// <param name="args">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
        private void RaiseListChanged(ListChangedEventArgs args)
        {
            base.OnListChanged(args);
        }

        /// <summary>
        /// Raises the filter changed.
        /// </summary>
        /// <param name="args">The <see cref="FilterChangedEventArgs" /> instance containing the event data.</param>
        private void RaiseFilterChanged(FilterChangedEventArgs args)
        {
            FilterChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the sort changed.
        /// </summary>
        /// <param name="args">The <see cref="SortChangedEventArgs"/> instance containing the event data.</param>
        private void RaiseSortChanged(SortChangedEventArgs args)
        {
            SortChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the notify list changed.
        /// </summary>
        /// <param name="args">The <see cref="NotifyListChangedEventArgs{T}"/> instance containing the event data.</param>
        private void RaiseNotifyListChanged(NotifyListChangedEventArgs<T> args)
        {
            NotifyListChanged?.Invoke(this, args);
        }

        #endregion
    }
}
