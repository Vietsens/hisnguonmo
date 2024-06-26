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
using MOS.SDO;

namespace MPS.Core.Mps000050
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000050RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines { get; set; }
        string bedRoomName;
        internal HisPrescriptionSDO HisPrescriptionSDO { get; set; }
        internal List<MedicineExpmestTypeADO> medicineExpmestTypeADOs { get; set; }
        internal string resultCreateMedicine;
        internal V_HIS_PRESCRIPTION vHisPrescription { get; set; }
        string mediStockExportName;

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000050RDO(
            string resultCreateMedicine,
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            TreatmentADO currentHisTreatment,
            HisPrescriptionSDO HisPrescriptionSDO,
            List<MedicineExpmestTypeADO> medicineExpmestTypeADOs,
            string bedRoomName,
            V_HIS_PRESCRIPTION vHisPrescription
            )
        {
            try
            {
                this.resultCreateMedicine = resultCreateMedicine;
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentHisTreatment = currentHisTreatment;
                this.medicineExpmestTypeADOs = medicineExpmestTypeADOs;
                this.bedRoomName = bedRoomName;
                this.HisPrescriptionSDO = HisPrescriptionSDO;
                this.vHisPrescription = vHisPrescription;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000050RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string mediStockExportName
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.vHisPrescription = serviceReq;
                this.expMestMedicines = expMestMedicines;
                this.mediStockExportName = mediStockExportName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000050RDO(
            PatientADO patient,
            PatyAlterBhytADO PatyAlterBhyt,
            V_HIS_PRESCRIPTION serviceReq,
            List<MPS.ADO.ExeExpMestMedicineSDO> expMestMedicines,
            string mediStockExportName, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.vHisPrescription = serviceReq;
                this.expMestMedicines = expMestMedicines;
                this.mediStockExportName = mediStockExportName;
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

                if (HisPrescriptionSDO != null)
                {
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.INTRUCTION_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisPrescriptionSDO.InstructionTime));
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.USE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisPrescriptionSDO.UseTime ?? 0));
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisPrescriptionSDO.UseTime ?? 0));
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.USE_DATE_FROM_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisPrescriptionSDO.InstructionTime));
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.USE_DATE_TO_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisPrescriptionSDO.UseTime??0));
                }
                if (vHisPrescription!= null)
                {
                    keyValues.Add(new KeyValue(Mps000050ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(vHisPrescription.USE_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000050ExtendSingleKey.USE_DATE_FROM_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(vHisPrescription.INTRUCTION_TIME)));
                    GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.USE_DATE_TO_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(vHisPrescription.USE_TIME ?? 0));

                    if (vHisPrescription.EXP_MEST_STT_ID == expMesttSttId__Approval || vHisPrescription.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        expMestMedicines = expMestMedicines.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (vHisPrescription.EXP_MEST_STT_ID == expMesttSttId__Draft || vHisPrescription.EXP_MEST_STT_ID == expMesttSttId__Request || vHisPrescription.EXP_MEST_STT_ID == expMesttSttId__Reject)
                    {
                        expMestMedicines = expMestMedicines.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                    }
                }

                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000050ExtendSingleKey.USE_DATE_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMestMedicines.Max(o=>o.USE_TIME_TO ?? 0))));
                }

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PRESCRIPTION>(vHisPrescription, keyValues, false);
                GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.EXP_MEST_CODE, resultCreateMedicine,false);
                GlobalQuery.AddSingeKey(keyValues, Mps000050ExtendSingleKey.BED_ROOM_NAME, bedRoomName);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);

                if (mediStockExportName!=null)
                    keyValues.Add(new KeyValue(Mps000050ExtendSingleKey.MEDI_STOCK_NAME, mediStockExportName));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
