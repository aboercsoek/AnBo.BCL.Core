//--------------------------------------------------------------------------
// File:    ArgChecker.cs
// Content:	Implementation of class ArgChecker
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// ArgChecker provides help methods for parameter checking.
	/// </summary>
	public static class ArgChecker
    {
        #region ShouldNotBeNull methods

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNull<T>([NotNull] T? argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == null)
                throw new ArgNullException(argName!, 
                    string.IsNullOrEmpty(errorMessage) ? StringResources.ErrorShouldNotBeNullValidationTemplate1Arg : errorMessage);
        }

        #endregion ShouldNotBeNull methods

        #region ShouldNotBeEmpty methods

        /// <summary>
        /// Check if <paramref name="argValue"/> is not empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty string.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(string argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue!.Length == 0)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                          : errorMessage);
            }
        }

        /// <summary>
        /// Checks if the Guid argument is not empty.
        /// </summary>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(Guid argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == Guid.Empty)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                          : errorMessage);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(StringBuilder argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue!.Length == 0)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                        ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                        : errorMessage);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not an empty collection.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty collection.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(IEnumerable argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            // Optimized check: try ICollection first, then enumerate once
            if (argValue is ICollection collection)
            {
                if (collection.Count == 0)
                    throw new ArgEmptyException(argName!);
                return;
            }

            if (!argValue!.AsSequence<object>().Any())
                throw new ArgEmptyException(argName!);
        }

        #endregion ShouldNotBeEmpty methods

        #region ShouldNotBeNullOrEmpty methods

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty string.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty([NotNull] string? argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, errorMessage: errorMessage, argName: argName);

            ShouldNotBeEmpty(argValue: argValue!, errorMessage: errorMessage, argName: argName);
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty([NotNull] Guid? argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, errorMessage: errorMessage, argName: argName);

            ShouldNotBeEmpty(argValue: argValue!.Value, errorMessage: errorMessage, argName: argName);
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty StringBuilder.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty([NotNull] StringBuilder? argValue, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, errorMessage: errorMessage, argName: argName);

            ShouldNotBeEmpty(argValue: argValue!, errorMessage: errorMessage, argName: argName);
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> and not an empty collection.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty collection.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty([NotNull] IEnumerable? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, argName: argName);

            ShouldNotBeEmpty(argValue: argValue!, argName: argName);
        }

        #endregion ShouldNotBeNullOrEmpty methods

        #region ShouldBeTrue, ShouldBeFalse methods

        /// <summary>
        /// Checks if the argument condition is true.
        /// </summary>
        /// <exception cref="ArgException{T}">Is thrown if <paramref name="argCondition"/> is false.</exception>
        /// <param name="argCondition">The argument condition.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldBeTrue([DoesNotReturnIf(false)] bool argCondition, string? errorMessage = null, [CallerArgumentExpression(nameof(argCondition))] string? argName = null)
        {
            if (!argCondition)
                throw new ArgException<bool>(false, argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentValidationFailedTemplate2Args
                                                          : errorMessage);
        }

        /// <summary>
        /// Checks if the argument condition is false.
        /// </summary>
        /// <exception cref="ArgException{T}">Is thrown if <paramref name="argCondition"/> is true.</exception>
        /// <param name="argCondition">The argument condition.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldBeFalse([DoesNotReturnIf(true)] bool argCondition, string? errorMessage = null, [CallerArgumentExpression(nameof(argCondition))] string? argName = null)
        {
            if (argCondition)
                throw new ArgException<bool>(true, argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentValidationFailedTemplate2Args
                                                          : errorMessage);
        }

        #endregion ShouldBeTrue, ShouldBeFalse methods

        #region ShouldBeInRange methods

        /// <summary>
        /// Checks if the argument is in a specified range. Specified minValue and maxValue are valid values of argValue.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <param name="minValue">The valid min value.</param>
        /// <param name="maxValue">The valid max value.</param>
        /// <exception cref="ArgOutOfRangeException{T}">Is thrown if <paramref name="argValue"/> is less than <paramref name="minValue"/> or greater than <paramref name="maxValue"</exception>
        [DebuggerStepThrough]
        public static void ShouldBeInRange<T>(T argValue, T minValue, T maxValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null) where T : struct, IComparable<T>
        {
            // Ensure min/max are in correct order
            var (actualMin, actualMax) = minValue.CompareTo(maxValue) <= 0 ? (minValue, maxValue) : (maxValue, minValue);

            if (argValue.CompareTo(actualMin) < 0 || argValue.CompareTo(actualMax) > 0)
                throw new ArgOutOfRangeException<T>(argValue, argName!, minValue, maxValue);
        }

        #endregion

        #region Filesystem validation methods

        /// <summary>
        /// Check if the <paramref name="argFilePath"/> value contains a path to a existing file.
        /// </summary>
        /// <param name="argFilePath">The argument file path value.</param>
        /// <param name="argName">The Name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argFilePath"/> is <see langword="null"/></exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argFilePath"/> is an empty string.</exception>
        /// <exception cref="FilePathTooLongException">Is thrown if <paramref name="argFilePath"/> length is too long (see <see cref="FileHelper.MAXIMUM_FILE_NAME_LENGTH"/>).</exception>
        /// <exception cref="ArgFilePathException">Is thrown if the <paramref name="argFilePath"/> value is not a path to a existing file.</exception>
        [DebuggerStepThrough]
        public static void ShouldBeExistingFile(string? argFilePath, [CallerArgumentExpression(nameof(argFilePath))] string? argName = null)
        {
            ShouldNotBeNullOrEmpty(argValue: argFilePath, argName: argName);
            try
            {
                // Use Path.GetFullPath for better validation
                var fullPath = Path.GetFullPath(argFilePath);

                if (fullPath.Length > FileHelper.MAXIMUM_FILE_NAME_LENGTH)
                    throw new FilePathTooLongException(argFilePath, argName!);

                if (File.Exists(fullPath) == false)
                    throw new ArgFilePathException(argFilePath, argName!);
            }
            catch (Exception ex) when (ex is not ArgFilePathException)
            {
                throw new ArgFilePathException(argFilePath, argName!);
            }
        }

        /// <summary>
        /// Check if the <paramref name="argFileInfo"/> is a existing file.
        /// </summary>
        /// <param name="argFileInfo">The file info to check.</param>
        /// <param name="argName">The Name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argFileInfo"/> is <see langword="null"/></exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argFileInfo"/> is an empty string.</exception>
        /// <exception cref="FilePathTooLongException">Is thrown if <paramref name="argFileInfo"/> length is too long (see <see cref="FileHelper.MAXIMUM_FILE_NAME_LENGTH"/>).</exception>
        /// <exception cref="ArgFilePathException">Is thrown if the <paramref name="argFileInfo"/> value is not a path to a existing file.</exception>
        [DebuggerStepThrough]
        public static void ShouldBeExistingFile(FileInfo? argFileInfo, [CallerArgumentExpression(nameof(argFileInfo))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argFileInfo, argName: argName);

            if (argFileInfo!.Exists == false)
                throw new ArgFilePathException(argFileInfo, argName!);
        }

        /// <summary>
        /// Check if the <paramref name="argDirectoryPath"/> value contains a path to a existing directory.
        /// </summary>
        /// <param name="argDirectoryPath">The argument directory path value.</param>
        /// <param name="argName">The Name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argDirectoryPath"/> is an empty string.</exception>
        /// <exception cref="DirectoryPathTooLongException">Is thrown if <paramref name="argDirectoryPath"/> length is too long (see <see cref="FileHelper.MAXIMUM_FOLDER_NAME_LENGTH"/>).</exception>
        /// <exception cref="ArgDirectoryPathException">Is thrown if the <paramref name="argDirectoryPath"/> value is not a path to a existing directory.</exception>
        [DebuggerStepThrough]
        public static void ShouldBeExistingDirectory(string? argDirectoryPath, [CallerArgumentExpression(nameof(argDirectoryPath))] string? argName = null)
        {
            ShouldNotBeNullOrEmpty(argValue: argDirectoryPath, argName: argName);

            try
            {
                // Use Path.GetFullPath for better validation
                var fullPath = Path.GetFullPath(argDirectoryPath);

                if (fullPath.Length > FileHelper.MAXIMUM_FOLDER_NAME_LENGTH)
                    throw new DirectoryPathTooLongException(argDirectoryPath, argName!);

                if (Directory.Exists(fullPath) == false)
                    throw new ArgDirectoryPathException(argDirectoryPath, argName!);
            }
            catch (Exception ex) when (ex is not ArgFilePathException)
            {
                throw new ArgFilePathException(argDirectoryPath, argName!);
            }
        }

        #endregion

        #region ShouldMatch methods

        /// <summary>
        /// Check if the <paramref name="argValue"/> value mathes the <paramref name="regexPattern">Regular Expression</paramref>
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="regexPattern">The regular expression to match.</param>
        /// <param name="options">Regex options (default: None).</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> or <paramref name="regexPattern"/> is <see langword="null"/></exception>
        /// <exception cref="ArgException{String}">Is thrown if the <paramref name="argValue"/> value does not mathes the <paramref name="regexPattern">Regular Expression</paramref>.</exception>
        [DebuggerStepThrough]
        public static void ShouldMatch(string? argValue, [NotNull] string regexPattern, 
            RegexOptions options = RegexOptions.None, 
            string? errorMessage = null, 
            [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, argName: argName);
            ShouldNotBeNullOrEmpty(regexPattern);

            Regex regex = new Regex(regexPattern!, options);

            // Use compiled regex for better performance
            if (!Regex.IsMatch(argValue!, regexPattern!, options | RegexOptions.Compiled))
            {
                if (errorMessage == null)
                    throw new ArgException<string>(argValue!, argName!);
                else
                    throw new ArgException<string>(argValue!, argName!, errorMessage);
            }

            //ShouldMatch(argValue!, regex, errorMessage, argName);
        }

        /// <summary>
        /// Check if the <paramref name="argValue"/> value mathes the <paramref name="regex">Regular Expression</paramref>
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="regex">The regular expression to match.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> or <paramref name="regex"/> is <see langword="null"/></exception>
        /// <exception cref="ArgException{String}">Is thrown if the <paramref name="argValue"/> value does not mathes the <paramref name="regex">Regular Expression</paramref>.</exception>
        [DebuggerStepThrough]
        public static void ShouldMatch(string? argValue, Regex regex, string? errorMessage = null, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue: argValue, argName: argName);
            ShouldNotBeNull(regex);

            if (RegexHelper.MatchAny(argValue!, regex) == -1)
            {
                if (errorMessage == null)
                    throw new ArgException<string>(argValue!, argName!);
                else
                    throw new ArgException<string>(argValue!, argName!, errorMessage);
            }
        }

        #endregion

        #region Type Validation methods

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="targetType">The argument type that will be assigned to.</param>
        /// <param name="sourceType">The type of the value being assigned.</param>
        /// <param name="sourceArgumentName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="sourceType"/> or <paramref name="targetType"/> is <see langword="null"</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="targetType"/> is not assignable from <paramref name="sourceType"</exception>
        [DebuggerStepThrough]
        public static void ShouldBeAssignableFrom(Type? sourceType, Type? targetType, string sourceArgumentName)
        {
            if (targetType == null)
            {
                throw new ArgNullException("targetType", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }
            if (sourceType == null)
            {
                throw new ArgNullException("sourceType", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }
            if (!targetType.IsAssignableFrom(sourceType))
            {

                throw new InvalidTypeCastException("Argument {0} error. {1}".SafeFormatWith(sourceArgumentName.SafeString(), StringResources.ErrorTypesAreNotAssignableTemplate2Args.SafeFormatWith(sourceType, targetType)));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <typeparam name="TSource">The argument type that will be assigned to.</typeparam>
        /// <typeparam name="TTarget">The type of the value being assigned.</typeparam>
        /// <param name="sourceArgumentName">Argument name.</param>
        /// <exception cref="InvalidTypeCastException">Is thrown if <typeparamref name="TTarget"/> is not assignable from <typeparamref name="TSource"</exception>
        [DebuggerStepThrough]
        public static void ShouldBeAssignableFrom<TSource, TTarget>(string sourceArgumentName)
        {
            ShouldBeAssignableFrom(typeof(TSource), typeof(TTarget), sourceArgumentName.SafeString());
        }

        /// <summary>
        /// Verifies that an argument instance is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy, or instance can be 
        /// assigned through a runtime wrapper, as is the case for COM Objects).
        /// </summary>
        /// <param name="targetType">The argument type that will be assigned to.</param>
        /// <param name="instance">The instance that will be assigned.</param>
        /// <param name="argName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="instance"/> or <paramref name="targetType"/> is <see langword="null"</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="assignmentInstance"/> is not an instance of <paramref name="targetType"</exception>
        [DebuggerStepThrough]
        public static void ShouldBeInstanceOfType(Type targetType, object instance, string argName)
        {
            if (instance == null)
            {
                throw new ArgNullException("assignmentInstance", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }
            if (targetType == null)
            {
                throw new ArgNullException("targetType", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }

            if (targetType.IsInstanceOfType(instance) == false)
            {
                throw new InvalidTypeCastException("Argument {0} error. {1}".SafeFormatWith(argName.SafeString(), 
                    StringResources.ErrorTypesAreNotAssignableTemplate2Args.SafeFormatWith(GetTypeName(instance), targetType)));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <typeparam name="TTarget">The type of the value being assigned.</typeparam>
        /// <param name="instance">The instance that will be assigned.</param>
        /// <param name="argName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="instance"/> is <see langword="null"</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="instance"/> is not an instance of <typeparamref name="TTarget"</exception>
        [DebuggerStepThrough]
        public static void ShouldBeInstanceOfType<TTarget>(object instance, [CallerArgumentExpression(nameof(instance))] string? argName = null)
        {
            ShouldNotBeNull(argValue: instance, argName: argName);

            if (instance is not TTarget)
            {
                throw new InvalidTypeCastException("Argument {0} error. {1}"
                    .SafeFormatWith(argName!.SafeString(), 
                    StringResources.ErrorTypesAreNotAssignableTemplate2Args.SafeFormatWith(GetTypeName(instance), typeof(TTarget))));
            }
        }


        private static string GetTypeName(object assignmentInstance)
        {
            try
            {
                return assignmentInstance.GetType().FullName ?? "<unknown>";
            }
            catch (Exception)
            {
                return "<unknown>";
            }
        }



        #endregion
    }
}
