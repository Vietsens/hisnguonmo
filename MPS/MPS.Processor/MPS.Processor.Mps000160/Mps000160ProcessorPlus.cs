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
using MPS.Processor.Mps000160.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000160
{
    partial class Mps000160Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.MaterialTypes));

                //SereServ FEE
                List<SereServADO> sereServAdoFees = this.SereServFeePayment(sereServADOTemps);
                if (sereServAdoFees != null && sereServAdoFees.Count > 0)
                {
                    sereServADOs.AddRange(sereServAdoFees);
                }

                //Co-Payment: ban DLL goc (04/2025) khong goi buoc nay, giu nguyen hanh vi
                //List<SereServADO> sereServAdoCoPayments = this.SereServBHYTCoPayment(sereServADOTemps);
                //if (sereServAdoCoPayments != null && sereServAdoCoPayments.Count > 0)
                //{
                //    sereServADOs.AddRange(sereServAdoCoPayments);
                //}

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private List<SereServADO> SereServBHYTCoPayment(List<SereServADO> sereServs)
        {
            List<SereServADO> sereServAdos = null;
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    sereServAdos = new List<SereServADO>();
                    var sereServGroups = sereServs
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT
                        && o.IS_EXPEND != 1
                        && o.IS_NO_EXECUTE != 1)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.VIR_TOTAL_PRICE,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.HEIN_LIMIT_PRICE,
                        o.PRICE_CO_PAYMENT,
                        o.VIR_TOTAL_HEIN_PRICE
                    }).ToList();

                    foreach (var sereServGroup in sereServGroups)
                    {
                        SereServADO sereServ = sereServGroup.FirstOrDefault();
                        sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                        sereServ.VIR_TOTAL_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                        sereServ.VIR_TOTAL_PATIENT_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                        sereServ.VIR_TOTAL_HEIN_PRICE = 0;
                        sereServ.VIR_PRICE = sereServ.PRICE_CO_PAYMENT;
                        sereServAdos.Add(sereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return sereServAdos;
        }

        private List<SereServADO> SereServFeePayment(List<SereServADO> sereServs)
        {
            List<SereServADO> sereServAdos = null;
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    sereServAdos = new List<SereServADO>();
                    var sereServGroups = sereServs
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__FEE
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.VIR_PRICE,
                        o.IS_EXPEND
                    }).ToList();

                    foreach (var sereServGroup in sereServGroups)
                    {
                        SereServADO sereServ = sereServGroup.FirstOrDefault();
                        sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                        sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServ.VIR_TOTAL_PATIENT_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServAdos.Add(sereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return sereServAdos;
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

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    heinServiceTypeADOs.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
