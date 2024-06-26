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
using MPS.Processor.Mps000122.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000122.ADO;

namespace MPS.Processor.Mps000122
{
    partial class Mps000122Processor : AbstractProcessor
    {
        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                sereServADOs = new List<SereServADO>();
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.SereServs
                                          select new SereServADO(r, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.medicineTypes, rdo.MaterialTypes, rdo.MedicineLines));

                //SereServ FEE

                List<SereServADO> sereServAdoFees = this.SereServFeePayment(sereServADOTemps);
                if (sereServAdoFees != null && sereServAdoFees.Count > 0)
                {
                    sereServADOs.AddRange(sereServAdoFees);
                }

                //Co-Payment
                List<SereServADO> sereServAdoCoPayments = this.SereServBHYTCoPayment(sereServADOTemps);
                if (sereServAdoCoPayments != null && sereServAdoCoPayments.Count > 0)
                {
                    sereServADOs.AddRange(sereServAdoCoPayments);
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();
                this.MedicineLineProcesss(sereServADOs);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        && o.PRICE_CO_PAYMENT > 0
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        && o.VIR_PRICE > 0
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
                        sereServ.VIR_TOTAL_PATIENT_PRICE = sereServGroup
                            .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServAdos.Add(sereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    heinServiceTypeADOs.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void MedicineLineProcesss(List<SereServADO> sereServADOTemps)
        {
            try
            {
                medicineLineADOs = new List<MedicineLineADO>();
                var sereServGroups = sereServADOTemps
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID }).ToList();
                foreach (var sereServs in sereServGroups)
                {

                    SereServADO sereServADO = sereServs.FirstOrDefault();
                    MedicineLineADO medicineLineADO = new MedicineLineADO();
                    medicineLineADO.ID = sereServADO.MEDICINE_LINE_ID;
                    medicineLineADO.HEIN_SERVICE_TYPE_ID = sereServADO.HEIN_SERVICE_TYPE_ID;
                    medicineLineADO.MEDICINE_LINE_CODE = sereServADO.MEDICINE_LINE_CODE;
                    medicineLineADO.MEDICINE_LINE_NAME = sereServADO.MEDICINE_LINE_NAME;
                    if (sereServADO.MEDICINE_LINE_ID <= 0 && sereServADO.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        medicineLineADO.MEDICINE_LINE_CODE = "Chưa xác định";
                        medicineLineADO.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    //Tinh so thang
                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = sereServs.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
                        if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                        {
                            medicineLineADO.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                        }
                    }

                    medicineLineADOs.Add(medicineLineADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
