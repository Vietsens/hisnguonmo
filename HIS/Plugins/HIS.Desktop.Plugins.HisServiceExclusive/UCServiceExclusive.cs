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
using Inventec.Common.Controls.EditorLoader;
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
    /// Man danh muc "Dich vu khong chi dinh dong thoi" (viec 57452 / tai lieu 3342 - PT-56258).
    ///
    /// Mo hinh BAN GHI CAU HINH theo tai lieu 3342: 1 dich vu goc + danh sach dich vu khong chi dinh dong thoi
    /// + Muc xu ly (mac dinh Canh bao) + Trang thai (Con/Ngung su dung) + Ghi chu.
    ///   - Luoi TRAI : chon 1 dich vu goc bang radio (tim theo ma/ten, loc loai dich vu, loc "Da khai bao").
    ///   - Luoi PHAI : tich cac dich vu khong duoc chi dinh cung dich vu goc.
    ///   - Panel duoi: Muc xu ly / Con su dung / Ghi chu ap dung cho CA ban ghi (moi cap cua dich vu goc).
    /// Du lieu luu theo tung cap (HIS_SERVICE_EXCLUSIVE: SERVICE_ID, EXCLUSIVE_ID, HANDLE_TYPE_ID, IS_ACTIVE, NOTE);
    /// quan he la DOI XUNG nen luc doc tra ca 2 chieu, khong bao gio tao cap nguoc trung.
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

        /// <summary>
        /// Toan bo cac cap loai tru cua dich vu goc dang chon (tra 2 chieu, KHONG loc IS_ACTIVE
        /// de con hien duoc ban ghi dang Ngung su dung). Nen so diff khi Luu.
        /// </summary>
        List<HIS_SERVICE_EXCLUSIVE> serviceExclusivesByService { get; set; }

        /// <summary>ID cac dich vu da co ban ghi cau hinh (dung cho bo loc "Da khai bao" o luoi trai)</summary>
        List<long> declaredServiceIds;

        V_HIS_SERVICE currentService;

        /// <summary>
        /// Cot tick o luoi phai dung repository "checkWarning" cua HIS.UC.Service (caption doi thanh "Chon"):
        /// handler cua no chi bat/tat co, KHONG tra cuu phong/gia nhu "checkService" nen nhe va khong tac dung phu.
        /// </summary>
        private const string EXCLUSIVE_CHECK_FIELD = "checkWarning";
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
                LoadComboHandleType();
                InitUcgrid1();
                InitUcgrid2();
                ResetRecordPanel();

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
                this.lciHandleType.Text = GetLang("UCServiceExclusive.lciHandleType.Text");
                this.lciNote.Text = GetLang("UCServiceExclusive.lciNote.Text");
                this.chkIsActive.Text = GetLang("UCServiceExclusive.chkIsActive.Text");
                this.chkOnlyDeclared.Text = GetLang("UCServiceExclusive.chkOnlyDeclared.Text");
                this.chkOnlyDeclared.ToolTip = GetLang("UCServiceExclusive.chkOnlyDeclared.ToolTip");
                this.txtKeyword1.Properties.NullValuePrompt = GetLang("UCServiceExclusive.txtKeyword1.Properties.NullValuePrompt");
                this.txtKeyword2.Properties.NullValuePrompt = GetLang("UCServiceExclusive.txtKeyword2.Properties.NullValuePrompt");
                this.txtNote.Properties.NullValuePrompt = GetLang("UCServiceExclusive.txtNote.NullValuePrompt");
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

        /// <summary>Combo Muc xu ly: 1 = Canh bao (mac dinh theo tai lieu 3342), 2 = Chan</summary>
        private void LoadComboHandleType()
        {
            try
            {
                List<HandleTypeItem> data = new List<HandleTypeItem>();
                data.Add(new HandleTypeItem((short)HandleType.Warning, GetLang("UCServiceExclusive.cboHandleType.Warning")));
                data.Add(new HandleTypeItem((short)HandleType.Block, GetLang("UCServiceExclusive.cboHandleType.Block")));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 170);
                ControlEditorLoader.Load(cboHandleType, data, controlEditorADO);
                cboHandleType.EditValue = (short)HandleType.Warning;
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

        /// <summary>Luoi PHAI: danh sach dich vu loai tru, tich chon nhieu (1 cot "Chon")</summary>
        private void InitUcgrid2()
        {
            try
            {
                exclusiveProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<HIS.UC.Service.ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewExclusive_MouseDown;

                HIS.UC.Service.ServiceColumn colCheck = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colChoose.Caption"), EXCLUSIVE_CHECK_FIELD, 50, true);
                colCheck.VisibleIndex = 0;
                colCheck.image = imageCollectionRoom.Images[0];
                colCheck.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck);

                HIS.UC.Service.ServiceColumn colExclusiveCode = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colExclusiveCode.Caption"), "SERVICE_CODE", 60, false);
                colExclusiveCode.VisibleIndex = 1;
                ado.ListServiceColumn.Add(colExclusiveCode);

                HIS.UC.Service.ServiceColumn colExclusiveName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colExclusiveName.Caption"), "SERVICE_NAME", 300, false);
                colExclusiveName.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colExclusiveName);

                HIS.UC.Service.ServiceColumn colExclusiveTypeName = new HIS.UC.Service.ServiceColumn(GetLang("UCServiceExclusive.colServiceTypeName.Caption"), "SERVICE_TYPE_NAME", 80, false);
                colExclusiveTypeName.VisibleIndex = 3;
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

        #region Check all (click header cot Chon)
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
                if (hi.HitTest != GridHitTest.Column || hi.Column.FieldName != EXCLUSIVE_CHECK_FIELD)
                {
                    return;
                }

                var lstCheckAll = lstExclusiveADOs;
                if (lstCheckAll == null || lstCheckAll.Count == 0)
                {
                    return;
                }

                WaitingManager.Show();
                int checkedNum = lstCheckAll.Count(o => o.checkWarning);
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
                    // Khong bao gio tich chinh dich vu goc
                    item.checkWarning = isCheckAll && item.ID != serviceIdChecked;
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

        #region Radio click -> nap ban ghi cau hinh cua dich vu goc
        /// <summary>
        /// Tich radio 1 dich vu ben trai -> GET api/HisServiceExclusive/Get (2 chieu, moi trang thai)
        /// -> tich san luoi phai + do Muc xu ly / Trang thai / Ghi chu cua ban ghi len panel.
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
                serviceIdChecked = data.ID;
                LoadServiceExclusivesByService();
                ApplyCheckedToExclusiveGrid();
                FillRecordPanel();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceExclusivesByService()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceExclusiveFilter filter = new HisServiceExclusiveFilter();
                filter.SERVICE_ID__OR__EXCLUSIVE_ID = serviceIdChecked;

                serviceExclusivesByService = new BackendAdapter(param).Get<List<HIS_SERVICE_EXCLUSIVE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param) ?? new List<HIS_SERVICE_EXCLUSIVE>();
            }
            catch (Exception ex)
            {
                serviceExclusivesByService = new List<HIS_SERVICE_EXCLUSIVE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Id dich vu doi ung cua 1 cap (quan he doi xung)</summary>
        private long GetOtherServiceId(HIS_SERVICE_EXCLUSIVE pair)
        {
            return pair.SERVICE_ID == serviceIdChecked ? pair.EXCLUSIVE_ID : pair.SERVICE_ID;
        }

        /// <summary>Map: id dich vu doi ung -> cap loai tru dang co (2 chieu)</summary>
        private Dictionary<long, HIS_SERVICE_EXCLUSIVE> BuildMappedDictionary()
        {
            Dictionary<long, HIS_SERVICE_EXCLUSIVE> result = new Dictionary<long, HIS_SERVICE_EXCLUSIVE>();
            try
            {
                if (serviceExclusivesByService == null || serviceIdChecked <= 0)
                {
                    return result;
                }

                foreach (var item in serviceExclusivesByService)
                {
                    if (item.SERVICE_ID != serviceIdChecked && item.EXCLUSIVE_ID != serviceIdChecked)
                    {
                        continue;
                    }
                    long otherId = GetOtherServiceId(item);
                    if (otherId == serviceIdChecked)
                    {
                        continue;
                    }
                    result[otherId] = item;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Tich lai luoi phai theo cac cap dang co cua dich vu goc</summary>
        private void ApplyCheckedToExclusiveGrid()
        {
            try
            {
                if (lstExclusiveADOs == null)
                {
                    return;
                }

                Dictionary<long, HIS_SERVICE_EXCLUSIVE> mapped = BuildMappedDictionary();

                foreach (var item in lstExclusiveADOs)
                {
                    item.checkWarning = item.ID != serviceIdChecked && mapped.ContainsKey(item.ID);
                    item.checkService = false;
                    item.checkServiceNotUse = false;
                }

                lstExclusiveADOs = lstExclusiveADOs.OrderByDescending(p => p.checkWarning).ToList();

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
        /// Do thong tin ban ghi (Muc xu ly / Trang thai / Ghi chu) cua dich vu goc len panel.
        /// Cac cap cua cung 1 dich vu goc duoc luu cung gia tri nen lay theo cap pho bien nhat.
        /// </summary>
        private void FillRecordPanel()
        {
            try
            {
                Dictionary<long, HIS_SERVICE_EXCLUSIVE> mapped = BuildMappedDictionary();
                if (mapped.Count == 0)
                {
                    ResetRecordPanel();
                    return;
                }

                List<HIS_SERVICE_EXCLUSIVE> pairs = mapped.Values.ToList();

                short handleTypeId = pairs
                    .GroupBy(o => o.HANDLE_TYPE_ID)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .First();
                cboHandleType.EditValue = handleTypeId == (short)HandleType.Block
                    ? (short)HandleType.Block
                    : (short)HandleType.Warning;

                chkIsActive.Checked = pairs.Any(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                HIS_SERVICE_EXCLUSIVE withNote = pairs.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.NOTE));
                txtNote.Text = withNote != null ? withNote.NOTE : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Gia tri mac dinh cua ban ghi moi: Canh bao, Con su dung, khong ghi chu (tai lieu 3342)</summary>
        private void ResetRecordPanel()
        {
            try
            {
                cboHandleType.EditValue = (short)HandleType.Warning;
                chkIsActive.Checked = true;
                txtNote.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Fill grid dich vu goc (trai)
        private void FillDataToGridService(UCServiceExclusive uc)
        {
            try
            {
                serviceIdChecked = 0;
                serviceExclusivesByService = null;
                ResetRecordPanel();
                ApplyCheckedToExclusiveGrid();

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

        /// <summary>
        /// Bo loc "Da khai bao": chi hien cac dich vu da co ban ghi cau hinh (moi trang thai).
        /// Lay id tu toan bo bang HIS_SERVICE_EXCLUSIVE (2 dau cua moi cap).
        /// </summary>
        private List<long> GetDeclaredServiceIds()
        {
            List<long> result = new List<long>();
            try
            {
                CommonParam param = new CommonParam();
                HisServiceExclusiveFilter filter = new HisServiceExclusiveFilter();
                List<HIS_SERVICE_EXCLUSIVE> all = new BackendAdapter(param).Get<List<HIS_SERVICE_EXCLUSIVE>>(
                    HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param) ?? new List<HIS_SERVICE_EXCLUSIVE>();

                result = all.Select(o => o.SERVICE_ID)
                    .Union(all.Select(o => o.EXCLUSIVE_ID))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

                if (chkOnlyDeclared.Checked)
                {
                    if (declaredServiceIds == null)
                    {
                        declaredServiceIds = GetDeclaredServiceIds();
                    }
                    // Khong co ban ghi nao -> truyen id khong ton tai de luoi rong (list rong se bi backend bo qua)
                    filter.IDs = declaredServiceIds.Count > 0 ? declaredServiceIds : new List<long> { -1 };
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
        /// <summary>
        /// Luu BAN GHI cau hinh cua dich vu goc dang chon:
        ///  - cap tich them   -> CreateList (Muc xu ly / Trang thai / Ghi chu theo panel)
        ///  - cap bo tich     -> DeleteList (chi xet cac dong dang hien tren luoi phai)
        ///  - cap giu lai     -> UpdateList neu Muc xu ly / Ghi chu / Trang thai khac panel
        /// Doi trang thai di qua ChangeLock: backend khong cho Update ban ghi dang khoa (IsUnLock),
        /// nen cap dang Ngung su dung phai mo khoa truoc roi moi cap nhat.
        /// </summary>
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
                    ShowInfo(Resources.ResourceMessage.ChuaChonDichVu);
                    return;
                }

                object exclusiveGridData = exclusiveProcessor.GetDataGridView(ucGridControlExclusive);
                if (exclusiveGridData is List<ServiceADO>)
                {
                    lstExclusiveADOs = (List<ServiceADO>)exclusiveGridData;
                }
                if (lstExclusiveADOs == null)
                {
                    lstExclusiveADOs = new List<ServiceADO>();
                }

                // Khong cho khai bao dich vu loai tru voi chinh no
                if (lstExclusiveADOs.Any(o => o.ID == serviceIdChecked && o.checkWarning))
                {
                    ShowInfo(Resources.ResourceMessage.KhongDuocChonChinhDichVuGoc);
                    return;
                }

                short desiredHandleTypeId = GetSelectedHandleTypeId();
                short desiredIsActive = chkIsActive.Checked
                    ? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    : IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
                string desiredNote = String.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim();

                Dictionary<long, HIS_SERVICE_EXCLUSIVE> mapped = BuildMappedDictionary();

                List<ServiceADO> dataCreates = new List<ServiceADO>();
                List<long> deleteIds = new List<long>();
                HashSet<long> deletedOtherIds = new HashSet<long>();

                foreach (var item in lstExclusiveADOs)
                {
                    if (item.ID == serviceIdChecked)
                    {
                        continue;
                    }
                    bool isMapped = mapped.ContainsKey(item.ID);
                    if (item.checkWarning && !isMapped)
                    {
                        dataCreates.Add(item);
                    }
                    else if (!item.checkWarning && isMapped)
                    {
                        deleteIds.Add(mapped[item.ID].ID);
                        deletedOtherIds.Add(item.ID);
                    }
                }

                // Cac cap giu lai (ke ca cap khong hien tren trang luoi hien tai) nhan gia tri ban ghi tren panel
                List<HIS_SERVICE_EXCLUSIVE> dataUpdates = mapped
                    .Where(o => !deletedOtherIds.Contains(o.Key))
                    .Select(o => o.Value)
                    .Where(o => o.HANDLE_TYPE_ID != desiredHandleTypeId
                             || (o.NOTE ?? "") != (desiredNote ?? "")
                             || (o.IS_ACTIVE ?? IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) != desiredIsActive)
                    .ToList();

                if (dataCreates.Count == 0 && deleteIds.Count == 0 && dataUpdates.Count == 0)
                {
                    if (mapped.Count == 0)
                    {
                        ShowInfo(Resources.ResourceMessage.ChuaChonDichVuLoaiTru);
                    }
                    else
                    {
                        ShowInfo(Resources.ResourceMessage.KhongCoThayDoiDeLuu);
                    }
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = true;

                // 1. Xoa cac cap bo tich
                if (deleteIds.Count > 0)
                {
                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                        HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_DELETE_LIST,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        deleteIds,
                        param);
                    success = success && deleteResult;
                }

                // 2. Cap nhat Muc xu ly / Ghi chu / Trang thai cho cac cap giu lai
                if (success && dataUpdates.Count > 0)
                {
                    // Cap dang Ngung su dung phai mo khoa truoc, backend khong cho Update ban ghi dang khoa
                    foreach (var pair in dataUpdates.Where(o => o.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                    {
                        success = success && ChangeLock(pair.ID, param);
                    }

                    if (success)
                    {
                        foreach (var pair in dataUpdates)
                        {
                            pair.HANDLE_TYPE_ID = desiredHandleTypeId;
                            pair.NOTE = desiredNote;
                            pair.IS_ACTIVE = desiredIsActive;
                        }
                        var updateResult = new BackendAdapter(param).Post<List<HIS_SERVICE_EXCLUSIVE>>(
                            HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_UPDATE_LIST,
                            HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            dataUpdates,
                            param);
                        success = success && updateResult != null && updateResult.Count > 0;
                    }
                }

                // 3. Them cap moi
                if (success && dataCreates.Count > 0)
                {
                    List<HIS_SERVICE_EXCLUSIVE> creates = new List<HIS_SERVICE_EXCLUSIVE>();
                    foreach (var item in dataCreates)
                    {
                        HIS_SERVICE_EXCLUSIVE serviceExclusive = new HIS_SERVICE_EXCLUSIVE();
                        serviceExclusive.SERVICE_ID = serviceIdChecked;
                        serviceExclusive.EXCLUSIVE_ID = item.ID;
                        serviceExclusive.HANDLE_TYPE_ID = desiredHandleTypeId;
                        serviceExclusive.NOTE = desiredNote;
                        serviceExclusive.IS_ACTIVE = desiredIsActive;
                        creates.Add(serviceExclusive);
                    }

                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_EXCLUSIVE>>(
                        HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_CREATE_LIST,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        creates,
                        param);
                    success = success && createResult != null && createResult.Count > 0;

                    // Ban ghi tao moi o trang thai Ngung su dung: backend co the ep IS_ACTIVE = 1 luc tao -> khoa lai
                    if (success && desiredIsActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        foreach (var created in createResult.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                        {
                            success = success && ChangeLock(created.ID, param);
                        }
                    }
                }

                // Cac man chi dinh dang nap danh muc nay trong RAM -> xoa cache de lan sau lay ban moi
                CheckServiceExclusiveManager.ResetData();
                declaredServiceIds = null;

                // Nap lai ban ghi tu backend de luoi + panel phan anh dung du lieu da luu
                LoadServiceExclusivesByService();
                ApplyCheckedToExclusiveGrid();
                FillRecordPanel();

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ChangeLock(long id, CommonParam param)
        {
            bool result = false;
            try
            {
                var lockResult = new BackendAdapter(param).Post<HIS_SERVICE_EXCLUSIVE>(
                    HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_CHANGE_LOCK,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    id,
                    param);
                result = lockResult != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private short GetSelectedHandleTypeId()
        {
            try
            {
                if (cboHandleType.EditValue != null
                    && Inventec.Common.TypeConvert.Parse.ToInt64(cboHandleType.EditValue.ToString()) == (long)HandleType.Block)
                {
                    return (short)HandleType.Block;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (short)HandleType.Warning;
        }

        private void ShowInfo(string message)
        {
            try
            {
                WaitingManager.Hide();
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    message,
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void chkOnlyDeclared_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                declaredServiceIds = null;
                FillDataToGridService(this);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
