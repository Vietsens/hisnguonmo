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
using DevExpress.XtraBars;
using EMR_MAIN;
using EMR_MAIN.DATABASE.BenhAn;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using HIS.Desktop.Plugins.BedRoomPartial.Popup;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class UCBedRoomPartial : UserControlBase
    {

        void BedRoomMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.treatmentBedRoomRow != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    BedRoomPopupMenuProcessor.ModuleType type = (BedRoomPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case BedRoomPopupMenuProcessor.ModuleType.GiayNamVien:
                            btnGiayNamVienClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisPhieuSoKetTruocMo:
                            btnPhieuSoKetTruocMoClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisSoKetBenhAnTruocPhauThuat:
                            btnSoKetBenhAnTruocPhauThuatClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisSoKetBenhAnTruocThuThuat:
                            btnSoKetBenhAnTruocThuThuatClick();
                            break;
                        #region ...
                        case BedRoomPopupMenuProcessor.ModuleType.Care:
                            btnCareClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.SummaryInforTreatment:
                            btnSummaryInforTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.btnBedRoomIn:
                            btnBedRoomInClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.btnBedRoomOut:
                            btnBedRoomOutClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AssignService:
                            btnAssignServiceClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AssignService__Plus:
                            btnAssignPrescriptionClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AssignBlood:
                            btnAssignBloodClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TreatmentPatientUpdate:
                            btnTreatmentPatientUpdateClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.CareSlipList:
                            btnCareSlipListClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TranPatiToInfo:
                            btnTranPatiToInfoClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.DebateDiagnostic:
                            btnDebateDiagnosticClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TaoBienBanHoiChan:
                            btnTaoBienBanHoiChanClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.MedicalInfomation:
                            btnMedicalInfomationClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TrackingTreatment:
                            btnTrackingTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.InfusionSumByTreatment:
                            btnInfusionSumByTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AccidentHurt:
                            btnAccidentHurtClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.MediReactSum:
                            btnMediReactSumClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.BedHistory:
                            btnBedHistoryClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.InfantInformation:
                            btnInfantInformationClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TreatmentHistory:
                            btnTreatmentHistoryClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TreatmentFinish:
                            btnTreatmentFinishClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TransDepartment:
                            btnTransDepartmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.RequestDeposit:
                            btnRequestDepositClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.EmrDocument:
                            btnEmrDocumentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.Phieuchamsoc_vobenhan:
                            btnPhieuChamSocVobClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.Bordereau:
                            btnBordereauClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.CanTheoDoi:
                            btnCanTheoDoi();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.BoTheoDoi:
                            btnBoTheoDoi();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.MedicinMaterialIUsed:
                            btnMedicinMaterialIUsedClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AggrHospitalFees:
                            btnAggrHospitalFeesClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.ServiceReqList:
                            btnServiceReqListClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.SumaryTestResults:
                            btnSumaryTestResultsClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PublicMedicineByPhased:
                            btnPublicMedicineByPhasedClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PublicMedicineByDate:
                            btnPublicMedicineByDateClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.MedicalAssessment:
                            btnMedicalAssessmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TuberclusisTreatment:
                            btnTuberclusisTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HivTreatment:
                            btnHivTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PublicService_NT:
                            btnPublicService_NTClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.OtherForms:
                            btnOtherFormsClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.ServicePackageView:
                            btnServicePackageViewClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TuTruc:
                            btnTuTrucClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AssignPaan:
                            btnAssignPaanClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.SuaHSDT:
                            btnEditICD();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.BacSiDt:
                            btnBacSiDt();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.Specialty:
                            btnSpecialty();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.Consultation:
                            btnConsultation();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisCoTreatmentCreate:
                            btnHisCoTreatmentCreate();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisCoTreatmentFinish:
                            btnHisCoTreatmentFinish();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TreatmentList:
                            btnTreatmentListByTreatmentCode();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisAdr:
                            btnHisAdr();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AllergyCard:
                            btnAllergyCard();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.AssignNutrition:
                            btnAssignNutrition();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.RationSchedule:
                            btnRationSchedule();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.BloodTransfusion:
                            btnBloodTransfusion();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.OtherFormAssTreatment:
                            btnOtherFormAssTreatmentClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisDhst:
                            btnHisDhstClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PREPARE:
                            btnPREPAREClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisDhstChart:
                            btnHisDhstChartClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.CheckInfoBHYT:
                            btnCheckInfoBHYTClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.TreatmentLog:
                            btnDongThoiGianClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.ApprovalFinish:
                            btnApprovalFinishClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.DisApprovalFinish:
                            btnDisApprovalFinishClick();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType._GiayTHXN:
                            btnPrintGiayTHXN();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.CheckingTreatmentEmr:
                            CheckingTreatmentEmr();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.CamKetNamGiuongTuNguyen:
                            CamKetNamGiuongTuNguyen();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.BanGiaoBNTruocPTTT:
                            BanGiaoBNTruocPTTT();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PhieuKhaiThacTienSuDiUng:
                            PhieuKhaiThacTienSuDiUng();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PhieuXNDuongMauMaoMach:
                            PhieuXNDuongMauMaoMach();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.HisTreatmentFile:
                            HoSoGiayTo();
                            break;
                        case BedRoomPopupMenuProcessor.ModuleType.PhanLoaiBenhNhan:
                            PhanLoaiBenhNhan();
                            break;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBacSiDt()
        {
            frmDoctorTreatment frmDoctorTreatment = new frmDoctorTreatment(treatmentBedRoomRow, (DelegateRefreshData)RefreshData);
            frmDoctorTreatment.ShowDialog();
        }

        private void btnSpecialty()
        {
            frmSpecialty frmSpecialty = new frmSpecialty(treatmentBedRoomRow, currentModule);
            frmSpecialty.ShowDialog();
        }

        private void btnConsultation()
        {
            frmConsultation frmConsultation = new frmConsultation(treatmentBedRoomRow, currentModule);
            frmConsultation.ShowDialog();
        }

        public void RefreshData()
        {
            try
            {
                FillDataToGridTreatmentBedRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HoSoGiayTo()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentFile").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTreatmentFile");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void PhanLoaiBenhNhan()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.UpdatePatientClassify").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.UpdatePatientClassify");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentBedRoomLViewFilter filter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        filter.ID = treatmentBedRoomRow.ID;
                        var ntreatmentBedRoomRow = new BackendAdapter(param).Get<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/getLView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                        if (ntreatmentBedRoomRow != null)
                            listArgs.Add(ntreatmentBedRoomRow);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnMedicinMaterialIUsedClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    WaitingManager.Show();
                    treatmentCode = this.treatmentBedRoomRow.TREATMENT_CODE;
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MedicineIsUsedPatient").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MedicineIsUsedPatient");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentCode);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnSoKetBenhAnTruocThuThuatClick()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000417_SO_KET_BENH_AN_TRUOC_THU_THUAT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSoKetBenhAnTruocPhauThuatClick()
        {
            try
            {
                var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;

                var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000416_SO_KET_BENH_AN_TRUOC_PHAU_THUAT);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintGiayTHXN()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    var treatment = new V_HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(treatment, treatmentBedRoomRow);
                    treatment.ID = treatmentBedRoomRow.TREATMENT_ID;
                    var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(wkRoomId, treatment);
                    if (printTest != null)
                    {
                        printTest.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDongThoiGianClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    List<object> listArgs = new List<object>();
                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();
                    TreatmentLogADO.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                    TreatmentLogADO.RoomId = wkRoomId;
                    listArgs.Add(TreatmentLogADO);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentLog", this.wkRoomId, this.wkRoomTypeId, listArgs);
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
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CheckInfoBHYT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CheckInfoBHYT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnPREPAREClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Prepare").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Prepare");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS_TREATMENT treatment = new HIS_TREATMENT();
                        treatment.ID = this.treatmentBedRoomRow.TREATMENT_ID;
                        listArgs.Add(treatment);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnHisDhstChartClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDhstChart").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDhstChart");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnHisDhstClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDhst").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDhst");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnOtherFormAssTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.OtherFormAssTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.OtherFormAssTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnBloodTransfusion()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BloodTransfusion").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.BloodTransfusion");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);

                        listArgs.Add(treatmentBedRoomRow.TREATMENT_CODE);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
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

        private void btnAssignNutrition()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignNutrition").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.AssignNutrition");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);

                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);

                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
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
        private void btnRationSchedule()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RationSchedule").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.RationSchedule");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);

                        listArgs.Add(treatmentBedRoomRow);

                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
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

        private void btnAllergyCard()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AllergyCard").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.AllergyCard");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
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

        private void btnHisAdr()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAdrList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.HisAdrList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
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

        private void btnTreatmentListByTreatmentCode()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + "HIS.Desktop.Plugins.TreatmentList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_CODE);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + treatmentBedRoomRow.TREATMENT_CODE, currentModule.text, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHisCoTreatmentFinish()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisCoTreatmentFinish").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisCoTreatmentFinish");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.CO_TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        listArgs.Add((DelegateSelectData)Refesh);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnHisCoTreatmentCreate()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisCoTreatmentCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisCoTreatmentCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        listArgs.Add((DelegateSelectData)Refesh);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnGiayNamVienClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    PrintProcessGiayChungNhan(PrintType.IN_GIAY_CHUNG_NHAN_NAM_VIEN);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPhieuSoKetTruocMoClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    List<PhauThuatThuThuat_HIS> PhauThuatThuThuat_HISs = new List<PhauThuatThuThuat_HIS>();

                    #region --- Load
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter _patientFilter = new MOS.Filter.HisPatientViewFilter();
                    _patientFilter.ID = this.treatmentBedRoomRow.PATIENT_ID;
                    var currentPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/getView", ApiConsumers.MosConsumer, _patientFilter, param);
                    if (currentPatient == null || currentPatient.Count == 0)
                        throw new NullReferenceException("Khong lay duoc V_HIS_PATIENT bang ID" + this.treatmentBedRoomRow.PATIENT_ID);
                    V_HIS_PATIENT _Patient = currentPatient.FirstOrDefault();

                    param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter _treatmentViewFilter = new MOS.Filter.HisTreatmentViewFilter();
                    _treatmentViewFilter.ID = this.treatmentBedRoomRow.TREATMENT_ID;
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, _treatmentViewFilter, param);
                    var currentTreatment = treatments != null ? treatments.FirstOrDefault() : null;

                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetApplied", ApiConsumers.MosConsumer, filter, param);


                    HisDhstFilter _dhstFilter = new HisDhstFilter();
                    _dhstFilter.TREATMENT_ID = this.treatmentBedRoomRow.TREATMENT_ID;
                    _dhstFilter.ORDER_DIRECTION = "DESC";
                    _dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    var currentDhst = new BackendAdapter(param).Get<List<HIS_DHST>>("/api/HisDhst/Get", ApiConsumers.MosConsumer, _dhstFilter, param);
                    HIS_DHST _DHST = new HIS_DHST();
                    currentDhst = (currentDhst != null && currentDhst.Count > 0) ? currentDhst.Where(o => GetRoomTypeByRoomId(o.EXECUTE_ROOM_ID) == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList() : null;
                    if (currentDhst != null && currentDhst.Count > 0)
                    {
                        _DHST = currentDhst.FirstOrDefault();
                    }
                    MOS.Filter.HisBabyViewFilter _babyFIlter = new HisBabyViewFilter();
                    _babyFIlter.TREATMENT_ID = currentTreatment.ID;
                    var currentBaby = new BackendAdapter(param).Get<List<V_HIS_BABY>>("/api/HisBaby/GetView", ApiConsumers.MosConsumer, _babyFIlter, param);
                    V_HIS_BABY _Baby = new V_HIS_BABY();
                    if (currentBaby != null && currentBaby.Count > 0)
                    {
                        _Baby = currentBaby.FirstOrDefault();
                    }

                    MOS.Filter.HisDepartmentTranViewFilter _departmentTranFilter = new HisDepartmentTranViewFilter();
                    _departmentTranFilter.TREATMENT_ID = currentTreatment.ID;
                    _departmentTranFilter.ORDER_DIRECTION = "ASC";
                    _departmentTranFilter.ORDER_FIELD = "CREATE_TIME";
                    var _DepartmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("/api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, _departmentTranFilter, param);
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        _DepartmentTrans = _DepartmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).ToList();
                    }

                    MOS.Filter.HisServiceReqViewFilter _reqFilter = new HisServiceReqViewFilter();
                    _reqFilter.TREATMENT_ID = currentTreatment.ID;
                    _reqFilter.ORDER_DIRECTION = "DESC";
                    _reqFilter.ORDER_FIELD = "MODIFY_TIME";
                    var currentServiceReqs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("/api/HisServiceReq/GetView", ApiConsumers.MosConsumer, _reqFilter, param);
                    V_HIS_SERVICE_REQ _ExamServiceReq = new V_HIS_SERVICE_REQ();
                    if (currentServiceReqs != null && currentServiceReqs.Count > 0)
                    {
                        _ExamServiceReq = currentServiceReqs.FirstOrDefault(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
                    }

                    int treatmentCount = 1;
                    MOS.Filter.HisTreatmentFilter treatmentCountFilter = new HisTreatmentFilter();
                    treatmentCountFilter.PATIENT_ID = _Patient.ID;
                    var treatmentPatients = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentCountFilter, param);
                    if (treatmentPatients != null && treatmentPatients.Count > 0)
                    {
                        treatmentCount = treatmentPatients.Count;
                    }

                    MOS.Filter.HisSereServPtttViewFilter sereServPtttFilter = new HisSereServPtttViewFilter();
                    sereServPtttFilter.TDL_TREATMENT_ID = currentTreatment.ID;
                    sereServPtttFilter.ORDER_DIRECTION = "DESC";
                    sereServPtttFilter.ORDER_FIELD = "MODIFY_TIME";
                    var sereServPttts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("/api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, sereServPtttFilter, param);

                    var currentServiceReqIdCls = currentServiceReqs != null ? currentServiceReqs.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).Select(o => o.ID).ToList() : new List<long>();

                    MOS.Filter.HisSereServFilter sereServFilter = new HisSereServFilter();
                    sereServFilter.TREATMENT_ID = currentTreatment.ID;
                    sereServFilter.ORDER_DIRECTION = "DESC";
                    sereServFilter.ORDER_FIELD = "MODIFY_TIME";
                    var sereServAlls = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);

                    var sereServCLSs = (sereServAlls != null && sereServAlls.Count > 0) ? sereServAlls.Where(o => (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                        && currentServiceReqIdCls.Contains(o.SERVICE_REQ_ID ?? 0)).ToList() : null;

                    #endregion
                    #region ------- HanhChinhBenhNhan
                    HanhChinhBenhNhan _HanhChinhBenhNhan = new HanhChinhBenhNhan();
                    _HanhChinhBenhNhan.SoYTe = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).PARENT_ORGANIZATION_NAME;
                    _HanhChinhBenhNhan.BenhVien = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(p => p.ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId).BRANCH_NAME;
                    _HanhChinhBenhNhan.MaYTe = "";
                    _HanhChinhBenhNhan.MaBenhNhan = _Patient.PATIENT_CODE;
                    _HanhChinhBenhNhan.TenBenhNhan = currentTreatment.TDL_PATIENT_NAME;
                    _HanhChinhBenhNhan.NgaySinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                    _HanhChinhBenhNhan.Tuoi = MPS.AgeUtil.CalculateFullAge(currentTreatment.TDL_PATIENT_DOB);
                    _HanhChinhBenhNhan.GioiTinh = currentTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? GioiTinh.Nam : currentTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? GioiTinh.Nu : GioiTinh.ChuaXacDinh;
                    _HanhChinhBenhNhan.NgheNghiep = _Patient.CAREER_NAME;
                    _HanhChinhBenhNhan.MaNgheNghiep = _Patient.CAREER_CODE;
                    _HanhChinhBenhNhan.DanToc = _Patient.ETHNIC_NAME;
                    _HanhChinhBenhNhan.MaDanhToc = _Patient.ETHNIC_CODE;
                    _HanhChinhBenhNhan.NgoaiKieu = _Patient.NATIONAL_NAME;
                    _HanhChinhBenhNhan.MaNgoaiKieu = _Patient.NATIONAL_CODE;
                    var mitiRank = _Patient.MILITARY_RANK_ID > 0 ? BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.ID == _Patient.MILITARY_RANK_ID).FirstOrDefault() : null;
                    _HanhChinhBenhNhan.CapBac = mitiRank != null ? mitiRank.MILITARY_RANK_NAME : "";
                    var branchPatient = _Patient.BRANCH_ID > 0 ? BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == _Patient.BRANCH_ID).FirstOrDefault() : null;
                    _HanhChinhBenhNhan.DonVi = branchPatient != null ? branchPatient.BRANCH_NAME : "";

                    if (String.IsNullOrEmpty(_Patient.COMMUNE_NAME) && String.IsNullOrEmpty(_Patient.DISTRICT_NAME) && String.IsNullOrEmpty(_Patient.PROVINCE_NAME))
                    {
                        _HanhChinhBenhNhan.SoNha = _Patient.VIR_ADDRESS;
                    }
                    else
                    {
                        _HanhChinhBenhNhan.SoNha = _Patient.ADDRESS;
                        _HanhChinhBenhNhan.ThonPho = "";
                        _HanhChinhBenhNhan.XaPhuong = _Patient.COMMUNE_NAME;
                        _HanhChinhBenhNhan.HuyenQuan = _Patient.DISTRICT_NAME;
                        _HanhChinhBenhNhan.MaHuyenQuan = _Patient.DISTRICT_CODE;
                        _HanhChinhBenhNhan.TinhThanhPho = _Patient.PROVINCE_NAME;
                        _HanhChinhBenhNhan.MaTinhThanhPho = _Patient.PROVINCE_CODE;
                    }

                    _HanhChinhBenhNhan.NoiLamViec = _Patient.WORK_PLACE;
                    _HanhChinhBenhNhan.DoiTuong = _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT ? DoiTuong.BHYT : _PatientTypeAlter.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__VP ? DoiTuong.ThuPhi : DoiTuong.Khac;
                    if (_PatientTypeAlter.HEIN_CARD_FROM_TIME > 0)
                        _HanhChinhBenhNhan.NgayDangKyBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) ?? null;
                    if (_PatientTypeAlter.HEIN_CARD_TO_TIME > 0)
                        _HanhChinhBenhNhan.NgayHetHanBHYT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) ?? null;
                    _HanhChinhBenhNhan.SoTheBHYT = _PatientTypeAlter.HEIN_CARD_NUMBER;
                    _HanhChinhBenhNhan.TenNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_NAME;
                    _HanhChinhBenhNhan.MaNoiDangKyBHYT = _PatientTypeAlter.HEIN_MEDI_ORG_CODE;
                    _HanhChinhBenhNhan.NgayDuocHuong5Nam = _PatientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_PatientTypeAlter.JOIN_5_YEAR_TIME ?? 0) : null;
                    _HanhChinhBenhNhan.HoTenDiaChiNguoiNha = _Patient.RELATIVE_NAME + " - " + _Patient.RELATIVE_ADDRESS;
                    #endregion

                    #region ------- ThongTinDieuTri
                    ThongTinDieuTri _ThongTinDieuTri = new ThongTinDieuTri();
                    if (_Baby != null)
                    {
                        _ThongTinDieuTri.LucVaoDe = "";
                        _ThongTinDieuTri.NgayDe = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_Baby.BORN_TIME ?? 0);
                        _ThongTinDieuTri.NgoiThai = _Baby.BORN_POSITION_NAME;
                        _ThongTinDieuTri.CachThucDe = _Baby.BORN_TYPE_NAME;
                        _ThongTinDieuTri.KiemSoatTuCung = false;
                        _ThongTinDieuTri.KiemSoatTuCung_Text = "";
                        _ThongTinDieuTri.TreSoSinh_LoaiThai = -1;
                        _ThongTinDieuTri.TreSoSinh_GioiTinh = _Baby.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? 1 : 0;
                        _ThongTinDieuTri.TreSoSinh_DiTat = 0;
                        _ThongTinDieuTri.TreSoSinh_DiTat_Text = "";
                        _ThongTinDieuTri.TreSoSinh_CanNang = (int?)_Baby.WEIGHT;
                        _ThongTinDieuTri.TreSoSinh_SongChet = (String.IsNullOrEmpty(_Baby.BORN_RESULT_CODE) || _Baby.BORN_RESULT_CODE == "01") ? 1 : 0;
                    }

                    _ThongTinDieuTri.NguyenNhan_BenhChinh_RaVien = currentTreatment.ICD_NAME;
                    if (currentTreatment.END_DEPARTMENT_ID > 0)
                    {
                        var dpEnd = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == currentTreatment.END_DEPARTMENT_ID).FirstOrDefault();
                        _ThongTinDieuTri.KhoaRaVien = dpEnd != null ? dpEnd.DEPARTMENT_NAME : "";
                    }

                    _ThongTinDieuTri.MaBenhAn = currentTreatment.TREATMENT_CODE;
                    _ThongTinDieuTri.GiuongRaVien = "";
                    _ThongTinDieuTri.SoLuuTru = currentTreatment.STORE_CODE;
                    _ThongTinDieuTri.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(currentTreatment.TREATMENT_CODE);//Mỗi lần vào điều trị có cái mã
                    _ThongTinDieuTri.MaBenhNhan = _Patient.PATIENT_CODE;
                    _ThongTinDieuTri.NgayVaoVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.IN_TIME) ?? null;
                    _ThongTinDieuTri.TrucTiepVao = (_ExamServiceReq != null && _ExamServiceReq.IS_EMERGENCY == 1) ? TrucTiepVao.CapCuu : _PatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM ? TrucTiepVao.KKB : TrucTiepVao.KhoaDieuTri;
                    _ThongTinDieuTri.NoiGioiThieu = (!String.IsNullOrEmpty(currentTreatment.TRANSFER_IN_MEDI_ORG_NAME) && !String.IsNullOrEmpty(currentTreatment.TRANSFER_IN_MEDI_ORG_CODE)) ? NoiGioiThieu.CoQuanYTe : NoiGioiThieu.TuDen;


                    var departmentIdClss = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1).Select(o => o.ID).ToArray();
                    _DepartmentTrans = (_DepartmentTrans != null && _DepartmentTrans.Count > 0) ? _DepartmentTrans.Where(o => departmentIdClss.Contains(o.DEPARTMENT_ID)).ToList() : null;
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        _DepartmentTrans = _DepartmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();

                        _ThongTinDieuTri.TenKhoaVao = _DepartmentTrans[0].DEPARTMENT_NAME;
                        _ThongTinDieuTri.NgayVaoKhoa = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0) ?? null;
                        if (_DepartmentTrans.Count > 1)
                        {
                            long? songay = null;
                            if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                            }
                            else
                            {
                                songay = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[0].DEPARTMENT_IN_TIME, _DepartmentTrans[1].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                            }

                            _ThongTinDieuTri.SoNgayDieuTriTaiKhoa = Inventec.Common.TypeConvert.Parse.ToInt32(songay.ToString());

                            _ThongTinDieuTri.ChuyenKhoa1 = _DepartmentTrans[1].DEPARTMENT_NAME;
                            _ThongTinDieuTri.NgayChuyenKhoa1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[1].DEPARTMENT_IN_TIME ?? 0) ?? null;
                            if (_DepartmentTrans.Count > 2)
                            {
                                long? songay1 = null;
                                if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                }
                                else
                                {
                                    songay1 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[1].DEPARTMENT_IN_TIME, _DepartmentTrans[2].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                }

                                _ThongTinDieuTri.SoNgayDieuTriKhoa1 = Inventec.Common.TypeConvert.Parse.ToInt32(songay1.ToString());

                                _ThongTinDieuTri.ChuyenKhoa2 = _DepartmentTrans[2].DEPARTMENT_NAME;
                                _ThongTinDieuTri.NgayChuyenKhoa2 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[2].DEPARTMENT_IN_TIME ?? 0) ?? null;
                                if (_DepartmentTrans.Count > 3)
                                {
                                    long? songay2 = null;
                                    if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    {
                                        songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                    }
                                    else
                                    {
                                        songay2 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[2].DEPARTMENT_IN_TIME, _DepartmentTrans[3].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                    }
                                    _ThongTinDieuTri.SoNgayDieuTriKhoa2 = Inventec.Common.TypeConvert.Parse.ToInt32(songay2.ToString());
                                    _ThongTinDieuTri.ChuyenKhoa3 = _DepartmentTrans[3].DEPARTMENT_NAME;
                                    _ThongTinDieuTri.NgayChuyenKhoa3 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_DepartmentTrans[3].DEPARTMENT_IN_TIME ?? 0) ?? null;

                                    if (_DepartmentTrans.Count > 4)
                                    {
                                        long? songay3 = null;
                                        if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                        {
                                            songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                                        }
                                        else
                                        {
                                            songay3 = HIS.Common.Treatment.Calculation.DayOfTreatment(_DepartmentTrans[3].DEPARTMENT_IN_TIME, _DepartmentTrans[4].DEPARTMENT_IN_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                                        }

                                        _ThongTinDieuTri.SoNgayDieuTriKhoa3 = Inventec.Common.TypeConvert.Parse.ToInt32(songay3.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (currentTreatment.TRAN_PATI_FORM_ID > 0)
                        _ThongTinDieuTri.ChuyenVien = GetChuyenVienFromTranspatiForm(currentTreatment.TRAN_PATI_FORM_ID);
                    _ThongTinDieuTri.TenVienChuyenBenhNhanDen = currentTreatment.MEDI_ORG_NAME;
                    _ThongTinDieuTri.MaVienChuyenBenhNhanDen = currentTreatment.MEDI_ORG_CODE;
                    if (currentTreatment.OUT_TIME > 0)
                        _ThongTinDieuTri.NgayRaVien = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.OUT_TIME ?? 0) ?? null;
                    if (currentTreatment.TREATMENT_END_TYPE_ID > 0)
                        _ThongTinDieuTri.TinhTrangRaVien = currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN ? TinhTrangRaVien.RaVien : currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON ? TinhTrangRaVien.BoVe : currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN ? TinhTrangRaVien.XinVe : TinhTrangRaVien.DuaVe;
                    long? _snDieuTri = null;
                    if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(currentTreatment.CLINICAL_IN_TIME, currentTreatment.OUT_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                    }
                    else
                    {
                        _snDieuTri = HIS.Common.Treatment.Calculation.DayOfTreatment(currentTreatment.CLINICAL_IN_TIME, currentTreatment.OUT_TIME, currentTreatment.TREATMENT_END_TYPE_ID, currentTreatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                    }
                    _ThongTinDieuTri.TongSoNgayDieuTri1 = _snDieuTri.ToString();
                    _ThongTinDieuTri.TongSoNgayDieuTri2 = _snDieuTri.ToString();
                    _ThongTinDieuTri.ChanDoan_NoiChuyenDen = currentTreatment.IN_ICD_NAME;
                    _ThongTinDieuTri.MaICD_NoiChuyenDen = currentTreatment.IN_ICD_CODE;
                    _ThongTinDieuTri.ChanDoan_KKB_CapCuu = "";
                    _ThongTinDieuTri.MaICD_KKB_CapCuu = "";

                    //Lấy chẩn đoán chính
                    if (_ExamServiceReq != null)
                    {
                        _ThongTinDieuTri.ChanDoan_KhiVaoKhoaDieuTri = _ExamServiceReq.ICD_NAME;
                        _ThongTinDieuTri.MaICD_KhiVaoKhoaDieuTri = _ExamServiceReq.ICD_CODE;
                    }

                    _ThongTinDieuTri.BenhChinh_RaVien = currentTreatment.ICD_NAME;
                    _ThongTinDieuTri.MaICD_BenhChinh_RaVien = currentTreatment.ICD_CODE;
                    _ThongTinDieuTri.BenhKemTheo_RaVien = currentTreatment.ICD_TEXT;
                    _ThongTinDieuTri.MaICD_BenhKemTheo_RaVien = currentTreatment.ICD_SUB_CODE;

                    _ThongTinDieuTri.VaoVienDoBenhNayLanThu = treatmentCount;
                    if (sereServPttts != null && sereServPttts.Count > 0)
                    {
                        PhauThuatThuThuat_HISs = new List<PhauThuatThuThuat_HIS>();
                        var sereServIds = sereServPttts.Select(o => o.SERE_SERV_ID).ToList();
                        param = new CommonParam();
                        HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                        ssExtFilter.SERE_SERV_IDs = sereServIds;
                        var sereServExts = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, param);

                        var sereServByTreatments = sereServAlls.Where(o => sereServIds.Contains(o.ID)).ToList();
                        var ssInKipIds = sereServByTreatments.Where(o => o.EKIP_ID.HasValue).Select(o => o.EKIP_ID ?? 0).ToList();
                        param = new CommonParam();
                        HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                        var hisEkipUsers = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, hisEkipUserFilter, param);

                        param = new CommonParam();
                        HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                        var hisEkipPlanUsers = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                        foreach (var sspttt in sereServPttts)
                        {
                            HIS_SERE_SERV_EXT ssext = sereServExts != null && sereServExts.Count > 0 ? sereServExts.Where(o => o.SERE_SERV_ID == sspttt.SERE_SERV_ID).FirstOrDefault() : null;

                            DateTime? beginTime = (ssext != null && ssext.BEGIN_TIME.HasValue) ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ssext.BEGIN_TIME.Value).Value : DateTime.MinValue;

                            var ss = sereServAlls != null ? sereServAlls.Where(o => o.ID == sspttt.SERE_SERV_ID).FirstOrDefault() : null;
                            var serviceReq = ss != null ? currentServiceReqs.Where(o => o.ID == ss.SERVICE_REQ_ID).FirstOrDefault() : null;
                            if (serviceReq != null && ss != null && ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq") == "1" && ((ssext != null && ssext.BEGIN_TIME == null) || ssext == null))
                            {
                                beginTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                            }

                            DateTime datePttt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(beginTime.Value.ToString("yyyyMMdd") + "000000")).Value;
                            string bacSyPhauThuat = "", bacSyPhauThuatHoVaTen = "", bacSyGayMe = "", bacSyGayMeHoVaTen = "";
                            string executeRoleCode__PhauThuat = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.MAIN");//TODO
                            string executeRoleCode__GayMe = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.ANESTHETIST");//TODO
                            if (ss != null && ss.EKIP_ID.HasValue)
                            {
                                var ekipUsers = hisEkipUsers != null ? hisEkipUsers.Where(o => o.EKIP_ID == ss.EKIP_ID).ToList() : null;
                                if (ekipUsers != null && ekipUsers.Count > 0)
                                {
                                    var ekipUserBSPhauThuat = ekipUsers.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__PhauThuat).FirstOrDefault();
                                    var ekipUserBSGatMe = ekipUsers.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__GayMe).FirstOrDefault();
                                    bacSyPhauThuat = ekipUserBSPhauThuat != null ? ekipUserBSPhauThuat.LOGINNAME : "";
                                    bacSyPhauThuatHoVaTen = ekipUserBSPhauThuat != null ? ekipUserBSPhauThuat.USERNAME : "";
                                    bacSyGayMe = ekipUserBSGatMe != null ? ekipUserBSGatMe.LOGINNAME : "";
                                    bacSyGayMeHoVaTen = ekipUserBSGatMe != null ? ekipUserBSGatMe.USERNAME : "";
                                }
                            }
                            else if (serviceReq != null && serviceReq.EKIP_PLAN_ID.HasValue)
                            {
                                var ekipPlans = hisEkipPlanUsers != null ? hisEkipPlanUsers.Where(o => o.EKIP_PLAN_ID == serviceReq.EKIP_PLAN_ID).ToList() : null;
                                if (ekipPlans != null && ekipPlans.Count > 0)
                                {
                                    var ekipUserPlanBSPhauThuat = ekipPlans != null ? ekipPlans.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__PhauThuat).FirstOrDefault() : null;
                                    var ekipUserPlanBSGatMe = ekipPlans != null ? ekipPlans.Where(o => o.EXECUTE_ROLE_CODE == executeRoleCode__GayMe).FirstOrDefault() : null;
                                    bacSyPhauThuat = ekipUserPlanBSPhauThuat != null ? ekipUserPlanBSPhauThuat.LOGINNAME : "";
                                    bacSyPhauThuatHoVaTen = ekipUserPlanBSPhauThuat != null ? ekipUserPlanBSPhauThuat.USERNAME : "";
                                    bacSyGayMe = ekipUserPlanBSGatMe != null ? ekipUserPlanBSGatMe.LOGINNAME : "";
                                    bacSyGayMeHoVaTen = ekipUserPlanBSGatMe != null ? ekipUserPlanBSGatMe.USERNAME : "";
                                }
                            }
                            PhauThuatThuThuat_HISs.Add(new PhauThuatThuThuat_HIS()
                            {
                                PhuongPhapPhauThuatThuThuat = sspttt.PTTT_METHOD_NAME,
                                PhuongPhapVoCam = sspttt.EMOTIONLESS_METHOD_NAME,
                                NgayPhauThuatThuThuat = datePttt,
                                NgayPhauThuatThuThuat_Gio = beginTime,
                                BacSyPhauThuat = bacSyPhauThuat,
                                BacSyPhauThuatHoVaTen = bacSyPhauThuatHoVaTen,
                                BacSyGayMe = bacSyGayMe,
                                BacSyGayMeHoVaTen = bacSyGayMeHoVaTen
                            });
                        }
                        PhauThuatThuThuat_HISs = PhauThuatThuThuat_HISs.OrderBy(o => o.NgayPhauThuatThuThuat).ToList();

                        _ThongTinDieuTri.TongSoLanPhauThuat = sereServPttts.Count;//TODO
                        _ThongTinDieuTri.TongSoNgayDieuTriSauPT = null;//TODO
                        _ThongTinDieuTri.LyDoTaiBienBienChung = null;//TODO
                        _ThongTinDieuTri.MaICD_ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_CODE;
                        _ThongTinDieuTri.MaICD_ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_CODE;
                        _ThongTinDieuTri.MaICD_NguyenNhan_BenhChinh_RV = sereServPttts[0].ICD_CODE;
                        _ThongTinDieuTri.ChanDoanSauPhauThuat = sereServPttts[0].AFTER_PTTT_ICD_NAME;
                        _ThongTinDieuTri.ChanDoanTruocPhauThuat = sereServPttts[0].BEFORE_PTTT_ICD_NAME;
                        _ThongTinDieuTri.TaiBien = sereServPttts.Any(o => o.PTTT_CATASTROPHE_ID > 0);
                    }

                    if (currentTreatment.TREATMENT_RESULT_ID > 0)
                        _ThongTinDieuTri.KetQuaDieuTri = currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__CHET ? KetQuaDieuTri.TuVong : currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO ? KetQuaDieuTri.GiamDo : currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI ? KetQuaDieuTri.Khoi : currentTreatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG ? KetQuaDieuTri.NangHon : KetQuaDieuTri.KhongThayDoi;
                    _ThongTinDieuTri.GiaiPhauBenh = 0;
                    if (currentTreatment.DEATH_TIME > 0)
                        _ThongTinDieuTri.NgayTuVong = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.DEATH_TIME ?? 0);
                    if (currentTreatment.DEATH_CAUSE_ID > 0)
                    {
                        var deathCause = BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.ID == currentTreatment.DEATH_CAUSE_ID).FirstOrDefault();
                        if (deathCause != null)
                        {
                            _ThongTinDieuTri.LyDoTuVong = deathCause.DEATH_CAUSE_CODE == "01" ? LyDoTuVong.DoBenh : deathCause.DEATH_CAUSE_CODE == "02" ? LyDoTuVong.DoTaiBienDieuTri : LyDoTuVong.Khac;
                            _ThongTinDieuTri.NguyenNhanChinhTuVong = deathCause.DEATH_CAUSE_NAME;
                            _ThongTinDieuTri.MaICD_NguyenNhanChinhTuVong = currentTreatment.ICD_CODE;
                        }
                    }
                    if (currentTreatment.DEATH_WITHIN_ID > 0)
                    {
                        _ThongTinDieuTri.ThoiGianTuVong = currentTreatment.DEATH_WITHIN_ID == 1 ? ThoiGianTuVong.Trong24hVaoVien : ThoiGianTuVong.Sau24hvaoVien;

                        _ThongTinDieuTri.KhamNghiemTuThi = false;
                        _ThongTinDieuTri.ChanDoanGiaiPhauTuThi = "";
                        _ThongTinDieuTri.MaICD_ChanDoanGiaiPhauTuThi = "";
                    }

                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = new V_HIS_TREATMENT_BED_ROOM();
                    param = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomFilter treatmentBedroomFilter = new HisTreatmentBedRoomFilter();
                    treatmentBedroomFilter.TREATMENT_ID = currentTreatment.ID;
                    var treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentBedroomFilter, null);
                    treatmentBedRoom = (treatmentBedRooms != null && treatmentBedRooms.Count() > 0) ? treatmentBedRooms.OrderByDescending(o => o.IN_TIME).FirstOrDefault() : null;
                    if (treatmentBedRoom != null)
                    {
                        _ThongTinDieuTri.Buong = treatmentBedRoom.BED_ROOM_NAME;
                        _ThongTinDieuTri.Giuong = treatmentBedRoom.BED_NAME;

                        var bedroom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.ID == treatmentBedRoom.BED_ROOM_ID).FirstOrDefault();
                        if (bedroom != null)
                        {
                            _ThongTinDieuTri.MaKhoa = bedroom.DEPARTMENT_CODE;
                            _ThongTinDieuTri.Khoa = bedroom.DEPARTMENT_NAME;
                        }
                    }
                    else
                    {
                        V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();

                        MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                        patientTypeAlterFilter.TREATMENT_ID = currentTreatment.ID;
                        param = new CommonParam();
                        var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                        if (patientTypeAlters != null && patientTypeAlters.Count() > 0)
                        {
                            patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                            MOS.Filter.HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                            departmentTranFilter.ID = patientTypeAlter.DEPARTMENT_TRAN_ID;
                            param = new CommonParam();
                            var departmentTrans = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, departmentTranFilter, param);
                            departmentTran = (departmentTrans != null && departmentTrans.Count > 0) ? departmentTrans.Where(p => p.DEPARTMENT_IN_TIME != null).First() : null;
                            _ThongTinDieuTri.MaKhoa = departmentTran != null ? departmentTran.DEPARTMENT_CODE : "";
                            _ThongTinDieuTri.Khoa = departmentTran != null ? departmentTran.DEPARTMENT_NAME : "";
                        }
                    }

                    _ThongTinDieuTri.MaGiamDocBenhVien = "";
                    _ThongTinDieuTri.NgayThangNamTrangBia = DateTime.Now;
                    _ThongTinDieuTri.MaTruongKhoa = "";
                    #endregion
                    #region ------ DHST
                    _ThongTinDieuTri.DauSinhTon = new DauSinhTon();
                    DauSinhTon _DauSinhTon = new DauSinhTon();
                    if (_DHST != null)
                    {
                        _DauSinhTon.CanNang = (double)(_DHST.WEIGHT ?? 0);
                        _DauSinhTon.HuyetAp = _DHST.BLOOD_PRESSURE_MAX + "/" + _DHST.BLOOD_PRESSURE_MIN;
                        _DauSinhTon.Mach = (int)(_DHST.PULSE ?? 0);
                        _DauSinhTon.NhietDo = (double)(_DHST.TEMPERATURE ?? 0);
                        _DauSinhTon.NhipTho = (int)(_DHST.BREATH_RATE ?? 0);
                        _DauSinhTon.ChieuCao = (double)(_DHST.HEIGHT ?? 0);
                        _DauSinhTon.BMI = (double)(_DHST.VIR_BMI ?? 0);
                    }
                    _ThongTinDieuTri.DauSinhTon = _DauSinhTon;
                    #endregion

                    #region ------ HoSo
                    _ThongTinDieuTri.HoSo = new HoSo();
                    HoSo _HoSo = new HoSo();
                    _ThongTinDieuTri.HoSo = _HoSo;
                    #endregion
                    BenhAnNgoaiKhoa _BenhAnCommonADO = new BenhAnNgoaiKhoa();

                    _BenhAnCommonADO.DauSinhTon = new DauSinhTon();
                    _BenhAnCommonADO.HoSo = new HoSo();
                    _BenhAnCommonADO.DacDiemLienQuanBenh = new EMR_MAIN.DacDiemLienQuanBenh()
                    {
                        DiUng = false,
                        DiUng_Text = "",
                        Khac_DacDiemLienQuanBenh = false,
                        Khac_DacDiemLienQuanBenh_Text = "",
                        MaTuy = false,
                        MaTuy_Text = "",
                        RuouBia = false,
                        RuouBia_Text = "",
                        ThuocLa = false,
                        ThuocLao = false,
                        ThuocLao_Text = "",
                        ThuocLa_Text = ""
                    };

                    if (_ExamServiceReq != null)
                    {
                        _BenhAnCommonADO.BacSyDieuTri = _ExamServiceReq.EXECUTE_USERNAME;
                        _BenhAnCommonADO.BacSyLamBenhAn = _ExamServiceReq.REQUEST_USERNAME;
                        _BenhAnCommonADO.BenhChinh = _ExamServiceReq.ICD_NAME;
                        _BenhAnCommonADO.BenhKemTheo = _ExamServiceReq.ICD_TEXT;
                        _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = sereServCLSs != null ? String.Join(";", sereServCLSs.Select(o => o.TDL_SERVICE_NAME).ToArray()) : "";
                        _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam = _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Length > 2048 ? _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam.Substring(0, 2048) : _BenhAnCommonADO.CacXetNghiemCanLamSangCanLam;
                        _BenhAnCommonADO.CoXuongKhop = _ExamServiceReq.PART_EXAM_MUSCLE_BONE;

                        _BenhAnCommonADO.HoHap = _ExamServiceReq.PART_EXAM_RESPIRATORY;
                        _BenhAnCommonADO.HuongDieuTri = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.HuongDieuTriVaCacCheDoTiepTheo = _ExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.Khac_CacCoQuan = _ExamServiceReq.PART_EXAM;
                        _BenhAnCommonADO.LyDoVaoVien = _ExamServiceReq.HOSPITALIZATION_REASON;
                        _BenhAnCommonADO.MaQuanLy = Inventec.Common.TypeConvert.Parse.ToDecimal(_ExamServiceReq.TREATMENT_CODE);
                        _BenhAnCommonADO.Mat = _ExamServiceReq.PART_EXAM_EYE;
                        _BenhAnCommonADO.NgayKhamBenh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.INTRUCTION_DATE) ?? DateTime.Now;
                        _BenhAnCommonADO.NgayTongKet = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_ExamServiceReq.FINISH_TIME ?? 0) ?? DateTime.Now;
                        _BenhAnCommonADO.NguoiGiaoHoSo = _ExamServiceReq.REQUEST_USERNAME;
                        _BenhAnCommonADO.NguoiNhanHoSo = _ExamServiceReq.EXECUTE_USERNAME;
                        _BenhAnCommonADO.PhanBiet = "";
                        _BenhAnCommonADO.PhuongPhapDieuTri = _ExamServiceReq.TREATMENT_INSTRUCTION;
                        _BenhAnCommonADO.QuaTrinhBenhLy = _ExamServiceReq.PATHOLOGICAL_PROCESS;
                        _BenhAnCommonADO.QuaTrinhBenhLyVaDienBien = _ExamServiceReq.NOTE;
                        _BenhAnCommonADO.RangHamMat = _ExamServiceReq.PART_EXAM_STOMATOLOGY;
                        _BenhAnCommonADO.TaiMuiHong = String.IsNullOrEmpty(_ExamServiceReq.PART_EXAM_ENT) ? _ExamServiceReq.PART_EXAM_EAR + " " + _ExamServiceReq.PART_EXAM_NOSE + " " + _ExamServiceReq.PART_EXAM_THROAT : _ExamServiceReq.PART_EXAM_ENT;
                        _BenhAnCommonADO.TienLuong = "";
                        _BenhAnCommonADO.TienSuBenhBanThan = _ExamServiceReq.PATHOLOGICAL_HISTORY;
                        _BenhAnCommonADO.TienSuBenhGiaDinh = _ExamServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        _BenhAnCommonADO.TieuHoa = _ExamServiceReq.PART_EXAM_DIGESTION;
                        _BenhAnCommonADO.TinhTrangNguoiBenhRaVien = _ExamServiceReq.ADVISE;
                        _BenhAnCommonADO.ToanThan = _ExamServiceReq.FULL_EXAM;
                        _BenhAnCommonADO.TomTatBenhAn = _ExamServiceReq.SUBCLINICAL;
                        _BenhAnCommonADO.TomTatKetQuaXetNghiem = "";
                        _BenhAnCommonADO.TuanHoan = _ExamServiceReq.PART_EXAM_CIRCULATION;
                        _BenhAnCommonADO.ThanKinh = _ExamServiceReq.PART_EXAM_NEUROLOGICAL;
                        _BenhAnCommonADO.ThanTietNieuSinhDuc = _ExamServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        _BenhAnCommonADO.VaoNgayThu = Inventec.Common.TypeConvert.Parse.ToInt32(_ExamServiceReq.SICK_DAY > 0 ? _ExamServiceReq.SICK_DAY.ToString() : "0");
                    }
                    #region Call Show ERM.Dll
                    ERMADO _ERMADO = new ERMADO();
                    _ERMADO.KyDienTu_ApplicationCode = GlobalVariables.APPLICATION_CODE;
                    _ERMADO.KyDienTu_DiaChiACS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_ACS;
                    _ERMADO.KyDienTu_DiaChiEMR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_EMR;
                    _ERMADO.KyDienTu_DiaChiThuVienKy = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS;
                    _ERMADO.KyDienTu_TREATMENT_CODE = currentTreatment.TREATMENT_CODE;

                    _ERMADO._HanhChinhBenhNhan_s = new HanhChinhBenhNhan();
                    _ERMADO._ThongTinDieuTri_s = new ThongTinDieuTri();
                    _ERMADO._LoaiBenhAnEMR_s = new LoaiBenhAnEMR();
                    _ERMADO._LoaiBenhAnEMR_s = LoaiBenhAnEMR.NgoaiKhoa;

                    // gán thông tin hành chính
                    _ERMADO._HanhChinhBenhNhan_s = _HanhChinhBenhNhan;
                    _ERMADO._ThongTinDieuTri_s = _ThongTinDieuTri;

                    //Gán dữ liệu vào SDO , tương đương với mỗi loại bệnh án

                    _ERMADO._BenhAnNgoaiKhoa_s = _BenhAnCommonADO;

                    _ERMADO.PhauThuatThuThuat_HIS_s = PhauThuatThuThuat_HISs;
                    string cmdLn = EncodeData(_ERMADO);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Integrate\\EMR\\ConnectToEMR.exe";
                    startInfo.Arguments = cmdLn;
                    Process.Start(startInfo);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        long GetRoomTypeByRoomId(long? roomId)
        {
            long roomTypeId = 0;
            try
            {
                var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault();
                roomTypeId = room != null ? room.ROOM_TYPE_ID : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomTypeId;
        }

        string EncodeData(object data)
        {
            string result = "";

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));

            result = System.Convert.ToBase64String(plainTextBytes);

            return Inventec.Common.String.StringCompressor.CompressString(result);
        }

        string DecodeData(string data)
        {
            string result = "";
            var base64EncodedBytes = System.Convert.FromBase64String(data);
            result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return result;
        }

        ChuyenVien GetChuyenVienFromTranspatiForm(long? tranpatiFormId)
        {
            ChuyenVien cv = ChuyenVien.Khac;
            try
            {
                if (tranpatiFormId.HasValue && tranpatiFormId > 0)
                {
                    cv = (IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE == tranpatiFormId.Value || tranpatiFormId.Value == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE) ? ChuyenVien.TuyenTren : tranpatiFormId.Value == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG ? ChuyenVien.TuyenDuoi : ChuyenVien.Khac;
                }
            }
            catch { }
            return cv;
        }

        private void btnAssignPaanClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && IsCheckDepartmentTran())
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnServicePackageViewClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServicePackageView").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServicePackageView");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnOtherFormsClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    HIS.Desktop.Plugins.Library.OtherForm.OtherFormsProcessor run = new Library.OtherForm.OtherFormsProcessor(this.currentModule, treatmentBedRoomRow.TREATMENT_ID);
                    run.RunProcess();
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
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicMedicineByDate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicMedicineByDate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTuberclusisTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTuberclusisTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTuberclusisTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnMedicalAssessmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMedicalAssessment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisMedicalAssessment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnHivTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisHivTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisHivTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        var treatment = histreatment.FirstOrDefault(o => o.ID == treatmentBedRoomRow.TREATMENT_ID);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatment);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicServices_NT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicServices_NT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow);
                        listArgs.Add(true);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnPublicMedicineByPhasedClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PublicMedicineByPhased").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PublicMedicineByPhased");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnCareClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CareCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnSummaryInforTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnBedRoomInClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomIn");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)bbtnSearch);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnBedRoomOutClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomOut").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomOut");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnAssignServiceClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        HIS.Desktop.ADO.AssignServiceADO assignServiceADO = new HIS.Desktop.ADO.AssignServiceADO(treatmentBedRoomRow.TREATMENT_ID, 0, 0);
                        assignServiceADO.PatientDob = treatmentBedRoomRow.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = treatmentBedRoomRow.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));


                        if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                        {
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            ((Form)extenceInstance).ShowDialog();

                        }
                        else
                        {
                            if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                            {
                                if (GlobalVariables.FormAssignService != null)
                                {
                                    GlobalVariables.FormAssignService.WindowState = FormWindowState.Maximized;
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { currentModule,assignServiceADO });
                                    Inventec.Common.Logging.LogSystem.Error("CASE 2 _START");
                                    GlobalVariables.FormAssignService.Activate();
                                    Inventec.Common.Logging.LogSystem.Error("CASE 2 _END");
                                }
                                else
                                {
                                    GlobalVariables.FormAssignService = (Form)PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    if (GlobalVariables.FormAssignService == null) throw new ArgumentNullException("moduleData is null");
                                    GlobalVariables.FormAssignService.Show();

                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { true });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignBloodClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisAssignBlood'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisAssignBlood' is not plugins");

                    AssignBloodADO assignBloodADO = new AssignBloodADO(treatmentBedRoomRow.TREATMENT_ID, 0, 0);
                    List<object> listArgs = new List<object>();
                    assignBloodADO.PatientDob = treatmentBedRoomRow.TDL_PATIENT_DOB;
                    assignBloodADO.PatientName = treatmentBedRoomRow.TDL_PATIENT_NAME;
                    assignBloodADO.GenderName = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                    listArgs.Add(assignBloodADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTuTrucClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(treatmentBedRoomRow.TREATMENT_ID, 0, 0);
                        assignServiceADO.IsCabinet = true;
                        assignServiceADO.PatientDob = treatmentBedRoomRow.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = treatmentBedRoomRow.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = treatmentBedRoomRow.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);

                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
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

        private void btnAssignPrescriptionClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(treatmentBedRoomRow.TREATMENT_ID, 0, 0);
                        assignServiceADO.PatientDob = treatmentBedRoomRow.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = treatmentBedRoomRow.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = treatmentBedRoomRow.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);

                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
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

        private void btnTreatmentPatientUpdateClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentPatientUpdate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentPatientUpdate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnCareSlipListClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisCareSum").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisCareSum");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTranPatiOutInfoClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTranPatiOutInfo").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTranPatiOutInfo");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTranPatiToInfoClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTranPatiToInfo").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTranPatiToInfo");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnDeathInfoClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDeathInfo").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDeathInfo");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTaoBienBanHoiChanClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                        filter.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        HIS_SERVICE_REQ rs = new HIS_SERVICE_REQ();
                        rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                        List<object> listArgs = new List<object>();
                        listArgs.Add(rs);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnDebateDiagnosticClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Debate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Debate");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnMedicalInfomationClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTrackingTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HisServiceReqGroupByDateSDO input = new HisServiceReqGroupByDateSDO();
                        input.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        input.InstructionDate = Convert.ToInt64(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString().Substring(0, 8));
                        listArgs.Add(input);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnInfusionSumByTreatmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfusionSumByTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.InfusionSumByTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnAccidentHurtClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AccidentHurt").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AccidentHurt");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnMediReactSumClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MediReactSum").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MediReactSum");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnBedHistoryClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BedHistory").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BedHistory");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnInfantInformationClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfantInformation").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.InfantInformation");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTreatmentHistoryClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentHistory'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentHistoryADO currentInput = new TreatmentHistoryADO();
                        currentInput.treatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        currentInput.treatment_code = treatmentBedRoomRow.TREATMENT_CODE;
                        listArgs.Add(currentInput);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnTreatmentFinishClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentFinish");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();
                        TreatmentLogADO.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        TreatmentLogADO.RoomId = this.wkRoomId;
                        listArgs.Add(TreatmentLogADO);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)bbtnSearch);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTransDepartmentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransDepartment").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransDepartment'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TransDepartmentADO transDepartmenADO = new TransDepartmentADO(treatmentBedRoomRow.TREATMENT_ID);
                        transDepartmenADO.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                        transDepartmenADO.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.wkRoomId).DepartmentId;
                        listArgs.Add(transDepartmenADO);
                        listArgs.Add((RefeshReference)FillDataToGridTreatmentBedRoom);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnRequestDepositClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RequestDeposit'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnEmrDocumentClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EmrDocument").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.EmrDocument'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_CODE);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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
        private void btnPhieuChamSocVobClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                    emrInputAdo.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                    emrInputAdo.PatientId = treatmentBedRoomRow.PATIENT_ID;
                    if (treatmentBedRoomRow.EMR_COVER_TYPE_ID != null)
                    {
                        emrInputAdo.EmrCoverTypeId = treatmentBedRoomRow.EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.ROOM_ID == this.wkRoomId
                        && o.TREATMENT_TYPE_ID == treatmentBedRoomRow.TDL_TREATMENT_TYPE_ID
                        ).ToList();
                        if (data != null && data.Count > 0)
                        {
                            if (data.Count == 1)
                            {
                                emrInputAdo.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;

                            }
                            else
                            {
                                emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                emrInputAdo.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                        else
                        {
                            var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.wkRoomId).DepartmentId;

                            var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == treatmentBedRoomRow.TDL_TREATMENT_TYPE_ID).ToList();

                            if (DataConfig != null && DataConfig.Count > 0)
                            {
                                if (DataConfig.Count == 1)
                                {
                                    emrInputAdo.EmrCoverTypeId = DataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                                }
                                else
                                {
                                    emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                    emrInputAdo.lstEmrCoverTypeId = DataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                                }
                            }
                        }
                    }

                    emrInputAdo.roomId = this.wkRoomId;
                    HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                    long EmrCoverTypeId_ = emrInputAdo.EmrCoverTypeId ?? 0;
                    long EmrCoverTypeId_Send;

                    if (EmrCoverTypeId_ <= 0)
                    {
                        EmrCoverTypeId_Send = 0;
                    }
                    else
                    {
                        EmrCoverTypeId_Send = EmrCoverTypeId_;
                    }


                    string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.BedRoomPartial.EmrFormOption");
                    string key_;

                    if (key == "1")
                    {
                        key_ = "CS2";
                    }
                    else
                    {
                        key_ = "CS";
                    }
                    processor.FormOpenEmr(EmrCoverTypeId_Send, emrInputAdo, key_);


                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnBordereauClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.Bordereau'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnCanTheoDoi()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    FrmFollow frm = new FrmFollow(treatmentBedRoomRow, (HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    frm.ShowDialog();
                    frm.Focus();    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBoTheoDoi()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    MOS.SDO.ObservedTimeSDO sdo = new ObservedTimeSDO();
                    sdo.TreatmentBedRoomId = treatmentBedRoomRow.ID;
                    sdo.IsUnobserved = true;
                    sdo.ObservedTimeFrom = treatmentBedRoomRow.TDL_OBSERVED_TIME_FROM ?? 0;
                    sdo.ObservedTimeTo = treatmentBedRoomRow.TDL_OBSERVED_TIME_TO ?? 0;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatmentBedRoom/SetObservedTime", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();

                    if (success)
                    {
                        
                        MessageManager.Show(this.ParentForm, param, success);
                        RefreshData();
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnAggrHospitalFeesClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnServiceReqListClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS_TREATMENT input = new HIS_TREATMENT();
                        input.ID = treatmentBedRoomRow.TREATMENT_ID;
                        input.TREATMENT_CODE = treatmentBedRoomRow.TREATMENT_CODE;
                        listArgs.Add(input);
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnSumaryTestResultsClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SumaryTestResults").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SumaryTestResults'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        private void btnEditICD()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentIcdEdit").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentIcdEdit'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatmentBedRoomRow.TREATMENT_ID);
                        listArgs.Add((RefeshReference)Refesh);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
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

        public void Refesh()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Refesh(object data)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDisApprovalFinishClick()
        {
            try
            {
                if (treatmentBedRoomRow != null && treatmentBedRoomRow.IS_APPROVE_FINISH == 1)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                       HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                       HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm),
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HisTreatmentApproveFinishSDO sdo = new HisTreatmentApproveFinishSDO();
                        sdo.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;

                        var _Treatment = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UnapproveFinish", ApiConsumers.MosConsumer, sdo, param);
                        if (_Treatment != null)
                        {
                            FillDataToGridTreatmentBedRoom();
                            success = true;
                        }

                        WaitingManager.Hide();
                        if (success)
                        {
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnApprovalFinishClick()
        {
            try
            {
                if (treatmentBedRoomRow != null)
                {
                    var form = new ApprovalFinish.FormApprovalFinish(treatmentBedRoomRow, FillDataToGridTreatmentBedRoom);
                    if (form != null)
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckingTreatmentEmr()
        {
            try
            {
                if (this.treatmentBedRoomRow != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentBedRoomRow.TREATMENT_ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTreatmentRecordChecking", this.wkRoomId, this.wkRoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CamKetNamGiuongTuNguyen()
        {
            try
            {
                if (this.treatmentBedRoomRow != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000403_CAM_KET_NAM_GIUONG_TU_NGUYEN);
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
                if (this.treatmentBedRoomRow != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;
                    ado.DepartmentId = this.DepartmentID;

                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000404_BAN_GIAO_BN_TRUOC_PTTT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PhieuKhaiThacTienSuDiUng()
        {
            try
            {
                if (this.treatmentBedRoomRow != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;
                    ado.TreatmentBedRoomId = this.treatmentBedRoomRow.ID;
                    ado.DepartmentId = this.DepartmentID;

                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000405_PHIEU_KHAI_THAC_TIEN_SU_DI_UNG);
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
                if (this.treatmentBedRoomRow != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatmentBedRoomRow.TREATMENT_ID;
                    ado.TreatmentBedRoomId = this.treatmentBedRoomRow.ID;
                    ado.DepartmentId = this.DepartmentID;

                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000406_PHIEU_XN_DUONG_MAU_MAO_MACH);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DevExpress.Utils.Menu.DXMenuItem> menuItem(DHisSereServ2 data)
        {
            List<DevExpress.Utils.Menu.DXMenuItem> dXmenuItem = new List<DevExpress.Utils.Menu.DXMenuItem>();
            try
            {
                _SereServADORightMouseClick = new DHisSereServ2();
                var dXmenu = new DevExpress.Utils.Menu.DXMenuItem();
                if (data != null && data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                {
                    _SereServADORightMouseClick = data;
                    dXmenu.Image = imageCollection1.Images[0];
                    dXmenu.Click += Event_RightMouseClick;
                    dXmenu.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KE_DON_DUOC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    dXmenuItem.Add(dXmenu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dXmenuItem;

        }

        private void Event_RightMouseClick(object sender, EventArgs e)
        {
            try
            {
                if (_SereServADORightMouseClick != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, _SereServADORightMouseClick);

                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(RowCellClickBedRoom.TREATMENT_ID,
                            0,
                            _SereServADORightMouseClick.SERVICE_REQ_ID ?? 0,
                            sereServInput);

                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = RowCellClickBedRoom.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        assignServiceADO.IsAutoCheckExpend = true;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
						if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
						((Form)extenceInstance).ShowDialog();
					}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    public class ERMADO
    {
        public string KyDienTu_DiaChiACS = "";
        public string KyDienTu_DiaChiEMR = "";
        public string KyDienTu_DiaChiThuVienKy = "";
        public string KyDienTu_ApplicationCode = "HIS";
        public string KyDienTu_TREATMENT_CODE = "";

        public LoaiBenhAnEMR _LoaiBenhAnEMR_s { get; set; }
        public HanhChinhBenhNhan _HanhChinhBenhNhan_s { get; set; }
        public ThongTinDieuTri _ThongTinDieuTri_s { get; set; }
        public BenhAnBong _BenhAnBong_s { get; set; }
        public BenhAnDaLieu _BenhAnDaLieu_s { get; set; }
        public BenhAnHuyetHocTruyenMau _BenhAnHuyetHocTruyenMau_s { get; set; }
        public BenhAnMatBanPhanTruoc _BenhAnMatBanPhanTruoc_s { get; set; }
        public BenhAnMatChanThuong _BenhAnMatChanThuong_s { get; set; }
        public BenhAnMatDayMat _BenhAnMatDayMat_s { get; set; }
        public BenhAnMatGlocom _BenhAnMatGlocom_s { get; set; }
        public BenhAnMatLac _BenhAnMatLac_s { get; set; }
        public BenhAnMatTreEm _BenhAnMatTreEm_s { get; set; }
        public BenhAnDieuTriBanNgay _BenhAnDieuTriBanNgay_s { get; set; }
        public BenhAnNgoaiKhoa _BenhAnNgoaiKhoa_s { get; set; }
        public BenhAnNgoaiTru _BenhAnNgoaiTru_s { get; set; }
        public BenhAnNgoaiTruRangHamMat _BenhAnNgoaiTruRangHamMat_s { get; set; }
        public BenhAnNgoaiTruTaiMuiHong _BenhAnNgoaiTruTaiMuiHong_s { get; set; }
        public BenhAnNgoaiTruYHCT _BenhAnNgoaiTruYHCT_s { get; set; }
        public BenhAnNhiKhoa _BenhAnNhiKhoa_s { get; set; }
        public BenhAnNoiKhoa _BenhAnNoiKhoa_s { get; set; }
        public BenhAnNoiTruYHCT _BenhAnNoiTruYHCT_s { get; set; }
        public BenhAnPhuKhoa _BenhAnPhuKhoa_s { get; set; }
        public BenhAnPhucHoiChucNang _BenhAnPhucHoiChucNang_s { get; set; }
        public BenhAnRangHamMat _BenhAnRangHamMat_s { get; set; }
        public BenhAnSanKhoa _BenhAnSanKhoa_s { get; set; }
        public BenhAnSoSinh _BenhAnSoSinh_s { get; set; }
        public BenhAnTaiMuiHong _BenhAnTaiMuiHong_s { get; set; }
        public BenhAnTamThan _BenhAnTamThan_s { get; set; }
        public BenhAnTruyenNhiem _BenhAnTruyenNhiem_s { get; set; }
        public BenhAnUngBuou _BenhAnUngBuou_s { get; set; }
        public BenhAnXaPhuong _BenhAnXaPhuong_s { get; set; }
        public List<PhauThuatThuThuat_HIS> PhauThuatThuThuat_HIS_s { get; set; }
        public BenhAnTim _BenhAnTim_s { get; set; }
    }

    internal class BenhAnCommonADO : BenhAnBase
    {
        public BenhAnCommonADO() : base() { }
    }
}
