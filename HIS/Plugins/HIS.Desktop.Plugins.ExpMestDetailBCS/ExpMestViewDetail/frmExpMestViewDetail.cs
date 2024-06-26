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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExpMestDetailBCS.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventec.Common.ThreadCustom;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.ExpMestDetailBCS.Config;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.ExpMestDetailBCS.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        V_HIS_EXP_MEST _CurrentExpMest { get; set; }//Check lại truyền vào Review
        Inventec.Desktop.Common.Modules.Module moduleData;
        Boolean? enableButton = null;
        long currentMedistockId = 0;
        DelegateSelectData delegateSelectData = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<ExpMestSDO> ExpMestChildFromAggs; // các phiếu con của phiếu lĩnh hiện tại
        List<ExpMestSDO> ExpMestAll;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();

        #endregion

        #region Construct
        public frmExpMestViewDetail(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_EXP_MEST _currentExpMest, DelegateSelectData _delegateSelectData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this._CurrentExpMest = _currentExpMest;
                this.moduleData = moduleData;
                this.delegateSelectData = _delegateSelectData;
                //this.delegateRefreshData = _delegateRefreshData;
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmExpMestViewDetail(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_EXP_MEST _currentExpMest, DelegateSelectData _delegateSelectData, Boolean? _EnableButton)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this._CurrentExpMest = _currentExpMest;
                this.moduleData = moduleData;
                this.delegateSelectData = _delegateSelectData;
                this.enableButton = _EnableButton;
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmExpMestViewDetail_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.VisibleColumnReplace();
                CreateThread();
                LoadDataToControlCommon();
                SetDataToGridControl();
                ShowTab();
                SetCaptionByLanguageKey();
                InitMenuToButtonPrint();
                GetControlAcs();
                EnableControl();


                GetChildExpMestFromXbttExpMest(this._CurrentExpMest.ID);
                GetdExpMestMedicineMaterial();
                LoadDataToGridMediMate(this.ExpMestChildFromAggs, true);
                SetDataToExpMestChildGrid(ExpMestChildFromAggs, false, false);
                gridViewExpMestChild.SelectAll();
                EnableControl(this._CurrentExpMest.EXP_MEST_STT_ID);
                toggleSwitchHaoPhi.IsOn = true;

                SetIcon();
                //CalculateTotalPrice();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private function

        private void VisibleColumnReplace()
        {
            try
            {
                if (this._CurrentExpMest != null
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                    && HisConfigs.Get<string>(HisConfigCFG.BCS_APPROVE_OTHER_TYPE_IS_ALLOW) == "1")
                {
                    gridColumnApprovalMaterialReplaceFor.Visible = true;
                    gridColumnApprovalMedicineReplaceFor.Visible = true;
                }
                else
                {
                    gridColumnApprovalMaterialReplaceFor.Visible = false;
                    gridColumnApprovalMedicineReplaceFor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //tính tổng tiền
        private void CalculateTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                if (this._ExpMestBloods != null && this._ExpMestBloods.Count > 0)//thực xuất máu
                {
                    foreach (var item in this._ExpMestBloods)
                    {
                        totalPrice += (item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
                    }
                }

                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    foreach (var item in _ExpMestMaterials)
                    {
                        totalPrice += (item.PRICE ?? 0) * item.AMOUNT * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
                    }
                }
                if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    foreach (var item in _ExpMestMedicines)
                    {
                        totalPrice += (item.PRICE ?? 0) * item.AMOUNT * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
                    }
                }
                //lblSumPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);

                //Inventec.Common.Logging.LogSystem.Debug("** frmExpMestMedicineViewDetail this._ExpMestMedicines: **: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ExpMestMedicines), this._ExpMestMedicines));
                //Inventec.Common.Logging.LogSystem.Debug("** frmExpMestMedicineViewDetail this._ExpMestBloods: **: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ExpMestBloods), this._ExpMestBloods));
                //Inventec.Common.Logging.LogSystem.Debug("** frmExpMestMedicineViewDetail this._ExpMestMaterials: **: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ExpMestMaterials), this._ExpMestMaterials));
                //Inventec.Common.Logging.LogSystem.Debug("** frmExpMestMedicineViewDetail totalPrice: **: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totalPrice), totalPrice));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void EnableControl()
        {
            try
            {
                Boolean btnApproveStt = true;
                Boolean btnExportStt = true;

                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)

                    btnApproveStt = true;
                else
                    btnApproveStt = false;


                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnExport) != null)
                    btnExportStt = true;
                else
                    btnExportStt = false;

                // nếu module khác truyền vào giá trị Boolean thì ưu tiên theo đó.
                if (this.enableButton != null)
                {
                    btnApproval.Enabled = (this.enableButton ?? true) && btnApproveStt;
                    btnExport.Enabled = (this.enableButton ?? true) && btnExportStt;
                    cboPrint.Enabled = this.enableButton ?? true;
                    return;
                }
                HIS_MEDI_STOCK medistock = null;
                // lấy kho đang làm việc
                if (this.moduleData != null)
                {
                    medistock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.moduleData.RoomId).FirstOrDefault();
                }
                if (medistock == null || this._CurrentExpMest == null)
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = false;
                    return;
                }

                if (medistock != null && medistock.ID == this._CurrentExpMest.MEDI_STOCK_ID
                    && (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                    && btnApproveStt)
                {
                    this.btnApproval.Enabled = true;
                }
                else if (medistock != null && medistock.ID == this._CurrentExpMest.MEDI_STOCK_ID
                    && (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                    && btnApproveStt
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                    && this._CurrentExpMest.IS_NOT_TAKEN != 1
                    )
                    this.btnApproval.Enabled = true;
                else
                    this.btnApproval.Enabled = false;

                if (medistock != null && medistock.ID == this._CurrentExpMest.MEDI_STOCK_ID
                    && this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                    && btnExportStt
                    && this._CurrentExpMest.IS_NOT_TAKEN != 1
                    )
                    this.btnExport.Enabled = true;
                else
                    this.btnExport.Enabled = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshData(object data)
        {
            try
            {
                //Review
                //if (data is HisExpMestApproveResultSDO)
                //{
                //    var expMestApprove = (HisExpMestApproveResultSDO)data;
                //    EnableBottomButton(expMestApprove.ExpMest.EXP_MEST_STT_ID, expMestApprove.ExpMest.MEDI_STOCK_ID, this.currentMedistockId);
                //}
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

        private void ResetControl()
        {
            try
            {
                //lblAdvise.Text = "";
                lblDob.Text = "";
                //lblIcdCode.Text = "";
                //lblIcdName.Text = "";
                //lblIcdText.Text = "";
                //lblInstructionTime.Text = "";
                lblPatientCode.Text = "";
                //lblReqLoginName.Text = "";
                //lblUserTimeFromTo.Text = "";
                lblVirAddress.Text = "";
                lblVirPatientName.Text = "";
                lblRequestRoom.Text = "";
                gridControlRequestMaterial.DataSource = null;
                gridControlRequestMedicine.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToOtherExpmestControl(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest)//Review V_HIS_OTHER_EXP_MEST
        {
            try
            {
                //Review
                if (expMest != null)
                {
                    lblExpMestCode.Text = expMest.EXP_MEST_CODE;
                    lblExpMedistock.Text = expMest.MEDI_STOCK_CODE + " - " + expMest.MEDI_STOCK_NAME;
                    //lblExpTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(expMest.EXP_TIME ?? 0);
                    //lblExpUserName.Text = expMest.EXP_LOGINNAME + " - " + expMest.EXP_USERNAME;
                    lblDescription.Text = expMest.DESCRIPTION;
                    lblExpMestSttName.Text = expMest.EXP_MEST_STT_NAME;
                    //lblApprovalUserName.Text = expMest.APPROVAL_LOGINNAME + " - " + expMest.APPROVAL_USERNAME;
                    //lblExpMestReasonName.Text = expMest.EXP_MEST_REASON_NAME;
                    lblRequestRoom.Text = expMest.REQ_ROOM_CODE + " - " + expMest.REQ_ROOM_NAME;
                    //lblReqLoginName.Text = expMest.REQ_LOGINNAME + " - " + expMest.REQ_USERNAME;
                }
                else
                {
                    lblExpMestCode.Text = "";
                    lblExpMedistock.Text = "";
                    lblExpTime.Text = "";
                    lblExpUserName.Text = "";
                    lblDescription.Text = "";
                    lblExpMestSttName.Text = "";
                    lblApprovalUserName.Text = "";
                    lblExpMestReasonName.Text = "";
                    //lblReqLoginName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load các phiếu xuất cụ thể dựa vào loại xuất đang chọn
        private void LoadSpecificExpMest()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //tính tổng tiền
        //private void CalculateTotalPrice()
        //{
        //    try
        //    {
        //        decimal totalPrice = 0;
        //        if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)//thực xuất máu
        //        {
        //            foreach (var item in this._ExpMestMaterials_Print)
        //            {
        //                totalPrice += (item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
        //            }
        //        }
        //        if (this._ExpMestBloods_Print != null && this._ExpMestBloods_Print.Count > 0)
        //        {

        //            foreach (var item in this._ExpMestBloods_Print)
        //            {
        //                totalPrice += (item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
        //            }
        //        }
        //        if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
        //        {
        //            foreach (var item in this._ExpMestMedicines_Print)
        //            {
        //                totalPrice += (item.PRICE ?? 0) * item.AMOUNT * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        List<HIS_EXP_MEST_BLTY_REQ> ConvertExpMestBltyViewToTable()
        {
            List<HIS_EXP_MEST_BLTY_REQ> expMestBltys = new List<HIS_EXP_MEST_BLTY_REQ>();
            try
            {
                foreach (var item in this._ExpMestBltyReqs_Print)
                {
                    HIS_EXP_MEST_BLTY_REQ expMestBltyReq = new HIS_EXP_MEST_BLTY_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_BLTY_REQ>(expMestBltyReq, item);
                    expMestBltys.Add(expMestBltyReq);
                }
            }
            catch (Exception ex)
            {
                expMestBltys = new List<HIS_EXP_MEST_BLTY_REQ>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return expMestBltys;
        }

        private void ShowTab()
        {
            try
            {
                if (gridControlApprovalMedicine.DataSource == null || gridViewApprovalMedicine.RowCount == 0)
                    TabPageApprovalMedicine.PageVisible = false;
                else
                    TabPageApprovalMedicine.PageVisible = true;

                if (gridControlApprovalMaterial.DataSource == null || gridViewApprovalMaterial.RowCount == 0)
                    TabPageApprovalMaterial.PageVisible = false;
                else
                    TabPageApprovalMaterial.PageVisible = true;

                if (gridControlRequestMedicine.DataSource == null || gridViewRequestMedicine.RowCount == 0)
                    tabPageRequestMedicine.PageVisible = false;
                else
                    tabPageRequestMedicine.PageVisible = true;

                if (gridControlRequestMaterial.DataSource == null || gridViewRequestMaterial.RowCount == 0)
                    tabPageRequestMaterial.PageVisible = false;
                else
                    tabPageRequestMaterial.PageVisible = true;

                if (gridControlRequestExpMestBlood.DataSource == null || gridViewRequestExpMestBlood.RowCount == 0)
                    TabPageRequestBlood.PageVisible = false;
                else
                    TabPageRequestBlood.PageVisible = true;

                if (gridControlAprroveExpMestBlood.DataSource == null || gridViewApproveExpMestBlood.RowCount == 0)
                    TabPageExportBlood.PageVisible = false;
                else
                    TabPageExportBlood.PageVisible = true;

                if (gridControlServiceReqMety.DataSource == null || gridViewServiceReqMety.RowCount == 0)
                    TabPageServiceReqMety.PageVisible = false;
                else
                    TabPageServiceReqMety.PageVisible = true;

                if (gridControlServiceReqMaty.DataSource == null || gridViewServiceReqMaty.RowCount == 0)
                    TabPageServiceReqMaty.PageVisible = false;
                else
                    TabPageServiceReqMaty.PageVisible = true;

                if (gridControlTestService.DataSource == null || gridViewTestService.RowCount == 0)
                    gridViewTestServicelayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                else
                    gridViewTestServicelayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST UpdateExpmest(MOS.EFMODEL.DataModels.HIS_EXP_MEST expMest, V_HIS_EXP_MEST expMestView)
        {
            try
            {
                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = expMest.ID;
                var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new
                CommonParam());
                if (expMests != null && expMests.Count > 0)
                {
                    expMestView = expMests.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return expMestView;
        }

        private void FillDataAfterSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is HisExpMestResultSDO)
                {
                    this._CurrentExpMest = UpdateExpmest(((HisExpMestResultSDO)prescription).ExpMest, this._CurrentExpMest);
                    LoadDataToControlCommon();
                    this.EnableControl();
                    CreateThread();
                    SetDataToGridControl();
                    ShowTab();
                    delegateSelectData(prescription);
                }
                else if (prescription != null && prescription is HIS_EXP_MEST)
                {
                    this._CurrentExpMest = UpdateExpmest((HIS_EXP_MEST)prescription, this._CurrentExpMest);
                    this.EnableControl();
                    CreateThread();
                    SetDataToGridControl();
                    ShowTab();
                    delegateSelectData(prescription);
                }
                else if (prescription != null && prescription is HisExpMestResultSDO)
                {
                    this._CurrentExpMest = UpdateExpmest(((HisExpMestResultSDO)prescription).ExpMest, this._CurrentExpMest);
                    this.EnableControl();
                    CreateThread();
                    SetDataToGridControl();
                    ShowTab();
                    delegateSelectData(prescription);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("prescription is null or not equal TYPE");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region public function
        #endregion

        #region Event handler
        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MEDICINE_1 dataRow = (V_HIS_EXP_MEST_MEDICINE_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "MEDICINE_TYPE_NAME_STR")
                        {
                            if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                                e.Value = String.Format("{0} {1}", dataRow.MEDICINE_TYPE_NAME, dataRow.CONCENTRA);
                            else
                                e.Value = dataRow.MEDICINE_TYPE_NAME;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = dataRow.PRICE ?? 0;
                                decimal amount = dataRow.AMOUNT;
                                decimal vatRatio = (dataRow.VAT_RATIO ?? 0);
                                decimal disCount = dataRow.DISCOUNT ?? 0;
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.IMP_VAT_RATIO) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == 1)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "CONVERT_AMOUNT")
                        {
                            if (dataRow.CONVERT_RATIO.HasValue)
                            {
                                e.Value = dataRow.AMOUNT * dataRow.CONVERT_RATIO.Value;
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

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MATERIAL_1 dataRow = (V_HIS_EXP_MEST_MATERIAL_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = dataRow.PRICE ?? 0;
                                decimal amount = dataRow.AMOUNT;
                                decimal vatRatio = (dataRow.VAT_RATIO ?? 0);
                                decimal disCount = (dataRow.DISCOUNT ?? 0);
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.IMP_VAT_RATIO) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == 1)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "CONVERT_AMOUNT")
                        {
                            if (dataRow.CONVERT_RATIO.HasValue)
                            {
                                e.Value = dataRow.AMOUNT * dataRow.CONVERT_RATIO.Value;
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

        private void gridViewRequestExpMestBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    //MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD dataRow = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApproveExpMestBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD dataRow = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = (dataRow.PRICE ?? 0);
                                decimal amount = 1;
                                decimal vatRatio = (dataRow.VAT_RATIO ?? 0);
                                decimal valueTotal = (price * amount * (1 + vatRatio));
                                e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.IMP_VAT_RATIO) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO nhập", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO xuất", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this._CurrentExpMest != null)
                {
                    if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                            || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                            || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BrowseExportTicket").FirstOrDefault();
                        if (moduleData == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BrowseExportTicket");
                            MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(_CurrentExpMest.ID);
                            listArgs.Add(this.moduleData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataAfterSave);
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                    }
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApproveExpMestBCS").FirstOrDefault();
                        if (moduleData == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ApproveExpMestBCS");
                            MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(_CurrentExpMest.ID);
                            listArgs.Add(this.moduleData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataAfterSave);
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                    }
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                        hisExpMestApproveSDO.ExpMestId = this._CurrentExpMest.ID;
                        hisExpMestApproveSDO.IsFinish = true;
                        hisExpMestApproveSDO.ReqRoomId = this.moduleData.RoomId;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                   "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataAfterSave(rs);
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                    else
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                        hisExpMestApproveSDO.ExpMestId = this._CurrentExpMest.ID;
                        hisExpMestApproveSDO.IsFinish = true;
                        hisExpMestApproveSDO.ReqRoomId = this.moduleData.RoomId;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                   "api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataAfterSave(rs);
                        }

                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (this._CurrentExpMest != null)
                {
                    if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    {
                        bool IsFinish = false;
                        if (this._CurrentExpMest.IS_EXPORT_EQUAL_APPROVE == 1)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (this._CurrentExpMest.IS_EXPORT_EQUAL_APPROVE == null || this._CurrentExpMest.IS_EXPORT_EQUAL_APPROVE != 1)
                        {
                            WaitingManager.Show();
                            List<AmountADO> amountAdo = new List<AmountADO>();

                            if (this._ExpMestMetyReqs_Print != null && this._ExpMestMetyReqs_Print.Count > 0)
                            {
                                foreach (var item in this._ExpMestMetyReqs_Print)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (this._ExpMestMatyReqs_Print != null && this._ExpMestMatyReqs_Print.Count > 0)
                            {
                                foreach (var item in this._ExpMestMatyReqs_Print)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (amountAdo != null && amountAdo.Count > 0)
                            {
                                var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                if (dataAdo != null && dataAdo.Count > 0)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        IsFinish = true;
                                    }
                                }
                                else
                                    IsFinish = true;
                            }

                            WaitingManager.Hide();
                        }

                        WaitingManager.Show();
                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = this._CurrentExpMest.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        sdo.IsFinish = IsFinish;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataAfterSave(apiresult);
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        WaitingManager.Show();
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = this._CurrentExpMest.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        //sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataAfterSave(apiresult);
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                    else
                    {
                        WaitingManager.Show();
                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = this._CurrentExpMest.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataAfterSave(apiresult);
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void gridViewApprovalMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestMedicineSDODetail dataRow = (ExpMestMedicineSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "MEDICINE_TYPE_NAME_STR")
                        {
                            if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                                e.Value = String.Format("{0} {1}", dataRow.MEDICINE_TYPE_NAME, dataRow.CONCENTRA);
                            else
                                e.Value = dataRow.MEDICINE_TYPE_NAME;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = dataRow.PRICE ?? 0;
                                decimal amount = dataRow.AMOUNT;
                                decimal vatRatio = (dataRow.VAT_RATIO ?? 0);
                                decimal disCount = dataRow.DISCOUNT ?? 0;
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.IMP_VAT_RATIO) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == 1)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "PRICE_DISPPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "CONVERT_AMOUNT")
                        {
                            if (dataRow.CONVERT_RATIO.HasValue)
                            {
                                e.Value = dataRow.AMOUNT * dataRow.CONVERT_RATIO.Value;
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

        private void gridViewApprovalMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestMaterialSDODetail dataRow = (ExpMestMaterialSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = dataRow.PRICE ?? 0;
                                decimal amount = dataRow.AMOUNT;
                                decimal vatRatio = (dataRow.VAT_RATIO ?? 0);
                                decimal disCount = (dataRow.DISCOUNT ?? 0);
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToString(valueTotal, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.IMP_VAT_RATIO) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((dataRow.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == 1)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "CONVERT_AMOUNT")
                        {
                            if (dataRow.CONVERT_RATIO.HasValue)
                            {
                                e.Value = dataRow.AMOUNT * dataRow.CONVERT_RATIO.Value;
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

        private void frmExpMestViewDetail_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void gridViewRequestMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int medicineGroupId = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "MEDICINE_GROUP_ID") ?? "").ToString());// là thuốc gây nghiện
                if (medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprovalMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int medicineGroupId = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "MEDICINE_GROUP_ID") ?? "").ToString());// là thuốc gây nghiện
                string replaceFor = (vw.GetRowCellValue(e.RowHandle, "REPLACE_FOR_NAME") ?? "").ToString();// thay the thuoc
                if (medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
                else if (!String.IsNullOrWhiteSpace(replaceFor))
                    e.Appearance.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqMety_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY dataRow = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "USE_TIME_TO_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.USE_TIME_TO ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong MODIFY_TIME_STR", ex);
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

        private void gridViewServiceReqMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY dataRow = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong CREATE_TIME_STR", ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong MODIFY_TIME_STR", ex);
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

        private void bbtnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnApproval.Enabled)
                {
                    btnApproval_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnExport.Enabled)
                {
                    btnExport_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridControlApprovalMaterial_Click(object sender, EventArgs e)
        {

        }

        private void gridViewTestService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERE_SERV dataRow = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "AMOUNT_STR")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "PRICE_STR")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "VAT_RATIO_STR")
                        {
                            e.Value = dataRow.VAT_RATIO * 100;
                        }

                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.MODIFY_TIME ?? 0);

                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.TDL_INTRUCTION_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                cboPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        List<ExpMestSDO> FilterWithSearch(List<ExpMestSDO> expMests)
        {
            List<ExpMestSDO> result = new List<ExpMestSDO>();
            try
            {
                if (expMests == null || expMests.Count == 0)
                {
                    return result;
                }
                result = expMests;

                if (dtInstructionDateFrom.EditValue != null && dtInstructionDateFrom.DateTime != DateTime.MinValue)
                {
                    long instructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInstructionDateFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    result = result.Where(o => o.TDL_INTRUCTION_TIME != null && o.TDL_INTRUCTION_TIME >= instructionTimeFrom).ToList();
                }

                if (dtInstructionDateTo.EditValue != null && dtInstructionDateTo.DateTime != DateTime.MinValue)
                {
                    long instructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInstructionDateTo.EditValue).ToString("yyyyMMdd") + "235959");

                    result = result.Where(o => o.TDL_INTRUCTION_TIME != null && o.TDL_INTRUCTION_TIME <= instructionTimeTo).ToList();
                }

                if (!String.IsNullOrWhiteSpace(txtPatientName.Text))
                {
                    result = result.Where(o => !String.IsNullOrWhiteSpace(o.TDL_PATIENT_NAME)
                        && o.TDL_PATIENT_NAME.ToLower().Contains(txtPatientName.Text.Trim().ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                result = new List<ExpMestSDO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        // lấy các phiếu con từ phiếu xuất bù tủ trực được chọn
        void GetChildExpMestFromXbttExpMest(long xbttExpMestId)
        {
            try
            {
                if (xbttExpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                    expMestViewFilter.XBTT_EXP_MEST_ID = xbttExpMestId;

                    var ExpMestChildFromAggApis = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                    ExpMestChildFromAggs = new List<ExpMestSDO>();
                    foreach (var item in ExpMestChildFromAggApis)
                    {
                        ExpMestSDO ExpMestSDO = new ExpMestSDO(item);
                        ExpMestChildFromAggs.Add(ExpMestSDO);
                    }
                    ExpMestAll = new List<ExpMestSDO>();
                    ExpMestAll.AddRange(ExpMestChildFromAggs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetdExpMestMedicineMaterial()
        {
            try
            {
                if (this.ExpMestChildFromAggs == null || this.ExpMestChildFromAggs.Count == 0)
                {
                    return;
                }
                List<Action> actions = new List<Action>();
                actions.Add(LoadExpMestMedicines);
                actions.Add(LoadExpMestMaterials);
                ThreadCustomManager.MultipleThreadWithJoin(actions);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExpMestMedicines()
        {
            try
            {
                if (this.ExpMestChildFromAggs == null || this.ExpMestChildFromAggs.Count == 0)
                    return;
                List<long> expMestIds = this.ExpMestChildFromAggs.Select(o => o.ID).Distinct().ToList();
                // thêm xuất bù lĩnh
                expMestIds.Add(this._CurrentExpMest.ID);
                CommonParam param = new CommonParam();

                if (expMestIds != null && expMestIds.Count > 0)
                {
                    this.expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    expMestIds = expMestIds.Distinct().ToList();

                    if (expMestIds != null && expMestIds.Count > 0)
                    {
                        int skip = 0;
                        while (expMestIds.Count - skip > 0)
                        {
                            var listIds = expMestIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                            expMestMedicineFilter.EXP_MEST_IDs = listIds;
                            var treat = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                            if (treat != null && treat.Count > 0)
                            {
                                this.expMestMedicines.AddRange(treat);
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

        private void LoadExpMestMaterials()
        {
            try
            {
                if (this.ExpMestChildFromAggs == null || this.ExpMestChildFromAggs.Count == 0)
                    return;
                CommonParam param = new CommonParam();
                List<long> expMestIds = this.ExpMestChildFromAggs.Select(o => o.ID).Distinct().ToList();
                // thêm xuất bù lĩnh
                expMestIds.Add(this._CurrentExpMest.ID);

                if (expMestIds != null && expMestIds.Count > 0)
                {
                    this.expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    expMestIds = expMestIds.Distinct().ToList();

                    if (expMestIds != null && expMestIds.Count > 0)
                    {
                        int skip = 0;
                        while (expMestIds.Count - skip > 0)
                        {
                            var listIds = expMestIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                            hisExpMestMaterialViewFilter.EXP_MEST_IDs = listIds;
                            var treat = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);
                            if (treat != null && treat.Count > 0)
                            {
                                this.expMestMaterials.AddRange(treat);
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

        private void LoadDataToGridMediMate(List<ExpMestSDO> expMestCheckeds, bool isDefault)
        {
            try
            {
                if (expMestCheckeds == null || expMestCheckeds.Count == 0)
                {
                    gridControlMedicineMaterialDetail.DataSource = null;
                    return;
                }
                var ExpMestMatyMetyReqSDODetailsDb = new List<ExpMestMatyMetyReqSDODetail>();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();

                List<long> expMestIds = expMestCheckeds.Select(o => o.ID).ToList();
                // nếu là load lên mặc định (check all các phiếu xuất)
                if (isDefault)
                {
                    expMestIds.Add(this._CurrentExpMest.ID);
                }
                if (this.expMestMedicines != null && this.expMestMedicines.Count > 0)
                {
                    expMestMedicineTemps = this.expMestMedicines.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                    if (expMestMedicineTemps != null && expMestMedicineTemps.Count > 0)
                    {
                        ExpMestMatyMetyReqSDODetailsDb.AddRange(from r in expMestMedicineTemps select new ExpMestMatyMetyReqSDODetail(r));
                    }
                }

                if (this.expMestMaterials != null && this.expMestMaterials.Count > 0)
                {
                    expMestMaterialTemps = this.expMestMaterials.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                    if (expMestMaterialTemps != null && expMestMaterialTemps.Count > 0)
                    {
                        ExpMestMatyMetyReqSDODetailsDb.AddRange(from r in expMestMaterialTemps select new ExpMestMatyMetyReqSDODetail(r));
                    }
                }

                List<ExpMestMatyMetyReqSDODetail> ImpMestMediMateADOTemps = new List<ExpMestMatyMetyReqSDODetail>();

                // group theo lô
                if (!toggleSwitchTheoLo.IsOn && toggleSwitchHaoPhi.IsOn)
                {
                    var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                    {
                        p.IS_MEDICINE,
                        p.MEDI_MATE_TYPE_ID
                    }).ToList();
                    foreach (var itemGroup in dataGroup)
                    {
                        ExpMestMatyMetyReqSDODetail ado = new ExpMestMatyMetyReqSDODetail(itemGroup.FirstOrDefault());
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        // cộng dồn số lô nếu cùng lô
                        //ado.PACKAGE_NUMBER = String.Join(", ", itemGroup.ToList().Where(o => !String.IsNullOrEmpty(o.PACKAGE_NUMBER)).Select(o => o.PACKAGE_NUMBER));
                        string packageNumber = "";
                        foreach (var item in itemGroup.ToList())
                        {
                            if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER) && !packageNumber.Contains(item.PACKAGE_NUMBER))
                                packageNumber += (item.PACKAGE_NUMBER + (itemGroup.ToList().IndexOf(item) < (itemGroup.ToList().Count - 1) ? ", " : ""));
                        }
                        if (!string.IsNullOrEmpty(packageNumber))
                        {
                            packageNumber = packageNumber.Trim().TrimEnd(new char[] { ',' });
                        }
                        ado.PACKAGE_NUMBER = packageNumber;
                        ImpMestMediMateADOTemps.Add(ado);
                    }
                }
                else if (!toggleSwitchTheoLo.IsOn && !toggleSwitchHaoPhi.IsOn)
                {
                    ExpMestMatyMetyReqSDODetailsDb = ExpMestMatyMetyReqSDODetailsDb.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList();
                    var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                    {
                        p.IS_MEDICINE,
                        p.MEDI_MATE_TYPE_ID
                    }).ToList();
                    foreach (var itemGroup in dataGroup)
                    {
                        ExpMestMatyMetyReqSDODetail ado = new ExpMestMatyMetyReqSDODetail(itemGroup.OrderBy(o => o.IS_EXPEND).FirstOrDefault());
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        // cộng dồn số lô nếu cùng lô
                        //ado.PACKAGE_NUMBER = String.Join(", ", itemGroup.ToList().Where(o => !String.IsNullOrEmpty(o.PACKAGE_NUMBER)).Select(o => o.PACKAGE_NUMBER));
                        string packageNumber = "";
                        foreach (var item in itemGroup.ToList())
                        {
                            if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER) && !packageNumber.Contains(item.PACKAGE_NUMBER))
                                packageNumber += (item.PACKAGE_NUMBER + (itemGroup.ToList().IndexOf(item) < (itemGroup.ToList().Count - 1) ? ", " : ""));
                        }
                        if (!string.IsNullOrEmpty(packageNumber))
                        {
                            packageNumber = packageNumber.Trim().TrimEnd(new char[] { ',' });
                        }
                        ado.PACKAGE_NUMBER = packageNumber;
                        ImpMestMediMateADOTemps.Add(ado);
                    }
                }
                else if (toggleSwitchTheoLo.IsOn && toggleSwitchHaoPhi.IsOn)
                {

                    var impMestMediMateADOGroups = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                    {
                        p.IS_MEDICINE,
                        p.MEDI_MATE_ID
                    }).ToList();
                    foreach (var impMestMediMateADOGroup in impMestMediMateADOGroups)
                    {
                        ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.First();
                        impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                        ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                    }
                }
                else
                {
                    //MOS.EFMODEL.DataModels.V_HIS_MEDICINE
                    ExpMestMatyMetyReqSDODetailsDb = ExpMestMatyMetyReqSDODetailsDb.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList();
                    var impMestMediMateADOGroups = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                    {
                        p.IS_MEDICINE,
                        p.MEDI_MATE_ID
                    }).ToList();
                    foreach (var impMestMediMateADOGroup in impMestMediMateADOGroups)
                    {
                        ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.OrderBy(o => o.IS_EXPEND).First();
                        impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                        ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                    }
                }

                ImpMestMediMateADOTemps = (ImpMestMediMateADOTemps != null && ImpMestMediMateADOTemps.Count > 0) ? ImpMestMediMateADOTemps.OrderBy(o => o.MEDI_MATE_TYPE_ID).ThenBy(o => o.IS_MEDICINE).ToList() : ImpMestMediMateADOTemps;

                SetDataToGridControlMedicineMaterial(ImpMestMediMateADOTemps);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDataToGridControlMedicineMaterial(List<ExpMestMatyMetyReqSDODetail> expMestMedicineMaterials)
        {
            try
            {
                gridControlMedicineMaterialDetail.BeginUpdate();
                gridControlMedicineMaterialDetail.DataSource = expMestMedicineMaterials;
                gridControlMedicineMaterialDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetDataToExpMestChildGrid(List<ExpMestSDO> expMests, bool isSearch, bool IsHightLight)
        {
            try
            {
                //if (expMests != null && expMests.Count > 0 && !isSearch)
                //{
                //    // check phiếu xuất bù lĩnh
                //    var checkExistExpMestMedicine = (this.expMestMedicines != null && this.expMestMedicines.Count > 0) ? this.expMestMedicines.FirstOrDefault(o => o.EXP_MEST_ID == this._CurrentExpMest.ID) : null;
                //    var checkExistExpMestMaterial = (this.expMestMaterials != null && this.expMestMaterials.Count > 0) ? this.expMestMaterials.FirstOrDefault(o => o.EXP_MEST_ID == this._CurrentExpMest.ID) : null;

                //    if (checkExistExpMestMedicine != null || checkExistExpMestMaterial != null)
                //    {
                //        ExpMestSDO expMestBuThuocLe = new ExpMestSDO();
                //        var bu = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                //        expMestBuThuocLe.EXP_MEST_TYPE_CODE = bu != null ? bu.EXP_MEST_TYPE_CODE : "";
                //        expMestBuThuocLe.EXP_MEST_TYPE_NAME = bu != null ? bu.EXP_MEST_TYPE_NAME : "";
                //        expMestBuThuocLe.EXP_MEST_CODE = this._CurrentExpMest.EXP_MEST_CODE;
                //        expMestBuThuocLe.AGGR_EXP_MEST_ID = this._CurrentExpMest.AGGR_EXP_MEST_ID;
                //        expMestBuThuocLe.ID = this._CurrentExpMest.ID;
                //        expMestBuThuocLe.TDL_PATIENT_NAME = this._CurrentExpMest.TDL_PATIENT_NAME;
                //        expMestBuThuocLe.TDL_TREATMENT_CODE = this._CurrentExpMest.TDL_TREATMENT_CODE;
                //        expMestBuThuocLe.TDL_PATIENT_DOB = this._CurrentExpMest.TDL_PATIENT_DOB;
                //        expMestBuThuocLe.TDL_PATIENT_GENDER_NAME = this._CurrentExpMest.TDL_PATIENT_GENDER_NAME;
                //        expMestBuThuocLe.EXP_MEST_TYPE_ID = bu != null ? bu.ID : 0;
                //        expMestBuThuocLe.EXP_MEST_STT_ID = this._CurrentExpMest.EXP_MEST_STT_ID;
                //        expMestBuThuocLe.EXP_MEST_STT_CODE = this._CurrentExpMest.EXP_MEST_STT_CODE;
                //        expMestBuThuocLe.EXP_MEST_STT_NAME = this._CurrentExpMest.EXP_MEST_STT_NAME;
                //        var checkBuExist = expMests.FirstOrDefault(o => o.ID == expMestBuThuocLe.ID);
                //        if (checkBuExist == null)
                //        {
                //            expMests.Add(expMestBuThuocLe);
                //        }
                //    }
                //    expMests = expMests.OrderBy(o => o.EXP_MEST_TYPE_ID).ToList();

                //}

                // setHightLight
                List<ExpMestSDO> expMestSDOs = new List<ExpMestSDO>();

                var focus = (ExpMestMatyMetyReqSDODetail)gridViewMedicineMaterialDetail.GetFocusedRow();

                foreach (var item in expMests)
                {
                    ExpMestSDO expMest = new ExpMestSDO(item);
                    if (IsHightLight && focus != null && focus.EXP_MEST_ID == item.ID)
                    {
                        expMest.IsHighLight = true;
                    }
                    else
                    {
                        expMest.IsHighLight = false;
                    }
                    expMestSDOs.Add(expMest);
                }

                gridControlExpMestChild.BeginUpdate();
                gridControlExpMestChild.DataSource = expMestSDOs;
                gridControlExpMestChild.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void EnableControl(long expMestSttId)
        {
            try
            {
                Boolean btnApproveStt = true;
                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)

                    btnApproveStt = true;
                else
                    btnApproveStt = false;
                if (moduleData == null)
                {
                    return;
                }
                var currentMedistock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == moduleData.RoomId).FirstOrDefault();
                if (expMestSttId <= 0)
                {
                    return;
                }
                if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (currentMedistock != null && currentMedistock.ID == this._CurrentExpMest.MEDI_STOCK_ID))
                {
                    btnApproval.Enabled = btnApproveStt;
                    btnExport.Enabled = false;
                }
                else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && (currentMedistock != null && currentMedistock.ID == this._CurrentExpMest.MEDI_STOCK_ID))
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = true;
                }
                else
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.ExpMestChildFromAggs = FilterWithSearch(this.ExpMestAll);
                GetdExpMestMedicineMaterial();
                LoadDataToGridMediMate(this.ExpMestChildFromAggs, true);
                bool isSearch = false;
                if (dtInstructionDateFrom.EditValue == null && dtInstructionDateTo.EditValue == null && String.IsNullOrWhiteSpace(txtPatientName.Text))
                {
                    isSearch = false;
                }
                else
                {
                    isSearch = true;
                }
                SetDataToExpMestChildGrid(ExpMestChildFromAggs, isSearch, false);
                gridViewExpMestChild.SelectAll();
                //GetControlAcs();
                EnableControl(this._CurrentExpMest.EXP_MEST_STT_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestChild_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELETE_ITEM")
                    {
                        if (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.RepositoryItem = ButtonEditRemoveEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditRemoveDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChild_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST pData = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        try
                        {
                            if (pData.TDL_PATIENT_DOB != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB ?? 0);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "INTRUCTION_DATE_STR" && pData.TDL_INTRUCTION_DATE != null)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_INTRUCTION_DATE ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineMaterialDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    ExpMestMatyMetyReqSDODetail pData = (ExpMestMatyMetyReqSDODetail)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        try
                        {
                            if (pData.EXPIRED_DATE != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.EXPIRED_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXP_DATE", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(pData.PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE")
                    {
                        decimal totalPrice = (pData.PRICE ?? 0) * pData.AMOUNT * ((pData.VAT_RATIO ?? 0) + 1);
                        e.Value = Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                    {
                        if (pData.IS_EXPEND == 1)
                        {
                            e.Value = imageCollection1.Images[0];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "CONVERT_AMOUNT"
                        && pData.CONVERT_RATIO != null
                        && pData.CONVERT_RATIO > 0)
                    {
                        e.Value = pData.AMOUNT * pData.CONVERT_RATIO;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineMaterialDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ExpMestMatyMetyReqSDODetail)gridViewMedicineMaterialDetail.GetRow(e.RowHandle);
                if (data != null)
                {
                    // nếu thuốc là gây nghiện hướng thần
                    if (data.IS_MEDICINE && (data.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT || data.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN))
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if (data.IS_MEDICINE)// thuốc thường
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChild_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var expMestFocus = (ExpMestSDO)gridViewExpMestChild.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "DELETE_ITEM")
                        {
                            if (this._CurrentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                ButtonEditRemoveEnable_ButtonClick(null, null);
                            }
                        }
                        if (hi.Column.FieldName == "Y_LENH_BN_DETAIL")
                        {
                            YLenhThuocBN(expMestFocus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void YLenhThuocBN(V_HIS_EXP_MEST expMestFocus)
        {
            try
            {
                if (expMestFocus == null || expMestFocus.TDL_PATIENT_ID == null)
                {
                    return;
                }
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqPatient").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqPatient'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqPatient' is not plugins");
                List<object> listArgs = new List<object>();
                listArgs.Add(expMestFocus.TDL_TREATMENT_ID);
                listArgs.Add(expMestFocus.REQ_DEPARTMENT_NAME);
                //listArgs.Add((DelegateSelectData)CloseServiceReqPatientForm);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditRemoveEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                var expMestFocus = (ExpMestSDO)gridViewExpMestChild.GetFocusedRow();
                if (expMestFocus != null)
                {
                    WaitingManager.Show();
                    HIS_EXP_MEST result = null;
                    HisExpMestSDO hisExpMestSDO = new HisExpMestSDO();
                    hisExpMestSDO.ExpMestId = expMestFocus.ID;
                    hisExpMestSDO.ReqRoomId = this.moduleData.RoomId;
                    if (this._CurrentExpMest != null && this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamRemove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                    }
                    else
                    {
                        result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrRemove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                    }

                    if (result != null)
                    {
                        success = true;
                        if (gridControlMedicineMaterialDetail.DataSource == null)
                        {
                            return;
                        }
                        List<ExpMestMatyMetyReqSDODetail> ExpMestMatyMetyReqSDODetailsResult = (List<ExpMestMatyMetyReqSDODetail>)gridControlMedicineMaterialDetail.DataSource;
                        if (ExpMestMatyMetyReqSDODetailsResult != null && ExpMestMatyMetyReqSDODetailsResult.Count > 0)
                        {
                            ExpMestMatyMetyReqSDODetailsResult.RemoveAll(o => o.EXP_MEST_ID == expMestFocus.ID);
                            SetDataToGridControlMedicineMaterial(ExpMestMatyMetyReqSDODetailsResult);
                        }
                        if (ExpMestChildFromAggs != null && ExpMestChildFromAggs.Count > 0)
                        {
                            ExpMestChildFromAggs.RemoveAll(o => o.ID == expMestFocus.ID);
                            SetDataToExpMestChildGrid(ExpMestChildFromAggs, false, false);
                        }

                        // check all gridViewExpMestChild
                        gridViewExpMestChild.SelectAll();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestChild_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ExpMestSDO)gridViewExpMestChild.GetRow(e.RowHandle);
                if (data != null && data.IsHighLight)
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<ExpMestSDO> expMestCheckeds = new List<ExpMestSDO>();
                int[] selectRows = gridViewExpMestChild.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        expMestCheckeds.Add((ExpMestSDO)gridViewExpMestChild.GetRow(selectRows[i]));
                    }
                }
                else
                {
                    var dataSource = (List<ExpMestSDO>)gridControlExpMestChild.DataSource;
                    foreach (var item in dataSource)
                    {
                        item.IsHighLight = false;
                    }
                    gridViewExpMestChild.BeginDataUpdate();
                    gridControlExpMestChild.DataSource = dataSource;
                    gridViewExpMestChild.EndDataUpdate();
                }

                LoadDataToGridMediMate(expMestCheckeds, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitchHaoPhi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestChild_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitchTheoLo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestChild_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void ButtonEdit_HightLightPatient_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (ExpMestMatyMetyReqSDODetail)gridViewMedicineMaterialDetail.GetFocusedRow();
                if (focus != null)
                {
                    var dataSource = (List<ExpMestSDO>)gridControlExpMestChild.DataSource;
                    var select = gridViewExpMestChild.GetSelectedRows();

                    if (toggleSwitchTheoLo.IsOn)// theo loại thuốc
                    {
                        if (focus.IS_MEDICINE)
                        {
                            List<V_HIS_EXP_MEST_MEDICINE> medicineCheck = (this.expMestMedicines != null && this.expMestMedicines.Count() > 0) ? this.expMestMedicines.Where(o => o.MEDICINE_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;
                            foreach (var item in dataSource)
                            {
                                if (medicineCheck != null && medicineCheck.Count() > 0 && medicineCheck.Select(o => o.EXP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                        else
                        {
                            List<V_HIS_EXP_MEST_MATERIAL> materialCheck = (this.expMestMaterials != null && this.expMestMaterials.Count() > 0) ? this.expMestMaterials.Where(o => o.MATERIAL_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;
                            foreach (var item in dataSource)
                            {
                                if (materialCheck != null && materialCheck.Count() > 0 && materialCheck.Select(o => o.EXP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }

                    }
                    else// theo lô
                    {
                        if (focus.IS_MEDICINE)
                        {
                            List<V_HIS_EXP_MEST_MEDICINE> medicineCheck = this.expMestMedicines != null && this.expMestMedicines.Count() > 0 ? this.expMestMedicines.Where(o => o.MEDICINE_ID == focus.MEDI_MATE_ID && o.MEDICINE_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;

                            foreach (var item in dataSource)
                            {
                                if (medicineCheck != null && medicineCheck.Count() > 0 && medicineCheck.Select(o => o.EXP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                        else
                        {
                            List<V_HIS_EXP_MEST_MATERIAL> materialCheck = this.expMestMaterials != null && this.expMestMaterials.Count() > 0 ? this.expMestMaterials.Where(o => o.MATERIAL_ID == focus.MEDI_MATE_ID && o.MATERIAL_TYPE_ID == focus.MEDI_MATE_TYPE_ID).ToList() : null;

                            foreach (var item in dataSource)
                            {
                                if (materialCheck != null && materialCheck.Count() > 0 && materialCheck.Select(o => o.EXP_MEST_ID).Contains(item.ID))
                                {
                                    item.IsHighLight = true;
                                }
                                else
                                {
                                    item.IsHighLight = false;
                                }
                            }
                        }
                    }


                    gridControlExpMestChild.BeginUpdate();
                    gridControlExpMestChild.DataSource = dataSource;
                    foreach (var item in select)
                    {
                        gridViewExpMestChild.SelectRow(item);
                    }
                    gridControlExpMestChild.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit__YLenhThuocTheoBN_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var expMestFocus = (ExpMestSDO)gridViewExpMestChild.GetFocusedRow();
                if (expMestFocus == null || expMestFocus.TDL_PATIENT_ID == null)
                {
                    return;
                }
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqPatient").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqPatient'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqPatient' is not plugins");
                List<object> listArgs = new List<object>();
                listArgs.Add(expMestFocus.TDL_TREATMENT_ID);
                listArgs.Add(expMestFocus.REQ_DEPARTMENT_NAME);
                //listArgs.Add((DelegateSelectData)CloseServiceReqPatientForm);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprovalMaterial_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
                string replaceFor = (vw.GetRowCellValue(e.RowHandle, "REPLACE_FOR_NAME") ?? "").ToString();// thay the thuoc
                if (!String.IsNullOrWhiteSpace(replaceFor))
                    e.Appearance.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
