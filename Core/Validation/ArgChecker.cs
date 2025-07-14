//--------------------------------------------------------------------------
// File:    ArgChecker.cs
// Content:	Implementation of class ArgChecker
// Author:	Andreas Börcsök
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
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/></exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNull(object? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == null)
                throw new ArgNullException(argName!);
        }


        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNull(object? argValue, string? errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == null)
                throw new ArgNullException(argName!, string.IsNullOrEmpty(errorMessage) ? StringResources.ErrorShouldNotBeNullValidationTemplate1Arg : errorMessage);
        }

        #endregion ShouldNotBeNull methods

        #region ShouldNotBeNullOrEmpty methods

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty string.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(string? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);

            if (argValue!.Length == 0)
                throw new ArgEmptyException(argName!);
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="argValue"/> is an empty string.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(string? argValue, string? errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName, errorMessage);

            if (argValue!.Length == 0)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                          : errorMessage);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(Guid? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);

            if (argValue!.Value == Guid.Empty)
            {
                throw new ArgEmptyException(argName!, StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(Guid? argValue, string? errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName, errorMessage);

            if (argValue!.Value == Guid.Empty)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                        ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                        : errorMessage);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="argValue"/> is an empty StringBuilder.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(StringBuilder? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);

            if (argValue!.IsEmpty())
            {
                throw new ArgEmptyException(argName!, StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgNullOrEmptyException">Is thrown if <paramref name="argValue"/> is an empty StringBuilder.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(StringBuilder? argValue, string? errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName, errorMessage);

            if (argValue!.IsEmpty())
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                        ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                        : errorMessage);
            }
        }

        /// <summary>
        /// Check if <paramref name="argValue"/> is not <see langword="null"/> and not an empty collection.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty collection.</exception>
        [DebuggerStepThrough]
        public static void ShouldNotBeNullOrEmpty(IEnumerable? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);

            if (argValue!.AsSequence<object>().Any())
                return;

            throw new ArgEmptyException(argName!);
        }

        #endregion ShouldNotBeNullOrEmpty methods

        #region ShouldNotBeEmpty methods

        /// <summary>
        /// Checks if the Guid argument is not empty.
        /// </summary>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(Guid argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == Guid.Empty)
                throw new ArgEmptyException(argName!);
        }

        /// <summary>
        /// Checks if the Guid argument is not empty.
        /// </summary>
        /// <exception cref="ArgEmptyException">Is thrown if <paramref name="argValue"/> is an empty Guid.</exception>
        /// <param name="argValue">The argument value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldNotBeEmpty(Guid argValue, string? errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            if (argValue == Guid.Empty)
            {
                throw new ArgEmptyException(argName!, string.IsNullOrEmpty(errorMessage)
                                                          ? StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg
                                                          : errorMessage);
            }
        }

        #endregion ShouldNotBeEmpty methods

        #region ShouldBeTrue, ShouldBeFalse methods

        /// <summary>
        /// Checks if the argument condition is true.
        /// </summary>
        /// <exception cref="ArgException{T}">Is thrown if <paramref name="argCondition"/> is false.</exception>
        /// <param name="argCondition">The argument condition.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void ShouldBeTrue([DoesNotReturnIf(false)] bool argCondition, string? errorMessage, [CallerArgumentExpression(nameof(argCondition))] string? argName = null)
        {
            if (argCondition == false)
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
        public static void ShouldBeFalse([DoesNotReturnIf(true)] bool argCondition, string? errorMessage, [CallerArgumentExpression(nameof(argCondition))] string? argName = null)
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
        /// <exception cref="ArgOutOfRangeException{T}">Is thrown if <paramref name="argValue"/> is less than <paramref name="minValue"/> or greater than <paramref name="maxValue"/>.</exception>
        [DebuggerStepThrough]
        public static void ShouldBeInRange<T>(T argValue, T minValue, T maxValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null) where T : struct, IComparable<T>
        {
            if (((IComparable<T>)maxValue).CompareTo(minValue) >= 0)
            {
                if (((IComparable<T>)argValue).CompareTo(minValue) < 0)
                    throw new ArgOutOfRangeException<T>(argValue, argName!, minValue, maxValue);

                if (((IComparable<T>)argValue).CompareTo(maxValue) > 0)
                    throw new ArgOutOfRangeException<T>(argValue, argName!, minValue, maxValue);
            }
            else
            {
                if (((IComparable<T>)argValue).CompareTo(maxValue) < 0)
                    throw new ArgOutOfRangeException<T>(argValue, argName!, maxValue, minValue);

                if (((IComparable<T>)argValue).CompareTo(minValue) > 0)
                    throw new ArgOutOfRangeException<T>(argValue, argName!, maxValue, minValue);

            }
        }

        #endregion

        #region ShouldBeExistingFile, ShouldBeExistingDirectory methods

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
            ShouldNotBeNullOrEmpty(argFilePath, argName);

            if (argFilePath!.Length > FileHelper.MAXIMUM_FILE_NAME_LENGTH)
                throw new FilePathTooLongException(argFilePath, argName!);

            if (File.Exists(argFilePath) == false)
                throw new ArgFilePathException(argFilePath, argName!);
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
            ShouldNotBeNull(argFileInfo, argName);

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
            ShouldNotBeNullOrEmpty(argDirectoryPath, argName);

            if (argDirectoryPath!.Length > FileHelper.MAXIMUM_FOLDER_NAME_LENGTH)
                throw new DirectoryPathTooLongException(argDirectoryPath, argName!);

            if (Directory.Exists(argDirectoryPath) == false)
                throw new ArgDirectoryPathException(argDirectoryPath, argName!);
        }

        #endregion

        #region ShouldMatch methods

        /// <summary>
        /// Check if the <paramref name="argValue"/> value mathes the <paramref name="regexPattern">Regular Expression</paramref>
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="regexPattern">The regular expression to match.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> or <paramref name="regexPattern"/> is <see langword="null"/></exception>
        /// <exception cref="ArgException{String}">Is thrown if the <paramref name="argValue"/> value does not mathes the <paramref name="regexPattern">Regular Expression</paramref>.</exception>
        [DebuggerStepThrough]
        public static void ShouldMatch(string? argValue, string regexPattern, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);
            ShouldNotBeNull(regexPattern);
            Regex regex = new Regex(regexPattern);
            ShouldMatch(argValue!, regex, argName);
        }

        /// <summary>
        /// Check if the <paramref name="argValue"/> value mathes the <paramref name="regex">Regular Expression</paramref>
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="regex">The regular expression to match.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> or <paramref name="regex"/> is <see langword="null"/></exception>
        /// <exception cref="ArgException{String}">Is thrown if the <paramref name="argValue"/> value does not mathes the <paramref name="regex">Regular Expression</paramref>.</exception>
        [DebuggerStepThrough]
        public static void ShouldMatch(string? argValue, Regex regex, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);
            ShouldNotBeNull(regex);

            if (RegexHelper.MatchAny(argValue!, regex!) == -1)
                throw new ArgException<string>(argValue!, argName!,
                                               "Argument {0} does not match the regular expresssion '{1}' (argument value: {2})".
                                                   SafeFormatWith(argName!, regex!.ToString(), argValue!));
        }


        /// <summary>
        /// Check if the <paramref name="argValue"/> value mathes the <paramref name="regexPattern">Regular Expression</paramref>
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="regexPattern">The regular expression to match.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="argName">The name of the argument.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="argValue"/> or <paramref name="regexPattern"/> is <see langword="null"/></exception>
        /// <exception cref="ArgException{String}">Is thrown if the <paramref name="argValue"/> value does not mathes the <paramref name="regexPattern">Regular Expression</paramref>.</exception>
        [DebuggerStepThrough]
        public static void ShouldMatch(string? argValue, string regexPattern, string errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);
            ShouldNotBeNull(regexPattern);
            Regex regex = new Regex(regexPattern!);
            ShouldMatch(argValue!, regex, errorMessage, argName);
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
        public static void ShouldMatch(string? argValue, Regex regex, string errorMessage, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
        {
            ShouldNotBeNull(argValue, argName);
            ShouldNotBeNull(regex);

            if (RegexHelper.MatchAny(argValue!, regex) == -1)
                throw new ArgException<string>(argValue!, argName!, errorMessage);
        }

        #endregion

        #region ShouldBeAssignable methods

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="targetType">The argument type that will be assigned to.</param>
        /// <param name="sourceType">The type of the value being assigned.</param>
        /// <param name="sourceArgumentName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="sourceType"/> or <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="targetType"/> is not assignable from <paramref name="sourceType"/>.</exception>
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
            if (targetType.IsAssignableFrom(sourceType) == false)
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
        /// <exception cref="InvalidTypeCastException">Is thrown if <typeparamref name="TTarget"/> is not assignable from <typeparamref name="TSource"/>.</exception>
        [DebuggerStepThrough]
        public static void ShouldBeAssignableFrom<TSource, TTarget>(string sourceArgumentName)
        {
            ShouldBeAssignableFrom(typeof(TSource), typeof(TTarget), sourceArgumentName.SafeString());
        }

        #endregion

        #region ShouldBeInstanceOfType methods

        /// <summary>
        /// Verifies that an argument instance is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy, or instance can be 
        /// assigned through a runtime wrapper, as is the case for COM Objects).
        /// </summary>
        /// <param name="targetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentInstance">The instance that will be assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="assignmentInstance"/> or <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="assignmentInstance"/> is not an instance of <paramref name="targetType"/>.</exception>
        public static void ShouldBeInstanceOfType(Type targetType, object assignmentInstance, string argumentName)
        {
            if (assignmentInstance == null)
            {
                throw new ArgNullException("assignmentInstance", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }
            if (targetType == null)
            {
                throw new ArgNullException("targetType", StringResources.ErrorShouldNotBeNullValidationTemplate1Arg);
            }

            if (targetType.IsInstanceOfType(assignmentInstance) == false)
            {

                throw new InvalidTypeCastException("Argument {0} error. {1}".SafeFormatWith(argumentName.SafeString(), StringResources.ErrorTypesAreNotAssignableTemplate2Args.SafeFormatWith(GetTypeName(assignmentInstance), targetType)));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <typeparam name="TTarget">The type of the value being assigned.</typeparam>
        /// <param name="assignmentInstance">The instance that will be assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="assignmentInstance"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidTypeCastException">Is thrown if <paramref name="assignmentInstance"/> is not an instance of <typeparamref name="TTarget"/>.</exception>
        public static void ShouldBeInstanceOfType<TTarget>(object assignmentInstance, string argumentName)
        {
            ShouldBeInstanceOfType(typeof(TTarget), assignmentInstance, argumentName.SafeString());
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
