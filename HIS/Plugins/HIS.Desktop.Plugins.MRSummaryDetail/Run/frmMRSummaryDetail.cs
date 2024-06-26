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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MRSummaryDetail.ADO;
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.MRSummaryDetail.ADO;
using HIS.Desktop.Controls.Session;
using MOS.SDO;
using HIS.Desktop.Plugins.MRSummaryDetail.Validation;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Web.UI.WebControls;

namespace HIS.Desktop.Plugins.MRSummaryDetail.Run
{
    public partial class frmMRSummaryDetail : FormBase
    {
        private int positionHandleControl;

        private Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        private Desktop.ADO.MRSummaryDetailADO currentADO { get; set; }
        private V_HIS_TREATMENT currentTreatment { get; set; }
        private HIS_MR_CHECK_SUMMARY checkSummary { get; set; }
        private HIS_MR_CHECK_SUMMARY OldcheckSummary { get; set; }
        private List<HIS_MR_CHECK_SUMMARY> lstCheckSummary { get; set; }
        private HIS_MR_CHECKLIST curentCheckList { get; set; }
        private List<HIS_MR_CHECK_ITEM> lstMrCheckItem { get; set; }
        private List<HIS_MR_CHECK_ITEM_TYPE> lstMrCheckItemType { get; set; }
        private List<MOS.EFMODEL.DataModels.HIS_MR_CHECKLIST> lstMrCheckList { get; set; }
        private long treatmentId { get; set; }
        private List<SummaryDetail> lstDetailAll { get; set; }
        public List<AcsUserADO> lstReAcsUserADO { get; set; }
        public RefeshReference delegateRefresh { get; set; }

        private bool IsLoadDisable { get; set; }

        public frmMRSummaryDetail(Inventec.Desktop.Common.Modules.Module moduleData, Desktop.ADO.MRSummaryDetailADO ado, RefeshReference delegateRefresh) : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.currentADO = ado;
                this.delegateRefresh = delegateRefresh;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                if (ado != null)
                {
                    treatmentId = ado.TreatmentId;
                    checkSummary = ado.CheckSummary;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn7) ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMRSummaryDetail_Load(object sender, EventArgs e)
        {
            try
            {
                ValidationDateTime();
                ValidationSpin();
                SetNullSpin();
                LoadComboUser();
                LoadDataToForm();
                LoadGrid();
                InitComboMedical();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMedical()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMR_COVER_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EMR_COVER_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMR_COVER_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboMedical, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToForm()
        {
            try
            {
                LoadCurrentTreatment();
                CheckOpenFrom();
                if (currentADO != null && currentADO.CheckSummary != null)
                {
                    ShowDataKHTH(checkSummary);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetNullSpin()
        {
            try
            {
                spnLowDay.EditValue = null;
                spnNumError.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueSpin(HIS_MR_CHECK_SUMMARY data)
        {
            try
            {

                if (data.LATE_DATE_NUMBER != null)
                {
                    spnLowDay.Value = data.LATE_DATE_NUMBER ?? 0;
                    spnLowDay.Enabled = false;
                }
                if (data.MEDI_RECORD_SUBMIT_DATE != null)
                {
                    dteSubmitOne.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.MEDI_RECORD_SUBMIT_DATE ?? 0) ?? DateTime.Now;
                    dteSubmitOne.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadGrid()
        {
            try
            {
                lstDetailAll = new List<SummaryDetail>();
                CommonParam param = new CommonParam();
                lstMrCheckItemType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MR_CHECK_ITEM_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();


                if (lstMrCheckItemType != null && lstMrCheckItemType.Count > 0)
                {
                    HisMrCheckItemFilter Itemfilter = new HisMrCheckItemFilter();
                    Itemfilter.CHECK_ITEM_TYPE_IDs = lstMrCheckItemType.Select(o => o.ID).ToList();

                    Itemfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    lstMrCheckItem = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM>>("api/HisMrCheckItem/Get", ApiConsumer.ApiConsumers.MosConsumer, Itemfilter, param);
                    if (lstMrCheckItem != null && lstMrCheckItem.Count() > 0 && cboMedical.EditValue != null)
                    {
                        lstMrCheckItem = lstMrCheckItem.Where(o => string.IsNullOrEmpty(o.EMR_COVER_TYPE_IDS) || o.EMR_COVER_TYPE_IDS.Contains(cboMedical.EditValue.ToString())).ToList();
                    }
                    if (checkSummary == null)
                    {
                        HisMrCheckSummaryFilter sfil = new HisMrCheckSummaryFilter();
                        sfil.TREATMENT_ID = treatmentId;
                        sfil.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        lstCheckSummary = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_SUMMARY>>("api/HisMrCheckSummary/Get", ApiConsumer.ApiConsumers.MosConsumer, sfil, param);
                        if (lstCheckSummary != null && lstCheckSummary.Count > 0)
                        {
                            //if (currentADO != null)
                            //{
                            //	if (currentADO.processType == MRSummaryDetailADO.OpenFrom.MedicalStoreV2)
                            //		lstCheckSummary = lstCheckSummary.Where(o => o.CHECK_PLACE == 2).ToList();
                            //	if (currentADO.processType == MRSummaryDetailADO.OpenFrom.TreatmentLatchApproveStore)
                            //		lstCheckSummary = lstCheckSummary.Where(o => o.CHECK_PLACE == 1).ToList();
                            //}
                            if (lstMrCheckItem != null && lstMrCheckItem.Count > 0)
                            {
                                HisMrChecklistFilter filter = new HisMrChecklistFilter();
                                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                filter.MR_CHECK_ITEM_IDs = lstMrCheckItem.Select(o => o.ID).ToList();
                                filter.MR_CHECK_SUMMARY_IDs = lstCheckSummary.Select(o => o.ID).ToList();
                                lstMrCheckList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECKLIST>>("api/HisMrCheckList/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                if (lstMrCheckList != null && lstMrCheckList.Count > 0)
                                    lstMrCheckList = lstMrCheckList.OrderByDescending(o => o.MODIFY_TIME).ToList();
                            }
                            if (lstCheckSummary != null && lstCheckSummary.Count > 0)
                                SetValueSpin(lstCheckSummary.Where(o => o.MEDI_RECORD_SUBMIT_DATE != null).OrderByDescending(o => o.MEDI_RECORD_SUBMIT_DATE).First());
                        }

                    }
                    else
                    {

                        if (lstMrCheckItem != null && lstMrCheckItem.Count > 0)
                        {
                            HisMrChecklistFilter filter = new HisMrChecklistFilter();
                            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            filter.MR_CHECK_ITEM_IDs = lstMrCheckItem.Select(o => o.ID).ToList();
                            filter.MR_CHECK_SUMMARY_ID = checkSummary.ID;
                            lstMrCheckList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECKLIST>>("api/HisMrCheckList/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        }
                        SetValueSpin(checkSummary);
                    }


                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HIS_MR_CHECKLIST__:", lstMrCheckList));

                    foreach (var parent in lstMrCheckItemType)
                    {

                        var checkItem = lstMrCheckItem.Where(o => o.CHECK_ITEM_TYPE_ID == parent.ID);
                        foreach (var item in checkItem)
                        {
                            SummaryDetail detail = new SummaryDetail();
                            detail.NUM_ORDER_PARENT = parent.NUM_ORDER;
                            detail.NUM_ORDER_CHILD = item.NUM_ORDER;
                            if (parent.NUM_ORDER != null)
                                detail.PARENT = ToRoman(Int32.Parse(parent.NUM_ORDER.ToString())) + ". " + parent.CHECK_ITEM_TYPE_NAME;
                            else
                                detail.PARENT = parent.CHECK_ITEM_TYPE_NAME;
                            detail.CHECK_ITEM_TYPE_NAME = item.NUM_ORDER + ". " + item.CHECK_ITEM_NAME;
                            if (lstMrCheckList != null && lstMrCheckList.Count > 0 && checkSummary != null)
                            {
                                var checkList = lstMrCheckList.FirstOrDefault(o => o.MR_CHECK_ITEM_ID == item.ID);
                                if (checkList != null)
                                {
                                    detail.CHECK_LIST_ITEM_ID = checkList.ID;
                                    detail.SUMMARY_ID = checkList.MR_CHECK_SUMMARY_ID;
                                    detail.CHECK_ITEM_ID = item.ID;
                                    detail.IS_CHECKER_CHECK = checkList.IS_CHECKER_CHECK == (short?)1;
                                    detail.IS_SELF_CHECK = checkList.IS_SELF_CHECK == (short?)1;
                                    detail.IS_CHECKER_NOT_USED = checkList.IS_CHECKER_NOT_USED == 1;
                                    detail.NOTE = checkList.NOTE;
                                    lstDetailAll.Add(detail);
                                }
                                else
                                {
                                    detail.SUMMARY_ID = checkSummary.ID;
                                    detail.CHECK_ITEM_ID = item.ID;
                                    lstDetailAll.Add(detail);
                                }
                            }
                            else if (checkSummary != null)
                            {
                                detail.SUMMARY_ID = checkSummary.ID;
                                detail.CHECK_ITEM_ID = item.ID;
                                lstDetailAll.Add(detail);
                            }
                            else if (checkSummary == null)
                            {
                                if (lstMrCheckList != null && lstMrCheckList.Count > 0)
                                {
                                    var lstMrCheckListTemp = lstMrCheckList.FirstOrDefault(o => o.MR_CHECK_ITEM_ID == item.ID);
                                    if (gridColumn2.OptionsColumn.AllowEdit)
                                    {
                                        detail.IS_SELF_CHECK = false;
                                        detail.IS_CHECKER_CHECK = lstMrCheckListTemp.IS_CHECKER_CHECK == (short?)1;
                                        detail.IS_CHECKER_NOT_USED = lstMrCheckListTemp.IS_CHECKER_NOT_USED == (short?)1;
                                    }
                                    else
                                    {
                                        detail.IS_SELF_CHECK = lstMrCheckListTemp.IS_SELF_CHECK == (short?)1;
                                        detail.IS_CHECKER_CHECK = false;
                                        detail.IS_CHECKER_NOT_USED = false;
                                    }
                                }
                                detail.CHECK_ITEM_ID = item.ID;
                                lstDetailAll.Add(detail);
                            }

                        }

                    }

                }
                lstDetailAll = lstDetailAll.OrderBy(o => o.NUM_ORDER_PARENT).ThenBy(o => o.NUM_ORDER_CHILD).ToList();
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstDetailAll;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        private void EnableCheckGrid(bool IsEnable)
        {
            try
            {
                gridColumn2.OptionsColumn.AllowEdit = !IsEnable;
                gridColumn3.OptionsColumn.AllowEdit = IsEnable;
                gridColumn6.OptionsColumn.AllowEdit = IsEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckOpenFrom()
        {
            try
            {
                if (currentADO != null)
                {

                    if (currentADO.processType == Desktop.ADO.MRSummaryDetailADO.OpenFrom.TreatmentList)
                    {
                        EnableControlGrid(false);
                    }
                    else
                    {
                        EnableControlGrid(true);
                        if (checkSummary == null)
                            EnabelSaveAndPrint(false);

                    }
                    if (currentADO.processType == Desktop.ADO.MRSummaryDetailADO.OpenFrom.MedicalStoreV2)
                    {
                        EnableControlKHTH(true);
                        CheckEditChangeText(chkCheckCom.Checked);
                        EnableCheckGrid(true);
                    }
                    else
                    {
                        EnableControlKHTH(false);
                    }

                    if (currentADO.processType == Desktop.ADO.MRSummaryDetailADO.OpenFrom.TreatmentLatchApproveStore)
                        EnableCheckGrid(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowDataKHTH(HIS_MR_CHECK_SUMMARY data)
        {
            try
            {

                chkCheckCom.Checked = data.IS_APPROVED == (short?)1;
                if (data.DEPARTMENT_RETURN_DATE != null)
                    dteDepartmentBack.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DEPARTMENT_RETURN_DATE ?? 0) ?? DateTime.Now;
                if (data.KHTH_RETURN_DATE != null)
                    dteSave.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.KHTH_RETURN_DATE ?? 0) ?? DateTime.Now;
                cboUserReciptKHTH.EditValue = data.KHTH_LOGIN_NAME;
                txtUserReciptKHTH.EditValue = data.KHTH_LOGIN_NAME;
                cboUserRecipt.EditValue = data.DEPARTMENT_LOGIN_NAME;
                txtUserRecipt.EditValue = data.DEPARTMENT_LOGIN_NAME;
                spnNumError.EditValue = data.ERROR_NUMBER;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlGrid(bool IsEnable)
        {
            try
            {
                dteDepartmentBack.Enabled = IsEnable;
                btnSave.Enabled = IsEnable;
                dteSave.Enabled = IsEnable;
                txtUserRecipt.Enabled = IsEnable;
                txtUserReciptKHTH.Enabled = IsEnable;
                cboUserRecipt.Enabled = IsEnable;
                cboUserReciptKHTH.Enabled = IsEnable;
                gridView1.OptionsBehavior.Editable = IsEnable;
                spnLowDay.Enabled = IsEnable;
                spnNumError.Enabled = IsEnable;
                dteSubmitOne.Enabled = IsEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlKHTH(bool IsEnable)
        {
            try
            {
                chkCheckCom.Enabled = IsEnable;
                spnNumError.Enabled = IsEnable;
                dteDepartmentBack.Enabled = IsEnable;
                txtUserRecipt.Enabled = IsEnable;
                cboUserRecipt.Enabled = IsEnable;
                dteSave.Enabled = IsEnable;
                cboUserReciptKHTH.Enabled = IsEnable;
                txtUserReciptKHTH.Enabled = IsEnable;
                IsLoadDisable = IsEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckEditChangeText(bool IsChecked)
        {
            try
            {
                if (!IsChecked)
                    layoutControlItem9.Text = "Bệnh án chưa đạt yêu cầu: trả lại khoa   Số lỗi:";
                else
                    layoutControlItem9.Text = "Bệnh án đạt yêu cầu: chuyển lưu trữ      Số lỗi:";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EnableDateEditKHTH(bool IsKHTH)
        {
            try
            {

                txtUserReciptKHTH.Enabled = IsKHTH;
                cboUserReciptKHTH.Enabled = IsKHTH;
                dteSave.Enabled = IsKHTH;

                txtUserRecipt.Enabled = !IsKHTH;
                cboUserRecipt.Enabled = !IsKHTH;
                dteDepartmentBack.Enabled = !IsKHTH;

                if (IsKHTH)
                {
                    txtUserRecipt.Text = String.Empty;
                    cboUserRecipt.EditValue = null;
                    dteDepartmentBack.EditValue = null;
                }
                else
                {
                    txtUserReciptKHTH.Text = String.Empty;
                    cboUserReciptKHTH.EditValue = null;
                    dteSave.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadCurrentTreatment()
        {
            try
            {
                if (treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    lblPatientName.Text = currentTreatment.TDL_PATIENT_NAME;
                    lblPatientCode.Text = currentTreatment.TDL_PATIENT_CODE;
                    lblDateEdit.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.OUT_DATE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserRecipt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtUserRecipt.Text))
                    {
                        var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtUserRecipt.Text.Trim());
                        if (checkUser != null)
                        {
                            cboUserRecipt.EditValue = checkUser.LOGINNAME;
                        }
                        else
                        {
                            cboUserRecipt.ShowPopup();
                            cboUserRecipt.Focus();
                        }
                    }
                    else
                    {
                        cboUserRecipt.ShowPopup();
                        cboUserRecipt.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserReciptKHTH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtUserReciptKHTH.Text))
                    {
                        var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtUserReciptKHTH.Text.Trim());
                        if (checkUser != null)
                        {
                            cboUserReciptKHTH.EditValue = checkUser.LOGINNAME;
                        }
                        else
                        {
                            cboUserReciptKHTH.ShowPopup();
                            cboUserReciptKHTH.Focus();
                        }
                    }
                    else
                    {
                        cboUserReciptKHTH.ShowPopup();
                        cboUserReciptKHTH.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate("Mps000472", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate()) return;
                HIS_MR_CHECK_SUMMARY obj = new HIS_MR_CHECK_SUMMARY();
                List<SummaryDetail> lstCheck = gridView1.DataSource as List<SummaryDetail>;
                var data = lstCheck.Where(o => !o.IS_CHECKER_CHECK && !o.IS_CHECKER_NOT_USED).ToList();

                if (data != null && data.Count > 0 && MessageBox.Show(String.Format("Các nội dung ({0}) chưa được tích “KHTH kiểm tra đạt” hoặc “KHTH kiểm tra không dùng”, bạn có muốn tiếp tục không?",
                    String.Join("; ", data.Select(o => o.CHECK_ITEM_TYPE_NAME))), "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                else
                {
                    var data1 = lstCheck.Where(o => o.IS_CHECKER_CHECK == true).ToList();
                    var data2 = lstCheck.Where(o => o.IS_CHECKER_NOT_USED == true).ToList();
                    var data3 = lstCheck.Where(o => o.IS_CHECKER_NOT_USED == true || o.IS_CHECKER_CHECK == true).ToList();
                    if (((data1 != null && data1.Count() > 0 && data1.Count() == lstCheck.Count()) ||
                        (data3 != null && data3.Count() > 0 && data3.Count() == lstCheck.Count()) ||
                        (data2 != null && data2.Count() > 0 && data2.Count() == lstCheck.Count())) &&
                         (chkCheckCom.Visible == false))
                    {
                        obj.IS_APPROVED = 1;
                    }
                    else 
                    {
                        obj.IS_APPROVED = chkCheckCom.Checked ? (short?)1 : 0;
                    }
                }

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDetailAll), lstDetailAll));
                MrCheckSummarySDO sdo = new MrCheckSummarySDO();
                #region SUMMARY               
                obj.TREATMENT_ID = treatmentId;
                obj.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModule.RoomId).DepartmentId;
                if (cboUserRecipt.EditValue != null)
                {
                    obj.DEPARTMENT_LOGIN_NAME = cboUserRecipt.EditValue.ToString();
                    var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUserRecipt.EditValue.ToString());
                    if (checkUser != null)
                    {
                        obj.DEPARTMENT_USER_NAME = checkUser.USERNAME;
                    }
                }
                if (cboUserReciptKHTH.EditValue != null)
                {
                    obj.KHTH_LOGIN_NAME = cboUserReciptKHTH.EditValue.ToString();
                    var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUserReciptKHTH.EditValue.ToString());
                    if (checkUser != null)
                    {
                        obj.KHTH_USER_NAME = checkUser.USERNAME;
                    }
                }
                if (dteDepartmentBack.EditValue != null && dteDepartmentBack.DateTime != DateTime.MinValue)
                    obj.DEPARTMENT_RETURN_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteDepartmentBack.DateTime);
                else
                    obj.DEPARTMENT_RETURN_DATE = null;
                if (dteSave.EditValue != null && dteSave.DateTime != DateTime.MinValue)
                    obj.KHTH_RETURN_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteSave.DateTime);
                else
                    obj.KHTH_RETURN_DATE = null;
                if (dteSubmitOne.EditValue != null && dteSubmitOne.DateTime != DateTime.MinValue)
                    obj.MEDI_RECORD_SUBMIT_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteSubmitOne.DateTime);
                else
                    obj.MEDI_RECORD_SUBMIT_DATE = null;
                if (spnLowDay.EditValue != null)
                    obj.LATE_DATE_NUMBER = Int64.Parse(spnLowDay.Value.ToString());
                if (spnNumError.EditValue != null)
                    obj.ERROR_NUMBER = Int64.Parse(spnNumError.Value.ToString());
                if (checkSummary != null)
                    obj.ID = checkSummary.ID;
                #endregion
                List<HIS_MR_CHECKLIST> lstCheckList = new List<HIS_MR_CHECKLIST>();
                foreach (var item in lstDetailAll)
                {
                    HIS_MR_CHECKLIST mobj = new HIS_MR_CHECKLIST();
                    mobj.IS_CHECKER_CHECK = item.IS_CHECKER_CHECK ? (short?)1 : 0;
                    mobj.IS_SELF_CHECK = item.IS_SELF_CHECK ? (short?)1 : 0;
                    mobj.IS_CHECKER_NOT_USED = item.IS_CHECKER_NOT_USED ? (short?)1 : 0;
                    // mobj.NOTE = item.NOTE;
                    if (item.SUMMARY_ID > 0)
                        mobj.MR_CHECK_SUMMARY_ID = item.SUMMARY_ID;
                    if (item.CHECK_LIST_ITEM_ID > 0)
                        mobj.ID = item.CHECK_LIST_ITEM_ID;
                    mobj.MR_CHECK_ITEM_ID = item.CHECK_ITEM_ID;
                    lstCheckList.Add(mobj);
                }

                if (currentADO != null)
                {
                    if (currentADO.processType == MRSummaryDetailADO.OpenFrom.TreatmentLatchApproveStore)
                        obj.CHECK_PLACE = 1;
                    if (currentADO.processType == MRSummaryDetailADO.OpenFrom.MedicalStoreV2)
                        obj.CHECK_PLACE = 2;
                }
                sdo.HisMrChecklists = lstCheckList;
                sdo.HisMrCheckSummary = obj;
                bool success = false;               
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var resultData = new BackendAdapter(param).Post<MrCheckSummarySDO>("api/HisMrCheckSummary/CreateOrUpdate", ApiConsumers.MosConsumer, sdo, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                if (resultData != null)
                {
                    EnabelSaveAndPrint(true);
                    success = true;
                    if (delegateRefresh != null)
                        delegateRefresh();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void EnabelSaveAndPrint(bool IsEnable)
        {
            try
            {
                btnSave.Enabled = !IsEnable;
                btnPrint.Enabled = IsEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dteDepartmentBack_EditValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadDisable)
                return;
            if (dteDepartmentBack.EditValue != null && dteDepartmentBack.DateTime != DateTime.MinValue)
            {
                dteDepartmentBack.Properties.Buttons[1].Visible = true;
                EnableDateEditKHTH(false);
            }
            else
            {
                dteDepartmentBack.Properties.Buttons[1].Visible = false;
                txtUserReciptKHTH.Enabled = true;
                cboUserReciptKHTH.Enabled = true;
                dteSave.Enabled = true;

                txtUserRecipt.Enabled = true;
                cboUserRecipt.Enabled = true;
                dteDepartmentBack.Enabled = true;
            }
        }

        private void dteSave_EditValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadDisable)
                return;
            if (dteSave.EditValue != null && dteSave.DateTime != DateTime.MinValue)
            {
                dteSave.Properties.Buttons[1].Visible = true;
                EnableDateEditKHTH(true);
            }
            else
            {
                dteSave.Properties.Buttons[1].Visible = false;
                txtUserReciptKHTH.Enabled = true;
                cboUserReciptKHTH.Enabled = true;
                dteSave.Enabled = true;

                txtUserRecipt.Enabled = true;
                cboUserRecipt.Enabled = true;
                dteDepartmentBack.Enabled = true;
            }
        }

        private void cboUserRecipt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboUserRecipt.EditValue != null)
                {
                    var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUserRecipt.EditValue.ToString());
                    if (checkUser != null)
                    {
                        txtUserRecipt.Text = checkUser.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserReciptKHTH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboUserReciptKHTH.EditValue != null)
                {
                    var checkUser = this.lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == cboUserReciptKHTH.EditValue.ToString());
                    if (checkUser != null)
                    {
                        txtUserReciptKHTH.Text = checkUser.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (currentADO.processType == Desktop.ADO.MRSummaryDetailADO.OpenFrom.MedicalStoreV2)
                {
                    CheckEditChangeText(chkCheckCom.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                SummaryDetail data = null;
                if (e.RowHandle > -1)
                {
                    data = (SummaryDetail)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IS_CHECKER_CHECK")
                    {
                        e.RepositoryItem = (data.IS_CHECKER_NOT_USED ? repChkIsCheckerCheckDisable : repChkIsCheckerCheck);
                    }
                    else if (e.Column.FieldName == "IS_CHECKER_NOT_USED")
                    {
                        e.RepositoryItem = (data.IS_CHECKER_CHECK ? repChkIsCheckerNotUsedDisable : repChkIsCheckerNotUsedEnable);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dteDepartmentBack_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    dteDepartmentBack.EditValue = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteSave_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    dteSave.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidationDateTime()
        {
            try
            {
                ExpiredDateValidationRule validate = new ExpiredDateValidationRule();
                validate.dtExpiredDate = dteSubmitOne;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dteSubmitOne, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSpin()
        {
            try
            {
                SpinValidationRule validate = new SpinValidationRule();
                validate.dtExpiredDate = spnLowDay;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spnLowDay, validate);
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
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                LoadGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedical_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedical.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void gridView1_ShownEditor(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
        //        SummaryDetail data = view.GetFocusedRow() as SummaryDetail;
        //        if (view.FocusedColumn.FieldName == "IS_CHECKER_NOT_USED" && view.ActiveEditor is CheckEdit)
        //        {
        //            CheckEdit editor = view.ActiveEditor as CheckEdit;
        //            if (data.IS_CHECKER_CHECK)
        //            {
        //                editor.ReadOnly = true;
        //                editor.Enabled = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
