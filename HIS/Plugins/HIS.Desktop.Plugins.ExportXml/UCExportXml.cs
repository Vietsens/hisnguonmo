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
using HIS.UC.SereServTree;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExportXml.Validation;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;

namespace HIS.Desktop.Plugins.ExportXml
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;

        List<V_HIS_HEIN_APPROVAL> listHeinAprroval = new List<V_HIS_HEIN_APPROVAL>();
        List<V_HIS_HEIN_APPROVAL> listSelection = new List<V_HIS_HEIN_APPROVAL>();

        V_HIS_HEIN_APPROVAL currentHeinApproval = null;

        HIS_BRANCH _Branch = null;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        int positionHandleControl = -1;

        List<HIS_BRANCH> branchSelecteds;

        //bool isInitXmlLocalData = false;

        public UCExportXml(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                InitSereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitSereServTree()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsCreateParentNodeWithSereServExpend = false;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCExportXml, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExportXml_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyUCLanguage();
                ValidControl();
                SetDefaultValueControl();
                FillDataToGridHeinApproval();
                LoadDataEmployeestoXML();
                this.InitBranchCheck();
                this.InitComboBranch();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtFindTreatmentCode.Text = "";
                cboFilterType.SelectedIndex = 0;
                txtKeyword.Text = "";
                dtFromExecuteTime.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                dtToExecuteTime.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                this._Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridHeinApproval()
        {
            try
            {
                FillDataToGridHeinApproval(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridHeinApproval, param, (int)ConfigApplications.NumPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridHeinApproval(object param)
        {
            try
            {
                listHeinAprroval = new List<V_HIS_HEIN_APPROVAL>();
                listSelection = new List<V_HIS_HEIN_APPROVAL>();
                gridControlHeinApprovalBhyt.DataSource = null;
                btnExportXml.Enabled = false;
                currentHeinApproval = null;
                FillDateToTreeSereServByHeinApproval();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisHeinApprovalViewFilter filter = new HisHeinApprovalViewFilter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "EXECUTE_TIME";

                if (this.branchSelecteds != null && this.branchSelecteds.Count > 0)
                {
                    filter.BRANCH_IDs = this.branchSelecteds.Select(o => o.ID).ToList();
                }
                else
                    filter.BRANCH_ID = (this._Branch != null) ? this._Branch.ID : 0;

                if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text.Trim()))
                {
                    string code = txtFindTreatmentCode.Text.Trim();
                    if (Char.IsDigit(code.FirstOrDefault()))
                    {
                        if (code.Length < 12)
                        {
                            try
                            {
                                code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                                txtFindTreatmentCode.Text = code;
                                filter.TREATMENT_CODE__EXACT = code;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else
                        {
                            filter.TREATMENT_CODE__EXACT = code;
                        }
                    }
                    else
                    {
                        filter.HEIN_CARD_NUMBER__EXACT = code.ToUpper();
                    }
                }

                if (cboFilterType.SelectedIndex != null && !String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    string[] heinCardArr = txtKeyword.Text.Trim().Split(new char[] { ',' });
                    if (heinCardArr != null && heinCardArr.Length > 0)
                    {
                        foreach (var item in heinCardArr)
                        {
                            if (String.IsNullOrEmpty(item.Trim()))
                                continue;
                            var card = item.Trim().ToUpper();
                            if (cboFilterType.SelectedIndex == 1)
                            {
                                if (filter.HEIN_CARD_NUMBER_PREFIXs == null) filter.HEIN_CARD_NUMBER_PREFIXs = new List<string>();
                                filter.HEIN_CARD_NUMBER_PREFIXs.Add(card);
                            }
                            else if (cboFilterType.SelectedIndex == 2)
                            {
                                if (filter.HEIN_CARD_NUMBER_PREFIX__NOT_INs == null) filter.HEIN_CARD_NUMBER_PREFIX__NOT_INs = new List<string>();
                                filter.HEIN_CARD_NUMBER_PREFIX__NOT_INs.Add(card);
                            }
                            else
                            {
                                if (filter.HEIN_CARD_NUMBER_PREFIXs == null) filter.HEIN_CARD_NUMBER_PREFIXs = new List<string>();
                                filter.HEIN_CARD_NUMBER_PREFIXs.Add(card);
                            }
                        }
                    }
                }

                if (dtFromExecuteTime.EditValue != null && dtFromExecuteTime.DateTime != DateTime.MinValue)
                {
                    filter.EXECUTE_TIME_FROM = Convert.ToInt64(dtFromExecuteTime.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtToExecuteTime.EditValue != null && dtToExecuteTime.DateTime != DateTime.MinValue)
                {
                    filter.EXECUTE_TIME_TO = Convert.ToInt64(dtToExecuteTime.DateTime.ToString("yyyyMMdd") + "235959");
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_HEIN_APPROVAL>>("api/HisHeinApproval/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listHeinAprroval = (List<V_HIS_HEIN_APPROVAL>)result.Data;
                    rowCount = (listHeinAprroval == null ? 0 : listHeinAprroval.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlHeinApprovalBhyt.BeginUpdate();
                gridControlHeinApprovalBhyt.DataSource = listHeinAprroval;
                gridControlHeinApprovalBhyt.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlFindTreatmentCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlFindTreatmentCode()
        {
            try
            {
                //FindTreatmentCodeValidationRule findTreatCodeRule = new FindTreatmentCodeValidationRule();
                //findTreatCodeRule.txtFindTreatmentCode = txtFindTreatmentCode;
                //dxValidationProvider1.SetValidationRule(txtFindTreatmentCode, findTreatCodeRule);
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
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeyUCLanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //Button
                this.btnExportXml.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__BTN_EXPORT_XML", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__BTN_FIND", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);

                //Layout
                this.layoutFromExecuteTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__LAYOUT_FROM_EXECUTE_TIME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.layoutToExecuteTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__LAYOUT_TO_EXECUTE_TIME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);

                //GridControl HeinAprrovalBhyt
                this.gridColumn_HeinApprovalBhyt_Address.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_ADDRESS", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HAS_BIRTH_CERTIFICATE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_APPROVAL_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_CARD_FROM_TIME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_CARD_NUMBER", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinCardToTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_CARD_TO_TIME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_MEDI_ORG_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_HEIN_MEDI_ORG_NAME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_LiveAreaCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_LIVE_AREA_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_RightRouteCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_RIGHT_ROUTE_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_RIGHT_ROUTE_TYPE_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_STT", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_TREATMENT_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_UpToStandardCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_UP_TO_STANDARD_CODE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.gridColumn_HeinApprovalBhyt_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__GRID_HEIN_APPROVAL__COLUMN_VIR_PATIENT_NAME", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);

                // txtnullvalue
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TXT_KEYWORD__NULL_VALUE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                this.txtFindTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__TXT_TREATMENT_CODE_HEIN_CARD__NULL_VALUE", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
                //Cbo Filter type
                cboFilterType.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__COMBO_FILTER__ITEM_ALL", Base.ResourceLangManager.LanguageUCExportXml, cultureLang));
                cboFilterType.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__COMBO_FILTER__ITEM_IN_LIST", Base.ResourceLangManager.LanguageUCExportXml, cultureLang));
                cboFilterType.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__COMBO_FILTER__ITEM_OUT_LIST", Base.ResourceLangManager.LanguageUCExportXml, cultureLang));

                lciCboBranch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXPORT_XML__LCI_CBO_BRANCH", Base.ResourceLangManager.LanguageUCExportXml, cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFind()
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

        public void BtnExportXml()
        {
            try
            {
                btnExportXml_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusTreatmentCode()
        {
            try
            {
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmployeestoXML()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                His.Bhyt.ExportXml.Base.GlobalConfigStore.ListEmployees = new Inventec.Common.Adapter.BackendAdapter(paramGet).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, paramGet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitBranchCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboBranch.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__CboBranch);
                CboBranch.Properties.Tag = gridCheck;
                CboBranch.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = CboBranch.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboBranch.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__CboBranch(object sender, EventArgs e)
        {
            try
            {
                branchSelecteds = new List<HIS_BRANCH>();
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        branchSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBranch()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_BRANCH>();
                if (datas != null)
                {
                    CboBranch.Properties.DataSource = datas;
                    CboBranch.Properties.DisplayMember = "BRANCH_NAME";
                    CboBranch.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = CboBranch.Properties.View.Columns.AddField("BRANCH_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    CboBranch.Properties.PopupFormWidth = 200;
                    CboBranch.Properties.View.OptionsView.ShowColumnHeaders = false;
                    CboBranch.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = CboBranch.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(CboBranch.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboBranch_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_BRANCH rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.BRANCH_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
