#region License

// Created by phuongdt

#endregion

using System.Collections.Generic;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Provides methods for processing the set of action attributes declared on a given target
    /// object, which is typically a tool.
    /// </summary>
    internal static class ActionAttributeProcessor
    {
        /// <summary>
        /// Processes the set of action attributes declared on a given target object to generate the
        /// corresponding set of <see cref="IAction"/> objects.
        /// </summary>
        /// <param name="actionTarget">The target object on which the attributes are declared, typically a tool.</param>
        /// <returns>The resulting set of actions, where each action is bound to the target object.</returns>
        internal static IAction[] Process(object actionTarget)
        {
            object[] attributes = actionTarget.GetType().GetCustomAttributes(typeof(ActionAttribute), true);

            // first pass - create an ActionBuilder for each initiator of the specified type
            List<ActionBuildingContext> actionBuilders = new List<ActionBuildingContext>();
            foreach (ActionAttribute a in attributes)
            {
                if (a is ActionInitiatorAttribute)
                {
                    ActionBuildingContext actionBuilder = new ActionBuildingContext(a.QualifiedActionID(actionTarget), actionTarget);
                    a.Apply(actionBuilder);
                    actionBuilders.Add(actionBuilder);
                }
            }

            // second pass - apply decorators to all ActionBuilders with same actionID
            foreach (ActionAttribute a in attributes)
            {
                if (a is ActionDecoratorAttribute)
                {
                    foreach (ActionBuildingContext actionBuilder in actionBuilders)
                    {
                        if (a.QualifiedActionID(actionTarget) == actionBuilder.ActionID)
                        {
                            a.Apply(actionBuilder);
                        }
                    }
                }
            }

            List<IAction> actions = new List<IAction>();
            foreach (ActionBuildingContext actionBuilder in actionBuilders)
            {
                actions.Add(actionBuilder.Action);
            }

            return actions.ToArray();
        }
    }
}
