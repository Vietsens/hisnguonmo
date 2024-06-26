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
using MPS.Processor.Mps000310.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000310
{
    class Mps000310Processor : AbstractProcessor
    {
        Mps000310PDO rdo;
        public Mps000310Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000310PDO)rdoBase;
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
                            case "lblPatientCode":
                                _SetSingleKeyADO.lblPatientCode = item.VALUE;
                                break;
                            case "lblPatientName":
                                _SetSingleKeyADO.lblPatientName = item.VALUE;
                                break;
                            case "lblAge":
                                _SetSingleKeyADO.lblAge = item.VALUE;
                                break;
                            case "lblGenderName":
                                _SetSingleKeyADO.lblGenderName = item.VALUE;
                                break;
                            //case "lblAddress":
                            //    _SetSingleKeyADO.lblAddress = item.VALUE;
                            //    break; 
                            case "chkLCDChanDoanXacDinhDBMV":
                                _SetSingleKeyADO.chkLCDChanDoanXacDinhDBMV = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLCDKhaoSatSongCon":
                                _SetSingleKeyADO.chkLCDKhaoSatSongCon = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLCDHepVanDMC":
                                _SetSingleKeyADO.chkLCDHepVanDMC = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLCDKhac":
                                _SetSingleKeyADO.chkLCDKhac = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkLCDKhac":
                                _SetSingleKeyADO.txtForchkLCDKhac = item.VALUE;
                                break;
                            case "txtChieuCao":
                                _SetSingleKeyADO.txtChieuCao = item.VALUE;
                                break;
                            case "txtCanNang":
                                _SetSingleKeyADO.txtCanNang = item.VALUE;
                                break;
                            case "txtMP":
                                _SetSingleKeyADO.txtMP = item.VALUE;
                                break;
                            case "txtMT":
                                _SetSingleKeyADO.txtMT = item.VALUE;
                                break;
                            case "chkKNCTAHKQGSUcCheBeta":
                                _SetSingleKeyADO.chkKNCTAHKQGSUcCheBeta = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkKNCTAHKQGSUcCheCali":
                                _SetSingleKeyADO.chkKNCTAHKQGSUcCheCali = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkKNCTAHKQGSlitrate":
                                _SetSingleKeyADO.chkKNCTAHKQGSlitrate = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkKNCTAHKQGSTrimethazidime":
                                _SetSingleKeyADO.chkKNCTAHKQGSTrimethazidime = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkKNCTAHKQGSDigoxin":
                                _SetSingleKeyADO.chkKNCTAHKQGSDigoxin = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkKNCTAHKQGSKhac":
                                _SetSingleKeyADO.chkKNCTAHKQGSKhac = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkKNCTAHKQGSDigoxin":
                                _SetSingleKeyADO.txtForchkKNCTAHKQGSDigoxin = item.VALUE;
                                break;
                            case "txtLucNghiNhipTim":
                                _SetSingleKeyADO.txtLucNghiNhipTim = item.VALUE;
                                break;
                            case "txtLucNghiHuyetAp":
                                _SetSingleKeyADO.txtLucNghiHuyetAp = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineGD5":
                                _SetSingleKeyADO.txtLucNghiDobutamineGD5 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineNhipTim5":
                                _SetSingleKeyADO.txtLucNghiDobutamineNhipTim5 = item.VALUE;
                                break;
                            case "txtLucNghiDobutaminehuyetAp5":
                                _SetSingleKeyADO.txtLucNghiDobutaminehuyetAp5 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineGD10":
                                _SetSingleKeyADO.txtLucNghiDobutamineGD10 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineNhipTim10":
                                _SetSingleKeyADO.txtLucNghiDobutamineNhipTim10 = item.VALUE;
                                break;
                            case "txtLucNghiDobutaminehuyetAp10":
                                _SetSingleKeyADO.txtLucNghiDobutaminehuyetAp10 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineGD20":
                                _SetSingleKeyADO.txtLucNghiDobutamineGD20 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineNhipTim20":
                                _SetSingleKeyADO.txtLucNghiDobutamineNhipTim20 = item.VALUE;
                                break;
                            case "txtLucNghiDobutaminehuyetAp20":
                                _SetSingleKeyADO.txtLucNghiDobutaminehuyetAp20 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineGD30":
                                _SetSingleKeyADO.txtLucNghiDobutamineGD30 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineNhipTim30":
                                _SetSingleKeyADO.txtLucNghiDobutamineNhipTim30 = item.VALUE;
                                break;
                            case "txtLucNghiDobutaminehuyetAp30":
                                _SetSingleKeyADO.txtLucNghiDobutaminehuyetAp30 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineGD40":
                                _SetSingleKeyADO.txtLucNghiDobutamineGD40 = item.VALUE;
                                break;
                            case "txtLucNghiDobutamineNhipTim40":
                                _SetSingleKeyADO.txtLucNghiDobutamineNhipTim40 = item.VALUE;
                                break;
                            case "txtLucNghiDobutaminehuyetAp40":
                                _SetSingleKeyADO.txtLucNghiDobutaminehuyetAp40 = item.VALUE;
                                break;
                            case "chkThemAtropine025":
                                _SetSingleKeyADO.chkThemAtropine025 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkThemAtropine050":
                                _SetSingleKeyADO.chkThemAtropine050 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkThemAtropine075":
                                _SetSingleKeyADO.chkThemAtropine075 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkThemAtropine100":
                                _SetSingleKeyADO.chkThemAtropine100 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkThemAtropineKhac":
                                _SetSingleKeyADO.txtForchkThemAtropineKhac = item.VALUE;
                                break;
                            case "txtBilirubinTT":
                                _SetSingleKeyADO.txtBilirubinTT = item.VALUE;
                                break;
                            case "txtAlbumin":
                                _SetSingleKeyADO.txtAlbumin = item.VALUE;
                                break;
                            case "txtSauNgungThuoc2pNhipTim":
                                _SetSingleKeyADO.txtSauNgungThuoc2pNhipTim = item.VALUE;
                                break;
                            case "txtSauNgungThuoc2pHuyetAp":
                                _SetSingleKeyADO.txtSauNgungThuoc2pHuyetAp = item.VALUE;
                                break;
                            case "txtSauNgungThuoc4pNhipTim":
                                _SetSingleKeyADO.txtSauNgungThuoc4pNhipTim = item.VALUE;
                                break;
                            case "txtSauNgungThuoc4pHuyetAp":
                                _SetSingleKeyADO.txtSauNgungThuoc4pHuyetAp = item.VALUE;
                                break;
                            case "txtSauNgungThuocForXXXp":
                                _SetSingleKeyADO.txtSauNgungThuocForXXXp = item.VALUE;
                                break;
                            case "txtSauNgungThuocXXXpNhipTim":
                                _SetSingleKeyADO.txtSauNgungThuocXXXpNhipTim = item.VALUE;
                                break;
                            case "txtSauNgungThuocXXXpHuyetAp":
                                _SetSingleKeyADO.txtSauNgungThuocXXXpHuyetAp = item.VALUE;
                                break;
                            case "cbodaytruoc1":
                                _SetSingleKeyADO.cbodaytruoc1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruoc2":
                                _SetSingleKeyADO.cbodaytruoc2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruoc3":
                                _SetSingleKeyADO.cbodaytruoc3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruoc4":
                                _SetSingleKeyADO.cbodaytruoc4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocvach1":
                                _SetSingleKeyADO.cbodaytruocvach1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocvach2":
                                _SetSingleKeyADO.cbodaytruocvach2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocvach3":
                                _SetSingleKeyADO.cbodaytruocvach3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocvach4":
                                _SetSingleKeyADO.cbodaytruocvach4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoivach1":
                                _SetSingleKeyADO.cbodayduoivach1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoivach2":
                                _SetSingleKeyADO.cbodayduoivach2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoivach3":
                                _SetSingleKeyADO.cbodayduoivach3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoivach4":
                                _SetSingleKeyADO.cbodayduoivach4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoi1":
                                _SetSingleKeyADO.cbodayduoi1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoi2":
                                _SetSingleKeyADO.cbodayduoi2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoi3":
                                _SetSingleKeyADO.cbodayduoi3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoi4":
                                _SetSingleKeyADO.cbodayduoi4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoiben1":
                                _SetSingleKeyADO.cbodayduoiben1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoiben2":
                                _SetSingleKeyADO.cbodayduoiben2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoiben3":
                                _SetSingleKeyADO.cbodayduoiben3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodayduoiben4":
                                _SetSingleKeyADO.cbodayduoiben4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocben1":
                                _SetSingleKeyADO.cbodaytruocben1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocben2":
                                _SetSingleKeyADO.cbodaytruocben2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocben3":
                                _SetSingleKeyADO.cbodaytruocben3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbodaytruocben4":
                                _SetSingleKeyADO.cbodaytruocben4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruoc1":
                                _SetSingleKeyADO.cbogiuatruoc1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruoc2":
                                _SetSingleKeyADO.cbogiuatruoc2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruoc3":
                                _SetSingleKeyADO.cbogiuatruoc3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruoc4":
                                _SetSingleKeyADO.cbogiuatruoc4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocvach1":
                                _SetSingleKeyADO.cbogiuatruocvach1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocvach2":
                                _SetSingleKeyADO.cbogiuatruocvach2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocvach3":
                                _SetSingleKeyADO.cbogiuatruocvach3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocvach4":
                                _SetSingleKeyADO.cbogiuatruocvach4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoivach1":
                                _SetSingleKeyADO.cbogiuaduoivach1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoivach2":
                                _SetSingleKeyADO.cbogiuaduoivach2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoivach3":
                                _SetSingleKeyADO.cbogiuaduoivach3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoivach4":
                                _SetSingleKeyADO.cbogiuaduoivach4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoi1":
                                _SetSingleKeyADO.cbogiuaduoi1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoi2":
                                _SetSingleKeyADO.cbogiuaduoi2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoi3":
                                _SetSingleKeyADO.cbogiuaduoi3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoi4":
                                _SetSingleKeyADO.cbogiuaduoi4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoiben1":
                                _SetSingleKeyADO.cbogiuaduoiben1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoiben2":
                                _SetSingleKeyADO.cbogiuaduoiben2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoiben3":
                                _SetSingleKeyADO.cbogiuaduoiben3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuaduoiben4":
                                _SetSingleKeyADO.cbogiuaduoiben4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocben1":
                                _SetSingleKeyADO.cbogiuatruocben1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocben2":
                                _SetSingleKeyADO.cbogiuatruocben2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocben3":
                                _SetSingleKeyADO.cbogiuatruocben3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbogiuatruocben4":
                                _SetSingleKeyADO.cbogiuatruocben4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomtruoc1":
                                _SetSingleKeyADO.cbomomtruoc1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomtruoc2":
                                _SetSingleKeyADO.cbomomtruoc2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomtruoc3":
                                _SetSingleKeyADO.cbomomtruoc3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomtruoc4":
                                _SetSingleKeyADO.cbomomtruoc4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoivach1":
                                _SetSingleKeyADO.cbomomduoivach1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoivach2":
                                _SetSingleKeyADO.cbomomduoivach2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoivach3":
                                _SetSingleKeyADO.cbomomduoivach3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoivach4":
                                _SetSingleKeyADO.cbomomduoivach4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoi1":
                                _SetSingleKeyADO.cbomomduoi1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoi2":
                                _SetSingleKeyADO.cbomomduoi2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoi3":
                                _SetSingleKeyADO.cbomomduoi3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomduoi4":
                                _SetSingleKeyADO.cbomomduoi4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomben1":
                                _SetSingleKeyADO.cbomomben1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomben2":
                                _SetSingleKeyADO.cbomomben2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomben3":
                                _SetSingleKeyADO.cbomomben3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomben4":
                                _SetSingleKeyADO.cbomomben4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomthuc1":
                                _SetSingleKeyADO.cbomomthuc1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomthuc2":
                                _SetSingleKeyADO.cbomomthuc2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomthuc3":
                                _SetSingleKeyADO.cbomomthuc3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbomomthuc4":
                                _SetSingleKeyADO.cbomomthuc4 = item.VALUE.Split('|')[0];
                                break;
                            case "cbochontheocot1":
                                _SetSingleKeyADO.cbochontheocot1 = item.VALUE.Split('|')[0];
                                break;
                            case "cbochontheocot2":
                                _SetSingleKeyADO.cbochontheocot2 = item.VALUE.Split('|')[0];
                                break;
                            case "cbochontheocot3":
                                _SetSingleKeyADO.cbochontheocot3 = item.VALUE.Split('|')[0];
                                break;
                            case "cbochontheocot4":
                                _SetSingleKeyADO.cbochontheocot4 = item.VALUE.Split('|')[0];
                                break;
                            case "chkLSConDauThatNgucOnDinhCCSC":
                                _SetSingleKeyADO.chkLSConDauThatNgucOnDinhCCSC = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSTuongDuongDauNguc":
                                _SetSingleKeyADO.chkLSTuongDuongDauNguc = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSBenhCoTimThieuMauCB":
                                _SetSingleKeyADO.chkLSBenhCoTimThieuMauCB = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSMNCTCapNgayThu":
                                _SetSingleKeyADO.chkLSMNCTCapNgayThu = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSHetDau":
                                _SetSingleKeyADO.chkLSHetDau = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSDauNgucKhongDB":
                                _SetSingleKeyADO.chkLSDauNgucKhongDB = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkLSKhac":
                                _SetSingleKeyADO.chkLSKhac = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkLSHetDau":
                                _SetSingleKeyADO.txtForchkLSHetDau = item.VALUE;
                                break;
                            case "txtForchkConDauThatNgucOnDinhCCSC":
                                _SetSingleKeyADO.txtForchkConDauThatNgucOnDinhCCSC = item.VALUE;
                                break;
                            case "txtForchkLSMNCTCapNgayThu":
                                _SetSingleKeyADO.txtForchkLSMNCTCapNgayThu = item.VALUE;
                                break;
                            case "txtForchkLSKhac":
                                _SetSingleKeyADO.txtForchkLSKhac = item.VALUE;
                                break;
                            case "txtTanSoTimDich":
                                _SetSingleKeyADO.txtTanSoTimDich = item.VALUE;
                                break;
                            case "picPhanVungThatTrai":
                                if (!String.IsNullOrEmpty(item.VALUE))
                                {
                                    var rsDLStream = Inventec.Fss.Client.FileDownload.GetFile(item.VALUE);
                                    if (rsDLStream != null && rsDLStream.Length > 0)
                                    {
                                        rsDLStream.Position = 0;
                                        _SetSingleKeyADO.picPhanVungThatTrai = rsDLStream.ToArray();
                                    }
                                }
                                break;
                            case "chkBienDoiECGKhongThayDoi":
                                _SetSingleKeyADO.chkBienDoiECGKhongThayDoi = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkBienDoiECGSTTChenhLen":
                                _SetSingleKeyADO.chkBienDoiECGSTTChenhLen = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkBienDoiECGSTTChenhXuong":
                                _SetSingleKeyADO.chkBienDoiECGSTTChenhXuong = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkBienDoiECGSTTLoanNhip":
                                _SetSingleKeyADO.chkBienDoiECGSTTLoanNhip = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkBienDoiECGSTTChenhLen":
                                _SetSingleKeyADO.txtForchkBienDoiECGSTTChenhLen = item.VALUE;
                                break;
                            case "txtBienDoiECGSTTSttCLChuyenDao":
                                _SetSingleKeyADO.txtBienDoiECGSTTSttCLChuyenDao = item.VALUE;
                                break;
                            case "txtForchkBienDoiECGSTTChenhXuong":
                                _SetSingleKeyADO.txtForchkBienDoiECGSTTChenhXuong = item.VALUE;
                                break;
                            case "txtBienDoiECGSTTSttCLXChuyenDao":
                                _SetSingleKeyADO.txtBienDoiECGSTTSttCLXChuyenDao = item.VALUE;
                                break;
                            case "txtForchkBienDoiECGSTTLoanNhip":
                                _SetSingleKeyADO.txtForchkBienDoiECGSTTLoanNhip = item.VALUE;
                                break;
                            case "chkTHRHATang220_120":
                                _SetSingleKeyADO.chkTHRHATang220_120 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHATut20":
                                _SetSingleKeyADO.chkTHRHATut20 = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHADauNguc":
                                _SetSingleKeyADO.chkTHRHADauNguc = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHAkhac":
                                _SetSingleKeyADO.chkTHRHAkhac = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHALoanNhipNguyHiem":
                                _SetSingleKeyADO.chkTHRHALoanNhipNguyHiem = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHABienDoiSTDangKe":
                                _SetSingleKeyADO.chkTHRHABienDoiSTDangKe = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "chkTHRHARLVDVung2V":
                                _SetSingleKeyADO.chkTHRHARLVDVung2V = (item.VALUE.Split('|').Length > 1 ? item.VALUE.Split('|')[1] == "1" ? "x" : "" : "");
                                break;
                            case "txtForchkTHRHAkhac":
                                _SetSingleKeyADO.txtForchkTHRHAkhac = item.VALUE;
                                break;
                            case "txtXyTri":
                                _SetSingleKeyADO.txtXyTri = item.VALUE;
                                break;
                            case "txtDanhGiaChung":
                                _SetSingleKeyADO.txtDanhGiaChung = item.VALUE;
                                break;
                            case "txtKetLuan":
                                _SetSingleKeyADO.txtKetLuan = item.VALUE;
                                break;
                            case "txtDeNghi":
                                _SetSingleKeyADO.txtDeNghi = item.VALUE;
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
            public string lblPatientCode { get; set; }
            public string lblPatientName { get; set; }
            public string lblAge { get; set; }
            public string lblGenderName { get; set; }

            public string chkLCDChanDoanXacDinhDBMV { get; set; }
            public string chkLCDKhaoSatSongCon { get; set; }
            public string chkLCDHepVanDMC { get; set; }
            public string chkLCDKhac { get; set; }
            public string txtForchkLCDKhac { get; set; }

            public string txtChieuCao { get; set; }
            public string txtCanNang { get; set; }
            public string txtMP { get; set; }
            public string txtMT { get; set; }

            public string chkKNCTAHKQGSUcCheBeta { get; set; }
            public string chkKNCTAHKQGSUcCheCali { get; set; }
            public string chkKNCTAHKQGSlitrate { get; set; }
            public string chkKNCTAHKQGSTrimethazidime { get; set; }
            public string chkKNCTAHKQGSDigoxin { get; set; }
            public string chkKNCTAHKQGSKhac { get; set; }
            public string txtForchkKNCTAHKQGSDigoxin { get; set; }

            public string txtLucNghiNhipTim { get; set; }
            public string txtLucNghiHuyetAp { get; set; }
            public string txtLucNghiDobutamineGD5 { get; set; }
            public string txtLucNghiDobutamineNhipTim5 { get; set; }
            public string txtLucNghiDobutaminehuyetAp5 { get; set; }
            public string txtLucNghiDobutamineGD10 { get; set; }
            public string txtLucNghiDobutamineNhipTim10 { get; set; }
            public string txtLucNghiDobutaminehuyetAp10 { get; set; }
            public string txtLucNghiDobutamineGD20 { get; set; }
            public string txtLucNghiDobutamineNhipTim20 { get; set; }
            public string txtLucNghiDobutaminehuyetAp20 { get; set; }
            public string txtLucNghiDobutamineGD30 { get; set; }
            public string txtLucNghiDobutamineNhipTim30 { get; set; }
            public string txtLucNghiDobutaminehuyetAp30 { get; set; }
            public string txtLucNghiDobutamineGD40 { get; set; }
            public string txtLucNghiDobutamineNhipTim40 { get; set; }
            public string txtLucNghiDobutaminehuyetAp40 { get; set; }

            public string chkThemAtropine025 { get; set; }
            public string chkThemAtropine050 { get; set; }
            public string chkThemAtropine075 { get; set; }
            public string chkThemAtropine100 { get; set; }
            public string txtForchkThemAtropineKhac { get; set; }

            public string txtBilirubinTT { get; set; }
            public string txtAlbumin { get; set; }
            public string txtSauNgungThuoc2pNhipTim { get; set; }
            public string txtSauNgungThuoc2pHuyetAp { get; set; }
            public string txtSauNgungThuoc4pNhipTim { get; set; }
            public string txtSauNgungThuoc4pHuyetAp { get; set; }
            public string txtSauNgungThuocForXXXp { get; set; }
            public string txtSauNgungThuocXXXpNhipTim { get; set; }
            public string txtSauNgungThuocXXXpHuyetAp { get; set; }
            public string cbodaytruoc1 { get; set; }
            public string cbodaytruoc2 { get; set; }
            public string cbodaytruoc3 { get; set; }
            public string cbodaytruoc4 { get; set; }
            public string cbodaytruocvach1 { get; set; }
            public string cbodaytruocvach2 { get; set; }
            public string cbodaytruocvach3 { get; set; }
            public string cbodaytruocvach4 { get; set; }
            public string cbodayduoivach1 { get; set; }
            public string cbodayduoivach2 { get; set; }
            public string cbodayduoivach3 { get; set; }
            public string cbodayduoivach4 { get; set; }
            public string cbodayduoi1 { get; set; }
            public string cbodayduoi2 { get; set; }
            public string cbodayduoi3 { get; set; }
            public string cbodayduoi4 { get; set; }
            public string cbodayduoiben1 { get; set; }
            public string cbodayduoiben2 { get; set; }
            public string cbodayduoiben3 { get; set; }
            public string cbodayduoiben4 { get; set; }
            public string cbodaytruocben1 { get; set; }
            public string cbodaytruocben2 { get; set; }
            public string cbodaytruocben3 { get; set; }
            public string cbodaytruocben4 { get; set; }
            public string cbogiuatruoc1 { get; set; }
            public string cbogiuatruoc2 { get; set; }
            public string cbogiuatruoc3 { get; set; }
            public string cbogiuatruoc4 { get; set; }
            public string cbogiuatruocvach1 { get; set; }
            public string cbogiuatruocvach2 { get; set; }
            public string cbogiuatruocvach3 { get; set; }
            public string cbogiuatruocvach4 { get; set; }
            public string cbogiuaduoivach1 { get; set; }
            public string cbogiuaduoivach2 { get; set; }
            public string cbogiuaduoivach3 { get; set; }
            public string cbogiuaduoivach4 { get; set; }
            public string cbogiuaduoi1 { get; set; }
            public string cbogiuaduoi2 { get; set; }
            public string cbogiuaduoi3 { get; set; }
            public string cbogiuaduoi4 { get; set; }
            public string cbogiuaduoiben1 { get; set; }
            public string cbogiuaduoiben2 { get; set; }
            public string cbogiuaduoiben3 { get; set; }
            public string cbogiuaduoiben4 { get; set; }
            public string cbogiuatruocben1 { get; set; }
            public string cbogiuatruocben2 { get; set; }
            public string cbogiuatruocben3 { get; set; }
            public string cbogiuatruocben4 { get; set; }
            public string cbomomtruoc1 { get; set; }
            public string cbomomtruoc2 { get; set; }
            public string cbomomtruoc3 { get; set; }
            public string cbomomtruoc4 { get; set; }
            public string cbomomduoivach1 { get; set; }
            public string cbomomduoivach2 { get; set; }
            public string cbomomduoivach3 { get; set; }
            public string cbomomduoivach4 { get; set; }
            public string cbomomduoi1 { get; set; }
            public string cbomomduoi2 { get; set; }
            public string cbomomduoi3 { get; set; }
            public string cbomomduoi4 { get; set; }
            public string cbomomben1 { get; set; }
            public string cbomomben2 { get; set; }
            public string cbomomben3 { get; set; }
            public string cbomomben4 { get; set; }
            public string cbomomthuc1 { get; set; }
            public string cbomomthuc2 { get; set; }
            public string cbomomthuc3 { get; set; }
            public string cbomomthuc4 { get; set; }
            public string cbochontheocot1 { get; set; }
            public string cbochontheocot2 { get; set; }
            public string cbochontheocot3 { get; set; }
            public string cbochontheocot4 { get; set; }

            public string chkLSConDauThatNgucOnDinhCCSC { get; set; }
            public string chkLSTuongDuongDauNguc { get; set; }
            public string chkLSBenhCoTimThieuMauCB { get; set; }
            public string chkLSMNCTCapNgayThu { get; set; }
            public string chkLSHetDau { get; set; }
            public string chkLSDauNgucKhongDB { get; set; }
            public string chkLSKhac { get; set; }
            public string txtForchkLSHetDau { get; set; }
            public string txtForchkConDauThatNgucOnDinhCCSC { get; set; }
            public string txtForchkLSMNCTCapNgayThu { get; set; }
            public string txtForchkLSKhac { get; set; }

            public string txtTanSoTimDich { get; set; }
            public Byte[] picPhanVungThatTrai { get; set; }

            public string chkBienDoiECGKhongThayDoi { get; set; }
            public string chkBienDoiECGSTTChenhLen { get; set; }
            public string chkBienDoiECGSTTChenhXuong { get; set; }
            public string chkBienDoiECGSTTLoanNhip { get; set; }
            public string txtForchkBienDoiECGSTTChenhLen { get; set; }
            public string txtBienDoiECGSTTSttCLChuyenDao { get; set; }
            public string txtForchkBienDoiECGSTTChenhXuong { get; set; }
            public string txtBienDoiECGSTTSttCLXChuyenDao { get; set; }
            public string txtForchkBienDoiECGSTTLoanNhip { get; set; }

            public string chkTHRHATang220_120 { get; set; }
            public string chkTHRHATut20 { get; set; }
            public string chkTHRHADauNguc { get; set; }
            public string chkTHRHAkhac { get; set; }
            public string chkTHRHALoanNhipNguyHiem { get; set; }
            public string chkTHRHABienDoiSTDangKe { get; set; }
            public string chkTHRHARLVDVung2V { get; set; }
            public string txtForchkTHRHAkhac { get; set; }

            public string txtXyTri { get; set; }
            public string txtDanhGiaChung { get; set; }
            public string txtKetLuan { get; set; }
            public string txtDeNghi { get; set; }
        }

    }
}
