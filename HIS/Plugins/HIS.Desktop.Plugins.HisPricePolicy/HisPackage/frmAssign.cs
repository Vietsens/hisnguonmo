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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.HisPricePolicy.HisPackage
{
    public partial class frmAssign : Form
    {
        #region Reclare
        HIS_PACKAGE curentHisPackage;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        long _PackageID;
        List<HIS_PACKAGE_DETAIL> checkID;
        List<HIS_SERVICE> Servicecheck;

        decimal Amount = 1;
        //long serviceID;

        #endregion
        public frmAssign(long packageID) : this(null, packageID) { }

        public frmAssign(HIS_PACKAGE HisPackage, long packageID)
        {
            InitializeComponent();
            this._PackageID = packageID;
            this.curentHisPackage = HisPackage;

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAssign_Load(object sender, EventArgs e)
        {
            try
            {
                LoadcboServiceType();
                FillDataToControl();
                SetDefaultValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtSearch.Text = null;
                cboServiceType.EditValue = null;
                cboServiceType.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                //load dữ liệu cá dịch vụ được check
                CommonParam param1 = new CommonParam();
                HisPackageDetailFilter filter1 = new HisPackageDetailFilter();
                filter1.PACKAGE_ID = _PackageID;
                checkID = new List<HIS_PACKAGE_DETAIL>();
                checkID = new BackendAdapter(param1).Get<List<HIS_PACKAGE_DETAIL>>("/api/HisPackageDetail/Get", ApiConsumers.MosConsumer, filter1, param1);

                Servicecheck = new List<HIS_SERVICE>();
                CommonParam param2 = new CommonParam();
                HisServiceFilter filter2 = new HisServiceFilter();
                filter2.IDs = checkID.Select(o => o.SERVICE_ID).ToList();
                Servicecheck = new BackendAdapter(param2).Get<List<HIS_SERVICE>>("/api/HisService/Get", ApiConsumers.MosConsumer, filter2, param2);


                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE>> apiResult = null;
                HisServiceFilter filter = new HisServiceFilter();
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text;
                }

                if (cboServiceType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = (long)cboServiceType.EditValue;
                }

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.IS_ACTIVE = 1;


                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE>>("/api/HisService/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var dataT = new List<HIS_SERVICE>();
                    var data1 = new List<HIS_SERVICE>();
                    var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            var check = Servicecheck.Where(o => o.ID == item.ID).Count();
                            if (check > 0)
                            {
                                dataT.Add(item);
                            }
                            else
                            {
                                data1.Add(item);
                            }
                        }

                        dataT.AddRange(data1);

                        gridView1.BeginUpdate();
                        gridView1.GridControl.DataSource = dataT;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                        if (checkID != null && checkID.Count > 0)
                        {

                            foreach (var item in checkID)
                            {
                                int rowHandle = gridView1.LocateByValue("ID", item.SERVICE_ID);
                                if (rowHandle != GridControl.InvalidRowHandle)
                                    gridView1.SelectRow(rowHandle);
                            }
                        }
                        gridView1.EndUpdate();
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost(paramCommon);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadcboServiceType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboServiceType, BackendDataWorker.Get<HIS_SERVICE_TYPE>(), controlEditorADO);
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
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.EditValue = null;
                    cboServiceType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboServiceType.Text))
                {
                    cboServiceType.Properties.Buttons[1].Visible = true;
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("btnSave_Click.1");
                HisPackageSDO PackageSDO = new HisPackageSDO();
                bool success = false;
                WaitingManager.Show();

                List<HisPackageDetailSDO> _DetailSDO = new List<HisPackageDetailSDO>();

                List<HIS_SERVICE> roleUserSdo = new List<HIS_SERVICE>();
                List<HIS_SERVICE> checkAdd = new List<HIS_SERVICE>();
                List<HIS_SERVICE> checkDelete = new List<HIS_SERVICE>();

                LogSystem.Debug("btnSave_Click.2");
                var listInt = gridView1.GetSelectedRows();
                var listDataSource = (List<HIS_SERVICE>)gridView1.DataSource;

                LogSystem.Debug("btnSave_Click.3");
                if (listInt != null && listInt.Count() > 0)
                {
                    foreach (var item in listInt)
                    {
                        HIS_SERVICE roleUser = (HIS_SERVICE)gridView1.GetRow(item);
                        roleUserSdo.Add(roleUser);
                    }
                }
                LogSystem.Debug("btnSave_Click.4");
                if (Servicecheck != null)
                {
                    if (listDataSource != null && listDataSource.Count > 0)
                    {
                        List<HIS_SERVICE> notChecks = listDataSource.Where(o => roleUserSdo == null || !roleUserSdo.Any(a => a.ID == o.ID)).ToList();
                        checkDelete = Servicecheck.Where(o => notChecks.Any(a => a.ID == o.ID)).ToList();
                    }
                    checkAdd = roleUserSdo.Where(o => !Servicecheck.Select(p => p.ID).Contains(o.ID)).ToList();
                    if (checkAdd != null && checkAdd.Count > 0)
                    {
                        Servicecheck.AddRange(checkAdd);
                    }

                    if (checkDelete != null && checkDelete.Count > 0)
                    {
                        Servicecheck = Servicecheck.Where(o => !checkDelete.Any(a => a.ID == o.ID)).ToList();
                    }
                }

                LogSystem.Debug("btnSave_Click.5");
                if (Servicecheck != null && Servicecheck.Count > 0)
                {
                    foreach (var item in Servicecheck)
                    {
                        HisPackageDetailSDO data1 = new HisPackageDetailSDO();
                        data1.ServiceId = item.ID;
                        //data1.Amount = Amount;
                        data1.Amount = checkID.Where(o => o.SERVICE_ID == item.ID).FirstOrDefault().AMOUNT;
                        _DetailSDO.Add(data1);
                    }
                    LogSystem.Debug("btnSave_Click.6");
                    PackageSDO.PackageId = _PackageID;
                    PackageSDO.Details = _DetailSDO;
                    CommonParam paramCommon = new CommonParam();
                    success = new BackendAdapter(paramCommon).Post<bool>("/api/HisPackage/UpdateDetail", ApiConsumers.MosConsumer, PackageSDO, paramCommon);
                    LogSystem.Debug("btnSave_Click.7");
                    if (success)
                    {
                        FillDataToControl();
                    }
                    LogSystem.Debug("btnSave_Click.8");
                    #region Hien thi message thong bao
                    WaitingManager.Hide();
                    MessageManager.Show(this, paramCommon, success);
                    #endregion
                }
                else
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Bạn chưa chọn dịch vụ trong gói. Vui lòng kiểm tra lại.");
                }
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                FillDataToControl();
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_SERVICE pData = (MOS.EFMODEL.DataModels.HIS_SERVICE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];


                    if (e.Column.FieldName == "AMOUNT_STR")
                    {
                        var amounts = checkID.Where(o => o.SERVICE_ID == pData.ID).ToList();
                        if (amounts != null && amounts.Count > 0)
                        {
                            e.Value = amounts.Sum(s => s.AMOUNT);
                        }
                        else
                        {
                            e.Value = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //try
            //{
            //    GridView view = sender as GridView;
            //    if (view == null) return;
            //    if (e.Column.Caption != "Số lượng") return;

            //    Amount = (decimal)e.Value;

            //    Inventec.Common.Logging.LogSystem.Warn("Dữ liệu AMOUNT_STR: " + Amount);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_SERVICE data = (HIS_SERVICE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "AMOUNT_STR")
                    {
                        e.RepositoryItem = repositoryItemTextEdit1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemTextEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void repositoryItemTextEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal value = decimal.Parse((sender as TextEdit).EditValue.ToString());
                HIS_SERVICE rowData = (HIS_SERVICE)gridView1.GetFocusedRow();
                // Amount = value;

                var data = checkID.Where(o => o.SERVICE_ID == rowData.ID).ToList();
                if (data != null && data.Count > 0)
                {
                    checkID.Where(o => o.SERVICE_ID == rowData.ID).FirstOrDefault().AMOUNT = value;
                }
                else
                {
                    HIS_PACKAGE_DETAIL packageDetail = new HIS_PACKAGE_DETAIL();
                    packageDetail.AMOUNT = value;
                    packageDetail.SERVICE_ID = rowData.ID;

                    checkID.Add(packageDetail);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == CollectionChangeAction.Add)
                {
                    HIS_SERVICE service = (HIS_SERVICE)gridView1.GetRow(e.ControllerRow);
                    if (service != null)
                    {
                        if (checkID == null || !checkID.Any(a => a.SERVICE_ID == service.ID))
                        {
                            if (checkID == null) checkID = new List<HIS_PACKAGE_DETAIL>();
                            HIS_PACKAGE_DETAIL packageDetail = new HIS_PACKAGE_DETAIL();
                            packageDetail.AMOUNT = 1;
                            packageDetail.SERVICE_ID = service.ID;

                            checkID.Add(packageDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

