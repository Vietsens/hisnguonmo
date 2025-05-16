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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Base;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ChooseICD;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Worker;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        long? serviceReqParentId;
        long treatmentId = 0;
        long? expMestTemplateId;
        string treatmentCode;
        int actionBosung = 0;
        int positionHandle = 0;
        int positionHandle__DuongDung = 0;
        internal int positionHandleControl = -1;
        internal int actionType = 0;
        internal int actionTypePrint = 0;
        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        DateTime timeSelested;
        internal int idRow = 1;
        internal long InstructionTime { get; set; }
        internal bool limitHeinMedicinePrice = false;
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

        internal bool isAutoCheckExpend = false;
        internal const int stepRow = 1;
        internal decimal totalPriceBHYT = 0;
        decimal totalHeinByTreatment = 0;
        decimal totalHeinPriceByTreatment = 0;//VIR_TOTAL_HEIN_PRICE
        internal HisTreatmentWithPatientTypeInfoSDO currentTreatmentWithPatientType { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        HIS_MEDICINE_TYPE_TUT medicineTypeTutSelected;
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatmentRaw = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        internal List<MediMatyTypeADO> mediMatyTypeADOBKs;
        internal List<MediMatyTypeADO> mediMatyTypeADOs;
        internal List<D_HIS_MEDI_STOCK_2> mediMatyTypeAvailables;
        internal MediMatyTypeADO currentMedicineTypeADOForEdit;
        internal List<DMediStock1ADO> mediStockD1ADOs;
        internal List<V_HIS_MEDICINE_TYPE> currentMedicineTypes;
        List<V_HIS_MATERIAL_TYPE> currentMaterialTypes;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ACIN> currentMedicineTypeAcins;
        internal HIS_ICD icdChoose { get; set; }
        public Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInMaMeConfigs;
        List<V_HIS_SERVICE_PACKAGE> servicePackageByServices;
        SubclinicalPresResultSDO outPrescriptionResultSDOs;
        InPatientPresResultSDO inPrescriptionResultSDOs;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> currentPrescriptions;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock = null;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByHeaderCard;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByNotInHeaderCard;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentWorkingMestRooms;
        AssignPrescriptionEditADO assignPrescriptionEditADO;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;
        //List<HIS_MEDICINE_TYPE_TUT> listMedicineTypeTut = new List<HIS_MEDICINE_TYPE_TUT>();

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

        int theRequiredWidth = 900, theRequiredHeight = 130;
        bool isShowContainerMediMaty = false;
        bool isShowContainerTutorial = false;
        bool isShowContainerHtu = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        bool isStopEventChangeMultiDate;
        bool IsObligatoryTranferMediOrg = false;
        bool IsAcceptWordNotInData = false;
        string[] icdSeparators = new string[] { ",", ";" };
        bool isAutoCheckIcd;
        string _TextIcdName = "";
        string _TextIcdNameCause = "";

        List<HIS_ICD> currentIcds;
        List<TrackingADO> trackingADOs { get; set; }
        List<HIS_ALLERGENIC> allergenics { get; set; }

        internal bool isMediMatyIsOutStock = false;

        //Bien luu thong tin don thuoc cu
        string provisionalDiagnosis;
        List<HIS_SERVICE_REQ> serviceReqPrints { get; set; }
        List<HIS_EXP_MEST> expMestPrints { get; set; }
        List<HIS_EXP_MEST_MEDICINE> expMestMedicinePrints { get; set; }
        List<HIS_EXP_MEST_MATERIAL> expMestMaterialPrints { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineEditPrints { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialEditPrints { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> serviceReqMetys { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> serviceReqMatys { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        HIS_DHST currentDhst;
        bool isNotLoadMediMatyByMediStockInitForm;
        bool IsHandlerWhileOpionGroupSelectedIndexChanged;
        bool isNotLoadWhileChangeInstructionTimeInFirst;
        bool isInitUcDate;
        bool isNotProcessRunWhileFilmChangedValue;
        decimal currentSoPhimHong;
        HIS_TREATMENT Histreatment;
        HIS_MEDICINE_TYPE_TUT hisMedicineTypeTut { get; set; }

        #endregion

        #region Construct
        public frmAssignPrescription(Inventec.Desktop.Common.Modules.Module module, AssignPrescriptionADO data)
            : base(module)
        {
            try
            {
                InitializeComponent();
                LogSystem.Debug("frmAssignPrescription InitializeComponent.1");
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                this.actionType = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.actionTypePrint = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.currentModule = module;
                this.processDataResult = data.DgProcessDataResult;
                this.processRefeshIcd = data.DgProcessRefeshIcd;
                this.processWhileAutoTreatmentEnd = data.DlgWhileAutoTreatmentEnd;
                this.treatmentId = data.TreatmentId;
                this.expMestTemplateId = data.ExpMestTemplateId;
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
                this.currentSereServ = data.SereServ;
                this.isAutoCheckExpend = data.IsAutoCheckExpend;
                this.assignPrescriptionEditADO = data.AssignPrescriptionEditADO;
                this.icdExam = data.IcdExam;
                this.currentDhst = data.Dhst;
                this.sereServsInTreatmentRaw = data.SereServsInTreatment;
                this.provisionalDiagnosis = data.ProvisionalDiagnosis;
                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription.frmAssignPrescription).Assembly);
                this.InitAssignPresctiptionType();
                this.InitDataForPrescriptionEdit();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.TreatmentId), data.TreatmentId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule.RoomId), currentModule.RoomId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule.RoomTypeId), currentModule.RoomTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.InPatientPrescription__ShowRoundAvailableAmount), HisConfigCFG.InPatientPrescription__ShowRoundAvailableAmount)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsUsingWarningHeinFee), HisConfigCFG.IsUsingWarningHeinFee)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarningOverTotalPatientPrice), HisConfigCFG.IsWarningOverTotalPatientPrice)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarringUseDayAndExpTimeBHYT), HisConfigCFG.IsWarringUseDayAndExpTimeBHYT)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsDontPresExpiredTime), HisConfigCFG.IsDontPresExpiredTime)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.isPrescriptionSplitOutMediStock), HisConfigCFG.isPrescriptionSplitOutMediStock)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsNotAllowingExpendWithoutHavingParent), HisConfigCFG.IsNotAllowingExpendWithoutHavingParent)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsBlockWhileAcinByMedicineType), HisConfigCFG.IsBlockWhileAcinByMedicineType)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.ManyDayPrescriptionOption), HisConfigCFG.ManyDayPrescriptionOption)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarningOddConvertAmount), HisConfigCFG.IsWarningOddConvertAmount)
                    );

                InitMultipleThread();
                this.SetCaptionByLanguageKey();
                LogSystem.Debug("frmAssignPrescription InitializeComponent.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
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

                //if (GlobalStore.IsCabinet)
                //{
                //    if (GlobalStore.IsTreatmentIn)
                //    {
                //        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //    }
                //    else
                //    {
                //        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //    }
                //}
                //else if (GlobalStore.IsTreatmentIn)
                //    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //else
                //    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

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
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnNew.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboExpMestTemplate.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboExpMestTemplate.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboMediStockExport.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboMediStockExport.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcIsExpendType.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcLoaiHaoPhi__MedicinePage.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcTocDoTruyen.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTocDoTruyen__MedicinePage.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcNumOrder.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcManuMedicineName__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcManuMedicineName__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcUnit__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUnit__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcAmount__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcAmount__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcPatientType__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcPatientType__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcExpend__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcExpend__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcIsOutKtcFee__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsOutKtcFee__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grcTutorial__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTutorial__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

                this.lciHuongDan.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHuongDan.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciMedicineUseForm.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMedicineUseForm.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciAmount.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTongTien.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTongTien.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciMediStockExpMest.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMediStockExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciExpMestTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_ICD_MAIN", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkEditIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Load
        private void frmAssignPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("frmAssignPrescription_Load Starting.... 1");
                WaitingManager.Show();
                this.LoadHisTreatment();
                this.AddBarManager(this.barManager1);
                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.ResetDataForm();
                this.SetDefaultData();
                this.SetDefaultUC();
                LogSystem.Debug("frmAssignPrescription_Load 1...");
                this.LoadDataToPatientInfo();
                LogSystem.Debug("frmAssignPrescription_Load 1.1");
                this.isNotLoadMediMatyByMediStockInitForm = true;
                this.ReSetDataInputAfterAdd__MedicinePage();
                LogSystem.Debug("frmAssignPrescription_Load. 2");

                this.InitComboPhieuDieuTri();
#if DEBUG
                {
                    Inventec.Common.Logging.LogSystem.Debug("frmAssignPrescription_Load .DEBUG true");
                    this.InitTabIndex();
                    this.ValidateForm();
                    this.ValidateBosung();
                    this.VisibleColumnInGridControlService();
                }
#else
                {
                    Inventec.Common.Logging.LogSystem.Debug("frmAssignPrescription_Load. 3 .DEBUG false");
                    List<Action> methods = new List<Action>();
                    methods.Add(this.InitTabIndex);
                    methods.Add(this.ValidateForm);
                    methods.Add(this.ValidateBosung);
                    methods.Add(this.VisibleColumnInGridControlService);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                }
#endif

                LogSystem.Debug("frmAssignPrescription_Load. 4");
                this.FillDataToControlsForm();

                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.OpionGroupSelectedChanged();
                    if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    {
                        this.cboMediStockExport.Focus();
                        this.cboMediStockExport.SelectAll();
                    }
                    else
                    {
                        this.InitMediMatyWithHasMediStockDefault();

                        this.txtMediMatyForPrescription.Focus();
                        this.txtMediMatyForPrescription.SelectAll();
                    }
                }
                else
                {
                    this.txtMediMatyForPrescription.Focus();
                    this.txtMediMatyForPrescription.SelectAll();
                }
                LogSystem.Debug("frmAssignPrescription_Load. 5");
                this.VisibleButton(this.actionBosung);
                LogSystem.Debug("frmAssignPrescription_Load. 6");
                this.LoadPrescriptionForEdit();                
                this.SetEnableButtonControl(this.actionType);
                this.isNotLoadMediMatyByMediStockInitForm = false;
                this.IsHandlerWhileOpionGroupSelectedIndexChanged = false;
                this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                LogSystem.Debug("frmAssignPrescription_Load. 7");

                WaitingManager.Hide();
                this.timerInitForm.Interval = 4000;//Fix 4s
                this.timerInitForm.Enabled = true;
                this.timerInitForm.Start();

                LogSystem.Debug("frmAssignPrescription_Load. 8");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChooseDefaultPatientTypeFromKey()
		{
			try
			{
                if(HisConfigCFG.IsDefaultPatientTypeOption && serviceReqParentId > 0 && currentSereServ != null)
				{                   
                    cboPatientType.EditValue = currentSereServ.PATIENT_TYPE_ID;                       
                }                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("timerInitForm_Tick 1...");
                this.timerInitForm.Stop();
                this.InitComboExpMestTemplate();
                this.InitMenuToButtonPrint();//TODO
                this.CheckAppoinmentEarly();//Hien thi thong bao den som thoi gian hen kham
                this.LoadAllergenic(this.currentTreatmentWithPatientType.PATIENT_ID);
                this.ThreadLoadDonThuocCu();
                //this.InitComboEquipment();
                //this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                this.InitMedicineTypeAcinInfo();
                LogSystem.Debug("timerInitForm_Tick. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        myResult = MessageBox.Show(ResourceMessage.CanhBaoThuocChuaLuuTatForm, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != DialogResult.Yes)
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Button
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

        private void btnBoSungPhacDo_Click(object sender, EventArgs e)
        {
            try
            {
                //Lay danh sach icd
                string icdCode = "";
                var icdValue = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    icdCode = icdValue.ICD_CODE;
                }

                var subIcd = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    icdCode += subIcd.ICD_SUB_CODE;
                }

                if (String.IsNullOrEmpty(icdCode))
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayThongTinICD, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (this.mediMatyTypeADOs == null || this.mediMatyTypeADOs.Count == 0)
                {
                    MessageBox.Show(ResourceMessage.VuiLongKeThuocVatTu, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                string[] icdCodeArr = icdCode.Split(';');
                var mediMatyTypeAllows = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).ToList();


                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.ToList();
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);

                List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID > 0).Select(o => o.SERVICE_ID.Value).ToList();
                List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID > 0).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();
                List<long> serviceNotConfigIds = mediMatyTypeAllows.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID)).Select(o => o.SERVICE_ID).ToList();
                List<HIS_ACTIVE_INGREDIENT> activeIngredientNotConfigs = null;
                List<long> metyIds = mediMatyTypeAllows.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)).Select(t => t.ID).ToList();

                var medicineTypeAcinF1s = currentMedicineTypeAcins.Where(o => metyIds.Contains(o.MEDICINE_TYPE_ID)).ToList();
                if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                {
                    var acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                    var acgrNotConfigIds = (acingrIdTmps != null && acingrIdTmps.Count > 0 ? (acingrIdTmps.Where(o => !acinIgrIds.Contains(o))) : null).ToList();
                    if (acgrNotConfigIds != null && acgrNotConfigIds.Count > 0)
                    {
                        activeIngredientNotConfigs = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>().Where(o => acgrNotConfigIds.Contains(o.ID)).ToList();
                    }
                }

                if (serviceNotConfigIds == null || serviceNotConfigIds.Count == 0)
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayThuocVatTuDuocCauHinhChanDoanICD, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                List<HIS_ICD> icds = this.currentIcds.Where(o => icdCodeArr.Contains(o.ICD_CODE)).Distinct().ToList();
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
                listObj.Add(activeIngredientNotConfigs);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.medicineTypeTutSelected == null)
                    this.medicineTypeTutSelected = new HIS_MEDICINE_TYPE_TUT();

                if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    this.medicineTypeTutSelected.MEDICINE_TYPE_ID = (this.currentMedicineTypeADOForEdit.ID);
                }
                else
                {
                    LogSystem.Warn("Huong dan su dung thuoc chi luu lai khi lam viec o tab thuoc - vat tu va tab thuoc ngoai kho");
                    return;
                }

                WaitingManager.Show();

                if (cboMedicineUseForm.EditValue != null)
                    this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? 0).ToString());
                else
                    this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID = null;

                this.medicineTypeTutSelected.TUTORIAL = txtTutorial.Text;
                this.medicineTypeTutSelected.HTU_TEXT = txtHtu.Text;
                this.medicineTypeTutSelected.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                CommonParam param = new CommonParam();
                bool success = true;
                //this.medicineTypeTutSelected = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE_TUT>((this.medicineTypeTutSelected.ID > 0 ? RequestUriStore.HIS_MEDICINE_TYPE_TUT_UPDATE : RequestUriStore.HIS_MEDICINE_TYPE_TUT_CREATE), ApiConsumers.MosConsumer, this.medicineTypeTutSelected, ProcessLostToken, param);
                this.medicineTypeTutSelected = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE_TUT>(RequestUriStore.HIS_MEDICINE_TYPE_TUT_CREATE, ApiConsumers.MosConsumer, this.medicineTypeTutSelected, ProcessLostToken, param);
                if (this.medicineTypeTutSelected == null || this.medicineTypeTutSelected.ID == 0)
                {
                    success = false;
                }
                MessageManager.ShowAlert(this, param, success);
                this.RefeshDataMedicineTutorial(this.medicineTypeTutSelected);

                var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID && o.LOGINNAME == loginName).ToList();
                this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
                this.RebuildHtulWithInControlContainer(medicineTypeTutFilters);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.actionType == GlobalVariables.ActionAdd)
                    this.ReleaseAllMediByUser();
                this.SetDefaultData();
                this.SetDefaultUC();
                this.ReSetDataInputAfterAdd__MedicinePage();
                this.LoadSereServTotalHeinPriceWithTreatment(this.treatmentId);
                this.InitComboMediStockAllow(0);
                InitComboExpMestReason();
                this.cboMediStockExport.ShowPopup();
                this.cboMediStockExport.ClosePopup();
                this.ResetFocusMediMaty(true);
                this.OpionGroupSelectedChangedAsync();
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
                this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE.SAVE);
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
                this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE.SAVE_PRINT_NOW);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndShowPrintPreview_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessSaveData(HIS.Desktop.Plugins.AssignPrescriptionCLS.SAVETYPE.SAVE_SHOW_PRINT_PREVIEW);
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
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    LogSystem.Warn("btnAdd_TabMedicine_Click => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }                
                bool valid = true;
                this.positionHandleControl = -1;
                valid = valid && dxValidProviderBoXung.Validate();
                valid = valid && CheckAllergenicByPatient();
                valid = valid && CheckActiveIngradientForMedicine();
                valid = valid && CheckMaxInPrescription(currentMedicineTypeADOForEdit, spinAmount.Value);
                valid = valid && CheckGenderMediMaty(currentMedicineTypeADOForEdit);
                valid = valid && CheckMaMePackage(currentMedicineTypeADOForEdit);
                valid = valid && CheckOddConvertUnit(currentMedicineTypeADOForEdit, spinAmount.Value);
                if (!valid) return;               
                if (this.mediMatyTypeADOs == null)
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                switch (this.actionBosung)
                {
                    case GlobalVariables.ActionAdd:
                        AddMediMatyClickHandler();
                        break;
                    case GlobalVariables.ActionEdit:
                        UpdateMediMatyClickHandler();
                        break;
                    default:
                        LogSystem.Warn("btnAdd_TabMedicine_Click => thao tac khong hop le. actionBosung = " + this.actionBosung);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                            myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanDiUngVoiThuocXBanCOmuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
                            {
                                result = false;
                            }
                        }
                        else if (allergencic.IS_DOUBT == 1)
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanNghiNgoDiUngVoiThuocXBanCoMuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
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

        private bool CheckActiveIngradientForMedicine()
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
                            myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanDiUngVoiThuocXBanCOmuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
                            {
                                result = false;
                            }
                        }
                        else if (allergencic.IS_DOUBT == 1)
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanNghiNgoDiUngVoiThuocXBanCoMuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
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
                //if (this.patientSelectProcessor != null && this.ucPatientSelect != null)
                //{
                //    this.patientSelectProcessor.FocusSearchTextbox(this.ucPatientSelect);
                //}
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
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSave.Enabled && this.btnSave.Visible)
                {
                    this.btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSvePrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndShowPrintPreview.Focus();
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSaveAndShowPrintPreview.Enabled && this.btnSaveAndShowPrintPreview.Visible)
                {
                    this.btnSaveAndShowPrintPreview_Click(null, null);
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
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSaveAndPrint.Enabled && this.btnSaveAndPrint.Visible)
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
                if (this.actionType == GlobalVariables.ActionView && this.lciPrintAssignPrescription.Enabled && this.lciPrintAssignPrescription.Visible)
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
                if (this.btnNew.Enabled && this.btnNew.Visible)
                    this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        //qtcode
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == this.ModuleLink).FirstOrDefault();
                if (!string.IsNullOrEmpty(HisConfigCFG.AllowSignaturePrintModuleLinks) && ("," + HisConfigCFG.AllowSignaturePrintModuleLinks + ",").Contains("," + this.ModuleLink + ",") && moduleData != null)
                {
                    PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, true, PrintConfig.PreviewType.EmrSignAndPrintNow);
                }
                else
                {
                    PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, true, null);
                }

                //PrescriptionPrintShow(PrintTypeCodes.PRINT_TYPE_CODE__BIEUMAU__PHIEU_KE_KHAI_THUOC_VATU__MPS000338, false, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //qtcode
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

        private void chkPhimHong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                spinSoPhimHong.Enabled = (chkPhimHong.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerTutorial = !isShowContainerTutorial;
                    if (isShowContainerTutorial)
                    {
                        Rectangle buttonBounds = new Rectangle(txtTutorial.Bounds.X, txtTutorial.Bounds.Y, txtTutorial.Bounds.Width, txtTutorial.Bounds.Height);
                        popupControlContainerTutorial.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboMediStockExport_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport_EditValueChanged.1");
                if (this.isNotLoadMediMatyByMediStockInitForm)
                {
                    this.isNotLoadMediMatyByMediStockInitForm = false;
                    return;
                }
                if (IsHandlerWhileOpionGroupSelectedIndexChanged)
                {
                    return;
                }

                if (cboMediStockExport.EditValue != null && isMediMatyIsOutStock)
                {
                    List<V_HIS_MEST_ROOM> lst = cboMediStockExport.Properties.DataSource as List<V_HIS_MEST_ROOM>;
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    if (lst != null && lst.Count > 0)
                    {
                        this.currentMediStock = lst.Where(o => o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockExport.EditValue.ToString())).ToList();
                    }
                }

                if (!cboMediStockExport.IsPopupOpen)
                    LoadDataToGridMetyMatyTypeInStock();
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport_EditValueChanged.2");
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

        private void cboUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && cbo.ContainsFocus)
                    this.OpionGroupSelectedChanged();
                SetDefaultComboPhieuDieuTri();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_EditValueChanged(object sender, EventArgs e)
        {

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

        public void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                //this.InitComboTracking(cboPhieuDieuTri);
                this.intructionTimeSelecteds = this.UcDateGetValue();
                this.isMultiDateState = this.UcDateGetChkMultiDateState();

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
                Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 1");
                if (this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotLoadWhileChangeInstructionTimeInFirst), isNotLoadWhileChangeInstructionTimeInFirst));
                    return;
                }

                this.isMultiDateState = this.UcDateGetChkMultiDateState();
                if (!this.isMultiDateState)
                {
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.Add(intructTime);
                    this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                }

                this.InstructionTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                this.SetDefaultComboPhieuDieuTri();
                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.InstructionTime);
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, this.InstructionTime);
                this.LoadTotalSereServByHeinWithTreatment();
                this.PatientTypeWithTreatmentView7();
                this.InitComboRepositoryPatientType(this.repositoryItemcboPatientType_TabMedicine_GridLookUp, this.currentPatientTypeWithPatientTypeAlter);
                this.FillTreatmentInfo__PatientType();

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
                        if (!item.IsMultiDateState && item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0)
                        {
                            item.IntructionTimeSelecteds = this.intructionTimeSelecteds;
                        }
                    }
                    this.RefeshResourceGridMedicine();
                }
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    this.OpionGroupSelectedChangedAsync();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
                Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 2");
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
                    this.mediMatyTypeAvailables = new List<D_HIS_MEDI_STOCK_2>();
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
                    this.txtMediMatyForPrescription.Focus();
                    this.txtMediMatyForPrescription.SelectAll();
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
                    this.txtMediMatyForPrescription.Focus();
                    this.txtMediMatyForPrescription.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChangeMediStock()
        {
            try
            {
                if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    this.idRow = 1;
            }
            catch (Exception ex)
            {
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
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport__SelectionChange.1");
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = this.cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
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
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sb), sb.ToString()));
                this.cboMediStockExport.Text = sb.ToString();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport__SelectionChange.2");
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
                        this.ResetFocusMediMaty(true);
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
                            this.ResetFocusMediMaty(true);
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
                            this.ResetFocusMediMaty(true);
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
                            this.SetHuongDanFromSoLuongNgay();
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
                    this.InitComboMedicineUseForm(null);
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
                        this.SetHuongDanFromSoLuongNgay();
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
                    this.SetHuongDanFromSoLuongNgay();
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

        private void spinAmount_EditValueChanged(object sender, EventArgs e)          
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_Leave(object sender, EventArgs e)
        {
            try
            {
                //this.CalculateAmount();
                //this.SetHuongDanFromSoLuongNgay();
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
                    //this.btnAdd.Focus();
                   txtTutorial.Focus();
                    e.Handled = true;
                }
                //else
                //{
                //    this.txtTutorial.Focus();
                //    this.txtTutorial.SelectionStart = this.txtTutorial.Text.Length + 1;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpionGroupSelectedChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChanged.1");
                this.theRequiredWidth = 1130;
                this.theRequiredHeight = 200;
                this.RebuildMediMatyWithInControlContainer();
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task OpionGroupSelectedChangedAsync()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChangedAsync.1");
                this.theRequiredWidth = 1130;
                this.theRequiredHeight = 200;
                this.RebuildMediMatyWithInControlContainerAsync();
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChangedAsync.2");
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
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    //btnAdd.Focus();
                    txtHtu.Focus();
                    if (e.KeyCode == Keys.Enter)
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
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%'", txtMediMatyForPrescription.Text);
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
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
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

        private void spinSoPhimHong_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotProcessRunWhileFilmChangedValue)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotProcessRunWhileFilmChangedValue), isNotProcessRunWhileFilmChangedValue));
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => spinSoPhimHong.OldEditValue), spinSoPhimHong.OldEditValue) + "" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => spinSoPhimHong.Value), spinSoPhimHong.Value));
                decimal ph = spinSoPhimHong.Value;
                if (this.currentSoPhimHong > 0)
                {
                    ph = spinSoPhimHong.Value - this.currentSoPhimHong;
                }
                decimal vl = spinAmount.Value + ph;
                spinAmount.EditValue = vl > 0 ? vl : 0;

                this.currentSoPhimHong = spinSoPhimHong.EditValue != null ? spinSoPhimHong.Value : 0;
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
                    this.actionBosung = GlobalVariables.ActionEdit;
                    isShowContainerMediMatyForChoose = true;
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                    }
                    else if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    {
                        MessageManager.Show(ResourceMessage.VatTuTSDKhongChoPhepSua);
                        return;
                    }
                    isNotProcessRunWhileFilmChangedValue = true;
                    decimal amountValue = (this.currentMedicineTypeADOForEdit.AMOUNT ?? 0);
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                        && this.currentMedicineTypeADOForEdit.FilmNumber.HasValue && this.currentMedicineTypeADOForEdit.FilmNumber > 0)
                    {

                        chkPhimHong.Checked = true;
                        spinSoPhimHong.EditValue = this.currentMedicineTypeADOForEdit.FilmNumber;
                    }
                    else
                    {
                        chkPhimHong.Checked = false;
                        spinSoPhimHong.EditValue = null;
                    }
                    this.currentSoPhimHong = spinSoPhimHong.Value;
                    isNotProcessRunWhileFilmChangedValue = false;
                    this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    this.cboMedicineUseForm.EditValue = this.currentMedicineTypeADOForEdit.MEDICINE_USE_FORM_ID;
                    this.InstructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();
                    this.SetDefaultComboPhieuDieuTri();
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
                    this.spinAmount.EditValue = amountValue;
                    this.txtTutorial.Text = this.currentMedicineTypeADOForEdit.TUTORIAL;
                    this.txtHtu.Text = this.currentMedicineTypeADOForEdit.HTU_TEXT;
                    this.btnAdd.Enabled = true;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    this.VisibleButton(this.actionBosung);
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();

                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                    List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == currentMedicineTypeADOForEdit.ID && o.LOGINNAME == loginName).ToList();
                    this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
                    this.RebuildHtulWithInControlContainer(medicineTypeTutFilters);

                    if (HisConfigCFG.ManyDayPrescriptionOption == 2
                        && this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds != null
                        && this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds.Count > 0)
                    {
                        if (this.isMultiDateState != this.currentMedicineTypeADOForEdit.IsMultiDateState)
                        {
                            chkMultiIntructionTime.Checked = this.currentMedicineTypeADOForEdit.IsMultiDateState;
                            this.isMultiDateState = chkMultiIntructionTime.Checked;
                        }

                        if (this.isMultiDateState)
                        {
                            UC.DateEditor.ADO.DateInputHasCheckADO dateInputForMediADO = new UC.DateEditor.ADO.DateInputHasCheckADO();
                            dateInputForMediADO.Dates = new List<DateTime?>();
                            dateInputForMediADO.IsMultiDayChecked = this.currentMedicineTypeADOForEdit.IsMultiDateState;
                            dateInputForMediADO.IsVisibleMultiDate = true;

                            foreach (var itemDate in this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds)
                            {
                                DateTime? itemDateCV = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemDate);
                                if (itemDateCV != null && itemDateCV.Value != DateTime.MinValue)
                                {
                                    dateInputForMediADO.Dates.Add(itemDateCV);
                                    dateInputForMediADO.Time = itemDateCV.Value;
                                }
                            }
                            UcDateSetValueHasCheck(dateInputForMediADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isNotProcessRunWhileFilmChangedValue = false;
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
                    mediMatyTypeADO.IsEdit = false;

                    if ((mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && mediMatyTypeADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (mediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0) <= 0)
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
                            if (e.Column.FieldName == "AMOUNT"
                                && (!CheckMaxInPrescription(mediMatyTypeADO, mediMatyTypeADO.AMOUNT)
                                || !CheckOddConvertUnit(mediMatyTypeADO, mediMatyTypeADO.AMOUNT)
                                || !WarningOddConvertWorker.CheckWarningOddConvertAmount(mediMatyTypeADO, (mediMatyTypeADO.AMOUNT ?? 0), null)
                                ))
                            {
                                decimal oldAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(view.ActiveEditor.OldEditValue.ToString());
                                SetOldAmountMediMaty(oldAmount, mediMatyTypeADO.ID, mediMatyTypeADO.PrimaryKey);
                            }

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
                                            if (!TakeOrReleaseBeanWorker.TakeForCreateBean(this.intructionTimeSelecteds, this.oldExpMestId, item, true, param))
                                            {
                                                item.IsNotTakeBean = true;
                                                MessageManager.Show(this, param, false);
                                                success = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            decimal amount = (item.AMOUNT ?? 0);
                                            if (!TakeOrReleaseBeanWorker.TakeForUpdateBean(this.intructionTimeSelecteds, this.oldExpMestId, item, amount, true, param))
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

                        if (e.Column.FieldName == "IsExpend" && mediMatyTypeADO.IsExpend == false)
                        {
                            mediMatyTypeADO.IsExpendType = false;
                        }
                    }
                    mediMatyTypeADO.BK_AMOUNT = mediMatyTypeADO.AMOUNT;

                    gridViewServiceProcess.BeginUpdate();
                    gridViewServiceProcess.GridControl.DataSource = mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                    gridViewServiceProcess.EndUpdate();

                    this.SetTotalPrice__TrongDon();
                    this.ReloadDataAvaiableMediBeanInCombo();
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
                        //if (this.actionType == GlobalVariables.ActionEdit)
                        // e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable;
                        //else
                        //{
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                            e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                        //}
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
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && ((HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && ((data.SereServParentId ?? 0) > 0 || GetSereServInKip() > 0)) || !HisConfigCFG.IsNotAllowingExpendWithoutHavingParent))
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage_Disable;
                    }
                    else if (e.Column.FieldName == "IsExpendType")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && data.IsExpend
                           && ((data.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0)//TODO//Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha"
                            )
                        {
                            e.RepositoryItem = this.repositoryItemchkIsExpendType_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemchkIsExpendType_Disable;
                        }
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
                        if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                            e.RepositoryItem = repositoryItemSpinAmount_Disable__MedicinePage;
                        else if (((data.IsAllowOdd.HasValue && data.IsAllowOdd.Value == true) || (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)) && (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet))
                            e.RepositoryItem = this.repositoryItemSpinAmount_Le_MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemSpinAmount__MedicinePage;
                    }
                    else if (e.Column.FieldName == "IsOutKtcFee")
                    {
                        e.RepositoryItem = this.repositoryItemChkOutKtcFee_Enable_TabMedicine;
                    }
                    else if (e.Column.FieldName == "PrescriptionDays")
                    {
                        if (this.actionType == GlobalVariables.ActionAdd)
                            e.RepositoryItem = this.repositoryItemButtonEditManyDayPres;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
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
                        else if (e.Column.FieldName == "PrescriptionDays")
                        {
                            if (HisConfigCFG.ManyDayPrescriptionOption == 2
                               && data_ManuMedicineADO.IntructionTimeSelecteds != null
                               && data_ManuMedicineADO.IntructionTimeSelecteds.Count > 0
                               )
                            {
                                e.Value = data_ManuMedicineADO.IsMultiDateState ? GetDateFromManyDayPres(data_ManuMedicineADO.IntructionTimeSelecteds) : Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.IntructionTimeSelecteds.First());
                            }
                        }
                        else if (e.Column.FieldName == "TotalPriceDisplay")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data_ManuMedicineADO.TotalPrice, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsUseOrginalUnitForPres ?? false) == false
                                && !String.IsNullOrEmpty(data_ManuMedicineADO.CONVERT_UNIT_CODE)
                                && !String.IsNullOrEmpty(data_ManuMedicineADO.CONVERT_UNIT_NAME))
                            {
                                e.Value = data_ManuMedicineADO.CONVERT_UNIT_NAME;
                            }
                            else
                            {
                                e.Value = data_ManuMedicineADO.SERVICE_UNIT_NAME;
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.EXPIRED_DATE ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_PACKAGE_NUMBER_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = data_ManuMedicineADO.TDL_PACKAGE_NUMBER;
                            }
                        }
                        else if (e.Column.FieldName == "REGISTER_NUMBER_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = data_ManuMedicineADO.REGISTER_NUMBER;
                            }
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
                                if (hi.Column.FieldName == "IsExpendType")//TODO//Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha"
                                {
                                    bool valid = ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && dataRow.IsExpend
                            && ((dataRow.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0));
                                    if (!valid)
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.return__FieldName:IsExpendType____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                                        return;
                                    }
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
                        //}
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
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
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
                else if (view.FocusedColumn.FieldName == "EQUIPMENT_SET_ID" && view.ActiveEditor is GridLookUpEdit
                    && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM))
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && data.EQUIPMENT_SET_ID != null)
                    {
                        editor.EditValue = data.EQUIPMENT_SET_ID;
                        editor.Properties.Buttons[1].Visible = true;
                        editor.ButtonClick += reposityButtonClick;
                    }
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

                if (view.FocusedColumn.FieldName == "IsExpendType")
                {
                    if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        && data.IsExpend
                        && ((data.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0))
                    {
                        //Nothing
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_ShowingEditorFieldName:IsExpendType.Cancel");
                        e.Cancel = true;
                    }
                }
                else if (view.FocusedColumn.FieldName == "IsExpend" && HisConfigCFG.IsNotAllowingExpendWithoutHavingParent)
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

        private void reposityButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridLookUpEdit editor = sender as GridLookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
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
                    else if (row.ErrorTypeAmountHasRound == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmountHasRound);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmountHasRound);
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
                    ErrorType errorType = (ErrorType)(ErrorType.None);
                    string errorText = "";

                    if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        errorType = (ErrorType)(row.ErrorTypeIsAssignDay);
                        errorText += (string)(row.ErrorMessageIsAssignDay);
                    }
                    if (row.ErrorTypeMediMatyBean == ErrorType.Warning)
                    {
                        errorType = (ErrorType)(row.ErrorTypeMediMatyBean);
                        errorText += ((String.IsNullOrEmpty(errorText) ? "" : "; ") + (string)(row.ErrorMessageMediMatyBean));
                    }

                    e.Info.ErrorType = errorType;
                    e.Info.ErrorText = errorText;
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
                    if ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceProcess_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.1");
                if (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet || GlobalStore.IsExecutePTTT)
                    return;
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.2");
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.3");
                    int visibleRowHandle = this.gridViewServiceProcess.GetVisibleRowHandle(hitInfo.RowHandle);
                    int[] selectedRows = this.gridViewServiceProcess.GetSelectedRows();
                    if (selectedRows != null && selectedRows.Length > 0)// && selectedRows.Contains(visibleRowHandle)
                    {
                        Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.4");
                        this.InitMenu();
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.5");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "REMOVE_SELECED_ROW")
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_RowCellClick. REMOVE_SELECED_ROW");
                    var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    WaitingManager.Show();
                    if (mediMatyTypeADO != null && TakeOrReleaseBeanWorker.ProcessDeleteRowMediMaty(this.intructionTimeSelecteds, mediMatyTypeADO))
                    {
                        bool isReloadAvaible = (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                                || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD);
                        if (this.gridViewServiceProcess.FocusedRowHandle == this.gridViewServiceProcess.DataRowCount - 1)
                        {
                            this.idRow = this.idRow - stepRow;
                            if (this.idRow <= 0) this.idRow = 1;
                        }
                        this.gridViewServiceProcess.BeginUpdate();
                        this.gridViewServiceProcess.DeleteRow(this.gridViewServiceProcess.FocusedRowHandle);
                        this.gridViewServiceProcess.EndUpdate();

                        this.mediMatyTypeADOs.Remove(mediMatyTypeADO);
                        if (isReloadAvaible)
                            this.ReloadDataAvaiableMediBeanInCombo();

                        if (currentMedicineTypeADOForEdit != null &&
                            mediMatyTypeADO.PrimaryKey == currentMedicineTypeADOForEdit.PrimaryKey)
                        {
                            this.actionBosung = GlobalVariables.ActionAdd;
                            this.VisibleButton(this.actionBosung);
                            this.ReSetDataInputAfterAdd__MedicinePage();
                            txtMediMatyForPrescription.Text = "";
                            currentMedicineTypeADOForEdit = null;
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Remove row in grid fail or Call release bean fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADO), mediMatyTypeADO));
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
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
                    MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                }              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is DMediStock1ADO)
                    {
                        DMediStock1ADO data = (DMediStock1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            //#22134
                            //- Sửa lại cột "giá nhập" --> "giá bán". Trường này lấy dữ liệu từ: LAST_EXP_PRICE, LAST_EXP_VAT_RATIO. Theo công thức:
                            //"giá bán" = LAST_EXP_PRICE * (1 + LAST_EXP_VAT_RATIO).
                            //- Sửa ở combobox "tồn thuốc", và "combobox" kê thuốc ngoài kho.
                            //- Lưu ý: cần check trường hợp null, nếu cả 2 trường null thì hiển thị trống. Còn 1 trong 2 trường null, thì trường nào null thì coi giá trị trường đấy = 0.
                            if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                            {
                                if (data.LAST_EXP_PRICE.HasValue || data.LAST_EXP_VAT_RATIO.HasValue)
                                {
                                    decimal? priceRaw = (data.LAST_EXP_PRICE ?? 0) * (1 + (data.LAST_EXP_VAT_RATIO ?? 0));
                                    //decimal? price = (data.CONVERT_RATIO.HasValue && data.CONVERT_RATIO > 0) ? priceRaw / data.CONVERT_RATIO.Value : priceRaw;
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(priceRaw ?? 0, ConfigApplications.NumberSeperator);
                                }
                            }
                            else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                            {
                                e.Value = data.IMP_VAT_RATIO * 100;
                            }
                            else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                            {
                                if (
                                    (data.IsUseOrginalUnitForPres ?? false) == false &&//TODO
                                    !String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                    && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "USE_REMAIN_COUNT_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                            }
                        }
                    }
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is MedicineMaterialTypeComboADO)
                    {
                        MedicineMaterialTypeComboADO data = (MedicineMaterialTypeComboADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                            {
                                if (data.LAST_EXP_PRICE.HasValue || data.LAST_EXP_VAT_RATIO.HasValue)
                                {
                                    decimal? priceRaw = (data.LAST_EXP_PRICE ?? 0) * (1 + (data.LAST_EXP_VAT_RATIO ?? 0));
                                    //decimal? price = (data.CONVERT_RATIO.HasValue && data.CONVERT_RATIO > 0) ? priceRaw / data.CONVERT_RATIO.Value : priceRaw;
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(priceRaw ?? 0, ConfigApplications.NumberSeperator);
                                }
                            }
                            else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                            {
                                e.Value = data.IMP_VAT_RATIO * 100;
                            }
                            else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "USE_REMAIN_COUNT_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
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

        private void gridViewTutorial_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = gridViewTutorial.GetFocusedRow() as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypeTut != null)
                {
                    popupControlContainerTutorial.HidePopup();
                    isShowContainerTutorial = false;
                    Tutorial_RowClick(medicineTypeTut);
                }
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
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
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
                                    text += ";" + ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
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

        void DelegateSelectMultiDateManyDayPres(List<DateTime?> datas, DateTime time)
        {
            try
            {
                List<long> insTimes = new List<long>();
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (datas != null && datas.Count > 0)
                {
                    for (int i = 0, j = datas.Count; i < j; i++)
                    {
                        var dt = new DateTime(datas[i].Value.Year, datas[i].Value.Month, datas[i].Value.Day, time.Hour, time.Minute, 0);
                        insTimes.Add(Inventec.Common.TypeConvert.Parse.ToInt64(dt.ToString("yyyyMMddHHmm") + "00"));
                    }

                }
                mediMatyTypeADO.IntructionTimeSelecteds = insTimes;
                mediMatyTypeADO.IsMultiDateState = true;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void DelegateSelectSingleDayPres(DateTime date)
        {
            try
            {
                List<long> insTimes = new List<long>();
                insTimes.Add(Inventec.Common.TypeConvert.Parse.ToInt64(date.ToString("yyyyMMddHHmm") + "00"));

                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                mediMatyTypeADO.IntructionTimeSelecteds = insTimes;
                mediMatyTypeADO.IsMultiDateState = false;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditManyDayPres_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                List<DateTime?> datas = new List<DateTime?>();
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();

                if (mediMatyTypeADO.IntructionTimeSelecteds != null && mediMatyTypeADO.IntructionTimeSelecteds.Count > 0)
                {
                    for (int i = 0, j = mediMatyTypeADO.IntructionTimeSelecteds.Count; i < j; i++)
                    {
                        var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(mediMatyTypeADO.IntructionTimeSelecteds[i]) ?? DateTime.Now;
                        datas.Add(dt);
                    }

                    if (mediMatyTypeADO.IsMultiDateState)
                    {
                        frmMultiIntructonTime frmMultiIntructonTime = new frmMultiIntructonTime(datas, datas[0].Value, DelegateSelectMultiDateManyDayPres);
                        frmMultiIntructonTime.ShowDialog();
                    }
                    else
                    {
                        frmChoiceSingleDate frmChoiceSingleDate = new frmChoiceSingleDate(datas[0], DelegateSelectSingleDayPres);
                        frmChoiceSingleDate.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookupEditExpendTypeHasValue_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    //mediMatyTypeADO.ExpendTypeId = null;

                    //if (this.gridViewServiceProcess.IsEditing)
                    //    this.gridViewServiceProcess.CloseEditor();

                    //if (this.gridViewServiceProcess.FocusedRowModified)
                    //    this.gridViewServiceProcess.UpdateCurrentRow();

                    //this.gridControlServiceProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoTreatmentFinish__Checked()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    btnSave.Enabled = btnSaveAndPrint.Enabled = btnSaveAndShowPrintPreview.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #endregion

        #region UCSecondaryIcd
        internal bool ShowPopupIcdChoose()
        {
            try
            {
                WaitingManager.Show();
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.currentIcds);
                WaitingManager.Hide();
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.currentIcds);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    UcIcdNextForcusOut();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.currentIcds);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
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
                                var checkInList = this.currentIcds.Where(o =>
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

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtIcdSubCode.Focus();
                            this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        #endregion

        #region UcIcd
        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMainText.Text = result.First().ICD_NAME;
                        cboIcds.EditValue = listData.First().ID;
                        chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);

                        if (chkEditIcd.Checked)
                        {
                            txtIcdMainText.Focus();
                            txtIcdMainText.SelectAll();
                        }
                        else
                        {
                            cboIcds.Focus();
                            cboIcds.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboIcds.Focus();
                    cboIcds.ShowPopup();
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
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD()
        {
            try
            {
                cboIcds.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMainText.Text = icd.ICD_NAME;
                    chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                    if (chkEditIcd.Checked)
                    {
                        this.NextForcusSubIcd();
                    }
                    else if (chkEditIcd.Enabled)
                    {
                        chkEditIcd.Focus();
                    }
                    else
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (HisConfigCFG.AutoCheckIcd != "2")
                {
                    this.chkEditIcd.Enabled = true;
                    this.chkEditIcd.Checked = true;
                }
                this.txtIcdMainText.Text = text;
                this.txtIcdMainText.Focus();
                this.txtIcdMainText.SelectAll();
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
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcds.ClosePopup();
                    cboIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcds.ClosePopup();
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboIcds.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcd.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainText.Text = this._TextIcdName;
                    else
                        txtIcdMainText.Text = cboIcds.Text;
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();
                }
                else if (chkEditIcd.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                    txtIcdMainText.Text = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProviderControl.RemoveControlError(txtIcdCode);
                        ValidationICD(10, 500, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    txtIcdMainText.Text = "";
                    cboIcds.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                //if (icd != null)
                //{
                //    if (icd != null && icd.IS_REQUIRE_CAUSE == 1)
                //    {
                //        this.LoadRequiredCause(true);
                //    }
                //    else
                //        this.LoadRequiredCause(false);
                //}
                //else
                //{
                //    this.LoadRequiredCause(false);
                //}

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region UcDate
        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelected, timeSelested, SelectMultiIntructionTime);
                    frmChooseIntructionTime.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    NextForcusUCDate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isStopEventChangeMultiDate)
                {
                    return;
                }

                this.txtInstructionTime.Visible = this.chkMultiIntructionTime.Checked;
                this.dtInstructionTime.Visible = !this.chkMultiIntructionTime.Checked;

                if (this.chkMultiIntructionTime.Checked)
                {
                    this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                    string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                    this.txtInstructionTime.Text = strTimeDisplay;
                }
                this.DelegateMultiDateChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkMultiIntructionTime.Enabled || lcichkMultiDate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.chkMultiIntructionTime.Focus();
                    }
                    else
                    {
                        this.NextForcusUCDate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_Leave(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

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

        private void cboPhieuDieuTri_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
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
                        if (this.currentDhst != null)
                        {
                            listArgs.Add(this.currentDhst);
                        }
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.InitComboPhieuDieuTri();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPhieuDieuTri()
        {

            try
            {
                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = this.treatmentId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);

                this.trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TrackingTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    this.trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "Thời gian", 250, 1));
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboPhieuDieuTri, trackingADOs, controlEditorADO);
                SetDefaultComboPhieuDieuTri();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetDefaultComboPhieuDieuTri(HIS_SERVICE_REQ serviceReq = null)
        {
            if (serviceReq != null)
            {
                cboPhieuDieuTri.EditValue = serviceReq.TRACKING_ID;
            }
            else if (HisConfigCFG.IsDefaultTracking && trackingADOs.Count > 0 && cboUser.EditValue != null && InstructionTime != 0)
            {
                cboPhieuDieuTri.EditValue = trackingADOs.Where(o => o.CREATOR == cboUser.EditValue.ToString() && o.TRACKING_TIME.ToString().Substring(0, 8) == InstructionTime.ToString().Substring(0, 8)).OrderByDescending(o => o.TRACKING_TIME).FirstOrDefault().ID;
            }
            else
            {
                cboPhieuDieuTri.EditValue = null;
            }
        }

        private void cboPhieuDieuTri_EditValueChanged(object sender, EventArgs e)
        {
            if(cboPhieuDieuTri.EditValue != null)
            {
                cboPhieuDieuTri.Properties.Buttons[2].Visible = true;
            }else
                cboPhieuDieuTri.Properties.Buttons[2].Visible = false;
        }        
        private void txtHtu_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerHtu = !isShowContainerHtu;
                    if (isShowContainerHtu)
                    {
                        Rectangle buttonBounds = new Rectangle(txtHtu.Bounds.X, txtHtu.Bounds.Y, txtHtu.Bounds.Width, txtHtu.Bounds.Height);
                        popupControlContainerHtu.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHtu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    btnAdd.Focus();
                    if (e.KeyCode == Keys.Enter)
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHtu_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                hisMedicineTypeTut = gridViewHtu.GetFocusedRow() as HIS_MEDICINE_TYPE_TUT;
                if (hisMedicineTypeTut != null)
                {
                    popupControlContainerHtu.HidePopup();
                    isShowContainerHtu = false;
                    Htu_RowClick(hisMedicineTypeTut);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHtu_KeyDown(object sender, KeyEventArgs e)
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
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
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
    }
}
