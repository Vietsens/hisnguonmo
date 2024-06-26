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

namespace MPS.Core.Mps000043
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000043RDO : RDOBase
    {
        //internal MPS.ADO.HisServiceReqCombo hisServiceReqCombo { get; set; }
        //internal List<MPS.ADO.SereServGroupPlusSDO> sereServs_All { get; set; }
        //internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> lstSereServResult { get; set; }
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string bebRoomName;
        //internal string departmentName;
        internal string resultCreateMedicine;
        internal List<MedicineExpmestTypeADO> lstMedicineExpmestTypeADO { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_DEPARTMENT department { get; set; }
        internal HisPrescriptionSDO HisPrescriptionSDO { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_ICD dataIcd { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_DHST dhst { get; set; }
        internal string mediStockExportName { get; set; }
        internal V_HIS_TREATMENT treatment { get; set; }

        public Mps000043RDO(
           string resultCreateMedicine,
           List<MedicineExpmestTypeADO> lstMedicineExpmestTypeADO,
            HisPrescriptionSDO HisPrescriptionSDO,
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
           MOS.EFMODEL.DataModels.HIS_DEPARTMENT department,
            MOS.EFMODEL.DataModels.HIS_ICD dataIcd,
            MOS.EFMODEL.DataModels.HIS_DHST dhst,
            string bebRoomName,
            string mediStockExportName,
            V_HIS_TREATMENT treatment
            )
        {
            try
            {
                this.resultCreateMedicine = resultCreateMedicine;
                this.lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO;
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.department = department;
                this.bebRoomName = bebRoomName;
                this.HisPrescriptionSDO = HisPrescriptionSDO;
                this.dataIcd = dataIcd;
                this.dhst = dhst;
                this.mediStockExportName = mediStockExportName;
                this.treatment = treatment;
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


                keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.EXP_MEST_CODE, resultCreateMedicine));
                if (mediStockExportName!=null)
                    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.MEDI_STOCK_NAME, mediStockExportName));
                keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.BED_ROOM_NAME, bebRoomName));
                if (HisPrescriptionSDO != null)
                {
                    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.REQ_USERNAME, this.HisPrescriptionSDO.ExpMest.REQ_USERNAME));
                    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.ICD_TEXT, this.HisPrescriptionSDO.IcdText));
                    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.ADVISE, this.HisPrescriptionSDO.Advise));
                    //keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.USE_TIME_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.HisPrescriptionSDO.PrescriptionMedicines.Max(o => o.UseTimeTo)??0)));
                    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.HisPrescriptionSDO.UseTime??0)));
                }

                if (dataIcd != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_ICD>(dataIcd, keyValues);
                }

                if (dhst != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(dhst, keyValues);
                }

                //if (hisServiceReqCombo != null)
                //{
                //    keyValues.Add(new KeyValue(Mps000043ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(hisServiceReqCombo.INTRUCTION_TIME)));

                //    GlobalQuery.AddObjectKeyIntoListkey<HisServiceReqCombo>(hisServiceReqCombo, keyValues, false);
                //}
                if (HisPrescriptionSDO != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HisPrescriptionSDO>(HisPrescriptionSDO, keyValues, false);
                }
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_DEPARTMENT>(department, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
