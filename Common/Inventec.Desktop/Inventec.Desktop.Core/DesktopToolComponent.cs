#region License

// Created by phuongdt

#endregion

using Inventec.Desktop.Core.Tools;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Defines an extension point for desktop tools, which are instantiated once per desktop window.
    /// </summary>
    /// <remarks>
    /// Desktop tools are owned by a desktop window. A desktop tool is instantiated once per desktop window.
    /// Extensions should expect to recieve a tool context of type <see cref="IDesktopToolContext"/>.
    /// </remarks>
    [ExtensionPoint()]
    public sealed class DesktopToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Tool context interface provided to tools that extend <see cref="DesktopToolExtensionPoint"/>.
    /// </summary>
    public interface IDesktopToolContext : IToolContext
    {
        
    }
}