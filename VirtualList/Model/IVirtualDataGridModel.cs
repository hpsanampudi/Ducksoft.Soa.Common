using Ducksoft.Soa.Common.VirtualList.View;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ducksoft.Soa.Common.VirtualList.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVirtualDataGridModel<T> where T : class
    {
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        IVirtualDataGridView<T> View { get; set; }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        VirtualBindingList<T> DataStore { get; }

        /// <summary>
        /// Gets the type of the data object.
        /// </summary>
        /// <value>
        /// The type of the data object.
        /// </value>
        Type DataObjectType { get; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        DataGridViewColumn[] Columns { get; set; }

        /// <summary>
        /// Gets the sorted columns.
        /// </summary>
        /// <value>
        /// The sorted columns.
        /// </value>
        IEnumerable<DataGridViewColumn> SortedColumns { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether status bar is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> when status bar visible; otherwise, <c>false</c>.
        /// </value>
        bool IsStatusBarVisible { get; set; }

        /// <summary>
        /// Automatically maps the column headers.
        /// </summary>
        void AutoMapColumnHeaders();

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        DataGridViewColumn GetColumn(string columnName);

        /// <summary>
        /// Sorts the columns by name.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="columnNames">The column names.</param>
        void SortColumnsBy(SortOrder order, params string[] columnNames);

        /// <summary>
        /// Removes the sort glyph.
        /// </summary>
        void RemoveSortGlyph();

        /// <summary>
        /// Shows or hide columns by name.
        /// </summary>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        /// <param name="columnNames">The column names.</param>
        void ShowOrHideColumnsBy(bool isReset, bool isShow, params string[] columnNames);

        /// <summary>
        /// Shows or hide all columns.
        /// </summary>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        void ShowOrHideAllColumns(bool isShow);
    }
}