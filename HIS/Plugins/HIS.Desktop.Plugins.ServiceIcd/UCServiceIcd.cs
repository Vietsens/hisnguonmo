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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.ServiceIcd.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.UC.ListIcd;
using HIS.UC.ListIcd.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ServiceIcd.Validtion;

namespace HIS.Desktop.Plugins.ServiceIcd
{
    public partial class UCServiceIcd : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCIcdProcessor icdProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlIcd;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.ListIcd.IcdADO> lstIcdADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstServiceIcdADOs { get; set; }
        List<HIS_ICD> listIcd;
        List<V_HIS_SERVICE> listService;
        long? ServiceIdCheckByService = 0;
        long? acIngrIdCheckByService = 0;
        long isChoseService;
        long isChoseIcd;
        long IcdIdCheckByIcd;
        string IcdCodeCheckByIcd;
        string IcdNameCheckByIcd;
        bool isCheckAll;
        List<HIS_ICD_SERVICE> ServiceIcds { get; set; }
        List<HIS_ICD_SERVICE> ServiceIcdViews { get; set; }
        ServiceADO currentService;
        HIS_ICD currentIcd;
        List<HIS_ICD> currentListIcd;
        List<long> currentListServices;
        List<long> currentListActiveIngredients;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ACIN> currentMedicineTypeAcins;

        IcdADO currentIcdADO_Processing;

        public UCServiceIcd(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
        }

        public UCServiceIcd(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (this.currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCServiceIcd(ServiceADO serviceData, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCServiceIcd(HIS_ICD _icd, List<long> _serviceIcds, List<long> activeIngredients)
        {
            InitializeComponent();
            try
            {
                this.currentIcd = _icd;
                this.currentListServices = _serviceIcds;
                this.currentListActiveIngredients = activeIngredients;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCServiceIcd(List<HIS_ICD> _listIcd, List<long> _serviceIcds, List<long> activeIngredients)
        {
            InitializeComponent();
            try
            {
                this.currentListIcd = _listIcd;
                this.currentListServices = _serviceIcds;
                this.currentListActiveIngredients = activeIngredients;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCServiceIcd_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService != null)
                {
                    FillDataToGrid1_ById(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                else if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentIcd != null)
                {
                    cboChoose.EditValue = (long)2;
                    FillDataToGrid2_ById(this);
                    btn_Radio_Enable_Click(this.currentIcd);
                    FillDataToGrid1_ByIds(this);
                }
                else if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentListIcd != null)
                {
                    FillDataToGrid2_ByIds(this);
                    FillDataToGrid1_ByIds(this);
                }
                else
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                ValidationControls();
                chkIsHoatChat.CheckState = CheckState.Unchecked;
                chkIsHoatChat.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControls()
        {
            try
            {
                //Nội dung chống chỉ định (Grid ICD)
                ValidMaxlengthTextBox(mmContraindicationContent, 4000, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTextBox(BaseEdit txtEdit, int? maxLength, bool isRequired)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.isRequired = isRequired;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseService == 1)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "checkService")
                        {
                            var lstCheckAll = lstServiceIcdADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstServiceIcdADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstServiceIcdADOs.Count();
                                if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionService.Images[1];
                                }

                                if (ServiceCheckedNum == ServiceNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionService.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkService = true;
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
                                            item.checkService = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ServiceProcessor.Reload(ucGridControlService, lstChecks);


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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceIcd.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceIcd.UCServiceIcd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceIcd.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceIcd.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceIcd.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceIcd.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCServiceIcd.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgrid1()
        {
            try
            {
                ServiceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<UC.Service.ServiceColumn>();
                //ado.gridViewService_MouseDownMest = gridViewService_MouseDown;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;

                ServiceColumn colRadio2 = new ServiceColumn("   ", "radioService", 20, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio2);

                ServiceColumn colCheck2 = new ServiceColumn("Cho phép chỉ định", "checkService", 50, true);
                colCheck2.VisibleIndex = 1;
                //colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck2);

                ServiceColumn colCheck3 = new ServiceColumn("Chặn chỉ định", "checkServiceNotUse", 80, true);
                colCheck3.VisibleIndex = 2;
                //colCheck3.image = imageCollectionService.Images[0];
                colCheck3.Visible = false;
                colCheck3.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck3);

                ServiceColumn colCheck4 = new ServiceColumn("Cảnh báo khi chỉ định", "checkWarning", 80, true);
                colCheck4.VisibleIndex = 3;
                //colCheck2.image = imageCollectionService.Images[0];
                colCheck4.Visible = false;
                colCheck4.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck4);

                ServiceColumn colMaDichvu = new ServiceColumn("Mã dịch vụ/ Hoạt chất", "SERVICE_CODE", 70, false);
                colMaDichvu.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colMaDichvu);

                ServiceColumn colTenDichvu = new ServiceColumn("Tên dịch vụ/ Hoạt chất", "SERVICE_NAME", 230, false);
                colTenDichvu.VisibleIndex = 5;
                ado.ListServiceColumn.Add(colTenDichvu);

                ServiceColumn colMaLoaidichvu = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 80, false);
                colMaLoaidichvu.VisibleIndex = 6;
                ado.ListServiceColumn.Add(colMaLoaidichvu);

                ServiceColumn colTGToiThieu = new ServiceColumn("TG tối thiểu (ngày)", "MIN_DURATION_STR", 90, true);
                colTGToiThieu.VisibleIndex = 7;
                colTGToiThieu.Visible = false;
                colTGToiThieu.Tooltip = "Thời gian tối thiểu giữa 2 lần chỉ định cho bệnh nhân";
                colTGToiThieu.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colTGToiThieu);

                this.ucGridControlService = (UserControl)ServiceProcessor.Run(ado);
                if (ucGridControlService != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlService);
                    this.ucGridControlService.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_ICD_SERVICE> GetIcdServicePreSaveByService(ServiceADO data)
        {
            List<HIS_ICD_SERVICE> result = new List<HIS_ICD_SERVICE>();
            try
            {
                CommonParam param = new CommonParam();
                if (data.ID > 0)
                {
                    MOS.Filter.HisIcdServiceFilter filter = new HisIcdServiceFilter();
                    filter.SERVICE_ID = data.ID;
                    result = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>(
                                        "api/HisIcdService/Get",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filter,
                                    param);
                }

                List<HIS_ICD_SERVICE> ServiceIcds1 = null;
                if (data.ACTIVE_INGREDIENT_ID > 0)
                {
                    param = new CommonParam();
                    MOS.Filter.HisIcdServiceFilter filter1 = new HisIcdServiceFilter();
                    filter1.ACTIVE_INGREDIENT_ID = data.ACTIVE_INGREDIENT_ID;
                    ServiceIcds1 = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>(
                                        "api/HisIcdService/Get",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filter1,
                                    param);
                }

                if (result == null)
                    result = new List<HIS_ICD_SERVICE>();
                if (ServiceIcds1 != null && ServiceIcds1.Count > 0)
                {
                    result.AddRange(ServiceIcds1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewMachine_MouseIcd(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseIcd == 2)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstIcdADOs;
                            List<HIS.UC.ListIcd.IcdADO> lstChecks = new List<HIS.UC.ListIcd.IcdADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MachineCheckedNum = lstIcdADOs.Where(o => o.check1 == true).Count();
                                var MachinetmNum = lstIcdADOs.Count();
                                if ((MachineCheckedNum > 0 && MachineCheckedNum < MachinetmNum) || MachineCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMachine.Images[1];
                                }

                                if (MachineCheckedNum == MachinetmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMachine.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
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
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                icdProcessor.Reload(ucGridControlIcd, lstChecks);


                            }
                        }
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(ServiceADO data)
        {
            try
            {
                this.acIngrIdCheckByService = data.ACTIVE_INGREDIENT_ID;
                this.ServiceIdCheckByService = data.ID;

                Inventec.Common.Logging.LogSystem.Debug("btn_Radio_Enable_Click1____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                ServiceIcds = new List<HIS_ICD_SERVICE>();
                WaitingManager.Show();

                ServiceIcds = GetIcdServicePreSaveByService(data);

                List<HIS.UC.ListIcd.IcdADO> dataNew = new List<HIS.UC.ListIcd.IcdADO>();
                //var list = BackendDataWorker.Get<HIS_ICD>();
                dataNew = (from r in listIcd select new IcdADO(r)).ToList();
                //dataNew = (from r in list select new IcdADO(r)).ToList();
                if (ServiceIcds != null && ServiceIcds.Count > 0)
                {
                    foreach (var itemIcd in ServiceIcds)
                    {
                        if (itemIcd.IS_INDICATION == 1)
                        {
                            var check = dataNew.FirstOrDefault(o => o.ICD_CODE == itemIcd.ICD_CODE);
                            if (check != null)
                            {
                                check.check1 = true;
                            }

                        }
                        if (itemIcd.IS_CONTRAINDICATION == 1)
                        {
                            var check = dataNew.FirstOrDefault(o => o.ICD_CODE == itemIcd.ICD_CODE);
                            if (check != null)
                            {
                                check.check2 = true;
                            }

                        }
                        if (itemIcd.IS_WARNING == 1)
                        {
                            var check = dataNew.FirstOrDefault(o => o.ICD_CODE == itemIcd.ICD_CODE);
                            if (check != null)
                            {
                                check.check3 = true;
                            }

                        }
                        var item = dataNew.FirstOrDefault(o => o.ICD_CODE == itemIcd.ICD_CODE);
                        if (item != null)
                        {
                            item.CONTRAINDICATION_CONTENT = itemIcd.CONTRAINDICATION_CONTENT;
                            item.MIN_DURATION_STR2 = itemIcd.MIN_DURATION;

                        }
                        //var check = dataNew.FirstOrDefault(o => o.ICD_CODE == itemIcd.ICD_CODE);
                        //if (check != null)
                        //{
                        //    check.check1 = true;
                        //}
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.check1).ThenByDescending(i => i.check2).ThenByDescending(i => i.check3).ThenBy(i => i.ICD_CODE).ToList();
                //dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlIcd != null)
                {
                    icdProcessor.Reload(ucGridControlIcd, dataNew);
                }
                else
                {
                    if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentListIcd != null)
                    {
                        FillDataToGrid2_ByIds(this);
                    }
                    else
                    {
                        FillDataToGrid2(this);
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

        private void InitUcgrid2()
        {
            try
            {
                icdProcessor = new UCIcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.ListIcdColumn = new List<UC.ListIcd.IcdColumn>();
                //ado.gridViewIcd_MouseDownIcd = gridViewMachine_MouseIcd;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.Delegate_repositoryItemButtonEdit_ContraindicationContent_ButtonClick = repositoryItemButtonEdit_ContraindicationContent_ButtonClick;
                ado.Delegate_repositoryItemButtonEdit_ContraindicationContent_EditValueChanged = repositoryItemButtonEdit_ContraindicationContent_EditValueChanged;

                IcdColumn colRadio1 = new IcdColumn("   ", "radio1", 20, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListIcdColumn.Add(colRadio1);

                IcdColumn colCheck1 = new IcdColumn("Cho phép chỉ định", "check1", 50, true);
                colCheck1.VisibleIndex = 1;
                //colCheck1.image = imageCollectionMachine.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListIcdColumn.Add(colCheck1);

                IcdColumn colCheck2 = new IcdColumn("Chặn chỉ định", "check2", 80, true);
                colCheck2.VisibleIndex = 2;
                //colCheck1.image = imageCollectionMachine.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListIcdColumn.Add(colCheck2);

                IcdColumn colCheck3 = new IcdColumn("Cảnh báo khi chỉ định", "check3", 80, true);
                colCheck3.VisibleIndex = 3;
                //colCheck1.image = imageCollectionMachine.Images[0];
                colCheck3.Visible = false;
                colCheck3.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListIcdColumn.Add(colCheck3);

                IcdColumn colMaPhong = new IcdColumn("Mã Icd", "ICD_CODE", 50, false);
                colMaPhong.VisibleIndex = 4;
                ado.ListIcdColumn.Add(colMaPhong);

                IcdColumn colTenPhong = new IcdColumn("Tên Icd", "ICD_NAME", 200, false);
                colTenPhong.VisibleIndex = 5;
                ado.ListIcdColumn.Add(colTenPhong);

                IcdColumn colNoiDungChongChiDinh = new IcdColumn("Nội dung chống chỉ định", "CONTRAINDICATION_CONTENT", 90, true);
                colNoiDungChongChiDinh.VisibleIndex = 6;
                colNoiDungChongChiDinh.Visible = true;
                colNoiDungChongChiDinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colNoiDungChongChiDinh.FieldName = "CONTRAINDICATION_CONTENT";
                ado.ListIcdColumn.Add(colNoiDungChongChiDinh);

                IcdColumn colTGToiThieu = new IcdColumn("TG tối thiểu (ngày)", "MIN_DURATION_STR2", 90, true);
                colTGToiThieu.VisibleIndex = 7;
                colTGToiThieu.Visible = false;
                colTGToiThieu.Tooltip = "Thời gian tối thiểu giữa 2 lần chỉ định cho bệnh nhân";
                colTGToiThieu.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListIcdColumn.Add(colTGToiThieu);


                this.ucGridControlIcd = (UserControl)icdProcessor.Run(ado);
                if (ucGridControlIcd != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlIcd);
                    this.ucGridControlIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ContraindicationContent_EditValueChanged(object sender, EventArgs e, IcdADO data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemButtonEdit_ContraindicationContent_EditValueChanged()");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("data", data));
                if (!String.IsNullOrEmpty(data.CONTRAINDICATION_CONTENT) && Inventec.Common.String.CountVi.Count(data.CONTRAINDICATION_CONTENT) > 4000)
                {
                    string content = data.CONTRAINDICATION_CONTENT;
                    UpdateDataUcGridControlIcd_CONTRAINDICATION_CONTENT(data.ID, "");

                    this.currentIcdADO_Processing = data;
                    ButtonEdit editor = sender as ButtonEdit;
                    Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                    popupContainerControlContraindicationContent.ShowPopup(new Point(buttonPosition.X + 700, buttonPosition.Bottom + 200));
                    mmContraindicationContent.Text = content;
                    mmContraindicationContent.Focus();
                    mmContraindicationContent.BeginInvoke(new MethodInvoker(delegate
                    {
                        //mmContraindicationContent.SelectionLength = mmContraindicationContent.Text.Length;
                        mmContraindicationContent.SelectionStart = mmContraindicationContent.Text.Length;
                    }));
                    if (!dxValidationProvider1.Validate(mmContraindicationContent))
                        return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ContraindicationContent_ButtonClick(object sender, ButtonPressedEventArgs e, IcdADO data)
        {
            try
            {
                this.currentIcdADO_Processing = data;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupContainerControlContraindicationContent.ShowPopup(new Point(buttonPosition.X + 700, buttonPosition.Bottom + 200));
                mmContraindicationContent.Text = data.CONTRAINDICATION_CONTENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(HIS_ICD data)
        {
            try
            {
                WaitingManager.Show();

                IcdIdCheckByIcd = data.ID;
                IcdCodeCheckByIcd = data.ICD_CODE;
                IcdNameCheckByIcd = data.ICD_NAME;

                this.ServiceIcdViews = new List<HIS_ICD_SERVICE>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisIcdServiceFilter filter = new HisIcdServiceFilter();
                filter.ICD_CODE__EXACT = data.ICD_CODE;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                this.ServiceIcdViews = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>(
                                "api/HisIcdService/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("list ServiceIcdViews", ServiceIcdViews));
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                //List<V_HIS_SERVICE> listSer = BackendDataWorker.Get<V_HIS_SERVICE>();
                //dataNew = (from r in listSer select new ServiceADO(r)).ToList();
                dataNew = (from r in listService select new ServiceADO(r)).ToList();
                if (this.ServiceIcdViews != null && this.ServiceIcdViews.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("ServiceIcdViews", ServiceIcdViews.Select(o => o.SERVICE_ID)));
                    foreach (var itemService in ServiceIcdViews)
                    {
                        //if (itemService.SERVICE_ID > 0)
                        //{
                        //    var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                        //    if (check != null)
                        //    {
                        //        check.checkService = true;
                        //    }
                        //}
                        //else if (itemService.ACTIVE_INGREDIENT_ID > 0)
                        //{
                        //    var check = dataNew.FirstOrDefault(o => o.ACTIVE_INGREDIENT_ID == itemService.ACTIVE_INGREDIENT_ID);
                        //    if (check != null)
                        //    {
                        //        check.checkService = true;
                        //    }
                        //}
                        if (itemService.SERVICE_ID > 0)
                        {
                            var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                            if (check != null)
                            {
                                if (itemService.IS_INDICATION == 1)
                                {
                                    check.checkService = true;
                                }
                                else if (itemService.IS_CONTRAINDICATION == 1)
                                {
                                    check.checkServiceNotUse = true;
                                }
                                else if (itemService.IS_WARNING == 1)
                                {
                                    check.checkWarning = true;
                                }
                                check.MIN_DURATION_STR = itemService.MIN_DURATION;
                            }
                        }
                        else if (itemService.ACTIVE_INGREDIENT_ID > 0)
                        {
                            var check = dataNew.FirstOrDefault(o => o.ACTIVE_INGREDIENT_ID == itemService.ACTIVE_INGREDIENT_ID);
                            if (check != null)
                            {
                                if (itemService.IS_INDICATION == 1)
                                {
                                    check.checkService = true;
                                }
                                else if (itemService.IS_CONTRAINDICATION == 1)
                                {
                                    check.checkServiceNotUse = true;
                                }
                                else if (itemService.IS_WARNING == 1)
                                {
                                    check.checkWarning = true;
                                }
                                check.MIN_DURATION_STR = itemService.MIN_DURATION;
                            }
                        }
                    }
                    dataNew = dataNew.OrderByDescending(p => p.checkService).ThenByDescending(i => i.checkServiceNotUse).ThenByDescending(i=>i.checkWarning).ToList();
                    if (ucGridControlService != null)
                    {
                        ServiceProcessor.Reload(ucGridControlService, dataNew);
                    }
                    var checkExistHC = dataNew.Where(o => o.ACTIVE_INGREDIENT_ID != null).FirstOrDefault();
                    if (checkExistHC != null)
                    {
                        chkIsHoatChat.Checked = true;
                    }
                    else { chkIsHoatChat.Checked = false; }
                }
                else
                {
                    if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentListIcd != null)
                    {
                        FillDataToGrid1_ByIds(this);
                    }
                    else
                    {
                        FillDataToGrid1(this);
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

        private void FillDataToGrid2(UCServiceIcd uCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridIcd(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridIcd, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2_ById(UCServiceIcd uCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridIcd_ById(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridIcd_ById, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2_ByIds(UCServiceIcd uCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridIcd_ByIds(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridIcd_ByIds, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridIcd(object data)
        {
            try
            {
                WaitingManager.Show();
                listIcd = new List<HIS_ICD>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisIcdFilter IcdFillter = new HisIcdFilter();
                IcdFillter.IS_ACTIVE = 1;
                IcdFillter.ORDER_FIELD = "MODIFY_TIME";
                IcdFillter.ORDER_DIRECTION = "DESC";
                IcdFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseIcd = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ICD>>(
                   "api/HisIcd/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      IcdFillter,
                    param);

                lstIcdADOs = new List<IcdADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listIcd = sar.Data;
                    foreach (var item in listIcd)
                    {
                        IcdADO roomaccountADO = new IcdADO(item);
                        if (isChoseIcd == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstIcdADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceIcds != null && ServiceIcds.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcds)
                    {
                        var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        if (check != null)
                        {
                            if (itemUsername.IS_INDICATION == 1)
                            {
                                check.check1 = true;
                            }
                            else if (itemUsername.IS_CONTRAINDICATION == 1)
                            {
                                check.check2 = true;
                            }
                            else if (itemUsername.IS_WARNING == 1)
                            {
                                check.check3 = true;
                            }
                            check.CONTRAINDICATION_CONTENT = itemUsername.CONTRAINDICATION_CONTENT;
                            check.MIN_DURATION_STR2 = itemUsername.MIN_DURATION;
                        }
                    }
                }
                lstIcdADOs = lstIcdADOs.OrderByDescending(p => p.check1).ThenByDescending(o => o.check2).ThenByDescending(o => o.check3).Distinct().ToList();

                if (ucGridControlIcd != null)
                {
                    icdProcessor.Reload(ucGridControlIcd, lstIcdADOs);
                }
                rowCount1 = (data == null ? 0 : lstIcdADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridIcd_ById(object data)
        {
            try
            {
                WaitingManager.Show();
                listIcd = new List<HIS_ICD>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisIcdFilter IcdFillter = new HisIcdFilter();
                IcdFillter.IS_ACTIVE = 1;
                IcdFillter.ORDER_FIELD = "MODIFY_TIME";
                IcdFillter.ORDER_DIRECTION = "DESC";
                IcdFillter.KEY_WORD = txtKeyword2.Text;
                IcdFillter.ID = this.currentIcd.ID;
                if ((long)cboChoose.EditValue == (long)2)
                {
                    isChoseIcd = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ICD>>(
                   "api/HisIcd/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      IcdFillter,
                    param);

                lstIcdADOs = new List<IcdADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listIcd = sar.Data;
                    foreach (var item in listIcd)
                    {
                        IcdADO roomaccountADO = new IcdADO(item);
                        if (isChoseIcd == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        roomaccountADO.radio1 = true;
                        lstIcdADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceIcds != null && ServiceIcds.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcds)
                    {
                        //if (itemUsername.IS_INDICATION == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check1 = true;
                        //}
                        //else if (itemUsername.IS_CONTRAINDICATION == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check2 = true;
                        //    check.CONTRAINDICATION_CONTENT = itemUsername.CONTRAINDICATION_CONTENT;
                        //}
                        //else if (itemUsername.IS_WARNING == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check3 = true;
                        //}
                        var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        if (check != null)
                        {
                            if (itemUsername.IS_INDICATION == 1)
                            {
                                check.check1 = true;
                            }
                            else if (itemUsername.IS_CONTRAINDICATION == 1)
                            {
                                check.check2 = true;
                                check.CONTRAINDICATION_CONTENT = itemUsername.CONTRAINDICATION_CONTENT;
                            }
                            else if (itemUsername.IS_WARNING == 1)
                            {
                                check.check3 = true;
                            }
                            check.MIN_DURATION_STR2 = itemUsername.MIN_DURATION;
                        }
                    }
                }
                lstIcdADOs = lstIcdADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlIcd != null)
                {
                    icdProcessor.Reload(ucGridControlIcd, lstIcdADOs);
                }
                rowCount1 = (data == null ? 0 : lstIcdADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridIcd_ByIds(object data)
        {
            try
            {
                WaitingManager.Show();
                listIcd = new List<HIS_ICD>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisIcdFilter IcdFillter = new HisIcdFilter();
                IcdFillter.IS_ACTIVE = 1;
                IcdFillter.ORDER_FIELD = "MODIFY_TIME";
                IcdFillter.ORDER_DIRECTION = "DESC";
                IcdFillter.KEY_WORD = txtKeyword2.Text;
                IcdFillter.IDs = this.currentListIcd.Select(o => o.ID).ToList();
                if ((long)cboChoose.EditValue == (long)2)
                {
                    isChoseIcd = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ICD>>(
                   "api/HisIcd/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      IcdFillter,
                    param);

                lstIcdADOs = new List<IcdADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listIcd = sar.Data;
                    foreach (var item in listIcd)
                    {
                        IcdADO roomaccountADO = new IcdADO(item);
                        if (isChoseIcd == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        //roomaccountADO.radio1 = true;
                        lstIcdADOs.Add(roomaccountADO);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceIcds), ServiceIcds));
                if (ServiceIcds != null && ServiceIcds.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcds)
                    {
                        //if (itemUsername.IS_INDICATION == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check1 = true;
                        //}
                        //else if (itemUsername.IS_CONTRAINDICATION == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check2 = true;
                        //    check.CONTRAINDICATION_CONTENT = itemUsername.CONTRAINDICATION_CONTENT;
                        //}
                        //else if (itemUsername.IS_WARNING == 1)
                        //{
                        //    var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        //    check.check3 = true;
                        //}
                        var check = lstIcdADOs.FirstOrDefault(o => o.ICD_CODE == itemUsername.ICD_CODE);
                        if (check != null)
                        {
                            if (itemUsername.IS_INDICATION == 1)
                            {
                                check.check1 = true;
                            }
                            else if (itemUsername.IS_CONTRAINDICATION == 1)
                            {
                                check.check2 = true;
                                check.CONTRAINDICATION_CONTENT = itemUsername.CONTRAINDICATION_CONTENT;
                            }
                            else if (itemUsername.IS_WARNING == 1)
                            {
                                check.check3 = true;
                            }
                            check.MIN_DURATION_STR2 = itemUsername.MIN_DURATION;
                        }
                    }
                }
                lstIcdADOs = lstIcdADOs.OrderByDescending(p => p.check1).ThenByDescending(o => o.check2).ThenByDescending(o => o.check3).Distinct().ToList();

                if (ucGridControlIcd != null)
                {
                    icdProcessor.Reload(ucGridControlIcd, lstIcdADOs);
                }
                rowCount1 = (data == null ? 0 : lstIcdADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCServiceIcd UCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_ById(UCServiceIcd UCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService_ById(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService_ById, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_ByIds(UCServiceIcd UCServiceIcd)
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService_ByIds(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService_ByIds, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if (cboServiceType.EditValue != null)
                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstServiceIcdADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceIcdADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceIcdADO.isKeyChooseService = true;
                        }
                        lstServiceIcdADOs.Add(ServiceIcdADO);
                    }
                }

                param = new CommonParam();
                HisActiveIngredientFilter activeIngredientFilter = new HisActiveIngredientFilter();
                activeIngredientFilter.IS_ACTIVE = 1;
                var currentActiveIngredients = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, activeIngredientFilter, param);
                foreach (var item in currentActiveIngredients)
                {
                    ServiceADO ServiceIcdADO = new ServiceADO();
                    if (isChoseService == 1)
                    {
                        ServiceIcdADO.isKeyChooseService = true;
                    }
                    ServiceIcdADO.SERVICE_CODE = item.ACTIVE_INGREDIENT_CODE;
                    ServiceIcdADO.SERVICE_NAME = item.ACTIVE_INGREDIENT_NAME;
                    ServiceIcdADO.ACTIVE_INGREDIENT_ID = item.ID;
                    lstServiceIcdADOs.Add(ServiceIcdADO);
                }
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                ////List<V_HIS_SERVICE> listSer = BackendDataWorker.Get<V_HIS_SERVICE>();
                ////dataNew = (from r in listSer select new ServiceADO(r)).ToList();
                dataNew = (from r in listService select new ServiceADO(r)).ToList();
                if (ServiceIcdViews != null && ServiceIcdViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcdViews)
                    {
                        if (itemUsername.SERVICE_ID > 0)
                        {
                            var check = lstServiceIcdADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                            if (check != null)
                            {
                                if (itemUsername.IS_INDICATION == 1)
                                {
                                    check.checkService = true;
                                }
                                else if (itemUsername.IS_CONTRAINDICATION == 1)
                                {
                                    check.checkServiceNotUse = true;
                                }
                                else if (itemUsername.IS_WARNING == 1)
                                {
                                    check.checkWarning = true;
                                }
                                check.MIN_DURATION_STR = itemUsername.MIN_DURATION;
                            }
                        }
                        else if (itemUsername.ACTIVE_INGREDIENT_ID > 0)
                        {
                            var check = lstServiceIcdADOs.FirstOrDefault(o => o.ACTIVE_INGREDIENT_ID == itemUsername.ACTIVE_INGREDIENT_ID);
                            if (check != null)
                            {
                                if (itemUsername.IS_INDICATION == 1)
                                {
                                    check.checkService = true;
                                }
                                else if (itemUsername.IS_CONTRAINDICATION == 1)
                                {
                                    check.checkServiceNotUse = true;
                                }
                                else if (itemUsername.IS_WARNING == 1)
                                {
                                    check.checkWarning = true;
                                }
                                check.MIN_DURATION_STR = itemUsername.MIN_DURATION;
                            }
                        }

                    }
                }

                if (lstServiceIcdADOs != null && lstServiceIcdADOs.Count() > 0 && chkIsHoatChat.CheckState == CheckState.Checked)
                {
                    List<HIS_ACTIVE_INGREDIENT> searchActiveIngredient = new List<HIS_ACTIVE_INGREDIENT>();
                    if (!String.IsNullOrWhiteSpace(txtKeyword1.Text))
                    {
                        searchActiveIngredient = currentActiveIngredients.Where(o =>
                                          o.ACTIVE_INGREDIENT_CODE.ToLower().Contains(txtKeyword1.Text.ToLower().Trim())
                                          || o.ACTIVE_INGREDIENT_NAME.ToLower().Contains(txtKeyword1.Text.ToLower().Trim())
                                          ).ToList();
                    }
                    else
                    {
                        searchActiveIngredient = currentActiveIngredients;
                    }

                    lstServiceIcdADOs = (lstServiceIcdADOs != null && searchActiveIngredient != null)
                        ? lstServiceIcdADOs.Where(o =>
                            o.ACTIVE_INGREDIENT_ID.HasValue
                            && searchActiveIngredient.Select(p => p.ID).Contains(o.ACTIVE_INGREDIENT_ID.Value)
                            ).ToList()
                            : lstServiceIcdADOs;

                }

                lstServiceIcdADOs = lstServiceIcdADOs != null ? lstServiceIcdADOs.OrderByDescending(p => p.checkService).ThenByDescending(o => o.checkServiceNotUse).ThenByDescending(o => o.checkWarning).Distinct().ToList() : lstServiceIcdADOs;
                //dataNew = dataNew.OrderByDescending(p => p.checkService).ThenByDescending(i => i.checkServiceNotUse).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceIcdADOs);
                    //ServiceProcessor.Reload(ucGridControlService, dataNew);
                    icdProcessor.Reload(ucGridControlIcd, lstIcdADOs);
                }
                rowCount = (data == null ? 0 : lstServiceIcdADOs.Count);
                //rowCount = (data == null ? 0 : dataNew.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_ById(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                if (this.currentService.ID > 0)
                {
                    CommonParam param = new CommonParam(start, limit);
                    MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                    ServiceFillter.IS_ACTIVE = 1;
                    ServiceFillter.ID = this.currentService.ID;

                    if (cboServiceType.EditValue != null)

                        ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                    if ((long)cboChoose.EditValue == 1)
                    {
                        isChoseService = (long)cboChoose.EditValue;
                    }

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                         "api/HisService/GetView",
                         HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                         ServiceFillter,
                         param);
                    Inventec.Common.Logging.LogSystem.Debug("rs 2" + start);
                    lstServiceIcdADOs = new List<ServiceADO>();

                    if (rs != null && rs.Data.Count > 0)
                    {
                        listService = rs.Data;
                        foreach (var item in listService)
                        {
                            ServiceADO ServiceIcdADO = new ServiceADO(item);
                            if (isChoseService == 1)
                            {
                                ServiceIcdADO.isKeyChooseService = true;
                                ServiceIcdADO.radioService = true;
                            }
                            lstServiceIcdADOs.Add(ServiceIcdADO);
                        }
                    }

                    rowCount = (data == null ? 0 : lstServiceIcdADOs.Count);
                    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }
                else if (this.currentService.ACTIVE_INGREDIENT_ID > 0)
                {
                    CommonParam param1 = new CommonParam();
                    HisActiveIngredientFilter activeIngredientFilter = new HisActiveIngredientFilter();
                    activeIngredientFilter.IS_ACTIVE = 1;
                    activeIngredientFilter.ID = this.currentService.ACTIVE_INGREDIENT_ID;
                    var currentActiveIngredients = new BackendAdapter(param1)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, activeIngredientFilter, param1);
                    foreach (var item in currentActiveIngredients)
                    {
                        ServiceADO ServiceIcdADO = new ServiceADO();
                        if (isChoseService == 1)
                        {
                            ServiceIcdADO.isKeyChooseService = true;
                            ServiceIcdADO.radioService = true;
                        }
                        ServiceIcdADO.SERVICE_CODE = item.ACTIVE_INGREDIENT_CODE;
                        ServiceIcdADO.SERVICE_NAME = item.ACTIVE_INGREDIENT_NAME;
                        ServiceIcdADO.ACTIVE_INGREDIENT_ID = item.ID;
                        lstServiceIcdADOs.Add(ServiceIcdADO);
                    }

                    rowCount = (currentActiveIngredients == null ? 0 : lstServiceIcdADOs.Count);
                    dataTotal = (currentActiveIngredients == null ? 0 : lstServiceIcdADOs.Count);
                }

                if (ServiceIcdViews != null && ServiceIcdViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcdViews)
                    {
                        var check = lstServiceIcdADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID || o.ACTIVE_INGREDIENT_ID == itemUsername.ACTIVE_INGREDIENT_ID);
                        if (check != null)
                        {
                            if (itemUsername.IS_INDICATION == 1)
                            {
                                check.checkService = true;
                            }
                            else if (itemUsername.IS_CONTRAINDICATION == 1)
                            {
                                check.checkServiceNotUse = true;
                            }
                            else if (itemUsername.IS_WARNING == 1)
                            {
                                check.checkWarning = true;
                            }
                            check.MIN_DURATION_STR = itemUsername.MIN_DURATION;
                        }
                    }
                }

                lstServiceIcdADOs = lstServiceIcdADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceIcdADOs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_ByIds(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                ServiceFillter.IS_ACTIVE = 1;
                if (cboServiceType.EditValue != null)
                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                ServiceFillter.IDs = this.currentListServices;



                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                  "api/HisService/GetView",
                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                  ServiceFillter,
                  param);
                Inventec.Common.Logging.LogSystem.Debug("rs 3" + start);
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                lstServiceIcdADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceIcdADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceIcdADO.isKeyChooseService = true;
                            if (this.currentListIcd == null && this.currentListServices == null)
                                ServiceIcdADO.checkService = true;
                        }
                        lstServiceIcdADOs.Add(ServiceIcdADO);
                    }
                }

                CommonParam param1 = new CommonParam();
                HisActiveIngredientFilter activeIngredientFilter = new HisActiveIngredientFilter();
                activeIngredientFilter.IS_ACTIVE = 1;
                activeIngredientFilter.IDs = currentListActiveIngredients;
                var currentActiveIngredients = new BackendAdapter(param1)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>>("api/HisActiveIngredient/Get", ApiConsumers.MosConsumer, activeIngredientFilter, param1);
                foreach (var item in currentActiveIngredients)
                {
                    ServiceADO ServiceIcdADO = new ServiceADO();
                    if (isChoseService == 1)
                    {
                        ServiceIcdADO.isKeyChooseService = true;
                        if (this.currentListIcd == null && this.currentListActiveIngredients == null)
                            ServiceIcdADO.checkService = true;
                    }
                    ServiceIcdADO.SERVICE_CODE = item.ACTIVE_INGREDIENT_CODE;
                    ServiceIcdADO.SERVICE_NAME = item.ACTIVE_INGREDIENT_NAME;
                    ServiceIcdADO.ACTIVE_INGREDIENT_ID = item.ID;
                    lstServiceIcdADOs.Add(ServiceIcdADO);
                }

                if (ServiceIcdViews != null && ServiceIcdViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceIcdViews)
                    {
                        var check = lstServiceIcdADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID || o.ACTIVE_INGREDIENT_ID == itemUsername.ACTIVE_INGREDIENT_ID);
                        if (check != null)
                        {
                            if (itemUsername.IS_INDICATION == 1)
                            {
                                check.checkService = true;
                            }
                            else if (itemUsername.IS_CONTRAINDICATION == 1)
                            {
                                check.checkServiceNotUse = true;
                            }
                            else if (itemUsername.IS_WARNING == 1)
                            {
                                check.checkWarning = true;
                            }
                            check.MIN_DURATION_STR = itemUsername.MIN_DURATION;
                        }
                    }
                }

                if (lstServiceIcdADOs != null && lstServiceIcdADOs.Count() > 0 && chkIsHoatChat.CheckState == CheckState.Checked)
                {
                    List<HIS_ACTIVE_INGREDIENT> searchActiveIngredient = new List<HIS_ACTIVE_INGREDIENT>();
                    if (!String.IsNullOrWhiteSpace(txtKeyword1.Text))
                    {
                        searchActiveIngredient = currentActiveIngredients.Where(o =>
                                          o.ACTIVE_INGREDIENT_CODE.ToLower().Contains(txtKeyword1.Text.ToLower().Trim())
                                          || o.ACTIVE_INGREDIENT_NAME.ToLower().Contains(txtKeyword1.Text.ToLower().Trim())
                                          ).ToList();
                    }
                    else
                    {
                        searchActiveIngredient = currentActiveIngredients;
                    }

                    lstServiceIcdADOs = (lstServiceIcdADOs != null && searchActiveIngredient != null)
                        ? lstServiceIcdADOs.Where(o =>
                            o.ACTIVE_INGREDIENT_ID.HasValue
                            && searchActiveIngredient.Select(p => p.ID).Contains(o.ACTIVE_INGREDIENT_ID.Value)
                            ).ToList()
                            : lstServiceIcdADOs;


                }
                lstIcdADOs.OrderByDescending(p => p.check1).ThenByDescending(o => o.check2).ThenByDescending(o => o.check3).Distinct().ToList();
                lstServiceIcdADOs = lstServiceIcdADOs != null ? lstServiceIcdADOs.OrderByDescending(p => p.checkService).ThenByDescending(p => p.checkServiceNotUse).ThenByDescending(p => p.checkWarning).Distinct().ToList() : lstServiceIcdADOs;
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceIcdADOs);
                }
                rowCount = (data == null ? 0 : lstServiceIcdADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTypeFilter ServiceTypeFilter = new HisServiceTypeFilter();
                ServiceType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TYPE>>(
                             "api/HisServiceType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceTypeFilter,
                    param);

                LoadDataToComboServiceType(cboServiceType, ServiceType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Dịch vụ/thuốc/hoạt chất"));
                status.Add(new Status(2, "Icd"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_SERVICE_TYPE> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentListIcd != null)
                {
                    FillDataToGrid1_ByIds(this);
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.currentListIcd != null && (this.currentListServices != null || this.currentListActiveIngredients != null))
                {
                    FillDataToGrid2_ByIds(this);
                }
                else
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                ServiceIcdViews = null;
                ServiceIcds = null;
                isChoseIcd = 0;
                isChoseService = 0;
                if (this.currentListIcd != null && this.currentListServices != null)
                {
                    FillDataToGrid1_ByIds(this);
                    FillDataToGrid2_ByIds(this);
                }
                else
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
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
                if (ucGridControlIcd != null && ucGridControlService != null)
                {
                    object Machine = icdProcessor.GetDataGridView(ucGridControlIcd);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseService == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceIdCheckByService), ServiceIdCheckByService) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acIngrIdCheckByService), acIngrIdCheckByService));

                        if (Machine is List<HIS.UC.ListIcd.IcdADO>)
                        {
                            lstIcdADOs = (List<HIS.UC.ListIcd.IcdADO>)Machine;

                            if (lstIcdADOs != null && lstIcdADOs.Count > 0)
                            {
                                //List<long> listServiceIcds = ServiceIcds.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstIcdADOs.Where(p => p.check1 == true || p.check2 || p.check3).ToList();

                                //List xoa
                                List<HIS.UC.ListIcd.IcdADO> dataDeletes = new List<IcdADO>();
                                List<HIS.UC.ListIcd.IcdADO> dataCreates = new List<IcdADO>();

                                ServiceADO dataServiceADO = new ServiceADO()
                                    {
                                        ID = (ServiceIdCheckByService ?? 0),
                                        ACTIVE_INGREDIENT_ID = acIngrIdCheckByService
                                    };
                                this.ServiceIcds = GetIcdServicePreSaveByService(dataServiceADO);
                                if (this.ServiceIcds != null && this.ServiceIcds.Count > 0)
                                {
                                    dataDeletes = lstIcdADOs.Where(o => this.ServiceIcds.Select(p => p.ICD_CODE)
                                        .Contains(o.ICD_CODE) && o.check1 == false && o.check2 == false && o.check3 == false).ToList();

                                    dataCreates = dataCheckeds.Where(o => !this.ServiceIcds.Select(p => p.ICD_CODE)
                                    .Contains(o.ICD_CODE)).ToList();
                                }
                                else
                                {
                                    dataCreates = dataCheckeds;
                                }
                                if (dataDeletes != null && dataDeletes.Count == 0 && dataCreates != null && dataCreates.Count == 0 && (ServiceIcds == null || ServiceIcds.Count == 0))
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn ICD", "Thông báo");
                                    return;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = this.ServiceIcds.Where(o => dataDeletes.Select(p => p.ICD_CODE)
                                        .Contains(o.ICD_CODE)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisIcdService/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    this.ServiceIcds = this.ServiceIcds.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_ICD_SERVICE> ServiceIcdCreates = new List<HIS_ICD_SERVICE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_ICD_SERVICE ServiceIcd = new HIS_ICD_SERVICE();
                                        ServiceIcd.SERVICE_ID = acIngrIdCheckByService > 0 ? null : ServiceIdCheckByService;
                                        ServiceIcd.ACTIVE_INGREDIENT_ID = acIngrIdCheckByService;
                                        if (item.check1)
                                        {
                                            ServiceIcd.IS_INDICATION = 1;
                                        }
                                        else
                                        {
                                            ServiceIcd.IS_INDICATION = null;
                                        }
                                        if (item.check2)
                                        {
                                            ServiceIcd.IS_CONTRAINDICATION = 1;
                                        }
                                        else
                                        {
                                            ServiceIcd.IS_CONTRAINDICATION = null;
                                        }
                                        if (item.check3)
                                        {
                                            ServiceIcd.IS_WARNING = 1;
                                        }
                                        else
                                        {
                                            ServiceIcd.IS_WARNING = null;
                                        }
                                        ServiceIcd.ICD_CODE = item.ICD_CODE;
                                        ServiceIcd.ICD_NAME = item.ICD_NAME;
                                        ServiceIcd.CONTRAINDICATION_CONTENT = item.CONTRAINDICATION_CONTENT;
                                        ServiceIcd.MIN_DURATION = item.MIN_DURATION_STR2;
                                        ServiceIcdCreates.Add(ServiceIcd);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_ICD_SERVICE>>(
                                               "api/HisIcdService/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceIcdCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_ICD_SERVICE, HIS_ICD_SERVICE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_ICD_SERVICE>, List<HIS_ICD_SERVICE>>(createResult);
                                    ServiceIcds.AddRange(vCreateResults);
                                }
                                else
                                {
                                    if (ServiceIcds != null && ServiceIcds.Count > 0)
                                    {
                                        List<HIS_ICD_SERVICE> ServiceIcdCreates = new List<HIS_ICD_SERVICE>();
                                        foreach (var item in dataCheckeds)
                                        {
                                            HIS_ICD_SERVICE ServiceIcd = new HIS_ICD_SERVICE();
                                            ServiceIcd.SERVICE_ID = acIngrIdCheckByService > 0 ? null : ServiceIdCheckByService;
                                            ServiceIcd.ACTIVE_INGREDIENT_ID = acIngrIdCheckByService;
                                            var service = ServiceIcds.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE);
                                            if (service != null)
                                            {
                                                ServiceIcd.ID = service.ID;
                                            }
                                            if (item.check1)
                                            {
                                                ServiceIcd.IS_INDICATION = 1;
                                            }
                                            else
                                            {
                                                ServiceIcd.IS_INDICATION = null;
                                            }
                                            if (item.check2)
                                            {
                                                ServiceIcd.IS_CONTRAINDICATION = 1;
                                            }
                                            else
                                            {
                                                ServiceIcd.IS_CONTRAINDICATION = null;
                                            }
                                            if (item.check3)
                                            {
                                                ServiceIcd.IS_WARNING = 1;
                                            }
                                            else
                                            {
                                                ServiceIcd.IS_WARNING = null;
                                            }
                                            ServiceIcd.ICD_CODE = item.ICD_CODE;
                                            ServiceIcd.ICD_NAME = item.ICD_NAME;
                                            ServiceIcd.CONTRAINDICATION_CONTENT = item.CONTRAINDICATION_CONTENT;
                                            ServiceIcd.MIN_DURATION = item.MIN_DURATION_STR2;
                                            ServiceIcdCreates.Add(ServiceIcd);
                                        }
                                        var createResult = new BackendAdapter(param).Post<List<HIS_ICD_SERVICE>>(
                                                   "api/HisIcdService/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   ServiceIcdCreates,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            success = true;
                                    }
                                }
                                lstIcdADOs = lstIcdADOs.OrderByDescending(p => p.check1).ThenByDescending(o => o.check2).ThenByDescending(o => o.check3).ToList();
                                if (ucGridControlIcd != null)
                                {
                                    icdProcessor.Reload(ucGridControlIcd, lstIcdADOs);
                                }
                            }
                        }
                    }
                    if (isChoseIcd == 2)
                    {
                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstServiceIcdADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstServiceIcdADOs != null && lstServiceIcdADOs.Count > 0)
                            {
                                var dataChecked = lstServiceIcdADOs.Where(p => p.checkService == true || p.checkServiceNotUse == true || p.checkWarning == true).ToList();
                                //List xoa
                                List<HIS.UC.Service.ServiceADO> dataDelete = new List<ServiceADO>();
                                List<HIS.UC.Service.ServiceADO> dataCreate = new List<ServiceADO>();

                                if (ServiceIcdViews != null && ServiceIcdViews.Count > 0)
                                {
                                    dataDelete = lstServiceIcdADOs.Where(o => (ServiceIcdViews.Where(t => t.SERVICE_ID > 0).Select(p => p.SERVICE_ID)
                                        .Contains(o.ID) || ServiceIcdViews.Where(t => t.ACTIVE_INGREDIENT_ID > 0).Select(p => p.ACTIVE_INGREDIENT_ID)
                                        .Contains(o.ACTIVE_INGREDIENT_ID)) && o.checkService == false && o.checkServiceNotUse == false && o.checkWarning == false).ToList();

                                    //list them
                                    dataCreate = dataChecked.Where(o => !ServiceIcdViews.Where(i => i.SERVICE_ID != null).Select(p => p.SERVICE_ID)
                                        .Contains(o.ID) && !ServiceIcdViews.Where(m => m.ACTIVE_INGREDIENT_ID != null).Select(p => p.ACTIVE_INGREDIENT_ID)
                                        .Contains(o.ACTIVE_INGREDIENT_ID)).ToList();

                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataChecked), dataChecked.Count));
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataChecked), dataChecked));
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceIcdViews), ServiceIcdViews.Count));

                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceIcdViews), ServiceIcdViews));
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCreate), dataCreate.Count));
                                }
                                else
                                {
                                    dataCreate = dataChecked;
                                }
                                if (dataDelete != null && dataDelete.Count == 0 && dataCreate != null && dataCreate.Count == 0 && (ServiceIcdViews == null || ServiceIcdViews.Count == 0))
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    List<long> deleteId = ServiceIcdViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID ?? 0)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisIcdService/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceIcdViews = ServiceIcdViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_ICD_SERVICE> ServiceIcdCreate = new List<HIS_ICD_SERVICE>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_ICD_SERVICE ServiceIcdID = new HIS_ICD_SERVICE();
                                        ServiceIcdID.ICD_CODE = IcdCodeCheckByIcd;
                                        ServiceIcdID.ICD_NAME = IcdNameCheckByIcd;
                                        if (item.checkService)
                                        {
                                            ServiceIcdID.IS_INDICATION = 1;
                                        }
                                        else
                                        {
                                            ServiceIcdID.IS_INDICATION = null;
                                        }
                                        if (item.checkServiceNotUse)
                                        {
                                            ServiceIcdID.IS_CONTRAINDICATION = 1;
                                        }
                                        else
                                        {
                                            ServiceIcdID.IS_CONTRAINDICATION = null;
                                        }
                                        if (item.checkWarning)
                                        {
                                            ServiceIcdID.IS_WARNING = 1;
                                        }
                                        else
                                        {
                                            ServiceIcdID.IS_WARNING = null;
                                        }
                                        ServiceIcdID.MIN_DURATION = item.MIN_DURATION_STR;
                                        ServiceIcdID.SERVICE_ID = item.ACTIVE_INGREDIENT_ID > 0 ? null : (long?)item.ID;
                                        ServiceIcdID.ACTIVE_INGREDIENT_ID = item.ACTIVE_INGREDIENT_ID;
                                        ServiceIcdCreate.Add(ServiceIcdID);
                                    }
                                    var createResult = new BackendAdapter(param).Post<List<HIS_ICD_SERVICE>>(
                                               "api/HisIcdService/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceIcdCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_ICD_SERVICE, HIS_ICD_SERVICE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_ICD_SERVICE>, List<HIS_ICD_SERVICE>>(createResult);
                                    ServiceIcdViews.AddRange(vCreateResults);
                                }
                                else
                                {
                                    if (ServiceIcdViews != null && ServiceIcdViews.Count > 0)
                                    {
                                        List<HIS_ICD_SERVICE> ServiceIcdCreate = new List<HIS_ICD_SERVICE>();

                                        foreach (var item in dataChecked)
                                        {
                                            HIS_ICD_SERVICE ServiceIcdID = new HIS_ICD_SERVICE();
                                            if (item.ACTIVE_INGREDIENT_ID > 0)
                                            {
                                                var service = ServiceIcdViews.FirstOrDefault(o => o.ACTIVE_INGREDIENT_ID == item.ACTIVE_INGREDIENT_ID);
                                                if (service != null)
                                                {
                                                    ServiceIcdID.ID = service.ID;
                                                }
                                            }
                                            else if (item.ID > 0)
                                            {
                                                var service = ServiceIcdViews.FirstOrDefault(o => o.SERVICE_ID == item.ID);
                                                if (service != null)
                                                {
                                                    ServiceIcdID.ID = service.ID;
                                                }
                                            }

                                            ServiceIcdID.ICD_CODE = IcdCodeCheckByIcd;
                                            ServiceIcdID.ICD_NAME = IcdNameCheckByIcd;
                                            if (item.checkService)
                                            {
                                                ServiceIcdID.IS_INDICATION = 1;
                                            }
                                            else
                                            {
                                                ServiceIcdID.IS_INDICATION = null;
                                            }
                                            if (item.checkServiceNotUse)
                                            {
                                                ServiceIcdID.IS_CONTRAINDICATION = 1;
                                            }
                                            else
                                            {
                                                ServiceIcdID.IS_CONTRAINDICATION = null;
                                            }
                                            if (item.checkWarning)
                                            {
                                                ServiceIcdID.IS_WARNING = 1;
                                            }
                                            else
                                            {
                                                ServiceIcdID.IS_WARNING = null;
                                            }
                                            ServiceIcdID.MIN_DURATION = item.MIN_DURATION_STR;
                                            ServiceIcdID.SERVICE_ID = item.ACTIVE_INGREDIENT_ID > 0 ? null : (long?)item.ID;
                                            ServiceIcdID.ACTIVE_INGREDIENT_ID = item.ACTIVE_INGREDIENT_ID;
                                            ServiceIcdCreate.Add(ServiceIcdID);
                                        }
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceIcdCreate), ServiceIcdCreate));
                                        var createResult = new BackendAdapter(param).Post<List<HIS_ICD_SERVICE>>(
                                                   "api/HisIcdService/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   ServiceIcdCreate,
                                                   param);
                                        if (createResult != null && createResult.Count > 0)
                                            success = true;
                                    }

                                }

                                lstServiceIcdADOs = lstServiceIcdADOs.OrderByDescending(p => p.checkService).ThenByDescending(o => o.checkServiceNotUse).ThenByDescending(o => o.checkWarning).ToList();
                                if (ucGridControlIcd != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, lstServiceIcdADOs);
                                }
                            }
                        }
                    }
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

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    if ((this.currentListServices != null || this.currentListActiveIngredients != null) && this.currentListIcd != null)
                    {
                        FillDataToGrid1_ByIds(this);
                    }
                    else
                    {
                        FillDataToGrid1(this);
                    }

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.currentListIcd != null && this.currentListServices != null)
                    {
                        FillDataToGrid2_ByIds(this);
                    }
                    else
                    {
                        FillDataToGrid2(this);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
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

        public void ImportShortcut()
        {
            try
            {
                btnImport.Focus();
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboServiceType.Focus();
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyword2.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        HIS_SERVICE_TYPE data = ServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            btnFind1.Focus();
                            if (data.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                chkIsHoatChat.Enabled = true;
                            }
                            else
                            {
                                chkIsHoatChat.Enabled = false;
                                chkIsHoatChat.CheckState = CheckState.Unchecked;
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

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.Properties.Buttons[1].Visible = false;
                    cboServiceType.EditValue = null;
                    chkIsHoatChat.CheckState = CheckState.Unchecked;
                    chkIsHoatChat.Enabled = false;
                }

                HisServiceTypeFilter filter = new HisServiceTypeFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    HIS_SERVICE_TYPE data = ServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                    if (data != null)
                    {
                        cboServiceType.Properties.Buttons[1].Visible = true;
                        btnFind1.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!btnImport.Enabled)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImportIcdService").FirstOrDefault();

                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisImportIcdService'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    //listArgs.Add((RefeshReference)FillDataToGridControl);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(L.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnOkContraindicationContent_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate(mmContraindicationContent))
                    return;
                if (this.currentIcdADO_Processing == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("currentIcdADO_Processing is null");
                }
                else
                {
                    UpdateDataUcGridControlIcd_CONTRAINDICATION_CONTENT(this.currentIcdADO_Processing.ID, mmContraindicationContent.Text);
                }

                mmContraindicationContent.Text = "";
                popupContainerControlContraindicationContent.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataUcGridControlIcd_CONTRAINDICATION_CONTENT(long IcdID, string ContraindicationContent)
        {
            try
            {
                object dataGridView = icdProcessor.GetDataGridView(ucGridControlIcd);
                if (dataGridView is List<HIS.UC.ListIcd.IcdADO>)
                {
                    var dataIcdADOs = (List<HIS.UC.ListIcd.IcdADO>)dataGridView;
                    if (dataIcdADOs != null && dataIcdADOs.Count() > 0)
                    {
                        var data = dataIcdADOs.FirstOrDefault(o => o.ID == IcdID);
                        if (data != null)
                        {
                            data.CONTRAINDICATION_CONTENT = ContraindicationContent;
                        }
                        //Reload Data ucGridControlIcd
                        if (ucGridControlIcd != null)
                        {
                            icdProcessor.Reload(ucGridControlIcd, dataIcdADOs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancelContraindicationContent_Click(object sender, EventArgs e)
        {
            try
            {
                mmContraindicationContent.Text = "";
                popupContainerControlContraindicationContent.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}







