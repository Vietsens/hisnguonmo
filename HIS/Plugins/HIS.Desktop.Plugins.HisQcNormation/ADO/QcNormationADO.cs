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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisQcNormation.ADO
{
    public class QcNormationADO : HIS_QC_NORMATION
    {
        public decimal? Amount { get; set; }
        public string MaterialTypeCode { get; set; }
        public string ServiceUnitName { get; set; }

        public QcNormationADO() { }

        public QcNormationADO(HIS_QC_NORMATION dataQc, List<MaterialTypeADO> lsttype)
        {
            if (dataQc != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<QcNormationADO>(this, dataQc);

                var MaterialADO = lsttype.FirstOrDefault(o => o.ID == dataQc.MATERIAL_TYPE_ID);

                if (dataQc.AMOUNT > 0)
                {
                    this.Amount = dataQc.AMOUNT;
                }
                else
                {
                    this.Amount = null;
                }

                if (MaterialADO != null)
                {
                    this.MaterialTypeCode = MaterialADO.MATERIAL_TYPE_CODE;
                    var ServiceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == MaterialADO.TDL_SERVICE_UNIT_ID);

                    if (ServiceUnit != null)
                    {
                        this.ServiceUnitName = ServiceUnit.SERVICE_UNIT_NAME;
                    }
                }
            }
        }

        //public QcNormationADO(HIS_QC_NORMATION dataQc, HIS_MATERIAL_TYPE type, HIS_SERVICE_UNIT unit)
        //{
        //    if (dataQc != null)
        //    {
        //        Inventec.Common.Mapper.DataObjectMapper.Map<QcNormationADO>(this, dataQc);

        //        if (dataQc.AMOUNT > 0)
        //        {
        //            this.Amount = dataQc.AMOUNT;
        //        }
        //        else
        //        {
        //            this.Amount = null;
        //        }

        //        if (type != null)
        //        {
        //            this.MaterialTypeCode = type.MATERIAL_TYPE_CODE;
        //        }

        //        if (unit != null)
        //        {
        //            this.ServiceUnitName = unit.SERVICE_UNIT_NAME;
        //        }
        //    }
        //}
    }
}
