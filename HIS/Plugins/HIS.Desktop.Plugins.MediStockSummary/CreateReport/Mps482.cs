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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000219.PDO;
using MPS.Processor.Mps000482.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
	internal class Mps482
	{
		List<Mps000482RDO> ListRdo = new List<Mps000482RDO>();
		Dictionary<long, Mps000482RDO> dicBlood = new Dictionary<long, Mps000482RDO>();
		decimal startBeginAmount;
		internal FilterADO _ReportFilter { get; set; }
		List<Mps000482RDO> lstBlood = new List<Mps000482RDO>();
		Mps000482RDO sumBlood = new Mps000482RDO();
		long RoomId;
		public Mps482(long roomId)
		{
			this.RoomId = roomId;
		}

		public void LoadDataPrint482(FilterADO _FilterADO, string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				this._ReportFilter = _FilterADO;

				GetData();

				ProcessData();

				MPS.Processor.Mps000482.PDO.SingKey482 _SingKey482 = new MPS.Processor.Mps000482.PDO.SingKey482();

				if (this._ReportFilter.TIME_FROM > 0)
				{
					_SingKey482.TIME_TO_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_FROM);
				}
				if (this._ReportFilter.TIME_TO > 0)
				{
					_SingKey482.TIME_FROM_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this._ReportFilter.TIME_TO);
				}
				V_HIS_BLOOD_TYPE medicineType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(p => p.ID == this._ReportFilter.BLOOD_TYPE_ID);
				if (medicineType != null)
				{
					_SingKey482.BLOOD_TYPE_CODE = medicineType.BLOOD_TYPE_CODE;
					_SingKey482.BLOOD_TYPE_NAME = medicineType.BLOOD_TYPE_NAME;
                    _SingKey482.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                    _SingKey482.DIC_OTHER_KEY = SetOtherKey(_SingKey482.DIC_OTHER_KEY, medicineType, medicineType.GetType().Name);
				}

				HIS_MEDI_STOCK mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this._ReportFilter.MEDI_STOCK_ID);
				if (mediStock != null)
				{
					_SingKey482.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
					_SingKey482.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
				}

				if (ListRdo != null && ListRdo.Count > 0)
				{
					_SingKey482.MEDI_BEGIN_AMOUNT = ListRdo.First().BEGIN_AMOUNT.ToString();
					_SingKey482.MEDI_END_AMOUNT = ListRdo.Last().END_AMOUNT.ToString();
				}


				lstBlood.Add(sumBlood);

				MPS.Processor.Mps000482.PDO.Mps000482PDO mps000482RDO = new MPS.Processor.Mps000482.PDO.Mps000482PDO(
					ListRdo,
					lstBlood,
					_SingKey482
			   );
				MPS.ProcessorBase.Core.PrintData PrintData = null;
				if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{
					PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000482RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
				}
				else
				{
					PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000482RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
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
				HisImpMestBloodViewFilter filter = new HisImpMestBloodViewFilter();
				filter.IMP_TIME_FROM = this._ReportFilter.TIME_FROM;
				filter.IMP_TIME_TO = this._ReportFilter.TIME_TO;
				filter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				filter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				filter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
				List<V_HIS_IMP_MEST_BLOOD> hisImpMestBlood = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, null);
				//

				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_FROM = this._ReportFilter.TIME_FROM;
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_TO;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);

				if (hisImpMestBlood != null && hisImpMestBlood.Count > 0)
				{
					var groupBlood = hisImpMestBlood.GroupBy(o => o.IMP_TIME);
					foreach (var item in groupBlood)
					{
						Mps000482RDO ado = new Mps000482RDO(item.ToList());
						ListRdo.Add(ado);
					}					
				}
				if (hisExpMestMedicine != null && hisExpMestMedicine.Count > 0)
				{
					var groupBlood = hisExpMestMedicine.GroupBy(o => o.EXP_TIME);
					foreach (var item in groupBlood)
					{
						Mps000482RDO ado = new Mps000482RDO(item.ToList());
						ListRdo.Add(ado);
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);

				result = false;
			}
			return result;
		}

		private bool ProcessData()
		{
			bool result = true;
			try
			{
				ProcessBeginAndEndAmount();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				result = false;
			}
			return result;
		}
		private void ProcessBeginAndEndAmount()
		{
			try
			{
				CommonParam paramGet = new CommonParam();
				ProcessGetPeriod(paramGet);
				if (ListRdo != null && ListRdo.Count > 0)
				{
					ListRdo = ListRdo.OrderBy(o => o.EXECUTE_TIME).ToList();
					ListRdo = ListRdo.GroupBy(g => g.EXECUTE_DATE_STR).Select(s => new Mps000482RDO
					{
						EXECUTE_DATE_STR = s.First().EXECUTE_DATE_STR,
						EXP_AMOUNT = s.Sum(s1 => s1.EXP_AMOUNT),
						IMP_AMOUNT = s.Sum(s2 => s2.IMP_AMOUNT),
						EXPIRED_DATE_STR = (s.FirstOrDefault(s3 => !String.IsNullOrWhiteSpace(s3.EXPIRED_DATE_STR)) ?? new Mps000482RDO()).EXPIRED_DATE_STR						
					}).ToList();
					decimal previousEndAmount = startBeginAmount;
					foreach (var rdo in ListRdo)
					{
						rdo.CalculateAmount(previousEndAmount);
						previousEndAmount = rdo.END_AMOUNT;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
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
							ProcessBeinAmountBloodByMediStockPeriod(paramGet, neighborPeriod);
						}
					}
					else
					{
						// Trường hợp không có kỳ chốt gần nhât - chưa được chốt kỳ nào
						ProcessBeinAmountBloodNotMediStockPriod(paramGet);
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

		private void ProcessBeinAmountBloodByMediStockPeriod(CommonParam paramGet, HIS_MEDI_STOCK_PERIOD neighborPeriod)
		{
			try
			{
				HisMestPeriodBloodViewFilter periodMediFilter = new HisMestPeriodBloodViewFilter();
				periodMediFilter.MEDI_STOCK_PERIOD_ID = neighborPeriod.ID;
				periodMediFilter.BLOOD_TYPE_ID = this._ReportFilter.MediMateId;
				List<V_HIS_MEST_PERIOD_BLOOD> hisMestPeriodMedi = new BackendAdapter(null).Get<List<V_HIS_MEST_PERIOD_BLOOD>>("api/HisMestPeriodBlood/GetView", ApiConsumers.MosConsumer, periodMediFilter, SessionManager.ActionLostToken, null);
				List<MestPeriodBloodADO> lstADO = new List<MestPeriodBloodADO>();
				sumBlood.EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
				if (hisMestPeriodMedi != null && hisMestPeriodMedi.Count > 0)
				{
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
				ProcessBloodBefore(hisImpMestMedicine);
				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_FROM = neighborPeriod.TO_TIME;
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
				ProcessBloodBefore(hisExpMestMedicine);
				startBeginAmount = sumBlood.BEGIN_AMOUNT;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ProcessBloodBefore(List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicine)
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

		private void ProcessBloodBefore(List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine)
		{
			try
			{
				sumBlood.BEGIN_AMOUNT -= hisExpMestMedicine.Count();
				sumBlood.END_AMOUNT -= hisExpMestMedicine.Count();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		//Tính số lượng đầu kỳ không có kỳ dữ liệu gần nhất
		private void ProcessBeinAmountBloodNotMediStockPriod(CommonParam paramGet)
		{
			try
			{
				sumBlood.EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this._ReportFilter.TIME_TO);
				HisImpMestBloodViewFilter impMediFilter = new HisImpMestBloodViewFilter();
				impMediFilter.IMP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				impMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				impMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				impMediFilter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
				List<V_HIS_IMP_MEST_BLOOD> hisImpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMediFilter, SessionManager.ActionLostToken, null);
				ProcessBloodBefore(hisImpMestMedicine);
				HisExpMestBloodViewFilter expMediFilter = new HisExpMestBloodViewFilter();
				expMediFilter.EXP_TIME_TO = this._ReportFilter.TIME_FROM - 1;
				expMediFilter.MEDI_STOCK_ID = this._ReportFilter.MEDI_STOCK_ID;
				expMediFilter.BLOOD_TYPE_ID = this._ReportFilter.BLOOD_TYPE_ID;
				expMediFilter.IS_EXPORT = true;
				List<V_HIS_EXP_MEST_BLOOD> hisExpMestMedicine = new BackendAdapter(null).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMediFilter, SessionManager.ActionLostToken, null);
				ProcessBloodBefore(hisExpMestMedicine);
				startBeginAmount = sumBlood.BEGIN_AMOUNT;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
