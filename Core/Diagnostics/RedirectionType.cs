//--------------------------------------------------------------------------
// File:    RedirectionType.cs
// Content:	Implementation of class RedirectionType
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Enumeration of available redirection types.
/// Defines the different strategies for console output redirection.
/// </summary>
public enum RedirectionType
{
    /// <summary>
    /// Simple redirection to a single file without any special features.
    /// </summary>
    Simple,

    /// <summary>
    /// Redirection with automatic timestamp in the filename for unique sessions.
    /// </summary>
    Timestamped,

    /// <summary>
    /// Redirection with automatic size-based file rotation and retention.
    /// </summary>
    Rotating,

    /// <summary>
    /// Combination of timestamped and rotating redirection for maximum flexibility.
    /// </summary>
    TimestampedRotating
}