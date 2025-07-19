//--------------------------------------------------------------------------
// File:    SpanExtensionsUnitTest.cs
// Content: Unit tests for SpanExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test
{
    public class SpanExtensionsUnitTest
    {
        #region Count<T>(this Span<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase001_Count_Span_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //Span<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase001_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase001_HelperMethod()
        {
            // Arrange
            Span<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.Count(predicate!);
        }

        [Fact]
        public void TestCase002_Count_Span_With_Empty_Span_Should_Return_Zero()
        {
            // Arrange
            Span<int> span = Span<int>.Empty;

            // Act
            var result = span.Count(x => x > 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase003_Count_Span_With_All_Matching_Elements_Should_Return_Total_Count()
        {
            // Arrange
            Span<int> span = [2, 4, 6, 8];

            // Act
            var result = span.Count(x => x % 2 == 0);

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void TestCase004_Count_Span_With_No_Matching_Elements_Should_Return_Zero()
        {
            // Arrange
            Span<int> span = [1, 3, 5, 7];

            // Act
            var result = span.Count(x => x % 2 == 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase005_Count_Span_With_Some_Matching_Elements_Should_Return_Correct_Count()
        {
            // Arrange
            Span<int> span = [1, 2, 3, 4, 5, 6];

            // Act
            var result = span.Count(x => x > 3);

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public void TestCase006_Count_Span_With_String_Elements_Should_Work_Correctly()
        {
            // Arrange
            Span<string> span = ["apple", "banana", "cherry", "apricot"];

            // Act
            var result = span.Count(s => s.StartsWith("a"));

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void TestCase007_Count_Span_With_Single_Element_Matching_Should_Return_One()
        {
            // Arrange
            Span<int> span = [42];

            // Act
            var result = span.Count(x => x == 42);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void TestCase008_Count_Span_With_Single_Element_Not_Matching_Should_Return_Zero()
        {
            // Arrange
            Span<int> span = [42];

            // Act
            var result = span.Count(x => x == 100);

            // Assert
            result.Should().Be(0);
        }

        #endregion

        #region Count<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase009_Count_ReadOnlySpan_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //ReadOnlySpan<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase009_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase009_HelperMethod()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.Count(predicate!);
        }

        [Fact]
        public void TestCase010_Count_ReadOnlySpan_With_Empty_Span_Should_Return_Zero()
        {
            // Arrange
            ReadOnlySpan<int> span = ReadOnlySpan<int>.Empty;

            // Act
            var result = span.Count(x => x > 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase011_Count_ReadOnlySpan_With_All_Matching_Elements_Should_Return_Total_Count()
        {
            // Arrange
            ReadOnlySpan<int> span = [2, 4, 6, 8];

            // Act
            var result = span.Count(x => x % 2 == 0);

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void TestCase012_Count_ReadOnlySpan_With_No_Matching_Elements_Should_Return_Zero()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 3, 5, 7];

            // Act
            var result = span.Count(x => x % 2 == 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase013_Count_ReadOnlySpan_With_Some_Matching_Elements_Should_Return_Correct_Count()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5, 6];

            // Act
            var result = span.Count(x => x > 3);

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public void TestCase014_Count_ReadOnlySpan_With_String_Elements_Should_Work_Correctly()
        {
            // Arrange
            ReadOnlySpan<string> span = ["apple", "banana", "cherry", "apricot"];

            // Act
            var result = span.Count(s => s.StartsWith("a"));

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void TestCase015_Count_ReadOnlySpan_With_Large_Array_Should_Work_Correctly()
        {
            // Arrange
            var array = new int[10000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
            ReadOnlySpan<int> span = array;

            // Act
            var result = span.Count(x => x % 100 == 0);

            // Assert
            result.Should().Be(100); // 0, 100, 200, ..., 9900
        }

        #endregion

        #region CountSimple<T>(this Span<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase016_CountSimple_Span_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //Span<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase016_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase016_HelperMethod()
        {
            // Arrange
            Span<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.CountSimple(predicate!);
        }

        [Fact]
        public void TestCase017_CountSimple_Span_With_Empty_Span_Should_Return_Zero()
        {
            // Arrange
            Span<int> span = Span<int>.Empty;

            // Act
            var result = span.CountSimple(x => x > 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase018_CountSimple_Span_With_All_Matching_Elements_Should_Return_Total_Count()
        {
            // Arrange
            Span<int> span = [2, 4, 6, 8];

            // Act
            var result = span.CountSimple(x => x % 2 == 0);

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void TestCase019_CountSimple_Span_With_Some_Matching_Elements_Should_Return_Correct_Count()
        {
            // Arrange
            Span<int> span = [1, 2, 3, 4, 5, 6];

            // Act
            var result = span.CountSimple(x => x > 3);

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public void TestCase020_CountSimple_Span_Should_Return_Same_Result_As_Count()
        {
            // Arrange
            Span<int> span = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            // Act
            var countResult = span.Count(x => x % 3 == 0);
            var countSimpleResult = span.CountSimple(x => x % 3 == 0);

            // Assert
            countResult.Should().Be(countSimpleResult);
            countResult.Should().Be(3); // 3, 6, 9
        }

        #endregion

        #region CountSimple<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase021_CountSimple_ReadOnlySpan_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //ReadOnlySpan<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase021_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase021_HelperMethod()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.CountSimple(predicate!);
        }

        [Fact]
        public void TestCase022_CountSimple_ReadOnlySpan_With_Empty_Span_Should_Return_Zero()
        {
            // Arrange
            ReadOnlySpan<int> span = ReadOnlySpan<int>.Empty;

            // Act
            var result = span.CountSimple(x => x > 0);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase023_CountSimple_ReadOnlySpan_With_All_Matching_Elements_Should_Return_Total_Count()
        {
            // Arrange
            ReadOnlySpan<int> span = [2, 4, 6, 8];

            // Act
            var result = span.CountSimple(x => x % 2 == 0);

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void TestCase024_CountSimple_ReadOnlySpan_Should_Return_Same_Result_As_Count()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            // Act
            var countResult = span.Count(x => x % 3 == 0);
            var countSimpleResult = span.CountSimple(x => x % 3 == 0);

            // Assert
            countResult.Should().Be(countSimpleResult);
            countResult.Should().Be(3); // 3, 6, 9
        }

        #endregion

        #region Any<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase025_Any_ReadOnlySpan_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //ReadOnlySpan<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase025_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase025_HelperMethod()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.Any(predicate!);
        }

        [Fact]
        public void TestCase026_Any_ReadOnlySpan_With_Empty_Span_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = ReadOnlySpan<int>.Empty;

            // Act
            var result = span.Any(x => x > 0);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase027_Any_ReadOnlySpan_With_No_Matching_Elements_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 3, 5, 7];

            // Act
            var result = span.Any(x => x % 2 == 0);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase028_Any_ReadOnlySpan_With_Some_Matching_Elements_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 3, 5, 8];

            // Act
            var result = span.Any(x => x % 2 == 0);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase029_Any_ReadOnlySpan_With_All_Matching_Elements_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = [2, 4, 6, 8];

            // Act
            var result = span.Any(x => x % 2 == 0);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase030_Any_ReadOnlySpan_Should_Return_True_On_First_Match()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5];

            // Act
            var result = span.Any(x => x == 2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase031_Any_ReadOnlySpan_With_String_Elements_Should_Work_Correctly()
        {
            // Arrange
            ReadOnlySpan<string> span = ["apple", "banana", "cherry"];

            // Act
            var resultTrue = span.Any(s => s.Contains("ban"));
            var resultFalse = span.Any(s => s.Contains("xyz"));

            // Assert
            resultTrue.Should().BeTrue();
            resultFalse.Should().BeFalse();
        }

        [Fact]
        public void TestCase032_Any_ReadOnlySpan_With_Single_Element_Matching_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = [42];

            // Act
            var result = span.Any(x => x == 42);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase033_Any_ReadOnlySpan_With_Single_Element_Not_Matching_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = [42];

            // Act
            var result = span.Any(x => x == 100);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region All<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate) Tests

        [Fact]
        public void TestCase034_All_ReadOnlySpan_With_Null_Predicate_Should_Throw_ArgumentNullException()
        {
            // Arrange
            //ReadOnlySpan<int> span = new int[] { 1, 2, 3 };
            //Func<int, bool>? predicate = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(TestCase034_HelperMethod);
            exception.ParamName.Should().Be("predicate");
        }

        private static void TestCase034_HelperMethod()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 3];
            Func<int, bool>? predicate = null;

            span.All(predicate!);
        }

        [Fact]
        public void TestCase035_All_ReadOnlySpan_With_Empty_Span_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = ReadOnlySpan<int>.Empty;

            // Act
            var result = span.All(x => x > 0);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase036_All_ReadOnlySpan_With_All_Matching_Elements_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = [2, 4, 6, 8];

            // Act
            var result = span.All(x => x % 2 == 0);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase037_All_ReadOnlySpan_With_Some_Matching_Elements_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 2, 4, 6];

            // Act
            var result = span.All(x => x % 2 == 0);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase038_All_ReadOnlySpan_With_No_Matching_Elements_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = [1, 3, 5, 7];

            // Act
            var result = span.All(x => x % 2 == 0);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase039_All_ReadOnlySpan_Should_Return_False_On_First_Non_Match()
        {
            // Arrange
            ReadOnlySpan<int> span = [2, 4, 3, 6, 8];

            // Act
            var result = span.All(x => x % 2 == 0);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase040_All_ReadOnlySpan_With_String_Elements_Should_Work_Correctly()
        {
            // Arrange
            ReadOnlySpan<string> span = ["apple", "apricot", "avocado"];

            // Act
            var resultTrue = span.All(s => s.StartsWith("a"));
            var resultFalse = span.All(s => s.Contains("apple"));

            // Assert
            resultTrue.Should().BeTrue();
            resultFalse.Should().BeFalse();
        }

        [Fact]
        public void TestCase041_All_ReadOnlySpan_With_Single_Element_Matching_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<int> span = [42];

            // Act
            var result = span.All(x => x == 42);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase042_All_ReadOnlySpan_With_Single_Element_Not_Matching_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<int> span = [42];

            // Act
            var result = span.All(x => x == 100);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Performance Comparison Tests (Count vs CountSimple)

        [Fact]
        public void TestCase043_Count_And_CountSimple_Should_Have_Same_Results_For_Various_Types()
        {
            // Arrange
            int[] intArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            double[] doubleArray = [1.1, 2.2, 3.3, 4.4, 5.5];
            string[] stringArray = ["one", "two", "three", "four"];

            Span<int> intSpan = intArray;
            Span<double> doubleSpan = doubleArray;
            Span<string> stringSpan = stringArray;

            ReadOnlySpan<int> intReadOnlySpan = intArray;
            ReadOnlySpan<double> doubleReadOnlySpan = doubleArray;
            ReadOnlySpan<string> stringReadOnlySpan = stringArray;

            // Act & Assert for int
            intSpan.Count(x => x > 5).Should().Be(intSpan.CountSimple(x => x > 5));
            intReadOnlySpan.Count(x => x > 5).Should().Be(intReadOnlySpan.CountSimple(x => x > 5));

            // Act & Assert for double
            doubleSpan.Count(x => x > 3.0).Should().Be(doubleSpan.CountSimple(x => x > 3.0));
            doubleReadOnlySpan.Count(x => x > 3.0).Should().Be(doubleReadOnlySpan.CountSimple(x => x > 3.0));

            // Act & Assert for string
            stringSpan.Count(s => s.Length > 3).Should().Be(stringSpan.CountSimple(s => s.Length > 3));
            stringReadOnlySpan.Count(s => s.Length > 3).Should().Be(stringReadOnlySpan.CountSimple(s => s.Length > 3));
        }

        #endregion

        #region Edge Cases and Complex Predicates

        [Fact]
        public void TestCase044_All_Methods_With_Complex_Predicate_Should_Work_Correctly()
        {
            // Arrange
            int[] data = [10, 15, 20, 25, 30, 35, 40];
            ReadOnlySpan<int> span = data;

            // Complex predicate: divisible by 5 and greater than 12
            Func<int, bool> complexPredicate = x => x % 5 == 0 && x > 12;

            // Act
            var countResult = span.Count(complexPredicate);
            var anyResult = span.Any(complexPredicate);
            var allResult = span.All(complexPredicate);

            // Assert
            countResult.Should().Be(6); // 15, 20, 25, 30, 35, 40
            anyResult.Should().BeTrue();
            allResult.Should().BeFalse(); // 10 doesn't satisfy the condition
        }

        [Fact]
        public void TestCase045_All_Methods_With_Nullable_Reference_Types_Should_Work()
        {
            // Arrange
            string?[] data = ["test", null, "hello", "world", null];
            ReadOnlySpan<string?> span = data;

            // Act
            var countNotNull = span.Count(s => s != null);
            var anyNull = span.Any(s => s == null);
            var allNotNull = span.All(s => s != null);

            // Assert
            countNotNull.Should().Be(3);
            anyNull.Should().BeTrue();
            allNotNull.Should().BeFalse();
        }

        [Fact]
        public void TestCase046_All_Methods_With_Custom_Objects_Should_Work()
        {
            // Arrange
            var people = new[]
            {
                new Person("Alice", 25),
                new Person("Bob", 30),
                new Person("Charlie", 35),
                new Person("Diana", 28)
            };
            ReadOnlySpan<Person> span = people;

            // Act
            var countAdults = span.Count(p => p.Age >= 30);
            var anyYoung = span.Any(p => p.Age < 30);
            var allAdults = span.All(p => p.Age >= 18);

            // Assert
            countAdults.Should().Be(2); // Bob and Charlie
            anyYoung.Should().BeTrue(); // Alice and Diana
            allAdults.Should().BeTrue(); // All are adults
        }

        #endregion

        #region Helper Classes for Testing

        private class Person
        {
            public string Name { get; }
            public int Age { get; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public override bool Equals(object? obj)
            {
                return obj is Person other && Name == other.Name && Age == other.Age;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Age);
            }

            public override string ToString()
            {
                return $"{Name} ({Age})";
            }
        }

        #endregion
    }
}