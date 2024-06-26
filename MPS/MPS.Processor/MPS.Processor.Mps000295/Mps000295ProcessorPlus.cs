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
using MPS.Processor.Mps000295.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000295.ADO;
using MPS.Processor.Mps000295.PDO.Config;
using Newtonsoft.Json;

namespace MPS.Processor.Mps000295
{
    public partial class Mps000295Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.medicineTypes, rdo.MedicineLines, rdo.materialTypes, rdo.PatientTypeCFG));

                //sereServ la bhyt, gom nhom
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_NO_EXECUTE != 1)
                    //&& o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.IS_EXPEND,
                        o.PRICE_FEE,
                        o.PRICE_SERVICE,
                        o.NUMBER_OF_FILM,
                        o.PATIENT_TYPE_ID,
                        o.PRIMARY_PATIENT_TYPE_ID
                    }).ToList();

                foreach (var g in sereServGroups)
                {
                    SereServADO sereServ = g.FirstOrDefault();
                    sereServ.AMOUNT = g.Sum(o => o.AMOUNT);
                    sereServ.TOTAL_PRICE_SERVICE = g.Sum(o => o.TOTAL_PRICE_SERVICE);
                    sereServ.TOTAL_PRICE_FEE = g.Sum(o => o.TOTAL_PRICE_FEE);
                    sereServ.DISCOUNT = g.Sum(o => o.DISCOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServADOs.Add(sereServ);
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();
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
                var sereServGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID }).ToList();

                foreach (var g in sereServGroups)
                {
                    HeinServiceTypeADO heinServiceType = new HeinServiceTypeADO();
                    SereServADO sereServBHYT = g.FirstOrDefault();
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                        heinServiceType.NUM_ORDER = sereServBHYT.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    heinServiceType.TOTAL_PRICE_FEE = g.Sum(o => o.TOTAL_PRICE_FEE);
                    heinServiceType.TOTAL_PRICE_SERVICE = g.Sum(o => o.TOTAL_PRICE_SERVICE);
                    heinServiceType.VIR_TOTAL_HEIN_PRICE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    heinServiceType.VIR_TOTAL_PATIENT_PRICE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    heinServiceType.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                    heinServiceType.DISCOUNT = g.Sum(o => o.DISCOUNT);
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
