using Inventec.UC.UpLoadFile.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.UpLoadFile
{
    public partial class MainUpLoadFile
    {
        public enum EnumTemplate
        {
            TEMPLATE1
        }

        public UserControl Init(EnumTemplate Template, UpLoadFileToServer UpLoad)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, UpLoad);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
