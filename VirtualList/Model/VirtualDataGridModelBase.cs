using Ducksoft.Soa.Common.Filters;
using Ducksoft.Soa.Common.Utilities;
using Ducksoft.Soa.Common.VirtualList.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Ducksoft.Soa.Common.VirtualList.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Ducksoft.Soa.Common.VirtualList.Model.IVirtualDataGridModel{T}" />
    /// <seealso cref="FA.CSAS.Common.Utilities.NotifyPropertyBase" />
    public abstract class VirtualDataGridModelBase<T> : NotifyPropertyBase, IVirtualDataGridModel<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public IVirtualDataGridView<T> View
        {
            get { return (GetPropertyValue<IVirtualDataGridView<T>>()); }
            set { SetPropertyValue(value); }
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public VirtualBindingList<T> DataStore { get; private set; }

        /// <summary>
        /// Gets the type of the data object.
        /// </summary>
        /// <value>
        /// The type of the data object.
        /// </value>
        public Type DataObjectType { get; private set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public DataGridViewColumn[] Columns
        {
            get { return (GetPropertyValue<DataGridViewColumn[]>()); }
            set
            {
                var modifiedColumns = value;
                SetPropertyValue(modifiedColumns);
                if ((null != modifiedColumns) && (!modifiedColumns.Any(C => (null == C))))
                {
                    View?.MapColumnHeaders();
                }
            }
        }

        /// <summary>
        /// Gets the sorted columns.
        /// </summary>
        /// <value>
        /// The sorted columns.
        /// </value>
        public IEnumerable<DataGridViewColumn> SortedColumns
        {
            get
            {
                return (Columns?.Cast<DataGridViewColumn>()
                    .Where(C => (C.HeaderCell.SortGlyphDirection != SortOrder.None)));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return (GetPropertyValue<bool>()); }
            set { SetPropertyValue(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether status bar is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> when status bar visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsStatusBarVisible
        {
            get { return (GetPropertyValue<bool>()); }
            set { SetPropertyValue(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDataGridModelBase"/> class.
        /// </summary>
        public VirtualDataGridModelBase() : this(new List<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDataGridModelBase" /> class.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        public VirtualDataGridModelBase(IEnumerable<T> rawData) : this(rawData, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDataGridModelBase" /> class.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="isFilterDuplicates">if set to <c>true</c> [is filter duplicates].</param>
        public VirtualDataGridModelBase(IEnumerable<T> rawData, bool isFilterDuplicates)
            : this(new VirtualBindingList<T>(rawData, isFilterDuplicates))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDataGridModelBase{T}"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        public VirtualDataGridModelBase(VirtualBindingList<T> dataStore)
        {
            DataStore = dataStore ?? new VirtualBindingList<T>();
            DataObjectType = typeof(T);

            //TODO: Hp --> Always supress the event and register since we are changing the model.
            DataStore.ListChanged += DataStore_ListChanged;
            DataStore.FilterChanged += DataStore_FilterChanged;
            DataStore.SortChanged += DataStore_SortChanged;
            DataStore.NotifyListChanged += DataStore_NotifyListChanged;
        }

        /// <summary>
        /// Called when [items deleted].
        /// </summary>
        /// <param name="deletedItems">The deleted items.</param>
        public abstract void OnItemsDeleted(IEnumerable<T> deletedItems);

        /// <summary>
        /// Handles the ListChanged event of the DataStore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
        private void DataStore_ListChanged(object sender, ListChangedEventArgs e)
        {
            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.Reset:
                        break;

                    case ListChangedType.ItemAdded:
                        {
                            if ((-1 == e.OldIndex) && (-1 == e.NewIndex))
                            {
                                //Hp --> Logic: Don't reset grid items, as we are adding bulk rows.
                                View?.RefreshGrid(true, isReset: false);
                            }
                        }
                        break;
                    case ListChangedType.ItemDeleted:
                        {
                            if ((-1 == e.OldIndex) && (-1 == e.NewIndex))
                            {
                                //Hp --> Logic: Reset grid items, as we are removing bulk rows.
                                View?.RefreshGrid(true, isReset: true);
                            }
                        }
                        break;
                    case ListChangedType.ItemMoved:
                        break;
                    case ListChangedType.ItemChanged:
                        break;
                    case ListChangedType.PropertyDescriptorAdded:
                        break;
                    case ListChangedType.PropertyDescriptorDeleted:
                        break;
                    case ListChangedType.PropertyDescriptorChanged:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ApplicationException(errMessage));
            }
        }

        /// <summary>
        /// Handles the FilterChanged event of the DataStore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FilterChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void DataStore_FilterChanged(object sender, FilterChangedEventArgs e)
        {
            try
            {
                View?.RefreshGrid(true, e?.IsReset ?? true);
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ApplicationException(errMessage));
            }
        }

        /// <summary>
        /// Handles the SortChanged event of the DataStore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SortChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void DataStore_SortChanged(object sender, SortChangedEventArgs e)
        {
            try
            {
                if (e?.IsRefresh ?? false)
                {
                    RemoveSortGlyph();
                    View?.RefreshGrid(false);
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ApplicationException(errMessage));
            }
        }

        /// <summary>
        /// Handles the NotifyListChanged event of the DataStore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyListChangedEventArgs{T}"/> instance containing the event data.</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void DataStore_NotifyListChanged(object sender, NotifyListChangedEventArgs<T> e)
        {
            ErrorBase.CheckArgIsNull(e, () => e);

            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedNotifyTypes.Added:
                        break;

                    case ListChangedNotifyTypes.Deleted:
                        {
                            if (!e.Items?.Any() ?? true)
                            {
                                return;
                            }

                            OnItemsDeleted(e.Items);
                        }
                        break;

                    case ListChangedNotifyTypes.Updated:
                        break;

                    case ListChangedNotifyTypes.None:
                    default:
                        {
                            var errMessage = $"The given {e.ListChangedType} is not handled!";
                            throw (new NotImplementedException(errMessage));
                        }
                }


            }
            catch (Exception ex)
            {
                var errMessage = string.Join(Environment.NewLine, ex.Messages());
                throw (new ApplicationException(errMessage));
            }
        }

        /// <summary>
        /// Automatically maps the column headers.
        /// </summary>
        public void AutoMapColumnHeaders()
        {
            if (Utility.IsDesignMode)
            {
                return;
            }

            var rawColumns = DataObjectType.GetAllProperties().Select(p => new
            {
                PropName = p.Name,
                DisplayName = p.GetAttributeValue<DisplayNameAttribute, string>(A => A.DisplayName),
                DataType = p.PropertyType
            });

            var colIndex = 0;
            int columnsCount = rawColumns.Count();
            Columns = new DataGridViewColumn[columnsCount];

            foreach (var item in rawColumns)
            {
                var column = default(DataGridViewColumn);
                var columnName = item.PropName;
                var displayName = item.DisplayName;
                var dataType = item.DataType;

                displayName = (string.IsNullOrWhiteSpace(displayName) ?
                    columnName.SplitCamelCase() : displayName).Trim();

                if (typeof(bool) == dataType)
                {
                    column = new DataGridViewCheckBoxColumn
                    {
                        CellTemplate = new DataGridViewCheckBoxCell
                        {
                            ValueType = dataType,
                        },
                        ThreeState = false,
                    };
                }
                else if (dataType.IsEnum)
                {
                    column = new DataGridViewComboBoxColumn
                    {
                        CellTemplate = GetEnumCellTemplate(dataType, columnName),
                        DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                    };
                }
                else
                {
                    column = new DataGridViewTextBoxColumn
                    {
                        CellTemplate = new DataGridViewTextBoxCell
                        {
                            ValueType = dataType,
                        },
                    };
                }

                column.Name = columnName;
                column.HeaderText = displayName;
                column.DataPropertyName = columnName;
                column.ValueType = dataType;
                column.DefaultCellStyle = GetCellStyle(dataType, columnName);

                column.Visible = true;
                column.Frozen = false;
                column.ReadOnly = true;
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
                column.MinimumWidth = 100;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                Columns[colIndex] = column;
                colIndex++;
            }

            View?.MapColumnHeaders();
        }

        /// <summary>
        /// Gets the enum cell template.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        protected virtual DataGridViewComboBoxCell GetEnumCellTemplate(Type dataType,
            string columnName)
        {
            ErrorBase.CheckArgIsNull(dataType, () => dataType);
            ErrorBase.CheckArgIsNullOrDefault(columnName, () => columnName);

            var dataSource = new BindingList<EnumMapper>(Enum.GetValues(dataType)
                .Cast<Enum>()
                .Select(E => new EnumMapper(E))
                .ToList());

            return new DataGridViewComboBoxCell
            {
                DataSource = dataSource,
                DisplayMember = nameof(EnumMapper.Description),
                ValueMember = nameof(EnumMapper.Value),
                ValueType = dataType,
            };
        }

        /// <summary>
        /// Gets the cell style.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        protected virtual DataGridViewCellStyle GetCellStyle(Type dataType, string columnName)
        {
            ErrorBase.CheckArgIsNull(dataType, () => dataType);
            ErrorBase.CheckArgIsNullOrDefault(columnName, () => columnName);

            return (new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Format = (typeof(DateTime?) == dataType) ? "dd/MM/yyyy HH:mm:ss" : string.Empty,
                WrapMode = DataGridViewTriState.NotSet,
            });
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public DataGridViewColumn GetColumn(string columnName)
        {
            ErrorBase.CheckArgIsNullOrDefault(columnName, () => columnName);
            return (Columns?.SingleOrDefault(C => C.Name.IsEqualTo(columnName)));
        }

        /// <summary>
        /// Sorts the column by names.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="columnNames">The column names.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        public void SortColumnsBy(SortOrder order, params string[] columnNames)
        {
            ErrorBase.CheckArgIsNull(columnNames, () => columnNames);
            if (!Columns?.Any() ?? true)
            {
                return;
            }

            var columnsToSort = new List<DataGridViewColumn>();
            foreach (var item in columnNames)
            {
                var columnToSort = GetColumn(item);
                if (null == columnToSort)
                {
                    var errMessage =
                        $"The given {nameof(columnToSort).SplitCamelCase()} instance is null," +
                        $" in method {nameof(SortColumnsBy)}";

                    throw (new NullReferenceException(errMessage));
                }

                columnsToSort.Add(columnToSort);
            }

            //Hp --> Assumption: User can sort multiple columns only in one direction.
            var sortDirection = (SortOrder.Descending == order) ?
                ListSortDirection.Descending : ListSortDirection.Ascending;

            var currentSortedColumns = SortedColumns;
            if (currentSortedColumns?.Any() ?? false)
            {
                foreach (var item in currentSortedColumns)
                {
                    item.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }

            //Hp --> Logic: Set sort glyph image to columns to sort.
            columnsToSort.ForEach(item =>
            item.HeaderCell.SortGlyphDirection = (ListSortDirection.Ascending == sortDirection) ?
            SortOrder.Ascending : SortOrder.Descending);

            //Hp --> Logic: Create sort descriptions for based on given sort columns.
            var sortDescriptions = columnNames.ToList()
                .Select(colName => new ListSortDescription(
                    DataObjectType.GetProperty(colName).PropertyDescriptor(), sortDirection))
                    .ToArray();

            DataStore?.ApplySort(new ListSortDescriptionCollection(sortDescriptions));
        }

        /// <summary>
        /// Removes the sort glyph.
        /// </summary>
        public void RemoveSortGlyph()
        {
            var currentSortedColumns = SortedColumns;
            if ((currentSortedColumns?.Any() ?? false) && (!DataStore?.IsSorted ?? false))
            {
                foreach (var item in currentSortedColumns)
                {
                    item.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
        }

        /// <summary>
        /// Shows or hide columns by names.
        /// </summary>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        /// <param name="columnNames">The column names.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        public void ShowOrHideColumnsBy(bool isReset, bool isShow, params string[] columnNames)
        {
            ErrorBase.CheckArgIsNull(columnNames, () => columnNames);
            if (!Columns?.Any() ?? true)
            {
                return;
            }

            var columnsToPerform = new List<DataGridViewColumn>();
            foreach (var item in columnNames)
            {
                var columnToHide = GetColumn(item);
                if (null == columnToHide)
                {
                    var errMessage =
                        $"The given {nameof(columnToHide).SplitCamelCase()} instance is null," +
                        $" in method {nameof(SortColumnsBy)}";

                    throw (new NullReferenceException(errMessage));
                }

                columnsToPerform.Add(columnToHide);
            }

            if (isReset)
            {
                ShowOrHideAllColumns(true);
            }

            columnsToPerform.ForEach(item => item.Visible = isShow);
        }

        /// <summary>
        /// Shows the or hide all columns.
        /// </summary>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        public void ShowOrHideAllColumns(bool isShow)
        {
            if (!Columns?.Any() ?? true)
            {
                return;
            }

            foreach (var item in Columns.Cast<DataGridViewColumn>())
            {
                item.Visible = isShow;
            }
        }
    }
}
