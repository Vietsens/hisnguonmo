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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.CompensationByBaseCreate.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.CompensationByBaseCreate
{
    public partial class UCCompensationByBaseCreate : UserControlBase
    {
        private V_HIS_MEDI_STOCK mediStock = null;
        private long expMediStockId;
        private int start = 0;
        private int limit = 0;
        private int dataTotal = 0;
        private int rowCount = 0;
        private const long TYPE_METY = (long)1;
        private const long TYPE_MATY = (long)2;
        private const long TYPE_KHA_DUNG = (long)3;
        private int lastRowHandle = -1;
        private ToolTipControlInfo lastInfo = null;

        HIS_EXP_MEST expMestPrint = null;

        private List<V_HIS_MEDI_STOCK_METY_1> mediStockMetys = null;
        private List<V_HIS_MEDI_STOCK_MATY_1> mediStockMatys = null;

        private List<MetyMatyADO> listCompensationAdo = new List<MetyMatyADO>();
        private bool isCheckAll = true;
        public bool IsReasonRequired { get; private set; }
        bool isInit = true;

        public static HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public static List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public UCCompensationByBaseCreate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void UCCompensationByBaseCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                bool isReadOnly = (this.mediStock == null) || (this.mediStock.IS_CABINET != 1);
                if (isReadOnly)
                {
                    btnNew.Enabled = false;
                    btnSave.Enabled = false;
                    XtraMessageBox.Show("Không phải tủ trực", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                this.SetDefaultValue();
                this.InitComboMediStock();
                this.LoadDataCompensation();
                this.LoadDataExpMest();
                IsReasonRequired = LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__IS_REASON_REQUIRED) == "1";
                LoadDataToComboReasonRequired();
                lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Black;
                if (IsReasonRequired)
                {
                    lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboReasonRequired);
                }
                this.isInit = false;
                this.InitControlState();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.CompensationByBaseCreate");
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == chkNotSelectMedi.Name)
                        {
                            chkNotSelectMedi.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                gridControlBcsExpMest.DataSource = null;
                gridControlBcsDetails.DataSource = null;
                gridControlCompensation.DataSource = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> _mediStocks = new List<V_HIS_MEDI_STOCK>();
                var datas = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == this.currentModuleBase.RoomId && p.IS_ACTIVE == 1).ToList();
                if (datas != null)
                {
                    List<long> mediStockIds = datas.Select(p => p.MEDI_STOCK_ID).ToList();
                    _mediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p =>
                        mediStockIds.Contains(p.ID)
                        && this.CheckBusiness(p)
                        && p.ID != this.mediStock.ID
                        && p.IS_ACTIVE == 1).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExpMediStock, _mediStocks, controlEditorADO);
                if (_mediStocks != null && _mediStocks.Count > 0)
                {
                    _mediStocks.OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    this.cboExpMediStock.EditValue = _mediStocks[0].ID;
                    this.expMediStockId = _mediStocks[0].ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckBusiness(V_HIS_MEDI_STOCK p)
        {
            try
            {
                if (this.mediStock.IS_BUSINESS == 1)
                {
                    return (p.IS_BUSINESS == 1);
                }
                else
                {
                    return (p.IS_BUSINESS != 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void LoadDataCompensation()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataCompensationToGrid(); }));
                }
                else
                {
                    this.LoadDataCompensationToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMest()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadBcsExpMest(); }));
                }
                else
                {
                    this.LoadBcsExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataCompensationToGrid()
        {
            try
            {
                listCompensationAdo = new List<MetyMatyADO>();
                HisMediStockMetyView1Filter mediStockMetyFilter = new HisMediStockMetyView1Filter();
                mediStockMetyFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mediStockMetys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_METY_1>>("api/HisMediStockMety/GetView1", ApiConsumers.MosConsumer, mediStockMetyFilter, null);

                HisMediStockMatyView1Filter mediStockMatyFilter = new HisMediStockMatyView1Filter();
                mediStockMatyFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mediStockMatys = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_MATY_1>>("api/HisMediStockMaty/GetView1", ApiConsumers.MosConsumer, mediStockMatyFilter, null);

                this.gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[6];

                //Lấy dữ liệu kho xuất đang chọn
                HisMedicineTypeStockViewFilter expMediStockFilter = new HisMedicineTypeStockViewFilter();
                expMediStockFilter.MEDI_STOCK_ID = this.expMediStockId;
                List<HisMedicineTypeInStockSDO> mediInExpStocks = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_GETINSTOCKMEDICINETYPE, ApiConsumers.MosConsumer, expMediStockFilter, null);

                HisMaterialTypeStockViewFilter expMateStockFilter = new HisMaterialTypeStockViewFilter();
                expMateStockFilter.MEDI_STOCK_ID = this.expMediStockId;
                List<HisMaterialTypeInStockSDO> mateInExpStocks = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, expMateStockFilter, null);

                //Lấy dữ liệu trong kho đang làm việc
                HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                mediFilter.MEDI_STOCK_ID = this.mediStock.ID;
                List<HisMedicineTypeInStockSDO> mediInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_GETINSTOCKMEDICINETYPE, ApiConsumers.MosConsumer, mediFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = this.mediStock.ID;
                List<HisMaterialTypeInStockSDO> mateInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);

                if (mediInStocks != null && mediInStocks.Count > 0)
                {
                    foreach (var item in mediInStocks)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.Id);
                        if (medicineType == null || medicineType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            continue;
                        }
                        MetyMatyADO ado = new MetyMatyADO();
                        ado.ACTIVE_INGR_BHYT_CODE = item.ActiveIngrBhytCode;
                        ado.ACTIVE_INGR_BHYT_NAME = item.ActiveIngrBhytName;
                        ado.CONCENTRA = item.Concentra;
                        ado.IN_STOCK_AMOUNT = (item.TotalAmount ?? 0);
                        ado.METY_MATY_CODE = item.MedicineTypeCode;
                        ado.METY_MATY_ID = item.Id;
                        ado.METY_MATY_NAME = item.MedicineTypeName;
                        ado.SERVICE_UNIT_NAME = item.ServiceUnitName;
                        V_HIS_MEDI_STOCK_METY_1 metyStock = this.mediStockMetys != null ? this.mediStockMetys.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.Id) : null;
                        if (metyStock != null)
                        {
                            ado.BASE_AMOUNT = (metyStock.ALERT_MAX_IN_STOCK ?? 0);
                            if (metyStock.EXP_MEDI_STOCK_ID.HasValue)
                            {
                                ado.MEDI_STOCK_NAME = metyStock.EXP_MEDI_STOCK_NAME;
                            }
                            ado.AMOUT_EXP_MEDI_STOCK = (metyStock.AMOUT_EXP_MEDI_STOCK ?? null);
                        }
                        else
                        {
                            ado.BASE_AMOUNT = 0;
                            HisMedicineTypeInStockSDO mediInExpStock = mediInExpStocks != null ? mediInExpStocks.FirstOrDefault(o => o.Id == item.Id) : null;
                            if (mediInExpStock != null)
                            {
                                ado.AMOUT_EXP_MEDI_STOCK = mediInExpStock.AvailableAmount ?? null;
                            }
                        }
                        if (ado.AMOUT_EXP_MEDI_STOCK != null && ado.AMOUT_EXP_MEDI_STOCK < 1)
                            ado.TYPE = TYPE_KHA_DUNG;
                        else
                            ado.TYPE = TYPE_METY;

                        listCompensationAdo.Add(ado);
                    }
                }

                if (this.mediStockMetys != null && this.mediStockMetys.Count > 0)
                {
                    foreach (var item in this.mediStockMetys)
                    {
                        if (listCompensationAdo.Any(a => a.TYPE == TYPE_METY && a.METY_MATY_ID == item.MEDICINE_TYPE_ID)) continue;
                        V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (medicineType == null || medicineType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            continue;
                        }
                        MetyMatyADO ado = new MetyMatyADO();
                        ado.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                        ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                        ado.CONCENTRA = medicineType.CONCENTRA;
                        ado.IN_STOCK_AMOUNT = 0;
                        ado.METY_MATY_CODE = medicineType.MEDICINE_TYPE_CODE;
                        ado.METY_MATY_ID = medicineType.ID;
                        ado.METY_MATY_NAME = medicineType.MEDICINE_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                        //ado.TYPE = TYPE_METY;

                        ado.BASE_AMOUNT = (item.ALERT_MAX_IN_STOCK ?? 0);
                        ado.AMOUT_EXP_MEDI_STOCK = (item.AMOUT_EXP_MEDI_STOCK ?? null);
                        if (item.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = item.EXP_MEDI_STOCK_NAME;
                        }
                        if (ado.AMOUT_EXP_MEDI_STOCK != null && ado.AMOUT_EXP_MEDI_STOCK < 1)
                            ado.TYPE = TYPE_KHA_DUNG;
                        else
                            ado.TYPE = TYPE_METY;
                        listCompensationAdo.Add(ado);
                    }
                }

                if (mateInStocks != null && mateInStocks.Count > 0)
                {
                    foreach (var item in mateInStocks)
                    {
                        V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.Id);
                        if (materialType == null || materialType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            continue;
                        }
                        MetyMatyADO ado = new MetyMatyADO();
                        ado.CONCENTRA = item.Concentra;
                        ado.IN_STOCK_AMOUNT = (item.TotalAmount ?? 0);
                        ado.METY_MATY_CODE = item.MaterialTypeCode;
                        ado.METY_MATY_ID = item.Id;
                        ado.METY_MATY_NAME = item.MaterialTypeName;
                        ado.SERVICE_UNIT_NAME = item.ServiceUnitName;
                        //ado.TYPE = TYPE_MATY;

                        V_HIS_MEDI_STOCK_MATY_1 matyStock = this.mediStockMatys != null ? this.mediStockMatys.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.Id) : null;
                        if (matyStock != null)
                        {
                            ado.BASE_AMOUNT = (matyStock.ALERT_MAX_IN_STOCK ?? 0);
                            if (matyStock.EXP_MEDI_STOCK_ID.HasValue)
                            {
                                ado.MEDI_STOCK_NAME = matyStock.EXP_MEDI_STOCK_NAME;
                            }
                            ado.AMOUT_EXP_MEDI_STOCK = (matyStock.AMOUT_EXP_MEDI_STOCK ?? null);
                        }
                        else
                        {
                            ado.BASE_AMOUNT = 0;
                            HisMaterialTypeInStockSDO mateInExpStock = mateInExpStocks != null ? mateInExpStocks.FirstOrDefault(o => o.Id == item.Id) : null;
                            if (mateInExpStock != null)
                            {
                                ado.AMOUT_EXP_MEDI_STOCK = mateInExpStock.AvailableAmount ?? null;
                            }
                        }
                        if (ado.AMOUT_EXP_MEDI_STOCK != null && ado.AMOUT_EXP_MEDI_STOCK < 1)
                            ado.TYPE = TYPE_KHA_DUNG;
                        else
                            ado.TYPE = TYPE_MATY;
                        listCompensationAdo.Add(ado);
                    }
                }

                if (this.mediStockMatys != null && this.mediStockMatys.Count > 0)
                {
                    foreach (var item in this.mediStockMatys)
                    {
                        if (listCompensationAdo.Any(a => a.TYPE == TYPE_MATY && a.METY_MATY_ID == item.MATERIAL_TYPE_ID)) continue;
                        V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (materialType == null || materialType.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            continue;
                        }
                        MetyMatyADO ado = new MetyMatyADO();
                        ado.CONCENTRA = materialType.CONCENTRA;
                        ado.IN_STOCK_AMOUNT = 0;
                        ado.METY_MATY_CODE = materialType.MATERIAL_TYPE_CODE;
                        ado.METY_MATY_ID = materialType.ID;
                        ado.METY_MATY_NAME = materialType.MATERIAL_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                        //ado.TYPE = TYPE_MATY;

                        ado.BASE_AMOUNT = (item.ALERT_MAX_IN_STOCK ?? 0);
                        ado.AMOUT_EXP_MEDI_STOCK = (item.AMOUT_EXP_MEDI_STOCK ?? null);
                        if (item.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = item.EXP_MEDI_STOCK_NAME;
                        }
                        if (ado.AMOUT_EXP_MEDI_STOCK != null && ado.AMOUT_EXP_MEDI_STOCK < 1)
                            ado.TYPE = TYPE_KHA_DUNG;
                        else
                            ado.TYPE = TYPE_MATY;
                        listCompensationAdo.Add(ado);
                    }
                }
                bool isCheck = false;
                listCompensationAdo.ForEach(o =>
                {
                    if (o.BASE_AMOUNT > 0 && o.BASE_AMOUNT > o.IN_STOCK_AMOUNT)
                    {
                        isCheck = true;
                        o.COMPENSATION_AMOUNT = o.BASE_AMOUNT - o.IN_STOCK_AMOUNT;
                        o.IsCheck = true;
                    }
                });
                listCompensationAdo = listCompensationAdo.OrderByDescending(o => o.IsCheck).ToList();
                if (isCheck)
                {
                    gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[5];
                    isCheckAll = false;
                }
                else
                {
                    gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[6];
                    isCheckAll = true;
                }
                gridControlCompensation.BeginUpdate();
                gridControlCompensation.DataSource = listCompensationAdo;
                gridControlCompensation.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadBcsExpMest()
        {
            try
            {
                int pageSize = ucPagingBcsExpMest.pagingGrid != null ? ucPagingBcsExpMest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingExpMestBCS(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingBcsExpMest.Init(PagingExpMestBCS, param, pageSize, gridControlBcsExpMest);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PagingExpMestBCS(object param)
        {
            try
            {
                List<HIS_EXP_MEST> listData = new List<HIS_EXP_MEST>();
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisExpMestFilter bcsFilter = new HisExpMestFilter();
                bcsFilter.ORDER_FIELD = "EXP_MEST_CODE";
                bcsFilter.ORDER_DIRECTION = "DESC";
                bcsFilter.IMP_MEDI_STOCK_ID = this.mediStock.ID;
                bcsFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, bcsFilter, paramCommon);
                if (apiResult != null)
                {
                    listData = (List<HIS_EXP_MEST>)apiResult.Data;
                    if (listData != null)
                    {
                        rowCount = listData.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
                gridControlBcsExpMest.BeginUpdate();
                gridControlBcsExpMest.DataSource = listData;
                gridControlBcsExpMest.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "COMPENSATION_AMOUNT")
                    return;
                var data = (MetyMatyADO)gridViewCompensation.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.IsCheck)
                    {
                        if (data.COMPENSATION_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng yêu cầu bù phải lớn hơn 0";
                        }
                        else if (data.COMPENSATION_AMOUNT > (data.BASE_AMOUNT - data.IN_STOCK_AMOUNT))
                        {
                            valid = false;
                            message = String.Format("Số lượng yêu cầu bù {0} lớn hơn số lượng cần bù {1}", data.COMPENSATION_AMOUNT, (data.BASE_AMOUNT - data.IN_STOCK_AMOUNT));
                        }

                    }
                    if (!valid)
                        gridViewCompensation.SetColumnError(gridViewCompensation.FocusedColumn, message);
                    else
                        gridViewCompensation.ClearColumnErrors();
                }
                gridControlCompensation.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewCompensation.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "COMPENSATION_AMOUNT")
                    {
                        if (row.IsCheck)
                        {
                            e.RepositoryItem = repositoryItemSpin_CompensationAmount_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpin_CompensationAmount_Disable;
                        }
                    }
                    if (e.Column.FieldName == "IsCheck")
                    {
                        if (row.BASE_AMOUNT > 0 && row.BASE_AMOUNT > row.IN_STOCK_AMOUNT)
                        {
                            e.RepositoryItem = repositoryItemCheck_IsCheck_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheck_IsCheck_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridViewCompensation_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[5];
                            gridControlCompensation.BeginUpdate();
                            if (this.listCompensationAdo == null)
                                this.listCompensationAdo = new List<MetyMatyADO>();
                            if (isCheckAll)
                            {
                                listCompensationAdo.ForEach(o =>
                                {
                                    if (o.BASE_AMOUNT > 0 && o.BASE_AMOUNT > o.IN_STOCK_AMOUNT)
                                    {
                                        o.COMPENSATION_AMOUNT = o.BASE_AMOUNT - o.IN_STOCK_AMOUNT;
                                        o.IsCheck = true;
                                    }

                                    //if (o.COMPENSATION_AMOUNT > 0 && o.AMOUT_EXP_MEDI_STOCK < 1)
                                    //    chkNotSelectMedi.Checked = false;
                                });
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[6];
                                listCompensationAdo.ForEach(o =>
                                {
                                    o.COMPENSATION_AMOUNT = 0;
                                    o.IsCheck = false;
                                });
                                isCheckAll = true;
                            }
                            gridControlCompensation.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewCompensation.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.IsCheck)
                    {
                        if (row.TYPE == TYPE_MATY)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else if (row.TYPE == TYPE_METY)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsExpMest_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (HIS_EXP_MEST)gridViewBcsExpMest.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "DELETE")
                    {
                        if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            || data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            e.RepositoryItem = repositoryItemButtonDelete_Enable;
                        else
                            e.RepositoryItem = repositoryItemButtonDelete_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsExpMest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HIS_EXP_MEST row = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(row.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MEDI_STOCK_NAME")
                    {
                        var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == row.MEDI_STOCK_ID);
                        if (mediStock != null)
                        {
                            e.Value = mediStock.MEDI_STOCK_NAME;
                        }
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        //trang: dang gui YC : màu vàng
                        if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu xanh
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsExpMest_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS_EXP_MEST row = (HIS_EXP_MEST)gridViewBcsExpMest.GetFocusedRow();
                LoadBcsDetail(row);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsDetails_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                MetyMatyADO row = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
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

        private void LoadBcsDetail(HIS_EXP_MEST row)
        {
            try
            {
                List<MetyMatyADO> listData = new List<MetyMatyADO>();
                if (row != null)
                {
                    HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = row.ID;
                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyFilter, null);

                    HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = row.ID;
                    List<HIS_EXP_MEST_METY_REQ> metyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, null);

                    if (metyReqs != null && metyReqs.Count > 0)
                    {
                        var Groups = metyReqs.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                            MetyMatyADO ado = new MetyMatyADO();
                            ado.AMOUNT = group.Sum(s => s.AMOUNT);
                            ado.METY_MATY_ID = group.Key;
                            if (medicineType != null)
                            {
                                ado.METY_MATY_CODE = medicineType.MEDICINE_TYPE_CODE;
                                ado.METY_MATY_NAME = medicineType.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            }
                            ado.TYPE = TYPE_METY;
                            listData.Add(ado);
                        }
                    }

                    if (matyReqs != null && matyReqs.Count > 0)
                    {
                        var Groups = matyReqs.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                            MetyMatyADO ado = new MetyMatyADO();
                            ado.AMOUNT = group.Sum(s => s.AMOUNT);
                            ado.METY_MATY_ID = group.Key;
                            if (materialType != null)
                            {
                                ado.METY_MATY_CODE = materialType.MATERIAL_TYPE_CODE;
                                ado.METY_MATY_NAME = materialType.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            }
                            ado.TYPE = TYPE_MATY;
                            listData.Add(ado);
                        }
                    }
                }
                gridControlBcsDetails.BeginUpdate();
                gridControlBcsDetails.DataSource = listData;
                gridControlBcsDetails.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (HIS_EXP_MEST)gridViewBcsExpMest.GetFocusedRow();
                if (dataRow != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Bạn có chắc muốn hủy dữ liệu?",
                        "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = dataRow.ID;
                        sdo.ReqRoomId = this.currentModuleBase.RoomId;
                        if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/BaseCompensationDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadBcsExpMest();
                            }
                        }
                        else if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__BASE)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/CompensationByBaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadBcsExpMest();
                            }
                        }
                        else
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadBcsExpMest();
                            }
                        }

                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.expMestPrint = (HIS_EXP_MEST)gridViewBcsExpMest.GetFocusedRow();
                if (this.expMestPrint != null)
                {
                    onClickPrintPhieuXuatBCS(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck_IsCheck_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewCompensation.PostEditor();
                MetyMatyADO row = (MetyMatyADO)gridViewCompensation.GetFocusedRow();
                if (row != null)
                {
                    if (row.IsCheck)
                    {
                        if (row.COMPENSATION_AMOUNT > 0 && row.AMOUT_EXP_MEDI_STOCK < 1)
                            chkNotSelectMedi.Checked = false;
                        row.COMPENSATION_AMOUNT = (row.BASE_AMOUNT - row.IN_STOCK_AMOUNT);
                    }
                    else
                    {
                        row.COMPENSATION_AMOUNT = 0;
                    }
                }
                gridControlCompensation.RefreshDataSource();
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
                if (!btnSave.Enabled) return;
                lastRowHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                List<MetyMatyADO> checkDatas = this.listCompensationAdo != null ? listCompensationAdo.Where(o => o.IsCheck).ToList() : null;
                if (checkDatas == null || checkDatas.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn thuốc/vật tư cần bù", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                var existsError = checkDatas.Where(o => o.COMPENSATION_AMOUNT > (o.BASE_AMOUNT - o.IN_STOCK_AMOUNT)).ToList();
                if (existsError == null || existsError.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư có số lượng yêu cầu bù lớn hơn số lượng cần bù: {0}", String.Join(",", existsError.Select(s => s.METY_MATY_NAME).ToList()));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                existsError = checkDatas.Where(o => o.COMPENSATION_AMOUNT <= 0).ToList();
                if (existsError == null || existsError.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư có số lượng yêu cầu bù nhỏ hơn hoặc bằng 0: {0}", String.Join(",", existsError.Select(s => s.METY_MATY_NAME).ToList()));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                if (checkDatas.Any(a => String.IsNullOrWhiteSpace(a.MEDI_STOCK_NAME)) && cboExpMediStock.EditValue == null)
                {
                    XtraMessageBox.Show("Tồn tại thuốc chưa được thiết lập kho xuất. Yêu cầu bạn chọn kho xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                var mediError = checkDatas.Where(o => o.AMOUT_EXP_MEDI_STOCK < 1).ToList();
                if (mediError == null || mediError.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư {0} có Số lượng khả dụng kho xuất đã hết. Bạn có muốn tiếp tục?", String.Join(",", mediError.Select(s => s.METY_MATY_NAME).ToList()));
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                CabinetBaseCompensationSDO sdo = new CabinetBaseCompensationSDO();
                if (cboReasonRequired.EditValue != null)
                    sdo.ExpMestReasonId = Int64.Parse(cboReasonRequired.EditValue.ToString());
                sdo.CabinetMediStockId = this.mediStock.ID;
                if (cboExpMediStock.EditValue != null)
                {
                    sdo.MediStockId = Convert.ToInt64(cboExpMediStock.EditValue);
                }
                sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                foreach (var item in checkDatas)
                {
                    if (item.TYPE == TYPE_METY)
                    {
                        if (sdo.MedicineTypes == null) sdo.MedicineTypes = new List<BaseMedicineTypeSDO>();
                        BaseMedicineTypeSDO mt = new BaseMedicineTypeSDO();
                        mt.Amount = item.COMPENSATION_AMOUNT;
                        mt.MedicineTypeId = item.METY_MATY_ID;
                        sdo.MedicineTypes.Add(mt);
                    }
                    else if (item.TYPE == TYPE_MATY)
                    {
                        if (sdo.MaterialTypes == null) sdo.MaterialTypes = new List<BaseMaterialTypeSDO>();
                        BaseMaterialTypeSDO mt = new BaseMaterialTypeSDO();
                        mt.Amount = item.COMPENSATION_AMOUNT;
                        mt.MaterialTypeId = item.METY_MATY_ID;
                        sdo.MaterialTypes.Add(mt);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                List<HIS_EXP_MEST> rs = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/CompensationByBaseCreate", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null && rs.Count > 0)
                {
                    success = true;
                    btnNew_Click(null, null);
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled) return;
                WaitingManager.Show();
                this.SetDefaultValue();
                this.LoadDataCompensationToGrid();
                this.LoadBcsExpMest();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlBcsExpMest)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlBcsExpMest.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle)
                        {
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                long stt = (long)view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_ID");
                                var name = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(p => p.ID == stt);
                                if (name != null)
                                    text = name.EXP_MEST_STT_NAME;
                                else
                                    text = "";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
                if (e.Info == null && e.SelectedControl == gridControlCompensation)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlCompensation.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle)
                        {
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            bool khadung = ((MetyMatyADO)view.GetRow(lastRowHandle)).TYPE == TYPE_KHA_DUNG;
                            if (khadung)
                            {
                                text = "Không đủ khả dụng kho xuất";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void NEW()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SAVE()
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

        private void onClickPrintPhieuXuatBCS(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("MPS000215", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case "MPS000215":
                            InPhieuXuatBCS(ref result, printTypeCode, fileName);
                            break;
                        case "MPS000254":
                            MPS000254(ref result, printTypeCode, fileName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DC_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCGNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCHTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TCs { get; set; }

        V_HIS_EXP_MEST _BcsExpMest { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_TREATMENT> ListTreatment { get; set; }

        private void InPhieuXuatBCS(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                long keyOrder = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODER_OPTION));
                #region TT Chung
                WaitingManager.Show();
                HisExpMestViewFilter bcsFilter = new HisExpMestViewFilter();
                bcsFilter.ID = this.expMestPrint.ID;
                var listBcsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, bcsFilter, null);
                if (listBcsExpMest == null || listBcsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                _BcsExpMest = listBcsExpMest.First();

                CommonParam param = new CommonParam();

                _ExpMestMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                ListTreatment = new List<HIS_TREATMENT>();
                List<long> treatmentIds = new List<long>();

                if (_BcsExpMest != null)
                {
                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/get", ApiConsumers.MosConsumer, metyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMetyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/get", ApiConsumers.MosConsumer, matyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMatyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    if (_BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || _BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    }

                    if (treatmentIds != null && treatmentIds.Count > 0)
                    {
                        int skip = 0;
                        while (treatmentIds.Count - skip > 0)
                        {
                            var listIds = treatmentIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                            treatFilter.IDs = listIds;
                            var treat = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                            if (treat != null && treat.Count > 0)
                            {
                                ListTreatment.AddRange(treat);
                            }
                        }
                    }
                }
                #endregion

                //#region In Tong Hop
                //if (keyPrintType == 1)
                //{
                //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                //    MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                //(
                // _BcsExpMest,
                // _ExpMestMedicines,
                // _ExpMestMaterials,
                // _ExpMestMetyReqs,
                // _ExpMestMatyReqs,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                // MPS.Processor.Mps000215.PDO.keyTitles.tonghop,
                // ListTreatment
                //  );
                //    WaitingManager.Hide();
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //#endregion
                //else
                {
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();
                    List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();

                    #region --- Xu Ly Tach GN_HT -----
                    if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                    {
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                        var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                        bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);

                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in _ExpMestMetyReqs)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCGNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCHTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                                {
                                    _ExpMestMetyReq_TCs.Add(item);
                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    WaitingManager.Hide();
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("MPS000254", delegatePrintTemplate);

                    #region ----VatTu----
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 _ExpMestMatyReqs,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.vattu,
                  ListTreatment,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region ----- Thuong ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.thuong,
                  ListTreatment,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- TC ----
                    if (_ExpMestMetyReq_TCs != null && _ExpMestMetyReq_TCs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_TCs,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.tienchat,
                  ListTreatment,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MPS000254(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GOP GN HT -----
                    if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GN_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.tonghop,
                 ListTreatment
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion


                    #region ---- GOP DC GN HT -----
                    if (_ExpMestMetyReq_DC_GN_HTs != null && _ExpMestMetyReq_DC_GN_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDODC = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                      (
                        this._BcsExpMest,
                       _ExpMestMedicines,
                       _ExpMestMetyReq_DC_GN_HTs,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                       BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                       BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000254.PDO.keyTitles.tonghopHc,
                        ListTreatment
                        );

                        MPS.ProcessorBase.Core.PrintData PrintDataDC = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintDataDC);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GNs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.gaynghien,
                 ListTreatment
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT -----
                    if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.huongthan,
                 ListTreatment
                 );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCGN ----
                    if (_ExpMestMetyReq_DCGNs != null && _ExpMestMetyReq_DCGNs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._BcsExpMest,
                            _ExpMestMedicines,
                            _ExpMestMetyReq_DCGNs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcGayNghien,
                             ListTreatment

                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCHT -----
                    if (_ExpMestMetyReq_DCHTs != null && _ExpMestMetyReq_DCHTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._BcsExpMest,
                            _ExpMestMedicines,
                            _ExpMestMetyReq_DCHTs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcHuongThan,
                             ListTreatment

                             );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ----- TD -----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_TDs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocdoc,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX -----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_PXs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocphongxa,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO -----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_COs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Corticoid,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT -----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_DTs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.DichTruyen,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS -----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_KSs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.KhangSinh,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- lao -----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_LAOs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Lao,
             ListTreatment
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboReasonRequired()
        {
            try
            {
                var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboReasonRequired, reason, controlEditorADO);
                cboReasonRequired.Properties.ImmediatePopup = true;
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
                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (lastRowHandle == -1)
                {
                    lastRowHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (lastRowHandle > edit.TabIndex)
                {
                    lastRowHandle = edit.TabIndex;
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

        private void chkNotSelectMedi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkNotSelectMedi.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.CompensationByBaseCreate").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkNotSelectMedi.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotSelectMedi.Name;
                    csAddOrUpdate.VALUE = chkNotSelectMedi.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.CompensationByBaseCreate";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();
                if (chkNotSelectMedi.Checked)
                {
                    bool isCheck = true;
                    listCompensationAdo.ForEach(o =>
                    {
                        if (o.COMPENSATION_AMOUNT > 0 && o.AMOUT_EXP_MEDI_STOCK < 1)
                        {
                            isCheck = false;
                            o.IsCheck = false;
                        }
                    });
                    listCompensationAdo = listCompensationAdo.OrderByDescending(o => o.IsCheck).ToList();
                    if (isCheck)
                    {
                        gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[5];
                        isCheckAll = false;
                    }
                    else
                    {
                        gridColumn_Compensation_IsCheck.Image = imageListIcon.Images[6];
                        isCheckAll = true;
                    }
                    gridControlCompensation.BeginUpdate();
                    gridControlCompensation.DataSource = listCompensationAdo;
                    gridControlCompensation.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                if (cboExpMediStock.EditValue != null && cboExpMediStock.EditValue != cboExpMediStock.OldEditValue)
                {
                    this.expMediStockId = Convert.ToInt64(cboExpMediStock.EditValue);
                    LoadDataCompensationToGrid();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
