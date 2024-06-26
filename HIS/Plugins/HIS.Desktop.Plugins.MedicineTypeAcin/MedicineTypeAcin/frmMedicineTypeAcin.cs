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
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.MedicineTypeAcin.ADO;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.ImpMestType.ADO;
using HIS.UC.Account.ADO;
using ACS.Filter;
using HIS.UC.HisActiveIngredient;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using System.Linq;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.HisActiveIngredient.ADO;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using System.Drawing;

namespace HIS.Desktop.Plugins.MedicineTypeAcin.MedicineTypeAcin
{
    public partial class frmMedicineTypeAcin : DevExpress.XtraEditors.XtraForm
    {
        #region Declare
        List<HIS.UC.HisActiveIngredient.ActiveIngredientADO> ActiveIngredientADOs;
        HisActiveIngredientInitADO activeIngredientADO;
        ActiveIngredientProcessor activeIngredientProcessor;
        public UserControl ucActiveIngredient;
        internal List<HIS.UC.HisActiveIngredient.ActiveIngredientADO> listActiveIngredientADO;
        bool isCheckAll;
        int rowCount = 0;
        int dataTotal = 0;
        long medicineTypeId = 0;
        public long? serviceGroupIdCheckByService;

        #endregion

        #region Constructor
        public frmMedicineTypeAcin(long _medicineTypeId)
        {
            try
            {
                this.medicineTypeId = _medicineTypeId;
                InitializeComponent();
                this.medicineTypeId = 4327;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMedicineTypeAcin_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                InitActiveIngredientUC();
                FillDataToGridActiveIngredient();
                LoadDataToMedicineTypeInfo(this.medicineTypeId);
                SetIcon();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        #region LoadColumn

        private void FillDataToGridActiveIngredient()
        {
            try
            {
                int numPageSize = 1000;
                FillDataToGridActiveIngredientByPaging(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(FillDataToGridActiveIngredientByPaging, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitActiveIngredientUC()
        {
            try
            {
                this.activeIngredientProcessor = new ActiveIngredientProcessor();
                activeIngredientADO = new HisActiveIngredientInitADO();
                activeIngredientADO.activeIngredientColumns = new List<ActiveIngredientColumn>();
                activeIngredientADO.gridViewService_MouseDown = gridViewAccount_MouseDownAccountDelegate;
                activeIngredientADO.ServiceGrid_CustomUnboundColumnData = ServiceGrid_CustomUnboundColumnData;
                ActiveIngredientColumn colCheck2 = new ActiveIngredientColumn(" ", "check2", 20, true, false);
                colCheck2.VisibleIndex = 0;
                colCheck2.Visible = true;
                colCheck2.image = imageCollectionMediStock.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                activeIngredientADO.activeIngredientColumns.Add(colCheck2);

                ActiveIngredientColumn colActiveIngredientCode = new ActiveIngredientColumn("Mã hoạt chất", "ACTIVE_INGREDIENT_CODE", 100, false, true);
                colActiveIngredientCode.VisibleIndex = 1;
                colActiveIngredientCode.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colActiveIngredientCode);

                ActiveIngredientColumn colActiveIngredientName = new ActiveIngredientColumn("Tên hoạt chất", "ACTIVE_INGREDIENT_NAME", 250, false, true);
                colActiveIngredientName.VisibleIndex = 2;
                colActiveIngredientName.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colActiveIngredientName);

                ActiveIngredientColumn colCreator = new ActiveIngredientColumn("Người tạo", "CREATOR", 100, false, true);
                colCreator.VisibleIndex = 3;
                colCreator.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colCreator);

                ActiveIngredientColumn colCreateTimeDisplay = new ActiveIngredientColumn("Thời gian tạo", "CREATE_TIME_DISPLAY", 120, false, true);
                colCreateTimeDisplay.VisibleIndex = 4;
                colCreateTimeDisplay.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCreateTimeDisplay.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colCreateTimeDisplay);

                ActiveIngredientColumn colModifier = new ActiveIngredientColumn("Người sửa", "MODIFIER", 100, false, true);
                colModifier.VisibleIndex = 5;
                colModifier.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colModifier);

                ActiveIngredientColumn colModifierTimeDisplay = new ActiveIngredientColumn("Thời gian sửa", "MODIFIER_TIME_DISPLAY", 120, false, true);
                colModifierTimeDisplay.VisibleIndex = 6;
                colModifierTimeDisplay.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colModifierTimeDisplay.Visible = true;
                activeIngredientADO.activeIngredientColumns.Add(colModifierTimeDisplay);

                this.ucActiveIngredient = (UserControl)this.activeIngredientProcessor.Run(activeIngredientADO);
                if (this.ucActiveIngredient != null)
                {
                    this.pnlActiveIngredient.Controls.Add(this.ucActiveIngredient);
                    this.ucActiveIngredient.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ServiceGrid_CustomUnboundColumnData(HIS_ACTIVE_INGREDIENT data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                {
                    Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(data.CREATE_TIME ?? 0);
                }
                else if (e.Column.FieldName == "MODIFIER_TIME_DISPLAY")
                {
                    Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(data.MODIFY_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccount_MouseDownAccountDelegate(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check2")
                        {
                            var lstCheckAll = listActiveIngredientADO;
                            List<HIS.UC.HisActiveIngredient.ActiveIngredientADO> lstChecks = new List<HIS.UC.HisActiveIngredient.ActiveIngredientADO>();

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
                                }

                                activeIngredientProcessor.Reload(ucActiveIngredient, lstChecks);
                            }
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Event

        private void btnSearchAccount_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridActiveIngredient();
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
                saveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywordUserAccount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridActiveIngredient();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load Data To Combo

        #endregion

        #region Method

        void LoadDataToMedicineTypeInfo(long medicneTypeId)
        {
            try
            {
                MOS.Filter.HisMedicineTypeViewFilter medicineTypeViewfilter = new HisMedicineTypeViewFilter();
                medicineTypeViewfilter.ID = medicineTypeId;
                var medicineType = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE_TYPE>>("/api/HisMedicineType/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineTypeViewfilter, new CommonParam()).FirstOrDefault();
                if (medicineType != null)
                {
                    lblMedicineTypeCode.Text = medicineType.MEDICINE_TYPE_CODE;
                    lblMedicineTypeName.Text = medicineType.MEDICINE_TYPE_NAME;
                    lblServiceUnitName.Text = medicineType.SERVICE_UNIT_NAME;
                }
                else
                {
                    lblServiceUnitName.Text = "";
                    lblMedicineTypeName.Text = "";
                    lblMedicineTypeCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void FillDataToGridActiveIngredientByPaging(object data)
        {
            try
            {
                if (data == null)
                    return;
                WaitingManager.Show();
                listActiveIngredientADO = new List<ActiveIngredientADO>();
                ActiveIngredientADOs = new List<ActiveIngredientADO>();
                int start = (((CommonParam)data).Start ?? 0);
                int limit = (((CommonParam)data).Limit ?? 0);
                CommonParam param = new CommonParam(start, limit);
                HisActiveIngredientFilter hisActiveIngredientFilter = new HisActiveIngredientFilter();
                hisActiveIngredientFilter.KEY_WORD = txtKeywordActiveIngredient.Text;
                hisActiveIngredientFilter.ORDER_FIELD = "ACTIVE_INGREDIENT_NAME";
                hisActiveIngredientFilter.ORDER_DIRECTION = "ASC";

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_ACTIVE_INGREDIENT>>(
                     HisRequestUriStore.HIS_ACTIVE_GREDIENT_GET,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisActiveIngredientFilter,
                     param);

                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    foreach (var item in rs.Data)
                    {
                        HIS.UC.HisActiveIngredient.ActiveIngredientADO activeIngredientADO = new HIS.UC.HisActiveIngredient.ActiveIngredientADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS.UC.HisActiveIngredient.ActiveIngredientADO>(activeIngredientADO, item);
                        listActiveIngredientADO.Add(activeIngredientADO);
                    }
                }
                if (ucActiveIngredient != null)
                {
                    activeIngredientProcessor.Reload(ucActiveIngredient, listActiveIngredientADO);
                }

                listActiveIngredientADO = (from r in listActiveIngredientADO select new HIS.UC.HisActiveIngredient.ActiveIngredientADO(r)).ToList();
                if (listActiveIngredientADO != null && listActiveIngredientADO.Count > 0)
                {
                    foreach (var activeIngredientADO in listActiveIngredientADO)
                    {
                        var check = listActiveIngredientADO.FirstOrDefault(o => o.ID == activeIngredientADO.ID);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }

                    listActiveIngredientADO = listActiveIngredientADO.OrderByDescending(p => p.check2).ToList();
                    if (ucActiveIngredient != null)
                    {
                        this.activeIngredientProcessor.Reload(ucActiveIngredient, listActiveIngredientADO);
                    }
                }

                rowCount = (data == null ? 0 : listActiveIngredientADO.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void saveProcess()
        {
            try
            {
                if (ucActiveIngredient != null)
                {
                    object activeIngredientADO = activeIngredientProcessor.GetDataGridView(ucActiveIngredient);
                    CommonParam param = new CommonParam();
                    bool success = false;

                    if (activeIngredientADO is List<HIS.UC.HisActiveIngredient.ActiveIngredientADO>)
                    {
                        var activeIngredientADOs = (List<HIS.UC.HisActiveIngredient.ActiveIngredientADO>)activeIngredientADO;

                        MOS.Filter.HisMedicineTypeAcinViewFilter hisMedicineTypeAcinFilter = new HisMedicineTypeAcinViewFilter();
                        hisMedicineTypeAcinFilter.MEDICINE_TYPE_ID = this.medicineTypeId;

                        var medicineTypeAcines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE_ACIN>>(
                        HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        hisMedicineTypeAcinFilter,
                        param);

                        if (activeIngredientADOs != null && activeIngredientADOs.Count > 0)
                        {
                            //Danh sach cac ingredient duoc check

                            var dataCheckeds = activeIngredientADOs.Where(p => p.check2 == true).ToList();

                            //List xoa
                            var dataDeletes = activeIngredientADOs.Where(o => medicineTypeAcines.Select(p => p.ACTIVE_INGREDIENT_ID)
                                .Contains(o.ID) && o.check2 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !medicineTypeAcines.Select(p => p.ACTIVE_INGREDIENT_ID)
                                .Contains(o.ID)).ToList();

                            if (dataDeletes != null && dataDeletes.Count > 0)
                            {
                                List<long> deleteIds = medicineTypeAcines.Where(o => dataDeletes.Select(p => p.ID)
                                    .Contains(o.ACTIVE_INGREDIENT_ID)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_DELETE_LIST,
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    success = true;
                                    medicineTypeAcines = medicineTypeAcines.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                }
                            }

                            if (dataCreates != null && dataCreates.Count > 0)
                            {
                                List<HIS_MEDICINE_TYPE_ACIN> medicineTypeAcins = new List<HIS_MEDICINE_TYPE_ACIN>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_MEDICINE_TYPE_ACIN medicineTypeAcine = new HIS_MEDICINE_TYPE_ACIN();
                                    medicineTypeAcine.ACTIVE_INGREDIENT_ID = item.ID;
                                    medicineTypeAcine.MEDICINE_TYPE_ID = this.medicineTypeId;
                                    medicineTypeAcins.Add(medicineTypeAcine);
                                }

                                var createResult = new BackendAdapter(param).Post<List<HIS_MEDICINE_TYPE_ACIN>>(
                                           HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_CREATE_LIST,
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           medicineTypeAcins,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    success = true;
                                    //medicineTypeAcines.AddRange(createResult);

                                }
                            }

                            activeIngredientADOs = activeIngredientADOs.OrderByDescending(p => p.check2).ToList();
                            if (ucActiveIngredient != null)
                            {
                                activeIngredientProcessor.Reload(ucActiveIngredient, activeIngredientADOs);
                            }
                            else
                            {
                                FillDataToGridActiveIngredient();
                            }

                            MessageManager.Show(this, param, success);
                            #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckAllAccounts(List<HIS.UC.HisActiveIngredient.ActiveIngredientADO> ActiveIngredientADOs, bool checkState)
        {
            try
            {
                foreach (var item in ActiveIngredientADOs)
                {
                    item.check2 = checkState;
                }
                if (ucActiveIngredient != null)
                {
                    activeIngredientProcessor.Reload(ucActiveIngredient, ActiveIngredientADOs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region public function
        public void SearchAccount()
        {
            try
            {
                FillDataToGridActiveIngredient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Save()
        {
            btnSave_Click(null, null);
        }
        #endregion


    }
}
