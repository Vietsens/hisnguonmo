#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Declares an observer binding for the <see cref="IconSet"/> value of an action.
	/// </summary>
	/// <remarks>
	/// This attribute causes the <see cref="IconSet"/> value of the action specified by the action ID to be
	/// bound to the state of the specified property on the class to which this attribute applies.
	/// The property name must refer to a public <see cref="IconSet"/> property on the target class that has get access.
	/// The change event name must refer to a public event on the class that will fire whenever the value of the property
	/// changes.
	/// </remarks>
	public class IconSetObserverAttribute : StateObserverAttribute
	{
		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
		/// <param name="propertyName">The name of the property to bind to.</param>
		/// <param name="changeEventName">The name of the property change notification event to bind to.</param>
		public IconSetObserverAttribute(string actionId, string propertyName, string changeEventName)
			: base(actionId, propertyName, changeEventName)
		{
		}


		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
		/// <param name="propertyName">The name of the property to bind to.</param>
		public IconSetObserverAttribute(string actionId, string propertyName)
			: base(actionId, propertyName)
		{
		}

		/// <summary>
		/// Binds the <see cref="IAction.IconSet"/> property and <see cref="IAction.IconSetChanged"/> event 
		/// to the corresponding items on the target object, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
		{
			Bind<IconSet>(builder, "IconSet", "IconSetChanged");
		}
	}
}
