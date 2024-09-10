#region License

// Created by phuongdt

#endregion

using System;
using System.Reflection;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Attribute class used to define <see cref="DropDownButtonAction"/>s.
	/// </summary>
	public class DropDownButtonActionAttribute : ButtonActionAttribute
	{
		private readonly string _menuModelPropertyName;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        /// <param name="clickHandler">Name of the method that will be invoked when the button is clicked.</param>
		/// <param name="menuModelPropertyName">The name of the property in the target class (i.e. the
		/// class to which this attribute is applied) that returns the menu model as an <see cref="ActionModelNode"/>.</param>
		public DropDownButtonActionAttribute(string actionID, string pathHint, string clickHandler, string menuModelPropertyName)
            : base(actionID, pathHint)
        {
        	_menuModelPropertyName = menuModelPropertyName;
        }

		/// <summary>
		/// Constructs/initializes an <see cref="DropDownButtonAction"/> via the given <see cref="IActionBuildingContext"/>.
		/// </summary>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			base.Apply(builder);

			((DropDownButtonAction)builder.Action).SetMenuModelDelegate(
				DropDownActionAttribute.CreateGetMenuModelDelegate(builder.ActionTarget, _menuModelPropertyName));
		}

        /// <summary>
        /// Factory method to instantiate the action.
        /// </summary>
		/// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="path">The path to the action in the toolbar model.</param>
        /// <param name="flags">Flags specifying how the button should respond to being clicked.</param>
        /// <param name="resolver">The action resource resolver used to resolve the action path and icons.</param>
        /// <returns>A <see cref="ClickAction"/>.</returns>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
        {
            return new DropDownButtonAction(actionID, path, flags, resolver);
        }
	}
}
