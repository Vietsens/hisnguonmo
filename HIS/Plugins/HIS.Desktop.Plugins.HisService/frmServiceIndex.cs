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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisService
{
    public partial class frmServiceIndex : Form
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<List<V_HIS_SUIM_INDEX>> SendData { get; set; }
        List<V_HIS_SUIM_INDEX> listHisSuimIndex;
        List<V_HIS_SUIM_INDEX> listHisSuimIndexSource;
        List<HIS_SUIM_INDEX> lstSource;
        string code { get; set; }
        string name { get; set; }
        #endregion
        public frmServiceIndex(Action<List<V_HIS_SUIM_INDEX>> SendData, List<HIS_SUIM_INDEX> lstSource, string code, string name)
        {
            InitializeComponent();

            pagingGrid = new PagingGrid();
            this.moduleData = moduleData;
            this.SendData = SendData;
            this.lstSource = lstSource;
            this.code = code;
            this.name = name;
            //gridControlFormList.ToolTipController = toolTipControllerGrid;

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmServiceIndex_Load(object sender, EventArgs e)
        {

            try
            {
                lblTitle.Text = code + " - " + name;
                CommonParam param = new CommonParam();

                listHisSuimIndexSource = new List<V_HIS_SUIM_INDEX>();
                if (lstSource != null && lstSource.Count > 0)
                {
                    HisSuimIndexUnitFilter SetyFilter = new HisSuimIndexUnitFilter();
                    SetyFilter.IDs = lstSource.Select(o => o.SUIM_INDEX_UNIT_ID ?? 0).ToList();
                    var listHisSuimSety = new BackendAdapter(param).Get<List<HIS_SUIM_INDEX_UNIT>>("api/HisSuimIndexUnit/Get", ApiConsumers.MosConsumer, SetyFilter, null).ToList();
                    foreach (var item in lstSource)
                    {
                        V_HIS_SUIM_INDEX updateDTO = new V_HIS_SUIM_INDEX();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SUIM_INDEX>(updateDTO, item);
                        if (item.SUIM_INDEX_UNIT_ID != null)
                        {
                            var unit = listHisSuimSety.FirstOrDefault(o => o.ID == item.SUIM_INDEX_UNIT_ID);
                            updateDTO.SUIM_INDEX_UNIT_CODE = unit != null ? unit.SUIM_INDEX_UNIT_CODE : null;
                            updateDTO.SUIM_INDEX_UNIT_NAME = unit != null ? unit.SUIM_INDEX_UNIT_NAME : null;
                        }
                        listHisSuimIndexSource.Add(updateDTO);
                    }
                }
                gridControl2.DataSource = listHisSuimIndexSource;

                FillDataToGrid();


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
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridIndex(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridIndex, param, pageSize, gridControl1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridIndex(object param)
        {
            try
            {

                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                listHisSuimIndex = new List<V_HIS_SUIM_INDEX>();
                HisSuimIndexFilter filter = new HisSuimIndexFilter();
                filter.KEY_WORD = txtKey.Text.Trim();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SUIM_INDEX>>("api/HisSuimIndex/GetView", ApiConsumers.MosConsumer, filter, null);
                if (result != null)
                {
                    listHisSuimIndex = result.Data;
                    rowCount = (listHisSuimIndex == null ? 0 : listHisSuimIndex.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControl1.DataSource = listHisSuimIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGrid();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FillDataToGrid();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null,null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                SendData(listHisSuimIndexSource);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            try
            {
                int[] rows = gridView1.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    var data = (MOS.EFMODEL.DataModels.V_HIS_SUIM_INDEX)gridView1.GetRow(rows[i]);
                    if (listHisSuimIndexSource.FirstOrDefault(o=>o.ID == data.ID) == null)
                        listHisSuimIndexSource.Add(data);
                }
                this.gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                V_HIS_SUIM_INDEX ssADO = (V_HIS_SUIM_INDEX)gridView2.GetFocusedRow();
                if (ssADO != null)
                {
                    listHisSuimIndexSource.Remove(ssADO);
                    this.gridControl2.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
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
