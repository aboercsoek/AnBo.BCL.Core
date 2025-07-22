//--------------------------------------------------------------------------
// File:    ArgCheckerUnitTest.cs
// Content:	Unit tests for ArgChecker class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnBo.Test;

public class ArgCheckerUnitTest
{
    #region ShouldNotBeEmpty String Method Tests

    [Fact]
    public void ShouldNotBeEmpty_String_With_Valid_String_Should_Not_Throw()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_String_With_Empty_String_Should_Throw_ArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Argument value may not be empty*");
    }

    [Fact]
    public void ShouldNotBeEmpty_String_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        var value = string.Empty;
        var customMessage = "Custom empty error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Custom empty error message*");
    }

    #endregion

    #region ShouldNotBeEmpty Guid Method Tests

    [Fact]
    public void ShouldNotBeEmpty_Guid_With_Valid_Guid_Should_Not_Throw()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_Guid_With_Empty_Guid_Should_Throw_ArgumentException()
    {
        // Arrange
        var value = Guid.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Argument value may not be empty*");
    }

    [Fact]
    public void ShouldNotBeEmpty_Guid_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        var value = Guid.Empty;
        var customMessage = "Custom Guid empty error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Custom Guid empty error message (Parameter 'value')");
    }

    #endregion

    #region ShouldNotBeEmpty StringBuilder Method Tests

    [Fact]
    public void ShouldNotBeEmpty_StringBuilder_With_Valid_StringBuilder_Should_Not_Throw()
    {
        // Arrange
        var value = new StringBuilder("test");

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_StringBuilder_With_Empty_StringBuilder_Should_Throw_ArgumentException()
    {
        // Arrange
        var value = new StringBuilder();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*may not be empty*");
    }

    [Fact]
    public void ShouldNotBeEmpty_StringBuilder_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        var value = new StringBuilder();
        var customMessage = "Custom StringBuilder empty error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Custom StringBuilder empty error message*");
    }

    #endregion

    #region ShouldNotBeEmpty IEnumerable Method Tests

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Valid_Collection_Should_Not_Throw()
    {
        // Arrange
        var value = new List<string> { "item1", "item2" };

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Empty_Collection_Should_Throw_ArgumentException()
    {
        // Arrange
        var value = new List<string>();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Array_Should_Work()
    {
        // Arrange
        var value = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Empty_Array_Should_Throw()
    {
        // Arrange
        var value = new int[0];

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Non_ICollection_Should_Work()
    {
        // Arrange
        IEnumerable value = GetTestEnumerable();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Empty_Non_ICollection_Should_Throw()
    {
        // Arrange
        IEnumerable value = GetEmptyTestEnumerable();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    private IEnumerable GetTestEnumerable()
    {
        yield return "item1";
        yield return "item2";
    }

    private IEnumerable GetEmptyTestEnumerable()
    {
        yield break;
    }

    #endregion

    #region ShouldNotBeNullOrEmpty String Method Tests

    [Fact]
    public void ShouldNotBeNullOrEmpty_String_With_Valid_String_Should_Not_Throw()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_String_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_String_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ShouldNotBeNullOrEmpty Guid Method Tests

    [Fact]
    public void ShouldNotBeNullOrEmpty_Guid_With_Valid_Guid_Should_Not_Throw()
    {
        // Arrange
        Guid? value = Guid.NewGuid();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_Guid_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Guid? value = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_Guid_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        Guid? value = Guid.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ShouldNotBeNullOrEmpty StringBuilder Method Tests

    [Fact]
    public void ShouldNotBeNullOrEmpty_StringBuilder_With_Valid_StringBuilder_Should_Not_Throw()
    {
        // Arrange
        StringBuilder? value = new StringBuilder("test");

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_StringBuilder_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? value = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_StringBuilder_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        StringBuilder? value = new StringBuilder();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ShouldNotBeNullOrEmpty IEnumerable Method Tests

    [Fact]
    public void ShouldNotBeNullOrEmpty_IEnumerable_With_Valid_Collection_Should_Not_Throw()
    {
        // Arrange
        IEnumerable? value = new List<string> { "item1", "item2" };

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_IEnumerable_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable? value = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldNotBeNullOrEmpty_IEnumerable_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        IEnumerable? value = new List<string>();

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeNullOrEmpty(value);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ShouldBeTrue Method Tests

    [Fact]
    public void ShouldBeTrue_With_True_Should_Not_Throw()
    {
        // Arrange
        bool condition = true;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeTrue(condition);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeTrue_With_False_Should_Throw_ArgumentException()
    {
        // Arrange
        bool condition = false;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeTrue(condition);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Argument validation failed*");
    }

    [Fact]
    public void ShouldBeTrue_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        bool condition = false;
        var customMessage = "Custom true validation error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeTrue(condition, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Custom true validation error message*");
    }

    #endregion

    #region ShouldBeFalse Method Tests

    [Fact]
    public void ShouldBeFalse_With_False_Should_Not_Throw()
    {
        // Arrange
        bool condition = false;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeFalse(condition);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeFalse_With_True_Should_Throw_ArgumentException()
    {
        // Arrange
        bool condition = true;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeFalse(condition);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*validation failed*");
    }

    [Fact]
    public void ShouldBeFalse_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        bool condition = true;
        var customMessage = "Custom false validation error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeFalse(condition, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Custom false validation error message*");
    }

    #endregion

    #region ShouldBeExistingFile String Method Tests

    [Fact]
    public void ShouldBeExistingFile_String_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? filePath = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(filePath);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldBeExistingFile_String_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        string filePath = string.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(filePath);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldBeExistingFile_String_With_Non_Existing_File_Should_Throw_ArgFilePathException()
    {
        // Arrange
        string filePath = @"C:\NonExistentFile.txt";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(filePath);
        action.Should().Throw<ArgFilePathException>();
    }

    [Fact]
    public void ShouldBeExistingFile_String_With_Invalid_Path_Should_Throw_ArgFilePathException()
    {
        // Arrange
        string filePath = "InvalidPath|<>?";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(filePath);
        action.Should().Throw<ArgFilePathException>();
    }

    [Fact]
    public void ShouldBeExistingFile_String_With_Existing_File_Should_Not_Throw()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();

        try
        {
            // Act & Assert
            var action = () => ArgChecker.ShouldBeExistingFile(tempFile);
            action.Should().NotThrow();
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    #endregion

    #region ShouldBeExistingFile FileInfo Method Tests

    [Fact]
    public void ShouldBeExistingFile_FileInfo_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        FileInfo? fileInfo = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(fileInfo);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldBeExistingFile_FileInfo_With_Non_Existing_File_Should_Throw_ArgFilePathException()
    {
        // Arrange
        var fileInfo = new FileInfo(@"C:\NonExistentFile.txt");

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingFile(fileInfo);
        action.Should().Throw<ArgFilePathException>();
    }

    [Fact]
    public void ShouldBeExistingFile_FileInfo_With_Existing_File_Should_Not_Throw()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();

        try
        {
            var fileInfo = new FileInfo(tempFile);

            // Act & Assert
            var action = () => ArgChecker.ShouldBeExistingFile(fileInfo);
            action.Should().NotThrow();
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    #endregion

    #region ShouldBeExistingDirectory Method Tests

    [Fact]
    public void ShouldBeExistingDirectory_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? directoryPath = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingDirectory(directoryPath);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldBeExistingDirectory_With_Empty_Should_Throw_ArgumentException()
    {
        // Arrange
        string directoryPath = string.Empty;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingDirectory(directoryPath);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldBeExistingDirectory_With_Non_Existing_Directory_Should_Throw_ArgDirectoryPathException()
    {
        // Arrange
        string directoryPath = @"C:\NonExistentDirectory";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingDirectory(directoryPath);
        action.Should().Throw<ArgFilePathException>();
    }

    [Fact]
    public void ShouldBeExistingDirectory_With_Existing_Directory_Should_Not_Throw()
    {
        // Arrange
        string directoryPath = Path.GetTempPath();

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingDirectory(directoryPath);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeExistingDirectory_With_Invalid_Path_Should_Throw_ArgFilePathException()
    {
        // Arrange
        string directoryPath = "InvalidPath|<>?";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeExistingDirectory(directoryPath);
        action.Should().Throw<ArgFilePathException>();
    }

    #endregion

    #region ShouldMatch String RegexPattern Method Tests

    [Fact]
    public void ShouldMatch_String_Pattern_With_Null_Value_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? value = null;
        string pattern = @"\d+";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldMatch_String_Pattern_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string value = "123";
        string? pattern = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldMatch_String_Pattern_With_Matching_Value_Should_Not_Throw()
    {
        // Arrange
        string value = "123";
        string pattern = @"\d+";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldMatch_String_Pattern_With_Non_Matching_Value_Should_Throw_ArgumentException()
    {
        // Arrange
        string value = "abc";
        string pattern = @"\d+";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldMatch_String_Pattern_With_RegexOptions_Should_Work()
    {
        // Arrange
        string value = "ABC";
        string pattern = @"abc";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern, RegexOptions.IgnoreCase);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldMatch_String_Pattern_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        string value = "abc";
        string pattern = @"\d+";
        string customMessage = "Custom regex match error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern, RegexOptions.None, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Custom regex match error message*");
    }

    #endregion

    #region ShouldMatch Regex Object Method Tests

    [Fact]
    public void ShouldMatch_Regex_Object_With_Null_Value_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? value = null;
        var regex = new Regex(@"\d+");

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, regex);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldMatch_Regex_Object_With_Null_Regex_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string value = "123";
        Regex? regex = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, regex!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldMatch_Regex_Object_With_Matching_Value_Should_Not_Throw()
    {
        // Arrange
        string value = "123";
        var regex = new Regex(@"\d+");

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, regex);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldMatch_Regex_Object_With_Non_Matching_Value_Should_Throw_ArgumentException()
    {
        // Arrange
        string value = "abc";
        var regex = new Regex(@"\d+");

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, regex);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldMatch_Regex_Object_With_Custom_Error_Message_Should_Use_Custom_Message()
    {
        // Arrange
        string value = "abc";
        var regex = new Regex(@"\d+");
        string customMessage = "Custom regex object match error message";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, regex, customMessage);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Custom regex object match error message*");
    }

    #endregion

    #region ShouldBeAssignableFrom Type Method Tests

    [Fact]
    public void ShouldBeAssignableFrom_Type_With_Null_Source_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? sourceType = null;
        Type targetType = typeof(object);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom(sourceType, targetType, "sourceType");
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("*sourceType*");
    }

    [Fact]
    public void ShouldBeAssignableFrom_Type_With_Null_Target_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type sourceType = typeof(string);
        Type? targetType = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom(sourceType, targetType, "sourceType");
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("*targetType*");
    }

    [Fact]
    public void ShouldBeAssignableFrom_Type_With_Assignable_Types_Should_Not_Throw()
    {
        // Arrange
        Type sourceType = typeof(string);
        Type targetType = typeof(object);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom(sourceType, targetType, "sourceType");
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeAssignableFrom_Type_With_Non_Assignable_Types_Should_Throw_InvalidTypeCastException()
    {
        // Arrange
        Type sourceType = typeof(int);
        Type targetType = typeof(string);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom(sourceType, targetType, "sourceType");
        action.Should().Throw<InvalidTypeCastException>();
    }

    [Fact]
    public void ShouldBeAssignableFrom_Type_With_Interface_Assignment_Should_Work()
    {
        // Arrange
        Type sourceType = typeof(List<string>);
        Type targetType = typeof(IEnumerable<string>);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom(sourceType, targetType, "sourceType");
        action.Should().NotThrow();
    }

    #endregion

    #region ShouldBeAssignableFrom Generic Method Tests

    [Fact]
    public void ShouldBeAssignableFrom_Generic_With_Assignable_Types_Should_Not_Throw()
    {
        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom<string, object>("testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeAssignableFrom_Generic_With_Non_Assignable_Types_Should_Throw_InvalidTypeCastException()
    {
        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom<int, string>("testParam");
        action.Should().Throw<InvalidTypeCastException>();
    }

    [Fact]
    public void ShouldBeAssignableFrom_Generic_With_Interface_Assignment_Should_Work()
    {
        // Act & Assert
        var action = () => ArgChecker.ShouldBeAssignableFrom<List<string>, IEnumerable<string>>("testParam");
        action.Should().NotThrow();
    }

    #endregion

    #region ShouldBeInstanceOfType Type Method Tests

    [Fact]
    public void ShouldBeInstanceOfType_Type_With_Null_Instance_Should_Throw_ArgumentNullException()
    {
        // Arrange
        object? instance = null;
        Type targetType = typeof(string);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType(targetType, instance!, "testParam");
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("*instance*");
    }

    [Fact]
    public void ShouldBeInstanceOfType_Type_With_Null_Target_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        object instance = "test";
        Type? targetType = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType(targetType!, instance, "testParam");
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("*targetType*");
    }

    [Fact]
    public void ShouldBeInstanceOfType_Type_With_Valid_Instance_Should_Not_Throw()
    {
        // Arrange
        object instance = "test";
        Type targetType = typeof(string);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType(targetType, instance, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Type_With_Invalid_Instance_Should_Throw_InvalidTypeCastException()
    {
        // Arrange
        object instance = 42;
        Type targetType = typeof(string);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType(targetType, instance, "testParam");
        action.Should().Throw<InvalidTypeCastException>();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Type_With_Derived_Instance_Should_Work()
    {
        // Arrange
        object instance = new List<string>();
        Type targetType = typeof(IEnumerable);

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType(targetType, instance, "testParam");
        action.Should().NotThrow();
    }

    #endregion

    #region ShouldBeInstanceOfType Generic Method Tests

    [Fact]
    public void ShouldBeInstanceOfType_Generic_With_Null_Instance_Should_Throw_ArgumentNullException()
    {
        // Arrange
        object? instance = null;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType<string>(instance!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Generic_With_Valid_Instance_Should_Not_Throw()
    {
        // Arrange
        object instance = "test";

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType<string>(instance);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Generic_With_Invalid_Instance_Should_Throw_InvalidTypeCastException()
    {
        // Arrange
        object instance = 42;

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType<string>(instance);
        action.Should().Throw<InvalidTypeCastException>();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Generic_With_Derived_Instance_Should_Work()
    {
        // Arrange
        object instance = new List<string>();

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType<IEnumerable>(instance);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldBeInstanceOfType_Generic_With_Interface_Instance_Should_Work()
    {
        // Arrange
        object instance = new List<string>();

        // Act & Assert
        var action = () => ArgChecker.ShouldBeInstanceOfType<ICollection<string>>(instance);
        action.Should().NotThrow();
    }

    #endregion

    #region Class Structure Tests

    [Fact]
    public void ArgChecker_Class_Should_Be_Static()
    {
        // Act & Assert
        typeof(ArgChecker).IsAbstract.Should().BeTrue();
        typeof(ArgChecker).IsSealed.Should().BeTrue();
    }

    [Fact]
    public void ArgChecker_Class_Should_Be_Public()
    {
        // Act & Assert
        typeof(ArgChecker).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void ArgChecker_Class_Should_Be_In_AnBo_Core_Namespace()
    {
        // Act & Assert
        typeof(ArgChecker).Namespace.Should().Be("AnBo.Core");
    }

    [Fact]
    public void ArgChecker_Methods_Should_Have_DebuggerStepThrough_Attribute()
    {
        // Arrange
        var methods = typeof(ArgChecker).GetMethods();

        // Act & Assert
        foreach (var method in methods.Where(m => m.DeclaringType == typeof(ArgChecker)))
        {
            var attributes = method.GetCustomAttributes(typeof(System.Diagnostics.DebuggerStepThroughAttribute), false);
            attributes.Should().NotBeEmpty($"Method {method.Name} should have DebuggerStepThrough attribute");
        }
    }

    #endregion

    #region Edge Cases and Special Scenarios

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Custom_IEnumerable_Should_Work()
    {
        // Arrange
        var customEnumerable = new CustomEnumerable(new[] { 1, 2, 3 });

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(customEnumerable);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldNotBeEmpty_IEnumerable_With_Empty_Custom_IEnumerable_Should_Throw()
    {
        // Arrange
        var customEnumerable = new CustomEnumerable(Array.Empty<int>());

        // Act & Assert
        var action = () => ArgChecker.ShouldNotBeEmpty(customEnumerable);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldMatch_With_Complex_Regex_Pattern_Should_Work()
    {
        // Arrange
        string value = "user@example.com";
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, emailPattern);
        action.Should().NotThrow();
    }

    [Fact]
    public void ShouldMatch_With_Multiline_Pattern_Should_Work()
    {
        // Arrange
        string value = "Line1\nLine2";
        string pattern = @"Line1.*Line2";

        // Act & Assert
        var action = () => ArgChecker.ShouldMatch(value, pattern, RegexOptions.Singleline);
        action.Should().NotThrow();
    }

    [Fact]
    public void Multiple_Validations_In_Sequence_Should_Work()
    {
        // Arrange
        string value = "TestValue";
        var list = new List<string> { "item1", "item2" };

        // Act & Assert
        var action = () =>
        {
            ArgChecker.ShouldNotBeNullOrEmpty(value);
            ArgChecker.ShouldNotBeNullOrEmpty(list);
            ArgChecker.ShouldBeTrue(value.Length > 5);
        };

        action.Should().NotThrow();
    }

    // Helper class for testing custom IEnumerable
    private class CustomEnumerable : IEnumerable
    {
        private readonly int[] _items;

        public CustomEnumerable(int[] items)
        {
            _items = items;
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    #endregion
}