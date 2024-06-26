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

namespace MPS.Core.Mps000044
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000044RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal HIS_DHST dhsts { get; set; }
        internal V_HIS_PRESCRIPTION serviceReq { get; set; }
        internal List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines { get; set; }
        string bedRoomName;
        string medi_stock_name;
        internal long keyNameTitles;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000044RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string medi_stock_name
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000044RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string medi_stock_name,
            long keyNameTitles
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
                this.keyNameTitles = keyNameTitles;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000044RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string medi_stock_name,
            long keyNameTitles, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export
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
                this.keyNameTitles = keyNameTitles;
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
                if (keyNameTitles == 1)
                {
                    keyValues.Add(new KeyValue(Mps000044ExtendSingleKey.KEY_NAME_TITLES, "H"));
                }
                else if (keyNameTitles == 2)
                {
                    keyValues.Add(new KeyValue(Mps000044ExtendSingleKey.KEY_NAME_TITLES, "N"));
                }
                else if (keyNameTitles == 3)
                {
                    keyValues.Add(new KeyValue(Mps000044ExtendSingleKey.KEY_NAME_TITLES, "TP"));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000044ExtendSingleKey.KEY_NAME_TITLES, " "));
                }


                if (!string.IsNullOrEmpty(medi_stock_name))
                {
                    keyValues.Add(new KeyValue(Mps000044ExtendSingleKey.MEDI_STOCK_NAME, medi_stock_name));
                }
                if (dhsts != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(dhsts, keyValues, false);
                }
                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    long maxUseTimeTo = expMestMedicines.Max(a => a.USE_TIME_TO ?? 0);
                    //keyValues.Add((new KeyValue(Mps000044ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxUseTimeTo))));
                    keyValues.Add((new KeyValue(Mps000044ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.USE_TIME_TO ?? 0))));
                    keyValues.Add((new KeyValue(Mps000044ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.USE_TIME ?? 0))));
                }

                if (serviceReq != null)
                {
                    if (serviceReq.EXP_MEST_STT_ID == expMesttSttId__Approval || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        expMestMedicines = expMestMedicines.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (serviceReq.EXP_MEST_STT_ID == expMesttSttId__Draft || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Request || serviceReq.EXP_MEST_STT_ID == expMesttSttId__Reject)
                    {
                        expMestMedicines = expMestMedicines.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }
                }
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PRESCRIPTION>(serviceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
