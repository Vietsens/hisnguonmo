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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Core;
using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.Config;
using Inventec.UC.CreateReport.Mrs.Create;
using MSS.APP.Modules.Report.CreateReport.UseMedicineReport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly
{
    internal partial class TemplateBetweenTimeFilterOnly
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
                Inventec.UC.CreateReport.Mrs.Create.SarReportCreate sarReportCreate = new Mrs.Create.SarReportCreate(param);
                setDataToFilterTotal();
                sarReportCreate.Behavior = new SarReportCreateBehaviorDefault(param, sarReport);
                success = new Mrs.Create.SarReportCreateFactory(param).createFactory(sarReport).Create(sarReport);                 if (_HasException != null) _HasException(param);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
        private void txtReportTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    CreateReportProcess.LoadReportTemplateCombo(strValue, false, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboReportTemplate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboReportTemplate.EditValue != null)
                    {
                        SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE data = GlobalStore.reportTemplates.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboReportTemplate.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtReportTemplateCode.Text = data.REPORT_TEMPLATE_CODE;
                            txtReportName.Focus();
                            txtReportName.SelectAll();
                        }
                    }
                    else
                    {
                        txtReportName.Focus();
                        txtReportName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtReportName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTimeFrom.Focus();
                    dtTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtTimeTo.Focus();
                    dtTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }       
        private void dtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }               
    }
}
