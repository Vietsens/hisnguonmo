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
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    delegate void MouseRight_Click(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_EXP_MEST_2 pData2;
        V_HIS_EXP_MEST pData;
        V_HIS_MEDI_STOCK medistock;
        BarManager barManager;
        MouseRight_Click mouseRight_Click;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<HIS_IMP_MEST> listImpMest;
        PopupMenu popupMenu;
        long statusIdCheckForButtonEdit;
        long mediStockId;
        long impMediStockId;
        long expMestTypeId;
        string creator;
        string LoggingName;

        internal PopupMenuProcessor() { }

        internal PopupMenuProcessor(ExpMestADO expMestAdo, V_HIS_EXP_MEST expMest, V_HIS_EXP_MEST_2 expMest2, BarManager barManager, MouseRight_Click mouseRight_Click)
        {
            try
            {
                this.barManager = barManager;
                this.pData = expMest;
                this.pData2 = expMest2;
                this.mouseRight_Click = mouseRight_Click;
                this.medistock = expMestAdo.MediStock;
                this.LoggingName = expMestAdo.LoginName;
                this.controlAcs = expMestAdo.controlAcs;
                this.listImpMest = expMestAdo.listImpMest;
                this.statusIdCheckForButtonEdit = expMest.EXP_MEST_STT_ID;
                this.mediStockId = expMest.MEDI_STOCK_ID;
                this.impMediStockId = expMest.IMP_MEDI_STOCK_ID ?? 0;
                this.expMestTypeId = expMest.EXP_MEST_TYPE_ID;
                this.creator = expMest.CREATOR;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum ItemType
        {
            XemChiTiet,
            Sua,
            Xoa,
            Duyet,
            HuyDuyet,
            TuChoiDuyet,
            HuyTuChoiDuyet,
            ThucXuat,
            HuyThucXuat,
            HoanThanh,
            TaoPhieuNhapHaoPhiTraLai,
            TaoPhieuNhapThuHoiMau,
            TaoPhieuNhapThuHoi,
            TaoPhieuNhapBuCoSo,
            TaoPhieuNhapChuyenKho,
            TaoPhieuNhapBuLe,
            ChiDinhXetNghiem,
            LichSuTacDong,
            ThuHoiDonPhongKham,
            KhongLay,
            PhucHoiDonKhongLay,
            XacNhanNo,
            LyDoXuat,
            InHoaDonDt
        }

        internal void InitMenu()
        {
            try
            {
                long currentDepartment = WorkPlace.GetDepartmentId();
                if (this.barManager == null || this.mouseRight_Click == null)
                    return;
                if (this.popupMenu == null)
                    this.popupMenu = new PopupMenu(this.barManager);

                #region Thao tác

                //BarSubItem barSubThaoTac = new BarSubItem(this.barManager, "Thao tác", 1);

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT ||
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                        && (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN && !pData.DEBT_ID.HasValue)
                        )
                    {
                        BarButtonItem bbtSua = new BarButtonItem(this.barManager, "Sửa", 1);
                        bbtSua.Tag = ItemType.Sua;
                        bbtSua.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        popupMenu.ItemLinks.Add(bbtSua);
                    }
                }

                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    if (this.medistock == null && currentDepartment == pData.REQ_DEPARTMENT_ID)
                    {
                        BarButtonItem bbtXoa = new BarButtonItem(this.barManager, "Xóa", 1);
                        bbtXoa.Tag = ItemType.Xoa;
                        bbtXoa.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        popupMenu.ItemLinks.Add(bbtXoa);
                    }
                }
                else if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                        (
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                        && (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN && !pData.DEBT_ID.HasValue)
                        )
                    {
                        if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            if (medistock == null)
                            {
                                BarButtonItem bbtXoa = new BarButtonItem(this.barManager, "Xóa", 1);
                                bbtXoa.Tag = ItemType.Xoa;
                                bbtXoa.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtXoa);
                            }
                        }
                        else
                        {
                            BarButtonItem bbtXoa = new BarButtonItem(this.barManager, "Xóa", 1);
                            bbtXoa.Tag = ItemType.Xoa;
                            bbtXoa.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                            popupMenu.ItemLinks.Add(bbtXoa);
                        }
                    }
                }

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if (pData.IS_NOT_TAKEN == 1 || (pData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && HisConfigCFG.MUST_CONFIRM_BEFORE_APPROVE == "1" && pData.IS_CONFIRM != 1))
                    {
                    }
                    else
                    {
                        if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                                 )
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                BarButtonItem bbtDuyet = new BarButtonItem(this.barManager, "Duyệt", 1);
                                bbtDuyet.Tag = ItemType.Duyet;
                                bbtDuyet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtDuyet);
                            }
                            else if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                BarButtonItem bbtDuyet = new BarButtonItem(this.barManager, "Duyệt", 1);
                                bbtDuyet.Tag = ItemType.Duyet;
                                bbtDuyet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtDuyet);
                            }
                        }
                    }
                }

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    if (pData.IS_NOT_TAKEN == 1)
                    {
                    }
                    else
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                            )
                        {
                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            {
                                BarButtonItem bbtHuyTuChoiDuyet = new BarButtonItem(this.barManager, "Hủy từ chối duyệt", 1);
                                bbtHuyTuChoiDuyet.Tag = ItemType.HuyTuChoiDuyet;
                                bbtHuyTuChoiDuyet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtHuyTuChoiDuyet);
                            }
                            else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                BarButtonItem bbtTuChoiDuyet = new BarButtonItem(this.barManager, "Từ chối duyệt", 1);
                                bbtTuChoiDuyet.Tag = ItemType.TuChoiDuyet;
                                bbtTuChoiDuyet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtTuChoiDuyet);
                            }
                        }
                    }
                }

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if (pData.IS_NOT_TAKEN == 1)
                    {
                    }
                    else
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (controlAcs != null
                                && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnExport) != null
                                && (!HisConfigCFG.EXPORT_SALE__MUST_BILL || pData.BILL_ID.HasValue || pData.DEBT_ID.HasValue))
                            {
                                BarButtonItem bbtThucXuat = new BarButtonItem(this.barManager, "Thực xuất", 1);
                                bbtThucXuat.Tag = ItemType.ThucXuat;
                                bbtThucXuat.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtThucXuat);
                            }
                        }
                        else if (
                            statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                            && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnCancelExport) != null)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    BarButtonItem bbtHuyThucXuat = new BarButtonItem(this.barManager, "Hủy thực xuất", 1);
                                    bbtHuyThucXuat.Tag = ItemType.HuyThucXuat;
                                    bbtHuyThucXuat.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                    popupMenu.ItemLinks.Add(bbtHuyThucXuat);
                                }
                            }
                        }
                    }
                }

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if (pData.IS_NOT_TAKEN == 1)
                    {
                    }
                    else
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            if (medistock != null && medistock.ID == mediStockId
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                        )
                            {
                                BarButtonItem bbtHuyDuyet = new BarButtonItem(this.barManager, "Hủy duyệt", 1);
                                bbtHuyDuyet.Tag = ItemType.HuyDuyet;
                                bbtHuyDuyet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                popupMenu.ItemLinks.Add(bbtHuyDuyet);
                            }
                        }
                    }
                }

                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                            && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && pData.IS_EXPORT_EQUAL_APPROVE == 1
                            && medistock != null
                            && pData.MEDI_STOCK_ID == medistock.ID)
                {
                    BarButtonItem bbtHoanThanh = new BarButtonItem(this.barManager, "Hoàn thành", 1);
                    bbtHoanThanh.Tag = ItemType.HoanThanh;
                    bbtHoanThanh.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                    popupMenu.ItemLinks.Add(bbtHoanThanh);
                }

                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN
                        && !pData.BILL_ID.HasValue && !pData.DEBT_ID.HasValue)
                {
                    BarButtonItem bbtXacNhanNo = new BarButtonItem(this.barManager, "Xác nhận nợ", 3);
                    bbtXacNhanNo.Tag = ItemType.XacNhanNo;
                    bbtXacNhanNo.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                    this.popupMenu.AddItems(new BarItem[] { bbtXacNhanNo });
                }

                //if (barSubThaoTac.ItemLinks.Count > 0)
                //{
                //    this.popupMenu.AddItems(new BarItem[] { barSubThaoTac });
                //}

                #endregion

                BarButtonItem bbtXemChiTiet = new BarButtonItem(this.barManager, "Xem chi tiết", 2);
                bbtXemChiTiet.Tag = ItemType.XemChiTiet;
                bbtXemChiTiet.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                this.popupMenu.AddItems(new BarItem[] { bbtXemChiTiet });

                if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                    {
                        BarButtonItem bbtTaoPhieuNhapHaoPhiTraLai = new BarButtonItem(this.barManager, "Tạo nhập hao phí trả lại", 3);
                        bbtTaoPhieuNhapHaoPhiTraLai.Tag = ItemType.TaoPhieuNhapHaoPhiTraLai;
                        bbtTaoPhieuNhapHaoPhiTraLai.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapHaoPhiTraLai });
                    }
                    else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        BarButtonItem bbtTaoPhieuNhapThuHoiMau = new BarButtonItem(this.barManager, "Tạo nhập thu hồi máu", 3);
                        bbtTaoPhieuNhapThuHoiMau.Tag = ItemType.TaoPhieuNhapThuHoiMau;
                        bbtTaoPhieuNhapThuHoiMau.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapThuHoiMau });
                    }
                    else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                        && HisConfigs.Get<string>("HIS_EXP_MEST.EXP_MEST_TYPE.EXAM_PRES.IS_ALLOW_MOBA") == "1"
                        && medistock != null && medistock.ID == mediStockId
                        )
                    {
                        BarButtonItem bbtTaoPhieuNhapThuHoiDPK = new BarButtonItem(this.barManager, "Tạo nhập thu hồi đơn phòng khám", 3);
                        bbtTaoPhieuNhapThuHoiDPK.Tag = ItemType.ThuHoiDonPhongKham;
                        bbtTaoPhieuNhapThuHoiDPK.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapThuHoiDPK });
                    }
                    else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    {
                        BarButtonItem bbtTaoPhieuNhapThuHoi = new BarButtonItem(this.barManager, "Tạo nhập thu hồi", 3);
                        bbtTaoPhieuNhapThuHoi.Tag = ItemType.TaoPhieuNhapThuHoi;
                        bbtTaoPhieuNhapThuHoi.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapThuHoi });
                    }
                }
                else
                {
                    if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN
                        && !pData.BILL_ID.HasValue && !pData.DEBT_ID.HasValue))
                    {
                        BarButtonItem bbtKhongLay = new BarButtonItem(this.barManager, "Đánh dấu đơn bệnh nhân không lấy", 3);
                        bbtKhongLay.Tag = ItemType.KhongLay;
                        bbtKhongLay.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtKhongLay });
                    }

                }
                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if (
                        expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && (
                        statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                    {
                        BarButtonItem bbtChiDinhXetNghiem = new BarButtonItem(this.barManager, "Chỉ định xét nghiệm", 4);
                        bbtChiDinhXetNghiem.Tag = ItemType.ChiDinhXetNghiem;
                        bbtChiDinhXetNghiem.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtChiDinhXetNghiem });
                    }
                }

                if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                {
                    if (medistock != null && medistock.ID == impMediStockId && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS))
                    {

                        BarButtonItem bbtTaoPhieuNhapBuCoSo = new BarButtonItem(this.barManager, "Tạo phiếu nhập bù cơ số", 5);
                        bbtTaoPhieuNhapBuCoSo.Tag = ItemType.TaoPhieuNhapBuCoSo;
                        bbtTaoPhieuNhapBuCoSo.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapBuCoSo });

                    }
                    else if (medistock != null && medistock.ID == impMediStockId && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL))
                    {
                        BarButtonItem bbtTaoPhieuNhapBuLe = new BarButtonItem(this.barManager, "Tạo phiếu nhập bù lẻ", 5);
                        bbtTaoPhieuNhapBuLe.Tag = ItemType.TaoPhieuNhapBuLe;
                        bbtTaoPhieuNhapBuLe.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                        this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapBuLe });
                    }
                    else if (medistock != null && medistock.ID == impMediStockId && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK))
                    {
                        if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            if (listImpMest != null && listImpMest.Count > 0)
                            {
                                var impMests = listImpMest.Where(o => o.CHMS_EXP_MEST_ID == pData.ID).ToList();
                                if (impMests != null && impMests.Count > 0)
                                {
                                }
                                else
                                {
                                    BarButtonItem bbtTaoPhieuNhapChuyenKho = new BarButtonItem(this.barManager, "Tạo phiếu nhập chuyển kho", 5);
                                    bbtTaoPhieuNhapChuyenKho.Tag = ItemType.TaoPhieuNhapChuyenKho;
                                    bbtTaoPhieuNhapChuyenKho.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                    this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapChuyenKho });
                                }
                            }
                            else
                            {
                                BarButtonItem bbtTaoPhieuNhapChuyenKho = new BarButtonItem(this.barManager, "Tạo phiếu nhập chuyển kho", 5);
                                bbtTaoPhieuNhapChuyenKho.Tag = ItemType.TaoPhieuNhapChuyenKho;
                                bbtTaoPhieuNhapChuyenKho.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                                this.popupMenu.AddItems(new BarItem[] { bbtTaoPhieuNhapChuyenKho });
                            }
                        }
                    }
                }

                if ((expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT) && pData.IS_NOT_TAKEN == 1 && pData.MEDI_STOCK_ID == this.medistock.ID)
                {
                    BarButtonItem bbtPhucHoi = new BarButtonItem(this.barManager, "Phục hồi đơn không lấy", 4);
                    bbtPhucHoi.Tag = ItemType.PhucHoiDonKhongLay;
                    bbtPhucHoi.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                    this.popupMenu.AddItems(new BarItem[] { bbtPhucHoi });
                }

                BarButtonItem bbtLichSuTacDong = new BarButtonItem(this.barManager, "Lịch sử tác động", 6);
                bbtLichSuTacDong.Tag = ItemType.LichSuTacDong;
                bbtLichSuTacDong.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                this.popupMenu.AddItems(new BarItem[] { bbtLichSuTacDong });

                if ((this.pData.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnLyDoXuat) != null))
                    && ((medistock != null && this.pData.MEDI_STOCK_ID == this.medistock.ID) || this.pData.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                {
                    BarButtonItem bbtnLyDoXuat = new BarButtonItem(this.barManager, "Lý do xuất", 7);
                    bbtnLyDoXuat.Tag = ItemType.LyDoXuat;
                    bbtnLyDoXuat.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                    this.popupMenu.AddItems(new BarItem[] { bbtnLyDoXuat });
                }
                if (!string.IsNullOrEmpty(pData2.INVOICE_CODE))
                {
                    BarButtonItem bbtInHoaDonDienTu = new BarButtonItem(this.barManager, "In hóa đơn điện tử", 6);
                    bbtInHoaDonDienTu.Tag = ItemType.InHoaDonDt;
                    bbtInHoaDonDienTu.ItemClick += new ItemClickEventHandler(this.mouseRight_Click);
                    this.popupMenu.AddItems(new BarItem[] { bbtInHoaDonDienTu });
                }
                this.popupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
