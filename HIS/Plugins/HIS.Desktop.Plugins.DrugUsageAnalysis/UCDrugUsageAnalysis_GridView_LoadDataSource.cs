using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<L_HIS_TREATMENT_BED_ROOM>> apiResult = null;
                MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                SetFilter(ref treatFilter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();
                //
                //if (gridControl1.DataSource != null)
                //{
                //    foreach (GridColumn item in gridView1.Columns)
                //    {
                //        item.BestFit();
                //    }
                //}
                //
                this.treeListDateTime.DataSource = null;
                //
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisTreatmentBedRoomLViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "IN_DATE ";
                filter.TREATMENT_TYPE_ID = 3;
                // Mã bệnh nhân (PATIENT_CODE) - pad left 12
                if (!string.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = code.PadLeft(10, '0');
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE = code;
                }
                // Mã điều trị (TREATMENT_CODE) - pad left 12
                if (!string.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = code.PadLeft(12, '0');
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE = code;
                }

                // Từ khóa tìm kiếm (KEY_WORD)
                if (!string.IsNullOrWhiteSpace(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }

                // Khoa điều trị (DEPARTMENT_IDs) - multi-select, comma separated
                if (this.DepartmentSelecteds != null && this.DepartmentSelecteds.Count > 0)
                    filter.DEPARTMENT_IDs = this.DepartmentSelecteds.Select(o => o.ID.ToString()).ToList();

                // Buồng bệnh (BED_ROOM_IDs) - multi-select, comma separated
                if (this.BedRoomSelecteds != null && this.BedRoomSelecteds.Count > 0)
                    filter.BED_ROOM_IDs = this.BedRoomSelecteds.Select(o => o.ID).ToList();

                // Đối tượng bệnh nhân (PATIENT_TYPE_IDs) - multi-select, comma separated
                if (this.PatientTypeSelecteds != null && this.PatientTypeSelecteds.Count > 0)
                    filter.PATIENT_TYPE_IDs = this.PatientTypeSelecteds.Select(o => o.ID).ToList();

                if (string.IsNullOrEmpty(filter.PATIENT_CODE) && string.IsNullOrEmpty(filter.TREATMENT_CODE))
                {
                    // Ngày vào viện từ (IN_DATE_FROM)
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    else
                    {
                        filter.IN_DATE_FROM = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                    }
                    // Ngày vào viện đến (IN_DATE_TO)
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                    }
                    else
                    {
                        filter.IN_DATE_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "235959");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        RefeshReference refeshReference { get; set; }
        private void LoadDicRefresh()
        {
            try
            {
                if (GlobalVariables.DicRefreshData == null)
                {
                    GlobalVariables.DicRefreshData = new Dictionary<string, RefeshReference>();
                }
                GlobalVariables.DicRefreshData.Add(currentModule.RoomId.ToString(), (HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.DicRefreshData),
                    GlobalVariables.DicRefreshData));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
