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
using HIS.Desktop.Plugins.PayClinicalResult.Base;
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
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqRightClick;
        ExecuteRoomMouseRight_Click executeRoomMouseRightClick;
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
            Execute,
            UnExecute,
            UnStart,
            ServiceReqList,
            OtherForms,
            BenhAnNgoaiTru,
            Debate,
            SuaYeuCauKham,
            AssignPaan,
            TreatmentList,
            AllergyCard
        }
        internal ModuleType moduleType { get; set; }

        internal ExecuteRoomPopupMenuProcessor(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentServiceReq, ExecuteRoomMouseRight_Click executeRoomMouseRightClick, BarManager barManager, long _roomId)
        {
            this.serviceReqRightClick = currentServiceReq;
            this.executeRoomMouseRightClick = executeRoomMouseRightClick;
            this.barManager = barManager;
            this.roomId = _roomId;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();



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

                BarButtonItem itemTreatmentList = new BarButtonItem(barManager, "Hồ sơ điều trị", 3);
                itemTreatmentList.Tag = ModuleType.TreatmentList;
                itemTreatmentList.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemTreatmentHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 3);
                itemTreatmentHistory.Tag = ModuleType.TreatmentHistory;
                itemTreatmentHistory.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemTreatmentHistory2 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory2.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 1);
                itemTreatmentHistory2.Tag = ModuleType.TreatmentHistory2;
                itemTreatmentHistory2.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemRoomTran = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRoomTran.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemRoomTran.Tag = ModuleType.RoomTran;
                itemRoomTran.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemDepositReq = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnDepositReq.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemDepositReq.Tag = ModuleType.DepositReq;
                itemDepositReq.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemBordereau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBordereau.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 5);
                itemBordereau.Tag = ModuleType.Bordereau;
                itemBordereau.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemExecute = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()), 2);
                itemExecute.Tag = ModuleType.Execute;
                itemExecute.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

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

                BarButtonItem itemSuaYeuCauKham = new BarButtonItem(barManager, "Sửa yêu cầu khám", 3);
                itemSuaYeuCauKham.Tag = ModuleType.SuaYeuCauKham;
                itemSuaYeuCauKham.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                BarButtonItem itemAllergyCard = new BarButtonItem(barManager, "Thẻ dị ứng", 3);
                itemAllergyCard.Tag = ModuleType.AllergyCard;
                itemAllergyCard.ItemClick += new ItemClickEventHandler(executeRoomMouseRightClick);

                if (this.serviceReqRightClick != null)
                {
                        if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            menu.AddItems(new BarItem[] { itemUnExecute });
                        }
                        else
                        {
                            menu.AddItems(new BarItem[] { itemExecute });
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

                    //var executeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_EMERGENCY == 1 && o.ROOM_ID == roomId).FirstOrDefault();
                    //if (executeRooms != null)


                    menu.AddItems(new BarItem[] { itemBordereau, itemSummaryInforTreatmentRecords, itemAggrHospitalFees, itemTreatmentList, itemServiceReqList, itemTreatmentHistory, itemOtherForm, itemBenhAnNgoaiTru, itemDebate, itemAssignPaan, itemAllergyCard });

                    if (this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                            || this.serviceReqRightClick.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        //btnRoomTran.Enabled = false;
                    }
                    else
                    {
                        menu.AddItems(new BarItem[] { itemRoomTran });
                    }
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
