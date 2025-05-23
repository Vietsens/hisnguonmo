#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// Represents the root node of an action model.
    /// </summary>
    public class ActionModelRoot : ActionModelNode
    {
		private readonly string _site;
		
		/// <summary>
        /// Creates the action model with the specified namespace and site, using the specified
        /// set of actions as input.
        /// </summary>
        /// <remarks>
        /// If an action model specification for the namespace/site
        /// does not exist, it will be created.  If it does exist, it will be used as guidance
        /// in constructing the action model tree.
        /// </remarks>
        /// <param name="namespace">A namespace to qualify the site, typically the class name of the calling class is a good choice.</param>
        /// <param name="site">The site (<see cref="ActionPath.Site"/>).</param>
        /// <param name="actions">The set of actions from which to construct the model.</param>
        /// <returns>An action model tree.</returns>
        public static ActionModelRoot CreateModel(string @namespace, string site, IActionSet actions)
        {
            return ActionModelSettings.DefaultInstance.BuildAndSynchronize(@namespace, site, actions.Select(delegate(IAction action) { return action.Path.Site == site; }));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionModelRoot()
            :this(null)
        {
        }

        /// <summary>
		/// Constructor.
        /// </summary>
        /// <param name="site">The site to which this model corresponds.</param>
        public ActionModelRoot(string site)
            : base(null)
        {
            _site = site;
        }

        /// <summary>
        /// Gets the site (the first component of the path).
        /// </summary>
        public string Site
        {
            get { return _site; }
        }

        /// <summary>
        /// Inserts the specified actions into this model in the specified order.
        /// </summary>
        /// <param name="actions">The actions to insert.</param>
        public void InsertActions(IAction[] actions)
        {
            foreach (IAction action in actions)
            {
                InsertAction(action);
            }
        }

        /// <summary>
        /// Inserts the specified action into this model.
        /// </summary>
        /// <param name="action">The action to insert.</param>
        public void InsertAction(IAction action)
        {
			Insert(action.Path, 1,
				delegate(PathSegment segment)
				{
					return new ActionNode(segment, action);
				});
		}

		/// <summary>
		/// Inserts a separator into the action model at the specified path.
		/// </summary>
		/// <param name="separatorPath"></param>
		public void InsertSeparator(Path separatorPath)
		{
			Insert(separatorPath, 1,
				delegate(PathSegment segment)
				{
					return new SeparatorNode(segment);
				});
		}

		/// <summary>
        /// Used by the <see cref="ActionModelNode.CloneTree"/> method.
        /// </summary>
        /// <param name="pathSegment">The path segment which this node represents.</param>
        /// <returns>A new node of this type.</returns>
        protected override ActionModelNode CloneNode(PathSegment pathSegment)
        {
            return new ActionModelRoot();
        }
    }
}
