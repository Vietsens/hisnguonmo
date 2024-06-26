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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Base;
using Inventec.Desktop.Common.LanguageManager;
using AutoMapper;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Resources;
using MOS.Filter;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class RehaServiceReqExecuteControl : HIS.Desktop.Utility.UserControlBase
    {
        #region Declaretion
        internal int action = -1;
        int positionHandle = -1;
        internal long treatmentId = 0;
        internal MPS.ADO.PatientADO currentPatient { get; set; }
        internal HIS_TREATMENT Treatment { get; set; }
        internal V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO { get; set; }
        internal HisRehaServiceReqUpdateSDO currentHisRehaResultSDO { get; set; }
        internal V_HIS_SERE_SERV_12 SereServ;
        internal List<V_HIS_SERE_SERV_12> SereServs;
        internal V_HIS_SERE_SERV_REHA currentSereServReha;
        internal List<SereServRehaADO> sereServRehas { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModuleData { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        #endregion

        public RehaServiceReqExecuteControl()
        {
            InitializeComponent();
        }

        public RehaServiceReqExecuteControl(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ _serReqData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.HisServiceReqWithOrderSDO = _serReqData;
                this.currentModuleData = moduleData;
                this.treatmentId = _serReqData.TREATMENT_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RehaServiceReqExecuteControl_Load(object sender, EventArgs e)
        {
            try
            {
                InitLanguage();
                LoadTreatment();
                LoadPatientTypeAlter();
                this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                ComboChuanDoanTD(cboIcds);
                RehaServiceReqExecuteControlProcess.LoadEnableAndDisableButton(this);
                if (this.HisServiceReqWithOrderSDO != null)
                {
                    LoadDropDownButtonOther();
                    FillDataToGridSereServSurgServiceReq(HisServiceReqWithOrderSDO);
                    FillDataDetailRehaServiceReq(HisServiceReqWithOrderSDO);
                }

                EnableButtonByServiceReqStt(this.HisServiceReqWithOrderSDO.SERVICE_REQ_STT_ID);
                FillDataToButtonPrintReha();
                txtSymptom_Before.Focus();
                txtSymptom_Before.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatmentId;
                Treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDropDownButtonOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                if (Treatment.IS_PAUSE != 1)
                {
                    string assignServiceCaption = "";
                    if (ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_TAB) == 1)
                    {
                        menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_BEDROOMPARTIALCONTROL_TMS_ASSIGN_SERVICE_PLUS", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnAssignServicePlus_Click)));
                        assignServiceCaption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_BEDROOMPARTIALCONTROL_TMS_ASSIGN_MEDI_MATY", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                    }
                    else
                    {
                        assignServiceCaption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_DIIM_SERVICE_REQ_EXCUTE_CHI_DINH_DICH_VU", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                    }
                    menu.Items.Add(new DXMenuItem(assignServiceCaption, new EventHandler(btnAssignService_Click)));
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_DAU_HIEU_SINH_TON", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDhst_Click)));
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_CHUYEN_KHOA", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnDepartmentTran_Click)));
                    //Close MediRecord
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_KET_THUC_DIEU_TRI", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(btnTreatmentClose_Click)));
                }

                //ddbtnOther.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshData()
        {
            try
            {
                gridControlRehaTrain.DataSource = RehaServiceReqExecuteControlProcess.LoadDataRehaTrainForSereServReha(this.currentSereServReha, this.HisServiceReqWithOrderSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Save ket qua tham do chuc nang
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderControl.Validate())
                    return;

                HisRehaServiceReqUpdateSDO hisSurgResultSDO = new MOS.SDO.HisRehaServiceReqUpdateSDO();

                ProcessRehaResult(hisSurgResultSDO);
                SaveRehaResult(hisSurgResultSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessRehaResult(HisRehaServiceReqUpdateSDO fuexResultSDO)
        {
            try
            {
                fuexResultSDO.ServiceReqId = this.HisServiceReqWithOrderSDO.ID;
                fuexResultSDO.SymptomBefore = txtSymptom_Before.Text;
                fuexResultSDO.SymptomAfter = txtSymptom_After.Text;
                fuexResultSDO.RespiratoryBefore = txtRespiratory_Before.Text;
                fuexResultSDO.RespiratoryAfter = txtRespiratory_After.Text;
                fuexResultSDO.EcgBefore = txtECG_Before.Text;
                fuexResultSDO.EcgAfter = txtECG_After.Text;
                fuexResultSDO.Advise = txtAdvice.Text;

                fuexResultSDO.IcdName = !String.IsNullOrEmpty(txtIcdMainText.Text) ? txtIcdMainText.Text : cboIcds.EditValue != null ? cboIcds.Text : "";

                if (!String.IsNullOrEmpty(txtIcdMainCode.Text))
                {
                    fuexResultSDO.IcdCode = txtIcdMainCode.Text;
                }

                if (!string.IsNullOrEmpty(txtIcdExtraCode.Text))
                {
                    fuexResultSDO.IcdSubCode = txtIcdExtraCode.Text;
                }

                if (!string.IsNullOrEmpty(txtIcdExtraName.Text))
                {
                    fuexResultSDO.IcdText = txtIcdExtraName.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveRehaResult(HisRehaServiceReqUpdateSDO fuexResultSDO)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                currentHisRehaResultSDO = new BackendAdapter(param)
                    .Post<HisRehaServiceReqUpdateSDO>("api/HisServiceReq/RehaUpdate", ApiConsumers.MosConsumer, fuexResultSDO, param);

                if (currentHisRehaResultSDO != null)
                {
                    success = true;
                    this.HisServiceReqWithOrderSDO.ADVISE = currentHisRehaResultSDO.Advise;
                    this.HisServiceReqWithOrderSDO.ECG_AFTER = currentHisRehaResultSDO.EcgAfter;
                    this.HisServiceReqWithOrderSDO.ECG_BEFORE = currentHisRehaResultSDO.EcgBefore;
                    this.HisServiceReqWithOrderSDO.ICD_CODE = currentHisRehaResultSDO.IcdCode;
                    this.HisServiceReqWithOrderSDO.ICD_NAME = currentHisRehaResultSDO.IcdName;
                    this.HisServiceReqWithOrderSDO.ICD_SUB_CODE = currentHisRehaResultSDO.IcdSubCode;
                    this.HisServiceReqWithOrderSDO.ICD_TEXT = currentHisRehaResultSDO.IcdText;
                    this.HisServiceReqWithOrderSDO.RESPIRATORY_AFTER = currentHisRehaResultSDO.RespiratoryAfter;
                    this.HisServiceReqWithOrderSDO.RESPIRATORY_BEFORE = currentHisRehaResultSDO.RespiratoryBefore;
                    this.HisServiceReqWithOrderSDO.SYMPTOM_AFTER = currentHisRehaResultSDO.SymptomAfter;
                    this.HisServiceReqWithOrderSDO.SYMPTOM_BEFORE = currentHisRehaResultSDO.SymptomBefore;
                    //btnPrint.Enabled = true;
                }
                else
                {
                    LogSystem.Warn("Du lieu truyen vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisRehaResultSDO), currentHisRehaResultSDO));
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Log ket qua vao Db
        void SuccessLog(V_HIS_SERVICE_REQ result)
        {
            try
            {
                if (result != null)
                {
                    //string message = String.Format(EventLogUtil.SetLog(His.EventLog.Message.Enum.TraKetQuaDichVuPHCN), result.ID, result.SERVICE_REQ_CODE, result.FINISH_TIME, result.EXECUTE_LOGINNAME, result.EXECUTE_USERNAME, result.TREATMENT_CODE, result.PATIENT_CODE, result.VIR_PATIENT_NAME);
                    //His.EventLog.Logger.Log(LOGIC.LocalStore.GlobalStore.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Control
        private void gridControlParticipants_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                    DevExpress.XtraGrid.Views.Grid.GridView view = grid.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        //txtPathological_history.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Validate
        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {

        }

        //Ket thuc
        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.HisServiceReqWithOrderSDO == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                    return;
                }
                WaitingManager.Show();

                var result = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_FINISH, ApiConsumers.MosConsumer, this.HisServiceReqWithOrderSDO.ID, param);
                if (result != null)
                {
                    success = true;
                    EnableButtonByServiceReqStt(result.SERVICE_REQ_STT_ID);
                    SuccessLog(HisServiceReqWithOrderSDO);
                    btnFinish.Enabled = false;
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.HisServiceReqWithOrderSDO == null)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                //    return;
                //}

                //EXE.APP.Modules.AssignService_Merger.frmAssignService_Merger chiDinhDichVu = new EXE.APP.Modules.AssignService_Merger.frmAssignService_Merger(this.HisServiceReqWithOrderSDO.TREATMENT_ID, this.HisServiceReqWithOrderSDO.INTRUCTION_TIME, this.HisServiceReqWithOrderSDO.ID);
                //chiDinhDichVu.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignServicePlus_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.HisServiceReqWithOrderSDO == null)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                //    return;
                //}

                //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                //currentModule.RoomId = EXE.LOGIC.Token.TokenManager.GetRoomId();
                //currentModule.RoomTypeId = EXE.LOGIC.Token.TokenManager.GetRoomTypeId();
                //HIS.Desktop.Plugins.AssignService.AssignService.frmAssignService frmAssignService_Plus = new HIS.Desktop.Plugins.AssignService.AssignService.frmAssignService(currentModule, this.HisServiceReqWithOrderSDO.TREATMENT_ID, this.HisServiceReqWithOrderSDO.INTRUCTION_TIME, this.HisServiceReqWithOrderSDO.ID);
                //frmAssignService_Plus.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDhst_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.HisServiceReqWithOrderSDO == null)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                //    return;
                //}

                //EXE.APP.Modules.ServiceExtra.Dhst.frmVitalSignsAdd frmDhstAdd = new Modules.ServiceExtra.Dhst.frmVitalSignsAdd(this.HisServiceReqWithOrderSDO.TREATMENT_ID);
                //frmDhstAdd.action = GlobalStore.ActionAdd;
                //frmDhstAdd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepartmentTran_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.HisServiceReqWithOrderSDO == null)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                //    return;
                //}
                //EXE.APP.Modules.DepartmentTran.frmDepartmentTran departmentTran = new EXE.APP.Modules.DepartmentTran.frmDepartmentTran(this.HisServiceReqWithOrderSDO.TREATMENT_ID);
                //departmentTran.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentClose_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.HisServiceReqWithOrderSDO == null)
                //    throw new ArgumentNullException("HisServiceReqWithOrderSDO is null");

                //EXE.APP.Modules.Main.frmMain formMain = SessionManager.GetFormMain();
                //if (formMain == null)
                //    throw new ArgumentNullException("formMain is null");

                //EXE.APP.Modules.ResolvedFinish.frmCloseTreatment closeTreatmentControl = new EXE.APP.Modules.ResolvedFinish.frmCloseTreatment(this.HisServiceReqWithOrderSDO.TREATMENT_ID);
                ////formMain.TabCreating(formMain.tabControlMain, "" + "__" + this.HisServiceReqWithOrderSDO.TREATMENT_CODE + "_" + this.HisServiceReqWithOrderSDO.PATIENT_CODE, this.HisServiceReqWithOrderSDO.TREATMENT_CODE + " - " + this.HisServiceReqWithOrderSDO.VIR_PATIENT_NAME, closeTreatmentControl);
                ////closeTreatmentControl.MeShow(this.HisServiceReqWithOrderSDO.TREATMENT_ID);
                //closeTreatmentControl.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSereServSurgReq_Click(object sender, EventArgs e)
        {
            try
            {
                SereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12)gridViewSereServRehaReq.GetFocusedRow();
                if (SereServ != null)
                {
                    FillDataToSereServReha(SereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSymptom_Before_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSymptom_After.Focus();
                    txtSymptom_After.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSymptom_After_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRespiratory_Before.Focus();
                    txtRespiratory_Before.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRespiratory_Before_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRespiratory_After.Focus();
                    txtRespiratory_After.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRespiratory_After_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtECG_Before.Focus();
                    txtECG_Before.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtECG_Before_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtECG_After.Focus();
                    txtECG_After.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtECG_After_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAdvice.Focus();
                    txtAdvice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExcuteTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtDescription.Focus();
                    //txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultNote_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnSaveSereServReha_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA> rehaSereServs = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA>();
                int[] rows = gridViewSereServReha.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA)gridViewSereServReha.GetRow(rows[i]);
                    rehaSereServs.Add(rowData);
                }

                HisSereServRehaSDO sereServRehaSDO = new MOS.SDO.HisSereServRehaSDO();
                sereServRehaSDO.SereServId = SereServ.ID;
                sereServRehaSDO.RehaTrainTypeIds = rehaSereServs.Select(o => o.REHA_TRAIN_TYPE_ID).ToList();

                var rs = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_REHA>>(HisRequestUriStore.HIS_SERE_SERV_REHA__CREATE, ApiConsumers.MosConsumer, sereServRehaSDO, param);

                if (rs != null)
                {
                    //btnSaveRehaTrain.Enabled = true;
                    success = true;
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSereServReha_Click(object sender, EventArgs e)
        {
            try
            {
                gridControlRehaTrain.DataSource = null;
                this.currentSereServReha = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA)gridViewSereServReha.GetFocusedRow();
                if (this.currentSereServReha != null)
                {
                    RefeshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveRehaTrain_Click(object sender, EventArgs e)
        {
            try
            {
                //frmRehaTrain frmRehaTrain = new frmRehaTrain(this.HisServiceReqWithOrderSDO, RefeshData);
                //frmRehaTrain.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var VrehaTrain = (MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN)gridViewRehaTrain.GetFocusedRow();

                if (VrehaTrain != null)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN, HIS_REHA_TRAIN>();
                    HIS_REHA_TRAIN rehaTrain = AutoMapper.Mapper.Map<V_HIS_REHA_TRAIN, HIS_REHA_TRAIN>(VrehaTrain);

                    bool success = new BackendAdapter(param)
                    .Post<bool>(HisRequestUriStore.HIS_REHA_TRAIN__DELETE, ApiConsumers.MosConsumer, rehaTrain.ID, param);
                    if (success)
                    {
                        RefeshData();
                    }

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRehaTrain_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN data = (MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "TRAIN_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRAIN_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFinish_Click(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ICD Combo
        private void txtIcdMainCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    //RehaServiceReqExecuteControlProcess.LoadIcdCombo(strValue, false, this);
                    LoadIcdCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadIcdCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboIcds.Focus();
                    cboIcds.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboIcds.EditValue = data[0].ID;
                            chkIcds.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ICD_CODE == searchCode);
                            if (search != null)
                            {
                                cboIcds.EditValue = search.ID;
                                chkIcds.Focus();
                            }
                            else
                            {
                                cboIcds.EditValue = null;
                                cboIcds.Focus();
                                cboIcds.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboIcds.EditValue = null;
                        cboIcds.Focus();
                        cboIcds.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
        #endregion

        void update(HIS_ICD dataIcd)
        {
            txtIcdExtraName.Text = dataIcd.ICD_NAME;
        }

        void stringIcds(string delegateIcds)
        {
            if (!string.IsNullOrEmpty(delegateIcds))
            {
                txtIcdExtraName.Text = delegateIcds;
            }
        }

        private void txtIcdExtraName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show(); Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins"); HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdExtraName.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleData.RoomId, this.currentModuleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null"); WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {

            try
            {

                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdExtraName.Text = delegateIcdNames;

                }

                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {

                    txtIcdExtraCode.Text = delegateIcdCodes;

                }

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }

        }


        private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIcds.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcds_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcds.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    txtIcdMainText.Text = (!String.IsNullOrEmpty(txtIcdMainText.Text) && cboIcds.Text != txtIcdMainText.Text) ? txtIcdMainText.Text : cboIcds.Text;
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();
                }
                else if (chkIcds.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraName.Focus();
                    txtIcdExtraName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboIcds.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdMainCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboIcds.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdMainCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewSereServSurgReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                V_HIS_SERE_SERV_12 sereServ = (V_HIS_SERE_SERV_12)gridViewSereServRehaReq.GetRow(e.RowHandle);
                if (sereServ != null && sereServ.IS_NO_EXECUTE != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServReha_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SereServRehaADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (SereServRehaADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    //if (this.currentHisTreatment.IS_PAUSE != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_PAUSE__TRUE)
                    //{
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CHOOSE")
                        {
                            if (data.choose == true)
                                e.RepositoryItem = repositoryItemButtonEdit_Ep_Enable;
                            else
                                e.RepositoryItem = repositoryItemButtonEdit_Ep_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveTap_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                List<SereServRehaADO> sereServRehas = gridControlSereServReha.DataSource as List<SereServRehaADO>;
                List<SereServRehaADO> rehaSereServs = new List<SereServRehaADO>();
                if (sereServRehas != null && sereServRehas.Count > 0)
                {
                    rehaSereServs = sereServRehas.Where(o => o.choose == true).ToList();
                }

                HisSereServRehaSDO sereServRehaSDO = new MOS.SDO.HisSereServRehaSDO();
                sereServRehaSDO.SereServId = SereServ.ID;
                sereServRehaSDO.RehaTrainTypeIds = rehaSereServs.Select(o => o.REHA_TRAIN_TYPE_ID).ToList();

                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_REHA>>(HisRequestUriStore.HIS_SERE_SERV_REHA__CREATE, ApiConsumers.MosConsumer, sereServRehaSDO, param);
                WaitingManager.Hide();
                if (rs != null)
                {
                    success = true;
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Ep_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServRehaADO sereServADO = gridViewSereServReha.GetFocusedRow() as SereServRehaADO;
                CommonParam param = new CommonParam();
                bool success = false;
                List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> rehaTrains = null;
                MOS.Filter.HisRehaTrainViewFilter hisRehaTrainViewFilter = new MOS.Filter.HisRehaTrainViewFilter();
                hisRehaTrainViewFilter.SERE_SERV_REHA_ID = sereServADO.ID;
                if (sereServADO != null)
                {
                    WaitingManager.Show();
                    rehaTrains = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>(HisRequestUriStore.HIS_REHA_TRAIN_GETVIEW, ApiConsumers.MosConsumer, hisRehaTrainViewFilter, new CommonParam());
                    if (rehaTrains != null && rehaTrains.Count > 0)
                    {

                        param.Messages.Add("Kỹ thuật tập này có thông tin chi tiết tập");
                        ResultManager.ShowMessage(param, null);
                        return;
                    }

                    HIS_SERE_SERV_REHA sereServReha = new HIS_SERE_SERV_REHA();
                    sereServReha.SERE_SERV_ID = sereServADO.SERE_SERV_ID;
                    sereServReha.REHA_TRAIN_TYPE_ID = sereServADO.REHA_TRAIN_TYPE_ID;
                    //Thay doi trang thai


                    bool result = new BackendAdapter(param)
                         .Post<bool>("api/HisSereServReha/Delete", ApiConsumers.MosConsumer, sereServReha, param);
                    if (result)
                    {
                        success = true;
                        if (sereServRehas != null && sereServRehas.Count > 0)
                        {
                            foreach (var item in sereServRehas)
                            {
                                if (item.ID == sereServADO.ID)
                                {
                                    item.choose = false;
                                    break;
                                }

                            }
                            gridViewSereServReha.LayoutChanged();
                        }
                    }

                    EnableThemKyThuatTap();
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Ep_Disable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                SereServRehaADO sereServADO = gridViewSereServReha.GetFocusedRow() as SereServRehaADO;
                if (sereServADO != null)
                {
                    WaitingManager.Show();
                    AutoMapper.Mapper.CreateMap<SereServRehaADO, HIS_SERE_SERV_REHA>();
                    HIS_SERE_SERV_REHA sereServReha = AutoMapper.Mapper.Map<SereServRehaADO, HIS_SERE_SERV_REHA>(sereServADO);

                    HIS_SERE_SERV_REHA sereServRehaResult = new BackendAdapter(param)
                    .Post<HIS_SERE_SERV_REHA>("api/HisSereServReha/CreateSingle", ApiConsumers.MosConsumer, sereServReha, param);
                    if (sereServRehaResult != null)
                    {
                        success = true;
                        if (sereServRehas != null && sereServRehas.Count > 0)
                        {
                            foreach (var item in sereServRehas)
                            {
                                if (item.ID == sereServADO.ID)
                                {
                                    item.choose = true;
                                    item.ID = sereServRehaResult.ID;
                                    break;
                                }
                            }
                            gridViewSereServReha.LayoutChanged();
                        }
                    }
                    EnableThemKyThuatTap();
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnThemTTTap_Click(object sender, EventArgs e)
        {
            try
            {
                frmRehaTrain frm = new frmRehaTrain(HisServiceReqWithOrderSDO, sereServRehas, ReLoadRehaTrain);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadRehaTrain()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemSpinEdit3_ContextButtonValueChanged(object sender, DevExpress.Utils.ContextButtonValueEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewRehaTrain_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "AMOUNT")
                {
                    V_HIS_REHA_TRAIN rehaTrain = gridViewRehaTrain.GetRow(e.RowHandle) as V_HIS_REHA_TRAIN;
                    if (rehaTrain != null)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        V_HIS_REHA_TRAIN rehaTrainResult = new BackendAdapter(param)
                    .Post<V_HIS_REHA_TRAIN>("api/HisRehaTrain/Update", ApiConsumers.MosConsumer, rehaTrain, param);
                        if (rehaTrainResult != null)
                        {
                            success = true;
                        }

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraName.Focus();
                    txtIcdExtraName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.Focus();
                    txtIcdExtraCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignService_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!new HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager().Run(
                treatmentId,
                PatientTypeAlter.PATIENT_TYPE_ID,
                currentModuleData.RoomId
                ))
                {
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                    AssignServiceADO assignServiceADO = new AssignServiceADO(HisServiceReqWithOrderSDO.TREATMENT_ID, intructionTime, this.HisServiceReqWithOrderSDO.ID);
                    assignServiceADO.GenderName = HisServiceReqWithOrderSDO.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.PatientName = HisServiceReqWithOrderSDO.TDL_PATIENT_NAME;
                    assignServiceADO.PatientDob = HisServiceReqWithOrderSDO.TDL_PATIENT_DOB;
                    listArgs.Add(assignServiceADO);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModuleData.RoomId, currentModuleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPre_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(HisServiceReqWithOrderSDO.TREATMENT_ID, intructionTime, this.HisServiceReqWithOrderSDO.ID);
                    assignPrescription.GenderName = HisServiceReqWithOrderSDO.TDL_PATIENT_GENDER_NAME;
                    assignPrescription.PatientName = HisServiceReqWithOrderSDO.TDL_PATIENT_NAME;
                    assignPrescription.PatientDob = HisServiceReqWithOrderSDO.TDL_PATIENT_DOB;
                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModuleData.RoomId, currentModuleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_PreviewKeyDown_2(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtIcdExtraCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdExtraCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdExtraCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdExtraCode.Focus();
                            txtIcdExtraCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdExtraCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdExtraName.Text == txtIcdExtraName.Properties.NullValuePrompt ? "" : txtIcdExtraName.Text);
                txtIcdExtraName.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraName.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraCode.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtIcdExtraName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.SelectAll();
                    txtIcdExtraCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_Leave(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            cboPrint.ShowDropDown();
        }
    }
}
