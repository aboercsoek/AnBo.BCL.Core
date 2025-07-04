//--------------------------------------------------------------------------
// File:    TypeOf.cs
// Content:	Implementation of class TypeOf
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnBo.Core
{
    ///<summary>Common Type provider class</summary>
	public class TypeOf
    {
        /// <summary>
        /// Bool Type
        /// </summary>
        public static readonly Type Boolean = typeof(bool);
        /// <summary>
        /// Int32 (int) Type
        /// </summary>
        public static readonly Type Int32 = typeof(int);
        /// <summary>
        /// Int64 (long) Type
        /// </summary>
        public static readonly Type Int64 = typeof(long);
        /// <summary>
        /// Object Type
        /// </summary>
        public static readonly Type Object = typeof(object);
        /// <summary>
        /// String Type
        /// </summary>
        public static readonly Type String = typeof(string);

    }
}
