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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000103.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MPS.Processor.Mps000103
{
    class Mps000103Processor : AbstractProcessor
    {
        Mps000103PDO rdo;
        public Mps000103Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000103PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServ1", rdo._ListSereServ);
                objectTag.AddObjectData(store, "SereServ2", rdo._ListSereServ);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalPatientPrice = 0;
                if (this.rdo._ListSereServ != null && this.rdo._ListSereServ.Count > 0)
                {
                    totalPrice = this.rdo._ListSereServ.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalPatientPrice = this.rdo._ListSereServ.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    string serviceTypeName = "";
                    var Groups = this.rdo._ListSereServ.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var item in Groups)
                    {
                        if (String.IsNullOrEmpty(serviceTypeName))
                            serviceTypeName = item.ToList<V_HIS_SERE_SERV>().First().SERVICE_TYPE_NAME;
                        else
                            serviceTypeName = serviceTypeName + " - " + item.ToList<V_HIS_SERE_SERV>().First().SERVICE_TYPE_NAME;
                    }
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.SERVICE_TYPE_NAME, serviceTypeName));
                }

                if (this.rdo._Transaction != null)
                {

                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = this.rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this.rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.RATIO_TEXT, rdo.ratioText));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));

                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._Transaction.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.DESCRIPTION, this.rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000103ExtendSingleKey.EXEMPTION_REASON, this.rdo._Transaction.EXEMPTION_REASON));
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this.rdo._Transaction, false);
                    if (this.rdo._PatientTypeAlter != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(this.rdo._PatientTypeAlter, false);
                    }
                }


                if (this.rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(this.rdo._Patient, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
