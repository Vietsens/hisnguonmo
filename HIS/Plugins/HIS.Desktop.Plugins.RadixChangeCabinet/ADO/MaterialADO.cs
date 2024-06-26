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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RadixChangeCabinet.ADO
{
    public class MaterialADO : HisMaterialTypeInStockSDO
    {
        public decimal? AMOUNT { get; set; }
        public decimal? EXP_MEST_ID { get; set; }
        public bool IsBoSung { get; set; }

        public MaterialADO()
        {

        }

        public MaterialADO(HisMaterialTypeInStockSDO data, List<HIS_EXP_MEST_MATY_REQ> _MatyReqs)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialADO>(this, data);
                    if (_MatyReqs != null && _MatyReqs.Count > 0)
                    {
                        var _dataOld = _MatyReqs.FirstOrDefault(p => p.MATERIAL_TYPE_ID == data.Id);
                        if (_dataOld != null)
                        {
                            this.AMOUNT = _dataOld.AMOUNT;
                            this.EXP_MEST_ID = _dataOld.EXP_MEST_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
