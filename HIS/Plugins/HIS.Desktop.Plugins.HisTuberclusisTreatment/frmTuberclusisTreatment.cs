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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.UC.WorkPlace;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Common;
using System.IO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.HisTuberclusisTreatment.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.HisTuberclusisTreatment
{
    public partial class frmTuberclusisTreatment : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentTreatment = null;
        long treatmentId = 0;
        DelegateSelectData refeshReference;
        internal int ActionType = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_TUBERCULOSIS_TREAT currentTuberclusisTreatment = null;
        List<ComboADO> hivPatientStatusSelecteds;
        ComboADO comboAdo = new ComboADO();

        int prescriptionArcDay = 0;
        int positionHandle = -1;
        #endregion

        public frmTuberclusisTreatment(Inventec.Desktop.Common.Modules.Module _Module, long treatmentId, DelegateSelectData _refeshReference)
            : base(_Module)
        {
            try
            {
                InitializeComponent();
                this.treatmentId = treatmentId;
                this.refeshReference = _refeshReference;
                this.currentModule = _Module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTuberclusisTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();
                GetTuberclusisTreatment();
                FillDataToControlsForm();
                FillDataToControl();
                btnDel.Enabled = currentTuberclusisTreatment != null;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTuberclusisTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                treatFilter.ID = treatmentId;
                var dataTreat = new BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                if (dataTreat != null && dataTreat.Count > 0)
                {
                    currentTreatment = dataTreat.OrderByDescending(o => o.ID).First();
                }

                if (currentTreatment != null)
                {
                    HisTuberculosisTreatFilter filter = new HisTuberculosisTreatFilter();
                    filter.TREATMENT_ID = currentTreatment.ID;
                    var data = new BackendAdapter(param)
                        .Get<List<HIS_TUBERCULOSIS_TREAT>>("api/HisTuberculosisTreat/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentTuberclusisTreatment = data.OrderByDescending(o => o.ID).First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Init combo
        void FillDataToGridLookupEdit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, object datasource, string Value = "Value", string Name = "Name")
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(Value, "", 30, 1));
                columnInfos.Add(new ColumnInfo(Name, "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(Name, Value, columnInfos, false, 330);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataToControl()
        {
            try
            {
                if (currentTreatment != null)
                {
                    lblTreatmentCode.Text = currentTreatment.TREATMENT_CODE;
                    lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
                    lblPatientGenderName.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
                    if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB);
                    }
                    lblHeinCardNumber.Text = currentTreatment.TDL_HEIN_CARD_NUMBER;
                    if (currentTreatment.TDL_PATIENT_CCCD_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CCCD_NUMBER;
                    }
                    else if (currentTreatment.TDL_PATIENT_CMND_NUMBER != null)
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_CMND_NUMBER;
                    }
                    else
                    {
                        lblPatientCCCDNumber.Text = currentTreatment.TDL_PATIENT_PASSPORT_NUMBER;
                    }
                    lblPatientAddress.Text = currentTreatment.TDL_PATIENT_ADDRESS;

                    if (currentTuberclusisTreatment != null)
                    {
                        cboTuberculosisClassifyLocation.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_CLASSIFY_LOCATION;
                        cboTuberculosisClassifyTs.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_CLASSIFY_TS;
                        cboTuberculosisClassifyHiv.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_CLASSIFY_HIV;
                        cboTuberculosisClassifyVk.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_CLASSIFY_VK;
                        cboTuberculosisClassifyKt.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_CLASSIFY_KT;
                        cboTuberculosisTreatmentType.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_TREATMENT_TYPE;
                        cboRegimenTuberculosis.EditValue = currentTuberclusisTreatment.REGIMEN_TUBERCULOSIS_CODE;
                        cboTuberculosisTreatmentResult.EditValue = currentTuberclusisTreatment.TUBERCULOSIS_TREATMEN_RESULT;
                        if (currentTuberclusisTreatment.BEGIN_TIME.HasValue)
                            dteTuberculosisTreatmentBegin.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTuberclusisTreatment.BEGIN_TIME ?? 0) ?? DateTime.Now;
                        if (currentTuberclusisTreatment.END_TIME.HasValue)
                            dteTuberculosisTreatmentEnd.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTuberclusisTreatment.END_TIME ?? 0) ?? DateTime.Now;
                        if (currentTuberclusisTreatment.HIV_KD_DATE.HasValue)
                            dteHivKdDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTuberclusisTreatment.HIV_KD_DATE ?? 0) ?? DateTime.Now;
                        if (currentTuberclusisTreatment.ARV_BEGIN_TIME.HasValue)
                            dteArvTreatmentBegin.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTuberclusisTreatment.ARV_BEGIN_TIME ?? 0) ?? DateTime.Now;
                        if (currentTuberclusisTreatment.CTX_BEGIN_TIME.HasValue)
                            dteCtxTreatmentBegin.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTuberclusisTreatment.CTX_BEGIN_TIME ?? 0) ?? DateTime.Now;
                    }
                    else
                    {

                        cboTuberculosisClassifyLocation.EditValue = null;
                        cboTuberculosisClassifyTs.EditValue = null;
                        cboTuberculosisClassifyHiv.EditValue = null;
                        cboTuberculosisClassifyVk.EditValue = null;
                        cboTuberculosisClassifyKt.EditValue = null;
                        cboTuberculosisTreatmentType.EditValue = null;
                        cboRegimenTuberculosis.EditValue = null;
                        cboTuberculosisTreatmentResult.EditValue = null;
                        dteTuberculosisTreatmentBegin.EditValue = null;
                        dteTuberculosisTreatmentEnd.EditValue = null;
                        dteHivKdDate.EditValue = null;
                        dteArvTreatmentBegin.EditValue = null;
                        dteCtxTreatmentBegin.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void FillDataToControlsForm()
        {
            try
            {
                FillDataToGridLookupEdit(this.cboTuberculosisClassifyLocation, comboAdo.ListTuberculosisClassifyLocation());
                FillDataToGridLookupEdit(this.cboTuberculosisClassifyTs, comboAdo.ListTuberculosisClassifyTs());
                FillDataToGridLookupEdit(this.cboTuberculosisClassifyHiv, comboAdo.ListTuberculosisClassifyHiv());
                FillDataToGridLookupEdit(this.cboTuberculosisClassifyVk, comboAdo.ListTuberculosisClassifyVk());
                FillDataToGridLookupEdit(this.cboTuberculosisClassifyKt, comboAdo.ListTuberculosisClassifyKt());
                FillDataToGridLookupEdit(this.cboTuberculosisTreatmentType, comboAdo.ListTuberculosisTreatmentType());
                FillDataToGridLookupEdit(this.cboTuberculosisTreatmentResult, comboAdo.ListTuberculosisTreatmentResult());
                FillDataToGridLookupEdit(this.cboRegimenTuberculosis, BackendDataWorker.Get<HIS_REGIMEN_TUBERCULOSIS>().ToList(), "REGIMEN_TUBERCULOSIS_CODE", "REGIMEN_TUBERCULOSIS_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (dteTuberculosisTreatmentBegin.EditValue != null && dteTuberculosisTreatmentBegin.DateTime != DateTime.MinValue && dteTuberculosisTreatmentEnd.EditValue != null && dteTuberculosisTreatmentEnd.DateTime != DateTime.MinValue)
                {
                    var begin = Int64.Parse(dteTuberculosisTreatmentBegin.DateTime.ToString("yyyyMMdd"));
                    var end = Int64.Parse(dteTuberculosisTreatmentEnd.DateTime.ToString("yyyyMMdd"));
                    if(begin > end) {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian bắt đầu điều trị lao không được lớn hơn thời gian kết thúc điều trị lao", "Thông báo");
                        return;
                    }
                }    
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT updateDTO = new MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT();
                if (currentTreatment != null)
                {
                    if (this.currentTuberclusisTreatment != null && this.currentTuberclusisTreatment.ID > 0)
                    {
                        LoadCurrent(this.currentTuberclusisTreatment.ID, ref updateDTO);
                    }
                    UpdateDTOFromDataForm(ref updateDTO);

                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                if (this.currentTuberclusisTreatment != null && this.currentTuberclusisTreatment.ID > 0)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT>("api/HisTuberculosisTreat/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        this.currentTuberclusisTreatment = resultData;
                        FillDataToControl();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT>("api/HisTuberculosisTreat/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        this.currentTuberclusisTreatment = resultData;
                        FillDataToControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_TUBERCULOSIS_TREAT>();
                    btnDel.Enabled = true;
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTuberculosisTreatFilter filter = new HisTuberculosisTreatFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT>>("api/HisTuberculosisTreat/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_TUBERCULOSIS_TREAT TuberclusisTreatmentDTO)
        {
            try
            {
                TuberclusisTreatmentDTO.TREATMENT_ID = currentTreatment.ID;
                TuberclusisTreatmentDTO.TUBERCULOSIS_CLASSIFY_LOCATION = cboTuberculosisClassifyLocation.EditValue != null ? cboTuberculosisClassifyLocation.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_CLASSIFY_TS = cboTuberculosisClassifyTs.EditValue != null ? cboTuberculosisClassifyTs.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_CLASSIFY_HIV = cboTuberculosisClassifyHiv.EditValue != null ? cboTuberculosisClassifyHiv.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_CLASSIFY_VK = cboTuberculosisClassifyVk.EditValue != null ? cboTuberculosisClassifyVk.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_CLASSIFY_KT = cboTuberculosisClassifyKt.EditValue != null ? cboTuberculosisClassifyKt.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_TREATMENT_TYPE = cboTuberculosisTreatmentType.EditValue != null ? cboTuberculosisTreatmentType.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.TUBERCULOSIS_TREATMEN_RESULT = cboTuberculosisTreatmentResult.EditValue != null ? cboTuberculosisTreatmentResult.EditValue.ToString() : null;
                TuberclusisTreatmentDTO.REGIMEN_TUBERCULOSIS_CODE = cboRegimenTuberculosis.EditValue != null ? cboRegimenTuberculosis.EditValue.ToString() : null;


                if (dteTuberculosisTreatmentBegin.EditValue != null && dteTuberculosisTreatmentBegin.DateTime != DateTime.MinValue)
                    TuberclusisTreatmentDTO.BEGIN_TIME = Int64.Parse(dteTuberculosisTreatmentBegin.DateTime.ToString("yyyyMMdd000000"));
                else
                    TuberclusisTreatmentDTO.BEGIN_TIME = null;

                if (dteTuberculosisTreatmentEnd.EditValue != null && dteTuberculosisTreatmentEnd.DateTime != DateTime.MinValue)
                    TuberclusisTreatmentDTO.END_TIME = Int64.Parse(dteTuberculosisTreatmentEnd.DateTime.ToString("yyyyMMdd000000"));
                else
                    TuberclusisTreatmentDTO.END_TIME = null;


                if (dteHivKdDate.EditValue != null && dteHivKdDate.DateTime != DateTime.MinValue)
                    TuberclusisTreatmentDTO.HIV_KD_DATE = Int64.Parse(dteHivKdDate.DateTime.ToString("yyyyMMdd000000"));
                else
                    TuberclusisTreatmentDTO.HIV_KD_DATE = null;


                if (dteArvTreatmentBegin.EditValue != null && dteArvTreatmentBegin.DateTime != DateTime.MinValue)
                    TuberclusisTreatmentDTO.ARV_BEGIN_TIME = Int64.Parse(dteArvTreatmentBegin.DateTime.ToString("yyyyMMdd000000"));
                else
                    TuberclusisTreatmentDTO.ARV_BEGIN_TIME = null;


                if (dteCtxTreatmentBegin.EditValue != null && dteCtxTreatmentBegin.DateTime != DateTime.MinValue)
                    TuberclusisTreatmentDTO.CTX_BEGIN_TIME = Int64.Parse(dteCtxTreatmentBegin.DateTime.ToString("yyyyMMdd000000"));
                else
                    TuberclusisTreatmentDTO.CTX_BEGIN_TIME = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void cboTuberculosisClassifyLocation_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dteTuberculosisTreatmentBegin_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                DateEdit dte = sender as DateEdit;
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dte.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                bool rs = false;
                
                CommonParam param = new CommonParam();
                if (this.btnDel.Enabled == false) return;
                if (MessageBox.Show(this, "Xóa thông tin điều trị bệnh lao?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var ID = currentTuberclusisTreatment.ID;
                    rs = new BackendAdapter(param).Post<bool>("api/HisTuberculosisTreat/Delete", ApiConsumers.MosConsumer, ID, param);
                    
                    MessageManager.Show(this,param,rs);
                    if (rs)
                    {
                        foreach (Control control in this.layoutControl3.Controls)
                        {
                            control.Text = "";
                        }
                        btnDel.Enabled = false;
                        currentTuberclusisTreatment = null;
                    }
                }
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
