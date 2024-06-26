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
using MPS.Processor.Mps000289.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000289
{
    class Mps000289Processor : AbstractProcessor
    {
        Mps000289PDO rdo;
        public Mps000289Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000289PDO)rdoBase;
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
                            case "lblAge":
                                _SetSingleKeyADO.Age = item.VALUE;
                                break;
                            case "lblGenderName":
                                _SetSingleKeyADO.Gender_Name = item.VALUE;
                                break;
                            case "lblCode":
                                _SetSingleKeyADO.Code = item.VALUE;
                                break;
                            case "lblDepartment":
                                _SetSingleKeyADO.Department = item.VALUE;
                                break;
                            case "lblBranch":
                                _SetSingleKeyADO.Branch_Name = item.VALUE;
                                break;
                            case "lblInCode":
                                _SetSingleKeyADO.In_Code = item.VALUE;
                                break;
                            case "lblIcdName":
                                _SetSingleKeyADO.Icd_Name = item.VALUE;
                                break;
                            case "txtTienSuDiUng":
                                _SetSingleKeyADO.Tien_Su_Di_Ung = item.VALUE;
                                break;
                            case "chkVeSinh":
                                _SetSingleKeyADO.Ve_Sinh = item.VALUE;
                                break;
                            case "chkSonMongTay":
                                _SetSingleKeyADO.Son_Mong_Tay = item.VALUE;
                                break;
                            case "chkCatMongTay":
                                _SetSingleKeyADO.Cat_Mong_Tay = item.VALUE;
                                break;
                            case "chkThaoDoTrangSuc":
                                _SetSingleKeyADO.Thao_Do_Trang_Suc = item.VALUE;
                                break;
                            case "chkThaoRangGia":
                                _SetSingleKeyADO.Thao_Rang_Gia = item.VALUE;
                                break;
                            case "chkQuanAo":
                                _SetSingleKeyADO.Quan_Ao = item.VALUE;
                                break;
                            case "chkNguoiBenhDVSR":
                                _SetSingleKeyADO.Nguoi_Benh_DVSR = item.VALUE;
                                break;
                            case "chkVeSinhVungDaRon":
                                _SetSingleKeyADO.Ve_Sinh_Vung_Da_Ron = item.VALUE;
                                break;
                            case "chkBangVoTrung":
                                _SetSingleKeyADO.Bang_Vo_Trung = item.VALUE;
                                break;
                            case "chkHoSoBenhAn":
                                _SetSingleKeyADO.Ho_So_Benh_An = item.VALUE;
                                break;
                            case "chkPhieuCamDoan":
                                _SetSingleKeyADO.Phieu_Cam_Doan = item.VALUE;
                                break;
                            case "chkPhieuXetNghiem":
                                _SetSingleKeyADO.Phieu_Xet_Nghiem = item.VALUE;
                                break;
                            case "chkPhimChup":
                                _SetSingleKeyADO.Phim_Chup = item.VALUE;
                                break;
                            case "chkDienTim":
                                _SetSingleKeyADO.Dien_Tim = item.VALUE;
                                break;
                            case "chkNhinAn":
                                _SetSingleKeyADO.Nhin_An = item.VALUE;
                                break;
                            case "spinMach":
                                _SetSingleKeyADO.spin_Mach = item.VALUE;
                                break;
                            case "spinNhietDo":
                                _SetSingleKeyADO.spin_Nhiet_Do = item.VALUE;
                                break;
                            case "spinHuyetAp1":
                                _SetSingleKeyADO.spin_Huyet_Ap1 = item.VALUE;
                                break;
                            case "spinHuyetAp2":
                                _SetSingleKeyADO.spin_Huyet_Ap2 = item.VALUE;
                                break;
                            case "spinNhipTho":
                                _SetSingleKeyADO.spin_Nhip_Tho = item.VALUE;
                                break;
                            case "spinChieuCao":
                                _SetSingleKeyADO.spin_Chieu_Cao = item.VALUE;
                                break;
                            case "spinCanNang":
                                _SetSingleKeyADO.spin_Can_Nang = item.VALUE;
                                break;
                            case "dtChuyenLenKhoa":
                                _SetSingleKeyADO.dt_Chuyen_Len_Khoa = item.VALUE;
                                break;
                            case "dtNhanVaoKhoa":
                                _SetSingleKeyADO.dt_Nhan_Vao_Khoa = item.VALUE;
                                break;
                            case "txtDescription":
                                _SetSingleKeyADO.txt_Description = item.VALUE;
                                break;
                            case "cboNguoiChuanBi":
                                _SetSingleKeyADO.cbo_Nguoi_Chuan_Bi = item.VALUE.Split('|')[0];
                                break;
                            case "cboNguoiGiao":
                                _SetSingleKeyADO.cbo_Nguoi_Giao = item.VALUE.Split('|')[0];
                                break;
                            case "cboNguoiNhan":
                                _SetSingleKeyADO.cbo_Nguoi_Nhan = item.VALUE.Split('|')[0];
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
            public string Age { get; set; }
            public string Gender_Name { get; set; }
            public string Icd_Name { get; set; }
            public string In_Code { get; set; }
            public string Code { get; set; }
            public string Department { get; set; }
            public string Branch_Name { get; set; }
            public string Tien_Su_Di_Ung { get; set; }
            public string Ve_Sinh { get; set; }
            public string Son_Mong_Tay { get; set; }
            public string Cat_Mong_Tay { get; set; }
            public string Thao_Do_Trang_Suc { get; set; }
            public string Thao_Rang_Gia { get; set; }
            public string Quan_Ao { get; set; }
            public string Nguoi_Benh_DVSR { get; set; }
            public string Ve_Sinh_Vung_Da_Ron { get; set; }
            public string Bang_Vo_Trung { get; set; }
            public string Ho_So_Benh_An { get; set; }
            public string Phieu_Cam_Doan { get; set; }
            public string Phieu_Xet_Nghiem { get; set; }
            public string Phim_Chup { get; set; }
            public string Dien_Tim { get; set; }
            public string Nhin_An { get; set; }
            public string spin_Mach { get; set; }
            public string spin_Nhiet_Do { get; set; }
            public string spin_Huyet_Ap1 { get; set; }
            public string spin_Huyet_Ap2 { get; set; }
            public string spin_Nhip_Tho { get; set; }
            public string spin_Chieu_Cao { get; set; }
            public string spin_Can_Nang { get; set; }
            public string dt_Chuyen_Len_Khoa { get; set; }
            public string dt_Nhan_Vao_Khoa { get; set; }
            public string txt_Description { get; set; }
            public string cbo_Nguoi_Chuan_Bi { get; set; }
            public string cbo_Nguoi_Giao { get; set; }
            public string cbo_Nguoi_Nhan { get; set; }
        }

    }
}
