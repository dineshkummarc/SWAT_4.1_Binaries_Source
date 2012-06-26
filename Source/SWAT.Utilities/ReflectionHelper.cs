using System;
using System.Linq;
using System.Reflection;

namespace SWAT.Reflection
{
    /// <summary>
    /// This class contains helper methods for reflecting on hidden fields in your unit and integration tests.
    /// </summary>
    public static class ReflectionHelper
    {
        private const BindingFlags SearchFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// This static helper method will invoke a method on a class regardless of whether the method is located in
        /// the actual class or in a base class and return the result.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method</typeparam>
        /// <param name="obj">The object instance to invoke the method on.</param>
        /// <param name="methodName">The method to invoke.</param>
        /// <param name="args">Parameters to the method.</param>
        /// <returns>The result of invoking the method.</returns>
        public static TReturn InvokeMethod<TReturn>(this object obj, string methodName, params object[] args)
        {
            Type[] objArgTypes = args.Select(x => x.GetType()).ToArray();
            MethodInfo methodToInvoke = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                methodToInvoke = objType.GetMethod(methodName, SearchFlags, null, objArgTypes, null);
                if (methodToInvoke != null)
                {
                    break;
                }
            }

            if (methodToInvoke == null)
            {
                throw new MissingMethodException(obj.GetType().FullName, methodName);
            }

            try
            {
                return (TReturn)methodToInvoke.Invoke(obj, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// This static helper method will invoke a method on a class regardless of whether the method is located in
        /// the actual class or in a base class.
        /// </summary>
        /// <param name="obj">The object instance to invoke the method on.</param>
        /// <param name="methodName">The method to invoke.</param>
        /// <param name="args">Parameters to the method.</param>
        public static void InvokeMethod(this object obj, string methodName, params object[] args)
        {
            Type[] objArgTypes = args.Select(x => x.GetType()).ToArray();
            MethodInfo methodToInvoke = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                methodToInvoke = objType.GetMethod(methodName, SearchFlags, null, objArgTypes, null);
                if (methodToInvoke != null)
                {
                    break;
                }
            }

            if (methodToInvoke == null)
            {
                throw new MissingMethodException(obj.GetType().FullName, methodName);
            }

            try
            {
                methodToInvoke.Invoke(obj, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// This static helper method will set a field on an object instance regardless of whether the field
        /// is located in the actual object or in it's base class.
        /// </summary>
        /// <param name="obj">The object instance to set the field on.</param>
        /// <param name="fieldName">The name of the field to set.</param>
        /// <param name="value">The value to set the field to.</param>
        public static void SetField(this object obj, string fieldName, object value)
        {
            FieldInfo fieldToSet = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                fieldToSet = objType.GetField(fieldName, SearchFlags);
                if (fieldToSet != null)
                {
                    break;
                }
            }

            if (fieldToSet == null)
            {
                throw new MissingFieldException(obj.GetType().FullName, fieldName);
            }

            try
            {
                fieldToSet.SetValue(obj, value);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// This static helper method will retrieve the value of a field of an object instance regardless of whether it
        /// is located in the actual object or in a base class.
        /// </summary>
        /// <typeparam name="TReturn">The type of the field to retrieve.</typeparam>
        /// <param name="obj">The object instance to retrieve the field from.</param>
        /// <param name="fieldName">The field to retrieve.</param>
        /// <returns>The current value of the field.</returns>
        public static TReturn GetField<TReturn>(this object obj, string fieldName)
        {
            FieldInfo fieldToGet = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                fieldToGet = objType.GetField(fieldName, SearchFlags);
                if (fieldToGet != null)
                {
                    break;
                }
            }

            if (fieldToGet == null)
            {
                throw new MissingFieldException(obj.GetType().FullName, fieldName);
            }

            try
            {
                return (TReturn)fieldToGet.GetValue(obj);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// This static helper method will set the value of a property on an object regardless of whether this property is located
        /// in the object itself or in a base class.
        /// </summary>
        /// <param name="obj">The object to set the property on.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set on the property.</param>
        public static void SetProperty(this object obj, string propertyName, object value)
        {
            PropertyInfo propertyToSet = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                propertyToSet = objType.GetProperty(propertyName, SearchFlags);
                if (propertyToSet != null)
                {
                    break;
                }
            }

            if (propertyToSet == null)
            {
                throw new MissingFieldException(obj.GetType().FullName, propertyName);
            }

            try
            {
                propertyToSet.SetValue(obj, value, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// This static helper method will retrieve the value of a property on an object regardless of whether this property is located
        /// in the object itself or in a base class.
        /// </summary>
        /// <typeparam name="TReturn">The type of the property to retrieve.</typeparam>
        /// <param name="obj">The object to retrieve the property from.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value of the property.</returns>
        public static TReturn GetProperty<TReturn>(this object obj, string propertyName)
        {
            PropertyInfo propertyToGet = null;
            for (Type objType = obj.GetType(); objType != null; objType = objType.BaseType)
            {
                propertyToGet = objType.GetProperty(propertyName, SearchFlags);
                if (propertyToGet != null)
                {
                    break;
                }
            }

            if (propertyToGet == null)
            {
                throw new MissingFieldException(obj.GetType().FullName, propertyName);
            }

            try
            {
                return (TReturn) propertyToGet.GetValue(obj, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}
