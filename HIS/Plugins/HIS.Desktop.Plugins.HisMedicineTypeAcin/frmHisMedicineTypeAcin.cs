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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.HisMedicineTypeAcin
{
    public partial class frmHisMedicineTypeAcin : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount1 = 0;
        int dataTotal1 = 0;
        List<ActiveIngredientADO> listActiveIngredient;
        List<ActiveIngredientADO> listActiveIngredientCheck = new List<ActiveIngredientADO>();
        List<ActiveIngredientADO> listActiveIngredientCheckForGridView = new List<ActiveIngredientADO>();
        long medicineTypeId;
        bool isCheckAll;
        List<V_HIS_MEDICINE_TYPE_ACIN> medicineTypeAcins;
        DelegateReturnMutilObject resultActiveIngredient;
        List<HIS_ACTIVE_INGREDIENT> lsActiveIngredientChecked = new List<HIS_ACTIVE_INGREDIENT>();
        #endregion

        #region Contructor

        public frmHisMedicineTypeAcin(long medicineTypeId)
        {
            InitializeComponent();
            this.medicineTypeId = medicineTypeId;
        }

        public frmHisMedicineTypeAcin(long medicineTypeId, DelegateReturnMutilObject resultActiveIngredient, List<V_HIS_MEDICINE_TYPE_ACIN> listMedicineTypeAcin)
        {
            InitializeComponent();
            this.medicineTypeId = medicineTypeId;
            this.resultActiveIngredient = resultActiveIngredient;
            this.medicineTypeAcins = listMedicineTypeAcin;
        }

        private void frmHisMedicineTypeAcin_Load(object sender, EventArgs e)
        {
            try
            {
                if (resultActiveIngredient != null)
                {
                    btnSaveServiceGroup.Enabled = false;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (this.medicineTypeAcins == null)
                {
                    GetMedicineTypeAcineByMedicineType();
                }

                FillDataToGrid2();
                LoadMedicineType();
                LoadDataChecked();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMedicineTypeAcineByMedicineType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeAcinViewFilter medicineTypeAcinFilter = new HisMedicineTypeAcinViewFilter();
                medicineTypeAcinFilter.MEDICINE_TYPE_ID = this.medicineTypeId;
                this.medicineTypeAcins = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>("api/HisMedicineTypeAcin/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeAcinFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewFilter = new HisMedicineTypeViewFilter();
                medicineTypeViewFilter.ID = this.medicineTypeId;
                var medicineTypes = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>("api/HisMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeViewFilter, param);
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    SetDataToMedicineTypeCommon(medicineTypes.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToMedicineTypeCommon(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE medicineType)
        {
            try
            {
                txtMedicineTypeCode.Text = medicineType.MEDICINE_TYPE_CODE;
                txtMedicineTypeName.Text = medicineType.MEDICINE_TYPE_NAME;
                txtServiceUnitName.Text = medicineType.SERVICE_UNIT_NAME;
                txtImpPrice.Text = medicineType.IMP_PRICE != null ? medicineType.IMP_PRICE.ToString() : "";
                txtImpVatRatio.Text = medicineType.IMP_VAT_RATIO != null ? (medicineType.IMP_VAT_RATIO * 100).ToString() : "";
                txtInternalPrice.Text = medicineType.INTERNAL_PRICE != null ? (medicineType.INTERNAL_PRICE * 100).ToString() : "";
                txtRegisterNumber.Text = medicineType.REGISTER_NUMBER;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid2()
        {
            try
            {
                int numPageSize;
                if (ucPagingService.pagingGrid != null)
                {
                    numPageSize = ucPagingService.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridActiveIngredient(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPagingService.Init(FillDataToGridActiveIngredient, param, numPageSize);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridActiveIngredient(object data)
        {
            try
            {
                WaitingManager.Show();
                listActiveIngredient = new List<ActiveIngredientADO>();
                listActiveIngredientCheck = new List<ActiveIngredientADO>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisActiveIngredientFilter hisActiveIngredientFilter = new HisActiveIngredientFilter();
                hisActiveIngredientFilter.KEY_WORD = txtSearch.Text;
                hisActiveIngredientFilter.ORDER_FIELD = "MODIFY_TIME";
                hisActiveIngredientFilter.ORDER_DIRECTION = "DESC";

                var activeIngredients = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_ACTIVE_INGREDIENT>>(
                    "/api/HisActiveIngredient/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    hisActiveIngredientFilter,
                    param);

                if (activeIngredients != null && activeIngredients.Data.Count > 0)
                {
                    foreach (var item in activeIngredients.Data)
                    {
                        ActiveIngredientADO activeIngredient = new ActiveIngredientADO();
                        AutoMapper.Mapper.CreateMap<HIS_ACTIVE_INGREDIENT, ActiveIngredientADO>();
                        activeIngredient = AutoMapper.Mapper.Map<ActiveIngredientADO>(item);

                        var checkActiveIngredient = (medicineTypeAcins != null && medicineTypeAcins.Count > 0) ? medicineTypeAcins.FirstOrDefault(o => o.ACTIVE_INGREDIENT_ID == item.ID) : null;
                        if (checkActiveIngredient != null)
                        {
                            activeIngredient.check2 = true;
                            listActiveIngredientCheck.Add(activeIngredient);

                        }
                        listActiveIngredient.Add(activeIngredient);
                    }
                    listActiveIngredient = listActiveIngredient.OrderByDescending(o => o.check2).ToList();
                }
                foreach (var item in listActiveIngredient)
                {
                    var checkItem = listActiveIngredientCheckForGridView.FirstOrDefault(o => o.ID == item.ID);
                    if (checkItem != null)
                    {
                        item.check2 = true;
                    }
                }
                gridControlActiveIngredient.BeginUpdate();
                gridControlActiveIngredient.DataSource = listActiveIngredient;
                gridControlActiveIngredient.EndUpdate();
                rowCount1 = (data == null ? 0 : listActiveIngredient.Count);
                dataTotal1 = (activeIngredients.Param == null ? 0 : activeIngredients.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataChecked()
        {
            try
            {
                this.lsActiveIngredientChecked = new List<HIS_ACTIVE_INGREDIENT>();
                if (medicineTypeAcins != null && medicineTypeAcins.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisActiveIngredientFilter hisActiveIngredientFilter = new HisActiveIngredientFilter();
                    var activeIngredients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACTIVE_INGREDIENT>>(
                        "/api/HisActiveIngredient/Get",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        hisActiveIngredientFilter,
                        param);
                    if (activeIngredients != null)
                    {
                        foreach (var item in medicineTypeAcins)
                        {
                            var checkActiveIngredient = (activeIngredients.FirstOrDefault(o => o.ID == item.ACTIVE_INGREDIENT_ID) ?? null);
                            if (checkActiveIngredient != null)
                            {
                                this.lsActiveIngredientChecked.Add(checkActiveIngredient);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void loadDataToGridControlActiveIngredient()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisActiveIngredientFilter hisActiveIngredientFilter = new HisActiveIngredientFilter();
                hisActiveIngredientFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var activeIngredients = new BackendAdapter(param).Get<List<HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumer.ApiConsumers.MosConsumer, hisActiveIngredientFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void txtSearchService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearchActiveIngredient_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewActiveIngredient_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT)gridViewActiveIngredient.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "LOCK_UNLOCK_COLUMN")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = ButtonEditUnLock;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditLock;
                            }
                        }
                        else if (e.Column.FieldName == "DELETE_COLUMN")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = ButtonEditDeleteEnable;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditDeleteDisable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewActiveIngredient_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        #endregion

        private void txtServiceGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineTypeName.Focus();
                    txtMedicineTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceGroupName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            LockUnlockBranch();
        }

        private void ButtonEditUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            LockUnlockBranch();
        }

        private void ButtonEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var currentActiveIngredient = (ActiveIngredientADO)gridViewActiveIngredient.GetFocusedRow();
                if (currentActiveIngredient != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_ACTIVE_INGREDIENT_DELETE, ApiConsumers.MosConsumer, currentActiveIngredient.ID, param);
                    if (success)
                    {
                        FillDataToGrid2();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // khóa, bỏ khóa
        private void LockUnlockBranch()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var updateDTO = (ActiveIngredientADO)gridViewActiveIngredient.GetFocusedRow();
                var result = new BackendAdapter(param).Post<HIS_ACTIVE_INGREDIENT>(HisRequestUriStore.MOSHIS_ACTIVE_INGREDIENT_CHANGE_LOCK, ApiConsumers.MosConsumer, updateDTO.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGrid2();
                    //ResetFormData();
                    //SetFocusEditor();
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveServiceGroup_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<ActiveIngredientADO> activeIngredientAdos = (List<ActiveIngredientADO>)gridControlActiveIngredient.DataSource;
                if (activeIngredientAdos != null && activeIngredientAdos.Count > 0)
                {
                    var activeIngredientAdoChecks = activeIngredientAdos.Where(o => o.check2).ToList();
                    if (activeIngredientAdoChecks != null && activeIngredientAdoChecks.Count > 0)
                    {
                        // thêm
                        List<ActiveIngredientADO> activeIngredientAddAdos = new List<ActiveIngredientADO>();
                        List<HIS_MEDICINE_TYPE_ACIN> medicineTypeAcinAdds = new List<HIS_MEDICINE_TYPE_ACIN>();
                        if (this.listActiveIngredientCheck != null && this.listActiveIngredientCheck.Count > 0)
                        {
                            activeIngredientAddAdos = activeIngredientAdoChecks.Where(p => !this.listActiveIngredientCheck.Select(o => o.ID).Contains(p.ID)).ToList();
                        }
                        else
                        {
                            activeIngredientAddAdos = activeIngredientAdoChecks;
                        }

                        foreach (var item in activeIngredientAddAdos)
                        {
                            HIS_MEDICINE_TYPE_ACIN medicineTypeAcin = new HIS_MEDICINE_TYPE_ACIN();
                            medicineTypeAcin.ACTIVE_INGREDIENT_ID = item.ID;
                            medicineTypeAcin.MEDICINE_TYPE_ID = this.medicineTypeId;
                            medicineTypeAcinAdds.Add(medicineTypeAcin);
                        }
                        if (medicineTypeAcinAdds != null && medicineTypeAcinAdds.Count > 0)
                        {
                            var result = new BackendAdapter(param).Post<List<HIS_MEDICINE_TYPE_ACIN>>("/api/HisMedicineTypeAcin/CreateList", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeAcinAdds, param);
                            if (result != null)
                            {
                                success = true;
                            }
                        }

                        // xóa
                        if (this.listActiveIngredientCheck != null && this.listActiveIngredientCheck.Count > 0)
                        {
                            List<ActiveIngredientADO> activeIngredientDeleteAdos = new List<ActiveIngredientADO>();
                            activeIngredientDeleteAdos = this.listActiveIngredientCheck.Where(p => !activeIngredientAdoChecks.Select(o => o.ID).Contains(p.ID)).ToList();

                            if (activeIngredientDeleteAdos != null && activeIngredientDeleteAdos.Count > 0)
                            {
                                List<long> medicineTypeAcineDeleteIds = new List<long>();
                                var medicineTypeAcineDeletes = this.medicineTypeAcins.Where(o => activeIngredientDeleteAdos.Select(p => p.ID).Contains(o.ACTIVE_INGREDIENT_ID) && o.MEDICINE_TYPE_ID == this.medicineTypeId).ToList();
                                medicineTypeAcineDeleteIds = medicineTypeAcineDeletes.Select(o => o.ID).ToList();
                                var resultDelete = new BackendAdapter(param).Post<bool>("/api/HisMedicineTypeAcin/DeleteList", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeAcineDeleteIds, param);
                                if (resultDelete)
                                {
                                    success = true;
                                }
                            }
                        }
                    }
                    if (success)
                    {
                        //FillDataToGrid2();
                        btnSaveServiceGroup.Enabled = false;

                    }
                }
                else
                {
                    MessageManager.Show("Chưa chọn hoạt chất");
                    return;
                }
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewActiveIngredient_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check2")
                        {
                            var lstCheckAll = this.listActiveIngredient;
                            List<ActiveIngredientADO> lstChecks = new List<ActiveIngredientADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }


                                //ReloadData
                                gridControlActiveIngredient.BeginUpdate();
                                gridControlActiveIngredient.DataSource = lstChecks;
                                gridControlActiveIngredient.EndUpdate();
                                //??

                            }
                        }
                    }

                    if (hi.InRowCell && hi.Column.FieldName == "check2")
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RadioSelect_Click(object sender, EventArgs e)
        {

        }

        private void gridViewActiveIngredient_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var focusRow = gridViewActiveIngredient.GetFocusedRow() as ActiveIngredientADO;

                if (e.Column.FieldName == "check2" && focusRow != null)
                {
                    var checkExistItem = listActiveIngredient.FirstOrDefault(o => o.ID == focusRow.ID);
                    if (checkExistItem.check2)
                    {
                        listActiveIngredientCheckForGridView.Add(checkExistItem);
                    }
                    else
                    {
                        listActiveIngredientCheckForGridView.RemoveAll(o => o.ID == checkExistItem.ID);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSaveServiceGroup_Click(null, null);
        }

        private void frmHisMedicineTypeAcin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //try
            //{
            //    if (this.resultActiveIngredient != null)
            //    {
            //        var indexSelects = ((List<ActiveIngredientADO>)this.gridViewActiveIngredient.DataSource).Where(o => o.check2).ToList();
            //        List<HIS_ACTIVE_INGREDIENT> listActiveIngredient = new List<HIS_ACTIVE_INGREDIENT>();
            //        if (indexSelects != null)
            //            foreach (var item in indexSelects)
            //            {
            //                HIS_ACTIVE_INGREDIENT activeIngredient = new HIS_ACTIVE_INGREDIENT();
            //                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ACTIVE_INGREDIENT>(activeIngredient, item);
            //                listActiveIngredient.Add(activeIngredient);

            //            }
            //        this.resultActiveIngredient(new List<HIS_ACTIVE_INGREDIENT>[] { listActiveIngredient });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void bbtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.resultActiveIngredient != null && this.lsActiveIngredientChecked != null)
                {
                    //var indexSelects = ((List<ActiveIngredientADO>)this.gridViewActiveIngredient.DataSource).Where(o => o.check2).ToList();
                    //List<HIS_ACTIVE_INGREDIENT> listActiveIngredient = new List<HIS_ACTIVE_INGREDIENT>();
                    //if (indexSelects != null)
                    //    foreach (var item in indexSelects)
                    //    {
                    //        HIS_ACTIVE_INGREDIENT activeIngredient = new HIS_ACTIVE_INGREDIENT();
                    //        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ACTIVE_INGREDIENT>(activeIngredient, item);
                    //        listActiveIngredient.Add(activeIngredient);

                    //    }
                    this.resultActiveIngredient(new List<HIS_ACTIVE_INGREDIENT>[] { this.lsActiveIngredientChecked });
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RadioSelect_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var datarow = (ActiveIngredientADO)gridViewActiveIngredient.GetFocusedRow();
                if (datarow != null)
                {
                    if (datarow.check2 == false)
                    {
                        bool check = this.lsActiveIngredientChecked.Any(o => o.ID == datarow.ID);
                        if (!check)
                        {
                            datarow.check2 = true;
                            this.lsActiveIngredientChecked.Add(datarow);
                        }
                    }
                    else
                    {
                        var remove = this.lsActiveIngredientChecked.FirstOrDefault(o => o.ID == datarow.ID);
                        if (remove != null)
                        {
                            this.lsActiveIngredientChecked.Remove(remove);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
