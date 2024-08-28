#region License

// Created by phuongdt

#endregion

using System;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Models a toolbar item that has both a drop-down and a button, where each can behave independently.
	/// </summary>
	public class DropDownButtonAction : ButtonAction, IDropDownAction
	{
	    private Func<ActionModelNode> _menuModelDelegate;

	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The fully qualified action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="flags">Flags that control the style of the action.</param>
		/// <param name="resolver">A resource resolver that will be used to resolve text and image resources.</param>
		public DropDownButtonAction(string actionId, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
			: base(actionId, path, flags, resolver)
		{
		}

		#region IDropDownButtonAction Members

		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get { return _menuModelDelegate(); }
		}

		#endregion

        public void SetMenuModelDelegate(Func<ActionModelNode> menuModelDelegate)
		{
			_menuModelDelegate = menuModelDelegate;
		}
	}
}
