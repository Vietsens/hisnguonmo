#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core.Tools
{
    /// <summary>
    /// Base class for all tool contexts.
    /// </summary>
    /// <remarks>
	/// Developers are encouraged to inherit this class 
	/// rather than implement <see cref="IToolContext"/> directly.
	/// </remarks>
    public abstract class ToolContext : IToolContext
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ToolContext()
		{
		}
    }
}
