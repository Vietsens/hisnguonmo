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
using LIS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SampleRoomPartialKXN.Entity;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.SampleRoomPartialKXN
{
    public partial class UCSampleRoomPartialKXN : UserControl
    {
        internal List<LIS_SAMPLE> currentSample { get; set; }
        internal List<V_HIS_TEST_SERVICE_REQ> lstTestServiceReq { get; set; }
        internal List<string> listServiceReqCodes = new List<string>();

        public void MeShow()
        {
            try
            {
                SetDefaultData();
                LoadComboStatus();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Tất cả"));
                status.Add(new Status(2, "Đã lấy mẫu"));
                status.Add(new Status(3, "Chưa lấy mẫu"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboFind, status, controlEditorADO);
                cboFind.EditValue = status[2].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                txtSearchKey.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridTestServiceReq(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTestServiceReq, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTestServiceReq(object data)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);

                MOS.Filter.HisTestServiceReqViewFilter hisTestServiceReqFilter = new HisTestServiceReqViewFilter();

                if (dtCreatefrom != null && dtCreatefrom.DateTime != DateTime.MinValue)
                    hisTestServiceReqFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreatefrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtCreateTo != null && dtCreateTo.DateTime != DateTime.MinValue)
                    hisTestServiceReqFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMddHHmm") + "00");

                hisTestServiceReqFilter.KEY_WORD = txtSearchKey.Text;
                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Filter yeu cau chua lấy mẫu
                if (cboFind.EditValue != null)
                {
                    //Chưa lấy mẫu
                    if ((long)cboFind.EditValue == 3)
                    {
                        hisTestServiceReqFilter.IS_SPECIMEN = false;
                    }
                    //Đã lấy mẫu
                    else if ((long)cboFind.EditValue == 2)
                    {
                        hisTestServiceReqFilter.IS_SPECIMEN = true;
                    }
                    //Tất cả
                    else
                    {
                        hisTestServiceReqFilter.IS_SPECIMEN = null;
                    }
                }

                var sar = new BackendAdapter(param).GetRO<List<V_HIS_TEST_SERVICE_REQ>>(
                    "api/HisTestServiceReq/GetView", ApiConsumers.MosConsumer, hisTestServiceReqFilter, param);
                if (sar != null)
                {
                    var dataTest = (List<V_HIS_TEST_SERVICE_REQ>)sar.Data;
                    if (dataTest != null && dataTest.Count > 0)
                    {
                        currentLisSample = new LIS_SAMPLE();
                        currentLisSample.SAMPLE_ROOM_CODE = dataTest[0].SAMPLE_ROOM_CODE;
                        currentLisSample.SAMPLE_ROOM_NAME = dataTest[0].SAMPLE_ROOM_NAME;
                        currentLisSample.SERVICE_REQ_CODE = dataTest[0].SERVICE_REQ_CODE;
                        currentLisSample.EXECUTE_ROOM_CODE = dataTest[0].EXECUTE_ROOM_CODE;
                        currentLisSample.EXECUTE_ROOM_NAME = dataTest[0].EXECUTE_ROOM_NAME;
                        currentLisSample.PATIENT_CODE = dataTest[0].PATIENT_CODE;
                        gridControlTestServiceReq.DataSource = dataTest;
                        gridViewTestServiceReq.FocusedRowHandle = 0;
                        LoadDataToGridSereServ(dataTest[0]);
                        LoadDataGridSample();

                        rowCount = (dataTest == null ? 0 : dataTest.Count);
                        dataTotal = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataGridSample()
        {
            try
            {
                WaitingManager.Show();
                listServiceReqCodes = new List<string>();
                CommonParam param = new CommonParam();

                var currentTestSerReq = (V_HIS_TEST_SERVICE_REQ)gridViewTestServiceReq.GetFocusedRow();
                listServiceReqCodes.Add(currentTestSerReq.SERVICE_REQ_CODE);

                LIS.Filter.LisSampleFilter sampleFilter = new LIS.Filter.LisSampleFilter();
                sampleFilter.SERVICE_REQ_CODEs = listServiceReqCodes;
                var listSample = new BackendAdapter(param).Get<List<LIS_SAMPLE>>(
                    "api/LisSample/Get", ApiConsumers.LisConsumer, sampleFilter, param);
                if (listSample != null)
                {
                    gridControlSimple.DataSource = listSample;
                    gridControlBarCode.DataSource = listSample;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridSereServ(V_HIS_TEST_SERVICE_REQ row)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_ID = row.SERVICE_REQ_ID;
                var sereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(
                    "api/HisSereServ/GetView", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServ != null)
                {
                    gridControlSereServ.DataSource = sereServ;
                }
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
