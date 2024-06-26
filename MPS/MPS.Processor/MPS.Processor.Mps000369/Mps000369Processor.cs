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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000369.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000369
{
    public class Mps000369Processor : AbstractProcessor
    {
        Mps000369PDO rdo;
        List<HIS_SERVICE_TYPE> _ListServiceType = null;
        List<SereServDebtADO> _ListSereServDebtADO = null;
        public Mps000369Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000369PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessObjectTag();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ServiceType", _ListServiceType);
                objectTag.AddObjectData(store, "SereServDebt", this._ListSereServDebtADO);
                objectTag.AddRelationship(store, "ServiceType", "SereServDebt", "ID", "TDL_SERVICE_TYPE_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessObjectTag()
        {
            try
            {
                if (rdo._listSereServDebt != null && rdo._listSereServDebt.Count > 0)
                {
                    List<long> serviceTypeIdList = rdo._listSereServDebt.Select(o => o.TDL_SERVICE_TYPE_ID).Distinct().ToList();
                    _ListServiceType = rdo._listServiceType.Where(o => serviceTypeIdList.Contains(o.ID)).ToList();
                    this._ListSereServDebtADO = new List<SereServDebtADO>();
                    foreach (var item in rdo._listSereServDebt)
                    {
                        SereServDebtADO ado = new SereServDebtADO();
                        AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_DEBT, SereServDebtADO>();
                        ado = AutoMapper.Mapper.Map<SereServDebtADO>(item);
                        var check = rdo._ListSereServ.Where(o => o.ID == item.SERE_SERV_ID).ToList();
                        if (check != null && check.Count > 0)
                        {
                            ado.PRICE = check[0].PRICE;
                            ado.VAT_RATIO = check[0].VAT_RATIO;
                            ado.PRICE_AFTER_VAT = ado.PRICE * (1 + ado.VAT_RATIO);
                        }
                        this._ListSereServDebtADO.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_NUM, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToString(rdo._Transaction.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    //string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));

                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_TEXT, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToString(amountAfterExem, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_NUM, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.DESCRIPTION, rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.EXEMPTION_REASON, rdo._Transaction.EXEMPTION_REASON));
                }

                if (rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    //var totalBhyt = rdo._ListSereServ.Where(o => o.PATIENT_TYPE_ID == rdo._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    //var totalNd = rdo._ListSereServ.Where(o => o.PATIENT_TYPE_ID != rdo._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    //SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.TOTAL_AMOUNT_BHYT, totalBhyt));
                    //SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.TOTAL_AMOUNT_ND, totalNd));

                    //string totalBhytText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalBhyt));
                    //string totalNdText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalNd));
                    //SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.TOTAL_AMOUNT_BHYT_UPPER_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalBhyt, 4)));
                    //SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.TOTAL_AMOUNT_ND_UPPER_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalNd, 4)));

                    decimal totalPrice = rdo._ListSereServ.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                }

                if (rdo._PatientTypeAlter != null)
                {
                    var ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(rdo._PatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, rdo._PatientTypeAlter.HEIN_CARD_NUMBER, rdo._PatientTypeAlter.LEVEL_CODE, rdo._PatientTypeAlter.RIGHT_ROUTE_CODE);
                    if (ratio.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.HEIN_RATIO_100, (1 - ratio.Value) * 100));
                    }
                    SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatientTypeAlter.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);
                }
                if (rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<HIS_PATIENT>(rdo._Patient, false);
                }
                if (rdo._DepartmentTran != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo._DepartmentTran, false);
                }
                //SetSingleKey(new KeyValue(Mps000369ExtendSingleKey.KEY_THU_PHI, rdo.KeyThuPhi));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public static class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
