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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.ADO;
using HIS.Desktop.Plugins.MediStockSummary.Base;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000483.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
	internal class Mps483
	{
		decimal startBeginAmount;
		List<Mps000483RDO> ListRdo = new List<Mps000483RDO>();
		List<Mps000483RDO> ListByPackages = new List<Mps000483RDO>();
		Dictionary<long, Mps000483RDO> dicBlood = new Dictionary<long, Mps000483RDO>();
		List<Mps000483RDO> lstBlood= new List<Mps000483RDO>();
		Mps000483RDO sumBlood = new Mps000483RDO();
		decimal BeginAmount = 0;
		decimal EndAmount = 0;
		private string a = "";
		long RoomId;
		internal FilterADO _ReportFilter { get; set; }
		public Mps483(long roomId)
		{
			this.RoomId = roomId;
		}
		public void LoadDataPrint483(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				this._ReportFilter = _FilterADO;
				GetData();

				ProcessData();

				MPS.Processor.Mps000483.PDO.SingKey483 _SingKey483 = new MPS.Processor.Mps000483.PDO.SingKey483();

				if (this._ReportFilter.TIME_FROM > 0)
				{
					_SingKey483.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
				}
				if (this._ReportFilter.TIME_TO > 0)
				{
					_SingKey483.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
				}
				_SingKey483.BEGIN_AMOUNT = BeginAmount.ToString();
				_SingKey483.END_AMOUNT = EndAmount.ToString();
				V_HIS_BLOOD_TYPE medicineType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.BLOOD_TYPE_ID);
				if (medicineType != null)
				{
					_SingKey483.BLOOD_TYPE_CODE = medicineType.BLOOD_TYPE_CODE;
					_SingKey483.BLOOD_TYPE_NAME = medicineType.BLOOD_TYPE_NAME;
                    _SingKey483.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey483.DIC_OTHER_KEY = SetOtherKey(_SingKey483.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);
				}

				HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
				if (mediStock != null)
				{
					_SingKey483.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
					_SingKey483.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
				}

				if (ListRdo != null && ListRdo.Count > 0)
				{
					_SingKey483.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
					_SingKey483.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
				}
				
				lstBlood.Add(sumBlood);
			

				MPS.Processor.Mps000483.PDO.Mps000483PDO mps000483RDO = new MPS.Processor.Mps000483.PDO.Mps000483PDO(
					ListRdo,
					lstBlood,
					ListByPackages,
					_SingKey483
			   );
				MPS.ProcessorBase.Core.PrintData PrintData = null;
				if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{
					PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000483RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
				}
				else
				{
					PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000483RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
				}
				Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, RoomId);
				PrintData.EmrInputADO = inputADO;

				result = MPS.MpsPrinter.Run(PrintData);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private Dictionary<string, object> SetOtherKey(Dictionary<string, object> dicOtherKey, object data, string prefix)
        {
            try
            {
                if (dicOtherKey == null) dicOtherKey = new Dictionary<string, object>();
                foreach (var prop in data.GetType().GetProperties())
                {
                    if (!dicOtherKey.ContainsKey(string.Format("{0}__{1}", prefix, prop.Name)))
                        dicOtherKey.Add(string.Format("{0}__{1}", prefix, prop.Name), prop.GetValue(data, null));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dicOtherKey;
        }

		private bool GetData()
		{
			var result = true;
			try
			{
				List<HIS_EXP_MEST_TYPE> expMestTypes = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();
				List<HIS_IMP_MEST_TYPE> impMestTypes = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>();

				HisImpMestBloodViewFilter impMediFilter = new HisImpMestBloodViewFilter();
				impMediFilter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
				impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
				impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				impMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
				List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicines = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);

				List<long> impMestIds = hisImpMestMedicines.Select(o => o.IMP_MEST_ID).ToList();
				var aggrImpId = hisImpMestMedicines.Select(o => o.AGGR_IMP_MEST_ID ?? 0).ToList();
				if (aggrImpId != null && aggrImpId.Count > 0)
					impMestIds.AddRange(aggrImpId);

				//
				List<V_HIS_IMP_MEST> ImpMest = new List<V_HIS_IMP_MEST>();
				if (impMestIds != null && impMestIds.Count > 0)
				{
					impMestIds = impMestIds.Distinct().ToList();
					var skip = 0;
					while (impMestIds.Count - skip > 0)
					{
						var listIDs = impMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
						skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
						HisImpMestViewFilter ImpFilter = new HisImpMestViewFilter()
						{
							IDs = listIDs
						};
						var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, ImpFilter, SessionManager.ActionLostToken, null);
						ImpMest.AddRange(x);
					}
				}

				//
				List<long> sourceMestIds = ImpMest.Where(o => o.CHMS_EXP_MEST_ID.HasValue).Select(s => s.CHMS_EXP_MEST_ID.Value).ToList();
				List<V_HIS_EXP_MEST> sourceMest = new List<V_HIS_EXP_MEST>();
				if (sourceMestIds != null && sourceMestIds.Count > 0)
				{
					var skip = 0;
					while (sourceMestIds.Count - skip > 0)
					{
						var listIDs = sourceMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
						skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
						HisExpMestViewFilter sourceFilter = new HisExpMestViewFilter()
						{
							IDs = listIDs,
							EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
						};
						var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, sourceFilter, SessionManager.ActionLostToken, null);
						sourceMest.AddRange(x);
					}
				}

				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);

				List<long> hisExpMestMedicineIds = hisExpMestMedicine.Select(o => o.EXP_MEST_ID).ToList();
				var aggrExpMest = hisExpMestMedicine.Select(o => o.AGGR_EXP_MEST_ID ?? 0).ToList();
				if (aggrExpMest != null && aggrExpMest.Count > 0)
					hisExpMestMedicineIds.AddRange(aggrExpMest);

				List<V_HIS_EXP_MEST> hisExpMest = new List<V_HIS_EXP_MEST>();
				if (hisExpMestMedicineIds != null && hisExpMestMedicineIds.Count > 0)
				{
					hisExpMestMedicineIds = hisExpMestMedicineIds.Distinct().ToList();
					var skip = 0;
					while (hisExpMestMedicineIds.Count - skip > 0)
					{
						var listIDs = hisExpMestMedicineIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
						skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
						HisExpMestViewFilter ExpFilter = new HisExpMestViewFilter()
						{
							IDs = listIDs
						};
						var x = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, ExpFilter, SessionManager.ActionLostToken, null);
						hisExpMest.AddRange(x);
					}
				}

				//
				List<long> destMestIds = hisExpMest.Where(p => p.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK).Select(o => o.ID).ToList();
				List<V_HIS_IMP_MEST> destMest = new List<V_HIS_IMP_MEST>();
				if (destMestIds != null && destMestIds.Count > 0)
				{
					var skip = 0;
					while (sourceMestIds.Count - skip > 0)
					{
						var listIDs = destMestIds.Skip(skip).Take(ManagerConstant.MAX_REQUEST_LENGTH_PARAM).ToList();
						skip = skip + ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
						HisImpMestViewFilter destFilter = new HisImpMestViewFilter()
						{
							CHMS_EXP_MEST_IDs = listIDs,
						};
						var x = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, destFilter, SessionManager.ActionLostToken, null);
						destMest.AddRange(x);
					}
				}

				if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
				{
					var groupBlood = hisImpMestMedicines.GroupBy(o => o.IMP_TIME);
					List<Mps000483RDO> listSub = new List<Mps000483RDO>();
					foreach (var item in groupBlood)
					{
						Mps000483RDO ado = new Mps000483RDO(item.ToList(), ImpMest, sourceMest, impMestTypes, BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>());
						listSub.Add(ado);
					}
					this.ProcessGroupByImpMest(listSub);
					this.ProcessGroupByImpMestPackage(listSub);
				}

				if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
				{
					var groupBlood = hisExpMestMedicine.GroupBy(o => o.EXP_TIME);
					List<Mps000483RDO> listSub = new List<Mps000483RDO>();
					foreach (var item in groupBlood)
					{
						Mps000483RDO ado = new Mps000483RDO(item.ToList(), hisExpMest, destMest, expMestTypes, BackendDataWorker.Get<HIS_MEDI_STOCK>(), BackendDataWorker.Get<HIS_DEPARTMENT>(), BackendDataWorker.Get<V_HIS_ROOM>());
						listSub.Add(ado);
					}
					this.ProcessGroupByExpMest(listSub);
					this.ProcessGroupByExpMestPackage(listSub);
				}				
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);

				result = false;
			}
			return result;
		}

		private void ProcessGroupByImpMest(List<Mps000483RDO> listSub)
		{
			var groupByCode = listSub.GroupBy(o => o.IMP_MEST_CODE).ToList();

			foreach (var group in groupByCode)
			{
				List<Mps000483RDO> p = group.ToList<Mps000483RDO>();
				Mps000483RDO rdo = new Mps000483RDO()
				{
					EXP_MEST_CODE = p.First().EXP_MEST_CODE,
					EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
                    EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
					IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
					EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
					PACKAGE_NUMBER = string.Join(", ", p.Select(q => q.PACKAGE_NUMBER).Distinct().ToList()),
					EXPIRED_DATE = p.First().EXPIRED_DATE,
					EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
					BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
					IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
					EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
					END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
					REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
					REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
					MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
					EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
					EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
					IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
					IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
					CLIENT_NAME = p.First().CLIENT_NAME,
					VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
					VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
					TREATMENT_CODE = p.First().TREATMENT_CODE,
					SUPPLIER_NAME = p.First().SUPPLIER_NAME,
					SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
					IMP_PRICE = p.First().IMP_PRICE,
					IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,

					IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
					IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
				};
				ListRdo.Add(rdo);
			}
		}
		private void ProcessGroupByImpMestPackage(List<Mps000483RDO> listSub)
		{
			var groupByCode = listSub.GroupBy(o => new { o.IMP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

			foreach (var group in groupByCode)
			{
				List<Mps000483RDO> p = group.ToList<Mps000483RDO>();
				Mps000483RDO rdo = new Mps000483RDO()
				{
					EXP_MEST_CODE = p.First().EXP_MEST_CODE,
					EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
					EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
					IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
					EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
					PACKAGE_NUMBER = p.First().PACKAGE_NUMBER,
					EXPIRED_DATE = p.First().EXPIRED_DATE,
					EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
					BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
					IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
					EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
					END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
					REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
					REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
					MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
					EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
					EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
					IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
					IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
					CLIENT_NAME = p.First().CLIENT_NAME,
					VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
					VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
					TREATMENT_CODE = p.First().TREATMENT_CODE,
					SUPPLIER_NAME = p.First().SUPPLIER_NAME,
					SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
					IMP_PRICE = p.First().IMP_PRICE,
					IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
					IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
					IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
				};
				ListByPackages.Add(rdo);
			}
		}
		private void ProcessGroupByExpMest(List<Mps000483RDO> listSub)
		{
			var groupByCode = listSub.GroupBy(o => o.EXP_MEST_CODE).ToList();

			foreach (var group in groupByCode)
			{
				List<Mps000483RDO> p = group.ToList<Mps000483RDO>();
				Mps000483RDO rdo = new Mps000483RDO()
				{
					EXP_MEST_CODE = p.First().EXP_MEST_CODE,
					EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
					EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
					IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
					EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
					PACKAGE_NUMBER = string.Join(", ", p.Select(q => q.PACKAGE_NUMBER).Distinct().ToList()),
					EXPIRED_DATE = p.First().EXPIRED_DATE,
					EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
					BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
					IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
					EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
					END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
					REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
					REQUEST_DEPARTMENT_NAME = p.First().REQUEST_DEPARTMENT_NAME,
					REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
					MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
					EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
					EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
					IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
					IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
					CLIENT_NAME = p.First().CLIENT_NAME,
					VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
					VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
					TREATMENT_CODE = p.First().TREATMENT_CODE,
					SUPPLIER_NAME = p.First().SUPPLIER_NAME,
					SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
					IMP_PRICE = p.First().IMP_PRICE,
					IMP_VAT_RATIO = p.First().IMP_VAT_RATIO,
					IMP_AMOUNT_HOAN = p.First().CHMS_TYPE_ID == 2 ? p.Sum(q => q.IMP_AMOUNT) : 0,
					IMP_AMOUNT_KHONG_GOM_HOAN = p.First().CHMS_TYPE_ID != 2 ? p.Sum(q => q.IMP_AMOUNT) : 0
				};
				ListRdo.Add(rdo);
			}
		}

		private void ProcessGroupByExpMestPackage(List<Mps000483RDO> listSub)
		{
			var groupByCode = listSub.GroupBy(o => new { o.EXP_MEST_CODE, o.PACKAGE_NUMBER }).ToList();

			foreach (var group in groupByCode)
			{
				List<Mps000483RDO> p = group.ToList<Mps000483RDO>();
				Mps000483RDO rdo = new Mps000483RDO()
				{
					EXP_MEST_CODE = p.First().EXP_MEST_CODE,
					EXECUTE_DATE_STR = p.First().EXECUTE_DATE_STR,
					EXECUTE_TIME = p.First().EXECUTE_TIME,
                    IMP_MEST_CODE = p.First().IMP_MEST_CODE,
                    DESCRIPTION = p.First().DESCRIPTION,
                    DESCRIPTION_DETAIL = p.First().DESCRIPTION_DETAIL,
                    CHMS_TYPE_ID = p.First().CHMS_TYPE_ID,
					IMP_MEST_TYPE_NAME = p.First().IMP_MEST_TYPE_NAME,
					EXP_MEST_TYPE_NAME = p.First().EXP_MEST_TYPE_NAME,
					PACKAGE_NUMBER = p.First().PACKAGE_NUMBER,
					EXPIRED_DATE = p.First().EXPIRED_DATE,
					EXPIRED_DATE_STR = p.First().EXPIRED_DATE_STR,
					BEGIN_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).First().BEGIN_AMOUNT,
					IMP_AMOUNT = p.Sum(q => q.IMP_AMOUNT) == 0 ? null : p.Sum(q => q.IMP_AMOUNT),
					EXP_AMOUNT = p.Sum(q => q.EXP_AMOUNT) == 0 ? null : p.Sum(q => q.EXP_AMOUNT),
					END_AMOUNT = p.OrderBy(q => q.EXECUTE_TIME).Last().END_AMOUNT,
					REQ_DEPARTMENT_NAME = p.First().REQ_DEPARTMENT_NAME,
					REQUEST_DEPARTMENT_NAME = p.First().REQUEST_DEPARTMENT_NAME,
					REQ_ROOM_NAME = p.First().REQ_ROOM_NAME,
					MEDI_STOCK_NAME = p.First().MEDI_STOCK_NAME,
					EXP_MEDI_STOCK_CODE = p.First().EXP_MEDI_STOCK_CODE,
					EXP_MEDI_STOCK_NAME = p.First().EXP_MEDI_STOCK_NAME,
					IMP_MEDI_STOCK_CODE = p.First().IMP_MEDI_STOCK_CODE,
					IMP_MEDI_STOCK_NAME = p.First().IMP_MEDI_STOCK_NAME,
					CLIENT_NAME = p.First().CLIENT_NAME,
					VIR_PATIENT_NAME = p.First().VIR_PATIENT_NAME,
					VIR_PATIENT_ADDRESS = p.First().VIR_PATIENT_ADDRESS,
					TREATMENT_CODE = p.First().TREATMENT_CODE,
					SUPPLIER_NAME = p.First().SUPPLIER_NAME,
					SECOND_MEST_CODE = p.First().SECOND_MEST_CODE,
					IMP_PRICE = p.First().IMP_PRICE,
					IMP_VAT_RATIO = p.First().IMP_VAT_RATIO
				};
				ListByPackages.Add(rdo);
			}
		}

		private bool ProcessData()
		{
			bool result = false;
			try
			{
				if (ListRdo != null || ListByPackages != null)
				{
					ProcessBeginAndEndAmount();
				}
				result = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				ListRdo.Clear();

			}
			return result;
		}

		private void ProcessBeginAndEndAmount()
		{
			try
			{
				CommonParam paramGet = new CommonParam();
				ProcessGetPeriod(paramGet);
				ListRdo = ListRdo.OrderBy(o => o.EXECUTE_TIME).ToList();
				//ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00067RDO
				//{
				//    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
				//    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
				//    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
				//    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00067RDO()).EXPIRED_DATE_STR,
				//    PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct())
				//}).ToList();
				ListByPackages = ListByPackages.OrderBy(o => o.EXECUTE_TIME).ToList();
				//ListByPackages = ListByPackages.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mrs00067RDO
				//{
				//    EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
				//    EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
				//    IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
				//    EXPIRED_DATE_STR = (s.First(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mrs00067RDO()).EXPIRED_DATE_STR,
				//    PACKAGE_NUMBER = string.Join(",", s.Select(o => o.PACKAGE_NUMBER).Distinct())
				//}).ToList();
				decimal previousEndAmount = startBeginAmount;
				foreach (var rdo in ListRdo)
				{
					rdo.CalculateAmount(previousEndAmount);
					previousEndAmount = rdo.END_AMOUNT;
				}
				
				decimal packgeEndAmount = startBeginAmount;
				foreach (var rdo in ListByPackages)
				{
					rdo.CalculateAmount(packgeEndAmount);
					packgeEndAmount = rdo.END_AMOUNT;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		// Lay kỳ chốt gần nhất với thời gian từ của báo cáo
		private void ProcessGetPeriod(CommonParam paramGet)
		{
			try
			{
				HisMediStockPeriodFilter periodFilter = new HisMediStockPeriodFilter();
				periodFilter.TO_TIME_TO = this._ReportFilter.TIME_FROM;
				periodFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				List<HIS_MEDI_STOCK_PERIOD> hisMediStockPeriod = new BackendAdapter(null).Get<List<HIS_MEDI_STOCK_PERIOD>>("api/HisMediStockPeriod/Get", ApiConsumers.MosConsumer, periodFilter, SessionManager.ActionLostToken, null);
				if (!paramGet.HasException)
				{
					if (hisMediStockPeriod != null && hisMediStockPeriod.Count > 0)
					{
						//Trường hợp có kỳ được chốt gần nhất
						HIS_MEDI_STOCK_PERIOD neighborPeriod = hisMediStockPeriod.OrderByDescending(d => d.TO_TIME).ToList()[0];
						if (neighborPeriod != null)
						{
							ProcessBeinAmountMedicineByMediStockPeriod(paramGet, neighborPeriod);
						}
					}
					else
					{
						// Trường hợp không có kỳ chốt gần nhât - chưa được chốt kỳ nào
						ProcessBeinAmountMedicineNotMediStockPriod(paramGet);
					}

					if (paramGet.HasException)
					{
						throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh tong hop du lieu.");
					}
				}
				else
				{
					throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh tong hop du lieu.");
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		// Tính số lượng đầu kỳ có kỳ dữ liệu gần nhất
		private void ProcessBeinAmountMedicineByMediStockPeriod(CommonParam paramGet, HIS_MEDI_STOCK_PERIOD neighborPeriod)
		{
			try
			{
				List<Mps000483RDO> listrdo = new List<Mps000483RDO>();
				HisMestPeriodBloodViewFilter periodMediFilter = new HisMestPeriodBloodViewFilter();
				periodMediFilter.MEDI_STOCK_PERIOD_ID = neighborPeriod.ID;
				periodMediFilter.BLOOD_TYPE_ID = this._ReportFilter.MediMateId;
				List<V_HIS_MEST_PERIOD_BLOOD> hisMestPeriodMedi = new BackendAdapter(null).Get<List<V_HIS_MEST_PERIOD_BLOOD>>("api/HisMestPeriodBlood/GetView", ApiConsumers.MosConsumer, periodMediFilter, SessionManager.ActionLostToken, null);
				List<MestPeriodBloodADO> lstADO = new List<MestPeriodBloodADO>();
				sumBlood.EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
				if (hisMestPeriodMedi != null && hisMestPeriodMedi.Count > 0)
				{
					sumBlood.EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
					sumBlood.BEGIN_AMOUNT = hisMestPeriodMedi.Count();
					sumBlood.END_AMOUNT = hisMestPeriodMedi.Count();
				}				

				HisImpMestBloodViewFilter impMediFilter = new HisImpMestBloodViewFilter();
				impMediFilter.IMP_TIME_FROM = neighborPeriod.TO_TIME + 1;
				impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				impMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
				List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
				ProcessMedicineBefore(hisImpMestMedicine);
				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME + 1;
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
				ProcessMedicineBefore(hisExpMestMedicine);

				BeginAmount = EndAmount = startBeginAmount = sumBlood.BEGIN_AMOUNT;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ProcessMedicineBefore(List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicine)
		{
			try
			{				
				sumBlood.BEGIN_AMOUNT += hisImpMestMedicine.Count();
				sumBlood.END_AMOUNT += hisImpMestMedicine.Count();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ProcessMedicineBefore(List<V_HIS_EXP_MEST_BLOOD> hisMestPeriodMedi)
		{
			try
			{
				sumBlood.BEGIN_AMOUNT -= hisMestPeriodMedi.Count();
				sumBlood.END_AMOUNT -= hisMestPeriodMedi.Count();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}


		//Tính số lượng đầu kỳ không có kỳ dữ liệu gần nhất
		private void ProcessBeinAmountMedicineNotMediStockPriod(CommonParam paramGet)
		{
			try
			{
				sumBlood.EXP_MEST_TYPE_NAME = "Tồn cuối kỳ";
				sumBlood.EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);


				HisImpMestBloodViewFilter impMediFilter = new HisImpMestBloodViewFilter();
				impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				impMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
				List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
				ProcessMedicineBefore(hisImpMestMedicine);
				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
				ProcessMedicineBefore(hisExpMestMedicine);
				BeginAmount = EndAmount = startBeginAmount = sumBlood.BEGIN_AMOUNT;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
