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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ContactDeclaration.ADO;
using HIS.Desktop.Plugins.ContactDeclaration.Resources;
using HIS.Desktop.Plugins.ContactDeclaration.UcObject;
using HIS.Desktop.Plugins.ContactDeclaration.Validate;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContactDeclaration.ContactDeclaration
{
    public partial class frmContactDeclaration : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        public List<HisEmployeeADO> lstHisEmployeeAdo { get; set; }
        public List<V_HIS_PATIENT_1> lstHisPatient { get; set; }
        public List<HIS_GENDER> lstHisGender { get; set; }
        public List<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO> lstAgeADO { get; set; }
        public List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> lstCommuneADO { get; set; }
        public List<V_SDA_PROVINCE> lstSdaProvince { get; set; }
        public List<V_SDA_DISTRICT> lstSdaDistrict { get; set; }
        public List<V_SDA_COMMUNE> lstSdaCommune { get; set; }

        List<ACS_USER> lstAcsUser = new List<ACS_USER>();
        List<ContactPointADO> lstContactPointADO = new List<ContactPointADO>();

        V_HIS_CONTACT_POINT ContactPointInformation = new V_HIS_CONTACT_POINT();
        V_HIS_CONTACT_POINT ContactPointList = new V_HIS_CONTACT_POINT();

        UcStaff UcStaffInformation;
        UcPatient UcPatientInformation;
        UcOrther UcOrtherInformation;

        UcStaff UcStaffList;
        UcPatient UcPatientList;
        UcOrther UcOrtherList;

        bool isSave = false;
        int positionHandle = -1;

        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        public frmContactDeclaration(Inventec.Desktop.Common.Modules.Module moduledata, V_HIS_CONTACT_POINT ContactPoint)
            : base(moduledata)
        {
            InitializeComponent();

            this.moduleData = moduledata;
            this.ContactPointInformation = ContactPoint;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmContactDeclaration(Inventec.Desktop.Common.Modules.Module moduledata)
            : base(moduledata)
        {
            InitializeComponent();

            this.moduleData = moduledata;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmContactDeclaration_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("ContactPointInformation: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ContactPointInformation), ContactPointInformation));
                //LoadEmployee();
                //LoadHisPatient();
                //LoadBackendDataWorker();

                lstContactPointADO = new List<ContactPointADO>();

                CreateThreadLoadData();

                CreatUC();

                SetDefautValueControl();

                //SetValuePatientInformation(null);



                if (this.ContactPointInformation != null && this.ContactPointInformation.ID > 0)
                {
                    if (this.ContactPointInformation.EMPLOYEE_ID != null)
                    {
                        chkStaff.Checked = true;
                    }
                    else if (this.ContactPointInformation.PATIENT_ID != null)
                    {
                        chkPatient.Checked = true;
                    }
                    else
                    {
                        chkOrther.Checked = true;
                        txtOrther.Text = this.ContactPointInformation.CONTACT_POINT_OTHER_TYPE_NAME;
                    }

                    SetReadOnlyContactInFor(true);
                    SetEnableContactList(true);

                    FillDataToGridControl(this.ContactPointInformation.ID);
                    //chkAmTinh1.Checked = this.ContactPointInformation.TEST_RESULT_1 == 0 ? true : false;
                    //chkAmTinh2.Checked = this.ContactPointInformation.TEST_RESULT_2 == 0 ? true : false;
                    //chkAmTinh3.Checked = this.ContactPointInformation.TEST_RESULT_3 == 0 ? true : false;

                    //chkDuongTinh1.Checked = this.ContactPointInformation.TEST_RESULT_1 == 1 ? true : false;
                    //chkDuongTinh2.Checked = this.ContactPointInformation.TEST_RESULT_2 == 1 ? true : false;
                    //chkDuongTinh3.Checked = this.ContactPointInformation.TEST_RESULT_3 == 1 ? true : false;

                    //SpinClassify.EditValue = this.ContactPointInformation.CONTACT_LEVEL;
                    SetValuePatientInformation(this.ContactPointInformation);

                }
                else
                {
                    SetReadOnlyContactInFor(false);

                    SetEnableContactList(false);
                    //SetValuePatientInformation(this.ContactPointInformation);
                }

                //set ngon ngu
                SetCaptionByLanguagekey();

                ValidateForm();

                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreatUC()
        {
            try
            {
                UcOrtherList = new UcOrther(lstHisGender, lstAgeADO, lstCommuneADO, lstSdaProvince, lstSdaDistrict, lstSdaCommune, this.SetValuePatientList, null);

                UcPatientList = new UcPatient(this.ContactPointList.PATIENT_ID, this.SetValuePatientList);


                UcStaffList = new UcStaff(lstHisEmployeeAdo, this.SetValuePatientList);

                UcOrtherInformation = new UcOrther(lstHisGender, lstAgeADO, lstCommuneADO, lstSdaProvince, lstSdaDistrict, lstSdaCommune, this.SetValuePatientInformation, this.ContactPointInformation);

                UcPatientInformation = new UcPatient(this.ContactPointInformation.PATIENT_ID, this.SetValuePatientInformation);

                UcStaffInformation = new UcStaff(lstHisEmployeeAdo, this.SetValuePatientInformation);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageMessage = new ResourceManager("HIS.Desktop.Plugins.ContactDeclaration.Resources.Lang", typeof(HIS.Desktop.Plugins.ContactDeclaration.ContactDeclaration.frmContactDeclaration).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.groupBox1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.groupBox2.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.labelControl2.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.labelControl1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.chkStaff.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkStaff.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkPatient.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkPatient.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkOrther.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkOrther.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkStaff1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkStaff1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkPatient1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkPatient1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkOrther1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkOrther1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.chkAmTinh1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkAmTinh1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkAmTinh2.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkAmTinh2.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkAmTinh3.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkAmTinh3.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.chkDuongTinh1.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkDuongTinh1.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkDuongTinh2.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkDuongTinh2.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.chkDuongTinh3.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.chkDuongTinh3.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.btnSave.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.btnNew.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.btnAdd.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.bbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.bbtnNew.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                //this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());
                //this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmContactDeclaration.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmContactDeclaration.Text", Resources.ResourceLanguageManager.LanguageMessage, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                if (chkOrther.Checked)
                {
                    ValidationSingleControl(txtOrther);
                }
                if (chkOrther1.Checked)
                {
                    ValidationSingleControl(txtOrther1);
                }
                ValidateSpinEdit(SpinClassify);
                //ValidationSingleControl(dtContactTime);
                ValidateSpinEdit(SpinClassify1);

                ValidationDatetime(dtContactTime);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateSpinEdit(SpinEdit control)
        {
            try
            {
                if (control.Enabled)
                {
                    ValidateSpinEdit validRule = new ValidateSpinEdit();
                    validRule.spinEdit = control;
                    validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(control, validRule);
                }
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
                if (control.Enabled)
                {
                    ControlEditValidationRule validRule = new ControlEditValidationRule();
                    validRule.editor = control;
                    validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(control, validRule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationDatetime(DateEdit control)
        {
            try
            {
                if (control.Enabled)
                {
                    ValidateDateTime validRule = new ValidateDateTime();
                    validRule.dateEdit = control;
                    //validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(control, validRule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            Thread threadEmployee = new System.Threading.Thread(LoadEmployee);
            //Thread threadLoadHisPatient = new System.Threading.Thread(LoadHisPatient);
            //Thread threadLoadBackendDataWorker = new System.Threading.Thread(LoadBackendDataWorker);
            Thread threadLoadPatientADataWorker = new System.Threading.Thread(LoadPatientADataWorker);
            try
            {
                threadEmployee.Start();
                threadLoadPatientADataWorker.Start();

                threadEmployee.Join();
                threadLoadPatientADataWorker.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadEmployee.Abort();
                threadLoadPatientADataWorker.Abort();
            }
        }

        private void LoadBackendDataWorker()
        {
            try
            {
                lstHisGender = BackendDataWorker.Get<HIS_GENDER>();
                lstAgeADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>();
                lstCommuneADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                lstSdaProvince = BackendDataWorker.Get<V_SDA_PROVINCE>();
                lstSdaDistrict = BackendDataWorker.Get<V_SDA_DISTRICT>();
                lstSdaCommune = BackendDataWorker.Get<V_SDA_COMMUNE>();

                if (this.lstHisGender == null)
                {
                    this.lstHisGender = new List<HIS_GENDER>();
                }

                if (this.lstAgeADO == null)
                {
                    this.lstAgeADO = new List<LocalStorage.BackendData.ADO.AgeADO>();
                }

                if (this.lstCommuneADO == null)
                {
                    this.lstCommuneADO = new List<LocalStorage.BackendData.ADO.CommuneADO>();
                }

                if (this.lstSdaDistrict == null)
                {
                    this.lstSdaDistrict = new List<V_SDA_DISTRICT>();
                }

                if (this.lstSdaProvince == null)
                {
                    this.lstSdaProvince = new List<V_SDA_PROVINCE>();
                }

                if (this.lstSdaCommune == null)
                {
                    this.lstSdaCommune = new List<V_SDA_COMMUNE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadEmployee()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("LoadEmployee1");
                lstHisEmployeeAdo = new List<HisEmployeeADO>();

                var HisEmployee = BackendDataWorker.Get<V_HIS_EMPLOYEE>();
                lstAcsUser = BackendDataWorker.Get<ACS_USER>();

                Inventec.Common.Logging.LogSystem.Warn("LoadEmployee3");
                if (HisEmployee != null && HisEmployee.Count > 0)
                {
                    foreach (var item in HisEmployee)
                    {
                        ACS_USER checkUserName = (lstAcsUser != null && lstAcsUser.Count > 0) ? lstAcsUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME) : null;
                        HisEmployeeADO ado = new HisEmployeeADO(item, checkUserName);
                        lstHisEmployeeAdo.Add(ado);
                    }
                }
                Inventec.Common.Logging.LogSystem.Warn("LoadEmployee4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientADataWorker()
        {
            Inventec.Common.Logging.LogSystem.Warn("LoadPatientADataWorker1");
            //LoadHisPatient();
            Inventec.Common.Logging.LogSystem.Warn("LoadPatientADataWorker2");
            LoadBackendDataWorker();
            Inventec.Common.Logging.LogSystem.Warn("LoadPatientADataWorker3");
        }

        private void LoadHisPatient(List<long> listId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("LoadHisPatient1");
                if (listId != null && listId.Count > 0)
                {
                    lstHisPatient = new List<V_HIS_PATIENT_1>();

                    listId = listId.Distinct().ToList();
                    int skip = 0;
                    while (listId.Count - skip > 0)
                    {
                        var lstIds = listId.Skip(skip).Take(100).ToList();
                        skip += 100;

                        lstHisPatient = new List<V_HIS_PATIENT_1>();
                        HisPatientView1Filter filter = new HisPatientView1Filter();
                        filter.IDs = lstIds;
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<V_HIS_PATIENT_1>>("api/HisPatient/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            lstHisPatient.AddRange(apiResult);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Warn("LoadHisPatient2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableContactList(bool enable)
        {
            try
            {
                isSave = enable;
                chkStaff1.Enabled = enable;
                chkPatient1.Enabled = enable;
                chkOrther1.Enabled = enable;
                dtContactTime.Enabled = enable;
                SpinClassify1.Enabled = enable;
                btnAdd.Enabled = enable;
                this.panelControl2.Enabled = enable;
                chkStaff1.Checked = enable;
                if (enable)
                {
                    chkStaff1.Focus();
                }
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetReadOnlyContactInFor(bool readOnly)
        {
            try
            {
                chkPatient.ReadOnly = readOnly;
                chkOrther.ReadOnly = readOnly;
                chkStaff.ReadOnly = readOnly;
                txtOrther.ReadOnly = readOnly;
                chkAmTinh1.ReadOnly = readOnly;
                chkAmTinh2.ReadOnly = readOnly;
                chkAmTinh3.ReadOnly = readOnly;
                chkDuongTinh1.ReadOnly = readOnly;
                chkDuongTinh2.ReadOnly = readOnly;
                chkDuongTinh3.ReadOnly = readOnly;
                SpinClassify.ReadOnly = readOnly;
                this.panelControl1.Enabled = !readOnly;
                btnSave.Enabled = !readOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SetDefautValueControl()
        {
            try
            {
                txtOrther.Enabled = false;
                txtOrther1.Enabled = false;
                if (this.ContactPointInformation == null || this.ContactPointInformation.ID <= 0)
                {
                    SetValueWhenChangeCheckInfo();
                }
                if (this.ContactPointList == null || this.ContactPointList.ID <= 0)
                {
                    SetValueWhenChangeCheckList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueWhenChangeCheckInfo()
        {
            try
            {
                txtOrther.Enabled = false;
                txtOrther.Text = "";
                chkAmTinh1.Checked = false;
                chkAmTinh2.Checked = false;
                chkAmTinh3.Checked = false;
                chkDuongTinh1.Checked = false;
                chkDuongTinh2.Checked = false;
                chkDuongTinh3.Checked = false;
                SpinClassify.EditValue = null;
                chkStaff.Checked = true;
                chkStaff.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueWhenChangeCheckList()
        {
            try
            {

                dtContactTime.DateTime = DateTime.Now;
                SpinClassify1.EditValue = null;
                txtOrther1.Enabled = false;
                txtOrther1.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultWhenChangeCheckInfo()
        {
            try
            {
                chkAmTinh1.Checked = false;
                chkAmTinh2.Checked = false;
                chkAmTinh3.Checked = false;
                chkDuongTinh1.Checked = false;
                chkDuongTinh2.Checked = false;
                chkDuongTinh3.Checked = false;
                SpinClassify.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        //private void SetDefaultWhenChangeChecklist()
        //{
        //    try
        //    {
        //        if (this.chkStaff1.Checked)
        //        {
        //            this.UcStaffList.SetValuecboStaff(null);
        //        }

        //        if (this.chkPatient1.Checked)
        //        {
        //            this.UcPatientList.SetValueeCboPatient(null);
        //        }

        //        if (this.chkOrther1.Checked)
        //        {
        //            this.UcOrtherList.SetDefautValueControl(null);
        //        }

        //        dtContactTime.DateTime = DateTime.Now;
        //        SpinClassify1.EditValue = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void SpinClassify1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValuePatientList(V_HIS_CONTACT_POINT data) 
        {
            try
            {
                if (data != null)
                {
                    ContactPointList = data;
                    if (data.CONTACT_LEVEL == null)
                    {
                        SpinClassify1.EditValue = ContactPointInformation.CONTACT_LEVEL + 1;
                    }
                    else if (data.CONTACT_LEVEL >= (ContactPointInformation.CONTACT_LEVEL + 1))
                    {
                        SpinClassify1.EditValue = ContactPointInformation.CONTACT_LEVEL + 1;
                    }
                    else
                    {
                        SpinClassify1.EditValue = data.CONTACT_LEVEL;
                    }


                    if (data.CONTACT_LEVEL < (ContactPointInformation.CONTACT_LEVEL - 1))
                    {
                        string FullName1 = "", FullName2 = "";

                        if (!String.IsNullOrEmpty(data.FULL_NAME))
                        {
                            FullName1 = data.FULL_NAME;
                        }
                        else if (data.EMPLOYEE_ID != null && String.IsNullOrEmpty(FullName1))
                        {
                            FullName1 = (this.lstHisEmployeeAdo != null && this.lstHisEmployeeAdo.Count > 0) ? lstHisEmployeeAdo.FirstOrDefault(o => o.ID == data.EMPLOYEE_ID).USERNAME : "";
                        }
                        else if (data.PATIENT_ID != null && String.IsNullOrEmpty(FullName1))
                        {
                            FullName1 = (this.lstHisPatient != null && this.lstHisPatient.Count > 0) ? lstHisPatient.FirstOrDefault(o => o.ID == data.PATIENT_ID).VIR_PATIENT_NAME : "";
                        }

                        if (!String.IsNullOrEmpty(ContactPointInformation.FULL_NAME))
                        {
                            FullName2 = ContactPointInformation.FULL_NAME;
                        }
                        else if (ContactPointInformation.EMPLOYEE_ID != null && String.IsNullOrEmpty(FullName2))
                        {
                            FullName2 = (this.lstHisEmployeeAdo != null && this.lstHisEmployeeAdo.Count > 0) ? lstHisEmployeeAdo.FirstOrDefault(o => o.ID == ContactPointInformation.EMPLOYEE_ID).USERNAME : "";
                        }
                        else if (ContactPointInformation.PATIENT_ID != null && String.IsNullOrEmpty(FullName2))
                        {
                            FullName1 = (this.lstHisPatient != null && this.lstHisPatient.Count > 0) ? lstHisPatient.FirstOrDefault(o => o.ID == data.PATIENT_ID).VIR_PATIENT_NAME : "";
                        }

                        WaitingManager.Hide();
                        if (MessageBox.Show(String.Format(ResourceMessage.CapNhatlaiThongTinPhanLoai, FullName1, data.CONTACT_LEVEL, FullName2), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            SpinClassify.EditValue = data.CONTACT_LEVEL + 1;

                            CommonParam param = new CommonParam();
                            HisContactLevelSDO ContactLevelSDO = new HisContactLevelSDO();

                            ContactLevelSDO.ContactLevel = (long)SpinClassify.Value;
                            ContactLevelSDO.ContactPointId = ContactPointInformation.ID;

                            var resultData = new BackendAdapter(param).Post<V_HIS_CONTACT_POINT>("/api/HisContactPoint/SetContactLevel", ApiConsumers.MosConsumer, ContactLevelSDO, param);

                            if (resultData != null)
                            {
                                FillDataToGridControl(resultData.ID);
                            }
                        }
                    }
                }
                else
                {
                    ContactPointList = new V_HIS_CONTACT_POINT();
                    dtContactTime.EditValue = DateTime.Now;
                    SpinClassify1.EditValue = ContactPointInformation.CONTACT_LEVEL + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValuePatientInformation(V_HIS_CONTACT_POINT data)
        {
            try
            {
                if (data != null)
                {
                    ContactPointInformation = data;
                    chkAmTinh1.Checked = data.TEST_RESULT_1 == 0 ? true : false;
                    chkAmTinh2.Checked = data.TEST_RESULT_2 == 0 ? true : false;
                    chkAmTinh3.Checked = data.TEST_RESULT_3 == 0 ? true : false;

                    chkDuongTinh1.Checked = data.TEST_RESULT_1 == 1 ? true : false;
                    chkDuongTinh2.Checked = data.TEST_RESULT_2 == 1 ? true : false;
                    chkDuongTinh3.Checked = data.TEST_RESULT_3 == 1 ? true : false;

                    SpinClassify.EditValue = data.CONTACT_LEVEL;
                }
                else
                {
                    ContactPointInformation = new V_HIS_CONTACT_POINT();
                    chkAmTinh1.Checked = false;
                    chkAmTinh2.Checked = false;
                    chkAmTinh3.Checked = false;

                    chkDuongTinh1.Checked = false;
                    chkDuongTinh2.Checked = false;
                    chkDuongTinh3.Checked = false;

                    SpinClassify.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAmTinh1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAmTinh1.Checked)
                {
                    chkDuongTinh1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDuongTinh1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDuongTinh1.Checked)
                {
                    chkAmTinh1.Checked = false;
                    SpinClassify.EditValue = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAmTinh2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAmTinh2.Checked)
                {
                    chkDuongTinh2.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDuongTinh2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDuongTinh2.Checked)
                {
                    chkAmTinh2.Checked = false;
                    SpinClassify.EditValue = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAmTinh3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAmTinh3.Checked)
                {
                    chkDuongTinh3.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDuongTinh3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDuongTinh3.Checked)
                {
                    chkAmTinh3.Checked = false;
                    SpinClassify.EditValue = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi Tao uc
        /// </summary>
        private void InitPanelControl(PanelControl panelControl, Control value)
        {
            try
            {
                panelControl.Controls.Clear();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                if (value != null)
                {
                    value.Dock = DockStyle.Fill;
                    panelControl.Controls.Add(value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkStaff_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkStaff.Checked)
                {

                    SetDefaultWhenChangeCheckInfo();

                    InitPanelControl(this.panelControl1, UcStaffInformation);

                    if (this.ContactPointInformation != null)
                    {
                        this.UcStaffInformation.SetValuecboStaff(this.ContactPointInformation.EMPLOYEE_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPatient_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPatient.Checked)
                {

                    List<V_HIS_CONTACT_POINT> GetContactPoint = new List<V_HIS_CONTACT_POINT>();
                    List<long> lst = new List<long>() { this.ContactPointInformation.ID };
                    GetContactPoint = dataContactPoint(lst);

                    if (GetContactPoint != null && GetContactPoint.Count > 0)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointInformation = GetContactPoint.FirstOrDefault();
                    }

                    SetDefaultWhenChangeCheckInfo();

                    InitPanelControl(this.panelControl1, UcPatientInformation);
                    if (this.ContactPointInformation != null)
                    {
                        this.UcPatientInformation.SetValueeCboPatient(this.ContactPointInformation.PATIENT_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkOrther_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetDefaultWhenChangeCheckInfo();

                if (chkOrther.Checked && chkOrther.Enabled == true)
                {

                    List<V_HIS_CONTACT_POINT> GetContactPoint = new List<V_HIS_CONTACT_POINT>();
                    List<long> lst = new List<long>() { this.ContactPointInformation.ID };
                    GetContactPoint = dataContactPoint(lst);

                    if (GetContactPoint != null && GetContactPoint.Count > 0)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointInformation = GetContactPoint.FirstOrDefault();
                    }
                    InitPanelControl(this.panelControl1, UcOrtherInformation);

                    txtOrther.Enabled = true;
                    if (this.ContactPointInformation != null)
                    {
                        this.txtOrther.Text = this.ContactPointInformation.CONTACT_POINT_OTHER_TYPE_NAME;
                    }
                }
                else
                {
                    txtOrther.Enabled = false;
                    dxValidationProvider1.SetValidationRule(txtOrther, null);
                }
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkStaff1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkStaff1.Checked && chkStaff1.Enabled == true)
                {

                    List<V_HIS_CONTACT_POINT> GetContactPoint = new List<V_HIS_CONTACT_POINT>();
                    List<long> lst = new List<long>() { this.ContactPointInformation.ID };
                    GetContactPoint = dataContactPoint(lst);

                    if (GetContactPoint != null && GetContactPoint.Count > 0)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointInformation = GetContactPoint.FirstOrDefault();
                    }

                    //SetDefaultWhenChangeChecklist();

                    InitPanelControl(this.panelControl2, UcStaffList);

                }
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
                if (!btnSave.Enabled)
                    return;

                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    Inventec.Common.Logging.LogSystem.Warn("btnSave_Click: Validate");
                    return;
                }

                if (chkOrther.Checked && !UcOrtherInformation.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcOrtherInformation: Validate");
                    return;
                }
                else if (chkPatient.Checked && !UcPatientInformation.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcPatientInformation: Validate");
                    return;
                }
                else if (chkStaff.Checked && !UcStaffInformation.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcStaffInformation: Validate");
                    return;
                }

                WaitingManager.Show();

                HIS_CONTACT_POINT updateDTO = new HIS_CONTACT_POINT();

                if (this.ContactPointInformation != null && this.ContactPointInformation.ID > 0)
                {
                    LoadCurrent(this.ContactPointInformation.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);


                var resultData = new BackendAdapter(param).Post<HIS_CONTACT_POINT>("/api/HisContactPoint/Save", ApiConsumers.MosConsumer, updateDTO, param);

                if (resultData != null)
                {

                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_CONTACT_POINT>(ContactPointInformation, resultData);

                    success = true;
                    SetEnableContactList(true);
                    SetReadOnlyContactInFor(true);
                    //ContactPointInformation = resultData;
                    FillDataToGridControl(ContactPointInformation.ID);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// truy vấn d/s tiếp xúc
        /// </summary>
        private void FillDataToGridControl(long? dataID)
        {
            try
            {
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                if (dataID != null)
                {
                    this.lstContactPointADO = new List<ContactPointADO>();
                    HisContactFilter filter = new HisContactFilter();

                    filter.CONTACT_POINT1_ID__OR__CONTACT_POINT2_ID = dataID;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";

                    var resultData = new BackendAdapter(param).Get<List<HIS_CONTACT>>("/api/hisContact/Get", ApiConsumers.MosConsumer, filter, param);

                    if (resultData != null && resultData.Count > 0)
                    {
                        foreach (var item in resultData)
                        {
                            List<long> dataIDs = new List<long>();

                            if (item.CONTACT_POINT1_ID != dataID)
                            {
                                dataIDs.Add(item.CONTACT_POINT1_ID);
                            }

                            if (item.CONTACT_POINT2_ID != dataID)
                            {
                                dataIDs.Add(item.CONTACT_POINT2_ID);
                            }

                            List<V_HIS_CONTACT_POINT> Contacts = new List<V_HIS_CONTACT_POINT>();

                            Contacts = dataContactPoint(dataIDs);
                            if (Contacts != null && Contacts.Count > 0)
                            {
                                LoadHisPatient(Contacts.Select(s => s.PATIENT_ID ?? 0).ToList());
                                foreach (var itemC in Contacts)
                                {
                                    ContactPointADO ado = new ContactPointADO(itemC);
                                    ado.CONTACT_TIME = item.CONTACT_TIME;
                                    ado.CONTACT_ID = item.ID;
                                    if (itemC.PATIENT_ID != null)
                                    {
                                        var patient = lstHisPatient.FirstOrDefault(o => o.ID == itemC.PATIENT_ID);
                                        ado.CODE_STR = patient != null ? patient.PATIENT_CODE : "";
                                        ado.FULL_NAME = patient != null ? patient.VIR_PATIENT_NAME : "";
                                        ado.DEPARTMENT_NAME = patient != null ? patient.DEPARTMENT_NAME : "";
                                        //ado.DEPARTMENT_ID = patient != null ? patient.LAST_DEPARTMENT_ID : null;
                                    }
                                    if (itemC.EMPLOYEE_ID != null)
                                    {
                                        var Employee = lstHisEmployeeAdo.FirstOrDefault(o => o.ID == itemC.EMPLOYEE_ID);
                                        ado.FULL_NAME = Employee != null ? Employee.USERNAME : "";
                                        ado.CODE_STR = Employee != null ? Employee.LOGINNAME : "";
                                        ado.DEPARTMENT_NAME = Employee != null ? Employee.DEPARTMENT_NAME : "";
                                    }

                                    this.lstContactPointADO.Add(ado);
                                }
                            }
                        }
                    }
                }
                WaitingManager.Hide();
                gridControl1.BeginUpdate();
                gridControl1.DataSource = lstContactPointADO;
                gridControl1.EndUpdate();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_CONTACT_POINT> dataContactPoint(List<long> LstId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                List<V_HIS_CONTACT_POINT> ContactPoint = new List<V_HIS_CONTACT_POINT>();

                HisContactPointViewFilter filter = new HisContactPointViewFilter();

                filter.IDs = LstId;
                ContactPoint = new BackendAdapter(paramCommon).Get<List<V_HIS_CONTACT_POINT>>("/api/HisContactPoint/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                return ContactPoint;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_CONTACT_POINT updateDTO)
        {
            try
            {
                updateDTO.CONTACT_LEVEL = (long)SpinClassify.Value;

                if (chkAmTinh1.Checked)
                {
                    updateDTO.TEST_RESULT_1 = 0;
                }
                else if (chkDuongTinh1.Checked)
                {
                    updateDTO.TEST_RESULT_1 = 1;
                }
                else
                {
                    updateDTO.TEST_RESULT_1 = null;
                }

                if (chkAmTinh2.Checked)
                {
                    updateDTO.TEST_RESULT_2 = 0;
                }
                else if (chkDuongTinh2.Checked)
                {
                    updateDTO.TEST_RESULT_2 = 1;
                }
                else
                {
                    updateDTO.TEST_RESULT_2 = null;
                }

                if (chkAmTinh3.Checked)
                {
                    updateDTO.TEST_RESULT_3 = 0;
                }
                else if (chkDuongTinh3.Checked)
                {
                    updateDTO.TEST_RESULT_3 = 1;
                }
                else
                {
                    updateDTO.TEST_RESULT_3 = null;
                }

                if (chkStaff.Checked)
                {
                    updateDTO.CONTACT_TYPE = 2;
                    var EmployeeADO = UcStaffInformation.GetvalueHisEmployeeADO();
                    updateDTO.EMPLOYEE_ID = EmployeeADO != null ? EmployeeADO.ID : (long?)null;
                }

                if (chkPatient.Checked)
                {
                    updateDTO.CONTACT_TYPE = 1;

                    var patient = UcPatientInformation.GetValueHisPatient();

                    updateDTO.PATIENT_ID = patient != null ? patient.ID : (long?)null;
                }

                if (chkOrther.Checked)
                {
                    updateDTO.CONTACT_TYPE = 3;
                    updateDTO.CONTACT_POINT_OTHER_TYPE_NAME = txtOrther.Text.Trim();

                    int idx = this.UcOrtherInformation.GetValuePatientName().LastIndexOf(" ");

                    updateDTO.FIRST_NAME = (idx > -1 ? this.UcOrtherInformation.GetValuePatientName().Substring(idx).Trim() : this.UcOrtherInformation.GetValuePatientName());
                    updateDTO.LAST_NAME = (idx > -1 ? this.UcOrtherInformation.GetValuePatientName().Substring(0, idx).Trim() : "");

                    //updateDTO.VIR_FULL_NAME = this.UcOrtherInformation.GetValuePatientName();
                    updateDTO.GENDER_ID = this.UcOrtherInformation.GetValueGender();
                    updateDTO.DOB = this.UcOrtherInformation.GetValuePatientDob();
                    updateDTO.PHONE = this.UcOrtherInformation.GetValuePhone();
                    updateDTO.ADDRESS = this.UcOrtherInformation.GetValueAddress();

                    V_SDA_PROVINCE province = new V_SDA_PROVINCE();
                    V_SDA_DISTRICT district = new V_SDA_DISTRICT();
                    V_SDA_COMMUNE commune = new V_SDA_COMMUNE();

                    province = this.UcOrtherInformation.GetValueProvince();
                    district = this.UcOrtherInformation.GetValueDistrict();
                    commune = this.UcOrtherInformation.GetValueCommune();

                    if (province != null && province.ID > 0 )
                    {
                        updateDTO.PROVINCE_CODE = province.PROVINCE_CODE;
                        updateDTO.PROVINCE_NAME = province.PROVINCE_NAME;
                    }

                    if (district != null && district.ID > 0)
                    {
                        updateDTO.DISTRICT_CODE = district.DISTRICT_CODE;
                        updateDTO.DISTRICT_NAME = district.DISTRICT_NAME;
                    }

                    if (commune != null && commune.ID > 0)
                    {
                        updateDTO.COMMUNE_CODE = commune.COMMUNE_CODE;
                        updateDTO.COMMUNE_NAME = commune.COMMUNE_NAME;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_CONTACT_POINT Update)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisContactPointFilter filter = new HisContactPointFilter();
                filter.ID = currentId;
                Update = new BackendAdapter(param).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    Inventec.Common.Logging.LogSystem.Warn("btnAdd_Click: Validate");
                    return;
                }

                if (chkOrther1.Checked && !UcOrtherList.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcOrtherList: Validate");
                    return;
                }
                else if (chkPatient1.Checked && !UcPatientList.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcPatientList: Validate");
                    return;
                }
                else if (chkStaff1.Checked && !UcStaffList.ValidateForm())
                {
                    Inventec.Common.Logging.LogSystem.Warn("UcStaffInformation: Validate");
                    return;
                }

                WaitingManager.Show();

                HisContactSDO ContactSDO = new HisContactSDO();
                //Inventec.Common.Mapper.DataObjectMapper.Map<HisContactSDO>(ContactSDO, this.ContactPointList);

                ContactSDO.ID = this.ContactPointList.ID;
                if (chkStaff1.Checked)
                {
                    //this.ContactPointList = UcStaffList.GetCurrentContactPoint();
                    ContactSDO.CONTACT_TYPE = 2;
                    var EmployeeADO = UcStaffList.GetvalueHisEmployeeADO();
                    ContactSDO.EMPLOYEE_ID = EmployeeADO != null ? EmployeeADO.ID : (long?)null;
                }
                if (chkPatient1.Checked)
                {
                    //this.ContactPointList = UcPatientList.GetCurrentContactPoint();

                    ContactSDO.CONTACT_TYPE = 1;

                    var patient = UcPatientList.GetValueHisPatient();

                    ContactSDO.PATIENT_ID = patient != null ? patient.ID : (long?)null;
                }
                if (chkOrther1.Checked)
                {
                    //this.ContactPointList = UcOrtherList.GetCurrentContactPoint();
                    ContactSDO.CONTACT_TYPE = 3;
                    ContactSDO.CONTACT_POINT_OTHER_TYPE_NAME = txtOrther1.Text.Trim();

                    int idx = this.UcOrtherList.GetValuePatientName().LastIndexOf(" ");

                    ContactSDO.FIRST_NAME = (idx > -1 ? this.UcOrtherList.GetValuePatientName().Substring(idx).Trim() : this.UcOrtherList.GetValuePatientName());
                    ContactSDO.LAST_NAME = (idx > -1 ? this.UcOrtherList.GetValuePatientName().Substring(0, idx).Trim() : "");

                    ContactSDO.VIR_FULL_NAME = this.UcOrtherList.GetValuePatientName();
                    ContactSDO.GENDER_ID = this.UcOrtherList.GetValueGender();
                    ContactSDO.DOB = this.UcOrtherList.GetValuePatientDob();
                    ContactSDO.PHONE = this.UcOrtherList.GetValuePhone();
                    ContactSDO.ADDRESS = this.UcOrtherList.GetValueAddress();

                    V_SDA_PROVINCE province = new V_SDA_PROVINCE();
                    V_SDA_DISTRICT district = new V_SDA_DISTRICT();
                    V_SDA_COMMUNE commune = new V_SDA_COMMUNE();

                    province = this.UcOrtherList.GetValueProvince();
                    district = this.UcOrtherList.GetValueDistrict();
                    commune = this.UcOrtherList.GetValueCommune();

                    if (province != null && province.ID > 0)
                    {
                        ContactSDO.PROVINCE_CODE = province.PROVINCE_CODE;
                        ContactSDO.PROVINCE_NAME = province.PROVINCE_NAME;
                    }

                    if (district != null && district.ID > 0)
                    {
                        ContactSDO.DISTRICT_CODE = district.DISTRICT_CODE;
                        ContactSDO.DISTRICT_NAME = district.DISTRICT_NAME;
                    }

                    if (commune != null && commune.ID > 0)
                    {
                        ContactSDO.COMMUNE_CODE = commune.COMMUNE_CODE;
                        ContactSDO.COMMUNE_NAME = commune.COMMUNE_NAME;
                    }

                }

                ContactSDO.ContactTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtContactTime.DateTime) ?? 0;
                ContactSDO.ContactPointId = this.ContactPointInformation.ID;
                ContactSDO.CONTACT_LEVEL = (long)SpinClassify1.Value;

                Inventec.Common.Logging.LogSystem.Info("ContactSDO: "+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ContactSDO), ContactSDO));

                var ContactResult = new BackendAdapter(param).Post<HisContactResultSDO>("/api/HisContactPoint/AddContactInfo", ApiConsumers.MosConsumer, ContactSDO, param);

                if (ContactResult != null)
                {
                    success = true;
                    //SetDefaultWhenChangeChecklist();
                    this.ContactPointList = new V_HIS_CONTACT_POINT();
                    dtContactTime.EditValue = DateTime.Now;
                    SpinClassify1.EditValue = this.ContactPointInformation.CONTACT_LEVEL + 1;

                    this.UcStaffList.reset();
                    this.UcPatientList.reset();
                    this.UcOrtherList.reset();

                    FillDataToGridControl(this.ContactPointInformation.ID);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstContactPointADO != null && lstContactPointADO.Count > 0)
                {
                    this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                    this.ContactPointList = new V_HIS_CONTACT_POINT();
                    this.lstContactPointADO = new List<ContactPointADO>();

                    positionHandle = -1;
                    SetDefautValueControl();
                    SetReadOnlyContactInFor(false);

                    SetEnableContactList(false);

                    this.UcStaffInformation.reset();
                    this.UcStaffList.reset();
                    this.UcPatientInformation.reset();
                    this.UcPatientList.reset();
                    this.UcOrtherInformation.reset();
                    this.UcOrtherList.reset();

                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = lstContactPointADO;
                    gridControl1.EndUpdate();

                    SetValuePatientInformation(this.ContactPointInformation);

                    chkStaff1.Checked = false;
                    chkPatient1.Checked = false;
                    chkOrther1.Checked = false;
                    panelControl2.Controls.Clear();

                    dxValidationProvider1.SetValidationRule(SpinClassify1, null);
                    dxValidationProvider1.SetValidationRule(dtContactTime, null);

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
                else
                {
                    if (MessageBox.Show(ResourceMessage.BanCoMuonNhapNguoiBenhMoiKhong, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointList = new V_HIS_CONTACT_POINT();
                        this.lstContactPointADO = new List<ContactPointADO>();

                        positionHandle = -1;
                        SetDefautValueControl();
                        SetReadOnlyContactInFor(false);

                        SetEnableContactList(false);

                        this.UcStaffInformation.reset();
                        this.UcStaffList.reset();
                        this.UcPatientInformation.reset();
                        this.UcPatientList.reset();
                        this.UcOrtherInformation.reset();
                        this.UcOrtherList.reset();

                        gridControl1.BeginUpdate();
                        gridControl1.DataSource = lstContactPointADO;
                        gridControl1.EndUpdate();

                        SetValuePatientInformation(this.ContactPointInformation);

                        chkStaff1.Checked = false;
                        chkPatient1.Checked = false;
                        chkOrther1.Checked = false;
                        panelControl2.Controls.Clear();

                        dxValidationProvider1.SetValidationRule(SpinClassify1, null);
                        dxValidationProvider1.SetValidationRule(dtContactTime, null);

                        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    }
                    else
                    {
                        SetReadOnlyContactInFor(true);
                        chkStaff1.Checked = true;
                        chkStaff1.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ContactPointADO pData = (ContactPointADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "CONTACT_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)pData.CONTACT_TIME).Value.ToString("dd/MM/yyyy hh:mm");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    if (e.Column.FieldName == "CONTACT_LEVEL_STR")
                    {
                        try
                        {
                            if (pData.CONTACT_LEVEL != null)
                            {
                                e.Value = "F" + pData.CONTACT_LEVEL;
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    if (e.Column.FieldName == "FULL_NAME_STR")
                    {
                        try
                        {
                            string FullName1 = "";

                            if (!String.IsNullOrEmpty(pData.FULL_NAME))
                            {
                                FullName1 = pData.FULL_NAME;
                            }

                            e.Value = FullName1;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    if (e.Column.FieldName == "CONTACT_TYPE_STR")
                    {

                        try
                        {
                            //1: Benh nhan, 2: Bac sy, 3: Khac
                            if (pData.CONTACT_TYPE == 1)
                            {
                                e.Value = "Bệnh nhân";
                            }
                            else if (pData.CONTACT_TYPE == 2)
                            {
                                e.Value = "Nhân viên";
                            }
                            else
                            {
                                e.Value = "Khác";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnG_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (ContactPointADO)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisContact/Delete", ApiConsumers.MosConsumer, rowData.CONTACT_ID, param);
                        if (success)
                        {
                            this.lstContactPointADO = new List<ContactPointADO>();
                            FillDataToGridControl(this.ContactPointInformation.ID);

                        }
                        MessageManager.Show(this, param, success);
                    }
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
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = btnG_Delete;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkPatient1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPatient1.Checked && chkPatient1.Enabled == true)
                {
                    List<V_HIS_CONTACT_POINT> GetContactPoint = new List<V_HIS_CONTACT_POINT>();
                    List<long> lst = new List<long>() { this.ContactPointInformation.ID};
                    GetContactPoint = dataContactPoint(lst);

                    if (GetContactPoint != null && GetContactPoint.Count > 0)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointInformation = GetContactPoint.FirstOrDefault();
                    }

                    //SetDefaultWhenChangeChecklist();

                    InitPanelControl(this.panelControl2, UcPatientList);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkOrther1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //SetDefaultWhenChangeChecklist();

                if (chkOrther1.Checked && chkOrther1.Enabled == true)
                {
                    List<V_HIS_CONTACT_POINT> GetContactPoint = new List<V_HIS_CONTACT_POINT>();
                    List<long> lst = new List<long>() { this.ContactPointInformation.ID };
                    GetContactPoint = dataContactPoint(lst);

                    if (GetContactPoint != null && GetContactPoint.Count > 0)
                    {
                        this.ContactPointInformation = new V_HIS_CONTACT_POINT();
                        this.ContactPointInformation = GetContactPoint.FirstOrDefault();
                    }


                    InitPanelControl(this.panelControl2, UcOrtherList);
                    txtOrther1.Enabled = true;

                }
                else
                {
                    txtOrther1.Enabled = false;
                    dxValidationProvider1.SetValidationRule(txtOrther1, null);
                }

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

      
        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                    Inventec.Common.Logging.LogSystem.Warn("_ValidationFailed_1: " + edit.Name);
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                    Inventec.Common.Logging.LogSystem.Warn("_ValidationFailed: " + edit.Name);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

    }
}
