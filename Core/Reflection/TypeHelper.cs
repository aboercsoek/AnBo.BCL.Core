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
        /// Erstellt eine tiefe Kopie eines Objekts (Object-Version für Kompatibilität)
        /// </summary>
        /// <param name="original">Das zu klonende Objekt</param>
        /// <returns>Eine tiefe Kopie des ursprünglichen Objekts</returns>
        public static object? DeepClone(object? original)
        {
            if (original == null)
                return null;

            Type originalType = original.GetType();

            try
            {
                // Serialisiere das Objekt zu JSON
                string jsonString = JsonSerializer.Serialize(original, originalType, _jsonOptions);

                // Deserialisiere zurück zum ursprünglichen Typ
                return JsonSerializer.Deserialize(jsonString, originalType, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Deep Clone von Typ {originalType.Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Erstellt eine tiefe Kopie eines Objekts mittels JSON-Serialisierung
        /// </summary>
        /// <typeparam name="T">Der Typ des zu klonenden Objekts</typeparam>
        /// <param name="original">Das zu klonende Objekt</param>
        /// <returns>Eine tiefe Kopie des ursprünglichen Objekts</returns>
        public static T? DeepClone<T>(T? original)
        {
            if (original == null)
                return default(T);

            try
            {
                // Serialisiere das Objekt zu JSON
                string jsonString = JsonSerializer.Serialize(original, _jsonOptions);

                // Deserialisiere zurück zum ursprünglichen Typ
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Deep Clone von Typ {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Dispose helper methods

        /// <summary>
        /// Disposes the specified obj if the object has implemented <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="obj">The object that should be disposed.</param>
        public static void DisposeIfNecessary(object? obj)
        {
            if (obj == null)
                return;

            if (obj.GetType().IsCOMObject)
#pragma warning disable CA1416 // Validate platform compatibility
                Marshal.ReleaseComObject(obj);
#pragma warning restore CA1416 // Validate platform compatibility
            else
            {
                if (obj is IDisposable id)
                    id.Dispose();
            }
        }

        /// <summary>
        /// Disposes the elements of the sequence if the elements implemented <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="enumerable">The sequence of elements that should be disposed.</param>
        [DebuggerStepThrough]
        public static void DisposeElementsIfNecessary(IEnumerable? enumerable)
        {
            if (enumerable == null)
                return;
            IEnumerator iter = enumerable.GetEnumerator();
            while (iter.MoveNext())
                DisposeIfNecessary(iter.Current);
        }

        /// <summary>
        /// Disposes the values of a dictionary if the type of the values implement <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="dict">The dictionary how's values should be disposed.</param>
        [DebuggerStepThrough]
        public static void DisposeValuesIfNecessary(IDictionary? dict)
        {
            if (dict == null)
                return;
            IDictionaryEnumerator iter = dict.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Value != null) // Ensure iter.Value is not null before passing it
                    DisposeIfNecessary(iter.Value);
            }
        }

        #endregion

    }
}
