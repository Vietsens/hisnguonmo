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
using DevExpress.XtraBars;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.ConfigSystem;
using EMR_MAIN;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TreatmentList.Base;
using Inventec.Common.RichEditor.DAL;
using Inventec.Desktop.Common.LanguageManager;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ModuleExt;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.TreatmentList.Config;
using HIS.UC.UCCauseOfDeath.ADO;

namespace HIS.Desktop.Plugins.TreatmentList
{

    public partial class UCTreatmentList : UserControlBase
    {

        void Treatment_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.currentTreatment != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    switch (type)
                    {
                        #region -----

                        case PopupMenuProcessor.ItemType.EventLog:
                            btnEvenLogClick();
                            break;
                        case PopupMenuProcessor.ItemType.Tracking:
                            btnTrackingClick();
                            break;
                        case PopupMenuProcessor.ItemType.vi:
                            btnviClick();
                            break;
                        case PopupMenuProcessor.ItemType.Timeline:
                            btnTimelineClick();
                            break;
                        case PopupMenuProcessor.ItemType.ExamAggr:
                            btnTongHopDonPhongKhamClick();
                            break;
                        case PopupMenuProcessor.ItemType.PatientUpdate:
                            btnPatientUpdateClick();
                            break;
                        case PopupMenuProcessor.ItemType.Finishtreat:
                            btnFinishtreatClick();
                            break;
                        case PopupMenuProcessor.ItemType.OpenTreat:
                            btnOpenTreatClick();
                            break;
                        case PopupMenuProcessor.ItemType.Bo:
                            btnBoClick();
                            break;
                        case PopupMenuProcessor.ItemType.print:
                            btnprintClick();
                            break;
                        case PopupMenuProcessor.ItemType.BornInfo:
                            btnBornInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.DeathInfo:
                            btnDeathInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.TranPatiOutInfo:
                            btnTranPatiOutInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.TranPatiInInfo:
                            btnTranPatiInInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.AssignService:
                            btnAssignServiceClick();
                            break;
                        case PopupMenuProcessor.ItemType.AssignPan:
                            btnAssignPanClick();
                            break;
                        case PopupMenuProcessor.ItemType.AppointmentInfo:
                            btnAppointmentInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.HoSoGiayToDinhKem:
                            HoSoGiayToDinhKemClick();
                            break;
                        case PopupMenuProcessor.ItemType.AppointmentService:
                            btnAppointmentServiceClick();
                            break;
                        case PopupMenuProcessor.ItemType.ComminBed:
                            btnComminBedClick();
                            break;
                        case PopupMenuProcessor.ItemType.fixTreatment:
                            btnfixTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.ViewPackge:
                            btnViewPackgeClick();
                            break;
                        case PopupMenuProcessor.ItemType.patientInf:
                            btnpatientInfClick();
                            break;
                        case PopupMenuProcessor.ItemType.MargePatient:
                            btnMargePatientClick();
                            break;
                        case PopupMenuProcessor.ItemType.SarprintList:
                            btnSarprintListClick();
                            break;
                        case PopupMenuProcessor.ItemType.HistoryTreat:
                            btnHistoryTreatClick();
                            break;
                        case PopupMenuProcessor.ItemType.Feehop:
                            btnFeehopClick();
                            break;
                        case PopupMenuProcessor.ItemType.RequestDeposit:
                            btnRequestDeposit();
                            break;
                        case PopupMenuProcessor.ItemType.MedicalAssessment:
                            btnMedicalAssessment();
                            break;
                        case PopupMenuProcessor.ItemType.HivTreatment:
                            btnHivTreatment();
                            break;
                        case PopupMenuProcessor.ItemType.TuberclusisTreatment:
                            btnTuberclusisTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.TimelineTest:
                            btnTimelineTestClick();
                            break;
                        case PopupMenuProcessor.ItemType.PublicMedicineByPhased:
                            btnPublicMedicineByPhasedClick();
                            break;
                        case PopupMenuProcessor.ItemType.HisAdr:
                            btnHisAdrClick();
                            break;
                        case PopupMenuProcessor.ItemType.AllergyCard:
                            btnAllergyCardClick();
                            break;
                        case PopupMenuProcessor.ItemType.DongboEMR:
                            btnDongboEMRClick();
                            break;
                        #endregion
                        case PopupMenuProcessor.ItemType.ViewHSSKCN:
                            ViewHSSHCNClick();
                            break;
                        case PopupMenuProcessor.ItemType.GiayRaVien:
                            GiayRaVienPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType.GiayPTTT:
                            GiayPTTTPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType.GiayTT:
                            GiayTTPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType._PhieuHenKham:
                            PhieuhenKhamPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType._PhieuHenMo:
                            PhieuhenMoPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType._PhieuChuyenVien:
                            PhieuChuyenVienPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuCongKhaiThuocTheoNgay:
                            btnPublicMedicineByDateClick();
                            break;
                        case PopupMenuProcessor.ItemType._GiayKhamBenhVaoVien:
                            GiayKhamBenhVaoVienClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType._BenhAnNgoaiTru:
                            InBenhAnNgoaiTruClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType._GiayTHXN:
                            GiayKetQuaXetNghiemClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType._HSSKCN:
                            HoSoQuanLySucKhoeCaNhan(null, null);
                            break;
                        case PopupMenuProcessor.ItemType._THE_BENH_NHAN:
                            TheBenhNhanClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.TreatmentBedRoomList:
                            TreatmentBedRoomListClick();
                            break;
                        case PopupMenuProcessor.ItemType.CertificateOfTBTreatment:
                            CertificateOfTBTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.BedHistory:
                            BedHistoryClick();
                            break;
                        case PopupMenuProcessor.ItemType.OtherFormAssTreatment:
                            btnOtherFormAssTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.HisDhst:
                            btnHisDhstTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.PREPARE:
                            btnPrepareClick();
                            break;
                        case PopupMenuProcessor.ItemType.HisDhstChart:
                            btnHisDhstChartClick();
                            break;
                        case PopupMenuProcessor.ItemType.SumaryTestResults:
                            btnSumaryTestResultsClick();
                            break;
                        case PopupMenuProcessor.ItemType.MRRegulationslist:
                            btnMRRegulationslistClick();
                            break;
                        case PopupMenuProcessor.ItemType.DebateDiagnostic:
                            btnDebateDiagnosticClick();
                            break;
                        case PopupMenuProcessor.ItemType.DaSuaBenhAn:
                            btnDaSuaBenhAnClick();
                            break;
                        case PopupMenuProcessor.ItemType.CareSlipList:
                            btnCareSlipListClick();
                            break;
                        case PopupMenuProcessor.ItemType.MediReactSum:
                            btnMediReactSumClick();
                            break;
                        case PopupMenuProcessor.ItemType.AccidentHurt:
                            btnAccidentHurtClick();
                            break;
                        case PopupMenuProcessor.ItemType.InfusionSumByTreatment:
                            btnInfusionSumByTreatmentClick();
                            break;
                        case PopupMenuProcessor.ItemType.BloodTransfusion:
                            btnBloodTransfusionClick();
                            break;
                        case PopupMenuProcessor.ItemType.SendOldSystemIntegration:
                            btnSendOldSystemIntegrationClick();
                            break;
                        case PopupMenuProcessor.ItemType.SendTreatmentOfOldSystem:
                            btnSendTreatmentOfOldSystemClick();
                            break;
                        case PopupMenuProcessor.ItemType.PublicServices_NT:
                            btnPublicService_NTClick();
                            break;
                        case PopupMenuProcessor.ItemType.CheckInfoBHYT:
                            btnCheckInfoBHYTClick();
                            break;
                        case PopupMenuProcessor.ItemType.EndTypeExt:
                            btnEndTypeExt();
                            break;
                        case PopupMenuProcessor.ItemType.PublicServices_NT_ByDay:
                            btnPublicService_NT_ByDayClick();
                            break;
                        case PopupMenuProcessor.ItemType.TomTatBenhAn330or331:
                            InTomTatBenhAnClick330or331(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.PhieuDKSDThuocDVKTNgoaiBHYT:
                            PhieuDangKySuDungThuocDVKTNgoaiBHYTClick();
                            break;
                        case PopupMenuProcessor.ItemType.BangKiemTruocTiemChung:
                            InBangKiemTruocTiemChungClick();
                            break;
                        case PopupMenuProcessor.ItemType.CheckingTreatmentEmr:
                            CheckingTreatmentEmr();
                            break;
                        case PopupMenuProcessor.ItemType.BanGiaoBNTruocPTTT:
                            BanGiaoBNTruocPTTT();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuXNDuongMauMaoMach:
                            PhieuXNDuongMauMaoMach();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuSangLocDinhDuongNguoiBenh:
                            PhieuSangLocDinhDuongNguoiBenh();
                            break;
                        case PopupMenuProcessor.ItemType.BenhAnNgoaiTruDayMat:
                            BenhAnNgoaiTruDayMatClick();
                            break;
                        case PopupMenuProcessor.ItemType.BenhAnNgoaiTruGlaucoma:
                            BenhAnNgoaiTruGlaucomaClick();
                            break;
                        case PopupMenuProcessor.ItemType.deathdiagnosis:
                            DeathDiagnosisClick();
                            break;
                        case PopupMenuProcessor.ItemType.severeillness:
                            SevereillnessClick();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuHuyThuocVatTu:
                            PhieuHuyThuocVatTuClick();
                            break;
                        case PopupMenuProcessor.ItemType.ChiTietBenhAn:
                            ChiTietBenhAnClick();
                            break;
                        case PopupMenuProcessor.ItemType.ThongTinDichTe:
                            ThongTinDichTeClick();
                            break;
                        case PopupMenuProcessor.ItemType.InDonThuoc:
                            ThongTinInDonThuoc();
                            break;
                        case PopupMenuProcessor.ItemType.ChanDoanTuVong:
                            ChanDoanNguyenNhanTuVong();
                            break;
                        case PopupMenuProcessor.ItemType.InDonThuocPTTT:
                            ThongTinInDonThuocPTTT();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BenhNangXinVe(object sender, ItemClickEventArgs e)
        {
            try
            {
                try
                {
                    var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                    richEditorMain.RunPrintTemplate("Mps000484", DelegateRunPrinter);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ThongTinTuVong(object sender, ItemClickEventArgs e)
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000485", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessPrintMps000485(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000485");
                var treatmentId = (gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4).ID;
                if (treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = treatmentId;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH == 1);
                        if (SevereIllnessInfo != null)
                        {
                            departmentTran = GetDepartmentTran(treatmentId, SevereIllnessInfo.DEPARTMENT_ID ?? 0);
                            department = GetDepartment(SevereIllnessInfo.DEPARTMENT_ID ?? 0);
                            lstEvents = GetEventCauseDeath(SevereIllnessInfo.ID);
                        }
                    }
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = GetPatientTypeAlter(treatmentId);
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();
                    List<HIS_ICD> currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                    WaitingManager.Hide();
                    MPS.Processor.Mps000485.PDO.Mps000485PDO pdo = new MPS.Processor.Mps000485.PDO.Mps000485PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        currentIcds,
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList());
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessPrintMps000484(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000484");
                var treatmentId = (gridViewtreatmentList.GetFocusedRow() as V_HIS_TREATMENT_4).ID;
                if (treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = treatmentId;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatmentId;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH != 1);
                        if (SevereIllnessInfo != null)
                        {
                            departmentTran = GetDepartmentTran(treatmentId, SevereIllnessInfo.DEPARTMENT_ID ?? 0);
                            department = GetDepartment(SevereIllnessInfo.DEPARTMENT_ID ?? 0);
                            lstEvents = GetEventCauseDeath(SevereIllnessInfo.ID);
                        }
                    }
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = GetPatientTypeAlter(treatmentId);
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();
                    List<HIS_ICD> currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                    WaitingManager.Hide();
                    MPS.Processor.Mps000484.PDO.Mps000484PDO pdo = new MPS.Processor.Mps000484.PDO.Mps000484PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        currentIcds,
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList());
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_PATIENT GetPatientByID(long id)
        {
            HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private HIS_PATIENT_TYPE_ALTER GetPatientTypeAlter(long treatmentId)
        {
            CommonParam param = new CommonParam();
            HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
            HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
            patientTypeAlterFilter.TREATMENT_ID = treatmentId;
            var patientTypeAlterData = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
            if (patientTypeAlterData != null && patientTypeAlterData.Count > 0)
            {
                patientTypeAlter = patientTypeAlterData.OrderByDescending(o => o.CREATE_TIME).ToList()[0];
            }
            return patientTypeAlter;
        }
        private HIS_DEPARTMENT_TRAN GetDepartmentTran(long treatmentId,long departmentId)
        {
            CommonParam param = new CommonParam();
            HIS_DEPARTMENT_TRAN departmentTran = null;
            MOS.Filter.HisDepartmentTranFilter filterDepartmentTran = new HisDepartmentTranFilter();
            filterDepartmentTran.TREATMENT_ID = treatmentId;
            filterDepartmentTran.DEPARTMENT_ID = departmentId;
            var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDepartmentTran, null);
            if (datas != null && datas.Count > 0)
                departmentTran = datas.Last();
            return departmentTran;
        }
        private HIS_DEPARTMENT GetDepartment(long departmentId)
        {
            CommonParam param = new CommonParam();
            HIS_DEPARTMENT department = null;
            MOS.Filter.HisDepartmentFilter filterDepartment = new HisDepartmentFilter();
            filterDepartment.ID = departmentId;
            var datasDepatment = new BackendAdapter(null).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filterDepartment, null);
            if (datasDepatment != null && datasDepatment.Count > 0)
                department = datasDepatment.First();
            return department;
        }
        private List<HIS_EVENTS_CAUSES_DEATH> GetEventCauseDeath(long id)
        {
            CommonParam param = new CommonParam();
            List<HIS_EVENTS_CAUSES_DEATH> lstEvents = null;
            HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
            filterChild.SEVERE_ILLNESS_INFO_ID = id;
            lstEvents = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
            return lstEvents;
        }
        private void ThongTinInDonThuocPTTT()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    this.currentTreatmentPrint = currentTreatment;
                    richEditorMain.RunPrintTemplate("Mps000478", DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThongTinInDonThuoc()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    SetDataInDonThuoc(currentTreatment);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThongTinDichTeClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EpidemiologyInfo", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChiTietBenhAnClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.TREATMENT_CODE);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EmrDocument", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuHuyThuocVatTuClick()
        {
            try
            {

                List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();


                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestViewFilter prescriptionViewFIlter = new HisExpMestViewFilter();
                prescriptionViewFIlter.TDL_TREATMENT_ID = this.currentTreatment.ID;

                lstExpMest = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, prescriptionViewFIlter, param) ?? new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>();

                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                //medicineFilter.IS_NOT_PRES = 1;
                medicineFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                var expMestMedicine = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();

                //MOS.Filter.HisExpMestMaterialViewFilter materialFilter = new MOS.Filter.HisExpMestMaterialFilter();
                ////materialFilter.IS_NOT_PRES = 1;
                //materialFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                //var expMestMaterial = new BackendAdapter(param)
                //    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param) ?? new List<HIS_EXP_MEST_MATERIAL>();

                List<long> expMestIds = lstExpMest.Select(o => o.ID).Distinct().ToList();
                MOS.Filter.HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                hisExpMestMaterialViewFilter.EXP_MEST_IDs = expMestIds;
                var expMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);


                if (expMestMedicine != null && expMestMedicine.Count > 0)
                {
                    expMestMedicineTemps = expMestMedicine.Where(o => o.IS_NOT_PRES == 1).ToList();
                }

                if (expMestMaterial != null && expMestMaterial.Count > 0)
                {
                    expMestMaterialTemps = expMestMaterial.Where(o => o.IS_NOT_PRES == 1).ToList();
                }

                if (expMestMedicineTemps.Count <= 0 && expMestMaterialTemps.Count <= 0)
                {
                    MessageManager.Show("Không có dữ liệu hủy thuốc/vật tư");
                    return;
                }

                var mps000434 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(lstExpMest, expMestMedicineTemps, expMestMaterialTemps);
                mps000434.RoomId = this.currentModule.RoomId;
                if (mps000434 != null)
                {
                    mps000434.Print("Mps000434", false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnDaSuaBenhAnClick()
        {
            try
            {
                try
                {
                    if (this.currentTreatment != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisTreatmentRejectStoreSDO treatmentIn = new HisTreatmentRejectStoreSDO();
                        treatmentIn.TreatmentId = this.currentTreatment.ID;
                        var treatOut = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/HandledRejectStore", ApiConsumers.MosConsumer, treatmentIn, param);
                        if (treatOut != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ThongTinCungChiTra()
        //{
        //    try
        //    {
        //        List<object> listArgs = new List<object>();
        //        HIS_TREATMENT treatment = new HIS_TREATMENT();
        //        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, this.currentTreatment);
        //        listArgs.Add(treatment);
        //        listArgs.Add((RefeshReference)BtnSearch);
        //        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentFundInfo", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnPublicService_NT_ByDayClick()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicServices_NT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicServices_NT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEndTypeExt()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PregnancyRest").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PregnancyRest");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatment.ID);
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

        private void btnCheckInfoBHYTClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CheckInfoBHYT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CheckInfoBHYT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatment.ID);
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

        private void btnPublicService_NTClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {

                    HIS.Desktop.Plugins.Library.PrintPublicMedicines.PrintPublicMedicinesProcessor pross = new Library.PrintPublicMedicines.PrintPublicMedicinesProcessor(this.currentTreatment.ID, false, this.currentModule != null ? this.currentModule.RoomId : 0);
                    pross.Print();

                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicServices_NT").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicServices_NT");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    //    List<object> listArgs = new List<object>();
                    //    listArgs.Add(this.currentTreatment);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSendTreatmentOfOldSystemClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatment/SendOldPatientToOldSystem", ApiConsumers.MosConsumer, this.currentTreatment.ID, param);
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, rs);

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSumaryTestResultsClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.SumaryTestResults", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHisDhstChartClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisDhstChart", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHisDhstTreatmentClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisDhst", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrepareClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = this.currentTreatment.ID;
                    listArgs.Add(treatment);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Prepare", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOtherFormAssTreatmentClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    OtherFormAssTreatmentInputADO otherFormAssTreatmentInputADO = new OtherFormAssTreatmentInputADO();
                    otherFormAssTreatmentInputADO.TreatmentId = this.currentTreatment.ID;
                    List<object> listObj = new List<object>();
                    listObj.Add(otherFormAssTreatmentInputADO);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewtreatmentList_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewtreatmentList.GetVisibleRowHandle(hi.RowHandle);

                    this.currentTreatment = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(rowHandle);

                    gridViewtreatmentList.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewtreatmentList.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager2 == null)
                    {
                        barManager2 = new BarManager();
                        barManager2.Form = this;
                    }
                    if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                    popupMenuProcessor = new PopupMenuProcessor(this.currentTreatment, barManager2, Treatment_MouseRightClick, (RefeshReference)BtnSearch);//(RefeshReference)BtnSearch
                    popupMenuProcessor.InitMenu(this.emrMenuPopupProcessor, this.currentModule.RoomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPublicMedicineByDateClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicMedicineByDate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicMedicineByDate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatment.ID);
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

        private void BADienTuEmrClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EmrDocument", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuChuyenVienPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var hisTreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, this.currentTreatment);
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                    printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuhenKhamPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var hisTreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, this.currentTreatment);
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                    printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuhenMoPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var hisTreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, this.currentTreatment);
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                    printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_HEN_MO__Mps000389, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GiayPTTTPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT.ToString());
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ListSurgMisuByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GiayTTPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT.ToString());
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ListSurgMisuByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreatmentBedRoomListClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.TREATMENT_CODE);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentBedRoomList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CertificateOfTBTreatmentClick()
        {
            try
            {
                HIS_MEDI_ORG MediOrg = new HIS_MEDI_ORG();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_ORG>(MediOrg, this.currentTreatment);
                frmTuberculosisTreatment frm = new frmTuberculosisTreatment(this.currentTreatment.ID);
                frm.ShowDialog();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BedHistoryClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    V_HIS_TREATMENT_BED_ROOM HaveTreatmentID = new V_HIS_TREATMENT_BED_ROOM();
                    HaveTreatmentID.TREATMENT_ID = this.currentTreatment.ID;
                    listArgs.Add(HaveTreatmentID);
                    listArgs.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BedHistory", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ViewHSSHCNClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var card = GetHisCard(this.currentTreatment.PATIENT_ID);
                    //if (card != null)
                    //{
                    //    new LaunchChrome().Launch(card.CARD_CODE, ConfigSystems.URI_API_HSSK);
                    //}
                    //else
                    //{
                    //    MessageManager.Show("Bệnh nhân chưa có thẻ thông minh");
                    //}

                    ProcessDataForViewEHR(this.currentTreatment, card);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataForViewEHR(V_HIS_TREATMENT_4 treatment, HIS_CARD card)
        {
            try
            {
                if (treatment != null && !String.IsNullOrWhiteSpace(HisConfigCFG.HSSKAddress))
                {
                    string fulladdress = HisConfigCFG.HSSKAddress;

                    if (card != null)
                    {
                        fulladdress = fulladdress.Replace(":CardCode", card.CARD_CODE);
                    }

                    string base64param = "";
                    if (!String.IsNullOrWhiteSpace(HisConfigCFG.HSSKBase64UrlParamInput))
                    {
                        string urlParam = HisConfigCFG.HSSKBase64UrlParamInput;
                        Dictionary<string, string> dicData = new Dictionary<string, string>();

                        CommonParam param = new CommonParam();
                        HisPatientFilter patientFilter = new HisPatientFilter();
                        patientFilter.ID = treatment.PATIENT_ID;
                        var apiData = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param);

                        if (apiData != null && apiData.Count > 0)
                        {
                            var patient = apiData.FirstOrDefault();
                            if (patient != null)
                            {
                                dicData.Add("COMMUNE_NAME", patient.COMMUNE_NAME);
                                dicData.Add("DISTRICT_NAME", patient.DISTRICT_NAME);
                                dicData.Add("PROVINCE_NAME", patient.PROVINCE_NAME);
                                dicData.Add("PATIENT_NAME", patient.VIR_PATIENT_NAME);
                                dicData.Add("DOB", patient.DOB.ToString().Substring(0, 8));
                                dicData.Add("CMND_NUMBER", patient.CMND_NUMBER);
                                dicData.Add("REGISTER_CODE", patient.REGISTER_CODE);
                                dicData.Add("REQ_CODE", !String.IsNullOrWhiteSpace(patient.REGISTER_CODE) ? patient.REGISTER_CODE : (card != null ? card.CARD_CODE : ""));
                                dicData.Add("PHONE", !String.IsNullOrWhiteSpace(patient.MOBILE) ? patient.MOBILE : patient.PHONE);

                                if (patient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                                {
                                    dicData.Add("GENDER_CODE", "1");
                                }
                                else if (patient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                                {
                                    dicData.Add("GENDER_CODE", "2");
                                }
                                else
                                {
                                    dicData.Add("GENDER_CODE", "3");
                                }
                            }
                        }

                        if (card != null)
                        {
                            dicData.Add("CARD_CODE", card.CARD_CODE);
                        }

                        dicData.Add("HEIN_CARD_NUMBER", treatment.TDL_HEIN_CARD_NUMBER);
                        dicData.Add("HEIN_MEDI_ORG_CODE", treatment.TDL_HEIN_MEDI_ORG_CODE);

                        if (BranchDataWorker.Branch != null)
                        {
                            var branch = BranchDataWorker.Branch;
                            dicData.Add("CURRENT_HEIN_MEDI_ORG_CODE", branch.HEIN_MEDI_ORG_CODE);
                        }


                        foreach (var data in dicData)
                        {
                            urlParam = urlParam.Replace(string.Format("<#{0};>", data.Key), data.Value);
                        }

                        base64param = Convert.ToBase64String(Encoding.UTF8.GetBytes(urlParam));
                    }

                    if (!String.IsNullOrWhiteSpace(base64param))
                    {
                        fulladdress = fulladdress.Replace("<#BASE_64_PARAM;>", base64param);
                    }

                    System.Diagnostics.Process.Start(fulladdress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        HIS_CARD GetHisCard(long patientId)
        {
            try
            {
                HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = patientId;
                cardFilter.ORDER_DIRECTION = "DESC";
                cardFilter.ORDER_FIELD = "CREATE_TIME";
                var result = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>(HisRequestUriStore.HIS_CARD_GET, ApiConsumer.ApiConsumers.MosConsumer, cardFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (result != null && result.Count > 0)
                {
                    return result[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void GiayRaVienPrintClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var hisTreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(hisTreatment, this.currentTreatment);
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatment, currentModule != null ? currentModule.RoomId : 0);
                    printProcess.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEvenLogClick()
        {
            CommonParam param = new CommonParam();

            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit dataInit = new Inventec.UC.EventLogControl.Data.DataInit(ConfigApplications.NumPageSize, "", "", currentTreatment.TREATMENT_CODE, "", "", "");
                    KeyCodeADO ado = new KeyCodeADO();
                    ado.treatmentCode = currentTreatment.TREATMENT_CODE;
                    listArgs.Add(ado);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EventLog", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void exceptionProcess(CommonParam param)
        {
            Inventec.Common.Logging.LogSystem.Info("param: " + Inventec.Common.Logging.LogUtil.TraceData("", param));
        }

        private void btnTrackingClick()
        {
            CommonParam param = new CommonParam();

            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTrackingList").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTrackingList");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    //var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnviClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT thistreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(thistreatment, currentTreatment);
                    listArgs.Add(thistreatment);
                    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //((Form)extenceInstance).ShowDialog();
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTimelineClick()
        {
            try
            {
                WaitingManager.Show();
                if (currentTreatment != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentLog").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentLog'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();
                    //Set data to assignBloodADO
                    TreatmentLogADO.TreatmentId = currentTreatment.ID;
                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    listArgs.Add(TreatmentLogADO);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTongHopDonPhongKhamClick()
        {
            try
            {
                WaitingManager.Show();
                if (currentTreatment != null)
                {
                    //List<object> listArgs = new List<object>();
                    //listArgs.Add(currentTreatment);
                    //HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ExpMestAggrExam", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    //HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + currentTreatment.TREATMENT_CODE, currentTreatment.TREATMENT_CODE + " - " + currentTreatment.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestAggrExam").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.ExpMestAggrExam");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                        listArgs.Add(currentTreatment);
                        //listArgs.Add((DelegateSelectData)ReLoadServiceReq);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + currentTreatment.TREATMENT_CODE, currentTreatment.TREATMENT_CODE + " - " + currentTreatment.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPatientUpdateClick()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                treatmentFilter.ID = this.currentTreatment.ID;
                V_HIS_TREATMENT treatment = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                if (treatment != null)
                {
                    //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                    //    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.PatientUpdate'");
                    //    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //    {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(treatment.PATIENT_ID);
                    listArgs.Add(treatment.ID);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //        ((Form)extenceInstance).ShowDialog();
                    //    }
                    //    else
                    //    {
                    //        MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //    }
                    //}
                    //WaitingManager.Hide();

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public void Refesh()
        //{
        //    try
        //    {
        //        btnRefresh_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnFinishtreatClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    if (status_ispause == 1)
                    {
                        MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentFinish'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();

                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    TreatmentLogADO.TreatmentId = currentTreatment.ID;
                    listArgs.Add(TreatmentLogADO);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentFinish", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOpenTreatClick()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (currentTreatment != null)
                {
                    short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    if (status_ispause != 1)
                    {
                        MessageManager.Show("Bệnh nhân chưa kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    string mess = null;

                    bool IsContinue = true;
                    if (HisConfigCFG.IsRequiredReasonWhenOpenTreatment)
                    {
                        frmReasonOpenTreatment frm = new frmReasonOpenTreatment(reason =>
                        {
                            if (!String.IsNullOrEmpty(reason))
                            {
                                mess = reason;
                            }
                        }, IsClose =>
                        {
                            IsContinue = !IsClose;
                        }
                        );
                        frm.ShowDialog();
                    }
                    if (!IsContinue)
                        return;
                    WaitingManager.Show();
                    MOS.SDO.HisTreatmentUnfinishSDO sdo = new MOS.SDO.HisTreatmentUnfinishSDO();
                    sdo.Reason = mess;
                    sdo.treatmentId = currentTreatment.ID;
                    bool unFinishTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("/api/HisTreatment/Unfinish", ApiConsumers.MosConsumer, sdo, param);
                    //CloseTreatmentProcessor.TreatmentUnFinish(, param);
                    WaitingManager.Hide();
                    if (unFinishTreatment == true)
                    {
                        success = true;
                        FillDataToGrid();
                    }

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    //moduleData.RoomId = currentModule.RoomId;
                    //moduleData.RoomTypeId = currentModule.RoomTypeId;
                    listArgs.Add(currentTreatment.ID);
                    //    //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Bordereau", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnprintClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    if (currentTreatment.TREATMENT_END_TYPE_ID == null) MessageManager.Show("Không thể in. Bệnh nhân không có thông tin ra viện");
                    else if (currentTreatment.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
&& currentTreatment.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
&& currentTreatment.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
&& currentTreatment.TREATMENT_END_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)
                    {
                        MessageManager.Show(string.Format("Không thể in. Loại ra viện: {0} không có thông tin", currentTreatment.TREATMENT_END_TYPE_NAME));
                    }
                    else
                    {
                        if (barManagerPrint == null)
                        {
                            barManagerPrint = new DevExpress.XtraBars.BarManager();
                        }
                        barManagerPrint.Form = this;
                        LoadPrintTreatment(barManagerPrint);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBornInfoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfantInformation").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.InfantInformation");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentTreatment.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeathInfoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDeathInfo").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisDeathInfo'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)BtnSearch);
                    listArgs.Add(currentTreatment.ID);
                    //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //} 
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisDeathInfo", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTranPatiOutInfoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTranPatiOutInfo").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisTranPatiOutInfo'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)BtnSearch);
                    listArgs.Add(currentTreatment.ID);

                    //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTranPatiOutInfo", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTranPatiInInfoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTranPatiToInfo").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisTranPatiToInfo'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)BtnSearch);
                    listArgs.Add(currentTreatment.ID);

                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTranPatiToInfo", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAppointmentServiceClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    //short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    //if (status_ispause == 1)
                    //{
                    //    MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                    //    return;
                    //}
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AppointmentService", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void HoSoGiayToDinhKemClick()
        {
            try
            {
                WaitingManager.Hide();
                if (this.currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentFile").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTreatmentFile");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatment.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }



                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnAppointmentInfoClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment);
                    listArgs.Add((RefeshReference)BtnSearch);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AppointmentInfo", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignPanClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentTreatment.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnAssignServiceClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    if (status_ispause == 1)
                    {
                        MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignService'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    AssignServiceADO AssignServiceADO = new AssignServiceADO(currentTreatment.ID, 0, 0);
                    AssignServiceADO.TreatmentId = currentTreatment.ID;
                    AssignServiceADO.PatientDob = currentTreatment.TDL_PATIENT_DOB;
                    AssignServiceADO.PatientName = currentTreatment.TDL_PATIENT_NAME;
                    AssignServiceADO.GenderName = currentTreatment.TDL_PATIENT_GENDER_NAME;
                    AssignServiceADO.IsAutoEnableEmergency = true;
                    listArgs.Add(AssignServiceADO);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignService", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnComminBedClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    if (status_ispause == 1)
                    {
                        MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisBedRoomIn'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    listArgs.Add(currentTreatment.ID);
                    //listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisBedRoomIn", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnfixTreatmentClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentIcdEdit").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentIcdEdit'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)BtnSearch);
                    listArgs.Add(currentTreatment.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentIcdEdit", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnViewPackgeClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServicePackageView").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServicePackageView'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServicePackageView", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnpatientInfClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SummaryInforTreatmentRecords'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    listArgs.Add(currentTreatment.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.SummaryInforTreatmentRecords", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMargePatientClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentPatientUpdate").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentPatientUpdate'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();

                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    TreatmentLogADO.TreatmentId = currentTreatment.ID;
                    listArgs.Add(currentTreatment.ID);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentPatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    btnFind_Click(null, null);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSarprintListClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "SAR.Desktop.Plugins.SarPrintList").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'SAR.Desktop.Plugins.SarPrintList'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    SarPrintADO sarPrintADO = new SarPrintADO();
                    sarPrintADO.JSON_PRINT_ID = currentTreatment.JSON_PRINT_ID;
                    listArgs.Add(sarPrintADO);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.SarPrintList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHistoryTreatClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentHistory'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    TreatmentHistoryADO currentInput = new TreatmentHistoryADO();
                    currentInput.treatmentId = currentTreatment.ID;
                    currentInput.treatment_code = currentTreatment.TREATMENT_CODE;
                    listArgs.Add(currentInput);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentHistory", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFeehopClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    listArgs.Add(currentTreatment.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AggrHospitalFees", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRequestDeposit()
        {
            try
            {
                if (currentTreatment != null)
                {
                    //short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((currentTreatment.IS_PAUSE ?? -1).ToString());
                    //if (status_ispause == 1)
                    //{
                    //    MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                    //    return;
                    //}
                    if (!IsStayingDepartment(currentTreatment.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.RequestDeposit", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnMedicalAssessment()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisMedicalAssessment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnHivTreatment()
        {
            try
            {
                if (currentTreatment != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, this.currentTreatment);
                    listArgs.Add(treatment);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisHivTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTuberclusisTreatmentClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTuberclusisTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTuberclusisTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentTreatment.ID);
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

        private void btnTimelineTestClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentLog").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentLog'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();
                    //Set data to assignBloodADO
                    TreatmentLogADO.TreatmentId = currentTreatment.ID;
                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    listArgs.Add(TreatmentLogADO);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPublicMedicineByPhasedClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PublicMedicineByPhased", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHisAdrClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAdrList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.HisAdrList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                        listArgs.Add(currentTreatment.ID);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAllergyCardClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AllergyCard", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDongboEMRClick()
        {

            try
            {
                CommonParam param = new CommonParam();
                bool result = false;
                if (currentTreatment != null)
                {
                    result = new BackendAdapter(param).Post<bool>("api/HisTreatment/UploadEmr", ApiConsumers.MosConsumer, currentTreatment.ID, param);
                }
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnBedHistoryClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ListSurgMisuByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMRRegulationslistClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    MRSummaryDetailADO inputADO = new MRSummaryDetailADO();
                    inputADO.TreatmentId = this.currentTreatment.ID;
                    inputADO.processType = MRSummaryDetailADO.OpenFrom.TreatmentList;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(inputADO);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MRSummaryList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDebateDiagnosticClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Debate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCareSlipListClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(false);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisCareSum", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMediReactSumClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MediReactSum", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAccidentHurtClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(true);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AccidentHurt", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInfusionSumByTreatmentClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(false);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.InfusionSumByTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBloodTransfusionClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.TREATMENT_CODE);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BloodTransfusion", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSendOldSystemIntegrationClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatment/SendToOldSystem", ApiConsumers.MosConsumer, this.currentTreatment.ID, param);
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, rs);

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckingTreatmentEmr()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTreatmentRecordChecking", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuDangKySuDungThuocDVKTNgoaiBHYTClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(this.currentTreatment.ID, this.currentTreatment.PATIENT_ID, HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000402_PHIEU_DKSD_THUOC_DVKT_NGOAI_BHYT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BanGiaoBNTruocPTTT()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (room != null)
                    {
                        ado.DepartmentId = room.DEPARTMENT_ID;
                    }
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000404_BAN_GIAO_BN_TRUOC_PTTT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChanDoanNguyenNhanTuVong()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000476_CHAN_DOAN_NGUYEN_NHAN_TU_VONG);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuXNDuongMauMaoMach()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    ado.RoomId = this.currentModule.RoomId;
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (room != null)
                    {
                        ado.DepartmentId = room.DEPARTMENT_ID;
                    }
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000406_PHIEU_XN_DUONG_MAU_MAO_MACH);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuSangLocDinhDuongNguoiBenh()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (room != null)
                    {
                        ado.DepartmentId = room.DEPARTMENT_ID;
                    }
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000409_PHIEU_SANG_LOC_DINH_DUONG_NGUOI_BENH);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BenhAnNgoaiTruDayMatClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    ado.PatientId = this.currentTreatment.PATIENT_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000418_BENH_AN_NGOAI_TRU_DAY_MAT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BenhAnNgoaiTruGlaucomaClick()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.currentTreatment.ID;
                    ado.PatientId = this.currentTreatment.PATIENT_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000419_BENH_AN_NGOAI_TRU_GLAUCOMA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DeathDiagnosisClick()
        {
            try
            {

                var severeIllnessInfo = GetSevereIllnessInfo(currentTreatment.ID);
                HIS_TREATMENT treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, this.currentTreatment);

                CauseOfDeathADO causeOfDeathADO = new CauseOfDeathADO();
                causeOfDeathADO.Treatment = treatment;
                if (severeIllnessInfo != null)
                {
                    causeOfDeathADO.SevereIllNessInfo = severeIllnessInfo;
                    causeOfDeathADO.ListEventsCausesDeath = GetListEventsCausesDeath(severeIllnessInfo.ID);
                }
                frmCauseOfDeath frm = new frmCauseOfDeath(causeOfDeathADO, true,this.currentModule);
                frm.ShowDialog();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_EVENTS_CAUSES_DEATH> GetListEventsCausesDeath(long _SsIllnessInfoId)
        {
            List<HIS_EVENTS_CAUSES_DEATH> rs = null;
            try
            {
                HisEventsCausesDeathFilter ft = new HisEventsCausesDeathFilter();
                ft.SEVERE_ILLNESS_INFO_ID = _SsIllnessInfoId;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, ft, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private HIS_SEVERE_ILLNESS_INFO GetSevereIllnessInfo(long _TreatmentId)
        {
            HIS_SEVERE_ILLNESS_INFO rs = null;
            try
            {
                HisSevereIllnessInfoFilter ft = new HisSevereIllnessInfoFilter();
                ft.TREATMENT_ID = _TreatmentId;
                ft.IS_DEATH = true;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void SevereillnessClick()
        {
            try
            {
                if (currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    listArgs.Add(false);
                    listArgs.Add(currentModule);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.InformationAllowGoHome", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    throw new ArgumentNullException("Treatment is null");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
