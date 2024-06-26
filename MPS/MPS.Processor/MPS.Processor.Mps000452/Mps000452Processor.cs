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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000452.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000452
{
    public class Mps000452Processor : AbstractProcessor
    {
        Mps000452PDO rdo;
        public Mps000452Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000452PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                //objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                //objectTag.AddObjectData(store, "KskOverEighteen", new List<HIS_KSK_OVER_EIGHTEEN>() { rdo.HisKskOverEighteen });
                //objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                objectTag.AddObjectData(store, "ExamRank",  rdo.examRank );
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisKskOverEighteen != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_OVER_EIGHTEEN>(rdo.HisKskOverEighteen, false);
                }
                 if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                } if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

       
    }
}
