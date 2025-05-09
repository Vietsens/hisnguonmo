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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmTutorial : HIS.Desktop.Utility.FormBase
    {
        int positionHandleControl = -1;
        ListMedicineADO listMedicineADO;
        HIS.Desktop.Common.DelegateRefreshData delegateRefresh;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.ServiceReqList";

        public frmTutorial(HIS.Desktop.Common.DelegateRefreshData delegateRefresh, ListMedicineADO listMedicineADO)
            : base(null)
        {
            InitializeComponent();
            try
            {
                SetCaptionByLanguageKey();
                this.listMedicineADO = listMedicineADO;
                this.delegateRefresh = delegateRefresh;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
                //Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqList.Resources.Lang", typeof(frmTutorial).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTutorial.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTutorial.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTutorial.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTutorial.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmTutorial.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmTutorial.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmTutorial.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTutorial.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciSpinCountUsedBefore.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTutorial.lciSpinCountUsedBefore.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.lciSpinCountUsedBefore.Text = Inventec.Common.Resource.Get.Value("frmTutorial.lciSpinCountUsedBefore.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTutorial.Text", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTutorial_Load(object sender, EventArgs e)
        {
            try
            {


                txtLieuDung.Text = listMedicineADO.HuongDanSuDung;
                txtCachDung.Text = listMedicineADO.CachDung;
                spinSpeed.EditValue = listMedicineADO.TocDoTruyen;
                CommonParam param = new CommonParam();
                HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                filter.ID = listMedicineADO.ExpMestMedicineId;
                var rs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (rs != null)
                {
                    var firstItem = rs.FirstOrDefault();
                    if (firstItem != null && firstItem.PREVIOUS_USING_COUNT != null)
                    {
                        spinCountUsedBefore.Value = Inventec.Common.TypeConvert.Parse.ToDecimal(firstItem.PREVIOUS_USING_COUNT.ToString());
                    }
                }
                long a;


                if (listMedicineADO.xpMestMedicine != null)
                {
                    a = listMedicineADO.xpMestMedicine.ID;
                }


                if (listMedicineADO.serviceReqMety != null && listMedicineADO.serviceReqMety.HTU_TEXT == null)
                { 
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (listMedicineADO.USE_TIME_TO != null)
                {
                    DateTime? useTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.USE_TIME_TO.Value);
                    Inventec.Common.Logging.LogSystem.Debug("useTimeTo" + useTimeTo);
                    DateTime? intructionTime = null;
                    if (listMedicineADO.USE_TIME.HasValue)
                        intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.USE_TIME.Value);
                    else
                        intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.TDL_INTRUCTION_TIME);
                    Inventec.Common.Logging.LogSystem.Debug("intructionTime" + intructionTime);
                    if (useTimeTo.Value.CompareTo(intructionTime) == 0)
                    {
                        spinDayNumber.EditValue = 1;
                    }
                    else
                    {
                        TimeSpan? timeSpan = useTimeTo - intructionTime;
                        if (timeSpan.HasValue)
                        {
                            spinDayNumber.EditValue = timeSpan.Value.Days + 1;
                        }
                    }
                }
                else if (listMedicineADO.serviceReqMety.USE_TIME_TO != null)
                {
                    DateTime? useTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.serviceReqMety.USE_TIME_TO.Value);
                    Inventec.Common.Logging.LogSystem.Debug("useTimeTo" + useTimeTo);

                    DateTime? intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.serviceReqMety.CREATE_TIME.Value);
                    Inventec.Common.Logging.LogSystem.Debug("intructionTime" + intructionTime);
                    if (useTimeTo.Value.CompareTo(intructionTime) == 0)
                    {
                        spinDayNumber.EditValue = 1;
                    }
                    else
                    {
                        TimeSpan? timeSpan = useTimeTo - intructionTime;
                        if (timeSpan.HasValue)
                        {
                            spinDayNumber.EditValue = timeSpan.Value.Days + 1;
                        }
                    }
                }
                ValidationMaxlength(txtLieuDung, 1000);
                ValidationMaxlength(txtCachDung, 1024);
               // ValidateMaxLength2(txtCachDung, 1024);
                ValidationBiggerThan0(spinDayNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                bool success = false;
                if (!dxValidationProvider1.Validate())
                    return;
               
                CommonParam param = new CommonParam();

                if (listMedicineADO.serviceReqMety == null)
                {
                    HIS_EXP_MEST_MEDICINE expMestMedicine = new HIS_EXP_MEST_MEDICINE();
                    expMestMedicine.ID = listMedicineADO.ExpMestMedicineId;
                    expMestMedicine.TUTORIAL = txtLieuDung.Text.Trim();
                    expMestMedicine.HTU_TEXT = txtCachDung.Text.Trim();
                    expMestMedicine.SPEED = (decimal?)spinSpeed.EditValue;
                    DateTime? intructionDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.USE_TIME > 0 ? listMedicineADO.USE_TIME.Value : listMedicineADO.TDL_INTRUCTION_TIME);
                    long addNumber = 0;
                    //if (spinDayNumber.EditValue.ToString() == "0")
                    //{
                    //    if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.Ngaynhapphailonhon0, Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                    //        return;
                    //}
                    if (spinDayNumber.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(spinDayNumber.EditValue.ToString()) > 0)
                    {
                        addNumber = Inventec.Common.TypeConvert.Parse.ToInt64(spinDayNumber.EditValue.ToString()) - 1;
                    }
                    if (spinCountUsedBefore.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(spinCountUsedBefore.EditValue.ToString()) > 0)
                    {
                        expMestMedicine.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinCountUsedBefore.Value.ToString());
                    }
                    DateTime useTimeTo = intructionDate.Value.AddDays(addNumber);
                    expMestMedicine.USE_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(useTimeTo);
                    WaitingManager.Show();
                    var rs = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>("api/HisExpMestMedicine/UpdateCommonInfo", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicine, param);
                    if (rs != null)
                    {
                        success = true;
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                        this.Close();
                    }
                    WaitingManager.Hide();
                }
                else
                {
                    listMedicineADO.serviceReqMety.TUTORIAL = txtLieuDung.Text.Trim();
                    listMedicineADO.serviceReqMety.HTU_TEXT = txtCachDung.Text.Trim();
                    listMedicineADO.serviceReqMety.SPEED = (decimal?)spinSpeed.EditValue;
                    if (listMedicineADO.CachDung == null)
                    {

                        layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;


                    }
                    DateTime? intructionDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(listMedicineADO.serviceReqMety.CREATE_TIME.Value);
                    long addNumber = 0;
                    if (spinDayNumber.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(spinDayNumber.EditValue.ToString()) > 0)
                    {
                        addNumber = Inventec.Common.TypeConvert.Parse.ToInt64(spinDayNumber.EditValue.ToString()) - 1;
                    }
                    DateTime useTimeTo = intructionDate.Value.AddDays(addNumber);
                    listMedicineADO.serviceReqMety.USE_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(useTimeTo);
                    WaitingManager.Show();
                    var rs = new BackendAdapter(param).Post<HIS_SERVICE_REQ_METY>("api/HisServiceReqMety/UpdateCommonInfo", ApiConsumer.ApiConsumers.MosConsumer, listMedicineADO.serviceReqMety, param);
                    if (rs != null)
                    {
                        success = true;
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                        this.Close();
                    }
                    WaitingManager.Hide();
                }

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                try
                {
                    BaseEdit edit = e.InvalidControl as BaseEdit;
                    if (edit == null)
                        return;

                    BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                    if (viewInfo == null)
                        return;

                    if (this.positionHandleControl == -1)
                    {
                        this.positionHandleControl = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                    if (this.positionHandleControl > edit.TabIndex)
                    {
                        this.positionHandleControl = edit.TabIndex;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
       
        private void ValidationMaxlength(MemoEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.maxLength = maxLength;
                validRule.memoEdit = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxLength2(MemoEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength2 validRule = new ValidateMaxLength2();
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;

                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationBiggerThan0(SpinEdit control)
        {
            try
            {
                ValidateBiggerThan0 validRule = new ValidateBiggerThan0();
                validRule.spinEdit = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        if (item.KEY == spinDayNumber.Name)
                        {
                            spinDayNumber.EditValue = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDayNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLieuDung.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSpeed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
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
                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                delegateRefresh = null;
                listMedicineADO = null;
                positionHandleControl = 0;
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.spinDayNumber.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinDayNumber_PreviewKeyDown);
                this.spinSpeed.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinSpeed_PreviewKeyDown);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmTutorial_Load);
                lciSpinCountUsedBefore = null;
                spinCountUsedBefore = null;
                emptySpaceItem2 = null;
                layoutControlItem4 = null;
                spinDayNumber = null;
                layoutControlItem3 = null;
                layoutControlItem5 = null;
                spinSpeed = null;
                dxValidationProvider1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                txtLieuDung = null;
                txtCachDung = null;
                btnSave = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
