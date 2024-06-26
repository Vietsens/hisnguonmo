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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using MOS.EFMODEL.DataModels;
//using HIS.Desktop.Plugins.EquipmentSet.Ado;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.EquipmentSet.ADO;
using HIS.Desktop.LocalStorage.BackendData;


namespace HIS.Desktop.Plugins.EquipmentSet.frmEdit
{
    public partial class frmEditEquipmentSet : DevExpress.XtraEditors.XtraForm
    {
        #region deckare
        internal List<HisEquipmentSetADO> equipmentsetAdoTemps = new List<HisEquipmentSetADO>();
        DelegateRefreshData refeshData;
        MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET equipmentset = new HIS_EQUIPMENT_SET();
        MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET_MATY currentData;
        MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET currentData2;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int positionHandle = -1;
        int actionType;
        #endregion

        #region load
        public frmEditEquipmentSet(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET _data, DelegateRefreshData _refeshData)
        {
            InitializeComponent();

            this.refeshData = _refeshData;
            this.equipmentset = _data;
            actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
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

        public frmEditEquipmentSet(DelegateRefreshData _refeshData)
        {
            InitializeComponent();

            this.refeshData = _refeshData;
            try
            {
                actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void frmEditEquipmentSet_Load(object sender, EventArgs e)
        {
            LoadDataToCombo();
            FillDatatoEquipmentSetUser(equipmentset);
            if (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
            {
                btnReset.Enabled = false;
                textEdit1.Text = equipmentset.EQUIPMENT_SET_CODE;
                textEdit2.Text = equipmentset.EQUIPMENT_SET_NAME;
                spinEdit1.EditValue = equipmentset.HEIN_SET_LIMIT_PRICE;
            }
            if (grdControlInformationSurg.DataSource == null)
            {
                InitGrid();
            }
            ValidationSingleControl(textEdit1);
            ValidationSingleControl(textEdit2);
            ValidationSingleControl(spinEdit1);
        }

        private void InitGrid()
        {
            try
            {
                List<HisEquipmentSetADO> hisEkipUserADOs = new List<HisEquipmentSetADO>();
                HisEquipmentSetADO equipmentMatyAdoTemp = new HisEquipmentSetADO();
                equipmentMatyAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                hisEkipUserADOs.Add(equipmentMatyAdoTemp);
                grdControlInformationSurg.DataSource = hisEkipUserADOs;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ComboExecuteRole(Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                ComboExecuteRole(cboPosition1);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDatatoEquipmentSetUser(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET data)
        {
            try
            {
                List<HisEquipmentSetADO> equipmentsetmatys = new List<HisEquipmentSetADO>();
                CommonParam param = new CommonParam();
                HisEquipmentSetMatyViewFilter hisEkipUserFilter = new HisEquipmentSetMatyViewFilter();
                hisEkipUserFilter.EQUIPMENT_SET_ID = data.ID;
                var lst = new BackendAdapter(param)
        .Get<List<V_HIS_EQUIPMENT_SET_MATY>>("api/HisEquipmentSetMaty/GetView", ApiConsumers.MosConsumer, hisEkipUserFilter, param);

                if (lst.Count > 0)
                {
                    int i = 0;
                    foreach (var item in lst)
                    {
                        HisEquipmentSetADO equipmentsetmaty = new HisEquipmentSetADO();

                        if (i != 0)
                        {
                            equipmentsetmaty.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisEquipmentSetADO>(equipmentsetmaty, item);
                        equipmentsetmatys.Add(equipmentsetmaty);
                        i++;
                    }
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = equipmentsetmatys;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void grdViewInformationSurg_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region event_click
        private void btnAdd_Click(object sender, EventArgs e)
        {

            try
            {
                var equipments = grdControlInformationSurg.DataSource as List<HisEquipmentSetADO>;
                if (equipments == null || equipments.Count < 1)
                {
                    HisEquipmentSetADO equipmentMatyAdoTemp = new HisEquipmentSetADO();
                    equipmentMatyAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    equipmentsetAdoTemps.Add(equipmentMatyAdoTemp);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = equipmentsetAdoTemps;
                }
                else
                {
                    HisEquipmentSetADO participant = new HisEquipmentSetADO();
                    participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    equipments.Add(participant);
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = equipments;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var equipments = grdControlInformationSurg.DataSource as List<HisEquipmentSetADO>;
                var ekipUser = (HisEquipmentSetADO)grdViewInformationSurg.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (equipments.Count > 0)
                    {
                        equipments.Remove(ekipUser);
                        grdControlInformationSurg.DataSource = null;
                        grdControlInformationSurg.DataSource = equipments;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bool valid = true;
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {
                    // WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    equipmentset.EQUIPMENT_SET_CODE = textEdit1.Text;
                    equipmentset.EQUIPMENT_SET_NAME = textEdit2.Text;
                    equipmentset.HEIN_SET_LIMIT_PRICE = (decimal)spinEdit1.EditValue;
                    HIS_EQUIPMENT_SET equipmentsetRS = new HIS_EQUIPMENT_SET();
                    if (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        var equipments = grdControlInformationSurg.DataSource as List<HisEquipmentSetADO>;
                        List<HIS_EQUIPMENT_SET_MATY> equipmentsetmatys = new List<HIS_EQUIPMENT_SET_MATY>();
                        if (equipments != null && equipments.Count > 0)
                        {
                            foreach (var item in equipments)
                            {
                                if (item.MATERIAL_TYPE_ID != 0 && item.AMOUNT != 0)
                                {
                                    HIS_EQUIPMENT_SET_MATY equipmentsetmaty = new HIS_EQUIPMENT_SET_MATY();
                                    equipmentsetmaty.MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                                    equipmentsetmaty.AMOUNT = item.AMOUNT;
                                    equipmentsetmatys.Add(equipmentsetmaty);
                                }

                            }
                            equipmentset.HIS_EQUIPMENT_SET_MATY = equipmentsetmatys;
                        }
                        equipmentsetRS = new BackendAdapter(param)
                       .Post<HIS_EQUIPMENT_SET>(HisRequestUriStore.MOSHIS_EQUIPMENT_SET_UPDATE, ApiConsumers.MosConsumer, equipmentset, param);
                    }
                    else
                    {
                        var equipments = grdControlInformationSurg.DataSource as List<HisEquipmentSetADO>;
                        List<HIS_EQUIPMENT_SET_MATY> equipmentsetmatys = new List<HIS_EQUIPMENT_SET_MATY>();
                        if (equipments.Count == 1 && (equipments[0].MATERIAL_TYPE_ID == 0 || equipments[0].MATERIAL_TYPE_ID == null))
                        {
                            MessageManager.Show("Chưa có vật tư.");
                            //WaitingManager.Hide();
                            return;
                        }
                        foreach (var item in equipments)
                        {
                            if (item.AMOUNT == 0)
                            {
                                MessageManager.Show("Có vật tư chưa nhập số lượng");
                                //WaitingManager.Hide();
                                return;
                            }
                        }
                        if (equipments != null && equipments.Count > 0)
                        {
                            foreach (var item in equipments)
                            {
                                if (item.MATERIAL_TYPE_ID != 0 && item.AMOUNT != 0)
                                {
                                    HIS_EQUIPMENT_SET_MATY equipmentsetmaty = new HIS_EQUIPMENT_SET_MATY();
                                    equipmentsetmaty.MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                                    equipmentsetmaty.AMOUNT = item.AMOUNT;
                                    equipmentsetmatys.Add(equipmentsetmaty);
                                }
                            }
                            equipmentset.HIS_EQUIPMENT_SET_MATY = equipmentsetmatys;
                        }
                        equipmentsetRS = new BackendAdapter(param)
                       .Post<HIS_EQUIPMENT_SET>("api/HisEquipmentSet/Create", ApiConsumers.MosConsumer, equipmentset, param);
                    }
                    //WaitingManager.Hide();
                    if (equipmentsetRS != null)
                    {
                        if (refeshData != null)
                            refeshData();
                        success = true;
                        BackendDataWorker.Reset<HIS_EQUIPMENT_SET>();
                        this.Close();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            textEdit1.EditValue = null;
            textEdit2.EditValue = null;
            spinEdit1.EditValue = null;
            grdControlInformationSurg.DataSource = null;
            InitGrid();
        }
        #endregion

        #region validate
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            simpleButton1_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                simpleButton2_Click(null, null);
            }
        }

        private void cboPosition1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cboPosition1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPosition1_Click(object sender, EventArgs e)
        {
            try
            {
                var cbo = sender as GridLookUpEdit;
                cbo.ShowPopup();
            }
            catch (Exception)
            {

            }
        }
    }
}
