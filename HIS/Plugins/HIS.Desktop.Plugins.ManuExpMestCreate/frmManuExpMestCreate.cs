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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuExpMestCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using MOS.Filter;

namespace HIS.Desktop.Plugins.ManuExpMestCreate
{
    public partial class frmManuExpMestCreate : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<RepayMedicineSDO> lstRepayMedicine;
        List<RepayMaterialSDO> lstRepayMaterial;
        List<RepayBloodSDO> lstRepayBlood;
        int Action { get; set; }
        internal V_HIS_IMP_MEST _ManuImpMest;
        internal V_HIS_EXP_MEST _ManuExpMest;
        internal HisExpMestResultSDO manuExpMestResult;
        internal long _ImpMestId;
        internal long _ExpMestId;
        internal long _MediStockId;
        public frmManuExpMestCreate()
        {
            InitializeComponent();
        }

        public frmManuExpMestCreate(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST ImpMest)
            : base(currentModule)
        {
            InitializeComponent();
            this._ManuImpMest = ImpMest;
            this.currentModule = currentModule;
            this.Action = GlobalVariables.ActionAdd;
            this._ImpMestId = ImpMest.ID;
            this._MediStockId = ImpMest.MEDI_STOCK_ID;
        }

        public frmManuExpMestCreate(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this._ManuExpMest = expMest;
                this.currentModule = currentModule;
                this.Action = GlobalVariables.ActionEdit;
                this._ImpMestId = expMest.MANU_IMP_MEST_ID ?? 0;
                this._MediStockId = expMest.MEDI_STOCK_ID;
                this._ExpMestId = expMest.ID;
                this.txtDescription.Text = expMest.DESCRIPTION;
                this.cboExpMestReason.EditValue = expMest.EXP_MEST_REASON_ID;
                btnNew.Enabled = false;
                LoadDataExpMest(expMest.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmManuExpMestCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                SetIconFrm();
                Config.HisConfigCFG.LoadConfig();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                if (this._ManuExpMest != null && this._ManuExpMest.SUPPLIER_ID > 0)
                {
                    var data = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == this._ManuExpMest.SUPPLIER_ID);
                    if (data != null)
                    {
                        lblNhaCungCap.Text = data.SUPPLIER_NAME;
                    }
                }
                else if (this._ManuImpMest != null && this._ManuImpMest.SUPPLIER_ID > 0)
                {
                    var data = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == this._ManuImpMest.SUPPLIER_ID);
                    if (data != null)
                    {
                        lblNhaCungCap.Text = data.SUPPLIER_NAME;
                    }
                }
                InitComboExpMestReason();
                SetEnableControl(Action);
                loadDataExpMestMedicine();
                loadDataExpMestMaterial();
                LoadDataExpMestBlood();
                if (this.lstRepayMedicine != null && this.lstRepayMedicine.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                }
                else if (this.lstRepayMaterial != null && lstRepayMaterial.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 1;
                }
                else if (this.lstRepayBlood != null && lstRepayBlood.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 2;
                }
                else
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboExpMestReason()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                int width = cboExpMestReason.Width;
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", width-70, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, width);
                ControlEditorLoader.Load(cboExpMestReason, data, controlEditorADO);

                if (Config.HisConfigCFG.ExpMestCFG_IsReasonRequired == "1")
                {
                    lciExpMestReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboExpMestReason);
                }
                else
                {
                    lciExpMestReason.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.SetValidationRule(cboExpMestReason, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ManuExpMestCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.ManuExpMestCreate.frmManuExpMestCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumnMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAvailableAmount.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumnAvailableAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmManuExpMestCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void LoadDataExpMest(long _expMestId)
        {
            try
            {
                manuExpMestResult = new HisExpMestResultSDO();
                manuExpMestResult.ExpMest = new HIS_EXP_MEST();
                manuExpMestResult.ExpMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                manuExpMestResult.ExpMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                manuExpMestResult.ExpBloods = new List<HIS_EXP_MEST_BLOOD>();

                HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                if (this._ManuExpMest != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(expMest, this._ManuExpMest);
                }

                manuExpMestResult.ExpMest = expMest;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineFilter medicineFilter = new MOS.Filter.HisExpMestMedicineFilter();
                medicineFilter.EXP_MEST_ID = _expMestId;
                var dataMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    manuExpMestResult.ExpMedicines = dataMedicines;
                }
                MOS.Filter.HisExpMestMaterialFilter materialFilter = new MOS.Filter.HisExpMestMaterialFilter();
                materialFilter.EXP_MEST_ID = _expMestId;
                var dataMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GET, ApiConsumer.ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    manuExpMestResult.ExpMaterials = dataMaterials;
                }
                MOS.Filter.HisExpMestBloodFilter bloodFilter = new MOS.Filter.HisExpMestBloodFilter();
                bloodFilter.EXP_MEST_ID = _expMestId;
                var dataBloods = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GET, ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataBloods != null && dataBloods.Count > 0)
                {
                    manuExpMestResult.ExpBloods = dataBloods;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestBlood()
        {
            try
            {
                CommonParam param = new CommonParam();
                this.lstRepayBlood = new List<RepayBloodSDO>();
                var dataBloods = new BackendAdapter(param).Get<List<HisImpMestBloodWithInStockInfoSDO>>("api/HisImpMestBlood/GetViewWithInStockInfo", ApiConsumer.ApiConsumers.MosConsumer, this._ImpMestId, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (dataBloods != null && dataBloods.Count > 0)
                {
                    foreach (var item in dataBloods)
                    {
                        RepayBloodSDO ado = new RepayBloodSDO(item);
                        if (this.manuExpMestResult != null && this.manuExpMestResult.ExpBloods != null && this.manuExpMestResult.ExpBloods.Count > 0)
                        {
                            var data = this.manuExpMestResult.ExpBloods.FirstOrDefault(p => p.BLOOD_ID == item.BLOOD_ID && item.IsAvailable);
                            if (data != null)
                            {
                                ado.IsCheck = true;
                            }
                        }
                        this.lstRepayBlood.Add(ado);
                    }
                }
                gridControlExpMestBlood.DataSource = null;
                gridControlExpMestBlood.DataSource = this.lstRepayBlood;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void loadDataExpMestMedicine()
        {
            CommonParam param = new CommonParam();
            try
            {
                this.lstRepayMedicine = new List<RepayMedicineSDO>();
                var rs = new BackendAdapter(param).Get<List<HisImpMestMedicineWithInStockAmountSDO>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW_WITH_IN_STOCK_AMOUNT, ApiConsumers.MosConsumer, this._ImpMestId,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (rs != null && rs.Count > 0)
                {
                    foreach (var item in rs)
                    {
                        RepayMedicineSDO repayMedicine = new RepayMedicineSDO();
                        Mapper.CreateMap<HisImpMestMedicineWithInStockAmountSDO, RepayMedicineSDO>();
                        repayMedicine = Mapper.Map<HisImpMestMedicineWithInStockAmountSDO, RepayMedicineSDO>(item);
                        if (this.manuExpMestResult != null && this.manuExpMestResult.ExpMedicines != null && this.manuExpMestResult.ExpMedicines.Count > 0)
                        {
                            var data = this.manuExpMestResult.ExpMedicines.FirstOrDefault(p => p.MEDICINE_ID == item.MEDICINE_ID);
                            if (data != null && data.AMOUNT > 0)
                            {
                                repayMedicine.RepayAmountInput = data.AMOUNT;
                                repayMedicine.IsCheck = true;
                                if (this.Action == GlobalVariables.ActionAdd && item.AvailableAmount <= 0)
                                {
                                    repayMedicine.IsCheck = false;
                                }
                                repayMedicine.AmountOld = data.AMOUNT;
                            }
                        }
                        lstRepayMedicine.Add(repayMedicine);
                    }
                }
                if (this.lstRepayMedicine != null && this.lstRepayMedicine.Count > 0)
                {
                    this.lstRepayMedicine = this.lstRepayMedicine.OrderByDescending(o => o.NUM_ORDER).ToList();
                }
                gridControlExpMestMedicine.DataSource = null;
                gridControlExpMestMedicine.DataSource = lstRepayMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadDataExpMestMaterial()
        {
            CommonParam param = new CommonParam();
            try
            {
                this.lstRepayMaterial = new List<RepayMaterialSDO>();
                var rs = new BackendAdapter(param).Get<List<HisImpMestMaterialWithInStockAmountSDO>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW_WITH_IN_STOCK_AMOUNT, ApiConsumers.MosConsumer, this._ImpMestId, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (rs != null)
                {
                    foreach (var item in rs)
                    {
                        RepayMaterialSDO repayMaterial = new RepayMaterialSDO();
                        Mapper.CreateMap<HisImpMestMaterialWithInStockAmountSDO, RepayMaterialSDO>();
                        repayMaterial = Mapper.Map<HisImpMestMaterialWithInStockAmountSDO, RepayMaterialSDO>(item);
                        if (this.manuExpMestResult != null && this.manuExpMestResult.ExpMaterials != null && this.manuExpMestResult.ExpMaterials.Count > 0)
                        {
                            var data = this.manuExpMestResult.ExpMaterials.FirstOrDefault(p => p.MATERIAL_ID == item.MATERIAL_ID);
                            if (data != null && data.AMOUNT > 0)
                            {
                                repayMaterial.RepayAmountInput = data.AMOUNT;
                                repayMaterial.IsCheck = true;
                                if (this.Action == GlobalVariables.ActionAdd && item.AvailableAmount <= 0)
                                {
                                    repayMaterial.IsCheck = false;
                                }
                                repayMaterial.AmountOld = data.AMOUNT;
                            }
                        }
                        lstRepayMaterial.Add(repayMaterial);
                    }
                }
                if (this.lstRepayMaterial != null && this.lstRepayMaterial.Count > 0)
                {
                    this.lstRepayMaterial = this.lstRepayMaterial.OrderByDescending(o => o.NUM_ORDER).ToList();
                }
                gridControlExpMestMaterial.DataSource = null;
                gridControlExpMestMaterial.DataSource = lstRepayMaterial;
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
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (RepayBloodSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            if (data.EXPIRED_DATE > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "IMP_TIME_STR")
                        {
                            if (data.IMP_TIME > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    RepayMedicineSDO data = (RepayMedicineSDO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(data.EXPIRED_DATE.ToString()));
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                    {
                        e.Value = data.AMOUNT;
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        e.Value = (data.IMP_VAT_RATIO * 100);
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                    {
                        e.Value = data.IMP_PRICE;
                    }
                    else if (e.Column.FieldName == "SUM_PRICE_DISPLAY")
                    {
                        decimal SUM_PRICE = data.IMP_PRICE * data.AMOUNT * (1 + data.IMP_VAT_RATIO);
                        e.Value = SUM_PRICE;
                    }
                    //else if (e.Column.FieldName == "DX$CheckboxSelectorColumn" && data.RepayAmountInput > 0)
                    //{
                    //    gridViewExpMestMedicine.SelectRow(e.ListSourceRowIndex);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    RepayMaterialSDO data = (RepayMaterialSDO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(data.EXPIRED_DATE.ToString()));
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                    {
                        e.Value = data.AMOUNT;
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        e.Value = (data.IMP_VAT_RATIO * 100);
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                    {
                        e.Value = data.IMP_PRICE;
                    }
                    else if (e.Column.FieldName == "SUM_PRICE_DISPLAY")
                    {
                        decimal SUM_PRICE = data.IMP_PRICE * data.AMOUNT * (1 + data.IMP_VAT_RATIO);
                        e.Value = SUM_PRICE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var indexArr = gridViewExpMestMedicine.GetSelectedRows();
                var listDataSource = (List<RepayMedicineSDO>)gridControlExpMestMedicine.DataSource;
                foreach (var item in listDataSource)
                {
                    item.RepayAmountInput = 0;
                }
                foreach (var i in indexArr)
                {
                    var data = (RepayMedicineSDO)gridViewExpMestMedicine.GetRow(i);
                    if (data != null)
                    {
                        data.RepayAmountInput = data.AvailableAmount ?? 0;
                    }
                    else
                    {
                        data.RepayAmountInput = 0;
                    }
                }
                gridControlExpMestMedicine.RefreshDataSource();
                //var row = (RepayMedicineSDO)gridViewExpMestMedicine.GetRow(gridViewExpMestMedicine.FocusedRowHandle);
                //if (row != null)
                //{
                //    if (gridViewExpMestMedicine.GetSelectedRows().Contains(gridViewExpMestMedicine.FocusedRowHandle))
                //    {
                //        row.RepayAmountInput = row.AvailableAmount ?? 0;
                //    }
                //    else
                //    {
                //        row.RepayAmountInput = 0;
                //    }
                //    gridViewExpMestMedicine.RefreshData();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMaterial_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var indexArr = gridViewExpMestMaterial.GetSelectedRows();
                var listDataSource = (List<RepayMaterialSDO>)gridControlExpMestMaterial.DataSource;
                foreach (var item in listDataSource)
                {
                    item.RepayAmountInput = 0;
                }
                foreach (var i in indexArr)
                {
                    var data = (RepayMaterialSDO)gridViewExpMestMaterial.GetRow(i);
                    if (data != null)
                    {
                        data.RepayAmountInput = data.AvailableAmount ?? 0;
                    }
                    else
                    {
                        data.RepayAmountInput = 0;
                    }
                }
                gridControlExpMestMaterial.RefreshDataSource();
                //var row = (RepayMaterialSDO)gridViewExpMestMaterial.GetRow(gridViewExpMestMaterial.FocusedRowHandle);
                //if (row != null)
                //{
                //    if (gridViewExpMestMaterial.GetSelectedRows().Contains(gridViewExpMestMaterial.FocusedRowHandle))
                //    {
                //        row.RepayAmountInput = row.AvailableAmount ?? 0;
                //    }
                //    else
                //    {
                //        row.RepayAmountInput = 0;
                //    }
                //    gridViewExpMestMaterial.RefreshData();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            BaseEditViewInfo info = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo)e.Cell).ViewInfo;
            string error = GetErrorMedicine(e.RowHandle, e.Column);
            SetError(info, error);
            info.CalcViewInfo(e.Graphics);
        }

        public string GetErrorMedicine(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == gridColumnMobaAmount.FieldName)
                {
                    RepayMedicineSDO data = (RepayMedicineSDO)gridViewExpMestMedicine.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;
                    if (data.RepayAmountInput < 0 || data.RepayAmountInput > data.AMOUNT)
                    {
                        return "Giá trị không hợp lệ.";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        private void gridViewExpMestMaterial_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            BaseEditViewInfo info = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo)e.Cell).ViewInfo;
            string error = GetErrorMaterial(e.RowHandle, e.Column);
            SetError(info, error);
            info.CalcViewInfo(e.Graphics);
        }

        public string GetErrorMaterial(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == gridColumnMobaAmount.FieldName)
                {
                    RepayMaterialSDO data = (RepayMaterialSDO)gridViewExpMestMaterial.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;
                    if (data.RepayAmountInput < 0 || data.RepayAmountInput > data.AMOUNT)
                    {
                        return "Giá trị không hợp lệ.";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        void SetError(BaseEditViewInfo cellInfo, string errorIconText)
        {
            try
            {
                if (errorIconText == string.Empty)
                {
                    cellInfo.ErrorIconText = null;
                    cellInfo.ShowErrorIcon = false;
                    return;
                }
                cellInfo.ErrorIconText = errorIconText;
                cellInfo.ShowErrorIcon = true;
                cellInfo.FillBackground = true;
                cellInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMaterial_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                string error = GetErrorMaterial(gridViewExpMestMaterial.FocusedRowHandle, gridViewExpMestMaterial.FocusedColumn);
                if (error == string.Empty) return;
                gridViewExpMestMaterial.SetColumnError(gridViewExpMestMaterial.FocusedColumn, error);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                string error = GetErrorMedicine(gridViewExpMestMedicine.FocusedRowHandle, gridViewExpMestMedicine.FocusedColumn);
                if (error == string.Empty) return;
                gridViewExpMestMedicine.SetColumnError(gridViewExpMestMedicine.FocusedColumn, error);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableControl(int action)
        {
            try
            {
                //  btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnPrint.Enabled = (action == GlobalVariables.ActionView || action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
               
                bool success = false;

                if (!dxValidationProvider1.Validate())
                    return;

                CommonParam param = new CommonParam();
                HisExpMestManuSDO manuExpMest = new HisExpMestManuSDO();
                manuExpMest.Medicines = new List<ExpMedicineSDO>();
                manuExpMest.Materials = new List<ExpMaterialSDO>();
                manuExpMest.Bloods = new List<ExpBloodSDO>();
                manuExpMest.ManuImpMestId = this._ImpMestId;
                manuExpMest.ReqRoomId = this.currentModule.RoomId;
                manuExpMest.Description = txtDescription.Text;
                if (cboExpMestReason.EditValue!= null)
                    manuExpMest.ExpMestReasonId = Convert.ToInt64(cboExpMestReason.EditValue);
                manuExpMest.MediStockId = this._MediStockId;
                if (this._ExpMestId > 0)
                {
                    manuExpMest.ExpMestId = this._ExpMestId;
                }
                if (
                    gridViewExpMestMedicine.SelectedRowsCount <= 0
                    && gridViewExpMestMaterial.SelectedRowsCount <= 0
                    && gridViewExpMestBlood.SelectedRowsCount <= 0
                    )
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                    return;
                }
                if (this.lstRepayBlood != null && this.lstRepayBlood.Count > 0)
                {
                    var dataChecks = this.lstRepayBlood.Where(p => p.IsCheck).ToList();
                    if (dataChecks != null && dataChecks.Count > 0)
                    {
                        foreach (var item in dataChecks)
                        {
                            ExpBloodSDO Bloodsdo = new ExpBloodSDO();
                            Bloodsdo.BloodId = item.BLOOD_ID;
                            manuExpMest.Bloods.Add(Bloodsdo);
                        }
                    }
                }
                if (this.lstRepayMedicine != null && this.lstRepayMedicine.Count > 0)
                {
                    var dataChecks = this.lstRepayMedicine.Where(p => p.IsCheck).ToList();
                    if (dataChecks != null && dataChecks.Count > 0)
                    {
                        foreach (var item in dataChecks)
                        {
                            if (item.RepayAmountInput <= 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải trả lớn hơn 0", "Thông báo");
                                return;
                            }
                            if (item.RepayAmountInput > item.AMOUNT)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải trả lớn hơn số lượng nhập", "Thông báo");
                                return;
                            }
                            ExpMedicineSDO medicineAmount = new ExpMedicineSDO();
                            medicineAmount.MedicineId = item.MEDICINE_ID;
                            medicineAmount.Amount = item.RepayAmountInput;
                            manuExpMest.Medicines.Add(medicineAmount);
                        }
                    }
                }
                if (this.lstRepayMaterial != null && this.lstRepayMaterial.Count > 0)
                {
                    var dataChecks = this.lstRepayMaterial.Where(p => p.IsCheck).ToList();
                    if (dataChecks != null && dataChecks.Count > 0)
                    {
                        foreach (var item in dataChecks)
                        {
                            if (item.RepayAmountInput <= 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải trả lớn hơn 0", "Thông báo");
                                return;
                            }
                            if (item.RepayAmountInput > item.AMOUNT)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng phải trả lớn hơn số lượng nhập", "Thông báo");
                                return;
                            }
                            ExpMaterialSDO materialAmount = new ExpMaterialSDO();
                            materialAmount.MaterialId = item.MATERIAL_ID;
                            materialAmount.Amount = item.RepayAmountInput;
                            manuExpMest.Materials.Add(materialAmount);
                        }
                    }
                }

                if (manuExpMest.Bloods.Count == 0 && manuExpMest.Materials.Count == 0 && manuExpMest.Medicines.Count == 0)
                {
                    string mess = "";
                    if (this.Action == GlobalVariables.ActionAdd)
                    {
                        mess = "Dữ liệu rỗng.Vui lòng chọn loại dịch vụ";
                    }
                    else
                    {
                        mess = "Xử lý thất bại. Yêu cầu chọn loại dịch vụ để sửa.";
                    }
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                    return;
                }
                WaitingManager.Show();
                manuExpMestResult = new HisExpMestResultSDO();
                if (this.Action == GlobalVariables.ActionAdd)
                {
                    //Create
                    manuExpMestResult = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/ManuCreate", ApiConsumers.MosConsumer, manuExpMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
                else
                {
                    //Update
                    manuExpMestResult = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/ManuUpdate", ApiConsumers.MosConsumer, manuExpMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
                if (manuExpMestResult != null)
                {
                    success = true;
                    this._MediStockId = this.manuExpMestResult.ExpMest.MEDI_STOCK_ID;
                    this._ExpMestId = this.manuExpMestResult.ExpMest.ID;
                    this._ImpMestId = this.manuExpMestResult.ExpMest.MANU_IMP_MEST_ID ?? 0;
                    this.Action = GlobalVariables.ActionView;
                    SetEnableControl(this.Action);
                    loadDataExpMestMaterial();
                    loadDataExpMestMedicine();
                    LoadDataExpMestBlood();
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.IN_PHIEU_TRA_NHA_CUNG_CAP);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
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
                WaitingManager.Show();
                this.manuExpMestResult = new HisExpMestResultSDO();
                txtDescription.Text = "";
                cboExpMestReason.EditValue = null;
                this.Action = GlobalVariables.ActionAdd;
                SetEnableControl(Action);
                loadDataExpMestMedicine();
                loadDataExpMestMaterial();
                LoadDataExpMestBlood();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlood_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (RepayBloodSDO)gridViewExpMestBlood.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IsCheck")
                    {
                        e.RepositoryItem = repositoryItemCheck_E;
                        if (this.Action == GlobalVariables.ActionAdd || this.Action == GlobalVariables.ActionView)
                        {
                            if (data != null && !data.IsAvailable)
                                e.RepositoryItem = repositoryItemCheck_D;
                            else
                                e.RepositoryItem = repositoryItemCheck_E;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isCheckAll = true;
        private void gridViewExpMestBlood_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
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
                                if (checkEdit.Properties.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style2)
                                    return;
                                checkEdit.Checked = !checkEdit.Checked;
                                //if (gridViewExpMestBlood.SelectedRowsCount > 0)
                                //{
                                //    gridColumnIsCheck.Image = imageListIcon.Images[5];
                                //}
                                //else
                                //{
                                //    gridColumnIsCheck.Image = imageListIcon.Images[6];
                                //}
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnIsCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestBlood.BeginUpdate();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this.lstRepayBlood)
                                {
                                    item.IsCheck = true;
                                    if (!item.IsAvailable && this.Action == GlobalVariables.ActionAdd)
                                    {
                                        item.IsCheck = false;
                                    }
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnIsCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this.lstRepayBlood)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewExpMestBlood.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (RepayMedicineSDO)gridViewExpMestMedicine.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IsCheck")
                    {
                        e.RepositoryItem = repositoryItemCheck_E;
                        if (this.Action == GlobalVariables.ActionAdd)
                        {
                            if (data != null && data.AvailableAmount <= 0)
                                e.RepositoryItem = repositoryItemCheck_D;
                            else
                                e.RepositoryItem = repositoryItemCheck_E;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isCheckAllMedi = true;
        private void gridViewExpMestMedicine_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
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
                                if (checkEdit.Properties.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style2)
                                    return;
                                checkEdit.Checked = !checkEdit.Checked;
                                //if (gridViewExpMestMedicine.SelectedRowsCount > 0)
                                //{
                                //    gridColumnMediCheck.Image = imageListIcon.Images[5];
                                //}
                                //else
                                //{
                                //    gridColumnMediCheck.Image = imageListIcon.Images[6];
                                //}
                                var dataRow = (RepayMedicineSDO)gridViewExpMestMedicine.GetRow(hi.RowHandle);
                                if (dataRow != null)
                                {
                                    if (checkEdit.Checked)
                                    {
                                        dataRow.RepayAmountInput = dataRow.AmountOld > 0 ? dataRow.AmountOld : dataRow.AvailableAmount ?? 0;
                                    }
                                    else
                                    {
                                        dataRow.RepayAmountInput = 0;
                                    }
                                }

                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnMediCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestMedicine.BeginUpdate();
                            if (isCheckAllMedi == true)
                            {
                                foreach (var item in this.lstRepayMedicine)
                                {
                                    item.IsCheck = true;
                                    item.RepayAmountInput = item.AmountOld > 0 ? item.AmountOld : item.AvailableAmount ?? 0;
                                    if (item.AvailableAmount <= 0)
                                    {
                                        item.IsCheck = false;
                                        item.RepayAmountInput = 0;
                                    }
                                }
                                isCheckAllMedi = false;
                            }
                            else
                            {
                                gridColumnMediCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this.lstRepayMedicine)
                                {
                                    item.IsCheck = false;
                                    item.RepayAmountInput = 0;
                                }
                                isCheckAllMedi = true;
                            }
                            gridViewExpMestMedicine.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (RepayMaterialSDO)gridViewExpMestMaterial.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IsCheck")
                    {
                        e.RepositoryItem = repositoryItemCheck_E;
                        if (this.Action == GlobalVariables.ActionAdd)
                        {
                            if (data != null && data.AvailableAmount <= 0)
                                e.RepositoryItem = repositoryItemCheck_D;
                            else
                                e.RepositoryItem = repositoryItemCheck_E;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isCheckAllMate = true;
        private void gridViewExpMestMaterial_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
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
                                if (checkEdit.Properties.CheckStyle == DevExpress.XtraEditors.Controls.CheckStyles.Style2)
                                    return;
                                checkEdit.Checked = !checkEdit.Checked;
                                //if (gridViewExpMestMaterial.SelectedRowsCount > 0)
                                //{
                                //    gridColumnMateCheck.Image = imageListIcon.Images[5];
                                //}
                                //else
                                //{
                                //    gridColumnMateCheck.Image = imageListIcon.Images[6];
                                //}
                                var dataRow = (RepayMaterialSDO)gridViewExpMestMaterial.GetRow(hi.RowHandle);
                                if (dataRow != null)
                                {
                                    if (checkEdit.Checked)
                                    {
                                        dataRow.RepayAmountInput = dataRow.AmountOld > 0 ? dataRow.AmountOld : dataRow.AvailableAmount ?? 0;
                                    }
                                    else
                                    {
                                        dataRow.RepayAmountInput = 0;
                                    }
                                }
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnMateCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestMaterial.BeginUpdate();
                            if (isCheckAllMate == true)
                            {
                                foreach (var item in this.lstRepayMaterial)
                                {
                                    item.IsCheck = true;
                                    item.RepayAmountInput = item.AmountOld > 0 ? item.AmountOld : item.AvailableAmount ?? 0;
                                    if (item.AvailableAmount <= 0)
                                    {
                                        item.IsCheck = false;
                                        item.RepayAmountInput = 0;
                                    }
                                }
                                isCheckAllMate = false;
                            }
                            else
                            {
                                gridColumnMateCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this.lstRepayMaterial)
                                {
                                    item.IsCheck = false;
                                    item.RepayAmountInput = 0;
                                }
                                isCheckAllMate = true;
                            }
                            gridViewExpMestMaterial.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMestReason_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExpMestReason.EditValue = null;
                    cboExpMestReason.Properties.Buttons[1].Visible = false;
                    cboExpMestReason.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExpMestReason.EditValue != null)
                {
                    cboExpMestReason.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboExpMestReason.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void txtKeyworkBloodInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lstRepayBlood != null && lstRepayBlood.Count > 0 && !string.IsNullOrEmpty(txtKeyworkBloodInStock.Text.Trim()) && (((List<RepayBloodSDO>)gridViewExpMestBlood.GridControl.DataSource) != null && ((List<RepayBloodSDO>)gridViewExpMestBlood.GridControl.DataSource).Count != 0))
                    {
                        var key = txtKeyworkBloodInStock.Text.Trim();
                        if (string.IsNullOrEmpty(key))
                        {
                            gridControlExpMestBlood.DataSource = lstRepayBlood;
                        }
                        else if (!String.IsNullOrEmpty(key) && !key.Contains("|") && lstRepayBlood != null)
                        {
                            var repayBloods = lstRepayBlood.Where(o =>
                                o.BLOOD_CODE.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())
                                || o.BLOOD_TYPE_NAME.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())).ToList();
                            if (repayBloods != null && repayBloods.Count > 0)
                            {
                                gridControlExpMestBlood.DataSource = repayBloods;
                            }

                            return;
                        }

                        //VjUwMDUyMjQwODExNzAwfE8gK3x8MTkvMTEvMjAyMnwzMC8xMi8yMDIyfEU3NDI2VjAwfEtI4buQSSBI4buSTkcgQ+G6plUgVOG7qiAzNTBtbCBNw4FVIFRPw4BOIFBI4bqmTnwoIDIgLSA2wrBDICk=
                        //V50052240811700|O +||19/11/2022|30/12/2022|E7426V00|KHỐI HỒNG CẦU TỪ 350ml MÁU TOÀN PHẦN|( 2 - 6°C )
                        //<Mã vạch>|<Nhóm máu> <Rh>||<Thời gian đóng gói>|<Hạn sử dụng>|<Mã loại máu>|<Tên loại máu>|<điều kiện bảo quản>
                        try
                        {
                            var QR = key.Split('|');
                            var packingTime = DecodeTime(QR[3] ?? "0");
                            var expriredTime = DecodeTime(QR[4] ?? "0");
                            var bloodInStocks = lstRepayBlood.Where(o =>
                            o.BLOOD_CODE.ToUpper().Contains(QR[0].Trim().ToUpper())
                            && o.BLOOD_ABO_CODE.ToUpper().Contains(QR[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper())
                            && o.BLOOD_RH_CODE.ToUpper().Contains(QR[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().ToUpper())
                            && o.BLOOD_TYPE_CODE.ToUpper().Contains(QR[5].Trim().ToUpper())
                            && o.BLOOD_TYPE_NAME.ToUpper().Contains(QR[6].Trim().ToUpper())
                            && o.PACKING_TIME != null && o.PACKING_TIME.ToString().Substring(0, 8).Equals(packingTime)
                            && o.EXPIRED_DATE != null && o.EXPIRED_DATE == Int64.Parse(expriredTime + "000000")).ToList();

                            if (bloodInStocks != null && bloodInStocks.Count == 1)
                            {
                                gridViewExpMestBlood.FocusedRowHandle = 0;
                                foreach (var item in bloodInStocks)
                                {
                                    if (item.IsAvailable)
                                    {
                                        item.IsCheck = true;
                                    }
                                }
                                gridControlExpMestBlood.DataSource = bloodInStocks;
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không có loại máu tương ứng");
                            }
                        }
                        catch (Exception ex)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu QR không hợp lệ vui lòng kiểm tra lại!");
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else
                    {
                        LoadDataExpMestBlood();
                    }
                    gridViewExpMestBlood.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string DecodeTime(string input)
        {
            string result = null;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    result = input.Split('/')[2] + input.Split('/')[1] + input.Split('/')[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
