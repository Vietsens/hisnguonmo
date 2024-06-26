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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.FinancePeriod
{
    public partial class UCFinancePeriod : HIS.Desktop.Utility.UserControlBase
    {
        private void LoadComboBranch()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBranch, data, controlEditorADO);

                cboBranch.EditValue = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridFinancePeriod()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    if (numPageSize == 0)
                        numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
                }

                FillDataToGridFinancePeriod(new CommonParam(0, (int)numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridFinancePeriod, param, numPageSize, gridControlFinancePeriod);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridFinancePeriod(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_FINANCE_PERIOD>> apiResult = new ApiResultObject<List<V_HIS_FINANCE_PERIOD>>();
                HisFinancePeriodViewFilter hisFinancePeriodFilter = new HisFinancePeriodViewFilter();
                //hisServiceReqFilter.HAS_NO_WITHDRAW = true; can check lai
                hisFinancePeriodFilter.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBranch.EditValue.ToString());
                hisFinancePeriodFilter.ORDER_FIELD = "ID";
                hisFinancePeriodFilter.ORDER_DIRECTION = "DESC";

                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_FINANCE_PERIOD>>("api/HisFinancePeriod/GetView", ApiConsumers.MosConsumer, hisFinancePeriodFilter, paramCommon);

                gridControlFinancePeriod.DataSource = null;

                if (apiResult != null)
                {
                    var financePeriods = (List<V_HIS_FINANCE_PERIOD>)apiResult.Data;
                    rowCount = (financePeriods == null ? 0 : financePeriods.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {
                        gridControlFinancePeriod.DataSource = financePeriods;
                    }
                }
                #region Process has exception
                SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion

                gridViewFinancePeriod.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewFinancePeriod.OptionsSelection.EnableAppearanceFocusedRow = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
    }
}
