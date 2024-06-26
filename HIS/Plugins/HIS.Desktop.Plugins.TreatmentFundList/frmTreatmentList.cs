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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TreatmentFundList.FundPay;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFundList
{
    public partial class frmTreatmentList : HIS.Desktop.Utility.FormBase
    {
        //V_HIS_TREATMENT_FEE_2D treatment = null;
        List<HIS_FUND> listHisFund = null;
        long FundId = 0;
        string loginName = null;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        bool isCheckAll;
        BarManager baManager = null;
        V_HIS_TREATMENT_FEE_2D curentTreatment = null;
        //PopupMenuProcessor popupMenuProcessor = null;

        int positionHandleControlInfo = -1;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmTreatmentList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                //Base.ResourceLangManager.InitResourceLanguageManager();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTreatmentList(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                //Base.ResourceLangManager.InitResourceLanguageManager();
                this.FundId = data;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTreatmentList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                SetDefaultValueControl();
                LoadCheckColumn();
                FillDataToGrid();
                //HisConfigCFG.LoadConfig();
                txtTreatmentCodeFind.Focus();
                txtTreatmentCodeFind.SelectAll();
                WaitingManager.Hide();
                ValidControl();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCheckColumn()
        {
            try
            {
                GridColumn col = this.gridViewtreatmentList.Columns.AddField("Check");
                col.Visible = true;
                col.VisibleIndex = 0;
                col.Width = 31;
                col.FieldName = "Check";
                col.OptionsColumn.AllowEdit = true;
                col.Caption = "";
                col.Image = this.imageCollectionCheck.Images[0];
                col.ImageAlignment = StringAlignment.Center;
                col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col.OptionsFilter.AllowFilter = false;
                col.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                col.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidationSingleControl(this.cboFund);
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
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.IsRequired = true;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyword.Text = "";
                txtPatientCodeFind.Text = "";
                txtTreatmentCodeFind.Text = "";

                SetDefaultControlFund();
                SetDefaultControlDateTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControlFund()
        {
            try
            {
                FilldatatocomboFund();
                if (this.FundId > 0)
                {
                    this.cboFund.EditValue = this.FundId;
                }
                //dtCreateTimeFrom.EditValue = null;
                //dtCreateTimeTo.EditValue = null;
                //if (accountBook != null || (accountBook == null && treatment == null))
                //{
                //dtCreateTimeFrom.DateTime = DateTime.Now;
                //    dtCreateTimeTo.DateTime = DateTime.Now;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FilldatatocomboFund()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisFundFilter filter = new HisFundFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listHisFund = new BackendAdapter(param).Get<List<HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FUND_CODE", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("FUND_NAME", "", 400, 1, true));

                ControlEditorADO controlEditorADO = new ControlEditorADO("FUND_NAME", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(this.cboFund, listHisFund, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultControlDateTime()
        {
            try
            {
                dtCreateTimeFrom.EditValue = null;
                dtCreateTimeTo.EditValue = null;
                //if (accountBook != null || (accountBook == null && treatment == null))
                //{
                //    dtCreateTimeFrom.DateTime = DateTime.Now;
                //    dtCreateTimeTo.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                FillDataToGridTreatment(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, (int)ConfigApplications.NumPageSize, this.gridControlTreatmentList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                List<V_HIS_TREATMENT_FEE_2D> listTreatment = new List<V_HIS_TREATMENT_FEE_2D>();
                gridControlTreatmentList.DataSource = null;
                positionHandleControlInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTreatmentFeeView2Filter filter = new HisTreatmentFeeView2Filter();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (FundId > 0)
                {
                    filter.FUND_ID = FundId;
                }
                else
                {
                    filter.HAS_FUND_ID = true;
                }

                if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.IN_DATE_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.IN_DATE_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (!String.IsNullOrEmpty(txtTreatmentCodeFind.Text))
                {
                    string code = String.Format("{0:000000000000}", Convert.ToInt64(txtTreatmentCodeFind.Text.Trim()));
                    txtTreatmentCodeFind.Text = code;
                    filter.TREATMENT_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtPatientCodeFind.Text))
                {
                    string code = String.Format("{0:0000000000}", Convert.ToInt64(txtPatientCodeFind.Text.Trim()));
                    txtPatientCodeFind.Text = code;
                    filter.PATIENT_CODE__EXACT = code;
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_FEE_2>>("api/HisTreatment/GetFeeView2", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    var listTm= (List<V_HIS_TREATMENT_FEE_2>)result.Data;
                    foreach (var item in listTm)
                    {
                        if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                        {
                            var KEY_WORD = txtKeyword.Text.Trim().ToLower();
                            if (!(item.TREATMENT_CODE.ToLower().Contains(KEY_WORD) || item.TDL_PATIENT_CODE.ToLower().Contains(KEY_WORD) || item.TDL_PATIENT_DOB.ToString().ToLower().Contains(KEY_WORD) || item.TDL_PATIENT_NAME.ToLower().Contains(KEY_WORD) || item.TDL_PATIENT_GENDER_NAME.ToLower().Contains(KEY_WORD) || item.FUND_NAME.ToLower().Contains(KEY_WORD)))
                            {
                                continue;
                            };
 
                        }

                        V_HIS_TREATMENT_FEE_2D tm = new V_HIS_TREATMENT_FEE_2D();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_FEE_2D>(tm, item);
                        listTreatment.Add(tm);
                    }
                    rowCount = (listTreatment == null ? 0 : listTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTreatmentList.BeginUpdate();
                gridControlTreatmentList.DataSource = listTreatment;
                gridControlTreatmentList.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGrid();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCodeFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCodeFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCodeFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCodeFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "FUND_SEND_FILE_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.FUND_SEND_FILE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "FUND_PAY_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.FUND_PAY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "FUND_STATUS")
                        {
                            e.Value = "";
                            if (data.FUND_PAY_TIME != null)
                            {
                                e.Value = "Đã thanh toán";
                            }
                            else
                            {
                                e.Value = "Chưa thanh toán";
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

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT_FEE_2D data = (V_HIS_TREATMENT_FEE_2D)this.gridViewtreatmentList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_FUND_PAY")
                    {
                        if (data.FUND_PAY_TIME != null)
                        {
                            e.RepositoryItem = repositoryItembtnFundUnPay;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItembtnFunPay;
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.FUND_PAY_TIME != null)
                        {
                            e.Appearance.ForeColor = Color.Blue; //Đã đánh dấu quỹ thanh toán=> Màu xanh nước biển
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Red; //Chưa đánh dấu quỹ thanh toán => Màu đỏ
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
                txtTreatmentCodeFind.Focus();
                txtTreatmentCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Refresh()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void bbtnRCRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFocusTreatmentCode_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCodeFind.Focus();
                txtTreatmentCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCFocusPatientTypeCode_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtPatientCodeFind.Focus();
                txtPatientCodeFind.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChangePay(List<V_HIS_TREATMENT_FEE_2D> data, bool IsPay)
        {
            try
            {
                if (data != null)
                {

                    CommonParam param = new CommonParam();
                    bool success = false;
                    List<HIS_TREATMENT> rs = null;
                    List<HIS_TREATMENT> curentTreatments = null;
                    LoadCurrent(data.Select(o => o.ID).ToList(), ref curentTreatments);
                    if (IsPay)
                    {
                        RefeshReference _refeshData = Refresh;
                        frmFundPay form = new frmFundPay(curentTreatments, data.Sum(s => s.TOTAL_BILL_FUND ?? 0), _refeshData);
                        form.ShowDialog();
                    }
                    else
                    {
                        WaitingManager.Show();
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/CancelFundPayTime", ApiConsumers.MosConsumer, curentTreatments, param);
                        WaitingManager.Hide();
                        SessionManager.ProcessTokenLost(param);

                        if (rs != null)
                        {
                            success = true;
                            //foreach (var item in data)
                            //{
                            //    HIS_TREATMENT ro = rs.FirstOrDefault(o => o.ID == item.ID);
                            //    item.FUND_PAY_TIME = ro.FUND_PAY_TIME;
                            //    item.MODIFY_TIME = ro.MODIFY_TIME;
                            //    item.MODIFIER = ro.MODIFIER;
                            //}
                            MessageManager.Show(this, param, success);
                            FillDataToGridTreatment(new CommonParam(start, limit));
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



        /// </summary>
        /// <param name="TreatmentId"></param>
        /// <param name="TreatmentDTO"></param>
        private void LoadCurrent(List<long> TreatmentIds, ref List<MOS.EFMODEL.DataModels.HIS_TREATMENT> TreatmentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.IDs = TreatmentIds;
                WaitingManager.Show();
                TreatmentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                //this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LIST__BTN_FIND", Base.ResourceLangManager.LanguageFrmTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LIST__BTN_REFRESH", Base.ResourceLangManager.LanguageFrmTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TREATMENT_LIST__TXT_KEY_WORD_NULL_VALUE", Base.ResourceLangManager.LanguageFrmTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFundUnPay_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                    if (data != null && data.FUND_PAY_TIME != null)
                    {
                        ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFunPay_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                    if (data != null && data.FUND_PAY_TIME == null)
                    {
                        ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBillDone_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_FEE_2D> data = (List<V_HIS_TREATMENT_FEE_2D>)this.gridViewtreatmentList.DataSource;
                if (data != null)
                {
                    data = data.Where(o => o.Check).ToList();
                }
                if (data != null && data.Count > 0)
                {

                    List<HIS_TREATMENT> curentTreatments = null;
                    LoadCurrent(data.Select(o => o.ID).ToList(), ref curentTreatments);
                    RefeshReference _refeshData = Refresh;
                    frmFundPay form = new frmFundPay(curentTreatments, data.Sum(s => s.TOTAL_BILL_FUND ?? 0), _refeshData);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Xử lý thất bại. Không có hồ sơ được chọn");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFunPay_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                    if (data != null && data.FUND_PAY_TIME == null)
                    {
                        ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFundUnPay_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                    if (data != null && data.FUND_PAY_TIME != null)
                    {
                        ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCtrlD_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_FEE_2D> data = (List<V_HIS_TREATMENT_FEE_2D>)this.gridViewtreatmentList.DataSource;
                if (data != null)
                {
                    data = data.Where(o => o.Check).ToList();
                }
                if (data != null && data.Count > 0)
                {

                    List<HIS_TREATMENT> curentTreatments = null;
                    LoadCurrent(data.Select(o => o.ID).ToList(), ref curentTreatments);
                    RefeshReference _refeshData = Refresh;
                    frmFundPay form = new frmFundPay(curentTreatments, data.Sum(s => s.TOTAL_BILL_FUND ?? 0), _refeshData);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Xử lý thất bại. Không có hồ sơ được chọn");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlInfo == -1)
                {
                    positionHandleControlInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlInfo > edit.TabIndex)
                {
                    positionHandleControlInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

      
        private void gridViewtreatmentList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "Check")
                        {
                            var lstCheckAll = (List<V_HIS_TREATMENT_FEE_2D>)view.DataSource;

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var CheckedNum = lstCheckAll.Where(o => o.Check == true).Count();
                                var Num = lstCheckAll.Count();
                                if ((CheckedNum > 0 && CheckedNum < Num) || CheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = this.imageCollectionCheck.Images[1];
                                }

                                if (CheckedNum == Num)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionCheck.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        item.Check = true;
                                    }
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        item.Check = false;
                                    }
                                    isCheckAll = true;
                                }

                                gridControlTreatmentList.BeginUpdate();
                                gridControlTreatmentList.DataSource = lstCheckAll;
                                gridControlTreatmentList.EndUpdate();


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

        private void repositoryItembtnFunPayE(V_HIS_TREATMENT_FEE_2D data)
        {
            try
            {
                data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                if (data != null && data.FUND_PAY_TIME == null)
                {
                    ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFundUnPayE(V_HIS_TREATMENT_FEE_2D data)
        {
            try
            {
                data = (V_HIS_TREATMENT_FEE_2D)gridViewtreatmentList.GetFocusedRow();
                if (data != null && data.FUND_PAY_TIME != null)
                {
                    ProcessChangePay(new List<V_HIS_TREATMENT_FEE_2D>() { data }, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
