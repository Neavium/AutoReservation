﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AutoReservation.Common.Extensions;

namespace AutoReservation.Common.DataTransferObjects
{
    /// <summary>
    /// The base of any Dto which provide <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    [Serializable] [DataContract]
    public abstract class AbstractDto : INotifyPropertyChanged
    {

        /// <summary>
        /// EventHandler for PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Calls observers when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// HelperFunction to set a property.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">The property to set.</param>
        /// <param name="value">The new value fot the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>True when property changed.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
            if (Equals(storage,value)) {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
