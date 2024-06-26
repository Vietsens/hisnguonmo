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
using HIS.Desktop.Plugins.MetyMaty.ADO;
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

namespace HIS.Desktop.Plugins.MetyMaty.Change
{
    public partial class frmChangePrepa : FormBase
    {
        private int positionHandleControl = -1;

        private long oldPrepaMatyId;

        bool IsMedicine = false;

        private List<HIS_MATERIAL_TYPE> listPrepaMaterialType = new List<HIS_MATERIAL_TYPE>();
        private List<HIS_MEDICINE_TYPE> listPrepaMedicineType = new List<HIS_MEDICINE_TYPE>();

        private List<MetyMatyADO> listData = new List<MetyMatyADO>();

        public frmChangePrepa(Inventec.Desktop.Common.Modules.Module module, long prepaMaterialTypeId, bool isMedicine)
            : base(module)
        {
            InitializeComponent();
            this.oldPrepaMatyId = prepaMaterialTypeId;
            this.IsMedicine = isMedicine;
        }

        private void frmChangePrepa_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIconFrm();
                this.ValidControl();
                this.LoadMaterialTypeOrMedicineTypePrepa();
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

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void LoadMaterialTypeOrMedicineTypePrepa()
        {
            try
            {
                if (IsMedicine)
                {
                    HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                    filter.IS_RAW_MEDICINE = true;
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    this.listPrepaMedicineType = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filter, null);
                }
                else
                {
                    HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                    filter.IS_RAW_MATERIAL = true;
                    filter.IS_ACTIVE = 1;
                    filter.IS_LEAF = true;
                    this.listPrepaMaterialType = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_TYPE>>("api/HisMaterialType/Get", ApiConsumers.MosConsumer, filter, null);
                }


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
                if (IsMedicine)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 300, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(this.cboPrepaMaty, this.listPrepaMedicineType, controlEditorADO);
                }
                else
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 300, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(this.cboPrepaMaty, this.listPrepaMaterialType, controlEditorADO);
                }

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
                this.listData = new List<MetyMatyADO>();

                CommonParam param = new CommonParam();
                if (IsMedicine)
                {
                    HisMetyMetyFilter filter = new HisMetyMetyFilter();
                    filter.PREPARATION_MEDICINE_TYPE_ID = this.oldPrepaMatyId;
                    var datas = new BackendAdapter(param).Get<List<HIS_METY_METY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_METY_GET, ApiConsumers.MosConsumer, filter, param);

                    List<HIS_MEDICINE_TYPE> allMedicineTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    this.listData = (from r in datas select new MetyMatyADO(r, allMedicineTypes.FirstOrDefault(o => o.ID == r.METY_PRODUCT_ID))).ToList();
                }
                else
                {
                    HisMetyMatyFilter filter = new HisMetyMatyFilter();
                    filter.MATERIAL_TYPE_ID = this.oldPrepaMatyId;
                    var datas = new BackendAdapter(param).Get<List<HIS_METY_MATY>>(HIS.Desktop.Plugins.MetyMaty.HisRequestUriStore.MOSHIS_METY_MATY_GET, ApiConsumers.MosConsumer, filter, param);

                    List<HIS_MEDICINE_TYPE> allMedicineTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    this.listData = (from r in datas select new MetyMatyADO(r, allMedicineTypes.FirstOrDefault(o => o.ID == r.METY_PRODUCT_ID))).ToList();

                    //HisMetyMatyFilter filter2 = new HisMetyMatyFilter();
                    ////filter2.PREPARATION_MATERIAL_TYPE_ID = this._crrId;
                    //var data2s = new BackendAdapter(param).Get<List<HIS_MATY_MATY>>("api/HisMetyMaty/Get", ApiConsumers.MosConsumer, filter2, param);
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
                MetyMatyADO row = (MetyMatyADO)gridViewMaty.GetFocusedRow();
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
                    XtraMessageBox.Show("Không có thuốc thành phầm để chuyển đổi", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                List<string> notAmounts = listData.Where(o => o.MATERIAL_TYPE_AMOUNT <= 0).Select(s => s.MEDICINE_TYPE_CODE).ToList();
                if (notAmounts != null && notAmounts.Count > 0)
                {
                    string message = String.Format("Các thuốc thành phẩm có mã sau chưa nhập số lượng: {0}", String.Join(",", notAmounts));
                    XtraMessageBox.Show(message, "Thông báo", DefaultBoolean.True);
                    return;
                }



                CommonParam param = new CommonParam();
                bool success = false;

                if (this.IsMedicine)
                {
                    HIS_MEDICINE_TYPE newPrepa = this.listPrepaMedicineType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrepaMaty.EditValue));
                    if (newPrepa.ID == oldPrepaMatyId)
                    {
                        XtraMessageBox.Show("Bạn chưa thay đổi chế phẩm", "Thông báo", DefaultBoolean.True);
                        return;
                    }

                    List<HIS_METY_METY> allExists = this.GetAllHisMetyMety(newPrepa.ID);

                    List<HIS_METY_METY> deletes = new List<HIS_METY_METY>();
                    List<HIS_METY_METY> updates = new List<HIS_METY_METY>();
                    foreach (var item in this.listData)
                    {
                        HIS_METY_METY ado = new HIS_METY_METY();
                        var exists = allExists != null ? allExists.FirstOrDefault(o => o.METY_PRODUCT_ID == item.METY_PRODUCT_ID) : null;
                        if (exists != null)
                        {
                            deletes.Add(exists);
                        }
                        ado.ID = item.ID;
                        ado.METY_PRODUCT_ID = item.METY_PRODUCT_ID;
                        ado.PREPARATION_AMOUNT = item.MATERIAL_TYPE_AMOUNT;
                        ado.PREPARATION_MEDICINE_TYPE_ID = newPrepa.ID;
                        updates.Add(ado);
                    }
                    bool valid = true;
                    if (deletes != null && deletes.Count > 0)
                    {
                        valid = new BackendAdapter(param).Post<bool>("api/HisMetyMety/DeleteList", ApiConsumers.MosConsumer, deletes.Select(s => s.ID).ToList(), param);
                    }
                    if (valid)
                    {
                        var rs = new BackendAdapter(param).Post<List<HIS_METY_METY>>("api/HisMetyMety/UpdateList", ApiConsumers.MosConsumer, updates, param);
                        if (rs != null)
                        {
                            success = true;
                        }
                    }
                }
                else
                {
                    HIS_MATERIAL_TYPE newPrepa = this.listPrepaMaterialType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPrepaMaty.EditValue));
                    if (newPrepa.ID == oldPrepaMatyId)
                    {
                        XtraMessageBox.Show("Bạn chưa thay đổi chế phẩm", "Thông báo", DefaultBoolean.True);
                        return;
                    }

                    List<HIS_METY_MATY> allExists = this.GetAllHisMetyMaty(newPrepa.ID);

                    List<HIS_METY_MATY> deletes = new List<HIS_METY_MATY>();
                    foreach (var item in this.listData)
                    {
                        var exists = allExists != null ? allExists.FirstOrDefault(o => o.METY_PRODUCT_ID == item.METY_PRODUCT_ID) : null;
                        if (exists != null)
                        {
                            deletes.Add(exists);
                        }
                        item.MATERIAL_TYPE_ID = newPrepa.ID;
                    }
                    bool valid = true;
                    if (deletes != null && deletes.Count > 0)
                    {
                        valid = new BackendAdapter(param).Post<bool>("api/HisMetyMaty/DeleteList", ApiConsumers.MosConsumer, deletes.Select(s => s.ID).ToList(), param);
                    }
                    if (valid)
                    {
                        var rs = new BackendAdapter(param).Post<List<HIS_METY_MATY>>("api/HisMetyMaty/UpdateList", ApiConsumers.MosConsumer, this.listData, param);
                        if (rs != null)
                        {
                            success = true;
                        }
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

        private List<HIS_METY_MATY> GetAllHisMetyMaty(long newPrepaId)
        {
            try
            {
                HisMetyMatyFilter filter = new HisMetyMatyFilter();
                filter.METY_PRODUCT_IDs = this.listData.Select(s => s.METY_PRODUCT_ID).ToList();
                filter.MATERIAL_TYPE_ID = newPrepaId;
                List<HIS_METY_MATY> allExists = new BackendAdapter(new CommonParam()).Get<List<HIS_METY_MATY>>("api/HisMetyMaty/Get", ApiConsumers.MosConsumer, filter, null);
                return allExists;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private List<HIS_METY_METY> GetAllHisMetyMety(long newPrepaId)
        {
            try
            {
                HisMetyMetyFilter filter = new HisMetyMetyFilter();
                filter.METY_PRODUCT_IDs = this.listData.Select(s => s.METY_PRODUCT_ID).ToList();
                filter.PREPARATION_MEDICINE_TYPE_ID = newPrepaId;
                List<HIS_METY_METY> allExists = new BackendAdapter(new CommonParam()).Get<List<HIS_METY_METY>>("api/HisMetyMety/Get", ApiConsumers.MosConsumer, filter, null);
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
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
