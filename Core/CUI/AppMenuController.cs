//--------------------------------------------------------------------------
// File:    AppMenuController.cs
// Content:	Implementation of class AppMenuController
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core
{
    ///<summary>Console Application Menu Controller</summary>
	public class AppMenuController : IEnumerable<MenuItemCommandBase>
    {
        #region Private Members

        private readonly List<MenuItemCommandBase> m_MenuItems = new List<MenuItemCommandBase>();
        private IAppMenuView m_View;

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new application menu.
        /// </summary>
        /// <param name="appMenuHeaderText">The application menu header text.</param>
        public AppMenuController(string appMenuHeaderText)
        {
            m_View = new ConsoleAppMenuView(appMenuHeaderText ?? "Console Application Menu");
        }

        /// <summary>
        /// Initializes a new application menu.
        /// </summary>
        /// <param name="appMenuHeaderText">The application menu header text.</param>
        /// <param name="menuItemActions">The menu item actions.</param>
        public AppMenuController(string appMenuHeaderText, params Action[] menuItemActions)
            : this(appMenuHeaderText)
        {
            foreach (var menuItemAction in menuItemActions)
            {
                Add(menuItemAction);
            }
        }

        #endregion

        #region Application Menu Execution Method

        /// <summary>
        /// Execute console application menu.
        /// </summary>
        public void Run()
        {
            m_View.InitView(this.Select(menuItem => menuItem.Text));

            while (!m_View.ShouldQuit)
            {
                m_View.DisplayMenu();
                HandleUserSelection();
                m_View.PromptToContinue();
            }

            Environment.Exit(0);
        }

        #endregion

        #region Application Menu Item Management

        /// <summary>
        /// Adds the specified menu item to the application menu.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        public void Add(MenuItemCommandBase menuItem)
        {
            if (menuItem == null)
                return;

            m_MenuItems.Add(menuItem);
        }

        /// <summary>
        /// Adds the specified menu item action to the application menu.
        /// </summary>
        /// <param name="menuItemAction">The menu item action.</param>
        public void Add(Action menuItemAction)
        {
            if (menuItemAction == null)
                return;

            m_MenuItems.Add(new ActionBasedMenuItemCmd(menuItemAction));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the menu item collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}">enumerator</see> instance that can be used to iterate through the menu item collection.
        /// </returns>
        public IEnumerator<MenuItemCommandBase> GetEnumerator()
        {
            return m_MenuItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Methods

        private void HandleUserSelection()
        {
            int selectedOption = m_View.WaitForValidUserInput();
            if (selectedOption != -1)
            {
                m_View.ClearView();
                m_View.WriteMenuOperationHeader(m_MenuItems[selectedOption].Text);
                try
                {
                    m_MenuItems[selectedOption].Execute();
                }
                catch (Exception ex)
                {
                    m_View.ShowExceptionDetails(ex);
                }
            }
        }

        #endregion

    }
}
