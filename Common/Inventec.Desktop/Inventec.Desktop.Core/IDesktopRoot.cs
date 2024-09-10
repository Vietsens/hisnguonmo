#region License

// Creater by phuongdt

#endregion

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Defines an desktop entry point.
    /// </summary>
    /// <seealso cref="DesktopRootExtensionPoint"/>
    public interface IDesktopRoot
    {


        /// <summary>
        /// Called by the platform to run the application.
        /// </summary>
        /// <remarks>
        /// It is expected that this method may block for the duration of the application's execution, if
        /// for instance, a GUI event message pump is started.
        /// </remarks>
        object Run(object[] args);
    }
}
