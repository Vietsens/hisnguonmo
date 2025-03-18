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
using MPS.Processor.Mps000453.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000453
{
    public class Mps000453Processor : AbstractProcessor
    {
        Mps000453PDO rdo;
        public Mps000453Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000453PDO)rdoBase;
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
                //objectTag.AddObjectData(store, "ServiceReq", new List<V_HIS_SERVICE_REQ>() { rdo.HisServiceReq });
                objectTag.AddObjectData(store, "KskUnderEighteen", new List<HIS_KSK_UNDER_EIGHTEEN>() { rdo.HisKskUnderEighteen });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                objectTag.AddRelationship(store, "KskUnderEighteen", "Dhst", "DHST_ID", "ID");
                SetSingleKey();
                SetSignatureKeyImageByCFG();

                if (rdo.lstUneiVaty == null)
                {
                    rdo.lstUneiVaty = new List<HIS_KSK_UNEI_VATY>();
                }
           
                objectTag.AddObjectData(store, "VaccineType", rdo.lstVaccineType);
                objectTag.AddObjectData(store, "KskUneiVaty", rdo.lstUneiVaty);
                objectTag.AddRelationship(store, "KskUneiVaty", "VaccineType", "VACCINE_TYPE_ID", "ID");
                objectTag.AddObjectData(store, "ExamRank", rdo.examRank);

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
                if (rdo.HisKskUnderEighteen != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_UNDER_EIGHTEEN>(rdo.HisKskUnderEighteen, false);
                }
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                } if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.HisDhst != null)
                {
                    SetSingleKey((new KeyValue(Mps000453ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
