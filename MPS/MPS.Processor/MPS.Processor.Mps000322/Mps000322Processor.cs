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
using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000322.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System;
using System.Linq;

namespace MPS.Processor.Mps000322
{
    class Mps000322Processor : AbstractProcessor
    {
        Mps000322PDO rdo;
        List<PhoneADO> _PhoneADOs;
        List<ServiceReqADO> _ServiceReqADOs;

        public Mps000322Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000322PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Phones", this._PhoneADOs);
                objectTag.AddObjectData(store, "ServiceReqs", this._ServiceReqADOs);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                this._PhoneADOs = new List<PhoneADO>();
                this._ServiceReqADOs = new List<ServiceReqADO>();
                SetSingleKeyADO _SetSingleKeyADO = new SetSingleKeyADO();
                if (rdo._SarFormDatas != null && rdo._SarFormDatas.Count > 0)
                {
                    foreach (var item in rdo._SarFormDatas)
                    {
                        switch (item.KEY)
                        {
                            case "lblAddress":
                                _SetSingleKeyADO.lblAddress = item.VALUE;
                                break;
                            case "lblHoTen":
                                _SetSingleKeyADO.lblHoTen = item.VALUE;
                                break;
                            case "lblNamSinh":
                                _SetSingleKeyADO.lblNamSinh = item.VALUE;
                                break;
                            case "lblGioiTinh":
                                _SetSingleKeyADO.lblGioiTinh = item.VALUE;
                                break;
                            case "lblBhyt":
                                _SetSingleKeyADO.lblBhyt = item.VALUE;
                                break;
                            case "spinEditCanNang":
                                _SetSingleKeyADO.spinEditCanNang = item.VALUE;
                                break;
                            case "spinEditChieuCao":
                                _SetSingleKeyADO.spinEditChieuCao = item.VALUE;
                                break;
                            case "spinEditHuyetAp1":
                                _SetSingleKeyADO.spinEditHuyetAp1 = item.VALUE;
                                break;
                            case "spinEditHuyetAp2":
                                _SetSingleKeyADO.spinEditHuyetAp2 = item.VALUE;
                                break;
                            case "spinEditMach":
                                _SetSingleKeyADO.spinEditMach = item.VALUE;
                                break;
                            case "lblBMI":
                                _SetSingleKeyADO.lblBMI = item.VALUE;
                                break;
                            case "chkTheoDoiCo":
                                _SetSingleKeyADO.chkTheoDoiCo = item.VALUE;
                                break;
                            case "chkTheoDoiKhong":
                                _SetSingleKeyADO.chkTheoDoiKhong = item.VALUE;
                                break;
                            #region cboNhoiMauCoTim
                            case "cboNhoiMauCoTim":
                                _SetSingleKeyADO.cboNhoiMauCoTim = item.VALUE;
                                break;
                            case "chkNhoiMauCoTim":
                                _SetSingleKeyADO.chkNhoiMauCoTim = item.VALUE;
                                break;
                            case "txtNhoiMauCoTim":
                                _SetSingleKeyADO.txtNhoiMauCoTim = item.VALUE;
                                break;
                            case "dtNhoiMauCoTim":
                                _SetSingleKeyADO.dtNhoiMauCoTim = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhTimTMCB
                            case "cboBenhTimTMCB":
                                _SetSingleKeyADO.cboBenhTimTMCB = item.VALUE;
                                break;
                            case "chkBenhTimTMCB":
                                _SetSingleKeyADO.chkBenhTimTMCB = item.VALUE;
                                break;
                            case "txtBenhTimTMCB":
                                _SetSingleKeyADO.txtBenhTimTMCB = item.VALUE;
                                break;
                            case "dtBenhTimTMCB":
                                _SetSingleKeyADO.dtBenhTimTMCB = item.VALUE;
                                break;
                            #endregion
                            #region cboTangHuyetAp
                            case "cboTangHuyetAp":
                                _SetSingleKeyADO.cboTangHuyetAp = item.VALUE;
                                break;
                            case "chkTangHuyetAp":
                                _SetSingleKeyADO.chkTangHuyetAp = item.VALUE;
                                break;
                            case "txtTangHuyetAp":
                                _SetSingleKeyADO.txtTangHuyetAp = item.VALUE;
                                break;
                            case "dtTangHuyetAp":
                                _SetSingleKeyADO.dtTangHuyetAp = item.VALUE;
                                break;
                            #endregion
                            #region cboTieuDuong
                            case "cboTieuDuong":
                                _SetSingleKeyADO.cboTieuDuong = item.VALUE;
                                break;
                            case "chkTieuDuong":
                                _SetSingleKeyADO.chkTieuDuong = item.VALUE;
                                break;
                            case "txtTieuDuong":
                                _SetSingleKeyADO.txtTieuDuong = item.VALUE;
                                break;
                            case "dtTieuDuong":
                                _SetSingleKeyADO.dtTieuDuong = item.VALUE;
                                break;
                            #endregion
                            #region cboRLLipidMau
                            case "cboRLLipidMau":
                                _SetSingleKeyADO.cboRLLipidMau = item.VALUE;
                                break;
                            case "chkRLLipidMau":
                                _SetSingleKeyADO.chkRLLipidMau = item.VALUE;
                                break;
                            case "txtRLLipidMau":
                                _SetSingleKeyADO.txtRLLipidMau = item.VALUE;
                                break;
                            case "dtRLLipidMau":
                                _SetSingleKeyADO.dtRLLipidMau = item.VALUE;
                                break;
                            #endregion
                            #region cboTsTBMNTIA
                            case "cboTsTBMNTIA":
                                _SetSingleKeyADO.cboTsTBMNTIA = item.VALUE;
                                break;
                            case "chkTsTBMNTIA":
                                _SetSingleKeyADO.chkTsTBMNTIA = item.VALUE;
                                break;
                            case "txtTsTBMNTIA":
                                _SetSingleKeyADO.txtTsTBMNTIA = item.VALUE;
                                break;
                            case "dtTsTBMNTIA":
                                _SetSingleKeyADO.dtTsTBMNTIA = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhCoTimGian
                            case "cboBenhCoTimGian":
                                _SetSingleKeyADO.cboBenhCoTimGian = item.VALUE;
                                break;
                            case "chkBenhCoTimGian":
                                _SetSingleKeyADO.chkBenhCoTimGian = item.VALUE;
                                break;
                            case "txtBenhCoTimGian":
                                _SetSingleKeyADO.txtBenhCoTimGian = item.VALUE;
                                break;
                            case "dtBenhCoTimGian":
                                _SetSingleKeyADO.dtBenhCoTimGian = item.VALUE;
                                break;
                            #endregion
                            #region cboTsViemCoTim
                            case "cboTsViemCoTim":
                                _SetSingleKeyADO.cboTsViemCoTim = item.VALUE;
                                break;
                            case "chkTsViemCoTim":
                                _SetSingleKeyADO.chkTsViemCoTim = item.VALUE;
                                break;
                            case "txtTsViemCoTim":
                                _SetSingleKeyADO.txtTsViemCoTim = item.VALUE;
                                break;
                            case "dtTsViemCoTim":
                                _SetSingleKeyADO.dtTsViemCoTim = item.VALUE;
                                break;
                            #endregion
                            #region cboBCTChuSan
                            case "cboBCTChuSan":
                                _SetSingleKeyADO.cboBCTChuSan = item.VALUE;
                                break;
                            case "chkBCTChuSan":
                                _SetSingleKeyADO.chkBCTChuSan = item.VALUE;
                                break;
                            case "txtBCTChuSan":
                                _SetSingleKeyADO.txtBCTChuSan = item.VALUE;
                                break;
                            case "dtBCTChuSan":
                                _SetSingleKeyADO.dtBCTChuSan = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhVHLHauThap
                            case "cboBenhVHLHauThap":
                                _SetSingleKeyADO.cboBenhVHLHauThap = item.VALUE;
                                break;
                            case "chkBenhVHLHauThap":
                                _SetSingleKeyADO.chkBenhVHLHauThap = item.VALUE;
                                break;
                            case "txtBenhVHLHauThap":
                                _SetSingleKeyADO.txtBenhVHLHauThap = item.VALUE;
                                break;
                            case "dtBenhVHLHauThap":
                                _SetSingleKeyADO.dtBenhVHLHauThap = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhVDMCHauThap
                            case "cboBenhVDMCHauThap":
                                _SetSingleKeyADO.cboBenhVDMCHauThap = item.VALUE;
                                break;
                            case "chkBenhVDMCHauThap":
                                _SetSingleKeyADO.chkBenhVDMCHauThap = item.VALUE;
                                break;
                            case "txtBenhVDMCHauThap":
                                _SetSingleKeyADO.txtBenhVDMCHauThap = item.VALUE;
                                break;
                            case "dtBenhVDMCHauThap":
                                _SetSingleKeyADO.dtBenhVDMCHauThap = item.VALUE;
                                break;
                            #endregion
                            #region cboHoVHLDoSaVan
                            case "cboHoVHLDoSaVan":
                                _SetSingleKeyADO.cboHoVHLDoSaVan = item.VALUE;
                                break;
                            case "chkHoVHLDoSaVan":
                                _SetSingleKeyADO.chkHoVHLDoSaVan = item.VALUE;
                                break;
                            case "txtHoVHLDoSaVan":
                                _SetSingleKeyADO.txtHoVHLDoSaVan = item.VALUE;
                                break;
                            case "dtHoVHLDoSaVan":
                                _SetSingleKeyADO.dtHoVHLDoSaVan = item.VALUE;
                                break;
                            #endregion
                            #region cboSuyThanManTinh
                            case "cboSuyThanManTinh":
                                _SetSingleKeyADO.cboSuyThanManTinh = item.VALUE;
                                break;
                            case "chkSuyThanManTinh":
                                _SetSingleKeyADO.chkSuyThanManTinh = item.VALUE;
                                break;
                            case "txtSuyThanManTinh":
                                _SetSingleKeyADO.txtSuyThanManTinh = item.VALUE;
                                break;
                            case "dtSuyThanManTinh":
                                _SetSingleKeyADO.dtSuyThanManTinh = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhDMNgoaiBien
                            case "cboBenhDMNgoaiBien":
                                _SetSingleKeyADO.cboBenhDMNgoaiBien = item.VALUE;
                                break;
                            case "chkBenhDMNgoaiBien":
                                _SetSingleKeyADO.chkBenhDMNgoaiBien = item.VALUE;
                                break;
                            case "txtBenhDMNgoaiBien":
                                _SetSingleKeyADO.txtBenhDMNgoaiBien = item.VALUE;
                                break;
                            case "dtBenhDMNgoaiBien":
                                _SetSingleKeyADO.dtBenhDMNgoaiBien = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhTimBamSinh
                            case "cboBenhTimBamSinh":
                                _SetSingleKeyADO.cboBenhTimBamSinh = item.VALUE;
                                break;
                            case "chkBenhTimBamSinh":
                                _SetSingleKeyADO.chkBenhTimBamSinh = item.VALUE;
                                break;
                            case "txtBenhTimBamSinh":
                                _SetSingleKeyADO.txtBenhTimBamSinh = item.VALUE;
                                break;
                            case "dtBenhTimBamSinh":
                                _SetSingleKeyADO.dtBenhTimBamSinh = item.VALUE;
                                break;
                            #endregion
                            #region cboTienSuXaTriHoaTri
                            case "cboTienSuXaTriHoaTri":
                                _SetSingleKeyADO.cboTienSuXaTriHoaTri = item.VALUE;
                                break;
                            case "chkTienSuXaTriHoaTri":
                                _SetSingleKeyADO.chkTienSuXaTriHoaTri = item.VALUE;
                                break;
                            case "txtTienSuXaTriHoaTri":
                                _SetSingleKeyADO.txtTienSuXaTriHoaTri = item.VALUE;
                                break;
                            case "dtTienSuXaTriHoaTri":
                                _SetSingleKeyADO.dtTienSuXaTriHoaTri = item.VALUE;
                                break;
                            #endregion
                            #region cboBenhNgoaiMangTim
                            case "cboBenhNgoaiMangTim":
                                _SetSingleKeyADO.cboBenhNgoaiMangTim = item.VALUE;
                                break;
                            case "chkBenhNgoaiMangTim":
                                _SetSingleKeyADO.chkBenhNgoaiMangTim = item.VALUE;
                                break;
                            case "txtBenhNgoaiMangTim":
                                _SetSingleKeyADO.txtBenhNgoaiMangTim = item.VALUE;
                                break;
                            case "dtBenhNgoaiMangTim":
                                _SetSingleKeyADO.dtBenhNgoaiMangTim = item.VALUE;
                                break;
                            #endregion
                            #region cboTienSuChanThuong
                            case "cboTienSuChanThuong":
                                _SetSingleKeyADO.cboTienSuChanThuong = item.VALUE;
                                break;
                            case "chkTienSuChanThuong":
                                _SetSingleKeyADO.chkTienSuChanThuong = item.VALUE;
                                break;
                            case "txtTienSuChanThuong":
                                _SetSingleKeyADO.txtTienSuChanThuong = item.VALUE;
                                break;
                            case "dtTienSuChanThuong":
                                _SetSingleKeyADO.dtTienSuChanThuong = item.VALUE;
                                break;
                            #endregion
                            #region txt12Khac
                            case "txt12Khac":
                                _SetSingleKeyADO.txt12Khac = item.VALUE;
                                break;
                            #endregion
                            #region cboDatStentNongDMV
                            case "cboDatStentNongDMV":
                                _SetSingleKeyADO.cboDatStentNongDMV = item.VALUE;
                                break;
                            case "chkDatStentNongDMV":
                                _SetSingleKeyADO.chkDatStentNongDMV = item.VALUE;
                                break;
                            case "txtDatStentNongDMV":
                                _SetSingleKeyADO.txtDatStentNongDMV = item.VALUE;
                                break;
                            case "dtDatStentNongDMV":
                                _SetSingleKeyADO.dtDatStentNongDMV = item.VALUE;
                                break;
                            #endregion
                            #region cboCABG
                            case "cboCABG":
                                _SetSingleKeyADO.cboCABG = item.VALUE;
                                break;
                            case "chkCABG":
                                _SetSingleKeyADO.chkCABG = item.VALUE;
                                break;
                            case "txtCABG":
                                _SetSingleKeyADO.txtCABG = item.VALUE;
                                break;
                            case "dtCABG":
                                _SetSingleKeyADO.dtCABG = item.VALUE;
                                break;
                            #endregion
                            #region cboDatMayTaoNhip
                            case "cboDatMayTaoNhip":
                                _SetSingleKeyADO.cboDatMayTaoNhip = item.VALUE;
                                break;
                            case "chkDatMayTaoNhip":
                                _SetSingleKeyADO.chkDatMayTaoNhip = item.VALUE;
                                break;
                            case "txtDatMayTaoNhip":
                                _SetSingleKeyADO.txtDatMayTaoNhip = item.VALUE;
                                break;
                            case "dtDatMayTaoNhip":
                                _SetSingleKeyADO.dtDatMayTaoNhip = item.VALUE;
                                break;
                            #endregion
                            #region cboPTThayVanTimDatVongVan
                            case "cboPTThayVanTimDatVongVan":
                                _SetSingleKeyADO.cboPTThayVanTimDatVongVan = item.VALUE;
                                break;
                            case "chkPTThayVanTimDatVongVan":
                                _SetSingleKeyADO.chkPTThayVanTimDatVongVan = item.VALUE;
                                break;
                            case "txtPTThayVanTimDatVongVan":
                                _SetSingleKeyADO.txtPTThayVanTimDatVongVan = item.VALUE;
                                break;
                            case "dtPTThayVanTimDatVongVan":
                                _SetSingleKeyADO.dtPTThayVanTimDatVongVan = item.VALUE;
                                break;
                            #endregion
                            #region cboPTSuaChuaTBS
                            case "cboPTSuaChuaTBS":
                                _SetSingleKeyADO.cboPTSuaChuaTBS = item.VALUE;
                                break;
                            case "chkPTSuaChuaTBS":
                                _SetSingleKeyADO.chkPTSuaChuaTBS = item.VALUE;
                                break;
                            case "txtPTSuaChuaTBS":
                                _SetSingleKeyADO.txtPTSuaChuaTBS = item.VALUE;
                                break;
                            case "dtPTSuaChuaTBS":
                                _SetSingleKeyADO.dtPTSuaChuaTBS = item.VALUE;
                                break;
                            #endregion
                            #region txt13Khac
                            case "txt13Khac":
                                _SetSingleKeyADO.txt13Khac = item.VALUE;
                                break;
                            #endregion
                            #region txtTienSuGiaDinh
                            case "txtTienSuGiaDinh":
                                _SetSingleKeyADO.txtTienSuGiaDinh = item.VALUE;
                                break;
                            #endregion
                            #region txtNgheNghiepCode
                            case "cboNgheNghiep":
                                _SetSingleKeyADO.cboNgheNghiep = item.VALUE.Split('|')[0];
                                break;
                            case "txtNgheNghiepCode":
                                _SetSingleKeyADO.txtNgheNghiepCode = item.VALUE;
                                break;
                            #endregion
                            #region chk17MotMinhCo
                            case "chk17MotMinhCo":
                                _SetSingleKeyADO.chk17MotMinhCo = item.VALUE;
                                break;
                            case "chk17MotMinhKhong":
                                _SetSingleKeyADO.chk17MotMinhKhong = item.VALUE;
                                break;
                            case "chk17NguoiThanCo":
                                _SetSingleKeyADO.chk17NguoiThanCo = item.VALUE;
                                break;
                            case "chk17NguoiThanKhong":
                                _SetSingleKeyADO.chk17NguoiThanKhong = item.VALUE;
                                break;
                            #endregion
                            #region chk18Co
                            case "chk18Co":
                                _SetSingleKeyADO.chk18Co = item.VALUE;
                                break;
                            case "chk18Khong":
                                _SetSingleKeyADO.chk18Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk19DocThan
                            case "chk19DocThan":
                                _SetSingleKeyADO.chk19DocThan = item.VALUE;
                                break;
                            case "chk19LapGiaDinh":
                                _SetSingleKeyADO.chk19LapGiaDinh = item.VALUE;
                                break;
                            case "chk19Goa":
                                _SetSingleKeyADO.chk19Goa = item.VALUE;
                                break;
                            case "chk19LyThan":
                                _SetSingleKeyADO.chk19LyThan = item.VALUE;
                                break;
                            #endregion
                            #region chk110KhongBietChu
                            case "chk110KhongBietChu":
                                _SetSingleKeyADO.chk110KhongBietChu = item.VALUE;
                                break;
                            case "chk110DaiHoc":
                                _SetSingleKeyADO.chk110DaiHoc = item.VALUE;
                                break;
                            case "chk110PhoThong":
                                _SetSingleKeyADO.chk110PhoThong = item.VALUE;
                                break;
                            case "chk110SauDaiHoc":
                                _SetSingleKeyADO.chk110SauDaiHoc = item.VALUE;
                                break;
                            #endregion
                            #region txt110TinhTrangKinhTe
                            case "txt110TinhTrangKinhTe":
                                _SetSingleKeyADO.txt110TinhTrangKinhTe = item.VALUE;
                                break;
                            #endregion
                            #region chk112HutThuocLaChuaBaoGio
                            case "chk112HutThuocLaChuaBaoGio":
                                _SetSingleKeyADO.chk112HutThuocLaChuaBaoGio = item.VALUE;
                                break;
                            case "chk112HutThuocLaDaBo":
                                _SetSingleKeyADO.chk112HutThuocLaDaBo = item.VALUE;
                                break;
                            case "chk112HutThuocLaDangHut":
                                _SetSingleKeyADO.chk112HutThuocLaDangHut = item.VALUE;
                                break;
                            #endregion
                            #region chk112UongRuouChuaBaoGio
                            case "chk112UongRuouChuaBaoGio":
                                _SetSingleKeyADO.chk112UongRuouChuaBaoGio = item.VALUE;
                                break;
                            case "chk112UongRuouHangNgay":
                                _SetSingleKeyADO.chk112UongRuouHangNgay = item.VALUE;
                                break;
                            case "chk112UongRuouThiThoang":
                                _SetSingleKeyADO.chk112UongRuouThiThoang = item.VALUE;
                                break;
                            case "chk112UongRuouDaBo":
                                _SetSingleKeyADO.chk112UongRuouDaBo = item.VALUE;
                                break;
                            #endregion
                            #region chk112TheDucKhongTap
                            case "chk112TheDucKhongTap":
                                _SetSingleKeyADO.chk112TheDucKhongTap = item.VALUE;
                                break;
                            case "chk112TheDuc12Lan":
                                _SetSingleKeyADO.chk112TheDuc12Lan = item.VALUE;
                                break;
                            case "chk112TheDuc35Lan":
                                _SetSingleKeyADO.chk112TheDuc35Lan = item.VALUE;
                                break;
                            case "chk112TheDucHangNgay":
                                _SetSingleKeyADO.chk112TheDucHangNgay = item.VALUE;
                                break;
                            #endregion
                            #region chk112MaTuyChuaBaoGio
                            case "chk112MaTuyChuaBaoGio":
                                _SetSingleKeyADO.chk112MaTuyChuaBaoGio = item.VALUE;
                                break;
                            case "chk112MaTuyDaBo":
                                _SetSingleKeyADO.chk112MaTuyDaBo = item.VALUE;
                                break;
                            case "chk112MaTuyDangSuDung":
                                _SetSingleKeyADO.chk112MaTuyDangSuDung = item.VALUE;
                                break;
                            #endregion
                            #region chk20TaiKhamDinhKi
                            case "chk20TaiKhamDinhKi":
                                _SetSingleKeyADO.chk20TaiKhamDinhKi = item.VALUE;
                                break;
                            case "chk20Co":
                                _SetSingleKeyADO.chk20Co = item.VALUE;
                                break;
                            case "chk20Khong":
                                _SetSingleKeyADO.chk20Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk21Co
                            case "chk21Co":
                                _SetSingleKeyADO.chk21Co = item.VALUE;
                                break;
                            case "chk21Khong":
                                _SetSingleKeyADO.chk21Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk22DienHinh
                            case "chk22DienHinh":
                                _SetSingleKeyADO.chk22DienHinh = item.VALUE;
                                break;
                            case "chk22KhongDau":
                                _SetSingleKeyADO.chk22KhongDau = item.VALUE;
                                break;
                            case "chk22KhongDienHinh":
                                _SetSingleKeyADO.chk22KhongDienHinh = item.VALUE;
                                break;
                            #endregion
                            #region chk23I
                            case "chk23I":
                                _SetSingleKeyADO.chk23I = item.VALUE;
                                break;
                            case "chk23II":
                                _SetSingleKeyADO.chk23II = item.VALUE;
                                break;
                            case "chk23III":
                                _SetSingleKeyADO.chk23III = item.VALUE;
                                break;
                            case "chk23IV":
                                _SetSingleKeyADO.chk23IV = item.VALUE;
                                break;
                            #endregion
                            #region chk24Co
                            case "chk24Co":
                                _SetSingleKeyADO.chk24Co = item.VALUE;
                                break;
                            case "chk24Khong":
                                _SetSingleKeyADO.chk24Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk25Co
                            case "chk25Co":
                                _SetSingleKeyADO.chk25Co = item.VALUE;
                                break;
                            case "chk25Khong":
                                _SetSingleKeyADO.chk25Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk26Co
                            case "chk26Co":
                                _SetSingleKeyADO.chk26Co = item.VALUE;
                                break;
                            case "chk26Khong":
                                _SetSingleKeyADO.chk26Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk27Co
                            case "chk27Co":
                                _SetSingleKeyADO.chk27Co = item.VALUE;
                                break;
                            case "chk27Khong":
                                _SetSingleKeyADO.chk27Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk28Co
                            case "chk28Co":
                                _SetSingleKeyADO.chk28Co = item.VALUE;
                                break;
                            case "chk28Khong":
                                _SetSingleKeyADO.chk28Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk29Co
                            case "chk29Co":
                                _SetSingleKeyADO.chk29Co = item.VALUE;
                                break;
                            case "chk29Khong":
                                _SetSingleKeyADO.chk29Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk210Co
                            case "chk210Co":
                                _SetSingleKeyADO.chk210Co = item.VALUE;
                                break;
                            case "chk210Khong":
                                _SetSingleKeyADO.chk210Khong = item.VALUE;
                                break;
                            #endregion
                            #region chk211Co
                            case "chk211Co":
                                _SetSingleKeyADO.chk211Co = item.VALUE;
                                break;
                            case "chk211Khong":
                                _SetSingleKeyADO.chk211Khong = item.VALUE;
                                break;
                            #endregion
                            #region txt212NhanXetKhac
                            case "txt212NhanXetKhac":
                                _SetSingleKeyADO.txt212NhanXetKhac = item.VALUE;
                                break;
                            #endregion
                            #region chk31NhipXoang
                            case "chk31NhipXoang":
                                _SetSingleKeyADO.chk31NhipXoang = item.VALUE;
                                break;
                            case "chk31BlockAV":
                                _SetSingleKeyADO.chk31BlockAV = item.VALUE;
                                break;
                            case "chk31RungCuongNhi":
                                _SetSingleKeyADO.chk31RungCuongNhi = item.VALUE;
                                break;
                            case "chk31LBBB":
                                _SetSingleKeyADO.chk31LBBB = item.VALUE;
                                break;
                            case "txt31TanSo":
                                _SetSingleKeyADO.txt31TanSo = item.VALUE;
                                break;
                            #endregion
                            #region chk32DD
                            case "chk32DD":
                                _SetSingleKeyADO.chk32DD = item.VALUE;
                                break;
                            case "chk32Ds":
                                _SetSingleKeyADO.chk32Ds = item.VALUE;
                                break;
                            case "chk32EF":
                                _SetSingleKeyADO.chk32EF = item.VALUE;
                                break;
                            case "chk32PAPs":
                                _SetSingleKeyADO.chk32PAPs = item.VALUE;
                                break;
                            #endregion
                            #region txtSdtLienHe
                            case "txtSdtLienHe":
                                _SetSingleKeyADO.txtSdtLienHe = item.VALUE;
                                break;
                            #endregion
                            #region txtTenNguoiDuocNoiChuyen
                            case "txtTenNguoiDuocNoiChuyen":
                                _SetSingleKeyADO.txtTenNguoiDuocNoiChuyen = item.VALUE;
                                break;
                            #endregion
                            #region txtQuanHe
                            case "txtQuanHe":
                                _SetSingleKeyADO.txtQuanHe = item.VALUE;
                                break;
                            #endregion
                            #region dtNgayNhapVien
                            case "dtNgayNhapVien":
                                _SetSingleKeyADO.dtNgayNhapVien = item.VALUE;
                                break;
                            #endregion
                            #region chkI4SuyTimNangLenCo
                            case "chkI4SuyTimNangLenCo":
                                _SetSingleKeyADO.chkI4SuyTimNangLenCo = item.VALUE;
                                break;
                            case "chkI4SuyTimNangLenKhong":
                                _SetSingleKeyADO.chkI4SuyTimNangLenKhong = item.VALUE;
                                break;
                            #endregion
                            #region chkI4TBMNCo
                            case "chkI4TBMNCo":
                                _SetSingleKeyADO.chkI4TBMNCo = item.VALUE;
                                break;
                            case "chkI4TBMNKhong":
                                _SetSingleKeyADO.chkI4TBMNKhong = item.VALUE;
                                break;
                            #endregion
                            #region chkI4NhiemKhuanCo
                            case "chkI4NhiemKhuanCo":
                                _SetSingleKeyADO.chkI4NhiemKhuanCo = item.VALUE;
                                break;
                            case "chkI4NhiemKhuanKhong":
                                _SetSingleKeyADO.chkI4NhiemKhuanKhong = item.VALUE;
                                break;
                            #endregion
                            #region chkI4NhiemKhuanCo
                            case "chkI4BienCoMachVanhCo":
                                _SetSingleKeyADO.chkI4BienCoMachVanhCo = item.VALUE;
                                break;
                            case "chkI4BienCoMachVanhKhong":
                                _SetSingleKeyADO.chkI4BienCoMachVanhKhong = item.VALUE;
                                break;
                            #endregion
                            #region chkI4BienCoMachKhacCo
                            case "chkI4BienCoMachKhacCo":
                                _SetSingleKeyADO.chkI4BienCoMachKhacCo = item.VALUE;
                                break;
                            case "chkI4BienCoMachKhacKhong":
                                _SetSingleKeyADO.chkI4BienCoMachKhacKhong = item.VALUE;
                                break;
                            #endregion
                            #region chkI4RoiLoanDOngMauCo
                            case "chkI4RoiLoanDOngMauCo":
                                _SetSingleKeyADO.chkI4RoiLoanDOngMauCo = item.VALUE;
                                break;
                            case "chkI4RoiLoanDOngMauKhong":
                                _SetSingleKeyADO.chkI4RoiLoanDOngMauKhong = item.VALUE;
                                break;
                            #endregion
                            #region txtI4LyDoKhac
                            case "txtI4LyDoKhac":
                                _SetSingleKeyADO.txtI4LyDoKhac = item.VALUE;
                                break;
                            #endregion
                            #region chkII1Co
                            case "chkII1Co":
                                _SetSingleKeyADO.chkII1Co = item.VALUE;
                                break;
                            case "chkII1Khong":
                                _SetSingleKeyADO.chkII1Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkII2Co
                            case "chkII2Co":
                                _SetSingleKeyADO.chkII2Co = item.VALUE;
                                break;
                            case "chkII2Khong":
                                _SetSingleKeyADO.chkII2Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkII3Co
                            case "chkII3Co":
                                _SetSingleKeyADO.chkII3Co = item.VALUE;
                                break;
                            case "chkII3Khong":
                                _SetSingleKeyADO.chkII3Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkII4Co
                            case "chkII4Co":
                                _SetSingleKeyADO.chkII4Co = item.VALUE;
                                break;
                            case "chkII4Khong":
                                _SetSingleKeyADO.chkII4Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkII5Co
                            case "chkII5Co":
                                _SetSingleKeyADO.chkII5Co = item.VALUE;
                                break;
                            case "chkII5Khong":
                                _SetSingleKeyADO.chkII5Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkII6Co
                            case "chkII6Co":
                                _SetSingleKeyADO.chkII6Co = item.VALUE;
                                break;
                            case "chkII6Khong":
                                _SetSingleKeyADO.chkII6Khong = item.VALUE;
                                break;
                            #endregion
                            #region spinEditII7
                            case "spinEditII7":
                                _SetSingleKeyADO.spinEditII7 = item.VALUE;
                                break;
                            #endregion
                            #region chkII8I
                            case "chkII8I":
                                _SetSingleKeyADO.chkII8I = item.VALUE;
                                break;
                            case "chkII8II":
                                _SetSingleKeyADO.chkII8II = item.VALUE;
                                break;
                            case "chkII8III":
                                _SetSingleKeyADO.chkII8III = item.VALUE;
                                break;
                            case "chkII8IV":
                                _SetSingleKeyADO.chkII8IV = item.VALUE;
                                break;
                            #endregion
                            #region chkIII1Co
                            case "chkIII1Co":
                                _SetSingleKeyADO.chkIII1Co = item.VALUE;
                                break;
                            case "chkIII1Khong":
                                _SetSingleKeyADO.chkIII1Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkIII2Co
                            case "chkIII2Co":
                                _SetSingleKeyADO.chkIII2Co = item.VALUE;
                                break;
                            case "chkIII2Khong":
                                _SetSingleKeyADO.chkIII2Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkIII3Co
                            case "chkIII3Co":
                                _SetSingleKeyADO.chkIII3Co = item.VALUE;
                                break;
                            case "chkIII3Khong":
                                _SetSingleKeyADO.chkIII3Khong = item.VALUE;
                                break;
                            #endregion
                            #region chkIII4Co
                            case "chkIII4Co":
                                _SetSingleKeyADO.chkIII4Co = item.VALUE;
                                break;
                            case "chkIII4Khong":
                                _SetSingleKeyADO.chkIII4Khong = item.VALUE;
                                break;
                            case "chkIII4KhongSuDung":
                                _SetSingleKeyADO.chkIII4KhongSuDung = item.VALUE;
                                break;
                            #endregion
                            #region chkIII5Co
                            case "chkIII5Co":
                                _SetSingleKeyADO.chkIII5Co = item.VALUE;
                                break;
                            case "chkIII5Khong":
                                _SetSingleKeyADO.chkIII5Khong = item.VALUE;
                                break;
                            case "chkIII5KhongSuDung":
                                _SetSingleKeyADO.chkIII5KhongSuDung = item.VALUE;
                                break;
                            #endregion
                            #region SoDienThoai-QuanHe
                            case "PHONE_NUMBER_RELATIVE":
                                _PhoneADOs = new List<PhoneADO>();
                                this._PhoneADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PhoneADO>>(item.VALUE);
                                break;
                            #endregion
                            #region DanhSachKham
                            case "EXAM_SERVICE_REQ":
                                _ServiceReqADOs = new List<ServiceReqADO>();
                                this._ServiceReqADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceReqADO>>(item.VALUE);
                                break;
                            #endregion
                            case "spinEditIV1":
                                _SetSingleKeyADO.spinEditIV1 = item.VALUE;
                                break;
                            case "spinEditIV2":
                                _SetSingleKeyADO.spinEditIV2 = item.VALUE;
                                break;
                            case "spinEditIV3":
                                _SetSingleKeyADO.spinEditIV3 = item.VALUE;
                                break;
                            case "spinEditIV4":
                                _SetSingleKeyADO.spinEditIV4 = item.VALUE;
                                break;
                            case "spinEditIV5":
                                _SetSingleKeyADO.spinEditIV5 = item.VALUE;
                                break;
                            case "lblTenBenhNhan":
                                _SetSingleKeyADO.lblTenBenhNhan = item.VALUE;
                                break;
                            case "dtNgayTienHanhDaoTao":
                                _SetSingleKeyADO.dtNgayTienHanhDaoTao = item.VALUE;
                                break;
                            case "txtHoTenNguoiDaoTao":
                                _SetSingleKeyADO.txtHoTenNguoiDaoTao = item.VALUE;
                                break;
                            case "chkBN41":
                                _SetSingleKeyADO.chkBN41 = item.VALUE;
                                break;
                            case "chkBN42":
                                _SetSingleKeyADO.chkBN42 = item.VALUE;
                                break;
                            case "chkBN43":
                                _SetSingleKeyADO.chkBN43 = item.VALUE;
                                break;
                            case "chkBN44":
                                _SetSingleKeyADO.chkBN44 = item.VALUE;
                                break;
                            case "chkBN45":
                                _SetSingleKeyADO.chkBN45 = item.VALUE;
                                break;
                            case "chkBN46":
                                _SetSingleKeyADO.chkBN46 = item.VALUE;
                                break;
                            case "chkI111":
                                _SetSingleKeyADO.chkI111 = item.VALUE;
                                break;
                            case "chkI112":
                                _SetSingleKeyADO.chkI112 = item.VALUE;
                                break;
                            case "chkI113":
                                _SetSingleKeyADO.chkI113 = item.VALUE;
                                break;
                            case "chkI114":
                                _SetSingleKeyADO.chkI114 = item.VALUE;
                                break;
                            case "chkI115":
                                _SetSingleKeyADO.chkI115 = item.VALUE;
                                break;
                            #region txtTenTaiLieuHuongDan
                            case "txtTenTaiLieuHuongDan":
                                _SetSingleKeyADO.txtTenTaiLieuHuongDan = item.VALUE;
                                break;
                            #endregion
                            default:
                                break;
                        }
                    }
                }
                AddObjectKeyIntoListkey<SetSingleKeyADO>(_SetSingleKeyADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class SetSingleKeyADO
        {
            public string spinEditMach { get; set; }
            public string spinEditHuyetAp1 { get; set; }
            public string spinEditHuyetAp2 { get; set; }
            public string spinEditChieuCao { get; set; }
            public string spinEditCanNang { get; set; }
            public string lblBMI { get; set; }
            public string lblHoTen { get; set; }
            public string lblNamSinh { get; set; }
            public string lblGioiTinh { get; set; }
            public string lblBhyt { get; set; }
            public string lblAddress { get; set; }
            public string cboNhoiMauCoTim { get; set; }
            public string chkNhoiMauCoTim { get; set; }
            public string txtNhoiMauCoTim { get; set; }
            public string dtNhoiMauCoTim { get; set; }
            public string cboBenhTimTMCB { get; set; }
            public string chkBenhTimTMCB { get; set; }
            public string txtBenhTimTMCB { get; set; }
            public string dtBenhTimTMCB { get; set; }
            public string cboTangHuyetAp { get; set; }
            public string chkTangHuyetAp { get; set; }
            public string txtTangHuyetAp { get; set; }
            public string dtTangHuyetAp { get; set; }
            public string cboTieuDuong { get; set; }
            public string chkTieuDuong { get; set; }
            public string txtTieuDuong { get; set; }
            public string dtTieuDuong { get; set; }
            public string cboRLLipidMau { get; set; }
            public string chkRLLipidMau { get; set; }
            public string txtRLLipidMau { get; set; }
            public string dtRLLipidMau { get; set; }
            public string cboTsTBMNTIA { get; set; }
            public string chkTsTBMNTIA { get; set; }
            public string txtTsTBMNTIA { get; set; }
            public string dtTsTBMNTIA { get; set; }
            public string cboBenhCoTimGian { get; set; }
            public string chkBenhCoTimGian { get; set; }
            public string txtBenhCoTimGian { get; set; }
            public string dtBenhCoTimGian { get; set; }
            public string cboTsViemCoTim { get; set; }
            public string chkTsViemCoTim { get; set; }
            public string txtTsViemCoTim { get; set; }
            public string dtTsViemCoTim { get; set; }
            public string cboBCTChuSan { get; set; }
            public string chkBCTChuSan { get; set; }
            public string txtBCTChuSan { get; set; }
            public string dtBCTChuSan { get; set; }
            public string cboBenhVHLHauThap { get; set; }
            public string chkBenhVHLHauThap { get; set; }
            public string txtBenhVHLHauThap { get; set; }
            public string dtBenhVHLHauThap { get; set; }
            public string cboBenhVDMCHauThap { get; set; }
            public string chkBenhVDMCHauThap { get; set; }
            public string txtBenhVDMCHauThap { get; set; }
            public string dtBenhVDMCHauThap { get; set; }
            public string cboHoVHLDoSaVan { get; set; }
            public string chkHoVHLDoSaVan { get; set; }
            public string txtHoVHLDoSaVan { get; set; }
            public string dtHoVHLDoSaVan { get; set; }
            public string cboSuyThanManTinh { get; set; }
            public string chkSuyThanManTinh { get; set; }
            public string txtSuyThanManTinh { get; set; }
            public string dtSuyThanManTinh { get; set; }
            public string cboBenhDMNgoaiBien { get; set; }
            public string chkBenhDMNgoaiBien { get; set; }
            public string txtBenhDMNgoaiBien { get; set; }
            public string dtBenhDMNgoaiBien { get; set; }
            public string cboBenhTimBamSinh { get; set; }
            public string chkBenhTimBamSinh { get; set; }
            public string txtBenhTimBamSinh { get; set; }
            public string dtBenhTimBamSinh { get; set; }
            public string cboTienSuXaTriHoaTri { get; set; }
            public string chkTienSuXaTriHoaTri { get; set; }
            public string txtTienSuXaTriHoaTri { get; set; }
            public string dtTienSuXaTriHoaTri { get; set; }
            public string cboBenhNgoaiMangTim { get; set; }
            public string chkBenhNgoaiMangTim { get; set; }
            public string txtBenhNgoaiMangTim { get; set; }
            public string dtBenhNgoaiMangTim { get; set; }
            public string cboTienSuChanThuong { get; set; }
            public string chkTienSuChanThuong { get; set; }
            public string txtTienSuChanThuong { get; set; }
            public string dtTienSuChanThuong { get; set; }
            public string txt12Khac { get; set; }
            public string cboDatStentNongDMV { get; set; }
            public string chkDatStentNongDMV { get; set; }
            public string txtDatStentNongDMV { get; set; }
            public string dtDatStentNongDMV { get; set; }
            public string cboCABG { get; set; }
            public string chkCABG { get; set; }
            public string txtCABG { get; set; }
            public string dtCABG { get; set; }
            public string cboDatMayTaoNhip { get; set; }
            public string chkDatMayTaoNhip { get; set; }
            public string txtDatMayTaoNhip { get; set; }
            public string dtDatMayTaoNhip { get; set; }
            public string cboPTThayVanTimDatVongVan { get; set; }
            public string chkPTThayVanTimDatVongVan { get; set; }
            public string txtPTThayVanTimDatVongVan { get; set; }
            public string dtPTThayVanTimDatVongVan { get; set; }
            public string cboPTSuaChuaTBS { get; set; }
            public string chkPTSuaChuaTBS { get; set; }
            public string txtPTSuaChuaTBS { get; set; }
            public string dtPTSuaChuaTBS { get; set; }
            public string txt13Khac { get; set; }
            public string txtTienSuGiaDinh { get; set; }
            public string cboNgheNghiep { get; set; }
            public string txtNgheNghiepCode { get; set; }
            public string chk17MotMinhCo { get; set; }
            public string chk17MotMinhKhong { get; set; }
            public string chk17NguoiThanCo { get; set; }
            public string chk17NguoiThanKhong { get; set; }
            public string chk18Co { get; set; }
            public string chk18Khong { get; set; }
            public string chk19DocThan { get; set; }
            public string chk19LapGiaDinh { get; set; }
            public string chk19Goa { get; set; }
            public string chk19LyThan { get; set; }
            public string chk110KhongBietChu { get; set; }
            public string chk110DaiHoc { get; set; }
            public string chk110PhoThong { get; set; }
            public string chk110SauDaiHoc { get; set; }
            public string txt110TinhTrangKinhTe { get; set; }
            public string chk112HutThuocLaChuaBaoGio { get; set; }
            public string chk112HutThuocLaDaBo { get; set; }
            public string chk112HutThuocLaDangHut { get; set; }
            public string chk112UongRuouChuaBaoGio { get; set; }
            public string chk112UongRuouHangNgay { get; set; }
            public string chk112UongRuouThiThoang { get; set; }
            public string chk112UongRuouDaBo { get; set; }
            public string chk112TheDucKhongTap { get; set; }
            public string chk112TheDuc12Lan { get; set; }
            public string chk112TheDuc35Lan { get; set; }
            public string chk112TheDucHangNgay { get; set; }
            public string chk112MaTuyChuaBaoGio { get; set; }
            public string chk112MaTuyDaBo { get; set; }
            public string chk112MaTuyDangSuDung { get; set; }
            public string chk20TaiKhamDinhKi { get; set; }
            public string chk20Co { get; set; }
            public string chk20Khong { get; set; }
            public string chk21Co { get; set; }
            public string chk21Khong { get; set; }
            public string chk22DienHinh { get; set; }
            public string chk22KhongDau { get; set; }
            public string chk22KhongDienHinh { get; set; }
            public string chk23I { get; set; }
            public string chk23II { get; set; }
            public string chk23III { get; set; }
            public string chk23IV { get; set; }
            public string chk24Co { get; set; }
            public string chk24Khong { get; set; }
            public string chk25Co { get; set; }
            public string chk25Khong { get; set; }
            public string chk26Co { get; set; }
            public string chk26Khong { get; set; }
            public string chk27Co { get; set; }
            public string chk27Khong { get; set; }
            public string chk28Co { get; set; }
            public string chk28Khong { get; set; }
            public string chk29Co { get; set; }
            public string chk29Khong { get; set; }
            public string chk210Co { get; set; }
            public string chk210Khong { get; set; }
            public string chk211Co { get; set; }
            public string chk211Khong { get; set; }
            public string txt212NhanXetKhac { get; set; }
            public string chk31NhipXoang { get; set; }
            public string chk31BlockAV { get; set; }
            public string chk31RungCuongNhi { get; set; }
            public string chk31LBBB { get; set; }
            public string txt31TanSo { get; set; }
            public string chk32DD { get; set; }
            public string chk32Ds { get; set; }
            public string chk32EF { get; set; }
            public string chk32PAPs { get; set; }
            public string txtSdtLienHe { get; set; }
            public string txtTenNguoiDuocNoiChuyen { get; set; }
            public string txtQuanHe { get; set; }
            public string dtNgayNhapVien { get; set; }
            public string chkI4SuyTimNangLenCo { get; set; }
            public string chkI4SuyTimNangLenKhong { get; set; }
            public string chkI4TBMNCo { get; set; }
            public string chkI4TBMNKhong { get; set; }
            public string chkI4NhiemKhuanCo { get; set; }
            public string chkI4NhiemKhuanKhong { get; set; }
            public string chkI4BienCoMachVanhCo { get; set; }
            public string chkI4BienCoMachVanhKhong { get; set; }
            public string chkI4BienCoMachKhacCo { get; set; }
            public string chkI4BienCoMachKhacKhong { get; set; }
            public string chkI4RoiLoanDOngMauCo { get; set; }
            public string chkI4RoiLoanDOngMauKhong { get; set; }
            public string txtI4LyDoKhac { get; set; }
            public string chkII1Co { get; set; }
            public string chkII1Khong { get; set; }
            public string chkII2Co { get; set; }
            public string chkII2Khong { get; set; }
            public string chkII3Co { get; set; }
            public string chkII3Khong { get; set; }
            public string chkII4Co { get; set; }
            public string chkII4Khong { get; set; }
            public string chkII5Co { get; set; }
            public string chkII5Khong { get; set; }
            public string chkII6Co { get; set; }
            public string chkII6Khong { get; set; }
            public string spinEditII7 { get; set; }
            public string chkII8I { get; set; }
            public string chkII8II { get; set; }
            public string chkII8III { get; set; }
            public string chkII8IV { get; set; }
            public string chkIII1Co { get; set; }
            public string chkIII1Khong { get; set; }
            public string chkIII2Co { get; set; }
            public string chkIII2Khong { get; set; }
            public string chkIII3Co { get; set; }
            public string chkIII3Khong { get; set; }
            public string chkIII4Co { get; set; }
            public string chkIII4Khong { get; set; }
            public string chkIII4KhongSuDung { get; set; }
            public string chkIII5Co { get; set; }
            public string chkIII5Khong { get; set; }
            public string chkIII5KhongSuDung { get; set; }

            public string spinEditIV1 { get; set; }
            public string spinEditIV2 { get; set; }
            public string spinEditIV3 { get; set; }
            public string spinEditIV4 { get; set; }
            public string spinEditIV5 { get; set; }
            public string lblTenBenhNhan { get; set; }
            public string dtNgayTienHanhDaoTao { get; set; }
            public string txtHoTenNguoiDaoTao { get; set; }
            public string chkBN41 { get; set; }
            public string chkBN42 { get; set; }
            public string chkBN43 { get; set; }
            public string chkBN44 { get; set; }
            public string chkBN45 { get; set; }
            public string chkBN46 { get; set; }

            public string chkI111 { get; set; }
            public string chkI112 { get; set; }
            public string chkI113 { get; set; }
            public string chkI114 { get; set; }
            public string chkI115 { get; set; }

            public string txtTenTaiLieuHuongDan { get; set; }

            public string chkTheoDoiCo { get; set; }
            public string chkTheoDoiKhong { get; set; }

        }

        public class PhoneADO
        {
            public int Action { get; set; }
            public string PHONE_NUMBER { get; set; }
            public string RELATIVE_NAME { get; set; }
        }

        public class ServiceReqADO
        {
            public long? START_TIME { get; set; }
            public long? FINISH_TIME { get; set; }
            public string REQUEST_DEPARTMENT_NAME { get; set; }
            public string EXECUTE_LOGINNAME { get; set; }
            public string RESULT { get; set; }
        }


    }
}
