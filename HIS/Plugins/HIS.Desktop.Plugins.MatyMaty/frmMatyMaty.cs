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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.MatyMaty.ADO;
using HIS.Desktop.Plugins.MatyMaty.Change;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.MatyMaty
{
    public partial class frmMatyMaty : FormBase
    {
        int matyStart = 0;
        int matyLimit = 0;
        int matyRowCount = 0;
        int matyTotalData = 0;

        int prepaStart = 0;
        int prepaLimit = 0;
        int prepaRowCount = 0;
        int prepaTotalData = 0;

        private List<MaterialTypeADO> ListMaterialType = new List<MaterialTypeADO>();
        private List<MaterialTypeADO> ListPrepaMaterialType = new List<MaterialTypeADO>();

        private MaterialTypeADO currentMateriaType = null;
        private long inputId = 0;
        private List<HIS_MATY_MATY> inputMatyMaty = new List<HIS_MATY_MATY>();
        public List<HIS_SERVICE_UNIT> ListServiceUnit { get; set; }

        public frmMatyMaty(Inventec.Desktop.Common.Modules.Module module, long matyId, List<HIS_MATY_MATY> data)
            : base(module)
        {
            InitializeComponent();
            this.inputId = matyId;
            this.inputMatyMaty = data;
        }

        public frmMatyMaty(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmMatyMaty_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                GetServiceUnit();
                this.SetDefaultValueControl();
                this.FillDataToGridMaty();
                this.FillDataToGridPrepaMaty();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceUnit()
        {
            try
            {
                if (this.ListServiceUnit == null || this.ListServiceUnit.Count == 0)
                {
                    MOS.Filter.HisServiceUnitFilter serviceUnitFilter = new HisServiceUnitFilter();
                    this.ListServiceUnit = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceUnitFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyword.Text = "";
                txtPrepaKeyword.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMaty()
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingMaty.pagingGrid != null)
                {
                    numPageSize = ucPagingMaty.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingMaty(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = matyRowCount;
                param.Count = matyTotalData;
                ucPagingMaty.Init(LoadPagingMaty, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingMaty(object param)
        {
            try
            {
                this.matyStart = ((CommonParam)param).Start ?? 0;
                this.matyLimit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(this.matyStart, this.matyLimit);
                List<HIS_MATERIAL_TYPE> listData = new List<HIS_MATERIAL_TYPE>();
                this.ListMaterialType = new List<MaterialTypeADO>();
                if (this.inputId <= 0)
                {
                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    filter.IS_RAW_MATERIAL = false;
                    filter.KEY_WORD = txtKeyword.Text;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    var rs = new BackendAdapter(paramCommon).GetRO<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (rs != null)
                    {
                        listData = (List<HIS_MATERIAL_TYPE>)rs.Data;
                        if (listData != null)
                        {
                            foreach (var item in listData)
                            {
                                MaterialTypeADO ado = new MaterialTypeADO(item);

                                if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                {
                                    var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                    if (checkServiceUnit != null)
                                        ado.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                }

                                this.ListMaterialType.Add(ado);
                            }

                        }
                        this.matyRowCount = (listData == null ? 0 : listData.Count);
                        this.matyTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                    this.currentMateriaType = null;
                }
                else
                {
                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    filter.IS_RAW_MATERIAL = false;
                    filter.ID = inputId;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    var rs = new BackendAdapter(new CommonParam()).GetRO<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, null);

                    if (rs != null)
                    {
                        listData = (List<HIS_MATERIAL_TYPE>)rs.Data;
                        if (listData != null && listData.Count == 1)
                        {
                            MaterialTypeADO ado = new MaterialTypeADO(listData[0]);

                            if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                            {
                                var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == listData[0].TDL_SERVICE_UNIT_ID);
                                if (checkServiceUnit != null)
                                    ado.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                            }
                            
                            ado.Check = true;
                            this.ListMaterialType.Add(ado);
                            this.currentMateriaType = ado;
                        }
                    }
                }

                gridControlMaty.BeginUpdate();
                gridControlMaty.DataSource = this.ListMaterialType;
                gridControlMaty.EndUpdate();

                this.FillDataCheckToPrepa(true);

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPrepaMaty()
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingPrepaMaty.pagingGrid != null)
                {
                    numPageSize = ucPagingPrepaMaty.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingPrepaMaty(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = prepaRowCount;
                param.Count = prepaTotalData;
                ucPagingPrepaMaty.Init(LoadPagingPrepaMaty, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingPrepaMaty(object param)
        {
            try
            {
                this.prepaStart = ((CommonParam)param).Start ?? 0;
                this.prepaLimit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(this.prepaStart, this.prepaLimit);
                List<HIS_MATERIAL_TYPE> listData = new List<HIS_MATERIAL_TYPE>();
                this.ListPrepaMaterialType = new List<MaterialTypeADO>();
                if (inputId <= 0)
                {
                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    filter.IS_RAW_MATERIAL = true;
                    filter.KEY_WORD = txtPrepaKeyword.Text;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    var rs = new BackendAdapter(paramCommon).GetRO<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (rs != null)
                    {
                        listData = (List<HIS_MATERIAL_TYPE>)rs.Data;
                        if (listData != null)
                        {
                            foreach (var item in listData)
                            {
                                MaterialTypeADO ado = new MaterialTypeADO(item);
                                if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                {
                                    var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                    if (checkServiceUnit != null)
                                        ado.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                }
                               
                                this.ListPrepaMaterialType.Add(ado);
                            }

                        }
                        this.prepaRowCount = (listData == null ? 0 : listData.Count);
                        this.prepaTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                }
                else
                {
                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    filter.IS_RAW_MATERIAL = true;
                    filter.KEY_WORD = txtPrepaKeyword.Text;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    var rs = new BackendAdapter(paramCommon).GetRO<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (rs != null)
                    {
                        listData = (List<HIS_MATERIAL_TYPE>)rs.Data;
                        if (listData != null)
                        {
                            foreach (var item in listData)
                            {
                                MaterialTypeADO ado = new MaterialTypeADO(item);

                                if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                                {
                                    var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                                    if (checkServiceUnit != null)
                                        ado.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                                }
                                this.ListPrepaMaterialType.Add(ado);
                            }

                        }
                        this.prepaRowCount = (listData == null ? 0 : listData.Count);
                        this.prepaTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                }
                this.ListPrepaMaterialType = this.ListPrepaMaterialType.OrderByDescending(o => o.Check).ToList();
                gridControlPrepaMaty.BeginUpdate();
                gridControlPrepaMaty.DataSource = this.ListPrepaMaterialType;
                gridControlPrepaMaty.EndUpdate();

                this.FillDataCheckToPrepa(true);
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridMaty();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MaterialTypeADO pData = (MaterialTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + this.matyStart;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrepaKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnPrepaFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrepaFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrepaFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridPrepaMaty();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPrepaMaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MaterialTypeADO pData = (MaterialTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + this.prepaStart;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewPrepaMaty.PostEditor();
                if (!btnSave.Enabled) return;
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.currentMateriaType == null)
                {
                    XtraMessageBox.Show("Bạn chưa chọn vật tư thành phẩm", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                List<string> notAmounts = this.ListPrepaMaterialType.Where(o => o.Check && o.AMOUNT <= 0).Select(s => s.MATERIAL_TYPE_CODE).ToList();
                if (notAmounts != null && notAmounts.Count > 0)
                {
                    string message = String.Format("Các vật tư chế phẩm có mã sau chưa nhập số lượng: {0}", String.Join(",", notAmounts));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                List<HIS_MATY_MATY> olds = this.GetMatyMatyByMaterialType(this.currentMateriaType.ID);
                List<HIS_MATY_MATY> updates = new List<HIS_MATY_MATY>();
                List<HIS_MATY_MATY> creates = new List<HIS_MATY_MATY>();
                List<HIS_MATY_MATY> deletes = new List<HIS_MATY_MATY>();

                foreach (var prepa in this.ListPrepaMaterialType)
                {
                    HIS_MATY_MATY ol = olds != null ? olds.FirstOrDefault(o => o.PREPARATION_MATERIAL_TYPE_ID == prepa.ID) : null;
                    if (ol != null)
                    {
                        if (prepa.Check && prepa.AMOUNT != ol.PREPARATION_AMOUNT)
                        {
                            HIS_MATY_MATY up = new HIS_MATY_MATY();
                            up.ID = ol.ID;
                            up.MATERIAL_TYPE_ID = ol.MATERIAL_TYPE_ID;
                            up.PREPARATION_MATERIAL_TYPE_ID = ol.PREPARATION_MATERIAL_TYPE_ID;
                            up.PREPARATION_AMOUNT = prepa.AMOUNT;
                            updates.Add(up);
                        }
                        else if (!prepa.Check)
                        {
                            deletes.Add(ol);
                        }
                    }
                    else if (prepa.Check)
                    {
                        HIS_MATY_MATY cre = new HIS_MATY_MATY();
                        cre.MATERIAL_TYPE_ID = this.currentMateriaType.ID;
                        cre.PREPARATION_MATERIAL_TYPE_ID = prepa.ID;
                        cre.PREPARATION_AMOUNT = prepa.AMOUNT;
                        creates.Add(cre);
                    }
                }

                if (creates.Count <= 0 && updates.Count <= 0 && deletes.Count <= 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không có dữ liệu cần lưu", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                bool valid = true;
                if (creates != null && creates.Count > 0)
                {
                    var resultData = new BackendAdapter(param).Post<List<HIS_MATY_MATY>>("api/HisMatyMaty/CreateList", ApiConsumers.MosConsumer, creates, param);
                    if (resultData == null)
                    {
                        valid = false;
                    }
                }

                if (valid && updates != null && updates.Count > 0)
                {
                    var resultData = new BackendAdapter(param).Post<List<HIS_MATY_MATY>>("api/HisMatyMaty/UpdateList", ApiConsumers.MosConsumer, updates, param);
                    if (resultData == null)
                    {
                        valid = false;
                    }
                }

                if (valid && deletes != null && deletes.Count > 0)
                {
                    var resultData = new BackendAdapter(param).Post<bool>("api/HisMatyMaty/DeleteList", ApiConsumers.MosConsumer, deletes.Select(s => s.ID).ToList(), param);
                    valid = resultData;
                }

                success = valid;
                this.FillDataCheckToPrepa(true);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MATY_MATY> GetMatyMatyByMaterialType(long materialTypeId)
        {
            HisMatyMatyFilter filter = new HisMatyMatyFilter();
            filter.MATERIAL_TYPE_ID = materialTypeId;
            return new BackendAdapter(new CommonParam()).Get<List<HIS_MATY_MATY>>("api/HisMatyMaty/Get", ApiConsumers.MosConsumer, filter, null);
        }

        private void repositoryItemPrepaMatyCheck_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MaterialTypeADO)gridViewPrepaMaty.GetFocusedRow();
                if (row.Check)
                {
                    row.Check = false;
                    row.AMOUNT = 0;
                }
                else
                {
                    row.Check = true;
                }
                gridControlPrepaMaty.BeginUpdate();
                gridControlPrepaMaty.DataSource = this.ListPrepaMaterialType;
                gridControlPrepaMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemMatyRadio_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MaterialTypeADO)gridViewMaty.GetFocusedRow();
                if (row.Check)
                {
                    row.Check = false;
                    this.currentMateriaType = null;
                }
                this.currentMateriaType = row;
                foreach (var item in this.ListMaterialType)
                {
                    item.Check = false;
                    if (item.ID == row.ID)
                    {
                        item.Check = true;
                    }
                }
                gridControlMaty.RefreshDataSource();
                this.FillDataCheckToPrepa(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCheckToPrepa(bool order)
        {
            try
            {
                List<HIS_MATY_MATY> matyMatys = new List<HIS_MATY_MATY>();
                if (this.currentMateriaType != null)
                {
                    matyMatys = this.GetMatyMatyByMaterialType(this.currentMateriaType.ID);
                }

                if (this.ListPrepaMaterialType != null)
                {
                    foreach (var item in this.ListPrepaMaterialType)
                    {
                        HIS_MATY_MATY pre = matyMatys != null ? matyMatys.FirstOrDefault(o => o.PREPARATION_MATERIAL_TYPE_ID == item.ID) : null;
                        if (pre != null)
                        {
                            item.Check = true;
                            item.AMOUNT = pre.PREPARATION_AMOUNT;
                        }
                        else
                        {
                            pre = this.inputMatyMaty != null ? this.inputMatyMaty.FirstOrDefault(o => o.PREPARATION_MATERIAL_TYPE_ID == item.ID) : null;
                            if (pre != null)
                            {
                                item.Check = true;
                                item.AMOUNT = pre.PREPARATION_AMOUNT;
                            }
                            else
                            {
                                item.Check = false;
                                item.AMOUNT = 0;
                            }
                        }

                        if (this.ListServiceUnit != null && this.ListServiceUnit.Count > 0)
                        {
                            var checkServiceUnit = this.ListServiceUnit.FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            if (checkServiceUnit != null)
                                item.SERVICE_UNIT_NAME = checkServiceUnit.SERVICE_UNIT_NAME;
                        }
                    }
                    if (order)
                        this.ListPrepaMaterialType = this.ListPrepaMaterialType.OrderByDescending(o => o.Check).ToList();
                }

                gridControlPrepaMaty.BeginUpdate();
                gridControlPrepaMaty.DataSource = this.ListPrepaMaterialType;
                gridControlPrepaMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonChange_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MaterialTypeADO row = (MaterialTypeADO)gridViewPrepaMaty.GetFocusedRow();
                if (row != null)
                {
                    frmChangePrepa frm = new frmChangePrepa(this.currentModuleBase, row.ID);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrepaFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrepaFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
