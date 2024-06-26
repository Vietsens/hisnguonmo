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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ApproveMobaImpMest.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ApproveMobaImpMest.Resources;
using MOS.SDO;
using HIS.Desktop.Common;
using DevExpress.XtraEditors.DXErrorProvider;


namespace HIS.Desktop.Plugins.ApproveMobaImpMest
{
    public partial class frmApproveMobaImpMest : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_IMP_MEST HisImpMest { get; set; }
        List<V_HIS_IMP_MEST_MEDICINE> listImpMestMedicine { get; set; }
        List<V_HIS_IMP_MEST_MATERIAL> listImpMestMaterial { get; set; }
        List<ImpMestMedicineADO> listMedicineADO { get; set; }
        List<ImpMestMaterialADO> listMaterialADO { get; set; }
        ImpMestAggrApprovalResultSDO rsSave = null;

        public DelegateSelectData delegateCloseForm { get; set; }

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.ApproveMobaImpMest";

        public frmApproveMobaImpMest(V_HIS_IMP_MEST _HisImpMest, Inventec.Desktop.Common.Modules.Module Module)
            : base(Module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = Module;
                this.HisImpMest = _HisImpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmApproveMobaImpMest(V_HIS_IMP_MEST _HisImpMest, DelegateSelectData _delegateCloseForm, Inventec.Desktop.Common.Modules.Module Module)
            : base(Module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = Module;
                this.HisImpMest = _HisImpMest;
                this.delegateCloseForm = _delegateCloseForm; 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApproveMobaImpMest_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadComboboxMediStock();

                loadDataToGridMedicine();
                loadDataToGridMaterial();

                InitControlState();

                SetDefaultValue();

                if (this.listMedicineADO != null && this.listMedicineADO.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                }
                else if (this.listMaterialADO != null && this.listMaterialADO.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 1;
                }

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
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNoAutoClose.Name)
                        {
                            chkNoAutoClose.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void loadDataToGridMaterial()
        {
            try
            {
                CommonParam param = new CommonParam();
                listImpMestMaterial = new List<V_HIS_IMP_MEST_MATERIAL>();
                listMaterialADO = new List<ImpMestMaterialADO>();

                HisImpMestMaterialViewFilter ImpMestmaterialFilter = new HisImpMestMaterialViewFilter();
                if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                {
                    ImpMestmaterialFilter.AGGR_IMP_MEST_ID = this.HisImpMest.ID;
                }
                if (this.HisImpMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                {
                    ImpMestmaterialFilter.IMP_MEST_ID = this.HisImpMest.ID;
                }

                listImpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, ImpMestmaterialFilter, param);

                if (listImpMestMaterial != null && listImpMestMaterial.Count > 0)
                {
                    var lstImpMestMediGroup = listImpMestMaterial.GroupBy(o => o.MATERIAL_ID);

                    foreach (var itemMaterial in lstImpMestMediGroup)
                    {
                        ImpMestMaterialADO ado = new ImpMestMaterialADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMaterialADO>(ado, itemMaterial.FirstOrDefault());

                        ado.REQ_AMOUNT = itemMaterial.Sum(o => o.REQ_AMOUNT);
                        ado.YCD_AMOUNT = itemMaterial.Sum(o => o.REQ_AMOUNT);

                        listMaterialADO.Add(ado);
                    }

                }

                gridViewMaterial.BeginUpdate();
                gridViewMaterial.GridControl.DataSource = listMaterialADO;
                gridViewMaterial.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void loadDataToGridMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                listImpMestMedicine = new List<V_HIS_IMP_MEST_MEDICINE>();
                listMedicineADO = new List<ImpMestMedicineADO>();

                HisImpMestMedicineViewFilter ImpMestmedicineFilter = new HisImpMestMedicineViewFilter();
                if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                {
                    ImpMestmedicineFilter.AGGR_IMP_MEST_ID = this.HisImpMest.ID;
                }
                if (this.HisImpMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                {
                    ImpMestmedicineFilter.IMP_MEST_ID = this.HisImpMest.ID;
                }

                listImpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, ImpMestmedicineFilter, param);

                if (listImpMestMedicine != null && listImpMestMedicine.Count > 0)
                {
                    var lstImpMestMediGroup = listImpMestMedicine.GroupBy(o => o.MEDICINE_ID);

                    foreach (var itemMedicine in lstImpMestMediGroup)
                    {
                        ImpMestMedicineADO ado = new ImpMestMedicineADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMedicineADO>(ado, itemMedicine.FirstOrDefault());

                        ado.REQ_AMOUNT = itemMedicine.Sum(o => o.REQ_AMOUNT);
                        ado.YCD_AMOUNT = itemMedicine.Sum(o => o.REQ_AMOUNT);

                        listMedicineADO.Add(ado);
                    }

                }

                gridViewMedicine.BeginUpdate();
                gridViewMedicine.GridControl.DataSource = listMedicineADO;
                gridViewMedicine.EndUpdate();

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
                btnThucNhap.Enabled = false;

                if (this.HisImpMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                {
                    btnSave.Enabled = true;
                }
                else 
                {
                    btnSave.Enabled = false;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApproveMobaImpMest.Resources.Lang", typeof(HIS.Desktop.Plugins.ApproveMobaImpMest.frmApproveMobaImpMest).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciMediStock.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStock.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciMediStock.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.cboMediStock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciPrint.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrint.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciPrint.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.chkPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciNoAutoClose.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciNoAutoClose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNoAutoClose.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.lciNoAutoClose.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNoAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.chkNoAutoClose.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnThucNhap.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.btnThucNhap.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnThucNhap.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.bbtnThucNhap.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //thuốc
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //vật tư
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmApproveMobaImpMest.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadComboboxMediStock()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisMediStockViewFilter filter = new HisMediStockViewFilter();
                filter.DEPARTMENT_ID = this.HisImpMest.REQ_DEPARTMENT_ID;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var data = new BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK>>("api/HisMediStock/GetView", ApiConsumers.MosConsumer, filter, param);

                if (data != null && data.Count > 0)
                {
                    data = data.Where(o => o.IS_FOR_REJECTED_MOBA == 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void chkNoAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNoAutoClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNoAutoClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNoAutoClose.Name;
                    csAddOrUpdate.VALUE = (chkNoAutoClose.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void cboMediStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMediStock.EditValue != null)
                {
                    cboMediStock.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboMediStock.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnThucNhap_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                HIS_IMP_MEST row = new HIS_IMP_MEST();

                if (this.HisImpMest != null)
                {
                    row.ID = this.HisImpMest.ID;
                
                    var apiresult = new BackendAdapter(param).Post<HIS_IMP_MEST>(ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_IMPORT, ApiConsumer.ApiConsumers.MosConsumer, row, param);

                    if (apiresult != null)
                    {
                        success = true;
                    }
                }

                if (success)
                {
                    btnThucNhap.Enabled = false;
                    if (this.delegateCloseForm != null)
                    {
                        this.delegateCloseForm(this.HisImpMest);
                    }
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        void PrintByPrintType(long printType)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrImpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrImpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    V_HIS_IMP_MEST impMestMap = new V_HIS_IMP_MEST();
                    AutoMapper.Mapper.CreateMap<V_HIS_IMP_MEST_2, V_HIS_IMP_MEST>();
                    impMestMap = AutoMapper.Mapper.Map<V_HIS_IMP_MEST>(this.HisImpMest);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(impMestMap);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                {
                    //In phiếu trả thuốc tổng hợp (Mps000078)
                    PrintByPrintType(2);
                }

                if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL || this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL)
                {
                    //In phiếu trả (Mps000100)
                    PrintByPrintType(4);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var listdata = (List<ImpMestMedicineADO>)gridControlMedicine.DataSource;

                foreach (var data in listdata)
                {
                    if (data.YCD_AMOUNT < 0 || data.YCD_AMOUNT > data.REQ_AMOUNT)
                    {
                        data.ErrorYcdAmount = ErrorType.Critical;
                        data.ErrorMessageYcdAmount = ResourceMessage.SpinEdit__SoLuongDuyetPhaiLonHonKhongVaNhoHonGiaTriYeuCau;
                    }
                    else
                    {
                        data.ErrorYcdAmount = ErrorType.None;
                        data.ErrorMessageYcdAmount = "";
                    }

                    if (Inventec.Common.String.CountVi.Count(data.NOTE) > 1000)
                    {
                        data.ErrorNote = ErrorType.Critical;
                        data.ErrorMessageNote = ResourceMessage.TextEdit__GhiChuVuotQuakyTuChoPhep;
                    }
                    else
                    {
                        data.ErrorNote = ErrorType.None;
                        data.ErrorMessageNote = "";
                    }
                }
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
                var listdata = (List<ImpMestMaterialADO>)gridControlMaterial.DataSource;

                foreach (var data in listdata)
                {
                    if (data.YCD_AMOUNT < 0 || data.YCD_AMOUNT > data.REQ_AMOUNT)
                    {
                        data.ErrorYcdAmount = ErrorType.Critical;
                        data.ErrorMessageYcdAmount = ResourceMessage.SpinEdit__SoLuongDuyetPhaiLonHonKhongVaNhoHonGiaTriYeuCau;
                    }
                    else
                    {
                        data.ErrorYcdAmount = ErrorType.None;
                        data.ErrorMessageYcdAmount = "";
                    }

                    if (Inventec.Common.String.CountVi.Count(data.NOTE) > 1000)
                    {
                        data.ErrorNote = ErrorType.Critical;
                        data.ErrorMessageNote = ResourceMessage.TextEdit__GhiChuVuotQuakyTuChoPhep;
                    }
                    else
                    {
                        data.ErrorNote = ErrorType.None;
                        data.ErrorMessageNote = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ImpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ImpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                    }
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
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.rsSave = new ImpMestAggrApprovalResultSDO();
                List<string> lstError_YcdAmount = new List<string>();
                List<string> lstError_Note = new List<string>();

                if (this.listMedicineADO != null && this.listMedicineADO.Count > 0)
                {
                    var check = this.listMedicineADO.Where(o => o.ErrorYcdAmount == ErrorType.Critical || o.ErrorNote == ErrorType.Critical).ToList();

                    if (check != null && check.Count > 0)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                    
                }

                if (this.listMaterialADO != null && this.listMaterialADO.Count > 0)
                {
                    var check = this.listMaterialADO.Where(o => o.ErrorYcdAmount == ErrorType.Critical || o.ErrorNote == ErrorType.Critical).ToList();

                    if (check != null && check.Count > 0)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }


                ImpMestAggrApprovalSDO data = new ImpMestAggrApprovalSDO();

                data.ImpMestId = this.HisImpMest.ID;
                data.RequestRoomId = this.currentModule.RoomId;

                if (cboMediStock.EditValue != null)
                {
                    data.RejectedMediStockId = (long)cboMediStock.EditValue;
                }
                else 
                {
                    data.RejectedMediStockId = null;
                }

                data.ApprovalMedicines = new List<MobaMedicineSDO>();
                data.ApprovalMaterials = new List<MobaMaterialSDO>();

                if (this.listMedicineADO != null && this.listMedicineADO.Count > 0)
                {
                    foreach (var itemMedi in listMedicineADO)
                    {
                        MobaMedicineSDO medicineSdo = new MobaMedicineSDO();
                        medicineSdo.MedicineId = itemMedi.MEDICINE_ID;
                        medicineSdo.Amount = itemMedi.YCD_AMOUNT.Value;
                        medicineSdo.Note = itemMedi.NOTE;

                        data.ApprovalMedicines.Add(medicineSdo);
                    }
                }

                if (this.listMaterialADO != null && this.listMaterialADO.Count > 0)
                {
                    foreach (var itemMater in listMaterialADO)
                    {
                        MobaMaterialSDO materialSdo = new MobaMaterialSDO();
                        materialSdo.MaterialId = itemMater.MATERIAL_ID;
                        materialSdo.Amount = itemMater.YCD_AMOUNT.Value;
                        materialSdo.Note = itemMater.NOTE;

                        data.ApprovalMaterials.Add(materialSdo);
                    }
                }

                this.rsSave = new Inventec.Common.Adapter.BackendAdapter(param).Post<ImpMestAggrApprovalResultSDO>(
                    "api/HisImpMest/AggrApprove", ApiConsumers.MosConsumer, data, param);

                if (this.rsSave != null )
                {
                    success = true;
                    this.HisImpMest = this.rsSave.ImpMest;
                }

                if (success)
                {
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);

                    if (chkPrint.Checked)
                    {
                        if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT)
                        {
                            //In phiếu trả thuốc tổng hợp (Mps000078)
                            PrintByPrintType(2);
                        }

                        if (this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL || this.HisImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL)
                        {
                            //In phiếu trả (Mps000100)
                            PrintByPrintType(4);
                        }
                    }

                    if (!chkNoAutoClose.Checked)
                    {
                        this.Close();

                        if (this.delegateCloseForm != null)
                        {
                            this.delegateCloseForm(this.HisImpMest);
                        }
                    }
                    else
                    {
                        btnSave.Enabled = false;

                        if (this.HisImpMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                        {
                            btnThucNhap.Enabled = true;
                        }
                    }

                }
                else
                {
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnThucNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnThucNhap.Enabled) return;
                btnThucNhap.Focus();
                btnThucNhap_Click(null, null);
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

        private void frmApproveMobaImpMest_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.delegateCloseForm != null)
                {
                    this.delegateCloseForm(this.HisImpMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewMedicine_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "YCD_AMOUNT" || e.ColumnName == "NOTE")
                {
                    this.gridViewMedicine_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicine_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewMedicine.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlMedicine.DataSource as List<ImpMestMedicineADO>;
                var row = listDatas[index];
                if (e.ColumnName == "YCD_AMOUNT")
                {
                    if (row.ErrorYcdAmount == ErrorType.Critical)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorYcdAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageYcdAmount);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "NOTE")
                {
                    if (row.ErrorNote == ErrorType.Critical)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorNote);
                        e.Info.ErrorText = (string)(row.ErrorMessageNote);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "YCD_AMOUNT" || e.ColumnName == "NOTE")
                {
                    this.gridViewMaterial_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewMaterial.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlMedicine.DataSource as List<ImpMestMaterialADO>;
                var row = listDatas[index];
                if (e.ColumnName == "YCD_AMOUNT")
                {
                    if (row.ErrorYcdAmount == ErrorType.Critical)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorYcdAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageYcdAmount);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "NOTE")
                {
                    if (row.ErrorNote == ErrorType.Critical)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorNote);
                        e.Info.ErrorText = (string)(row.ErrorMessageNote);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
