#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.
    /// </summary>
    /// <remarks>
	/// Tool classes should inherit this class rather than implement <see cref="ITool"/> directly.
	/// </remarks>
    public abstract class Tool<TContextInterface> : ToolBase
        where TContextInterface: IToolContext
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected Tool()
		{
		}

    	/// <summary>
        /// Provides a typed reference to the context in which the tool is operating.
        /// </summary>
        /// <remarks>
		/// Attempting to access this property before <see cref="ITool.SetContext"/> has 
		/// been called (e.g in the constructor of this tool) will return null.
		/// </remarks>
        protected TContextInterface Context
        {
            get { return (TContextInterface)this.ContextBase; }
        }
    }
}
