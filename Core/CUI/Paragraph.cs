using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
