//--------------------------------------------------------------------------
// File:    IEnvironmentService.cs
// Content:	Definition of interface IEnvironmentService
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Interface for environment operations to enable testability
/// Abstracts Environment.Exit for dependency injection in tests
/// </summary>
public interface IEnvironmentService
{
    /// <summary>
    /// Exits the application with the specified exit code
    /// </summary>
    /// <param name="exitCode">The exit code to return to the operating system</param>
    void Exit(int exitCode);
}
