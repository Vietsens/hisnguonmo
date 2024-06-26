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
using Inventec.Common.Logging;
using MPS.Processor.Mps000162.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000162
{
    partial class Mps000162Processor : AbstractProcessor
    {
        private void DataInputProcess()
        {
            try
            {
                this.patientADO = DataRawProcess.PatientRawToADO(this.rdo.Treatment);
                this.sereServADOs = new List<SereServADO>();
                List<SereServADO> list = new List<SereServADO>();
                list.AddRange(from r in this.rdo.SereServs
                              select new SereServADO(r, this.rdo.HeinServiceTypes, this.rdo.Services, this.rdo.Rooms, this.rdo.MaterialTypes));
                List<SereServADO> list2 = this.SereServProcess(list);
                if (list2 != null && list2.Count > 0)
                {
                    this.sereServADOs.AddRange(list2);
                }

                this.sereServADOs = (from o in this.sereServADOs orderby o.SERVICE_NAME select o).ToList<SereServADO>();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private List<SereServADO> SereServProcess(List<SereServADO> sereServs)
        {
            List<SereServADO> list = null;
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    list = new List<SereServADO>();
                    var list2 = (from o in sereServs
                                 where o.AMOUNT > 0 && o.IS_NO_EXECUTE != 1 && o.IS_EXPEND == 1
                                 orderby (o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                                 group o by new
                                 {
                                     SERVICE_ID = o.SERVICE_ID,
                                     VIR_PRICE_NO_EXPEND = o.VIR_PRICE_NO_EXPEND,
                                     IS_EXPEND = o.IS_EXPEND
                                 }).ToList();
                    foreach (var current in list2)
                    {
                        SereServADO sereServADO = current.FirstOrDefault<SereServADO>();
                        sereServADO.AMOUNT = current.Sum(o => o.AMOUNT);
                        sereServADO.VIR_TOTAL_HEIN_PRICE = current.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServADO.VIR_TOTAL_PATIENT_PRICE_NO_EXPEND = current.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_NO_EXPEND);
                        sereServADO.VIR_TOTAL_PRICE_NO_EXPEND = current.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                        list.Add(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return list;
        }

        private void HeinServiceTypeProcess()
        {
            try
            {
                this.heinServiceTypeADOs = new List<HeinServiceTypeADO>();
                List<IGrouping<long?, SereServADO>> list = (from o in this.sereServADOs
                                                            orderby o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999L
                                                            group o by o.HEIN_SERVICE_TYPE_ID).ToList<IGrouping<long?, SereServADO>>();
                foreach (IGrouping<long?, SereServADO> current in list)
                {
                    HeinServiceTypeADO heinServiceTypeADO = new HeinServiceTypeADO();
                    SereServADO sereServADO = current.FirstOrDefault<SereServADO>();
                    if (sereServADO.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceTypeADO.ID = new long?(sereServADO.HEIN_SERVICE_TYPE_ID.Value);
                        heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = sereServADO.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }
                    heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = current.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = current.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = current.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_NO_EXPEND);
                    this.heinServiceTypeADOs.Add(heinServiceTypeADO);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
