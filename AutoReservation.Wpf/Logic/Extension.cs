﻿using System;
using System.Reflection;

namespace AutoReservation.Wpf.Logic {
    public static class Extension {
        /// <summary>
        ///     Replace properties of an object with the properties of another object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void Replace(this object target, object source) {
            if (target.GetType() == source.GetType()) {
                PropertyInfo[] properties = target.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in properties) {
                    propertyInfo.SetValue(target, propertyInfo.GetValue(source));
                }
            } else {
                throw new ArgumentException("Target and Source are not of the same type.");
            }
        }

        /// <summary>
        ///     Replace properties of an object with the properties of another object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        public static void Replace(this object target, object source, Type type) {
            if (target.GetType().IsSubclassOf(type) && source.GetType().IsSubclassOf(type)) {
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo propertyInfo in properties) {
                    propertyInfo.SetValue(target, propertyInfo.GetValue(source));
                }
            } else {
                throw new ArgumentException("Target(\"" + target.GetType().Name +
                                            "\") or Source(\"" + source.GetType().Name +
                                            "\") is not subclass of defined type (\"" + type.Name + "\").");
            }
        }
    }
}