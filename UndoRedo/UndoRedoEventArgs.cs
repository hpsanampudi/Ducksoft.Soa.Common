using System;

namespace Ducksoft.SOA.Common.UndoRedo
{
    /// <summary>
    /// Class which holds the events arguments related to undo/redo functionality.
    /// </summary>
    public class UndoRedoEventArgs<T> : EventArgs where T : class
    {
        /// <summary>
        /// Gets the current data.
        /// </summary>
        /// <value>
        /// The current data.
        /// </value>
        public T CurrentData { get; private set; }

        /// <summary>
        /// Gets the type of the action.
        /// </summary>
        /// <value>
        /// The type of the action.
        /// </value>
        public UndoRedoStateTypes ActionType { get; private set; }

        /// <summary>
        /// Gets the undo count.
        /// </summary>
        /// <value>
        /// The undo count.
        /// </value>
        public int UndoCount { get; private set; }

        /// <summary>
        /// Gets the redo count.
        /// </summary>
        /// <value>
        /// The redo count.
        /// </value>
        public int RedoCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is undo allowed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is undo allowed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsUndoAllowed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is redo allowed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is redo allowed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsRedoAllowed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoEventArgs{T}"/> class.
        /// </summary>
        /// <param name="data">The current data.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="undoCount">The undo count.</param>
        /// <param name="redoCount">The redo count.</param>
        /// <param name="isUndoAllowed">if set to <c>true</c> [is undo allowed].</param>
        /// <param name="isRedoAllowed">if set to <c>true</c> [is redo allowed].</param>
        public UndoRedoEventArgs(T data, UndoRedoStateTypes actionType, int undoCount,
            int redoCount, bool isUndoAllowed, bool isRedoAllowed)
        {
            CurrentData = data;
            ActionType = actionType;
            UndoCount = undoCount;
            RedoCount = redoCount;
            IsUndoAllowed = isUndoAllowed;
            IsRedoAllowed = isRedoAllowed;
        }
    }
}
