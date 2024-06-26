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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000462.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000462
{
    class Mps000462Processor : AbstractProcessor
    {
        Mps000462PDO rdo;
        public Mps000462Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000462PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                //SetBarcodeKey();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.V_HIS_ANTIBIOTIC_REQUEST__________", rdo.V_HIS_ANTIBIOTIC_REQUEST));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.HIS_ANTIBIOTIC_MICROBI__________", rdo.HIS_ANTIBIOTIC_MICROBI));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.HIS_ANTIBIOTIC_OLD_REG__________", rdo.HIS_ANTIBIOTIC_OLD_REG));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.HIS_ANTIBIOTIC_NEW_REG", rdo.V_HIS_ANTIBIOTIC_NEW_REG));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("rdo.HIS_DHST__________", rdo.HIS_DHST));
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "AntibioticMicrobi", rdo.HIS_ANTIBIOTIC_MICROBI);
                objectTag.AddObjectData(store, "AntibioticOldReg", rdo.HIS_ANTIBIOTIC_OLD_REG);
                objectTag.AddObjectData(store, "AntibioticNewReg", rdo.V_HIS_ANTIBIOTIC_NEW_REG);
                // barCodeTag.ProcessData(store, dicImage);
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
                AddObjectKeyIntoListkey<V_HIS_ANTIBIOTIC_REQUEST>(rdo.V_HIS_ANTIBIOTIC_REQUEST, false);
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.HIS_DHST, false);
                if (rdo.HIS_ANTIBIOTIC_MICROBI == null)
                {
                    rdo.HIS_ANTIBIOTIC_MICROBI = new List<HIS_ANTIBIOTIC_MICROBI>();
                }
                if (rdo.HIS_ANTIBIOTIC_OLD_REG == null)
                {
                    rdo.HIS_ANTIBIOTIC_OLD_REG = new List<HIS_ANTIBIOTIC_OLD_REG>();
                }
                if (rdo.V_HIS_ANTIBIOTIC_NEW_REG == null)
                {
                    rdo.V_HIS_ANTIBIOTIC_NEW_REG = new List<V_HIS_ANTIBIOTIC_NEW_REG>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
