//--------------------------------------------------------------------------
// File:    TypeHelper.cs
// Content:	Implementation of class TypeHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Provides common type helper methods.
	/// </summary>
	public static class TypeHelper
    {
        #region DeepClone helper methods

        // Konfiguration für JSON-Serialisierung
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            // Erlaubt Referenz-Zyklen und behält Objekt-Referenzen bei
            ReferenceHandler = ReferenceHandler.Preserve,

            // Schließt Felder mit ein (nicht nur Properties)
            IncludeFields = true,

            // Ignoriert schreibgeschützte Properties nicht
            IgnoreReadOnlyProperties = false,

            // Behandelt Zahlen als Strings wenn nötig
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // Erlaubt Kommentare in JSON
            ReadCommentHandling = JsonCommentHandling.Skip,

            // Behandelt unbekannte Properties
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,

            // Konfiguriert Groß-/Kleinschreibung
            PropertyNamingPolicy = null,

            // Erlaubt nachgestellte Kommata
            AllowTrailingCommas = true
        };

        /// <summary>
        /// Creates a deep copy of an object using JSON serialization
        /// </summary>
        /// <param name="original">The object to be cloned</param>
        /// <param name="type">The type of object to be cloned</param>
        /// <returns>A deep copy of the original object</returns>
        public static object? DeepClone(object? original, Type type)
        {
            if (original == null)
                return null;

            //Type originalType = original.GetType();
            Type originalType = type ?? original.GetType();

            try
            {
                // Serialize the object to JSON
                string jsonString = JsonSerializer.Serialize(original, originalType, _jsonOptions);

                // Deserialize back to the original type
                return JsonSerializer.Deserialize(jsonString, originalType, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Deep Clone von Typ {originalType.Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a deep copy of an object using JSON serialization
        /// </summary>
        /// <typeparam name="T">The type of object to be cloned</typeparam>
        /// <param name="original">The object to be cloned</param>
        /// <returns>A deep copy of the original object</returns>
        public static T? DeepClone<T>(T? original)
        {
            return (T?)DeepClone(original, typeof(T));

            //if (typeof(T).IsValueType == false)
            //{
            //    if (Equals(original, default(T))) return default;
            //}

            //try
            //{
            //    // Serialisiere das Objekt zu JSON
            //    string jsonString = JsonSerializer.Serialize(original, _jsonOptions);

            //    // Deserialisiere zurück zum ursprünglichen Typ
            //    return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidOperationException($"Fehler beim Deep Clone von Typ {typeof(T).Name}: {ex.Message}", ex);
            //}
        }

        #endregion

        #region Dispose helper methods

        /// <summary>
        /// Disposes the specified obj if the object has implemented <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="obj">The object that should be disposed.</param>
        public static void SafeDispose(object? obj)
        {
            switch (obj)
            {
                case null:
                    return;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
                default:
                    // Handle COM objects
                    if (Marshal.IsComObject(obj))
                    {
                        try
                        {
#pragma warning disable CA1416 // Validate platform compatibility
                            Marshal.ReleaseComObject(obj);
#pragma warning restore CA1416 // Validate platform compatibility
                        }
                        catch
                        {
                            // Silently ignore COM release errors
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Safely disposes all elements in a sequence that implement <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="sequence">The sequence containing potentially disposable objects</param>
        public static void SafeDisposeAll(IEnumerable? sequence)
        {
            if (sequence is null) return;

            foreach (var item in sequence)
            {
                SafeDispose(item);
            }
        }

        /// <summary>
        /// Disposes the values of a dictionary if the type of the values implement <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="dict">The dictionary how's values should be disposed.</param>
        [DebuggerStepThrough]
        public static void SafeDisposeAllDictionaryValues(IDictionary? dict)
        {
            if (dict is null) return;

            foreach (var item in dict.Values)
            {
                SafeDispose(item);
            }
        }

        #endregion

    }
}
