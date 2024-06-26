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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisDhst.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000287.PDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDhst.HisDhst
{
    public partial class frmHisDhst : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        List<V_HIS_DHST> dataDhst;
        internal List<MPS.Processor.Mps000287.PDO.Mps000287ADO> _Mps000287ADOs { get; set; }
        List<long> lstIDcheck = new List<long>();
        List<V_HIS_DHST> lstcheck = new List<V_HIS_DHST>();
        List<V_HIS_DHST> _MessSelected;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        V_HIS_DHST currentData;
        V_HIS_DHST currentDataRightClick;
        List<MessFunctionADO> lstMessFuntADO;
        List<MessFunctionADO> lstMessFuntADOSelecteds;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        long? TreatmentID;

        #endregion

        #region Construct
        public frmHisDhst(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;
                this.TreatmentID = 71035;
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

        public frmHisDhst(Inventec.Desktop.Common.Modules.Module moduleData, long? _TreatmentID)
            : base(moduleData)
        {
            try
            {
                this.TreatmentID = _TreatmentID;
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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
        private void frmHisDhst_Load(object sender, EventArgs e)
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dropDownbtnPrint.Text = Inventec.Common.Resource.Get.Value("HisDhst.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDhst.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisDhst.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource


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
                this.ActionType = GlobalVariables.ActionAdd;

                ResetCombo(checkedCombo);
                checkedCombo.Enabled = false;
                checkedCombo.Enabled = true;
                spinBelly.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBloodPressureMin.EditValue = null;
                spinBreathRate.EditValue = null;
                spinChest.EditValue = null;
                spinHeight.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                //txtKeyword.Text = "";
                spinSPO2.EditValue = null;
                dateExecuteTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit checkedCombo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = checkedCombo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(/*cbo.Properties.DataSource*/null);
                }
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
                //txtKeyword.Focus();
                //txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtHtuName", 0);


                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
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
                Inventec.Core.ApiResultObject<List<V_HIS_DHST>> apiResult = null;
                HisDhstViewFilter filter = new HisDhstViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_FIELD = "EXECUTE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                //TRACKING_ID: Tờ điều trị (lấy ra các bản ghi V_HIS_DHST có TRACKING_ID khác null)
                //IS_IN_SERVICE_REQ: Phiếu khám (lấy ra các bản ghi có IS_IN_SERVICE_REQ = 1)
                //CARE_ID: Phiếu chăm sóc (lấy ra các bản ghi V_HIS_DHST có CARE_ID khác null)


                gridviewFormList.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug("Bat dau goi api HisDhst/GetView filter " + Inventec.Common.Logging.LogUtil.TraceData("", filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_DHST>>(HisRequestUriStore.MOSHIS_DHST_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug("Ket thuc goi api");
                if (apiResult != null)
                {
                    dataDhst = (List<V_HIS_DHST>)apiResult.Data;
                    if (dataDhst != null)
                    {
                        gridviewFormList.GridControl.DataSource = dataDhst;
                        rowCount = (dataDhst == null ? 0 : dataDhst.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                dropDownbtnPrint.Enabled = (rowCount > 0);
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

        private void SetFilterNavBar(ref HisDhstViewFilter filter)
        {
            try
            {
                filter.TREATMENT_ID = this.TreatmentID;
                if (dateFrom.EditValue != null)
                {
                    filter.EXECUTE_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateFrom.DateTime);
                }
                if (dateTo.EditValue != null)
                {
                    filter.EXECUTE_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTo.DateTime.Add(new TimeSpan(1, 0, 0, 0)));
                }
                if (lstMessFuntADOSelecteds != null && lstMessFuntADOSelecteds.Count > 0)
                {
                    filter.RECORD_TYPE_BELONGs = (from m in lstMessFuntADOSelecteds select (MOS.Filter.HisDhstViewFilter.RecordTypeBelong)m.ID).ToList();
                }

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
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
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
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
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
                    V_HIS_DHST pData = (V_HIS_DHST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                    {
                        try
                        {
                            string EXECUTE_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "EXECUTE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(EXECUTE_TIME));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXECUTE_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "BLOOD_PRESURE")
                    {
                        e.Value = pData.BLOOD_PRESSURE_MAX + "/" + pData.BLOOD_PRESSURE_MIN;
                    }
                    else if (e.Column.FieldName == "SPO2_DISPLAY" && pData.SPO2 > 0)
                    {
                        e.Value = pData.SPO2 * 100;
                    }

                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                    this.currentData = rowData;
                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(V_HIS_DHST data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    //btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) && data.CARE_ID == null && data.IS_IN_SERVICE_REQ != "1" && data.TRACKING_ID == null;
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) && data.CARE_ID == null && data.TRACKING_ID == null;
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(V_HIS_DHST data)
        {
            try
            {
                if (data != null)
                {
                    spinPulse.EditValue = data.PULSE;
                    spinBelly.EditValue = data.BELLY;
                    spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = data.BREATH_RATE;
                    spinChest.EditValue = data.CHEST;
                    spinHeight.EditValue = data.HEIGHT;
                    spinTemperature.EditValue = data.TEMPERATURE;
                    spinWeight.EditValue = data.WEIGHT;
                    if (data.SPO2 > 0)
                    {
                        spinSPO2.EditValue = data.SPO2 * 100;
                    }
                    else
                    {
                        spinSPO2.EditValue = null;
                    }
                    if (data.EXECUTE_TIME != null)
                    {
                        dateExecuteTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(data.EXECUTE_TIME));

                    }
                    else
                    {
                        dateExecuteTime.EditValue = null;
                    }
                    if (data.VIR_BMI != null)
                    {
                        lbBMI.Text = data.VIR_BMI.ToString();
                        if (data.VIR_BMI < 16)
                        {
                            lbBMI.Text = lbBMI.Text + " (Gầy độ III)";
                        }
                        else if (16 <= data.VIR_BMI && data.VIR_BMI < 17)
                        {
                            lbBMI.Text = lbBMI.Text + " (Gầy độ II)";
                        }
                        else if (17 <= data.VIR_BMI && data.VIR_BMI < (decimal)18.5)
                        {
                            lbBMI.Text = lbBMI.Text + " (Gầy độ I)";
                        }
                        else if ((decimal)18.5 <= data.VIR_BMI && data.VIR_BMI < 25)
                        {
                            lbBMI.Text = lbBMI.Text + " (Bình thường)";
                        }
                        else if (25 <= data.VIR_BMI && data.VIR_BMI < 30)
                        {
                            lbBMI.Text = lbBMI.Text + " (Thừa cân)";
                        }
                        else if (30 <= data.VIR_BMI && data.VIR_BMI < 35)
                        {
                            lbBMI.Text = lbBMI.Text + " (Béo phì độ III)";
                        }
                        else if (35 <= data.VIR_BMI && data.VIR_BMI < 40)
                        {
                            lbBMI.Text = lbBMI.Text + " (Béo phì độ II)";
                        }
                        else if (40 < data.VIR_BMI)
                        {
                            lbBMI.Text = lbBMI.Text + " (Béo phì độ I)";
                        }
                    }
                    else
                    {
                        lbBMI.Text = " ";
                    }

                    if (data.VIR_BODY_SURFACE_AREA != null)
                    {
                        lbBodySurfaceArea.Text = data.VIR_BODY_SURFACE_AREA.ToString() + " m²";
                    }
                    else
                    {
                        lbBodySurfaceArea.Text = " ";
                    }
                    txtNote.Text = data.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_DHST currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDhstFilter filter = new HisDhstFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>(HisRequestUriStore.MOSHIS_DHST_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_DHST)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HIS_DHST DataDelete = new HIS_DHST();
                        DataDelete.ID = rowData.ID;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DHST_DELETE, ApiConsumers.MosConsumer, DataDelete, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_DHST>();
                            ResetFormData();
                            FillDataToGridControl();
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
                lstIDcheck = new List<long>();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                this.currentData = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                lbBMI.Text = " ";
                this.dateExecuteTime.EditValue = DateTime.Now;
                lbBodySurfaceArea.Text = "";
                dateFrom.EditValue = null;
                dateTo.EditValue = null;
                FillDataToGridControl();
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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_DHST updateDTO = new MOS.EFMODEL.DataModels.HIS_DHST();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DHST>(HisRequestUriStore.MOSHIS_DHST_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DHST>(HisRequestUriStore.MOSHIS_DHST_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_DHST>();
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_DHST data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_DHST) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_DHST)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DHST>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_DHST currentDTO)
        {
            try
            {
                currentDTO.TREATMENT_ID = Convert.ToInt64(TreatmentID);
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                currentDTO.EXECUTE_LOGINNAME = loginName;
                currentDTO.EXECUTE_ROOM_ID = moduleData.RoomId;
                if (dateExecuteTime.Text != "")
                {
                    currentDTO.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateExecuteTime.DateTime);

                }
                else
                {
                    currentDTO.EXECUTE_TIME = null;
                }
                if (spinPulse.EditValue != null)
                {
                    currentDTO.PULSE = Convert.ToInt64(spinPulse.EditValue);
                }
                else
                {
                    currentDTO.PULSE = null;
                }
                if (spinTemperature.EditValue != null)
                {
                    currentDTO.TEMPERATURE = Convert.ToDecimal(spinTemperature.EditValue);
                }
                else
                {
                    currentDTO.TEMPERATURE = null;
                }
                if (spinBloodPressureMax.EditValue != null)
                {
                    currentDTO.BLOOD_PRESSURE_MAX = Convert.ToInt64(spinBloodPressureMax.EditValue);
                }
                else
                {
                    currentDTO.BLOOD_PRESSURE_MAX = null;
                }
                if (spinBloodPressureMin.EditValue != null)
                {
                    currentDTO.BLOOD_PRESSURE_MIN = Convert.ToInt64(spinBloodPressureMin.EditValue);
                }
                else
                {
                    currentDTO.BLOOD_PRESSURE_MIN = null;
                }
                if (spinBreathRate.EditValue != null)
                {
                    currentDTO.BREATH_RATE = Convert.ToDecimal(spinBreathRate.EditValue);
                }
                else
                {
                    currentDTO.BREATH_RATE = null;
                }
                if (spinWeight.EditValue != null)
                {
                    currentDTO.WEIGHT = Convert.ToDecimal(spinWeight.EditValue);
                }
                else
                {
                    currentDTO.WEIGHT = null;
                }
                if (spinHeight.EditValue != null)
                {
                    currentDTO.HEIGHT = Convert.ToDecimal(spinHeight.EditValue);
                }
                else
                {
                    currentDTO.HEIGHT = null;
                }
                if (spinChest.EditValue != null)
                {
                    currentDTO.CHEST = Convert.ToDecimal(spinChest.EditValue);
                }
                else
                {
                    currentDTO.CHEST = null;
                }
                if (spinBelly.EditValue != null)
                {
                    currentDTO.BELLY = Convert.ToDecimal(spinBelly.EditValue);
                }
                else
                {
                    currentDTO.BELLY = null;
                }
                if (spinSPO2.EditValue != null && Convert.ToInt64(spinSPO2.EditValue) >= 1)
                {
                    currentDTO.SPO2 = Convert.ToDecimal(spinSPO2.EditValue) / 100;
                }
                else
                {
                    currentDTO.SPO2 = null;
                }
                if (!string.IsNullOrEmpty(txtNote.Text))
                {
                    currentDTO.NOTE = txtNote.Text;
                }
                else
                {
                    currentDTO.NOTE = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(txtHtuName);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDhst.Resources.Lang", typeof(frmHisDhst).Assembly);

                InitMenuPrint();
                GetDataCombo();

                //khởi tạo checkcombo

                InitCombo(checkedCombo, lstMessFuntADO, "NAME", "ID");

                InitCheck(checkedCombo, SelectionGrid__ServiceReqFunt);


                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {
                lstMessFuntADO = new List<MessFunctionADO>();

                lstMessFuntADO.Add(new MessFunctionADO(1, "Tờ điều trị"));//TRACKING_ID
                lstMessFuntADO.Add(new MessFunctionADO(2, "Phiếu khám"));//IS_IN_SERVICE_REQ
                lstMessFuntADO.Add(new MessFunctionADO(0, "Phiếu chăm sóc"));//CARE_ID
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

        private void SelectionGrid__ServiceReqFunt(object sender, EventArgs e)
        {
            try
            {
                lstMessFuntADOSelecteds = new List<MessFunctionADO>();
                foreach (MessFunctionADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        lstMessFuntADOSelecteds.Add(rv);
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


        #endregion

        void InitMenuPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPrintDHST = new DXMenuItem(Inventec.Common.Resource.Get.Value("HisDhst.btnPrintDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(onClickDHST));
                menu.Items.Add(itemPrintDHST);

                DXMenuItem itemPrintDangKyKham = new DXMenuItem(Inventec.Common.Resource.Get.Value("HisDhst.btnPrintDangKyKham.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(onClickDangKyKham));
                menu.Items.Add(itemPrintDangKyKham);

                dropDownbtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
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
                //txtKeyword.Focus();
                //txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void spinEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtHtuName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //spinEdit1.Focus();
                    //spinEdit1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            onClickDHST(null, null);
        }

        private void onClickDangKyKham(object sender, EventArgs e)
        {
            try
            {
                if (!dropDownbtnPrint.Enabled) return;
                lstIDcheck = new List<long>();
                lstcheck = new List<V_HIS_DHST>();
                int[] rowHandles = gridviewFormList.GetSelectedRows();
                bool valid = (rowHandles != null && rowHandles.Length > 0);
                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_DHST)gridviewFormList.GetRow(i);
                        if (row != null)
                        {
                            lstIDcheck.Add(row.ID);
                            lstcheck.Add(row);
                        }
                    }
                }
                currentDataRightClick = null;
                if (lstIDcheck.Count > 0)
                {
                    PrintProcess("Mps000309");
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn bản ghi chức năng sống.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickDHST(object sender, EventArgs e)
        {
            try
            {
                if (!dropDownbtnPrint.Enabled) return;
                lstIDcheck = new List<long>();
                lstcheck = new List<V_HIS_DHST>();
                int[] rowHandles = gridviewFormList.GetSelectedRows();
                bool valid = (rowHandles != null && rowHandles.Length > 0);
                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_DHST)gridviewFormList.GetRow(i);
                        if (row != null)
                        {
                            lstIDcheck.Add(row.ID);
                            lstcheck.Add(row);
                        }
                    }
                }
                currentDataRightClick = null;
                if (lstIDcheck.Count > 0)
                {
                    PrintProcess("Mps000287");
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn bản ghi chức năng sống.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {
            var rowData = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
            if (lstIDcheck.Find(o => o == rowData.ID) == 0)
            {
                if (lstIDcheck.Count < 10)
                {
                    lstIDcheck.Add(rowData.ID);
                    lstcheck.Add(rowData);
                }
                else
                {
                    MessageBox.Show("Giới hạn chọn 10 bản ghi.");
                }
            }
            else
            {
                lstIDcheck.Remove(lstIDcheck.Find(o => o == rowData.ID));
                lstcheck.Remove(lstcheck.Find(o => o.ID == rowData.ID));
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    V_HIS_DHST data = null;
                    if (e.RowHandle > -1)
                    {
                        var index = gridviewFormList.GetDataSourceRowIndex(e.RowHandle);
                        data = (V_HIS_DHST)((IList)((BaseView)sender).DataSource)[index];
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if ((data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) && data.CARE_ID == null && data.IS_IN_SERVICE_REQ != "1" && data.TRACKING_ID == null)
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else
                        {
                            e.RepositoryItem = btnEEdit;
                        }
                    }
                    else if (e.Column.FieldName == "TRACKING_ID_HAVE")
                    {
                        if (data.TRACKING_ID != null)
                        {
                            e.RepositoryItem = btnTrackingIdHave;
                        }
                    }
                    else if (e.Column.FieldName == "CARE_ID_HAVE")
                    {
                        if (data.CARE_ID != null)
                        {
                            e.RepositoryItem = btnCareIdHave;
                        }
                    }
                    else if (e.Column.FieldName == "IS_IN_SERVICE_REQ_TRUE")
                    {
                        if (data.IS_IN_SERVICE_REQ == "1")
                        {
                            e.RepositoryItem = btnIsInServiceReq;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dateTo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dateTo.Properties.Buttons[1].Visible = true;
                    dateTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateFrom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dateFrom.Properties.Buttons[1].Visible = true;
                    dateFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region printf
        void PrintProcess(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                switch (printType)
                {
                    case "Mps000287":
                        richEditorMain.RunPrintTemplate("Mps000287", DelegateRunPrinter);
                        break;
                    case "Mps000293":
                        richEditorMain.RunPrintTemplate("Mps000293", DelegateRunPrinter);
                        break;
                    case "Mps000309":
                        richEditorMain.RunPrintTemplate("Mps000309", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000287":
                        LoadDHST(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000293":
                        Mps000293(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000309":
                        Mps0000309Process(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
                currentDataRightClick = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadDHST(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                _Mps000287ADOs = new List<MPS.Processor.Mps000287.PDO.Mps000287ADO>();
                var _WorkPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.moduleData.RoomId);//dang lam viec
                LoadMpsAdo();

                var chartControl1 = ChartDhstProcess.GenerateChartImage(LoadListChartADO());
                var chartControl2 = ChartDhstProcess.GenerateChartImageAll(LoadListChartADO());
                //// Lấy thông tin bệnh nhân
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = this.TreatmentID;
                var _Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();
                MOS.Filter.HisTreatmentBedRoomViewFilter treatmentbedRoomFilter = new HisTreatmentBedRoomViewFilter();
                treatmentbedRoomFilter.TREATMENT_ID = this.TreatmentID;
                var _TreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, treatmentbedRoomFilter, param).FirstOrDefault();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData.RoomId);
                MPS.Processor.Mps000287.PDO.Mps000287PDO mps000287PDO = new MPS.Processor.Mps000287.PDO.Mps000287PDO(
                    _Treatment,
                    _Mps000287ADOs,
                    _WorkPlace,
                    _TreatmentBedRoom,
                    ChartDhstProcess.GetChartImage(chartControl1, 0),
                    ChartDhstProcess.GetChartImage(chartControl1, 1),
                    ChartDhstProcess.GetChartImageAll(chartControl2)
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000287PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "")
                    {
                        EmrInputADO = inputADO,
                        saveFilePath = @"E:\test"
                    };
                }
                else
                {
                    //MPS.ProcessorBase.PrintConfig.IsExportExcel = true;
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000287PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "", 1, false, true)
                    {
                        EmrInputADO = inputADO,
                        isAllowExport = true
                    };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<ChartADO> LoadListChartADO()
        {
            List<ChartADO> lstChartAdo = new List<ChartADO>();

            List<V_HIS_DHST> lst = lstcheck.OrderBy(o => o.EXECUTE_TIME).ToList();
            foreach (var dhst in lst)
            {
                ChartADO chartAdo = new ChartADO();
                chartAdo.Date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dhst.EXECUTE_TIME.ToString());
                chartAdo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dhst.EXECUTE_TIME ?? 0);
                chartAdo.PULSE = dhst.PULSE;
                chartAdo.TEMPERATURE = dhst.TEMPERATURE;
                chartAdo.BLOOD_PRESSURE_MAX = dhst.BLOOD_PRESSURE_MAX;
                lstChartAdo.Add(chartAdo);
            }
            return lstChartAdo;
        }

        private void LoadMpsAdo()
        {
            List<V_HIS_DHST> lst = lstcheck.OrderBy(o => o.EXECUTE_TIME).ToList();
            if (lst != null && lst.Count > 0)
            {
                foreach (var dhst in lst)
                {
                    Mps000287ADO ado = new Mps000287ADO(dhst);
                    _Mps000287ADOs.Add(ado);
                }
            }
        }

        #endregion

        private void dateExecuteTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinPulse.Focus();
            }
        }

        private void spinPulse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinTemperature.Focus();
            }
        }

        private void spinTemperature_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinBloodPressureMax.Focus();
            }
        }

        private void spinBloodPressureMax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinBloodPressureMin.Focus();
            }
        }

        private void spinBloodPressureMin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinBreathRate.Focus();
            }
        }

        private void spinBreathRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                spinWeight.Focus();
            }
        }

        private void spinWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinHeight.Focus();
            }

        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinChest.Focus();
            }

        }

        private void spinChest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinBelly.Focus();
            }

        }

        private void spinBelly_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {


        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                fillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataToBmiAndLeatherArea()
        {
            decimal bmi = 0;
            if (spinHeight.Value != null && spinHeight.Value != 0)
            {
                bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
            }
            double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
            lbBMI.Text = Math.Round(bmi, 2) + "";
            lbBodySurfaceArea.Text = Math.Round(leatherArea, 2) + "";
            if (bmi < 16)
            {
                lbBMI.Text = lbBMI.Text + " (Gầy độ III)";
            }
            else if (16 <= bmi && bmi < 17)
            {
                lbBMI.Text = lbBMI.Text + " (Gầy độ II)";
            }
            else if (17 <= bmi && bmi < (decimal)18.5)
            {
                lbBMI.Text = lbBMI.Text + " (Gầy độ I)";
            }
            else if ((decimal)18.5 <= bmi && bmi < 25)
            {
                lbBMI.Text = lbBMI.Text + " (Bình thường)";
            }
            else if (25 <= bmi && bmi < 30)
            {
                lbBMI.Text = lbBMI.Text + " (Thừa cân)";
            }
            else if (30 <= bmi && bmi < 35)
            {
                lbBMI.Text = lbBMI.Text + " (Béo phì độ III)";
            }
            else if (35 <= bmi && bmi < 40)
            {
                lbBMI.Text = lbBMI.Text + " (Béo phì độ II)";
            }
            else if (40 < bmi)
            {
                lbBMI.Text = lbBMI.Text + " (Béo phì độ I)";
            }

        }

        private void spinBelly_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spinSPO2.Focus();
                spinSPO2.SelectAll();
            }
        }

        void Print_Grid_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Print_Grid_Click. 1");
                if (e.Item is BarButtonItem /*&& this.prescriptionPrint != null*/)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Print_Grid_Click. 2____" + e.Item.Tag);
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);
                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.Mps000309:
                            PrintProcess("Mps000309");
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Mps000287:
                            PrintProcess("Mps000287");
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Mps000293:
                            PrintProcess("Mps000293");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Print_Grid_Click. 3___" + e.Item.GetType().ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Printf_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (gridviewFormList.FocusedRowHandle >= 0)
                {
                    WaitingManager.Hide();
                    currentDataRightClick = (V_HIS_DHST)gridviewFormList.GetRow(gridviewFormList.FocusedRowHandle);
                    if (currentDataRightClick != null)
                    {
                        PrintPopupMenuProcessor PrintPopupMenuProcessor = new PrintPopupMenuProcessor(Print_Grid_Click, barManager1, this.TreatmentID ?? 0, currentDataRightClick, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                        PrintPopupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSPO2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (btnAdd.Enabled)
                    //{
                    //    btnAdd.Focus();
                    //}
                    //else
                    //{
                    //    btnEdit.Focus();
                    //}
                    txtNote.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps0000309Process(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                V_HIS_DHST _hisDHST = null;
                if (currentDataRightClick != null)
                {
                    _hisDHST = currentDataRightClick;
                }
                else
                {
                    List<V_HIS_DHST> lst = lstcheck.OrderByDescending(o => o.EXECUTE_TIME).ToList();
                    if (lst != null && lst.Count > 0)
                    {
                        _hisDHST = lst.First();
                    }
                }

                if (_hisDHST == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong co du lieu dau hieu sinh ton____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _hisDHST), _hisDHST));
                    return;
                }

                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.ID = this.TreatmentID;
                var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();

                MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                patientViewFilter.ID = treatment.PATIENT_ID;
                var patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientViewFilter, param).FirstOrDefault();

                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = this.TreatmentID;
                var patientTypeAlter = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == (treatment.IN_TREATMENT_TYPE_ID ?? 0));
                if (treatmentType != null && patientTypeAlter != null)
                {
                    patientTypeAlter.TREATMENT_TYPE_ID = treatmentType.ID;
                    patientTypeAlter.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    patientTypeAlter.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }

                HIS_DHST dhst = new HIS_DHST();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DHST>(dhst, _hisDHST);

                MPS.Processor.Mps000309.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000309.PDO.SingleKeyValue()
                {
                    LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()
                };

                MPS.Processor.Mps000309.PDO.Mps000309PDO rdo = new MPS.Processor.Mps000309.PDO.Mps000309PDO(
                    patient,
                    patientTypeAlter,
                    dhst,
                    treatment,
                    singleKeyValue
                    );

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatment != null ? treatment.TREATMENT_CODE : "", printTypeCode, this.moduleData.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000293(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var _hisDHST = (V_HIS_DHST)gridviewFormList.GetFocusedRow();
                if (_hisDHST == null)
                    return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.ID = this.TreatmentID;
                var _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData.RoomId);
                MPS.Processor.Mps000293.PDO.Mps000293PDO mps000293PDO = new MPS.Processor.Mps000293.PDO.Mps000293PDO(
                    _Treatment,
                    _hisDHST
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000293PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "")
                    {
                        EmrInputADO = inputADO
                    };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000293PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "", 1, false, true)
                    {
                        EmrInputADO = inputADO
                    };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownbtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                dropDownbtnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void checkedCombo_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string mess = "";
                if (lstMessFuntADOSelecteds != null && lstMessFuntADOSelecteds.Count > 0)
                {
                    foreach (var item in lstMessFuntADOSelecteds)
                    {
                        mess += item.NAME + ", ";
                    }
                }
                e.DisplayText = mess;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dateExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dateExecuteTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;

                    fillDataToBmiAndLeatherArea();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
