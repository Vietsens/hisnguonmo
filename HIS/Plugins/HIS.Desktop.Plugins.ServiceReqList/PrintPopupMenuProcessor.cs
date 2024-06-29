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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    delegate void PrintMedicine_Click(object sender, ItemClickEventArgs e);

    class PrintPopupMenuProcessor
    {
        PrintMedicine_Click PrintMouseClick;
        BarManager barManager;
        PopupMenu menu;
        ADO.ServiceReqADO ado;
        long serviceReqSttId;
        long serviceReqTypeId;
        string loginName;
        internal long currentDepartmentId;
        internal V_HIS_ROOM currentRoom;

        internal enum ModuleType
        {
            thuocTongHop,
            donThuoc,
            donThuocYHCT,
            chuyenKhoa,
            kham,
            Mps000033,
            Mps000035,
            Mps000097,
            Mps000204,
            Mps000420,
            _testPhieuYeuCau,
            _testDomSoi,
            Edit,
            Delete,
            Print,
            EditIntruction,
            BieuMauKhac,
            EvenLog,
            ExamMain,
            GuiLaiXN,
            BieuMauKhacV2,
            DanhSachVanBanDaKy
            ,
            SendOldSystemIntegration,
            OpenAttachFile,
            EnterInforBeforeSurgery,
            NoExecute,
            Execute,
            SampleInfo,
            TheBenhNhan,
            SampleType,
            PhieuThuKiemPhieuYcKham,
            ChangeRoom,
            AllowNotExecute,
            DisposeAllowNotExecute,
            HenKhamLai,
            DrugInterventionInfo,
            KetQuaHeThongBenhAnhDienTu,
            GiayDeNghiDoiTraDichVu,
            TaoPhieuYeuCauSuDungKhangSinh,
            HuyLayMau,
            ChuyenThanhDonTam,
            InVatTuTSD

        }

        internal PrintPopupMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager, string _loginName)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.barManager = barManager;
            this.loginName = _loginName;
        }

        internal PrintPopupMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager, long _serviceReqTypeId, string _loginName)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.barManager = barManager;
            this.serviceReqTypeId = _serviceReqTypeId;
            this.loginName = _loginName;
        }

        internal PrintPopupMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager, long _sereServSTT, long _serviceReqTypeId, string _loginName)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.barManager = barManager;
            this.serviceReqSttId = _sereServSTT;
            this.serviceReqTypeId = _serviceReqTypeId;
            this.loginName = _loginName;
        }

        internal PrintPopupMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager, ADO.ServiceReqADO ado, string _loginName, V_HIS_ROOM _currentRoom = null)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.barManager = barManager;
            this.ado = ado;
            this.loginName = _loginName;
            this.currentRoom = _currentRoom;
        }

        internal void RightMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();
                if ((ado.CREATOR == this.loginName || ado.REQUEST_LOGINNAME == this.loginName)
                            && ((ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            && ado.IS_NO_EXECUTE != Base.GlobalStore.IS_TRUE
                            || (ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                            && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1"))
                    //&& ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                    )
                {
                    BarButtonItem itemEdit = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.Edit", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                    itemEdit.Tag = ModuleType.Edit;
                    itemEdit.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemEdit });
                }

                if ((ado.CREATOR == this.loginName || ado.REQUEST_LOGINNAME == loginName || ado.DeleteCheck)
                           && (ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL))
                {
                    BarButtonItem itemDelete = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.Delete", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 2);
                    itemDelete.Tag = ModuleType.Delete;
                    itemDelete.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemDelete });
                }

                BarButtonItem itemPrint = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.Print", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 3);
                itemPrint.Tag = ModuleType.Print;
                itemPrint.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemPrint });

                if (!this.ado.IS_MAIN_EXAM.HasValue && this.ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                {
                    BarButtonItem itemExamMain = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.MainExam", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 4);
                    itemExamMain.Tag = ModuleType.ExamMain;
                    itemExamMain.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemExamMain });
                }

                if (ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT || ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    BarButtonItem KetQuaHeThongBenhAnhDienTu_ = new BarButtonItem(this.barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.KetQuaHeThongBenhAnhDienTu", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                    KetQuaHeThongBenhAnhDienTu_.Tag = ModuleType.KetQuaHeThongBenhAnhDienTu;
                    KetQuaHeThongBenhAnhDienTu_.ItemClick += new ItemClickEventHandler(this.PrintMouseClick);
                    menu.AddItems(new BarItem[] { KetQuaHeThongBenhAnhDienTu_ });
                }

                BarButtonItem itemEditIntruction = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.EditCommonInfo", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 4);
                itemEditIntruction.Tag = ModuleType.EditIntruction;
                itemEditIntruction.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemEditIntruction });


                if (ado != null && !string.IsNullOrEmpty(ado.JSON_PRINT_ID))
                {
                    BarButtonItem itemOther = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.OtherPrint", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 5);
                    itemOther.Tag = ModuleType.BieuMauKhac;
                    itemOther.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemOther });////xuandv bo
                }

                BarButtonItem itemOtherV2 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.OtherPrintByRequest", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 5);
                itemOtherV2.Tag = ModuleType.BieuMauKhacV2;
                itemOtherV2.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemOtherV2 });

                BarButtonItem itemEmrDocumentList = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.EmrDocumentList", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 5);
                itemEmrDocumentList.Tag = ModuleType.DanhSachVanBanDaKy;
                itemEmrDocumentList.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemEmrDocumentList });

                BarButtonItem itemEvenLog = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.EventLog", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 6);
                itemEvenLog.Tag = ModuleType.EvenLog;
                itemEvenLog.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemEvenLog });

                if (ado != null && (
                    ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA ||
                    ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS ||
                    ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC))
                {
                    BarButtonItem itemXN = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.ReSendAssignToLIS", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 7);
                    itemXN.Tag = ModuleType.GuiLaiXN;
                    itemXN.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemXN });
                }

                if (ado != null && HisConfigCFG.IsOldSystemIntegration && HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                {
                    BarButtonItem itemSendOldSystemIntegration = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.SendAssignToOldSystem", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 8);
                    itemSendOldSystemIntegration.Tag = ModuleType.SendOldSystemIntegration;
                    itemSendOldSystemIntegration.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemSendOldSystemIntegration });
                }
                if (ado != null && !String.IsNullOrEmpty(ado.ATTACHMENT_FILE_URL))
                {
                    BarButtonItem itemOpenAttachFile = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.AttackTreatment", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 9);
                    itemOpenAttachFile.Tag = ModuleType.OpenAttachFile;
                    itemOpenAttachFile.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemOpenAttachFile });
                }

                if (ado != null && ado.AddInforPTTT)
                {
                    BarButtonItem itemEnterInforBeforeSurgery = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.InputBeforeSurg", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 9);
                    itemEnterInforBeforeSurgery.Tag = ModuleType.EnterInforBeforeSurgery;
                    itemEnterInforBeforeSurgery.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemEnterInforBeforeSurgery });
                }

                //#17522
                if ((ado.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                          && (//ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                              //|| 
                            ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                          || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        && ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("RightMenu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                    BarButtonItem itemEdit = new BarButtonItem(barManager, ado.IS_NO_EXECUTE == Base.GlobalStore.IS_TRUE ? Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.Execute", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()) : Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.NoExecute", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                    itemEdit.Tag = ado.IS_NO_EXECUTE == Base.GlobalStore.IS_TRUE ? ModuleType.Execute : ModuleType.NoExecute;
                    itemEdit.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemEdit });
                }
                if (ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                    ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                    ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                {
                    BarButtonItem createAntibioticRequest = new BarButtonItem(barManager, "Tạo phiếu yêu cầu sử dụng kháng sinh", 9);
                    createAntibioticRequest.Tag = ModuleType.TaoPhieuYeuCauSuDungKhangSinh;
                    createAntibioticRequest.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { createAntibioticRequest });
                }

                if (ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                    && (!ado.IS_NO_EXECUTE.HasValue || ado.IS_NO_EXECUTE.Value != 1)
                    && ado.REQUEST_DEPARTMENT_ID == this.currentDepartmentId
                    && ado.SAMPLE_TIME == null)
                {
                    BarButtonItem itemSampleInfo = new BarButtonItem(barManager, "Lấy mẫu bệnh phẩm", 10);
                    itemSampleInfo.Tag = ModuleType.SampleInfo;
                    itemSampleInfo.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemSampleInfo });
                }
                if (ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                    && ado.REQUEST_DEPARTMENT_ID == this.currentDepartmentId
                    && ado.SAMPLE_TIME != null
                    && !HisConfigCFG.IsUseInventecLis
                    )
                {
                    BarButtonItem itemHuyLayMau = new BarButtonItem(barManager, "Hủy lấy mẫu", 11);
                    itemHuyLayMau.Tag = ModuleType.HuyLayMau;
                    itemHuyLayMau.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemHuyLayMau });
                }

                if (ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                {
                    //Chỉ định cùng kíp
                    BarButtonItem bbtSampleType = new BarButtonItem(barManager, "Cập nhật loại bệnh phẩm", 0);
                    bbtSampleType.Tag = ModuleType.SampleType;
                    bbtSampleType.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { bbtSampleType });
                }
                BarButtonItem itemChangeRoom = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.ChangeRoom", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                itemChangeRoom.Tag = ModuleType.ChangeRoom;
                itemChangeRoom.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemChangeRoom });

                if (ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                {
                    if (HisConfigCFG.ConnectDrugInterventionInfo)
                    {
                        BarButtonItem itemConnectDrugInterventionInfo = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.ConnectDrugInterventionInfo", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                        itemConnectDrugInterventionInfo.Tag = ModuleType.DrugInterventionInfo;
                        itemConnectDrugInterventionInfo.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                        menu.AddItems(new BarItem[] { itemConnectDrugInterventionInfo });
                    }
                }

                //if (ado.REQUEST_ROOM_ID == this.currentRoom.ID && workingRoom.IS_EXAM == 1
                //            && (ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                //            || ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                //            || ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                //            || ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                //            && ado.IS_ACCEPTING_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                //            && ado.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                //{
                //    //Chỉ định cùng kíp
                //    BarButtonItem bbtAllowNotExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.AllowNotExecute", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                //    bbtAllowNotExecute.Tag = ModuleType.AllowNotExecute;
                //    bbtAllowNotExecute.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                //    menu.AddItems(new BarItem[] { bbtAllowNotExecute });
                //}
                var executeRooms = BackendDataWorker.Get<HIS_ROOM>();
                var executeRoom = executeRooms != null && currentRoom != null ? executeRooms.FirstOrDefault(o => o.ID == currentRoom.ID) : null;

                //1. Danh sách y lệnh -> chuột phải y lệnh
                //a. Bổ sung menu "Cho phép không thực hiện" vào menu chuột phải khi click vào y lệnh:
                //- Menu này chỉ hiển thị nếu thỏa mãn các điều kiện:
                //+ Phòng chỉ định là phòng mà người dùng đang làm việc
                //+ Phòng người dùng đang làm việc là phòng khám
                //+ Không phải là thuốc/vật tư/máu
                //+ Y lệnh chưa bị check "không thực hiện" (his_service_req có IS_NO_EXECUTE khác 1)
                //+ Y lệnh chưa được check "cho phép không thực hiện" (his_service_req có IS_ACCEPTING_NO_EXECUTE khác 1)
                //- Khi click vào nút này thì gọi lên api để cập nhật trạng thái "Cho phép không thực hiện" (cập nhật his_service_req và his_sere_serv có IS_ACCEPTING_NO_EXECUTE = 1 )

                // b. Bổ sung menu "Hủy cho phép không thực hiện" vào menu chuột phải khi click vào y lệnh:
                //- Menu này chỉ hiển thị nếu thỏa mãn các điều kiện:
                //+ Phòng chỉ định là phòng mà người dùng đang làm việc
                //+ Phòng người dùng đang làm việc là phòng khám
                //+ Không phải là thuốc/vật tư/máu
                //+ Y lệnh chưa bị check "không thực hiện" (his_service_req có IS_NO_EXECUTE khác 1)
                //+ Y lệnh đã được check "cho phép không thực hiện" (his_service_req có IS_ACCEPTING_NO_EXECUTE = 1)
                //- Khi click vào nút này thì gọi lên api để cập nhật trạng thái "Hủy cho phép không thực hiện"

                if (currentRoom != null && ado.REQUEST_ROOM_ID == this.currentRoom.ID
                    && executeRoom != null && executeRoom.ROOM_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG
                    && ado.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                    && ado.IS_NO_EXECUTE != 1
                    && ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    if (ado.IS_ACCEPTING_NO_EXECUTE != 1)//&& ado.IS_ACCEPTING_NO_EXECUTE != 1//TODO
                    {
                        BarButtonItem itemAllowNotExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.AllowNotExecute", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                        itemAllowNotExecute.Tag = ModuleType.AllowNotExecute;
                        itemAllowNotExecute.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                        menu.AddItems(new BarItem[] { itemAllowNotExecute });
                    }
                    else //ado.IS_ACCEPTING_NO_EXECUTE == 1//TODO
                    {
                        BarButtonItem itemDisposeAllowNotExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.DisposeAllowNotExecute", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 1);
                        itemDisposeAllowNotExecute.Tag = ModuleType.DisposeAllowNotExecute;
                        itemDisposeAllowNotExecute.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                        menu.AddItems(new BarItem[] { itemDisposeAllowNotExecute });
                    }
                }

                BarButtonItem bbtnChangeService = new BarButtonItem(barManager, "Giấy đề nghị đổi trả dịch vụ", 11);
                bbtnChangeService.Tag = ModuleType.GiayDeNghiDoiTraDichVu;
                bbtnChangeService.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { bbtnChangeService });

                
                if ((HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName) || ado.REQUEST_LOGINNAME.Equals(loginName)) && ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT) {
                    if (LoadDataToCurrentTreatmentData(ado.TREATMENT_ID).IS_PAUSE != 1)
                    {
                        var emrTreatment = LoadDataToCurrentEmrTreatmentData(ado.TDL_TREATMENT_CODE);
                        if (emrTreatment == null || emrTreatment.STORE_TIME == null || emrTreatment.STORE_TIME == 0)
                        {
                            BarButtonItem bbtnUpdateToTemp = new BarButtonItem(barManager, "Chuyển thành đơn tạm", 11);
                            bbtnUpdateToTemp.Tag = ModuleType.ChuyenThanhDonTam;
                            bbtnUpdateToTemp.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                            menu.AddItems(new BarItem[] { bbtnUpdateToTemp });
                        }
                    }
                }

                if(ado != null && (this.ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || this.ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT || this.ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT))
                {
                    BarButtonItem bbtnPrintmaterialUse = new BarButtonItem(barManager, "In tem vật tư tái sử dụng", 11);
                    bbtnPrintmaterialUse.Tag = ModuleType.InVatTuTSD;
                    bbtnPrintmaterialUse.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { bbtnPrintmaterialUse });
                }
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_TREATMENT LoadDataToCurrentTreatmentData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;

                var listTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        private EMR_TREATMENT LoadDataToCurrentEmrTreatmentData(string treatmentCode)
        {
            EMR.EFMODEL.DataModels.EMR_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrTreatmentFilter filter = new EMR.Filter.EmrTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;

                var listTreatment = new BackendAdapter(param).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemTrackingTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__DON_THUOC", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemTrackingTreatment.Tag = ModuleType.donThuoc;
                itemTrackingTreatment.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemTrackingTreatment });

                BarButtonItem itemBedHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__THUOC_TONG_HOP", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemBedHistory.Tag = ModuleType.thuocTongHop;
                itemBedHistory.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemBedHistory });

                BarButtonItem itemTreatmentHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__DON_THUOC_YHCT", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemTreatmentHistory.Tag = ModuleType.donThuocYHCT;
                itemTreatmentHistory.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemTreatmentHistory });

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void InitMenuKham(HIS.Desktop.Plugins.ServiceReqList.ADO.ServiceReqADO ServiceReqADO)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemTrackingTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__KHAM", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemTrackingTreatment.Tag = ModuleType.kham;
                itemTrackingTreatment.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemTrackingTreatment });

                BarButtonItem itemThuKiemYcKham = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__PHIEU_THU_KIEM_YC_KHAM", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemThuKiemYcKham.Tag = ModuleType.PhieuThuKiemPhieuYcKham;
                itemThuKiemYcKham.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemThuKiemYcKham });

                BarButtonItem itemBedHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__CHUYEN_KHOA", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemBedHistory.Tag = ModuleType.chuyenKhoa;
                itemBedHistory.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemBedHistory });

                BarButtonItem itemTheBN = new BarButtonItem(barManager, "In thẻ bệnh nhân", 3);
                itemTheBN.Tag = ModuleType.TheBenhNhan;
                itemTheBN.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemTheBN });


                if (ServiceReqADO.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT && ServiceReqADO.APPOINTMENT_TIME != null)
                {
                    BarButtonItem itemHenKhamLai = new BarButtonItem(barManager, "In phiếu hẹn khám lại", 3);
                    itemHenKhamLai.Tag = ModuleType.HenKhamLai;
                    itemHenKhamLai.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { itemHenKhamLai });

                }


                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void InitMenuPttt()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem Mps000033 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__PHIEU_PHAU_THUAT", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                Mps000033.Tag = ModuleType.Mps000033;
                Mps000033.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { Mps000033 });

                BarButtonItem Mps000035 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__GIAY_CAM_DOAN", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                Mps000035.Tag = ModuleType.Mps000035;
                Mps000035.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { Mps000035 });

                BarButtonItem Mps000097 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__CACH_THUC_PHAU_THUAT", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                Mps000097.Tag = ModuleType.Mps000097;
                Mps000097.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { Mps000097 });

                if (this.serviceReqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT && this.serviceReqTypeId != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                {
                    BarButtonItem Mps000204 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__GIAY_CHUNG_NHAN_PHAU_THUAT", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    Mps000204.Tag = ModuleType.Mps000204;
                    Mps000204.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                    menu.AddItems(new BarItem[] { Mps000204 });
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void InitMenuXetNghiem()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemTestPhieuYeuCau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__PHIEU_YEU_CAU_XET_NGHIEM", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemTestPhieuYeuCau.Tag = ModuleType._testPhieuYeuCau;
                itemTestPhieuYeuCau.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemTestPhieuYeuCau });

                BarButtonItem itemBieuMauKhac = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_SERVICE_REQ_LIST__CLICK__BIEU_MAU_KHAC", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemBieuMauKhac.Tag = ModuleType._testDomSoi;
                itemBieuMauKhac.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { itemBieuMauKhac });

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
