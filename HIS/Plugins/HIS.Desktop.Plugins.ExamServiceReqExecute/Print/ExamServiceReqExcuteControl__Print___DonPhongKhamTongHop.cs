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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
	public partial class ExamServiceReqExecuteControl : UserControlBase
	{
		private void clickItemInDonPhongKhamTongHop(object sender, EventArgs e)
		{
			try
			{
				bool printNow = false;
				InDonPhongKhamTongHop(printNow, false);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void InDonPhongKhamTongHop(bool printNow, bool IsMenuButton)
		{
			try
			{
				CommonParam param = new CommonParam();
				//Load expmest
				HisExpMestFilter expMestFilter = new HisExpMestFilter();
				expMestFilter.TDL_TREATMENT_ID = this.treatmentId;
				List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
					 .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

				List<HIS_EXP_MEST> expMestsFake = new List<HIS_EXP_MEST>();
				List<HIS_SERVICE_REQ> ServiceReqFake = new List<HIS_SERVICE_REQ>();
				if ((expMests == null || expMests.Count == 0) || (expMests != null && (expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).ToList() == null || expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).ToList().Count == 0)))
				{
					if (IsMenuButton)
					{
						HIS_EXP_MEST obj = new HIS_EXP_MEST();
						//Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(obj, treatment);
						obj.ID = -1;
						obj.SERVICE_REQ_ID = -1;
						expMestsFake.Add(obj);

						HIS_SERVICE_REQ hIS_SERVICE_REQ = new HIS_SERVICE_REQ();
						Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(hIS_SERVICE_REQ, treatment);
						hIS_SERVICE_REQ.ID = -1;
						hIS_SERVICE_REQ.TREATMENT_ID = treatment.ID;
						hIS_SERVICE_REQ.REQUEST_ROOM_ID = moduleData.RoomId;
						ServiceReqFake.Add(hIS_SERVICE_REQ);

					}
					else
					{
						if (!HisConfigCFG.IsAllowPrintNoMedicine)
						{
							return;
						}
						else
						{
							HIS_EXP_MEST obj = new HIS_EXP_MEST();
							//Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(obj, treatment);
							obj.ID = -1;
							obj.SERVICE_REQ_ID = -1;
							expMestsFake.Add(obj);

							HIS_SERVICE_REQ hIS_SERVICE_REQ = new HIS_SERVICE_REQ();
							Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(hIS_SERVICE_REQ, treatment);
							hIS_SERVICE_REQ.ID = -1;
							hIS_SERVICE_REQ.TREATMENT_ID = treatment.ID;
							hIS_SERVICE_REQ.REQUEST_ROOM_ID = moduleData.RoomId;
							ServiceReqFake.Add(hIS_SERVICE_REQ);

						}
					}
				}


				HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
				serviceReqFilter.TREATMENT_ID = this.treatmentId;
				List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
					 .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

				//Lays thuoc vat tu trong kho
				IEnumerable<IGrouping<long?, HIS_EXP_MEST>> expMestGroups = null;
				List<HIS_EXP_MEST_MEDICINE> expMestMedicines = null;
				List<HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
				if (expMests != null && expMests.Count > 0)
				{
					HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
					expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
					expMestMedicines = new BackendAdapter(param)
						.Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

					HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
					expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
					expMestMaterials = new BackendAdapter(param)
						.Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

					expMestGroups = expMests.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK).GroupBy(o => o.AGGR_EXP_MEST_ID);
				}
				string printTypeCode = HisConfigCFG.MPS_PrintPrescription;
				if (string.IsNullOrEmpty(printTypeCode))
				{
					printTypeCode = PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_GOP_DON_THUOC__MPS000234;

				}

				if (expMestGroups != null && expMestGroups.ToList().Count > 0)
				{
					#region Có ExpMest
					foreach (var listExpMest in expMestGroups)
					{
						if (listExpMest.First().AGGR_EXP_MEST_ID == null)
						{
							foreach (var expMest in listExpMest)
							{
								List<long> serviceReqIdTemps = new List<long> { expMest.SERVICE_REQ_ID ?? 0 };
								List<long> expMestIdTemps = new List<long> { expMest.ID };
								List<HIS_SERVICE_REQ> serviceReqTemps = serviceReqs.Where(o => serviceReqIdTemps.Contains(o.ID)).ToList();
								List<HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = expMestMedicines.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();
								List<HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = expMestMaterials.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();

								List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
								if ((expMestMedicineTemps != null && expMestMedicineTemps.Count > 0)
											|| (expMestMaterialTemps != null && expMestMaterialTemps.Count > 0))
								{
									OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
									outPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST> { expMest };
									outPatientPresResultSDO.ServiceReqs = serviceReqTemps;
									outPatientPresResultSDO.Medicines = expMestMedicineTemps;
									outPatientPresResultSDO.Materials = expMestMaterialTemps;
									OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
								}
								PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMest, this.moduleData);
								printPrescriptionProcessor.Print(printTypeCode, false);
							}
						}
						else
						{
							HIS_EXP_MEST expMestPrimary = expMests.FirstOrDefault(o => o.ID == listExpMest.First().AGGR_EXP_MEST_ID);
							List<long> serviceReqIdTemps = listExpMest.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
							List<long> expMestIdTemps = listExpMest.Select(o => o.ID).ToList();

							List<HIS_SERVICE_REQ> serviceReqTemps = serviceReqs.Where(o => serviceReqIdTemps.Contains(o.ID)).ToList();
							List<HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = expMestMedicines.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();
							List<HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = expMestMaterials.Where(o => expMestIdTemps.Contains(o.EXP_MEST_ID ?? 0)).ToList();

							List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
							if ((expMestMedicineTemps != null && expMestMedicineTemps.Count > 0)
										|| (expMestMaterialTemps != null && expMestMaterialTemps.Count > 0))
							{
								OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
								outPatientPresResultSDO.ExpMests = listExpMest.ToList();
								outPatientPresResultSDO.ServiceReqs = serviceReqTemps;
								outPatientPresResultSDO.Medicines = expMestMedicineTemps;
								outPatientPresResultSDO.Materials = expMestMaterialTemps;
								OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
							}
							PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMestPrimary, this.moduleData);
							printPrescriptionProcessor.Print(printTypeCode, printNow);
						}
					}
					#endregion
				}
				else
				{
					List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();

					OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
					outPatientPresResultSDO.ExpMests = expMestsFake;
					outPatientPresResultSDO.ServiceReqs = ServiceReqFake;
					outPatientPresResultSDO.Medicines = new List<HIS_EXP_MEST_MEDICINE>();
					outPatientPresResultSDO.Materials = new List<HIS_EXP_MEST_MATERIAL>();
					OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);

					PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMestsFake.First(), this.moduleData);
					printPrescriptionProcessor.Print(printTypeCode, printNow);
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
