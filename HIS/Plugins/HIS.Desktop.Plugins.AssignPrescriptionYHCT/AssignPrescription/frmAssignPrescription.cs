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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Base;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ChooseICD;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.Icd;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraBars.Controls;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        bool isNotLoadWhileChangeInstructionTimeInFirst;
        long? serviceReqParentId;
        long treatmentId = 0;
        string treatmentCode;
        int actionBosung = 0;
        int positionHandle = 0;
        int positionHandle__DuongDung = 0;
        internal int positionHandleControl = -1;
        internal int actionType = 0;
        internal int actionTypePrint = 0;
        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal int idRow = 1;
        internal long InstructionTime { get; set; }
        internal bool limitHeinMedicinePrice = false;
        internal string TreatmentMethod;
        internal V_HIS_SERE_SERV currentSereServ { get; set; }
        internal V_HIS_SERE_SERV currentSereServInEkip { get; set; }
        V_HIS_SERVICE Service__Main;
        decimal currentExpendInServicePackage;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessRefeshIcd processRefeshIcd;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessWhileAutoTreatmentEnd processWhileAutoTreatmentEnd;
        bool isInKip;
        string patientName;
        internal long patientDob;
        string genderName;
        long patientId;

        internal bool isAutoCheckExpend = false;
        internal const int stepRow = 1;
        internal decimal totalPriceBHYT = 0;
        decimal totalHeinByTreatment = 0;
        internal HisTreatmentWithPatientTypeInfoSDO currentTreatmentWithPatientType { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypes = null;
        HIS_MEDICINE_TYPE_TUT medicineTypeTutSelected;
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;

        internal List<MediMatyTypeADO> mediMatyTypeADOs;
        internal MediMatyTypeADO currentMedicineTypeADOForEdit;

        internal List<DMediStock1ADO> mediStockD1ADOs;
        internal List<V_HIS_MEDICINE_TYPE> currentMedicineTypes;
        List<V_HIS_MATERIAL_TYPE> currentMaterialTypes;

        internal HIS_ICD icdChoose { get; set; }

        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();

        public Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;

        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages;
        List<V_HIS_SERVICE_PACKAGE> servicePackageByServices;

        OutPatientPresResultSDO outPrescriptionResultSDOs;
        InPatientPresResultSDO inPrescriptionResultSDOs;

        bool showMediStockDSDOs = false;

        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMestRoomByRooms;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByHeaderCard;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByNotInHeaderCard;
        AssignPrescriptionEditADO assignPrescriptionEditADO;
        decimal amountInput = 0;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        string[] periodSeparators = new string[] { "," };
        internal long oldExpMestId;
        internal HIS_EXP_MEST oldExpMest;
        internal HIS_SERVICE_REQ oldServiceReq;
        List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> listMedicineBeanForEdits = new List<V_HIS_MEDICINE_BEAN_1>();
        List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> listMaterialBeanForEdits = new List<V_HIS_MATERIAL_BEAN_1>();
        CommonParam paramCommon;
        internal IcdProcessor icdProcessor;
        internal IcdProcessor icdProcessorYHCT;
        internal UserControl ucIcd;
        internal UserControl ucIcdYHCT;
        internal IcdProcessor icdCauseProcessor;
        internal UserControl ucIcdCause;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal SecondaryIcdProcessor subIcdProcessorYHCT;
        internal UserControl ucSecondaryIcd;
        internal UserControl ucSecondaryIcdYHCT;
        internal UCDateProcessor ucDateProcessor;
        internal UserControl ucDate;
        internal PeriousExpMestListProcessor periousExpMestListProcessor;
        internal UserControl ucPeriousExpMestList;
        internal TreatmentFinishProcessor treatmentFinishProcessor;
        internal UserControl ucTreatmentFinish;
        internal PatientSelectProcessor patientSelectProcessor;
        internal UserControl ucPatientSelect;

        int theRequiredWidth = 900, theRequiredHeight = 130;
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        internal bool bIsSelectMultiPatientProcessing = false;
        CommonParam paramSaveList;
        bool successSaveList;

        List<TrackingADO> trackingADOs { get; set; }
        List<HIS_ALLERGENIC> allergenics { get; set; }


        //Bien luu thong tin don thuoc cu

        List<HIS_SERVICE_REQ> serviceReqPrints { get; set; }
        List<HIS_EXP_MEST> expMestPrints { get; set; }
        List<HIS_EXP_MEST_MEDICINE> expMestMedicinePrints { get; set; }
        List<HIS_EXP_MEST_MATERIAL> expMestMaterialPrints { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineEditPrints { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialEditPrints { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> serviceReqMetys { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> serviceReqMatys { get; set; }
        
        public List<V_HIS_EXP_MEST_MEDICINE> LstExpMestMedicine { get; set; }

        public long prescriptionTypeId { get; set; }
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;
        MOS.Filter.HisServiceReqView7Filter currentPrescriptionFilter;
        long ContructorIntructionTime;
        internal bool isNotChangeTutorial = false;

        internal List<V_HIS_MEDI_STOCK> currentMediStockNhaThuocSelecteds = null;
        internal List<D_HIS_MEDI_STOCK_2> mediMatyTypeAvailables;
        internal List<V_HIS_MEDI_STOCK> mediStockAllows;

        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        internal HIS_TREATMENT Histreatment;
        public string msgTuVong { get; set; }

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        internal List<HIS_EXP_MEST_REASON> lstExpMestReasons;
        bool MediStockNull = false;
        public long? TIME_TO { get; set; }
        public List<long> USE_TIME { get; set; }
        public bool IsManyDay { get; set; }
        public long Assign_time { get; set; }
        #endregion

        #region Construct
        public frmAssignPrescription(Inventec.Desktop.Common.Modules.Module module, AssignPrescriptionADO data)
            : base(module)
        {
            try
            {
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                this.LoadDonThuocTruocDay();
                this.actionType = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.actionTypePrint = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.currentModule = module;
                this.processDataResult = data.DgProcessDataResult;
                this.processRefeshIcd = data.DgProcessRefeshIcd;
                this.processWhileAutoTreatmentEnd = data.DlgWhileAutoTreatmentEnd;
                this.treatmentId = data.TreatmentId;
                this.prescriptionTypeId = data.PrescriptionTypeId == PrescriptionTypeCFG.Y_HOC_CO_TRUYEN ? PrescriptionTypeCFG.Y_HOC_CO_TRUYEN : PrescriptionTypeCFG.TAN_DON_DUOC;
                this.treatmentCode = data.TreatmentCode;
                if (data.ServiceReqId > 0)
                    this.serviceReqParentId = data.ServiceReqId;
                this.isInKip = data.IsInKip;
                GlobalStore.IsCabinet = data.IsCabinet;
                GlobalStore.IsTreatmentIn = data.IsExecutePTTT;
                GlobalStore.IsExecutePTTT = data.IsExecutePTTT;
                this.patientName = data.PatientName;
                this.patientDob = data.PatientDob;
                this.genderName = data.GenderName;
                if (this.isInKip)
                    this.currentSereServInEkip = data.SereServ;
                else
                    this.currentSereServ = data.SereServ;
                this.isAutoCheckExpend = data.IsAutoCheckExpend;
                this.assignPrescriptionEditADO = data.AssignPrescriptionEditADO;
                this.icdExam = data.IcdExam;
                this.ContructorIntructionTime = data.IntructionTime;
                this.tracking = data.Tracking;

                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription.frmAssignPrescription).Assembly);
                this.InitAssignPresctiptionType();
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        bool isCheckAssignServiceSimultaneityOption = false;
        private void CheckAssignServiceSimultaneityOption()
        {

            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                }
                else if (this.actionType == GlobalVariables.ActionEdit)
                {
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                }
                else
                {
                    this.btnSave.Enabled = this.btnAdd.Enabled = btnSaveAndPrint.Enabled = false;
                }
                isCheckAssignServiceSimultaneityOption = false;
                if ((HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "1" && HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "2") || cboUser.EditValue == null || intructionTimeSelecteds == null || intructionTimeSelecteds.Count == 0)
                    return;
                CommonParam param = new CommonParam();
                HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                sdo.TreatmentId = treatmentId;
                sdo.Loginnames = new List<string> { cboUser.EditValue.ToString() };
                sdo.SereTimes = intructionTimeSelecteds;
                var CheckSereTimes = new BackendAdapter(param).Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
                if (!CheckSereTimes)
                {
                    if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                    {
                        isCheckAssignServiceSimultaneityOption = true;
                        btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                        MessageManager.Show(this, param, CheckSereTimes);
                    }
                    else if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                    {
                        if (XtraMessageBox.Show(param.GetMessage(), "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            isCheckAssignServiceSimultaneityOption = true;
                            btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                        }
                    }
                }
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
                if (this.currentModule != null)
                {
                    this.currentWorkPlace = WorkPlace.GetWorkPlace(this.currentModule);
                    if (this.currentWorkPlace == null)
                        LogSystem.Warn("Get current WorkPlace theo phòng làm việc hiện tại không có dữ liệu, RoomId = " + GetRoomId() + " | RoomTypeId = " + GetRoomTypeId());

                    if (!String.IsNullOrEmpty(this.currentModule.text))
                        this.Text = this.currentModule.text;
                }

                if (GlobalStore.IsCabinet)
                {
                    if (GlobalStore.IsTreatmentIn)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    else
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                }
                else if (GlobalStore.IsTreatmentIn)
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                else
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt

                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barbtnSaveAndPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnSaveAndPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barbtnPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnNew.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bbtnF2.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF2.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bbtnBoSung.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnBoSung.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bbtnF3.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF3.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl6.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.chkYhct.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkYhct.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkYhct.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkYhct.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnNew.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnShowDetail.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnShowDetail.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtLoiDanBacSi.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboExpMestTemplate.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboExpMestTemplate.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboMediStockExport.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboMediStockExport.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcNumOrder.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcManuMedicineName__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcManuMedicineName__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcUnit__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUnit__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcAmount__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcAmount__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcPatientType__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcPatientType__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcExpend__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcExpend__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcIsOutKtcFee__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsOutKtcFee__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.grcUseForm__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUseForm__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemcboMedicineUseForm.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboMedicineUseForm.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcConcentra__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcConcentra__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcMediStockExpMest__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcMediStockExpMest__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcKHBHYT__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcKHBHYT__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcPrice__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcPrice__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcTotalPrice__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTotalPrice__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcUseTimeTo__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUseTimeTo__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabMedicine.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboPatientType_TabMedicine.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabMedicine_GridLookUp.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboPatientType_TabMedicine_GridLookUp.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboMedicineUseForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboMedicineUseForm.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.rdOpionGroup.Properties.Items[0].Description = Inventec.Common.Resource.Get.Value("frmAssignPrescription.xtraTabPage__MediMaty.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.rdOpionGroup.Properties.Items[1].Description = Inventec.Common.Resource.Get.Value("frmAssignPrescription.xtraTabPageMedicine_MedicineCategory.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.txtUnitOther.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtUnitOther.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtMedicineTypeOther.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtMedicineTypeOther.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciMedicineUseForm.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMedicineUseForm.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciAmount.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTongTien.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTongTien.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentTypeName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTreatmentTypeName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciMediStockExpMest.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMediStockExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciExpMestTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciLadder.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciLadder.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciLoiDanBacSi.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciLoiDanBacSi.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcgPrevousPrescription.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPriviousExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPatientName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciDob.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciGenderName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkYhct.Name)
                        {
                            chkYhct.Checked = item.VALUE == "1";
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        Timer timer = new Timer();
        #region Load
        private void frmAssignPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                this.LoadHisTreatment();
                InitMultipleThread();
                this.LoadExpMestReason();
                InitControlState();
                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                this.LoadDataToLocal();
                LogSystem.Debug("Starting...");
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.ResetDataForm();
                this.SetDefaultData();
                this.SetDefaultUC();
                LogSystem.Debug("Loaded SetDefaultUC");
                this.InitCurrentMestRoom();
                this.ReSetDataInputAfterAdd__MedicinePage();
                this.InitTabIndex();
                LogSystem.Debug("Loaded default data");
                this.FillDataToControlsForm();
                this.ValidateForm();
                this.ValidateBosung();
                this.VisibleButton(this.actionBosung);
                LogSystem.Debug("Loaded fillDataToControlsForm");
                this.InitMenuToButtonPrint();
                LogSystem.Debug("Loaded InitMenuToButtonPrint");
                LogSystem.Debug("Loaded LoadDefaultUser");

                this.InitControlByConfig();
                this.VisibleExecuteGroupByConfig();
                this.VisibleColumnInGridControlService();

                this.LoadDefaultTabpageMedicine();
                this.LoadDataByServicePackage();
                this.LoadPrescriptionForEdit();
                this.LoadTotalSereServByHeinWithTreatment(treatmentId);

                this.SetEnableButtonControl(this.actionType);
                this.LoadDataTracking();
                this.InitComboTracking(cboPhieuDieuTri);
                this.InitComboExpMestReason();
                this.InitComborepositoryItemCustomGridLookUpReasion();
                this.GetListExpMestMedicine();
                
                Task.Run(() => this.LoadAllergenic(currentTreatmentWithPatientType.PATIENT_ID));
                LogSystem.Debug("Loaded end");
                this.AddBarManager(this.barManager1);

                this.layoutControlItemDutru.Visibility = (GlobalStore.IsTreatmentIn 
                    && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //panelControl2.Enabled = !this.isMultiDateState;
                this.layoutControlItemTimeTo.Visibility = (GlobalStore.IsTreatmentIn
                    && !GlobalStore.IsCabinet) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            InitMultipleThread();
            timer.Stop();
        }

        private void LoadExpMestReason()
        {
            try
            {
                this.lstExpMestReasons = new List<HIS_EXP_MEST_REASON>();
                this.lstExpMestReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAssignPrescription_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                lblTongTien.Focus();
                if (this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit)
                {
                    var mediMatyTypeNotEdits = this.mediMatyTypeADOs != null ? this.mediMatyTypeADOs.Where(o => o.IsEdit == false) : null;
                    if (mediMatyTypeNotEdits != null && mediMatyTypeNotEdits.Count() > 0 && btnSave.Enabled == true)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(ResourceMessage.CanhBaoThuocChuaLuuTatForm, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    ReleaseAllMediByUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitAssignPresctiptionType()
        {
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                    //Set mặc định trường "là nội trú" hay không theo loại phòng
                    GlobalStore.IsTreatmentIn = (this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG);

                //Kiểm tra nếu sửa đơn thuốc thì sẽ set lại giá trị của trường "là nội trú" theo phòng yêu cầu của đơn cũ
                //Và set giá trị trường "là tủ trực" nếu đơn cũ là đơn tủ trực
                bool? isFromTypeOut = IsFromTypeOut();
                bool? isFromTypeTT = IsFromTypeTTByEditor();
                if (isFromTypeOut.HasValue && isFromTypeTT.HasValue)
                {
                    if (!GlobalStore.IsTreatmentIn) // Nếu được truyền ở bên ngoài thì không phải kiểm tra
                    {
                        if (isFromTypeOut == true)
                        {
                            GlobalStore.IsTreatmentIn = false;
                        }
                        else
                        {
                            GlobalStore.IsTreatmentIn = true;
                        }
                    }

                    if (!GlobalStore.IsCabinet) // Nếu được truyền ở bên ngoài thì không phải kiểm tra
                    {
                        if (isFromTypeTT.HasValue)
                        {
                            GlobalStore.IsCabinet = isFromTypeTT.Value;
                        }
                    }
                }

                //Loại đơn


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Button

        private void btnNew_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(New_Click, "btnNew_Click");
        }

        private void New_Click()
        {
            try
            {
                if (this.cboMediStockExport.Enabled)
                {
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                    }
                }

                WaitingManager.Show();
                if (this.actionType == GlobalVariables.ActionAdd)
                    this.ReleaseAllMediByUser();
                this.isCheckAssignServiceSimultaneityOption = false;
                this.SetDefaultData();
                this.SetDefaultUC();
                this.ReSetDataInputAfterAdd__MedicinePage();
                this.LoadSereServTotalHeinPriceWithTreatment(this.treatmentId);
                this.icdProcessor.ResetValidate(this.ucIcd);
                this.icdCauseProcessor.ResetValidate(this.ucIcdCause);
                this.subIcdProcessor.ResetValidate(this.ucSecondaryIcd);
                this.ucDateProcessor.ResetValidation(ucDate);
                this.ReloadDataAvaiableMediBeanInCombo();
                this.ResetFocusMediMaty(true);
                this.InitComboMediStockAllow(0);
                this.InitComboExpMestReason();
                this.LoadDefaultTabpageMedicine();
                if (this.rdOpionGroup.SelectedIndex == 0)
                {
                    this.InitDataMetyMatyTypeInStockD();
                    this.theRequiredWidth = 1130;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer(this.GetDataMediMatyInStock());
                }
                else 
                {
                    this.InitComboNhaThuoc();
                }
                CheckAssignServiceSimultaneityOption();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");
                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();//treatmentId, intructionTime, serviceReqParentId ?? 0
                treatment.ID = treatmentId;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
            }
        }

        private void btnSereservInTreatmentPreview_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereservInTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SereservInTreatment'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SereservInTreatment' is not plugins");

                SereservInTreatmentADO sereservInTreatmentADO = new SereservInTreatmentADO(treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First(), serviceReqParentId ?? 0);
                List<object> listArgs = new List<object>();
                listArgs.Add(sereservInTreatmentADO);
                listArgs.Add(this.currentModule);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveTemplate__MedicinePage_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                frmHisExpMestTemplateCreate frm = new frmHisExpMestTemplateCreate(mediMatyTypeADOs, RefeshExpMestTemplate, this.currentModule);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //this.ProcessSaveForListSelect(false);
                LogTheadInSessionInfo(() => this.ProcessSaveForListSelect(false), "btnSave_Click");
                //this.ProcessSaveData(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //this.ProcessSaveForListSelect(true);
                LogTheadInSessionInfo(() => this.ProcessSaveForListSelect(true), "btnSaveAndPrint_Click");
                //this.ProcessSaveData(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kê đơn phòng khám khi click vào nút lưu sẽ xử lý gọi api server => tạo/tách/ sửa/ xóa bean của thuốc luôn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_TabMedicine_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(Add_TabMedicine, "btnAdd_TabMedicine_Click");
        }

        private void Add_TabMedicine()
        {
            try
            {
                LogSystem.Debug("btnAdd_TabMedicine_Click => 1");
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Debug("btnAdd_TabMedicine_Click => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }
                LogSystem.Debug("btnAdd_TabMedicine_Click => 2");
                if (this.currentMedicineTypeADOForEdit == null || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU && this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                {
                    dxValidProviderBoXung.SetValidationRule(txtLadder, null);
                }
                else
                {
                    this.ValidationSingleControl(txtLadder, this.dxValidProviderBoXung);
                }
                LogSystem.Debug("btnAdd_TabMedicine_Click => 3");
                bool valid = true;
                this.positionHandleControl = -1;
                this.paramCommon = new CommonParam();
                valid = valid && (dxValidProviderBoXung.Validate());

                List<string> paramMessageErrorOther = new List<string>();
                List<string> paramMessageErrorEmpty = new List<string>();
                if (!valid)
                {

                    if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        this.ModuleControls = controlProcess.GetControls(this);
                    }

                    GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidProviderBoXung, this.ModuleControls, paramMessageErrorEmpty, paramMessageErrorOther);

                    if (paramMessageErrorOther.Count > 0)
                    {
                        paramMessageErrorOther = paramMessageErrorOther.Distinct().ToList();
                        paramCommon.Messages.AddRange(paramMessageErrorOther);
                    }
                    if (paramMessageErrorEmpty.Count > 0)
                    {
                        paramMessageErrorEmpty = paramMessageErrorEmpty.Distinct().ToList();
                        paramCommon.Messages.Add(String.Join(", ", paramMessageErrorEmpty) + " " + Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    }
                    string warning = this.paramCommon.GetMessage();
                    if (!String.IsNullOrEmpty(warning))
                    {
                        MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warning), warning));
                    }
                }

                valid = valid && CheckAllergenicByPatient();

                valid = valid && CheckContraidication();

                if (!valid)
                    return;
                LogSystem.Debug("btnAdd_TabMedicine_Click => 4");

                if (this.mediMatyTypeADOs == null)
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                switch (this.actionBosung)
                {
                    case GlobalVariables.ActionAdd:
                        LogSystem.Debug("btnAdd_TabMedicine_Click => ActionAdd__" + this.actionBosung);
                        AddMediMatyClickHandler();
                        break;
                    case GlobalVariables.ActionEdit:
                        LogSystem.Debug("btnAdd_TabMedicine_Click => ActionEdit__" + this.actionBosung);
                        UpdateMediMatyClickHandler();
                        break;
                    default:
                        LogSystem.Debug("btnAdd_TabMedicine_Click => thao tac khong hop le. actionBosung = " + this.actionBosung);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckContraidication()
        {
            bool result = true;
            try
            {
                if (this.currentMedicineTypeADOForEdit != null && !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONTRAINDICATION))
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME, this.currentMedicineTypeADOForEdit.CONTRAINDICATION), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (myResult != DialogResult.Yes)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckAllergenicByPatient()
        {
            bool result = true;
            try
            {
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    if (allergenics == null || allergenics.Count == 0)
                        return result;
                    HIS_ALLERGENIC allergencic = allergenics.FirstOrDefault(o => o.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID);
                    if (allergencic != null)
                    {
                        if (allergencic.IS_SURE == 1)
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(String.Format("Bệnh nhân dị ứng với thuốc {0}. Bạn có muốn tiếp tục không", this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
                            {
                                result = false;
                            }
                        }
                        else if (allergencic.IS_DOUBT == 1)
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(String.Format("Bệnh nhân nghi ngờ dị ứng với thuốc {0}. Bạn có muốn tiếp tục không", this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnTomLuocVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AggrHospitalFees' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.spinAmount.Focus();
                this.spinAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnF4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    this.patientSelectProcessor.FocusSearchTextbox(this.ucPatientSelect);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnBoSung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd
                    || this.actionType == GlobalVariables.ActionEdit)
                {
                    this.btnAdd_TabMedicine_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSave.Enabled)
                {
                    this.btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnSaveAndPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint.Focus();
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSaveAndPrint.Enabled)
                {
                    this.btnSaveAndPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView && this.lciPrintAssignPrescription.Enabled)
                {
                    this.btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnNew.Enabled)
                    this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //this.InDonThuocButton(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Control editor
        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetComboNhaThuoc()
        {
            try
            {
                this.cboNhaThuoc.EditValue = null;
                //this.cboNhaThuoc.Properties.Buttons[1].Visible = false;
                this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();

                GridCheckMarksSelection gridCheckMark = this.cboNhaThuoc.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(this.cboNhaThuoc.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                this.InitComboTracking(cboPhieuDieuTri);
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate).OrderByDescending(o => o).ToList();
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);

                ChangeIntructionTime(time);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTime(DateTime intructTime)
        {
            try
            {
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);
                panelControl2.Enabled = !isMultiDateState;
                if (!panelControl2.Enabled)
                {
                    this.USE_TIME.Clear();
                    txtDuTruTime.Text = "";
                }
                this.InstructionTime = (intructionTimeSelecteds != null && intructionTimeSelecteds.Count > 0) ? intructionTimeSelecteds.OrderByDescending(o => o).First() : 0;
                //long instructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();
                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.InstructionTime);
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, this.InstructionTime);
                this.LoadTotalSereServByHeinWithTreatment(treatmentId);
                this.PatientTypeWithTreatmentView7();
                this.InitComboRepositoryPatientType(this.repositoryItemcboPatientType_TabMedicine_GridLookUp, this.currentPatientTypeWithPatientTypeAlter);
                this.FillTreatmentInfo__PatientType();
                this.GetListExpMestMedicine();
                cboPhieuDieuTri.EditValue = null;
                this.LoadDataTracking();
                this.InitComboTracking(cboPhieuDieuTri);

                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime).Value;
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        if (item.UseDays > 0)
                        {
                            DateTime dtUseTimeTo = dtUseTime.AddDays((double)(item.UseDays.Value - 1));
                            item.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTimeTo);
                        }
                    }
                    gridControlServiceProcess.RefreshDataSource();
                }

                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    this.InitDataMetyMatyTypeInStockD();
                }
                CheckAssignServiceSimultaneityOption();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChangeMediStock()
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();
                if (rdOpionGroup.SelectedIndex == 0)
                {
                    this.RebuildMediMatyWithInControlContainer(GetDataMediMatyInStock());
                }
                else
                    rdOpionGroup.SelectedIndex = 0;

                //LogSystem.Debug("Load du lieu thuoc theo cac kho dang chon khi thay doi kho____danh sach cac kho:" + String.Join(",", currentMediStock.Select(o => o.MEDI_STOCK_CODE)) + "____ket qua tim thay " + (this.mediStockD1SDOs != null ? this.mediStockD1SDOs.Count : 0));
                if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    this.idRow = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private int FindRowHandleByRowObject(DevExpress.XtraGrid.Views.Grid.GridView view, object row)
        {
            if (row != null)
            {
                for (int i = 0; i < view.DataRowCount; i++)
                    if (row.Equals(view.GetRow(i)))
                        return i;
            }
            return DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        }

        private void SaveDataAndRefesh()
        {
            try
            {
                gridViewServiceProcess.BeginUpdate();
                gridViewServiceProcess.GridControl.DataSource = mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                gridViewServiceProcess.EndUpdate();
                ReSetDataInputAfterAdd__MedicinePage();
                SetEnableButtonControl(this.actionType);
                ResetFocusMediMaty(true);
                SetTotalPrice__TrongDon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(this.cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            CheckAssignServiceSimultaneityOption();
                            this.RebuildMediMatyWithInControlContainer(this.GetDataMediMatyInStock());
                            this.ResetFocusMediMaty(true);
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(this.cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboUser.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;

                            CheckAssignServiceSimultaneityOption();
                        }
                    }

                    this.ResetFocusMediMaty(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboUser.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;

                            CheckAssignServiceSimultaneityOption();
                            this.ResetFocusMediMaty(true);
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.ResetFocusMediMaty(false);
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                    this.cboMediStockExport.EditValue = null;
                    this.cboMediStockExport.Properties.Buttons[1].Visible = false;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediStockD1ADOs = new List<DMediStock1ADO>();
                    this.idRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    WaitingManager.Show();
                    this.ProcessChangeMediStock();
                    this.spinSoNgay.Focus();
                    this.spinSoNgay.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    this.ProcessChangeMediStock();
                    this.spinSoNgay.Focus();
                    this.spinSoNgay.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_MEST_ROOM> mestRoomSelectedNews = new List<V_HIS_MEST_ROOM>();
                    foreach (MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.MEDI_STOCK_NAME.ToString());
                            mestRoomSelectedNews.Add(rv);
                        }
                    }
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    this.currentMediStock.AddRange(mestRoomSelectedNews);
                }

                this.cboMediStockExport.Text = sb.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTemplate_Medicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    this.LoadTemplateMedicineCombo(this.cboExpMestTemplate, this.txtExpMestTemplateCode, strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTemplateMedicineCombo(GridLookUpEdit cboTemplateMedicine, TextEdit txtTemplateMedicineCode, string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboTemplateMedicine.Properties.Buttons[1].Visible = false;
                    cboTemplateMedicine.EditValue = null;
                    this.FocusShowpopup(cboTemplateMedicine, false);
                }
                else
                {
                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> lstData = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>);
                    var data = lstData.Where(o => o.EXP_MEST_TEMPLATE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.EXP_MEST_TEMPLATE_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        cboTemplateMedicine.Properties.Buttons[1].Visible = true;
                        cboTemplateMedicine.EditValue = searchResult[0].ID;
                        txtTemplateMedicineCode.Text = searchResult[0].EXP_MEST_TEMPLATE_CODE;
                        this.ProcessChoiceExpMestTemplate(searchResult[0]);
                        this.ResetFocusMediMaty(!MediStockNull);
                    }
                    else
                    {
                        cboTemplateMedicine.Properties.Buttons[1].Visible = false;
                        cboTemplateMedicine.EditValue = null;
                        this.FocusShowpopup(cboTemplateMedicine, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                    this.cboExpMestTemplate.EditValue = null;
                    this.txtExpMestTemplateCode.Text = "";
                    this.txtExpMestTemplateCode.Focus();
                    this.txtExpMestTemplateCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    
                    List<object> listObj = new List<object>();

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisExpMestTemplate", currentModule.RoomId, currentModule.RoomTypeId, listObj);
                    this.InitComboExpMestTemplate();
                } 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboExpMestTemplate.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExpMestTemplate.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceExpMestTemplate(data);
                            this.ResetFocusMediMaty(!MediStockNull);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExpMestTemplate.EditValue != null)
                    {
                        WaitingManager.Show();
                        HIS_EXP_MEST_TEMPLATE data = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExpMestTemplate.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceExpMestTemplate(data);
                            this.ResetFocusMediMaty(!MediStockNull);
                        }
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTemplate_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboExpMestTemplate.Text.Trim()) && this.cboExpMestTemplate.EditValue != null)
                {
                    this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                    this.cboExpMestTemplate.EditValue = null;
                    this.txtExpMestTemplateCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLadder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtMediMatyForPrescription.Focus();
                    this.txtMediMatyForPrescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMedicineUseForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMedicineUseForm.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                        }
                    }
                    this.FocusMedicineUseForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                    this.cboMedicineUseForm.EditValue = null;
                    this.FocusShowpopup(this.cboMedicineUseForm, false);
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMedicineUseForm").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisMedicineUseForm'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisMedicineUseForm' is not plugins");

                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                    this.InitComboMedicineUseForm(this.cboMedicineUseForm, null);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMedicineUseForm.EditValue ?? "").ToString()) > 0)
                    {
                        this.cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) && this.cboMedicineUseForm.EditValue != null)
                {
                    this.cboMedicineUseForm.EditValue = null;
                    this.cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinUseDays_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnAdd.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoiDanBacSi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TextLibrary'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.TextLibrary' is not plugins");

                    List<object> listArgs = new List<object>();
                    listArgs.Add("loidanbacsi");
                    listArgs.Add(this.currentModule);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    this.btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoNgay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdOpionGroup.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string vl = (sender as DevExpress.XtraEditors.TextEdit).Text;
                try
                {
                    if (vl.Contains(".") || vl.Contains(","))
                    {
                        vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        amountInput = Convert.ToDecimal(vl);
                    }
                    else if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            amountInput = Convert.ToDecimal(arrNumber[0]) / Convert.ToDecimal(arrNumber[1]);

                        }
                    }
                }
                catch (Exception ex)
                {
                    amountInput = 0;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextEdit).Text.IndexOf('.') > -1 || (sender as TextEdit).Text.IndexOf('/') > -1))
                {
                    e.Handled = true;
                }
                if ((e.KeyChar == '/') && ((sender as TextEdit).Text.IndexOf('/') > -1 || (sender as TextEdit).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount__MedicinePage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtHuongDan.Focus();
                    //this.txtHuongDan.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeOther_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtUnitOther.Focus();
                    this.txtUnitOther.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUnitOther_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeOther_Leave(object sender, EventArgs e)
        {
            try
            {
                this.btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdOpionGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("rdOpionGroup_SelectedIndexChanged => 1");
                LoadDataToGridMetyMatyTypeInStock();
                SetEnableControlMedicine();
                ValidateForm();
                LogSystem.Debug("rdOpionGroup_SelectedIndexChanged => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    txtMediMatyForPrescription.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{1}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{1}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{1}%' OR [PARENT_NAME] Like '%{0}%'", txtMediMatyForPrescription.Text, HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO.StringUtil.convertToUnSign3(txtMediMatyForPrescription.Text));
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                            isShow = false;
                        }

                        txtMediMatyForPrescription.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));

                        if (this.currentMedicineTypeADOForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMatyTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && listDatas[i].MEDI_STOCK_ID == this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (gridViewMediMaty.RowCount == 1)
                    //{
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        if (rdOpionGroup.SelectedIndex == 0)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                    }
                    //}
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMediMatyForPrescription.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPhieuDieuTri_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhieuDieuTri.EditValue != null)
                    {
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                        LoadICDFromTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadICDFromTracking()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);
                if(trackings != null)
                {
                    var currentTracking = trackings.FirstOrDefault(s => s.ID == Convert.ToInt64(cboPhieuDieuTri.EditValue));
                    if(currentTracking != null)
                    {
                        if ((string.IsNullOrEmpty(HisConfigs.Get<string>("HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION")) || (HisConfigs.Get<string>("HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION") != "1") ) 
                            && (!string.IsNullOrEmpty(HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd")) || (HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd") == "1")))
                        {
                            LogSystem.Debug("cau hinh :  HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION khong duoc bat va cau hinh HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd duoc bat. -> load icd yhct");
                            if(!string.IsNullOrEmpty(currentTracking.TRADITIONAL_ICD_CODE))
                                LoadICDFoUC("", "", "", "", currentTracking.TRADITIONAL_ICD_CODE, currentTracking.TRADITIONAL_ICD_NAME, currentTracking.TRADITIONAL_ICD_SUB_CODE, currentTracking.TRADITIONAL_ICD_TEXT);
                        }
                        else
                        {
                            LogSystem.Debug("KHONG LOAD ICD KHI TO DIEU TRI THAY DOI.(Dieu kien : SERVICE_REQ_ICD_OPTION  <> 1 va UpdateTreatmentIcd  == 1)");
                            LogSystem.Debug("cau hinh  HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION ): " + HisConfigs.Get<string>("HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION")
                                + " ,cau hinh HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd : "+ HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadICDFoUC(string iCD_CODE, string iCD_NAME, string iCD_SUB_CODE, string iCD_TEXT,
            string tRADITIONAL_ICD_CODE, string tRADITIONAL_ICD_NAME,string tRADITIONAL_ICD_SUB_CODE, string tRADITIONAL_ICD_TEXT)
        {
            try
            {
                if (!string.IsNullOrEmpty(iCD_CODE))
                {
                    HIS.UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                    icd.ICD_CODE = iCD_CODE;
                    icd.ICD_NAME = iCD_NAME;
                    if(ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }
                }
                
                if (!string.IsNullOrEmpty(iCD_SUB_CODE))
                {
                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subICD = new SecondaryIcdDataADO();
                    subICD.ICD_SUB_CODE = iCD_SUB_CODE;
                    subICD.ICD_TEXT = iCD_TEXT;
                    if(ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subICD);
                    }
                }

                if (!string.IsNullOrEmpty(tRADITIONAL_ICD_CODE))
                {
                    HIS.UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                    icd.ICD_CODE = tRADITIONAL_ICD_CODE;
                    icd.ICD_NAME = tRADITIONAL_ICD_NAME;
                    if (ucIcdYHCT != null)
                    {
                        icdProcessorYHCT.Reload(ucIcdYHCT, icd);
                    }
                }

                if (!string.IsNullOrEmpty(tRADITIONAL_ICD_SUB_CODE))
                {
                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subICD = new SecondaryIcdDataADO();
                    subICD.ICD_SUB_CODE = tRADITIONAL_ICD_SUB_CODE;
                    subICD.ICD_TEXT = tRADITIONAL_ICD_TEXT;
                    if (ucSecondaryIcdYHCT != null)
                    {
                        subIcdProcessorYHCT.Reload(ucSecondaryIcdYHCT, subICD);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                    cboPhieuDieuTri.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.treatmentId);
                        listArgs.Add((Action<HIS_TRACKING>)this.ProcessAfterChangeTrackingTime);//TODO
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.LoadDataTracking();
                        this.InitComboTracking(cboPhieuDieuTri);
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = (cboPhieuDieuTri.EditValue != null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBoSungPhacDo_Click(object sender, EventArgs e)
        {
            try
            {
                //Lay danh sach icd
                string icdCode = "";
                var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                {
                    icdCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        icdCode += ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    }
                }

                if (String.IsNullOrEmpty(icdCode))
                {
                    MessageBox.Show("Không tìm thấy thông tin ICD", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (this.mediMatyTypeADOs == null || this.mediMatyTypeADOs.Count == 0)
                {
                    MessageBox.Show("Vui lòng kê thuốc vật tư", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                string[] icdCodeArr = icdCode.Split(';');
                List<long> serviceIds = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).ToList();
                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.SERVICE_IDs = serviceIds;
                icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.ToList();
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                List<long> icdServiceIds = icdServices.Select(o => o.SERVICE_ID ?? 0).Distinct().ToList();
                List<long> serviceNotConfigIds = new List<long>();
                foreach (var item in serviceIds)
                {
                    if (!icdServiceIds.Contains(item))
                    {
                        serviceNotConfigIds.Add(item);
                    }
                }

                if (serviceNotConfigIds == null || serviceNotConfigIds.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy thuốc vật tư chưa được cấu hình chẩn đoán ICD", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(o => icdCodeArr.Contains(o.ICD_CODE)).Distinct().ToList();
                if (icds == null || icds.Count == 0)
                {
                    LogSystem.Debug("Khong tim thay ICD");
                    return;
                }

                if (icds.Count == 1)
                {
                    icdChoose = icds[0];
                }
                else
                {
                    //Mo form chon icd
                    icdChoose = new HIS_ICD();
                    frmChooseICD frm = new frmChooseICD(icds, refeshChooseIcd);
                    frm.ShowDialog();
                }


                if (icdChoose == null || icdChoose.ID == 0)
                    return;

                List<object> listObj = new List<object>();
                listObj.Add(icdChoose);
                listObj.Add(serviceNotConfigIds);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDichVuHenKham_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AppointmentService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AppointmentService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(treatmentId);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refeshChooseIcd(object data)
        {
            try
            {
                if (data != null && data is ICDADO)
                {
                    icdChoose = new HIS_ICD();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ICD>(icdChoose, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLadder_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtLadder_Leave(object sender, EventArgs e)
        {
            try
            {
                long ladder = Inventec.Common.TypeConvert.Parse.ToInt64(txtLadder.Text);
                if (ladder > 0)
                {
                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        var demycountMax = this.mediMatyTypeADOs.Max(o => o.RemedyCount);
                        if (ladder == demycountMax) return;
                        CommonParam param = new CommonParam();
                        bool success = false;
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                            {
                                item.RemedyCount = ladder;
                                item.AMOUNT = (item.AmountOneRemedy * item.RemedyCount);

                                SetHuongDanFromSoLuongNgayForOne(item);

                                //Nếu (kê đơn phòng khám hoặc kê tủ trực) và không phải trường hợp hết khả dụng chọn thuốc ngoài kho thay thế thì sẽ gọi hàm take bean
                                if ((!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet))
                                {
                                    if (TakeOrReleaseBeanWorker.TakeForUpdateBean(this.oldExpMestId, item, (item.AMOUNT ?? 0), true, param))
                                    {
                                        item.PrimaryKey = (item.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                                        success = true;
                                    }
                                    else
                                    {
                                        //Release stent
                                        MessageManager.Show(param, false);
                                    }
                                }
                                else
                                {
                                    item.TotalPrice = CalculatePrice(item);
                                    //if (servicePatyAllows != null && servicePatyAllows.ContainsKey(item.SERVICE_ID))
                                    //{
                                    //    var data_ServicePrice = servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                    //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                    //    {
                                    //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                                    //    }
                                    //}

                                    //this.SaveDataAndRefesh();
                                    //ReloadDataAvaiableMediBeanInCombo();
                                }
                            }
                        }

                        if (success)
                        {
                            this.SaveDataAndRefesh();
                            this.ReloadDataAvaiableMediBeanInCombo();
                        }

                        this.gridControlServiceProcess.RefreshDataSource();
                    }
                }
                SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoNgay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && spinSoNgay.EditValue != null)
                {
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        long useTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime) ?? DateTime.MinValue;
                        DateTime dtUseTimeTo = dtUseTime.AddDays((double)this.spinSoNgay.Value - 1);
                        long useTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtUseTimeTo.ToString("yyyyMMddHHmm") + "00");
                        item.UseDays = spinSoNgay.Value;
                        item.UseTimeTo = useTimeTo;

                        SetHuongDanFromSoLuongNgayForOne(item);
                    }

                    gridControlServiceProcess.RefreshDataSource();
                }
                SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_EditValueChanged(object sender, EventArgs e)
        {
            GridLookUpEdit cbo = sender as GridLookUpEdit;
            if (cbo != null && cbo.ContainsFocus)
                this.RebuildMediMatyWithInControlContainer(this.GetDataMediMatyInStock());
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHuongDan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    this.btnAdd.Focus();
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid control
        private void gridControlServiceProcess_DoubleClick(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    isNotChangeTutorial = true;
                    this.actionBosung = GlobalVariables.ActionEdit;
                    isShowContainerMediMatyForChoose = true;
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        //this.ChangeControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID ?? 0);
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                        if (rdOpionGroup.SelectedIndex != 0)
                            rdOpionGroup.SelectedIndex = 0;
                    }
                    else
                    {
                        if (rdOpionGroup.Properties.Items.Count > 1)
                        {
                            if (rdOpionGroup.SelectedIndex != 1)
                                rdOpionGroup.SelectedIndex = 1;
                        }
                        else if (rdOpionGroup.SelectedIndex != 0)
                            rdOpionGroup.SelectedIndex = 0;
                    }
                    this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        this.txtMediMatyForPrescription.Text = "";
                        this.txtUnitOther.Text = this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
                        this.txtMedicineTypeOther.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    }
                    else
                    {
                        this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    }

                    if (this.currentMedicineTypeADOForEdit.RemedyCount > 0)
                    {
                        txtLadder.Text = this.currentMedicineTypeADOForEdit.RemedyCount.ToString();
                    }

                    this.cboMedicineUseForm.EditValue = this.currentMedicineTypeADOForEdit.MEDICINE_USE_FORM_ID;

                    if (this.currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.UseTimeTo.HasValue && InstructionTime > 0)
                    {
                        System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentMedicineTypeADOForEdit.UseTimeTo ?? 0).Value;
                        System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime).Value;
                        TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                        this.currentMedicineTypeADOForEdit.UseDays = diff__Day.Days + 1;
                    }

                    //Tự động hiển thi số lượng là phân số nếu AMOUNT là số thập phân
                    //Vd: AMOUNT = 0.25 --> spinAmount.Text = 1/4
                    //Ngược lại nếu là số nguyên thì hiển thị giữ nguyên giá trị                    
                    this.spinAmount.EditValue = (this.currentMedicineTypeADOForEdit.AmountOneRemedy > 0 ? this.currentMedicineTypeADOForEdit.AmountOneRemedy : this.currentMedicineTypeADOForEdit.AMOUNT);
                    this.btnAdd.Enabled = true;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    this.VisibleButton(this.actionBosung);
                    this.isNotChangeTutorial = false;
                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.TUTORIAL))
                    {
                        this.txtHuongDan.Text = this.currentMedicineTypeADOForEdit.TUTORIAL;
                    }
                    this.memHtu.Text = this.currentMedicineTypeADOForEdit.HTU_TEXT;
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();

                    //string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    //var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                    //List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == currentMedicineTypeADOForEdit.ID && o.LOGINNAME == loginName).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                    DevExpress.XtraGrid.Views.Grid.GridView view = grid.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && (view.FocusedColumn.VisibleIndex == 0)))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        this.btnSave.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (mediMatyTypeADO != null)
                {
                    //Nếu Cột HP không cho phép chỉnh sửa, khi nhấn vào cột sẽ không load lại các dữ liệu của dòng.
                    if (e.Column.FieldName == "IsExpend" && mediMatyTypeADO.IsDisableExpend && gridViewServiceProcess.FocusedColumn == grcExpend__TabMedicine)
                    {
                        return;
                    }
                    mediMatyTypeADO.IsEdit = false;

                    if ((mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && mediMatyTypeADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (mediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0) <= 0 && (mediMatyTypeADO.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
                    {
                        mediMatyTypeADO.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                        mediMatyTypeADO.ErrorTypeMedicineUseForm = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageMedicineUseForm = "";
                        mediMatyTypeADO.ErrorTypeMedicineUseForm = ErrorType.None;
                    }

                    if ((mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && String.IsNullOrEmpty(mediMatyTypeADO.TUTORIAL))
                    {
                        mediMatyTypeADO.ErrorMessageTutorial = ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD;
                        mediMatyTypeADO.ErrorTypeTutorial = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageTutorial = "";
                        mediMatyTypeADO.ErrorTypeTutorial = ErrorType.None;
                    }

                    if (mediMatyTypeADO.PATIENT_TYPE_ID <= 0)
                    {
                        mediMatyTypeADO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        mediMatyTypeADO.ErrorTypePatientTypeId = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessagePatientTypeId = "";
                        mediMatyTypeADO.ErrorTypePatientTypeId = ErrorType.None;
                    }
                    if (mediMatyTypeADO.AMOUNT <= 0 || mediMatyTypeADO.AMOUNT == null)
                    {
                        mediMatyTypeADO.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        mediMatyTypeADO.ErrorTypeAmount = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageAmount = "";
                        mediMatyTypeADO.ErrorTypeAmount = ErrorType.None;
                    }

                    this.ValidRowChange(mediMatyTypeADO);
                    if (e.RowHandle >= 0)
                    {
                        if (e.Column.FieldName == "AMOUNT" || e.Column.FieldName == "PATIENT_TYPE_ID")
                        {
                            if ((mediMatyTypeADO.AmountAlert ?? 0) <= 0
                        && (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                            {
                                List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                                if (mediMatyTypeADO != null && mediMatyTypeADO.IsStent == true
                                    && mediMatyTypeADO.AMOUNT > 1)
                                {
                                    mediMatyTypeADOTemps = MediMatyProcessor.MakeMaterialSingleStent(mediMatyTypeADO);
                                }
                                else
                                {
                                    mediMatyTypeADOTemps.Add(mediMatyTypeADO);
                                }

                                bool success = true;
                                CommonParam param = new CommonParam();
                                foreach (var item in mediMatyTypeADOTemps)
                                {
                                    if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                                    {

                                        if (mediMatyTypeADOTemps.IndexOf(item) != 0)
                                        {
                                            item.MedicineBean1Result = null;
                                            item.MaterialBean1Result = null;
                                            item.ExpMestDetailIds = null;
                                            if (!TakeOrReleaseBeanWorker.TakeForCreateBean(this.oldExpMestId, item, true, param))
                                            {
                                                item.IsNotTakeBean = true;
                                                MessageManager.Show(this, param, false);
                                                success = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!TakeOrReleaseBeanWorker.TakeForUpdateBean(this.oldExpMestId, item, (item.AMOUNT ?? 0), true, param))
                                            {
                                                item.IsNotTakeBean = true;
                                                if (e.Column.FieldName == "AMOUNT")
                                                {
                                                    decimal oldAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(view.ActiveEditor.OldEditValue.ToString());
                                                    SetOldAmountMediMaty(oldAmount, item.ID, item.PrimaryKey);
                                                }
                                                else if (e.Column.FieldName == "PATIENT_TYPE_ID")
                                                {
                                                    long oldPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(view.ActiveEditor.OldEditValue.ToString());
                                                    SetOldPatientTypeMediMaty(oldPatientTypeId, item.ID, item.PrimaryKey); 
                                                }

                                                MessageManager.Show(this, param, false);
                                                success = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        item.TotalPrice = CalculatePrice(item);
                                        //if (servicePatyAllows != null && servicePatyAllows.ContainsKey(item.SERVICE_ID))
                                        //{
                                        //    var data_ServicePrice = servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                        //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                        //    {
                                        //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                                        //    }
                                        //}
                                    }

                                    item.PrimaryKey = (mediMatyTypeADO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                                    item.IsNotTakeBean = false;
                                }

                                //Takebean thành công tất cả thì xóa dòng cũ đi , add 2 bean mới vào
                                //Stent mới có trường hợp mediMatyTypeADOTemps >1
                                if (success && mediMatyTypeADOTemps.Count > 1)
                                {
                                    mediMatyTypeADOs.Remove(mediMatyTypeADO);
                                    mediMatyTypeADOs.AddRange(mediMatyTypeADOTemps);
                                }
                            }
                        }

                        if (e.Column.FieldName == "TUTORIAL")
                        {
                            if (Encoding.UTF8.GetByteCount(mediMatyTypeADO.TUTORIAL) > 1000)
                            {
                                mediMatyTypeADO.ErrorMessageTutorial = "Vượt quá ký tự cho phép";
                                mediMatyTypeADO.ErrorTypeTutorial = ErrorType.Warning;
                            }
                        }
                        if (e.Column.FieldName == "PATIENT_TYPE_ID")
                        {
                            this.UpdateExpMestReasonInDataRow(mediMatyTypeADO);
                        }
                    }


                    gridViewServiceProcess.BeginUpdate();
                    gridViewServiceProcess.GridControl.DataSource = mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                    gridViewServiceProcess.EndUpdate();

                    this.SetTotalPrice__TrongDon();
                    this.ReloadDataAvaiableMediBeanInCombo();
                    mediMatyTypeADO.BK_AMOUNT = mediMatyTypeADO.AMOUNT;
                    if (this.currentMedicineTypeADOForEdit != null && mediMatyTypeADO.ID == this.currentMedicineTypeADOForEdit.ID)
                    {
                        this.currentMedicineTypeADOForEdit = mediMatyTypeADO;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        if (rdOpionGroup.SelectedIndex == 0)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                if (medicineTypeADOForEdit != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    if (rdOpionGroup.SelectedIndex == 0)
                    {
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                    else
                    {
                        MedicineType_RowClick(medicineTypeADOForEdit);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DMediStock1ADO dMediStock = gridViewMediMaty.GetRow(e.RowHandle) as DMediStock1ADO;
                if (dMediStock != null && (dMediStock.IS_STAR_MARK ?? 0) == 1)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }

                MedicineMaterialTypeComboADO medicineMaterialTypeComboADO = gridViewMediMaty.GetRow
(e.RowHandle) as MedicineMaterialTypeComboADO;
                if (medicineMaterialTypeComboADO != null && (medicineMaterialTypeComboADO.IS_STAR_MARK ?? 0) == 1)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu takebean thất bại thì cho số lượng về giá trị cũ
        /// </summary>
        /// <param name="amount">Số lượng cũ</param>
        /// <param name="mediMateId"></param>
        /// <param name="privateKey"></param>
        private void SetOldAmountMediMaty(decimal amount, long mediMateId, string privateKey)
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    foreach (var item in mediMatyTypeADOs)
                    {
                        if (item.ID == mediMateId && privateKey == item.PrimaryKey)
                        {
                            item.AMOUNT = amount;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetOldPatientTypeMediMaty(long patientTypeId, long mediMateId, string privateKey)
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    foreach (var item in mediMatyTypeADOs)
                    {
                        if (item.ID == mediMateId && privateKey == item.PrimaryKey)
                        {
                            HIS_PATIENT_TYPE patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                            if (patientType == null)
                                throw new Exception("Sua DTTT takebean that bai. Khong lay duoc DTTT cu");

                            item.PATIENT_TYPE_ID = patientType.ID;
                            item.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            item.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowAlert(CommonParam param)
        {
            try
            {
                if ((param.Messages != null && param.Messages.Count > 0)
                                                    || (param.BugCodes != null && param.BugCodes.Count > 0))
                {
                    MessageManager.ShowAlert(this, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        if (this.actionType == GlobalVariables.ActionEdit)
                            e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable;
                        else
                        {
                            if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                                e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp;
                            else
                                e.RepositoryItem = this.TextEditPatient_Type_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "EQUIPMENT_SET_ID")
                    {
                        if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            e.RepositoryItem = this.repositoryItemGridLookUpEditEquipmentSet__Enabled;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemGridLookUpEditEquipmentSet__Disabled;
                        }
                    }
                    else if (e.Column.FieldName == "IsExpend")
                    {
                        //#16421 để key cấu hình giá trị 1: Không cho phép check hao phí với thuốc/vật tư không đính kèm
                        if (data.IsDisableExpend || (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && ((HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && ((data.SereServParentId ?? 0) > 0 || GetSereServInKip() > 0)) || !HisConfigCFG.IsNotAllowingExpendWithoutHavingParent))
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage_Disable;
                    }
                    else if (e.Column.FieldName == "IsKHBHYT")
                    {
                        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT))
                            e.RepositoryItem = this.repositoryItemChkIsKH__MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemChkIsKH_Disable__MedicinePage;
                    }
                    else if (e.Column.FieldName == "MEDICINE_USE_FORM_ID")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC))
                            e.RepositoryItem = this.repositoryItemcboMedicineUseForm;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        if (((data.IsAllowOdd.HasValue && data.IsAllowOdd.Value == true) || (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)) && (this.GetRoomTypeId() == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG || GlobalStore.IsCabinet))
                            e.RepositoryItem = this.repositoryItemSpinAmount_Le_MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemSpinAmount__MedicinePage;
                    }
                    else if (e.Column.FieldName == "IsOutKtcFee")
                    {
                        e.RepositoryItem = this.repositoryItemChkOutKtcFee_Enable_TabMedicine;
                    }
                    else if (e.Column.FieldName == "INFORMATION_MEDICINE")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC))
                            e.RepositoryItem = this.repInForMedicineEnable;
                        else
                            e.RepositoryItem = this.repInForMedicineDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMatyTypeADO data_ManuMedicineADO = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ManuMedicineADO != null)
                    {
                        if (e.Column.FieldName == "UseTimeToDisplay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.UseTimeTo ?? 0);
                        }
                        else if (e.Column.FieldName == "TotalPriceDisplay")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data_ManuMedicineADO.TotalPrice, ConfigApplications.NumberSeperator);
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {

                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewServiceProcess.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                if (hi.Column.FieldName == "IsExpend" && (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && (dataRow.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0))//Không cho phép check hao phí với thuốc/vật tư không đính kèm
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.return__FieldName:IsExpend");
                                    return;
                                }
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = view.GetFocusedRow() as MediMatyTypeADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "IsKHBHYT" && view.ActiveEditor is CheckEdit && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    editor.ReadOnly = true;
                    // Kiểm tra các điều kiện: 
                    //1. Đối tượng BN = BHYT
                    //2. Loại hình thanh toán !=BHYT
                    //3. Dịch vụ đó có giá bán = BHYT
                    //4. Dịch vụ đó có giá bán BHYT<giá bán của loại đối tượng TT (xemlai...)
                    if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && data.PATIENT_TYPE_ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID
                        //    && this.servicePatyAllows != null
                        )
                    {
                        //    var isExistsService = this.servicePatyAllows[data.SERVICE_ID].Any(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);
                        //    if (isExistsService)
                        //    {
                        editor.ReadOnly = false;
                    }
                    //}//TODO
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = view.GetFocusedRow() as MediMatyTypeADO;
                if (data == null) return;

                if (view.FocusedColumn.FieldName == "IsExpend" && HisConfigCFG.IsNotAllowingExpendWithoutHavingParent)
                {
                    if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        && (data.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_ShowingEditor__FieldName:IsExpend.Cancel");
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT"
                    || e.ColumnName == "PATIENT_TYPE_ID"
                    || e.ColumnName == "MEDICINE_USE_FORM_ID"
                    || e.ColumnName == "TUTORIAL"
                    || e.ColumnName == "MEDICINE_TYPE_NAME")
                {
                    this.gridViewServiceProcess_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                var row = listDatas[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.ErrorTypeAmount == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmount);
                    }
                    else if ((row.AmountAlert ?? 0) > 0)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "PATIENT_TYPE_ID")
                {
                    if (row.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MEDICINE_USE_FORM_ID")
                {
                    if (row.ErrorTypeMedicineUseForm == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeMedicineUseForm);
                        e.Info.ErrorText = (string)(row.ErrorMessageMedicineUseForm);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "TUTORIAL")
                {
                    if (row.ErrorTypeTutorial == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeTutorial);
                        e.Info.ErrorText = (string)(row.ErrorMessageTutorial);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MEDICINE_TYPE_NAME")
                {
                    if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeIsAssignDay);
                        e.Info.ErrorText = (string)(row.ErrorMessageIsAssignDay);
                    }
                    else if (row.ErrorTypeMediMatyBean == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeMediMatyBean);
                        e.Info.ErrorText = (string)(row.ErrorMessageMediMatyBean);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0) return;

                var listDatas = this.gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                var dataRow = listDatas[index];
                if (dataRow != null)
                {
                    //Thuốc đã hết hoặc không còn trong kho
                    if ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        && this.mediMatyTypeADOs != null
                        && this.mediMatyTypeADOs.Count > 0
                        && (dataRow.AmountAlert ?? 0) > 0)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Red;
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Italic);

                        //var mety = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == dataRow.SERVICE_ID);
                        //if (mety != null && (mety.AMOUNT ?? 0) <= (dataRow.ALERT_MIN_IN_STOCK ?? 0))
                        //{
                        //    //So luong thuoc ton kho nho hon canh bao ton kho cua thuoc thi boi do mau chu dong thuoc
                        //    e.Appearance.ForeColor = System.Drawing.Color.Red;
                        //    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Italic);
                        //}
                    }
                    //Thuoc trong danh muc & vat tu trong danh muc && thuoc tu tuc hien thi mau xanh
                    else if (dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                        || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC
                        )
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Green;
                    }

                    if ((dataRow.IS_STAR_MARK ?? 0) == 1)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentFinishProcessor != null)
                    this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTutorial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        if (rdOpionGroup.SelectedIndex == 0)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlServiceProcess)
                {
                    string text = "";
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlServiceProcess.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
                        {
                            this.lastColumn = info.Column;
                            this.lastRowHandle = info.RowHandle;
                            bool IsAssignDay = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(this.lastRowHandle, "IsAssignDay") ?? "false").ToString());
                            string ErrorMessageIsAssignDay = (view.GetRowCellValue(lastRowHandle, "ErrorMessageIsAssignDay") ?? "").ToString();
                            string ErrorMessageMediMatyBean = (view.GetRowCellValue(lastRowHandle, "ErrorMessageMediMatyBean") ?? "").ToString();
                            int DataType = Inventec.Common.TypeConvert.Parse.ToInt32((view.GetRowCellValue(this.lastRowHandle, "DataType") ?? "").ToString());
                            decimal AmountAlert = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(this.lastRowHandle, "AmountAlert") ?? "").ToString());

                            //Gán toolip cảnh báo thuốc đa kê trong ngày
                            if (!String.IsNullOrEmpty(ErrorMessageIsAssignDay))
                            {
                                if (DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                                {
                                    text = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                                }
                            }


                            //Gán tooltip cảnh báo thuốc đã hết hoặc không còn trong kho
                            if ((DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                                if (AmountAlert > 0)
                                {
                                    text += ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                                }
                                else if (!String.IsNullOrEmpty(ErrorMessageMediMatyBean))
                                {
                                    text = ErrorMessageMediMatyBean;
                                }
                            //else if (DataType == THUOC_DM || DataType == VATTU_DM || DataType == THUOC_TUTUC)
                            //{
                            //    text += ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                            //}
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDelete__MedicinePage_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.gridViewServiceProcess == null || this.gridViewServiceProcess.SelectedRowsCount == 0) return;
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                WaitingManager.Show();
                if (mediMatyTypeADO != null && TakeOrReleaseBeanWorker.ProcessDeleteRowMediMaty(mediMatyTypeADO))
                {
                    bool isReloadAvaible = (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
                    if (this.gridViewServiceProcess.FocusedRowHandle == this.gridViewServiceProcess.DataRowCount - 1)
                    {
                        this.idRow = this.idRow - stepRow;
                        if (this.idRow <= 0) this.idRow = 1;
                    }
                    this.gridViewServiceProcess.BeginUpdate();
                    this.gridViewServiceProcess.DeleteRow(this.gridViewServiceProcess.FocusedRowHandle);
                    this.gridViewServiceProcess.EndUpdate();

                    this.mediMatyTypeADOs.Remove(mediMatyTypeADO);
                    this.ReloadDataAvaiableMediBeanInCombo();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoTreatmentFinish__Checked()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridControlOtherPaySource_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_OTHER_PAY_SOURCE conditionRow = (HIS_OTHER_PAY_SOURCE)gridViewOtherPaySource.GetFocusedRow();
                if (conditionRow != null)
                {
                    popupControlContainerOtherPaySource.HidePopup();
                    var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                    if (currentRowMediMatyTypeADO != null)
                    {
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;

                        this.UpdateExpMestReasonInDataRow(currentRowMediMatyTypeADO);

                        this.gridControlServiceProcess.RefreshDataSource();

                        if (this.gridViewServiceProcess.IsEditing)
                            this.gridViewServiceProcess.CloseEditor();

                        if (this.gridViewServiceProcess.FocusedRowModified)
                            this.gridViewServiceProcess.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewOtherPaySource_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var conditionRow = (HIS_OTHER_PAY_SOURCE)this.gridViewOtherPaySource.GetFocusedRow();
                    if (conditionRow != null)
                    {
                        popupControlContainerOtherPaySource.HidePopup();
                        var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                        if (currentRowMediMatyTypeADO != null)
                        {
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
                            this.gridControlServiceProcess.RefreshDataSource();

                            if (this.gridViewServiceProcess.IsEditing)
                                this.gridViewServiceProcess.CloseEditor();

                            if (this.gridViewServiceProcess.FocusedRowModified)
                                this.gridViewServiceProcess.UpdateCurrentRow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonOthePaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                if (currentRowMediMatyTypeADO != null && currentRowMediMatyTypeADO.PATIENT_TYPE_ID > 0)
                {
                    if (e.Button.Kind == ButtonPredefines.Down || e.Button.Kind == ButtonPredefines.DropDown)
                    {
                        ButtonEdit editor = sender as ButtonEdit;
                        Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                        popupControlContainerOtherPaySource.ShowPopup(new Point(buttonPosition.X + 150, buttonPosition.Bottom + 300));

                        var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                        List<HIS_OTHER_PAY_SOURCE> dataConditionTmps = new List<HIS_OTHER_PAY_SOURCE>();
                        dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                        if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                        {
                            var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowMediMatyTypeADO.PATIENT_TYPE_ID).FirstOrDefault();

                            if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                            {
                                dataConditionTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                                if (currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID == null && dataConditionTmps != null && dataConditionTmps.Count == 1)
                                {
                                    currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = dataConditionTmps[0].ID;
                                    currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_CODE = dataConditionTmps[0].OTHER_PAY_SOURCE_CODE;
                                    currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = dataConditionTmps[0].OTHER_PAY_SOURCE_NAME;
                                }
                            }
                            else
                            {
                                dataConditionTmps.AddRange(dataOtherPaySources);
                            }

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataConditionTmps), dataConditionTmps));
                        }

                        gridControlOtherPaySource.DataSource = null;
                        gridControlOtherPaySource.DataSource = dataConditionTmps;
                        gridControlOtherPaySource.Focus();

                        int focusRow = 0;
                        if (currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID > 0 && dataConditionTmps != null && dataConditionTmps.Count > 0)
                        {

                            for (int i = 0; i < dataConditionTmps.Count; i++)
                            {
                                if (dataConditionTmps[i].ID == currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID)
                                {
                                    focusRow = i;
                                }
                            }
                        }
                        gridViewOtherPaySource.FocusedRowHandle = focusRow;
                    }
                    else if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = null;
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_CODE = "";
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = "";

                        this.UpdateExpMestReasonInDataRow(currentRowMediMatyTypeADO);

                        this.gridControlServiceProcess.RefreshDataSource();

                        if (this.gridViewServiceProcess.IsEditing)
                            this.gridViewServiceProcess.CloseEditor();

                        if (this.gridViewServiceProcess.FocusedRowModified)
                            this.gridViewServiceProcess.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboNhaThuoc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit editor = sender as GridLookUpEdit;
                editor.Properties.Buttons[1].Visible = (editor.EditValue != null);

                //if (HisConfigCFG.IsAutoCreateSaleExpMest)
                //{
                if (editor.EditValue == null)
                {
                    this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                    RebuildMedicineTypeWithInControlContainer();
                }
                else
                {
                    var nt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == (long)editor.EditValue).FirstOrDefault();
                    if (nt != null)
                    {
                        this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                        this.currentMediStockNhaThuocSelecteds.Add(nt);
                    }
                    RebuildNhaThuocMediMatyWithInControlContainer();
                }
                // }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.ResetComboNhaThuoc();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAssignPrescription_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkYhct.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkYhct.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkYhct.Name;
                    csAddOrUpdate.VALUE = (chkYhct.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.AssignPrescriptionYHCT";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkYhct_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.cboMediStockExport.Enabled)
                {
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                    }
                }
                this.InitComboMediStockAllow(0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExpMestReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    cboExpMestReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCustomGridLookUpReasion_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as GridLookUpEdit;

                    if (cbo != null && cbo.EditValue != null)
                    {
                        var row = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                        long ExpMestReasonId = (long)cbo.EditValue;

                        var data = this.mediMatyTypeADOs.FirstOrDefault(o => o.ID == row.ID);

                        var ExpMestReason = this.lstExpMestReasons.FirstOrDefault(o => o.ID == ExpMestReasonId);

                        if (ExpMestReason != null)
                        {
                            row.EXP_MEST_REASON_ID = ExpMestReason.ID;
                            row.EXP_MEST_REASON_CODE = ExpMestReason.EXP_MEST_REASON_CODE;
                            row.EXP_MEST_REASON_NAME = ExpMestReason.EXP_MEST_REASON_CODE;
                        }
                        else
                        {
                            row.EXP_MEST_REASON_ID = null;
                            row.EXP_MEST_REASON_CODE = "";
                            row.EXP_MEST_REASON_NAME = "";
                        }

                        this.gridControlServiceProcess.RefreshDataSource();

                        if (this.gridViewServiceProcess.IsEditing)
                            this.gridViewServiceProcess.CloseEditor();

                        if (this.gridViewServiceProcess.FocusedRowModified)
                            this.gridViewServiceProcess.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemChkIsExpend__MedicinePage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                if(currentRowSereServADO.IsDisableExpend)
                {
                    chk.Checked = true;
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

        private void repositoryItemCustomGridLookUpReasion_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                if (currentRowSereServADO != null)
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        currentRowSereServADO.EXP_MEST_REASON_ID = null;
                        currentRowSereServADO.EXP_MEST_REASON_CODE = "";
                        currentRowSereServADO.EXP_MEST_REASON_NAME = "";

                        this.gridControlServiceProcess.RefreshDataSource();

                        if (this.gridViewServiceProcess.IsEditing)
                            this.gridViewServiceProcess.CloseEditor();

                        if (this.gridViewServiceProcess.FocusedRowModified)
                            this.gridViewServiceProcess.UpdateCurrentRow();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
        #region chidinh den
        private void txtTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void cboTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTimeTo.EditValue != null)
                {
                    // Chuyển đổi giá trị từ ComboBox thành DateTime
                    DateTime selectedDate;
                    bool isValidDate = DateTime.TryParse(cboTimeTo.EditValue.ToString(), out selectedDate);

                    if (isValidDate)
                    {
                        // Định dạng ngày/thời gian thành dd/MM/yyyy hh:mm và gán vào TextEdit
                        txtTimeTo.Text = selectedDate.ToString("dd/MM/yyyy");
                    }
                    
                    this.TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(selectedDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTimeTo_Validated(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTimeTo.Text))
                {
                    string input = txtTimeTo.Text.Trim();
                    DateTime enteredDate;
                    if (input.Length == 8 && long.TryParse(input, out _))
                    {
                        string formattedDate = input.Insert(2, "/").Insert(5, "/");

                        bool isValidDate = DateTime.TryParseExact(formattedDate, "dd/MM/yyyy",
                                                                  System.Globalization.CultureInfo.InvariantCulture,
                                                                  System.Globalization.DateTimeStyles.None,
                                                                  out enteredDate);

                        if (isValidDate)
                        {
                            txtTimeTo.Text = enteredDate.ToString("dd/MM/yyyy");
                        }
                        
                    }
                    else
                    {
                        // Nếu nhập đúng định dạng, chỉ cần chuyển đổi
                        bool isValidDate = DateTime.TryParse(input, out enteredDate);

                        if (isValidDate)
                        {
                            txtTimeTo.Text = enteredDate.ToString("dd/MM/yyyy");
                        }
                        
                    }
                    this.TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(enteredDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

        private void txtTimeTo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboTimeTo.ShowPopup();
                }
                if(e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtTimeTo.Text = "";
                    cboTimeTo.EditValue = null;
                    this.TIME_TO = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region dutru
        private List<DateTime> selectedDates = new List<DateTime>(); 

        private void txtDuTruTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                selectedDates.Clear();
                if (IsManyDay)
                {
                    Point txtDuTruTimeLocationOnScreen = txtDuTruTime.PointToScreen(new Point(0, txtDuTruTime.Height));
                    popupControlContainer1.ShowPopup(txtDuTruTimeLocationOnScreen);
                    foreach (var date in selectedDates)
                    {
                        calendarControlDutru.DateTime = date;
                    }
                }
                else
                {
                    cboDuTruTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDuTruTime_Closed(object sender, ClosedEventArgs e)
        {
            
        }

        private void btnSave_duTru_Click(object sender, EventArgs e)
        {
            try
            {
                PopupContainerBarControl control = popupControlContainer1.Parent as PopupContainerBarControl;
                control.ClosePopup();
                setDefaultDutruTime();
                if (calendarControlDutru.SelectedRanges.Count == 0)
                {
                    
                    return;
                }
                foreach (DateRange item in calendarControlDutru.SelectedRanges)
                {
                    if (item != null)
                    {
                        var dt = item.StartDate;
                        while (dt.Date < item.EndDate.Date)
                        {

                            if(!selectedDates.Contains(dt)) selectedDates.Add(dt);
                            dt = dt.AddDays(1);
                        }
                    }

                }
                UpdateSelectedDatesText();
                
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void setDefaultDutruTime()
        {
            try
            {
                txtDuTruTime.Text = "";
                if(selectedDates != null)selectedDates.Clear();
                if (this.USE_TIME != null) this.USE_TIME.Clear();
                if (ucDate != null && ucDateProcessor != null)
                {
                    HIS.UC.DateEditor.ADO.DateInputADO ado = new UC.DateEditor.ADO.DateInputADO();
                    ado.IsDutruTime = false;
                    ucDateProcessor.Reload(ucDate, ado);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void calendarControlDutru_DateTimeChanged(object sender, EventArgs e)
        {
            try
            {
                
                
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDuTruTime_Validated(object sender, EventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(txtDuTruTime.Text))
                {
                    if (this.USE_TIME != null) this.USE_TIME.Clear();
                    if (this.selectedDates != null) this.selectedDates.Clear();
                    if (IsManyDay)
                    {
                        var date = txtDuTruTime.Text.Trim().Split(';');
                        foreach (var _date in date)
                        {
                            DateTime selectedDate;
                            bool isValidDate = DateTime.TryParse(_date.ToString(), out selectedDate);
                            if (isValidDate && !selectedDates.Contains(selectedDate)) selectedDates.Add(selectedDate);
                        }
                    }
                    else
                    {
                        if(cboDuTruTime.DateTime != null && cboDuTruTime.DateTime != DateTime.MaxValue && cboDuTruTime.DateTime != DateTime.MinValue)
                        {
                            DateTime selectedDate = cboDuTruTime.DateTime;
                            if (!selectedDates.Contains(selectedDate)) selectedDates.Add(selectedDate);
                            txtDuTruTime.Text = selectedDate.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            txtDuTruTime.Text = "";
                        }
                            
                    }

                    if (!string.IsNullOrEmpty(txtDuTruTime.Text)) UpdateSelectedDatesText();
                }
                else
                {
                    setDefaultDutruTime();


                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDuTruTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtDuTruTime.Text))
                //{
                //    HIS.UC.DateEditor.ADO.DateInputADO ado = new UC.DateEditor.ADO.DateInputADO();
                //    ado.IsDutruTime = false;
                //    ucDateProcessor.Reload(ucDate, ado);
                //}
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDuTruTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDuTruTime.DateTime != null && cboDuTruTime.DateTime != DateTime.MaxValue && cboDuTruTime.DateTime != DateTime.MinValue)
                {
                    if (this.USE_TIME != null) this.USE_TIME.Clear();
                    DateTime selectedDate = cboDuTruTime.DateTime;
                    if (!selectedDates.Contains(selectedDate))
                    {

                        selectedDates.Add(selectedDate);

                    }
                    txtDuTruTime.Text = selectedDate.ToString("dd/MM/yyyy");
                    

                    UpdateSelectedDatesText();

                }
                else
                {
                    setDefaultDutruTime();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repInForMedicineEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                if (currentRowSereServADO != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisProductInfo").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisProductInfo");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(new ProductInfoADO() { MedicineTypeId  = currentRowSereServADO.ID , ProductInfoOpen = 0});//TODO
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        // Hàm cập nhật TextEdit với chuỗi ngày đã chọn
        private void UpdateSelectedDatesText()
        {
            try
            {
                
                selectedDates = selectedDates.Distinct().ToList();
                if (IsManyDay)
                {
                    txtDuTruTime.Text = string.Join(";", selectedDates.Select(d => d.ToString("dd/MM")).ToArray());
                }
                else
                {
                    txtDuTruTime.Text = selectedDates.FirstOrDefault().ToString("dd/MM/yyyy");
                }
                if (this.USE_TIME == null) this.USE_TIME = new List<long>();
                this.USE_TIME.Clear();
                selectedDates.ForEach(date =>
                {
                    try
                    {
                        var date_number = Convert.ToInt64(date.ToString("yyyyMMdd") + "000000");
                        if (date_number > 0)
                        {
                            if(!this.USE_TIME.Contains(date_number))
                                this.USE_TIME.Add(date_number);
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("Loi khi convert date");
                    }
                });
                if (!string.IsNullOrEmpty(txtDuTruTime.Text))
                {
                    if (ucDate != null && ucDateProcessor != null)
                    {
                        HIS.UC.DateEditor.ADO.DateInputADO ado = new UC.DateEditor.ADO.DateInputADO();
                        ado.IsDutruTime = true;
                        ucDateProcessor.Reload(ucDate, ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

    }
}
