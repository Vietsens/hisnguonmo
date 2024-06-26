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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MatyMaty.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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

namespace HIS.Desktop.Plugins.MatyMaty.Change
{
    public partial class frmChangePrepa : FormBase
    {
        private int positionHandleControl = -1;

        private long oldPrepaMatyId;

        private List<HIS_MATERIAL_TYPE> listPrepaMaterialType = new List<HIS_MATERIAL_TYPE>();
        private List<MatyMatyADO> listData = new List<MatyMatyADO>();
        public List<HIS_SERVICE_UNIT> ListServiceUnit { get; set; }

        public frmChangePrepa(Inventec.Desktop.Common.Modules.Module module, long prepaMaterialTypeId)
            : base(module)
        {
            InitializeComponent();
            this.oldPrepaMatyId = prepaMaterialTypeId;
        }

        private void frmChangePrepa_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                GetServiceUnit();
                this.ValidControl();
                this.LoadMaterialTypePrepa();
                this.LoadComboPrepa();
                this.FillDataToGrid();
                cboPrepaMaty.EditValue = this.oldPrepaMatyId;
                txtPrepaMatyCode.Focus();
                txtPrepaMatyCode.SelectAll();
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

        private void ValidControl()
        {
            try
            {
                ValidControlComboPrepa();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlComboPrepa()
        {
            try
            {
                PrepaMatyValidationRule rule = new PrepaMatyValidationRule();
                rule.txtPrepaMetyCode = txtPrepaMatyCode;
                rule.cboPrepaMaty = cboPrepaMaty;
                dxValidationProvider1.SetValidationRule(txtPrepaMatyCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialTypePrepa()
        {
            try
            {
                HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                filter.IS_RAW_MATERIAL = true;
                filter.IS_ACTIVE = 1;
                filter.IS_LEAF = true;
                this.listPrepaMaterialType = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboPrepa()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPrepaMaty, this.listPrepaMaterialType, controlEditorADO);
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
                this.listData = new List<MatyMatyADO>();

                HisMatyMatyFilter filter = new HisMatyMatyFilter();
                filter.PREPARATION_MATERIAL_TYPE_ID = this.oldPrepaMatyId;
                List<HIS_MATY_MATY> matymatys = new BackendAdapter(new CommonParam()).Get<List<HIS_MATY_MATY>>("api/HisMatyMaty/Get", ApiConsumers.MosConsumer, filter, null);
                if (matymatys != null && matymatys.Count > 0)
                {
                    List<HIS_MATERIAL_TYPE> allMaterialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                    this.listData = (from r in matymatys select new MatyMatyADO(r, allMaterialTypes.FirstOrDefault(o => o.ID == r.MATERIAL_TYPE_ID), this.ListServiceUnit)).ToList();
                }

                gridControlMaty.BeginUpdate();
                gridControlMaty.DataSource = this.listData;
                gridControlMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrepaMatyCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtPrepaMatyCode.Text))
                    {
                        string key = txtPrepaMatyCode.Text.ToLower().Trim();
                        var listSearch = this.listPrepaMaterialType != null ? this.listPrepaMaterialType.Where(o => o.MATERIAL_TYPE_CODE.ToLower().Contains(key) || o.MATERIAL_TYPE_NAME.ToLower().Contains(key)).ToList() : null;
                        if (listSearch != null && listSearch.Count == 1)
                        {
                            cboPrepaMaty.EditValue = listSearch[0].ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboPrepaMaty.Focus();
                            cboPrepaMaty.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPrepaMaty.Focus();
                        cboPrepaMaty.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrepaMaty_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrepaMaty_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPrepaMatyCode.Text = "";
                if (cboPrepaMaty.EditValue != null)
                {
                    var data = this.listPrepaMaterialType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrepaMaty.EditValue));
                    if (data != null)
                    {
                        txtPrepaMatyCode.Text = data.MATERIAL_TYPE_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                MatyMatyADO row = (MatyMatyADO)gridViewMaty.GetFocusedRow();
                this.listData.Remove(row);
                gridControlMaty.BeginUpdate();
                gridControlMaty.DataSource = this.listData;
                gridControlMaty.EndUpdate();
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
                gridViewMaty.PostEditor();
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate()) return;
                if (listData == null && listData.Count <= 0)
                {
                    XtraMessageBox.Show("Không có vật tư thành phầm để chuyển đổi", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                List<string> notAmounts = listData.Where(o => o.PREPARATION_AMOUNT <= 0).Select(s => s.MATERIAL_TYPE_CODE).ToList();
                if (notAmounts != null && notAmounts.Count > 0)
                {
                    string message = String.Format("Các vật tư thành phẩm có mã sau chưa nhập số lượng: {0}", String.Join(",", notAmounts));
                    XtraMessageBox.Show(message, "Thông báo", DefaultBoolean.True);
                    return;
                }

                HIS_MATERIAL_TYPE newPrepa = this.listPrepaMaterialType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrepaMaty.EditValue));
                if (newPrepa.ID == oldPrepaMatyId)
                {
                    XtraMessageBox.Show("Bạn chưa thay đổi vật tư chế phẩm", "Thông báo", DefaultBoolean.True);
                    return;
                }
                List<HIS_MATY_MATY> allExists = this.GetAllHisMatyMaty(newPrepa.ID);
                CommonParam param = new CommonParam();
                bool success = false;
                List<HIS_MATY_MATY> deletes = new List<HIS_MATY_MATY>();
                foreach (var item in this.listData)
                {
                    var exists = allExists != null ? allExists.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                    if (exists != null)
                    {
                        deletes.Add(exists);
                    }
                    item.PREPARATION_MATERIAL_TYPE_ID = newPrepa.ID;
                }
                bool valid = true;
                if (deletes != null && deletes.Count > 0)
                {
                    valid = new BackendAdapter(param).Post<bool>("api/HisMatyMaty/DeleteList", ApiConsumers.MosConsumer, deletes.Select(s => s.ID).ToList(), param);
                }
                if (valid)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_MATY_MATY>>("api/HisMatyMaty/UpdateList", ApiConsumers.MosConsumer, this.listData, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MATY_MATY> GetAllHisMatyMaty(long newPrepaId)
        {
            try
            {
                HisMatyMatyFilter filter = new HisMatyMatyFilter();
                filter.MATERIAL_TYPE_IDs = this.listData.Select(s => s.MATERIAL_TYPE_ID).ToList();
                filter.PREPARATION_MATERIAL_TYPE_ID = newPrepaId;
                List<HIS_MATY_MATY> allExists = new BackendAdapter(new CommonParam()).Get<List<HIS_MATY_MATY>>("api/HisMatyMaty/Get", ApiConsumers.MosConsumer, filter, null);
                return allExists;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
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

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MatyMatyADO pData = (MatyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
