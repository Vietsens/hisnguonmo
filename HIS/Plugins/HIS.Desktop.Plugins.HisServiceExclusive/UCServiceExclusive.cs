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
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using HIS.Desktop.Plugins.Library.CheckServiceExclusive;
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO;

namespace HIS.Desktop.Plugins.HisServiceExclusive
{
    /// <summary>
    /// Man danh muc "Dich vu khong duoc chi dinh dong thoi" (viec 57452 / TTMB-TK-56258).
    ///
    /// Luoi TRAI  : chon 1 dich vu goc bang radio.
    /// Luoi PHAI  : tich cac dich vu khong duoc chi dinh cung dich vu goc, chon muc xu ly bang
    ///              2 cot tick loai tru nhau san co cua HIS.UC.Service:
    ///                 checkWarning        -> Canh bao (van cho chi dinh)
    ///                 checkServiceNotUse  -> Chan (khong cho chi dinh)
    /// Quan he la DOI XUNG: chi luu 1 ban ghi cho moi cap, luc doc thi tra 2 chieu.
    /// </summary>
    public partial class UCServiceExclusive : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        List<HIS_SERVICE_TYPE> serviceTypes;
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        UCServiceProcessor serviceProcessor;
        UCServiceProcessor exclusiveProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlExclusive;

        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;

        internal List<ServiceADO> lstServiceADOs { get; set; }
        internal List<ServiceADO> lstExclusiveADOs { get; set; }
        List<V_HIS_SERVICE> listService;
        List<V_HIS_SERVICE> listExclusive;

        /// <summary>ID dich vu goc dang chon radio o luoi trai</summary>
        long serviceIdChecked = 0;

        bool isCheckAll;

        /// <summary>Cac cap loai tru hien co cua dich vu goc dang chon (nen so diff khi Luu)</summary>
        List<HIS_SERVICE_EXCLUSIVE> serviceExclusivesByService { get; set; }

        V_HIS_SERVICE currentService;
        #endregion

        #region Constructor
        public UCServiceExclusive(Inventec.Desktop.Common.Modules.Module currentModule)
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

        public UCServiceExclusive(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_SERVICE serviceData)
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
        private void UCServiceExclusive_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();

                if (this.currentService == null)
                {
                    FillDataToGridService(this);
                }
                else
                {
                    FillDataToGrid1_Service(this);
                }
                FillDataToGridExclusive(this);

                if (this.currentService != null)
                {
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
                    "HIS.Desktop.Plugins.HisServiceExclusive.Resources.Lang",
                    typeof(UCServiceExclusive).Assembly);

                this.btnFind1.Text = GetLang("UCServiceExclusive.btnFind1.Text");
                this.btnFind2.Text = GetLang("UCServiceExclusive.btnFind2.Text");
                this.btnSave.Text = GetLang("UCServiceExclusive.btnSave.Text");
                this.layoutControlItem3.Text = GetLang("UCServiceExclusive.lciServiceType.Text");
                this.layoutControlItem2.Text = GetLang("UCServiceExclusive.lciServiceType2.Text");
                this.txtKeyword1.Properties.NullValuePrompt = GetLang("UCServiceExclusive.txtKeyword1.Properties.NullValuePrompt");
                this.txtKeyword2.Properties.NullValuePrompt = GetLang("UCServiceExclusive.txtKeyword2.Properties.NullValuePrompt");
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
                LoadDataToComboServiceType(cboServiceType2, serviceTypes);
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
                aColumnCode.Caption = GetLang("UCServiceExclusive.cboServiceType.ColCode");
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                aColumnName.Caption = GetLang("UCServiceExclusive.cboServiceType.ColName");
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
        /// <summary>Luoi TRAI: danh sach dich vu goc, chon 1 bang radio (isKeyChooseService = true)</summary>
        private void InitUcgrid1()
        {
            try
            {
                serviceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<HIS.UC.Service.ServiceColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;

                HIS.UC.Service.ServiceColumn colRadio = new HIS.UC.Service.ServiceColumn("   ", "radioService", 30, true);
                colRadio.VisibleIndex = 0;
                colRadio.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio);

                HIS.UC.Service.ServiceColumn colServiceCode = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colServiceCode.Caption"), "SERVICE_CODE", 60, false);
                colServiceCode.VisibleIndex = 1;
                ado.ListServiceColumn.Add(colServiceCode);

                HIS.UC.Service.ServiceColumn colServiceName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colServiceName.Caption"), "SERVICE_NAME", 300, false);
                colServiceName.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colServiceName);

                HIS.UC.Service.ServiceColumn colServiceTypeName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colServiceTypeName.Caption"), "SERVICE_TYPE_NAME", 80, false);
                colServiceTypeName.VisibleIndex = 3;
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

        /// <summary>
        /// Luoi PHAI: danh sach dich vu loai tru, tich bang 2 cot muc xu ly.
        /// checkWarning = Canh bao, checkServiceNotUse = Chan (2 cot nay loai tru nhau san trong HIS.UC.Service).
        /// </summary>
        private void InitUcgrid2()
        {
            try
            {
                exclusiveProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<HIS.UC.Service.ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewExclusive_MouseDown;

                HIS.UC.Service.ServiceColumn colWarning = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colWarning.Caption"), "checkWarning", 70, true);
                colWarning.VisibleIndex = 0;
                colWarning.image = imageCollectionRoom.Images[0];
                colWarning.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colWarning);

                HIS.UC.Service.ServiceColumn colBlock = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colBlock.Caption"), "checkServiceNotUse", 70, true);
                colBlock.VisibleIndex = 1;
                colBlock.image = imageCollectionRoom.Images[0];
                colBlock.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colBlock);

                HIS.UC.Service.ServiceColumn colExclusiveCode = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colExclusiveCode.Caption"), "SERVICE_CODE", 60, false);
                colExclusiveCode.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colExclusiveCode);

                HIS.UC.Service.ServiceColumn colExclusiveName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colExclusiveName.Caption"), "SERVICE_NAME", 300, false);
                colExclusiveName.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colExclusiveName);

                HIS.UC.Service.ServiceColumn colExclusiveTypeName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colServiceTypeName.Caption"), "SERVICE_TYPE_NAME", 80, false);
                colExclusiveTypeName.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colExclusiveTypeName);

                this.ucGridControlExclusive = (UserControl)exclusiveProcessor.Run(ado);
                if (ucGridControlExclusive != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlExclusive);
                    this.ucGridControlExclusive.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Check all (click header cot Chan)
        private void gridViewExclusive_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return;
                }

                GridView view = sender as GridView;
                if (view == null)
                {
                    return;
                }
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.HitTest != GridHitTest.Column)
                {
                    return;
                }
                if (hi.Column.FieldName != "checkWarning" && hi.Column.FieldName != "checkServiceNotUse")
                {
                    return;
                }

                var lstCheckAll = lstExclusiveADOs;
                if (lstCheckAll == null || lstCheckAll.Count == 0)
                {
                    return;
                }

                bool isWarningColumn = hi.Column.FieldName == "checkWarning";

                WaitingManager.Show();
                int checkedNum = isWarningColumn
                    ? lstCheckAll.Count(o => o.checkWarning)
                    : lstCheckAll.Count(o => o.checkServiceNotUse);

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
                    // Khong tich chinh dich vu goc
                    if (item.ID == serviceIdChecked)
                    {
                        item.checkWarning = false;
                        item.checkServiceNotUse = false;
                        continue;
                    }

                    if (isWarningColumn)
                    {
                        item.checkWarning = isCheckAll;
                        if (isCheckAll) item.checkServiceNotUse = false;
                    }
                    else
                    {
                        item.checkServiceNotUse = isCheckAll;
                        if (isCheckAll) item.checkWarning = false;
                    }
                }
                isCheckAll = !isCheckAll;

                exclusiveProcessor.Reload(ucGridControlExclusive, lstCheckAll);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Radio click -> load cac cap loai tru cua dich vu goc
        /// <summary>
        /// Tich radio 1 dich vu ben trai -> GET api/HisServiceExclusive/Get (2 chieu)
        /// -> tich san muc xu ly tuong ung o luoi phai.
        /// </summary>
        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {
                if (data == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisServiceExclusiveFilter filter = new HisServiceExclusiveFilter();
                filter.SERVICE_ID__OR__EXCLUSIVE_ID = data.ID;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                serviceIdChecked = data.ID;

                serviceExclusivesByService = new BackendAdapter(param).Get<List<HIS_SERVICE_EXCLUSIVE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param) ?? new List<HIS_SERVICE_EXCLUSIVE>();

                ApplyCheckedToExclusiveGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tich lai luoi phai theo danh sach cap loai tru dang co cua dich vu goc</summary>
        private void ApplyCheckedToExclusiveGrid()
        {
            try
            {
                if (lstExclusiveADOs == null)
                {
                    return;
                }

                Dictionary<long, short> mapped = BuildMappedDictionary();

                foreach (var item in lstExclusiveADOs)
                {
                    short handleTypeId;
                    if (item.ID != serviceIdChecked && mapped.TryGetValue(item.ID, out handleTypeId))
                    {
                        item.checkWarning = handleTypeId == (short)HandleType.Warning;
                        item.checkServiceNotUse = handleTypeId == (short)HandleType.Block;
                    }
                    else
                    {
                        item.checkWarning = false;
                        item.checkServiceNotUse = false;
                    }
                    item.checkService = false;
                }

                lstExclusiveADOs = lstExclusiveADOs
                    .OrderByDescending(p => p.checkServiceNotUse || p.checkWarning)
                    .ToList();

                if (ucGridControlExclusive != null)
                {
                    exclusiveProcessor.Reload(ucGridControlExclusive, lstExclusiveADOs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Quy doi danh sach cap loai tru thanh map: id dich vu doi ung -> muc xu ly.
        /// Vi quan he doi xung nen phai xet ca 2 chieu.
        /// </summary>
        private Dictionary<long, short> BuildMappedDictionary()
        {
            Dictionary<long, short> result = new Dictionary<long, short>();
            try
            {
                if (serviceExclusivesByService == null)
                {
                    return result;
                }

                foreach (var item in serviceExclusivesByService)
                {
                    long otherId;
                    if (item.SERVICE_ID == serviceIdChecked)
                    {
                        otherId = item.EXCLUSIVE_ID;
                    }
                    else if (item.EXCLUSIVE_ID == serviceIdChecked)
                    {
                        otherId = item.SERVICE_ID;
                    }
                    else
                    {
                        continue;
                    }

                    if (otherId == serviceIdChecked)
                    {
                        continue;
                    }
                    result[otherId] = item.HANDLE_TYPE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Tim ban ghi cap loai tru theo id dich vu doi ung (2 chieu)</summary>
        private HIS_SERVICE_EXCLUSIVE FindPair(long otherServiceId)
        {
            if (serviceExclusivesByService == null)
            {
                return null;
            }
            return serviceExclusivesByService.FirstOrDefault(o =>
                (o.SERVICE_ID == serviceIdChecked && o.EXCLUSIVE_ID == otherServiceId)
                || (o.EXCLUSIVE_ID == serviceIdChecked && o.SERVICE_ID == otherServiceId));
        }
        #endregion

        #region Fill grid dich vu goc (trai)
        private void FillDataToGridService(UCServiceExclusive uc)
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

        private void FillDataToGrid1_Service(UCServiceExclusive uc)
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

                var rs = new BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET_VIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstServiceADOs = new List<ServiceADO>();
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO serviceADO = new ServiceADO(item);
                        // Luoi trai luon o che do chon 1 bang radio
                        serviceADO.isKeyChooseService = true;
                        serviceADO.radioService = (item.ID == serviceIdChecked);
                        lstServiceADOs.Add(serviceADO);
                    }
                }

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

        /// <summary>Mo man tu mot dich vu cu the (truyen V_HIS_SERVICE qua args)</summary>
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

                var rs = new BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET_VIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstServiceADOs = new List<ServiceADO>();
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO serviceADO = new ServiceADO(item);
                        serviceADO.isKeyChooseService = true;
                        serviceADO.radioService = true;
                        lstServiceADOs.Add(serviceADO);
                    }
                }

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

        #region Fill grid dich vu loai tru (phai)
        private void FillDataToGridExclusive(UCServiceExclusive uc)
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

                FillDataToGridExclusive(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridExclusive, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExclusive(object data)
        {
            try
            {
                WaitingManager.Show();
                listExclusive = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter filter = new HisServiceViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword2.Text.Trim();

                if (cboServiceType2.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType2.EditValue ?? "0").ToString());
                }

                var rs = new BackendAdapter(param).GetRO<List<V_HIS_SERVICE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET_VIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                lstExclusiveADOs = new List<ServiceADO>();
                if (rs != null && rs.Data != null && rs.Data.Count > 0)
                {
                    listExclusive = rs.Data;
                    foreach (var item in listExclusive)
                    {
                        ServiceADO serviceADO = new ServiceADO(item);
                        // Luoi phai o che do tich chon nhieu -> isKeyChooseService = false
                        serviceADO.isKeyChooseService = false;
                        lstExclusiveADOs.Add(serviceADO);
                    }
                }

                ApplyCheckedToExclusiveGrid();

                rowCount1 = (data == null ? 0 : lstExclusiveADOs.Count);
                dataTotal1 = (rs == null || rs.Param == null ? 0 : rs.Param.Count ?? 0);
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
                if (ucGridControlExclusive == null || ucGridControlService == null)
                {
                    return;
                }

                if (serviceIdChecked == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonDichVu,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                object exclusiveGridData = exclusiveProcessor.GetDataGridView(ucGridControlExclusive);
                if (!(exclusiveGridData is List<ServiceADO>))
                {
                    return;
                }
                lstExclusiveADOs = (List<ServiceADO>)exclusiveGridData;
                if (lstExclusiveADOs == null || lstExclusiveADOs.Count == 0)
                {
                    return;
                }

                // Khong cho khai bao dich vu loai tru voi chinh no
                if (lstExclusiveADOs.Any(o => o.ID == serviceIdChecked && (o.checkWarning || o.checkServiceNotUse)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongDuocChonChinhDichVuGoc,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                WaitingManager.Show();
                if (serviceExclusivesByService == null)
                {
                    serviceExclusivesByService = new HIS_SERVICE_EXCLUSIVE[0].ToList();
                }

                Dictionary<long, short> mapped = BuildMappedDictionary();

                List<ServiceADO> dataCreates = new List<ServiceADO>();
                List<ServiceADO> dataUpdates = new List<ServiceADO>();
                List<ServiceADO> dataDeletes = new List<ServiceADO>();

                foreach (var item in lstExclusiveADOs)
                {
                    if (item.ID == serviceIdChecked)
                    {
                        continue;
                    }

                    short newHandleTypeId = 0;
                    if (item.checkServiceNotUse) newHandleTypeId = (short)HandleType.Block;
                    else if (item.checkWarning) newHandleTypeId = (short)HandleType.Warning;

                    bool isMapped = mapped.ContainsKey(item.ID);

                    if (newHandleTypeId == 0 && isMapped)
                    {
                        dataDeletes.Add(item);
                    }
                    else if (newHandleTypeId != 0 && !isMapped)
                    {
                        dataCreates.Add(item);
                    }
                    else if (newHandleTypeId != 0 && isMapped && mapped[item.ID] != newHandleTypeId)
                    {
                        dataUpdates.Add(item);
                    }
                }

                if (dataCreates.Count == 0 && dataUpdates.Count == 0 && dataDeletes.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoThayDoiDeLuu,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = true;
                bool hasCall = false;

                // 1. Xoa cac cap bo tich
                if (dataDeletes.Count > 0)
                {
                    hasCall = true;
                    List<long> deleteIds = new List<long>();
                    foreach (var item in dataDeletes)
                    {
                        HIS_SERVICE_EXCLUSIVE pair = FindPair(item.ID);
                        if (pair != null)
                        {
                            deleteIds.Add(pair.ID);
                        }
                    }

                    if (deleteIds.Count > 0)
                    {
                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                            HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_DELETE_LIST,
                            HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            deleteIds,
                            param);
                        if (deleteResult)
                        {
                            serviceExclusivesByService = serviceExclusivesByService
                                .Where(o => !deleteIds.Contains(o.ID)).ToList();
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }

                // 2. Doi muc xu ly cac cap da co
                if (dataUpdates.Count > 0)
                {
                    hasCall = true;
                    List<HIS_SERVICE_EXCLUSIVE> updates = new List<HIS_SERVICE_EXCLUSIVE>();
                    foreach (var item in dataUpdates)
                    {
                        HIS_SERVICE_EXCLUSIVE pair = FindPair(item.ID);
                        if (pair == null)
                        {
                            continue;
                        }
                        pair.HANDLE_TYPE_ID = item.checkServiceNotUse ? (short)HandleType.Block : (short)HandleType.Warning;
                        updates.Add(pair);
                    }

                    if (updates.Count > 0)
                    {
                        var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_EXCLUSIVE>>(
                            HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_UPDATE_LIST,
                            HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            updates,
                            param);
                        if (updateResult == null || updateResult.Count == 0)
                        {
                            success = false;
                        }
                    }
                }

                // 3. Them cap moi
                if (dataCreates.Count > 0)
                {
                    hasCall = true;
                    List<HIS_SERVICE_EXCLUSIVE> creates = new List<HIS_SERVICE_EXCLUSIVE>();
                    foreach (var item in dataCreates)
                    {
                        HIS_SERVICE_EXCLUSIVE serviceExclusive = new HIS_SERVICE_EXCLUSIVE();
                        serviceExclusive.SERVICE_ID = serviceIdChecked;
                        serviceExclusive.EXCLUSIVE_ID = item.ID;
                        serviceExclusive.HANDLE_TYPE_ID = item.checkServiceNotUse ? (short)HandleType.Block : (short)HandleType.Warning;
                        creates.Add(serviceExclusive);
                    }

                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_EXCLUSIVE>>(
                        HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_CREATE_LIST,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        creates,
                        param);
                    if (createResult != null && createResult.Count > 0)
                    {
                        serviceExclusivesByService.AddRange(createResult);
                    }
                    else
                    {
                        success = false;
                    }
                }

                if (hasCall)
                {
                    // Cac man chi dinh dang nap danh muc nay trong RAM -> xoa cache de lan sau lay ban moi
                    CheckServiceExclusiveManager.ResetData();

                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }

                ApplyCheckedToExclusiveGrid();
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

        private void btnFindExclusive_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridExclusive(this);
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
                    FillDataToGridExclusive(this);
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
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal && cboServiceType.EditValue != null)
                {
                    cboServiceType.Properties.Buttons[1].Visible = true;
                    btnFind1.Focus();
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

        private void cboServiceType2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal && cboServiceType2.EditValue != null)
                {
                    cboServiceType2.Properties.Buttons[1].Visible = true;
                    btnFind2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType2.Properties.Buttons[1].Visible = false;
                    cboServiceType2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType2.EditValue != null && serviceTypes != null)
                {
                    cboServiceType2.Properties.Buttons[1].Visible = true;
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

        public void FindShortcutExclusive()
        {
            try
            {
                btnFindExclusive_Click(null, null);
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
            btnFindExclusive_Click(null, null);
        }
        #endregion
    }
}
