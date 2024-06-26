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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisExpMestLaboratory.Resources;

namespace HIS.Desktop.Plugins.HisExpMestLaboratory.UC
{
    public partial class UcQc : UserControl
    {
        int positionHandle = -1;

        public UcQc()
        {
            InitializeComponent();
        }

        private void UcQc_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                List<MOS.EFMODEL.DataModels.HIS_QC_TYPE> dataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_QC_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("QC_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("QC_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("QC_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboQcType, dataSource, controlEditorADO);

                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = cboQcType;
                validate.ErrorText = ResourceLanguageManager.DuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboQcType, validate);

                ControlEditValidationRule validateAmount = new ControlEditValidationRule();
                validateAmount.editor = txtAmount;
                validateAmount.ErrorText = ResourceLanguageManager.DuLieuBatBuoc;
                validateAmount.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtAmount, validateAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcQc.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboQcType.Properties.NullText = Inventec.Common.Resource.Get.Value("UcQc.cboQcType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciQcType.Text = Inventec.Common.Resource.Get.Value("UcQc.lciQcType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("UcQc.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadCboTypeByMachineId(long machineId)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_QC_TYPE> lstQcType = new List<MOS.EFMODEL.DataModels.HIS_QC_TYPE>();
                if (machineId > 0)
                {
                    var lstQcNormation = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_QC_NORMATION>().Where(o => o.MACHINE_ID == machineId).ToList();
                    if (lstQcNormation != null && lstQcNormation.Count > 0)
                    {
                        lstQcType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_QC_TYPE>().Where(o => lstQcNormation.Select(s => s.QC_TYPE_ID).Contains(o.ID)).ToList();
                    }
                }
                cboQcType.EditValue = null;
                cboQcType.Properties.DataSource = lstQcType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void GetValue(ref long qcType, ref long amount)
        {
            try
            {
                if (cboQcType.EditValue != null)
                {
                    qcType = Inventec.Common.TypeConvert.Parse.ToInt64(cboQcType.EditValue.ToString());
                }

                if (!String.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    amount = Inventec.Common.TypeConvert.Parse.ToInt64(txtAmount.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateUc()
        {
            positionHandle = -1;
            return dxValidationProvider1.Validate();
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
