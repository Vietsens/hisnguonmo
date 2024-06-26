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
using Inventec.Core;
using MPS.ProcessorBase;
using System;
using System.Linq;
using MPS.Processor.Mps000332.PDO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
namespace MPS.Processor.Mps000332
{
    class Mps000332Processor : AbstractProcessor
    {
        Mps000332PDO rdo;
        List<GiaiDoanMoADO> _GiaiDoanMoADOs;
        public Mps000332Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000332PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "GiaiDoanMos", this._GiaiDoanMoADOs);
              
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        public void SetSingleKey()
        {
            try
            {
                this._GiaiDoanMoADOs = new List<GiaiDoanMoADO>();
                SetSingleKeyADO _SetSingleKeyADO = new SetSingleKeyADO();
                if (rdo.Treatment != null && rdo.Treatment.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblPatientCode, rdo.Treatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblPatientName, rdo.Treatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblAge, AgeUtil.CalculateFullAge(rdo.Treatment.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblNhomMau, rdo.SereServPttt.BLOOD_ABO_CODE));
                    if (rdo.Treatment.TDL_PATIENT_GENDER_NAME == "Nam")
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNam,"X"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNam, ""));
                    }
                    if (rdo.Treatment.TDL_PATIENT_GENDER_NAME == "Nữ")
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNu, "X"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNu, ""));
                    }
                }
                else if (rdo.ServiceReq != null && rdo.ServiceReq.ID > 0)
                {
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblPatientCode, rdo.ServiceReq.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblPatientName, rdo.ServiceReq.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblAge, AgeUtil.CalculateFullAge(rdo.ServiceReq.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblNhomMau, rdo.SereServPttt.BLOOD_ABO_CODE));
                    if (rdo.ServiceReq.TDL_PATIENT_GENDER_NAME == "Nam")
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNam, "X"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNam, ""));
                    }
                    if (rdo.ServiceReq.TDL_PATIENT_GENDER_NAME == "Nữ")
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNu, "X"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.ckNu, ""));
                    }
                    }
                else if (rdo.SereServPttt != null && rdo.SereServPttt.ID!=null)
                {
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtChuanDoanTruoc, rdo.SereServPttt.BEFORE_PTTT_ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.txtMay, rdo.SereServPttt.BLOOD_ABO_CODE));
                }
                else if (rdo.SereServExt != null && rdo.SereServExt.ID!=null)
                {
                    SetSingleKey(new KeyValue(Mps000332ExtendSingleKey.lblNhomMau, rdo.SereServExt.MACHINE_CODE));
                }
                if (rdo._SarFormDatas != null && rdo._SarFormDatas.Count > 0)
                {
                    foreach (var item in rdo._SarFormDatas)
                    {
                        switch (item.KEY)
                        {
                            case "lblPatientName":
                                _SetSingleKeyADO.lblPatientName = item.VALUE;
                                break;
                            case "lblGenderName":
                                _SetSingleKeyADO.lblGenderName = item.VALUE;
                                break;
                       
                            case "lblAge":
                                _SetSingleKeyADO.lblAge = item.VALUE;
                                break;
                            case "lblChieuCao":
                                _SetSingleKeyADO.lblChieuCao = item.VALUE;
                                break;
                            case "lblCanNang":
                                _SetSingleKeyADO.lblCanNang = item.VALUE;
                                break;
                            case "lblNhomMau":
                                _SetSingleKeyADO.lblNhomMau = item.VALUE;
                                break;
                            case "ckNam":
                                _SetSingleKeyADO.ckNam = "X";
                                break;
                            case "ckNu":
                                _SetSingleKeyADO.ckNu = "X";
                                break;
               

                        
                            case "txtChuanDoanTruoc":
                                _SetSingleKeyADO.txtChuanDoanTruoc = item.VALUE;
                                break;
                            case "txtChuanDoanSau":
                                _SetSingleKeyADO.txtChuanDoanSau = item.VALUE;
                                break;
                            case "txtDuKienMo":
                                _SetSingleKeyADO.txtDuKienMo = item.VALUE;
                                break;
                            case "txtDaMo":
                                _SetSingleKeyADO.txtDaMo = item.VALUE;
                                break;
                            case "txtThuocChuanBi":
                                _SetSingleKeyADO.txtThuocChuanBi = item.VALUE;
                                break;
                            case "txtPhuongPhapGayMe":
                                _SetSingleKeyADO.txtPhuongPhapGayMe = item.VALUE;
                                break;
                            case "txtKhoiMe":
                                _SetSingleKeyADO.txtKhoiMe = item.VALUE;
                                break;
                            case "txtNhomMo":
                                _SetSingleKeyADO.txtNhomMo = item.VALUE;
                                break;
                            case "txtNhomGayMeHoiSuc":
                                _SetSingleKeyADO.txtNhomGayMeHoiSuc = item.VALUE;
                                break;
                            case "txtTuTheTrenBanMo":
                                _SetSingleKeyADO.txtTuTheTrenBanMo = item.VALUE;
                                break;
                            case "txtHeThong":
                                _SetSingleKeyADO.txtHeThong = item.VALUE;
                                break;
                            case "txtNgay":
                                _SetSingleKeyADO.txtNgay = item.VALUE;
                                break;
                            case "txtMay":
                                _SetSingleKeyADO.txtMay = item.VALUE;
                                break;



                            case "GiaiDoanMo1":
                                var ss = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GiaiDoanMoADO>>(item.VALUE);
                                if (ss != null)
                                    this._GiaiDoanMoADOs.AddRange(ss);
                                break;
                            case "GiaiDoanMo2":
                                var ss2 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GiaiDoanMoADO>>(item.VALUE);
                                if (ss2 != null)
                                    this._GiaiDoanMoADOs.AddRange(ss2);
                                break;

                            case "GiaiDoanMo3":
                                var ss3 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GiaiDoanMoADO>>(item.VALUE);
                                if (ss3 != null)
                                    this._GiaiDoanMoADOs.AddRange(ss3);
                                break;


                            case "txtThuocGayMe1":
                                _SetSingleKeyADO.txtThuocGayMe1 = item.VALUE;
                                break;
                            case "txtThuocGayMe2":
                                _SetSingleKeyADO.txtThuocGayMe2 = item.VALUE;
                                break;
                            case "txtThuocGayMe3":
                                _SetSingleKeyADO.txtThuocGayMe3 = item.VALUE;
                                break;
                            case "txtThuocGayMe4":
                                _SetSingleKeyADO.txtThuocGayMe4 = item.VALUE;
                                break;
                            case "txtThuocGayMe5":
                                _SetSingleKeyADO.txtThuocGayMe5 = item.VALUE;
                                break;
                            case "txtThuocGayMe6":
                                _SetSingleKeyADO.txtThuocGayMe6 = item.VALUE;
                                break;

                            case "txtThuocGMHSC1H1":
                                _SetSingleKeyADO.txtThuocGMHSC1H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H1":
                                _SetSingleKeyADO.txtThuocGMHSC2H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H1":
                                _SetSingleKeyADO.txtThuocGMHSC3H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H1":
                                _SetSingleKeyADO.txtThuocGMHSC4H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H1":
                                _SetSingleKeyADO.txtThuocGMHSC5H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H1":
                                _SetSingleKeyADO.txtThuocGMHSC6H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H1":
                                _SetSingleKeyADO.txtThuocGMHSC7H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H1":
                                _SetSingleKeyADO.txtThuocGMHSC8H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H1":
                                _SetSingleKeyADO.txtThuocGMHSC9H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H1":
                                _SetSingleKeyADO.txtThuocGMHSC10H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H1":
                                _SetSingleKeyADO.txtThuocGMHSC11H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H1":
                                _SetSingleKeyADO.txtThuocGMHSC12H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H1":
                                _SetSingleKeyADO.txtThuocGMHSC13H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H1":
                                _SetSingleKeyADO.txtThuocGMHSC14H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H1":
                                _SetSingleKeyADO.txtThuocGMHSC15H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H1":
                                _SetSingleKeyADO.txtThuocGMHSC16H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H1":
                                _SetSingleKeyADO.txtThuocGMHSC17H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H1":
                                _SetSingleKeyADO.txtThuocGMHSC18H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H1":
                                _SetSingleKeyADO.txtThuocGMHSC19H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H1":
                                _SetSingleKeyADO.txtThuocGMHSC20H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H1":
                                _SetSingleKeyADO.txtThuocGMHSC21H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H1":
                                _SetSingleKeyADO.txtThuocGMHSC22H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H1":
                                _SetSingleKeyADO.txtThuocGMHSC23H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H1":
                                _SetSingleKeyADO.txtThuocGMHSC24H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H1":
                                _SetSingleKeyADO.txtThuocGMHSC25H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H1":
                                _SetSingleKeyADO.txtThuocGMHSC26H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H1":
                                _SetSingleKeyADO.txtThuocGMHSC27H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H1":
                                _SetSingleKeyADO.txtThuocGMHSC28H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H1":
                                _SetSingleKeyADO.txtThuocGMHSC29H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H1":
                                _SetSingleKeyADO.txtThuocGMHSC30H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H1":
                                _SetSingleKeyADO.txtThuocGMHSC31H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H1":
                                _SetSingleKeyADO.txtThuocGMHSC32H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H1":
                                _SetSingleKeyADO.txtThuocGMHSC33H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H1":
                                _SetSingleKeyADO.txtThuocGMHSC34H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H1":
                                _SetSingleKeyADO.txtThuocGMHSC35H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H1":
                                _SetSingleKeyADO.txtThuocGMHSC36H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H1":
                                _SetSingleKeyADO.txtThuocGMHSC37H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H1":
                                _SetSingleKeyADO.txtThuocGMHSC38H1 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H1":
                                _SetSingleKeyADO.txtThuocGMHSC39H1 = item.VALUE;
                                break;

                            case "txtThuocGMHSC1H2":
                                _SetSingleKeyADO.txtThuocGMHSC1H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H2":
                                _SetSingleKeyADO.txtThuocGMHSC2H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H2":
                                _SetSingleKeyADO.txtThuocGMHSC3H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H2":
                                _SetSingleKeyADO.txtThuocGMHSC4H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H2":
                                _SetSingleKeyADO.txtThuocGMHSC5H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H2":
                                _SetSingleKeyADO.txtThuocGMHSC6H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H2":
                                _SetSingleKeyADO.txtThuocGMHSC7H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H2":
                                _SetSingleKeyADO.txtThuocGMHSC8H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H2":
                                _SetSingleKeyADO.txtThuocGMHSC9H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H2":
                                _SetSingleKeyADO.txtThuocGMHSC10H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H2":
                                _SetSingleKeyADO.txtThuocGMHSC11H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H2":
                                _SetSingleKeyADO.txtThuocGMHSC12H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H2":
                                _SetSingleKeyADO.txtThuocGMHSC13H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H2":
                                _SetSingleKeyADO.txtThuocGMHSC14H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H2":
                                _SetSingleKeyADO.txtThuocGMHSC15H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H2":
                                _SetSingleKeyADO.txtThuocGMHSC16H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H2":
                                _SetSingleKeyADO.txtThuocGMHSC17H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H2":
                                _SetSingleKeyADO.txtThuocGMHSC18H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H2":
                                _SetSingleKeyADO.txtThuocGMHSC19H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H2":
                                _SetSingleKeyADO.txtThuocGMHSC20H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H2":
                                _SetSingleKeyADO.txtThuocGMHSC21H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H2":
                                _SetSingleKeyADO.txtThuocGMHSC22H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H2":
                                _SetSingleKeyADO.txtThuocGMHSC23H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H2":
                                _SetSingleKeyADO.txtThuocGMHSC24H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H2":
                                _SetSingleKeyADO.txtThuocGMHSC25H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H2":
                                _SetSingleKeyADO.txtThuocGMHSC26H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H2":
                                _SetSingleKeyADO.txtThuocGMHSC27H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H2":
                                _SetSingleKeyADO.txtThuocGMHSC28H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H2":
                                _SetSingleKeyADO.txtThuocGMHSC29H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H2":
                                _SetSingleKeyADO.txtThuocGMHSC30H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H2":
                                _SetSingleKeyADO.txtThuocGMHSC31H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H2":
                                _SetSingleKeyADO.txtThuocGMHSC32H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H2":
                                _SetSingleKeyADO.txtThuocGMHSC33H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H2":
                                _SetSingleKeyADO.txtThuocGMHSC34H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H2":
                                _SetSingleKeyADO.txtThuocGMHSC35H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H2":
                                _SetSingleKeyADO.txtThuocGMHSC36H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H2":
                                _SetSingleKeyADO.txtThuocGMHSC37H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H2":
                                _SetSingleKeyADO.txtThuocGMHSC38H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H2":
                                _SetSingleKeyADO.txtThuocGMHSC39H2 = item.VALUE;
                                break;
                            case "txtThuocGMHSC1H3":
                                _SetSingleKeyADO.txtThuocGMHSC1H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H3":
                                _SetSingleKeyADO.txtThuocGMHSC2H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H3":
                                _SetSingleKeyADO.txtThuocGMHSC3H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H3":
                                _SetSingleKeyADO.txtThuocGMHSC4H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H3":
                                _SetSingleKeyADO.txtThuocGMHSC5H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H3":
                                _SetSingleKeyADO.txtThuocGMHSC6H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H3":
                                _SetSingleKeyADO.txtThuocGMHSC7H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H3":
                                _SetSingleKeyADO.txtThuocGMHSC8H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H3":
                                _SetSingleKeyADO.txtThuocGMHSC9H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H3":
                                _SetSingleKeyADO.txtThuocGMHSC10H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H3":
                                _SetSingleKeyADO.txtThuocGMHSC11H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H3":
                                _SetSingleKeyADO.txtThuocGMHSC12H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H3":
                                _SetSingleKeyADO.txtThuocGMHSC13H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H3":
                                _SetSingleKeyADO.txtThuocGMHSC14H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H3":
                                _SetSingleKeyADO.txtThuocGMHSC15H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H3":
                                _SetSingleKeyADO.txtThuocGMHSC16H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H3":
                                _SetSingleKeyADO.txtThuocGMHSC17H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H3":
                                _SetSingleKeyADO.txtThuocGMHSC18H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H3":
                                _SetSingleKeyADO.txtThuocGMHSC19H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H3":
                                _SetSingleKeyADO.txtThuocGMHSC20H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H3":
                                _SetSingleKeyADO.txtThuocGMHSC21H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H3":
                                _SetSingleKeyADO.txtThuocGMHSC22H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H3":
                                _SetSingleKeyADO.txtThuocGMHSC23H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H3":
                                _SetSingleKeyADO.txtThuocGMHSC24H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H3":
                                _SetSingleKeyADO.txtThuocGMHSC25H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H3":
                                _SetSingleKeyADO.txtThuocGMHSC26H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H3":
                                _SetSingleKeyADO.txtThuocGMHSC27H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H3":
                                _SetSingleKeyADO.txtThuocGMHSC28H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H3":
                                _SetSingleKeyADO.txtThuocGMHSC29H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H3":
                                _SetSingleKeyADO.txtThuocGMHSC30H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H3":
                                _SetSingleKeyADO.txtThuocGMHSC31H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H3":
                                _SetSingleKeyADO.txtThuocGMHSC32H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H3":
                                _SetSingleKeyADO.txtThuocGMHSC33H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H3":
                                _SetSingleKeyADO.txtThuocGMHSC34H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H3":
                                _SetSingleKeyADO.txtThuocGMHSC35H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H3":
                                _SetSingleKeyADO.txtThuocGMHSC36H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H3":
                                _SetSingleKeyADO.txtThuocGMHSC37H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H3":
                                _SetSingleKeyADO.txtThuocGMHSC38H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H3":
                                _SetSingleKeyADO.txtThuocGMHSC39H3 = item.VALUE;
                                break;
                            case "txtThuocGMHSC1H4":
                                _SetSingleKeyADO.txtThuocGMHSC1H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H4":
                                _SetSingleKeyADO.txtThuocGMHSC2H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H4":
                                _SetSingleKeyADO.txtThuocGMHSC3H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H4":
                                _SetSingleKeyADO.txtThuocGMHSC4H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H4":
                                _SetSingleKeyADO.txtThuocGMHSC5H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H4":
                                _SetSingleKeyADO.txtThuocGMHSC6H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H4":
                                _SetSingleKeyADO.txtThuocGMHSC7H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H4":
                                _SetSingleKeyADO.txtThuocGMHSC8H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H4":
                                _SetSingleKeyADO.txtThuocGMHSC9H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H4":
                                _SetSingleKeyADO.txtThuocGMHSC10H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H4":
                                _SetSingleKeyADO.txtThuocGMHSC11H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H4":
                                _SetSingleKeyADO.txtThuocGMHSC12H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H4":
                                _SetSingleKeyADO.txtThuocGMHSC13H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H4":
                                _SetSingleKeyADO.txtThuocGMHSC14H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H4":
                                _SetSingleKeyADO.txtThuocGMHSC15H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H4":
                                _SetSingleKeyADO.txtThuocGMHSC16H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H4":
                                _SetSingleKeyADO.txtThuocGMHSC17H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H4":
                                _SetSingleKeyADO.txtThuocGMHSC18H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H4":
                                _SetSingleKeyADO.txtThuocGMHSC19H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H4":
                                _SetSingleKeyADO.txtThuocGMHSC20H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H4":
                                _SetSingleKeyADO.txtThuocGMHSC21H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H4":
                                _SetSingleKeyADO.txtThuocGMHSC22H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H4":
                                _SetSingleKeyADO.txtThuocGMHSC23H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H4":
                                _SetSingleKeyADO.txtThuocGMHSC24H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H4":
                                _SetSingleKeyADO.txtThuocGMHSC25H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H4":
                                _SetSingleKeyADO.txtThuocGMHSC26H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H4":
                                _SetSingleKeyADO.txtThuocGMHSC27H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H4":
                                _SetSingleKeyADO.txtThuocGMHSC28H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H4":
                                _SetSingleKeyADO.txtThuocGMHSC29H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H4":
                                _SetSingleKeyADO.txtThuocGMHSC30H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H4":
                                _SetSingleKeyADO.txtThuocGMHSC31H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H4":
                                _SetSingleKeyADO.txtThuocGMHSC32H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H4":
                                _SetSingleKeyADO.txtThuocGMHSC33H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H4":
                                _SetSingleKeyADO.txtThuocGMHSC34H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H4":
                                _SetSingleKeyADO.txtThuocGMHSC35H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H4":
                                _SetSingleKeyADO.txtThuocGMHSC36H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H4":
                                _SetSingleKeyADO.txtThuocGMHSC37H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H4":
                                _SetSingleKeyADO.txtThuocGMHSC38H4 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H4":
                                _SetSingleKeyADO.txtThuocGMHSC39H4 = item.VALUE;
                                break;

                            case "txtThuocGMHSC1H5":
                                _SetSingleKeyADO.txtThuocGMHSC1H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H5":
                                _SetSingleKeyADO.txtThuocGMHSC2H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H5":
                                _SetSingleKeyADO.txtThuocGMHSC3H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H5":
                                _SetSingleKeyADO.txtThuocGMHSC4H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H5":
                                _SetSingleKeyADO.txtThuocGMHSC5H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H5":
                                _SetSingleKeyADO.txtThuocGMHSC6H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H5":
                                _SetSingleKeyADO.txtThuocGMHSC7H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H5":
                                _SetSingleKeyADO.txtThuocGMHSC8H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H5":
                                _SetSingleKeyADO.txtThuocGMHSC9H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H5":
                                _SetSingleKeyADO.txtThuocGMHSC10H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H5":
                                _SetSingleKeyADO.txtThuocGMHSC11H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H5":
                                _SetSingleKeyADO.txtThuocGMHSC12H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H5":
                                _SetSingleKeyADO.txtThuocGMHSC13H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H5":
                                _SetSingleKeyADO.txtThuocGMHSC14H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H5":
                                _SetSingleKeyADO.txtThuocGMHSC15H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H5":
                                _SetSingleKeyADO.txtThuocGMHSC16H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H5":
                                _SetSingleKeyADO.txtThuocGMHSC17H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H5":
                                _SetSingleKeyADO.txtThuocGMHSC18H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H5":
                                _SetSingleKeyADO.txtThuocGMHSC19H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H5":
                                _SetSingleKeyADO.txtThuocGMHSC20H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H5":
                                _SetSingleKeyADO.txtThuocGMHSC21H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H5":
                                _SetSingleKeyADO.txtThuocGMHSC22H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H5":
                                _SetSingleKeyADO.txtThuocGMHSC23H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H5":
                                _SetSingleKeyADO.txtThuocGMHSC24H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H5":
                                _SetSingleKeyADO.txtThuocGMHSC25H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H5":
                                _SetSingleKeyADO.txtThuocGMHSC26H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H5":
                                _SetSingleKeyADO.txtThuocGMHSC27H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H5":
                                _SetSingleKeyADO.txtThuocGMHSC28H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H5":
                                _SetSingleKeyADO.txtThuocGMHSC29H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H5":
                                _SetSingleKeyADO.txtThuocGMHSC30H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H5":
                                _SetSingleKeyADO.txtThuocGMHSC31H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H5":
                                _SetSingleKeyADO.txtThuocGMHSC32H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H5":
                                _SetSingleKeyADO.txtThuocGMHSC33H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H5":
                                _SetSingleKeyADO.txtThuocGMHSC34H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H5":
                                _SetSingleKeyADO.txtThuocGMHSC35H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H5":
                                _SetSingleKeyADO.txtThuocGMHSC36H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H5":
                                _SetSingleKeyADO.txtThuocGMHSC37H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H5":
                                _SetSingleKeyADO.txtThuocGMHSC38H5 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H5":
                                _SetSingleKeyADO.txtThuocGMHSC39H5 = item.VALUE;
                                break;

                            case "txtThuocGMHSC1H6":
                                _SetSingleKeyADO.txtThuocGMHSC1H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC2H6":
                                _SetSingleKeyADO.txtThuocGMHSC2H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC3H6":
                                _SetSingleKeyADO.txtThuocGMHSC3H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC4H6":
                                _SetSingleKeyADO.txtThuocGMHSC4H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC5H6":
                                _SetSingleKeyADO.txtThuocGMHSC5H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC6H6":
                                _SetSingleKeyADO.txtThuocGMHSC6H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC7H6":
                                _SetSingleKeyADO.txtThuocGMHSC7H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC8H6":
                                _SetSingleKeyADO.txtThuocGMHSC8H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC9H6":
                                _SetSingleKeyADO.txtThuocGMHSC9H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC10H6":
                                _SetSingleKeyADO.txtThuocGMHSC10H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC11H6":
                                _SetSingleKeyADO.txtThuocGMHSC11H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC12H6":
                                _SetSingleKeyADO.txtThuocGMHSC12H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC13H6":
                                _SetSingleKeyADO.txtThuocGMHSC13H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC14H6":
                                _SetSingleKeyADO.txtThuocGMHSC14H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC15H6":
                                _SetSingleKeyADO.txtThuocGMHSC15H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC16H6":
                                _SetSingleKeyADO.txtThuocGMHSC16H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC17H6":
                                _SetSingleKeyADO.txtThuocGMHSC17H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC18H6":
                                _SetSingleKeyADO.txtThuocGMHSC18H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC19H6":
                                _SetSingleKeyADO.txtThuocGMHSC19H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC20H6":
                                _SetSingleKeyADO.txtThuocGMHSC20H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC21H6":
                                _SetSingleKeyADO.txtThuocGMHSC21H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC22H6":
                                _SetSingleKeyADO.txtThuocGMHSC22H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC23H6":
                                _SetSingleKeyADO.txtThuocGMHSC23H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC24H6":
                                _SetSingleKeyADO.txtThuocGMHSC24H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC25H6":
                                _SetSingleKeyADO.txtThuocGMHSC25H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC26H6":
                                _SetSingleKeyADO.txtThuocGMHSC26H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC27H6":
                                _SetSingleKeyADO.txtThuocGMHSC27H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC28H6":
                                _SetSingleKeyADO.txtThuocGMHSC28H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC29H6":
                                _SetSingleKeyADO.txtThuocGMHSC29H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC30H6":
                                _SetSingleKeyADO.txtThuocGMHSC30H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC31H6":
                                _SetSingleKeyADO.txtThuocGMHSC31H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC32H6":
                                _SetSingleKeyADO.txtThuocGMHSC32H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC33H6":
                                _SetSingleKeyADO.txtThuocGMHSC33H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC34H6":
                                _SetSingleKeyADO.txtThuocGMHSC34H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC35H6":
                                _SetSingleKeyADO.txtThuocGMHSC35H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC36H6":
                                _SetSingleKeyADO.txtThuocGMHSC36H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC37H6":
                                _SetSingleKeyADO.txtThuocGMHSC37H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC38H6":
                                _SetSingleKeyADO.txtThuocGMHSC38H6 = item.VALUE;
                                break;
                            case "txtThuocGMHSC39H6":
                                _SetSingleKeyADO.txtThuocGMHSC39H6 = item.VALUE;
                                break;


                            case "txtDichC1H1":
                                _SetSingleKeyADO.txtDichC1H1 = item.VALUE;
                                break;
                            case "txtDichC2H1":
                                _SetSingleKeyADO.txtDichC2H1 = item.VALUE;
                                break;
                            case "txtDichC3H1":
                                _SetSingleKeyADO.txtDichC3H1 = item.VALUE;
                                break;
                            case "txtDichC4H1":
                                _SetSingleKeyADO.txtDichC4H1 = item.VALUE;
                                break;
                            case "txtDichC5H1":
                                _SetSingleKeyADO.txtDichC5H1 = item.VALUE;
                                break;
                            case "txtDichC6H1":
                                _SetSingleKeyADO.txtDichC6H1 = item.VALUE;
                                break;
                            case "txtDichC7H1":
                                _SetSingleKeyADO.txtDichC7H1 = item.VALUE;
                                break;
                            case "txtDichC8H1":
                                _SetSingleKeyADO.txtDichC8H1 = item.VALUE;
                                break;
                            case "txtDichC9H1":
                                _SetSingleKeyADO.txtDichC9H1 = item.VALUE;
                                break;
                            case "txtDichC10H1":
                                _SetSingleKeyADO.txtDichC10H1 = item.VALUE;
                                break;
                            case "txtDichC11H1":
                                _SetSingleKeyADO.txtDichC11H1 = item.VALUE;
                                break;
                            case "txtDichC12H1":
                                _SetSingleKeyADO.txtDichC12H1 = item.VALUE;
                                break;
                            case "txtDichC13H1":
                                _SetSingleKeyADO.txtDichC13H1 = item.VALUE;
                                break;
                            case "txtDichC14H1":
                                _SetSingleKeyADO.txtDichC14H1 = item.VALUE;
                                break;
                            case "txtDichC15H1":
                                _SetSingleKeyADO.txtDichC15H1 = item.VALUE;
                                break;
                            case "txtDichC16H1":
                                _SetSingleKeyADO.txtDichC16H1 = item.VALUE;
                                break;
                            case "txtDichC17H1":
                                _SetSingleKeyADO.txtDichC17H1 = item.VALUE;
                                break;
                            case "txtDichC18H1":
                                _SetSingleKeyADO.txtDichC18H1 = item.VALUE;
                                break;
                            case "txtDichC19H1":
                                _SetSingleKeyADO.txtDichC19H1 = item.VALUE;
                                break;
                            case "txtDichC20H1":
                                _SetSingleKeyADO.txtDichC20H1 = item.VALUE;
                                break;
                            case "txtDichC21H1":
                                _SetSingleKeyADO.txtDichC21H1 = item.VALUE;
                                break;
                            case "txtDichC22H1":
                                _SetSingleKeyADO.txtDichC22H1 = item.VALUE;
                                break;
                            case "txtDichC23H1":
                                _SetSingleKeyADO.txtDichC23H1 = item.VALUE;
                                break;
                            case "txtDichC24H1":
                                _SetSingleKeyADO.txtDichC24H1 = item.VALUE;
                                break;
                            case "txtDichC25H1":
                                _SetSingleKeyADO.txtDichC25H1 = item.VALUE;
                                break;
                            case "txtDichC26H1":
                                _SetSingleKeyADO.txtDichC26H1 = item.VALUE;
                                break;
                            case "txtDichC27H1":
                                _SetSingleKeyADO.txtDichC27H1 = item.VALUE;
                                break;
                            case "txtDichC28H1":
                                _SetSingleKeyADO.txtDichC28H1 = item.VALUE;
                                break;
                            case "txtDichC29H1":
                                _SetSingleKeyADO.txtDichC29H1 = item.VALUE;
                                break;
                            case "txtDichC30H1":
                                _SetSingleKeyADO.txtDichC30H1 = item.VALUE;
                                break;
                            case "txtDichC31H1":
                                _SetSingleKeyADO.txtDichC31H1 = item.VALUE;
                                break;
                            case "txtDichC32H1":
                                _SetSingleKeyADO.txtDichC32H1 = item.VALUE;
                                break;
                            case "txtDichC33H1":
                                _SetSingleKeyADO.txtDichC33H1 = item.VALUE;
                                break;
                            case "txtDichC34H1":
                                _SetSingleKeyADO.txtDichC34H1 = item.VALUE;
                                break;
                            case "txtDichC35H1":
                                _SetSingleKeyADO.txtDichC35H1 = item.VALUE;
                                break;
                            case "txtDichC36H1":
                                _SetSingleKeyADO.txtDichC36H1 = item.VALUE;
                                break;
                            case "txtDichC37H1":
                                _SetSingleKeyADO.txtDichC37H1 = item.VALUE;
                                break;
                            case "txtDichC38H1":
                                _SetSingleKeyADO.txtDichC38H1 = item.VALUE;
                                break;
                            case "txtDichC39H1":
                                _SetSingleKeyADO.txtDichC39H1 = item.VALUE;
                                break;

                            case "txtDichC1H2":
                                _SetSingleKeyADO.txtDichC1H2 = item.VALUE;
                                break;
                            case "txtDichC2H2":
                                _SetSingleKeyADO.txtDichC2H2 = item.VALUE;
                                break;
                            case "txtDichC3H2":
                                _SetSingleKeyADO.txtDichC3H2 = item.VALUE;
                                break;
                            case "txtDichC4H2":
                                _SetSingleKeyADO.txtDichC4H2 = item.VALUE;
                                break;
                            case "txtDichC5H2":
                                _SetSingleKeyADO.txtDichC5H2 = item.VALUE;
                                break;
                            case "txtDichC6H2":
                                _SetSingleKeyADO.txtDichC6H2 = item.VALUE;
                                break;
                            case "txtDichC7H2":
                                _SetSingleKeyADO.txtDichC7H2 = item.VALUE;
                                break;
                            case "txtDichC8H2":
                                _SetSingleKeyADO.txtDichC8H2 = item.VALUE;
                                break;
                            case "txtDichC9H2":
                                _SetSingleKeyADO.txtDichC9H2 = item.VALUE;
                                break;
                            case "txtDichC10H2":
                                _SetSingleKeyADO.txtDichC10H2 = item.VALUE;
                                break;
                            case "txtDichC11H2":
                                _SetSingleKeyADO.txtDichC11H2 = item.VALUE;
                                break;
                            case "txtDichC12H2":
                                _SetSingleKeyADO.txtDichC12H2 = item.VALUE;
                                break;
                            case "txtDichC13H2":
                                _SetSingleKeyADO.txtDichC13H2 = item.VALUE;
                                break;
                            case "txtDichC14H2":
                                _SetSingleKeyADO.txtDichC14H2 = item.VALUE;
                                break;
                            case "txtDichC15H2":
                                _SetSingleKeyADO.txtDichC15H2 = item.VALUE;
                                break;
                            case "txtDichC16H2":
                                _SetSingleKeyADO.txtDichC16H2 = item.VALUE;
                                break;
                            case "txtDichC17H2":
                                _SetSingleKeyADO.txtDichC17H2 = item.VALUE;
                                break;
                            case "txtDichC18H2":
                                _SetSingleKeyADO.txtDichC18H2 = item.VALUE;
                                break;
                            case "txtDichC19H2":
                                _SetSingleKeyADO.txtDichC19H2 = item.VALUE;
                                break;
                            case "txtDichC20H2":
                                _SetSingleKeyADO.txtDichC20H2 = item.VALUE;
                                break;
                            case "txtDichC21H2":
                                _SetSingleKeyADO.txtDichC21H2 = item.VALUE;
                                break;
                            case "txtDichC22H2":
                                _SetSingleKeyADO.txtDichC22H2 = item.VALUE;
                                break;
                            case "txtDichC23H2":
                                _SetSingleKeyADO.txtDichC23H2 = item.VALUE;
                                break;
                            case "txtDichC24H2":
                                _SetSingleKeyADO.txtDichC24H2 = item.VALUE;
                                break;
                            case "txtDichC25H2":
                                _SetSingleKeyADO.txtDichC25H2 = item.VALUE;
                                break;
                            case "txtDichC26H2":
                                _SetSingleKeyADO.txtDichC26H2 = item.VALUE;
                                break;
                            case "txtDichC27H2":
                                _SetSingleKeyADO.txtDichC27H2 = item.VALUE;
                                break;
                            case "txtDichC28H2":
                                _SetSingleKeyADO.txtDichC28H2 = item.VALUE;
                                break;
                            case "txtDichC29H2":
                                _SetSingleKeyADO.txtDichC29H2 = item.VALUE;
                                break;
                            case "txtDichC30H2":
                                _SetSingleKeyADO.txtDichC30H2 = item.VALUE;
                                break;
                            case "txtDichC31H2":
                                _SetSingleKeyADO.txtDichC31H2 = item.VALUE;
                                break;
                            case "txtDichC32H2":
                                _SetSingleKeyADO.txtDichC32H2 = item.VALUE;
                                break;
                            case "txtDichC33H2":
                                _SetSingleKeyADO.txtDichC33H2 = item.VALUE;
                                break;
                            case "txtDichC34H2":
                                _SetSingleKeyADO.txtDichC34H2 = item.VALUE;
                                break;
                            case "txtDichC35H2":
                                _SetSingleKeyADO.txtDichC35H2 = item.VALUE;
                                break;
                            case "txtDichC36H2":
                                _SetSingleKeyADO.txtDichC36H2 = item.VALUE;
                                break;
                            case "txtDichC37H2":
                                _SetSingleKeyADO.txtDichC37H2 = item.VALUE;
                                break;
                            case "txtDichC38H2":
                                _SetSingleKeyADO.txtDichC38H2 = item.VALUE;
                                break;
                            case "txtDichC39H2":
                                _SetSingleKeyADO.txtDichC39H2 = item.VALUE;
                                break;
                            case "txtDichC1H3":
                                _SetSingleKeyADO.txtDichC1H3 = item.VALUE;
                                break;
                            case "txtDichC2H3":
                                _SetSingleKeyADO.txtDichC2H3 = item.VALUE;
                                break;
                            case "txtDichC3H3":
                                _SetSingleKeyADO.txtDichC3H3 = item.VALUE;
                                break;
                            case "txtDichC4H3":
                                _SetSingleKeyADO.txtDichC4H3 = item.VALUE;
                                break;
                            case "txtDichC5H3":
                                _SetSingleKeyADO.txtDichC5H3 = item.VALUE;
                                break;
                            case "txtDichC6H3":
                                _SetSingleKeyADO.txtDichC6H3 = item.VALUE;
                                break;
                            case "txtDichC7H3":
                                _SetSingleKeyADO.txtDichC7H3 = item.VALUE;
                                break;
                            case "txtDichC8H3":
                                _SetSingleKeyADO.txtDichC8H3 = item.VALUE;
                                break;
                            case "txtDichC9H3":
                                _SetSingleKeyADO.txtDichC9H3 = item.VALUE;
                                break;
                            case "txtDichC10H3":
                                _SetSingleKeyADO.txtDichC10H3 = item.VALUE;
                                break;
                            case "txtDichC11H3":
                                _SetSingleKeyADO.txtDichC11H3 = item.VALUE;
                                break;
                            case "txtDichC12H3":
                                _SetSingleKeyADO.txtDichC12H3 = item.VALUE;
                                break;
                            case "txtDichC13H3":
                                _SetSingleKeyADO.txtDichC13H3 = item.VALUE;
                                break;
                            case "txtDichC14H3":
                                _SetSingleKeyADO.txtDichC14H3 = item.VALUE;
                                break;
                            case "txtDichC15H3":
                                _SetSingleKeyADO.txtDichC15H3 = item.VALUE;
                                break;
                            case "txtDichC16H3":
                                _SetSingleKeyADO.txtDichC16H3 = item.VALUE;
                                break;
                            case "txtDichC17H3":
                                _SetSingleKeyADO.txtDichC17H3 = item.VALUE;
                                break;
                            case "txtDichC18H3":
                                _SetSingleKeyADO.txtDichC18H3 = item.VALUE;
                                break;
                            case "txtDichC19H3":
                                _SetSingleKeyADO.txtDichC19H3 = item.VALUE;
                                break;
                            case "txtDichC20H3":
                                _SetSingleKeyADO.txtDichC20H3 = item.VALUE;
                                break;
                            case "txtDichC21H3":
                                _SetSingleKeyADO.txtDichC21H3 = item.VALUE;
                                break;
                            case "txtDichC22H3":
                                _SetSingleKeyADO.txtDichC22H3 = item.VALUE;
                                break;
                            case "txtDichC23H3":
                                _SetSingleKeyADO.txtDichC23H3 = item.VALUE;
                                break;
                            case "txtDichC24H3":
                                _SetSingleKeyADO.txtDichC24H3 = item.VALUE;
                                break;
                            case "txtDichC25H3":
                                _SetSingleKeyADO.txtDichC25H3 = item.VALUE;
                                break;
                            case "txtDichC26H3":
                                _SetSingleKeyADO.txtDichC26H3 = item.VALUE;
                                break;
                            case "txtDichC27H3":
                                _SetSingleKeyADO.txtDichC27H3 = item.VALUE;
                                break;
                            case "txtDichC28H3":
                                _SetSingleKeyADO.txtDichC28H3 = item.VALUE;
                                break;
                            case "txtDichC29H3":
                                _SetSingleKeyADO.txtDichC29H3 = item.VALUE;
                                break;
                            case "txtDichC30H3":
                                _SetSingleKeyADO.txtDichC30H3 = item.VALUE;
                                break;
                            case "txtDichC31H3":
                                _SetSingleKeyADO.txtDichC31H3 = item.VALUE;
                                break;
                            case "txtDichC32H3":
                                _SetSingleKeyADO.txtDichC32H3 = item.VALUE;
                                break;
                            case "txtDichC33H3":
                                _SetSingleKeyADO.txtDichC33H3 = item.VALUE;
                                break;
                            case "txtDichC34H3":
                                _SetSingleKeyADO.txtDichC34H3 = item.VALUE;
                                break;
                            case "txtDichC35H3":
                                _SetSingleKeyADO.txtDichC35H3 = item.VALUE;
                                break;
                            case "txtDichC36H3":
                                _SetSingleKeyADO.txtDichC36H3 = item.VALUE;
                                break;
                            case "txtDichC37H3":
                                _SetSingleKeyADO.txtDichC37H3 = item.VALUE;
                                break;
                            case "txtDichC38H3":
                                _SetSingleKeyADO.txtDichC38H3 = item.VALUE;
                                break;
                            case "txtDichC39H3":
                                _SetSingleKeyADO.txtDichC39H3 = item.VALUE;
                                break;


                            case "txtThuocKhac1":
                                _SetSingleKeyADO.txtThuocKhac1 = item.VALUE;
                                break;
                            case "txtThuocKhac2":
                                _SetSingleKeyADO.txtThuocKhac2 = item.VALUE;
                                break;
                            case "txtThuocKhac3":
                                _SetSingleKeyADO.txtThuocKhac3 = item.VALUE;
                                break;
                            case "txtThuocKhac4":
                                _SetSingleKeyADO.txtThuocKhac4 = item.VALUE;
                                break;
                            case "txtThuocKhac5":
                                _SetSingleKeyADO.txtThuocKhac5 = item.VALUE;
                                break;
                            case "txtThuocKhac6":
                                _SetSingleKeyADO.txtThuocKhac6 = item.VALUE;
                                break;
                            case "txtThuocKhac7":
                                _SetSingleKeyADO.txtThuocKhac7 = item.VALUE;
                                break;
								



                            case "txtThuocKhacC1H1":
                                _SetSingleKeyADO.txtThuocKhacC1H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H1":
                                _SetSingleKeyADO.txtThuocKhacC2H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H1":
                                _SetSingleKeyADO.txtThuocKhacC3H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H1":
                                _SetSingleKeyADO.txtThuocKhacC4H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H1":
                                _SetSingleKeyADO.txtThuocKhacC5H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H1":
                                _SetSingleKeyADO.txtThuocKhacC6H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H1":
                                _SetSingleKeyADO.txtThuocKhacC7H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H1":
                                _SetSingleKeyADO.txtThuocKhacC8H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H1":
                                _SetSingleKeyADO.txtThuocKhacC9H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H1":
                                _SetSingleKeyADO.txtThuocKhacC10H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H1":
                                _SetSingleKeyADO.txtThuocKhacC11H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H1":
                                _SetSingleKeyADO.txtThuocKhacC12H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H1":
                                _SetSingleKeyADO.txtThuocKhacC13H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H1":
                                _SetSingleKeyADO.txtThuocKhacC14H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H1":
                                _SetSingleKeyADO.txtThuocKhacC15H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H1":
                                _SetSingleKeyADO.txtThuocKhacC16H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H1":
                                _SetSingleKeyADO.txtThuocKhacC17H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H1":
                                _SetSingleKeyADO.txtThuocKhacC18H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H1":
                                _SetSingleKeyADO.txtThuocKhacC19H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H1":
                                _SetSingleKeyADO.txtThuocKhacC20H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H1":
                                _SetSingleKeyADO.txtThuocKhacC21H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H1":
                                _SetSingleKeyADO.txtThuocKhacC22H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H1":
                                _SetSingleKeyADO.txtThuocKhacC23H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H1":
                                _SetSingleKeyADO.txtThuocKhacC24H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H1":
                                _SetSingleKeyADO.txtThuocKhacC25H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H1":
                                _SetSingleKeyADO.txtThuocKhacC26H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H1":
                                _SetSingleKeyADO.txtThuocKhacC27H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H1":
                                _SetSingleKeyADO.txtThuocKhacC28H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H1":
                                _SetSingleKeyADO.txtThuocKhacC29H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H1":
                                _SetSingleKeyADO.txtThuocKhacC30H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H1":
                                _SetSingleKeyADO.txtThuocKhacC31H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H1":
                                _SetSingleKeyADO.txtThuocKhacC32H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H1":
                                _SetSingleKeyADO.txtThuocKhacC33H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H1":
                                _SetSingleKeyADO.txtThuocKhacC34H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H1":
                                _SetSingleKeyADO.txtThuocKhacC35H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H1":
                                _SetSingleKeyADO.txtThuocKhacC36H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H1":
                                _SetSingleKeyADO.txtThuocKhacC37H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H1":
                                _SetSingleKeyADO.txtThuocKhacC38H1 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H1":
                                _SetSingleKeyADO.txtThuocKhacC39H1 = item.VALUE;
                                break;

                            case "txtThuocKhacC1H2":
                                _SetSingleKeyADO.txtThuocKhacC1H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H2":
                                _SetSingleKeyADO.txtThuocKhacC2H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H2":
                                _SetSingleKeyADO.txtThuocKhacC3H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H2":
                                _SetSingleKeyADO.txtThuocKhacC4H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H2":
                                _SetSingleKeyADO.txtThuocKhacC5H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H2":
                                _SetSingleKeyADO.txtThuocKhacC6H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H2":
                                _SetSingleKeyADO.txtThuocKhacC7H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H2":
                                _SetSingleKeyADO.txtThuocKhacC8H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H2":
                                _SetSingleKeyADO.txtThuocKhacC9H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H2":
                                _SetSingleKeyADO.txtThuocKhacC10H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H2":
                                _SetSingleKeyADO.txtThuocKhacC11H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H2":
                                _SetSingleKeyADO.txtThuocKhacC12H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H2":
                                _SetSingleKeyADO.txtThuocKhacC13H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H2":
                                _SetSingleKeyADO.txtThuocKhacC14H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H2":
                                _SetSingleKeyADO.txtThuocKhacC15H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H2":
                                _SetSingleKeyADO.txtThuocKhacC16H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H2":
                                _SetSingleKeyADO.txtThuocKhacC17H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H2":
                                _SetSingleKeyADO.txtThuocKhacC18H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H2":
                                _SetSingleKeyADO.txtThuocKhacC19H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H2":
                                _SetSingleKeyADO.txtThuocKhacC20H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H2":
                                _SetSingleKeyADO.txtThuocKhacC21H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H2":
                                _SetSingleKeyADO.txtThuocKhacC22H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H2":
                                _SetSingleKeyADO.txtThuocKhacC23H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H2":
                                _SetSingleKeyADO.txtThuocKhacC24H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H2":
                                _SetSingleKeyADO.txtThuocKhacC25H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H2":
                                _SetSingleKeyADO.txtThuocKhacC26H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H2":
                                _SetSingleKeyADO.txtThuocKhacC27H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H2":
                                _SetSingleKeyADO.txtThuocKhacC28H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H2":
                                _SetSingleKeyADO.txtThuocKhacC29H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H2":
                                _SetSingleKeyADO.txtThuocKhacC30H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H2":
                                _SetSingleKeyADO.txtThuocKhacC31H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H2":
                                _SetSingleKeyADO.txtThuocKhacC32H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H2":
                                _SetSingleKeyADO.txtThuocKhacC33H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H2":
                                _SetSingleKeyADO.txtThuocKhacC34H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H2":
                                _SetSingleKeyADO.txtThuocKhacC35H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H2":
                                _SetSingleKeyADO.txtThuocKhacC36H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H2":
                                _SetSingleKeyADO.txtThuocKhacC37H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H2":
                                _SetSingleKeyADO.txtThuocKhacC38H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H2":
                                _SetSingleKeyADO.txtThuocKhacC39H2 = item.VALUE;
                                break;
                            case "txtThuocKhacC1H3":
                                _SetSingleKeyADO.txtThuocKhacC1H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H3":
                                _SetSingleKeyADO.txtThuocKhacC2H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H3":
                                _SetSingleKeyADO.txtThuocKhacC3H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H3":
                                _SetSingleKeyADO.txtThuocKhacC4H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H3":
                                _SetSingleKeyADO.txtThuocKhacC5H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H3":
                                _SetSingleKeyADO.txtThuocKhacC6H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H3":
                                _SetSingleKeyADO.txtThuocKhacC7H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H3":
                                _SetSingleKeyADO.txtThuocKhacC8H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H3":
                                _SetSingleKeyADO.txtThuocKhacC9H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H3":
                                _SetSingleKeyADO.txtThuocKhacC10H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H3":
                                _SetSingleKeyADO.txtThuocKhacC11H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H3":
                                _SetSingleKeyADO.txtThuocKhacC12H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H3":
                                _SetSingleKeyADO.txtThuocKhacC13H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H3":
                                _SetSingleKeyADO.txtThuocKhacC14H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H3":
                                _SetSingleKeyADO.txtThuocKhacC15H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H3":
                                _SetSingleKeyADO.txtThuocKhacC16H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H3":
                                _SetSingleKeyADO.txtThuocKhacC17H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H3":
                                _SetSingleKeyADO.txtThuocKhacC18H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H3":
                                _SetSingleKeyADO.txtThuocKhacC19H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H3":
                                _SetSingleKeyADO.txtThuocKhacC20H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H3":
                                _SetSingleKeyADO.txtThuocKhacC21H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H3":
                                _SetSingleKeyADO.txtThuocKhacC22H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H3":
                                _SetSingleKeyADO.txtThuocKhacC23H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H3":
                                _SetSingleKeyADO.txtThuocKhacC24H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H3":
                                _SetSingleKeyADO.txtThuocKhacC25H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H3":
                                _SetSingleKeyADO.txtThuocKhacC26H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H3":
                                _SetSingleKeyADO.txtThuocKhacC27H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H3":
                                _SetSingleKeyADO.txtThuocKhacC28H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H3":
                                _SetSingleKeyADO.txtThuocKhacC29H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H3":
                                _SetSingleKeyADO.txtThuocKhacC30H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H3":
                                _SetSingleKeyADO.txtThuocKhacC31H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H3":
                                _SetSingleKeyADO.txtThuocKhacC32H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H3":
                                _SetSingleKeyADO.txtThuocKhacC33H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H3":
                                _SetSingleKeyADO.txtThuocKhacC34H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H3":
                                _SetSingleKeyADO.txtThuocKhacC35H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H3":
                                _SetSingleKeyADO.txtThuocKhacC36H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H3":
                                _SetSingleKeyADO.txtThuocKhacC37H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H3":
                                _SetSingleKeyADO.txtThuocKhacC38H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H3":
                                _SetSingleKeyADO.txtThuocKhacC39H3 = item.VALUE;
                                break;
                            case "txtThuocKhacC1H4":
                                _SetSingleKeyADO.txtThuocKhacC1H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H4":
                                _SetSingleKeyADO.txtThuocKhacC2H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H4":
                                _SetSingleKeyADO.txtThuocKhacC3H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H4":
                                _SetSingleKeyADO.txtThuocKhacC4H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H4":
                                _SetSingleKeyADO.txtThuocKhacC5H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H4":
                                _SetSingleKeyADO.txtThuocKhacC6H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H4":
                                _SetSingleKeyADO.txtThuocKhacC7H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H4":
                                _SetSingleKeyADO.txtThuocKhacC8H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H4":
                                _SetSingleKeyADO.txtThuocKhacC9H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H4":
                                _SetSingleKeyADO.txtThuocKhacC10H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H4":
                                _SetSingleKeyADO.txtThuocKhacC11H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H4":
                                _SetSingleKeyADO.txtThuocKhacC12H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H4":
                                _SetSingleKeyADO.txtThuocKhacC13H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H4":
                                _SetSingleKeyADO.txtThuocKhacC14H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H4":
                                _SetSingleKeyADO.txtThuocKhacC15H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H4":
                                _SetSingleKeyADO.txtThuocKhacC16H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H4":
                                _SetSingleKeyADO.txtThuocKhacC17H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H4":
                                _SetSingleKeyADO.txtThuocKhacC18H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H4":
                                _SetSingleKeyADO.txtThuocKhacC19H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H4":
                                _SetSingleKeyADO.txtThuocKhacC20H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H4":
                                _SetSingleKeyADO.txtThuocKhacC21H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H4":
                                _SetSingleKeyADO.txtThuocKhacC22H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H4":
                                _SetSingleKeyADO.txtThuocKhacC23H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H4":
                                _SetSingleKeyADO.txtThuocKhacC24H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H4":
                                _SetSingleKeyADO.txtThuocKhacC25H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H4":
                                _SetSingleKeyADO.txtThuocKhacC26H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H4":
                                _SetSingleKeyADO.txtThuocKhacC27H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H4":
                                _SetSingleKeyADO.txtThuocKhacC28H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H4":
                                _SetSingleKeyADO.txtThuocKhacC29H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H4":
                                _SetSingleKeyADO.txtThuocKhacC30H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H4":
                                _SetSingleKeyADO.txtThuocKhacC31H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H4":
                                _SetSingleKeyADO.txtThuocKhacC32H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H4":
                                _SetSingleKeyADO.txtThuocKhacC33H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H4":
                                _SetSingleKeyADO.txtThuocKhacC34H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H4":
                                _SetSingleKeyADO.txtThuocKhacC35H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H4":
                                _SetSingleKeyADO.txtThuocKhacC36H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H4":
                                _SetSingleKeyADO.txtThuocKhacC37H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H4":
                                _SetSingleKeyADO.txtThuocKhacC38H4 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H4":
                                _SetSingleKeyADO.txtThuocKhacC39H4 = item.VALUE;
                                break;

                            case "txtThuocKhacC1H5":
                                _SetSingleKeyADO.txtThuocKhacC1H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H5":
                                _SetSingleKeyADO.txtThuocKhacC2H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H5":
                                _SetSingleKeyADO.txtThuocKhacC3H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H5":
                                _SetSingleKeyADO.txtThuocKhacC4H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H5":
                                _SetSingleKeyADO.txtThuocKhacC5H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H5":
                                _SetSingleKeyADO.txtThuocKhacC6H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H5":
                                _SetSingleKeyADO.txtThuocKhacC7H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H5":
                                _SetSingleKeyADO.txtThuocKhacC8H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H5":
                                _SetSingleKeyADO.txtThuocKhacC9H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H5":
                                _SetSingleKeyADO.txtThuocKhacC10H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H5":
                                _SetSingleKeyADO.txtThuocKhacC11H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H5":
                                _SetSingleKeyADO.txtThuocKhacC12H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H5":
                                _SetSingleKeyADO.txtThuocKhacC13H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H5":
                                _SetSingleKeyADO.txtThuocKhacC14H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H5":
                                _SetSingleKeyADO.txtThuocKhacC15H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H5":
                                _SetSingleKeyADO.txtThuocKhacC16H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H5":
                                _SetSingleKeyADO.txtThuocKhacC17H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H5":
                                _SetSingleKeyADO.txtThuocKhacC18H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H5":
                                _SetSingleKeyADO.txtThuocKhacC19H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H5":
                                _SetSingleKeyADO.txtThuocKhacC20H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H5":
                                _SetSingleKeyADO.txtThuocKhacC21H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H5":
                                _SetSingleKeyADO.txtThuocKhacC22H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H5":
                                _SetSingleKeyADO.txtThuocKhacC23H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H5":
                                _SetSingleKeyADO.txtThuocKhacC24H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H5":
                                _SetSingleKeyADO.txtThuocKhacC25H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H5":
                                _SetSingleKeyADO.txtThuocKhacC26H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H5":
                                _SetSingleKeyADO.txtThuocKhacC27H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H5":
                                _SetSingleKeyADO.txtThuocKhacC28H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H5":
                                _SetSingleKeyADO.txtThuocKhacC29H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H5":
                                _SetSingleKeyADO.txtThuocKhacC30H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H5":
                                _SetSingleKeyADO.txtThuocKhacC31H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H5":
                                _SetSingleKeyADO.txtThuocKhacC32H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H5":
                                _SetSingleKeyADO.txtThuocKhacC33H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H5":
                                _SetSingleKeyADO.txtThuocKhacC34H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H5":
                                _SetSingleKeyADO.txtThuocKhacC35H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H5":
                                _SetSingleKeyADO.txtThuocKhacC36H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H5":
                                _SetSingleKeyADO.txtThuocKhacC37H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H5":
                                _SetSingleKeyADO.txtThuocKhacC38H5 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H5":
                                _SetSingleKeyADO.txtThuocKhacC39H5 = item.VALUE;
                                break;

                            case "txtThuocKhacC1H6":
                                _SetSingleKeyADO.txtThuocKhacC1H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H6":
                                _SetSingleKeyADO.txtThuocKhacC2H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H6":
                                _SetSingleKeyADO.txtThuocKhacC3H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H6":
                                _SetSingleKeyADO.txtThuocKhacC4H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H6":
                                _SetSingleKeyADO.txtThuocKhacC5H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H6":
                                _SetSingleKeyADO.txtThuocKhacC6H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H6":
                                _SetSingleKeyADO.txtThuocKhacC7H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H6":
                                _SetSingleKeyADO.txtThuocKhacC8H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H6":
                                _SetSingleKeyADO.txtThuocKhacC9H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H6":
                                _SetSingleKeyADO.txtThuocKhacC10H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H6":
                                _SetSingleKeyADO.txtThuocKhacC11H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H6":
                                _SetSingleKeyADO.txtThuocKhacC12H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H6":
                                _SetSingleKeyADO.txtThuocKhacC13H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H6":
                                _SetSingleKeyADO.txtThuocKhacC14H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H6":
                                _SetSingleKeyADO.txtThuocKhacC15H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H6":
                                _SetSingleKeyADO.txtThuocKhacC16H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H6":
                                _SetSingleKeyADO.txtThuocKhacC17H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H6":
                                _SetSingleKeyADO.txtThuocKhacC18H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H6":
                                _SetSingleKeyADO.txtThuocKhacC19H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H6":
                                _SetSingleKeyADO.txtThuocKhacC20H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H6":
                                _SetSingleKeyADO.txtThuocKhacC21H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H6":
                                _SetSingleKeyADO.txtThuocKhacC22H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H6":
                                _SetSingleKeyADO.txtThuocKhacC23H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H6":
                                _SetSingleKeyADO.txtThuocKhacC24H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H6":
                                _SetSingleKeyADO.txtThuocKhacC25H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H6":
                                _SetSingleKeyADO.txtThuocKhacC26H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H6":
                                _SetSingleKeyADO.txtThuocKhacC27H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H6":
                                _SetSingleKeyADO.txtThuocKhacC28H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H6":
                                _SetSingleKeyADO.txtThuocKhacC29H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H6":
                                _SetSingleKeyADO.txtThuocKhacC30H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H6":
                                _SetSingleKeyADO.txtThuocKhacC31H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H6":
                                _SetSingleKeyADO.txtThuocKhacC32H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H6":
                                _SetSingleKeyADO.txtThuocKhacC33H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H6":
                                _SetSingleKeyADO.txtThuocKhacC34H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H6":
                                _SetSingleKeyADO.txtThuocKhacC35H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H6":
                                _SetSingleKeyADO.txtThuocKhacC36H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H6":
                                _SetSingleKeyADO.txtThuocKhacC37H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H6":
                                _SetSingleKeyADO.txtThuocKhacC38H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H6":
                                _SetSingleKeyADO.txtThuocKhacC39H6 = item.VALUE;
                                break;
                            case "txtThuocKhacC1H7":
                                _SetSingleKeyADO.txtThuocKhacC1H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC2H7":
                                _SetSingleKeyADO.txtThuocKhacC2H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC3H7":
                                _SetSingleKeyADO.txtThuocKhacC3H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC4H7":
                                _SetSingleKeyADO.txtThuocKhacC4H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC5H7":
                                _SetSingleKeyADO.txtThuocKhacC5H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC6H7":
                                _SetSingleKeyADO.txtThuocKhacC6H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC7H7":
                                _SetSingleKeyADO.txtThuocKhacC7H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC8H7":
                                _SetSingleKeyADO.txtThuocKhacC8H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC9H7":
                                _SetSingleKeyADO.txtThuocKhacC9H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC10H7":
                                _SetSingleKeyADO.txtThuocKhacC10H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC11H7":
                                _SetSingleKeyADO.txtThuocKhacC11H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC12H7":
                                _SetSingleKeyADO.txtThuocKhacC12H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC13H7":
                                _SetSingleKeyADO.txtThuocKhacC13H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC14H7":
                                _SetSingleKeyADO.txtThuocKhacC14H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC15H7":
                                _SetSingleKeyADO.txtThuocKhacC15H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC16H7":
                                _SetSingleKeyADO.txtThuocKhacC16H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC17H7":
                                _SetSingleKeyADO.txtThuocKhacC17H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC18H7":
                                _SetSingleKeyADO.txtThuocKhacC18H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC19H7":
                                _SetSingleKeyADO.txtThuocKhacC19H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC20H7":
                                _SetSingleKeyADO.txtThuocKhacC20H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC21H7":
                                _SetSingleKeyADO.txtThuocKhacC21H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC22H7":
                                _SetSingleKeyADO.txtThuocKhacC22H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC23H7":
                                _SetSingleKeyADO.txtThuocKhacC23H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC24H7":
                                _SetSingleKeyADO.txtThuocKhacC24H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC25H7":
                                _SetSingleKeyADO.txtThuocKhacC25H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC26H7":
                                _SetSingleKeyADO.txtThuocKhacC26H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC27H7":
                                _SetSingleKeyADO.txtThuocKhacC27H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC28H7":
                                _SetSingleKeyADO.txtThuocKhacC28H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC29H7":
                                _SetSingleKeyADO.txtThuocKhacC29H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC30H7":
                                _SetSingleKeyADO.txtThuocKhacC30H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC31H7":
                                _SetSingleKeyADO.txtThuocKhacC31H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC32H7":
                                _SetSingleKeyADO.txtThuocKhacC32H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC33H7":
                                _SetSingleKeyADO.txtThuocKhacC33H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC34H7":
                                _SetSingleKeyADO.txtThuocKhacC34H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC35H7":
                                _SetSingleKeyADO.txtThuocKhacC35H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC36H7":
                                _SetSingleKeyADO.txtThuocKhacC36H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC37H7":
                                _SetSingleKeyADO.txtThuocKhacC37H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC38H7":
                                _SetSingleKeyADO.txtThuocKhacC38H7 = item.VALUE;
                                break;
                            case "txtThuocKhacC39H7":
                                _SetSingleKeyADO.txtThuocKhacC39H7 = item.VALUE;
                                break;
                            case "txtTruocGayMeH1":
                                _SetSingleKeyADO.txtTruocGayMeH1 = item.VALUE;
                                break;
                            case "txtTruocGayMeH2":
                                _SetSingleKeyADO.txtTruocGayMeH2 = item.VALUE;
                                break;
                            case "txtTruocGayMeH3":
                                _SetSingleKeyADO.txtTruocGayMeH3 = item.VALUE;
                                break;
                            case "txtKhoiMeH4":
                                _SetSingleKeyADO.txtKhoiMeH4 = item.VALUE;
                                break;
                            case "txtKhoiMeH5":
                                _SetSingleKeyADO.txtKhoiMeH5 = item.VALUE;
                                break;
                            case "txtKhoiMeH6":
                                _SetSingleKeyADO.txtKhoiMeH6 = item.VALUE;
                                break;
                            case "txtKhoiMeH7":
                                _SetSingleKeyADO.txtKhoiMeH7 = item.VALUE;
                                break;
                            case "txtKhoiMeH8":
                                _SetSingleKeyADO.txtKhoiMeH8 = item.VALUE;
                                break;
                            case "txtDuyTriMeH9":
                                _SetSingleKeyADO.txtDuyTriMeH9 = item.VALUE;
                                break;
                            case "txtDuyTriMeH10":
                                _SetSingleKeyADO.txtDuyTriMeH10 = item.VALUE;
                                break;
                            case "txtDuyTriMeH11":
                                _SetSingleKeyADO.txtDuyTriMeH11 = item.VALUE;
                                break;
                            case "txtDuyTriMeH12":
                                _SetSingleKeyADO.txtDuyTriMeH12 = item.VALUE;
                                break;
                            case "txtDuyTriMeH13":
                                _SetSingleKeyADO.txtDuyTriMeH13 = item.VALUE;
                                break;
                            case "txtDuyTriMeH14":
                                _SetSingleKeyADO.txtDuyTriMeH14 = item.VALUE;
                                break;
                            case "txtDuyTriMeH15":
                                _SetSingleKeyADO.txtDuyTriMeH15 = item.VALUE;
                                break;
                            case "txtDuyTriMeH16":
                                _SetSingleKeyADO.txtDuyTriMeH16 = item.VALUE;
                                break;
                            case "txtKetThucMeH17":
                                _SetSingleKeyADO.txtKetThucMeH17 = item.VALUE;
                                break;
                            case "txtKetThucMeH18":
                                _SetSingleKeyADO.txtKetThucMeH18 = item.VALUE;
                                break;
                            case "txtKetThucMeH19":
                                _SetSingleKeyADO.txtKetThucMeH19 = item.VALUE;
                                break;
                            case "txtKetThucMeH20":
                                _SetSingleKeyADO.txtKetThucMeH20 = item.VALUE;
                                break;
                            case "txtKetThucMeH21":
                                _SetSingleKeyADO.txtKetThucMeH21 = item.VALUE;
                                break;
                            case "txtKetThucMeH22":
                                _SetSingleKeyADO.txtKetThucMeH22 = item.VALUE;
                                break;
                            case "txtKetThucMeH23":
                                _SetSingleKeyADO.txtKetThucMeH23 = item.VALUE;
                                break;
                            case "txtKetThucMeH24":
                                _SetSingleKeyADO.txtKetThucMeH24 = item.VALUE;
                                break;
                            case "txtGhiChepSauPhauThuat":
                                _SetSingleKeyADO.txtGhiChepSauPhauThuat = item.VALUE;
                                break;
                                case "txtMach":
                                _SetSingleKeyADO.txtMach = item.VALUE;
                                break;
                                case "txtDiemVAS":
                                _SetSingleKeyADO.txtDiemVAS = item.VALUE;
                                break;
                                case "txtThongTinThem1":
                                _SetSingleKeyADO.txtThongTinThem1 = item.VALUE;
                                break;
                                case "txtThongTinThem2":
                                _SetSingleKeyADO.txtThongTinThem2 = item.VALUE;
                                break;
                                case "txtNon":
                                _SetSingleKeyADO.txtNon = item.VALUE;
                                break;
                                case "txtHA":
                                _SetSingleKeyADO.txtHA = item.VALUE;
                                break;
                                case "txtSPO2":
                                _SetSingleKeyADO.txtSPO2 = item.VALUE;
                                break;
                                case "txtRetRun":
                                _SetSingleKeyADO.txtRetRun = item.VALUE;
                                break;
                                case "txtĐDXacNhan1":
                                _SetSingleKeyADO.txtĐDXacNhan1 = item.VALUE;
                                break;
                                case "txtThongTinThem3":
                                _SetSingleKeyADO.txtThongTinThem3 = item.VALUE;
                                break;
                                case "txtNhipTho":
                                _SetSingleKeyADO.txtNhipTho = item.VALUE;
                                break;
                                case "txtVetMo":
                                _SetSingleKeyADO.txtVetMo = item.VALUE;
                                break;
                                case "txtĐDXacNhan2":
                                _SetSingleKeyADO.txtĐDXacNhan2 = item.VALUE;
                                break;
                                case "txtThongTinThem4":
                                _SetSingleKeyADO.txtThongTinThem4 = item.VALUE;
                                break;
                                case "txtYThucBN":
                                _SetSingleKeyADO.txtYThucBN = item.VALUE;
                                break;
                                case "txtĐDXacNhan3":
                                _SetSingleKeyADO.txtĐDXacNhan3 = item.VALUE;
                                break;
                                case "txtThongTinThem5":
                                _SetSingleKeyADO.txtThongTinThem5 = item.VALUE;
                                break;
                                 case "txtGioChuyenBNVeKhoaDieuTri":
                                _SetSingleKeyADO.txtGioChuyenBNVeKhoaDieuTri = item.VALUE;
                                break;
                                 case "txtDeNghi1":
                                _SetSingleKeyADO.txtDeNghi1 = item.VALUE;
                                break;
                                 case "txtDeNghi2":
                                _SetSingleKeyADO.txtDeNghi2 = item.VALUE;
                                break;
                                 case "txtGacPhat1":
                                _SetSingleKeyADO.txtGacPhat1 = item.VALUE;
                                break;
                                 case "txtGacPhat2":
                                _SetSingleKeyADO.txtGacPhat2 = item.VALUE;
                                break;
                                 case "txtGacPhat3":
                                _SetSingleKeyADO.txtGacPhat3 = item.VALUE;
                                break;
                                 case "txtGacPhat4":
                                _SetSingleKeyADO.txtGacPhat4 = item.VALUE;
                                break;
                                 case "txtGacPhat5":
                                _SetSingleKeyADO.txtGacPhat5 = item.VALUE;
                                break;
                                 case "txtGacPhat6":

                                _SetSingleKeyADO.txtGacPhat6 = item.VALUE;
                                break;
                                 case "txtGacPhat7":

                                _SetSingleKeyADO.txtGacPhat7 = item.VALUE;
                                break;
                                 case "txtGacThu1":
                                _SetSingleKeyADO.txtGacThu1 = item.VALUE;
                                break;
                                 case "txtGacThu2":
                                _SetSingleKeyADO.txtGacThu2 = item.VALUE;
                                break;
                                 case "txtGacThu3":
                                _SetSingleKeyADO.txtGacThu3 = item.VALUE;
                                break;
                                 case "txtGacThu4":
                                _SetSingleKeyADO.txtGacThu4 = item.VALUE;
                                break;
                                 case "txtGacThu5":
                                _SetSingleKeyADO.txtGacThu5 = item.VALUE;
                                break;
                                 case "txtGacThu6":
                                _SetSingleKeyADO.txtGacThu6 = item.VALUE;
                                break;
                                 case "txtGacThu7":
                                _SetSingleKeyADO.txtGacThu7 = item.VALUE;
                                break;
                                 case "txtGhiChu1":
                                _SetSingleKeyADO.txtGhiChu1 = item.VALUE;
                                break;
                                 case "txtGhiChu2":
                                _SetSingleKeyADO.txtGhiChu2 = item.VALUE;
                                break;
                                 case "txtGhiChu3":
                                _SetSingleKeyADO.txtGhiChu3 = item.VALUE;
                                break;
                                 case "txtGhiChu4":
                                _SetSingleKeyADO.txtGhiChu4 = item.VALUE;
                                break;
                                 case "txtGhiChu5":
                                _SetSingleKeyADO.txtGhiChu5 = item.VALUE;
                                break;
                                 case "txtGhiChu6":
                                _SetSingleKeyADO.txtGhiChu6 = item.VALUE;
                                break;
                                 case "txtGhiChu7":
                                _SetSingleKeyADO.txtGhiChu7 = item.VALUE;
                                break;
                                 case "txtPhauThuatVien":
                                _SetSingleKeyADO.txtPhauThuatVien = item.VALUE;
                                break;
                                 case "txtHuuTrung":
                                _SetSingleKeyADO.txtHuuTrung = item.VALUE;
                                break;
                                 case "txtVoTrung":
                                _SetSingleKeyADO.txtVoTrung = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo1":
                                _SetSingleKeyADO.txtThuocDungTrongMo1 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo2":
                                _SetSingleKeyADO.txtThuocDungTrongMo2 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo3":
                                _SetSingleKeyADO.txtThuocDungTrongMo3 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo4":
                                _SetSingleKeyADO.txtThuocDungTrongMo4 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo5":
                                _SetSingleKeyADO.txtThuocDungTrongMo5 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo6":
                                _SetSingleKeyADO.txtThuocDungTrongMo6 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo7":
                                _SetSingleKeyADO.txtThuocDungTrongMo7 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo8":
                                _SetSingleKeyADO.txtThuocDungTrongMo8 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo9":
                                _SetSingleKeyADO.txtThuocDungTrongMo9 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo10":
                                _SetSingleKeyADO.txtThuocDungTrongMo10 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo11":
                                _SetSingleKeyADO.txtThuocDungTrongMo11 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo12":
                                _SetSingleKeyADO.txtThuocDungTrongMo12 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo13":
                                _SetSingleKeyADO.txtThuocDungTrongMo13 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo14":
                                _SetSingleKeyADO.txtThuocDungTrongMo14 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo15":
                                _SetSingleKeyADO.txtThuocDungTrongMo15 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo16":
                                _SetSingleKeyADO.txtThuocDungTrongMo16 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo17":
                                _SetSingleKeyADO.txtThuocDungTrongMo17 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo18":
                                _SetSingleKeyADO.txtThuocDungTrongMo18 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo19":
                                _SetSingleKeyADO.txtThuocDungTrongMo19 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo20":
                                _SetSingleKeyADO.txtThuocDungTrongMo20 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo21":
                                _SetSingleKeyADO.txtThuocDungTrongMo21 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo22":
                                _SetSingleKeyADO.txtThuocDungTrongMo22 = item.VALUE;
                                break;
                                 case "txtThuocDungTrongMo23":
                                _SetSingleKeyADO.txtThuocDungTrongMo23 = item.VALUE;
                                break;
                                 case "txtThoiGianc1":
                                _SetSingleKeyADO.txtThoiGianc1 = item.VALUE;
                                break;
                                 case "txtThoiGianc2":
                                _SetSingleKeyADO.txtThoiGianc2 = item.VALUE;
                                break;
                                 case "txtThoiGianc3":
                                _SetSingleKeyADO.txtThoiGianc3 = item.VALUE;
                                break;
                                 case "txtThoiGianc4":
                                _SetSingleKeyADO.txtThoiGianc4 = item.VALUE;
                                break;
                                 case "txtThoiGianc5":
                                _SetSingleKeyADO.txtThoiGianc5 = item.VALUE;
                                break;
                                 case "txtThoiGianc6":
                                _SetSingleKeyADO.txtThoiGianc6 = item.VALUE;
                                break;
                                 case "txtThoiGianc7":
                                _SetSingleKeyADO.txtThoiGianc7 = item.VALUE;
                                break;
                                 case "txtThoiGianc8":
                                _SetSingleKeyADO.txtThoiGianc8 = item.VALUE;
                                break;
                                 case "txtThoiGianc9":
                                _SetSingleKeyADO.txtThoiGianc9 = item.VALUE;
                                break;
                                 case "txtThoiGianc10":
                                _SetSingleKeyADO.txtThoiGianc10 = item.VALUE;
                                break;
                                 case "txtThoiGianc11":
                                _SetSingleKeyADO.txtThoiGianc11 = item.VALUE;
                                break;
                                 case "txtThoiGianc12":
                                _SetSingleKeyADO.txtThoiGianc12 = item.VALUE;
                                break;
                                 case "txtThoiGianc13":
                                _SetSingleKeyADO.txtThoiGianc13 = item.VALUE;
                                break;
                                 case "txtThoiGianc14":
                                _SetSingleKeyADO.txtThoiGianc14 = item.VALUE;
                                break;
                                 case "txtThoiGianc15":
                                _SetSingleKeyADO.txtThoiGianc15 = item.VALUE;
                                break;
                                 case "txtThoiGianc16":
                                _SetSingleKeyADO.txtThoiGianc16 = item.VALUE;
                                break;
                                 case "txtThoiGianc17":
                                _SetSingleKeyADO.txtThoiGianc17 = item.VALUE;
                                break;
                                 case "txtThoiGianc18":
                                _SetSingleKeyADO.txtThoiGianc18 = item.VALUE;
                                break;
                                 case "txtThoiGianc19":
                                _SetSingleKeyADO.txtThoiGianc19 = item.VALUE;
                                break;
                                 case "txtThoiGianc20":
                                _SetSingleKeyADO.txtThoiGianc20 = item.VALUE;
                                break;
                                 case "txtThoiGianc21":
                                _SetSingleKeyADO.txtThoiGianc21 = item.VALUE;
                                break;
                                 case "txtThoiGianc22":
                                _SetSingleKeyADO.txtThoiGianc22 = item.VALUE;
                                break;
                                 case "txtThoiGianc23":
                                _SetSingleKeyADO.txtThoiGianc23 = item.VALUE;
                                break;
                                 case "txtThoiGianc24":
                                _SetSingleKeyADO.txtThoiGianc24 = item.VALUE;
                                break;
                                 case "txtThoiGianc25":
                                _SetSingleKeyADO.txtThoiGianc25 = item.VALUE;
                                break;
                                 case "txtThoiGianc26":
                                _SetSingleKeyADO.txtThoiGianc26 = item.VALUE;
                                break;
                                 case "txtThoiGianc27":
                                _SetSingleKeyADO.txtThoiGianc27 = item.VALUE;
                                break;
                                 case "txtThoiGianc28":
                                _SetSingleKeyADO.txtThoiGianc28 = item.VALUE;
                                break;
                                 case "txtThoiGianc29":
                                _SetSingleKeyADO.txtThoiGianc29 = item.VALUE;
                                break;
                                 case "txtThoiGianc30":
                                _SetSingleKeyADO.txtThoiGianc30 = item.VALUE;
                                break;
                                 case "txtThoiGianc31":
                                _SetSingleKeyADO.txtThoiGianc31 = item.VALUE;
                                break;
                                 case "txtThoiGianc32":
                                _SetSingleKeyADO.txtThoiGianc32 = item.VALUE;
                                break;
                                 case "txtThoiGianc33":
                                _SetSingleKeyADO.txtThoiGianc33 = item.VALUE;
                                break;
                                 case "txtThoiGianc34":
                                _SetSingleKeyADO.txtThoiGianc34 = item.VALUE;
                                break;
                                 case "txtThoiGianc35":
                                _SetSingleKeyADO.txtThoiGianc35 = item.VALUE;
                                break;
                                 case "txtThoiGianc36":
                                _SetSingleKeyADO.txtThoiGianc36 = item.VALUE;
                                break;
                                 case "txtThoiGianc37":
                                _SetSingleKeyADO.txtThoiGianc37 = item.VALUE;
                                break;
                                 case "txtThoiGianc38":
                                _SetSingleKeyADO.txtThoiGianc38 = item.VALUE;
                                break;
                                 case "txtThoiGianc39":
                                _SetSingleKeyADO.txtThoiGianc39 = item.VALUE;
                                break;
                        }
                    }
                }
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.ServiceReq, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.SereServPttt, false);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.SereServExt, false);
                AddObjectKeyIntoListkey<SetSingleKeyADO>(_SetSingleKeyADO,false);
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
      

        internal void DataInputProcess()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class GiaiDoanMoADO
        {
            public string GDM_1 { get; set; }
            public string GDM_2 { get; set; }
            public string GDM_3 { get; set; }
            public string GDM_4 { get; set; }
            public string GDM_5 { get; set; }
            public string GDM_6 { get; set; }
            public string GDM_7 { get; set; }
            public string GDM_8 { get; set; }
            public string GDM_9 { get; set; }
            public string GDM_10 { get; set; }
            public string GDM_11 { get; set; }
            public string GDM_12 { get; set; }
            public string GDM_13 { get; set; }
            public string GDM_14 { get; set; }
            public string GDM_15 { get; set; }
            public string GDM_16 { get; set; }
            public string GDM_17 { get; set; }
            public string GDM_18 { get; set; }
            public string GDM_19 { get; set; }
            public string GDM_20 { get; set; }
            public string GDM_21 { get; set; }
            public string GDM_22 { get; set; }
            public string GDM_23 { get; set; }
            public string GDM_24 { get; set; }
            public string GDM_25 { get; set; }
            public string GDM_26 { get; set; }
            public string GDM_27 { get; set; }
            public string GDM_28 { get; set; }
            public string GDM_29 { get; set; }
            public string GDM_30 { get; set; }
            public string GDM_31 { get; set; }
            public string GDM_32 { get; set; }
            public string GDM_33 { get; set; }
            public string GDM_34 { get; set; }
            public string GDM_35 { get; set; }
            public string GDM_36 { get; set; }
            public string GDM_37 { get; set; }
            public string GDM_38 { get; set; }

        }

        public class SetSingleKeyADO
        {
            public string txtChuanDoanTruoc { get; set; }
            public string txtChuanDoanSau { get; set; }
            public string txtDuKienMo { get; set; }
            public string txtDaMo { get; set; }
            public string txtThuocChuanBi { get; set; }
            public string txtPhuongPhapGayMe { get; set; }
            public string txtKhoiMe { get; set; }
            public string txtNhomMo { get; set; }
            public string txtNhomGayMeHoiSuc { get; set; }
            public string txtTuTheTrenBanMo { get; set; }
            public string txtHeThong { get; set; }
            public string txtNgay { get; set; }
            public string txtMay { get; set; }





            public string txtThuocGayMe1 { get; set; }
            public string txtThuocGayMe2 { get; set; }
            public string txtThuocGayMe3 { get; set; }
            public string txtThuocGayMe4 { get; set; }
            public string txtThuocGayMe5 { get; set; }
            public string txtThuocGayMe6 { get; set; }
            public string txtThuocGMHSC1H1 { get; set; }
            public string txtThuocGMHSC2H1 { get; set; }
            public string txtThuocGMHSC3H1 { get; set; }
            public string txtThuocGMHSC4H1 { get; set; }
            public string txtThuocGMHSC5H1 { get; set; }
            public string txtThuocGMHSC6H1 { get; set; }
            public string txtThuocGMHSC7H1 { get; set; }
            public string txtThuocGMHSC8H1 { get; set; }
            public string txtThuocGMHSC9H1 { get; set; }
            public string txtThuocGMHSC10H1 { get; set; }
            public string txtThuocGMHSC11H1 { get; set; }
            public string txtThuocGMHSC12H1 { get; set; }
            public string txtThuocGMHSC13H1 { get; set; }
            public string txtThuocGMHSC14H1 { get; set; }
            public string txtThuocGMHSC15H1 { get; set; }
            public string txtThuocGMHSC16H1 { get; set; }
            public string txtThuocGMHSC17H1 { get; set; }
            public string txtThuocGMHSC18H1 { get; set; }
            public string txtThuocGMHSC19H1 { get; set; }
            public string txtThuocGMHSC20H1 { get; set; }
            public string txtThuocGMHSC21H1 { get; set; }
            public string txtThuocGMHSC22H1 { get; set; }
            public string txtThuocGMHSC23H1 { get; set; }
            public string txtThuocGMHSC24H1 { get; set; }
            public string txtThuocGMHSC25H1 { get; set; }
            public string txtThuocGMHSC26H1 { get; set; }
            public string txtThuocGMHSC27H1 { get; set; }
            public string txtThuocGMHSC28H1 { get; set; }
            public string txtThuocGMHSC29H1 { get; set; }
            public string txtThuocGMHSC30H1 { get; set; }
            public string txtThuocGMHSC31H1 { get; set; }
            public string txtThuocGMHSC32H1 { get; set; }
            public string txtThuocGMHSC33H1 { get; set; }
            public string txtThuocGMHSC34H1 { get; set; }
            public string txtThuocGMHSC35H1 { get; set; }
            public string txtThuocGMHSC36H1 { get; set; }
            public string txtThuocGMHSC37H1 { get; set; }
            public string txtThuocGMHSC38H1 { get; set; }
            public string txtThuocGMHSC39H1 { get; set; }

            public string txtThuocGMHSC1H2 { get; set; }
            public string txtThuocGMHSC2H2 { get; set; }
            public string txtThuocGMHSC3H2 { get; set; }
            public string txtThuocGMHSC4H2 { get; set; }
            public string txtThuocGMHSC5H2 { get; set; }
            public string txtThuocGMHSC6H2 { get; set; }
            public string txtThuocGMHSC7H2 { get; set; }
            public string txtThuocGMHSC8H2 { get; set; }
            public string txtThuocGMHSC9H2 { get; set; }
            public string txtThuocGMHSC10H2 { get; set; }
            public string txtThuocGMHSC11H2 { get; set; }
            public string txtThuocGMHSC12H2 { get; set; }
            public string txtThuocGMHSC13H2 { get; set; }
            public string txtThuocGMHSC14H2 { get; set; }
            public string txtThuocGMHSC15H2 { get; set; }
            public string txtThuocGMHSC16H2 { get; set; }
            public string txtThuocGMHSC17H2 { get; set; }
            public string txtThuocGMHSC18H2 { get; set; }
            public string txtThuocGMHSC19H2 { get; set; }
            public string txtThuocGMHSC20H2 { get; set; }
            public string txtThuocGMHSC21H2 { get; set; }
            public string txtThuocGMHSC22H2 { get; set; }
            public string txtThuocGMHSC23H2 { get; set; }
            public string txtThuocGMHSC24H2 { get; set; }
            public string txtThuocGMHSC25H2 { get; set; }
            public string txtThuocGMHSC26H2 { get; set; }
            public string txtThuocGMHSC27H2 { get; set; }
            public string txtThuocGMHSC28H2 { get; set; }
            public string txtThuocGMHSC29H2 { get; set; }
            public string txtThuocGMHSC30H2 { get; set; }
            public string txtThuocGMHSC31H2 { get; set; }
            public string txtThuocGMHSC32H2 { get; set; }
            public string txtThuocGMHSC33H2 { get; set; }
            public string txtThuocGMHSC34H2 { get; set; }
            public string txtThuocGMHSC35H2 { get; set; }
            public string txtThuocGMHSC36H2 { get; set; }
            public string txtThuocGMHSC37H2 { get; set; }
            public string txtThuocGMHSC38H2 { get; set; }
            public string txtThuocGMHSC39H2 { get; set; }

            public string txtThuocGMHSC1H3 { get; set; }
            public string txtThuocGMHSC2H3 { get; set; }
            public string txtThuocGMHSC3H3 { get; set; }
            public string txtThuocGMHSC4H3 { get; set; }
            public string txtThuocGMHSC5H3 { get; set; }
            public string txtThuocGMHSC6H3 { get; set; }
            public string txtThuocGMHSC7H3 { get; set; }
            public string txtThuocGMHSC8H3 { get; set; }
            public string txtThuocGMHSC9H3 { get; set; }
            public string txtThuocGMHSC10H3 { get; set; }
            public string txtThuocGMHSC11H3 { get; set; }
            public string txtThuocGMHSC12H3 { get; set; }
            public string txtThuocGMHSC13H3 { get; set; }
            public string txtThuocGMHSC14H3 { get; set; }
            public string txtThuocGMHSC15H3 { get; set; }
            public string txtThuocGMHSC16H3 { get; set; }
            public string txtThuocGMHSC17H3 { get; set; }
            public string txtThuocGMHSC18H3 { get; set; }
            public string txtThuocGMHSC19H3 { get; set; }
            public string txtThuocGMHSC20H3 { get; set; }
            public string txtThuocGMHSC21H3 { get; set; }
            public string txtThuocGMHSC22H3 { get; set; }
            public string txtThuocGMHSC23H3 { get; set; }
            public string txtThuocGMHSC24H3 { get; set; }
            public string txtThuocGMHSC25H3 { get; set; }
            public string txtThuocGMHSC26H3 { get; set; }
            public string txtThuocGMHSC27H3 { get; set; }
            public string txtThuocGMHSC28H3 { get; set; }
            public string txtThuocGMHSC29H3 { get; set; }
            public string txtThuocGMHSC30H3 { get; set; }
            public string txtThuocGMHSC31H3 { get; set; }
            public string txtThuocGMHSC32H3 { get; set; }
            public string txtThuocGMHSC33H3 { get; set; }
            public string txtThuocGMHSC34H3 { get; set; }
            public string txtThuocGMHSC35H3 { get; set; }
            public string txtThuocGMHSC36H3 { get; set; }
            public string txtThuocGMHSC37H3 { get; set; }
            public string txtThuocGMHSC38H3 { get; set; }
            public string txtThuocGMHSC39H3 { get; set; }

            public string txtThuocGMHSC1H4 { get; set; }
            public string txtThuocGMHSC2H4 { get; set; }
            public string txtThuocGMHSC3H4 { get; set; }
            public string txtThuocGMHSC4H4 { get; set; }
            public string txtThuocGMHSC5H4 { get; set; }
            public string txtThuocGMHSC6H4 { get; set; }
            public string txtThuocGMHSC7H4 { get; set; }
            public string txtThuocGMHSC8H4 { get; set; }
            public string txtThuocGMHSC9H4 { get; set; }
            public string txtThuocGMHSC10H4 { get; set; }
            public string txtThuocGMHSC11H4 { get; set; }
            public string txtThuocGMHSC12H4 { get; set; }
            public string txtThuocGMHSC13H4 { get; set; }
            public string txtThuocGMHSC14H4 { get; set; }
            public string txtThuocGMHSC15H4 { get; set; }
            public string txtThuocGMHSC16H4 { get; set; }
            public string txtThuocGMHSC17H4 { get; set; }
            public string txtThuocGMHSC18H4 { get; set; }
            public string txtThuocGMHSC19H4 { get; set; }
            public string txtThuocGMHSC20H4 { get; set; }
            public string txtThuocGMHSC21H4 { get; set; }
            public string txtThuocGMHSC22H4 { get; set; }
            public string txtThuocGMHSC23H4 { get; set; }
            public string txtThuocGMHSC24H4 { get; set; }
            public string txtThuocGMHSC25H4 { get; set; }
            public string txtThuocGMHSC26H4 { get; set; }
            public string txtThuocGMHSC27H4 { get; set; }
            public string txtThuocGMHSC28H4 { get; set; }
            public string txtThuocGMHSC29H4 { get; set; }
            public string txtThuocGMHSC30H4 { get; set; }
            public string txtThuocGMHSC31H4 { get; set; }
            public string txtThuocGMHSC32H4 { get; set; }
            public string txtThuocGMHSC33H4 { get; set; }
            public string txtThuocGMHSC34H4 { get; set; }
            public string txtThuocGMHSC35H4 { get; set; }
            public string txtThuocGMHSC36H4 { get; set; }
            public string txtThuocGMHSC37H4 { get; set; }
            public string txtThuocGMHSC38H4 { get; set; }
            public string txtThuocGMHSC39H4 { get; set; }

            public string txtThuocGMHSC1H5 { get; set; }
            public string txtThuocGMHSC2H5 { get; set; }
            public string txtThuocGMHSC3H5 { get; set; }
            public string txtThuocGMHSC4H5 { get; set; }
            public string txtThuocGMHSC5H5 { get; set; }
            public string txtThuocGMHSC6H5 { get; set; }
            public string txtThuocGMHSC7H5 { get; set; }
            public string txtThuocGMHSC8H5 { get; set; }
            public string txtThuocGMHSC9H5 { get; set; }
            public string txtThuocGMHSC10H5 { get; set; }
            public string txtThuocGMHSC11H5 { get; set; }
            public string txtThuocGMHSC12H5 { get; set; }
            public string txtThuocGMHSC13H5 { get; set; }
            public string txtThuocGMHSC14H5 { get; set; }
            public string txtThuocGMHSC15H5 { get; set; }
            public string txtThuocGMHSC16H5 { get; set; }
            public string txtThuocGMHSC17H5 { get; set; }
            public string txtThuocGMHSC18H5 { get; set; }
            public string txtThuocGMHSC19H5 { get; set; }
            public string txtThuocGMHSC20H5 { get; set; }
            public string txtThuocGMHSC21H5 { get; set; }
            public string txtThuocGMHSC22H5 { get; set; }
            public string txtThuocGMHSC23H5 { get; set; }
            public string txtThuocGMHSC24H5 { get; set; }
            public string txtThuocGMHSC25H5 { get; set; }
            public string txtThuocGMHSC26H5 { get; set; }
            public string txtThuocGMHSC27H5 { get; set; }
            public string txtThuocGMHSC28H5 { get; set; }
            public string txtThuocGMHSC29H5 { get; set; }
            public string txtThuocGMHSC30H5 { get; set; }
            public string txtThuocGMHSC31H5 { get; set; }
            public string txtThuocGMHSC32H5 { get; set; }
            public string txtThuocGMHSC33H5 { get; set; }
            public string txtThuocGMHSC34H5 { get; set; }
            public string txtThuocGMHSC35H5 { get; set; }
            public string txtThuocGMHSC36H5 { get; set; }
            public string txtThuocGMHSC37H5 { get; set; }
            public string txtThuocGMHSC38H5 { get; set; }
            public string txtThuocGMHSC39H5 { get; set; }

            public string txtThuocGMHSC1H6 { get; set; }
            public string txtThuocGMHSC2H6 { get; set; }
            public string txtThuocGMHSC3H6 { get; set; }
            public string txtThuocGMHSC4H6 { get; set; }
            public string txtThuocGMHSC5H6 { get; set; }
            public string txtThuocGMHSC6H6 { get; set; }
            public string txtThuocGMHSC7H6 { get; set; }
            public string txtThuocGMHSC8H6 { get; set; }
            public string txtThuocGMHSC9H6 { get; set; }
            public string txtThuocGMHSC10H6 { get; set; }
            public string txtThuocGMHSC11H6 { get; set; }
            public string txtThuocGMHSC12H6 { get; set; }
            public string txtThuocGMHSC13H6 { get; set; }
            public string txtThuocGMHSC14H6 { get; set; }
            public string txtThuocGMHSC15H6 { get; set; }
            public string txtThuocGMHSC16H6 { get; set; }
            public string txtThuocGMHSC17H6 { get; set; }
            public string txtThuocGMHSC18H6 { get; set; }
            public string txtThuocGMHSC19H6 { get; set; }
            public string txtThuocGMHSC20H6 { get; set; }
            public string txtThuocGMHSC21H6 { get; set; }
            public string txtThuocGMHSC22H6 { get; set; }
            public string txtThuocGMHSC23H6 { get; set; }
            public string txtThuocGMHSC24H6 { get; set; }
            public string txtThuocGMHSC25H6 { get; set; }
            public string txtThuocGMHSC26H6 { get; set; }
            public string txtThuocGMHSC27H6 { get; set; }
            public string txtThuocGMHSC28H6 { get; set; }
            public string txtThuocGMHSC29H6 { get; set; }
            public string txtThuocGMHSC30H6 { get; set; }
            public string txtThuocGMHSC31H6 { get; set; }
            public string txtThuocGMHSC32H6 { get; set; }
            public string txtThuocGMHSC33H6 { get; set; }
            public string txtThuocGMHSC34H6 { get; set; }
            public string txtThuocGMHSC35H6 { get; set; }
            public string txtThuocGMHSC36H6 { get; set; }
            public string txtThuocGMHSC37H6 { get; set; }
            public string txtThuocGMHSC38H6 { get; set; }
            public string txtThuocGMHSC39H6 { get; set; }

            public string txtThuocGayMeC1H1 { get; set; }
            public string txtThuocGayMeC2H1 { get; set; }
            public string txtThuocGayMeC3H1 { get; set; }
            public string txtThuocGayMeC4H1 { get; set; }
            public string txtThuocGayMeC5H1 { get; set; }
            public string txtThuocGayMeC6H1 { get; set; }
            public string txtThuocGayMeC7H1 { get; set; }
            public string txtThuocGayMeC8H1 { get; set; }
            public string txtThuocGayMeC9H1 { get; set; }
            public string txtThuocGayMeC10H1 { get; set; }
            public string txtThuocGayMeC11H1 { get; set; }
            public string txtThuocGayMeC12H1 { get; set; }
            public string txtThuocGayMeC13H1 { get; set; }
            public string txtThuocGayMeC14H1 { get; set; }
            public string txtThuocGayMeC15H1 { get; set; }
            public string txtThuocGayMeC16H1 { get; set; }
            public string txtThuocGayMeC17H1 { get; set; }
            public string txtThuocGayMeC18H1 { get; set; }
            public string txtThuocGayMeC19H1 { get; set; }
            public string txtThuocGayMeC20H1 { get; set; }
            public string txtThuocGayMeC21H1 { get; set; }
            public string txtThuocGayMeC22H1 { get; set; }
            public string txtThuocGayMeC23H1 { get; set; }
            public string txtThuocGayMeC24H1 { get; set; }
            public string txtThuocGayMeC25H1 { get; set; }
            public string txtThuocGayMeC26H1 { get; set; }
            public string txtThuocGayMeC27H1 { get; set; }
            public string txtThuocGayMeC28H1 { get; set; }
            public string txtThuocGayMeC29H1 { get; set; }
            public string txtThuocGayMeC30H1 { get; set; }
            public string txtThuocGayMeC31H1 { get; set; }
            public string txtThuocGayMeC32H1 { get; set; }
            public string txtThuocGayMeC33H1 { get; set; }
            public string txtThuocGayMeC34H1 { get; set; }
            public string txtThuocGayMeC35H1 { get; set; }
            public string txtThuocGayMeC36H1 { get; set; }
            public string txtThuocGayMeC37H1 { get; set; }
            public string txtThuocGayMeC38H1 { get; set; }
            public string txtThuocGayMeC39H1 { get; set; }

            public string txtThuocGayMeC1H2 { get; set; }
            public string txtThuocGayMeC2H2 { get; set; }
            public string txtThuocGayMeC3H2 { get; set; }
            public string txtThuocGayMeC4H2 { get; set; }
            public string txtThuocGayMeC5H2 { get; set; }
            public string txtThuocGayMeC6H2 { get; set; }
            public string txtThuocGayMeC7H2 { get; set; }
            public string txtThuocGayMeC8H2 { get; set; }
            public string txtThuocGayMeC9H2 { get; set; }
            public string txtThuocGayMeC10H2 { get; set; }
            public string txtThuocGayMeC11H2 { get; set; }
            public string txtThuocGayMeC12H2 { get; set; }
            public string txtThuocGayMeC13H2 { get; set; }
            public string txtThuocGayMeC14H2 { get; set; }
            public string txtThuocGayMeC15H2 { get; set; }
            public string txtThuocGayMeC16H2 { get; set; }
            public string txtThuocGayMeC17H2 { get; set; }
            public string txtThuocGayMeC18H2 { get; set; }
            public string txtThuocGayMeC19H2 { get; set; }
            public string txtThuocGayMeC20H2 { get; set; }
            public string txtThuocGayMeC21H2 { get; set; }
            public string txtThuocGayMeC22H2 { get; set; }
            public string txtThuocGayMeC23H2 { get; set; }
            public string txtThuocGayMeC24H2 { get; set; }
            public string txtThuocGayMeC25H2 { get; set; }
            public string txtThuocGayMeC26H2 { get; set; }
            public string txtThuocGayMeC27H2 { get; set; }
            public string txtThuocGayMeC28H2 { get; set; }
            public string txtThuocGayMeC29H2 { get; set; }
            public string txtThuocGayMeC30H2 { get; set; }
            public string txtThuocGayMeC31H2 { get; set; }
            public string txtThuocGayMeC32H2 { get; set; }
            public string txtThuocGayMeC33H2 { get; set; }
            public string txtThuocGayMeC34H2 { get; set; }
            public string txtThuocGayMeC35H2 { get; set; }
            public string txtThuocGayMeC36H2 { get; set; }
            public string txtThuocGayMeC37H2 { get; set; }
            public string txtThuocGayMeC38H2 { get; set; }
            public string txtThuocGayMeC39H2 { get; set; }

            public string txtThuocGayMeC1H3 { get; set; }
            public string txtThuocGayMeC2H3 { get; set; }
            public string txtThuocGayMeC3H3 { get; set; }
            public string txtThuocGayMeC4H3 { get; set; }
            public string txtThuocGayMeC5H3 { get; set; }
            public string txtThuocGayMeC6H3 { get; set; }
            public string txtThuocGayMeC7H3 { get; set; }
            public string txtThuocGayMeC8H3 { get; set; }
            public string txtThuocGayMeC9H3 { get; set; }
            public string txtThuocGayMeC10H3 { get; set; }
            public string txtThuocGayMeC11H3 { get; set; }
            public string txtThuocGayMeC12H3 { get; set; }
            public string txtThuocGayMeC13H3 { get; set; }
            public string txtThuocGayMeC14H3 { get; set; }
            public string txtThuocGayMeC15H3 { get; set; }
            public string txtThuocGayMeC16H3 { get; set; }
            public string txtThuocGayMeC17H3 { get; set; }
            public string txtThuocGayMeC18H3 { get; set; }
            public string txtThuocGayMeC19H3 { get; set; }
            public string txtThuocGayMeC20H3 { get; set; }
            public string txtThuocGayMeC21H3 { get; set; }
            public string txtThuocGayMeC22H3 { get; set; }
            public string txtThuocGayMeC23H3 { get; set; }
            public string txtThuocGayMeC24H3 { get; set; }
            public string txtThuocGayMeC25H3 { get; set; }
            public string txtThuocGayMeC26H3 { get; set; }
            public string txtThuocGayMeC27H3 { get; set; }
            public string txtThuocGayMeC28H3 { get; set; }
            public string txtThuocGayMeC29H3 { get; set; }
            public string txtThuocGayMeC30H3 { get; set; }
            public string txtThuocGayMeC31H3 { get; set; }
            public string txtThuocGayMeC32H3 { get; set; }
            public string txtThuocGayMeC33H3 { get; set; }
            public string txtThuocGayMeC34H3 { get; set; }
            public string txtThuocGayMeC35H3 { get; set; }
            public string txtThuocGayMeC36H3 { get; set; }
            public string txtThuocGayMeC37H3 { get; set; }
            public string txtThuocGayMeC38H3 { get; set; }
            public string txtThuocGayMeC39H3 { get; set; }

            public string txtThuocGayMeC1H4 { get; set; }
            public string txtThuocGayMeC2H4 { get; set; }
            public string txtThuocGayMeC3H4 { get; set; }
            public string txtThuocGayMeC4H4 { get; set; }
            public string txtThuocGayMeC5H4 { get; set; }
            public string txtThuocGayMeC6H4 { get; set; }
            public string txtThuocGayMeC7H4 { get; set; }
            public string txtThuocGayMeC8H4 { get; set; }
            public string txtThuocGayMeC9H4 { get; set; }
            public string txtThuocGayMeC10H4 { get; set; }
            public string txtThuocGayMeC11H4 { get; set; }
            public string txtThuocGayMeC12H4 { get; set; }
            public string txtThuocGayMeC13H4 { get; set; }
            public string txtThuocGayMeC14H4 { get; set; }
            public string txtThuocGayMeC15H4 { get; set; }
            public string txtThuocGayMeC16H4 { get; set; }
            public string txtThuocGayMeC17H4 { get; set; }
            public string txtThuocGayMeC18H4 { get; set; }
            public string txtThuocGayMeC19H4 { get; set; }
            public string txtThuocGayMeC20H4 { get; set; }
            public string txtThuocGayMeC21H4 { get; set; }
            public string txtThuocGayMeC22H4 { get; set; }
            public string txtThuocGayMeC23H4 { get; set; }
            public string txtThuocGayMeC24H4 { get; set; }
            public string txtThuocGayMeC25H4 { get; set; }
            public string txtThuocGayMeC26H4 { get; set; }
            public string txtThuocGayMeC27H4 { get; set; }
            public string txtThuocGayMeC28H4 { get; set; }
            public string txtThuocGayMeC29H4 { get; set; }
            public string txtThuocGayMeC30H4 { get; set; }
            public string txtThuocGayMeC31H4 { get; set; }
            public string txtThuocGayMeC32H4 { get; set; }
            public string txtThuocGayMeC33H4 { get; set; }
            public string txtThuocGayMeC34H4 { get; set; }
            public string txtThuocGayMeC35H4 { get; set; }
            public string txtThuocGayMeC36H4 { get; set; }
            public string txtThuocGayMeC37H4 { get; set; }
            public string txtThuocGayMeC38H4 { get; set; }
            public string txtThuocGayMeC39H4 { get; set; }

            public string txtThuocGayMeC1H5 { get; set; }
            public string txtThuocGayMeC2H5 { get; set; }
            public string txtThuocGayMeC3H5 { get; set; }
            public string txtThuocGayMeC4H5 { get; set; }
            public string txtThuocGayMeC5H5 { get; set; }
            public string txtThuocGayMeC6H5 { get; set; }
            public string txtThuocGayMeC7H5 { get; set; }
            public string txtThuocGayMeC8H5 { get; set; }
            public string txtThuocGayMeC9H5 { get; set; }
            public string txtThuocGayMeC10H5 { get; set; }
            public string txtThuocGayMeC11H5 { get; set; }
            public string txtThuocGayMeC12H5 { get; set; }
            public string txtThuocGayMeC13H5 { get; set; }
            public string txtThuocGayMeC14H5 { get; set; }
            public string txtThuocGayMeC15H5 { get; set; }
            public string txtThuocGayMeC16H5 { get; set; }
            public string txtThuocGayMeC17H5 { get; set; }
            public string txtThuocGayMeC18H5 { get; set; }
            public string txtThuocGayMeC19H5 { get; set; }
            public string txtThuocGayMeC20H5 { get; set; }
            public string txtThuocGayMeC21H5 { get; set; }
            public string txtThuocGayMeC22H5 { get; set; }
            public string txtThuocGayMeC23H5 { get; set; }
            public string txtThuocGayMeC24H5 { get; set; }
            public string txtThuocGayMeC25H5 { get; set; }
            public string txtThuocGayMeC26H5 { get; set; }
            public string txtThuocGayMeC27H5 { get; set; }
            public string txtThuocGayMeC28H5 { get; set; }
            public string txtThuocGayMeC29H5 { get; set; }
            public string txtThuocGayMeC30H5 { get; set; }
            public string txtThuocGayMeC31H5 { get; set; }
            public string txtThuocGayMeC32H5 { get; set; }
            public string txtThuocGayMeC33H5 { get; set; }
            public string txtThuocGayMeC34H5 { get; set; }
            public string txtThuocGayMeC35H5 { get; set; }
            public string txtThuocGayMeC36H5 { get; set; }
            public string txtThuocGayMeC37H5 { get; set; }
            public string txtThuocGayMeC38H5 { get; set; }
            public string txtThuocGayMeC39H5 { get; set; }

            public string txtThuocGayMeC1H6 { get; set; }
            public string txtThuocGayMeC2H6 { get; set; }
            public string txtThuocGayMeC3H6 { get; set; }
            public string txtThuocGayMeC4H6 { get; set; }
            public string txtThuocGayMeC5H6 { get; set; }
            public string txtThuocGayMeC6H6 { get; set; }
            public string txtThuocGayMeC7H6 { get; set; }
            public string txtThuocGayMeC8H6 { get; set; }
            public string txtThuocGayMeC9H6 { get; set; }
            public string txtThuocGayMeC10H6 { get; set; }
            public string txtThuocGayMeC11H6 { get; set; }
            public string txtThuocGayMeC12H6 { get; set; }
            public string txtThuocGayMeC13H6 { get; set; }
            public string txtThuocGayMeC14H6 { get; set; }
            public string txtThuocGayMeC15H6 { get; set; }
            public string txtThuocGayMeC16H6 { get; set; }
            public string txtThuocGayMeC17H6 { get; set; }
            public string txtThuocGayMeC18H6 { get; set; }
            public string txtThuocGayMeC19H6 { get; set; }
            public string txtThuocGayMeC20H6 { get; set; }
            public string txtThuocGayMeC21H6 { get; set; }
            public string txtThuocGayMeC22H6 { get; set; }
            public string txtThuocGayMeC23H6 { get; set; }
            public string txtThuocGayMeC24H6 { get; set; }
            public string txtThuocGayMeC25H6 { get; set; }
            public string txtThuocGayMeC26H6 { get; set; }
            public string txtThuocGayMeC27H6 { get; set; }
            public string txtThuocGayMeC28H6 { get; set; }
            public string txtThuocGayMeC29H6 { get; set; }
            public string txtThuocGayMeC30H6 { get; set; }
            public string txtThuocGayMeC31H6 { get; set; }
            public string txtThuocGayMeC32H6 { get; set; }
            public string txtThuocGayMeC33H6 { get; set; }
            public string txtThuocGayMeC34H6 { get; set; }
            public string txtThuocGayMeC35H6 { get; set; }
            public string txtThuocGayMeC36H6 { get; set; }
            public string txtThuocGayMeC37H6 { get; set; }
            public string txtThuocGayMeC38H6 { get; set; }
            public string txtThuocGayMeC39H6 { get; set; }

            public string txtDichC1H1 { get; set; }
            public string txtDichC2H1 { get; set; }
            public string txtDichC3H1 { get; set; }
            public string txtDichC4H1 { get; set; }
            public string txtDichC5H1 { get; set; }
            public string txtDichC6H1 { get; set; }
            public string txtDichC7H1 { get; set; }
            public string txtDichC8H1 { get; set; }
            public string txtDichC9H1 { get; set; }
            public string txtDichC10H1 { get; set; }
            public string txtDichC11H1 { get; set; }
            public string txtDichC12H1 { get; set; }
            public string txtDichC13H1 { get; set; }
            public string txtDichC14H1 { get; set; }
            public string txtDichC15H1 { get; set; }
            public string txtDichC16H1 { get; set; }
            public string txtDichC17H1 { get; set; }
            public string txtDichC18H1 { get; set; }
            public string txtDichC19H1 { get; set; }
            public string txtDichC20H1 { get; set; }
            public string txtDichC21H1 { get; set; }
            public string txtDichC22H1 { get; set; }
            public string txtDichC23H1 { get; set; }
            public string txtDichC24H1 { get; set; }
            public string txtDichC25H1 { get; set; }
            public string txtDichC26H1 { get; set; }
            public string txtDichC27H1 { get; set; }
            public string txtDichC28H1 { get; set; }
            public string txtDichC29H1 { get; set; }
            public string txtDichC30H1 { get; set; }
            public string txtDichC31H1 { get; set; }
            public string txtDichC32H1 { get; set; }
            public string txtDichC33H1 { get; set; }
            public string txtDichC34H1 { get; set; }
            public string txtDichC35H1 { get; set; }
            public string txtDichC36H1 { get; set; }
            public string txtDichC37H1 { get; set; }
            public string txtDichC38H1 { get; set; }
            public string txtDichC39H1 { get; set; }

            public string txtDichC1H2 { get; set; }
            public string txtDichC2H2 { get; set; }
            public string txtDichC3H2 { get; set; }
            public string txtDichC4H2 { get; set; }
            public string txtDichC5H2 { get; set; }
            public string txtDichC6H2 { get; set; }
            public string txtDichC7H2 { get; set; }
            public string txtDichC8H2 { get; set; }
            public string txtDichC9H2 { get; set; }
            public string txtDichC10H2 { get; set; }
            public string txtDichC11H2 { get; set; }
            public string txtDichC12H2 { get; set; }
            public string txtDichC13H2 { get; set; }
            public string txtDichC14H2 { get; set; }
            public string txtDichC15H2 { get; set; }
            public string txtDichC16H2 { get; set; }
            public string txtDichC17H2 { get; set; }
            public string txtDichC18H2 { get; set; }
            public string txtDichC19H2 { get; set; }
            public string txtDichC20H2 { get; set; }
            public string txtDichC21H2 { get; set; }
            public string txtDichC22H2 { get; set; }
            public string txtDichC23H2 { get; set; }
            public string txtDichC24H2 { get; set; }
            public string txtDichC25H2 { get; set; }
            public string txtDichC26H2 { get; set; }
            public string txtDichC27H2 { get; set; }
            public string txtDichC28H2 { get; set; }
            public string txtDichC29H2 { get; set; }
            public string txtDichC30H2 { get; set; }
            public string txtDichC31H2 { get; set; }
            public string txtDichC32H2 { get; set; }
            public string txtDichC33H2 { get; set; }
            public string txtDichC34H2 { get; set; }
            public string txtDichC35H2 { get; set; }
            public string txtDichC36H2 { get; set; }
            public string txtDichC37H2 { get; set; }
            public string txtDichC38H2 { get; set; }
            public string txtDichC39H2 { get; set; }

            public string txtDichC1H3 { get; set; }
            public string txtDichC2H3 { get; set; }
            public string txtDichC3H3 { get; set; }
            public string txtDichC4H3 { get; set; }
            public string txtDichC5H3 { get; set; }
            public string txtDichC6H3 { get; set; }
            public string txtDichC7H3 { get; set; }
            public string txtDichC8H3 { get; set; }
            public string txtDichC9H3 { get; set; }
            public string txtDichC10H3 { get; set; }
            public string txtDichC11H3 { get; set; }
            public string txtDichC12H3 { get; set; }
            public string txtDichC13H3 { get; set; }
            public string txtDichC14H3 { get; set; }
            public string txtDichC15H3 { get; set; }
            public string txtDichC16H3 { get; set; }
            public string txtDichC17H3 { get; set; }
            public string txtDichC18H3 { get; set; }
            public string txtDichC19H3 { get; set; }
            public string txtDichC20H3 { get; set; }
            public string txtDichC21H3 { get; set; }
            public string txtDichC22H3 { get; set; }
            public string txtDichC23H3 { get; set; }
            public string txtDichC24H3 { get; set; }
            public string txtDichC25H3 { get; set; }
            public string txtDichC26H3 { get; set; }
            public string txtDichC27H3 { get; set; }
            public string txtDichC28H3 { get; set; }
            public string txtDichC29H3 { get; set; }
            public string txtDichC30H3 { get; set; }
            public string txtDichC31H3 { get; set; }
            public string txtDichC32H3 { get; set; }
            public string txtDichC33H3 { get; set; }
            public string txtDichC34H3 { get; set; }
            public string txtDichC35H3 { get; set; }
            public string txtDichC36H3 { get; set; }
            public string txtDichC37H3 { get; set; }
            public string txtDichC38H3 { get; set; }
            public string txtDichC39H3 { get; set; }

            public string txtThuocKhac1 { get; set; }
            public string txtThuocKhac2 { get; set; }
            public string txtThuocKhac3 { get; set; }
            public string txtThuocKhac4 { get; set; }
            public string txtThuocKhac5 { get; set; }
            public string txtThuocKhac6 { get; set; }
            public string txtThuocKhac7 { get; set; }

            public string txtThuocKhacC1H1 { get; set; }
            public string txtThuocKhacC2H1 { get; set; }
            public string txtThuocKhacC3H1 { get; set; }
            public string txtThuocKhacC4H1 { get; set; }
            public string txtThuocKhacC5H1 { get; set; }
            public string txtThuocKhacC6H1 { get; set; }
            public string txtThuocKhacC7H1 { get; set; }
            public string txtThuocKhacC8H1 { get; set; }
            public string txtThuocKhacC9H1 { get; set; }
            public string txtThuocKhacC10H1 { get; set; }
            public string txtThuocKhacC11H1 { get; set; }
            public string txtThuocKhacC12H1 { get; set; }
            public string txtThuocKhacC13H1 { get; set; }
            public string txtThuocKhacC14H1 { get; set; }
            public string txtThuocKhacC15H1 { get; set; }
            public string txtThuocKhacC16H1 { get; set; }
            public string txtThuocKhacC17H1 { get; set; }
            public string txtThuocKhacC18H1 { get; set; }
            public string txtThuocKhacC19H1 { get; set; }
            public string txtThuocKhacC20H1 { get; set; }
            public string txtThuocKhacC21H1 { get; set; }
            public string txtThuocKhacC22H1 { get; set; }
            public string txtThuocKhacC23H1 { get; set; }
            public string txtThuocKhacC24H1 { get; set; }
            public string txtThuocKhacC25H1 { get; set; }
            public string txtThuocKhacC26H1 { get; set; }
            public string txtThuocKhacC27H1 { get; set; }
            public string txtThuocKhacC28H1 { get; set; }
            public string txtThuocKhacC29H1 { get; set; }
            public string txtThuocKhacC30H1 { get; set; }
            public string txtThuocKhacC31H1 { get; set; }
            public string txtThuocKhacC32H1 { get; set; }
            public string txtThuocKhacC33H1 { get; set; }
            public string txtThuocKhacC34H1 { get; set; }
            public string txtThuocKhacC35H1 { get; set; }
            public string txtThuocKhacC36H1 { get; set; }
            public string txtThuocKhacC37H1 { get; set; }
            public string txtThuocKhacC38H1 { get; set; }
            public string txtThuocKhacC39H1 { get; set; }

            public string txtThuocKhacC1H2 { get; set; }
            public string txtThuocKhacC2H2 { get; set; }
            public string txtThuocKhacC3H2 { get; set; }
            public string txtThuocKhacC4H2 { get; set; }
            public string txtThuocKhacC5H2 { get; set; }
            public string txtThuocKhacC6H2 { get; set; }
            public string txtThuocKhacC7H2 { get; set; }
            public string txtThuocKhacC8H2 { get; set; }
            public string txtThuocKhacC9H2 { get; set; }
            public string txtThuocKhacC10H2 { get; set; }
            public string txtThuocKhacC11H2 { get; set; }
            public string txtThuocKhacC12H2 { get; set; }
            public string txtThuocKhacC13H2 { get; set; }
            public string txtThuocKhacC14H2 { get; set; }
            public string txtThuocKhacC15H2 { get; set; }
            public string txtThuocKhacC16H2 { get; set; }
            public string txtThuocKhacC17H2 { get; set; }
            public string txtThuocKhacC18H2 { get; set; }
            public string txtThuocKhacC19H2 { get; set; }
            public string txtThuocKhacC20H2 { get; set; }
            public string txtThuocKhacC21H2 { get; set; }
            public string txtThuocKhacC22H2 { get; set; }
            public string txtThuocKhacC23H2 { get; set; }
            public string txtThuocKhacC24H2 { get; set; }
            public string txtThuocKhacC25H2 { get; set; }
            public string txtThuocKhacC26H2 { get; set; }
            public string txtThuocKhacC27H2 { get; set; }
            public string txtThuocKhacC28H2 { get; set; }
            public string txtThuocKhacC29H2 { get; set; }
            public string txtThuocKhacC30H2 { get; set; }
            public string txtThuocKhacC31H2 { get; set; }
            public string txtThuocKhacC32H2 { get; set; }
            public string txtThuocKhacC33H2 { get; set; }
            public string txtThuocKhacC34H2 { get; set; }
            public string txtThuocKhacC35H2 { get; set; }
            public string txtThuocKhacC36H2 { get; set; }
            public string txtThuocKhacC37H2 { get; set; }
            public string txtThuocKhacC38H2 { get; set; }
            public string txtThuocKhacC39H2 { get; set; }

            public string txtThuocKhacC1H3 { get; set; }
            public string txtThuocKhacC2H3 { get; set; }
            public string txtThuocKhacC3H3 { get; set; }
            public string txtThuocKhacC4H3 { get; set; }
            public string txtThuocKhacC5H3 { get; set; }
            public string txtThuocKhacC6H3 { get; set; }
            public string txtThuocKhacC7H3 { get; set; }
            public string txtThuocKhacC8H3 { get; set; }
            public string txtThuocKhacC9H3 { get; set; }
            public string txtThuocKhacC10H3 { get; set; }
            public string txtThuocKhacC11H3 { get; set; }
            public string txtThuocKhacC12H3 { get; set; }
            public string txtThuocKhacC13H3 { get; set; }
            public string txtThuocKhacC14H3 { get; set; }
            public string txtThuocKhacC15H3 { get; set; }
            public string txtThuocKhacC16H3 { get; set; }
            public string txtThuocKhacC17H3 { get; set; }
            public string txtThuocKhacC18H3 { get; set; }
            public string txtThuocKhacC19H3 { get; set; }
            public string txtThuocKhacC20H3 { get; set; }
            public string txtThuocKhacC21H3 { get; set; }
            public string txtThuocKhacC22H3 { get; set; }
            public string txtThuocKhacC23H3 { get; set; }
            public string txtThuocKhacC24H3 { get; set; }
            public string txtThuocKhacC25H3 { get; set; }
            public string txtThuocKhacC26H3 { get; set; }
            public string txtThuocKhacC27H3 { get; set; }
            public string txtThuocKhacC28H3 { get; set; }
            public string txtThuocKhacC29H3 { get; set; }
            public string txtThuocKhacC30H3 { get; set; }
            public string txtThuocKhacC31H3 { get; set; }
            public string txtThuocKhacC32H3 { get; set; }
            public string txtThuocKhacC33H3 { get; set; }
            public string txtThuocKhacC34H3 { get; set; }
            public string txtThuocKhacC35H3 { get; set; }
            public string txtThuocKhacC36H3 { get; set; }
            public string txtThuocKhacC37H3 { get; set; }
            public string txtThuocKhacC38H3 { get; set; }
            public string txtThuocKhacC39H3 { get; set; }

            public string txtThuocKhacC1H4 { get; set; }
            public string txtThuocKhacC2H4 { get; set; }
            public string txtThuocKhacC3H4 { get; set; }
            public string txtThuocKhacC4H4 { get; set; }
            public string txtThuocKhacC5H4 { get; set; }
            public string txtThuocKhacC6H4 { get; set; }
            public string txtThuocKhacC7H4 { get; set; }
            public string txtThuocKhacC8H4 { get; set; }
            public string txtThuocKhacC9H4 { get; set; }
            public string txtThuocKhacC10H4 { get; set; }
            public string txtThuocKhacC11H4 { get; set; }
            public string txtThuocKhacC12H4 { get; set; }
            public string txtThuocKhacC13H4 { get; set; }
            public string txtThuocKhacC14H4 { get; set; }
            public string txtThuocKhacC15H4 { get; set; }
            public string txtThuocKhacC16H4 { get; set; }
            public string txtThuocKhacC17H4 { get; set; }
            public string txtThuocKhacC18H4 { get; set; }
            public string txtThuocKhacC19H4 { get; set; }
            public string txtThuocKhacC20H4 { get; set; }
            public string txtThuocKhacC21H4 { get; set; }
            public string txtThuocKhacC22H4 { get; set; }
            public string txtThuocKhacC23H4 { get; set; }
            public string txtThuocKhacC24H4 { get; set; }
            public string txtThuocKhacC25H4 { get; set; }
            public string txtThuocKhacC26H4 { get; set; }
            public string txtThuocKhacC27H4 { get; set; }
            public string txtThuocKhacC28H4 { get; set; }
            public string txtThuocKhacC29H4 { get; set; }
            public string txtThuocKhacC30H4 { get; set; }
            public string txtThuocKhacC31H4 { get; set; }
            public string txtThuocKhacC32H4 { get; set; }
            public string txtThuocKhacC33H4 { get; set; }
            public string txtThuocKhacC34H4 { get; set; }
            public string txtThuocKhacC35H4 { get; set; }
            public string txtThuocKhacC36H4 { get; set; }
            public string txtThuocKhacC37H4 { get; set; }
            public string txtThuocKhacC38H4 { get; set; }
            public string txtThuocKhacC39H4 { get; set; }

            public string txtThuocKhacC1H5 { get; set; }
            public string txtThuocKhacC2H5 { get; set; }
            public string txtThuocKhacC3H5 { get; set; }
            public string txtThuocKhacC4H5 { get; set; }
            public string txtThuocKhacC5H5 { get; set; }
            public string txtThuocKhacC6H5 { get; set; }
            public string txtThuocKhacC7H5 { get; set; }
            public string txtThuocKhacC8H5 { get; set; }
            public string txtThuocKhacC9H5 { get; set; }
            public string txtThuocKhacC10H5 { get; set; }
            public string txtThuocKhacC11H5 { get; set; }
            public string txtThuocKhacC12H5 { get; set; }
            public string txtThuocKhacC13H5 { get; set; }
            public string txtThuocKhacC14H5 { get; set; }
            public string txtThuocKhacC15H5 { get; set; }
            public string txtThuocKhacC16H5 { get; set; }
            public string txtThuocKhacC17H5 { get; set; }
            public string txtThuocKhacC18H5 { get; set; }
            public string txtThuocKhacC19H5 { get; set; }
            public string txtThuocKhacC20H5 { get; set; }
            public string txtThuocKhacC21H5 { get; set; }
            public string txtThuocKhacC22H5 { get; set; }
            public string txtThuocKhacC23H5 { get; set; }
            public string txtThuocKhacC24H5 { get; set; }
            public string txtThuocKhacC25H5 { get; set; }
            public string txtThuocKhacC26H5 { get; set; }
            public string txtThuocKhacC27H5 { get; set; }
            public string txtThuocKhacC28H5 { get; set; }
            public string txtThuocKhacC29H5 { get; set; }
            public string txtThuocKhacC30H5 { get; set; }
            public string txtThuocKhacC31H5 { get; set; }
            public string txtThuocKhacC32H5 { get; set; }
            public string txtThuocKhacC33H5 { get; set; }
            public string txtThuocKhacC34H5 { get; set; }
            public string txtThuocKhacC35H5 { get; set; }
            public string txtThuocKhacC36H5 { get; set; }
            public string txtThuocKhacC37H5 { get; set; }
            public string txtThuocKhacC38H5 { get; set; }
            public string txtThuocKhacC39H5 { get; set; }

            public string txtThuocKhacC1H6 { get; set; }
            public string txtThuocKhacC2H6 { get; set; }
            public string txtThuocKhacC3H6 { get; set; }
            public string txtThuocKhacC4H6 { get; set; }
            public string txtThuocKhacC5H6 { get; set; }
            public string txtThuocKhacC6H6 { get; set; }
            public string txtThuocKhacC7H6 { get; set; }
            public string txtThuocKhacC8H6 { get; set; }
            public string txtThuocKhacC9H6 { get; set; }
            public string txtThuocKhacC10H6 { get; set; }
            public string txtThuocKhacC11H6 { get; set; }
            public string txtThuocKhacC12H6 { get; set; }
            public string txtThuocKhacC13H6 { get; set; }
            public string txtThuocKhacC14H6 { get; set; }
            public string txtThuocKhacC15H6 { get; set; }
            public string txtThuocKhacC16H6 { get; set; }
            public string txtThuocKhacC17H6 { get; set; }
            public string txtThuocKhacC18H6 { get; set; }
            public string txtThuocKhacC19H6 { get; set; }
            public string txtThuocKhacC20H6 { get; set; }
            public string txtThuocKhacC21H6 { get; set; }
            public string txtThuocKhacC22H6 { get; set; }
            public string txtThuocKhacC23H6 { get; set; }
            public string txtThuocKhacC24H6 { get; set; }
            public string txtThuocKhacC25H6 { get; set; }
            public string txtThuocKhacC26H6 { get; set; }
            public string txtThuocKhacC27H6 { get; set; }
            public string txtThuocKhacC28H6 { get; set; }
            public string txtThuocKhacC29H6 { get; set; }
            public string txtThuocKhacC30H6 { get; set; }
            public string txtThuocKhacC31H6 { get; set; }
            public string txtThuocKhacC32H6 { get; set; }
            public string txtThuocKhacC33H6 { get; set; }
            public string txtThuocKhacC34H6 { get; set; }
            public string txtThuocKhacC35H6 { get; set; }
            public string txtThuocKhacC36H6 { get; set; }
            public string txtThuocKhacC37H6 { get; set; }
            public string txtThuocKhacC38H6 { get; set; }
            public string txtThuocKhacC39H6 { get; set; }

            public string txtThuocKhacC1H7 { get; set; }
            public string txtThuocKhacC2H7 { get; set; }
            public string txtThuocKhacC3H7 { get; set; }
            public string txtThuocKhacC4H7 { get; set; }
            public string txtThuocKhacC5H7 { get; set; }
            public string txtThuocKhacC6H7 { get; set; }
            public string txtThuocKhacC7H7 { get; set; }
            public string txtThuocKhacC8H7 { get; set; }
            public string txtThuocKhacC9H7 { get; set; }
            public string txtThuocKhacC10H7 { get; set; }
            public string txtThuocKhacC11H7 { get; set; }
            public string txtThuocKhacC12H7 { get; set; }
            public string txtThuocKhacC13H7 { get; set; }
            public string txtThuocKhacC14H7 { get; set; }
            public string txtThuocKhacC15H7 { get; set; }
            public string txtThuocKhacC16H7 { get; set; }
            public string txtThuocKhacC17H7 { get; set; }
            public string txtThuocKhacC18H7 { get; set; }
            public string txtThuocKhacC19H7 { get; set; }
            public string txtThuocKhacC20H7 { get; set; }
            public string txtThuocKhacC21H7 { get; set; }
            public string txtThuocKhacC22H7 { get; set; }
            public string txtThuocKhacC23H7 { get; set; }
            public string txtThuocKhacC24H7 { get; set; }
            public string txtThuocKhacC25H7 { get; set; }
            public string txtThuocKhacC26H7 { get; set; }
            public string txtThuocKhacC27H7 { get; set; }
            public string txtThuocKhacC28H7 { get; set; }
            public string txtThuocKhacC29H7 { get; set; }
            public string txtThuocKhacC30H7 { get; set; }
            public string txtThuocKhacC31H7 { get; set; }
            public string txtThuocKhacC32H7 { get; set; }
            public string txtThuocKhacC33H7 { get; set; }
            public string txtThuocKhacC34H7 { get; set; }
            public string txtThuocKhacC35H7 { get; set; }
            public string txtThuocKhacC36H7 { get; set; }
            public string txtThuocKhacC37H7 { get; set; }
            public string txtThuocKhacC38H7 { get; set; }
            public string txtThuocKhacC39H7 { get; set; }

            public string txtTruocGayMeH1 { get; set; }
            public string txtTruocGayMeH2 { get; set; }
            public string txtTruocGayMeH3 { get; set; }
            public string txtKhoiMeH4 { get; set; }
            public string txtKhoiMeH5 { get; set; }
            public string txtKhoiMeH6 { get; set; }
            public string txtKhoiMeH7 { get; set; }
            public string txtKhoiMeH8 { get; set; }

            public string txtDuyTriMeH9 { get; set; }
            public string txtDuyTriMeH10 { get; set; }
            public string txtDuyTriMeH11 { get; set; }
            public string txtDuyTriMeH12 { get; set; }
            public string txtDuyTriMeH13 { get; set; }
            public string txtDuyTriMeH14 { get; set; }
            public string txtDuyTriMeH15 { get; set; }
            public string txtDuyTriMeH16 { get; set; }
            public string txtKetThucMeH17 { get; set; }
            public string txtKetThucMeH18 { get; set; }
            public string txtKetThucMeH19 { get; set; }
            public string txtKetThucMeH20 { get; set; }
            public string txtKetThucMeH21 { get; set; }
            public string txtKetThucMeH22 { get; set; }
            public string txtKetThucMeH23 { get; set; }
            public string txtKetThucMeH24 { get; set; }
            public string txtGhiChepSauPhauThuat { get; set; }
            public string txtMach { get; set; }
            public string txtDiemVAS { get; set; }
            public string txtThongTinThem1 { get; set; }
            public string txtThongTinThem2 { get; set; }
            public string txtNon { get; set; }
            public string txtHA { get; set; }
            public string txtSPO2 { get; set; }
            public string txtRetRun { get; set; }
            public string txtĐDXacNhan1 { get; set; }
            public string txtThongTinThem3 { get; set; }
            public string txtNhipTho { get; set; }
            public string txtVetMo { get; set; }
            public string txtĐDXacNhan2 { get; set; }
            public string txtThongTinThem4 { get; set; }
            public string txtYThucBN { get; set; }
            public string txtĐDXacNhan3 { get; set; }
            public string txtThongTinThem5 { get; set; }
            public string txtGioChuyenBNVeKhoaDieuTri { get; set; }
            public string txtDeNghi1 { get; set; }
            public string txtDeNghi2 { get; set; }
            public string txtGacPhat1 { get; set; }
            public string txtGacPhat2 { get; set; }
            public string txtGacPhat3 { get; set; }
            public string txtGacPhat4 { get; set; }
            public string txtGacPhat5 { get; set; }
            public string txtGacPhat6 { get; set; }
            public string txtGacPhat7 { get; set; }

            public string txtGacThu1 { get; set; }
            public string txtGacThu2 { get; set; }
            public string txtGacThu3 { get; set; }
            public string txtGacThu4 { get; set; }
            public string txtGacThu5 { get; set; }
            public string txtGacThu6 { get; set; }
            public string txtGacThu7 { get; set; }

            public string txtGhiChu1 { get; set; }
            public string txtGhiChu2 { get; set; }
            public string txtGhiChu3 { get; set; }
            public string txtGhiChu4 { get; set; }
            public string txtGhiChu5 { get; set; }
            public string txtGhiChu6 { get; set; }
            public string txtGhiChu7 { get; set; }
            public string txtPhauThuatVien { get; set; }
            public string txtHuuTrung { get; set; }
            public string txtVoTrung { get; set; }
            public string txtThuocDungTrongMo1 { get; set; }
            public string txtThuocDungTrongMo2 { get; set; }
            public string txtThuocDungTrongMo3 { get; set; }
            public string txtThuocDungTrongMo4 { get; set; }
            public string txtThuocDungTrongMo5 { get; set; }
            public string txtThuocDungTrongMo6 { get; set; }
            public string txtThuocDungTrongMo7 { get; set; }
            public string txtThuocDungTrongMo8 { get; set; }
            public string txtThuocDungTrongMo9 { get; set; }
            public string txtThuocDungTrongMo10 { get; set; }
            public string txtThuocDungTrongMo11 { get; set; }
            public string txtThuocDungTrongMo12 { get; set; }
            public string txtThuocDungTrongMo13 { get; set; }
            public string txtThuocDungTrongMo14 { get; set; }
            public string txtThuocDungTrongMo15 { get; set; }
            public string txtThuocDungTrongMo16 { get; set; }
            public string txtThuocDungTrongMo17 { get; set; }
            public string txtThuocDungTrongMo18 { get; set; }
            public string txtThuocDungTrongMo19 { get; set; }
            public string txtThuocDungTrongMo20 { get; set; }
            public string txtThuocDungTrongMo21 { get; set; }
            public string txtThuocDungTrongMo22 { get; set; }
            public string txtThuocDungTrongMo23 { get; set; }

            public string txtThoiGianc1 { get; set; }
            public string txtThoiGianc2 { get; set; }
            public string txtThoiGianc3 { get; set; }
            public string txtThoiGianc4 { get; set; }
            public string txtThoiGianc5 { get; set; }
            public string txtThoiGianc6 { get; set; }
            public string txtThoiGianc7 { get; set; }
            public string txtThoiGianc8 { get; set; }
            public string txtThoiGianc9 { get; set; }
            public string txtThoiGianc10 { get; set; }
            public string txtThoiGianc11 { get; set; }
            public string txtThoiGianc12 { get; set; }
            public string txtThoiGianc13 { get; set; }
            public string txtThoiGianc14 { get; set; }
            public string txtThoiGianc15 { get; set; }
            public string txtThoiGianc16 { get; set; }
            public string txtThoiGianc17 { get; set; }
            public string txtThoiGianc18 { get; set; }
            public string txtThoiGianc19 { get; set; }
            public string txtThoiGianc20 { get; set; }
            public string txtThoiGianc21 { get; set; }
            public string txtThoiGianc22 { get; set; }
            public string txtThoiGianc23 { get; set; }
            public string txtThoiGianc24 { get; set; }
            public string txtThoiGianc25 { get; set; }
            public string txtThoiGianc26 { get; set; }
            public string txtThoiGianc27 { get; set; }
            public string txtThoiGianc28 { get; set; }
            public string txtThoiGianc29 { get; set; }
            public string txtThoiGianc30 { get; set; }
            public string txtThoiGianc31 { get; set; }
            public string txtThoiGianc32 { get; set; }
            public string txtThoiGianc33 { get; set; }
            public string txtThoiGianc34 { get; set; }
            public string txtThoiGianc35 { get; set; }
            public string txtThoiGianc36 { get; set; }
            public string txtThoiGianc37 { get; set; }
            public string txtThoiGianc38 { get; set; }
            public string txtThoiGianc39 { get; set; }
            public string lblPatientName { get; set; }
            public string lblAge { get; set; }
            public string lblGenderName { get; set; }
            public string lblChieuCao { get; set; }
            public string lblCanNang { get; set; }
            public string lblNhomMau { get; set; }
            public string ckNam { get; set; }
            public string ckNu { get; set; }

       




        }
    }
}
