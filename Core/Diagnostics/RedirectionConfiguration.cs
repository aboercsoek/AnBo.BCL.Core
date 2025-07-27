//--------------------------------------------------------------------------
// File:    RedirectionConfiguration.cs
// Content:	Implementation of class RedirectionConfiguration
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Configuration class for console redirection settings.
/// Encapsulates all parameters needed for different types of file redirection.
/// </summary>
public class RedirectionConfiguration
{
    /// <summary>
    /// Gets or sets the base file path for redirection.
    /// </summary>
    public string BasePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of redirection to use.
    /// </summary>
    public RedirectionType RedirectionType { get; set; } = RedirectionType.Simple;

    /// <summary>
    /// Gets or sets the maximum file size in bytes before rotation occurs.
    /// </summary>
    public long MaxSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB default

    /// <summary>
    /// Gets or sets the maximum number of backup files to keep during rotation.
    /// </summary>
    public int MaxFiles { get; set; } = 10;

    /// <summary>
    /// Gets or sets the timestamp format for timestamped redirections.
    /// </summary>
    public string TimestampFormat { get; set; } = "yyyyMMdd-HHmmss";
}
