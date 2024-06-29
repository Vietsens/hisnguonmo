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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AggrExpMestDetail.ADO;
using HIS.Desktop.Plugins.AggrExpMestDetail.Resources;
using HIS.Desktop.Plugins.ExpMestViewDetail;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail
{
    public partial class frmAggrExpMestDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST AggExpMest;// phiếu lĩnh truyền sang
        List<ExpMestSDO> ExpMestChildFromAggs; // các phiếu con của phiếu lĩnh hiện tại
        List<ExpMestSDO> ExpMestAll;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        string moduleLink = "HIS.Desktop.Plugins.AggrExpMestDetail";
        private bool isNotLoadWhileChangeControlStateInFirst;
        DelegateSelectData delegateSelectData = null;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        bool InitData;

        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        List<HIS_EXP_MEST_REASON> reason;
        bool IsReasonRequired { get; set; }
        long RoomIdFromMediStock { get; set; }
        #endregion

        #region Construct
        public frmAggrExpMestDetail(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _expMest, DelegateSelectData _delegateSelectData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                delegateSelectData = _delegateSelectData;
                this.moduleData = moduleData;
                this.AggExpMest = _expMest;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmAggrExpMestDetail_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                InitData = true;
                IsReasonRequired = HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__IS_REASON_REQUIRED) == "1";
                isNotLoadWhileChangeControlStateInFirst = true;
                LoadDataToComboReasonRequired();
                GetControlAcs();
                SetDataToCommonInfo(this.AggExpMest);
                CheckEnableIconSave(this.AggExpMest);
                GetChildExpMestFromAggExpMest(this.AggExpMest.ID);
                GetdExpMestMedicineMaterial();
                LoadDataToGridMediMate(this.ExpMestChildFromAggs, true);
                SetDataToExpMestChildGrid(ExpMestChildFromAggs, false, false);
                gridViewExpMestChild.SelectAll();
                EnableControl(this.AggExpMest.EXP_MEST_STT_ID);
                SetCaptionByLanguageKey();
                SetIcon();
                InitControlState();
                chkHaoPhi.Checked = true;
                isNotLoadWhileChangeControlStateInFirst = false;
                InitData = false;

                gridControlExpMestChild.ToolTipController = toolTipControllerGrid;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private function
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
                DataToCombo(repReasonRequired, reason);

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
        private void DataToCombo(Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit cbo, List<HIS_EXP_MEST_REASON> data)
        {
            try
            {
                cbo.DataSource = data;
                cbo.DisplayMember = "EXP_MEST_REASON_NAME";
                cbo.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                //cbo.View.ForceInitialize();
                cbo.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(250, 200);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.View.Columns.AddField("EXP_MEST_REASON_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.View.Columns.AddField("EXP_MEST_REASON_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 250;

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

        private void LoadExpMestMedicines()
        {
            try
            {
                if (this.ExpMestChildFromAggs == null || this.ExpMestChildFromAggs.Count == 0)
                    return;
                List<long> expMestIds = this.ExpMestChildFromAggs.Select(o => o.ID).Distinct().ToList();
                // thêm xuất bù lĩnh
                expMestIds.Add(this.AggExpMest.ID);
                CommonParam param = new CommonParam();
                int skip = 0;
                this.expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                while (expMestIds.Count - skip > 0)
                {
                    var ids = expMestIds.Skip(skip).Take(100).ToList();
                    MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = ids;
                    var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                    skip += 100;
                    if (expMestMedicines != null && expMestMedicines.Count > 0)
                    {
                        this.expMestMedicines.AddRange(expMestMedicines);
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
                expMestIds.Add(this.AggExpMest.ID);
                int skip = 0;
                this.expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                while (expMestIds.Count - skip > 0)
                {
                    var ids = expMestIds.Skip(skip).Take(100).ToList();
                    MOS.Filter.HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                    hisExpMestMaterialViewFilter.EXP_MEST_IDs = ids;
                    var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);
                    skip += 100;
                    if (expMestMaterials != null && expMestMaterials.Count > 0)
                    {
                        this.expMestMaterials.AddRange(expMestMaterials);
                    }
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
                if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (currentMedistock != null && currentMedistock.ID == this.AggExpMest.MEDI_STOCK_ID))
                {
                    btnApproval.Enabled = btnApproveStt;
                    btnExport.Enabled = false;
                }
                else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && (currentMedistock != null && currentMedistock.ID == this.AggExpMest.MEDI_STOCK_ID))
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

        void SetDataToGridControlMedicineMaterial(List<ExpMestMatyMetyReqSDODetail> expMestMedicineMaterials)
        {
            try
            {
                if (expMestMedicineMaterials != null && expMestMedicineMaterials.Count > 0)
                {
                    expMestMedicineMaterials = expMestMedicineMaterials.OrderByDescending(o => o.IS_MEDICINE).ThenBy(o => o.MEDICINE_TYPE_NUM_ORDER ?? 99999).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }

                gridControlMedicineMaterialDetail.BeginUpdate();
                gridControlMedicineMaterialDetail.DataSource = expMestMedicineMaterials;
                gridControlMedicineMaterialDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // gán dữ liệu vào thông tin chung
        void SetDataToCommonInfo(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST AggExpMest)
        {
            try
            {
                if (AggExpMest != null)
                {
                    CommonParam param = new CommonParam();
                    //if (!this.InitData)
                    {
                        HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                        expMestFilter.ID = AggExpMest.ID;
                        var aggrExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, param);
                        if (aggrExpMest != null && aggrExpMest.Count > 0)
                        {
                            this.AggExpMest = aggrExpMest.FirstOrDefault();
                        }
                    }

                    lblCreator.Text = AggExpMest.CREATOR;
                    lblCreateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(AggExpMest.CREATE_TIME ?? 0);
                    lblExpMestCode.Text = AggExpMest.EXP_MEST_CODE;
                    lblMedistock.Text = AggExpMest.MEDI_STOCK_CODE + " - " + AggExpMest.MEDI_STOCK_NAME;
                    lblReqDepartment.Text = AggExpMest.REQ_DEPARTMENT_CODE + " - " + AggExpMest.REQ_DEPARTMENT_NAME;
                    lblReqName.Text = AggExpMest.REQ_LOGINNAME + " - " + AggExpMest.REQ_USERNAME;
                    cboExpMestReason.EditValue = this.AggExpMest.EXP_MEST_REASON_ID;
                    HisMediStockFilter f = new HisMediStockFilter();
                    f.ID = AggExpMest.MEDI_STOCK_ID;
                    var mediStock = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumer.ApiConsumers.MosConsumer, f, param);
                    if (mediStock != null && mediStock.Count > 0)
                    {
                        this.RoomIdFromMediStock = mediStock.FirstOrDefault().ROOM_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // lấy các phiếu con từ phiếu lĩnh được chọn
        void GetChildExpMestFromAggExpMest(long AggExpMestId)
        {
            try
            {
                if (AggExpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                    expMestViewFilter.AGGR_EXP_MEST_ID = AggExpMestId;

                    var ExpMestChildFromAggApis = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("___ExpMestChildFromAggApis", ExpMestChildFromAggApis));
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
                    expMestIds.Add(this.AggExpMest.ID);
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

                gridColumn16.VisibleIndex = -1;
                gridColumn17.VisibleIndex = -1;
                // group theo lô
                if (!chkTheoLo.Checked && chkHaoPhi.Checked)
                {
                    //List<IGrouping<GroupListADO, ExpMestMatyMetyReqSDODetail>> dataGroup = new List<IGrouping<GroupListADO, ExpMestMatyMetyReqSDODetail>>();
                    if (chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        gridColumn17.VisibleIndex = 8;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.PATIENT_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
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
                    else if (!chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn17.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
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
                    else if (chkPatientType.Checked && !chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.PATIENT_TYPE_ID
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
                    else
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


                }
                else if (!chkTheoLo.Checked && !chkHaoPhi.Checked)
                {
                    ExpMestMatyMetyReqSDODetailsDb = ExpMestMatyMetyReqSDODetailsDb.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList();
                    if (chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        gridColumn17.VisibleIndex = 8;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.PATIENT_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
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
                    else if (!chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn17.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
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
                    else if (chkPatientType.Checked && !chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_TYPE_ID,
                            p.PATIENT_TYPE_ID
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
                    else
                    {
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

                }
                else if (chkTheoLo.Checked && chkHaoPhi.Checked)
                {
                    if (chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        gridColumn17.VisibleIndex = 8;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.PATIENT_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else if (!chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn17.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.OTHER_PAY_SOURCE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else if (chkPatientType.Checked && !chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.PATIENT_TYPE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else
                    {
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                }
                else
                {
                    //MOS.EFMODEL.DataModels.V_HIS_MEDICINE
                    ExpMestMatyMetyReqSDODetailsDb = ExpMestMatyMetyReqSDODetailsDb.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList();
                    if (chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        gridColumn17.VisibleIndex = 8;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.PATIENT_TYPE_ID,
                            p.OTHER_PAY_SOURCE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.OrderBy(o => o.IS_EXPEND).First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else if (!chkPatientType.Checked && chkOtherPaySource.Checked)
                    {
                        gridColumn17.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.OTHER_PAY_SOURCE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.OrderBy(o => o.IS_EXPEND).First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else if (chkPatientType.Checked && !chkOtherPaySource.Checked)
                    {
                        gridColumn16.VisibleIndex = 7;
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID,
                            p.PATIENT_TYPE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.OrderBy(o => o.IS_EXPEND).First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
                    }
                    else
                    {
                        var dataGroup = ExpMestMatyMetyReqSDODetailsDb.GroupBy(p => new
                        {
                            p.IS_MEDICINE,
                            p.MEDI_MATE_ID
                        }).ToList();
                        foreach (var impMestMediMateADOGroup in dataGroup)
                        {
                            ExpMestMatyMetyReqSDODetail impMestMediMateADO = impMestMediMateADOGroup.OrderBy(o => o.IS_EXPEND).First();
                            impMestMediMateADO.AMOUNT = impMestMediMateADOGroup.Sum(o => o.AMOUNT);
                            ImpMestMediMateADOTemps.Add(impMestMediMateADO);
                        }
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

        void SetDataToExpMestChildGrid(List<ExpMestSDO> expMests, bool isSearch, bool IsHightLight)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewExpMestChild);
                if (expMests != null && expMests.Count > 0 && !isSearch)
                {
                    // check phiếu xuất bù lĩnh
                    var checkExistExpMestMedicine = (this.expMestMedicines != null && this.expMestMedicines.Count > 0) ? this.expMestMedicines.FirstOrDefault(o => o.EXP_MEST_ID == this.AggExpMest.ID) : null;
                    var checkExistExpMestMaterial = (this.expMestMaterials != null && this.expMestMaterials.Count > 0) ? this.expMestMaterials.FirstOrDefault(o => o.EXP_MEST_ID == this.AggExpMest.ID) : null;

                    if (checkExistExpMestMedicine != null || checkExistExpMestMaterial != null)
                    {
                        ExpMestSDO expMestBuThuocLe = new ExpMestSDO();
                        var bu = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL);
                        expMestBuThuocLe.EXP_MEST_TYPE_CODE = bu.EXP_MEST_TYPE_CODE;
                        expMestBuThuocLe.EXP_MEST_TYPE_NAME = bu.EXP_MEST_TYPE_NAME;
                        expMestBuThuocLe.EXP_MEST_CODE = this.AggExpMest.EXP_MEST_CODE;
                        expMestBuThuocLe.AGGR_EXP_MEST_ID = this.AggExpMest.AGGR_EXP_MEST_ID;
                        expMestBuThuocLe.ID = this.AggExpMest.ID;
                        expMestBuThuocLe.TDL_PATIENT_NAME = this.AggExpMest.TDL_PATIENT_NAME;
                        expMestBuThuocLe.TDL_TREATMENT_CODE = this.AggExpMest.TDL_TREATMENT_CODE;
                        expMestBuThuocLe.TDL_PATIENT_DOB = this.AggExpMest.TDL_PATIENT_DOB;
                        expMestBuThuocLe.TDL_PATIENT_GENDER_NAME = this.AggExpMest.TDL_PATIENT_GENDER_NAME;
                        expMestBuThuocLe.EXP_MEST_TYPE_ID = bu.ID;
                        expMestBuThuocLe.EXP_MEST_STT_ID = this.AggExpMest.EXP_MEST_STT_ID;
                        expMestBuThuocLe.EXP_MEST_STT_CODE = this.AggExpMest.EXP_MEST_STT_CODE;
                        expMestBuThuocLe.EXP_MEST_STT_NAME = this.AggExpMest.EXP_MEST_STT_NAME;
                        expMestBuThuocLe.REASION_ID = this.AggExpMest.EXP_MEST_REASON_ID != null ? this.AggExpMest.EXP_MEST_REASON_ID.ToString() : "";
                        expMestBuThuocLe.EXP_MEST_REASON_ID = this.AggExpMest.EXP_MEST_REASON_ID;
                        expMestBuThuocLe.EXP_MEST_REASON_CODE = this.AggExpMest.EXP_MEST_REASON_CODE;
                        expMestBuThuocLe.EXP_MEST_REASON_NAME = this.AggExpMest.EXP_MEST_REASON_NAME;
                        var checkBuExist = expMests.FirstOrDefault(o => o.ID == expMestBuThuocLe.ID);
                        if (checkBuExist == null)
                        {
                            expMests.Add(expMestBuThuocLe);
                        }
                    }
                    expMests = expMests.OrderBy(o => o.EXP_MEST_TYPE_ID).ToList();

                }

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
                    expMest.REASION_ID = item.EXP_MEST_REASON_ID != null ? item.EXP_MEST_REASON_ID.ToString() : "";
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

        #endregion

        #region public function
        #endregion

        #region Event handler

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.AggExpMest != null && this.AggExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    clickItemInGopDonThuoc(this.AggExpMest);
                }
                else
                {
                    PrintAggregateExpMest(this.AggExpMest);
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
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.moduleData == null)
                {
                    return;
                }

                //            List<ExpMestSDO> data1 = (List<ExpMestSDO>)gridControlExpMestChild.DataSource;
                //            string mess = "Bắt buộc nhập lý do xuất.";
                //            mess += "\r\nCác mã phiếu chưa nhập: ";
                //            List<string> lstStr = data1.Where(o => o.EXP_MEST_REASON_ID == null).Select(o => o.EXP_MEST_CODE).ToList();
                //            if(lstStr!=null && lstStr.Count>0 && IsReasonRequired)
                //{
                //                MessageBox.Show(mess + String.Join(", ", lstStr), "Thông báo", MessageBoxButtons.OK);
                //                return;

                //            }                    
                List<ExpMestMatyMetyReqSDODetail> dataSource = (List<ExpMestMatyMetyReqSDODetail>)gridControlMedicineMaterialDetail.DataSource;
                // get expMestMedicine
                List<ExpMestMatyMetyReqSDODetail> expMestMedicines = dataSource != null && dataSource.Count() > 0 ? dataSource.Where(o => o.IS_MEDICINE == true).ToList() : dataSource;
                if (expMestMedicines != null && expMestMedicines.Count() > 0)
                {
                    string message = "";
                    foreach (var item in expMestMedicines)
                    {
                        var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_MEDICINE_TYPE_ID && o.IS_STAR_MARK == 1);
                        if (medicineType != null)
                        {
                            message += medicineType.MEDICINE_TYPE_NAME + " số lượng: " + item.AMOUNT + "; ";
                        }
                    }

                    if (!String.IsNullOrEmpty(message))
                    {
                        message = String.Format("Phiếu lĩnh có thuốc * gồm: {0} \nBạn có đồng ý duyệt?", message);
                        if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }
                }

                WaitingManager.Show();

                List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> apiresult = null;
                HisExpMestSDO sdo = new HisExpMestSDO();
                sdo.ExpMestId = this.AggExpMest.ID;
                sdo.ReqRoomId = this.moduleData.RoomId;

                //sdo.IsFinish = true;
                if (this.AggExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    apiresult = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                    ("api/HisExpMest/AggrExamApprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                }
                else
                {
                    apiresult = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                    ("api/HisExpMest/AggrApprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                }

                if (apiresult != null)
                {
                    success = true;
                    this.AggExpMest.EXP_MEST_STT_ID = apiresult.FirstOrDefault().EXP_MEST_STT_ID;
                    SetDataToCommonInfo(this.AggExpMest);
                    GetChildExpMestFromAggExpMest(this.AggExpMest.ID);
                    GetdExpMestMedicineMaterial();
                    SetDataToExpMestChildGrid(ExpMestChildFromAggs, false, false);
                    gridViewExpMestChild.SelectAll();
                    LoadDataToGridMediMate(this.ExpMestChildFromAggs, true);
                    EnableControl(apiresult.FirstOrDefault().EXP_MEST_STT_ID);
                    delegateSelectData(apiresult);
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void refreshData(object data)
        {
            try
            {
                if (data is HisAggrExpMestSDO)
                {
                    var expMestApprove = (MOS.SDO.HisAggrExpMestSDO)data;
                    //EnableBottomButton(expMestApprove.ExpMestIds, expMestApprove.RequestRoomId);
                }
                delegateSelectData(new HIS_EXP_MEST());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.AggExpMest != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST apiresult;
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = this.AggExpMest.ID;
                    sdo.ReqRoomId = this.moduleData.RoomId;
                    if (this.AggExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    }
                    else
                    {
                        apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (RequestUriStore.HIS_EXP_MEST_AGGREXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    }

                    WaitingManager.Hide();
                    if (apiresult != null)
                    {
                        success = true;
                        this.AggExpMest.EXP_MEST_STT_ID = apiresult.EXP_MEST_STT_ID;
                        SetDataToCommonInfo(this.AggExpMest);
                        GetChildExpMestFromAggExpMest(this.AggExpMest.ID);
                        GetdExpMestMedicineMaterial();
                        SetDataToExpMestChildGrid(ExpMestChildFromAggs, false, false);
                        gridViewExpMestChild.SelectAll();
                        LoadDataToGridMediMate(this.ExpMestChildFromAggs, true);
                        EnableControl(apiresult.EXP_MEST_STT_ID);
                        delegateSelectData(apiresult);
                    }

                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
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

        private void ButtonEditRemoveEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var expMestFocus = (ExpMestSDO)gridViewExpMestChild.GetFocusedRow();
                if (expMestFocus != null)
                {
                    ProcessHisExpMestAggrRemove(expMestFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHisExpMestAggrRemove(ExpMestSDO expMest)
        {
            try
            {
                if (expMest != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    HIS_EXP_MEST result = null;
                    HisExpMestSDO hisExpMestSDO = new HisExpMestSDO();
                    hisExpMestSDO.ExpMestId = expMest.ID;
                    hisExpMestSDO.ReqRoomId = this.moduleData.RoomId;
                    if (this.AggExpMest != null && this.AggExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
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
                            ExpMestMatyMetyReqSDODetailsResult.RemoveAll(o => o.EXP_MEST_ID == expMest.ID);
                            SetDataToGridControlMedicineMaterial(ExpMestMatyMetyReqSDODetailsResult);
                        }
                        if (ExpMestChildFromAggs != null && ExpMestChildFromAggs.Count > 0)
                        {
                            ExpMestChildFromAggs.RemoveAll(o => o.ID == expMest.ID);
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChild_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELETE_ITEM")
                    {
                        if (this.AggExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.RepositoryItem = ButtonEditRemoveEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditRemoveDisable;
                    }
                    else if (e.Column.FieldName == "ANTIBIOTIC_REQUEST_STT_ICON")
                    {
                        if (data.IS_USING_APPROVAL_REQUIRED != 1)
                        {
                            e.RepositoryItem = null;
                        }
                    }
                    //               else if(e.Column.FieldName == "REASION_ID")
                    //{
                    //                   if(AggExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || RoomIdFromMediStock != moduleData.RoomId)
                    //	{
                    //                       e.RepositoryItem = repTextDisable;
                    //                       if ((gridViewExpMestChild.GetRowCellValue(e.RowHandle, "REASION_ID") ?? "") != null)
                    //                       {
                    //                           string value = (gridViewExpMestChild.GetRowCellValue(e.RowHandle, "REASION_ID") ?? "").ToString();
                    //                           if (!string.IsNullOrEmpty(value))
                    //                               gridViewExpMestChild.SetRowCellValue(e.RowHandle, gridColumn15, reason.FirstOrDefault(o=>o.ID == Int64.Parse(value)).EXP_MEST_REASON_NAME);
                    //                       }
                    //                   }
                    //                   else
                    //	{
                    //                       e.RepositoryItem = repReasonRequired;
                    //	}                            
                    //}                        
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
                    else if (e.Column.FieldName == "ANTIBIOTIC_REQUEST_STT_ICON")//trạng thái phiếu yêu cầu sử dụng kháng sinh
                    {
                        if (pData.IS_USING_APPROVAL_REQUIRED == 1)
                        {
                            if (pData.ANTIBIOTIC_REQUEST_CODE == null)
                            {
                                e.Value = imageListStatus.Images[7];//màu cam
                            }
                            else
                            {
                                if (pData.ANTIBIOTIC_REQUEST_STT == 2)
                                {
                                    e.Value = imageListStatus.Images[3];//màu xanh lá
                                }
                                else
                                {
                                    e.Value = imageListStatus.Images[6];//màu nâu
                                }
                            }
                        }
                        else
                        {
                            e.Value = imageListStatus.Images[8];//màu trắng (Empty)
                        }
                    }
                    else if (e.Column.FieldName == "REQ_NAME_DISPLAY")
                    {
                        e.Value = pData.REQ_LOGINNAME + "/" + pData.REQ_USERNAME;
                    }
                    else if (e.Column.FieldName == "REQ_ROOM_DISPLAY")
                    {
                        e.Value = pData.REQ_ROOM_CODE + "/" + pData.REQ_ROOM_NAME;
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

        private void gridViewMedicineMaterialDetail_RowCellStyle(object sender, RowCellStyleEventArgs e)
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
        V_HIS_EXP_MEST expMestFocus;
        private string loginName;

        private void gridViewExpMestChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                            if (this.AggExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                ProcessHisExpMestAggrRemove(expMestFocus);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

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

        private void bbtnYLenhThuocBN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //btnYLenhThuocBenhNhan_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        public void CloseServiceReqPatientForm(object obj)
        {
            try
            {
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
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
                GetControlAcs();
                EnableControl(this.AggExpMest.EXP_MEST_STT_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void gridViewMedicineMaterialDetail_Click(object sender, EventArgs e)
        {
            try
            {

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

                    if (chkTheoLo.Checked)// theo loại thuốc
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
        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkOtherPaySource.Name)
                        {
                            chkOtherPaySource.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPatientType.Name)
                        {
                            chkPatientType.Checked = item.VALUE == "1";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestChild)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlExpMestChild.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "ANTIBIOTIC_REQUEST_STT_ICON")
                            {
                                long isUsingApprovalRequired = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "IS_USING_APPROVAL_REQUIRED") ?? "").ToString());
                                string antibioticRequestCode = (view.GetRowCellValue(lastRowHandle, "ANTIBIOTIC_REQUEST_CODE") ?? "").ToString();
                                long antibioticRequestSTT = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "ANTIBIOTIC_REQUEST_STT") ?? "").ToString());

                                if (isUsingApprovalRequired == 1)
                                {
                                    if (String.IsNullOrEmpty(antibioticRequestCode))
                                    {
                                        text = Inventec.Common.Resource.Get.Value("frmAggrExpMestDetail.ToolTipControl.MauCam", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                    }
                                    else
                                    {
                                        if (antibioticRequestSTT == 2)
                                        {
                                            text = Inventec.Common.Resource.Get.Value("frmAggrExpMestDetail.ToolTipControl.MauXanhLa", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                        }
                                        else
                                        {
                                            text = Inventec.Common.Resource.Get.Value("frmAggrExpMestDetail.ToolTipControl.MauNau", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                        }
                                    }
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewExpMestChild_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

        }

        private void repReasonRequired_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                var expMestFocus = (ExpMestSDO)gridViewExpMestChild.GetFocusedRow();
                if (cbo.EditValue != null)
                {
                    CommonParam param = new CommonParam();
                    ExpMestUpdateReasonSDO sdo = new ExpMestUpdateReasonSDO();
                    sdo.ExpMestReasonId = Int64.Parse(cbo.EditValue.ToString());
                    sdo.ExpMestId = expMestFocus.ID;
                    sdo.WorkingRoomId = moduleData.RoomId;
                    var result = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/UpdateReason", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    MessageManager.Show(this.ParentForm, param, result != null);
                }
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
                if (cboExpMestReason.EditValue != null && !string.IsNullOrEmpty(cboExpMestReason.EditValue.ToString()))
                    sdo.ExpMestReasonId = Int64.Parse(cboExpMestReason.EditValue.ToString());
                sdo.ExpMestId = AggExpMest.ID;
                sdo.WorkingRoomId = moduleData.RoomId;
                var result = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/UpdateReason", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    var dataSource = (List<ExpMestSDO>)gridControlExpMestChild.DataSource;
                    foreach (var item in dataSource)
                    {
                        item.EXP_MEST_REASON_ID = result.EXP_MEST_REASON_ID;
                        if (result.EXP_MEST_REASON_ID != null)
                        {
                            item.EXP_MEST_REASON_CODE = reason.FirstOrDefault(o => o.ID == result.EXP_MEST_REASON_ID).EXP_MEST_REASON_CODE;
                            item.EXP_MEST_REASON_NAME = reason.FirstOrDefault(o => o.ID == result.EXP_MEST_REASON_ID).EXP_MEST_REASON_NAME;
                        }
                        else
                        {
                            item.EXP_MEST_REASON_CODE = null;
                            item.EXP_MEST_REASON_NAME = null;
                        }
                    }
                    gridViewExpMestChild.BeginDataUpdate();
                    gridControlExpMestChild.DataSource = dataSource;
                    gridViewExpMestChild.EndDataUpdate();
                }
                MessageManager.Show(this.ParentForm, param, result != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHaoPhi_CheckedChanged(object sender, EventArgs e)
        {
            gridViewExpMestChild_SelectionChanged(null, null);
        }

        private void chkTheoLo_CheckedChanged(object sender, EventArgs e)
        {
            gridViewExpMestChild_SelectionChanged(null, null);
        }

        private void chkPatientType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestChild_SelectionChanged(null, null);
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPatientType.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPatientType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPatientType.Name;
                    csAddOrUpdate.VALUE = (chkPatientType.Checked ? "1" : "");
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOtherPaySource_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestChild_SelectionChanged(null, null);
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkOtherPaySource.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkOtherPaySource.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkOtherPaySource.Name;
                    csAddOrUpdate.VALUE = (chkOtherPaySource.Checked ? "1" : "");
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_ButtonClick(object sender, ButtonPressedEventArgs e)
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
    }
}
