//--------------------------------------------------------------------------
// File:    MenuItemCommandBase.cs
// Content:	Implementation of class MenuItemCommandBase
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// A class that encapsulates the behavior of a single menu item,
	/// and is responsible for it's execution.
	/// </summary>
	public abstract class MenuItemCommandBase
    {
        #region Public Methods

        /// <summary>
        /// Execute the menu option
        /// </summary>
        public void Execute()
        {
            DoExecute();
        }

        #endregion

        #region Abstract Members

        /// <summary>
        /// Text to display for menu item.
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// Execute the menu item operation.
        /// </summary>
        protected abstract void DoExecute();

        #endregion
    }
}
