#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a menu containing other <see cref="IAction"/>s.
	/// </summary>
	public interface IDropDownAction : IAction
	{
		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		ActionModelNode DropDownMenuModel { get; }
	}
}
