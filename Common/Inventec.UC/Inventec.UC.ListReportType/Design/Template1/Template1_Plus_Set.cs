using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Design.Template1
{
    internal partial class Template1
    {
        internal bool SetDelegateCreateReport_Click(CreateReport_Click createReport)
        {
            bool result = false;
            try
            {
                this._createReport = createReport;
                if (this._createReport != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => createReport), createReport));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateUpdateDataForPanging(UpdateDataForPaging updateData)
        {
            bool result = false;
            try
            {
                this._updateData = updateData;
                if (this._updateData != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateData), updateData));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDataForGridControl(long ReportTypeGroupId)
        {
            bool result = true;
            try
            {
                this.ReportTypeGroupId = ReportTypeGroupId;
                Data.SearchData Sf = new Data.SearchData();
                SetSearchData(Sf);
                FillDataToGridControl(Sf);
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
