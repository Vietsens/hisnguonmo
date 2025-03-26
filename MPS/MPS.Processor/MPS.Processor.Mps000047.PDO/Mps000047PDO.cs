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

namespace MPS.Processor.Mps000047.PDO
{
    public class Mps000047PDO : RDOBase
    {
        public List<Mps000047ADO> MedicineExpmestTypeADOs { get; set; }
        public V_HIS_EXP_MEST AggrExpMest { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms { get; set; }
        public List<V_HIS_BED_LOG> _listBedLog { get; set; }
        public long keyColumnSize { get; set; }
        public long TimeFilterOption { get; set; }

        public Mps000047PDO() { }

        public Mps000047PDO(
           List<Mps000047ADO> medicineExpmestTypeADOs,
           V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
           HIS_DEPARTMENT department,
            List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
            long _keyColumnSize
            )
        {
            try
            {
                this.MedicineExpmestTypeADOs = medicineExpmestTypeADOs;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this.vHisTreatmentBedRooms = vHisTreatmentBedRooms;
                keyColumnSize = _keyColumnSize;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000047PDO(
          List<Mps000047ADO> medicineExpmestTypeADOs,
          V_HIS_EXP_MEST aggrExpMest,
           List<HIS_EXP_MEST> _expMests_Print,
          HIS_DEPARTMENT department,
           List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
            List<V_HIS_BED_LOG> listBedLog,
            long _keyColumnSize
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
                keyColumnSize = _keyColumnSize;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000047PDO(
         List<Mps000047ADO> medicineExpmestTypeADOs,
         V_HIS_EXP_MEST aggrExpMest,
          List<HIS_EXP_MEST> _expMests_Print,
         HIS_DEPARTMENT department,
          List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
           List<V_HIS_BED_LOG> listBedLog,
           long _keyColumnSize,
           long _TimeFilterOption
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
                keyColumnSize = _keyColumnSize;
                this.TimeFilterOption = _TimeFilterOption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000047ADO
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
        public string CONCENTRA { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long REQ_ROOM_ID { get; set; }
        public string REQ_ROOM_NAME { get; set; }

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
        public long SERVICE_ID45 { get; set; }
        public long SERVICE_ID46 { get; set; }
        public long SERVICE_ID47 { get; set; }
        public long SERVICE_ID48 { get; set; }
        public long SERVICE_ID49 { get; set; }
        public long SERVICE_ID50 { get; set; }
        public long SERVICE_ID51 { get; set; }
        public long SERVICE_ID52 { get; set; }
        public long SERVICE_ID53 { get; set; }
        public long SERVICE_ID54 { get; set; }
        public long SERVICE_ID55 { get; set; }
        public long SERVICE_ID56 { get; set; }
        public long SERVICE_ID57 { get; set; }
        public long SERVICE_ID58 { get; set; }
        public long SERVICE_ID59 { get; set; }
        public long SERVICE_ID60 { get; set; }
        public long SERVICE_ID61 { get; set; }
        public long SERVICE_ID62 { get; set; }
        public long SERVICE_ID63 { get; set; }
        public long SERVICE_ID64 { get; set; }
        public long SERVICE_ID65 { get; set; }
        public long SERVICE_ID66 { get; set; }
        public long SERVICE_ID67 { get; set; }
        public long SERVICE_ID68 { get; set; }
        public long SERVICE_ID69 { get; set; }
        public long SERVICE_ID70 { get; set; }
        public long SERVICE_ID71 { get; set; }
        public long SERVICE_ID72 { get; set; }
        public long SERVICE_ID73 { get; set; }
        public long SERVICE_ID74 { get; set; }
        public long SERVICE_ID75 { get; set; }
        public long SERVICE_ID76 { get; set; }
        public long SERVICE_ID77 { get; set; }
        public long SERVICE_ID78 { get; set; }
        public long SERVICE_ID79 { get; set; }
        public long SERVICE_ID80 { get; set; }
        public long SERVICE_ID81 { get; set; }
        public long SERVICE_ID82 { get; set; }
        public long SERVICE_ID83 { get; set; }
        public long SERVICE_ID84 { get; set; }
        public long SERVICE_ID85 { get; set; }
        public long SERVICE_ID86 { get; set; }
        public long SERVICE_ID87 { get; set; }
        public long SERVICE_ID88 { get; set; }
        public long SERVICE_ID89 { get; set; }
        public long SERVICE_ID90 { get; set; }
        public long SERVICE_ID91 { get; set; }
        public long SERVICE_ID92 { get; set; }
        public long SERVICE_ID93 { get; set; }
        public long SERVICE_ID94 { get; set; }
        public long SERVICE_ID95 { get; set; }
        public long SERVICE_ID96 { get; set; }
        public long SERVICE_ID97 { get; set; }
        public long SERVICE_ID98 { get; set; }
        public long SERVICE_ID99 { get; set; }
        public long SERVICE_ID100 { get; set; }

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
        public string MEDICINE_TYPE_NAME45 { get; set; }
        public string MEDICINE_TYPE_NAME46 { get; set; }
        public string MEDICINE_TYPE_NAME47 { get; set; }
        public string MEDICINE_TYPE_NAME48 { get; set; }
        public string MEDICINE_TYPE_NAME49 { get; set; }
        public string MEDICINE_TYPE_NAME50 { get; set; }
        public string MEDICINE_TYPE_NAME51 { get; set; }
        public string MEDICINE_TYPE_NAME52 { get; set; }
        public string MEDICINE_TYPE_NAME53 { get; set; }
        public string MEDICINE_TYPE_NAME54 { get; set; }
        public string MEDICINE_TYPE_NAME55 { get; set; }
        public string MEDICINE_TYPE_NAME56 { get; set; }
        public string MEDICINE_TYPE_NAME57 { get; set; }
        public string MEDICINE_TYPE_NAME58 { get; set; }
        public string MEDICINE_TYPE_NAME59 { get; set; }
        public string MEDICINE_TYPE_NAME60 { get; set; }
        public string MEDICINE_TYPE_NAME61 { get; set; }
        public string MEDICINE_TYPE_NAME62 { get; set; }
        public string MEDICINE_TYPE_NAME63 { get; set; }
        public string MEDICINE_TYPE_NAME64 { get; set; }
        public string MEDICINE_TYPE_NAME65 { get; set; }
        public string MEDICINE_TYPE_NAME66 { get; set; }
        public string MEDICINE_TYPE_NAME67 { get; set; }
        public string MEDICINE_TYPE_NAME68 { get; set; }
        public string MEDICINE_TYPE_NAME69 { get; set; }
        public string MEDICINE_TYPE_NAME70 { get; set; }
        public string MEDICINE_TYPE_NAME71 { get; set; }
        public string MEDICINE_TYPE_NAME72 { get; set; }
        public string MEDICINE_TYPE_NAME73 { get; set; }
        public string MEDICINE_TYPE_NAME74 { get; set; }
        public string MEDICINE_TYPE_NAME75 { get; set; }
        public string MEDICINE_TYPE_NAME76 { get; set; }
        public string MEDICINE_TYPE_NAME77 { get; set; }
        public string MEDICINE_TYPE_NAME78 { get; set; }
        public string MEDICINE_TYPE_NAME79 { get; set; }
        public string MEDICINE_TYPE_NAME80 { get; set; }
        public string MEDICINE_TYPE_NAME81 { get; set; }
        public string MEDICINE_TYPE_NAME82 { get; set; }
        public string MEDICINE_TYPE_NAME83 { get; set; }
        public string MEDICINE_TYPE_NAME84 { get; set; }
        public string MEDICINE_TYPE_NAME85 { get; set; }
        public string MEDICINE_TYPE_NAME86 { get; set; }
        public string MEDICINE_TYPE_NAME87 { get; set; }
        public string MEDICINE_TYPE_NAME88 { get; set; }
        public string MEDICINE_TYPE_NAME89 { get; set; }
        public string MEDICINE_TYPE_NAME90 { get; set; }
        public string MEDICINE_TYPE_NAME91 { get; set; }
        public string MEDICINE_TYPE_NAME92 { get; set; }
        public string MEDICINE_TYPE_NAME93 { get; set; }
        public string MEDICINE_TYPE_NAME94 { get; set; }
        public string MEDICINE_TYPE_NAME95 { get; set; }
        public string MEDICINE_TYPE_NAME96 { get; set; }
        public string MEDICINE_TYPE_NAME97 { get; set; }
        public string MEDICINE_TYPE_NAME98 { get; set; }
        public string MEDICINE_TYPE_NAME99 { get; set; }
        public string MEDICINE_TYPE_NAME100 { get; set; }

        public string CONCENTRA1 { get; set; }
        public string CONCENTRA2 { get; set; }
        public string CONCENTRA3 { get; set; }
        public string CONCENTRA4 { get; set; }
        public string CONCENTRA5 { get; set; }
        public string CONCENTRA6 { get; set; }
        public string CONCENTRA7 { get; set; }
        public string CONCENTRA8 { get; set; }
        public string CONCENTRA9 { get; set; }
        public string CONCENTRA10 { get; set; }
        public string CONCENTRA11 { get; set; }
        public string CONCENTRA12 { get; set; }
        public string CONCENTRA13 { get; set; }
        public string CONCENTRA14 { get; set; }
        public string CONCENTRA15 { get; set; }
        public string CONCENTRA16 { get; set; }
        public string CONCENTRA17 { get; set; }
        public string CONCENTRA18 { get; set; }
        public string CONCENTRA19 { get; set; }
        public string CONCENTRA20 { get; set; }
        public string CONCENTRA21 { get; set; }
        public string CONCENTRA22 { get; set; }
        public string CONCENTRA23 { get; set; }
        public string CONCENTRA24 { get; set; }
        public string CONCENTRA25 { get; set; }
        public string CONCENTRA26 { get; set; }
        public string CONCENTRA27 { get; set; }
        public string CONCENTRA28 { get; set; }
        public string CONCENTRA29 { get; set; }
        public string CONCENTRA30 { get; set; }
        public string CONCENTRA31 { get; set; }
        public string CONCENTRA32 { get; set; }
        public string CONCENTRA33 { get; set; }
        public string CONCENTRA34 { get; set; }
        public string CONCENTRA35 { get; set; }
        public string CONCENTRA36 { get; set; }
        public string CONCENTRA37 { get; set; }
        public string CONCENTRA38 { get; set; }
        public string CONCENTRA39 { get; set; }
        public string CONCENTRA40 { get; set; }
        public string CONCENTRA41 { get; set; }
        public string CONCENTRA42 { get; set; }
        public string CONCENTRA43 { get; set; }
        public string CONCENTRA44 { get; set; }
        public string CONCENTRA45 { get; set; }
        public string CONCENTRA46 { get; set; }
        public string CONCENTRA47 { get; set; }
        public string CONCENTRA48 { get; set; }
        public string CONCENTRA49 { get; set; }
        public string CONCENTRA50 { get; set; }
        public string CONCENTRA51 { get; set; }
        public string CONCENTRA52 { get; set; }
        public string CONCENTRA53 { get; set; }
        public string CONCENTRA54 { get; set; }
        public string CONCENTRA55 { get; set; }
        public string CONCENTRA56 { get; set; }
        public string CONCENTRA57 { get; set; }
        public string CONCENTRA58 { get; set; }
        public string CONCENTRA59 { get; set; }
        public string CONCENTRA60 { get; set; }
        public string CONCENTRA61 { get; set; }
        public string CONCENTRA62 { get; set; }
        public string CONCENTRA63 { get; set; }
        public string CONCENTRA64 { get; set; }
        public string CONCENTRA65 { get; set; }
        public string CONCENTRA66 { get; set; }
        public string CONCENTRA67 { get; set; }
        public string CONCENTRA68 { get; set; }
        public string CONCENTRA69 { get; set; }
        public string CONCENTRA70 { get; set; }
        public string CONCENTRA71 { get; set; }
        public string CONCENTRA72 { get; set; }
        public string CONCENTRA73 { get; set; }
        public string CONCENTRA74 { get; set; }
        public string CONCENTRA75 { get; set; }
        public string CONCENTRA76 { get; set; }
        public string CONCENTRA77 { get; set; }
        public string CONCENTRA78 { get; set; }
        public string CONCENTRA79 { get; set; }
        public string CONCENTRA80 { get; set; }
        public string CONCENTRA81 { get; set; }
        public string CONCENTRA82 { get; set; }
        public string CONCENTRA83 { get; set; }
        public string CONCENTRA84 { get; set; }
        public string CONCENTRA85 { get; set; }
        public string CONCENTRA86 { get; set; }
        public string CONCENTRA87 { get; set; }
        public string CONCENTRA88 { get; set; }
        public string CONCENTRA89 { get; set; }
        public string CONCENTRA90 { get; set; }
        public string CONCENTRA91 { get; set; }
        public string CONCENTRA92 { get; set; }
        public string CONCENTRA93 { get; set; }
        public string CONCENTRA94 { get; set; }
        public string CONCENTRA95 { get; set; }
        public string CONCENTRA96 { get; set; }
        public string CONCENTRA97 { get; set; }
        public string CONCENTRA98 { get; set; }
        public string CONCENTRA99 { get; set; }
        public string CONCENTRA100 { get; set; }

        public string MEDICINE_USE_FORM_NAME1 { get; set; }
        public string MEDICINE_USE_FORM_NAME2 { get; set; }
        public string MEDICINE_USE_FORM_NAME3 { get; set; }
        public string MEDICINE_USE_FORM_NAME4 { get; set; }
        public string MEDICINE_USE_FORM_NAME5 { get; set; }
        public string MEDICINE_USE_FORM_NAME6 { get; set; }
        public string MEDICINE_USE_FORM_NAME7 { get; set; }
        public string MEDICINE_USE_FORM_NAME8 { get; set; }
        public string MEDICINE_USE_FORM_NAME9 { get; set; }
        public string MEDICINE_USE_FORM_NAME10 { get; set; }
        public string MEDICINE_USE_FORM_NAME11 { get; set; }
        public string MEDICINE_USE_FORM_NAME12 { get; set; }
        public string MEDICINE_USE_FORM_NAME13 { get; set; }
        public string MEDICINE_USE_FORM_NAME14 { get; set; }
        public string MEDICINE_USE_FORM_NAME15 { get; set; }
        public string MEDICINE_USE_FORM_NAME16 { get; set; }
        public string MEDICINE_USE_FORM_NAME17 { get; set; }
        public string MEDICINE_USE_FORM_NAME18 { get; set; }
        public string MEDICINE_USE_FORM_NAME19 { get; set; }
        public string MEDICINE_USE_FORM_NAME20 { get; set; }
        public string MEDICINE_USE_FORM_NAME21 { get; set; }
        public string MEDICINE_USE_FORM_NAME22 { get; set; }
        public string MEDICINE_USE_FORM_NAME23 { get; set; }
        public string MEDICINE_USE_FORM_NAME24 { get; set; }
        public string MEDICINE_USE_FORM_NAME25 { get; set; }
        public string MEDICINE_USE_FORM_NAME26 { get; set; }
        public string MEDICINE_USE_FORM_NAME27 { get; set; }
        public string MEDICINE_USE_FORM_NAME28 { get; set; }
        public string MEDICINE_USE_FORM_NAME29 { get; set; }
        public string MEDICINE_USE_FORM_NAME30 { get; set; }
        public string MEDICINE_USE_FORM_NAME31 { get; set; }
        public string MEDICINE_USE_FORM_NAME32 { get; set; }
        public string MEDICINE_USE_FORM_NAME33 { get; set; }
        public string MEDICINE_USE_FORM_NAME34 { get; set; }
        public string MEDICINE_USE_FORM_NAME35 { get; set; }
        public string MEDICINE_USE_FORM_NAME36 { get; set; }
        public string MEDICINE_USE_FORM_NAME37 { get; set; }
        public string MEDICINE_USE_FORM_NAME38 { get; set; }
        public string MEDICINE_USE_FORM_NAME39 { get; set; }
        public string MEDICINE_USE_FORM_NAME40 { get; set; }
        public string MEDICINE_USE_FORM_NAME41 { get; set; }
        public string MEDICINE_USE_FORM_NAME42 { get; set; }
        public string MEDICINE_USE_FORM_NAME43 { get; set; }
        public string MEDICINE_USE_FORM_NAME44 { get; set; }
        public string MEDICINE_USE_FORM_NAME45 { get; set; }
        public string MEDICINE_USE_FORM_NAME46 { get; set; }
        public string MEDICINE_USE_FORM_NAME47 { get; set; }
        public string MEDICINE_USE_FORM_NAME48 { get; set; }
        public string MEDICINE_USE_FORM_NAME49 { get; set; }
        public string MEDICINE_USE_FORM_NAME50 { get; set; }
        public string MEDICINE_USE_FORM_NAME51 { get; set; }
        public string MEDICINE_USE_FORM_NAME52 { get; set; }
        public string MEDICINE_USE_FORM_NAME53 { get; set; }
        public string MEDICINE_USE_FORM_NAME54 { get; set; }
        public string MEDICINE_USE_FORM_NAME55 { get; set; }
        public string MEDICINE_USE_FORM_NAME56 { get; set; }
        public string MEDICINE_USE_FORM_NAME57 { get; set; }
        public string MEDICINE_USE_FORM_NAME58 { get; set; }
        public string MEDICINE_USE_FORM_NAME59 { get; set; }
        public string MEDICINE_USE_FORM_NAME60 { get; set; }
        public string MEDICINE_USE_FORM_NAME61 { get; set; }
        public string MEDICINE_USE_FORM_NAME62 { get; set; }
        public string MEDICINE_USE_FORM_NAME63 { get; set; }
        public string MEDICINE_USE_FORM_NAME64 { get; set; }
        public string MEDICINE_USE_FORM_NAME65 { get; set; }
        public string MEDICINE_USE_FORM_NAME66 { get; set; }
        public string MEDICINE_USE_FORM_NAME67 { get; set; }
        public string MEDICINE_USE_FORM_NAME68 { get; set; }
        public string MEDICINE_USE_FORM_NAME69 { get; set; }
        public string MEDICINE_USE_FORM_NAME70 { get; set; }
        public string MEDICINE_USE_FORM_NAME71 { get; set; }
        public string MEDICINE_USE_FORM_NAME72 { get; set; }
        public string MEDICINE_USE_FORM_NAME73 { get; set; }
        public string MEDICINE_USE_FORM_NAME74 { get; set; }
        public string MEDICINE_USE_FORM_NAME75 { get; set; }
        public string MEDICINE_USE_FORM_NAME76 { get; set; }
        public string MEDICINE_USE_FORM_NAME77 { get; set; }
        public string MEDICINE_USE_FORM_NAME78 { get; set; }
        public string MEDICINE_USE_FORM_NAME79 { get; set; }
        public string MEDICINE_USE_FORM_NAME80 { get; set; }
        public string MEDICINE_USE_FORM_NAME81 { get; set; }
        public string MEDICINE_USE_FORM_NAME82 { get; set; }
        public string MEDICINE_USE_FORM_NAME83 { get; set; }
        public string MEDICINE_USE_FORM_NAME84 { get; set; }
        public string MEDICINE_USE_FORM_NAME85 { get; set; }
        public string MEDICINE_USE_FORM_NAME86 { get; set; }
        public string MEDICINE_USE_FORM_NAME87 { get; set; }
        public string MEDICINE_USE_FORM_NAME88 { get; set; }
        public string MEDICINE_USE_FORM_NAME89 { get; set; }
        public string MEDICINE_USE_FORM_NAME90 { get; set; }
        public string MEDICINE_USE_FORM_NAME91 { get; set; }
        public string MEDICINE_USE_FORM_NAME92 { get; set; }
        public string MEDICINE_USE_FORM_NAME93 { get; set; }
        public string MEDICINE_USE_FORM_NAME94 { get; set; }
        public string MEDICINE_USE_FORM_NAME95 { get; set; }
        public string MEDICINE_USE_FORM_NAME96 { get; set; }
        public string MEDICINE_USE_FORM_NAME97 { get; set; }
        public string MEDICINE_USE_FORM_NAME98 { get; set; }
        public string MEDICINE_USE_FORM_NAME99 { get; set; }
        public string MEDICINE_USE_FORM_NAME100 { get; set; }
        public List<ExpMestAggregatePrintADO> ExpMestAggregatePrintADOs { get; set; }
        public List<ExpMestAggregatePrintADO> ExpMestAggregateReqRoomPrintADOs { get; set; }
    }

    public class ExpMestAggregatePrintADO
    {
        public string REQ_ROOM_NAME { get; set; }
        public long PATIENT_ID { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public string PATIENT_CODE { get; set; }
        public string AGE { get; set; }
        public long DOB { get; set; }
        public string GENDER_NAME { get; set; }
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
        public decimal? AMOUNT45 { get; set; }
        public decimal? AMOUNT46 { get; set; }
        public decimal? AMOUNT47 { get; set; }
        public decimal? AMOUNT48 { get; set; }
        public decimal? AMOUNT49 { get; set; }
        public decimal? AMOUNT50 { get; set; }
        public decimal? AMOUNT51 { get; set; }
        public decimal? AMOUNT52 { get; set; }
        public decimal? AMOUNT53 { get; set; }
        public decimal? AMOUNT54 { get; set; }
        public decimal? AMOUNT55 { get; set; }
        public decimal? AMOUNT56 { get; set; }
        public decimal? AMOUNT57 { get; set; }
        public decimal? AMOUNT58 { get; set; }
        public decimal? AMOUNT59 { get; set; }
        public decimal? AMOUNT60 { get; set; }
        public decimal? AMOUNT61 { get; set; }
        public decimal? AMOUNT62 { get; set; }
        public decimal? AMOUNT63 { get; set; }
        public decimal? AMOUNT64 { get; set; }
        public decimal? AMOUNT65 { get; set; }
        public decimal? AMOUNT66 { get; set; }
        public decimal? AMOUNT67 { get; set; }
        public decimal? AMOUNT68 { get; set; }
        public decimal? AMOUNT69 { get; set; }
        public decimal? AMOUNT70 { get; set; }
        public decimal? AMOUNT71 { get; set; }
        public decimal? AMOUNT72 { get; set; }
        public decimal? AMOUNT73 { get; set; }
        public decimal? AMOUNT74 { get; set; }
        public decimal? AMOUNT75 { get; set; }
        public decimal? AMOUNT76 { get; set; }
        public decimal? AMOUNT77 { get; set; }
        public decimal? AMOUNT78 { get; set; }
        public decimal? AMOUNT79 { get; set; }
        public decimal? AMOUNT80 { get; set; }
        public decimal? AMOUNT81 { get; set; }
        public decimal? AMOUNT82 { get; set; }
        public decimal? AMOUNT83 { get; set; }
        public decimal? AMOUNT84 { get; set; }
        public decimal? AMOUNT85 { get; set; }
        public decimal? AMOUNT86 { get; set; }
        public decimal? AMOUNT87 { get; set; }
        public decimal? AMOUNT88 { get; set; }
        public decimal? AMOUNT89 { get; set; }
        public decimal? AMOUNT90 { get; set; }
        public decimal? AMOUNT91 { get; set; }
        public decimal? AMOUNT92 { get; set; }
        public decimal? AMOUNT93 { get; set; }
        public decimal? AMOUNT94 { get; set; }
        public decimal? AMOUNT95 { get; set; }
        public decimal? AMOUNT96 { get; set; }
        public decimal? AMOUNT97 { get; set; }
        public decimal? AMOUNT98 { get; set; }
        public decimal? AMOUNT99 { get; set; }
        public decimal? AMOUNT100 { get; set; }
    }
}

