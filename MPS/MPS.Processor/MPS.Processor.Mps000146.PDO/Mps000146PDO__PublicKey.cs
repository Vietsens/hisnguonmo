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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000146.PDO
{
    public partial class Mps000146PDO : RDOBase
    {
        public V_HIS_INFUSION_SUM _InfusionSum = null;
        public V_HIS_TREATMENT_2 _Treatment = null;
        public List<V_HIS_INFUSION> _ListInfusion = null;
        public List<V_HIS_MEDICINE_TYPE> _ListMedicineType = null;
        public HIS_MEDICINE HisMedicine = null;

        public List<Mps000146ADO> _ListAdo = new List<Mps000146ADO>();
        public List<Mps000146ADO> _ListMixedMedicines = new List<Mps000146ADO>();


        public class Mps000146ADO : V_HIS_INFUSION
        {
            public string START_TIME_STR { get; set; }
            public string FINISH_TIME_STR { get; set; }
            public string CREATE_TIME_STR { get; set; }
            public string EXECUTE_DEPARTMENT_NAME { get; set; }
            public string EXECUTE_ROOM_NAME { get; set; }
            public string EXECUTE_DEPARTMENT_CODE { get; set; }
            public string EXECUTE_ROOM_CODE { get; set; }
            public string CONCENTRA { get; set; }
            public string MEDICINE_TYPE_CODE { get; set; }
            public string MIXED_MEDICINE { get; set; }
            //
            public long INFUSION_ID { get; set; }
            public string PACKAGE_NUMBER { get; set; }
            public decimal? VOLUME { get; set; }
            public string MEDICINE_TYPE_NAME { get; set; }
            public string SERVICE_UNIT_NAME { get; set; }
            //public decimal AMOUNT_TEST { get; set; }
            public string MODIFY_TIME_STR { get; set; }

            public Mps000146ADO() { }

            public Mps000146ADO(V_HIS_INFUSION data)
            {
                try
                {
                    if (data != null)
                    {

                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000146ADO>(this, data);
                        this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CREATE_TIME ?? 0);
                        if (this.START_TIME.HasValue)
                        {
                            this.START_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.START_TIME.Value);
                        }

                        if (this.FINISH_TIME.HasValue)
                        {
                            this.FINISH_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.FINISH_TIME.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }

            public Mps000146ADO(HIS_MIXED_MEDICINE data)
            {
                try
                {
                    if (data != null)
                    {
                        this.INFUSION_ID = data.INFUSION_ID;
                        this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                        this.VOLUME = data.VOLUME;
                        this.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        this.AMOUNT = data.AMOUNT ?? 0;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

    }
}
