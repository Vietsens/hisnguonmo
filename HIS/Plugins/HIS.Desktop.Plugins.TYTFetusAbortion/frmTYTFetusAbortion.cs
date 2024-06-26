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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;
using TYT.Filter;

namespace TYT.Desktop.Plugins.FetusAbortion
{
    public partial class frmTYTFetusAbortion : FormBase
    {

        public enum TYPE
        {
            CREATE,
            UPDATE
        }

        TYPE actionType = TYPE.CREATE;
        int positionHandleControl = -1;
        V_HIS_PATIENT patient { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        TYT_FETUS_ABORTION fetusAbortion { get; set; }
        List<TYT_FETUS_ABORTION> lstfetusAbortion;
        string PatientCode { get; set; }
        DelegateSelectData refeshData { get; set; }

        public frmTYTFetusAbortion(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_PATIENT patient)
        {
            InitializeComponent();
            try
            {
                this.patient = patient;
                this.moduleData = moduleData;
                PatientCode = patient.PATIENT_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmTYTFetusAbortion(Inventec.Desktop.Common.Modules.Module moduleData, long fetusAbortionId, DelegateSelectData refeshData)
        {
            InitializeComponent();
            try
            {
                CommonParam param = new CommonParam();
                TytFetusAbortionFilter filter = new TytFetusAbortionFilter();
                filter.ID = fetusAbortionId;
                var lstfetusAbortion = new BackendAdapter(param)
                  .Get<List<TYT_FETUS_ABORTION>>("api/TytFetusAbortion/Get", ApiConsumers.TytConsumer, filter, param);
                if (lstfetusAbortion != null)
                {
                    this.PatientCode = lstfetusAbortion.FirstOrDefault().PATIENT_CODE;
                    fetusAbortion = lstfetusAbortion.FirstOrDefault();
                }
                LoadDataToControl();
                actionType = TYPE.UPDATE;
                Loadpatient();
                this.moduleData = moduleData;
                this.refeshData = refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Loadpatient()
        {
            CommonParam param = new CommonParam();
            HisPatientFilter filter = new HisPatientFilter();
            filter.PATIENT_CODE__EXACT = this.PatientCode;
            var lstpatient = new BackendAdapter(param)
              .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
            if (lstpatient != null)
            {
                this.patient = lstpatient.FirstOrDefault();
            }
        }

        private void frmTYTFetusAbortion_Load(object sender, EventArgs e)
        {
            try
            {
                //Load Icon
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                LoadDataToGrid();
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
                LoadValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadValidate()
        {
            ValidationControlMaxLength(mmAbortionMethod, 100);
            ValidationControlMaxLength(mmObstetricComplication, 100);
            ValidationControlMaxLength(txtExecuteName, 100);
            ValidationControlMaxLength(mmNote, 100);
        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = false;
            validate.ErrorText = "Nhập quá kí tự cho phép";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validate);
        }
        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;
                TYT_FETUS_ABORTION fetusAbortion = new TYT_FETUS_ABORTION();
                MakeDataFetusAbortion(ref fetusAbortion);

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param)
                    .Post<TYT_FETUS_ABORTION>("api/TytFetusAbortion/Update", ApiConsumers.TytConsumer, fetusAbortion, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    PatientCode = result.PATIENT_CODE;
                    this.fetusAbortion = result;
                    InitLabelSave();
                    LoadDataToGrid();

                    if (refeshData != null)
                    {
                        this.refeshData(result);
                    }
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboThang_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDiagnoseTest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDiagnoseTest.Properties.Buttons[1].Visible = false;
                    cboDiagnoseTest.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSMResult_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSMResult.Properties.Buttons[1].Visible = false;
                    cboSMResult.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDiagnoseTest_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboDiagnoseTest.EditValue != null)
                {
                    cboDiagnoseTest.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSMResult_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboSMResult.EditValue != null)
                {
                    cboSMResult.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLastMensesTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtAbortionTime.Focus();
                    dtAbortionTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSingle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaChildCount.Focus();
                    spinParaChildCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinParaChildCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExecuteName.Focus();
                    txtExecuteName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDiagnoseTest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSMResult.Focus();
                    cboSMResult.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSMResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmAbortionMethod.Focus();
                    mmAbortionMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsDeath_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkExamAfterTwoWeek.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamAfterTwoWeek_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSingle.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExecuteName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmNote.Focus();
                    mmNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAbortionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDiagnoseTest.Focus();
                    cboDiagnoseTest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    TYT_FETUS_ABORTION data = (TYT_FETUS_ABORTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "LAST_MENSES_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.LAST_MENSES_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "ABORTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.ABORTION_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DiagnoseTest")
                    {
                        if (data.DIAGNOSE_TEST.HasValue)
                        {
                            if (data.DIAGNOSE_TEST == 0)
                            {
                                e.Value = "Xét nghiệm có thai";
                            }
                            else
                            {
                                e.Value = "Xét nghiệm không có thai";
                            }
                        }
                    }
                    else if (e.Column.FieldName == "SMResult")
                    {
                        if (data.SM_RESULT.HasValue)
                        {
                            if (data.SM_RESULT == 1)
                            {
                                e.Value = "Có tổ chức mổ thai";
                            }
                            else
                            {
                                e.Value = "Không có tổ chức mổ thai";
                            }
                        }
                    }
                    else if (e.Column.FieldName == "Death")
                    {
                        if (data.IS_DEATH == 1)
                        {
                            e.Value = true;
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    else if (e.Column.FieldName == "ExamAfterTwoWeek")
                    {
                        if (data.EXAM_AFTER_TWO_WEEK == 1)
                        {
                            e.Value = true;
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    else if (e.Column.FieldName == "Single")
                    {
                        if (data.IS_SINGLE == 1)
                        {
                            e.Value = true;
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    // gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            fetusAbortion = (TYT_FETUS_ABORTION)gridView1.GetFocusedRow();
            actionType = TYPE.UPDATE;
            LoadDataToControl();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                btnReset_Click(null, null);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtLastMensesTime.EditValue = null;
            dtAbortionTime.EditValue = null;
            cboDiagnoseTest.EditValue = null;
            cboSMResult.EditValue = null;
            mmAbortionMethod.EditValue = null;
            mmObstetricComplication.EditValue = null;
            chkIsDeath.Checked = false;
            chkExamAfterTwoWeek.Checked = false;
            chkIsSingle.Checked = false;
            spinParaChildCount.EditValue = null;
            txtExecuteName.EditValue = null;
            mmNote.EditValue = null;
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
            actionType = TYPE.CREATE;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;
                TYT_FETUS_ABORTION fetusAbortion = new TYT_FETUS_ABORTION();
                MakeDataFetusAbortion(ref fetusAbortion);

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param)
                    .Post<TYT_FETUS_ABORTION>("api/TytFetusAbortion/Create", ApiConsumers.TytConsumer, fetusAbortion, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    PatientCode = result.PATIENT_CODE;
                    this.fetusAbortion = result;
                    InitLabelSave();
                    LoadDataToGrid();
                    btnReset_Click(null, null);
                    if (refeshData != null)
                    {
                        this.refeshData(result);
                    }
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rbtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (TYT_FETUS_ABORTION)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/TytFetusAbortion/Delete", ApiConsumers.TytConsumer, rowData.ID, param);
                    if (success)
                    {
                        btnReset_Click(null, null);
                        LoadDataToGrid();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex) { }
        }
    }
}
