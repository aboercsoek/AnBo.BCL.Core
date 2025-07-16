//--------------------------------------------------------------------------
// File:    Paragraph.cs
// Content:	Implementation of class Paragraph
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    [Flags]
    public enum Paragraph
    {
        /// <summary>No additional paragraphs are added.</summary>
        Default = 0,
        /// <summary>No additional paragraphs are added.</summary>
        AddNoParagraph = 0,
        /// <summary>Add paragraph before WriteLine.</summary>
        AddBefore = 1,
        /// <summary>Add paragraph after WriteLine.</summary>
        AddAfter = 2,
        /// <summary>Add paragraph before and after WriteLine.</summary>
        AddBeforeAndAfter = 3
    }
}
