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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System.Configuration;
using HIS.Desktop.LocalStorage.Location;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.ApproveExpMestBCS.ADO;
using HIS.Desktop.Plugins.ApproveExpMestBCS.Validation;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Plugins.ApproveExpMestBCS
{
    public partial class frmReplace : DevExpress.XtraEditors.XtraForm
    {
        MedicineTypeADO expMestMedicineADO = null;
        MaterialTypeADO expMestMaterialADO = null;
        HIS.Desktop.Common.DelegateSelectData delegateSelectData;
        List<MetyMatyTypeADO> mediMatyTypeCombos;
        int positionHandle = -1;
        long currentRoomId = 0;

        public frmReplace()
        {
            InitializeComponent();
        }

        public frmReplace(MedicineTypeADO expMestMedicineADO, long currentRoomId, Desktop.Common.DelegateSelectData delegateSelectData)// là thuốc
        {
            InitializeComponent();
            SetBiThayThe(expMestMedicineADO);
            this.expMestMedicineADO = expMestMedicineADO;
            this.delegateSelectData = delegateSelectData;
            this.currentRoomId = currentRoomId;
        }

        public frmReplace(MaterialTypeADO expMestMaterialADO, long currentRoomId, Desktop.Common.DelegateSelectData delegateSelectData)// là vật tư
        {
            InitializeComponent();
            SetBiThayThe(expMestMaterialADO);
            this.expMestMaterialADO = expMestMaterialADO;
            this.delegateSelectData = delegateSelectData;
            this.currentRoomId = currentRoomId;
        }

        private void frmReplace_Load(object sender, EventArgs e)
        {
            SetIcon();
            this.mediMatyTypeCombos = SetDataToMedicineTypeCombo();
            InItMedicineTypeCombo(this.mediMatyTypeCombos);
            ValidControlAmount();
            ValidControlComboMediMaty();
            cboMedicineType.Focus();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlAmount()
        {
            try
            {
                SpinAmountValidationRule bloodCodeRule = new SpinAmountValidationRule();
                if (this.expMestMedicineADO != null)
                {
                    bloodCodeRule.ycAmount = this.expMestMedicineADO.AMOUNT - (this.expMestMedicineADO.CURRENT_DD_AMOUNT);
                }
                else if (this.expMestMaterialADO != null)
                {
                    bloodCodeRule.ycAmount = this.expMestMaterialADO.AMOUNT - (this.expMestMaterialADO.CURRENT_DD_AMOUNT);
                }

                bloodCodeRule.spinAmount = spinAmount;
                dxValidationProvider1.SetValidationRule(spinAmount, bloodCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlComboMediMaty()
        {
            try
            {
                ComboMediMatyValidationRule bloodCodeRule = new ComboMediMatyValidationRule();
                bloodCodeRule.cboMediMaty = cboMedicineType;
                dxValidationProvider1.SetValidationRule(cboMedicineType, bloodCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Dictionary<long, string> GetActiveIngredient(List<long> MedicineTypeIdList, string activeIdStr)
        {
            Dictionary<long, string> dicmety = new Dictionary<long, string>();
            try
            {
                List<long> activeIdList = null;
                if (!String.IsNullOrWhiteSpace(activeIdStr) && activeIdStr.Length > 0)
                {
                    string[] activeIdArr = activeIdStr.Split(';');
                    if (activeIdArr != null && activeIdArr.Length > 0)
                    {
                        activeIdList = new List<long>();
                        foreach (var item in activeIdArr)
                        {
                            long id = 0;
                            if (Int64.TryParse(item, out id))
                            {
                                activeIdList.Add(id);
                            }
                        }

                    }
                }

                CommonParam param = new CommonParam();
                HisMedicineTypeAcinViewFilter medicineTypeAcinFilter = new HisMedicineTypeAcinViewFilter();
                medicineTypeAcinFilter.MEDICINE_TYPE_IDs = MedicineTypeIdList;
                if (activeIdList != null && activeIdList.Count > 0)
                {
                    medicineTypeAcinFilter.ACTIVE_INGREDIENT_IDs = activeIdList;
                }
                var medicineTypeAcin = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(ApiConsumer.HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW, ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);
                if (medicineTypeAcin != null)
                {
                    var groupByMedicine = medicineTypeAcin.GroupBy(o => o.MEDICINE_TYPE_ID).ToList();
                    foreach (var group in groupByMedicine)
                    {
                        var activeIngredients = group.GroupBy(p => p.ACTIVE_INGREDIENT_ID).Select(o => o.First().ACTIVE_INGREDIENT_ID).ToList();
                        if (activeIngredients != null && activeIngredients.Count > 0)
                            dicmety.Add(group.First().MEDICINE_TYPE_ID, string.Join(";", activeIngredients.OrderBy(o => o)));
                    }
                }
            }
            catch (Exception ex)
            {
                dicmety = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dicmety;
        }

        private List<MetyMatyTypeADO> SetDataToMedicineTypeCombo()
        {
            List<MetyMatyTypeADO> result = new List<MetyMatyTypeADO>();
            try
            {
                if (this.expMestMedicineADO != null)
                {
                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentRoomId).ID;
                    mediFilter.IS_LEAF = true;

                    //mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;

                    var listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);
                    if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        if (chkMappingMediMaty.Checked)
                        {
                            var dicActiveReplace = GetActiveIngredient(new List<long> { this.expMestMedicineADO.MEDICINE_TYPE_ID }, "");
                            Dictionary<long, string> dicActiveInStock = new Dictionary<long, string>();
                            if (dicActiveReplace != null && dicActiveReplace.Count > 0)
                            {
                                dicActiveInStock = GetActiveIngredient(listMediTypeInStock.Select(o => o.Id).Distinct().ToList(), dicActiveReplace.Values.FirstOrDefault());
                            }

                            if (dicActiveReplace != null && dicActiveReplace.Count > 0 && dicActiveInStock != null && dicActiveInStock.Count > 0)
                            {
                                Dictionary<long, string> dicCheck = new Dictionary<long, string>();
                                foreach (var dic in dicActiveInStock)
                                {
                                    if (dicActiveReplace.Values.FirstOrDefault() == dic.Value)
                                    {
                                        dicCheck.Add(dic.Key, dic.Value);
                                    }
                                }
                                if (dicCheck != null && dicCheck.Count() > 0)
                                {
                                    listMediTypeInStock = listMediTypeInStock.Where(o => dicCheck.Keys.Contains(o.Id)).ToList();
                                }
                            }
                        }
                    }
                    if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        foreach (var item in listMediTypeInStock)
                        {
                            MetyMatyTypeADO ado = new MetyMatyTypeADO(item);
                            ado.ADDITION_INFO = item.MedicineTypeName;
                            result.Add(ado);
                        }
                    }

                }
                else if (this.expMestMaterialADO != null)
                {
                    HisMaterialTypeStockViewFilter mediFilter = new HisMaterialTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentRoomId).ID;
                    mediFilter.IS_LEAF = true;

                    //mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;

                    var listMaterialInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);
                    if (listMaterialInStock != null && listMaterialInStock.Count > 0)
                    {
                        if (chkMappingMediMaty.Checked)
                        {
                            var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == this.expMestMaterialADO.MATERIAL_TYPE_ID);
                            listMaterialInStock = listMaterialInStock.Where(o => !materialType.MATERIAL_TYPE_MAP_ID.HasValue
                                || materialType.MATERIAL_TYPE_MAP_ID == o.MaterialTypeMapId).ToList();
                        }
                    }
                    if (listMaterialInStock != null && listMaterialInStock.Count > 0)
                    {
                        foreach (var item in listMaterialInStock)
                        {
                            MetyMatyTypeADO ado = new MetyMatyTypeADO(item);
                            ado.ADDITION_INFO = item.MaterialTypeName;
                            ado.MedicineTypeCode = item.MaterialTypeCode;
                            ado.MedicineTypeName = item.MaterialTypeName;
                            ado.ServiceUnitName = item.ServiceUnitName;
                            ado.AvailableAmount = item.AvailableAmount;
                            ado.Id = item.Id;
                            ado.IsLeaf = item.IsLeaf;
                            ado.TotalAmount = item.TotalAmount;
                            result.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InItMedicineTypeCombo(List<MetyMatyTypeADO> result)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MedicineTypeCode", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("MedicineTypeName", "Tên", 200, 2));
                columnInfos.Add(new ColumnInfo("ServiceUnitName", "DVT", 100, 3));
                columnInfos.Add(new ColumnInfo("AvailableAmount", "Khả dụng", 100, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ADDITION_INFO", "Id", columnInfos, false, 900);
                ControlEditorLoader.Load(this.cboMedicineType, result, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBiThayThe(MaterialTypeADO expMestMedicineADO)
        {
            try
            {
                lblBiThayThe_MedicineTypeCode.Text = expMestMedicineADO.MATERIAL_TYPE_CODE;
                lblBiThayThe_MedicineTypeName.Text = expMestMedicineADO.MATERIAL_TYPE_NAME;
                lblBiThayThe_ActiveGredientName.Text = "";
                lblBiThayThe_Concentra.Text = "";
                lblBiThayThe_ServiceUnitName.Text = expMestMedicineADO.SERVICE_UNIT_NAME;
                lblBiThayThe_RequestName.Text = expMestMedicineADO.AMOUNT + "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetBiThayThe(MedicineTypeADO expMestMedicineADO)
        {
            try
            {
                lblBiThayThe_MedicineTypeCode.Text = expMestMedicineADO.MEDICINE_TYPE_CODE;
                lblBiThayThe_MedicineTypeName.Text = expMestMedicineADO.MEDICINE_TYPE_NAME;
                lblBiThayThe_ActiveGredientName.Text = expMestMedicineADO.ACTIVE_INGR_BHYT_NAME;
                lblBiThayThe_Concentra.Text = expMestMedicineADO.CONCENTRA;
                lblBiThayThe_ServiceUnitName.Text = expMestMedicineADO.SERVICE_UNIT_NAME;
                lblBiThayThe_RequestName.Text = expMestMedicineADO.AMOUNT + "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnChoose_Click(null, null);
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            positionHandle = -1;
            if (!dxValidationProvider1.Validate())
                return;

            if (cboMedicineType.EditValue != null)
            {
                var select = this.mediMatyTypeCombos.FirstOrDefault(o => o.Id == Convert.ToInt64(cboMedicineType.EditValue.ToString()));
                select.YCD_AMOUNT = spinAmount.Value;
                if (delegateSelectData != null)
                {
                    delegateSelectData(select);
                }

                this.Close();
            }
        }

        private void cboMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboMedicineType.ClosePopup();
                    cboMedicineType.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboMedicineType.ClosePopup();
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
                else
                    cboMedicineType.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicineType.EditValue == null)
                    {
                        cboMedicineType.ShowPopup();
                    }
                    else
                    {
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void chkMappingMediMaty_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.mediMatyTypeCombos = SetDataToMedicineTypeCombo();
                InItMedicineTypeCombo(this.mediMatyTypeCombos);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
    }
}
