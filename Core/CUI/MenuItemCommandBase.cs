//--------------------------------------------------------------------------
// File:    MenuItemCommandBase.cs
// Content:	Implementation of class MenuItemCommandBase
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

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
