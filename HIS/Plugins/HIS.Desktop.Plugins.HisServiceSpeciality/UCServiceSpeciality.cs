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
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisServiceSpeciality.Entity;
using HIS.Desktop.Plugins.HisServiceSpeciality.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using HIS.UC.Speciality;
using HIS.UC.Speciality.ADO;

namespace HIS.Desktop.Plugins.HisServiceSpeciality
{
    public partial class UCServiceSpeciality : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        List<HIS_SERVICE_TYPE> serviceTypes;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        SpecialityProcessor specialityProcessor;
        UCServiceProcessor serviceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlSpeciality;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Speciality.SpecialityADO> lstSpecialityADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstServiceADOs { get; set; }
        List<HIS_SPECIALITY> listSpeciality;
        List<V_HIS_SERVICE> listService;

        /// <summary>ID dich vu dang chon radio (mode Chon theo Dich vu)</summary>
        long serviceIdChecked = 0;
        /// <summary>ID pham vi chuyen mon dang chon radio (mode Chon theo Pham vi chuyen mon)</summary>
        long specialityIdChecked = 0;
        /// <summary>= (long)EnumChooseBy.Service khi combo Chon theo = Dich vu</summary>
        long isChoseService;
        /// <summary>= (long)EnumChooseBy.Speciality khi combo Chon theo = Pham vi chuyen mon</summary>
        long isChoseSpeciality;
        bool isCheckAll;

        /// <summary>Danh sach map hien tai cua dich vu dang chon radio (nen so diff khi Luu)</summary>
        List<HIS_SERVICE_SPECIALITY> serviceSpecialitiesByService { get; set; }
        /// <summary>Danh sach map hien tai cua pham vi chuyen mon dang chon radio (nen so diff khi Luu)</summary>
        List<HIS_SERVICE_SPECIALITY> serviceSpecialitiesBySpeciality { get; set; }
        V_HIS_SERVICE currentService;
        #endregion

        #region Constructor
        public UCServiceSpeciality(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
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

        public UCServiceSpeciality(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_SERVICE serviceData)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
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
        #endregion

        #region Load
        private void UCServiceSpeciality_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService == null)
                {
                    FillDataToGridService(this);
                    FillDataToGridSpeciality(this);
                }
                else
                {
                    FillDataToGrid1_Service(this);
                    FillDataToGridSpeciality(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisServiceSpeciality.Resources.Lang",
                    typeof(UCServiceSpeciality).Assembly);

                this.btnFind1.Text = GetLang("UCServiceSpeciality.btnFind1.Text");
                this.btnFind2.Text = GetLang("UCServiceSpeciality.btnFind2.Text");
                this.btnSave.Text = GetLang("UCServiceSpeciality.btnSave.Text");
                this.layoutControlItem3.Text = GetLang("UCServiceSpeciality.lciServiceType.Text");
                this.layoutControlItem2.Text = GetLang("UCServiceSpeciality.lciChoose.Text");
                this.txtKeyword1.Properties.NullValuePrompt = GetLang("UCServiceSpeciality.txtKeyword1.Properties.NullValuePrompt");
                this.txtKeyword2.Properties.NullValuePrompt = GetLang("UCServiceSpeciality.txtKeyword2.Properties.NullValuePrompt");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadDataToCombo()
        {
            try
            {
                serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.SERVICE_TYPE_CODE)
                    .ToList();
                LoadDataToComboServiceType(cboServiceType, serviceTypes);
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
                status.Add(new Status((long)EnumChooseBy.Service, GetLang("UCServiceSpeciality.cboChoose.Service")));
                status.Add(new Status((long)EnumChooseBy.Speciality, GetLang("UCServiceSpeciality.cboChoose.Speciality")));

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

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cbo, List<HIS_SERVICE_TYPE> data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("SERVICE_TYPE_CODE");
                aColumnCode.Caption = GetLang("UCServiceSpeciality.cboServiceType.ColCode");
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                aColumnName.Caption = GetLang("UCServiceSpeciality.cboServiceType.ColName");
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Init 2 grid UC
        private void InitUcgrid1()
        {
            try
            {
                serviceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<HIS.UC.Service.ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewService_MouseDown;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;

                HIS.UC.Service.ServiceColumn colRadio = new HIS.UC.Service.ServiceColumn("   ", "radioService", 30, true);
                colRadio.VisibleIndex = 0;
                colRadio.Visible = false;
                colRadio.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio);

                HIS.UC.Service.ServiceColumn colCheck = new HIS.UC.Service.ServiceColumn("   ", "checkService", 30, true);
                colCheck.VisibleIndex = 1;
                colCheck.image = imageCollectionService.Images[0];
                colCheck.Visible = false;
                colCheck.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck);

                HIS.UC.Service.ServiceColumn colServiceCode = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceSpeciality.colServiceCode.Caption"), "SERVICE_CODE", 60, false);
                colServiceCode.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colServiceCode);

                HIS.UC.Service.ServiceColumn colServiceName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceSpeciality.colServiceName.Caption"), "SERVICE_NAME", 300, false);
                colServiceName.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colServiceName);

                HIS.UC.Service.ServiceColumn colServiceTypeName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceSpeciality.colServiceTypeName.Caption"), "SERVICE_TYPE_NAME", 80, false);
                colServiceTypeName.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colServiceTypeName);

                this.ucGridControlService = (UserControl)serviceProcessor.Run(ado);
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

        private void InitUcgrid2()
        {
            try
            {
                specialityProcessor = new SpecialityProcessor();
                SpecialityInitADO ado = new SpecialityInitADO();
                ado.ListSpecialityColumn = new List<HIS.UC.Speciality.SpecialityColumn>();
                ado.GridViewSpeciality_MouseDown = gridViewSpeciality_MouseDown;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;

                HIS.UC.Speciality.SpecialityColumn colRadio = new HIS.UC.Speciality.SpecialityColumn("   ", "radio1", 30, true);
                colRadio.VisibleIndex = 0;
                colRadio.Visible = false;
                colRadio.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListSpecialityColumn.Add(colRadio);

                HIS.UC.Speciality.SpecialityColumn colCheck = new HIS.UC.Speciality.SpecialityColumn("   ", "check1", 30, true);
                colCheck.VisibleIndex = 1;
                colCheck.image = imageCollectionRoom.Images[0];
                colCheck.Visible = false;
                colCheck.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListSpecialityColumn.Add(colCheck);

                HIS.UC.Speciality.SpecialityColumn colSpecialityCode = new HIS.UC.Speciality.SpecialityColumn(GetLang("UCServiceSpeciality.colSpecialityCode.Caption"), "SPECIALITY_CODE", 80, false);
                colSpecialityCode.VisibleIndex = 2;
                ado.ListSpecialityColumn.Add(colSpecialityCode);

                HIS.UC.Speciality.SpecialityColumn colSpecialityName = new HIS.UC.Speciality.SpecialityColumn(GetLang("UCServiceSpeciality.colSpecialityName.Caption"), "SPECIALITY_NAME", 300, false);
                colSpecialityName.VisibleIndex = 3;
                ado.ListSpecialityColumn.Add(colSpecialityName);

                this.ucGridControlSpeciality = (UserControl)specialityProcessor.Run(ado);
                if (ucGridControlSpeciality != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlSpeciality);
                    this.ucGridControlSpeciality.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Check all (click header cot checkbox)
        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseService == (long)EnumChooseBy.Service)
                {
                    return;
                }

                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column && hi.Column.FieldName == "checkService")
                    {
                        var lstCheckAll = lstServiceADOs;
                        if (lstCheckAll != null && lstCheckAll.Count > 0)
                        {
                            WaitingManager.Show();
                            var checkedNum = lstCheckAll.Count(o => o.checkService);
                            if (checkedNum < lstCheckAll.Count)
                            {
                                isCheckAll = true;
                                hi.Column.Image = imageCollectionService.Images[1];
                            }
                            else
                            {
                                isCheckAll = false;
                                hi.Column.Image = imageCollectionService.Images[0];
                            }

                            foreach (var item in lstCheckAll)
                            {
                                item.checkService = isCheckAll;
                            }
                            isCheckAll = !isCheckAll;

                            serviceProcessor.Reload(ucGridControlService, lstCheckAll);
                            WaitingManager.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSpeciality_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseSpeciality == (long)EnumChooseBy.Speciality)
                {
                    return;
                }

                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column && hi.Column.FieldName == "check1")
                    {
                        var lstCheckAll = lstSpecialityADOs;
                        if (lstCheckAll != null && lstCheckAll.Count > 0)
                        {
                            WaitingManager.Show();
                            var checkedNum = lstCheckAll.Count(o => o.check1);
                            if (checkedNum < lstCheckAll.Count)
                            {
                                isCheckAll = true;
                                hi.Column.Image = imageCollectionRoom.Images[1];
                            }
                            else
                            {
                                isCheckAll = false;
                                hi.Column.Image = imageCollectionRoom.Images[0];
                            }

                            foreach (var item in lstCheckAll)
                            {
                                item.check1 = isCheckAll;
                            }
                            isCheckAll = !isCheckAll;

                            specialityProcessor.Reload(ucGridControlSpeciality, lstCheckAll);
                            WaitingManager.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Radio click -> load map + auto check
        /// <summary>
        /// Mode Chon theo Dich vu: tick radio 1 dich vu -> load map tu api/HisServiceSpeciality/Get (SERVICE_ID)
        /// -> auto check cac pham vi chuyen mon da gan ben grid phai.
        /// </summary>
        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ADO.HisServiceSpecialityFilter filter = new ADO.HisServiceSpecialityFilter();
                filter.SERVICE_ID = data.ID;
                serviceIdChecked = data.ID;

                serviceSpecialitiesByService = new BackendAdapter(param).Get<List<HIS_SERVICE_SPECIALITY>>(
                    HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param) ?? new List<HIS_SERVICE_SPECIALITY>();

                List<HIS.UC.Speciality.SpecialityADO> dataNew = (from r in (listSpeciality ?? new List<HIS_SPECIALITY>())
                                                                 select new HIS.UC.Speciality.SpecialityADO(r)).ToList();
                if (serviceSpecialitiesByService.Count > 0)
                {
                    HashSet<long> mappedSpecialityIds = new HashSet<long>(serviceSpecialitiesByService.Select(o => o.SPECIALITY_ID));
                    foreach (var item in dataNew)
                    {
                        if (mappedSpecialityIds.Contains(item.ID))
                        {
                            item.check1 = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                lstSpecialityADOs = dataNew;
                if (ucGridControlSpeciality != null)
                {
                    specialityProcessor.Reload(ucGridControlSpeciality, dataNew);
                }
                else
                {
                    FillDataToGridSpeciality(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mode Chon theo Pham vi chuyen mon: tick radio 1 PVCM -> load map tu api/HisServiceSpeciality/Get (SPECIALITY_ID)
        /// -> auto check cac dich vu da gan ben grid trai.
        /// </summary>
        private void btn_Radio_Enable_Click(HIS_SPECIALITY data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ADO.HisServiceSpecialityFilter filter = new ADO.HisServiceSpecialityFilter();
                filter.SPECIALITY_ID = data.ID;
                specialityIdChecked = data.ID;

                serviceSpecialitiesBySpeciality = new BackendAdapter(param).Get<List<HIS_SERVICE_SPECIALITY>>(
                    HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param) ?? new List<HIS_SERVICE_SPECIALITY>();

                List<HIS.UC.Service.ServiceADO> dataNew = (from r in (listService ?? new List<V_HIS_SERVICE>())
                                                           select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (serviceSpecialitiesBySpeciality.Count > 0)
                {
                    HashSet<long> mappedServiceIds = new HashSet<long>(serviceSpecialitiesBySpeciality.Select(o => o.SERVICE_ID));
                    foreach (var item in dataNew)
                    {
                        if (mappedServiceIds.Contains(item.ID))
                        {
                            item.checkService = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();
                lstServiceADOs = dataNew;
                if (ucGridControlService != null)
                {
                    serviceProcessor.Reload(ucGridControlService, dataNew);
                }
                else
                {
                    FillDataToGridService(this);
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

        #region Fill grid Pham vi chuyen mon (phai)
        private void FillDataToGridSpeciality(UCServiceSpeciality uc)
        {
            try
            {
                specialityIdChecked = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridSpeciality(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridSpeciality, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridSpeciality(object data)
        {
            try
            {
                WaitingManager.Show();
                listSpeciality = new List<HIS_SPECIALITY>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisSpecialityFilter filter = new HisSpecialityFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword2.Text.Trim();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if ((long)cboChoose.EditValue == (long)EnumChooseBy.Speciality)
                {
                    isChoseSpeciality = (long)cboChoose.EditValue;
                }

                var sar = new BackendAdapter(param).GetRO<List<HIS_SPECIALITY>>(
                    HisRequestUriStore.MOSHIS_SPECIALITY_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstSpecialityADOs = new List<HIS.UC.Speciality.SpecialityADO>();
                if (sar != null && sar.Data != null && sar.Data.Count > 0)
                {
                    listSpeciality = sar.Data;
                    foreach (var item in listSpeciality)
                    {
                        HIS.UC.Speciality.SpecialityADO specialityADO = new HIS.UC.Speciality.SpecialityADO(item);
                        if (isChoseSpeciality == (long)EnumChooseBy.Speciality)
                        {
                            specialityADO.isKeyChoose = true;
                        }
                        lstSpecialityADOs.Add(specialityADO);
                    }
                }

                if (serviceSpecialitiesByService != null && serviceSpecialitiesByService.Count > 0)
                {
                    HashSet<long> mappedSpecialityIds = new HashSet<long>(serviceSpecialitiesByService.Select(o => o.SPECIALITY_ID));
                    foreach (var item in lstSpecialityADOs)
                    {
                        if (mappedSpecialityIds.Contains(item.ID))
                        {
                            item.check1 = true;
                        }
                    }
                }
                lstSpecialityADOs = lstSpecialityADOs.OrderByDescending(p => p.check1).ToList();

                if (ucGridControlSpeciality != null)
                {
                    specialityProcessor.Reload(ucGridControlSpeciality, lstSpecialityADOs);
                }
                rowCount1 = (data == null ? 0 : lstSpecialityADOs.Count);
                dataTotal1 = (sar == null || sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Fill grid Dich vu (trai)
        private void FillDataToGridService(UCServiceSpeciality uc)
        {
            try
            {
                serviceIdChecked = 0;
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

        private void FillDataToGrid1_Service(UCServiceSpeciality uc)
        {
            try
            {
                serviceIdChecked = 0;
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService_Default, param, numPageSize);
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
                MOS.Filter.HisServiceViewFilter filter = new HisServiceViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword1.Text.Trim();

                if (cboServiceType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                }

                if ((long)cboChoose.EditValue == (long)EnumChooseBy.Service)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET_VIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstServiceADOs = new List<HIS.UC.Service.ServiceADO>();
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        HIS.UC.Service.ServiceADO serviceADO = new HIS.UC.Service.ServiceADO(item);
                        if (isChoseService == (long)EnumChooseBy.Service)
                        {
                            serviceADO.isKeyChooseService = true;
                        }
                        lstServiceADOs.Add(serviceADO);
                    }
                }

                if (serviceSpecialitiesBySpeciality != null && serviceSpecialitiesBySpeciality.Count > 0)
                {
                    HashSet<long> mappedServiceIds = new HashSet<long>(serviceSpecialitiesBySpeciality.Select(o => o.SERVICE_ID));
                    foreach (var item in lstServiceADOs)
                    {
                        if (mappedServiceIds.Contains(item.ID))
                        {
                            item.checkService = true;
                        }
                    }
                }

                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    serviceProcessor.Reload(ucGridControlService, lstServiceADOs);
                }
                rowCount = (data == null ? 0 : lstServiceADOs.Count);
                dataTotal = (rs == null || rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter filter = new HisServiceViewFilter();
                filter.ID = this.currentService.ID;

                if (cboServiceType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                }

                if ((long)cboChoose.EditValue == (long)EnumChooseBy.Service)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET_VIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstServiceADOs = new List<HIS.UC.Service.ServiceADO>();
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        HIS.UC.Service.ServiceADO serviceADO = new HIS.UC.Service.ServiceADO(item);
                        if (isChoseService == (long)EnumChooseBy.Service)
                        {
                            serviceADO.isKeyChooseService = true;
                            serviceADO.radioService = true;
                        }
                        lstServiceADOs.Add(serviceADO);
                    }
                }

                if (serviceSpecialitiesBySpeciality != null && serviceSpecialitiesBySpeciality.Count > 0)
                {
                    HashSet<long> mappedServiceIds = new HashSet<long>(serviceSpecialitiesBySpeciality.Select(o => o.SERVICE_ID));
                    foreach (var item in lstServiceADOs)
                    {
                        if (mappedServiceIds.Contains(item.ID))
                        {
                            item.checkService = true;
                            item.radioService = true;
                        }
                    }
                }

                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    serviceProcessor.Reload(ucGridControlService, lstServiceADOs);
                }
                rowCount = (data == null ? 0 : lstServiceADOs.Count);
                dataTotal = (rs == null || rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (ucGridControlSpeciality != null && ucGridControlService != null)
                {
                    object specialityGridData = specialityProcessor.GetDataGridView(ucGridControlSpeciality);
                    object serviceGridData = serviceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    bool hasCall = false;
                    CommonParam param = new CommonParam();

                    if (isChoseService == (long)EnumChooseBy.Service)
                    {
                        if (serviceIdChecked == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                                Resources.ResourceMessage.ChuaChonDichVu,
                                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            return;
                        }
                        if (specialityGridData is List<HIS.UC.Speciality.SpecialityADO>)
                        {
                            lstSpecialityADOs = (List<HIS.UC.Speciality.SpecialityADO>)specialityGridData;

                            if (lstSpecialityADOs != null && lstSpecialityADOs.Count > 0)
                            {
                                if (serviceSpecialitiesByService == null)
                                    serviceSpecialitiesByService = new List<HIS_SERVICE_SPECIALITY>();

                                HashSet<long> mappedSpecialityIds = new HashSet<long>(serviceSpecialitiesByService.Select(p => p.SPECIALITY_ID));
                                var dataCheckeds = lstSpecialityADOs.Where(p => p.check1).ToList();
                                var dataDeletes = lstSpecialityADOs.Where(o => mappedSpecialityIds.Contains(o.ID) && !o.check1).ToList();
                                var dataCreates = dataCheckeds.Where(o => !mappedSpecialityIds.Contains(o.ID)).ToList();

                                if (dataDeletes.Count == 0 && dataCreates.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show(
                                        Resources.ResourceMessage.KhongCoThayDoiDeLuu,
                                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                    return;
                                }
                                success = true;

                                if (dataDeletes.Count > 0)
                                {
                                    hasCall = true;
                                    HashSet<long> deleteSpecialityIds = new HashSet<long>(dataDeletes.Select(p => p.ID));
                                    List<long> deleteIds = serviceSpecialitiesByService
                                        .Where(o => deleteSpecialityIds.Contains(o.SPECIALITY_ID))
                                        .Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                        HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_DELETE_LIST,
                                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                        deleteIds,
                                        param);
                                    if (deleteResult)
                                    {
                                        HashSet<long> deletedIds = new HashSet<long>(deleteIds);
                                        serviceSpecialitiesByService = serviceSpecialitiesByService.Where(o => !deletedIds.Contains(o.ID)).ToList();
                                    }
                                    else
                                    {
                                        success = false;
                                    }
                                }

                                if (dataCreates.Count > 0)
                                {
                                    hasCall = true;
                                    List<HIS_SERVICE_SPECIALITY> creates = new List<HIS_SERVICE_SPECIALITY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_SPECIALITY serviceSpeciality = new HIS_SERVICE_SPECIALITY();
                                        serviceSpeciality.SERVICE_ID = serviceIdChecked;
                                        serviceSpeciality.SPECIALITY_ID = item.ID;
                                        creates.Add(serviceSpeciality);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_SPECIALITY>>(
                                        HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_CREATE_LIST,
                                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                        creates,
                                        param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        serviceSpecialitiesByService.AddRange(createResult);
                                    }
                                    else
                                    {
                                        success = false;
                                    }
                                }

                                lstSpecialityADOs = lstSpecialityADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlSpeciality != null)
                                {
                                    specialityProcessor.Reload(ucGridControlSpeciality, lstSpecialityADOs);
                                }
                            }
                        }
                    }

                    if (isChoseSpeciality == (long)EnumChooseBy.Speciality)
                    {
                        if (specialityIdChecked == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                                Resources.ResourceMessage.ChuaChonPhamViChuyenMon,
                                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            return;
                        }

                        if (serviceGridData is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstServiceADOs = (List<HIS.UC.Service.ServiceADO>)serviceGridData;

                            if (lstServiceADOs != null && lstServiceADOs.Count > 0)
                            {
                                if (serviceSpecialitiesBySpeciality == null)
                                    serviceSpecialitiesBySpeciality = new List<HIS_SERVICE_SPECIALITY>();

                                HashSet<long> mappedServiceIds = new HashSet<long>(serviceSpecialitiesBySpeciality.Select(p => p.SERVICE_ID));
                                var dataCheckeds = lstServiceADOs.Where(p => p.checkService).ToList();
                                var dataDeletes = lstServiceADOs.Where(o => mappedServiceIds.Contains(o.ID) && !o.checkService).ToList();
                                var dataCreates = dataCheckeds.Where(o => !mappedServiceIds.Contains(o.ID)).ToList();

                                if (dataDeletes.Count == 0 && dataCreates.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show(
                                        Resources.ResourceMessage.KhongCoThayDoiDeLuu,
                                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                    return;
                                }
                                success = true;

                                if (dataDeletes.Count > 0)
                                {
                                    hasCall = true;
                                    HashSet<long> deleteServiceIds = new HashSet<long>(dataDeletes.Select(p => p.ID));
                                    List<long> deleteIds = serviceSpecialitiesBySpeciality
                                        .Where(o => deleteServiceIds.Contains(o.SERVICE_ID))
                                        .Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                        HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_DELETE_LIST,
                                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                        deleteIds,
                                        param);
                                    if (deleteResult)
                                    {
                                        HashSet<long> deletedIds = new HashSet<long>(deleteIds);
                                        serviceSpecialitiesBySpeciality = serviceSpecialitiesBySpeciality.Where(o => !deletedIds.Contains(o.ID)).ToList();
                                    }
                                    else
                                    {
                                        success = false;
                                    }
                                }

                                if (dataCreates.Count > 0)
                                {
                                    hasCall = true;
                                    List<HIS_SERVICE_SPECIALITY> creates = new List<HIS_SERVICE_SPECIALITY>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_SPECIALITY serviceSpeciality = new HIS_SERVICE_SPECIALITY();
                                        serviceSpeciality.SPECIALITY_ID = specialityIdChecked;
                                        serviceSpeciality.SERVICE_ID = item.ID;
                                        creates.Add(serviceSpeciality);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_SPECIALITY>>(
                                        HisRequestUriStore.MOSHIS_SERVICE_SPECIALITY_CREATE_LIST,
                                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                        creates,
                                        param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        serviceSpecialitiesBySpeciality.AddRange(createResult);
                                    }
                                    else
                                    {
                                        success = false;
                                    }
                                }

                                lstServiceADOs = lstServiceADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlService != null)
                                {
                                    serviceProcessor.Reload(ucGridControlService, lstServiceADOs);
                                }
                            }
                        }
                    }

                    if (hasCall)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                        SessionManager.ProcessTokenLost(param);
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

        #region Event search/combo
        private void btnFindService_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridService(this);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFindSpeciality_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridSpeciality(this);
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
                serviceSpecialitiesByService = null;
                serviceSpecialitiesBySpeciality = null;
                isChoseSpeciality = 0;
                isChoseService = 0;
                FillDataToGridService(this);
                FillDataToGridSpeciality(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridService(this);
                }
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
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridSpeciality(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind1.Focus();
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        HIS_SERVICE_TYPE data = serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            btnFind1.Focus();
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
                }
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
                if (cboServiceType.EditValue != null && serviceTypes != null)
                {
                    HIS_SERVICE_TYPE data = serviceTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                    if (data != null)
                    {
                        cboServiceType.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        public void FindShortcutService()
        {
            try
            {
                btnFindService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcutSpeciality()
        {
            try
            {
                btnFindSpeciality_Click(null, null);
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

        public void refreshForm()
        {
            btnFindService_Click(null, null);
            btnFindSpeciality_Click(null, null);
        }
        #endregion
    }
}
