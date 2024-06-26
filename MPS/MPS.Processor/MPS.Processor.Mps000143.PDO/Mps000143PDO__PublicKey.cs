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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000143.PDO
{
    public partial class Mps000143PDO : RDOBase
    {
        public V_HIS_IMP_MEST _ChmsImpMest = null;
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials = null;
        public List<Mps000143ADO> _ListAdo = null;
        public Mps000143Key _mps000143Key = null;
        public int NumberDigit;

        public class Mps000143Key
        {
            public string EXP_MEDI_STOCK_NAME { get; set; }
            public string EXP_MEDI_STOCK_CODE { get; set; }
            public string KEY_NAME_TITLES { get; set; }
        }

        public class Mps000143ADO : V_HIS_IMP_MEST_MEDICINE
        {
            public string MEDI_MATE_TYPE_NAME { get; set; }
            public string EXPIRED_DATE_STR { get; set; }
            public long TYPE_ID { get; set; }
            public long MEDI_MATE_NUM_ORDER { get; set; }
            public string MEDI_MATE_TYPE_CODE { get; set; }

            public Mps000143ADO(V_HIS_IMP_MEST_MEDICINE medicine)
            {
                try
                {
                    if (medicine != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000143ADO>(this, medicine);
                        this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
                        this.TYPE_ID = 1;
                        this.MEDI_MATE_NUM_ORDER = medicine.MEDICINE_NUM_ORDER ?? 0;
                        this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }

            public Mps000143ADO(V_HIS_IMP_MEST_MATERIAL material)
            {
                try
                {
                    if (material != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000143ADO>(this, material);
                        this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
                        this.TYPE_ID = 2;
                        this.MEDI_MATE_NUM_ORDER = material.MEDICINE_NUM_ORDER ?? 0;
                        this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
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
