#region License

// Creater by phuongdt

#endregion

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// An abstract base class for extension filters.  
    /// </summary>
    /// <remarks>
	/// Extension filters are used to filter the extension points returned by 
	/// one of the <b>CreateExtensions</b> methods.  Subclasses of this
	/// class implement specific types of filters.
	/// </remarks>
    public abstract class ExtensionFilter
    {
        /// <summary>
        /// Tests the specified extension against the criteria of this filter.
        /// </summary>
        /// <param name="extension">The extension to test.</param>
        /// <returns>True if the extension meets the criteria, false otherwise.</returns>
        public abstract bool Test(ExtensionInfo extension);
    }
}
