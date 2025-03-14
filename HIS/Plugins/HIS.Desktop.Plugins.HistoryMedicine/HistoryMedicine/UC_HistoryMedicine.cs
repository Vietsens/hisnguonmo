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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HistoryMedicine.HistoryMedicine
{
    public partial class UC_HistoryMedicine : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        long medicineTypeId;
        //List<MedicineTypeADO> listMedicineTypeAdos;
        List<MedicineTypeADO> listMetyAdos;
        List<long> listMediStockIds;

        List<V_HIS_MEDI_STOCK> medistocks;
        List<V_HIS_EXP_MEST> expMest;//Các phiếu xuất
        List<V_HIS_IMP_MEST> impMest;//Các phiếu nhập
        List<HIS_EXP_MEST> expMestByImp;//Các phiếu xuất theo phiếu nhập

        List<V_HIS_IMP_MEST_MEDICINE> ListImpMestMedicine;
        List<V_HIS_EXP_MEST_MEDICINE> ListExpMestmedicine;

        List<HIS_DEPARTMENT> glstDepartment = new List<HIS_DEPARTMENT>();
        List<V_HIS_MEDI_STOCK> glstMediStock = new List<V_HIS_MEDI_STOCK>();
        List<HIS_EXP_MEST_STT> glstExpMestStt = new List<HIS_EXP_MEST_STT>();
        List<HIS_IMP_MEST_STT> glstImpMestStt = new List<HIS_IMP_MEST_STT>();

        List<MedicineADO> listMedicineADO;

        private int MAX_REQUEST_LENGTH_PARAM = 500;

        Dictionary<long, string> dicChmsImpMest = new Dictionary<long, string>();

        Inventec.Desktop.Common.Modules.Module _Module;
        #endregion

        public UC_HistoryMedicine(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                gridviewFormList.OptionsView.ColumnAutoWidth = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UC_HistoryMedicine(Inventec.Desktop.Common.Modules.Module _module, long medicineTypeId)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.medicineTypeId = medicineTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UC_HistoryMedicine(Inventec.Desktop.Common.Modules.Module _module, long medicineTypeId, string _pakage_number)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.medicineTypeId = medicineTypeId;
                this.txtPakageNumber.Text = _pakage_number;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UC_HistoryMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                InitCombo(cboMedicineType);
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
                dtCreateTo.EditValue = DateTime.Now;
                dtCreateFrom.EditValue = DateTime.Now.AddMonths(-1);

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                var _branchId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this._Module.RoomId).BranchId;
                var rooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.BRANCH_ID == _branchId).ToList();
                List<long> _RoomIds = (rooms != null && rooms.Count > 0) ? rooms.Select(p => p.ID).ToList() : null;
                List<long> _DepartmentIds = (rooms != null && rooms.Count > 0) ? rooms.Select(p => p.DEPARTMENT_ID).ToList() : null;

                glstDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                glstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                glstExpMestStt = BackendDataWorker.Get<HIS_EXP_MEST_STT>();
                glstImpMestStt = BackendDataWorker.Get<HIS_IMP_MEST_STT>();
                this.medistocks = glstMediStock.Where(p => p.IS_ACTIVE == 1 && _RoomIds.Contains(p.ROOM_ID)).ToList();

                //Load Combo
                InitCheck(cboSTT, SelectionGrid__Status);
                InitCombo(cboSTT, glstExpMestStt.Where(p => p.IS_ACTIVE == 1).ToList(), "EXP_MEST_STT_NAME", "ID");

                InitCheck(cboDepartment, SelectionGrid__Department);
                InitCombo(cboDepartment, glstDepartment.Where(p => p.IS_ACTIVE == 1 && _DepartmentIds.Contains(p.ID)).ToList(), "DEPARTMENT_NAME", "ID");

                InitCheck(cboImpMediStock, SelectionGrid__ImpMediStock);
                InitCombo(cboImpMediStock, medistocks.Where(p => p.IS_ACTIVE == 1 && _RoomIds.Contains(p.ROOM_ID)).ToList(), "MEDI_STOCK_NAME", "ID");

                InitCheck(cboExpMediStock, SelectionGrid__ExpMediStock);
                InitCombo(cboExpMediStock, medistocks.Where(p => p.IS_ACTIVE == 1 && _RoomIds.Contains(p.ROOM_ID)).ToList(), "MEDI_STOCK_NAME", "ID");

                InitCheck(cboImpMestType, SelectionGrid__ImpMestType);
                InitCombo(cboImpMestType, BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList(), "IMP_MEST_TYPE_NAME", "ID");

                InitCheck(cboExpMestType, SelectionGrid__ExpMestType);
                InitCombo(cboExpMestType, BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList(), "EXP_MEST_TYPE_NAME", "ID");

                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitCombo(CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                listMedicineADO = new List<MedicineADO>();
                foreach (var item in data)
                {
                    MedicineADO ado = new MedicineADO(item);
                    listMedicineADO.Add(ado);
                }

                cbo.Properties.DataSource = listMedicineADO;
                cbo.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["MEDICINE_TYPE_NAME_UNSIGN"].Width = 0;

                if (this.medicineTypeId > 0)
                {
                    var mater = listMedicineADO.FirstOrDefault(o => o.ID == this.medicineTypeId);
                    cboMedicineType.EditValue = mater.ID;
                    txtMedicineTypeCode.Text = mater.MEDICINE_TYPE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultLoadForm()
        {
            try
            {
                cboSTT.Enabled = false;
                cboDepartment.Enabled = false;
                cboImpMediStock.Enabled = false;
                cboExpMediStock.Enabled = false;
                cboImpMestType.Enabled = false;
                cboExpMestType.Enabled = false;
                cboSTT.Enabled = true;
                cboDepartment.Enabled = true;
                cboImpMediStock.Enabled = true;
                cboExpMediStock.Enabled = true;
                cboImpMestType.Enabled = true;
                cboExpMestType.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HistoryMedicine.Resources.Lang", typeof(HIS.Desktop.Plugins.HistoryMedicine.HistoryMedicine.UC_HistoryMedicine).Assembly);

                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHistoryMedicine.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Begin FillDataToGridControl");
                LoadDefaultLoadForm();

                listMetyAdos = new List<MedicineTypeADO>();
                var listMedicineTypeAdos = new List<MedicineTypeADO>();
                CommonParam param = new CommonParam();

                impMest = new List<V_HIS_IMP_MEST>();
                expMest = new List<V_HIS_EXP_MEST>();
                expMestByImp = new List<HIS_EXP_MEST>();
                ListImpMestMedicine = new List<V_HIS_IMP_MEST_MEDICINE>();
                ListExpMestmedicine = new List<V_HIS_EXP_MEST_MEDICINE>();

                Inventec.Common.Logging.LogSystem.Info("Begin CreatThreadLoadDataMestMedcine");
                CreatThreadLoadDataMestMedcine();
                Inventec.Common.Logging.LogSystem.Info("End CreatThreadLoadDataMestMedcine");

                List<HIS_MEDI_STOCK_PERIOD> _MediStockPeriods = new List<HIS_MEDI_STOCK_PERIOD>();
                List<long> MEDI_STOCK_PERIOD_IDs = new List<long>();

                if (ListImpMestMedicine != null && ListImpMestMedicine.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs.AddRange(ListImpMestMedicine.Select(p => p.MEDI_STOCK_PERIOD_ID ?? 0).ToList());
                }

                if (ListExpMestmedicine != null && ListExpMestmedicine.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs.AddRange(ListExpMestmedicine.Select(p => p.MEDI_STOCK_PERIOD_ID ?? 0).ToList());
                }

                if (MEDI_STOCK_PERIOD_IDs != null && MEDI_STOCK_PERIOD_IDs.Count > 0)
                {
                    MEDI_STOCK_PERIOD_IDs = MEDI_STOCK_PERIOD_IDs.Distinct().ToList();

                    var skip = 0;
                    while (MEDI_STOCK_PERIOD_IDs.Count - skip > 0)
                    {
                        var listIDs = MEDI_STOCK_PERIOD_IDs.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + MAX_REQUEST_LENGTH_PARAM;
                        MOS.Filter.HisMediStockPeriodFilter filterPeriod = new HisMediStockPeriodFilter();
                        filterPeriod.IDs = listIDs;
                        var Periods = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_PERIOD>>("api/HisMediStockPeriod/Get", ApiConsumers.MosConsumer, filterPeriod, null);
                        if (Periods != null && Periods.Count > 0)
                        {
                            _MediStockPeriods.AddRange(Periods);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("End _MediStockPeriods");

                if (ListImpMestMedicine != null && ListImpMestMedicine.Count > 0)
                {
                    var glstHisImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => ListImpMestMedicine.Select(imp => imp.IMP_MEST_TYPE_ID).Contains(o.ID)).ToList();
                    foreach (var item in ListImpMestMedicine)
                    {
                        MedicineTypeADO ado = new MedicineTypeADO();
                        ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.TIME = item.IMP_TIME;
                        ado.CREATE_TIME = item.CREATE_TIME;
                        ado.DOCUMENT_NUMBER = item.DOCUMENT_NUMBER;
                        ado.MEST_ID = item.IMP_MEST_ID;
                        ado.MEST_CODE = item.IMP_MEST_CODE;
                        var _ImpType = glstHisImpMestType.FirstOrDefault(p => p.ID == item.IMP_MEST_TYPE_ID);
                        if (_ImpType != null)
                        {
                            if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)//loại nhập chuyển kho
                            {
                                var _ExpMest = impMest.FirstOrDefault(p => p.ID == item.IMP_MEST_ID);
                                if (_ExpMest != null)
                                {
                                    if (_ExpMest.CHMS_TYPE_ID == null)
                                        ado.MEST_TYPE = "Nhập chuyển kho";
                                    if (_ExpMest.CHMS_TYPE_ID == 1)
                                        ado.MEST_TYPE = "Bổ sung cơ số";
                                    if (_ExpMest.CHMS_TYPE_ID == 2)
                                        ado.MEST_TYPE = "Thu hồi cơ số";
                                }
                            }
                            else if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                            {
                                ado.MEST_TYPE = !string.IsNullOrEmpty(item.SUPPLIER_NAME) ? "Nhà cung cấp " + " - " + item.SUPPLIER_NAME : "Nhà cung cấp";
                            }
                            else
                                ado.MEST_TYPE = _ImpType != null ? _ImpType.IMP_MEST_TYPE_NAME : "";
                        }

                        ado.IMP_MEST_TYPE_ID = item.IMP_MEST_TYPE_ID;
                        ado.AMOUNT = item.AMOUNT;
                        Inventec.Common.Logging.LogSystem.Info("item.IMP_VAT_RATIO 1" + item.IMP_VAT_RATIO);
                        if (item.IMP_VAT_RATIO > 0)
                        {
                            ado.PRICE = item.PRICE + (item.IMP_VAT_RATIO * item.PRICE);
                            Inventec.Common.Logging.LogSystem.Info("ado.PRICE 1" + ado.PRICE);
                        }
                        else
                        {
                            ado.PRICE = item.PRICE;
                        }
                        ado.IsExp = false;
                        var _Period = _MediStockPeriods.FirstOrDefault(p => p.ID == item.MEDI_STOCK_PERIOD_ID);
                        ado.MEDI_STOCK_PERIOD_NAME = _Period != null ? _Period.MEDI_STOCK_PERIOD_NAME : "";
                        var _MediStock = glstMediStock.FirstOrDefault(p => p.ID == item.MEDI_STOCK_ID);
                        ado.MEDI_STOCK_NAME = _MediStock != null ? _MediStock.MEDI_STOCK_NAME : "";
                        ado.IMP_MEDI_STOCK_NAME = ado.MEDI_STOCK_NAME;
                        var _STT = glstImpMestStt.FirstOrDefault(p => p.ID == item.IMP_MEST_STT_ID);
                        ado.STT_NAME = _STT != null ? _STT.IMP_MEST_STT_NAME : "";
                        ado.STT_ID = _STT != null ? _STT.ID : 0;
                        var _Department = glstDepartment.FirstOrDefault(p => p.ID == item.REQ_DEPARTMENT_ID);
                        ado.REQ_DEPARTMENT_NAME = _Department != null ? _Department.DEPARTMENT_NAME : "";
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                        listMedicineTypeAdos.Add(ado);
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("End ListImpMestMedicine");

                if (ListExpMestmedicine != null && ListExpMestmedicine.Count > 0)
                {
                    var glstHisExpMestType = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().Where(o => ListExpMestmedicine.Select(imp => imp.EXP_MEST_TYPE_ID).Contains(o.ID)).ToList();

                    foreach (var item in ListExpMestmedicine)
                    {
                        MedicineTypeADO ado = new MedicineTypeADO();
                        ado.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.TIME = item.EXP_TIME;
                        ado.MEST_ID = item.EXP_MEST_ID ?? 0;
                        ado.MEST_CODE = item.EXP_MEST_CODE;
                        ado.CREATE_TIME = item.CREATE_TIME;
                        ado.TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME;
                        var _ExpType = glstHisExpMestType.FirstOrDefault(p => p.ID == item.EXP_MEST_TYPE_ID);
                        if (_ExpType != null)
                        {
                            if (item.EXP_MEST_ID != null && item.EXP_MEST_TYPE_ID == 3)//loại xuất chuyển kho
                            {
                                var _ExpMest = expMest.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                                if (_ExpMest != null)
                                {
                                    if (_ExpMest.CHMS_TYPE_ID == null)
                                        ado.MEST_TYPE = "Xuất chuyển kho";
                                    if (_ExpMest.CHMS_TYPE_ID == 1)
                                        ado.MEST_TYPE = "Bổ sung cơ số";
                                    if (_ExpMest.CHMS_TYPE_ID == 2)
                                        ado.MEST_TYPE = "Thu hồi cơ số";
                                }
                            }
                            else
                                ado.MEST_TYPE = _ExpType != null ? _ExpType.EXP_MEST_TYPE_NAME : "";
                        }

                        ado.EXP_MEST_TYPE_ID = item.EXP_MEST_TYPE_ID;
                        ado.AMOUNT = item.AMOUNT;
                        //Inventec.Common.Logging.LogSystem.Info("item.IMP_VAT_RATIO 2" + item.IMP_VAT_RATIO);
                        //if (item.IMP_VAT_RATIO > 0)
                        //{
                        //    ado.PRICE = item.PRICE + (item.IMP_VAT_RATIO * item.PRICE);
                        //    Inventec.Common.Logging.LogSystem.Info("ado.PRICE 2" + ado.PRICE);
                        //}
                        //else
                        //{
                        ado.PRICE = item.PRICE;
                        //}
                        ado.IsExp = true;
                        var _Period = _MediStockPeriods.FirstOrDefault(p => p.ID == item.MEDI_STOCK_PERIOD_ID);
                        ado.MEDI_STOCK_PERIOD_NAME = _Period != null ? _Period.MEDI_STOCK_PERIOD_NAME : "";
                        var _MediStock = glstMediStock.FirstOrDefault(p => p.ID == item.MEDI_STOCK_ID);
                        ado.MEDI_STOCK_NAME = _MediStock != null ? _MediStock.MEDI_STOCK_NAME : "";
                        ado.EXP_MEDI_STOCK_NAME = ado.MEDI_STOCK_NAME;
                        var _STT = glstExpMestStt.FirstOrDefault(p => p.ID == item.EXP_MEST_STT_ID);
                        ado.STT_NAME = _STT != null ? _STT.EXP_MEST_STT_NAME : "";
                        ado.STT_ID = _STT != null ? _STT.ID : 0;
                        var _Department = glstDepartment.FirstOrDefault(p => p.ID == item.REQ_DEPARTMENT_ID);
                        ado.REQ_DEPARTMENT_NAME = _Department != null ? _Department.DEPARTMENT_NAME : "";
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        listMedicineTypeAdos.Add(ado);
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("End ListExpMestmedicine");

                Inventec.Common.Logging.LogSystem.Debug("listMedicineTypeAdos___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMedicineTypeAdos), listMedicineTypeAdos));
                //Các phiếu xuất nhập được group theo mã và giá
                listMedicineTypeAdos = listMedicineTypeAdos.OrderByDescending(o => o.TIME).ToList();
                var listGroup = listMedicineTypeAdos.GroupBy(o => new { o.MEST_ID, o.MEST_CODE, o.PRICE, o.IsExp }).ToList();
                List<MedicineTypeADO> listAdo = new List<MedicineTypeADO>();
                foreach (var item in listGroup)
                {
                    MedicineTypeADO ado = new MedicineTypeADO();
                    ado.MEST_ID = item.First().MEST_ID;
                    ado.CREATE_TIME = item.First().CREATE_TIME;
                    ado.TDL_INTRUCTION_TIME = item.First().TDL_INTRUCTION_TIME;
                    ado.MEDICINE_TYPE_CODE = item.First().MEDICINE_TYPE_CODE;
                    ado.AMOUNT = item.Sum(s => s.AMOUNT);
                    ado.TIME = item.First().TIME;
                    ado.MEST_CODE = item.First().MEST_CODE;
                    ado.MEST_TYPE = item.First().MEST_TYPE;
                    ado.IMP_MEST_TYPE_ID = item.First().IMP_MEST_TYPE_ID;
                    ado.EXP_MEST_TYPE_ID = item.First().EXP_MEST_TYPE_ID;
                    ado.PRICE = item.First().PRICE;
                    ado.MEDI_STOCK_PERIOD_NAME = item.First().MEDI_STOCK_PERIOD_NAME;
                    ado.MEDI_STOCK_NAME = item.First().MEDI_STOCK_NAME;
                    ado.IsExp = item.First().IsExp;
                    ado.MEDI_STOCK_NAME = item.First().MEDI_STOCK_NAME;
                    ado.IMP_MEDI_STOCK_NAME = item.First().IMP_MEDI_STOCK_NAME;
                    ado.EXP_MEDI_STOCK_NAME = item.First().EXP_MEDI_STOCK_NAME;
                    ado.STT_NAME = item.First().STT_NAME;
                    ado.REQ_DEPARTMENT_NAME = item.First().REQ_DEPARTMENT_NAME;
                    ado.DOCUMENT_NUMBER = item.First().DOCUMENT_NUMBER;
                    ado.PACKAGE_NUMBER = item.First().PACKAGE_NUMBER;
                    ado.KEY_WORD = convertToUnSign3(item.First().EXP_MEDI_STOCK_NAME) + item.First().EXP_MEDI_STOCK_NAME
                                + convertToUnSign3(item.First().IMP_MEDI_STOCK_NAME) + item.First().IMP_MEDI_STOCK_NAME
                                + convertToUnSign3(item.First().MEDI_STOCK_NAME) + item.First().MEDI_STOCK_NAME
                                + convertToUnSign3(item.First().MEDI_STOCK_PERIOD_NAME) + item.First().MEDI_STOCK_PERIOD_NAME
                                + convertToUnSign3(item.First().REQ_DEPARTMENT_NAME) + item.First().REQ_DEPARTMENT_NAME
                                + convertToUnSign3(item.First().STT_NAME) + item.First().STT_NAME
                                + convertToUnSign3(item.First().MEDICINE_TYPE_CODE) + item.First().MEDICINE_TYPE_CODE
                                + convertToUnSign3(item.First().MEST_CODE) + item.First().MEST_CODE
                                + convertToUnSign3(item.First().MEST_TYPE) + item.First().MEST_TYPE;
                    listAdo.Add(ado);
                }

                Inventec.Common.Logging.LogSystem.Info("End listGroup");
                listMetyAdos = listAdo;
                if (listAdo != null && listAdo.Count > 0)
                {
                    listAdo = listAdo.OrderByDescending(p => p.TIME).ThenByDescending(p => p.MEST_CODE).ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug("listAdo__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAdo), listAdo));
                gridControlFormList.BeginUpdate();
                gridControlFormList.DataSource = listAdo;
                gridControlFormList.EndUpdate();
                Inventec.Common.Logging.LogSystem.Info("End FillDataToGridControl");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void CreatThreadLoadDataMestMedcine()
        {
            System.Threading.Thread impMest = new System.Threading.Thread(ProcessGetImpMest);
            System.Threading.Thread expMest = new System.Threading.Thread(ProcessGetExpMest);
            try
            {
                impMest.Start();
                expMest.Start();

                impMest.Join();
                expMest.Join();
            }
            catch (Exception ex)
            {
                impMest.Abort();
                expMest.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetExpMest()
        {
            try
            {
                if (cboMedicineType.EditValue != null && string.IsNullOrEmpty(txtDocumentNumber.Text))
                {
                    //Lấy danh sách các phiếu xuất của loại thuốc
                    HisExpMestMedicineViewFilter ExpFilter = new HisExpMestMedicineViewFilter();
                    ExpFilter.TDL_MEDICINE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString());
                    if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                    {
                        ExpFilter.EXP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                    {
                        ExpFilter.EXP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (dtCreateFrom.EditValue != null && dtCreateFrom.DateTime != DateTime.MinValue)
                    {
                        ExpFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtCreateTo.EditValue != null && dtCreateTo.DateTime != DateTime.MinValue)
                    {
                        ExpFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (!string.IsNullOrEmpty(txtPakageNumber.Text.Trim()))
                    {
                        ExpFilter.PACKAGE_NUMBER__EXACT = txtPakageNumber.Text.Trim();
                    }
                    if (this._StatusSelecteds != null && this._StatusSelecteds.Count > 0)
                    {
                        ExpFilter.EXP_MEST_STT_IDs = new List<long>();
                        ExpFilter.EXP_MEST_STT_IDs = this._StatusSelecteds.Select(p => p.ID).ToList();
                    }
                    if (this._DepartmentSelecteds != null && this._DepartmentSelecteds.Count > 0)
                    {
                        ExpFilter.REQ_DEPARTMENT_IDs = new List<long>();
                        ExpFilter.REQ_DEPARTMENT_IDs = this._DepartmentSelecteds.Select(p => p.ID).ToList();
                    }
                    if (this._ExpMediStockSelecteds != null && this._ExpMediStockSelecteds.Count > 0)
                    {
                        ExpFilter.MEDI_STOCK_IDs = new List<long>();
                        ExpFilter.MEDI_STOCK_IDs = this._ExpMediStockSelecteds.Select(p => p.ID).ToList();
                    }

                    if (this._ExpMestTypeSelecteds != null && this._ExpMestTypeSelecteds.Count > 0)
                    {
                        ExpFilter.EXP_MEST_TYPE_IDs = new List<long>();
                        ExpFilter.EXP_MEST_TYPE_IDs = this._ExpMestTypeSelecteds.Select(p => p.ID).ToList();
                    }
                    else
                    {
                        return;
                    }

                    var listExp = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, ExpFilter, null);
                    if (listExp != null && listExp.Count > 0)
                    {
                        ListExpMestmedicine.AddRange(listExp);
                        List<long> expMestIds = listExp.Select(s => s.EXP_MEST_ID ?? 0).Distinct().ToList();
                        var skip = 0;
                        while (expMestIds.Count - skip > 0)
                        {
                            var listIDs = expMestIds.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                            skip = skip + MAX_REQUEST_LENGTH_PARAM;
                            HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                            expMestFilter.IDs = listIDs;
                            var vExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, null);
                            if (vExpMest != null && vExpMest.Count > 0)
                            {
                                this.expMest.AddRange(vExpMest);
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

        private void ProcessGetImpMest()
        {
            try
            {
                if (cboMedicineType.EditValue != null)
                {
                    CommonParam param = new CommonParam();

                    //Lấy danh sách các phiếu nhập của loại thuốc
                    HisImpMestMedicineViewFilter ImpFilter = new HisImpMestMedicineViewFilter();
                    ImpFilter.MEDICINE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicineType.EditValue.ToString());

                    if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                    {
                        ImpFilter.IMP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                    {
                        ImpFilter.IMP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959");
                    }
                    if (dtCreateFrom.EditValue != null && dtCreateFrom.DateTime != DateTime.MinValue)
                    {
                        ImpFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtCreateFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (dtCreateTo.EditValue != null && dtCreateTo.DateTime != DateTime.MinValue)
                    {
                        ImpFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtCreateTo.DateTime.ToString("yyyyMMdd") + "235959");
                    }
                    if (this._StatusSelecteds != null && this._StatusSelecteds.Count > 0)
                    {
                        ImpFilter.IMP_MEST_STT_IDs = new List<long>();
                        ImpFilter.IMP_MEST_STT_IDs = this._StatusSelecteds.Select(p => p.ID).ToList();
                    }
                    if (this._DepartmentSelecteds != null && this._DepartmentSelecteds.Count > 0)
                    {
                        ImpFilter.REQ_DEPARTMENT_IDs = new List<long>();
                        ImpFilter.REQ_DEPARTMENT_IDs = this._DepartmentSelecteds.Select(p => p.ID).ToList();
                    }
                    if (this._ImpMediStockSelecteds != null && this._ImpMediStockSelecteds.Count > 0)
                    {
                        ImpFilter.MEDI_STOCK_IDs = new List<long>();
                        ImpFilter.MEDI_STOCK_IDs = this._ImpMediStockSelecteds.Select(p => p.ID).ToList();
                    }
                    if (this._ImpMestTypeSelecteds != null && this._ImpMestTypeSelecteds.Count > 0)
                    {
                        ImpFilter.IMP_MEST_TYPE_IDs = new List<long>();
                        ImpFilter.IMP_MEST_TYPE_IDs = this._ImpMestTypeSelecteds.Select(p => p.ID).ToList();
                    }
                    else
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(txtDocumentNumber.Text.Trim()))
                    {
                        ImpFilter.DOCUMENT_NUMBER = txtDocumentNumber.Text.Trim();
                    }
                    if (!string.IsNullOrEmpty(txtPakageNumber.Text.Trim()))
                    {
                        ImpFilter.PACKAGE_NUMBER__EXACT = txtPakageNumber.Text.Trim();
                    }

                    var listImp = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, ImpFilter, param);
                    if (listImp != null && listImp.Count > 0)
                    {
                        ListImpMestMedicine.AddRange(listImp);

                        List<long> impMestIds = listImp.Select(s => s.IMP_MEST_ID).Distinct().ToList();
                        var skip1 = 0;
                        while (impMestIds.Count - skip1 > 0)
                        {
                            var listIDs = impMestIds.Skip(skip1).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                            skip1 = skip1 + MAX_REQUEST_LENGTH_PARAM;

                            HisImpMestFilter impMestFilter = new HisImpMestFilter();
                            impMestFilter.IDs = listIDs;
                            if (!string.IsNullOrEmpty(txtDocumentNumber.Text)) impMestFilter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text;

                            var impMestapi = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, null);
                            if (impMestapi != null && impMestapi.Count > 0)
                            {
                                impMest.AddRange(impMestapi);
                                var chmsImpMest = impMestapi.Where(o => o.CHMS_EXP_MEST_ID.HasValue).ToList();
                                if (chmsImpMest != null && chmsImpMest.Count > 0)
                                {
                                    var chmsIds = chmsImpMest.Select(o => o.CHMS_EXP_MEST_ID ?? 0).Distinct().ToList();
                                    if (chmsIds != null && chmsIds.Count > 0)
                                    {
                                        var skip = 0;
                                        while (chmsIds.Count - skip > 0)
                                        {
                                            var IDs = chmsIds.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                                            skip = skip + MAX_REQUEST_LENGTH_PARAM;
                                            HisExpMestFilter expMestByImpFilter = new HisExpMestFilter();
                                            expMestByImpFilter.IDs = IDs;
                                            var expMestByImpapi = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestByImpFilter, null);

                                            if (expMestByImpapi != null && expMestByImpapi.Count > 0)
                                            {
                                                expMestByImp.AddRange(expMestByImpapi);
                                            }
                                        }
                                    }

                                    if (expMestByImp != null && expMestByImp.Count > 0)
                                    {
                                        foreach (var item in chmsImpMest)
                                        {
                                            var exp = expMestByImp.FirstOrDefault(o => o.ID == item.CHMS_EXP_MEST_ID);
                                            if (exp != null)
                                            {
                                                var stock = glstMediStock.FirstOrDefault(p => p.ID == exp.MEDI_STOCK_ID);
                                                dicChmsImpMest[item.ID] = stock != null ? stock.MEDI_STOCK_NAME : "";
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (impMest != null && impMest.Count > 0)
                        {
                            ListImpMestMedicine = ListImpMestMedicine.Where(o => impMest.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MedicineTypeADO pData = (MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (pData != null)
                    {
                        if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            if (pData.TDL_INTRUCTION_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.TDL_INTRUCTION_TIME ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            if (pData.CREATE_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                            }
                        }
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MEDI_STOCK_NAME_STR")
                        {
                            try
                            {
                                //Check từng dòng xem là loại xuất nhập nào để hiển thị thông tin 
                                //bool IsExp = Convert.ToBoolean((view.GetRowCellValue(e.ListSourceRowIndex, "IsExp") ?? "False").ToString());
                                if (pData.IsExp)
                                {
                                    if (expMest != null && expMest.Count > 0)
                                    {
                                        long MEST_ID = pData.MEST_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MEST_ID") ?? 0).ToString());
                                        long EXP_MEST_TYPE_ID = pData.EXP_MEST_TYPE_ID;// long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "EXP_MEST_TYPE_ID") ?? 0).ToString());
                                        if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                        {
                                            var mest = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            if (mest != null)
                                            {
                                                var stock = medistocks.FirstOrDefault(p => p.ID == mest.IMP_MEDI_STOCK_ID);
                                                e.Value = stock != null ? stock.MEDI_STOCK_NAME : "";
                                            }
                                            else
                                                e.Value = "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT || EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_TREATMENT_CODE + "-" + expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.REQ_DEPARTMENT_NAME + "-" + expMestEdit.REQ_ROOM_NAME : "";
                                        }
                                        else if (EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                                        {
                                            var expMestEdit = expMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = expMestEdit != null ? expMestEdit.TDL_PATIENT_NAME : "";
                                        }
                                    }
                                }
                                else
                                {
                                    if (impMest != null && impMest.Count > 0)
                                    {
                                        long MEST_ID = pData.MEST_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "MEST_ID") ?? 0).ToString());
                                        long IMP_MEST_TYPE_ID = pData.EXP_MEST_TYPE_ID; //long.Parse((view.GetRowCellValue(e.ListSourceRowIndex, "IMP_MEST_TYPE_ID") ?? 0).ToString());

                                        if (IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                                        {
                                            e.Value = dicChmsImpMest.ContainsKey(MEST_ID) ? dicChmsImpMest[MEST_ID] : "";
                                        }
                                        else if (IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL ||
                                            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL ||
                                            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL ||
                                            IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL)
                                        {
                                            var moba = impMest.FirstOrDefault(o => o.ID == MEST_ID);
                                            e.Value = moba != null ? moba.TDL_MOBA_EXP_MEST_CODE : "";
                                        }
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Search()
        {
            btnSearch_Click(null, null);
        }

        public void Export()
        {
            btnExport_Click(null, null);
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (MedicineTypeADO)gridviewFormList.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IsExp)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        if (data.MEST_ID > 0)
                        {
                            var exp = expMest.FirstOrDefault(o => o.ID == data.MEST_ID);
                            if (exp != null && exp.IS_NOT_TAKEN == 1)
                            {
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                            }
                        }
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTo.EditValue == null)
                    {
                        dtTo.Focus();
                        dtTo.ShowPopup();
                    }
                    else
                    {
                        cboSTT.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtFrom.EditValue == null)
                    {
                        dtFrom.Focus();
                        dtFrom.ShowPopup();
                    }
                    else
                    {
                        dtFrom.Focus();
                        dtFrom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_EXP_MEST_STT> _StatusSelecteds;
        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _StatusSelecteds = new List<HIS_EXP_MEST_STT>();
                foreach (HIS_EXP_MEST_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _StatusSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_DEPARTMENT> _DepartmentSelecteds;
        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                _DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DepartmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> _ImpMediStockSelecteds;
        private void SelectionGrid__ImpMediStock(object sender, EventArgs e)
        {
            try
            {
                _ImpMediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ImpMediStockSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> _ExpMediStockSelecteds;
        private void SelectionGrid__ExpMediStock(object sender, EventArgs e)
        {
            try
            {
                _ExpMediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ExpMediStockSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_IMP_MEST_TYPE> _ImpMestTypeSelecteds;
        private void SelectionGrid__ImpMestType(object sender, EventArgs e)
        {
            try
            {
                _ImpMestTypeSelecteds = new List<HIS_IMP_MEST_TYPE>();
                foreach (HIS_IMP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ImpMestTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_EXP_MEST_TYPE> _ExpMestTypeSelecteds;
        private void SelectionGrid__ExpMestType(object sender, EventArgs e)
        {
            try
            {
                _ExpMestTypeSelecteds = new List<HIS_EXP_MEST_TYPE>();
                foreach (HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ExpMestTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSTT_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    foreach (var item in _StatusSelecteds)
                    {
                        statusName += item.EXP_MEST_STT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _DepartmentSelecteds)
                    {
                        statusName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ImpMediStockSelecteds != null && _ImpMediStockSelecteds.Count > 0)
                {
                    foreach (var item in _ImpMediStockSelecteds)
                    {
                        statusName += item.MEDI_STOCK_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ExpMediStockSelecteds != null && _ExpMediStockSelecteds.Count > 0)
                {
                    foreach (var item in _ExpMediStockSelecteds)
                    {
                        statusName += item.MEDI_STOCK_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMestType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ImpMestTypeSelecteds != null && _ImpMestTypeSelecteds.Count > 0)
                {
                    foreach (var item in _ImpMestTypeSelecteds)
                    {
                        statusName += item.IMP_MEST_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMestType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ExpMestTypeSelecteds != null && _ExpMestTypeSelecteds.Count > 0)
                {
                    foreach (var item in _ExpMestTypeSelecteds)
                    {
                        statusName += item.EXP_MEST_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                List<MedicineTypeADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MedicineTypeADO> rearchResult = new List<MedicineTypeADO>();
                    rearchResult = this.listMetyAdos.Where(o => ((o.KEY_WORD ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))).Distinct().ToList();
                    listResult = rearchResult;
                }
                else
                {
                    listResult = this.listMetyAdos;
                }

                if (listResult != null && listResult.Count > 0)
                {
                    listResult = listResult.OrderBy(p => p.TIME).ThenBy(p => p.MEST_CODE).ToList();
                }

                gridControlFormList.BeginUpdate();
                gridControlFormList.DataSource = listResult;
                gridControlFormList.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSTT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpMediStock.Focus();
                    cboImpMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExpMediStock.Focus();
                    cboExpMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpMestType.Focus();
                    cboImpMestType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpMestType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExpMestType.Focus();
                    cboExpMestType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_View_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var ExpMestData = (MedicineTypeADO)gridviewFormList.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = new Inventec.Desktop.Common.Modules.Module();
                V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();
                if (ExpMestData.IsExp)
                {
                    moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestViewDetail").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestViewDetail");
                    if (this.expMest != null && this.expMest.Count > 0)
                    {
                        expMest = this.expMest.FirstOrDefault(p => p.ID == ExpMestData.MEST_ID);
                    }
                    else
                    {
                        expMest.ID = ExpMestData.MEST_ID;
                        expMest.EXP_MEST_TYPE_ID = ExpMestData.EXP_MEST_TYPE_ID;
                    }
                }
                else
                {
                    moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ImpMestViewDetail").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ImpMestViewDetail");
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    ImpMestViewDetailADO impMestADO = new ImpMestViewDetailADO(ExpMestData.MEST_ID, ExpMestData.IMP_MEST_TYPE_ID, ExpMestData.STT_ID);

                    listArgs.Add(impMestADO);
                    listArgs.Add(expMest);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId));

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicineTypeCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtMedicineTypeCode.Text))
                    {
                        cboMedicineType.EditValue = null;
                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                    else
                    {
                        List<MedicineADO> searchs = null;
                        var listData1 = this.listMedicineADO.Where(o => o.MEDICINE_TYPE_CODE.ToUpper().Contains(txtMedicineTypeCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.MEDICINE_TYPE_CODE.ToUpper() == txtMedicineTypeCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtMedicineTypeCode.Text = searchs[0].MEDICINE_TYPE_CODE;
                            cboMedicineType.EditValue = searchs[0].ID;
                            txtDocumentNumber.Focus();
                            txtDocumentNumber.SelectAll();
                        }
                        else
                        {
                            cboMedicineType.Focus();
                            cboMedicineType.ShowPopup();
                        }
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
                    if (cboMedicineType.EditValue != null)
                    {
                        var data = this.listMedicineADO.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtMedicineTypeCode.Text = data.MEDICINE_TYPE_CODE;
                            txtMedicineTypeCode.Focus();
                            txtMedicineTypeCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<MedicineTypeADO> LsMedicineTypeADO = new List<MedicineTypeADO>();
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "LichSuXuatNhapThuoc.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    if (gridControlFormList.DataSource != null)
                        LsMedicineTypeADO = (List<MedicineTypeADO>)gridControlFormList.DataSource;
                    foreach (var item in LsMedicineTypeADO)
                    {
                        if (item.IsExp)
                        {
                            if (expMest != null && expMest.Count > 0)
                            {
                                if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                {
                                    var mest = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                                    if (mest != null)
                                    {
                                        var stock = medistocks.FirstOrDefault(p => p.ID == mest.IMP_MEDI_STOCK_ID);
                                        item.MEDI_STOCK_NAME__STR = stock != null ? stock.MEDI_STOCK_NAME : "";
                                    }
                                    else
                                        item.MEDI_STOCK_NAME__STR = "";
                                }
                                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK 
                                    || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                                    || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                                {
                                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.TDL_TREATMENT_CODE + "-" + expMestEdit.TDL_PATIENT_NAME : "";
                                }
                                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                {
                                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.REQ_DEPARTMENT_NAME + "-" + expMestEdit.REQ_ROOM_NAME : "";
                                }
                                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                                {
                                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.TDL_PATIENT_NAME : "";
                                }
                            }
                        }
                        else
                        {
                            if (impMest != null && impMest.Count > 0)
                            {
                                if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                                {
                                    item.MEDI_STOCK_NAME__STR = dicChmsImpMest.ContainsKey(item.MEST_ID) ? dicChmsImpMest[item.MEST_ID] : "";
                                }
                                else if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL ||
                                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL ||
                                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL ||
                                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL)
                                {
                                    var moba = impMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                                    item.MEDI_STOCK_NAME__STR = moba != null ? moba.TDL_MOBA_EXP_MEST_CODE : "";
                                }
                            }
                        }
                    }

                    ProcessData(LsMedicineTypeADO, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<MedicineTypeADO> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void txtPakageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
