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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraBars;
using EMR_MAIN;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Process;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.ConfigKeys;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord
{
    public class MediRecordMenuPopupProcessor
    {
        private string BUTTON__CONFIG__CODE = "HIS000025";
        //private List<BarItemADO> _LstBarItem = new List<BarItemADO>();
        private List<HIS_EMR_COVER_TYPE> _LstEmrCoverType = new List<HIS_EMR_COVER_TYPE>();
        private List<HIS_EMR_FORM> _LstEmrForm = new List<HIS_EMR_FORM>();
        private List<SDA_HIDE_CONTROL> _HideControls;
        private EmrInputADO inputAdo;

        public enum MenuType
        {
            EMR_YeuCauSuDungKhangSinh,
            Emr_NoiKhoa,
            Emr_NgoaiKhoa,
            Emr_DaLieu,
            Emr_Bong,
            Emr_HuyetHocTruyenMau,
            Emr_MatBanPhanTruoc,
            Emr_MatChanThuong,
            Emr_DayMat,
            Emr_Glocom,
            Emr_MatLac,
            Emr_MatTreEm,
            Emr_NgoaiTru,
            Emr_NgoaiTruRangHamMat,
            Emr_NgoaiTruTaiMuiHong,
            Emr_NgoaiTruYHocCoTruyen,
            Emr_NhiKhoa,
            Emr_NoiTruYHocCoTruyen,
            Emr_NoiTruNhiYHCT,
            Emr_DieuTriBanNgay,
            Emr_BANTBanNgayYHCT,
            Emr_PhucHoiChucNang,
            Emr_PhuKhoa,
            Emr_RangHamMat,
            Emr_SanKhoa,
            Emr_SoSinh,
            Emr_TaiMuiHong,
            Emr_TamThan,
            Emr_TruyenNhiem,
            Emr_UngBieu,
            Emr_XaPhuong,
            Emr_Tim,
            Emr_LuuCapCuu,
            Emr_Mat,
            Emr_PhaThai1,
            Emr_PhaThai2,
            Emr_PhaThai3,
            Emr_YHCTPhucHoiChucNang,
            Emr_ThanNhantao,
            Emr_CMU,
            Emr_NgoaiTruPhongKham,
            Emr_TayChanMieng,
            Emr_NgoaiTruMat,
            Emr_NgoaiTruPhucHoiChucNang,
            Emr_DaLieuTrungUong,
            Emr_NhiKhoaPhucHoiChucNang,
            Emr_PhucHoiChucNangNoiTru,
            Emr_TruyenNhiemII,
            Emr_NgoaiTruVayNenThongThuong,
            Emr_NgoaiTruHoTroSinhSan,
            Emr_NgoaiTruAVayNen,
            Emr_NgoaiTruPemphigoid,
            Emr_NgoaiTruVayPhanDoNangLong,
            Emr_NgoaiTruLupusBanDoManTinh,
            Emr_NgoaiTruLupusBanDoHeThong,
            Emr_NgoaiTruViemBiCo,
            Emr_HoiChungTrungLap,
            Emr_DuhringBrocq,
            Emr_NgoaiTruPemphigus,
            Emr_NgoaiTruUngThuDaHacTo,
            Emr_NgoaiTruUngThuDaKhongHacTo,
            Emr_NgoaiTruVayNenTheKhop,
            Emr_NgoaiTruVayNenTheMu,
            Emr_DaiThaoDuong,
            Emr_NgoaiTruHIV,
            Emr_BenhAnSuyTim,
            Emr_ThoiDoiVaDieuTriCoKiemSoatStentDongMachVanh,
            Emr_ThoiDoiVaDieuTriBenhBasedow,
            Emr_TheoDoiQuanLyDieuTriCoKiemSoatThieuMauCoTimCucBo,
            Emr_TheoDoiQuanLyBenhPhoiTacNghenManTinh,
            Emr_TheoDoiDieuTRiBenhViemGanSieuViB,
            Emr_BenhAnTangHuyetAp,
            Emr_NgoaiTruhenPheQuan,
            Emr_XoCungBiHeThong,
            Emr_XoCungbiKhutru,
            Emr_NgoaiTruDaLieuTW,
            Emr_NoiKhoaThan
        }

        public MediRecordMenuPopupProcessor()
        {
            this._HideControls = HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.GetByModule("HIS.Desktop.Plugins.Library.FormMedicalRecord");
            HisConfigCFG.LoadConfig();
        }

        public void InitMenu(PopupMenu _Menu, BarManager barManager, EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;
                this._LstEmrCoverType = new List<HIS_EMR_COVER_TYPE>();

                this._LstEmrCoverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().ToList();
             
                this._LstEmrForm = new List<HIS_EMR_FORM>();
                this._LstEmrForm = BackendDataWorker.Get<HIS_EMR_FORM>().ToList();

                long UsingFormVersion = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.IS_HIDE_COVER_TYPE));


                string EmrFormOptionCS = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.EMR_FORM_OPTION)) == 1 ? "CS2" : "CS";

                if (UsingFormVersion == 1 && this.inputAdo.EmrCoverTypeId != null)
                {
                    if (this.inputAdo.EmrCoverTypeId != null)
                    {
                        string ten = string.Format("Vỏ bệnh án ({0})", NameMedicalRecord(this.inputAdo.EmrCoverTypeId.Value));
                        BarButtonItem bbtnEmr2 = new BarButtonItem(barManager, ten, 4);
                        bbtnEmr2.Tag = this.inputAdo.EmrCoverTypeId.Value;
                        bbtnEmr2.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        _Menu.AddItem(bbtnEmr2);
                    }

                    BarSubItem bbtnEmrForm = new BarSubItem(barManager, "Phiếu vỏ bệnh án", 4);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu chăm sóc", EmrFormOptionCS, this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi và truyền dịch", "TD", this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi chức năng sống", "PTDCNS", this.EmrFormMouseRight_Click);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Khác", "", this.FromPhieu__RightClick);

                    _Menu.AddItems(new BarItem[] { bbtnEmrForm });
                    
                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            _Menu.ItemLinks.Add(bbtnErm);
                        }
                    }
                }
                else if (this.inputAdo.lstEmrCoverTypeId != null && this.inputAdo.lstEmrCoverTypeId.Count > 0)
                {
                    BarSubItem sub2 = new BarSubItem(barManager, "Vỏ bệnh án", 4);

                    foreach (var item in this.inputAdo.lstEmrCoverTypeId)
                    {
                        string ten = string.Format("{0}", NameMedicalRecord(item));
                        //BarButtonItem bbtnEmr3 = new BarButtonItem(barManager, ten, 4);
                        //bbtnEmr3.Tag = item;
                        //bbtnEmr3.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        //_Menu.AddItem(bbtnEmr3);
                        this.AddBarItemList(barManager, sub2, ten, item, item, this.EmrMouseRightsingle_Click);
                    }
                    _Menu.AddItems(new BarItem[] { sub2 });

                    BarSubItem bbtnEmrForm = new BarSubItem(barManager, "Phiếu vỏ bệnh án", 4);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu chăm sóc", EmrFormOptionCS, this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi và truyền dịch", "TD", this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi chức năng sống", "PTDCNS", this.EmrFormMouseRight_Click);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Khác", "", this.FromPhieu__RightClick);

                    _Menu.AddItems(new BarItem[] { bbtnEmrForm });

                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            _Menu.ItemLinks.Add(bbtnErm);
                        }
                    }
                }
                else
                {
                    BarSubItem sub1 = new BarSubItem(barManager, "Vỏ bệnh án", 4);
                    //sub1.Visibility = BarItemVisibility.Never;

                    //this._LstBarItem = new List<BarItemADO>();

                    //BarSubItem subPhieu = new BarSubItem(barManager, "Phiếu", 4);
                    //subPhieu.Visibility = BarItemVisibility.Never;
                    //this.AddBarItem(barManager, subPhieu, "Yêu cầu sử dụng Kháng sinh", MenuType.EMR_YeuCauSuDungKhangSinh, this.EmrMouseRight_Click);
                    //sub1.AddItems(new BarItem[] { subPhieu });

                    this.AddBarItem(barManager, sub1, "Nội khoa", MenuType.Emr_NoiKhoa, LoaiBenhAnEMR.NoiKhoa, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Ngoại khoa", MenuType.Emr_NgoaiKhoa, LoaiBenhAnEMR.NgoaiKhoa, this.EmrMouseRight_Click);

                    BarSubItem subDaLieu = new BarSubItem(barManager, "Da liễu", 4);
                    subDaLieu.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subDaLieu, "Da liễu", MenuType.Emr_DaLieu, LoaiBenhAnEMR.DaLieu, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subDaLieu, "Da liễu _ Trung ương", MenuType.Emr_DaLieuTrungUong, LoaiBenhAnEMR.DaLieuTW, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subDaLieu });

                    this.AddBarItem(barManager, sub1, "Bỏng", MenuType.Emr_Bong, LoaiBenhAnEMR.Bong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "CMU", MenuType.Emr_CMU, LoaiBenhAnEMR.CMU, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Đái tháo đường", MenuType.Emr_DaiThaoDuong, LoaiBenhAnEMR.DaiThaoDuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Hồ sơ bệnh án suy tim", MenuType.Emr_BenhAnSuyTim, LoaiBenhAnEMR.BenhAnSuyTim, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Huyết học truyền máu", MenuType.Emr_HuyetHocTruyenMau, LoaiBenhAnEMR.HuyetHocTruyenMau, this.EmrMouseRight_Click);

                    BarSubItem subMat = new BarSubItem(barManager, "Mắt", 4);
                    subMat.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subMat, "Mắt", MenuType.Emr_Mat, LoaiBenhAnEMR.Mat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Bán phần trước", MenuType.Emr_MatBanPhanTruoc, LoaiBenhAnEMR.MatBanPhanTruoc, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Chấn thương", MenuType.Emr_MatChanThuong, LoaiBenhAnEMR.MatChanThuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Đáy mắt", MenuType.Emr_DayMat, LoaiBenhAnEMR.MatDayMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Glocom", MenuType.Emr_Glocom, LoaiBenhAnEMR.MatGloCom, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Mắt lác", MenuType.Emr_MatLac, LoaiBenhAnEMR.MatLac, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Mắt trẻ em", MenuType.Emr_MatTreEm, LoaiBenhAnEMR.MatTreEm, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subMat });

                    BarSubItem subNgoaiTru = new BarSubItem(barManager, "Ngoại trú", 4);
                    subNgoaiTru.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú", MenuType.Emr_NgoaiTru, LoaiBenhAnEMR.NgoaiTru, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Á vảy nến", MenuType.Emr_NgoaiTruAVayNen, LoaiBenhAnEMR.NgoaiTru_AVayNen, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Duhring Brocq", MenuType.Emr_DuhringBrocq, LoaiBenhAnEMR.NgoaiTru_DuhringBrocq, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hen phế quản", MenuType.Emr_NgoaiTruhenPheQuan, LoaiBenhAnEMR.BenhAnHenPheQuan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hỗ trợ sinh sản", MenuType.Emr_NgoaiTruHoTroSinhSan, LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hội chứng trùng lặp", MenuType.Emr_HoiChungTrungLap, LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ HIV", MenuType.Emr_NgoaiTruHIV, LoaiBenhAnEMR.BenhAnNgoaiTruHIV, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Lupus ban đỏ hệ thống", MenuType.Emr_NgoaiTruLupusBanDoHeThong, LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Lupus ban đỏ mạn tính", MenuType.Emr_NgoaiTruLupusBanDoManTinh, LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Mắt", MenuType.Emr_NgoaiTruMat, LoaiBenhAnEMR.NgoaiTruMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Răng hàm mặt", MenuType.Emr_NgoaiTruRangHamMat, LoaiBenhAnEMR.NgoaiTruRangHamMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ PEMPHIGOID", MenuType.Emr_NgoaiTruPemphigoid, LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Pemphigus", MenuType.Emr_NgoaiTruPemphigus, LoaiBenhAnEMR.NgoaiTru_Pemphigus, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phẫu thuật", MenuType.Emr_NgoaiTruDaLieuTW, LoaiBenhAnEMR.NgoaiTru_DaLieuTW, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phòng khám", MenuType.Emr_NgoaiTruPhongKham, LoaiBenhAnEMR.BANgoaTruPK, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phục hồi chức năng", MenuType.Emr_NgoaiTruPhucHoiChucNang, LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Ung thư hắc tố", MenuType.Emr_NgoaiTruUngThuDaHacTo, LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Ung thư không hắc tố", MenuType.Emr_NgoaiTruUngThuDaKhongHacTo, LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Xơ cứng bì hệ thống", MenuType.Emr_XoCungBiHeThong, LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Xơ cứng bì khu trú", MenuType.Emr_XoCungbiKhutru, LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thể mủ", MenuType.Emr_NgoaiTruVayNenTheMu, LoaiBenhAnEMR.NgoaiTru_VayNenTheMu, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thể khớp", MenuType.Emr_NgoaiTruVayNenTheKhop, LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thông thường", MenuType.Emr_NgoaiTruVayNenThongThuong, LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Viêm bì cơ", MenuType.Emr_NgoaiTruViemBiCo, LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy phấn đỏ nang lông", MenuType.Emr_NgoaiTruVayPhanDoNangLong, LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Tai mũi họng", MenuType.Emr_NgoaiTruTaiMuiHong, LoaiBenhAnEMR.NgoaiTruTaiMuiHong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ YHCT", MenuType.Emr_NgoaiTruYHocCoTruyen, LoaiBenhAnEMR.NgoaiTruYHCT, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subNgoaiTru });

                    BarSubItem subNhiKhoa = new BarSubItem(barManager, "Nhi khoa", 4);
                    subNhiKhoa.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subNhiKhoa, "Nhi khoa", MenuType.Emr_NhiKhoa, LoaiBenhAnEMR.NhiKhoa, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNhiKhoa, "Nhi khoa _ Phục hồi chức năng", MenuType.Emr_NhiKhoaPhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNangNhi, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subNhiKhoa });

                    this.AddBarItem(barManager, sub1, "Nội khoa thận", MenuType.Emr_NoiKhoaThan, LoaiBenhAnEMR.NoiKhoaThan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Nội trú _ YHCT", MenuType.Emr_NoiTruYHocCoTruyen, LoaiBenhAnEMR.NoiTruYHCT, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Nội trú nhi _ YHCT", MenuType.Emr_NoiTruNhiYHCT, LoaiBenhAnEMR.NoiTruNhiYHCT, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "YHCT-Phục hồi chức năng", MenuType.Emr_YHCTPhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNangYHCT, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Điều trị ban ngày", MenuType.Emr_DieuTriBanNgay, LoaiBenhAnEMR.DieuTriBanNgay, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Nội trú ban ngày - YHCT", MenuType.Emr_BANTBanNgayYHCT, LoaiBenhAnEMR.BANTBanNgayYHCT, this.EmrMouseRight_Click);

                    BarSubItem subPhaThai = new BarSubItem(barManager, "Phá thai", 4);
                    subPhaThai.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subPhaThai, "Phá thai I", MenuType.Emr_PhaThai1, LoaiBenhAnEMR.PhaThaiI, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subPhaThai, "Phá thai II", MenuType.Emr_PhaThai2, LoaiBenhAnEMR.PhaThaiII, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subPhaThai, "Phá thai III", MenuType.Emr_PhaThai3, LoaiBenhAnEMR.PhaThaiIII, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subPhaThai });

                    this.AddBarItem(barManager, sub1, "Phục hồi chức năng", MenuType.Emr_PhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNang, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Phục hồi chức năng nội trú", MenuType.Emr_PhucHoiChucNangNoiTru, LoaiBenhAnEMR.PHCNII, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Phụ khoa", MenuType.Emr_PhuKhoa, LoaiBenhAnEMR.PhuKhoa, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Răng hàm mặt", MenuType.Emr_RangHamMat, LoaiBenhAnEMR.RangHamMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Sản khoa", MenuType.Emr_SanKhoa, LoaiBenhAnEMR.SanKhoa, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Sơ sinh", MenuType.Emr_SoSinh, LoaiBenhAnEMR.SoSinh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Tai mũi họng", MenuType.Emr_TaiMuiHong, LoaiBenhAnEMR.TaiMuiHong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Tay Chân Miệng", MenuType.Emr_TayChanMieng, LoaiBenhAnEMR.TayChanMieng, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Tâm thần", MenuType.Emr_TamThan, LoaiBenhAnEMR.TamThan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Tăng huyết áp", MenuType.Emr_BenhAnTangHuyetAp, LoaiBenhAnEMR.BenhTangHuyetAp, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Thận nhân tạo", MenuType.Emr_ThanNhantao, LoaiBenhAnEMR.ThanNhanTao, this.EmrMouseRight_Click);

                    BarSubItem subTheoDoi = new BarSubItem(barManager, "Theo dõi", 4);
                    subTheoDoi.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi và điều trị có kiểm soát stent động mạnh vành", MenuType.Emr_ThoiDoiVaDieuTriCoKiemSoatStentDongMachVanh, LoaiBenhAnEMR.StentDongMachVanh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi và điều trị bệnh Basedow", MenuType.Emr_ThoiDoiVaDieuTriBenhBasedow, LoaiBenhAnEMR.NgoaiTru_BenhBasedow, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi điều trị bệnh viêm gan siêu vi B", MenuType.Emr_TheoDoiDieuTRiBenhViemGanSieuViB, LoaiBenhAnEMR.ViemGanBManTinh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi quản lý điều trị có kiểm soát thiếu máu cơ tim cục bộ", MenuType.Emr_TheoDoiQuanLyDieuTriCoKiemSoatThieuMauCoTimCucBo, LoaiBenhAnEMR.ThieuMauCoTimCucBo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi quản lý bệnh phổi tắc nghẽn mãn tính", MenuType.Emr_TheoDoiQuanLyBenhPhoiTacNghenManTinh, LoaiBenhAnEMR.PhoiTacNghenManTinh, this.EmrMouseRight_Click);

                    sub1.AddItems(new BarItem[] { subTheoDoi });

                    this.AddBarItem(barManager, sub1, "Truyền nhiễm", MenuType.Emr_TruyenNhiem, LoaiBenhAnEMR.TruyenNhiem, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Truyền nhiễm II", MenuType.Emr_TruyenNhiemII, LoaiBenhAnEMR.TruyenNhiemII, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Ung bướu", MenuType.Emr_UngBieu, LoaiBenhAnEMR.UngBuou, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Xã phường", MenuType.Emr_XaPhuong, LoaiBenhAnEMR.XaPhuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Tim", MenuType.Emr_Tim, LoaiBenhAnEMR.Tim, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, sub1, "Lưu cấp cứu", MenuType.Emr_LuuCapCuu, LoaiBenhAnEMR.LuuCapCuu, this.EmrMouseRight_Click);

                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            sub1.ItemLinks.Add(bbtnErm);
                        }
                    }

                    _Menu.AddItems(new BarItem[] { sub1 });

                    BarSubItem bbtnEmrForm = new BarSubItem(barManager, "Phiếu vỏ bệnh án", 4);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu chăm sóc", EmrFormOptionCS, this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi và truyền dịch", "TD", this.EmrFormMouseRight_Click);
                    this.AddBarItemForm(barManager, bbtnEmrForm, "Phiếu theo dõi chức năng sống", "PTDCNS", this.EmrFormMouseRight_Click);

                    this.AddBarItemForm(barManager, bbtnEmrForm, "Khác", "", this.FromPhieu__RightClick);

                    _Menu.AddItems(new BarItem[] { bbtnEmrForm });

                    if (this.inputAdo.EmrCoverTypeId != null)
                    {
                        string ten = string.Format("Vỏ bệnh án ({0})", NameMedicalRecord(this.inputAdo.EmrCoverTypeId.Value));
                        BarButtonItem bbtnEmr2 = new BarButtonItem(barManager, ten, 4);
                        bbtnEmr2.Tag = this.inputAdo.EmrCoverTypeId.Value;
                        bbtnEmr2.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        _Menu.AddItem(bbtnEmr2);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitMenuButton(PopupMenu _Menu, BarManager barManager, EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;
                this._LstEmrCoverType = new List<HIS_EMR_COVER_TYPE>();

                //CommonParam param = new CommonParam();
                //HisEmrCoverTypeFilter filter = new HisEmrCoverTypeFilter();
                ////filter.ORDER_DIRECTION = "ACS";
                ////filter.ORDER_FIELD = "ID";
                //this._LstEmrCoverType =  new BackendAdapter(param).Get<List<HIS_EMR_COVER_TYPE>>("api/HisEmrCoverType/Get", ApiConsumers.MosConsumer, filter, param);

                this._LstEmrCoverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().ToList();                              

                if (this.inputAdo.EmrCoverTypeId != null)
                {
                    Emrsingle_Click(this.inputAdo);
                }
                else if (this.inputAdo.lstEmrCoverTypeId != null && this.inputAdo.lstEmrCoverTypeId.Count > 0)
                {
                    foreach (var item in this.inputAdo.lstEmrCoverTypeId)
                    {
                        string ten = string.Format("{0}", NameMedicalRecord(item));
                        //BarButtonItem bbtnEmr3 = new BarButtonItem(barManager, ten, 4);
                        //bbtnEmr3.Tag = item;
                        //bbtnEmr3.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        //_Menu.AddItem(bbtnEmr3);

                        this.AddBarItemPopupMenuList(barManager, _Menu, ten, item, item, this.EmrMouseRightsingle_Click);
                    }

                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            _Menu.ItemLinks.Add(bbtnErm);
                        }
                    }
                }
                else
                {
                    this.AddBarItemPopupMenu(barManager, _Menu, "Nội khoa", MenuType.Emr_NoiKhoa, LoaiBenhAnEMR.NoiKhoa, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Ngoại khoa", MenuType.Emr_NgoaiKhoa, LoaiBenhAnEMR.NgoaiKhoa, this.EmrMouseRight_Click);

                    BarSubItem subDaLieu = new BarSubItem(barManager, "Da liễu", 4);
                    subDaLieu.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subDaLieu, "Da liễu", MenuType.Emr_DaLieu, LoaiBenhAnEMR.DaLieu, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subDaLieu, "Da liễu _ Trung ương", MenuType.Emr_DaLieuTrungUong, LoaiBenhAnEMR.DaLieuTW, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subDaLieu });

                    this.AddBarItemPopupMenu(barManager, _Menu, "Bỏng", MenuType.Emr_Bong, LoaiBenhAnEMR.Bong, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "CMU", MenuType.Emr_CMU, LoaiBenhAnEMR.CMU, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Đái tháo đường", MenuType.Emr_DaiThaoDuong, LoaiBenhAnEMR.DaiThaoDuong, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Hồ sơ bệnh án suy tim", MenuType.Emr_BenhAnSuyTim, LoaiBenhAnEMR.BenhAnSuyTim, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Huyết học truyền máu", MenuType.Emr_HuyetHocTruyenMau, LoaiBenhAnEMR.HuyetHocTruyenMau, this.EmrMouseRight_Click);

                    BarSubItem subMat = new BarSubItem(barManager, "Mắt", 4);
                    subMat.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subMat, "Mắt", MenuType.Emr_Mat, LoaiBenhAnEMR.Mat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Bán phần trước", MenuType.Emr_MatBanPhanTruoc, LoaiBenhAnEMR.MatBanPhanTruoc, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Chấn thương", MenuType.Emr_MatChanThuong, LoaiBenhAnEMR.MatChanThuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Đáy mắt", MenuType.Emr_DayMat, LoaiBenhAnEMR.MatDayMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Glocom", MenuType.Emr_Glocom, LoaiBenhAnEMR.MatGloCom, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Mắt lác", MenuType.Emr_MatLac, LoaiBenhAnEMR.MatLac, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subMat, "Măt _ Mắt trẻ em", MenuType.Emr_MatTreEm, LoaiBenhAnEMR.MatTreEm, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subMat });

                    BarSubItem subNgoaiTru = new BarSubItem(barManager, "Ngoại trú", 4);
                    subNgoaiTru.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú", MenuType.Emr_NgoaiTru, LoaiBenhAnEMR.NgoaiTru, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Á vảy nến", MenuType.Emr_NgoaiTruAVayNen, LoaiBenhAnEMR.NgoaiTru_AVayNen, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Duhring Brocq", MenuType.Emr_DuhringBrocq, LoaiBenhAnEMR.NgoaiTru_DuhringBrocq, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hen phế quản", MenuType.Emr_NgoaiTruhenPheQuan, LoaiBenhAnEMR.BenhAnHenPheQuan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hỗ trợ sinh sản", MenuType.Emr_NgoaiTruHoTroSinhSan, LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Hội chứng trùng lặp", MenuType.Emr_HoiChungTrungLap, LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ HIV", MenuType.Emr_NgoaiTruHIV, LoaiBenhAnEMR.BenhAnNgoaiTruHIV, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Lupus ban đỏ hệ thống", MenuType.Emr_NgoaiTruLupusBanDoHeThong, LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Lupus ban đỏ mạn tính", MenuType.Emr_NgoaiTruLupusBanDoManTinh, LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Mắt", MenuType.Emr_NgoaiTruMat, LoaiBenhAnEMR.NgoaiTruMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Răng hàm mặt", MenuType.Emr_NgoaiTruRangHamMat, LoaiBenhAnEMR.NgoaiTruRangHamMat, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ PEMPHIGOID", MenuType.Emr_NgoaiTruPemphigoid, LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Pemphigus", MenuType.Emr_NgoaiTruPemphigus, LoaiBenhAnEMR.NgoaiTru_Pemphigus, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phẫu thuật", MenuType.Emr_NgoaiTruDaLieuTW, LoaiBenhAnEMR.NgoaiTru_DaLieuTW, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phòng khám", MenuType.Emr_NgoaiTruPhongKham, LoaiBenhAnEMR.BANgoaTruPK, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Phục hồi chức năng", MenuType.Emr_NgoaiTruPhucHoiChucNang, LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Ung thư hắc tố", MenuType.Emr_NgoaiTruUngThuDaHacTo, LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Ung thư không hắc tố", MenuType.Emr_NgoaiTruUngThuDaKhongHacTo, LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Xơ cứng bì hệ thống", MenuType.Emr_XoCungBiHeThong, LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Xơ cứng bì khu trú", MenuType.Emr_XoCungbiKhutru, LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thể mủ", MenuType.Emr_NgoaiTruVayNenTheMu, LoaiBenhAnEMR.NgoaiTru_VayNenTheMu, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thể khớp", MenuType.Emr_NgoaiTruVayNenTheKhop, LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy nến thông thường", MenuType.Emr_NgoaiTruVayNenThongThuong, LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Viêm bì cơ", MenuType.Emr_NgoaiTruViemBiCo, LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Vảy phấn đỏ nang lông", MenuType.Emr_NgoaiTruVayPhanDoNangLong, LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ Tai mũi họng", MenuType.Emr_NgoaiTruTaiMuiHong, LoaiBenhAnEMR.NgoaiTruTaiMuiHong, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNgoaiTru, "Ngoại trú _ YHCT", MenuType.Emr_NgoaiTruYHocCoTruyen, LoaiBenhAnEMR.NgoaiTruYHCT, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subNgoaiTru });

                    BarSubItem subNhiKhoa = new BarSubItem(barManager, "Nhi khoa", 4);
                    subNhiKhoa.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subNhiKhoa, "Nhi khoa", MenuType.Emr_NhiKhoa, LoaiBenhAnEMR.NhiKhoa, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subNhiKhoa, "Nhi khoa _ Phục hồi chức năng", MenuType.Emr_NhiKhoaPhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNangNhi, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subNhiKhoa });

                    this.AddBarItemPopupMenu(barManager, _Menu, "Nội khoa thận", MenuType.Emr_NoiKhoaThan, LoaiBenhAnEMR.NoiKhoaThan, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Nội trú _ YHCT", MenuType.Emr_NoiTruYHocCoTruyen, LoaiBenhAnEMR.NoiTruYHCT, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Nội trú nhi _ YHCT", MenuType.Emr_NoiTruNhiYHCT, LoaiBenhAnEMR.NoiTruNhiYHCT, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "YHCT-Phục hồi chức năng", MenuType.Emr_YHCTPhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNangYHCT, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Điều trị ban ngày", MenuType.Emr_DieuTriBanNgay, LoaiBenhAnEMR.DieuTriBanNgay, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Nội trú ban ngày - YHCT", MenuType.Emr_BANTBanNgayYHCT, LoaiBenhAnEMR.BANTBanNgayYHCT, this.EmrMouseRight_Click);

                    BarSubItem subPhaThai = new BarSubItem(barManager, "Phá thai", 4);
                    subPhaThai.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subPhaThai, "Phá thai I", MenuType.Emr_PhaThai1, LoaiBenhAnEMR.PhaThaiI, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subPhaThai, "Phá thai II", MenuType.Emr_PhaThai2, LoaiBenhAnEMR.PhaThaiII, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subPhaThai, "Phá thai III", MenuType.Emr_PhaThai3, LoaiBenhAnEMR.PhaThaiIII, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subPhaThai });

                    this.AddBarItemPopupMenu(barManager, _Menu, "Phục hồi chức năng", MenuType.Emr_PhucHoiChucNang, LoaiBenhAnEMR.PhucHoiChucNang, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Phục hồi chức năng nội trú", MenuType.Emr_PhucHoiChucNangNoiTru, LoaiBenhAnEMR.PHCNII, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Phụ khoa", MenuType.Emr_PhuKhoa, LoaiBenhAnEMR.PhuKhoa, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Răng hàm mặt", MenuType.Emr_RangHamMat, LoaiBenhAnEMR.RangHamMat, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Sản khoa", MenuType.Emr_SanKhoa, LoaiBenhAnEMR.SanKhoa, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Sơ sinh", MenuType.Emr_SoSinh, LoaiBenhAnEMR.SoSinh, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Tai mũi họng", MenuType.Emr_TaiMuiHong, LoaiBenhAnEMR.TaiMuiHong, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Tay Chân Miệng", MenuType.Emr_TayChanMieng, LoaiBenhAnEMR.TayChanMieng, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Tâm thần", MenuType.Emr_TamThan, LoaiBenhAnEMR.TamThan, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Tăng huyết áp", MenuType.Emr_BenhAnTangHuyetAp, LoaiBenhAnEMR.BenhTangHuyetAp, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Thận nhân tạo", MenuType.Emr_ThanNhantao, LoaiBenhAnEMR.ThanNhanTao, this.EmrMouseRight_Click);

                    BarSubItem subTheoDoi = new BarSubItem(barManager, "Theo dõi", 4);
                    subTheoDoi.Visibility = BarItemVisibility.Never;

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi và điều trị có kiểm soát stent động mạnh vành", MenuType.Emr_ThoiDoiVaDieuTriCoKiemSoatStentDongMachVanh, LoaiBenhAnEMR.StentDongMachVanh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi và điều trị bệnh Basedow", MenuType.Emr_ThoiDoiVaDieuTriBenhBasedow, LoaiBenhAnEMR.NgoaiTru_BenhBasedow, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi điều trị bệnh viêm gan siêu vi B", MenuType.Emr_TheoDoiDieuTRiBenhViemGanSieuViB, LoaiBenhAnEMR.ViemGanBManTinh, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi quản lý điều trị có kiểm soát thiếu máu cơ tim cục bộ", MenuType.Emr_TheoDoiQuanLyDieuTriCoKiemSoatThieuMauCoTimCucBo, LoaiBenhAnEMR.ThieuMauCoTimCucBo, this.EmrMouseRight_Click);

                    this.AddBarItem(barManager, subTheoDoi, "Theo dõi quản lý bệnh phổi tắc nghẽn mãn tính", MenuType.Emr_TheoDoiQuanLyBenhPhoiTacNghenManTinh, LoaiBenhAnEMR.PhoiTacNghenManTinh, this.EmrMouseRight_Click);

                    _Menu.AddItems(new BarItem[] { subTheoDoi });

                    this.AddBarItemPopupMenu(barManager, _Menu, "Truyền nhiễm", MenuType.Emr_TruyenNhiem, LoaiBenhAnEMR.TruyenNhiem, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Truyền nhiễm II", MenuType.Emr_TruyenNhiemII, LoaiBenhAnEMR.TruyenNhiemII, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Ung bướu", MenuType.Emr_UngBieu, LoaiBenhAnEMR.UngBuou, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Xã phường", MenuType.Emr_XaPhuong, LoaiBenhAnEMR.XaPhuong, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Tim", MenuType.Emr_Tim, LoaiBenhAnEMR.Tim, this.EmrMouseRight_Click);

                    this.AddBarItemPopupMenu(barManager, _Menu, "Lưu cấp cứu", MenuType.Emr_LuuCapCuu, LoaiBenhAnEMR.LuuCapCuu, this.EmrMouseRight_Click);

                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            _Menu.ItemLinks.Add(bbtnErm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FormOpenEmr(long type, EmrInputADO ado, string _MaPhieu) 
        {
            try
            {
                MediRecordProcessor processor = new MediRecordProcessor();
                processor.LoadDataEmr((LoaiBenhAnEMR)type, ado, _MaPhieu);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }


        private string NameMedicalRecord(long TypeId)
        {
            switch (TypeId)
            {
                #region Tên các loại bệnh án
                case 1:
                    return "Nội khoa";
                    break;
                case 2:
                    return "Nhi khoa";
                    break;
                case 3:
                    return "Truyền nhiễm";
                    break;
                case 4:
                    return "Phụ khoa";
                    break;
                case 5:
                    return "Sản khoa";
                    break;
                case 6:
                    return "Sơ sinh";
                    break;
                case 7:
                    return "Tâm thần";
                    break;
                case 8:
                    return "Da liễu";
                    break;
                case 9:
                    return "Phục hồi chức năng";
                    break;
                case 10:
                    return "Huyết học truyền máu";
                    break;
                case 11:
                    return "Ngoại khoa";
                    break;
                case 12:
                    return "Bỏng";
                    break;
                case 13:
                    return "Ung bướu";
                    break;
                case 14:
                    return "Răng - Hàm - Mặt";
                    break;
                case 15:
                    return "Tai - Mũi - Họng";
                    break;
                case 16:
                    return "Ngoại trú";
                    break;
                case 17:
                    return "Ngoại trú Răng - Hàm - Mặt";
                    break;
                case 18:
                    return "Ngoại trú Tai - Mũi - Họng";
                    break;
                case 19:
                    return "Xã phường";
                    break;
                case 20:
                    return "Mắt lác";
                    break;
                case 21:
                    return "Mắt trẻ em";
                    break;
                case 22:
                    return "Mắt Glôcôm";
                    break;
                case 23:
                    return "Mắt - Đáy mắt";
                    break;
                case 24:
                    return "Mắt chấn thương";
                    break;
                case 25:
                    return "Mắt bán phần trước";
                    break;
                case 26:
                    return "Nội trú YHCT";
                    break;
                case 27:
                    return "Ngoại trú YHCT";
                    break;
                case 28:
                    return "Phá thai I";
                    break;
                case 29:
                    return "Phá thai II";
                    break;
                case 30:
                    return "Điều trị ban ngày";
                    break;
                case 31:
                    return "Thận nhân tạo";
                    break;
                case 32:
                    return "Mắt";
                    break;
                case 33:
                    return "Tim";
                    break;
                case 34:
                    return "YHCT-Phục hồi chức năng";
                    break;
                case 35:
                    return "Lưu cấp cứu";
                    break;
                case 36:
                    return "CMU";
                    break;
                case 37:
                    return "Phá thai III";
                    break;
                case 38:
                    return "Bệnh án ngoại trú phòng khám";
                    break;
                case 39:
                    return "Tay Chân Miệng";
                    break;
                case 40:
                    return "Ngoại trú _ Mắt";
                    break;
                case 41:
                    return "Ngoại trú _ Phục hồi chức năng";
                    break;
                case 42:
                    return "Nhi khoa _ Phục hồi chức năng";
                    break;
                case 43:
                    return "Da liễu _ Trung ương";
                    break;
                case 44:
                    return "Phục hồi chức năng nội trú";
                    break;
                case 45:
                    return "Truyền nhiễm II";
                    break;
                case 46:
                    return "Ngoại trú _ Vẩy nến thông thường";
                    break;
                case 47:
                    return "Ngoại trú _ Á Vẩy nến";
                    break;
                case 48:
                    return "Ngoại trú _ Hỗ trợ sinh sản";
                    break;
                case 49:
                    return "Ngoại trú _ Viêm bì cơ";
                    break;
                case 50:
                    return "Ngoại trú _ Lupus ban đỏ hệ thống";
                    break;
                case 51:
                    return "Ngoại trú _ PEMPHIGOID";
                    break;
                case 52:
                    return "Ngoại trú _ Vảy phấn đỏ nang lông";
                    break;
                case 53:
                    return "Ngoại trú _ Lupus ban đỏ mạn tính";
                    break;

                case 54:
                    return "Ngoại trú _ Vảy nến thể mủ";
                    break;
                case 55:
                    return "Ngoại trú _ Duhring Brocq";
                    break;
                case 56:
                    return "Đái tháo đường";
                    break;
                case 57:
                    return "Ngoại trú _ Ung thư hắc tố";
                    break;
                case 58:
                    return "Ngoại trú _ Ung thư không hắc tố";
                    break;
                case 59:
                    return "Ngoại trú _ Pemphigus";
                    break;
                case 60:
                    return "Ngoại trú _ Vảy nến thể khớp";
                    break;
                case 61:
                    return "Ngoại trú _ Hội chứng trùng lặp";
                    break;
                case 62:
                    return "Theo dõi và điều trị có kiểm soát stent động mạnh vành";
                    break;
                case 63:
                    return "Theo dõi quản lý điều trị có kiểm soát thiếu máu cơ tim cục bộ";
                    break;
                case 64:
                    return "Theo dõi và điều trị bệnh Basedow";
                    break;
                case 65:
                    return "Theo dõi điều trị bệnh viêm gan siêu vi B";
                    break;
                case 66:
                    return "Theo dõi quản lý bệnh phổi tắc nghẽn mãn tính";
                    break;
                case 67:
                    return "Tăng huyết áp";
                    break;
                case 68:
                    return "Ngoại trú _ HIV";
                    break;
                case 69:
                    return "Ngoại trú _ Hen phế quản";
                    break;
                case 70:
                    return "Nội trú ban ngày-YHCT";
                    break;
                case 71:
                    return "Hồ sơ bệnh án suy tim";
                    break;
                case 73:
                    return "Xơ cứng bì hệ thống";
                    break;
                case 74:
                    return "Xơ cứng bì khu trú";
                    break;
                case 75:
                    return "Ngoại trú _ Phẫu thuật";
                    break;
                case 76:
                    return "Nội trú nhi _ YHCT";
                    break;
                case 78:
                    return "Nội khoa thận";
                    break;

                default:
                    return "";
                    break;
                #endregion
            }
        }

        private void AddBarItem(BarManager barManager, BarSubItem sub1, string ten, MenuType mType, LoaiBenhAnEMR lType, ItemClickEventHandler mouseRightClick)
        {
            try
            {
                BarButtonItem bbtnErm = new BarButtonItem(barManager, ten, 4);
                bbtnErm.Tag = mType;
                bbtnErm.ItemClick += new ItemClickEventHandler(mouseRightClick);

                HIS_EMR_COVER_TYPE EmrCoverType = this._LstEmrCoverType != null ? this._LstEmrCoverType.FirstOrDefault(o => o.ID == (long)lType) : null;

                if (EmrCoverType != null && EmrCoverType.IS_ACTIVE == 1)
                {
                    //bbtnErm.Visibility = BarItemVisibility.Always;
                    sub1.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    //sub1.Visibility = BarItemVisibility.Never;
                    bbtnErm.Visibility = BarItemVisibility.Never;
                }
                sub1.ItemLinks.Add(bbtnErm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddBarItemList(BarManager barManager, BarSubItem sub1, string ten, long mType, long lType, ItemClickEventHandler mouseRightClick)
        {
            try
            {
                BarButtonItem bbtnErm = new BarButtonItem(barManager, ten, 4);
                bbtnErm.Tag = mType;
                bbtnErm.ItemClick += new ItemClickEventHandler(mouseRightClick);

                HIS_EMR_COVER_TYPE EmrCoverType = this._LstEmrCoverType != null ? this._LstEmrCoverType.FirstOrDefault(o => o.ID == lType) : null;

                if (EmrCoverType != null && EmrCoverType.IS_ACTIVE == 1)
                {
                    //bbtnErm.Visibility = BarItemVisibility.Always;
                    sub1.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    //sub1.Visibility = BarItemVisibility.Never;
                    bbtnErm.Visibility = BarItemVisibility.Never;
                }
                sub1.ItemLinks.Add(bbtnErm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddBarItemPopupMenu(BarManager barManager, PopupMenu Menu, string ten, MenuType mType, LoaiBenhAnEMR lType, ItemClickEventHandler mouseRightClick)
        {
            try
            {
                BarButtonItem bbtnErm = new BarButtonItem(barManager, ten, 4);
                bbtnErm.Tag = mType;
                bbtnErm.ItemClick += new ItemClickEventHandler(mouseRightClick);

                HIS_EMR_COVER_TYPE EmrCoverType = this._LstEmrCoverType != null ? this._LstEmrCoverType.FirstOrDefault(o => o.ID == (long)lType) : null;

                if (EmrCoverType != null && EmrCoverType.IS_ACTIVE == 1)
                {

                }
                else
                {
                    bbtnErm.Visibility = BarItemVisibility.Never;
                }
                Menu.ItemLinks.Add(bbtnErm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddBarItemPopupMenuList(BarManager barManager, PopupMenu Menu, string ten, long mType, long lType, ItemClickEventHandler mouseRightClick)
        {
            try
            {
                BarButtonItem bbtnErm = new BarButtonItem(barManager, ten, 4);
                bbtnErm.Tag = mType;
                bbtnErm.ItemClick += new ItemClickEventHandler(mouseRightClick);

                HIS_EMR_COVER_TYPE EmrCoverType = this._LstEmrCoverType != null ? this._LstEmrCoverType.FirstOrDefault(o => o.ID == lType) : null;

                if (EmrCoverType != null && EmrCoverType.IS_ACTIVE == 1)
                {

                }
                else
                {
                    bbtnErm.Visibility = BarItemVisibility.Never;
                }
                Menu.ItemLinks.Add(bbtnErm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FromConfig__RightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                FromConfig.frmConfigHide frm = new FromConfig.frmConfigHide(this._LstEmrCoverType);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FromPhieu__RightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                FromConfig.frmPhieu frm = new FromConfig.frmPhieu(this.inputAdo);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void EmrMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                MediRecordProcessor processor = new MediRecordProcessor();
                if (e.Item is BarButtonItem && this.inputAdo != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    MediRecordMenuPopupProcessor.MenuType type = (MediRecordMenuPopupProcessor.MenuType)(e.Item.Tag);
                    switch (type)
                    {
                        //case MediRecordMenuPopupProcessor.MenuType.EMR_YeuCauSuDungKhangSinh:
                        //    processor.LoadDataEmr(0, this.inputAdo, "YCSDKS");
                        //    break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NoiKhoa:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoa, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiKhoa:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiKhoa, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DaLieu:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieu, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_Bong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Bong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_HuyetHocTruyenMau:
                            processor.LoadDataEmr(LoaiBenhAnEMR.HuyetHocTruyenMau, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_MatBanPhanTruoc:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatBanPhanTruoc, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_MatChanThuong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatChanThuong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DayMat:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatDayMat, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_Glocom:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatGloCom, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_MatLac:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatLac, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_MatTreEm:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatTreEm, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTru:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruRangHamMat:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruRangHamMat, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruTaiMuiHong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruTaiMuiHong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruYHocCoTruyen:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruYHCT, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NhiKhoa:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NhiKhoa, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NoiTruYHocCoTruyen:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruYHCT, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NoiTruNhiYHCT:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruNhiYHCT, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhucHoiChucNang:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNang, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhuKhoa:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhuKhoa, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_RangHamMat:
                            processor.LoadDataEmr(LoaiBenhAnEMR.RangHamMat, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_SanKhoa:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SanKhoa, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_SoSinh:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SoSinh, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TaiMuiHong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TaiMuiHong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TamThan:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TamThan, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TruyenNhiem:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiem, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_UngBieu:
                            processor.LoadDataEmr(LoaiBenhAnEMR.UngBuou, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_XaPhuong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.XaPhuong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DieuTriBanNgay:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DieuTriBanNgay, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_BANTBanNgayYHCT:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANTBanNgayYHCT, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_Tim:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Tim, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_LuuCapCuu:
                            processor.LoadDataEmr(LoaiBenhAnEMR.LuuCapCuu, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_Mat:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Mat, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhaThai1:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiI, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhaThai2:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiII, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhaThai3:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiIII, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_YHCTPhucHoiChucNang:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangYHCT, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_ThanNhantao:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThanNhanTao, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_CMU:
                            processor.LoadDataEmr(LoaiBenhAnEMR.CMU, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruPhongKham:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANgoaTruPK, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TayChanMieng:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TayChanMieng, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruMat:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruMat, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruPhucHoiChucNang:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DaLieuTrungUong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieuTW, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NhiKhoaPhucHoiChucNang:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangNhi, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_PhucHoiChucNangNoiTru:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PHCNII, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TruyenNhiemII:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiemII, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruVayNenThongThuong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruAVayNen:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_AVayNen, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruHoTroSinhSan:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruPemphigoid:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruVayPhanDoNangLong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruLupusBanDoManTinh:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruLupusBanDoHeThong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruViemBiCo:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_HoiChungTrungLap:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DuhringBrocq:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DuhringBrocq, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruPemphigus:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_Pemphigus, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruUngThuDaHacTo:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruUngThuDaKhongHacTo:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruVayNenTheKhop:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruVayNenTheMu:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheMu, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_DaiThaoDuong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaiThaoDuong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_ThoiDoiVaDieuTriCoKiemSoatStentDongMachVanh:
                            processor.LoadDataEmr(LoaiBenhAnEMR.StentDongMachVanh, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TheoDoiQuanLyDieuTriCoKiemSoatThieuMauCoTimCucBo:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThieuMauCoTimCucBo, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_ThoiDoiVaDieuTriBenhBasedow:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhBasedow, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TheoDoiDieuTRiBenhViemGanSieuViB:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ViemGanBManTinh, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_TheoDoiQuanLyBenhPhoiTacNghenManTinh:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhoiTacNghenManTinh, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_BenhAnTangHuyetAp:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhTangHuyetAp, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruHIV:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnNgoaiTruHIV, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruhenPheQuan:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnHenPheQuan, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_BenhAnSuyTim:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnSuyTim, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_XoCungBiHeThong:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_XoCungbiKhutru:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NgoaiTruDaLieuTW:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DaLieuTW, this.inputAdo);
                            break;
                        case MediRecordMenuPopupProcessor.MenuType.Emr_NoiKhoaThan:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoaThan, this.inputAdo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrMouseRightsingle_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                MediRecordProcessor processor = new MediRecordProcessor();
                if (e.Item is BarButtonItem && this.inputAdo != null)
                {
                    var bbtnItem = sender as BarButtonItem;

                    long tag = (long)e.Item.Tag;

                    switch (tag)
                    {
                        case 1:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoa, this.inputAdo);
                            break;
                        case 2:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NhiKhoa, this.inputAdo);
                            break;
                        case 3:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiem, this.inputAdo);
                            break;
                        case 4:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhuKhoa, this.inputAdo);
                            break;
                        case 5:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SanKhoa, this.inputAdo);
                            break;
                        case 6:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SoSinh, this.inputAdo);
                            break;
                        case 7:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TamThan, this.inputAdo);
                            break;
                        case 8:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieu, this.inputAdo);
                            break;
                        case 9:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNang, this.inputAdo);
                            break;
                        case 10:
                            processor.LoadDataEmr(LoaiBenhAnEMR.HuyetHocTruyenMau, this.inputAdo);
                            break;
                        case 11:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiKhoa, this.inputAdo);
                            break;
                        case 12:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Bong, this.inputAdo);
                            break;
                        case 13:
                            processor.LoadDataEmr(LoaiBenhAnEMR.UngBuou, this.inputAdo);
                            break;
                        case 14:
                            processor.LoadDataEmr(LoaiBenhAnEMR.RangHamMat, this.inputAdo);
                            break;
                        case 15:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TaiMuiHong, this.inputAdo);
                            break;
                        case 16:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru, this.inputAdo);
                            break;
                        case 17:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruRangHamMat, this.inputAdo);
                            break;
                        case 18:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruTaiMuiHong, this.inputAdo);
                            break;
                        case 19:
                            processor.LoadDataEmr(LoaiBenhAnEMR.XaPhuong, this.inputAdo);
                            break;
                        case 20:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatLac, this.inputAdo);
                            break;
                        case 21:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatTreEm, this.inputAdo);
                            break;
                        case 22:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatGloCom, this.inputAdo);
                            break;
                        case 23:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatDayMat, this.inputAdo);
                            break;
                        case 24:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatChanThuong, this.inputAdo);
                            break;
                        case 25:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatBanPhanTruoc, this.inputAdo);
                            break;
                        case 26:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruYHCT, this.inputAdo);
                            break;
                        case 27:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruYHCT, this.inputAdo);
                            break;
                        case 28:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiI, this.inputAdo);
                            break;
                        case 29:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiII, this.inputAdo);
                            break;
                        case 30:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DieuTriBanNgay, this.inputAdo);
                            break;
                        case 31:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThanNhanTao, this.inputAdo);
                            break;
                        case 32:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Mat, this.inputAdo);
                            break;
                        case 33:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Tim, this.inputAdo);
                            break;
                        case 34:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangYHCT, this.inputAdo);
                            break;
                        case 35:
                            processor.LoadDataEmr(LoaiBenhAnEMR.LuuCapCuu, this.inputAdo);
                            break;
                        case 36:
                            processor.LoadDataEmr(LoaiBenhAnEMR.CMU, this.inputAdo);
                            break;
                        case 37:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiIII, this.inputAdo);
                            break;
                        case 38:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANgoaTruPK, this.inputAdo);
                            break;
                        case 39:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TayChanMieng, this.inputAdo);
                            break;
                        case 40:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruMat, this.inputAdo);
                            break;
                        case 41:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang, this.inputAdo);
                            break;
                        case 42:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangNhi, this.inputAdo);
                            break;
                        case 43:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieuTW, this.inputAdo);
                            break;
                        case 44:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PHCNII, this.inputAdo);
                            break;
                        case 45:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiemII, this.inputAdo);
                            break;
                        case 46:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong, this.inputAdo);
                            break;
                        case 47:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_AVayNen, this.inputAdo);
                            break;
                        case 48:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan, this.inputAdo);
                            break;
                        case 49:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo, this.inputAdo);
                            break;
                        case 50:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong, this.inputAdo);
                            break;
                        case 51:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID, this.inputAdo);
                            break;
                        case 52:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong, this.inputAdo);
                            break;
                        case 53:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh, this.inputAdo);
                            break;
                        case 54:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheMu, this.inputAdo);
                            break;
                        case 55:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DuhringBrocq, this.inputAdo);
                            break;
                        case 56:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaiThaoDuong, this.inputAdo);
                            break;
                        case 57:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo, this.inputAdo);
                            break;
                        case 58:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo, this.inputAdo);
                            break;
                        case 59:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_Pemphigus, this.inputAdo);
                            break;
                        case 60:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop, this.inputAdo);
                            break;
                        case 61:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap, this.inputAdo);
                            break;
                        case 62:
                            processor.LoadDataEmr(LoaiBenhAnEMR.StentDongMachVanh, this.inputAdo);
                            break;
                        case 63:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThieuMauCoTimCucBo, this.inputAdo);
                            break;
                        case 64:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhBasedow, this.inputAdo);
                            break;
                        case 65:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ViemGanBManTinh, this.inputAdo);
                            break;
                        case 66:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhoiTacNghenManTinh, this.inputAdo);
                            break;
                        case 67:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhTangHuyetAp, this.inputAdo);
                            break;
                        case 68:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnNgoaiTruHIV, this.inputAdo);
                            break;
                        case 69:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnHenPheQuan, this.inputAdo);
                            break;
                        case 70:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANTBanNgayYHCT, this.inputAdo);
                            break;
                        case 71:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnSuyTim, this.inputAdo);
                            break;
                        case 73:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong, this.inputAdo);
                            break;
                        case 74:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru, this.inputAdo);
                            break;
                        case 75:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DaLieuTW, this.inputAdo);
                            break;
                        case 76:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruNhiYHCT, this.inputAdo);
                            break;
                        case 78:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoaThan, this.inputAdo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Emrsingle_Click(EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;
                MediRecordProcessor processor = new MediRecordProcessor();
                if (this.inputAdo.EmrCoverTypeId != null)
                {
                    switch (this.inputAdo.EmrCoverTypeId)
                    {
                        case 1:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoa, this.inputAdo);
                            break;
                        case 2:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NhiKhoa, this.inputAdo);
                            break;
                        case 3:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiem, this.inputAdo);
                            break;
                        case 4:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhuKhoa, this.inputAdo);
                            break;
                        case 5:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SanKhoa, this.inputAdo);
                            break;
                        case 6:
                            processor.LoadDataEmr(LoaiBenhAnEMR.SoSinh, this.inputAdo);
                            break;
                        case 7:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TamThan, this.inputAdo);
                            break;
                        case 8:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieu, this.inputAdo);
                            break;
                        case 9:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNang, this.inputAdo);
                            break;
                        case 10:
                            processor.LoadDataEmr(LoaiBenhAnEMR.HuyetHocTruyenMau, this.inputAdo);
                            break;
                        case 11:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiKhoa, this.inputAdo);
                            break;
                        case 12:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Bong, this.inputAdo);
                            break;
                        case 13:
                            processor.LoadDataEmr(LoaiBenhAnEMR.UngBuou, this.inputAdo);
                            break;
                        case 14:
                            processor.LoadDataEmr(LoaiBenhAnEMR.RangHamMat, this.inputAdo);
                            break;
                        case 15:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TaiMuiHong, this.inputAdo);
                            break;
                        case 16:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru, this.inputAdo);
                            break;
                        case 17:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruRangHamMat, this.inputAdo);
                            break;
                        case 18:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruTaiMuiHong, this.inputAdo);
                            break;
                        case 19:
                            processor.LoadDataEmr(LoaiBenhAnEMR.XaPhuong, this.inputAdo);
                            break;
                        case 20:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatLac, this.inputAdo);
                            break;
                        case 21:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatTreEm, this.inputAdo);
                            break;
                        case 22:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatGloCom, this.inputAdo);
                            break;
                        case 23:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatDayMat, this.inputAdo);
                            break;
                        case 24:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatChanThuong, this.inputAdo);
                            break;
                        case 25:
                            processor.LoadDataEmr(LoaiBenhAnEMR.MatBanPhanTruoc, this.inputAdo);
                            break;
                        case 26:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruYHCT, this.inputAdo);
                            break;
                        case 27:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruYHCT, this.inputAdo);
                            break;
                        case 28:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiI, this.inputAdo);
                            break;
                        case 29:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiII, this.inputAdo);
                            break;
                        case 30:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DieuTriBanNgay, this.inputAdo);
                            break;
                        case 31:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThanNhanTao, this.inputAdo);
                            break;
                        case 32:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Mat, this.inputAdo);
                            break;
                        case 33:
                            processor.LoadDataEmr(LoaiBenhAnEMR.Tim, this.inputAdo);
                            break;
                        case 34:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangYHCT, this.inputAdo);
                            break;
                        case 35:
                            processor.LoadDataEmr(LoaiBenhAnEMR.LuuCapCuu, this.inputAdo);
                            break;
                        case 36:
                            processor.LoadDataEmr(LoaiBenhAnEMR.CMU, this.inputAdo);
                            break;
                        case 37:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhaThaiIII, this.inputAdo);
                            break;
                        case 38:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANgoaTruPK, this.inputAdo);
                            break;
                        case 39:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TayChanMieng, this.inputAdo);
                            break;
                        case 40:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruMat, this.inputAdo);
                            break;
                        case 41:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTruPhucHoiChucNang, this.inputAdo);
                            break;
                        case 42:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhucHoiChucNangNhi, this.inputAdo);
                            break;
                        case 43:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaLieuTW, this.inputAdo);
                            break;
                        case 44:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PHCNII, this.inputAdo);
                            break;
                        case 45:
                            processor.LoadDataEmr(LoaiBenhAnEMR.TruyenNhiemII, this.inputAdo);
                            break;
                        case 46:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenThuongThuong, this.inputAdo);
                            break;
                        case 47:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_AVayNen, this.inputAdo);
                            break;
                        case 48:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoTroSinhSan, this.inputAdo);
                            break;
                        case 49:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnViemBiCo, this.inputAdo);
                            break;
                        case 50:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhAnLupusBanDoHeThong, this.inputAdo);
                            break;
                        case 51:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_PEMPHIGOID, this.inputAdo);
                            break;
                        case 52:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayPhanDoNangLong, this.inputAdo);
                            break;
                        case 53:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_LuPusBanDoManTinh, this.inputAdo);
                            break;
                        case 54:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheMu, this.inputAdo);
                            break;
                        case 55:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DuhringBrocq, this.inputAdo);
                            break;
                        case 56:
                            processor.LoadDataEmr(LoaiBenhAnEMR.DaiThaoDuong, this.inputAdo);
                            break;
                        case 57:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaHacTo, this.inputAdo);
                            break;
                        case 58:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_UngThuDaKhongHacTo, this.inputAdo);
                            break;
                        case 59:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_Pemphigus, this.inputAdo);
                            break;
                        case 60:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_VayNenTheKhop, this.inputAdo);
                            break;
                        case 61:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_HoiChungTrungLap, this.inputAdo);
                            break;
                        case 62:
                            processor.LoadDataEmr(LoaiBenhAnEMR.StentDongMachVanh, this.inputAdo);
                            break;
                        case 63:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ThieuMauCoTimCucBo, this.inputAdo);
                            break;
                        case 64:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_BenhBasedow, this.inputAdo);
                            break;
                        case 65:
                            processor.LoadDataEmr(LoaiBenhAnEMR.ViemGanBManTinh, this.inputAdo);
                            break;
                        case 66:
                            processor.LoadDataEmr(LoaiBenhAnEMR.PhoiTacNghenManTinh, this.inputAdo);
                            break;
                        case 67:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhTangHuyetAp, this.inputAdo);
                            break;
                        case 68:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnNgoaiTruHIV, this.inputAdo);
                            break;
                        case 69:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnHenPheQuan, this.inputAdo);
                            break;
                        case 70:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BANTBanNgayYHCT, this.inputAdo);
                            break;
                        case 71:
                            processor.LoadDataEmr(LoaiBenhAnEMR.BenhAnSuyTim, this.inputAdo);
                            break;
                        case 73:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiHeThong, this.inputAdo);
                            break;
                        case 74:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_XoCungBiKhuTru, this.inputAdo);
                            break;
                        case 75:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NgoaiTru_DaLieuTW, this.inputAdo);
                            break;
                        case 76:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiTruNhiYHCT, this.inputAdo);
                            break;
                        case 78:
                            processor.LoadDataEmr(LoaiBenhAnEMR.NoiKhoaThan, this.inputAdo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm thêm menu "Phiếu vỏ bệnh án"
        /// </summary>
        /// <param name="barManager"></param>
        /// <param name="sub1"></param>
        /// <param name="ten"></param>
        /// <param name="FormType"></param>
        /// <param name="mouseRightClick"></param>
        private void AddBarItemForm(BarManager barManager, BarSubItem sub1, string ten, string FormType, ItemClickEventHandler mouseRightClick)
        {
            try
            {
                BarButtonItem bbtnForm = new BarButtonItem(barManager, ten, 4);
                if (!String.IsNullOrEmpty(FormType))
                {
                    bbtnForm.Tag = FormType;
                    bbtnForm.ItemClick += new ItemClickEventHandler(mouseRightClick);

                    HIS_EMR_FORM EmrType = this._LstEmrForm != null ? this._LstEmrForm.FirstOrDefault(o => o.EMR_FORM_CODE == FormType) : null;

                    if (EmrType != null && EmrType.IS_ACTIVE == 1)
                    {
                        bbtnForm.Visibility = BarItemVisibility.Always;
                    }
                    else
                    {
                        bbtnForm.Visibility = BarItemVisibility.Never;
                    }
                }
                else
                {
                    bbtnForm.ItemClick += new ItemClickEventHandler(mouseRightClick);
                    bbtnForm.Visibility = BarItemVisibility.Always;
                }
                sub1.ItemLinks.Add(bbtnForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrFormMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.inputAdo != null)
                {
                    string tag = e.Item.Tag.ToString();
                    MediRecordProcessor processor = new MediRecordProcessor();

                    if (this.inputAdo.EmrCoverTypeId > 0)
                    {
                        processor.LoadDataEmr((LoaiBenhAnEMR)this.inputAdo.EmrCoverTypeId, this.inputAdo, tag);
                    }
                    else
                    {
                        processor.LoadDataEmr(0, this.inputAdo, tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
