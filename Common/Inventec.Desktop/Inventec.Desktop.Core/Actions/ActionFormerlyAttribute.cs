#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Indicates to the framework that an action used to have one or more different IDs
    /// and should replace the old entries in the action model configuration.
    /// </summary>
    public class ActionFormerlyAttribute : ActionDecoratorAttribute
    {
        private readonly string[] _formerActionIds;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The id of the action.</param>
        /// <param name="formerActionIds">One or more fully qualified "former" action IDs.</param>
        public ActionFormerlyAttribute(string actionID, params string[] formerActionIds) : base(actionID)
        {
            _formerActionIds = formerActionIds;
        }

        public override void Apply(IActionBuildingContext builder)
        {
            foreach (var formerActionId in _formerActionIds)
                builder.Action.FormerActionIDs.Add(formerActionId);
        }
    }
}
