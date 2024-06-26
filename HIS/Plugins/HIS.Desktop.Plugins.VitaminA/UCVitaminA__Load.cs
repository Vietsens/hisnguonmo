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
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.VitaminA.ADO;

namespace HIS.Desktop.Plugins.VitaminA
{
    public partial class UCVitaminA : UserControl
    {

        private void InitDataControl()
        {
            try
            {

                dtRequestTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtRequestTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                cboStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridVitaminA()
        {
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
            }

            FillDataToGridVitaminA(new CommonParam(0, (int)numPageSize));
            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridVitaminA, param, numPageSize, gridControlVitaminA);
        }

        internal void FillDataToGridVitaminA(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_VITAMIN_A>> apiResult = new ApiResultObject<List<V_HIS_VITAMIN_A>>();
                HisVitaminAViewFilter hisVitaminAFilter = new HisVitaminAViewFilter();
                //hisServiceReqFilter.HAS_NO_WITHDRAW = true; can check lai
                hisVitaminAFilter.EXECUTE_ROOM_ID = roomId;
                hisVitaminAFilter.KEY_WORD = txtKeyword.Text.Trim();

                if (dtRequestTimeFrom != null && dtRequestTimeFrom.DateTime != DateTime.MinValue)
                    hisVitaminAFilter.REQUEST_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtRequestTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtRequestTimeTo != null && dtRequestTimeTo.DateTime != DateTime.MinValue)
                    hisVitaminAFilter.REQUEST_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtRequestTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                //Chua uong : 0
                //Da uong : 1
                //Tat ca : 2

                if (cboStatus.EditValue != null)
                {
                    switch (cboStatus.SelectedIndex)
                    {
                        case 0:
                            hisVitaminAFilter.VITAMIN_STT = MOS.Filter.HisVitaminAViewFilter.VITAMIN_STT_ENUM.NOT_DRINK;
                            break;
                        case 1:
                            hisVitaminAFilter.VITAMIN_STT = MOS.Filter.HisVitaminAViewFilter.VITAMIN_STT_ENUM.DRINK;
                            break;
                        case 2:
                            hisVitaminAFilter.VITAMIN_STT = null;
                            break;
                    }
                }

                hisVitaminAFilter.ORDER_FIELD = "REQUEST_TIME";
                hisVitaminAFilter.ORDER_DIRECTION = "DESC";
                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumers.MosConsumer, hisVitaminAFilter, paramCommon);

                gridControlVitaminA.DataSource = null;
                if (apiResult != null)
                {
                    rowCount = (apiResult.Data == null ? 0 : apiResult.Data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlVitaminA.DataSource = apiResult.Data;
                }

                gridViewVitaminA.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewVitaminA.OptionsSelection.EnableAppearanceFocusedRow = false;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadVitaminAHistory(long patientId)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisVitaminAViewFilter filter = new HisVitaminAViewFilter();
                filter.PATIENT_ID = patientId;
                filter.VITAMIN_STT = MOS.Filter.HisVitaminAViewFilter.VITAMIN_STT_ENUM.OUT_OF_STOCK;
                filter.ORDER_FIELD = "REQUEST_TIME";
                filter.ORDER_DIRECTION = "DESC";
                List<V_HIS_VITAMIN_A> listVitaminAHistory = new BackendAdapter(param)
                    .Get<List<V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumers.MosConsumer, filter, param);
                gridControlVitaminAHistory.DataSource = listVitaminAHistory;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfoVitaminAPatient(V_HIS_VITAMIN_A vitaminA)
        {
            try
            {
                if (vitaminA != null)
                {
                    lblPatientName.Text = vitaminA.TDL_PATIENT_NAME;
                    lblPatientCode.Text = vitaminA.TDL_PATIENT_CODE;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(vitaminA.TDL_PATIENT_DOB);
                    lblGender.Text = vitaminA.TDL_PATIENT_GENDER_NAME;
                    lblCaseType.Text = vitaminA.CASE_TYPE == 1 ? "Trẻ em" : vitaminA.CASE_TYPE == 2 ? "Ba mẹ" : "Khác";
                    string note = vitaminA.IS_SICK == 1 ? "Bị bệnh thiếu VitaminA" : "";
                    note = note + (vitaminA.IS_ONE_MONTH_BORN == 1 ? "Sau sinh 1 tháng" : "");
                    lblNote.Text = note;
                    LoadVitaminAHistory(vitaminA.PATIENT_ID);
                }
                else
                {
                    lblPatientName.Text = null;
                    lblPatientCode.Text = null;
                    lblDob.Text = null;
                    lblGender.Text = null;
                    gridControlVitaminAHistory.DataSource = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
