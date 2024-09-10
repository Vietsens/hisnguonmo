using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("Inventec.Common.FlexCelPrint.Resources.Lang", typeof(Inventec.Common.FlexCelPrint.frmSetupPrintPreviewMerge).Assembly);

        internal static System.Resources.ResourceManager languageMessageTemplateKey = new System.Resources.ResourceManager("Inventec.Common.FlexCelPrint.Resources.Lang", typeof(Inventec.Common.FlexCelPrint.TemplateKey.PreviewTemplateKey).Assembly);
    }
}
