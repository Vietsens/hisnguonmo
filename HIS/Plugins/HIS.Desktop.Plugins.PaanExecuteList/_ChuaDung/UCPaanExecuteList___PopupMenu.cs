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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.PaanExecuteList.Base;
using Inventec.Desktop.Common.Modules;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    public partial class UCPaanExecuteList
    {
        void PaanMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.currentServiceReq != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PaanPopupMenuProcessor.ModuleType type = (PaanPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case PaanPopupMenuProcessor.ModuleType.SummaryInforTreatmentRecords:
                            SummaryInforTreatmentRecordsClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.AggrHospitalFees:
                            AggrHospitalFeesClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.TreatmentHistory:
                            TreatmentHistoryClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.TreatmentHistory2:
                            TreatmentHistory2Click(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.RoomTran:
                            RoomTranClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.DepositReq:
                            DepositReqClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.Bordereau:
                            BordereauClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.Execute:
                            ExecuteClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.ServiceExecuteGroup:
                            ServiceExecuteGroupClick(this.CheckServiceExecuteGroup);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.UnStart:
                            UnStartClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.UnExecute:
                            CancelFinish(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.OtherForms:
                            OtherFormClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.ServiceReqList:
                            ServiceReqListClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.BenhAnNgoaiTru:
                            InBenhAnNgoaiTru(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.Debate:
                            DebateClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.SuaYeuCauKham:
                            SuaYeuCauKham(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.AssignPaan:
                            AssignPaanClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.TreatmentList:
                            TreatmentListClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.AnalyzeMedicalImageAI:
                            AnalyzeMedicalImageAIClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.AllergyCard:
                            AllergyCardClick(this.currentServiceReq);
                            break;

                        case PaanPopupMenuProcessor.ModuleType.ThongTinChuyenDen:
                            ThongTinChuyenDenClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.InPhieuKetQuaDaKy:
                            InPhieuKetQuaDaKy(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.PhieuVoBenhAn:
                            InPhieuVoBenhAn(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.KeThuocVatTu:
                            FormKeThuocVatTu(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.Thietlapkhotieuhao:
                            FormThietlapkhotieuhao();
                            break;
                        case PaanPopupMenuProcessor.ModuleType.Khamsuckhoe:
                            OpenFormEnterKskInfomantionVer2(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.DetailMedicalRecord:
                            ChiTietBenhAn(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.PhanLoaiBenhNhan:
                            PhanLoaiBenhNhan(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.HivTreatment:
                            FormHivTreatment(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.TuberclusisTreatment:
                            btnTuberclusisTreatmentClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.ChonMayXuLy:
                            FormMachine(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.MoiHoiChan:
                            MoiHoiChanClick(this.currentServiceReq);
                            break;
                        case PaanPopupMenuProcessor.ModuleType.HisTransReqList:
                            try
                            {
                                var moduleData = GlobalVariables.currentModuleRaws
                                    .FirstOrDefault(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTransReqList");

                                if (moduleData == null)
                                    throw new NullReferenceException("Không tìm thấy module HIS.Desktop.Plugins.HisTransReqList");

                                List<object> args = new List<object>();

                                // truyền treatmentId
                                args.Add(this.treatmentId);

                                // truyền module theo phòng
                                var moduleWithRoom = HIS.Desktop.Utility.PluginInstance
                                    .GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId);

                                args.Add(moduleWithRoom);

                                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, args);

                                if (instance == null)
                                    throw new ArgumentNullException("Không khởi tạo được form Danh sách QR");

                                ((Form)instance).ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MoiHoiChanClick(ADO.PaanSereServADO currentServiceReq)
        {
            try
            {
                if (currentServiceReq == null)
                    return;

                MOS.Filter.HisSpecialistExamViewFilter filterExam = new MOS.Filter.HisSpecialistExamViewFilter();
                // INVITE_TYPE = 2 -> Mời hội chẩn (giữ consistency với ApprovaleDebateList, BedRoomPartial)
                filterExam.INVITE_TYPE = 2;

                WaitingManager.Show();
                var examList = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SPECIALIST_EXAM>>(
                    "api/HisSpecialistExam/GetView",
                    ApiConsumers.MosConsumer,
                    filterExam,
                    new CommonParam());
                WaitingManager.Hide();

                // Filter client-side: cùng TREATMENT_ID, IS_ACTIVE=1, IS_DELETE!=1, IS_APPROVAL!=1 (chưa hoàn tất)
                var pending = examList != null
                    ? examList.FirstOrDefault(o => o.TREATMENT_ID == currentServiceReq.TREATMENT_ID
                        && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                        && o.IS_APPROVAL != 1)
                    : null;

                if (pending != null)
                {
                    string deptName = "";
                    var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()
                        .FirstOrDefault(o => o.ID == pending.EXAM_EXECUTE_DEPARMENT_ID);
                    if (dept != null)
                        deptName = dept.DEPARTMENT_NAME;

                    // Fallback safety: ResourceManager has been observed returning empty in some
                    // deployments where plugin DLL is loaded from a path that does not pick up
                    // the rebuilt embedded .resources. Log + use literal so the dialog always renders.
                    string template = Resources.ResourceMessage.BNDangCoPhieuHoiChanChuaHoanTat;
                    if (string.IsNullOrEmpty(template))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "ResourceMessage.BNDangCoPhieuHoiChanChuaHoanTat returned empty - check DLL deployment");
                        template = "Bệnh nhân đang có phiếu hội chẩn chưa hoàn tất với khoa {0}. Bạn có muốn tạo thêm phiếu mới không?";
                    }
                    string warningMsg = string.Format(template, deptName);

                    string title = Resources.ResourceMessage.ThongBao;
                    if (string.IsNullOrEmpty(title)) title = "Thông báo";

                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        warningMsg,
                        title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                HisServiceReqViewFilter filterReq = new HisServiceReqViewFilter();
                filterReq.ID = currentServiceReq.ID;

                WaitingManager.Show();
                var serviceReqView = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(
                        "api/HisServiceReq/getView",
                        ApiConsumers.MosConsumer,
                        filterReq,
                        new CommonParam())
                    ?.FirstOrDefault();
                WaitingManager.Hide();

                if (serviceReqView == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("MoiHoiChanClick: V_HIS_SERVICE_REQ null"
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => filterReq), filterReq));
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InviteConsultation")
                    .FirstOrDefault();
                if (moduleData == null)
                {
                    // Truoc day thoat im lang -> nguoi dung tuong chuc nang hong. Bao ro nhu man buong benh.
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.InviteConsultation");
                    MessageBox.Show(Resources.ResourceMessage.ChucNangDangPhatTrienLienHeQuanTri,
                        Resources.ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(
                        "moduleLink = HIS.Desktop.Plugins.InviteConsultation khong phai plugin hoac thieu ExtensionInfo."
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                    MessageBox.Show(Resources.ResourceMessage.ChucNangDangPhatTrienLienHeQuanTri,
                        Resources.ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                {
                    var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                        moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleWithRoom);
                    listArgs.Add(false);
                    listArgs.Add(serviceReqView);

                    var extenceInstance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                    if (extenceInstance == null)
                        throw new ArgumentNullException("Khong khoi tao duoc form Moi hoi chan");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTuberclusisTreatmentClick(PaanSereServADO currentServiceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTuberclusisTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTuberclusisTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentServiceReq.TREATMENT_ID);
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

        private void FormMachine(PaanSereServADO currentServiceReq)
        {
            try
            {
                var serviceReqId = new List<long>();
                List<string> lst = new List<string>();
                foreach (var i in gridViewPaan.GetSelectedRows())
                {
                    var row = (HIS.Desktop.Plugins.PaanExecuteList.ADO.PaanSereServADO)gridViewPaan.GetRow(i);
                    if (!(row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                   || row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                   || row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                   || row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                   || row.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                   ))
                        lst.Add(row.SERVICE_REQ_CODE);
                    else
                        serviceReqId.Add(row.ID);
                }
                if(lst != null && lst.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Các y lệnh {0} không thuộc 1 trong các loại sau: {1}", string.Join(", ", lst),string.Join("; ",BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().Where(row => row.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                   || row.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                   || row.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                   || row.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                   || row.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA).Select(o=>o.SERVICE_REQ_TYPE_NAME))), Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                if(serviceReqId == null || serviceReqId.Count == 0) {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn y lệnh.", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
                frmMachine frm = new frmMachine(GetResultServiceReq, serviceReqId, null);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetResultServiceReq(bool isSuccess)
        {
            try
            {
                if (isSuccess)
                    FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PhanLoaiBenhNhan(PaanSereServADO currentServiceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.UpdatePatientClassify").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.UpdatePatientClassify");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        L_HIS_TREATMENT_BED_ROOM treatmentBedRoom = new L_HIS_TREATMENT_BED_ROOM();
                        treatmentBedRoom.PATIENT_TYPE_CODE = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == currentServiceReq.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                        treatmentBedRoom.TDL_PATIENT_CLASSIFY_ID = currentServiceReq.TDL_PATIENT_CLASSIFY_ID;
                        treatmentBedRoom.PATIENT_ID = GetPatient(currentServiceReq).ID;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReqRightClick______", currentServiceReq));

                        listArgs.Add(treatmentBedRoom);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                        FillDataToGridControl();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormHivTreatment(PaanSereServADO currentServiceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisHivTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisHivTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.ID = this.currentServiceReq.TREATMENT_ID;
                        HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReqRightClick______", currentServiceReq));
                        if (treatment != null)
                        {
                            listArgs.Add(treatment);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            ((Form)extenceInstance).ShowDialog();
                        }
                        FillDataToGridControl();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChiTietBenhAn(PaanSereServADO currentServiceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EmrDocument").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.EmrDocument");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(currentServiceReq.TDL_TREATMENT_CODE);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpenFormEnterKskInfomantionVer2(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReq.ID;
                var dataService = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/getView", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();


                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EnterKskInfomantionVer2").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.EnterKskInfomantionVer2");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(dataService);
                    listArgs.Add(currentModule);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FormThietlapkhotieuhao()
        {
            try
            {
                long? idReturn = Library.MediStockExpend.MediStockExpendProcessor.GetMediStock(GetRoomId(), true); // true

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void FormKeThuocVatTu(ADO.PaanSereServADO serviceReqADO)
        {
            try
            {

                long? idReturn = Library.MediStockExpend.MediStockExpendProcessor.GetMediStock(GetRoomId(), false); // true

                if (idReturn == null || idReturn == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho tiêu hao", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                }
                else
                {
                    if (gridViewPaan.GetSelectedRows().Count() > 0)
                    {
                        int select = gridViewPaan.GetSelectedRows().Count();
                        var data = gridViewPaan.GetSelectedRows();
                        List<HIS.Desktop.Plugins.PaanExecuteList.ADO.PaanSereServADO> hisreq = new List<HIS.Desktop.Plugins.PaanExecuteList.ADO.PaanSereServADO>();
                        if (data != null && data.Count() > 0)
                        {
                            foreach (var i in data)
                            {
                                var row = (HIS.Desktop.Plugins.PaanExecuteList.ADO.PaanSereServADO)gridViewPaan.GetRow(i);
                                hisreq.Add(row);
                            }
                            ReqChangeService.FormKeThuoc form = new ReqChangeService.FormKeThuoc(idReturn, select, GetRoomId(), hisreq);
                            form.Show();
                            System.Threading.Thread.Sleep(1000);
                            form.GetDataExpPresCreateByConfig();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void SelectDataResult(object data)
        //{
        //    if (data != null)
        //    {
        //        long ID_ = (long)data;
        //        ReqChangeService.FormKeThuoc form = new ReqChangeService.FormKeThuoc(ID_);
        //        form.Show();
        //    }
        //    else
        //    {
        //        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho tiêu hao", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK);
        //    }
        //}
        private void InPhieuVoBenhAn(ADO.PaanSereServADO serviceReqADO)
        {
            try
            {
                long roomIdLocal = GetRoomId();

                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = serviceReqADO.TREATMENT_ID;

                V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();

                emrInputAdo.TreatmentId = treatment.ID;
                emrInputAdo.PatientId = treatment.PATIENT_ID;
                var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == GetRoomId() && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID).ToList();
                if (treatment.EMR_COVER_TYPE_ID != null)
                {
                    emrInputAdo.EmrCoverTypeId = treatment.EMR_COVER_TYPE_ID.Value;
                }
                else
                {
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
                        var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == GetRoomId()).DepartmentId;

                        var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID).ToList();

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

                emrInputAdo.roomId = GetRoomId();


                HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig.frmPhieu frm = new Library.FormMedicalRecord.FromConfig.frmPhieu(emrInputAdo);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuKetQuaDaKy(ADO.PaanSereServADO serviceReqADO)
        {
            try
            {
                if (!HisConfigCFG.IsHasConnectionEmr)
                    return;
                EmrDocumentFilter emrFilter = new EmrDocumentFilter();
                emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                emrFilter.TREATMENT_CODE__EXACT = serviceReqADO.TDL_TREATMENT_CODE;
                var emrDocumentList = new BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumer.ApiConsumers.EmrConsumer, emrFilter, null);

                if (emrDocumentList != null && emrDocumentList.Count > 0)
                {
                    emrDocumentList = emrDocumentList.Where(o => o.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE).ToList();
                    emrDocumentList = emrDocumentList.Where(o => o.HIS_CODE.Contains(serviceReqADO.TDL_TREATMENT_CODE)).ToList();
                }

                if (emrDocumentList != null && emrDocumentList.Count > 0)
                {
                    foreach (var item in emrDocumentList)
                    {
                        CommonParam paramCommon = new CommonParam();
                        EmrVersionFilter filter = new EmrVersionFilter();
                        filter.DOCUMENT_ID = item.ID;
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "ID";
                        List<EMR_VERSION> apiResult = new BackendAdapter(paramCommon).Get<List<EMR_VERSION>>("api/EmrVersion/Get", ApiConsumers.EmrConsumer, filter, paramCommon);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Info("apiResult.FirstOrDefault().URL: " + apiResult.FirstOrDefault().URL);
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(apiResult.FirstOrDefault().URL);
                            DevExpress.XtraPdfViewer.PdfViewer pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
                            pdfViewer1.LoadDocument(stream);
                            DevExpress.Pdf.PdfPrinterSettings pdfPrinterSettings = new DevExpress.Pdf.PdfPrinterSettings();
                            pdfViewer1.Print(new DevExpress.Pdf.PdfPrinterSettings());
                        }
                    }
                }
                else
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.
                   Show(Resources.ResourceMessage.KhongTonTaiPhieuDaKy, Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ThongTinChuyenDenClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTranPatiToInfo").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTranPatiToInfo");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AllergyCardClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AllergyCard").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AllergyCard");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignPaanClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreatmentListClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TDL_TREATMENT_CODE);
                    Module currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId());
                    listArgs.Add(serviceReq);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE + serviceReq.TDL_TREATMENT_CODE, serviceReq.TDL_TREATMENT_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AnalyzeMedicalImageAIClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AnalyzeMedicalImage").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AnalyzeMedicalImage");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    AnalyzeImageADO treatmentAiIdAdo = new AnalyzeImageADO
                    {
                        TreatmentId = serviceReq.TREATMENT_ID,
                    };
                    List<object> listArgs = new List<object>();
                    listArgs.Add(treatmentAiIdAdo);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SuaYeuCauKham(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.UpdateExamServiceReq").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.UpdateExamServiceReq");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.ID);
                    listArgs.Add(true);//La phong kham
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SummaryInforTreatmentRecordsClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    bool suc = false;
                    listArgs.Add(suc);
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AggrHospitalFeesClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrHospitalFees");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TreatmentHistoryClick(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                btnTreatmentHistory_Click(null, null);
                //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                //    List<object> listArgs = new List<object>();
                //    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                //    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                //    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                //    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                //    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                //    if (treatment != null)
                //    {
                //        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                //        treatmentHistory.treatmentId = treatment.ID;
                //        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                //        listArgs.Add(treatmentHistory);
                //        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                //        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                //        ((Form)extenceInstance).Show();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TreatmentHistory2Click(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.treatmentId = treatment.ID;
                        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RoomTranClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReqInput);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DepositReqClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.RequestDeposit");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BordereauClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    moduleData.RoomId = GetRoomId();
                    moduleData.RoomTypeId = GetRoomTypeId();
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OtherFormClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.OtherForms").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.OtherForms");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null"); ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ServiceExecuteGroupClick(List<PaanSereServADO> serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null && serviceReqInput.Count > 0)
                {
                    //V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, _sereServRowMenu);

                    List<L_HIS_SERVICE_REQ> data = new List<L_HIS_SERVICE_REQ>();

                    foreach (var item in serviceReqInput)
                    {
                        L_HIS_SERVICE_REQ madata = new L_HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<L_HIS_SERVICE_REQ>(madata, item);
                        data.Add(madata);
                    }
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceExecuteGroup").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.OtherForms");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add((DelegateRefeshData)FillDataToGridControl);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null"); ((Form)extenceInstance).ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExecuteClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                LoadModuleExecuteService(serviceReqInput);
                InitEnableControl();
                //  CreateThreadCallPatientRefresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnStartClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                LogTheadInSessionInfo(() => UnStartClick_Action(serviceReqInput), "UnStartClick");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnStartClick_Action(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                this.currentServiceReq = (HIS.Desktop.Plugins.PaanExecuteList.ADO.PaanSereServADO)gridViewPaan.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentServiceReq), currentServiceReq));
                if (serviceReqInput != null)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var serviceReq = new BackendAdapter(param)
                        .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReq), serviceReq));

                    if (serviceReq != null && serviceReq.ID > 0)
                    {
                        long dtFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                        long dtTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "232359");

                        if (dtFrom <= serviceReq.INTRUCTION_TIME && serviceReq.INTRUCTION_TIME <= dtTo)
                        {
                            if (HisConfigCFG.RequestLimitWarningOption == "2")
                            {
                                if (desk != null && currentServiceReq != null && currentServiceReq.EXE_DESK_ID == desk.ID)
                                {
                                    LoadServiceReqCount(false, -1);
                                }
                            }
                            else
                            {
                                if (currentServiceReq != null && currentServiceReq.EXECUTE_LOGINNAME != null)
                                {
                                    if (currentServiceReq.EXECUTE_LOGINNAME.Equals(loginName))
                                    {
                                        LoadServiceReqCount(false, -1);
                                    }
                                }
                            }

                        }
                        success = true;
                        btnUnStart.Enabled = false;
                        //Reload data

                        foreach (var item in serviceReqs)
                        {
                            if (currentServiceReq != null && currentServiceReq.ID != null && item.ID == currentServiceReq.ID)
                            {
                                item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;

                                currentServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                            }
                        }

                        gridControlPaan.RefreshDataSource();

                        LoadSereServCount();
                    }

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

        private void ServiceReqMatyClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqMaty").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqMaty");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(serviceReqInput.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InBenhAnNgoaiTru(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DebateClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Debate").FirstOrDefault();
                    if (moduleData == null)
                    {
                        // Thieu return o day gay NullReferenceException ngay dong duoi, exception lai bi nuot trong catch
                        // -> nguoi dung bam menu khong thay gi xay ra. Bao ro nhu man buong benh.
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Debate");
                        MessageBox.Show(Resources.ResourceMessage.ChucNangDangPhatTrienLienHeQuanTri,
                            Resources.ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(serviceReqInput.TREATMENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
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

        private void ServiceReqListClick(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS_TREATMENT treatment = new HIS_TREATMENT();
                        treatment.ID = serviceReqInput.TREATMENT_ID;
                        listArgs.Add(treatment);
                        listArgs.Add(serviceReqInput.TREATMENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                LoadBieuMauPhieuYCBenhAnNgoaiTru(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
