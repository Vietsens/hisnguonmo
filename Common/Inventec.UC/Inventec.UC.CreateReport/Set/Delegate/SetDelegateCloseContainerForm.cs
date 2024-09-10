using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Set.Delegate
{
    class SetDelegateCloseContainerForm : ISetDelegateCloseContainerForm
    {
        public bool SetDelegateClose(System.Windows.Forms.UserControl UC, CloseContainerForm close)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly))
                {
                    Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly UCCreatReport = (Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly)UC;
                    result = UCCreatReport.SetDelegateCloseContainerForm(close);
                    
                }
                else if (UC.GetType()==typeof(Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2))
                {
                    Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2 ucCreateReport = (Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2)UC;
                    result = ucCreateReport.SetDelegateCloseContainerForm(close);
                }
                if (result == false)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => close), close));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
