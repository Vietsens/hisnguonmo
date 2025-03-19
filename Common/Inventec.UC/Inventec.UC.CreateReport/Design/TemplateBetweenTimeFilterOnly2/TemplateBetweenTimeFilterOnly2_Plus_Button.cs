/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.Utils;
using Inventec.Core;
using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.Config;
using Inventec.UC.CreateReport.Mrs.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2
{
    internal partial class TemplateBetweenTimeFilterOnly2
    {

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                waitLoad = new WaitDialogForm(MessageUtil.GetMessage(MessageLang.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));
                sarReport = new MRS.SDO.CreateReportSDO();
                sarReport.ReportTypeCode = GlobalStore.reportType.REPORT_TYPE_CODE;
                sarReport.ReportTemplateCode = GlobalStore.reportTemplates.Where(o => o.ID == (long)(cboReportTemplate.EditValue)).FirstOrDefault().REPORT_TEMPLATE_CODE;
                sarReport.ReportName = txtReportName.Text;
                sarReport.Description = txtDescription.Text;
                sarReport.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sarReport.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Data.SaveClickSDO sdo = new Data.SaveClickSDO();
                sdo.TimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));
                sdo.TimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                if (this._GetFilter != null)
                {
                    sarReport.Filter = this._GetFilter(sdo);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Delegate GetObjectFilter chua duoc set." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _GetFilter), _GetFilter));
                }
                Inventec.Common.Logging.LogSystem.Debug("Bat dau goi Api tao bao cao: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sarReport), sarReport));
                Inventec.UC.CreateReport.Mrs.Create.SarReportCreate sarReportCreate = new Mrs.Create.SarReportCreate(param);
                sarReportCreate.Behavior = new SarReportCreateBehaviorDefault(param, sarReport);
                success = new Mrs.Create.SarReportCreateFactory(param).createFactory(sarReport).Create(sarReport); if (_HasException != null) _HasException(param);
                waitLoad.Dispose();
                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion
                if (success)
                {
                    if (_CloseContainerForm != null) _CloseContainerForm();
                }
                #region Process has exception
                if (_HasException != null) _HasException(param);
                #endregion
            }
            catch (Exception ex)
            {
                waitLoad.Dispose();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
