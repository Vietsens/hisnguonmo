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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using SAR.Desktop.Plugins.SarRetyFofi.ADO;
//using SAR.Desktop.Plugins.SarRetyFofi.SarRetyFofi;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using His.UC.CreateReport;
using HIS.Desktop.LocalStorage.BackendData;
using SAR.Desktop.Plugins.SarRetyFofi.SarRetyFofi;

namespace SAR.Desktop.Plugins.SarRetyFofi
{
    public partial class frmSarRetyFofi1 : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI currentRetyFofi;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<InitListPreview> listPreview = new List<InitListPreview>();
        bool isChangeRowIndex = false;
        int row = 1;
        int col = 1;
        List<V_SAR_RETY_FOFI> dataRetyFofi = new List<V_SAR_RETY_FOFI>();
        SAR_REPORT_TYPE reportType = new SAR_REPORT_TYPE();
        List<V_SAR_RETY_FOFI> currentFormFields;
        HIS.UC.FormType.GenerateRDO generateRDO;
        #endregion

        #region Construct
        public frmSarRetyFofi1(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormListClick.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmSarRetyFofi_Load(object sender, EventArgs e)
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

        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarRetyFofi.Resources.Lang", typeof(SAR.Desktop.Plugins.SarRetyFofi.frmSarRetyFofi1).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt


                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSarRetyFofi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetDefaultValue()
        {
            try
            {
                this.currentRetyFofi = null;
                this.ActionType = GlobalVariables.ActionAdd;
                spinColNumber.Value = (int)1;
                spinRowNumber.Value = (int)1;
                //ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            //try
            //{
            //    dicOrderTabIndexControl.Add("txtProgramCode", 0);
            //    dicOrderTabIndexControl.Add("txtProgramName", 1);


            //    if (dicOrderTabIndexControl != null)
            //    {
            //        foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
            //        {
            //            SetTabIndexToControl(itemOrderTab, lcEditorInfo);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void FillDataToControlsForm()
        {
            try
            {
                His.UC.CreateReport.CreateReportConfig.Language = LanguageManager.GetLanguage();
                His.UC.CreateReport.CreateReportConfig.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.UC.CreateReport.CreateReportConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                His.UC.CreateReport.CreateReportConfig.BranchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                HIS.UC.FormType.FormTypeConfig.BranchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();

                HIS.UC.FormType.FormTypeConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>().Where(o => o.IS_ACTIVE == 1).ToList();
                His.UC.CreateReport.CreateReportConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>().Where(o => o.IS_ACTIVE == 1).ToList();
                His.UC.CreateReport.CreateReportConfig.RetyFofis = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_SAR_RETY_FOFI>>("api/SarRetyFofi/GetView", ApiConsumers.SarConsumer, new SarRetyFofiViewFilter() { IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE }, null);
                HIS.UC.FormType.FormTypeConfig.Language = LanguageManager.GetLanguage();

                HIS.UC.FormType.Config.HisFormTypeConfig.VHisMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                HIS.UC.FormType.Config.HisFormTypeConfig.VHisExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                HIS.UC.FormType.Config.HisFormTypeConfig.VHisBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisDepartments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisKskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisTreatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisExpMestTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisServiceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>();
                HIS.UC.FormType.FormTypeConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                HIS.UC.FormType.ApiConsumerStore.MosConsumer = ApiConsumers.MosConsumer;
                HIS.UC.FormType.ApiConsumerStore.AcsConsumer = ApiConsumers.AcsConsumer;
                HIS.UC.FormType.ApiConsumerStore.SarConsumer = ApiConsumers.SarConsumer;
                HIS.UC.FormType.ApiConsumerStore.SdaConsumer = ApiConsumers.SdaConsumer;


                HIS.UC.FormType.Base.ResouceManager.ResourceLanguageManager();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                //FillDataToControlsForm();
                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                SarReportTypeFilter reportTypeFilter = new SarReportTypeFilter();
                SetFilterNavBar(ref reportTypeFilter);
                reportTypeFilter.ORDER_DIRECTION = "DESC";
                reportTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                reportTypeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                var datareportType = new BackendAdapter(paramCommon).GetRO<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.SarConsumer, reportTypeFilter, paramCommon);

                if (datareportType != null)
                {
                    var data = (List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>)datareportType.Data;

                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (datareportType.Param == null ? 0 : datareportType.Param.Count ?? 0);
                        this.reportType = data[0];
                        LoadDataToGridTruongLocBC(this.reportType);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private string[] statementsSeperate(string sql)
        {
            string[] result = null;
            try
            {
                var fixedInput = System.Text.RegularExpressions.Regex.Replace(sql, "[^a-zA-Z0-9%: ._]", " ");
                result = fixedInput.Split(' ');
                result = result.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        System.Windows.Forms.UserControl GenerateControl(V_SAR_RETY_FOFI data)
        {
            System.Windows.Forms.UserControl result = null;
            try
            {
                result = HIS.UC.FormType.FormTypeMain.Run(data, this.generateRDO) as System.Windows.Forms.UserControl;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void CreateReportControlByReportType(SAR_REPORT_TYPE sarReportType)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Load Create Report ControlC");

                WaitingManager.Show();
                this.generateRDO = new HIS.UC.FormType.GenerateRDO();
                generateRDO.DetailData = sarReportType;

                this.currentFormFields = CreateReportConfig.RetyFofis.Where(o => o.REPORT_TYPE_ID == sarReportType.ID).OrderBy(o => o.NUM_ORDER).ToList();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentFormFields", currentFormFields)
                    + Inventec.Common.Logging.LogUtil.TraceData("reportType", sarReportType));

                //nếu là tự khai báo sẽ tạo ra retyfofi để gen control.
                if (sarReportType.REPORT_TYPE_CODE.ToUpper().StartsWith("TKB") && sarReportType.SQL != null && this.currentFormFields.Count == 0)
                {
                    var querry = System.Text.Encoding.UTF8.GetString(sarReportType.SQL);
                    querry = querry.Replace(":", " :").Replace("&", " :");
                    var lstFilter = statementsSeperate(querry);
                    if (lstFilter != null)
                    {
                        List<string> lstOutPut = new List<string>();
                        int ic = -1;
                        while (true)
                        {
                            //lấy giá trị đầu tiên chứa dấu : (từ sau vị trí hiện tại)
                            string value = lstFilter.Skip(ic + 1).FirstOrDefault(o => o.StartsWith(":"));
                            //lấy  vị trị có giá trị bằng giá trị đó (từ sau vị trí hiện tại)
                            ic = Array.IndexOf<string>(lstFilter, value, ic + 1);
                            //nếu không có vị trí đó (ic=-1) thì thoát khỏi vòng lặp
                            if (ic < 0)
                            {
                                break;
                            }
                            //bỏ dấu : ra khỏi giá trị
                            value = value.Replace(":", "");
                            //nếu sau khi bỏ dấu : được giá trị rỗng thì lấy giá trị tiếp theo (áp dụng đối với trường hợp dấu : đặt xa so với giá trị)
                            if (string.IsNullOrWhiteSpace(value) && ic < lstFilter.Length - 1)
                            {
                                value = lstFilter[ic + 1].Replace(":", "");
                            }
                            //cuổi cùng kiểm tra nếu giá trị không rỗng thì Add vào danh sách
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                lstOutPut.Add(value);
                            }
                        }

                        lstOutPut = lstOutPut.Distinct().ToList();

                        V_SAR_RETY_FOFI fofi = new V_SAR_RETY_FOFI();
                        fofi.REPORT_TYPE_ID = sarReportType.ID;
                        fofi.REPORT_TYPE_CODE = sarReportType.REPORT_TYPE_CODE;
                        fofi.REPORT_TYPE_NAME = sarReportType.REPORT_TYPE_NAME;
                        fofi.NUM_ORDER = 10;
                        fofi.IS_REQUIRE = 1;
                        fofi.FORM_FIELD_CODE = "FTHIS000034";
                        fofi.JSON_OUTPUT = string.Join(";", lstOutPut);
                        fofi.WIDTH_RATIO = 2;
                        fofi.HEIGHT = 30;

                        if (currentFormFields == null) currentFormFields = new List<V_SAR_RETY_FOFI>();

                        currentFormFields.Add(fofi);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField.Width___runtime size:", ("[" + lcGenerateReportField.Width
                    + ", " + lcGenerateReportField.Height + "],ClientSize=" + lcGenerateReportField.ClientSize
                    + ", MaximumSize=" + lcGenerateReportField.MaximumSize
                    + ",Screen.PrimaryScreen.Bounds.Size:" + System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size))
                    + ", design size: [1356, 653]"
                    + "currentFormFields.count=" + ((this.currentFormFields != null && this.currentFormFields.Count > 0) ? currentFormFields.Count : 0)
                    );

                lcGenerateReportField.BeginUpdate();
                lcGenerateReportField.Controls.Clear();
                layoutControlGroupGenerateReportField.Clear();
                //layoutControlGroupGenerateReportField1.Clear();

                if (this.currentFormFields != null && this.currentFormFields.Count > 0)
                {
                    //xtraTabControlFilter.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

                    var dataRH = currentFormFields.Where(o => o.ROW_COUNT > 0 && o.COLUMN_COUNT > 0 && o.ROW_INDEX != null).FirstOrDefault();
                    var dataHasInfor = currentFormFields.Where(o => o.ROW_COUNT > 0 && o.COLUMN_COUNT > 0 && o.ROW_INDEX != null).ToList();

                    var dataNoInfor = currentFormFields.Where(o => !(o.ROW_COUNT > 0 && o.COLUMN_COUNT > 0 && o.ROW_INDEX != null)).ToList();

                    if (dataHasInfor != null && dataHasInfor.Count > 0)
                    {

                        //}
                        // long rowCount = dataRH != null ? dataRH.ROW_COUNT.Value : 0;
                        //long columnCount = dataRH != null ? dataRH.COLUMN_COUNT.Value : 0;
                        long rowCount = dataHasInfor.First().ROW_COUNT.Value;
                        long columnCount = dataHasInfor.First().COLUMN_COUNT.Value;
                        //if (rowCount > 0 && columnCount > 0)
                        //{
                        //lcGenerateReportField.BeginUpdate();

                        //int w__Row = (int)((lcGenerateReportField.Width ) / columnCount);
                        int w__Row = (int)((lcGenerateReportField.Width - 30) / columnCount);

                        for (int i = 1; i <= rowCount; i++)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowCount), rowCount)
                                + Inventec.Common.Logging.LogUtil.TraceData("rowindex:i=", i));
                            List<BaseLayoutItem> layoutControlItemAdd_Rows = new List<BaseLayoutItem>();
                            for (int j = 0; j < columnCount; j++)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => columnCount), columnCount)
                                                                + Inventec.Common.Logging.LogUtil.TraceData("columnindex:j=", j));
                                LayoutControl lcGenerateReportField__Row = new LayoutControl();
                                lcGenerateReportField__Row.Root.GroupBordersVisible = false;
                                lcGenerateReportField__Row.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);

                                LayoutControlGroup layoutControlGroupGenerateReportField__Row = lcGenerateReportField__Row.Root;
                                layoutControlGroupGenerateReportField__Row.GroupBordersVisible = false;
                                layoutControlGroupGenerateReportField__Row.ExpandButtonVisible = false;
                                layoutControlGroupGenerateReportField__Row.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);

                                int layout__row_height = 0;

                                long rowIndexFilter = ((j + 1) + (i - 1) * columnCount);//
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowIndexFilter), rowIndexFilter));
                                var formFieldInRows = dataHasInfor.Where(o => o.ROW_INDEX == rowIndexFilter).OrderBy(k => k.NUM_ORDER).ToList();

                                if (formFieldInRows != null && formFieldInRows.Count > 0)
                                {
                                    int dem = 1;
                                    int totalwidthRatio = 0;

                                    List<BaseLayoutItem> layoutControlItemAdds = new List<BaseLayoutItem>();
                                    foreach (var item in formFieldInRows)
                                    {
                                        System.Windows.Forms.UserControl control = GenerateControl(item);

                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.FORM_FIELD_CODE), item.FORM_FIELD_CODE)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.REPORT_TYPE_CODE), item.REPORT_TYPE_CODE)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.WIDTH_RATIO), item.WIDTH_RATIO)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.HEIGHT), item.HEIGHT)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.NUM_ORDER), item.NUM_ORDER)
                                            + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));

                                        control.Name = Guid.NewGuid().ToString();// (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                                        LayoutControlItem item1 = layoutControlGroupGenerateReportField__Row.AddItem();
                                        layoutControlItemAdds.Add(item1);
                                        // Bind a control to the layout item.

                                        int h = (int)(item.HEIGHT ?? 0);
                                        int wr = (int)(item.WIDTH_RATIO ?? 0);
                                        //int wr = (int)(item.WIDTH_RATIO ?? 3);
                                        int w = wr > 0 ? (w__Row * wr / 3) : 0;
                                        //int w = (w__Row) * wr / 3;
                                        int realH = (h > 0 ? h : (control.Height + 5));
                                        if (dem == formFieldInRows.Count) realH += 10;
                                        item1.Control = control;
                                        item1.TextVisible = false;
                                        item1.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                                        item1.SizeConstraintsType = SizeConstraintsType.Custom;

                                        item1.Height = realH;
                                        item1.MaxSize = new System.Drawing.Size(0, realH);
                                        item1.MinSize = new System.Drawing.Size(w, realH);
                                        if (w > 0 && realH > 0)
                                            item1.Size = new System.Drawing.Size(w, realH);

                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item1.Size", item1.Size)
                                            + Inventec.Common.Logging.LogUtil.TraceData("control.Height", control.Height)
                                            + Inventec.Common.Logging.LogUtil.TraceData("realH", realH)
                                            + Inventec.Common.Logging.LogUtil.TraceData("h", h));

                                        if (wr < 3 && totalwidthRatio > 0 && totalwidthRatio + wr <= 3)
                                        {
                                            item1.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                            Inventec.Common.Logging.LogSystem.Debug("item1.Control.Name=" + item1.Control.Name + ",layoutControlItemAdds[dem - 2].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdds[dem - 2])).Control.Name + ",totalwidthRatio=" + totalwidthRatio);

                                        }
                                        else
                                        {
                                            totalwidthRatio = 0;
                                            layout__row_height += realH;
                                        }
                                        dem++;
                                        totalwidthRatio += wr;
                                    }
                                }
                                lcGenerateReportField__Row.Height = layout__row_height + 5;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("layoutControlGroupGenerateReportField__Row.Width", layoutControlGroupGenerateReportField__Row.Width)
                                  + Inventec.Common.Logging.LogUtil.TraceData("layoutControlGroupGenerateReportField__Row.Height", layoutControlGroupGenerateReportField__Row.Height)
                                  + Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField__Row.Height", lcGenerateReportField__Row.Height));

                                //LayoutControlItem item111 = layoutControlGroupGenerateReportField1.AddItem();
                                LayoutControlItem item111 = layoutControlGroupGenerateReportField.AddItem();
                                layoutControlItemAdd_Rows.Add(item111);
                                // Bind a control to the layout item.
                                item111.SizeConstraintsType = SizeConstraintsType.Custom;
                                int realH_Row = lcGenerateReportField__Row.Height;
                                item111.Height = realH_Row;
                                item111.MaxSize = new System.Drawing.Size(w__Row, realH_Row);
                                item111.MinSize = new System.Drawing.Size(w__Row, realH_Row);
                                item111.Size = new System.Drawing.Size(w__Row, realH_Row);

                                item111.Control = lcGenerateReportField__Row;
                                item111.TextVisible = false;
                                item111.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                                item111.SizeConstraintsType = SizeConstraintsType.Custom;
                                //int realH_Row = lcGenerateReportField__Row.Height;
                                //item111.Height = realH_Row;
                                //item111.MaxSize = new System.Drawing.Size(w__Row, realH_Row);
                                //item111.MinSize = new System.Drawing.Size(w__Row, realH_Row);
                                //item111.Size = new System.Drawing.Size(w__Row, realH_Row);

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item111.Size", item111.Size));
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => j), j));
                                if (j > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("j - 1=" + (j - 1) + ",layoutControlItemAdd_Rows.count =" + layoutControlItemAdd_Rows.Count);
                                    item111.Move(layoutControlItemAdd_Rows[j - 1], DevExpress.XtraLayout.Utils.InsertType.Right);
                                }

                                //Inventec.Common.Logging.LogSystem.Debug("item111.Control.Name=" + item111.Control.Name + ",layoutControlItemAdd_Rows[j - 1].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdd_Rows[j - 1])).Control.Name);

                            }
                        }
                        //lcGenerateReportField.EndUpdate();
                    }
                    if (dataNoInfor != null && dataNoInfor.Count > 0)
                    // else if ((!(rowCount > 0 && columnCount > 0)) || (rowCount !=0 && columnCount !=0 && isNoRowIndex))
                    {
                        int dem = 1;
                        int totalwidthRatio = 0;
                        //lcGenerateReportField.BeginUpdate();
                        List<BaseLayoutItem> layoutControlItemAdds = new List<BaseLayoutItem>();
                        //foreach (var item in this.currentFormFields)
                        foreach (var item in dataNoInfor)
                        {
                            System.Windows.Forms.UserControl control = GenerateControl(item);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.FORM_FIELD_CODE), item.FORM_FIELD_CODE)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.REPORT_TYPE_CODE), item.REPORT_TYPE_CODE)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.WIDTH_RATIO), item.WIDTH_RATIO)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.HEIGHT), item.HEIGHT)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.NUM_ORDER), item.NUM_ORDER)
                                + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));

                            control.Name = Guid.NewGuid().ToString();// (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                            LayoutControlItem item1 = layoutControlGroupGenerateReportField.AddItem();
                            //LayoutControlItem item1 = layoutControlGroupGenerateReportField1.AddItem();
                            layoutControlItemAdds.Add(item1);
                            // Bind a control to the layout item.
                            item1.Control = control;
                            item1.TextVisible = false;
                            item1.Name = Guid.NewGuid().ToString();// String.Format("lciForItemControlTemp{0}", (item.DESCRIPTION ?? "XtraUserControl" + DateTime.Now.ToString("yyyyMMddHHmmss")));
                            item1.SizeConstraintsType = SizeConstraintsType.Custom;
                            int h = (int)(item.HEIGHT ?? 0);
                            int wr = (int)(item.WIDTH_RATIO ?? 3);
                            int w = lcGenerateReportField.Width * wr / 3;
                            int realH = (h > 0 ? h : (control.Height + 5));
                            item1.Height = realH;
                            item1.MaxSize = new System.Drawing.Size(0, realH);
                            item1.MinSize = new System.Drawing.Size(w, realH);
                            if (w > 0 && realH > 0)
                                item1.Size = new System.Drawing.Size(w, realH);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item1.Size", item1.Size));
                            if (wr < 3 && totalwidthRatio > 0 && totalwidthRatio + wr <= 3)
                            {
                                item1.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                Inventec.Common.Logging.LogSystem.Debug("item1.Control.Name=" + item1.Control.Name + ",layoutControlItemAdds[dem - 2].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdds[dem - 2])).Control.Name + ",totalwidthRatio=" + totalwidthRatio);
                            }
                            else
                            {
                                totalwidthRatio = 0;
                            }
                            dem++;
                            totalwidthRatio += wr;
                        }
                        //lcGenerateReportField.EndUpdate();
                    }
                }
                lcGenerateReportField.EndUpdate();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetFilterNavBar(ref SarReportTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }


        private void LoadCurrent(long currentId, ref SAR.EFMODEL.DataModels.SAR_RETY_FOFI currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarRetyFofiFilter filter = new SarRetyFofiFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI)gridviewFormList.GetFocusedRow();

                    SarRetyFofiFilter filter = new SarRetyFofiFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SARV_SAR_RETY_FOFI_DELETE, ApiConsumers.SarConsumer, data, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentRetyFofi = ((List<V_SAR_RETY_FOFI>)gridControlFormListClick.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                //ResetFormData();
                SetFocusEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UpdateRowDataAfterEdit(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI) is null");
                var rowData = (SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Validate



        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                WaitingManager.Show();
                
                //Gan gia tri mac dinh
                SetDefaultValue();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set enable control default
                //EnableControlChanged(this.ActionType);

                //LoadComboTrangthai();
                //Load du lieu
                //FillDataToControlsForm();
                FillDataToGridControl();

                

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                //ValidateForm();

                //Focus default
                SetDefaultFocus();

                //CreateReportTypeArea();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcut
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


        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)FillDataToGridControl);
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.SarImportRetyRofi, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.SarImportRetyRofi, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            CreateExport();
        }

        private void CreateExport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_RETY_FOFI.xlsx");

                //chọn đường dẫn
                saveFileDialog1.FileName = "";
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

                    SarRetyFofiFilter filter = new SarRetyFofiFilter();
                    filter.KEY_WORD = txtKeyword.Text.Trim();

                    var SarRetyFofis = new BackendAdapter(new CommonParam()).Get<List<V_SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_GETVIEW, HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, filter, null);

                    ProcessData(SarRetyFofis, ref store);
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<V_SAR_RETY_FOFI> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<RetyFofiExportADO> glstRetyFofiExport = new List<RetyFofiExportADO>();//all

                foreach (var itemGroup in data)
                {
                    RetyFofiExportADO ado = new RetyFofiExportADO();
                    Mapper.CreateMap<V_SAR_RETY_FOFI, RetyFofiExportADO>();
                    ado = Mapper.Map<V_SAR_RETY_FOFI, RetyFofiExportADO>(itemGroup);
                    ado.NUM_ORDER_STR = itemGroup.NUM_ORDER.ToString();
                    if (itemGroup.IS_REQUIRE == 1)
                        ado.IS_REQUIRE_STR = "x";
                    glstRetyFofiExport.Add(ado);
                }

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", glstRetyFofiExport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void cboTrangthai_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                FillDataToGridControl();
            }
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnExport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridTruongLocBC(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE rowData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Load Grid Truong BC");
                WaitingManager.Show();
                FillDataToControlsForm();
                gridControlFormListClick.DataSource = null;
                this.reportType = rowData;
                CommonParam commonParam = new CommonParam();
                SarReportTypeFilter reportTypeFilter = new SarReportTypeFilter();
                reportTypeFilter.REPORT_TYPE_CODE = rowData.REPORT_TYPE_CODE;
                CommonParam paramCommon = new CommonParam();
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_GETVIEW, ApiConsumers.SarConsumer, reportTypeFilter, paramCommon);
                if (apiResult != null)
                {
                    this.dataRetyFofi = apiResult.Data.OrderBy(o => o.NUM_ORDER).ToList();
                    if (dataRetyFofi != null && dataRetyFofi.Count > 0)
                    {
                        gridControlFormListClick.DataSource = dataRetyFofi;
                        this.currentRetyFofi = dataRetyFofi.First();
                        spinRowNumber.Value = (decimal)((this.currentRetyFofi.ROW_COUNT != null && this.currentRetyFofi.ROW_COUNT >= 1 && this.currentRetyFofi.ROW_COUNT <= 3) ? this.currentRetyFofi.ROW_COUNT : (long?)1);
                        spinColNumber.Value = (decimal)((this.currentRetyFofi.COLUMN_COUNT != null && this.currentRetyFofi.COLUMN_COUNT >= 1 && this.currentRetyFofi.COLUMN_COUNT <= 3) ? this.currentRetyFofi.COLUMN_COUNT : (long?)1);
                        LoadPreview();
                        CreateReportControlByReportType(this.reportType);
                    }
                    else
                    {
                        spinRowNumber.Value = 1;
                        spinColNumber.Value = 1;
                        lcGenerateReportField.Controls.Clear();
                        layoutControlGroupGenerateReportField.Clear();
                        //layoutControlGroupGenerateReportField1.Clear();
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
        private void lcGenerateReportField_ClientSizeChanged(object sender, EventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField_Resize.lcGenerateReportField.Width___runtime size:", ("[" + lciGenerateReportField.Width + ", " + lciGenerateReportField.Height + "],ClientSize=" + lcGenerateReportField.ClientSize + ", MaximumSize=" + lcGenerateReportField.MaximumSize)) + ", design size: [300, 429]");
        }

        private void btnGDeleteClick_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI)gridViewFormListClick.GetFocusedRow();

                    SarRetyFofiFilter filter = new SarRetyFofiFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SARV_SAR_RETY_FOFI_DELETE, ApiConsumers.SarConsumer, data, param);
                        if (success)
                        {
                            LoadDataToGridTruongLocBC(this.reportType);
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.reportType = (SAR_REPORT_TYPE)(gridControlFormList.DataSource as List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>)[dnNavigation.Position];
                if (this.currentRetyFofi != null)
                {
                    ChangedDataRow(this.currentRetyFofi);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(V_SAR_RETY_FOFI data)
        {
            try
            {
                if (data != null)
                {
                    // FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    // EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_Click_1(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.reportType = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)gridviewFormList.GetFocusedRow();
                if (reportType != null)
                {
                    LoadDataToGridTruongLocBC(reportType);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit_1(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            //try
            //{
            //    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //    if (e.RowHandle >= 0)
            //    {
            //        SAR_REPORT_TYPE data = (SAR_REPORT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
            //        if (e.Column.FieldName == "isLock")
            //        {
            //            e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
            //        }
            //        if (e.Column.FieldName == "Delete")
            //        {
            //            e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSession.Warn(ex);
            //}
        }

        private void gridviewFormList_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.SAR_REPORT_TYPE pData = (SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                }

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void gridViewFormListClick_Click_1(object sender, EventArgs e)
        {
            try
            {
                var rowData = (SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI)gridViewFormListClick.GetFocusedRow();
                if (rowData != null)
                {
                    this.currentRetyFofi = rowData;
                    //ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormListClick_CustomRowCellEdit_1(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewFormListClick.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    short isRequire = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewFormListClick.GetRowCellValue(e.RowHandle, "IS_REQUIRE") ?? "").ToString());
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLockClick : btnGUnLockClick);
                    }
                    if (e.Column.FieldName == "BatBuoc")
                    {
                        if (isRequire == 1)
                            e.RepositoryItem = btnGBatBuoc;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewFormListClick_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI pData = (SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControlFormListClick.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormListClick_RowCellStyle_1(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

            //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //if (e.RowHandle >= 0)
            //{
            //    V_SAR_RETY_FOFI data = (V_SAR_RETY_FOFI)((IList)((BaseView)sender).DataSource)[e.RowHandle];
            //    if (e.Column.FieldName == "IS_ACTIVE_STR")
            //    {
            //        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
            //            e.Appearance.ForeColor = Color.Red;
            //        else
            //            e.Appearance.ForeColor = Color.Green;
            //    }
            //}
        }

        private void btnGLockClick_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SAR_RETY_FOFI success = new SAR_RETY_FOFI();
            //bool notHandler = false;
            try
            {

                V_SAR_RETY_FOFI data = (V_SAR_RETY_FOFI)gridViewFormListClick.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_RETY_FOFI data1 = new SAR_RETY_FOFI();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_RETY_FOFI>(HisRequestUriStore.SARV_SAR_RETY_FOFI_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        LoadDataToGridTruongLocBC(this.reportType);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnLockClick_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SAR_RETY_FOFI success = new SAR_RETY_FOFI();
            //bool notHandler = false;
            try
            {
                V_SAR_RETY_FOFI data = (V_SAR_RETY_FOFI)gridViewFormListClick.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_RETY_FOFI data1 = new SAR_RETY_FOFI();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_RETY_FOFI>(HisRequestUriStore.SARV_SAR_RETY_FOFI_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);

                    WaitingManager.Hide();
                    if (success != null)
                    {
                        LoadDataToGridTruongLocBC(this.reportType);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRowNumber_EditValueChanged(object sender, EventArgs e)
        {
            if (spinRowNumber.Value > 3)
            {
                spinRowNumber.Value = 3;
            }
            else if (spinRowNumber.Value < 1)
            {
                spinRowNumber.Value = 1;
            }
        }

        private void spinColNumber_EditValueChanged(object sender, EventArgs e)
        {
            if (spinColNumber.Value > 3)
            {
                spinColNumber.Value = 3;
            }
            else if (spinColNumber.Value < 1)
            {
                spinColNumber.Value = 1;
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridViewFormListClick.PostEditor();
                Inventec.Common.Logging.LogSystem.Warn("Load Save All");
                CommonParam param = new CommonParam();
                List<SAR_RETY_FOFI> data1 = new List<SAR_RETY_FOFI>();

                foreach (var item in (List<V_SAR_RETY_FOFI>)gridViewFormListClick.DataSource)
                {
                    SAR_RETY_FOFI sarRetyFofi = new SAR_RETY_FOFI();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>(sarRetyFofi, item);

                    sarRetyFofi.ROW_COUNT = (spinRowNumber.Value >= 1 && spinRowNumber.Value <= 3) ? (int)spinRowNumber.Value : sarRetyFofi.ROW_COUNT ?? 1;
                    sarRetyFofi.COLUMN_COUNT = (spinColNumber.Value >= 1 && spinColNumber.Value <= 3) ? (int)spinColNumber.Value : sarRetyFofi.COLUMN_COUNT ?? 1;

                    if (sarRetyFofi.WIDTH_RATIO > 3)
                        sarRetyFofi.WIDTH_RATIO = 3;
                    if (sarRetyFofi.WIDTH_RATIO < 1)
                        sarRetyFofi.WIDTH_RATIO = 1;
                    if (sarRetyFofi.ROW_INDEX > sarRetyFofi.ROW_COUNT * sarRetyFofi.COLUMN_COUNT || sarRetyFofi.ROW_INDEX == null || sarRetyFofi.ROW_INDEX < 1)
                    {
                        sarRetyFofi.ROW_INDEX = sarRetyFofi.ROW_COUNT * sarRetyFofi.COLUMN_COUNT;
                    }
                    Inventec.Common.Logging.LogSystem.Warn("Add Data1");
                    data1.Add(sarRetyFofi);
                }
                Inventec.Common.Logging.LogSystem.Warn("Update resultData");
                var resultData = new BackendAdapter(param).Post<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>>(HisRequestUriStore.SARV_SAR_RETY_FOFI_UPDATELIST, ApiConsumers.SarConsumer, data1, param);
                
                if (resultData != null && resultData.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("resultData != null");
                    //FillDataToControlsForm();
                    LoadDataToGridTruongLocBC(this.reportType);
                    MessageManager.Show(this, param, true);
                }
                else
                    Inventec.Common.Logging.LogSystem.Warn("resultData == null");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAll_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                LoadPreview();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            LoadPreview();
        }

        private void LoadPreview()
        {
            gridControlPreview.DataSource = null;
            InitListPreview();
            gridControlPreview.DataSource = listPreview;
            gridControlPreview.RefreshDataSource();
            Column3.Visible = true;
            Column2.Visible = true;
            Column1.Visible = true;

            if (spinColNumber.Value == 2)
            {
                Column3.Visible = false;
            }
            if (spinColNumber.Value == 1)
            {
                Column2.Visible = false;
                Column3.Visible = false;
            }
        }

        private void InitListPreview()
        {
            listPreview.Clear();
            for (int i = 0; i < spinRowNumber.Value; i++)
            {
                InitListPreview a = new InitListPreview();
                a.gridColumn1 = (int)(1 + spinColNumber.Value * i);
                a.gridColumn2 = (int)(2 + spinColNumber.Value * i);
                a.gridColumn3 = (int)(3 + spinColNumber.Value * i);
                listPreview.Add(a);
            }
        }

    }
}
