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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000224.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000224.ADO;

namespace MPS.Processor.Mps000224
{
    public partial class Mps000224Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                patyAlterBHYTADOs = DataRawProcess.PatyAlterBHYTRawToADOs(rdo.PatyAlters, rdo.Branch, rdo.TreatmentType);
                sereServADOs = new List<SereServADO>();

                dicSereServ = new Dictionary<DicKey.TYPE, List<SereServADO>>();
                dicSereServ[DicKey.TYPE.SERE_SERV_PTTT] = new List<SereServADO>();
                dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT] = new List<SereServADO>();
                dicSereServ[DicKey.TYPE.SERE_SERV_OTHER] = new List<SereServADO>();

                List<HIS_SERE_SERV> totalSereServ = new List<HIS_SERE_SERV>();
                if (rdo.SereServPTTTId > 0 && rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    totalSereServ = rdo.SereServs.Where(o => o.ID == rdo.SereServPTTTId || o.PARENT_ID == rdo.SereServPTTTId).ToList();
                }
                else if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                {
                    totalSereServ = rdo.SereServs;
                }

                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in totalSereServ
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.PatyAlters));

                var sereServBHYTGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.IS_EXPEND,
                        o.PARENT_ID
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServADOs.Add(sereServ);
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();

                if (rdo.SereServPTTTId > 0 && sereServADOs != null)
                {
                    dicSereServ[DicKey.TYPE.SERE_SERV_PTTT] = sereServADOs.Where(o => o.ID == rdo.SereServPTTTId).ToList();
                    dicSereServ[DicKey.TYPE.SERE_SERV_HAS_PARENT] = sereServADOs.Where(o =>
                        o.PARENT_ID == rdo.SereServPTTTId
                        && o.IS_EXPEND == 1
                        && o.IS_NO_EXECUTE != 1
                        && (o.SERVICE_TYPE_ID == rdo.ServiceTypeCFG.SERVICE_TYPE_ID__MEDI
                            || o.SERVICE_TYPE_ID == rdo.ServiceTypeCFG.SERVICE_TYPE_ID__MATE))
                        .ToList();
                    dicSereServ[DicKey.TYPE.SERE_SERV_OTHER] = sereServADOs.Where(o =>
                        o.PARENT_ID == rdo.SereServPTTTId
                        //&& o.IS_EXPEND == 1
                        && o.IS_NO_EXECUTE != 1
                        && o.SERVICE_TYPE_ID != rdo.ServiceTypeCFG.SERVICE_TYPE_ID__MEDI
                            && o.SERVICE_TYPE_ID != rdo.ServiceTypeCFG.SERVICE_TYPE_ID__MATE)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void HeinServiceTypeProcess()
        {
            try
            {
                heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                    heinServiceTypeADOs.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
