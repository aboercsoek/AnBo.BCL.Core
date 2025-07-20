using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;
using System.Diagnostics;
using System.Threading;

namespace AnBo.Test
{
    public class GlobalsUnitTest
    {
        #region LockingObject Tests

        [Fact]
        public void LockingObject_Should_Not_Be_Null()
        {
            // Act & Assert
            Globals.LockingObject.Should().NotBeNull();
        }

        [Fact]
        public void LockingObject_Should_Be_Same_Instance_On_Multiple_Accesses()
        {
            // Act
            var firstAccess = Globals.LockingObject;
            var secondAccess = Globals.LockingObject;

            // Assert
            firstAccess.Should().BeSameAs(secondAccess);
        }

        [Fact]
        public void LockingObject_Should_Be_Usable_For_Lock_Statement()
        {
            // Arrange
            var executed = false;

            // Act & Assert
            var action = () =>
            {
                lock (Globals.LockingObject)
                {
                    executed = true;
                }
            };

            action.Should().NotThrow();
            executed.Should().BeTrue();
        }

        [Fact]
        public void LockingObject_Should_Support_Multiple_Concurrent_Lock_Attempts()
        {
            // Arrange
            var lockTaken1 = false;
            var lockTaken2 = false;
            var barrier = new Barrier(2);
            var lockObject = Globals.LockingObject;

            var thread1 = new Thread(() =>
            {
                barrier.SignalAndWait(); // Synchronize start
                lock (lockObject)
                {
                    lockTaken1 = true;
                    Thread.Sleep(100); // Hold lock briefly
                }
            });

            var thread2 = new Thread(() =>
            {
                barrier.SignalAndWait(); // Synchronize start
                Thread.Sleep(50); // Ensure thread1 gets lock first
                lock (lockObject)
                {
                    lockTaken2 = true;
                }
            });

            // Act
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // Assert
            lockTaken1.Should().BeTrue();
            lockTaken2.Should().BeTrue();
        }

        #endregion

        #region BreakForDebugging Tests

        [Fact]
        public void TestCase005_BreakForDebugging_Should_Not_Throw_When_No_Debugger_Attached()
        {
            // Act & Assert
            var action = () => Globals.BreakForDebugging();
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase006_BreakForDebugging_Should_Be_Marked_With_DebuggerNonUserCode_Attribute()
        {
            // Arrange
            var method = typeof(Globals).GetMethod(nameof(Globals.BreakForDebugging));

            // Act
            var attributes = method?.GetCustomAttributes(typeof(DebuggerNonUserCodeAttribute), false);

            // Assert
            attributes.Should().NotBeNull();
            attributes.Should().HaveCount(1);
        }

        [Fact]
        public void TestCase007_BreakForDebugging_Should_Be_Public_Static_Method()
        {
            // Arrange
            var method = typeof(Globals).GetMethod(nameof(Globals.BreakForDebugging));

            // Act & Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
            method.IsStatic.Should().BeTrue();
        }

        [Fact]
        public void TestCase008_BreakForDebugging_Should_Have_Void_Return_Type()
        {
            // Arrange
            var method = typeof(Globals).GetMethod(nameof(Globals.BreakForDebugging));

            // Act & Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void TestCase009_BreakForDebugging_Should_Have_No_Parameters()
        {
            // Arrange
            var method = typeof(Globals).GetMethod(nameof(Globals.BreakForDebugging));

            // Act & Assert
            method.Should().NotBeNull();
            method!.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void TestCase010_BreakForDebugging_Multiple_Calls_Should_Not_Throw()
        {
            // Act & Assert
            var action = () =>
            {
                Globals.BreakForDebugging();
                Globals.BreakForDebugging();
                Globals.BreakForDebugging();
            };
            
            action.Should().NotThrow();
        }

        #endregion

        #region Class Structure Tests

        [Fact]
        public void TestCase011_Globals_Class_Should_Be_Static()
        {
            // Act & Assert
            typeof(Globals).IsAbstract.Should().BeTrue();
            typeof(Globals).IsSealed.Should().BeTrue();
        }

        [Fact]
        public void TestCase012_Globals_Class_Should_Be_Public()
        {
            // Act & Assert
            typeof(Globals).IsPublic.Should().BeTrue();
        }

        [Fact]
        public void TestCase013_Globals_Class_Should_Be_In_AnBo_Core_Namespace()
        {
            // Act & Assert
            typeof(Globals).Namespace.Should().Be("AnBo.Core");
        }

        [Fact]
        public void TestCase014_Globals_Class_Should_Have_Expected_Members()
        {
            // Arrange
            var type = typeof(Globals);

            // Act
            var lockingObjectField = type.GetField(nameof(Globals.LockingObject));
            var breakForDebuggingMethod = type.GetMethod(nameof(Globals.BreakForDebugging));

            // Assert
            lockingObjectField.Should().NotBeNull();
            lockingObjectField!.IsPublic.Should().BeTrue();
            lockingObjectField.IsStatic.Should().BeTrue();
            lockingObjectField.IsInitOnly.Should().BeTrue();

            breakForDebuggingMethod.Should().NotBeNull();
            breakForDebuggingMethod!.IsPublic.Should().BeTrue();
            breakForDebuggingMethod.IsStatic.Should().BeTrue();
        }

        #endregion
    }
}