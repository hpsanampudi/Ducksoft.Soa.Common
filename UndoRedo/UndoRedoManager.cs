using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;

namespace Ducksoft.SOA.Common.UndoRedo
{
    /// <summary>
    /// Class which is used to provide undo/redo functionality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoRedoManager<T> where T : class
    {
        /// <summary>
        /// The undo stack
        /// </summary>
        private readonly Stack<T> undoStack;

        /// <summary>
        /// The redo stack
        /// </summary>
        private readonly Stack<T> redoStack;

        /// <summary>
        /// The current data
        /// </summary>
        private T currentData;

        /// <summary>
        /// Gets or sets a value indicating whether [is busy].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is busy]; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Occurs when [on undo redo action changed].
        /// </summary>
        public event EventHandler<UndoRedoEventArgs<T>> OnUndoRedoActionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoManager{T}" /> class.
        /// </summary>
        public UndoRedoManager()
        {
            undoStack = new Stack<T>();
            redoStack = new Stack<T>();
            currentData = null;
            IsBusy = false;
        }

        /// <summary>
        /// Clears all undo redo cache.
        /// </summary>
        public void ClearAllUndoRedoCache()
        {
            undoStack.Clear();
            redoStack.Clear();
            currentData = null;

            //Hp --> Logic: Free unreferenced memory
            GC.Collect();
        }

        /// <summary>
        /// Saves the current data.
        /// </summary>
        /// <param name="copyOfCurrentData">The copy of current data. 
        /// Note: Donot pass current data directly, always pass copy of current data.</param>
        public void SaveCurrentData(T copyOfCurrentData)
        {
            ErrorBase.CheckArgIsNull(copyOfCurrentData, () => copyOfCurrentData);
            if (IsBusy)
            {
                //Hp --> Logic: To avoid duplicate requests from UI while updating current data.
                return;
            }

            if (null == currentData)
            {
                //Hp --> Logic: Record current data to local variable to reduce complexity.                
                currentData = copyOfCurrentData;

                //Hp --> Logic: First time we do not want to record to undo stack.
                return;
            }

            //Hp --> Logic: Whenever user ask to save data, push original data to undo stack.
            undoStack.Push(currentData);

            //Hp --> Note: The below assignment is pass by reference.
            currentData = copyOfCurrentData;
            if (null != OnUndoRedoActionChanged)
            {
                OnUndoRedoActionChanged.Invoke(this, new UndoRedoEventArgs<T>(
                    currentData, UndoRedoStateTypes.None, undoStack.Count,
                    redoStack.Count, (0 != undoStack.Count),
                    (0 != redoStack.Count)));
            }
        }

        /// <summary>
        /// Undoes the current data.
        /// </summary>
        public void UndoCurrentData()
        {
            ErrorBase.Require(null != currentData);
            ErrorBase.Require(0 < undoStack.Count);

            //Hp --> Logic: Before collecting the undo data, push current data to redo stack.
            redoStack.Push(currentData);

            //Hp --> Logic: Free unreferenced memory
            GC.Collect();

            //Hp --> Logic: Read current undo data from stack
            currentData = undoStack.Pop();
            if (null != OnUndoRedoActionChanged)
            {
                OnUndoRedoActionChanged.Invoke(this, new UndoRedoEventArgs<T>(
                    currentData, UndoRedoStateTypes.Undo, undoStack.Count,
                    redoStack.Count, (0 != undoStack.Count),
                    (0 != redoStack.Count)));
            }
        }

        /// <summary>
        /// Redoes the current data.
        /// </summary>
        public void RedoCurrentData()
        {
            ErrorBase.Require(null != currentData);
            ErrorBase.Require(0 < redoStack.Count);

            //Hp --> Logic: Before collecting the redo data, push current data to undo stack.
            undoStack.Push(currentData);

            //Hp --> Logic: Free unreferenced memory
            GC.Collect();

            //Hp --> Logic: Read current redo data from stack
            currentData = redoStack.Pop();
            if (null != OnUndoRedoActionChanged)
            {
                OnUndoRedoActionChanged.Invoke(this, new UndoRedoEventArgs<T>(
                    currentData, UndoRedoStateTypes.Redo, undoStack.Count,
                    redoStack.Count, (0 != undoStack.Count),
                    (0 != redoStack.Count)));
            }
        }
    }
}
