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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000454.PDO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000454.ADO;
namespace MPS.Processor.Mps000454
{
    public class Mps000454Processor : AbstractProcessor
    {
        List<KskDriverDityADO> lstADO = new List<KskDriverDityADO>();
        List<KskDriverDityADO> lstFullADO = new List<KskDriverDityADO>();
        Mps000454PDO rdo;
        public Mps000454Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000454PDO)rdoBase;
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
                //objectTag.AddObjectData(store, "KskPeriodDriver", new List<HIS_KSK_PERIOD_DRIVER>() { rdo.HisKskPeriodDriver });

                SetSingleKey();
                SetSignatureKeyImageByCFG();
                objectTag.AddObjectData(store, "ExamRank", rdo.examRank);
                if (rdo.lstDriverDity == null)
                {
                    rdo.lstDriverDity = new List<HIS_PERIOD_DRIVER_DITY>();
                }
                SetData();
                objectTag.AddObjectData(store, "KskDriverDity", lstADO);
                objectTag.AddObjectData(store, "KskDriverDityFull", lstFullADO);
                objectTag.AddObjectData(store, "DiseaseType", rdo.lstDiseaseType);
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

        private void SetData()
        {
            try
            {
                int rowContTable1 = 0;

                if (rdo.lstDriverDity.Count % 2 == 0)
                {
                    rowContTable1 = rdo.lstDriverDity.Count / 2;

                }
                else
                {
                    rowContTable1 = rdo.lstDriverDity.Count / 2 + 1;
                }

                for (int i = 0; i < rdo.lstDriverDity.Count; i++)
                {
                    KskDriverDityADO ado = new KskDriverDityADO();
                    var name = rdo.lstDiseaseType.Where(o => o.ID == rdo.lstDriverDity[i].DISEASE_TYPE_ID).First().DISEASE_TYPE_NAME;
                    string y = "";
                    string n = "";
                    if (rdo.lstDriverDity[i].IS_YES_NO == "1")
                    {
                        y = "X";
                    }
                    else if (rdo.lstDriverDity[i].IS_YES_NO == "0")
                    {
                        n = "X";
                    }
                    ado.NAME_DITY = name;
                    ado.IS_YES = y;
                    ado.IS_NO = n;

                    int indexr = i % rowContTable1;

                    if (lstADO.Count == indexr)
                    {
                        ado.NAME_DITY_1 = name;
                        ado.IS_YES_1 = y;
                        ado.IS_NO_1 = n;
                        lstADO.Add(ado);
                    }
                    else
                    {
                        lstADO[indexr].NAME_DITY_2 = name;
                        lstADO[indexr].IS_YES_2 = y;
                        lstADO[indexr].IS_NO_2 = n;
                    }
                    lstFullADO.Add(ado);

                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisKskPeriodDriver != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_PERIOD_DRIVER>(rdo.HisKskPeriodDriver, false);
                }
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
