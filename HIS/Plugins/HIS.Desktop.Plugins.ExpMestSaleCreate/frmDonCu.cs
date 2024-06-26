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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public delegate void ChooseDonCu(List<V_HIS_EXP_MEST> listData);

    public partial class frmDonCu : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        bool? IsTreatmentCode;
        string code;
        ChooseDonCu ChooseDonCu;



        public frmDonCu(bool? isCode, string _Code, Inventec.Desktop.Common.Modules.Module module, ChooseDonCu choose)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.ChooseDonCu = choose;
                this.IsTreatmentCode = isCode;
                this.code = _Code;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDonCu_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                if (IsTreatmentCode == true)
                {
                    txtTreatmentCode.Text = code;
                }
                else if (IsTreatmentCode == false)
                {
                    txtPatientCode.Text = code;
                }
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {
                dtCreateTimeFrom.EditValue = DateTime.Now;
                dtCreateTimeTo.EditValue = DateTime.Now;
                txtKeyWord.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
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
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_EXP_MEST>> apiResult = null;
                HisExpMestViewFilter filter = new HisExpMestViewFilter();

                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon)
                .GetRO<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

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

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        private void SetFilter(ref HisExpMestViewFilter filter)
        {
            try
            {
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TDL_TREATMENT_CODE__EXACT = txtTreatmentCode.Text;

                }
                else
                {
                    if (!string.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtPatientCode.Text = code;
                        }
                        filter.TDL_PATIENT_CODE__EXACT = txtPatientCode.Text;
                    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                filter.KEY_WORD = txtKeyWord.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                        {
                            e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB.ToString());

                    }
                    else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                    {
                        if (data.TDL_INTRUCTION_TIME != null)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                        }
                        else
                        {
                            e.Value = "";
                        }

                    }
                    else if (e.Column.FieldName == "REQ_LOGINNAME_USERMAME_STR")
                    {
                        try
                        {
                            e.Value = data.REQ_LOGINNAME + (String.IsNullOrEmpty(data.REQ_USERNAME) ? "" : " - " + data.REQ_USERNAME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                List<V_HIS_EXP_MEST> lstData = new List<V_HIS_EXP_MEST>();
                var rowSelect = gridView1.GetSelectedRows();
                Inventec.Common.Logging.LogSystem.Debug("_______Chọn " + rowSelect.Count() + " dòng");
                if (rowSelect.Count() > 0)
                {
                    if (rowSelect.Count() > 1)
                    {
                        for (int i = 0; i < rowSelect.Count(); i++)
                        {
                            if (i < rowSelect.Count() - 1)
                            {
                                var currentRow = (V_HIS_EXP_MEST)gridView1.GetRow(rowSelect[i]);
                                var nextRow = (V_HIS_EXP_MEST)gridView1.GetRow(rowSelect[i + 1]);
                                if (currentRow.TDL_PATIENT_ID != nextRow.TDL_PATIENT_ID)
                                {
                                    count++;
                                }
                            }
                        }
                        if (count > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn đang chọn đơn thuốc của các bệnh nhân khác nhau. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                        foreach (var item in rowSelect)
                        {
                            var row = (V_HIS_EXP_MEST)gridView1.GetRow(item);
                            if (row != null)
                            {
                                if (!lstData.Exists(o => o.ID == row.ID))
                                {
                                    lstData.Add(row);
                                }

                            }
                        }
                    }
                    else
                    {
                        var rowCount = (V_HIS_EXP_MEST)gridView1.GetRow(rowSelect[0]);
                        if (!lstData.Exists(o => o.ID == rowCount.ID))
                        {
                            lstData.Add(rowCount);
                        }
                    }
                    ChooseDonCu(lstData);
                    this.Close();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn đơn nào", "Thông báo");

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnOK_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
