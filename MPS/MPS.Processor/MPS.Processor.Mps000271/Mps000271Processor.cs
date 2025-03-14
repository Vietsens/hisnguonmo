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
using FlexCel.Report;
using HID.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000271.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using SCN.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000271
{
    public partial class Mps000271Processor : AbstractProcessor
    {
        Mps000271PDO rdo;
        public Mps000271Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000271PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                if (rdo.mps000271Ado != null && rdo.mps000271Ado.transfusions != null && rdo.mps000271Ado.transfusions.Count > 0)
                {
                    rdo.mps000271Ado.transfusions = rdo.mps000271Ado.transfusions.OrderBy(o => o.MEASURE_TIME).ToList();
                }
                objectTag.AddObjectData(store, "ListTransfusion", rdo.mps000271Ado.transfusions);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.mps000271Ado != null && rdo.mps000271Ado.treatment != null)
                {
                    // Mã biểu in
                    string printCode = "Mps000271";
                    string treatmentCode = "TREATMENT_CODE:" +
                        (rdo.mps000271Ado.treatment.TREATMENT_CODE != null ? rdo.mps000271Ado.treatment.TREATMENT_CODE : "");
                    string expMestCode = "EXP_MEST_CODE:" +
                        (rdo.mps000271Ado.expMestBlood != null && rdo.mps000271Ado.expMestBlood.EXP_MEST_CODE != null
                        ? rdo.mps000271Ado.expMestBlood.EXP_MEST_CODE
                        : "");
                    List<string> transfusionDetails = new List<string>();
                    if (rdo.mps000271Ado.transfusions != null && rdo.mps000271Ado.transfusions.Count > 0)
                    {
                        transfusionDetails = rdo.mps000271Ado.transfusions
                            .OrderBy(t => t.ID)
                            .Select(t => "HIS_TRANSFUSION:" + t.ID)
                            .ToList();
                    }
                    List<string> parts = new List<string> { printCode, treatmentCode, expMestCode };
                    parts.AddRange(transfusionDetails);
                    List<string> validParts = new List<string>();
                    foreach (string part in parts)
                    {
                        if (!string.IsNullOrWhiteSpace(part))
                        {
                            validParts.Add(part);
                        }
                    }

                    result = string.Join(" ", validParts);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }


        void SetSingleKey()
        {
            try
            {
                if (rdo.mps000271Ado != null)
                {
                    if (rdo.mps000271Ado.transfusionSum != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_TRANSFUSION_SUM>(rdo.mps000271Ado.transfusionSum, false);
                    }
                    if (rdo.mps000271Ado.expMestBlood != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_EXP_MEST_BLOOD>(rdo.mps000271Ado.expMestBlood, false);
                    }
                    if (rdo.mps000271Ado.treatment != null)
                    {
                        AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.mps000271Ado.treatment, false);
                    }
                }

                if (rdo.singleKey != null)
                {
                    AddObjectKeyIntoListkey<SingleKey>(rdo.singleKey, false);
                    //SetSingleKey(new KeyValue(Mps000271ExtendSingleKey.CURRENT_DEPARTMENT_NAME, rdo.singleKey.CURRENT_DEPARTMENT_NAME));
                    //SetSingleKey(new KeyValue(Mps000271ExtendSingleKey.CURRENT_ROOM_NAME, rdo.singleKey.CURRENT_ROOM_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
