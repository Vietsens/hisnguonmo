#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
    /// <summary>
    /// A subclass of <see cref="Path"/> that is used by <see cref="IAction"/> to represent an action path.
    /// </summary>
    public class ActionPath : Path
    {
        /// <summary>
        /// Constructs an action path from the specified path string, using the specified resource resolver.
        /// </summary>
        /// <param name="pathString">A string respresenting the path.</param>
        /// <param name="resolver">A resource resolver used to localize each path segment. If
        /// the resource resolver is null, the path segments will be treated as localized text.</param>
        public ActionPath(string pathString, IResourceResolver resolver)
            :base(pathString, resolver)
        {
        }

        /// <summary>
        /// Gets the action site (the first segment of the action path).
        /// </summary>
        public string Site
        {
            get { return this.Segments.Count > 0 ? this.Segments[0].ResourceKey : null; }
        }
    }
}
