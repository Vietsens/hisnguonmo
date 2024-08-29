#region License

// Author: Phuongdt

#endregion

using Inventec.Desktop.Core.Tools;
using Inventec.Desktop.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Core
{
    //TODO (CR Mar 2010): name, method names?
    //It's a factory, so Create methods?
    //IViewerSetupHelper?
    public interface IViewerSetupHelper
    {
        ITool[] GetTools(ExtensionInfo extensionInfo);
    }

    public class ViewerSetupHelper : IViewerSetupHelper
    {
        public ViewerSetupHelper()
        {
        }

        public ITool[] Tools { get; set; }

        protected virtual ITool[] GetTools(ExtensionInfo extensionInfo)
        {
            if (Tools != null)
                return Tools;

            try
            {
                object[] extensions = new DesktopToolExtensionPoint().CreateExtensions(o => o.ExtensionClass.AssemblyName == extensionInfo.ExtensionClass.AssemblyName);
                return CollectionUtils.Map(extensions, (object tool) => (ITool)tool).ToArray();
            }
            catch (NotSupportedException)
            {
                Platform.Log(LogLevel.Debug, "No viewer tool extensions found.");
                return new ITool[0];
            }
        }

        #region IViewerSetupHelper Members

        ITool[] IViewerSetupHelper.GetTools(ExtensionInfo extensionInfo)
        {
            return GetTools(extensionInfo);
        }

        #endregion
    }
}
