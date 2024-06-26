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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TransDepartment.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.TransDepartment.Loader;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.SecondaryIcd;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.TransDepartment
{
    public partial class frmDepartmentTran : HIS.Desktop.Utility.FormBase
    {
        List<HIS_DEPARTMENT> listDepartments = new List<HIS_DEPARTMENT>();
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereservs = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12> servicereq12 = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>();
        internal long treatmentId = 0;
        internal long departmentId = 0;
        int positionHandleControl = -1;
        private WaitDialogForm waitLoad = null;

        long roomId;
        //int ActionType = 0;
        HIS.Desktop.Common.RefeshReference RefeshReference;
        DelegateReturnSuccess DelegateReturnSuccess = null;
        V_HIS_DEPARTMENT_TRAN departmentTran = null;

        bool isView = false;
        HIS.UC.Icd.IcdProcessor icdProcessor = null;
        UserControl ucIcd = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        public long WarningOptionInCaseOfUnassignTrackingServiceReq;
        public long IsNotShowOutMediAndMate;
        public frmDepartmentTran(Inventec.Desktop.Common.Modules.Module moduleData, TransDepartmentADO data, HIS.Desktop.Common.RefeshReference RefeshReference, DelegateReturnSuccess DelegateReturnSuccess)
            : base(moduleData)
        {
            try
            {

                InitializeComponent();
                //this.ucBedRoomPartialControl = bedRoomPartialControl;
                this.treatmentId = data.TreatmentId;
                this.departmentId = data.DepartmentId;
                this.roomId = moduleData.RoomId;
                this.RefeshReference = RefeshReference;
                this.DelegateReturnSuccess = DelegateReturnSuccess;
                this.chkAutoLeave.Checked = true;
                this.currentModule = moduleData;
                this.isView = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        public frmDepartmentTran(Inventec.Desktop.Common.Modules.Module moduleData, TransDepartmentADO data, HIS.Desktop.Common.RefeshReference RefeshReference, DelegateReturnSuccess DelegateReturnSuccess, V_HIS_DEPARTMENT_TRAN departmentTran, bool _isView)
            : base(moduleData)
        {
            try
            {

                InitializeComponent();
                //this.ucBedRoomPartialControl = bedRoomPartialControl;
                this.treatmentId = data.TreatmentId;
                this.departmentId = data.DepartmentId;
                this.roomId = moduleData.RoomId;
                this.departmentTran = departmentTran;
                this.currentModule = moduleData;
                this.RefeshReference = RefeshReference;
                this.DelegateReturnSuccess = DelegateReturnSuccess;
                this.chkAutoLeave.Checked = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd") == 1 ? true : false;
                this.isView = _isView;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void frmDepartmentTran_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    WarningOptionInCaseOfUnassignTrackingServiceReq = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.TransDepartment.WarningOptionInCaseOfUnassignTrackingServiceReq"));
                    IsNotShowOutMediAndMate = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate"));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                btnSave.Enabled = isView;

                ValidControl();
                //if (IcdGeneraCFG.AutoCheckIcd == "1")
                //{
                //    chkIcds.Checked = true;
                //}
                LoadCboChuanDoanDT();
                InitUcSecondaryIcd();
                if (this.departmentTran == null)
                {
                    InitControl(true);
                    dtLogTime.EditValue = DateTime.Now;
                    dtLogTime.Enabled = false;
                    lciDTLogTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Size = new Size(this.Size.Width, this.Size.Height - 24);
                    LoadDataFillForm();

                }
                else
                {
                    InitControl(false);
                    cboDepartment.Enabled = false;
                    txtDepartmentCode.Enabled = false;
                    //if (!string.IsNullOrEmpty(this.departmentTran.ICD_CODE))
                    //{
                    HIS.UC.Icd.ADO.IcdInputADO inputAdo = new HIS.UC.Icd.ADO.IcdInputADO();
                    inputAdo.ICD_CODE = this.departmentTran.ICD_CODE;
                    inputAdo.ICD_NAME = this.departmentTran.ICD_NAME;

                    if (icdProcessor != null && ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, inputAdo);
                    }
                    //}
                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subAdo = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                    subAdo.ICD_SUB_CODE = this.departmentTran.ICD_SUB_CODE;
                    subAdo.ICD_TEXT = this.departmentTran.ICD_TEXT;

                    if (subIcdProcessor != null && ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subAdo);
                    }

                    cboDepartment.EditValue = this.departmentTran.DEPARTMENT_ID;
                    if (!string.IsNullOrEmpty(cboDepartment.Text))
                        txtDepartmentCode.Text = this.departmentTran.DEPARTMENT_CODE;
                    if (this.departmentTran.DEPARTMENT_IN_TIME != null)
                    {
                        lciDTLogTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                        dtLogTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.departmentTran.DEPARTMENT_IN_TIME ?? 0);
                        ValidLogTime();
                    }
                    else
                    {
                        dtLogTime.EditValue = DateTime.Now;
                        dtLogTime.Enabled = false;
                        lciDTLogTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.Size = new Size(this.Size.Width, this.Size.Height - 24);
                        //dtLogTime.EditValue = null;
                    }
                }
                LoadDefault();

                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                var listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmDepartmentTran.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmDepartmentTran.cboICDSub.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                chkAutoLeave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.TransDepartment.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TransDepartment.Resources.Lang", typeof(frmDepartmentTran).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentTran.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoLeave.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDepartmentTran.chkAutoLeave.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDepartmentTran.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDTLogTime.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.lciDTLogTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartmentTo.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.lciDepartmentTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmDepartmentTran.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitControl(bool view)
        {
            try
            {
                List<HIS_DEPARTMENT> data = new List<HIS_DEPARTMENT>();
                if (view)
                {
                    data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.ID != this.departmentId && o.IS_ACTIVE == 1).ToList();
                }
                else
                {
                    data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                }
                if (chkCCS.Checked == true)
                {
                    data = data.Where(o => o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()).ToList();
                }
                var branch = BranchDataWorker.GetCurrentBranchId();
                data = data.OrderByDescending(o => o.BRANCH_ID == branch).ToList();
                this.cboDepartment.Properties.DataSource = data;
                listDepartments = data;
                DepartmentLoader.LoadDataToCombo(this.cboDepartment);

                //TreatmentTypeLoader.LoadDataToComboTreatmentType(this.cboTreatmentType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadKeysFromlanguage()
        {
            try
            {
                //this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMDEPARTMENTTRAN_FRM_DEPARTMENT_TRAN", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmDepartmentTran, EXE.LOGIC.Base.LanguageManager.GetCulture());
                //lciDTLogTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMDEPARTMENTTRAN_LCI_D_T_LOG_TIME", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmDepartmentTran, EXE.LOGIC.Base.LanguageManager.GetCulture());
                //lciDepartmentTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMDEPARTMENTTRAN_LCI_DEPARTMENT_TO", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmDepartmentTran, EXE.LOGIC.Base.LanguageManager.GetCulture());
                //chkAutoLeave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMDEPARTMENTTRAN_LCI_AUTO_LEAVE", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmDepartmentTran, EXE.LOGIC.Base.LanguageManager.GetCulture());
                //btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRMDEPARTMENTTRAN_BTN_SAVE", EXE.APP.Resources.ResourceLanguageManager.LanguageFrmDepartmentTran, EXE.LOGIC.Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDepartmentCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                            cboDepartment.Properties.Buttons[1].Visible = true;
                            if (icdProcessor != null && ucIcd != null)
                            {
                                icdProcessor.FocusControl(ucIcd);
                            }
                        }
                    }
                    else
                    {
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        }
                        if (icdProcessor != null && ucIcd != null)
                        {
                            icdProcessor.FocusControl(ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLogTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void dtLogTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                txtDepartmentCode.Focus();
                txtDepartmentCode.SelectAll();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.Properties.Buttons[1].Visible = false;
                    cboDepartment.EditValue = null;
                    txtDepartmentCode.Text = "";
                    cboDepartment.Focus();
                    cboDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoLeave_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        bool IsValiICD()
        {
            bool result = true;
            try
            {
                result = (bool)icdProcessor.ValidationIcd(ucIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsValiICDSub()
        {
            bool result = true;
            try
            {
                result = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void TranslateDepartment(ref bool success, CommonParam param)
        {
            HisDepartmentTranSDO hisDepartmentTranSDO = new HisDepartmentTranSDO();

            if (icdProcessor != null && ucIcd != null)
            {
                var icd = (HIS.UC.Icd.ADO.IcdInputADO)icdProcessor.GetValue(ucIcd);
                if (icd != null)
                {
                    //hisDepartmentTranSDO.IcdId = icd.ICD_ID;
                    hisDepartmentTranSDO.IcdName = icd.ICD_NAME;
                    hisDepartmentTranSDO.IcdCode = icd.ICD_CODE;

                }
            }
            //service.ICD = txtICD.Text;
            if (subIcdProcessor != null && ucSecondaryIcd != null)
            {
                var subIcd = (HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcdProcessor.GetValue(ucSecondaryIcd);
                if (subIcd != null)
                {
                    hisDepartmentTranSDO.IcdSubCode = subIcd.ICD_SUB_CODE;
                    hisDepartmentTranSDO.IcdText = subIcd.ICD_TEXT;
                }
            }

            hisDepartmentTranSDO.RequestRoomId = this.roomId;
            hisDepartmentTranSDO.TreatmentId = this.treatmentId;
            hisDepartmentTranSDO.Time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;

            UpdateDepartmentTranFromDataForm(ref hisDepartmentTranSDO);

            if (departmentTran == null)
            {
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_DEPARTMENT_TRAN>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_CREATE, ApiConsumers.MosConsumer, hisDepartmentTranSDO, null);
                if (rs != null)
                {
                    success = true;
                }
            }
            else
            {
                var departmentTranMap = new HIS_DEPARTMENT_TRAN();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT_TRAN>(departmentTranMap, departmentTran);
                departmentTranMap.DEPARTMENT_ID = hisDepartmentTranSDO.DepartmentId;
                //departmentTranMap.ICD_ID = hisDepartmentTranSDO.IcdId; 
                departmentTranMap.ICD_CODE = hisDepartmentTranSDO.IcdCode;
                departmentTranMap.ICD_NAME = hisDepartmentTranSDO.IcdName;
                departmentTranMap.ICD_SUB_CODE = hisDepartmentTranSDO.IcdSubCode;
                departmentTranMap.ICD_TEXT = hisDepartmentTranSDO.IcdText;

                if (this.departmentTran.DEPARTMENT_IN_TIME != null)
                {
                    departmentTranMap.DEPARTMENT_IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime);
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_DEPARTMENT_TRAN>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_UPDATE, ApiConsumers.MosConsumer, departmentTranMap, null);
                if (rs != null)
                {
                    success = true;
                }
            }

            WaitingManager.Hide();
        }

        private void ConditionA(ref bool success, ref CommonParam param)
        {
            try
            {
                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.TREATMENT_ID = treatmentId;
                sereServFilter.HAS_EXECUTE = true;
                sereServFilter.TDL_REQUEST_DEPARTMENT_ID = departmentId;
                sereServFilter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.Now();
                sereservs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                if (room != null)
                    this.departmentId = room.DEPARTMENT_ID;
                WaitingManager.Hide();
                sereservs = sereservs.Where(o => o.TDL_INTRUCTION_TIME > Inventec.Common.DateTime.Get.Now() && o.AMOUNT != 0).ToList();
                if (sereservs != null && sereservs.Count() > 0)
                {
                    var sereservscode = sereservs.Select(o => o.TDL_SERVICE_REQ_CODE).Distinct().ToList();
                    if (MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscode) + " có thời gian y lệnh lớn hơn thời gian hiện tại. Bạn có muốn chuyển khoa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        TranslateDepartment(ref success, param);
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                    }
                }
                else
                {
                    TranslateDepartment(ref success, param);
                    Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MessageDepartment(ref bool success, ref CommonParam param)
        {
            try
            {
                if (MessageBox.Show("Bạn chưa chỉ định dịch vụ Phẫu thuật. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    TranslateDepartment(ref success, param);
                    Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckCondition(ref bool success, ref CommonParam param)
        {
            try
            {
                MOS.Filter.HisServiceReqFilter filterS = new HisServiceReqFilter();
                filterS.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT;
                filterS.SERVICE_REQ_STT_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL };
                filterS.REQUEST_DEPARTMENT_ID = departmentId;
                filterS.TREATMENT_ID = treatmentId;
                var checkServicereq = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filterS, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkServicereq), checkServicereq));
                if (checkServicereq != null && checkServicereq.Count > 0)
                {
                    checkServicereq = checkServicereq.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                    if (checkServicereq != null && checkServicereq.Count > 0)
                    {
                        TranslateDepartment(ref success, param);
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                    }
                    else
                    {
                        MessageDepartment(ref success, ref param);
                    }
                }
                else
                {
                    MessageDepartment(ref success, ref param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConditionB(ref bool success, ref CommonParam param, bool IsWarningWhenIsNoSurg)
        {
            try
            {
                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.TREATMENT_ID = treatmentId;
                sereServFilter.HAS_EXECUTE = true;
                sereServFilter.TDL_REQUEST_DEPARTMENT_ID = departmentId;
                //sereServFilter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.Now();
                sereservs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                V_HIS_ROOM roomA = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                if (roomA != null)
                    this.departmentId = roomA.DEPARTMENT_ID;
                WaitingManager.Hide();
                var sereservsIntruction = sereservs.Where(o => o.TDL_INTRUCTION_TIME > Inventec.Common.DateTime.Get.Now() && o.AMOUNT != 0).ToList();
                if (sereservsIntruction != null && sereservsIntruction.Count() > 0)
                {
                    var sereservscodeA = sereservs.Select(o => o.TDL_SERVICE_REQ_CODE).Distinct().ToList();
                    if (MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscodeA) + " có thời gian y lệnh lớn hơn thời gian hiện tại. Bạn có muốn chuyển khoa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {

                        ConditionBplus(ref success, ref param, IsWarningWhenIsNoSurg);
                    }
                }
                else
                {
                    ConditionBplus(ref success, ref param, IsWarningWhenIsNoSurg);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConditionBplus(ref bool success, ref CommonParam param, bool IsWarningWhenIsNoSurg)
        {
            try
            {

                if (IsNotShowOutMediAndMate == 1)
                {
                    List<long> listL = new List<long>();
                    listL.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                    listL.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN);
                    MOS.Filter.HisServiceReqView12Filter filter = new HisServiceReqView12Filter();
                    // filter.TRACKING_ID = null;
                    filter.REQUEST_DEPARTMENT_ID = departmentId;
                    filter.TREATMENT_ID = treatmentId;
                    filter.TREATMENT_TYPE_ID = 3;
                    filter.NOT_IN_SERVICE_REQ_TYPE_IDs = listL;
                    servicereq12 = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>>("api/HisServiceReq/GetView12", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                    WaitingManager.Hide();
                    servicereq12 = servicereq12.Where(o => o.TRACKING_ID == null).ToList();
                    if (servicereq12 != null && servicereq12.Count() > 0)
                    {

                        var sereservscode = servicereq12.Select(o => o.SERVICE_REQ_CODE).Distinct().ToList();
                        if (WarningOptionInCaseOfUnassignTrackingServiceReq == 1)
                        {
                            MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscode) + " chưa gắn tờ điều trị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                            return;
                        }
                        else if (WarningOptionInCaseOfUnassignTrackingServiceReq == 2)
                        {
                            if (MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscode) + " chưa gắn tờ điều trị. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                if (IsWarningWhenIsNoSurg)
                                {
                                    CheckCondition(ref success, ref param);
                                }
                                else
                                {
                                    TranslateDepartment(ref success, param);
                                    Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                                }
                            }
                        }
                        else if (WarningOptionInCaseOfUnassignTrackingServiceReq == 3)
                        {
                            List<string> serviceReqCode = new List<string>();
                            foreach (var item in servicereq12)
                            {
                                if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                                  || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                                  || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                                {
                                    if (sereservs.Exists(o => o.SERVICE_REQ_ID == item.ID && o.EXP_MEST_MEDICINE_ID != null))
                                    {
                                        serviceReqCode.Add(item.SERVICE_REQ_CODE);
                                    }
                                }
                                else
                                    serviceReqCode.Add(item.SERVICE_REQ_CODE);
                            }
                            if (serviceReqCode == null || serviceReqCode.Count == 0 || MessageBox.Show("Các y lệnh " + string.Join(", ", serviceReqCode) + " chưa gắn tờ điều trị. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                if (IsWarningWhenIsNoSurg)
                                {
                                    CheckCondition(ref success, ref param);
                                }
                                else
                                {
                                    TranslateDepartment(ref success, param);
                                    Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (IsWarningWhenIsNoSurg)
                        {
                            CheckCondition(ref success, ref param);
                        }
                        else
                        {
                            TranslateDepartment(ref success, param);
                            Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        }
                    }
                }
                else
                {
                    List<long> listL = new List<long>();
                    listL.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
                    listL.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN);
                    MOS.Filter.HisServiceReqView12Filter filter = new HisServiceReqView12Filter();
                    // filter.TRACKING_ID = null;
                    filter.REQUEST_DEPARTMENT_ID = departmentId;
                    filter.TREATMENT_ID = treatmentId;
                    filter.TREATMENT_TYPE_ID = 3;
                    filter.NOT_IN_SERVICE_REQ_TYPE_IDs = listL;
                    servicereq12 = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>>("api/HisServiceReq/GetView12", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    WaitingManager.Hide();
                    servicereq12 = servicereq12.Where(o => o.TRACKING_ID == null).ToList();
                    if (servicereq12 != null && servicereq12.Count() > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => servicereq12), servicereq12));
                        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12> servicereq12_ = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_12>();
                        foreach (var item in servicereq12)
                        {
                            if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                            {
                                if (item.EXP_MEST_ID != null && item.EXP_MEST_ID > 0)
                                {
                                    servicereq12_.Add(item);

                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                }
                            }
                            else
                            {
                                servicereq12_.Add(item);
                            }
                        }
                        if (servicereq12_ != null && servicereq12_.Count() > 0)
                        {
                            var sereservscode = servicereq12_.Select(o => o.SERVICE_REQ_CODE).Distinct().ToList();

                            if (WarningOptionInCaseOfUnassignTrackingServiceReq == 1)
                            {
                                MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscode) + " chưa gắn tờ điều trị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                                return;
                            }
                            else if (WarningOptionInCaseOfUnassignTrackingServiceReq == 2)
                            {
                                if (MessageBox.Show("Các y lệnh " + string.Join(", ", sereservscode) + " chưa gắn tờ điều trị. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    if (IsWarningWhenIsNoSurg)
                                    {
                                        CheckCondition(ref success, ref param);
                                    }
                                    else
                                    {
                                        TranslateDepartment(ref success, param);
                                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                                    }
                                }
                            }
                            else if (WarningOptionInCaseOfUnassignTrackingServiceReq == 3)
                            {
                                List<string> serviceReqCode = new List<string>();
                                foreach (var item in servicereq12)
                                {
                                    if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                                      || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                                      || item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                                    {
                                        if (sereservs.Exists(o => o.SERVICE_REQ_ID == item.ID && o.EXP_MEST_MEDICINE_ID != null))
                                        {
                                            serviceReqCode.Add(item.SERVICE_REQ_CODE);
                                        }
                                    }
                                    else
                                        serviceReqCode.Add(item.SERVICE_REQ_CODE);
                                }
                                if (serviceReqCode == null || serviceReqCode.Count == 0 || MessageBox.Show("Các y lệnh " + string.Join(", ", serviceReqCode) + " chưa gắn tờ điều trị. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    if (IsWarningWhenIsNoSurg)
                                    {
                                        CheckCondition(ref success, ref param);
                                    }
                                    else
                                    {
                                        TranslateDepartment(ref success, param);
                                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (IsWarningWhenIsNoSurg)
                            {
                                CheckCondition(ref success, ref param);
                            }
                            else
                            {
                                TranslateDepartment(ref success, param);
                                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                            }
                        }
                    }
                    else
                    {
                        if (IsWarningWhenIsNoSurg)
                        {
                            CheckCondition(ref success, ref param);
                        }
                        else
                        {
                            TranslateDepartment(ref success, param);
                            Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {

                dtLogTime.Update();
                // goi api o day
                bool vali = true;
                this.positionHandleControl = -1;
                vali = IsValiICD() && vali;
                vali = IsValiICDSub() && vali;
                vali = dxValidationProvider1.Validate() && vali;
                if (!vali)//!dxValidationProvider1.Validate() || !IsValiICD())
                    return;

                //minhnq
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = treatmentId;
                serviceReqFilter.HAS_EXECUTE = true;
                serviceReqFilter.REQUEST_DEPARTMENT_ID = departmentId;
                serviceReqFilter.INTRUCTION_TIME_FROM = dtLogTime.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) : Inventec.Common.DateTime.Get.Now();

                var lstServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null);
                if (lstServiceReq != null && lstServiceReq.Count() > 0)
                {
                    if (MessageBox.Show("Các y lệnh " + string.Join(", ", string.Join(",", lstServiceReq.Select(o => o.SERVICE_REQ_CODE).ToList())) + " có thời gian y lệnh lớn hơn thời gian chuyển khoa. Bạn có muốn chuyển khoa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                WaitingManager.Show();
                if (WarningOptionInCaseOfUnassignTrackingServiceReq == 0 || (WarningOptionInCaseOfUnassignTrackingServiceReq != 1 && WarningOptionInCaseOfUnassignTrackingServiceReq != 2 && WarningOptionInCaseOfUnassignTrackingServiceReq != 3))
                {
                    ConditionA(ref success, ref param);
                }
                else if (WarningOptionInCaseOfUnassignTrackingServiceReq == 1 || WarningOptionInCaseOfUnassignTrackingServiceReq == 2 || WarningOptionInCaseOfUnassignTrackingServiceReq == 3)
                {
                    var checkDepartment = listDepartments.FirstOrDefault(o => o.ID == Int64.Parse(cboDepartment.EditValue.ToString()));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkDepartment), checkDepartment));
                    ConditionB(ref success, ref param, checkDepartment != null && checkDepartment.WARNING_WHEN_IS_NO_SURG == 1);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
            #region Hien thi message thong bao
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion

            if (success)
            {
                if (RefeshReference != null)
                    RefeshReference();
                if (DelegateReturnSuccess != null)
                    DelegateReturnSuccess(success);
                this.Close();
            }
        }

        void UpdateDepartmentTranFromDataForm(ref HisDepartmentTranSDO DepartmentTranData)
        {
            try
            {
                if (cboDepartment.EditValue != null)
                    DepartmentTranData.DepartmentId = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString());
                DepartmentTranData.AutoLeaveRoom = (chkAutoLeave.Checked);
                DepartmentTranData.IsReceive = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDepartmentTran_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) SendKeys.Send("{TAB}");
        }

        private void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            //if (e.CloseMode == PopupCloseMode.Normal)
            //{
            //	if (cboTreatmentType.EditValue != null)
            //	{
            //		MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
            //		if (data != null)
            //		{
            //			txtDepartmentCode.Text = data.TREATMENT_TYPE_CODE;
            //			cboTreatmentType.Properties.Buttons[1].Visible = true;
            //		}
            //	}
            //	//chkAutoLeave.Focus();
            //}
        }

        private void chkCCS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.departmentTran == null)
                {
                    InitControl(true);
                }
                else
                {
                    InitControl(false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //private void chkIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtICDSub.Focus();
        //            txtICDSub.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboICDSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            chkAutoLeave.Properties.FullFocusRect = true;
        //            chkAutoLeave.Focus();
        //        }
        //        else if (e.KeyCode == Keys.F1)
        //        {
        //            //hien thi popup chon icd
        //            WaitingManager.Show();

        //            HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(stringIcds, cboICDSub.Text, cboICDSub.Text);

        //            List<object> listArgs = new List<object>();
        //            listArgs.Add(secondaryIcdADO);
        //            CallModule callModule = new CallModule(CallModule.SecondaryIcd, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

        //            chkAutoLeave.Properties.FullFocusRect = true;
        //            chkAutoLeave.Focus();
        //            WaitingManager.Hide();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void stringIcds(string icdCode, string icdName)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(icdCode))
        //        {
        //            txtICDSub.Text = icdCode;
        //        }
        //        if (!string.IsNullOrEmpty(icdName))
        //        {
        //            cboICDSub.Text = icdName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private bool CheckSubIcd()
        //{
        //    bool rs = false;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(txtICDSub.Text.Trim()))
        //        {
        //            var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
        //            var icd = icdAlls.FirstOrDefault(o => o.ICD_CODE == txtICDSub.Text.Trim());
        //            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Icd", icd));
        //            if (icd == null)
        //            {
        //                Inventec.Common.Logging.LogSystem.Warn("Vao log save" + txtICDSub.Text.Trim());
        //                MessageManager.Show(String.Format("Không tìm thấy Icd tương ứng với mã sau: {0}", txtICDSub.Text.Trim()));

        //                rs = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        return false;
        //    }
        //    return rs;
        //}

        //private void txtICDSub_Validating(object sender, CancelEventArgs e)
        //{
        //    try
        //    {
        //        string seperate = ";";
        //        string strIcdNames = "";
        //        string strWrongIcdCodes = "";
        //        string[] periodSeparators = new string[1];
        //        periodSeparators[0] = seperate;
        //        string[] arrIcdExtraCodes = txtICDSub.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
        //        if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
        //        {
        //            var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
        //            foreach (var itemCode in arrIcdExtraCodes)
        //            {
        //                var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
        //                if (icdByCode != null && icdByCode.ID > 0)
        //                {
        //                    strIcdNames += (seperate + icdByCode.ICD_NAME);
        //                }
        //                else
        //                {
        //                    strWrongIcdCodes += (seperate + itemCode);
        //                }
        //            }
        //            //strIcdNames += seperate;
        //            //if (!String.IsNullOrEmpty(strWrongIcdCodes))
        //            //{
        //            //    MessageManager.Show(String.Format(ResourceLanguageManager.KhongTimThayIcdTuongUng, strWrongIcdCodes));
        //            //    //txtIcdExtraCode.Focus();
        //            //    //txtIcdExtraCode.SelectAll();
        //            //}
        //        }
        //        cboICDSub.Text = strIcdNames;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtICDSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            cboICDSub.Focus();
        //            cboICDSub.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
