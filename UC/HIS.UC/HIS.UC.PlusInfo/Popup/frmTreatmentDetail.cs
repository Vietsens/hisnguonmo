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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.UC.PlusInfo.Popup
{
    public partial class frmTreatmentDetail : HIS.Desktop.Utility.FormBase
    {
        long treatmentID;
        V_HIS_TREATMENT_3 currentTreatment;
        HIS_DHST currentDhst;
        V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter;
        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;
        List<V_HIS_SERE_SERV_5> listSereServ = new List<V_HIS_SERE_SERV_5>();
        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill;
        public frmTreatmentDetail()
        {
            InitializeComponent();
        }

        public frmTreatmentDetail(long treatmentID)
        {
            InitializeComponent();
            try
            {
                SetCaptionByLanguageKey();
                this.treatmentID = treatmentID;
                Task.Run(() => InitSereServTree()).Wait();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmTreatmentDetail
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                 ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(frmTreatmentDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl1.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl4.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox3.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.groupBox3.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl7.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl3.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.groupBox2.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl6.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem18.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem19.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem21.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem22.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem23.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem24.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem27.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem28.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem29.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem30.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem31.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem31.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem32.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem25.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem26.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem35.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem35.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem36.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl2.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.groupBox1.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControl5.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem5.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem6.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem6.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem7.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem8.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem8.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem9.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem10.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem11.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem12.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem13.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem14.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem14.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem15.OptionsToolTip.ToolTip",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem15.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem16.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.layoutControlItem20.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn1.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn2.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn3.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn4.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn5.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.treeListColumn6.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTreatmentDetail.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitSereServTree()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitSereServTree => 1");
                this.ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsShowForRegisterV2 = true;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();         
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_CustomUnboundColumnData = sereServTree_CustomUnboundColumnData;
                ado.LayoutSereServExpend = "Hao phí";
                ado.IsAutoWidth = true;
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn("Tên dịch vụ", "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0; 
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn("SL", "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn("Đơn giá", "VIR_PRICE_DISPLAY", 70, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn("Thành tiền", "VIR_TOTAL_PRICE_DISPLAY", 75, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column Bảo hiểm chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn("Bảo hiểm trả", "VIR_TOTAL_HEIN_PRICE_DISPLAY", 75, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn("Bệnh nhân trả", "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 80, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn("VAT (%)", "VAT", 50, false);
                virVatRatioCol.VisibleIndex = 6;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn("Mã dịch vụ", "TDL_SERVICE_CODE", 60, false);
                serviceCodeCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn("Chiết khấu", "DISCOUNT_DISPLAY", 70, false);
                virDiscountCol.VisibleIndex = 8;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn("Hao phí", "IsExpend", 55, false);
                virIsExpendCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                //Column mã giao dịch
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);

                this.ucSereServTree = (UserControl)this.ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panel1.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void sereServTree_CustomUnboundColumnData(SereServADO data, TreeListCustomColumnDataEventArgs e)
		{
            try
            {
                if (e.IsGetData && e.Row != null)
                {
                    SereServADO rowData = e.Row as SereServADO;
                    if (rowData != null)
                    {
                        if (!e.Node.HasChildren)
                        {

                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {

                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_HEIN_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {

                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_PATIENT_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                            {

                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.DISCOUNT ?? 0), ConfigApplications.NumberSeperator);
                            }                           
                        }
                        else
                        {

                            if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_PRICE_DISPLAY");
                                e.Value = e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_HEIN_PRICE_DISPLAY");
                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_HEIN_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                            else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                this.GetTotalPriceOfChildChoice(rowData, e.Node.Nodes, "VIR_TOTAL_PATIENT_PRICE_DISPLAY");
                                e.Value = Inventec.Common.Number.Convert.NumberToString((rowData.VIR_TOTAL_PATIENT_PRICE ?? 0), ConfigApplications.NumberSeperator);
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTotalPriceOfChildChoice(SereServADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren)
                        {

                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                        else if (item.HasChildren)
                        {

                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                    }
                }
                if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                {

                    data.VIR_TOTAL_PRICE = totalChoicePrice;
                }
                else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                {

                    data.VIR_TOTAL_HEIN_PRICE = totalChoicePrice;
                }
                else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                {

                    data.VIR_TOTAL_PATIENT_PRICE = totalChoicePrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, CustomDrawNodeCellEventArgs e)
		{
            try
            {
				if (data != null && e.Node.HasChildren && e.Column.FieldName != "VAT")
				{
					e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
				}			
			}
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void frmHisTreatmentDetailGOV_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();            
                CheckDetailClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private async void CheckDetailClick()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentView3Filter filter = new HisTreatmentView3Filter();
                filter.ID = treatmentID;
                var dtTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>("api/HisTreatment/GetView3", ApiConsumers.MosConsumer, filter, param);
                if(dtTreatment!=null && dtTreatment.Count > 0)
				{
                    currentTreatment = dtTreatment[0];
                    HisDhstFilter Dhstfilter = new HisDhstFilter();
                    Dhstfilter.TREATMENT_ID = currentTreatment.ID;
                    var dtDhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, Dhstfilter, param);
                    if(dtDhst!=null && dtDhst.Count > 0)
					{
                        currentDhst = dtDhst.OrderByDescending(o=>o.ID).ToList()[0];
					}

                    HisPatientTypeAlterViewFilter patientfilter = new HisPatientTypeAlterViewFilter();
                    patientfilter.TREATMENT_ID = currentTreatment.ID;
                    var dtPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientfilter, param);
                    if (dtPatient != null && dtPatient.Count > 0)
                    {
                        currentPatientTypeAlter = dtPatient.OrderByDescending(o => o.ID).ToList()[0];
                    }
                    FillData(currentTreatment);
                    BindTreePlus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillData(V_HIS_TREATMENT_3 data)
        {
            try
            {
                if (data != null)
                {
                    txtMaDieuTri.Text = data.TREATMENT_CODE;
                    txtMaBenhNhan.Text = data.TDL_PATIENT_CODE;
                    txtMaTheBHYT.Text = data.TDL_HEIN_CARD_NUMBER;
                    txtKhuVuc.Text = currentPatientTypeAlter != null ? currentPatientTypeAlter.LIVE_AREA_CODE : "";
                    txtKCBBanDau.Text = currentPatientTypeAlter != null && !string.IsNullOrEmpty(currentPatientTypeAlter.HEIN_MEDI_ORG_CODE) ? (currentPatientTypeAlter.HEIN_MEDI_ORG_CODE + " - " + currentPatientTypeAlter.HEIN_MEDI_ORG_NAME) : "";
                    txtHoTen.Text = data.TDL_PATIENT_NAME;
                    txtDiaChi.Text = data.TDL_PATIENT_ADDRESS;
                    txtNgaySinh.Text = data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    txtGioiTinh.Text = data.TDL_PATIENT_GENDER_NAME;
                    txtTenChaMe.Text = (data.TDL_PATIENT_FATHER_NAME ?? data.TDL_PATIENT_MOTHER_NAME) ?? data.TDL_PATIENT_RELATIVE_NAME;
                    txtHanTu.Text = currentPatientTypeAlter != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) : "";
                    txtHanDen.Text = currentPatientTypeAlter != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) : "";

                    //txtMienCUngChiTra.Text = data.Mieuta;

                    if (currentPatientTypeAlter != null)
                    {
                        if (currentPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE != null)
                        {
                            switch (currentPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE)
                            {
                                case "TT":
                                    txtTuyen.Text = "Thông tuyến";
                                    break;
                                case "HK":
                                    txtTuyen.Text = "Hẹn khám";
                                    break;
                                case "GT":
                                    txtTuyen.Text = "Giới thiệu";
                                    break;
                                case "CC":
                                    txtTuyen.Text = "Cấp cứu";
                                    break;
                                default:
                                    txtTuyen.Text = "";
                                    break;
                            }
                        }
                        else
                        {
                            if (currentPatientTypeAlter.RIGHT_ROUTE_CODE != null)
                            {
                                switch (currentPatientTypeAlter.RIGHT_ROUTE_CODE)
                                {
                                    case "DT":
                                        txtTuyen.Text = "Đúng tuyến";
                                        break;
                                    case "TT":
                                        txtTuyen.Text = "Trái truyến";
                                        break;
                                    default:
                                        txtTuyen.Text = "";
                                        break;
                                }
                            }
                        }
                    }

                    txtCanNang.Text = currentDhst != null ? currentDhst.WEIGHT != null ? currentDhst.WEIGHT.ToString() : "" : "";
                    txtMaKhoa.Text = data.END_DEPARTMENT_CODE;
                    //txtMaBacSi.Text = data.MaBacsy;
                    txtMaBenh.Text = data.ICD_CODE;
                    txtMaBenhKhac.Text = data.ICD_SUB_CODE;
                    txtNgayVao.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.IN_TIME);
                    txtNgayRa.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.OUT_TIME ?? 0);


                    txtMaTaiNan.Text = data.ACCIDENT_HURT_TYPE_BHYT_CODE;

                    txtNoiChuyenDen.Text = !string.IsNullOrEmpty(data.TRANSFER_IN_MEDI_ORG_CODE) ? data.TRANSFER_IN_MEDI_ORG_CODE + " - " + data.TRANSFER_IN_MEDI_ORG_NAME : "";
                    // txtLyDoVaoVien.Text = data.MaLydoVvien;
                    // txtTenBacSi.Text = data.MaBacsy;
                    txtTenBenh.Text = data.ICD_NAME + (!string.IsNullOrEmpty(data.ICD_TEXT)? "; " + data.ICD_TEXT : "");
                    txtSoNgayDieuTri.Text = data.TREATMENT_DAY_COUNT != null ? data.TREATMENT_DAY_COUNT.ToString() : "";
                    txtNgayThanhToan.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.FEE_LOCK_TIME ?? 0);
                    var checkEndType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == data.TREATMENT_END_TYPE_ID);
                    if (checkEndType != null)
                        txtTinhTrangRaVien.Text = checkEndType.TREATMENT_END_TYPE_NAME;
                    txtNoiChuyenDi.Text = !string.IsNullOrEmpty(data.MEDI_ORG_CODE) ? data.MEDI_ORG_CODE + " - " + data.MEDI_ORG_NAME : "";
                    var checkPatiRe = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == data.TRAN_PATI_REASON_ID);
                    if (checkPatiRe != null)
                        txtLyDoChuyen.Text = checkPatiRe.TRAN_PATI_REASON_NAME;
                    var checkPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == data.TRAN_PATI_FORM_ID);
                    if (checkPatiForm != null)
                        txtHinhThucChuyen.Text = checkPatiForm.TRAN_PATI_FORM_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindTreePlus()
        {
			try
			{
                this.listSereServ = new List<V_HIS_SERE_SERV_5>();
                this.dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                Inventec.Common.Logging.LogSystem.Debug("step 6");
                HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                ssFilter.ORDER_DIRECTION = "TDL_INTRUCTION_TIME";
                ssFilter.ORDER_FIELD = "ACS";
                ssFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                this.listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                Inventec.Common.Logging.LogSystem.Debug("step 7");
                this.ssTreeProcessor.Reload(this.ucSereServTree, this.listSereServ);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                dicSereServBill = null;
                listSereServ = null;
                ucSereServTree = null;
                ssTreeProcessor = null;
                currentPatientTypeAlter = null;
                currentDhst = null;
                currentTreatment = null;
                treatmentID = 0;
                this.Load -= new System.EventHandler(this.frmHisTreatmentDetailGOV_Load);
                layoutControlItem34 = null;
                panel1 = null;
                layoutControlItem36 = null;
                txtTuyen = null;
                layoutControlItem35 = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                txtNoiChuyenDi = null;
                txtLyDoChuyen = null;
                txtHinhThucChuyen = null;
                layoutControlItem20 = null;
                txtMaDieuTri = null;
                treeListColumn6 = null;
                treeListColumn5 = null;
                treeListColumn4 = null;
                treeListColumn3 = null;
                treeListColumn2 = null;
                treeListColumn1 = null;
                layoutControlItem17 = null;
                layoutControlItem32 = null;
                layoutControlItem31 = null;
                layoutControlItem30 = null;
                layoutControlItem29 = null;
                layoutControlItem28 = null;
                layoutControlItem27 = null;
                layoutControlItem24 = null;
                layoutControlItem23 = null;
                layoutControlItem22 = null;
                layoutControlItem21 = null;
                layoutControlItem19 = null;
                layoutControlItem18 = null;
                layoutControlGroup5 = null;
                txtCanNang = null;
                txtMaKhoa = null;
                txtMaBenh = null;
                txtMaBenhKhac = null;
                txtNgayVao = null;
                txtMaTaiNan = null;
                txtTenBenh = null;
                txtSoNgayDieuTri = null;
                txtNoiChuyenDen = null;
                txtNgayThanhToan = null;
                txtTinhTrangRaVien = null;
                txtNgayRa = null;
                layoutControl6 = null;
                groupBox2 = null;
                layoutControlItem33 = null;
                layoutControlGroup6 = null;
                layoutControl7 = null;
                groupBox3 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControlItem4 = null;
                Root = null;
                emptySpaceItem1 = null;
                layoutControlItem16 = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                layoutControlItem10 = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlGroup4 = null;
                txtMaBenhNhan = null;
                txtMaTheBHYT = null;
                txtKhuVuc = null;
                txtKCBBanDau = null;
                txtHoTen = null;
                txtDiaChi = null;
                txtHanTu = null;
                txtHanDen = null;
                txtNgaySinh = null;
                txtTenChaMe = null;
                txtMienCUngChiTra = null;
                txtGioiTinh = null;
                layoutControl5 = null;
                groupBox1 = null;
                layoutControl2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControlGroup3 = null;
                layoutControl4 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
