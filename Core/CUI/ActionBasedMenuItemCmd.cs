//--------------------------------------------------------------------------
// File:    ActionBasedMenuItemCmd.cs
// Content:	Implementation of class ActionBasedMenuItemCmd
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;

#endregion

namespace AnBo.Core
{
    ///<summary>Delegate-based menu item command class</summary>
	public class ActionBasedMenuItemCmd : MenuItemCommandBase
    {
        private readonly Action m_MenuItemAction;
        private readonly string m_MenuItemText;


        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBasedMenuItemCmd"/> class.
        /// </summary>
        /// <param name="menuItemAction">The menu item action delegate.</param>
        public ActionBasedMenuItemCmd(Action menuItemAction)
        {
            ArgChecker.ShouldNotBeNull(menuItemAction);

            m_MenuItemAction = menuItemAction;
            m_MenuItemText = GetDescriptionFromOptionCodeDelegate();
        }

        /// <summary>
        /// Text to display in the menu.
        /// </summary>
        public override string Text => m_MenuItemText;

        /// <summary>
        /// Execute the actual operation.
        /// </summary>
        protected override void DoExecute()
        {
            m_MenuItemAction();
        }

        /// <summary>
        /// Pull the description off attributes on the delegate passed in.
        /// This only works if you pass in actual methods, but that's ok
        /// for our purposes.
        /// </summary>
        /// <returns>Description text to display.</returns>
        private string GetDescriptionFromOptionCodeDelegate()
        {
            DescriptionAttribute? description =
                m_MenuItemAction.Method.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .AsSequence<DescriptionAttribute>().FirstOrDefault();

            return description != null ? description.Description : "No description present";
        }
    }
}
