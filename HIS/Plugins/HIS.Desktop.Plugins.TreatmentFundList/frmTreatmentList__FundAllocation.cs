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
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TreatmentFundList
{
    /// <summary>
    /// Fund allocation info (HIS_FUND_ALLOCATION, current version) shown on the treatment list
    /// and the "Allocate" button opening the module HIS.Desktop.Plugins.FundAllocation.
    /// </summary>
    public partial class frmTreatmentList
    {
        /// <summary>Module link of the fund allocation plugin</summary>
        private const string MODULE_LINK__FUND_ALLOCATION = "HIS.Desktop.Plugins.FundAllocation";

        /// <summary>Value of the flags IS_FINISH / IS_COST_CHANGED of HIS_FUND_ALLOCATION meaning "yes"</summary>
        private const short FUND_ALLOCATION_FLAG__TRUE = 1;

        /// <summary>
        /// Load the current fund allocation of the treatments on the page (one API call)
        /// and fill the allocation columns of each row.
        /// </summary>
        private void ProcessFundAllocationInfo(List<V_HIS_TREATMENT_FEE_2D> listTreatment)
        {
            try
            {
                if (listTreatment == null || listTreatment.Count == 0)
                    return;

                List<long> treatmentIds = listTreatment.Select(o => o.ID).Distinct().ToList();
                Dictionary<long, V_HIS_FUND_ALLOCATION> dicAllocation = LoadCurrentFundAllocation(treatmentIds);
                if (dicAllocation == null)
                {
                    //API failed: leave the allocation columns empty instead of showing a wrong status
                    return;
                }

                string statusNotAllocated = GetLanguageValue("frmTreatmentList.FundAllocationStatus.NotAllocated");
                string statusSaved = GetLanguageValue("frmTreatmentList.FundAllocationStatus.Saved");
                string statusFinished = GetLanguageValue("frmTreatmentList.FundAllocationStatus.Finished");
                string statusCostChanged = GetLanguageValue("frmTreatmentList.FundAllocationStatus.CostChanged");

                foreach (var item in listTreatment)
                {
                    V_HIS_FUND_ALLOCATION allocation = null;
                    if (dicAllocation.TryGetValue(item.ID, out allocation) && allocation != null)
                    {
                        bool isCostChanged = (allocation.IS_COST_CHANGED == FUND_ALLOCATION_FLAG__TRUE);
                        string status = (allocation.IS_FINISH == FUND_ALLOCATION_FLAG__TRUE) ? statusFinished : statusSaved;
                        if (isCostChanged)
                        {
                            status += statusCostChanged;
                        }

                        item.FUND_ALLOCATION_STATUS = status;
                        item.FUND_ALLOCATION_FINISH_USERNAME = allocation.FINISH_USERNAME;
                        item.FUND_ALLOCATION_FINISH_TIME_STR = allocation.FINISH_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(allocation.FINISH_TIME.Value) : "";
                        item.FUND_ALLOCATION_TOTAL_FUND_PRICE = allocation.TOTAL_FUND_PRICE;
                        item.IS_FUND_ALLOCATION_COST_CHANGED = isCostChanged;
                    }
                    else
                    {
                        item.FUND_ALLOCATION_STATUS = statusNotAllocated;
                        item.FUND_ALLOCATION_FINISH_USERNAME = null;
                        item.FUND_ALLOCATION_FINISH_TIME_STR = null;
                        item.FUND_ALLOCATION_TOTAL_FUND_PRICE = null;
                        item.IS_FUND_ALLOCATION_COST_CHANGED = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Get the current fund allocation (IS_CURRENT = true) of the given treatments, keyed by TREATMENT_ID.
        /// Returns null when the API call failed.
        /// </summary>
        private Dictionary<long, V_HIS_FUND_ALLOCATION> LoadCurrentFundAllocation(List<long> treatmentIds)
        {
            Dictionary<long, V_HIS_FUND_ALLOCATION> result = new Dictionary<long, V_HIS_FUND_ALLOCATION>();
            try
            {
                if (treatmentIds == null || treatmentIds.Count == 0)
                    return result;

                CommonParam param = new CommonParam();
                HisFundAllocationViewFilter filter = new HisFundAllocationViewFilter();
                filter.TREATMENT_IDs = treatmentIds;
                filter.IS_CURRENT = true;
                List<V_HIS_FUND_ALLOCATION> allocations = new BackendAdapter(param).Get<List<V_HIS_FUND_ALLOCATION>>(RequestUri.HIS_FUND_ALLOCATION_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                if (allocations == null)
                {
                    if (param.HasException || (param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Call api " + RequestUri.HIS_FUND_ALLOCATION_GETVIEW + " failed. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        return null;
                    }
                    return result;
                }

                foreach (var item in allocations)
                {
                    V_HIS_FUND_ALLOCATION existed = null;
                    //Only one current version per treatment is expected; keep the latest version if the data is inconsistent
                    if (!result.TryGetValue(item.TREATMENT_ID, out existed) || existed == null || item.VERSION_NUMBER > existed.VERSION_NUMBER)
                    {
                        result[item.TREATMENT_ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void repositoryItembtnFundAllocation_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle < 0)
                    return;

                var data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                if (data == null)
                    return;

                List<object> listArgs = new List<object>();
                listArgs.Add(data.ID);
                listArgs.Add((RefeshReference)RefreshAfterFundAllocation);

                long roomId = (this.currentModule != null) ? this.currentModule.RoomId : 0;
                long roomTypeId = (this.currentModule != null) ? this.currentModule.RoomTypeId : 0;
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(MODULE_LINK__FUND_ALLOCATION, roomId, roomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Callback of the fund allocation module: reload the current page of the list.
        /// </summary>
        private void RefreshAfterFundAllocation()
        {
            try
            {
                if (this.IsDisposed)
                    return;

                WaitingManager.Show();
                FillDataToGridTreatment(new CommonParam(start, limit));
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
