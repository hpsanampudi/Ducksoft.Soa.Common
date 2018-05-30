using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;

namespace ClientManager.Module.Helpers
{
    /// <summary>
    /// Custom generic list collection to enumerate each record forward and backward.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomNavigationList<T> : IList<T>, IEnumerator<T>
    {
        /// <summary>
        /// Stores the generic list collection internally.
        /// </summary>
        private readonly IList<T> innerList = new List<T>();

        /// <summary>
        /// Gets the index of the current.
        /// </summary>
        /// <value>
        /// The index of the current enumerator item.
        /// </value>
        public int CurrentIndex { get; private set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNavigationList&lt;T&gt;"/> class.
        /// </summary>
        public CustomNavigationList()
        {
            Reset();
            innerList.Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNavigationList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public CustomNavigationList(IEnumerable<T> collection)
            : this()
        {
            innerList = new List<T>(collection);
        }
        #endregion

        #region Interface: IList<T> implementation
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            ErrorBase.CheckArgIsNull(item, nameof(item));
            return (innerList.IndexOf(item));
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
        public void Insert(int index, T item)
        {
            ErrorBase.CheckArgIsNull(item, nameof(item));
            ErrorBase.CheckArgIsValid(index, nameof(index), (I => (I <= innerList.Count)));
            innerList.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
        public void RemoveAt(int index)
        {
            ErrorBase.CheckArgIsValid(index, nameof(index), (I => (I < innerList.Count)));
            innerList.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
        public T this[int index]
        {
            get
            {
                return (innerList[index]);
            }
            set
            {
                innerList[index] = value;
            }
        }
        #endregion

        #region Interface: ICollection<T> implementation
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add(T item)
        {
            ErrorBase.CheckArgIsNull(item, nameof(item));
            innerList.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear()
        {
            innerList.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            ErrorBase.CheckArgIsNull(item, nameof(item));
            return (innerList.Contains(item));
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ErrorBase.CheckArgIsNull(array, nameof(array));
            ErrorBase.CheckArgIsValid(arrayIndex, nameof(arrayIndex), (I => (I < array.Length)));
            innerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get { return (innerList.Count); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return (innerList.IsReadOnly); }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(T item)
        {
            ErrorBase.CheckArgIsNull(item, nameof(item));
            return (innerList.Remove(item));
        }
        #endregion

        #region Interface: IEnumerable implementation
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            //return IEnumerator of our Custom Type
            return (this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }
        #endregion

        #region Interface: IEnumerator<T> implementation
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        public T Current
        {
            get
            {
                return (((0 <= CurrentIndex) && (CurrentIndex < innerList.Count)) ?
                         innerList[CurrentIndex] : default(T));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        object System.Collections.IEnumerator.Current
        {
            get { return (Current); }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public virtual bool MoveNext()
        {
            CurrentIndex++;
            return (CurrentIndex < innerList.Count);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public void Reset()
        {
            CurrentIndex = -1;
        }

        public bool SetCurrentIndex(int index)
        {
            bool isIndexExists = ((0 <= index) && (index < innerList.Count));
            if (isIndexExists)
            {
                CurrentIndex = index;
            }
            return (isIndexExists);
        }

        #endregion

        #region Interface: IDisposable implementation
        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects). 
                Reset();
                innerList.Clear();
            }
            // Free your own state (unmanaged objects).

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="XlsWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        ~CustomNavigationList()
        {
            Dispose(false);
        }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Moves the back.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveBack()
        {
            CurrentIndex--;
            return (0 <= CurrentIndex);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the array collection items.
        /// </summary>
        /// <param name="arrayCollection">The array collection.</param>
        public void AddRange(T[] arrayCollection)
        {
            ErrorBase.CheckArgIsNull(arrayCollection, nameof(arrayCollection));
            foreach (T item in arrayCollection)
            {
                innerList.Add(item);
            }
        }
        #endregion
    }
}
