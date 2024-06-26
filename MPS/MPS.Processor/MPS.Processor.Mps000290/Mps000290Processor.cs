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
using MPS.Processor.Mps000290.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000290
{
    class Mps000290Processor : AbstractProcessor
    {
        Mps000290PDO rdo;
        public Mps000290Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000290PDO)rdoBase;
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
                                _SetSingleKeyADO.Patient_Name = item.VALUE;
                                break;
                            case "lblIcdName":
                                _SetSingleKeyADO.lblIcdName = item.VALUE;
                                break;
                            case "cboPhuauthuatVien":
                                _SetSingleKeyADO.cboPhuauthuatVien = item.VALUE.Split('|')[0];
                                break;
                            case "cboBacSyGayMe":
                                _SetSingleKeyADO.cboBacSyGayMe = item.VALUE.Split('|')[0];
                                break;
                            case "cboDieuDuongGayMe":
                                _SetSingleKeyADO.cboDieuDuongGayMe = item.VALUE.Split('|')[0];
                                break;
                            case "chkPhauThuaChuongTrinh":
                                _SetSingleKeyADO.chkPhauThuaChuongTrinh = "X";
                                break;
                            case "chkPhauThuatCapCuu":
                                _SetSingleKeyADO.chkPhauThuatCapCuu = "X";
                                break;
                            case "chkLoaiDB":
                                _SetSingleKeyADO.chkLoaiDB = "X";
                                break;
                            case "chkLoai1":
                                _SetSingleKeyADO.chkLoai1 = "X";
                                break;
                            case "chkLoai2":
                                _SetSingleKeyADO.chkLoai2 = "X";
                                break;
                            case "chkLoai3":
                                _SetSingleKeyADO.chkLoai3 = "X";
                                break;
                            case "chkTruocGM1Co":
                                _SetSingleKeyADO.chkTruocGM1Co = "X";
                                break;
                            case "chkTruocGM1Khong":
                                _SetSingleKeyADO.chkTruocGM1Khong = "X";
                                break;
                            case "chkTruocGM2Co":
                                _SetSingleKeyADO.chkTruocGM2Co = "X";
                                break;
                            case "chkTruocGM2Khong":
                                _SetSingleKeyADO.chkTruocGM2Khong = "X";
                                break;
                            case "chkTruocGM3Co":
                                _SetSingleKeyADO.chkTruocGM3Co = "X";
                                break;
                            case "chkTruocGM3Khong":
                                _SetSingleKeyADO.chkTruocGM3Khong = "X";
                                break;
                            case "chkTruocGM4Co":
                                _SetSingleKeyADO.chkTruocGM4Co = "X";
                                break;
                            case "chkTruocGM4Khong":
                                _SetSingleKeyADO.chkTruocGM4Khong = "X";
                                break;
                            case "chkTruocGM5Co":
                                _SetSingleKeyADO.chkTruocGM5Co = "X";
                                break;
                            case "chkTruocGM5Khong":
                                _SetSingleKeyADO.chkTruocGM5Khong = "X";
                                break;
                            case "chkTruocGM6TSDUCo":
                                _SetSingleKeyADO.chkTruocGM6TSDUCo = "X";
                                break;
                            case "chkTruocGM6TSDUKhong":
                                _SetSingleKeyADO.chkTruocGM6TSDUKhong = "X";
                                break;
                            case "chkTruocGM6DTKCo":
                                _SetSingleKeyADO.chkTruocGM6DTKCo = "X";
                                break;
                            case "chkTruocGM6DTKKhong":
                                _SetSingleKeyADO.chkTruocGM6DTKKhong = "X";
                                break;
                            case "chkTruocGM6MMCo":
                                _SetSingleKeyADO.chkTruocGM6MMCo = "X";
                                break;
                            case "chkTruocGM6MMKhong":
                                _SetSingleKeyADO.chkTruocGM6MMKhong = "X";
                                break;
                            case "txtTKRG1":
                                _SetSingleKeyADO.txtTKRG1 = item.VALUE;
                                break;
                            case "chkTKRD2Co":
                                _SetSingleKeyADO.chkTKRD2Co = "X";
                                break;
                            case "chkTKRD2Khong":
                                _SetSingleKeyADO.chkTKRD2Khong = "X";
                                break;
                            case "chkTKRD3BTCo":
                                _SetSingleKeyADO.chkTKRD3BTCo = "X";
                                break;
                            case "chkTKRD3BTKhong":
                                _SetSingleKeyADO.chkTKRD3BTKhong = "X";
                                break;
                            case "dtTKRD3":
                                _SetSingleKeyADO.dtTKRD3 = item.VALUE;
                                break;
                            case "chkTKRD3MMCo":
                                _SetSingleKeyADO.chkTKRD3MMCo = "X";
                                break;
                            case "chkTKRD3MMKhong":
                                _SetSingleKeyADO.chkTKRD3MMKhong = "X";
                                break;
                            case "chkTKRD4Co":
                                _SetSingleKeyADO.chkTKRD4Co = "X";
                                break;
                            case "chkTKRD4Khong":
                                _SetSingleKeyADO.chkTKRD4Khong = "X";
                                break;
                            case "chkTKRD51Co":
                                _SetSingleKeyADO.chkTKRD51Co = "X";
                                break;
                            case "chkTKRD51Khong":
                                _SetSingleKeyADO.chkTKRD51Khong = "X";
                                break;
                            case "chkTKRD52Co":
                                _SetSingleKeyADO.chkTKRD52Co = "X";
                                break;
                            case "chkTKRD52Khong":
                                _SetSingleKeyADO.chkTKRD52Khong = "X";
                                break;
                            case "chkTKRD6Co":
                                _SetSingleKeyADO.chkTKRD6Co = "X";
                                break;
                            case "chkTKRD6Khong":
                                _SetSingleKeyADO.chkTKRD6Khong = "X";
                                break;
                            case "chkNB11":
                                _SetSingleKeyADO.chkNB11 = "X";
                                break;
                            case "chkNB12":
                                _SetSingleKeyADO.chkNB12 = "X";
                                break;
                            case "chkNB13":
                                _SetSingleKeyADO.chkNB13 = "X";
                                break;
                            case "chkNB14":
                                _SetSingleKeyADO.chkNB14 = "X";
                                break;
                            case "txtNB2":
                                _SetSingleKeyADO.txtNB2 = item.VALUE;
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
            public string Patient_Name { get; set; }
            public string lblIcdName { get; set; }
            public string cboPhuauthuatVien { get; set; }
            public string cboBacSyGayMe { get; set; }
            public string cboDieuDuongGayMe { get; set; }
            public string chkPhauThuaChuongTrinh { get; set; }
            public string chkPhauThuatCapCuu { get; set; }
            public string chkLoaiDB { get; set; }
            public string chkLoai1 { get; set; }
            public string chkLoai2 { get; set; }
            public string chkLoai3 { get; set; }
            public string chkTruocGM1Co { get; set; }
            public string chkTruocGM1Khong { get; set; }
            public string chkTruocGM2Co { get; set; }
            public string chkTruocGM2Khong { get; set; }
            public string chkTruocGM3Co { get; set; }
            public string chkTruocGM3Khong { get; set; }
            public string chkTruocGM4Co { get; set; }
            public string chkTruocGM4Khong { get; set; }
            public string chkTruocGM5Co { get; set; }
            public string chkTruocGM5Khong { get; set; }
            public string chkTruocGM6TSDUCo { get; set; }
            public string chkTruocGM6TSDUKhong { get; set; }
            public string chkTruocGM6DTKCo { get; set; }
            public string chkTruocGM6DTKKhong { get; set; }
            public string chkTruocGM6MMCo { get; set; }
            public string chkTruocGM6MMKhong { get; set; }
            public string txtTKRG1 { get; set; }
            public string chkTKRD2Co { get; set; }
            public string chkTKRD2Khong { get; set; }
            public string chkTKRD3BTCo { get; set; }
            public string chkTKRD3BTKhong { get; set; }
            public string dtTKRD3 { get; set; }
            public string chkTKRD3MMCo { get; set; }
            public string chkTKRD3MMKhong { get; set; }
            public string chkTKRD4Co { get; set; }
            public string chkTKRD4Khong { get; set; }
            public string chkTKRD51Co { get; set; }
            public string chkTKRD51Khong { get; set; }
            public string chkTKRD52Co { get; set; }
            public string chkTKRD52Khong { get; set; }
            public string chkTKRD6Co { get; set; }
            public string chkTKRD6Khong { get; set; }
            public string chkNB11 { get; set; }
            public string chkNB12 { get; set; }
            public string chkNB13 { get; set; }
            public string chkNB14 { get; set; }
            public string txtNB2 { get; set; }
        }

    }
}
