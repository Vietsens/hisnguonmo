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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExpMestViewDetail.ADO;
using HIS.Desktop.Plugins.ExpMestViewDetail.Config;
using HIS.Desktop.Plugins.ExpMestViewDetail.Resources;
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
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.ExpMestViewDetail.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        V_HIS_EXP_MEST _CurrentExpMest { get; set; }//Check lại truyền vào Review
        HIS_EXP_MEST prescriptionPrint;
        HIS_SERVICE_REQ _serviceReq;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Boolean? enableButton = null;
        long currentMedistockId = 0;
        DelegateSelectData delegateSelectData = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicinePrint165 = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialPrint165 = new List<V_HIS_EXP_MEST_MATERIAL>();
        bool isPrintXuatHoaChatXN = false;
        bool isPrintIndonthuoc = false;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.ExpMestViewDetail";
        List<V_EMR_DOCUMENT> emrDocuments { get; set; }
        public bool IsReasonRequired { get; private set; }
        public List<HIS_CONFIG> lstConfig;
        RefeshReference refreshData = null;
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
                HisConfigCFG.LoadConfig();
                //this.delegateRefreshData = _delegateRefreshData;
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

                this.gridControlRequestMedicine.ToolTipController = this.toolTipController1;
                this.gridControlRequestMaterial.ToolTipController = this.toolTipController1;
                this.gridControlApprovalMedicine.ToolTipController = this.toolTipController1;
                this.gridControlApprovalMaterial.ToolTipController = this.toolTipController1;
                IsReasonRequired = HisConfigs.Get<string>("MOS.EXP_MEST.IS_REASON_REQUIRED") == "1";
                ReloadExpMest();
                GetControlAcs();
                LoadDataToComboReasonRequired();
                CheckEnableIconSave(this._CurrentExpMest);
                CreateThread();
                LoadDataToControlCommon();
                SetDataToGridControl();
                ShowTab();
                SetCaptionByLanguageKey();
                InitMenuToButtonPrint();
                InitControlState();
                EnableControl();
                SetIcon();
                CalculateTotalPrice();
                VisibleColumnPreAmount();
                GetListKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetListKey()
        {
            try
            {
                  lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE")).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadExpMest()
        {
            _CurrentExpMest = UpdateExpmest(new HIS_EXP_MEST() { ID = _CurrentExpMest.ID }, _CurrentExpMest);
        }
        #endregion

        #region private function
        private void VisibleColumnPreAmount()
        {
            try
            {
                if (HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignPrescriptionPK.ShowPresAmount") != "1")
                {
                    gc_PreAmount_ApproMaterial.VisibleIndex = gc_PreAmount_ApproMedicine.VisibleIndex = gc_PreAmount_ReqMaterial.VisibleIndex = gc_PreAmount_ReqMedicine.VisibleIndex = gc_PreAmount_ReqMety.VisibleIndex = gc_PreAmount_ReqMaty.VisibleIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckEnableIconSave(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST AggExpMest)
        {
            try
            {
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if ((AggExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnIconSave && o.IS_ACTIVE == IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE) != null))
                    && (BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.ID == AggExpMest.MEDI_STOCK_ID).FirstOrDefault().ROOM_ID == moduleData.RoomId || loginName == AggExpMest.CREATOR))
                {
                    btnIconSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboReasonRequired()
        {
            try
            {
                reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                InitComboExpMestReason(reason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboExpMestReason(List<HIS_EXP_MEST_REASON> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExpMestReason, data.ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadEmrDocument()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentViewFilter emrFilter = new EmrDocumentViewFilter();
                emrFilter.IS_MEDICAL_PAYMENT_EVIDENCE = true;
                emrFilter.TREATMENT_CODE__EXACT = _CurrentExpMest.TDL_TREATMENT_CODE;
                emrFilter.IS_DELETE = false;
                emrDocuments = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, paramCommon);

                this.ucViewEmrDocument.ReloadDocument(emrDocuments);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("ACS control", controlAcs));
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
                lblSumPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);

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
                if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK && this._CurrentExpMest.CHMS_TYPE_ID.HasValue)
                {

                    if (this._CurrentExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || medistock.IS_CABINET == (short)1 || !btnApproveStt)
                    {
                        btnApproval.Enabled = false;
                    }
                    else
                    {
                        btnApproval.Enabled = true;
                    }
                    if (this._CurrentExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || medistock.IS_CABINET == (short)1 || !btnExportStt)
                    {
                        btnExport.Enabled = false;
                    }
                    else
                    {
                        btnExport.Enabled = true;
                    }

                }
                else
                {
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
                    if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && HisConfigCFG.MUST_CONFIRM_BEFORE_APPROVE == "1" && this._CurrentExpMest.IS_CONFIRM != 1)
                    {
                        this.btnApproval.Enabled = false;
                    }

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
                lblAdvise.Text = "";
                lblDob.Text = "";
                lblIcdCode.Text = "";
                lblIcdName.Text = "";
                lblIcdText.Text = "";
                lblInstructionTime.Text = "";
                lblPatientCode.Text = "";
                lblReqLoginName.Text = "";
                lblUserTimeFromTo.Text = "";
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
                    lblReqLoginName.Text = expMest.REQ_LOGINNAME + " - " + expMest.REQ_USERNAME;

                    //
                    lblRecipient.Text = expMest.RECIPIENT;
                    lblRecevingPlace.Text = expMest.RECEIVING_PLACE;
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
                    lblReqLoginName.Text = "";
                    lblRecevingPlace.Text = "";
                    lblRecipient.Text = "";
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
                else if (prescription != null && prescription is CabinetBaseResultSDO)
                {
                    this._CurrentExpMest = UpdateExpmest(((CabinetBaseResultSDO)prescription).ExpMest, this._CurrentExpMest);
                    this.EnableControl();
                    CreateThread();
                    SetDataToGridControl();
                    ShowTab();
                    delegateSelectData(prescription);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("prescription is null or not equal TYPE");
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
                        if (e.Column.FieldName == "MEDICINE_TYPE_NAME")
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                if (dataRow.EXPIRED_DATE != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.EXPIRED_DATE ?? 0);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                if (dataRow.EXPIRED_DATE != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.EXPIRED_DATE ?? 0);
                                }

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO xuất", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                bool success = false;
                if (this._CurrentExpMest != null)
                {
                    if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                            || this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
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
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        WaitingManager.Show();
                        //bool success = false;
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
                    else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                    {
                        DateTime? creatTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_CurrentExpMest.CREATE_TIME + 100 ?? 0);
                        DateTime? modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(_CurrentExpMest.MODIFY_TIME ?? 0);

                        if (modifyTime > creatTime)
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("KEY_WARNING_MODIFIED_PRESCRIPTION_OPTION" + HisConfigCFG.WARNING_MODIFIED_PRESCRIPTION_OPTION);

                            if (HisConfigCFG.WARNING_MODIFIED_PRESCRIPTION_OPTION == "1" && HisConfigCFG.WARNING_MODIFIED_PRESCRIPTION_OPTION != null)
                            {
                                #region
                                if (DevExpress.XtraEditors.XtraMessageBox.Show("Phiếu xuất " + _CurrentExpMest.EXP_MEST_CODE + " đã có sự chỉnh sửa. Bạn có chắc muốn duyệt không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    WaitingManager.Show();
                                    //bool success = false;
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
                                #endregion
                            }
                            else
                            {
                                #region
                                WaitingManager.Show();
                                //bool success = false;
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
                                #endregion
                            }
                        }
                        else
                        {
                            #region
                            WaitingManager.Show();
                            //bool success = false;
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
                            #endregion
                        }

                    }
                    else
                    {
                        WaitingManager.Show();
                        //bool success = false;
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

                CommonParam paramMediStock = new CommonParam();
                HisMediStockExtyFilter extyFilter = new HisMediStockExtyFilter();
                extyFilter.MEDI_STOCK_ID = this._CurrentExpMest.MEDI_STOCK_ID;
                var listMediStockExty = new BackendAdapter(paramMediStock).Get<List<HIS_MEDI_STOCK_EXTY>>("api/HisMediStockExty/Get", ApiConsumers.MosConsumer, extyFilter, paramMediStock).ToList();
                if (listMediStockExty != null && listMediStockExty.Count > 0)
                {
                    //Inventec.Common.Logging.LogSystem.Info("listMediStockExty: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMediStockExty), listMediStockExty));
                    if (listMediStockExty.FirstOrDefault().IS_AUTO_EXECUTE == 1 && success && chkInHDSD.Checked)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate("Mps000099", deletePrintTemplate);
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
                        object apiresult = null;
                        if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK && this._CurrentExpMest.CHMS_TYPE_ID.HasValue)
                        {
                            apiresult = new BackendAdapter(param).Post<CabinetBaseResultSDO>("api/HisExpMest/BaseExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        }
                        else
                        {
                            apiresult = new BackendAdapter(param).Post<HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        }
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

                    if (success && chkInHDSD.Checked)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Thực hiện gọi hàm in HDSD ");
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate("Mps000099", deletePrintTemplate);
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
                    V_HIS_EXP_MEST_MEDICINE_1 dataRow = (V_HIS_EXP_MEST_MEDICINE_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MEDICINE_TYPE_NAME")
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                        else if (e.Column.FieldName == "EXP_DATE_STR")
                        {
                            if (dataRow.EXPIRED_DATE.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXPIRED_DATE ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.IMP_VAT_RATIO) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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
                        else if (e.Column.FieldName == "EXP_DATE_STR")
                        {
                            if (dataRow.EXPIRED_DATE.HasValue)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXPIRED_DATE ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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

        private void gridViewRequestMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int medicineGroupId = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "MEDICINE_GROUP_ID") ?? "").ToString());// là thuốc gây nghiện
                if (medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    || medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;

                int _IS_NOT_PRES = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "IS_NOT_PRES") ?? "").ToString());
                if (_IS_NOT_PRES == 1)
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
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
                if (medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    || medicineGroupId == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;

                int _IS_NOT_PRES = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "IS_NOT_PRES") ?? "").ToString());
                if (_IS_NOT_PRES == 1)
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
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
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                        else if (e.Column.FieldName == "PRES_AMOUNT_DISPLAY")
                        {
                            try
                            {
                                e.Value = dataRow.PRES_AMOUNT != null ? dataRow.PRES_AMOUNT : dataRow.AMOUNT;

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
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
                Inventec.Common.Logging.LogSystem.Warn("------ Load cboPrint_click");
                if (isPrintXuatHoaChatXN)
                {
                    try
                    {
                        //if (!cboPrint.Enabled || !cboPrint.Visible)
                        //return;

                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate("MPS000165", deletePrintTemplate);
                        Inventec.Common.Logging.LogSystem.Warn("------ Load RunPrintTemplate");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                else
                    cboPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {


            bool result = false;
            try
            {
                //if (this.expMestTestResultSDO == null)
                //return result;
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case "MPS000165":
                            MPS000165(printTypeCode, fileName);
                            break;
                        case "Mps000099":
                            InHuongDanSuDungThuoc(printTypeCode, fileName);
                            break;
                        //case "Mps0000134":
                        //    InPhieuXuatTheoDieuKien(printTypeCode, fileName);
                        //    break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InHuongDanSuDungThuoc(string printTypeCode, string fileName)
        {
            try
            {
                bool result = false;

                CommonParam param = new CommonParam();

                //List<V_HIS_EXP_MEST> lstExpMestData = new List<V_HIS_EXP_MEST>();

                //lstExpMestData.Add(this._CurrentExpMest);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);


                //HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                //expMestMaterialFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                //List<V_HIS_EXP_MEST_MATERIAL> expMestMaterial = new BackendAdapter(param)
                //    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(this._CurrentExpMest, expMestMedicines);


                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MPS000165(string printTypeCode, string fileName)
        {
            try
            {
                bool result = false;

                MPS.Processor.Mps000165.PDO.Mps000165PDO rdo = new MPS.Processor.Mps000165.PDO.Mps000165PDO(
                        this._CurrentExpMest,
                        expMestMedicinePrint165,
                        //null,
                        expMestMaterialPrint165,
                        //this.expMestTestResultSDO.ExpMestMaterials,
                        BackendDataWorker.Get<HIS_MACHINE>());

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRequestMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int _IS_NOT_PRES = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "IS_NOT_PRES") ?? "").ToString());
                if (_IS_NOT_PRES == 1)
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprovalMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                int _IS_NOT_PRES = Inventec.Common.TypeConvert.Parse.ToInt16((vw.GetRowCellValue(e.RowHandle, "IS_NOT_PRES") ?? "").ToString());
                if (_IS_NOT_PRES == 1)
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        private List<HIS_EXP_MEST_REASON> reason;
        private string loginName;

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlRequestMedicine)
                {
                    ToolTipDetail(gridControlRequestMedicine, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlRequestMaterial)
                {
                    ToolTipDetail(gridControlRequestMaterial, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlApprovalMedicine)
                {
                    ToolTipDetail(gridControlApprovalMedicine, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlApprovalMaterial)
                {
                    ToolTipDetail(gridControlApprovalMaterial, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ToolTipDetail(DevExpress.XtraGrid.GridControl gridControl, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle)
                    {
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        var isNotPres = (short?)view.GetRowCellValue(lastRowHandle, "IS_NOT_PRES");
                        if (isNotPres == 1)
                        {
                            text = "Là phần lẻ bệnh nhân phải thanh toán thêm, không phải thuốc/vật tư do bác sỹ kê trong đơn thuốc";
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FormatGridColumn(DevExpress.XtraGrid.Columns.GridColumn grdColName, decimal number)
        {
            try
            {
                int munberSeperator = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator;
                int munbershowDecimal = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.ShowDecimalOption");
                string Format = "";
                if (munbershowDecimal == 0)
                {
                    if (munberSeperator == 0)
                    {
                        Format = "#,##0";
                    }
                    else
                    {
                        Format = "#,##0.";
                        for (int i = 0; i < munberSeperator; i++)
                        {
                            Format += "0";
                        }
                    }

                }
                else if (munbershowDecimal == 1)
                {
                    if (number != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(number.ToString() ?? "") % 1 > 0)
                        {
                            if (munberSeperator == 0)
                            {
                                Format = "#,##0";
                            }
                            else
                            {
                                Format = "#,##0.";
                                for (int i = 0; i < munberSeperator; i++)
                                {
                                    Format += "0";
                                }
                            }
                        }
                        else
                        {
                            Format = "#,##0";
                        }
                    }
                }
                grdColName.DisplayFormat.FormatString = Format;
                grdColName.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkInHDSD.Name)
                        {
                            chkInHDSD.Checked = item.VALUE == "1";
                        }
                    }
                    layoutControlGroup4.Expanded = currentControlStateRDO.Where(o => o.KEY == layoutControlGroup4.Name) != null && currentControlStateRDO.Where(o => o.KEY == layoutControlGroup4.Name).FirstOrDefault().VALUE == "1";
                }
                else
                {
                    layoutControlGroup4.Expanded = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;

        }

        private void chkInHDSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInHDSD.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInHDSD.Name;
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void layoutControl2_GroupExpandChanged(object sender, DevExpress.XtraLayout.Utils.LayoutGroupEventArgs e)
        {
            try
            {
                string name = e.Group.Name;
                string value = "";

                if (e.Group.Name == layoutControlGroup4.Name)
                {
                    value = layoutControlGroup4.Expanded ? "1" : null;
                    if (value == "1") LoadEmrDocument();
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                if (!string.IsNullOrEmpty(csAddOrUpdate.KEY) && !string.IsNullOrEmpty(csAddOrUpdate.MODULE_LINK))
                {
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMestReason_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboExpMestReason.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnIconSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsReasonRequired && cboExpMestReason.EditValue == null)
                {
                    XtraMessageBox.Show(ResourceLanguageManager.BatBuocNhapLyDoXuat, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    cboExpMestReason.Focus();
                    cboExpMestReason.ShowPopup();
                    return;
                }
                CommonParam param = new CommonParam();
                ExpMestUpdateReasonSDO sdo = new ExpMestUpdateReasonSDO();
                if (cboExpMestReason.EditValue != null)
                    sdo.ExpMestReasonId = Int64.Parse(cboExpMestReason.EditValue.ToString());
                sdo.ExpMestId = _CurrentExpMest.ID;
                sdo.WorkingRoomId = moduleData.RoomId;
                var result = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/UpdateReason", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                MessageManager.Show(this.ParentForm, param, result != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
