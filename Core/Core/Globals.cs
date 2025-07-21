//--------------------------------------------------------------------------
// File:    Globals.cs
// Content:	Implementation of class Globals
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directivesusing System;

using System.Diagnostics;

#endregion

namespace AnBo.Core;

/// <summary>
/// Static class encasulating global elements.
/// </summary>
public static class Globals
{
    /// <summary>
    /// All synchronization code should exclusively use this lock object,
    /// hence making it trivial to ensure that there are no deadlocks.
    /// It also means that the lock should never be held for long.
    /// In particular, no code holding this lock should ever wait on another thread.
    /// </summary>
    public static readonly object LockingObject = new object();

    //private const bool forceSingleStep = true;

    /// <summary>
    /// Signals a breakpoint to the attached Debugger. Useful in development mode to pause execution (e.g., when a condition is met).
    /// </summary>
    [DebuggerNonUserCode]
    public static void BreakForDebugging()
    {
        //if ( forceSingleStep )
        //{
        Debugger.Break();
        //}
    }
}
