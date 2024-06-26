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
                        case PopupMenuProcessor.ItemType.Erm:
                            LoadData_ERMv3(LoaiBenhAnEMR.NoiKhoa);
                            break;
                        case PopupMenuProcessor.ItemType.Erm2:
                            LoadData_ERMv3(LoaiBenhAnEMR.NgoaiKhoa);
                            break;
                        case PopupMenuProcessor.ItemType.Erm3:
                            LoadData_ERMv3(LoaiBenhAnEMR.DaLieu);
                            break;
                        case PopupMenuProcessor.ItemType.Erm4:
                            LoadData_ERMv3(LoaiBenhAnEMR.Bong);
                            break;
                        case PopupMenuProcessor.ItemType.Erm5:
                            LoadData_ERMv3(LoaiBenhAnEMR.HuyetHocTruyenMau);
                            break;
                        case PopupMenuProcessor.ItemType.Erm6:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatBanPhanTruoc);
                            break;
                        case PopupMenuProcessor.ItemType.Erm7:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatChanThuong);
                            break;
                        case PopupMenuProcessor.ItemType.Erm8:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatDayMat);
                            break;
                        case PopupMenuProcessor.ItemType.Erm9:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatGloCom);
                            break;
                        case PopupMenuProcessor.ItemType.Erm10:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatLac);
                            break;
                        case PopupMenuProcessor.ItemType.Erm11:
                            LoadData_ERMv3(LoaiBenhAnEMR.MatTreEm);
                            break;
                        case PopupMenuProcessor.ItemType.Erm12:
                            LoadData_ERMv3(LoaiBenhAnEMR.NgoaiTru);
                            break;
                        case PopupMenuProcessor.ItemType.Erm13:
                            LoadData_ERMv3(LoaiBenhAnEMR.NgoaiTruRangHamMat);
                            break;
                        case PopupMenuProcessor.ItemType.Erm14:
                            LoadData_ERMv3(LoaiBenhAnEMR.NgoaiTruTaiMuiHong);
                            break;
                        case PopupMenuProcessor.ItemType.Erm15:
                            LoadData_ERMv3(LoaiBenhAnEMR.NgoaiTruYHCT);
                            break;
                        case PopupMenuProcessor.ItemType.Erm16:
                            LoadData_ERMv3(LoaiBenhAnEMR.NhiKhoa);
                            break;
                        case PopupMenuProcessor.ItemType.Erm17:
                            LoadData_ERMv3(LoaiBenhAnEMR.NoiTruYHCT);
                            break;
                        case PopupMenuProcessor.ItemType.Erm18:
                            LoadData_ERMv3(LoaiBenhAnEMR.PhucHoiChucNang);
                            break;
                        case PopupMenuProcessor.ItemType.Erm19:
                            LoadData_ERMv3(LoaiBenhAnEMR.PhuKhoa);
                            break;
                        case PopupMenuProcessor.ItemType.Erm20:
                            LoadData_ERMv3(LoaiBenhAnEMR.RangHamMat);
                            break;
                        case PopupMenuProcessor.ItemType.Erm21:
                            LoadData_ERMv3(LoaiBenhAnEMR.SanKhoa);
                            break;
                        case PopupMenuProcessor.ItemType.Erm22:
                            LoadData_ERMv3(LoaiBenhAnEMR.SoSinh);
                            break;
                        case PopupMenuProcessor.ItemType.Erm23:
                            LoadData_ERMv3(LoaiBenhAnEMR.TaiMuiHong);
                            break;
                        case PopupMenuProcessor.ItemType.Erm24:
                            LoadData_ERMv3(LoaiBenhAnEMR.TamThan);
                            break;
                        case PopupMenuProcessor.ItemType.Erm25:
                            LoadData_ERMv3(LoaiBenhAnEMR.TruyenNhiem);
                            break;
                        case PopupMenuProcessor.ItemType.Erm26:
                            LoadData_ERMv3(LoaiBenhAnEMR.UngBuou);
                            break;
                        case PopupMenuProcessor.ItemType.Erm27:
                            LoadData_ERMv3(LoaiBenhAnEMR.XaPhuong);
                            break;
                        case PopupMenuProcessor.ItemType.Erm30:
                            LoadData_ERMv3(LoaiBenhAnEMR.DieuTriBanNgay);
                            break;
                        case PopupMenuProcessor.ItemType.Erm33:
                            LoadData_ERMv3(LoaiBenhAnEMR.Tim);
                            break;
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
                        case PopupMenuProcessor.ItemType._PhieuHenKham:
                            PhieuhenKhamPrintClick();
                            break;
                        case PopupMenuProcessor.ItemType._PhieuChuyenVien:
                            PhieuChuyenVienPrintClick();
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
                        case PopupMenuProcessor.ItemType.DebateDiagnostic:
                            btnDebateDiagnosticClick();
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
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

                    popupMenuProcessor = new PopupMenuProcessor(this.currentTreatment, barManager2, Treatment_MouseRightClick, (RefeshReference)BtnRefreshs);
                    popupMenuProcessor.InitMenu();
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

        private void GiayPTTTPrintClick()
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
                    if (card != null)
                    {
                        new LaunchChrome().Launch(card.CARD_CODE, "/#/ybadientu_n", ConfigSystems.URI_API_HSSK);
                    }
                    else
                    {
                        MessageManager.Show("Bệnh nhân chưa có thẻ thông minh");
                    }
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
                cardFilter.ORDER_FIELD = "MODIFY_TIME";
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
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "Inventec.Desktop.Plugins.EventLog").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'Inventec.Desktop.Plugins.EventLog'");
                    //if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'Inventec.Desktop.Plugins.EventLog' is not plugins");

                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, string.Format("TREATMENT_CODE: {0}", currentTreatment.TREATMENT_CODE));

                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    //var moduleWK = PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
                    //var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                    //if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.EventLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
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
                    listArgs.Add(treatment);
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
                    WaitingManager.Show();

                    bool unFinishTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("/api/HisTreatment/Unfinish", ApiConsumers.MosConsumer, currentTreatment.ID, param);
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
                    listArgs.Add(true);
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
                    listArgs.Add(true);
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
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(true);
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
    }
}
