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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000372.PDO
{
    public class Mps000372PDO : RDOBase 
    {
        public List<Mps000372ADO> MedicineExpmestTypeADOs { get; set; }
        public V_HIS_EXP_MEST AggrExpMest { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms { get; set; }
        public List<V_HIS_BED_LOG> _listBedLog { get; set; }

        public Mps000372PDO(
           List<Mps000372ADO> medicineExpmestTypeADOs,
           V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
           HIS_DEPARTMENT department,
            List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms
            )
        {
            try
            {
                this.MedicineExpmestTypeADOs = medicineExpmestTypeADOs;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.vHisTreatmentBedRooms = vHisTreatmentBedRooms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000372PDO(
          List<Mps000372ADO> medicineExpmestTypeADOs,
          V_HIS_EXP_MEST aggrExpMest,
           List<HIS_EXP_MEST> _expMests_Print,
          HIS_DEPARTMENT department,
           List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
            List<V_HIS_BED_LOG> listBedLog
           )
        {
            try
            {
                this.MedicineExpmestTypeADOs = medicineExpmestTypeADOs;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.vHisTreatmentBedRooms = vHisTreatmentBedRooms;
                this._listBedLog = listBedLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }

    public class Mps000372ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public long SERVICE_ID { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string IS_EXPEND_DISPLAY { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public long? NUM_ORDER { get; set; }

        public decimal AMOUNT_EXPORTED { get; set; }
        public decimal AMOUNT_EXCUTE { get; set; }
        public decimal AMOUNT { get; set; }

        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }

        public long MEDI_MATE_NUM_ORDER { get; set; }


        public V_HIS_PATIENT Patient { get; set; }
        public long TreatmentId { get; set; }
        public string TREATMENT_CODE { get; set; }


    }

    public class ExpMestAggregatePrintByPageADO
    {
        public long SERVICE_ID1 { get; set; }
        public long SERVICE_ID2 { get; set; }
        public long SERVICE_ID3 { get; set; }
        public long SERVICE_ID4 { get; set; }
        public long SERVICE_ID5 { get; set; }
        public long SERVICE_ID6 { get; set; }
        public long SERVICE_ID7 { get; set; }
        public long SERVICE_ID8 { get; set; }
        public long SERVICE_ID9 { get; set; }
        public long SERVICE_ID10 { get; set; }
        public long SERVICE_ID11 { get; set; }
        public long SERVICE_ID12 { get; set; }
        public long SERVICE_ID13 { get; set; }
        public long SERVICE_ID14 { get; set; }
        public long SERVICE_ID15 { get; set; }
        public long SERVICE_ID16 { get; set; }
        public long SERVICE_ID17 { get; set; }
        public long SERVICE_ID18 { get; set; }
        public long SERVICE_ID19 { get; set; }
        public long SERVICE_ID20 { get; set; }
        public long SERVICE_ID21 { get; set; }
        public long SERVICE_ID22 { get; set; }
        public long SERVICE_ID23 { get; set; }
        public long SERVICE_ID24 { get; set; }
        public long SERVICE_ID25 { get; set; }
        public long SERVICE_ID26 { get; set; }
        public long SERVICE_ID27 { get; set; }
        public long SERVICE_ID28 { get; set; }
        public long SERVICE_ID29 { get; set; }
        public long SERVICE_ID30 { get; set; }
        public long SERVICE_ID31 { get; set; }
        public long SERVICE_ID32 { get; set; }
        public long SERVICE_ID33 { get; set; }
        public long SERVICE_ID34 { get; set; }
        public long SERVICE_ID35 { get; set; }
        public long SERVICE_ID36 { get; set; }
        public long SERVICE_ID37 { get; set; }
        public long SERVICE_ID38 { get; set; }
        public long SERVICE_ID39 { get; set; }
        public long SERVICE_ID40 { get; set; }
        public long SERVICE_ID41 { get; set; }
        public long SERVICE_ID42 { get; set; }
        public long SERVICE_ID43 { get; set; }
        public long SERVICE_ID44 { get; set; }

        public string MEDICINE_TYPE_NAME1 { get; set; }
        public string MEDICINE_TYPE_NAME2 { get; set; }
        public string MEDICINE_TYPE_NAME3 { get; set; }
        public string MEDICINE_TYPE_NAME4 { get; set; }
        public string MEDICINE_TYPE_NAME5 { get; set; }
        public string MEDICINE_TYPE_NAME6 { get; set; }
        public string MEDICINE_TYPE_NAME7 { get; set; }
        public string MEDICINE_TYPE_NAME8 { get; set; }
        public string MEDICINE_TYPE_NAME9 { get; set; }
        public string MEDICINE_TYPE_NAME10 { get; set; }
        public string MEDICINE_TYPE_NAME11 { get; set; }
        public string MEDICINE_TYPE_NAME12 { get; set; }
        public string MEDICINE_TYPE_NAME13 { get; set; }
        public string MEDICINE_TYPE_NAME14 { get; set; }
        public string MEDICINE_TYPE_NAME15 { get; set; }
        public string MEDICINE_TYPE_NAME16 { get; set; }
        public string MEDICINE_TYPE_NAME17 { get; set; }
        public string MEDICINE_TYPE_NAME18 { get; set; }
        public string MEDICINE_TYPE_NAME19 { get; set; }
        public string MEDICINE_TYPE_NAME20 { get; set; }
        public string MEDICINE_TYPE_NAME21 { get; set; }
        public string MEDICINE_TYPE_NAME22 { get; set; }
        public string MEDICINE_TYPE_NAME23 { get; set; }
        public string MEDICINE_TYPE_NAME24 { get; set; }
        public string MEDICINE_TYPE_NAME25 { get; set; }
        public string MEDICINE_TYPE_NAME26 { get; set; }
        public string MEDICINE_TYPE_NAME27 { get; set; }
        public string MEDICINE_TYPE_NAME28 { get; set; }
        public string MEDICINE_TYPE_NAME29 { get; set; }
        public string MEDICINE_TYPE_NAME30 { get; set; }
        public string MEDICINE_TYPE_NAME31 { get; set; }
        public string MEDICINE_TYPE_NAME32 { get; set; }
        public string MEDICINE_TYPE_NAME33 { get; set; }
        public string MEDICINE_TYPE_NAME34 { get; set; }
        public string MEDICINE_TYPE_NAME35 { get; set; }
        public string MEDICINE_TYPE_NAME36 { get; set; }
        public string MEDICINE_TYPE_NAME37 { get; set; }
        public string MEDICINE_TYPE_NAME38 { get; set; }
        public string MEDICINE_TYPE_NAME39 { get; set; }
        public string MEDICINE_TYPE_NAME40 { get; set; }
        public string MEDICINE_TYPE_NAME41 { get; set; }
        public string MEDICINE_TYPE_NAME42 { get; set; }
        public string MEDICINE_TYPE_NAME43 { get; set; }
        public string MEDICINE_TYPE_NAME44 { get; set; }

        public List<ExpMestAggregatePrintADO> ExpMestAggregatePrintADOs { get; set; }
    }

    public class ExpMestAggregatePrintADO
    {
        public long PATIENT_ID { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public string PATIENT_CODE { get; set; }
        public string AGE { get; set; }
        public string BED_ROOM_NAMEs { get; set; }
        public long MEDICINE_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string BED_CODE { get; set; }
        public string BED_NAME { get; set; }
        public decimal? AMOUNT1 { get; set; }
        public decimal? AMOUNT2 { get; set; }
        public decimal? AMOUNT3 { get; set; }
        public decimal? AMOUNT4 { get; set; }
        public decimal? AMOUNT5 { get; set; }
        public decimal? AMOUNT6 { get; set; }
        public decimal? AMOUNT7 { get; set; }
        public decimal? AMOUNT8 { get; set; }
        public decimal? AMOUNT9 { get; set; }
        public decimal? AMOUNT10 { get; set; }
        public decimal? AMOUNT11 { get; set; }
        public decimal? AMOUNT12 { get; set; }
        public decimal? AMOUNT13 { get; set; }
        public decimal? AMOUNT14 { get; set; }
        public decimal? AMOUNT15 { get; set; }
        public decimal? AMOUNT16 { get; set; }
        public decimal? AMOUNT17 { get; set; }
        public decimal? AMOUNT18 { get; set; }
        public decimal? AMOUNT19 { get; set; }
        public decimal? AMOUNT20 { get; set; }
        public decimal? AMOUNT21 { get; set; }
        public decimal? AMOUNT22 { get; set; }
        public decimal? AMOUNT23 { get; set; }
        public decimal? AMOUNT24 { get; set; }
        public decimal? AMOUNT25 { get; set; }
        public decimal? AMOUNT26 { get; set; }
        public decimal? AMOUNT27 { get; set; }
        public decimal? AMOUNT28 { get; set; }
        public decimal? AMOUNT29 { get; set; }
        public decimal? AMOUNT30 { get; set; }
        public decimal? AMOUNT31 { get; set; }
        public decimal? AMOUNT32 { get; set; }
        public decimal? AMOUNT33 { get; set; }
        public decimal? AMOUNT34 { get; set; }
        public decimal? AMOUNT35 { get; set; }
        public decimal? AMOUNT36 { get; set; }
        public decimal? AMOUNT37 { get; set; }
        public decimal? AMOUNT38 { get; set; }
        public decimal? AMOUNT39 { get; set; }
        public decimal? AMOUNT40 { get; set; }
        public decimal? AMOUNT41 { get; set; }
        public decimal? AMOUNT42 { get; set; }
        public decimal? AMOUNT43 { get; set; }
        public decimal? AMOUNT44 { get; set; }
    }
}

