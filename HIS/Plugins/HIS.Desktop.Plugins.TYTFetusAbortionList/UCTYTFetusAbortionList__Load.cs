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
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using TYT.Desktop.Plugins.FetusAbortionList.Base;
using Inventec.Desktop.Common.Message;
using TYT.EFMODEL.DataModels;
using TYT.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;

namespace TYT.Desktop.Plugins.FetusAbortionList
{
    public partial class UCTYTFetusAbortionList : HIS.Desktop.Utility.UserControlBase
    {
        private void LoadDataToDefault()
        {
            dtTimeFrom.Properties.VistaDisplayMode = DefaultBoolean.True;
            dtTimeFrom.Properties.VistaEditTime = DefaultBoolean.True;
            dtTimeTo.Properties.VistaDisplayMode = DefaultBoolean.True;
            dtTimeTo.Properties.VistaEditTime = DefaultBoolean.True;
            dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
            dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);

            txtKeyWord.Text = "";
            txtPatientCode.Text = "";
            txtPersonCode.Text = "";

        }

        private void FillDataToControl()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__NUM_PAGESIZE);
                }

                FillDataToGrid(new CommonParam(0, (int)numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGrid, param, numPageSize, gridControlFetusAbortion);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        internal void FillDataToGrid(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<TYT_FETUS_ABORTION>> apiResult = new ApiResultObject<List<TYT_FETUS_ABORTION>>();
                TytFetusAbortionFilter filter = new TytFetusAbortionFilter();

                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtPersonCode.Text))
                {
                    string code = txtPersonCode.Text.Trim();
                    if (code.Length < 9 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000}", Convert.ToInt64(code));
                        txtPersonCode.Text = code;
                    }
                    filter.PERSON_CODE__EXACT = code;
                }

                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (dtTimeFrom != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtTimeTo != null && dtTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                WaitingManager.Show();
                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<TYT_FETUS_ABORTION>>("api/TytFetusAbortion/Get", ApiConsumers.TytConsumer, filter, paramCommon);
                WaitingManager.Hide();
                gridControlFetusAbortion.DataSource = null;

                if (apiResult != null)
                {
                    rowCount = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {
                        gridControlFetusAbortion.DataSource = apiResult.Data;
                    }
                }
                gridViewFetusAbortion.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewFetusAbortion.OptionsSelection.EnableAppearanceFocusedRow = false;

                //transitionManager1.EndTransition();
                
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
    }
}
