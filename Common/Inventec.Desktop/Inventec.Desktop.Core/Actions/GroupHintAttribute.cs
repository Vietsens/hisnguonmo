#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Declares a 'group hint' for an action.
	/// </summary>
	/// <remarks>
	/// Group Hints are used to determine as appropriate a place 
	/// as possible to place an action within an action model.
	/// </remarks>
	public class GroupHintAttribute : ActionDecoratorAttribute
	{
		private readonly string _groupHint;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical Id of the action.</param>
        /// <param name="groupHint">The action's group hint.</param>
		public GroupHintAttribute(string actionID, string groupHint)
			:base(actionID)
		{
			if (groupHint == null)
				groupHint = "";

			_groupHint = groupHint;
		}

		/// <summary>
		/// Sets the <see cref="IAction"/>'s <see cref="GroupHint"/>, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.GroupHint = new GroupHint(_groupHint);
		}
	}
}
