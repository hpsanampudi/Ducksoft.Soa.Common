using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Base class which implements INotifyPropertyChanged interface.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class NotifyPropertyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The notified properties
        /// </summary>
        private Dictionary<string, object> notifiedProperties = new Dictionary<string, object>();

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns>T.</returns>
        protected T GetPropertyValue<T>(T defaultValue = default(T),
            [CallerMemberName]string propName = null)
        {
            ErrorBase.CheckArgIsNullOrDefault(propName, nameof(propName));
            object value = null;
            if (notifiedProperties.TryGetValue(propName, out value))
            {
                return ((null == value) ? defaultValue : (T)value);
            }

            return (defaultValue);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyValueHolder">The property value holder.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool SetPropertyValue<T>(T newValue, [CallerMemberName]string propName = null)
        {
            ErrorBase.CheckArgIsNullOrDefault(propName, nameof(propName));
            var srcValue = GetPropertyValue<T>(propName: propName);
            if (CanSkipAssignement(newValue, srcValue))
            {
                return (false);
            }

            notifiedProperties[propName] = newValue;
            RaisePropertyChanged(propName);
            return (true);
        }

        /// <summary>
        /// Determines whether this instance [can skip assignement] the specified old value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        private bool CanSkipAssignement(object oldValue, object newValue)
        {
            if (ReferenceEquals(oldValue, newValue))
            {
                return true;
            }
            if (oldValue is ValueType && newValue is ValueType && Equals(oldValue, newValue))
            {
                return true;
            }
            if (oldValue is string && newValue is string && Equals(oldValue, newValue))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
