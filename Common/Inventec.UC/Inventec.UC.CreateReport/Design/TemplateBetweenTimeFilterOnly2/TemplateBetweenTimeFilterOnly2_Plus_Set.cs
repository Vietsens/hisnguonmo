using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2
{
    internal partial class TemplateBetweenTimeFilterOnly2
    {

        internal bool SetDelegateHasException(ProcessHasException hasException)
        {

            bool result = false;
            try
            {
                this._HasException = hasException;
                if (this._HasException != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasException), hasException));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateCloseContainerForm(CloseContainerForm closeContainerForm)
        {

            bool result = false;
            try
            {
                this._CloseContainerForm = closeContainerForm;
                if (this._CloseContainerForm != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => closeContainerForm), closeContainerForm));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateGetObjectFilter(GetObjectFilter getFilter)
        {
            bool result = false;
            try
            {
                this._GetFilter = getFilter;
                if (_GetFilter != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong Set Duoc delegate cho GetObjectFilter. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => getFilter), getFilter));
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
