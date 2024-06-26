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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;

namespace MPS.Core.Mps000118
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000118RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal HIS_DHST dhsts { get; set; }
        internal V_HIS_PRESCRIPTION serviceReq { get; set; }
        internal List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines { get; set; }
        string bedRoomName;
        string medi_stock_name;
        internal V_HIS_EXAM_SERVICE_REQ examServiceReq { get; set; }
        string title;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000118RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string medi_stock_name,
            V_HIS_EXAM_SERVICE_REQ examServiceReq,
            string _title
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.dhsts = dhsts;
                this.serviceReq = serviceReq;
                this.expMestMedicines = expMestMedicines;
                this.medi_stock_name = medi_stock_name;
                this.examServiceReq = examServiceReq;
                this.title = _title;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000118RDO(
         PatientADO patient,
         PatyAlterBhytADO PatyAlterBhyt,
         HIS_DHST dhsts,
         V_HIS_PRESCRIPTION serviceReq,
         List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
         string medi_stock_name,
         V_HIS_EXAM_SERVICE_REQ examServiceReq,
         string _title, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export
         )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.dhsts = dhsts;
                this.serviceReq = serviceReq;
                this.expMestMedicines = expMestMedicines;
                this.medi_stock_name = medi_stock_name;
                this.examServiceReq = examServiceReq;
                this.title = _title;
                this.expMesttSttId__Draft = _expMesttSttId__Draft;
                this.expMesttSttId__Request = _expMesttSttId__Request;
                this.expMesttSttId__Reject = _expMesttSttId__Reject;
                this.expMesttSttId__Approval = _expMesttSttId__Approval;
                this.expMesttSttId__Export = _expMesttSttId__Export;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                keyValues.Add(new KeyValue(Mps000118ExtendSingleKey.TITLE, title));
                if (!string.IsNullOrEmpty(medi_stock_name))
                {
                    keyValues.Add(new KeyValue(Mps000118ExtendSingleKey.MEDI_STOCK_NAME, medi_stock_name));
                }
                if (dhsts != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(dhsts, keyValues, false);
                }
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    expMestMedicines = expMestMedicines.OrderBy(p => p.NUM_ORDER).ToList();
                    if (serviceReq.EXP_MEST_STT_ID == expMesttSttId__Approval || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        expMestMedicines = expMestMedicines.Where(o => ((o.Type == 1 || o.Type == 2) && (o.IN_EXECUTE ?? -1) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE) || (o.Type != 1 && o.Type != 2)).ToList();
                    }
                    else if (serviceReq.EXP_MEST_STT_ID == expMesttSttId__Request || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Reject || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Draft)
                    {
                        expMestMedicines = expMestMedicines.Where(o => ((o.Type == 1 || o.Type == 2) && (o.IN_REQUEST ?? -1) == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE) || (o.Type != 1 && o.Type != 2)).ToList();
                    }
                    var expMestMedicineGroup = expMestMedicines.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.PRICE, o.IN_EXECUTE, o.IN_REQUEST, o.IS_EXPEND, o.MEDICINE_USE_FORM_ID, o.Type }).Select(y =>
                       new
                       {
                           MEDICINE_TYPE_ID = y.First().MEDICINE_TYPE_ID,
                           AMOUNT = y.Sum(o => o.AMOUNT),
                           Type = y.First().Type,

                       }).ToList();
                    expMestMedicines = expMestMedicines.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.PRICE, o.IN_EXECUTE, o.IN_REQUEST, o.IS_EXPEND, o.MEDICINE_USE_FORM_ID, o.Type }).Select(o => o.First()).ToList();
                    foreach (var item in expMestMedicines)
                    {
                        var medicineCheck = expMestMedicineGroup.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && o.Type == item.Type);
                        if (medicineCheck != null)
                        {
                            item.AMOUNT = medicineCheck.AMOUNT;
                        }
                    }
                    if (expMestMedicines != null && expMestMedicines.Count > 0)
                    {
                        long maxUseTimeTo = expMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                        keyValues.Add((new KeyValue(Mps000118ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxUseTimeTo))));
                        keyValues.Add((new KeyValue(Mps000118ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMestMedicines[0].CREATE_TIME ?? 0))));
                    }
                }

                if (this.serviceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000118ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.serviceReq.INTRUCTION_TIME)));
                    keyValues.Add(new KeyValue(Mps000118ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.serviceReq.INTRUCTION_TIME)));
                }

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PRESCRIPTION>(serviceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);

                if (examServiceReq != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(examServiceReq, keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
