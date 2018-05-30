using Ducksoft.SOA.Common.VirtualList.Model;

namespace Ducksoft.SOA.Common.VirtualList.View
{
    public interface IVirtualDataGridView<T> where T : class
    {
        /// <summary>
        /// Gets or sets the model binded to this view.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        IVirtualDataGridModel<T> Model { get; set; }

        /// <summary>
        /// Refreshes the grid.
        /// </summary>
        /// <param name="isForce">if set to <c>true</c> [is force].</param>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        void RefreshGrid(bool isForce, bool isReset = true);

        /// <summary>
        /// Maps the column headers.
        /// </summary>
        void MapColumnHeaders();
    }
}