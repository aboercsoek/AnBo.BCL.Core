//--------------------------------------------------------------------------
// File:    ObjectExtensions.cs
// Content:	Implementation of class ObjectExtensions
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
using System.Collections;

namespace AnBo.Core
{
    ///<summary>Fluent <see cref="Object"/> and <see cref="Type"/> Extensions.</summary>
	public static class ObjectEx
    {
        /// <summary>
        /// Fluent version of C# "as" keyword
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="item">value to be casted</param>
        /// <returns>casted value</returns>
        //public static T? As<T>(this object item)
        //{
        //    if (item == null)
        //        return default(T);

        //    if (item is T)
        //        return (T)item;

        //    return default(T);
        //}

        public static Nullable<T> AsValue<T>(this object? item) where T : struct
        {
            // Prüft, ob 'item' mit dem Typ 'T' kompatibel ist.
            // Wenn ja, wird 'result' der umgewandelte Wert zugewiesen und zurückgegeben.
            if (item is T result)
            {
                return result;
            }
            // Wenn die Umwandlung nicht möglich ist, gib den Standardwert zurück
            // (was 'null' für alle Referenztypen und nullable Wertetypen ist).
            return null;
        }
        

        /// <summary>
        /// Fluent version of C# "as" keyword
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="item">value to be casted</param>
        /// <returns>casted value</returns>
        public static T? AsUniversal<T>(this object? item)
        {
            // Prüft, ob 'item' mit dem Typ 'T' kompatibel ist.
            // Wenn ja, wird 'result' der umgewandelte Wert zugewiesen und zurückgegeben.
            if (item is T result)
            {
                return result;
            }

            // Wenn die Umwandlung nicht möglich ist, gib den Standardwert zurück
            // (was 'null' für alle Referenztypen und nullable Wertetypen ist).
            return default(T);
        }

        /// <summary>
        /// Fluent version of C# "as" keyword applied to sequences.
        /// </summary>
        /// <typeparam name="TSource">Source sequence item type to cast from</typeparam>
        /// <typeparam name="TTarget">Target sequence item type to cast to</typeparam>
        /// <param name="source">The source sequence</param>
        /// <returns>The casted target sequence.</returns>
        public static IEnumerable<TTarget> AsSequence<TSource, TTarget>(this IEnumerable<TSource> source)
        {
            if (source == null)
                yield break;

            //if (typeof(TTarget).IsAssignableFrom(typeof(TSource)) == false)
            //    yield break;

            foreach (var sourceItem in source)
            {
                if (sourceItem == null)
                    continue;

                var castedItem = sourceItem.AsUniversal<TTarget>();
                if (castedItem != null)
                {
                    yield return castedItem;
                }
            }
        }

        /// <summary>
        /// Fluent version of C# "as" keyword applied to sequences.
        /// </summary>
        /// <typeparam name="TTarget">Target sequence item type to cast to</typeparam>
        /// <param name="source">The source sequence</param>
        /// <returns>The casted target sequence.</returns>
        public static IEnumerable<TTarget> AsSequence<TTarget>(this IEnumerable source)
        {
            if (source == null)
                yield break;

            foreach (var sourceItem in source)
            {
                if (sourceItem == null)
                    continue;

                TTarget? item = sourceItem.AsUniversal<TTarget>();

                if (item.IsDefaultValue() && typeof(TTarget).IsValueType.IsFalse())
                    continue;

                yield return item!;
            }
        }

        /// <summary>
        /// Fluent version of type casts
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="item">value to be casted</param>
        /// <exception cref="InvalidCastException">Thrown when the item cannot be casted to the specified type.</exception>"
        /// <returns>casted value</returns>
        public static T? Cast<T>(this object? item)
        {
            if (item == null)
                return default(T);

            if (item is T result)
                return result;

            throw new InvalidCastException(string.Format("Can not cast from type {0} to {1}", item.GetType().Name, typeof(T).Name));
        }

        /// <summary>
        /// Fluent version of type casts for sequences
        /// </summary>
        /// <typeparam name="TSource">Source sequence item type to cast from</typeparam>
        /// <typeparam name="TTarget">Target sequence item type to cast to</typeparam>
        /// <param name="source">The source sequence</param>
        /// <returns>The casted target sequence.</returns>
        public static IEnumerable<TTarget> CastSequence<TSource, TTarget>(this IEnumerable<TSource> source)
        {
            if (source == null)
                yield break;

            foreach (var sourceItem in source)
            {
                if (sourceItem == null)
                    continue;

                yield return sourceItem.Cast<TTarget>();

            }
        }

        /// <summary>
        /// Creates an instance of the given type
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <returns>Instance of the given Type</returns>
        public static object? New(this Type? type)
        {
            if (type == null)
                return null;
            
            if (type == typeof(string))
                return string.Empty; // Special case for string to avoid Activator.CreateInstance throwing an exception

            return Activator.CreateInstance(type);
        }


        /// <summary>
        /// Creates an instance using the factory service.
        /// </summary>
        /// <typeparam name="T">Type to instantiate</typeparam>
        /// <returns>Instance of the given Type</returns>
        /// <remarks>
        /// <para>If T is one of the application service types, the method returns the requested application service.</para>
        /// <para>Supported AppService types: ILogManager, IFactoryService, IConfigService.</para>
        /// <para>For all other types AppContext.GetHostingAwareInstance().FactoryService.Retrieve{T}() is called.</para></remarks>
        //public static T Create<T>()
        //{

        //    if (typeof(T) == typeof(ILogManager))
        //        return (T)AppContext.GetHostingAwareInstance().LogManager;

        //    if (typeof(T) == typeof(ILogService))
        //        return (T)AppContext.GetHostingAwareInstance().LogService;
        //    if (typeof(T) == typeof(IFactoryService))
        //        return (T)AppContext.GetHostingAwareInstance().FactoryService;
        //    if (typeof(T) == typeof(IConfigService))
        //        return (T)AppContext.GetHostingAwareInstance().ConfigService;

        //    return AppContext.GetHostingAwareInstance().FactoryService != null ? AppContext.GetHostingAwareInstance().FactoryService.Retrieve<T>() : default(T);
        //}



        /// <summary>
        /// Creates an instance using the factory service.
        /// </summary>
        /// <typeparam name="T">Type to instantiate</typeparam>
        /// <param name="registrationName">Registration name in the factory service configuration.</param>
        /// <returns>Instance of the given Type</returns>
        /// <remarks>
        /// <para>If T is one of the application service types, the method returns the requested application service.</para>
        /// <para>Supported AppService types: ILogManager, IFactoryService, IConfigService.</para>
        /// <para>For all other types AppContext.GetHostingAwareInstance().FactoryService.Retrieve{T}(registrationName) is called.</para></remarks>
        //public static T Create<T>(string registrationName)
        //{
        //    if (registrationName.IsNullOrEmptyWithTrim())
        //        return Create<T>();

        //    return AppContext.GetHostingAwareInstance().FactoryService != null ? AppContext.GetHostingAwareInstance().FactoryService.Retrieve<T>(registrationName) : Create<T>();
        //}


        /// <summary>
        /// Calls the action delegate with control as paramter and return control after the action call.
        /// </summary>
        /// <typeparam name="T">Type of control object</typeparam>
        /// <param name="control">The control object.</param>
        /// <param name="action">The action to apply.</param>
        /// <returns>The control object after action(control) was called.</returns>
        public static T With<T>(this T control, Action<T> action)
        {
            action(control);
            return control;
        }

        /// <summary>
        /// Calls action(disposable) and ensures disposing of disposable after that call.
        /// </summary>
        /// <typeparam name="T">Type of the disposable object.</typeparam>
        /// <param name="disposable">The disposable object.</param>
        /// <param name="action">The action that should be applied to disposable object.</param>
        public static void WithDispose<T>(this T disposable, Action<T> action) where T : IDisposable
        {
            using (disposable)
            {
                action(disposable);
            }
        }

        /// <summary>
        /// Calls action for each item in <paramref name="sequence"/> and ensures disposing each <paramref name="sequence"/> item after action was called.
        /// </summary>
        /// <typeparam name="T">Type of the disposable <paramref name="sequence"/> item.</typeparam>
        /// <param name="sequence">The disposable items sequence.</param>
        /// <param name="action">The action that should be applied to each disposable item inside <paramref name="sequence"/>.</param>
        public static void WithDispose<T>(this IEnumerable<T> sequence, Action<T> action) where T : IDisposable
        {
            foreach (var item in sequence)
            {
                using (item)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Calls func(<paramref name="disposable"/>) and ensures disposing of <paramref name="disposable"/> after that call.
        /// </summary>
        /// <typeparam name="TDisposable">Type of the disposable object</typeparam>
        /// <typeparam name="TResult">Result type of func delegate.</typeparam>
        /// <param name="disposable">The disposable object.</param>
        /// <param name="func">The func delegate to apply on the disposable object.</param>
        /// <returns>Returns the result of func(<paramref name="disposable"/>)</returns>
        public static TResult WithDispose<TDisposable, TResult>(this TDisposable disposable, Func<TDisposable, TResult> func) where TDisposable : IDisposable
        {
            using (disposable)
            {
                return func(disposable);
            }
        }

        /// <summary>
        /// Calls func(<paramref name="disposable"/>, <paramref name="funcParam2"/>) and ensures disposing of <paramref name="disposable"/> after that call.
        /// </summary>
        /// <typeparam name="TDisposable">Type of the disposable object</typeparam>
        /// <typeparam name="TFunc2">Type of the second func parameter</typeparam>
        /// <typeparam name="TResult">Result type of func delegate.</typeparam>
        /// <param name="disposable">The disposable object.</param>
        /// <param name="funcParam2">The second func delegate argument.</param>
        /// <param name="func">The func delegate to apply.</param>
        /// <returns>Returns the result of func(<paramref name="disposable"/>, <paramref name="funcParam2"/>)</returns>
        public static TResult WithDispose<TDisposable, TFunc2, TResult>(this TDisposable disposable, TFunc2 funcParam2, Func<TDisposable, TFunc2, TResult> func) where TDisposable : IDisposable
        {
            using (disposable)
            {
                return func(disposable, funcParam2);
            }
        }

        /// <summary>
        /// <para>Disposes the specified object if the object has implemented <see cref="IDisposable"/>.</para>
        /// <para>If <paramref name="obj"/> has not implemented <see cref="IDisposable"/> or <paramref name="obj"/> is <see langword="null"/> nothing is done.</para>
        /// <para>If <paramref name="obj"/> is a COM-Object <see cref="Marshal.ReleaseComObject"/> is called.</para>
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void DisposeIfNecessary(this object obj)
        {
            TypeHelper.DisposeIfNecessary(obj);
        }

        /// <summary>
        /// <para>Disposes the elements of a sequence if the elements implemented <see cref="IDisposable"/>.</para>
        /// <para>If the <paramref name="sequence"/> items have not implemented <see cref="IDisposable"/> 
        /// or the <paramref name="sequence"/> is <see langword="null"/> nothing is done.</para>
        /// <para>If the <paramref name="sequence"/> items are COM-Objects <see cref="Marshal.ReleaseComObject"/> is called.</para>
        /// </summary>
        /// <param name="sequence">The sequence to dispose.</param>
        public static void DisposeElementsIfNecessary(this IEnumerable sequence)
        {
            TypeHelper.DisposeElementsIfNecessary(sequence);
        }

        /// <summary>
        /// Determines whether the specified instance is null. The method handels reference types and value types (value types always return false).
        /// </summary>
        /// <typeparam name="T">Type of the instance</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified instance is null; or <see langword="false"/> if T is a value type or T is not null.
        /// </returns>
        public static bool IsNull<T>(this T instance)
        {
            return instance is null;
        }

        /// <summary>
        /// Determines whether the specified instance is not null. The method handels reference types and value types (value types always return true).
        /// </summary>
        /// <typeparam name="T">Type of the instance</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified instance is a value type or not null; or <see langword="false"/> if T is null.
        /// </returns>
        public static bool IsNotNull<T>(this T instance)
        {
            return (instance.IsNull() == false);
        }

        /// <summary>
        /// Gets whether the <paramref name="value" /> is the default value for this reference or value type.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="value" /> is the default value for this reference or value type; or <see langword="false"/> not.
        /// </returns>
        public static bool IsDefaultValue(this object? value)
        {
            if (ReferenceEquals(value, null))
            {
                return true;
            }

            Type type = value.GetType();

            if (type.IsValueType.IsFalse())
            {
                return false;
            }
            return Equals(Activator.CreateInstance(type), value);
        }

        /// <summary>
        /// Gets whether the <paramref name="value" /> is the default value for its reference or value type, or an empty string.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="value" /> is the default value for this reference or value type, or an empty string; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsDefaultValueOrEmptyString(this object value)
        {
            if (ReferenceEquals(value, null))
            {
                return true;
            }

            Type type = value.GetType();

            if (type.IsValueType.IsFalse())
            {
                return ((value as string) == string.Empty);
            }
            return Equals(Activator.CreateInstance(type), value);
        }

    }
}
