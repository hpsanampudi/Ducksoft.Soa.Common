using Ducksoft.Soa.Common.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Ducksoft.Soa.Common.Configuration
{
    /// <summary>
    /// Generic configuration file collection for storing multiple items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Configuration.ConfigurationElementCollection" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    public class ConfigElementCollectionBase<T> : ConfigurationElementCollection, IEnumerable<T>
        where T : ConfigurationElement, new()
    {
        /// <summary>
        /// The property key information
        /// </summary>
        protected readonly PropertyInfo PropKeyInfo;

        /// <summary>
        /// The property key name
        /// </summary>
        protected readonly string PropKeyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigElementCollectionBase{T}"/> class.
        /// </summary>
        public ConfigElementCollectionBase()
        {
            PropKeyInfo = typeof(T)
                .GetAllProperties<ConfigurationPropertyAttribute>(A => A.IsKey)
                .SingleOrDefault();

            PropKeyName = PropKeyInfo.GetPropertyName();
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        /// A newly created <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement() => new T();

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element) =>
            ((T)element).GetPropertyValue(PropKeyName);

        /// <summary>
        /// Gets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="T"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return (T)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Adds the specified custom element.
        /// </summary>
        /// <param name="customElement">The custom element.</param>
        public void Add(T customElement) => BaseAdd(customElement);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear() => BaseClear();

        /// <summary>
        /// Removes the specified custom element.
        /// </summary>
        /// <param name="customElement">The custom element.</param>
        public void Remove(T customElement) => BaseRemove(GetElementKey(customElement));

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index) => BaseRemoveAt(index);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<T> GetEnumerator()
        {
            var count = base.Count;
            for (int index = 0; index < count; index++)
            {
                yield return (BaseGet(index) as T);
            }
        }
    }
}
