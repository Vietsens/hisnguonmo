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
using MPS.Processor.Mps000292.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000292
{
    class Mps000292Processor : AbstractProcessor
    {
        Mps000292PDO rdo;
        public Mps000292Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000292PDO)rdoBase;
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
                SetSingleKeyADO _SetSingleKeyADO = new SetSingleKeyADO();
                if (rdo._SarFormDatas != null && rdo._SarFormDatas.Count > 0)
                {
                    foreach (var item in rdo._SarFormDatas)
                    {
                        switch (item.KEY)
                        {
                            case "lblPatientName":
                                _SetSingleKeyADO.lblPatientName = item.VALUE;
                                break;
                            case "lblAge":
                                _SetSingleKeyADO.lblAge = item.VALUE;
                                break;
                            case "lblGenderName":
                                _SetSingleKeyADO.lblGenderName = item.VALUE;
                                break;
                            case "lblAddress":
                                _SetSingleKeyADO.lblAddress = item.VALUE;
                                break;
                            case "cboBuong":
                                _SetSingleKeyADO.cboBuong = item.VALUE.Split('|')[0];
                                break;
                            case "cboGiuong":
                                _SetSingleKeyADO.cboGiuong = item.VALUE.Split('|')[0];
                                break;
                            case "lblInTime":
                                _SetSingleKeyADO.lblInTime = item.VALUE;
                                break;
                            case "txtLyDoVaoVien":
                                _SetSingleKeyADO.txtLyDoVaoVien = item.VALUE;
                                break;
                            case "txtTrieuChungLamSang":
                                _SetSingleKeyADO.txtTrieuChungLamSang = item.VALUE;
                                break;
                            case "txtXetNghiemLamSang":
                                _SetSingleKeyADO.txtXetNghiemLamSang = item.VALUE;
                                break;
                            case "txtCongThucHongCau":
                                _SetSingleKeyADO.txtCongThucHongCau = item.VALUE;
                                break;
                            case "txtCongThucBachCau":
                                _SetSingleKeyADO.txtCongThucBachCau = item.VALUE;
                                break;
                            case "txtCongThucN":
                                _SetSingleKeyADO.txtCongThucN = item.VALUE;
                                break;
                            case "txtCongThucL":
                                _SetSingleKeyADO.txtCongThucL = item.VALUE;
                                break;
                            case "txtNhomMau":
                                _SetSingleKeyADO.txtNhomMau = item.VALUE;
                                break;
                            case "txtThoiGianMauChay":
                                _SetSingleKeyADO.txtThoiGianMauChay = item.VALUE;
                                break;
                            case "txtThoiGianMauDong":
                                _SetSingleKeyADO.txtThoiGianMauDong = item.VALUE;
                                break;
                            case "txtHuetSacTo":
                                _SetSingleKeyADO.txtHuetSacTo = item.VALUE;
                                break;
                            case "txtHongCau":
                                _SetSingleKeyADO.txtHongCau = item.VALUE;
                                break;
                            case "txtBachCau":
                                _SetSingleKeyADO.txtBachCau = item.VALUE;
                                break;
                            case "txtProtein":
                                _SetSingleKeyADO.txtProtein = item.VALUE;
                                break;
                            case "txtTrieuNuc":
                                _SetSingleKeyADO.txtTrieuNuc = item.VALUE;
                                break;
                            case "txtDuongNieu":
                                _SetSingleKeyADO.txtDuongNieu = item.VALUE;
                                break;
                            case "txtSacToMat":
                                _SetSingleKeyADO.txtSacToMat = item.VALUE;
                                break;
                            case "txtSinhHoaUre":
                                _SetSingleKeyADO.txtSinhHoaUre = item.VALUE;
                                break;
                            case "txtCreatimin":
                                _SetSingleKeyADO.txtCreatimin = item.VALUE;
                                break;
                            case "txtGlucose":
                                _SetSingleKeyADO.txtGlucose = item.VALUE;
                                break;
                            case "txtBilirubinTP":
                                _SetSingleKeyADO.txtBilirubinTP = item.VALUE;
                                break;
                            case "txtBilirubinTT":
                                _SetSingleKeyADO.txtBilirubinTT = item.VALUE;
                                break;
                            case "txtBilirubinGT":
                                _SetSingleKeyADO.txtBilirubinGT = item.VALUE;
                                break;
                            case "txtProteinTP":
                                _SetSingleKeyADO.txtProteinTP = item.VALUE;
                                break;
                            case "txtAlbumin":
                                _SetSingleKeyADO.txtAlbumin = item.VALUE;
                                break;
                            case "txtGOT":
                                _SetSingleKeyADO.txtGOT = item.VALUE;
                                break;
                            case "txtGPT":
                                _SetSingleKeyADO.txtGPT = item.VALUE;
                                break;
                            case "txtDongMauPT":
                                _SetSingleKeyADO.txtDongMauPT = item.VALUE;
                                break;
                            case "txtAPTT":
                                _SetSingleKeyADO.txtAPTT = item.VALUE;
                                break;
                            case "txtFibrinogen":
                                _SetSingleKeyADO.txtFibrinogen = item.VALUE;
                                break;
                            case "txtXetNghiemHIV":
                                _SetSingleKeyADO.txtXetNghiemHIV = item.VALUE;
                                break;
                            case "txtHbsAg":
                                _SetSingleKeyADO.txtHbsAg = item.VALUE;
                                break;
                            case "txtHCV":
                                _SetSingleKeyADO.txtHCV = item.VALUE;
                                break;
                            case "txtXQuang":
                                _SetSingleKeyADO.txtXQuang = item.VALUE;
                                break;
                            case "txtSieuAm":
                                _SetSingleKeyADO.txtSieuAm = item.VALUE;
                                break;
                            case "txtDienTim":
                                _SetSingleKeyADO.txtDienTim = item.VALUE;
                                break;
                            case "txtXetNghiemKhac":
                                _SetSingleKeyADO.txtXetNghiemKhac = item.VALUE;
                                break;
                            case "txtChanDoan":
                                _SetSingleKeyADO.txtChanDoan = item.VALUE;
                                break;
                            case "txtPhuongPhapPhauThuat":
                                _SetSingleKeyADO.txtPhuongPhapPhauThuat = item.VALUE;
                                break;
                            case "txtPhuongPhapVoCam":
                                _SetSingleKeyADO.txtPhuongPhapVoCam = item.VALUE;
                                break;
                            case "cboPhauThuatVien":
                                _SetSingleKeyADO.cboPhauThuatVien = item.VALUE.Split('|')[0];
                                break;
                            case "dtNgayPhauThuat":
                                _SetSingleKeyADO.dtNgayPhauThuat = item.VALUE;
                                break;
                            case "txtDuTruMauKhiMo":
                                _SetSingleKeyADO.txtDuTruMauKhiMo = item.VALUE;
                                break;
                            case "cboDuyetLanhDao":
                                _SetSingleKeyADO.cboDuyetLanhDao = item.VALUE.Split('|')[0];
                                break;
                            case "cboBacSyGayMe":
                                _SetSingleKeyADO.cboBacSyGayMe = item.VALUE.Split('|')[0];
                                break;
                            case "cboTruongPhongKH":
                                _SetSingleKeyADO.cboTruongPhongKH = item.VALUE.Split('|')[0];
                                break;
                            case "cboTruongKhoa":
                                _SetSingleKeyADO.cboTruongKhoa = item.VALUE.Split('|')[0];
                                break;
                            case "cboBacSyDieuTri":
                                _SetSingleKeyADO.cboBacSyDieuTri = item.VALUE.Split('|')[0];
                                break;
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
            public string lblPatientName { get; set; }
            public string lblAge { get; set; }
            public string lblGenderName { get; set; }
            public string lblAddress { get; set; }
            public string cboBuong { get; set; }
            public string cboGiuong { get; set; }
            public string lblInTime { get; set; }
            public string txtLyDoVaoVien { get; set; }
            public string txtTrieuChungLamSang { get; set; }
            public string txtXetNghiemLamSang { get; set; }
            public string txtCongThucHongCau { get; set; }
            public string txtCongThucBachCau { get; set; }
            public string txtCongThucN { get; set; }
            public string txtCongThucL { get; set; }
            public string txtNhomMau { get; set; }
            public string txtThoiGianMauChay { get; set; }
            public string txtThoiGianMauDong { get; set; }
            public string txtHuetSacTo { get; set; }
            public string txtHongCau { get; set; }
            public string txtBachCau { get; set; }
            public string txtProtein { get; set; }
            public string txtTrieuNuc { get; set; }
            public string txtDuongNieu { get; set; }
            public string txtSacToMat { get; set; }
            public string txtSinhHoaUre { get; set; }
            public string txtCreatimin { get; set; }
            public string txtGlucose { get; set; }
            public string txtBilirubinTP { get; set; }
            public string txtBilirubinTT { get; set; }
            public string txtBilirubinGT { get; set; }
            public string txtProteinTP { get; set; }
            public string txtAlbumin { get; set; }
            public string txtGOT { get; set; }
            public string txtGPT { get; set; }
            public string txtDongMauPT { get; set; }
            public string txtAPTT { get; set; }
            public string txtFibrinogen { get; set; }
            public string txtXetNghiemHIV { get; set; }
            public string txtHbsAg { get; set; }
            public string txtHCV { get; set; }
            public string txtXQuang { get; set; }
            public string txtSieuAm { get; set; }
            public string txtDienTim { get; set; }
            public string txtXetNghiemKhac { get; set; }
            public string txtChanDoan { get; set; }
            public string txtPhuongPhapPhauThuat { get; set; }
            public string txtPhuongPhapVoCam { get; set; }
            public string cboPhauThuatVien { get; set; }
            public string dtNgayPhauThuat { get; set; }
            public string txtDuTruMauKhiMo { get; set; }
            public string cboDuyetLanhDao { get; set; }
            public string cboBacSyGayMe { get; set; }
            public string cboTruongPhongKH { get; set; }
            public string cboTruongKhoa { get; set; }
            public string cboBacSyDieuTri { get; set; }
        }

    }
}
