//--------------------------------------------------------------------------
// File:    CommandLine.cs
// Content:	A command line parser implementation.
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Collections.Specialized;

#endregion

namespace AnBo.Core;

///<summary>A command line parser</summary>
public sealed class CommandLine
{
    #region Private Fields

    private string[] m_Arguments;
    private HybridDictionary? m_Options;
    private bool m_ShowHelp;

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLine"/> class.
    /// </summary>
    /// <param name="args">The command line args.</param>
    public CommandLine(string[] args)
        : this(args, false)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLine"/> class.
    /// </summary>
    /// <param name="args">The command line args.</param>
    /// <param name="showHelpIfCmdLineParamsIsEmpty">if set to <see langword="true"/> <see cref="ShowHelp"/> is set to <see langword="true"/> if args is empty or <see langword="null"/>.</param>
    public CommandLine(string[] args, bool showHelpIfCmdLineParamsIsEmpty)
    {
        m_Arguments = [];
        ParseCommandLineArgs(args, showHelpIfCmdLineParamsIsEmpty);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks the required options.
    /// </summary>
    /// <param name="requiredOptions">The required options.</param>
    /// <returns><see langword="true"/> if all required options where found, otherwise <see langword="false"/>.</returns>
    public bool CheckRequiredOptions(params string[] requiredOptions)
    {
        if ((requiredOptions == null) || (requiredOptions.Length == 0))
            return true;

        return requiredOptions.All(requiredOption => Options.Contains(requiredOption));
    }

    /// <summary>
    /// Checks the valid min and max range of the options count.
    /// </summary>
    /// <param name="min">The min value.</param>
    /// <param name="max">The max value.</param>
    /// <returns><see langword="true"/> if options count is in range, otherwise <see langword="false"/>.</returns>
    public bool CheckOptionsMinMaxCount(int min, int max)
    {
        return (Options.Count >= min && Options.Count <= max);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the commandline arguments.
    /// </summary>
    /// <value>The commandline arguments.</value>
    public string[] Arguments
    {
        get
        {
            return m_Arguments;
        }
    }

    /// <summary>
    /// Gets the commandline options.
    /// </summary>
    /// <value>The commandline options.</value>
    public IDictionary Options
    {
        get { return m_Options ??= new HybridDictionary(false); }
    }

    /// <summary>
    /// Gets a value indicating whether help command was past via the commandline.
    /// </summary>
    /// <value><see langword="true"/> if help command was past via the commandline; otherwise, <see langword="false"/>.</value>
    public bool ShowHelp
    {
        get { return m_ShowHelp; }
    }

    #endregion

    #region Private Methods

    private void ParseCommandLineArgs(string[] cmdLineParams, bool showHelpIfCmdLineParamsIsEmpty)
    {
        List<string> list = new List<string>();

        if (cmdLineParams.IsNotNull())
        {
            foreach (string arg in cmdLineParams)
            {
                char ch = arg[0];
                if ((ch != '/') && (ch != '-'))
                {
                    list.Add(arg);
                }
                else
                {
                    int index = arg.IndexOf(':');
                    if (index == -1)
                    {
                        string strA = arg.Substring(1);
                        if ((string.Compare(strA, "help", StringComparison.OrdinalIgnoreCase) == 0) || strA.Equals("?"))
                        {
                            m_ShowHelp = true;
                        }
                        else
                        {
                            Options[strA] = string.Empty;
                        }
                    }
                    else
                    {
                        Options[arg.Substring(1, index - 1)] = arg.Substring(index + 1);
                    }
                }
            }
        }

        if ((list.Count == 0) && (Options.Count == 0) && (showHelpIfCmdLineParamsIsEmpty))
            m_ShowHelp = true;

        m_Arguments = list.ToArray();
    }

    #endregion
}
