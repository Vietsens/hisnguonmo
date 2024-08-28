using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Development.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainDevelopment.EmumTemp enumT, string contentDonVi, string contentPhatTrien)
        {
            UserControl result = null;
            try
            {
                if (enumT == MainDevelopment.EmumTemp.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(contentDonVi, contentPhatTrien);
                }

                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Content truyen vao luc Khoi tao UserControl Phat trien san pham: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contentDonVi), contentDonVi));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contentPhatTrien), contentPhatTrien));
                }
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
