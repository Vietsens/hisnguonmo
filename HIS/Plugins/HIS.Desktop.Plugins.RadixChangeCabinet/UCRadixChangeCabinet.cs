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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.RadixChangeCabinet.ADO;
using DevExpress.Data;
using Inventec.Core;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestAggregate.Resources;

namespace HIS.Desktop.Plugins.RadixChangeCabinet
{
    public partial class UCRadixChangeCabinet : UserControl
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_MEDI_STOCK _MediStock { get; set; }
        long _CABINET_MEDI_STOCK_ID = 0;

        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        int theRequiredWidth = 900, theRequiredHeight = 220;
        MediMatyTypeADO currentMedicineTypeADOForEdit;

        List<MediMatyTypeADO> _MediMatyTypeADOs { get; set; }

        List<HisMedicineTypeInStockSDO> _MedicineTypeInStockSDOs { get; set; }
        List<HisMaterialTypeInStockSDO> _MaterialTypeInStockSDOs { get; set; }

        List<MedicineADO> _MedicineADOs { get; set; }
        List<MaterialADO> _MaterialADOs { get; set; }

        HisExpMestResultSDO _ExpMestResultSDO { get; set; }

        List<HIS_EXP_MEST_METY_REQ> _DataMetys { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _DataMatys { get; set; }

        V_HIS_EXP_MEST_4 _currentExpMest { get; set; }

        enum ActionType
        {
            Edit,
            Add
        }


        public UCRadixChangeCabinet()
        {
            InitializeComponent();
        }

        public UCRadixChangeCabinet(Inventec.Desktop.Common.Modules.Module _module, V_HIS_EXP_MEST_4 expMest, V_HIS_MEDI_STOCK _mediStock)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _module;
                this._currentExpMest = expMest;
                this._MediStock = _mediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCRadixChangeCabinet(Inventec.Desktop.Common.Modules.Module _module, V_HIS_MEDI_STOCK _mediStock)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _module;
                this._MediStock = _mediStock;
                this.btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRadixChangeCabinet_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataCbo();

                ValidateControlCbo();

                LoadDataByExpMest();

                LoadDataGrid();

                LoadDataToGridMetyMatyTypeInStock();

                if (_currentExpMest != null)
                {
                    btnSave.Enabled = false;
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = true;
                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByExpMest()
        {
            try
            {
                if (this._currentExpMest != null && this._currentExpMest.ID > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = this._currentExpMest.ID;

                    _DataMetys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);


                    MOS.Filter.HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = this._currentExpMest.ID;

                    _DataMatys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControlCbo()
        {
            try
            {
                cboValidationRule _rule = new cboValidationRule();
                _rule.cbo = this.cboKhoNhap;
                _rule.ErrorText = "Trường dữ liệu bắt buộc";
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboKhoNhap, _rule);

                cboValidationRule _rule2 = new cboValidationRule();
                _rule2.cbo = this.cboTuTrucXuat;
                _rule2.ErrorText = "Trường dữ liệu bắt buộc";
                _rule2.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboTuTrucXuat, _rule2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                WaitingManager.Show();
                gridControlMedicine.DataSource = null;
                gridControlMaterial.DataSource = null;
                var mestMetyDepaList = LoadMestMetyDepa();
                var mestMatyDepaList = LoadMestMatyDepa();
                if (!dxValidationProvider1.Validate())
                    return;

                if (this._MediStock != null && this._MediStock.IS_CABINET == 1)
                {
                    _CABINET_MEDI_STOCK_ID = this._MediStock.ID;
                }
                else
                    if (chkHoanCoSo.Checked)
                    {
                        _CABINET_MEDI_STOCK_ID = (long)cboTuTrucXuat.EditValue;
                    }
                    else if (chkBoSungCoSo.Checked)
                    {
                        _CABINET_MEDI_STOCK_ID = (long)cboKhoNhap.EditValue;
                    }

                HisMetyStockWithBaseInfoViewFilter mediFilter = new HisMetyStockWithBaseInfoViewFilter();
                mediFilter.EXP_MEDI_STOCK_ID = (long)cboTuTrucXuat.EditValue;
                mediFilter.CABINET_MEDI_STOCK_ID = _CABINET_MEDI_STOCK_ID;
                mediFilter.IS_AVAILABLE = true;
                mediFilter.IS_ACTIVE = true;
                _MedicineTypeInStockSDOs = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineTypeWithBaseInfo", ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                HisMatyStockWithBaseInfoViewFilter madiFilter = new HisMatyStockWithBaseInfoViewFilter();
                madiFilter.EXP_MEDI_STOCK_ID = (long)cboTuTrucXuat.EditValue;
                madiFilter.CABINET_MEDI_STOCK_ID = _CABINET_MEDI_STOCK_ID;
                madiFilter.IS_AVAILABLE = true;
                madiFilter.IS_ACTIVE = true;
                _MaterialTypeInStockSDOs = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialTypeWithBaseInfo", ApiConsumers.MosConsumer, madiFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                _MedicineADOs = new List<MedicineADO>();

                if (_MedicineTypeInStockSDOs != null && _MedicineTypeInStockSDOs.Count > 0)
                {
                    if (mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                    {
                        _MedicineTypeInStockSDOs = _MedicineTypeInStockSDOs.Where(o => !mestMetyDepaList.Select(p => p.MEDICINE_TYPE_ID).Contains(o.Id)).ToList();
                    }

                    if (this._DataMetys != null && this._DataMetys.Count > 0)
                    {
                        List<long> _medicineTypeIds = this._DataMetys.Select(p => p.MEDICINE_TYPE_ID).Distinct().ToList();
                        _MedicineTypeInStockSDOs = _MedicineTypeInStockSDOs.Where(p => p.BaseAmount != null || _medicineTypeIds.Contains(p.Id)).ToList();
                    }
                    else
                    {
                        _MedicineTypeInStockSDOs = _MedicineTypeInStockSDOs.Where(p => p.BaseAmount != null).ToList();
                    }

                    _MedicineADOs.AddRange((from r in _MedicineTypeInStockSDOs select new MedicineADO(r, this._DataMetys)).ToList());
                    _MedicineADOs = _MedicineADOs.OrderByDescending(p => p.EXP_MEST_ID).ThenBy(p => p.MedicineTypeCode).ToList();
                }

                _MaterialADOs = new List<MaterialADO>();

                if (_MaterialTypeInStockSDOs != null && _MaterialTypeInStockSDOs.Count > 0)
                {
                    if (mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                    {
                        _MaterialTypeInStockSDOs = _MaterialTypeInStockSDOs.Where(o => !mestMatyDepaList.Select(p => p.MATERIAL_TYPE_ID).Contains(o.Id)).ToList();
                    }

                    if (this._DataMatys != null && this._DataMatys.Count > 0)
                    {
                        List<long> _materialTypeIds = this._DataMatys.Select(p => p.MATERIAL_TYPE_ID).Distinct().ToList();
                        _MaterialTypeInStockSDOs = _MaterialTypeInStockSDOs.Where(p => p.BaseAmount != null || _materialTypeIds.Contains(p.Id)).ToList();
                    }
                    else
                    {
                        _MaterialTypeInStockSDOs = _MaterialTypeInStockSDOs.Where(p => p.BaseAmount != null).ToList();
                    }

                    _MaterialADOs.AddRange((from r in _MaterialTypeInStockSDOs select new MaterialADO(r, this._DataMatys)).ToList());
                    _MaterialADOs = _MaterialADOs.OrderByDescending(p => p.EXP_MEST_ID).ThenBy(p => p.MaterialTypeCode).ToList();
                }

                gridControlMedicine.DataSource = _MedicineADOs;
                gridControlMaterial.DataSource = _MaterialADOs;
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataCbo()
        {
            try
            {
                var datas = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.IS_ACTIVE == 1).ToList();

                Inventec.Common.Logging.LogSystem.Debug("HIS.Desktop.Plugins.RadixChangeCabinet SetDataCbo datas count" + datas.Count);

                List<ColumnInfo> columninfos = new List<ColumnInfo>();
                columninfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 150, 1));
                columninfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 300, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columninfos, false, 450);
                ControlEditorLoader.Load(cboKhoNhap, datas, controlEditorADO);

                List<ColumnInfo> columninfo2s = new List<ColumnInfo>();
                columninfo2s.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 150, 1));
                columninfo2s.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 300, 2));

                ControlEditorADO controlEditorADO2 = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columninfo2s, false, 450);
                ControlEditorLoader.Load(cboTuTrucXuat, datas, controlEditorADO2);

                this._DataMetys = null;
                this._DataMatys = null;


                if (this._currentExpMest != null && this._currentExpMest.ID > 0)
                {
                    this.btnPrint.Enabled = true;

                    cboTuTrucXuat.ReadOnly = true;
                    cboKhoNhap.ReadOnly = true;
                    chkHoanCoSo.ReadOnly = true;
                    chkBoSungCoSo.ReadOnly = true;

                    cboTuTrucXuat.EditValue = this._currentExpMest.MEDI_STOCK_ID;
                    cboKhoNhap.EditValue = this._currentExpMest.IMP_MEDI_STOCK_ID;
                    if (this._currentExpMest.CHMS_TYPE_ID != null && this._currentExpMest.CHMS_TYPE_ID == 1)
                    {
                        chkBoSungCoSo.Checked = true;
                        lciThuoc.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciSoLuong.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciThem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        gridColMediSLHoanBu.Caption = "Số lượng bổ sung";
                        gridColMateSLHoanBu.Caption = "Số lượng bổ sung";
                    }
                    else if (this._currentExpMest.CHMS_TYPE_ID != null && this._currentExpMest.CHMS_TYPE_ID == 2)
                    {
                        chkHoanCoSo.Checked = true;
                        lciThuoc.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciSoLuong.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciThem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        gridColMediSLHoanBu.Caption = "Số lượng hoàn";
                        gridColMateSLHoanBu.Caption = "Số lượng hoàn";
                    }
                }
                else
                {
                    if (chkHoanCoSo.Checked)
                    {
                        lciThuoc.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciSoLuong.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciThem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (chkBoSungCoSo.Checked)
                    {
                        lciThuoc.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciSoLuong.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciThem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    if (_MediStock.IS_CABINET == 1)
                    {
                        if (chkHoanCoSo.Checked)
                        {
                            cboTuTrucXuat.EditValue = _MediStock.ID;
                            cboTuTrucXuat.ReadOnly = true;
                            cboKhoNhap.EditValue = null;
                            cboKhoNhap.ReadOnly = false;

                            gridColMediSLHoanBu.Caption = "Số lượng hoàn";
                            gridColMateSLHoanBu.Caption = "Số lượng hoàn";

                            var dataMestRooms = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.MEDI_STOCK_ID == _MediStock.ID && p.IS_ACTIVE == 1).ToList();
                            var listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
                            if (dataMestRooms != null && dataMestRooms.Count > 0)
                            {
                                var roomIds = dataMestRooms.Select(s => s.ROOM_ID).ToList();
                                listCurrentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_BLOOD != 1 && roomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == 1 && o.IS_CABINET != 1).OrderBy(p => p.MEDI_STOCK_CODE).ToList();
                            }

                            if (listCurrentMediStock != null && listCurrentMediStock.Count > 0)
                            {
                                if (this._MediStock.IS_BUSINESS == 1)
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS == 1).ToList();
                                else
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS != 1).ToList();
                                cboKhoNhap.EditValue = listCurrentMediStock[0].ID;
                            }
                            cboKhoNhap.Properties.DataSource = listCurrentMediStock;
                        }
                        else
                        {
                            cboKhoNhap.EditValue = _MediStock.ID;
                            cboKhoNhap.ReadOnly = true;
                            cboTuTrucXuat.EditValue = null;
                            cboTuTrucXuat.ReadOnly = false;
                            gridColMediSLHoanBu.Caption = "Số lượng bổ sung";
                            gridColMateSLHoanBu.Caption = "Số lượng bổ sung";

                            var dataMestRooms = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == _MediStock.ROOM_ID && p.IS_ACTIVE == 1).ToList();
                            var listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
                            if (dataMestRooms != null && dataMestRooms.Count > 0)
                            {
                                var roomIds = dataMestRooms.Select(s => s.MEDI_STOCK_ID).ToList();
                                listCurrentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_BLOOD != 1 && roomIds.Contains(o.ID) && o.IS_ACTIVE == 1 && o.IS_CABINET != 1).OrderBy(p => p.MEDI_STOCK_CODE).ToList();
                            }

                            if (listCurrentMediStock != null && listCurrentMediStock.Count > 0)
                            {
                                if (this._MediStock.IS_BUSINESS == 1)
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS == 1).ToList();
                                else
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS != 1).ToList();
                                cboTuTrucXuat.EditValue = listCurrentMediStock[0].ID;
                            }
                            cboTuTrucXuat.Properties.DataSource = listCurrentMediStock;
                        }
                    }
                    else
                    {
                        if (chkHoanCoSo.Checked)
                        {
                            cboKhoNhap.EditValue = _MediStock.ID;
                            cboKhoNhap.ReadOnly = true;
                            cboTuTrucXuat.EditValue = null;
                            cboTuTrucXuat.ReadOnly = false;

                            gridColMediSLHoanBu.Caption = "Số lượng hoàn";
                            gridColMateSLHoanBu.Caption = "Số lượng hoàn";

                            var dataMestRooms = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.MEDI_STOCK_ID == _MediStock.ID && p.IS_ACTIVE == 1).ToList();
                            var listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
                            if (dataMestRooms != null && dataMestRooms.Count > 0)
                            {
                                var roomIds = dataMestRooms.Select(s => s.ROOM_ID).ToList();
                                listCurrentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_BLOOD != 1 && roomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == 1 && o.IS_CABINET == 1).OrderBy(p => p.MEDI_STOCK_CODE).ToList();
                            }

                            if (listCurrentMediStock != null && listCurrentMediStock.Count > 0)
                            {
                                if (this._MediStock.IS_BUSINESS == 1)
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS == 1).ToList();
                                else
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS != 1).ToList();

                                cboTuTrucXuat.EditValue = listCurrentMediStock[0].ID;
                            }
                            cboTuTrucXuat.Properties.DataSource = listCurrentMediStock;
                        }
                        else
                        {
                            cboTuTrucXuat.EditValue = _MediStock.ID;
                            cboTuTrucXuat.ReadOnly = true;
                            cboKhoNhap.EditValue = null;
                            cboKhoNhap.ReadOnly = false;

                            gridColMediSLHoanBu.Caption = "Số lượng bổ sung";
                            gridColMateSLHoanBu.Caption = "Số lượng bổ sung";

                            var dataMestRooms = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == _MediStock.ROOM_ID && p.IS_ACTIVE == 1).ToList();
                            var listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
                            if (dataMestRooms != null && dataMestRooms.Count > 0)
                            {
                                var roomIds = dataMestRooms.Select(s => s.MEDI_STOCK_ID).ToList();
                                listCurrentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_BLOOD != 1 && roomIds.Contains(o.ID) && o.IS_ACTIVE == 1 && o.IS_CABINET == 1).OrderBy(p => p.MEDI_STOCK_CODE).ToList();
                            }


                            if (listCurrentMediStock != null && listCurrentMediStock.Count > 0)
                            {
                                if (this._MediStock.IS_BUSINESS == 1)
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS == 1).ToList();
                                else
                                    listCurrentMediStock = listCurrentMediStock.Where(p => p.IS_BUSINESS != 1).ToList();
                                cboKhoNhap.EditValue = listCurrentMediStock[0].ID;
                            }
                            cboKhoNhap.Properties.DataSource = listCurrentMediStock;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        var selectedOpionGroup = GetSelectedOpionGroup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }

                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal int GetSelectedOpionGroup()
        {
            int selectedOpionGroup = 1;
            try
            {
                //int iSelectedIndex = this.rdOpionGroup.SelectedIndex;
                //iSelectedIndex = iSelectedIndex == -1 ? 0 : iSelectedIndex;
                //selectedOpionGroup = (int)this.rdOpionGroup.Properties.Items[iSelectedIndex].Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return selectedOpionGroup;
        }

        private void MetyMatyTypeInStock_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);
                    txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    spinAmount.Focus();
                    spinAmount.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is MediMatyTypeADO)
                    {
                        MediMatyTypeADO data = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //DMediStock1ADO dMediStock = gridViewMediMaty.GetRow(e.RowHandle) as DMediStock1ADO;
                //if (dMediStock != null && (dMediStock.IS_STAR_MARK ?? 0) == 1)
                //{
                //    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                if (medicineTypeADOForEdit != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    var selectedOpionGroup = GetSelectedOpionGroup();
                    if (selectedOpionGroup == 1)
                    {
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load lại dữ liệu của tab page đang chọn
        /// </summary>
        private void LoadDataToGridMetyMatyTypeInStock()
        {
            try
            {
                this.RebuildMediMatyWithInControlContainer();

                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetFocusMediMaty(bool isFocus, bool fucusOnly = false)
        {
            try
            {
                if (!fucusOnly)
                {
                    currentMedicineTypeADOForEdit = null;
                    txtMediMatyForPrescription.Text = "";
                }

                if (isFocus)
                    txtMediMatyForPrescription.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildMediMatyWithInControlContainer()
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();//Get data


                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                //popupControlContainerMediMaty.Width = theRequiredWidth;
                //popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = "Mã";
                col1.Width = 100;
                col1.VisibleIndex = 0;
                col1.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_NAME";
                col2.Caption = "Tên";
                col2.Width = 250;
                col2.VisibleIndex = 1;
                col2.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col2);


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME";
                col3.Caption = "ĐVT";
                col3.Width = 100;
                col3.VisibleIndex = 2;
                col3.OptionsColumn.AllowEdit = false;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = "Hàm lượng";
                col8.Width = 100;
                col8.VisibleIndex = 3;
                col8.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col8);


                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = "Tên BHYT";
                col7.Width = 160;
                col7.VisibleIndex = 4;
                col7.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col7);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT";
                col4.Caption = "Khả dụng";
                col4.Width = 90;
                col4.VisibleIndex = 5;
                col4.OptionsColumn.AllowEdit = false;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col4);


                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MANUFACTURER_NAME";
                col10.Caption = "Hãng SX";
                col10.Width = 150;
                col10.VisibleIndex = 12;
                col10.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "NATIONAL_NAME";
                col11.Caption = "Quốc gia";
                col11.Width = 80;
                col11.VisibleIndex = 13;
                col11.OptionsColumn.AllowEdit = false;
                gridViewMediMaty.Columns.Add(col11);

                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col14);

                gridViewMediMaty.GridControl.DataSource = this._MediMatyTypeADOs;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataMetyMatyTypeInStockD()
        {
            try
            {
                if (cboTuTrucXuat.EditValue != null)
                {
                    //lay du lieu theo kho xuat
                    V_HIS_MEDI_STOCK medi = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == (long)cboTuTrucXuat.EditValue);
                    LoadDataToCboMediMate(medi);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboMediMate(V_HIS_MEDI_STOCK mestRoom)
        {
            try
            {
                _MediMatyTypeADOs = new List<MediMatyTypeADO>();


                List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();

                if (mestRoom != null)
                {
                    var mestMetyDepaList = LoadMestMetyDepa();
                    var mestMatyDepaList = LoadMestMatyDepa();

                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = mestRoom.ID;
                    mediFilter.IS_AVAILABLE = true;
                    mediFilter.IS_ACTIVE = true;
                    mediFilter.IS_LEAF = true;
                    listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        if (mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                        {
                            listMediTypeInStock = listMediTypeInStock.Where(o => !mestMetyDepaList.Select(p => p.MEDICINE_TYPE_ID).Contains(o.Id)).ToList();
                        }
                        foreach (var item in listMediTypeInStock)
                        {
                            var dataCheck = _MedicineTypeInStockSDOs.FirstOrDefault(p => p.Id == item.Id && p.ServiceId == item.ServiceId);
                            if (dataCheck != null) continue;

                            MediMatyTypeADO ado = new MediMatyTypeADO(item);

                            _MediMatyTypeADOs.Add(ado);
                        }
                    }

                    HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = mestRoom.ID;
                    mateFilter.IS_AVAILABLE = true;
                    mateFilter.IS_ACTIVE = true;
                    mateFilter.IS_LEAF = true;
                    listMateTypeInStock = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    if (listMateTypeInStock != null && listMateTypeInStock.Count > 0)
                    {
                        if (mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                        {
                            listMateTypeInStock = listMateTypeInStock.Where(o => !mestMatyDepaList.Select(p => p.MATERIAL_TYPE_ID).Contains(o.Id)).ToList();
                        }
                        foreach (var item in listMateTypeInStock)
                        {
                            var dataCheck = _MaterialTypeInStockSDOs.FirstOrDefault(p => p.Id == item.Id && p.ServiceId == item.ServiceId);
                            if (dataCheck != null) continue;

                            MediMatyTypeADO ado = new MediMatyTypeADO(item);

                            _MediMatyTypeADOs.Add(ado);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom - 265));

                        if (this.currentMedicineTypeADOForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMatyTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && listDatas[i].MEDI_STOCK_ID == this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var selectedOpionGroup = GetSelectedOpionGroup();
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                    }
                    //}
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom - 265));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMediMatyForPrescription.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    txtMediMatyForPrescription.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%'", txtMediMatyForPrescription.Text);
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom - 265));
                            isShow = false;
                        }

                        txtMediMatyForPrescription.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                //this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBoSungCoSo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkBoSungCoSo.Checked)
                {
                    chkHoanCoSo.Checked = false;
                }
                SetDataCbo();
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHoanCoSo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHoanCoSo.Checked)
                {
                    chkBoSungCoSo.Checked = false;
                }
                SetDataCbo();
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTuTrucXuat_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    LoadDataGrid();

                    LoadDataToGridMetyMatyTypeInStock();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboKhoNhap_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    LoadDataGrid();
                    LoadDataToGridMetyMatyTypeInStock();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkHoanCoSo.Checked)
                    return;
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    //cboKhoNhap.ReadOnly = true;
                    //cboTuTrucXuat.ReadOnly = true;

                    if (this.currentMedicineTypeADOForEdit.IsMedicine)
                    {
                        var checkMedicineADOs = this._MedicineADOs.Where(o => o.Id == this.currentMedicineTypeADOForEdit.ID).ToList();
                        if (checkMedicineADOs != null && checkMedicineADOs.Count > 0)
                        {
                            if (MessageBox.Show("Thuốc đã được bổ sung. Ban có muốn thay đổi?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                this._MedicineADOs.FirstOrDefault(o => o.Id == this.currentMedicineTypeADOForEdit.ID).AMOUNT = spinAmount.Value;

                                gridControlMedicine.BeginUpdate();
                                gridControlMedicine.DataSource = this._MedicineADOs;
                                gridControlMedicine.EndUpdate();
                            }
                        }
                        else
                        {
                            MedicineADO ado = new MedicineADO();
                            ado.ServiceId = this.currentMedicineTypeADOForEdit.SERVICE_ID;
                            ado.Id = this.currentMedicineTypeADOForEdit.ID;
                            ado.MedicineTypeName = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                            ado.MedicineTypeCode = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
                            ado.AvailableAmount = this.currentMedicineTypeADOForEdit.AMOUNT;
                            ado.Concentra = this.currentMedicineTypeADOForEdit.CONCENTRA;
                            ado.ServiceUnitName = this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
                            ado.ManufacturerName = this.currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
                            ado.RegisterNumber = this.currentMedicineTypeADOForEdit.REGISTER_NUMBER;
                            ado.NationalName = this.currentMedicineTypeADOForEdit.NATIONAL_NAME;
                            ado.ActiveIngrBhytCode = this.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_CODE;
                            ado.ActiveIngrBhytName = this.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME;
                            ado.AMOUNT = spinAmount.Value;
                            ado.IsBoSung = true;

                            if (this._MedicineADOs == null)
                                this._MedicineADOs = new List<MedicineADO>();

                            this._MedicineADOs.Add(ado);

                            gridControlMedicine.BeginUpdate();
                            gridControlMedicine.DataSource = this._MedicineADOs;
                            gridControlMedicine.EndUpdate();
                        }
                        xtraTabControl1.SelectedTabPage = xtraTabPageMedicine;
                    }
                    else
                    {
                        var checkMaterialADOs = this._MaterialADOs.Where(o => o.Id == this.currentMedicineTypeADOForEdit.ID).ToList();
                        if (checkMaterialADOs != null && checkMaterialADOs.Count > 0)
                        {
                            if (MessageBox.Show("Vật tư đã được bổ sung. Ban có muốn thay đổi?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                this._MaterialADOs.FirstOrDefault(o => o.Id == this.currentMedicineTypeADOForEdit.ID).AMOUNT = spinAmount.Value;

                                gridControlMaterial.BeginUpdate();
                                gridControlMaterial.DataSource = this._MaterialADOs;
                                gridControlMaterial.EndUpdate();
                            }
                        }
                        else
                        {
                            MaterialADO ado = new MaterialADO();
                            ado.ServiceId = this.currentMedicineTypeADOForEdit.SERVICE_ID;
                            ado.Id = this.currentMedicineTypeADOForEdit.ID;
                            ado.MaterialTypeName = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                            ado.MaterialTypeCode = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
                            ado.AvailableAmount = this.currentMedicineTypeADOForEdit.AMOUNT;
                            ado.Concentra = this.currentMedicineTypeADOForEdit.CONCENTRA;
                            ado.ServiceUnitName = this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
                            ado.ManufacturerName = this.currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
                            ado.NationalName = this.currentMedicineTypeADOForEdit.NATIONAL_NAME;
                            ado.AMOUNT = spinAmount.Value;
                            ado.IsBoSung = true;
                            if (this._MaterialADOs == null)
                                this._MaterialADOs = new List<MaterialADO>();
                            this._MaterialADOs.Add(ado);

                            gridControlMaterial.BeginUpdate();
                            gridControlMaterial.DataSource = this._MaterialADOs;
                            gridControlMaterial.EndUpdate();
                        }
                        xtraTabControl1.SelectedTabPage = xtraTabPageMaterial;
                    }
                }
                txtMediMatyForPrescription.Text = "";
                this.currentMedicineTypeADOForEdit = null;
                spinAmount.EditValue = null;
                txtMediMatyForPrescription.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateData()
        {
            try
            {
                //btnSave.Focus();

                gridViewMedicine.PostEditor();
                gridViewMaterial.PostEditor();

                //if (!btnSave.Enabled)
                //    return;

                List<ExpMedicineTypeSDO> _mediSdos = new List<ExpMedicineTypeSDO>();
                if (this._MedicineADOs != null && this._MedicineADOs.Count > 0)
                {
                    var datas = this._MedicineADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMedicineTypeSDO ado = new ExpMedicineTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại thuốc có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }


                        ado.MedicineTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mediSdos.Add(ado);
                    }
                }

                List<ExpMaterialTypeSDO> _mateSdos = new List<ExpMaterialTypeSDO>();
                if (this._MaterialADOs != null && this._MaterialADOs.Count > 0)
                {
                    var datas = this._MaterialADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMaterialTypeSDO ado = new ExpMaterialTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại vật tư có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }

                        ado.MaterialTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mateSdos.Add(ado);
                    }
                }

                bool success = false;
                CommonParam param = new CommonParam();
                _ExpMestResultSDO = null;

                if ((_mateSdos != null && _mateSdos.Count > 0) || (_mediSdos != null && _mediSdos.Count > 0))
                {
                    if (this._currentExpMest != null && this._currentExpMest.ID > 0)
                    {
                        if (chkHoanCoSo.Checked)
                        {
                            CabinetBaseReductionSDO sdo2 = new CabinetBaseReductionSDO();
                            sdo2.Description = txtDescription.Text;
                            //sdo2.ImpMestMediStockId = (long)cboKhoNhap.EditValue;
                            //sdo2.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo2.WorkingRoomId = this.moduleData.RoomId;
                            sdo2.MaterialTypes = _mateSdos;
                            sdo2.MedicineTypes = _mediSdos;
                            sdo2.Id = this._currentExpMest.ID;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseReductionUpdate", ApiConsumers.MosConsumer, sdo2, param);
                        }
                        else
                        {
                            CabinetBaseAdditionSDO sdo = new CabinetBaseAdditionSDO();
                            sdo.Description = txtDescription.Text;
                            //sdo.ExpMestMediStockId = (long)cboTuTrucXuat.EditValue;
                            //sdo.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo.WorkingRoomId = this.moduleData.RoomId;
                            sdo.MaterialTypes = _mateSdos;
                            sdo.MedicineTypes = _mediSdos;
                            sdo.Id = this._currentExpMest.ID;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseAdditionUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                    }
                    else
                        if (chkHoanCoSo.Checked)
                        {
                            CabinetBaseReductionSDO sdo2 = new CabinetBaseReductionSDO();
                            sdo2.Description = txtDescription.Text;
                            sdo2.ImpMestMediStockId = (long)cboKhoNhap.EditValue;
                            sdo2.WorkingRoomId = this.moduleData.RoomId;
                            sdo2.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo2.MaterialTypes = _mateSdos;
                            sdo2.MedicineTypes = _mediSdos;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseReductionCreate", ApiConsumers.MosConsumer, sdo2, param);
                            btnSave.Enabled = _ExpMestResultSDO != null ? false : true;
                            btnEdit.Enabled = _ExpMestResultSDO != null ? true : false;
                        }
                        else
                        {
                            CabinetBaseAdditionSDO sdo = new CabinetBaseAdditionSDO();
                            sdo.Description = txtDescription.Text;
                            sdo.ExpMestMediStockId = (long)cboTuTrucXuat.EditValue;
                            sdo.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo.WorkingRoomId = this.moduleData.RoomId;
                            sdo.MaterialTypes = _mateSdos;
                            sdo.MedicineTypes = _mediSdos;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseAdditionCreate", ApiConsumers.MosConsumer, sdo, param);

                            btnSave.Enabled = _ExpMestResultSDO != null ? false : true;
                            btnEdit.Enabled = _ExpMestResultSDO != null ? true : false;
                        }

                    if (_ExpMestResultSDO != null)
                    {
                        this._currentExpMest = new V_HIS_EXP_MEST_4();
                        this._currentExpMest.ID = _ExpMestResultSDO.ExpMest.ID;
                        this._currentExpMest.CHMS_TYPE_ID = _ExpMestResultSDO.ExpMest.CHMS_TYPE_ID;
                        this._currentExpMest.MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.MEDI_STOCK_ID;
                        this._currentExpMest.IMP_MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.IMP_MEDI_STOCK_ID;

                        success = true;
                        btnPrint.Enabled = true;
                        ReloadBefSave();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
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
                btnSave.Focus();
                if (!btnSave.Enabled)
                {
                    return;
                }

                gridViewMedicine.PostEditor();
                gridViewMaterial.PostEditor();

                List<ExpMedicineTypeSDO> _mediSdos = new List<ExpMedicineTypeSDO>();
                if (this._MedicineADOs != null && this._MedicineADOs.Count > 0)
                {
                    var datas = this._MedicineADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMedicineTypeSDO ado = new ExpMedicineTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại thuốc có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }


                        ado.MedicineTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mediSdos.Add(ado);
                    }
                }

                List<ExpMaterialTypeSDO> _mateSdos = new List<ExpMaterialTypeSDO>();
                if (this._MaterialADOs != null && this._MaterialADOs.Count > 0)
                {
                    var datas = this._MaterialADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMaterialTypeSDO ado = new ExpMaterialTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại vật tư có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }

                        ado.MaterialTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mateSdos.Add(ado);
                    }
                }

                bool success = false;
                CommonParam param = new CommonParam();
                _ExpMestResultSDO = null;

                if ((_mateSdos != null && _mateSdos.Count > 0) || (_mediSdos != null && _mediSdos.Count > 0))
                {
                    if (chkHoanCoSo.Checked)
                    {
                        CabinetBaseReductionSDO sdo2 = new CabinetBaseReductionSDO();
                        sdo2.Description = txtDescription.Text;
                        sdo2.ImpMestMediStockId = (long)cboKhoNhap.EditValue;
                        sdo2.WorkingRoomId = this.moduleData.RoomId;
                        sdo2.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                        sdo2.MaterialTypes = _mateSdos;
                        sdo2.MedicineTypes = _mediSdos;
                        _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseReductionCreate", ApiConsumers.MosConsumer, sdo2, param);
                    }
                    else
                    {
                        CabinetBaseAdditionSDO sdo = new CabinetBaseAdditionSDO();
                        sdo.Description = txtDescription.Text;
                        sdo.ExpMestMediStockId = (long)cboTuTrucXuat.EditValue;
                        sdo.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                        sdo.WorkingRoomId = this.moduleData.RoomId;
                        sdo.MaterialTypes = _mateSdos;
                        sdo.MedicineTypes = _mediSdos;
                        _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseAdditionCreate", ApiConsumers.MosConsumer, sdo, param);
                    }

                    if (_ExpMestResultSDO != null)
                    {
                        this._currentExpMest = new V_HIS_EXP_MEST_4();
                        this._currentExpMest.ID = _ExpMestResultSDO.ExpMest.ID;
                        this._currentExpMest.CHMS_TYPE_ID = _ExpMestResultSDO.ExpMest.CHMS_TYPE_ID;
                        this._currentExpMest.MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.MEDI_STOCK_ID;
                        this._currentExpMest.IMP_MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.IMP_MEDI_STOCK_ID;
                        btnEdit.Enabled = true;
                        btnSave.Enabled = false;
                        success = true;
                        btnPrint.Enabled = true;
                        ReloadBefSave();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                chkBoSungCoSo.ReadOnly = false;
                chkHoanCoSo.ReadOnly = false;
                cboKhoNhap.ReadOnly = false;
                cboTuTrucXuat.ReadOnly = false;

                btnPrint.Enabled = false;
                this._DataMatys = null;
                this._DataMetys = null;
                gridControlADO.DataSource = null;
                txtDescription.Text = "";
                spinAmount.EditValue = null;
                txtMediMatyForPrescription.Text = "";
                btnEdit.Enabled = false;
                btnSave.Enabled = true;

                this._currentExpMest = null;
                this.SetDataCbo();

                _MedicineADOs = new List<MedicineADO>();
                if (_MedicineTypeInStockSDOs != null && _MedicineTypeInStockSDOs.Count > 0)
                {
                    _MedicineADOs.AddRange((from r in _MedicineTypeInStockSDOs select new MedicineADO(r, this._DataMetys)).ToList());
                }

                _MaterialADOs = new List<MaterialADO>();

                if (_MaterialTypeInStockSDOs != null && _MaterialTypeInStockSDOs.Count > 0)
                {
                    _MaterialADOs.AddRange((from r in _MaterialTypeInStockSDOs select new MaterialADO(r, this._DataMatys)).ToList());
                }

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = this._MedicineADOs;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = this._MaterialADOs;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadBefSave()
        {
            try
            {
                gridControlADO.DataSource = null;

                if (this._ExpMestResultSDO != null && this._ExpMestResultSDO.ExpMest != null)
                {
                    CommonParam param = new CommonParam();

                    MOS.Filter.HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = this._ExpMestResultSDO.ExpMest.ID;

                    _DataMetys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);


                    MOS.Filter.HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = this._ExpMestResultSDO.ExpMest.ID;

                    _DataMatys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);

                    List<MediMatyTypeADO> datas = new List<MediMatyTypeADO>();

                    if (_DataMetys != null && _DataMetys.Count > 0)
                    {
                        foreach (var item in _DataMetys)
                        {
                            var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataType != null)
                            {
                                MediMatyTypeADO ado = new MediMatyTypeADO();
                                ado.IsMedicine = true;
                                ado.SERVICE_ID = dataType.SERVICE_ID;
                                ado.ID = dataType.ID;
                                ado.MEDICINE_TYPE_NAME = dataType.MEDICINE_TYPE_NAME;
                                ado.MEDICINE_TYPE_CODE = dataType.MEDICINE_TYPE_CODE;
                                ado.AMOUNT = item.AMOUNT;
                                ado.CONCENTRA = dataType.CONCENTRA;
                                ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                                ado.MANUFACTURER_NAME = dataType.MANUFACTURER_NAME;
                                ado.REGISTER_NUMBER = dataType.REGISTER_NUMBER;
                                ado.NATIONAL_NAME = dataType.NATIONAL_NAME;
                                ado.ACTIVE_INGR_BHYT_CODE = dataType.ACTIVE_INGR_BHYT_CODE;
                                ado.ACTIVE_INGR_BHYT_NAME = dataType.ACTIVE_INGR_BHYT_NAME;
                                datas.Add(ado);
                            }
                        }
                    }

                    if (_DataMatys != null && _DataMatys.Count > 0)
                    {
                        foreach (var item in _DataMatys)
                        {
                            var dataType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataType != null)
                            {
                                MediMatyTypeADO ado = new MediMatyTypeADO();
                                ado.IsMedicine = false;
                                ado.SERVICE_ID = dataType.SERVICE_ID;
                                ado.ID = dataType.ID;
                                ado.MEDICINE_TYPE_NAME = dataType.MATERIAL_TYPE_NAME;
                                ado.MEDICINE_TYPE_CODE = dataType.MATERIAL_TYPE_CODE;
                                ado.AMOUNT = item.AMOUNT;
                                ado.CONCENTRA = dataType.CONCENTRA;
                                ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                                ado.MANUFACTURER_NAME = dataType.MANUFACTURER_NAME;
                                ado.NATIONAL_NAME = dataType.NATIONAL_NAME;
                                datas.Add(ado);
                            }
                        }
                    }

                    gridControlADO.DataSource = datas;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPrint(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST chmsExpMest { get; set; }
        List<HIS_MEDICINE> _Medicines { get; set; }
        List<HIS_MATERIAL> _Materials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

        private void onClickPrint(object sender, EventArgs e)
        {
            try
            {
                if (
                    !((this._DataMetys != null && this._DataMetys.Count > 0)
                    || (this._DataMatys != null && this._DataMatys.Count > 0))
                    )
                    return;

                #region TT Chung
                WaitingManager.Show();
                chmsExpMest = new V_HIS_EXP_MEST();
                _Medicines = new List<HIS_MEDICINE>();
                _Materials = new List<HIS_MATERIAL>();
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

                long _expMestId = this._ExpMestResultSDO.ExpMest.ID;
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = _expMestId;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();

                long _EXP_MEST_STT_ID = chmsExpMest.EXP_MEST_STT_ID;
                

                if (_EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    || _EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _MedicineIds;
                        _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                    matyFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = _MaterialIds;
                        _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    }
                }

                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                foreach (var item in this._DataMetys)
                {
                    var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (dataType != null)
                    {
                        if (dataType.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMetyReq_HCs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        {
                            _ExpMestMetyReq_GNs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                        {
                            _ExpMestMetyReq_HTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                        {
                            _ExpMestMetyReq_TDs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                        {
                            _ExpMestMetyReq_PXs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                        {
                            _ExpMestMetyReq_COs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                        {
                            _ExpMestMetyReq_DTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                        {
                            _ExpMestMetyReq_KSs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                        {
                            _ExpMestMetyReq_LAOs.Add(item);
                        }
                        else
                        {
                            _ExpMestMetyReq_Ts.Add(item);
                        }
                    }
                }

                foreach (var item in this._DataMatys)
                {
                    var dataMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                WaitingManager.Hide();
                #endregion

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (chkHoanCoSo.Checked)
                {
                    store.RunPrintTemplate("Mps000346", delegatePrintTemplate);
                }
                else if (chkBoSungCoSo.Checked)
                {
                    store.RunPrintTemplate("Mps000347", delegatePrintTemplate);
                }
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
                        case "Mps000346":
                            Mps000346(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000347":
                            Mps000347(ref result, printTypeCode, fileName);
                            break;
                        default:
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

        private void Mps000346(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- ThuocDoc ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "THUỐC ĐỘC"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PhongXa ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_LAOs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ----- TC -----
                if (_ExpMestMetyReq_TC != null && _ExpMestMetyReq_TC.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TC,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "PHIẾU TRẢ CƠ SỐ THUỐC TIỀN CHẤT"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000347(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMedicines), _ExpMestMedicines));
                
                    //CommonParam param = new CommonParam();
                    //MOS.Filter.HisMedicineUseFormFilter mediFilter = new HisMedicineUseFormFilter();
                    //mediFilter.EXP_MEST_METY_REQ_IDs = _ExpMestMetyReq_Ts.Select(o => o.ID).ToList();
                    //_ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    var mediuseform = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>();
                
                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG",
       mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- ThuocDoc ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "THUỐC ĐỘC",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PhongXa ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH",
         mediuseform,
          Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_LAOs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO",
         mediuseform,
         Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"))
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.moduleData.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MEST_MATY_DEPA> LoadMestMatyDepa()
        {

            List<HIS_MEST_MATY_DEPA> mestMatyDepaList = null;
            List<HIS_MEST_MATY_DEPA> mestMatyDepaAll = null;
            try
            {
                if (!BackendDataWorker.IsExistsKey<HIS_MEST_MATY_DEPA>())
                {
                    MOS.Filter.HisMestMatyDepaFilter metyDepaFilter = new HisMestMatyDepaFilter();
                    metyDepaFilter.IS_ACTIVE = 1;
                    mestMatyDepaAll = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_MATY_DEPA>>("api/HisMestMatyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, metyDepaFilter, null);

                    if (mestMatyDepaAll != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_MATY_DEPA), mestMatyDepaAll, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    mestMatyDepaAll = BackendDataWorker.Get<HIS_MEST_MATY_DEPA>().Where(o => o.IS_ACTIVE == 1).ToList();
                }

                if (cboKhoNhap.EditValue != null)
                {
                    var impMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (long)cboKhoNhap.EditValue);

                    mestMatyDepaList = mestMatyDepaAll.Where(o => o.DEPARTMENT_ID == impMediStock.DEPARTMENT_ID && o.IS_JUST_PRESCRIPTION != 1).ToList();
                    if (cboTuTrucXuat.EditValue != null && mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                    {
                        mestMatyDepaList = mestMatyDepaList.Where(o => !o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTuTrucXuat.EditValue.ToString())).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                mestMatyDepaList = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mestMatyDepaList;
        }

        private List<HIS_MEST_METY_DEPA> LoadMestMetyDepa()
        {
            List<HIS_MEST_METY_DEPA> mestMatyDepaAll = null;
            List<HIS_MEST_METY_DEPA> mestMetyDepaList = null;
            try
            {
                if (!BackendDataWorker.IsExistsKey<HIS_MEST_METY_DEPA>())
                {
                    MOS.Filter.HisMestMetyDepaFilter metyDepaFilter = new HisMestMetyDepaFilter();
                    metyDepaFilter.IS_ACTIVE = 1;
                    mestMatyDepaAll = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_METY_DEPA>>("api/HisMestMetyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, metyDepaFilter, null);

                    if (mestMatyDepaAll != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA), mestMatyDepaAll, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    mestMatyDepaAll = BackendDataWorker.Get<HIS_MEST_METY_DEPA>().Where(o => o.IS_ACTIVE == 1).ToList();
                }

                if (cboKhoNhap.EditValue != null)
                {
                    var impMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == (long)cboKhoNhap.EditValue);

                    mestMetyDepaList = mestMatyDepaAll.Where(o => o.DEPARTMENT_ID == impMediStock.DEPARTMENT_ID && o.IS_JUST_PRESCRIPTION != 1).ToList();
                    if (cboTuTrucXuat.EditValue != null && mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                    {
                        mestMetyDepaList = mestMetyDepaList.Where(o => !o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTuTrucXuat.EditValue.ToString())).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                mestMetyDepaList = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mestMetyDepaList;
        }

        public void FocusF2()
        {
            try
            {
                txtMediMatyForPrescription.Focus();
                txtMediMatyForPrescription.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Them()
        {
            try
            {
                if (lciThem.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Sua()
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Luu()
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

        public void In()
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Moi()
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

        private void gridViewMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMedicine.FocusedRowHandle < 0 || gridViewMedicine.FocusedColumn.FieldName != "AMOUNT")
                    return;
                var data = (MedicineADO)((IList)((BaseView)sender).DataSource)[gridViewMedicine.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.AMOUNT <= 0)
                    {
                        valid = false;
                        if (chkHoanCoSo.Checked)
                            message = "Số lượng hoàn phải lớn hơn 0";
                        else
                            message = "Số lượng bổ sung phải lớn hơn 0";
                    }
                    if (chkHoanCoSo.Checked && data.AMOUNT > 0 && data.AMOUNT > data.BaseAmount)
                    {
                        valid = false;
                        message = "Số lượng hoàn không được lớn hơn số lượng thiết lập cơ số";
                        //if (chkHoanCoSo.Checked)
                        //    message = "Số lượng hoàn không được lớn hơn số lượng thiết lập cơ số";
                        //else
                        //    message = "Số lượng bổ sung không được lớn hơn số lượng thiết lập cơ số";
                    }

                    if (!valid)
                    {
                        //btnSave.Enabled = false;
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    }
                    else
                    {
                        gridViewMedicine.ClearColumnErrors();
                        //btnSave.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
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

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMaterial.FocusedRowHandle < 0 || gridViewMaterial.FocusedColumn.FieldName != "AMOUNT")
                    return;
                var data = (MaterialADO)((IList)((BaseView)sender).DataSource)[gridViewMaterial.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.AMOUNT <= 0)
                    {
                        valid = false;
                        if (chkHoanCoSo.Checked)
                            message = "Số lượng hoàn phải lớn hơn 0";
                        else
                            message = "Số lượng bổ sung phải lớn hơn 0";
                    }
                    if (chkHoanCoSo.Checked && data.AMOUNT > 0 && data.AMOUNT > data.BaseAmount)
                    {
                        valid = false;
                        message = "Số lượng hoàn không được lớn hơn số lượng thiết lập cơ số";
                        //if (chkHoanCoSo.Checked)
                        //    message = "Số lượng hoàn không được lớn hơn số lượng thiết lập cơ số";
                        //else
                        //    message = "Số lượng bổ sung không được lớn hơn số lượng thiết lập cơ số";
                    }

                    if (!valid)
                    {
                        //btnSave.Enabled = false;
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    }
                    else
                    {
                        gridViewMaterial.ClearColumnErrors();
                        //btnSave.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
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

        private void spinAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                MedicineADO dMediStock = gridViewMedicine.GetRow(e.RowHandle) as MedicineADO;
                if (dMediStock != null && dMediStock.EXP_MEST_ID != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                MaterialADO dMediStock = gridViewMaterial.GetRow(e.RowHandle) as MaterialADO;
                if (dMediStock != null && dMediStock.EXP_MEST_ID != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewADO_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                MediMatyTypeADO dMediStock = gridViewADO.GetRow(e.RowHandle) as MediMatyTypeADO;
                if (dMediStock != null && dMediStock.IsMedicine)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Focus();
                if (!btnEdit.Enabled)
                    return;

                gridViewMedicine.PostEditor();
                gridViewMaterial.PostEditor();

                List<ExpMedicineTypeSDO> _mediSdos = new List<ExpMedicineTypeSDO>();
                if (this._MedicineADOs != null && this._MedicineADOs.Count > 0)
                {
                    var datas = this._MedicineADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMedicineTypeSDO ado = new ExpMedicineTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại thuốc có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }


                        ado.MedicineTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mediSdos.Add(ado);
                    }
                }

                List<ExpMaterialTypeSDO> _mateSdos = new List<ExpMaterialTypeSDO>();
                if (this._MaterialADOs != null && this._MaterialADOs.Count > 0)
                {
                    var datas = this._MaterialADOs.Where(p => p.AMOUNT > 0).ToList();

                    foreach (var item in datas)
                    {
                        ExpMaterialTypeSDO ado = new ExpMaterialTypeSDO();
                        if (chkHoanCoSo.Checked && item.AMOUNT > item.BaseAmount)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại vật tư có số lượng hoàn lớn hơn số lượng thiết lập cơ số", "Thông báo");
                            return;
                        }

                        ado.MaterialTypeId = item.Id;
                        ado.Amount = item.AMOUNT ?? 0;
                        _mateSdos.Add(ado);
                    }
                }

                bool success = false;
                CommonParam param = new CommonParam();
                _ExpMestResultSDO = null;

                if ((_mateSdos != null && _mateSdos.Count > 0) || (_mediSdos != null && _mediSdos.Count > 0))
                {
                    if (this._currentExpMest != null && this._currentExpMest.ID > 0)
                    {
                        if (chkHoanCoSo.Checked)
                        {
                            CabinetBaseReductionSDO sdo2 = new CabinetBaseReductionSDO();
                            sdo2.Description = txtDescription.Text;
                            //sdo2.ImpMestMediStockId = (long)cboKhoNhap.EditValue;
                            //sdo2.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo2.WorkingRoomId = this.moduleData.RoomId;
                            sdo2.MaterialTypes = _mateSdos;
                            sdo2.MedicineTypes = _mediSdos;
                            sdo2.Id = this._currentExpMest.ID;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseReductionUpdate", ApiConsumers.MosConsumer, sdo2, param);
                        }
                        else
                        {
                            CabinetBaseAdditionSDO sdo = new CabinetBaseAdditionSDO();
                            sdo.Description = txtDescription.Text;
                            //sdo.ExpMestMediStockId = (long)cboTuTrucXuat.EditValue;
                            //sdo.CabinetMediStockId = this._CABINET_MEDI_STOCK_ID;
                            sdo.WorkingRoomId = this.moduleData.RoomId;
                            sdo.MaterialTypes = _mateSdos;
                            sdo.MedicineTypes = _mediSdos;
                            sdo.Id = this._currentExpMest.ID;
                            _ExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/BaseAdditionUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                    }

                    if (_ExpMestResultSDO != null)
                    {
                        this._currentExpMest = new V_HIS_EXP_MEST_4();
                        this._currentExpMest.ID = _ExpMestResultSDO.ExpMest.ID;
                        this._currentExpMest.CHMS_TYPE_ID = _ExpMestResultSDO.ExpMest.CHMS_TYPE_ID;
                        this._currentExpMest.MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.MEDI_STOCK_ID;
                        this._currentExpMest.IMP_MEDI_STOCK_ID = _ExpMestResultSDO.ExpMest.IMP_MEDI_STOCK_ID;

                        success = true;
                        btnPrint.Enabled = true;
                        ReloadBefSave();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditDelete_Enabed_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                gridViewMedicine.DeleteRow(gridViewMedicine.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var mediRecord = (MedicineADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "Delete" && mediRecord != null)
                    {
                        e.RepositoryItem = mediRecord.IsBoSung ? ButtonEditDeleteMedicine_Enabed : ButtonEditDeleteMedicine_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditDeleteMaterial_Enabled_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                gridViewMaterial.DeleteRow(gridViewMaterial.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var mediRecord = (MaterialADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "Delete" && mediRecord != null)
                    {
                        e.RepositoryItem = mediRecord.IsBoSung ? ButtonEditDeleteMaterial_Enabled : ButtonEditDeleteMaterial_Disable;
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
