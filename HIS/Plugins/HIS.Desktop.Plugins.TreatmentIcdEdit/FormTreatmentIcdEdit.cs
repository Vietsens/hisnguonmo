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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.TreatmentIcdEdit.Validation;
using HIS.Desktop.Plugins.TreatmentIcdEdit.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.TreatmentIcdEdit
{
    public partial class FormTreatmentIcdEdit : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        System.Globalization.CultureInfo cultureLang;
        MOS.EFMODEL.DataModels.HIS_TREATMENT currentVHisTreatment = null;
        MOS.EFMODEL.DataModels.HIS_TRACKING currentVHisTracking = null;
        MOS.EFMODEL.DataModels.HIS_TREATMENT_EXT currentVHisTreatmentExt = null;
        List<HIS_ICD> listIcd { get; set; }
        long roomId = 0;
        long roomTypeId = 0;
        long treatmentId = 0;
        int positionHandleTime = -1;
        HIS.Desktop.Common.RefeshReference RefreshData = null;
        List<V_HIS_PATIENT_TYPE_ALTER> listPatientType;
        //Dangth
        List<string> dataSelectedToPTDT;
        internal V_HIS_SERVICE_REQ HisServiceReqView { get; set; }
        internal Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter;
        short IS_TRUE = 1;
        private List<HIS_FUND> DataFunds;
        private List<HIS_OWE_TYPE> DataOweType;
        private List<HIS_OTHER_PAY_SOURCE> DataOtherSource;
        bool _IsAutoSetOweType;

        short? _IS_NOT_CHECK_LHMP;

        IcdProcessor icdProcessor;
        UserControl ucIcd;
        SecondaryIcdProcessor subIcdProcessor;
        UserControl ucSecondaryIcd;
        IcdProcessor IcdCauseProcessor { get; set; }
        UserControl ucIcdCause;
        string AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd");
        string PatientTypeBhyt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
        List<ProgramADO> ProgramADOList { get; set; }
        List<HIS_PATIENT_PROGRAM> PatientProgramList = null;
        List<V_HIS_DATA_STORE> DataStoreList = null;

        internal List<ADO.DoctorADO> listDoctors { get; set; }
        internal List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE> HisEmployee;

        //internal List<HIS_HOSPITALIZE_REASON> listReason { get; set; }
        internal List<HIS_HOSPITALIZE_REASON> HisHospitalizeReason;

        internal IcdProcessor icdYhctProcessor;
        internal UserControl ucIcdYhct;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;

        internal IcdProcessor inIcdProcessor;
        internal UserControl ucInIcd;

        internal SecondaryIcdProcessor subInIcdProcessor;
        internal UserControl ucSecondaryInIcd;
        #endregion

        #region Construct
        public FormTreatmentIcdEdit()
        {
            InitializeComponent();
        }

        public FormTreatmentIcdEdit(long _treatmentId, HIS.Desktop.Common.RefeshReference refresh, Inventec.Desktop.Common.Modules.Module module)
            : base( )
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;
                listIcd = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                this.Text = module.text;
                this.treatmentId = _treatmentId;
                this.roomId = module.RoomId;
                this.roomTypeId = module.RoomTypeId;
                if (refresh != null)
                {
                    RefreshData = refresh;
                }
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitUcIcd();
                InitUcIcdCause();
                InitUcSecondaryIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcInIcd()
        {
            try
            {
                inIcdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = false;
                ado.DelegateNextFocus = NextForcusInIcd;
                ado.Width = 440;
                ado.Height = 24;
                if (currentVHisTreatment != null && currentVHisTreatment.CLINICAL_IN_TIME != null)
                {
                    ado.IsColor = true;
                }
                ado.LblIcdMain = ResourceMessage.CDNhapVien;
                ado.ToolTipsIcdMain = ResourceMessage.ChanDoanNhapVien;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ado.DataIcds = listIcd;
                ucInIcd = (UserControl)inIcdProcessor.Run(ado);

                if (ucInIcd != null)
                {
                    this.panelControlInIcd.Controls.Add(ucInIcd);
                    ucInIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region init
        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateRequiredCause = LoadRequiredCause;
                ado.IsUCCause = false;
                ado.DelegateNextFocus = NextForcusIcdCause;
                ado.Width = 440;
                ado.Height = 24;
                //Check "Không nhập ICD" (IS_ALLOW_NO_ICD trong HIS_ROOM bằng 1)
                CommonParam paramCommon = new CommonParam();
                HisRoomFilter filter = new HisRoomFilter();
                filter.ID = this.roomId;
                var resultData = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (resultData != null && resultData.Count > 0)
                {
                    if (resultData.FirstOrDefault().IS_ALLOW_NO_ICD == 1)
                        ado.IsColor = false;
                    else
                        ado.IsColor = true;
                }
                else
                    ado.IsColor = true;

                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlUcIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdCause()
        {
            try
            {
                IcdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = true;
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 440;
                ado.Height = 24;
                ado.LblIcdMain = ResourceMessage.NNNgoai;
                ado.ToolTipsIcdMain = ResourceMessage.NguyenNhanNgoai;
                if (IsRequest())
                {
                    ado.IsColor = true;
                }
                else
                {
                    ado.IsColor = false;
                }
                ado.DataIcds = listIcd.Where(o => o.IS_CAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcdCause = (UserControl)IcdCauseProcessor.Run(ado);

                if (ucIcdCause != null)
                {
                    this.panelControlUcIcdCause.Controls.Add(ucIcdCause);
                    ucIcdCause.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                if (this.IcdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.SetRequired(this.ucIcdCause, isRequired);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsRequest()
        {
            bool result = false;
            try
            {
                var icd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                if (icd != null)
                {
                    var mainIcd = this.listIcd.FirstOrDefault(o => o.ICD_CODE == icd.ICD_CODE);
                    if (mainIcd != null && mainIcd.IS_REQUIRE_CAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void NextForcusIcdCause()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void NextForcusInIcd()
        {
            try
            {
                if (subInIcdProcessor != null && ucSecondaryInIcd != null)
                {
                    subInIcdProcessor.FocusControl(ucSecondaryInIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                txtDoctorLogginName.Focus();
                txtDoctorLogginName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_ICD_EDIT__LCI_ICD_SUB",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit,
                    cultureLang);
                ado.TextNullValue = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_ICD_EDIT__CBO_ICD_SUB_TEXT___NULL_VAULE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit,
                    cultureLang);
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSecondIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private string GetInIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.inIcdProcessor != null && this.ucInIcd != null)
                {
                    var icdValue = this.inIcdProcessor.GetValue(this.ucInIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void NextForcusOut()
        {
            try
            {
                if (icdYhctProcessor != null && ucIcdYhct != null)
                {
                    icdYhctProcessor.FocusControl(ucIcdYhct);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusOutYhct;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 24;
                ado.DataIcds = listIcd.Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = ResourceMessage.ChanDoanYHCT;
                ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (ucIcdYhct != null)
                {
                    this.panelIcdYhct.Controls.Add(ucIcdYhct);
                    ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                var icdYhct = listIcd.Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = ResourceMessage.CDYHCTPhu;
                ado.TootiplciIcdSubCode = ResourceMessage.ChanDoanYHCTKemTheo;
                ado.TextNullValue = ResourceMessage.NhanF1DeChonBenh;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.panelSubIcdYhct.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcdToDo()
        {

            try
            {
                if (ucIcdCause != null)
                {
                    IcdCauseProcessor.FocusControl(ucIcdCause);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOutYhct()
        {
            try
            {
                if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null)
                {
                    subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCodeYhct()
        {
            string mainCode = "";
            try
            {
                if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }
        #endregion

        #region Load
        private void FormTreatmentIcdEdit_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                HisConfig.GetConfig();

                LoadKeysFromlanguage();

                this._IsAutoSetOweType = ((HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Register.IsAutoSetOweTypeInCaseOfUsingFund")).Trim() == "1");

                CreateThreadGetData();

                InitCombo();

                SetDefaultValueControl();

                LoadCboDoctor();

                LoadCboReaSonNT();

                WaitingManager.Show();

                InitUcInIcd();

                InitUcSubInIcd();

                FillDataToControl();

                WaitingManager.Hide();

                validationControl();

                //Dangth


                if (HisConfig.checkSovaovien_ == false)
                {
                    btnSovaovien.Enabled = false;
                }
                if (currentVHisTreatment.IS_PAUSE != null || currentVHisTreatment.IS_PAUSE == IS_TRUE)
                {
                    if (HisConfig.SuaThongTinHoSoDieuTri_ == true)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadGetData()
        {
            try
            {
                System.Threading.Thread threadCboFund = new System.Threading.Thread(LoadDataFund);
                System.Threading.Thread threadCboNoVienPhi = new System.Threading.Thread(LoadDataNoVienphi);
                System.Threading.Thread threadCboOtherPay = new System.Threading.Thread(LoadDataOtherPay);
                System.Threading.Thread threadPatientTypeAlter = new System.Threading.Thread(LoadPatienTypeAlter);
                System.Threading.Thread threadEmployee = new System.Threading.Thread(LoadEmployee);
                System.Threading.Thread threadTreatment = new System.Threading.Thread(LoadTreatment);
                System.Threading.Thread threadDataStore = new System.Threading.Thread(LoadDataStore);
                System.Threading.Thread threadLoadReasonNT = new System.Threading.Thread(LoadReasonNT);
                
                try
                {
                    threadCboFund = new System.Threading.Thread(LoadDataFund);
                    threadCboNoVienPhi = new System.Threading.Thread(LoadDataNoVienphi);
                    threadCboOtherPay = new System.Threading.Thread(LoadDataOtherPay);
                    threadPatientTypeAlter = new System.Threading.Thread(LoadPatienTypeAlter);
                    threadEmployee = new System.Threading.Thread(LoadEmployee);
                    threadTreatment = new System.Threading.Thread(LoadTreatment);
                    threadDataStore = new System.Threading.Thread(LoadDataStore);
                    threadLoadReasonNT = new System.Threading.Thread(LoadReasonNT);

                    threadCboFund.Start();
                    threadCboNoVienPhi.Start();
                    threadCboOtherPay.Start();
                    threadPatientTypeAlter.Start();
                    threadEmployee.Start();
                    threadTreatment.Start();
                    threadDataStore.Start();
                    threadLoadReasonNT.Start();

                    threadCboFund.Join();
                    threadCboNoVienPhi.Join();
                    threadCboOtherPay.Join();
                    threadPatientTypeAlter.Join();
                    threadEmployee.Join();
                    threadTreatment.Join();
                    threadDataStore.Join();
                    threadLoadReasonNT.Join();
                }
                catch (Exception ex)
                {
                    threadCboFund.Abort();
                    threadCboNoVienPhi.Abort();
                    threadCboOtherPay.Abort();
                    threadPatientTypeAlter.Abort();
                    threadEmployee.Abort();
                    threadTreatment.Abort();
                    threadDataStore.Abort();
                    threadLoadReasonNT.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSubInIcd()
        {
            try
            {
                subInIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateGetIcdMain = GetInIcdMainCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = ResourceMessage.CDPhuNV;
                ado.TootiplciIcdSubCode = ResourceMessage.ChanDoanPhuNhapVien;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryInIcd = (UserControl)subInIcdProcessor.Run(ado);

                if (ucSecondaryInIcd != null)
                {
                    this.panelControlInIcdSub.Controls.Add(ucSecondaryInIcd);
                    ucSecondaryInIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            InitComboCommon(this.CboCCT, DataFunds, "ID", "FUND_NAME", "FUND_CODE");
            InitComboCommon(this.cboNoVienphi, DataOweType, "ID", "OWE_TYPE_NAME", "OWE_TYPE_CODE");
            InitComboCommon(this.CboOtherPaySource, DataOtherSource, "ID", "OTHER_PAY_SOURCE_NAME", "OTHER_PAY_SOURCE_CODE");
        }

        private void LoadPatientProgram()
        {
            try
            {
                if (this.currentVHisTreatment != null)
                {
                    MOS.Filter.HisPatientProgramFilter patientProgramFilter = new HisPatientProgramFilter();
                    patientProgramFilter.PATIENT_ID = this.currentVHisTreatment.PATIENT_ID;
                    PatientProgramList = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_PROGRAM>>("api/HisPatientProgram/Get", ApiConsumer.ApiConsumers.MosConsumer, patientProgramFilter, null);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPatientProgram this.currentVHisTreatment NULL");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataStore()
        {
            try
            {
                if (this.roomId > 0)
                {
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                    MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new HisDataStoreViewFilter();
                    dataStoreFilter.BRANCH_ID = currentRoom.BRANCH_ID;
                    DataStoreList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DATA_STORE>>("api/HisDataStore/GetView", ApiConsumer.ApiConsumers.MosConsumer, dataStoreFilter, null);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataStore this.moduleData NULL");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataNoVienphi()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisOweTypeFilter filter = new MOS.Filter.HisOweTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                DataOweType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_OWE_TYPE>>("api/HisOweType/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataFund()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisFundFilter filter = new MOS.Filter.HisFundFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                DataFunds = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataOtherPay()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisOtherPaySourceFilter filter = new MOS.Filter.HisOtherPaySourceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                DataOtherSource = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }

                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit = new ResourceManager("HIS.Desktop.Plugins.TreatmentIcdEdit.Resources.Lang", typeof(FormTreatmentIcdEdit).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.label2.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.label2.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.chkIsEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.chkIsEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.bar2.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.barButtonItemSave1.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.barButtonItemSave1.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.groupBox4.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.groupBox4.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.ChkUpdateSereServ.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.ChkUpdateSereServ.Properties.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.CboOtherPaySource.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.CboOtherPaySource.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.LciOtherPaySource.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.LciOtherPaySource.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.groupBox3.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.groupBox3.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.chkNeedSickLeaveCert.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.chkNeedSickLeaveCert.Properties.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.chkBHYT.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.chkBHYT.Properties.Caption", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.cboNoVienphi.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.cboNoVienphi.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.groupBox2.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.labelControl1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.CboCCT.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.CboCCT.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.groupBox1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.cboDoctorUserName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.cboDoctorUserName.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.lciInTime.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.lciInTime.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.lciClinicalInTime.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.lciClinicalInTime.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.lciOutTime.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.lciOutTime.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.layoutControlItem32.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.layoutControlItem32.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.btnSave.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.label1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.label1.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                chkBHYT.Checked = false;
                chkNeedSickLeaveCert.Checked = false;
                dtClinicalInTime.EditValue = null;
                dtInTime.EditValue = null;
                dtOutTime.EditValue = null;
                chkIsEmergency.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                icdProcessor.FocusControl(ucIcd);
                lblInCode.Text = "";
                lblEndCode.Text = "";
                chkTuberculosis.Checked = false;
                txtNumManager.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                if (this.currentVHisTreatment != null)
                {
                    if (currentVHisTreatment.IN_TIME != 0)
                    {
                        dtInTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentVHisTreatment.IN_TIME) ?? DateTime.MinValue;
                    }

                    if (currentVHisTreatment.OUT_TIME != null)
                    {
                        dtOutTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)currentVHisTreatment.OUT_TIME) ?? DateTime.MinValue;
                    }

                    if (currentVHisTreatment.CLINICAL_IN_TIME != null)
                    {
                        dtClinicalInTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)currentVHisTreatment.CLINICAL_IN_TIME) ?? DateTime.MinValue;
                    }
                    if (currentVHisTreatment.IS_EMERGENCY == 1)
                    {
                        chkIsEmergency.CheckState = CheckState.Checked;
                    }
                    else
                        chkIsEmergency.CheckState = CheckState.Unchecked;

                    lblInCode.Text = currentVHisTreatment.IN_CODE;
                    lblEndCode.Text = currentVHisTreatment.END_CODE;

                    LoadcboIcdsValue();
                    LoadcboIcdCauseValue();

                    LoadcboInIcdsValue();


                    this._IS_NOT_CHECK_LHMP = currentVHisTreatment.IS_NOT_CHECK_LHMP;

                    if (currentVHisTreatment.IS_NOT_CHECK_LHMP == IS_TRUE)
                    {
                        chkBHYT.Checked = true;
                    }
                    if (currentVHisTreatment.NEED_SICK_LEAVE_CERT == IS_TRUE)
                    {
                        chkNeedSickLeaveCert.Checked = true;
                    }

                    //SetDataDvCTCT
                    this.CboCCT.EditValue = currentVHisTreatment.FUND_ID;
                    this.TxtSoThe.Text = currentVHisTreatment.FUND_NUMBER;
                    this.txtSanPham.Text = currentVHisTreatment.FUND_TYPE_NAME;
                    this.txtReasonVV.Text = currentVHisTreatment.HOSPITALIZATION_REASON;
                    //Dangth
                    this.txtProvisionalDianosis.Text = currentVHisTreatment.PROVISIONAL_DIAGNOSIS;
                    this.txtTreatmentInstruction.Text = currentVHisTreatment.TREATMENT_METHOD;

                    if (!String.IsNullOrWhiteSpace(currentVHisTreatment.FUND_CUSTOMER_NAME))
                    {
                        this.txtTenKhachHang.Text = currentVHisTreatment.FUND_CUSTOMER_NAME;
                    }
                    else
                    {
                        this.txtTenKhachHang.Text = currentVHisTreatment.TDL_PATIENT_NAME;
                    }

                    this.txtCongTy.Text = currentVHisTreatment.FUND_COMPANY_NAME;
                    if (currentVHisTreatment.FUND_BUDGET > 0)
                    {
                        this.spinHanMuc.Value = currentVHisTreatment.FUND_BUDGET ?? 0;
                    }
                    else
                    {
                        this.spinHanMuc.EditValue = null;
                    }

                    if (currentVHisTreatment.FUND_FROM_TIME > 0)
                    {
                        dtThoiHanTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentVHisTreatment.FUND_FROM_TIME ?? 0) ?? DateTime.Now;
                    }

                    if (currentVHisTreatment.FUND_TO_TIME > 0)
                    {
                        dtThoiHanDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentVHisTreatment.FUND_TO_TIME ?? 0) ?? DateTime.Now;
                    }

                    if (currentVHisTreatment.FUND_ISSUE_TIME > 0)
                    {
                        dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentVHisTreatment.FUND_ISSUE_TIME ?? 0) ?? DateTime.Now;
                    }

                    this.cboNoVienphi.EditValue = currentVHisTreatment.OWE_TYPE_ID;

                    CboOtherPaySource.EditValue = currentVHisTreatment.OTHER_PAY_SOURCE_ID;

                    HIS.UC.Icd.ADO.IcdInputADO icdYhct = new IcdInputADO();
                    icdYhct.ICD_CODE = currentVHisTreatment.TRADITIONAL_ICD_CODE;
                    icdYhct.ICD_NAME = currentVHisTreatment.TRADITIONAL_ICD_NAME;
                    if (ucIcdYhct != null)
                    {
                        icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                    }

                    SecondaryIcdDataADO subIcdYhct = new SecondaryIcdDataADO();
                    subIcdYhct.ICD_SUB_CODE = currentVHisTreatment.TRADITIONAL_ICD_SUB_CODE;
                    subIcdYhct.ICD_TEXT = currentVHisTreatment.TRADITIONAL_ICD_TEXT;
                    if (ucSecondaryIcdYhct != null)
                    {
                        subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subIcdYhct);
                    }

                    if (currentVHisTreatment.IS_ACTIVE != 1)
                    {
                        ChkUpdateSereServ.Checked = false;
                        ChkUpdateSereServ.Enabled = false;
                    }

                    if (!String.IsNullOrEmpty(currentVHisTreatment.DOCTOR_LOGINNAME))
                    {
                        cboDoctorUserName.EditValue = currentVHisTreatment.DOCTOR_LOGINNAME;
                    }
                    else
                    {
                        HisServiceReqFilter ServiceReqFilter = new HisServiceReqFilter();
                        ServiceReqFilter.TREATMENT_ID = currentVHisTreatment.ID;
                        ServiceReqFilter.REQUEST_DEPARTMENT_ID = currentVHisTreatment.LAST_DEPARTMENT_ID;
                        ServiceReqFilter.ORDER_DIRECTION = "DESC";
                        ServiceReqFilter.ORDER_FIELD = "INTRUCTION_TIME";

                        var ServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, ServiceReqFilter, new CommonParam());
                        if (ServiceReq != null && ServiceReq.Count > 0)
                        {
                            cboDoctorUserName.EditValue = ServiceReq.FirstOrDefault().REQUEST_LOGINNAME;
                        }
                        else
                        {
                            cboDoctorUserName.EditValue = null;
                        }
                    }

                    if (!String.IsNullOrEmpty(currentVHisTreatment.HOSPITALIZE_REASON_NAME))
                    {
                        cboReasonNT.EditValue = currentVHisTreatment.HOSPITALIZE_REASON_CODE;
                        //cboReasonNT.Text = currentVHisTreatment.HOSPITALIZE_REASON_NAME;
                        //txtReasonNTCode.Text = currentVHisTreatment.HOSPITALIZE_REASON_CODE;
                    }
                    else
                    {
                        cboReasonNT.EditValue = "";
                    }

                    if (!String.IsNullOrEmpty(currentVHisTreatment.HOSPITALIZATION_REASON))
                    {
                        txtReasonVV.Text = currentVHisTreatment.HOSPITALIZATION_REASON;
                    }
                    else
                    {
                        txtReasonVV.Text = "";
                    }

                    //Dangth                 
                    if (!String.IsNullOrEmpty(currentVHisTreatment.PROVISIONAL_DIAGNOSIS))
                    {
                        txtProvisionalDianosis.Text = currentVHisTreatment.PROVISIONAL_DIAGNOSIS;
                    }
                    else
                    {
                        txtProvisionalDianosis.Text = "";
                    }

                    if (!String.IsNullOrEmpty(currentVHisTreatment.TREATMENT_METHOD))
                    {
                        txtTreatmentInstruction.Text = currentVHisTreatment.TREATMENT_METHOD;
                    }
                    else
                    {
                        txtTreatmentInstruction.Text = "";
                    }   

                    if (currentVHisTreatment.IS_TUBERCULOSIS == 1)
                    {
                        chkTuberculosis.Checked = true;
                    }
                    else
                    {
                        chkTuberculosis.Checked = false;
                    }
                    txtNumManager.Text = currentVHisTreatment.TUBERCULOSIS_CODE;
                    //if (chkTuberculosis.Checked == false)
                    //{
                    //    txtNumManager.Enabled = false;
                    //    if (!string.IsNullOrEmpty(txtNumManager.Text))
                    //    {
                    //        if (MessageBox.Show("Bạn có muốn xóa số quản lý hay không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //        {
                    //            txtNumManager.Text = "";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    txtNumManager.Enabled = true;
                    //    txtNumManager.Properties.MaxLength = 100;
                    //}

                }
                if (currentVHisTreatment.TREATMENT_ORDER.HasValue)
                {
                    this.txtTreatmentOrder.Text = currentVHisTreatment.TREATMENT_ORDER.Value.ToString();
                }
                else
                {
                    this.txtTreatmentOrder.Text = "";
                }
                //Dangth
                HIS_TREATMENT_EXT HisTreatmentExt = new HIS_TREATMENT_EXT();
                HisTreatmentExtFilter hisTreatmentExtFilter = new HisTreatmentExtFilter();
                hisTreatmentExtFilter.TREATMENT_ID = currentVHisTreatment.ID;
                var TreatmentId = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT_EXT>>("api/HisTreatmentExt/Get", ApiConsumers.MosConsumer, hisTreatmentExtFilter, new CommonParam());
                if (TreatmentId != null && TreatmentId.Count > 0)
                {
                    HIS_TREATMENT_EXT hisTreatmentExtResult = TreatmentId.FirstOrDefault();
                    if (!String.IsNullOrEmpty(hisTreatmentExtResult.SUBCLINICAL_RESULT))
                    {
                        txtSubclinical.Text = hisTreatmentExtResult.SUBCLINICAL_RESULT;
                    }
                    else
                    {
                        txtSubclinical.Text = "";
                    }
                    if (!String.IsNullOrEmpty(hisTreatmentExtResult.CLINICAL_NOTE))
                    {
                        txtPathologicalProcess.Text = hisTreatmentExtResult.CLINICAL_NOTE;
                    }
                    else
                    {
                        txtPathologicalProcess.Text = "";
                    }
                }
                //else
                //{
                //    cboDoctorUserName.EditValue = null;
                //}
                //if (this.currentVHisTreatmentExt != null)
                //{
                //    this.txtSubclinical.Text = currentVHisTreatmentExt.SUBCLINICAL_RESULT;
                //    this.txtPathologicalProcess.Text = currentVHisTreatmentExt.CLINICAL_NOTE;

                //    if (!String.IsNullOrEmpty(currentVHisTreatmentExt.SUBCLINICAL_RESULT))
                //    {
                //        txtSubclinical.Text = currentVHisTreatmentExt.SUBCLINICAL_RESULT;
                //    }
                //    else
                //    {
                //        txtSubclinical.Text = "";
                //    }

                //    if (!String.IsNullOrEmpty(currentVHisTreatmentExt.CLINICAL_NOTE))
                //    {
                //        txtPathologicalProcess.Text = currentVHisTreatmentExt.CLINICAL_NOTE;
                //    }
                //    else
                //    {
                //        txtPathologicalProcess.Text = "";
                //    }
                //}    

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadcboInIcdsValue()
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = currentVHisTreatment.IN_ICD_CODE;
                icd.ICD_NAME = currentVHisTreatment.IN_ICD_NAME;
                if (ucInIcd != null)
                {
                    inIcdProcessor.Reload(ucInIcd, icd);
                }

                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = currentVHisTreatment.IN_ICD_SUB_CODE;
                subIcd.ICD_TEXT = currentVHisTreatment.IN_ICD_TEXT;
                if (ucSecondaryInIcd != null)
                {
                    subInIcdProcessor.Reload(ucSecondaryInIcd, subIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.treatmentId > 0)
                {
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = treatmentId;

                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (result != null && result.Count > 0)
                    {
                        currentVHisTreatment = result.FirstOrDefault();
                    }
                    LoadPatientProgram();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboIcdsValue()
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = currentVHisTreatment.ICD_CODE;
                icd.ICD_NAME = currentVHisTreatment.ICD_NAME;
                if (ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, icd);
                }

                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = currentVHisTreatment.ICD_SUB_CODE;
                subIcd.ICD_TEXT = currentVHisTreatment.ICD_TEXT;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadcboIcdCauseValue()
        {
            try
            {
                IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = currentVHisTreatment.ICD_CAUSE_CODE;
                icd.ICD_NAME = currentVHisTreatment.ICD_CAUSE_NAME;
                if (ucIcdCause != null)
                {
                    IcdCauseProcessor.Reload(ucIcdCause, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        private void dtInTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtInTime.EditValue != null)
                    {
                        dtClinicalInTime.Focus();
                        dtClinicalInTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderTime_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleTime == -1)
                {
                    positionHandleTime = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleTime > edit.TabIndex)
                {
                    positionHandleTime = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void validationControl()
        {
            try
            {
                ValidateTimeEdit(dtInTime, dxValidationProviderTime);
                if (CboCCT.EditValue != null)
                    ValidTextControlMaxlength(this.TxtSoThe, 255, true);
                else
                    ValidTextControlMaxlength(this.TxtSoThe, 255, false);
                ValidTextControlMaxlength(this.txtTenKhachHang, 200, false);
                ValidTextControlMaxlength(this.txtCongTy, 200, false);
                ValidTextControlMaxlength(this.txtSanPham, 200, false);
                ValidTextControlMaxlength(this.txtReasonVV, 200, false);
                ValidTextControlMaxlength(this.txtReasonNTCode, 10, false);
                ValidTextControlMaxlength(this.cboReasonNT, 1000, false);
                layoutControlItem39.AppearanceItemCaption.ForeColor = Color.Black;
                if (dtClinicalInTime.EditValue != null)
                {
                    //Dangth
                    ValidationRequired(txtReasonVV);
                    //ValidationRequired(txtSubclinical);
                    //ValidationRequired(txtProvisionalDianosis);
                    //ValidationRequired(txtTreatmentInstruction);
                    //ValidationRequired(txtPathologicalProcess);
                    ValidationReason();
                    layoutControlItem39.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                if (currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU ||
                    currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ||
                    currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    lblCaptionDiagnostic.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblPathologicalProcess.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblCaptionConclude.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciProvisionalDianosis.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationRequired(txtSubclinical);
                    ValidationRequired(txtProvisionalDianosis);
                    ValidationRequired(txtTreatmentInstruction);
                    ValidationRequired(txtPathologicalProcess);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationReason()
        {

            try
            {
                GridLookupReasonEditValidationRule rule = new GridLookupReasonEditValidationRule();
                rule.cbo = cboReasonNT;
                rule.txt = txtReasonNTCode;
                rule.MaxLengthCode = 10;
                rule.MaxLengthName = 1000;
                rule.IsRequired = true;
                dxValidationProviderTime.SetValidationRule(txtReasonNTCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        //Dangth
        private void DelegateSelectDataContentSubclinical(object data)
        {
            try
            {
                if (data != null && data is String)
                {
                    txtSubclinical.Text = data.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void dataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dt = data as string;


                    //if (!string.IsNullOrEmpty(dt))
                    //{
                    //    if (dt.Contains(";"))
                    //    {
                    //        string[] serviceName = dt.Split(';');
                    //        foreach (var item in serviceName)
                    //        {
                    //            dataSelectedToPTDT.Add(item.Trim());
                    //        }
                    //    }
                    //    else
                    //    {
                    //        dataSelectedToPTDT.Add(dt);
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(dt))
                    {
                        txtTreatmentInstruction.Text = string.Join("; ",
                            data
                            // dataSelectedToPTDT.Distinct()
                            );
                    }
                    else
                    {
                        txtTreatmentInstruction.Text = "";
                    }

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationRequired(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderTime.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidTextControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength, bool isVali)
        {
            try
            {
                TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
                _rule.txtEdit = control;
                _rule.maxlength = maxlength;
                _rule.isVali = isVali;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderTime.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateTimeEdit(DevExpress.XtraEditors.DateEdit dt, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidation)
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = dt;
                validate.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidation.SetValidationRule(dtInTime, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string codeCheckCD = "";
                string nameCheckCD = "";
                string codeCheckCDYHCT = "";

                btnSave.Focus();
                validationControl();
                CommonParam param = new CommonParam();
                bool success = false;
                bool valid = (bool)icdProcessor.ValidationIcd(ucIcd);
                valid = (bool)inIcdProcessor.ValidationIcd(ucInIcd) && valid;
                valid = subIcdProcessor.GetValidate(ucSecondaryIcd) && valid;
                valid = subInIcdProcessor.GetValidate(ucSecondaryInIcd) && valid;
                valid = IsValiICDCause() && valid;
                if (!valid) return;
                if (!this.CheckTreatmentOrder())
                {
                    return;
                }

                positionHandleTime = -1;
                if (!dxValidationProviderTime.Validate()) return;

                //WaitingManager.Show();
                HisTreatmentCommonInfoUpdateSDO data = new HisTreatmentCommonInfoUpdateSDO();
                data.Id = currentVHisTreatment.ID;
                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue != null && icdValue is IcdInputADO)
                    {
                        data.IcdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        data.IcdName = ((IcdInputADO)icdValue).ICD_NAME;
                        codeCheckCD  = ((IcdInputADO)icdValue).ICD_CODE;
                        nameCheckCD = ((IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        data.IcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        data.IcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                        codeCheckCD += ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        nameCheckCD += ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                if (ucSecondaryIcdYhct != null)
                {
                    var subIcdYHCT = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    var icd = BackendDataWorker.Get<HIS_ICD>()
                        .Where(s => s.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && s.IS_TRADITIONAL == 1).ToList();
                    if (subIcdYHCT != null && subIcdYHCT is SecondaryIcdDataADO)
                    {
                        codeCheckCDYHCT = ((SecondaryIcdDataADO)subIcdYHCT).ICD_SUB_CODE;

                        if (!string.IsNullOrEmpty(codeCheckCDYHCT))
                        {
                            foreach (var item in codeCheckCDYHCT.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList())
                            {
                                if (!icd.Exists(o => o.ICD_CODE == item))
                                {
                                    MessageBox.Show("Chẩn đoán YHCT phụ không có trong danh mục");
                                    throw new InvalidOperationException("Chẩn đoán YHCT phụ không có trong danh mục"); // Ném ngoại lệ khi có lỗi
                                }
                            }
                        }
                    }
                }
                if (ucIcdYhct != null)
                {
                    var IcdYHCT = icdYhctProcessor.GetValue(ucIcdYhct);
                    if (IcdYHCT != null && IcdYHCT is IcdInputADO)
                    {
                        codeCheckCDYHCT += ((IcdInputADO)IcdYHCT).ICD_CODE;
                    }
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCD) > 100)
                {
                    XtraMessageBox.Show("Mã chẩn đoán phụ nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Inventec.Common.String.CountVi.Count(nameCheckCD) > 1500)
                {
                    XtraMessageBox.Show("Tên chẩn đoán phụ nhập quá 1500 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCDYHCT) > 255)
                {
                    XtraMessageBox.Show("Mã chẩn đoán YHCT phụ nhập quá 255 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(data.IcdSubCode))
                {
                    var checkICDSubCode = data.IcdSubCode.Split(';').ToList();
                    if (checkICDSubCode != null && checkICDSubCode.Count > 12)
                    {
                        if (HisConfig.IsCheckSubIcdExceedLimit == "1")
                        {
                            XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else if (HisConfig.IsCheckSubIcdExceedLimit == "2")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục không?",
                               "Thông báo",
                              MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                    }
                }


                if (this.ucIcdCause != null)
                {
                    var icdValue = this.IcdCauseProcessor.GetValue(this.ucIcdCause);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        data.IcdCauseCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        data.IcdCauseName = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                        Inventec.Common.Logging.LogSystem.Info("TreatmentCause:" + (data.IcdCauseCode ?? "") + (data.IcdCauseName ?? ""));
                    }
                }
                if (this.ucInIcd != null)
                {
                    var icdValue = this.inIcdProcessor.GetValue(this.ucInIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        data.InIcdCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        data.InIcdName = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (this.ucSecondaryInIcd != null)
                {
                    var icdValue = this.subInIcdProcessor.GetValue(this.ucSecondaryInIcd);
                    if (icdValue != null && icdValue is SecondaryIcdDataADO)
                    {
                        data.InIcdSubCode = ((SecondaryIcdDataADO)icdValue).ICD_SUB_CODE;
                        data.InIcdText = ((SecondaryIcdDataADO)icdValue).ICD_TEXT;
                    }
                }

                if (dtInTime.EditValue != null && dtInTime.DateTime != DateTime.MinValue)
                    data.InTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtInTime.DateTime.ToString("yyyyMMddHHmm") + "00");

                if (dtOutTime.EditValue != null && dtOutTime.DateTime != DateTime.MinValue)
                    data.OutTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtOutTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                else data.OutTime = null;

                if (dtClinicalInTime.EditValue != null && dtClinicalInTime.DateTime != DateTime.MinValue)
                    data.ClinicalInTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtClinicalInTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                else data.ClinicalInTime = null;

                if (chkIsEmergency.Checked)
                {
                    data.IsEmergency = true;
                }
                else
                {
                    data.IsEmergency = false;
                }

                if (chkBHYT.Checked)
                {
                    data.IsNotCheckLhmp = true;
                }
                else
                {
                    data.IsNotCheckLhmp = false;
                }

                if (chkNeedSickLeaveCert.Checked)
                {
                    data.NeedSickLeaveCert = IS_TRUE;
                }
                else
                {
                    data.NeedSickLeaveCert = null;
                }

                if (data.OutTime.HasValue)
                {
                    long inTime = data.ClinicalInTime.HasValue ? data.ClinicalInTime.Value : data.InTime;
                    HIS.Common.Treatment.PatientTypeEnum.TYPE type = HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI;
                    if (patientTypeAlter != null)
                    {
                        var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == PatientTypeBhyt);
                        if (patientType != null && patientTypeAlter.PATIENT_TYPE_ID == patientType.ID)
                        {
                            type = HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT;
                        }
                    }

                    data.TreatmentDayCount = HIS.Common.Treatment.Calculation.DayOfTreatment(inTime, data.OutTime, this.currentVHisTreatment.TREATMENT_END_TYPE_ID, this.currentVHisTreatment.TREATMENT_RESULT_ID, type);
                }
                else
                {
                    data.TreatmentDayCount = null;
                }

                if (CheckTime(data)) return;

                //Dangth
                if (!String.IsNullOrWhiteSpace(this.txtSubclinical.Text))
                {
                    data.SubclinicalResult = this.txtSubclinical.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(this.txtProvisionalDianosis.Text))
                {
                    data.ProvisionalDiagnosis = this.txtProvisionalDianosis.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(this.txtTreatmentInstruction.Text))
                {
                    data.TreatmentMethod = this.txtTreatmentInstruction.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(this.txtPathologicalProcess.Text))
                {
                    data.ClinicalNote = this.txtPathologicalProcess.Text.Trim();
                }

                if (!String.IsNullOrWhiteSpace(this.txtTreatmentOrder.Text))
                {
                    data.TreatmentOrder = Convert.ToInt64(txtTreatmentOrder.Text);
                }
                else
                {
                    data.TreatmentOrder = null;
                }
                data.OweTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboNoVienphi.EditValue ?? "0").ToString());

                //Update treatment fund
                if (CboCCT.EditValue != null)
                    data.FundId = Inventec.Common.TypeConvert.Parse.ToInt64((CboCCT.EditValue ?? "0").ToString());
                else
                    data.FundId = null;
                if (cboNoVienphi.EditValue != null)
                    data.OweTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboNoVienphi.EditValue ?? "0").ToString());
                else
                    data.OweTypeId = null;
                data.FundNumber = this.TxtSoThe.Text;
                data.FundBudget = this.spinHanMuc.Value;
                data.FundCompanyName = this.txtCongTy.Text;
                data.FundTypeName = this.txtSanPham.Text;
                data.HospitalizationReason = this.txtReasonVV.Text;
                data.FundCustomerName = this.txtTenKhachHang.Text;
                if (dtThoiHanTu.EditValue != null && dtThoiHanTu.DateTime != DateTime.MinValue)
                {
                    data.FundFromTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtThoiHanTu.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    data.FundFromTime = null;
                }

                if (dtThoiHanDen.EditValue != null && dtThoiHanDen.DateTime != DateTime.MinValue)
                {
                    data.FundToTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtThoiHanDen.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    data.FundToTime = null;
                }

                if (dtNgayCap.EditValue != null && dtNgayCap.DateTime != DateTime.MinValue)
                {
                    data.FundIssueTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                             Convert.ToDateTime(dtNgayCap.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    data.FundIssueTime = null;
                }

                if (CboOtherPaySource.EditValue != null)
                {
                    data.OtherPaySourceId = Inventec.Common.TypeConvert.Parse.ToInt64((CboOtherPaySource.EditValue ?? "0").ToString());
                }
                else
                {
                    data.OtherPaySourceId = null;
                }

                data.UpdateOtherPaySourceIdForSereServ = ChkUpdateSereServ.Checked;

                if (ucIcdYhct != null)
                {
                    var icdValue = icdYhctProcessor.GetValue(ucIcdYhct);
                    if (icdValue != null && icdValue is IcdInputADO)
                    {
                        data.TraditionalIcdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        data.TraditionalIcdName = ((IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (ucSecondaryIcdYhct != null)
                {
                    var subIcd = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        data.TraditionalIcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        data.TraditionalIcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                if ((currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || 
                    currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || 
                    currentVHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) && 
                    currentVHisTreatment.IN_CODE != null && dtClinicalInTime.EditValue == null)
                {
                    //ValidationRequired(txtSubclinical);
                    //ValidationRequired(txtProvisionalDianosis);
                    //ValidationRequired(txtTreatmentInstruction);
                    //ValidationRequired(txtPathologicalProcess);
                    //WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NgayVaoKhongDuocDeTrong, ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        return;
                }

                if (!string.IsNullOrEmpty(txtDoctorLogginName.Text))
                {
                    data.DoctorLoginName = txtDoctorLogginName.Text;
                }

                //if (!string.IsNullOrEmpty(txtReasonNTCode.Text))
                //{
                //    data.HospitalizeReasonCode = txtReasonNTCode.Text;
                //}


                if (cboDoctorUserName.EditValue != null)
                {
                    var checkDoctor = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboDoctorUserName.EditValue.ToString().ToUpper());
                    if (checkDoctor != null)
                    {
                        data.DoctorUserName = checkDoctor.USERNAME;
                    }
                }

                if (!string.IsNullOrEmpty(txtReasonNTCode.Text))
                {
                    data.HospitalizeReasonCode = txtReasonNTCode.Text;
                }

                if (!string.IsNullOrEmpty(txtReasonVV.Text))
                {
                    data.HospitalizationReason = txtReasonVV.Text;
                }
                if (cboReasonNT.EditValue != null)
                {
                    var checkReasonNT = this.HisHospitalizeReason.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE.ToUpper() == cboReasonNT.EditValue.ToString().ToUpper());
                    if (checkReasonNT != null)
                    {
                        data.HospitalizeReasonName = checkReasonNT.HOSPITALIZE_REASON_NAME;
                    }
                }
                if (chkTuberculosis.Checked == true)
                {
                    data.IsTuberculosis = 1;
                }
                else
                {
                    data.IsTuberculosis = 0;
                }
                if (!string.IsNullOrEmpty(txtNumManager.Text))
                {
                    data.TuberculosisCode = txtNumManager.Text;
                }
                

                HisTreatmentCommonInfoUpdateSDO result = new BackendAdapter(param).Post<HisTreatmentCommonInfoUpdateSDO>(RequestUriStore.HIS_TREATMENT_UPDATE_COMMON_INFO, ApiConsumer.ApiConsumers.MosConsumer, data, param);

                WaitingManager.Hide();
                if (result != null && param.Messages.Count == 0)
                {
                    success = true;
                    if ((this._IS_NOT_CHECK_LHMP == IS_TRUE) != result.IsNotCheckLhmp)
                    {
                        LichSuTacDong();
                    }
                    if (RefreshData != null)
                        RefreshData();
                    this.Close();
                }
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        bool IsValiICDCause()
        {
            bool result = true;
            try
            {
                result = (bool)this.IcdCauseProcessor.ValidationIcd(this.ucIcdCause);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LichSuTacDong()
        {
            try
            {
                string message = "Không giới hạn tiền thuốc của hồ sơ điều trị" + "  TREATMENT_CODE: " + currentVHisTreatment.TREATMENT_CODE + "  Thời gian sửa: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người sửa: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckTime(HisTreatmentCommonInfoUpdateSDO data)
        {
            try
            {
                if (data.ClinicalInTime != null)
                {
                    if (patientTypeAlter != null
                        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                        && patientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(ResourceMessage.BenhNhanKoPhaiNoiTruKhongChoPhepNhapNgay);
                        return true;
                    }

                    if (data.InTime > data.ClinicalInTime)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(ResourceMessage.NgayVaoKhongDuocLonHonNgayVaoNoiTru);
                        return true;
                    }

                    if (data.OutTime != null && data.OutTime < data.ClinicalInTime)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(ResourceMessage.NgayRaKhongDuocNhoHonNgayVaoNoiTru);
                        return true;
                    }
                }

                if (data.OutTime != null && data.OutTime < data.InTime)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(ResourceMessage.NgayRaKhongDuocNhoHonNgayVao);
                    return true;
                }

                if (data.InTime > Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00"))
                {
                    WaitingManager.Hide();
                    MessageBox.Show(ResourceMessage.NgayVaoKhongDuocLonHonNgayHienTai);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        #endregion

        #region shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled == true)
                {
                    btnSave_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion


        private void txtTreatmentOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckTreatmentOrder()
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrWhiteSpace(txtTreatmentOrder.Text))
                {
                    HisTreatmentOrderSDO sdo = new HisTreatmentOrderSDO();
                    sdo.TreatmentOrder = Convert.ToInt64(txtTreatmentOrder.Text);
                    sdo.InDate = Convert.ToInt64(dtInTime.DateTime.ToString("yyyyMMdd") + "000000");
                    sdo.Id = this.currentVHisTreatment.ID;

                    var rs = new BackendAdapter(new CommonParam()).PostRO<HisTreatmentOrderSDO>("api/HisTreatment/CheckExistsTreatmentOrder", ApiConsumers.MosConsumer, sdo, null);
                    if (rs == null || !rs.Success)
                    {
                        string code = "";
                        if (rs.Param != null && rs.Param.Messages != null && rs.Param.Messages.Count > 0)
                        {
                            code = String.Join(",", rs.Param.Messages);
                        }
                        if (XtraMessageBox.Show(String.Format(ResourceMessage.STTHoSoDaTonTai, code), ResourceMessage.CanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True) == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }

        private void CboCCT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboCCT.EditValue != null)
                {
                    CboCCT.Properties.Buttons[1].Visible = true;
                    layoutControlItem15.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    CboCCT.Properties.Buttons[1].Visible = false;
                    layoutControlItem15.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProviderTime.RemoveControlError(this.TxtSoThe);
                }
                checkAutoSetOweType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkAutoSetOweType()
        {
            try
            {
                if (_IsAutoSetOweType)
                {
                    if (this.CboCCT.EditValue != null)
                        cboNoVienphi.EditValue = IMSys.DbConfig.HIS_RS.HIS_OWE_TYPE.ID__INSURANCE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboCCT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    TxtSoThe.SelectAll();
                    TxtSoThe.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtSoThe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSanPham.SelectAll();
                    txtSanPham.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSanPham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtThoiHanTu.SelectAll();
                    dtThoiHanTu.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtThoiHanTu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtThoiHanDen.SelectAll();
                    dtThoiHanDen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtThoiHanDen_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNgayCap.SelectAll();
                    dtNgayCap.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtNgayCap_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTenKhachHang.SelectAll();
                    txtTenKhachHang.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTenKhachHang_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCongTy.SelectAll();
                    txtCongTy.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCongTy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHanMuc.SelectAll();
                    spinHanMuc.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHanMuc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboOtherPaySource.Focus();
                    CboOtherPaySource.SelectAll();
                    CboOtherPaySource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoVienphi_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBHYT.SelectAll();
                    chkBHYT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBHYT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNeedSickLeaveCert.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoVienphi_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    chkBHYT.SelectAll();
                    chkBHYT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.icdProcessor.FocusControl(ucIcd);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoVienphi_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNoVienphi.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNoVienphi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboNoVienphi.EditValue != null)
                {
                    cboNoVienphi.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboNoVienphi.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboCCT_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboCCT.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNeedSickLeaveCert_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboOtherPaySource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboOtherPaySource_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    ChkUpdateSereServ.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkUpdateSereServ_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboNoVienphi.Focus();
                    cboNoVienphi.SelectAll();
                    cboNoVienphi.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDoctorLogginName.Text.Trim()))
                    {
                        string code = txtDoctorLogginName.Text.Trim().ToLower();
                        var listData = this.listDoctors.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDoctorLogginName.Text = result.First().LOGINNAME;
                            cboDoctorUserName.EditValue = result.First().LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                            chkIsEmergency.Focus();
                            chkIsEmergency.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
                            chkIsEmergency.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboDoctorUserName.Focus();
                        cboDoctorUserName.SelectAll();
                        cboDoctorUserName.ShowPopup();
                    }
                    else
                    {
                        this.inIcdProcessor.FocusControl(ucInIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDoctorUserName.EditValue != null)
                    {
                        var data = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboDoctorUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtDoctorLogginName.Text = data.LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                        }
                        this.inIcdProcessor.FocusControl(ucInIcd);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboDoctorUserName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserName_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtDoctorLogginName.EditValue = null;
                    cboDoctorUserName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void cboReasonNT_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            txtReasonNTCode.EditValue = null;
        //            cboReasonNT.EditValue = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void cboUserName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDoctorUserName.EditValue != null)
                {
                    cboDoctorUserName.Properties.Buttons[1].Visible = true;

                    var data = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboDoctorUserName.EditValue.ToString().ToLower());
                    if (data != null)
                    {
                        txtDoctorLogginName.Text = data.LOGINNAME;
                    }
                }
                else
                {
                    cboDoctorUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboDoctor()
        {
            try
            {
                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                listDoctors = new List<ADO.DoctorADO>();
                if (HisEmployee != null && HisEmployee.Count > 0)
                {
                    foreach (var item in HisEmployee)
                    {
                        if (item.IS_DOCTOR.HasValue && item.IS_DOCTOR.Value == 1)
                        {
                            var user = acsUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                            if (user == null) continue;
                            listDoctors.Add(new ADO.DoctorADO(user));
                        }
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDoctorUserName, listDoctors, controlEditorADO);



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboReaSonNT()
        {
            try
            {
                //var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                //listDoctors = new List<HIS_HOSPITALIZE_REASON>();
                //if (HisEmployee != null && HisEmployee.Count > 0)
                //{
                //    foreach (var item in HisEmployee)
                //    {
                //        if (item.IS_DOCTOR.HasValue && item.IS_DOCTOR.Value == 1)
                //        {
                //            var user = acsUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                //            if (user == null) continue;
                //            listDoctors.Add(new ADO.DoctorADO(user));
                //        }
                //    }
                //}

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HOSPITALIZE_REASON_NAME", "HOSPITALIZE_REASON_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboReasonNT, HisHospitalizeReason, controlEditorADO);



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatienTypeAlter()
        {
            try
            {
                if (treatmentId > 0)
                {
                    listPatientType = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    MOS.Filter.HisPatientTypeAlterViewFilter alterFilter = new MOS.Filter.HisPatientTypeAlterViewFilter();
                    alterFilter.TREATMENT_ID = treatmentId;
                    listPatientType = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_VIEW, ApiConsumer.ApiConsumers.MosConsumer, alterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listPatientType != null && listPatientType.Count > 0)
                    {
                        patientTypeAlter = listPatientType.OrderByDescending(o => o.LOG_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadEmployee()
        {
            try
            {
                MOS.Filter.HisEmployeeFilter filter = new MOS.Filter.HisEmployeeFilter();
                filter.IS_ACTIVE = 1;
                HisEmployee = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadReasonNT()
        {
            try
            {
                HisHospitalizeReasonFilter filter = new HisHospitalizeReasonFilter();
                filter.IS_ACTIVE = 1;
                HisHospitalizeReason = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>>("api/HisHospitalizeReason/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDoctorUserName.EditValue != null)
                    {
                        var data = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboDoctorUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtDoctorLogginName.Text = data.LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                        }

                        this.inIcdProcessor.FocusControl(ucInIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboCCT.Focus();
                    chkIsEmergency.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    CboCCT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSoravien_Click(object sender, EventArgs e)
        {
            try
            {

                FormSovaovien frm = new FormSovaovien((HIS.Desktop.Common.DelegateSelectData)ReloadSoVaoVien, this.currentVHisTreatment.ID);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ReloadSoVaoVien(object obj)
        {
            try
            {
                if (obj != null && obj is string)
                {
                    this.lblInCode.Text = obj.ToString();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtClinicalInTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtClinicalInTime.EditValue != null)
                    {
                        dtOutTime.Focus();
                        dtOutTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                ucSecondaryInIcd = null;
                subInIcdProcessor = null;
                ucInIcd = null;
                inIcdProcessor = null;
                ucSecondaryIcdYhct = null;
                subIcdYhctProcessor = null;
                ucIcdYhct = null;
                icdYhctProcessor = null;
                HisEmployee = null;
                HisHospitalizeReason = null;
                listDoctors = null;
                DataStoreList = null;
                PatientProgramList = null;
                ProgramADOList = null;
                PatientTypeBhyt = null;
                AutoCheckIcd = null;
                ucIcdCause = null;
                IcdCauseProcessor = null;
                ucSecondaryIcd = null;
                subIcdProcessor = null;
                ucIcd = null;
                icdProcessor = null;
                _IS_NOT_CHECK_LHMP = null;
                _IsAutoSetOweType = false;
                DataOtherSource = null;
                DataOweType = null;
                DataFunds = null;
                IS_TRUE = 0;
                patientTypeAlter = null;
                listPatientType = null;
                RefreshData = null;
                positionHandleTime = 0;
                treatmentId = 0;
                roomTypeId = 0;
                roomId = 0;
                listIcd = null;
                currentVHisTreatment = null;
                cultureLang = null;
                this.chkIsEmergency.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkIsEmergency_PreviewKeyDown);
                this.barButtonItemSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemSave_ItemClick);
                this.barButtonItemSave1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemSave_ItemClick);
                this.ChkUpdateSereServ.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.ChkUpdateSereServ_PreviewKeyDown);
                this.CboOtherPaySource.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.CboOtherPaySource_Closed);
                this.CboOtherPaySource.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.CboOtherPaySource_ButtonClick);
                this.chkNeedSickLeaveCert.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkNeedSickLeaveCert_PreviewKeyDown);
                this.chkBHYT.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.chkBHYT_KeyDown);
                this.cboNoVienphi.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoVienphi_Properties_ButtonClick);
                this.cboNoVienphi.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoVienphi_Closed);
                this.cboNoVienphi.EditValueChanged -= new System.EventHandler(this.cboNoVienphi_EditValueChanged);
                this.cboNoVienphi.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboNoVienphi_KeyDown);
                this.txtSanPham.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtSanPham_KeyDown);
                this.TxtSoThe.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.TxtSoThe_KeyDown);
                this.spinHanMuc.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinHanMuc_KeyDown);
                this.txtCongTy.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtCongTy_KeyDown);
                this.txtTenKhachHang.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtTenKhachHang_KeyDown);
                this.dtNgayCap.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtNgayCap_KeyDown);
                this.dtThoiHanDen.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtThoiHanDen_KeyDown);
                this.dtThoiHanTu.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtThoiHanTu_KeyDown);
                this.CboCCT.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.CboCCT_Properties_ButtonClick);
                this.CboCCT.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.CboCCT_Closed);
                this.CboCCT.EditValueChanged -= new System.EventHandler(this.CboCCT_EditValueChanged);
                this.btnSovaovien.Click -= new System.EventHandler(this.btnSoravien_Click);
                this.cboDoctorUserName.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboUserName_Properties_ButtonClick);
                this.cboDoctorUserName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDoctorUserName_Closed);
                this.cboDoctorUserName.EditValueChanged -= new System.EventHandler(this.cboUserName_EditValueChanged);
                this.cboDoctorUserName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboUserName_PreviewKeyDown);
                this.txtDoctorLogginName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoginName_PreviewKeyDown);
                this.dtInTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtInTime_Closed);
                this.dtClinicalInTime.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtClinicalInTime_Closed);
                this.txtTreatmentOrder.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtTreatmentOrder_KeyDown);
                this.txtTreatmentOrder.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtTreatmentOrder_KeyPress);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dxValidationProviderTime.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderTime_ValidationFailed);
                this.Load -= new System.EventHandler(this.FormTreatmentIcdEdit_Load);
                //this.cboReasonNT.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboReasonNT_Properties_ButtonClick);
                gridView1.GridControl.DataSource = null;
                cboDoctorUserName.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                CboOtherPaySource.Properties.DataSource = null;
                cboNoVienphi.Properties.DataSource = null;
                CboCCT.Properties.DataSource = null;
                cboReasonNT.Properties.DataSource = null;
                txtReasonNTCode.Text = null;
                layoutControlItem34 = null;
                layoutControlItem33 = null;
                panelControlInIcd = null;
                panelControlInIcdSub = null;
                layoutControlItem32 = null;
                btnSovaovien = null;
                emptySpaceItem1 = null;
                label1 = null;
                layoutControlItem31 = null;
                layoutControlItem30 = null;
                chkIsEmergency = null;
                label2 = null;
                barButtonItemSave = null;
                bar2 = null;
                layoutControlItem28 = null;
                layoutControlItem29 = null;
                txtDoctorLogginName = null;
                gridView1 = null;
                cboDoctorUserName = null;
                layoutControlItem17 = null;
                panelSubIcdYhct = null;
                layoutControlItem9 = null;
                panelIcdYhct = null;
                layoutControlItem10 = null;
                LciOtherPaySource = null;
                gridLookUpEdit1View = null;
                CboOtherPaySource = null;
                ChkUpdateSereServ = null;
                layoutControlGroup5 = null;
                layoutControl5 = null;
                layoutControlItem11 = null;
                groupBox4 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                TxtSoThe = null;
                txtSanPham = null;
                txtReasonVV = null;
                layoutControlItem6 = null;
                labelControl1 = null;
                layoutControlItem27 = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                cboNoVienphi = null;
                chkBHYT = null;
                chkNeedSickLeaveCert = null;
                layoutControlItem24 = null;
                layoutControlItem23 = null;
                spinHanMuc = null;
                layoutControlGroup4 = null;
                layoutControl4 = null;
                groupBox3 = null;
                layoutControlItem22 = null;
                layoutControlItem21 = null;
                layoutControlItem20 = null;
                layoutControlItem19 = null;
                layoutControlItem18 = null;
                dtThoiHanTu = null;
                dtThoiHanDen = null;
                dtNgayCap = null;
                txtTenKhachHang = null;
                txtCongTy = null;
                layoutControlItem5 = null;
                layoutControlGroup3 = null;
                CboCCT = null;
                layoutControl3 = null;
                layoutControlItem1 = null;
                layoutControlItem14 = null;
                layoutControlItem2 = null;
                panelControlUcIcdCause = null;
                panelControlSecondIcd = null;
                groupBox2 = null;
                layoutControlItem8 = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                lciOutTime = null;
                lciClinicalInTime = null;
                lciInTime = null;
                layoutControlGroup2 = null;
                panelControlUcIcd = null;
                layoutControl2 = null;
                groupBox1 = null;
                txtTreatmentOrder = null;
                lblInCode = null;
                lblEndCode = null;
                dtClinicalInTime = null;
                dxValidationProviderTime = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItemSave1 = null;
                barManager1 = null;
                layoutControlItem7 = null;
                emptySpaceItem2 = null;
                btnSave = null;
                dtInTime = null;
                dtOutTime = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTuberculosis_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chkTuberculosis.Checked == true)
                {
                    txtNumManager.Enabled = true;
                }
                else
                {
                    txtNumManager.Enabled = false;
                    if (!string.IsNullOrEmpty(txtNumManager.Text))
                    {
                        if (MessageBox.Show("Bạn có muốn xóa số quản lý hay không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            txtNumManager.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private bool CheckMaxLength(string Str, int Length)
        {
            try
            {
                return (Str != null && Encoding.UTF8.GetByteCount(Str.Trim()) <= Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private string TruncateToMaxLength(string str, int maxLength)
        {
            if (str == null) return null;
            if (Encoding.UTF8.GetByteCount(str) <= maxLength) return str;

            int length = str.Length;
            while (length > 0)
            {
                string truncated = str.Substring(0, length);

                int byteCount = Encoding.UTF8.GetByteCount(truncated);

                if (byteCount <= maxLength)
                {
                    return truncated;
                }

                length--;
            }

            return string.Empty; // Trường hợp chuỗi hoàn toàn bị cắt bỏ
        }

        private void txtNumManager_Validated(object sender, EventArgs e)
        {
            try
            {
                string text = txtNumManager.Text;

                if (!CheckMaxLength(text, 100))
                {
                    MessageBox.Show("Không được nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    string truncatedText = TruncateToMaxLength(text, 100);
                    txtNumManager.Text = truncatedText;
                    txtNumManager.SelectionStart = txtNumManager.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReasonVV_Validated(object sender, EventArgs e)
        {
            try
            {
                //string text = txtReasonVV.Text;

                //if (!CheckMaxLength(text, 200))
                //{
                //    //MessageBox.Show("Không được nhập quá 200 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    string truncatedText = TruncateToMaxLength(text, 200);
                //    txtReasonVV.Text = truncatedText;
                //    txtReasonVV.SelectionStart = txtReasonVV.Text.Length;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReasonNT_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboReasonNT.EditValue != null)
                {
                    cboReasonNT.Properties.Buttons[1].Visible = true;

                    var data = this.HisHospitalizeReason.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE.ToLower() == cboReasonNT.EditValue.ToString().ToLower());
                    if (data != null)
                    {
                        txtReasonNTCode.Text = data.HOSPITALIZE_REASON_CODE;
                    }
                }
                else
                {
                    cboReasonNT.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReasonNT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboReasonNT.EditValue != null)
                    {
                        var data = this.HisHospitalizeReason.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE.ToLower() == cboReasonNT.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtReasonNTCode.Text = data.HOSPITALIZE_REASON_CODE;
                            cboReasonNT.Properties.Buttons[1].Visible = true;
                        }

                        this.inIcdProcessor.FocusControl(ucInIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtClinicalInTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(dtClinicalInTime.Text))
                {
                    lblReasonVV.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem39.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lblReasonVV.AppearanceItemCaption.ForeColor = Color.Black;
                    layoutControlItem39.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReasonNT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtReasonNTCode.EditValue = null;
                    cboReasonNT.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReasonNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboReasonNT.EditValue != null)
                    {
                        var data = this.HisHospitalizeReason.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE.ToLower() == cboReasonNT.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtReasonNTCode.Text = data.HOSPITALIZE_REASON_CODE;
                            cboReasonNT.Properties.Buttons[1].Visible = true;
                        }
                        this.inIcdProcessor.FocusControl(ucInIcd);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboReasonNT.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Dangth
        private void btnContentSubclinicalEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance
                        .GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnInfomationExecute_Click(object sender, EventArgs e)
        {
            try
            {
                dataSelectedToPTDT = new List<string>();
                string contentShare = txtTreatmentInstruction.Text.Trim();
                if (!string.IsNullOrEmpty(contentShare))
                {
                    if (contentShare.Contains(";"))
                    {
                        string[] serviceName = contentShare.Split(';');
                        foreach (var item in serviceName)
                        {
                            dataSelectedToPTDT.Add(item.Trim());
                        }
                    }
                    else
                    {
                        dataSelectedToPTDT.Add(contentShare);
                    }
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfomationExecute").FirstOrDefault();

                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfomationExecute'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add(this.dataSelectedToPTDT);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)dataResult);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();

                }
                else
                {
                    MessageManager.Show("Chức năng chưa được hỗ trợ ở phiên bản này");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
