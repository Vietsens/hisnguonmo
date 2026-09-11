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
using HIS.Desktop.Plugins.PaanExecuteList.ADO;
using HIS.Desktop.Plugins.PaanExecuteList.Base;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>Loai muc menu chuot phai.</summary>
    enum PaanMenuType
    {
        SummaryInforTreatmentRecords,
        AggrHospitalFees,
        TreatmentHistory,
        RoomTran,
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
        AllergyCard,
        ThongTinChuyenDen,
        Khamsuckhoe,
        DetailMedicalRecord,
        PhanLoaiBenhNhan,
        HivTreatment,
        TuberclusisTreatment,
        MoiHoiChan
    }

    delegate void PaanMouseRight_Click(PaanMenuType type);

    /// <summary>
    /// Dung menu chuot phai tren luoi danh sach Giai phau benh.
    ///
    /// VIET LAI (khong chep nguyen) tu
    /// HIS.Desktop.Plugins.ExecuteRoom\ExecuteRoomPopupMenuProcessor.cs.
    ///
    /// LY DO VIET LAI: ban goc bam chat vao ha tang rieng cua man cu
    /// (desk, serviceReqs, HisConfigCFG, PrintTypeCodeWorker,
    /// LoadModuleExecuteService...) nen chep nguyen se keo theo hang nghin dong.
    /// Ban nay chi giu dung phan dung menu, con viec mo plugin do
    /// UCPaanExecuteList___PopupMenu.cs lo.
    ///
    /// GIU NGUYEN so voi man cu:
    ///   - Nhan tieng Viet: lay tu cung bo khoa "UCExecuteRoom.*"
    ///     (file Resources\LangExecuteRoom.vi.resx chep tu man cu).
    ///   - Dieu kien hien/an tung muc.
    ///
    /// DA BO so voi man cu (co ly do, xem ghi chu tung cho):
    ///   - Nhom vo benh an EMR: can cau hinh theo dung PHONG lam viec.
    ///   - Ke thuoc/vat tu, Thiet lap kho tieu hao: thao tac tren KHO cua phong.
    ///   - Tra ket qua tong hop, Chon may xu ly, Danh sach QR, In phieu ket qua
    ///     da ky, Phan tich hinh anh AI: phu thuoc ha tang rieng man cu.
    ///   - Cac muc von da chet o man cu (Chi tiet y lenh, Yeu cau tam ung,
    ///     Phieu vo benh an, HisServiceReqMaty).
    /// </summary>
    class PaanPopupMenuProcessor
    {
        L_HIS_SERVICE_REQ serviceReq;
        PaanMouseRight_Click onClick;
        BarManager barManager;
        PopupMenu menu;

        internal PaanPopupMenuProcessor(
            L_HIS_SERVICE_REQ serviceReq,
            PaanMouseRight_Click onClick,
            BarManager barManager)
        {
            this.serviceReq = serviceReq;
            this.onClick = onClick;
            this.barManager = barManager;
        }

        /// <summary>Dung va hien menu.</summary>
        internal void InitMenu()
        {
            try
            {
                if (serviceReq == null || barManager == null) return;

                menu = new PopupMenu(barManager);

                List<BarItem> items = new List<BarItem>();

                // --- Nhom thao tac y lenh ---

                // "Chi tiet benh an" - nhan literal o ban goc.
                items.Add(NewItem("Chi tiết bệnh án", PaanMenuType.DetailMedicalRecord, 1));

                // "Huy ket thuc" - chi khi y lenh DA HOAN THANH.
                if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    items.Add(NewItemLang("UCExecuteRoom.btnUnFinish.Text", "Hủy kết thúc",
                        PaanMenuType.UnExecute, 3));
                }

                // "Xu ly" - chi khi y lenh CHUA hoan thanh.
                if (serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    items.Add(NewItemLang("UCExecuteRoom.btnExecute.Text", "Xử lý (Ctrl X)",
                        PaanMenuType.Execute, 1));
                }

                // "Huy bat dau" - chi khi y lenh DANG XU LY.
                if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    items.Add(NewItemLang("UCExecuteRoom.btnUnStart.Text", "Hủy bắt đầu",
                        PaanMenuType.UnStart, 2));
                }

                // "Sua yeu cau kham" - chi voi y lenh KHAM va CHUA xu ly.
                if (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                    && serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    items.Add(NewItemLang("UCExecuteRoom.btnUpdateExamServiceReq.Text", "Sửa yêu cầu khám",
                        PaanMenuType.SuaYeuCauKham, 3));
                }

                // "Thong tin kham ho so suc khoe" - chi voi y lenh KHAM.
                if (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                {
                    items.Add(NewItemLang("UCExecuteRoom.menu2.Text", "Thông tin khám hồ sơ sức khỏe",
                        PaanMenuType.Khamsuckhoe, 3));
                }

                // "Chuyen phong" - chi khi CHUA xu ly va CHUA hoan thanh.
                if (serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    && serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    items.Add(NewItemLang("UCExecuteRoom.btnRoomTran.Text", "Chuyển phòng",
                        PaanMenuType.RoomTran, 3));
                }

                // --- Nhom tra cuu, luon hien ---

                items.Add(NewItemLang("UCExecuteRoom.btnBordereau.Text", "Bảng kê (F5)",
                    PaanMenuType.Bordereau, 2));
                items.Add(NewItemLang("UCExecuteRoom.btnAggrHospitalFees.Text", "Viện phí",
                    PaanMenuType.AggrHospitalFees, 2));
                items.Add(NewItemLang("UCExecuteRoom.menu1.Text", "Hồ sơ điều trị",
                    PaanMenuType.TreatmentList, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnServiceReqList.Text", "Danh sách y lệnh",
                    PaanMenuType.ServiceReqList, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnTreatmentHistory.Text", "Lịch sử điều trị",
                    PaanMenuType.TreatmentHistory, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnSummaryInforTreatmentRecords.Text", "Thông tin bệnh án",
                    PaanMenuType.SummaryInforTreatmentRecords, 1));
                items.Add(NewItemLang("UCExecuteRoom.btnOtherForms.Text", "Biểu mẫu khác",
                    PaanMenuType.OtherForms, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnBenhAnNgoaiTru.Text", "Bệnh án ngoại trú",
                    PaanMenuType.BenhAnNgoaiTru, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnDebate.Text", "Biên bản hội chẩn",
                    PaanMenuType.Debate, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnMoiHoiChan.Text", "Mời hội chẩn",
                    PaanMenuType.MoiHoiChan, 3));

                // "Chi dinh giai phau benh ly" - dac biet co y nghia o man nay.
                items.Add(NewItemLang("UCExecuteRoom.btnAssignPaan.Text", "Chỉ định giải phẫu bệnh lý",
                    PaanMenuType.AssignPaan, 3));

                items.Add(NewItem("Phân loại bệnh nhân", PaanMenuType.PhanLoaiBenhNhan, 3));
                items.Add(NewItemLang("UCExecuteRoom.menu4.Text", "Thẻ dị ứng",
                    PaanMenuType.AllergyCard, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnThongTinChuyenDen.Text", "Thông tin chuyển đến",
                    PaanMenuType.ThongTinChuyenDen, 3));
                items.Add(NewItemLang("UCExecuteRoom.btnHivTreatment.Text", "Thông tin điều trị HIV/AIDS",
                    PaanMenuType.HivTreatment, 3));
                items.Add(NewItem("Thông tin điều trị bệnh lao", PaanMenuType.TuberclusisTreatment, 3));

                menu.AddItems(items.ToArray());
                menu.ShowPopup(System.Windows.Forms.Control.MousePosition);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tao mot muc menu, nhan lay tu bo tai nguyen ngon ngu cua man cu.
        /// Neu khong tim thay khoa thi dung nhan du phong truyen vao.
        /// </summary>
        private BarButtonItem NewItemLang(string languageKey, string fallback,
            PaanMenuType type, int imageIndex)
        {
            string caption = fallback;
            try
            {
                if (ResourceLangManager.LanguageUCExecuteRoom != null)
                {
                    string value = Inventec.Common.Resource.Get.Value(
                        languageKey,
                        ResourceLangManager.LanguageUCExecuteRoom,
                        LanguageManager.GetCulture());

                    if (!String.IsNullOrWhiteSpace(value)) caption = value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return NewItem(caption, type, imageIndex);
        }

        /// <summary>Tao mot muc menu voi nhan co dinh.</summary>
        private BarButtonItem NewItem(string caption, PaanMenuType type, int imageIndex)
        {
            BarButtonItem item = new BarButtonItem(barManager, caption, imageIndex);
            item.Tag = type;
            item.ItemClick += new ItemClickEventHandler(Item_ItemClick);
            return item;
        }

        private void Item_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item != null && e.Item.Tag is PaanMenuType && onClick != null)
                {
                    onClick((PaanMenuType)e.Item.Tag);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
