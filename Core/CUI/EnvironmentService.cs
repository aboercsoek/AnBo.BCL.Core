//--------------------------------------------------------------------------
// File:    EnvironmentService.cs
// Content:	Implementation of class EnvironmentService
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Production implementation of environment service
/// Calls actual Environment.Exit in production code
/// </summary>
public class EnvironmentService : IEnvironmentService
{
    /// <summary>
    /// Exits the application using Environment.Exit
    /// </summary>
    /// <param name="exitCode">The exit code to return</param>
    public void Exit(int exitCode)
    {
        Environment.Exit(exitCode);
    }
}
