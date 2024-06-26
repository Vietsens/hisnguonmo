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

namespace MPS.Processor.Mps000149.PDO
{
    public partial class Mps000149PDO : RDOBase
    {
        public V_HIS_IMP_MEST _ManuImpMest = null;
        public List<V_HIS_IMP_MEST_BLOOD> _ListImpMestBlood = null;
        public List<Mps000149ADO> _ListAdo = new List<Mps000149ADO>();

        public class Mps000149ADO : V_HIS_IMP_MEST_BLOOD
        {
            public decimal AMOUNT { get; set; }
            public decimal PRICE { get; set; }
            public string PRICE_SEPARATE { get; set; }

            public Mps000149ADO() { }

            public Mps000149ADO(V_HIS_IMP_MEST_BLOOD impBlood)
            {
                try
                {
                    if (impBlood != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000149ADO>(this, impBlood);
                        this.AMOUNT = 1;
                        this.PRICE = impBlood.IMP_PRICE;
                        this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
