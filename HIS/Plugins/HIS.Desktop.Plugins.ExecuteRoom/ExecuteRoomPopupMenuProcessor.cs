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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ExecuteRoom
{
    delegate void ExecuteRoomMouseRight_Click(object sender, ItemClickEventArgs e);

    class ExecuteRoomPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ serviceReqRightClick;
        ExecuteRoomMouseRight_Click executeRoomMouseRightClick;
        public List<ServiceReqADO> CheckServiceExecuteGroup = new List<ServiceReqADO>();
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = new MediRecordMenuPopupProcessor();
        BarManager barManager;
        PopupMenu menu;
        long roomId;
        internal enum ModuleType
        {
            SummaryInforTreatmentRecords,
            AggrHospitalFees,
            TreatmentHistory,
            TreatmentHistory2,
            RoomTran,
            DepositReq,
            Bordereau,
            PhieuVoBenhAn,
            Execute,
            ServiceExecuteGroup,
            UnExecute,
            UnStart,
            ServiceReqList,
            OtherForms,
            BenhAnNgoaiTru,
            Debate,
            SuaYeuCauKham,
            AssignPaan,
            TreatmentList,
            AllergyCard,
            ThongTinChuyenDen,
            InPhieuKetQuaDaKy,
            KeThuocVatTu,
            Thietlapkhotieuhao,
            Khamsuckhoe,
            DetailMedicalRecord,
            PhanLoaiBenhNhan,
            HivTreatment,
            ChonMayXuLy,
            TuberclusisTreatment
        }
        internal ModuleType moduleType { get; set; }

        internal ExecuteRoomPopupMenuProcessor(MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ currentServiceReq, ExecuteRoomMouseRight_Click executeRoomMouseRightClick, BarManager barManager, long _roomId, List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO> CheckServiceExecuteGroup_, MediRecordMenuPopupProcessor _emrMenuPopupProcessor)
        {
            this.serviceReqRightClick = currentServiceReq;
            this.executeRoomMouseRightClick = executeRoomMouseRightClick;
            this.barManager = barManager;
            this.roomId = _roomId;
            CheckServiceExecuteGroup = CheckServiceExecuteGroup_;
            this.emrMenuPopupProcessor = _emrMenuPopupProcessor;
        }

        public void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();


                //Vỏ bệnh án
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.serviceReqRightClick.TREATMENT_ID;

                HIS_TREATMENT treatment = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = treatment.ID;
                emrInputAdo.PatientId = treatment.PATIENT_ID;
                var EmrCoverConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == roomId && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID).ToList();
                if (treatment.EMR_COVER_TYPE_ID != null)
                {
                    emrInputAdo.EmrCoverTypeId = treatment.EMR_COVER_TYPE_ID.Value;
                }
                else
                {
                    if (EmrCoverConfig != null && EmrCoverConfig.Count > 0)
                    {
                        if (EmrCoverConfig.Count == 1)
                        {
                            emrInputAdo.EmrCoverTypeId = EmrCoverConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdo.lstEmrCoverTypeId = new List<long>();
                            emrInputAdo.lstEmrCoverTypeId = EmrCoverConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                    else
                    {
                        var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;

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

                emrInputAdo.roomId = roomId;
                emrInputAdo.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;
                emrInputAdo.TreatmentTypeId = serviceReqRightClick.TDL_TREATMENT_TYPE_ID;
                emrInputAdo.Treatment = treatment;
                emrMenuPopupProcessor.InitMenu(menu, barManager, emrInputAdo);

                BarButtonItem itemSummaryInforTreatmentRecords = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnSummaryInforTreatmentRecords.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 1);
                itemSummaryInforTreatmentRecords.Tag = ModuleType.SummaryInforTreatmentRecords;
                itemSummaryInforTreatmentRecords.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemUnStart = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnUnStart.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemUnStart.Tag = ModuleType.UnStart;
                itemUnStart.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemUnExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnUnFinish.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemUnExecute.Tag = ModuleType.UnExecute;
                itemUnExecute.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemAggrHospitalFees = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnAggrHospitalFees.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemAggrHospitalFees.Tag = ModuleType.AggrHospitalFees;
                itemAggrHospitalFees.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemTreatmentList = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.menu1.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemTreatmentList.Tag = ModuleType.TreatmentList;
                itemTreatmentList.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemTreatmentHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemTreatmentHistory.Tag = ModuleType.TreatmentHistory;
                itemTreatmentHistory.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemKsk = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.menu2.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemKsk.Tag = ModuleType.Khamsuckhoe;
                itemKsk.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemTreatmentHistory2 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory2.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 1);
                itemTreatmentHistory2.Tag = ModuleType.TreatmentHistory2;
                itemTreatmentHistory2.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemRoomTran = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRoomTran.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemRoomTran.Tag = ModuleType.RoomTran;
                itemRoomTran.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemDepositReq = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnDepositReq.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemDepositReq.Tag = ModuleType.DepositReq;
                itemDepositReq.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemInphieuketquadaky = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnInKetQuaDaKy.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 1);
                itemInphieuketquadaky.Tag = ModuleType.InPhieuKetQuaDaKy;
                itemInphieuketquadaky.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemDetailMedicalRecord = new BarButtonItem(barManager, "Chi tiết bệnh án", 1);
                itemDetailMedicalRecord.Tag = ModuleType.DetailMedicalRecord;
                itemDetailMedicalRecord.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemExecute.Tag = ModuleType.Execute;
                itemExecute.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);



                BarButtonItem itemServiceExecuteGroup = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnitemServiceExecuteGroup.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemServiceExecuteGroup.Tag = ModuleType.ServiceExecuteGroup;
                itemServiceExecuteGroup.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemDebate = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBienBanHoiChan.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemDebate.Tag = ModuleType.Debate;
                itemDebate.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemServiceReqList = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnServiceReqList.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemServiceReqList.Tag = ModuleType.ServiceReqList;
                itemServiceReqList.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemOtherForm = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBieuMauKhac.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemOtherForm.Tag = ModuleType.OtherForms;
                itemOtherForm.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemBenhAnNgoaiTru = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBenhAnNgoaiTru.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemBenhAnNgoaiTru.Tag = ModuleType.BenhAnNgoaiTru;
                itemBenhAnNgoaiTru.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemAssignPaan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnAssignPaan.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemAssignPaan.Tag = ModuleType.AssignPaan;
                itemAssignPaan.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemSuaYeuCauKham = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.menu3.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemSuaYeuCauKham.Tag = ModuleType.SuaYeuCauKham;
                itemSuaYeuCauKham.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemAllergyCard = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.menu4.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemAllergyCard.Tag = ModuleType.AllergyCard;
                itemAllergyCard.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemThongTinChuyenDen = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnThongTinChuyenDen.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 1);
                itemThongTinChuyenDen.Tag = ModuleType.ThongTinChuyenDen;
                itemThongTinChuyenDen.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemPhanLoaiBenhNhan = new BarButtonItem(barManager, "Phân loại bệnh nhân", 5);
                itemPhanLoaiBenhNhan.Tag = ModuleType.PhanLoaiBenhNhan;
                itemPhanLoaiBenhNhan.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemBordereau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBordereau.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemBordereau.Tag = ModuleType.Bordereau;
                itemBordereau.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                //BarButtonItem itemPhieuVoBenhAn = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnPhieuVoBenhAn.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 10);
                //itemPhieuVoBenhAn.Tag = ModuleType.PhieuVoBenhAn;
                //itemPhieuVoBenhAn.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemKeThuocVatTu = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnKeThuocVatTu.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 4);
                itemKeThuocVatTu.Tag = ModuleType.KeThuocVatTu;
                itemKeThuocVatTu.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);


                BarButtonItem itemThietlapkhotieuhao = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnThietlapkhotieuhao.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 7);
                itemThietlapkhotieuhao.Tag = ModuleType.Thietlapkhotieuhao;
                itemThietlapkhotieuhao.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemHivTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnHivTreatment.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 7);
                itemHivTreatment.Tag = ModuleType.HivTreatment;
                itemHivTreatment.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                #region ----- Thông tin điều trị bệnh lao
                BarButtonItem itemBenhLao = new BarButtonItem(barManager, "Thông tin điều trị bệnh lao", 8);
                itemBenhLao.Tag = ModuleType.TuberclusisTreatment;
                itemBenhLao.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);
                #endregion

                BarButtonItem itemMachine = new BarButtonItem(barManager, "Chọn máy xử lý cho các chỉ định được chọn", 7);
                itemMachine.Tag = ModuleType.ChonMayXuLy;
                itemMachine.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);
                if (this.serviceReqRightClick != null)
                {
                    menu.AddItems(new BarItem[] { itemDetailMedicalRecord });

                    if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        menu.AddItems(new BarItem[] { itemUnExecute });
                    }
                    else
                    {
                        menu.AddItems(new BarItem[] { itemExecute });
                    }
                    if (CheckServiceExecuteGroup != null && CheckServiceExecuteGroup.Count > 0)
                    {
                        var data = CheckServiceExecuteGroup.Where(o => o.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__XULYDV);
                        if (data != null && data.Count() > 1)
                        {
                            menu.AddItems(new BarItem[] { itemServiceExecuteGroup });
                        }
                    }
                    if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                    {
                        menu.AddItems(new BarItem[] { itemUnStart });
                    }

                    if (this.serviceReqRightClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                        && this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    {
                        menu.AddItems(new BarItem[] { itemSuaYeuCauKham });
                    }

                    if (this.serviceReqRightClick.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        menu.AddItems(new BarItem[] { itemKsk });
                    }
                    //var executeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_EMERGENCY == 1 && o.ROOM_ID == roomId).FirstOrDefault();
                    //if (executeRooms != null)

                    if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                        && this.serviceReqRightClick.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        menu.AddItems(new BarItem[] { itemInphieuketquadaky });
                    }
                    menu.AddItems(new BarItem[] { itemBordereau, itemAggrHospitalFees, itemTreatmentList, itemServiceReqList, itemTreatmentHistory, itemOtherForm, itemBenhAnNgoaiTru, itemDebate, itemAssignPaan,itemPhanLoaiBenhNhan, itemAllergyCard, itemThongTinChuyenDen });

                    menu.AddItems(new BarItem[] { itemSummaryInforTreatmentRecords });

                    if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                            || this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        //btnRoomTran.Enabled = false;
                    }
                    else
                    {
                        menu.AddItems(new BarItem[] { itemRoomTran });
                    }
                    menu.AddItems(new BarItem[] { itemKeThuocVatTu });
                    menu.AddItems(new BarItem[] { itemThietlapkhotieuhao });
                    menu.AddItems(new BarItem[] { itemHivTreatment });
                    menu.AddItems(new BarItem[] { itemBenhLao });
                    menu.AddItems(new BarItem[] { itemMachine });
                }

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
