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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using MOS.Filter;

namespace HIS.Desktop.Plugins.MRSummaryDetail.Run
{
	public partial class frmMRSummaryDetail
	{
		private bool DelegateRunPrinter(string printTypeCode, string fileName)
		{
			bool result = false;
			try
			{
				switch (printTypeCode)
				{
					case "Mps000472":
						LoadBieuMauMps472(printTypeCode, fileName, ref result);
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return result;
		}

		private void LoadBieuMauMps472(string printTypeCode, string fileName, ref bool result)
		{
			try
			{
				WaitingManager.Show();
				CommonParam param = new CommonParam();
				HisMrCheckSummaryFilter mrCheckFilter = new HisMrCheckSummaryFilter();
				mrCheckFilter.TREATMENT_ID = treatmentId;
				var lstCheckSummary = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_SUMMARY>>("api/HisMrCheckSummary/Get", ApiConsumer.ApiConsumers.MosConsumer, mrCheckFilter, param);

				List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM> lstMrCheckItem = null;
				List < MOS.EFMODEL.DataModels.HIS_MR_CHECKLIST > lstMrCheckList = null;
				if (lstCheckSummary != null && lstCheckSummary.Count > 0)
				{
					var lstCheckSummaryTemp = lstCheckSummary;
					if (lstCheckSummaryTemp != null && lstCheckSummaryTemp.Count > 0) {

						HisMrCheckItemFilter Itemfilter = new HisMrCheckItemFilter();
						Itemfilter.CHECK_ITEM_TYPE_IDs = lstMrCheckItemType.Select(o => o.ID).ToList();
						Itemfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
						lstMrCheckItem = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM>>("api/HisMrCheckItem/Get", ApiConsumer.ApiConsumers.MosConsumer, Itemfilter, param);
						if (lstMrCheckItem != null && lstMrCheckItem.Count > 0)
						{
							HisMrChecklistFilter filter = new HisMrChecklistFilter();
							filter.MR_CHECK_ITEM_IDs = lstMrCheckItem.Select(o => o.ID).ToList();
							filter.MR_CHECK_SUMMARY_IDs = lstCheckSummaryTemp.Select(o=>o.ID).ToList();
							lstMrCheckList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECKLIST>>("api/HisMrCheckList/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
						}
					}
				}

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatment), currentTreatment));
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCheckSummary), lstCheckSummary));
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMrCheckList), lstMrCheckList));
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMrCheckItem), lstMrCheckItem));
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMrCheckItemType), lstMrCheckItemType));
				MPS.Processor.Mps000472.PDO.Mps000472PDO pdo = new MPS.Processor.Mps000472.PDO.Mps000472PDO(
					currentTreatment,
					lstCheckSummary,
					lstMrCheckList,
					lstMrCheckItem,
					lstMrCheckItemType
					);
				WaitingManager.Hide();
				string printerName = "";
				if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
				{
					printerName = GlobalVariables.dicPrinter[printTypeCode];
				}

				Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.currentTreatment.TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);

				if (ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW") == "1")
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
				}
				else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
				}
				else
				{
					result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
				}


			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
				WaitingManager.Hide();
			}
		}
	}
}
