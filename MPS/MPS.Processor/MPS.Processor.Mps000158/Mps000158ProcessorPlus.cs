/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Khoi phuc tu ban deploy MPS.Processor.Mps000158.dll (file nguon khong co trong git).
 */
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000158.ADO;
using MPS.Processor.Mps000158.PDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000158
{
	public partial class Mps000158Processor
	{
		internal void DataInputProcess()
		{
			try
			{
				patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
				patyAlterBHYTADOs = DataRawProcess.PatyAlterBHYTRawToADOs(rdo.PatyAlters, rdo.Branch, rdo.TreatmentType, rdo.CurrentPatyAlter, rdo.Treatment);
				sereServADOs = new List<SereServADO>();
				List<SereServADO> list = new List<SereServADO>();
				list.AddRange(rdo.SereServs.Select((HIS_SERE_SERV r) => new SereServADO(r, rdo.SereServExts, rdo.HeinServiceTypes, rdo.Services, rdo.Rooms, rdo.materialTypes, rdo.PatyAlters)));
				var list2 = (from o in list
					where o.AMOUNT > 0m && o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT && o.IS_NO_EXECUTE != 1 && o.IS_EXPEND == 1
					orderby o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999
					group o by new { o.SERVICE_ID, o.TOTAL_HEIN_PRICE_ONE_AMOUNT, o.IS_EXPEND, o.NUMBER_OF_FILM }).ToList();
				foreach (var item in list2)
				{
					SereServADO sereServADO = item.FirstOrDefault();
					sereServADO.AMOUNT = item.Sum((SereServADO o) => o.AMOUNT);
					sereServADO.VIR_TOTAL_HEIN_PRICE = item.Sum((SereServADO o) => o.VIR_TOTAL_HEIN_PRICE);
					sereServADO.VIR_TOTAL_PATIENT_PRICE_BHYT = item.Sum((SereServADO o) => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
					sereServADO.TOTAL_PRICE_BHYT = item.Sum((SereServADO o) => o.TOTAL_PRICE_BHYT);
					sereServADO.TOTAL_PRICE_PATIENT_SELF = item.Sum((SereServADO o) => o.TOTAL_PRICE_PATIENT_SELF);
					sereServADOs.Add(sereServADO);
				}
				sereServADOs = sereServADOs.OrderBy((SereServADO o) => o.SERVICE_NAME).ToList();
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}

		internal void HeinServiceTypeProcess()
		{
			try
			{
				heinServiceTypeADOs = new List<HeinServiceTypeADO>();
				List<IGrouping<long?, SereServADO>> list = (from o in sereServADOs
					orderby o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999
					group o by o.HEIN_SERVICE_TYPE_ID).ToList();
				foreach (IGrouping<long?, SereServADO> item in list)
				{
					HeinServiceTypeADO heinServiceTypeADO = new HeinServiceTypeADO();
					SereServADO sereServADO = item.FirstOrDefault();
					if (sereServADO.HEIN_SERVICE_TYPE_ID.HasValue)
					{
						heinServiceTypeADO.ID = sereServADO.HEIN_SERVICE_TYPE_ID.Value;
						heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = sereServADO.HEIN_SERVICE_TYPE_NAME;
					}
					else
					{
						heinServiceTypeADO.HEIN_SERVICE_TYPE_NAME = "Khác";
					}
					heinServiceTypeADO.TOTAL_PRICE_HEIN_SERVICE_TYPE = item.Sum((SereServADO o) => o.TOTAL_PRICE_BHYT);
					heinServiceTypeADO.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = item.Sum((SereServADO o) => o.VIR_TOTAL_HEIN_PRICE.Value);
					heinServiceTypeADO.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = item.Sum((SereServADO o) => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);
					heinServiceTypeADO.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = item.Sum((SereServADO o) => o.TOTAL_PRICE_PATIENT_SELF);
					heinServiceTypeADOs.Add(heinServiceTypeADO);
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
		}	}
}
