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
using System.Resources;
using System.Collections;
using System.IO;

using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

using HIS.Desktop.Utility;
using HIS.Desktop.Utilities;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;


using MOS.EFMODEL.DataModels;
using MOS.Filter;

using EMR.EFMODEL.DataModels;
using EMR.Filter;

using Inventec.Common.Logging;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Common.SignLibrary;

using iTextSharp.text.pdf;
using iTextSharp.text;

using HIS.Desktop.Plugins.PatientDocumentIssued.Resources;
using HIS.Desktop.Plugins.PatientDocumentIssued.ADO;
using HIS.Desktop.Plugins.PatientDocumentIssued.Form;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList;
using EMR.SDO;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.PatientDocumentIssued
{
    public partial class UCPatientDocumentIssued : HIS.Desktop.Utility.UserControlBase
    {
        //string loginName = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        //.Desktop.Common.DelegateSelectData delegateSelect;
        List<EMR_DOCUMENT_TYPE> emrDocumentTypeSelecteds;
        List<HIS_DEPARTMENT> departmentSelecteds;
        List<HIS_DEPARTMENT> listDepartment;
        List<EMR_DOCUMENT_TYPE> listEmrDocumentType;
        List<SignatureStatusADO> signatureStatusSelecteds;
        List<SignatureStatusADO> listSignatureStatus;
        // List emr_document for print
        List<V_EMR_DOCUMENT> listEmrDocument;

        //
        int numEmrDocumentSelecteds = 0;
        bool IsExpand;
        string outPdfFile;
        Dictionary<long, string> DicoutPdfFile;
        /// <summary>
        V_EMR_DOCUMENT VEmrDocumentRightMouseClick = new V_EMR_DOCUMENT();

        V_EMR_DOCUMENT TreeClickData;
        /// </summary>

        internal string typeCodeFind__KeyWork_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__InMonth = "Trong tháng";//Set lại giá trị trong resource
        internal string typeCodeFind__InYear = "Trong năm";//Set lại giá trị trong resource
        internal string typeCodeFind__InTime = "Khoảng ngày";//Set lại giá trị trong resource

        internal string typeCodeFind__KeyWork_OutDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_OutDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__OutMonth = "Trong tháng";//Set lại giá trị trong resource
        internal string typeCodeFind__OutYear = "Trong năm";//Set lại giá trị trong resource
        internal string typeCodeFind__OutTime = "Khoảng ngày";//Set lại giá trị trong resource

        public UCPatientDocumentIssued(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                //this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCPatientDocumentIssued_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();

            SetResourceByLanguageKey();

            GetDataCombo();

            InitTypeFind();
            InitComboOutHopital();

            InitCheck(cboEmrDocumentType, SelectionGrid__EmrDocumentType);
            InitCombo(cboEmrDocumentType, listEmrDocumentType, "DOCUMENT_TYPE_NAME", "ID");

            InitCheck(cboSignatureStatus, SelectionGrid__SingnatureStatus);
            InitCombo(cboSignatureStatus, listSignatureStatus, "StatusName", "ID");

            InitCheck(cboDepartmentFinish, SelectionGrid__DepartmentFinish);
            InitCombo(cboDepartmentFinish, listDepartment, "DEPARTMENT_NAME", "ID");
            SetDefaultControl();
            FillDataToGrid();

            WaitingManager.Hide();
        }

        private void FillDataToGrid()
        {
            try
            {
                List<EmrDocumentADO> EmrDocumentADOs = new List<EmrDocumentADO>();
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                SetEmrDocumentViewFilter(ref filter);
                var listEmrrDocument = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>(RequestUriStore.V_EMR_DOCUMENT_GET, ApiConsumers.EmrConsumer, filter, param);
                LogSystem.Debug(listEmrrDocument.Count.ToString());
                if (listEmrrDocument != null && listEmrrDocument.Count > 0)
                {
                    var listRootSety = listEmrrDocument.GroupBy(o => o.TREATMENT_CODE).ToList();
                    //var listDepartmentFind = listDepartment.GroupBy(o => o.DEPARTMENT_CODE).ToList();
                    foreach (var rootSety in listRootSety)
                    {
                        EmrDocumentADO ssRootSety = new EmrDocumentADO();

                        ssRootSety.CONCRETE_ID__IN_SETY = rootSety.First().TREATMENT_CODE + "_" + rootSety.First().PATIENT_CODE;
                        ssRootSety.PARENT_ID__IN_SETY = rootSety.First().TREATMENT_CODE;
                        ssRootSety.DOCUMENT_CODE = rootSety.First().TREATMENT_CODE + "-" + rootSety.First().PATIENT_CODE + "-" + rootSety.First().VIR_PATIENT_NAME + "-" + Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().DOB).Substring(Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().DOB).Length - 4) + "-" + rootSety.First().GENDER_NAME;
                        ssRootSety.IS_PARENT = true;
                        //ssRootSety.DOCUMENT_NAME = rootSety.First().DOCUMENT_NAME;
                        //ssRootSety.DOCUMENT_TYPE_ID = rootSety.First().DOCUMENT_TYPE_ID;

                        //ssRootSety.CREATE_TIME = rootSety.First().CREATE_TIME;
                        //ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
                        EmrDocumentADOs.Add(ssRootSety);
                        int d = 0;
                        foreach (var item in rootSety)
                        {
                            d++;
                            EmrDocumentADO ado = new EmrDocumentADO(item);
                            ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                            ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                            //if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
                            //{
                            //    ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);
                            //}
                            ////   else
                            ////   {
                            //ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
                            ////   }

                            EmrDocumentADOs.Add(ado);
                        }
                    }
                }

                EmrDocumentADOs = EmrDocumentADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ToList();
                treeEmrDocument.DataSource = new BindingList<EmrDocumentADO>(EmrDocumentADOs);
                treeEmrDocument.ExpandAll();
                Expand(true);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEmrDocumentViewFilter(ref EmrDocumentViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtPatientName.Text))
                {
                    filter.VIR_PATIENT_NAME = txtPatientName.Text.Trim();
                }

                if (this.signatureStatusSelecteds != null && this.signatureStatusSelecteds.Count > 0)
                {
                    if (this.signatureStatusSelecteds.Exists(o => o.ID == 1) && this.signatureStatusSelecteds.Exists(o => o.ID == 2))
                    {
                        filter.IS_DELETE = false;
                    }
                    else if (this.signatureStatusSelecteds.Exists(o => o.ID == 1))
                    {
                        filter.IS_NEXT_SIGNER_NOT_NULL = false;
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_DELETE = false;
                    }
                    else if (this.signatureStatusSelecteds.Exists(o => o.ID == 2))
                    {
                        filter.IS_NEXT_SIGNER_NOT_NULL = true;
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_DELETE = false;
                    }
                }
                if (rdoNotPrinted.Checked == true)
                {
                    filter.IS_PATIENT_ISSUED = false;
                }
                if (rdoPrinted.Checked == true)
                {
                    filter.IS_PATIENT_ISSUED = true;
                }
                if (this.emrDocumentTypeSelecteds != null && this.emrDocumentTypeSelecteds.Count > 0)
                {
                    filter.DOCUMENT_TYPE_IDs = emrDocumentTypeSelecteds.Select(o => o.ID).ToList();
                }
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count() > 0)
                {
                    filter.CURRENT_DEPARTMENT_CODEs = departmentSelecteds.Select(o => o.DEPARTMENT_CODE).ToList();
                }

                //if ((dtInHospital.EditValue == null || dtInHospital.EditValue.ToString() == "") && (dtOutHospital.EditValue == null || dtOutHospital.EditValue.ToString() == ""))
                //{
                //    WaitingManager.Hide();
                //    if (DevExpress.XtraEditors.XtraMessageBox.Show(this.navCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued_Message__TranhCaoTai",
                //   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture()), Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued_Message__Thongbao",
                //   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture()), System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                //        return;
                //}
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                    && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                    && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMM") + "00000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InYear
                    && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_YEAR__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyy") + "0000000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InTime
                    && dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue
                    && dtInDateCome.EditValue != null && dtInDateCome.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtInHospital.EditValue).ToString("yyyyMMdd") + "000000");
                    filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtInDateCome.EditValue).ToString("yyyyMMdd") + "000000");
                }

                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate
                        && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutMonth
                    && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                {
                    filter.OUT_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMM") + "00000000");
                }
                else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutYear
                    && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue)
                {
                    filter.OUT_YEAR__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyy") + "0000000000");
                }
                else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutTime
                    && dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue
                    && dtOutDateCome.EditValue != null && dtOutDateCome.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE__FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtOutHospital.EditValue).ToString("yyyyMMdd") + "000000");
                    filter.OUT_DATE__TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtOutDateCome.EditValue).ToString("yyyyMMdd") + "000000");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public void Expand(bool isExpand)
        {
            try
            {
                this.IsExpand = isExpand;
                if (isExpand)
                {
                    treeEmrDocument.ExpandAll();
                }
                else
                {
                    treeEmrDocument.BeginUpdate();
                    treeEmrDocument.CollapseAll();
                    treeEmrDocument.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {

                GetDataSignatureStatus();
                GetDataEmrDocumentType();
                GetDataDepartment();

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GetDataDepartment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count() > 0)
                    listDepartment = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataEmrDocumentType()
        {
            CommonParam commonParam = new CommonParam();
            EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
            filter.IS_ACTIVE = 1;
            filter.IS_ALLOW_PATIENT_ISSUE = true;
            listEmrDocumentType = new BackendAdapter(commonParam).Get<List<EMR_DOCUMENT_TYPE>>(RequestUriStore.EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, commonParam);
        }

        private void SetResourceByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResources = new ResourceManager("HIS.Desktop.Plugins.PatientDocumentIssued.Resources.Lang",
                    typeof(HIS.Desktop.Plugins.PatientDocumentIssued.UCPatientDocumentIssued).Assembly);

                this.navCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.navCreateTime.Caption",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.navOutTime.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.navOutTime.Caption",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.navDocumentType.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.navDocumentType.Caption",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.navSignatureStatus.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.navSignatureStatus.Caption",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.navPrintingStatus.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.navPrintingStatus.Caption",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.tc_DOCUMENT_CODE.Caption = Inventec.Common.Resource.Get.Value("LANGUAGE_KEY__UC_PATIENT_DOCUMENT_ISSUED__TLC__DOCUMENT_CODE",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.tc_DOCUMENT_NAME.Caption = Inventec.Common.Resource.Get.Value("LANGUAGE_KEY__UC_PATIENT_DOCUMENT_ISSUED__TLC__DOCUMENT_NAME",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.tc_DOCUMENT_TYPE_NAME.Caption = Inventec.Common.Resource.Get.Value("LANGUAGE_KEY__UC_PATIENT_DOCUMENT_ISSUED__TLC__DOCUMENT_TYPE_NAME",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.tc_CREATE_TIME.Caption = Inventec.Common.Resource.Get.Value("LANGUAGE_KEY__UC_PATIENT_DOCUMENT_ISSUED__TLC__CREATE_TIME",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.tc_REQUEST_LOGINNAME.Caption = Inventec.Common.Resource.Get.Value("LANGUAGE_KEY__UC_PATIENT_DOCUMENT_ISSUED__TLC__REQUEST_LOGINNAME",
                   Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());

                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.btnFind.Text",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnPrintAllCheck.Text = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.btnPrintAllCheck.Text",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.bbtnFind.Caption = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.bbtnFind.Csaption",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());

                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.txtPatientCode.Properties.NullValuePrompt",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.txtPatientName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.txtPatientName.Properties.NullValuePrompt",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.txtTreatmentCode.Properties.NullValuePrompt",
                  Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());


            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GetDataSignatureStatus()
        {
            listSignatureStatus = new List<SignatureStatusADO>();
            listSignatureStatus.Add(new SignatureStatusADO(1, "Hoàn thành"));
            listSignatureStatus.Add(new SignatureStatusADO(2, "Chưa hoàn thành"));
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(btnCodeFind_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(btnCodeFind_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                DXMenuItem itemInYear = new DXMenuItem(typeCodeFind__InYear, new EventHandler(btnCodeFind_Click));
                itemInYear.Tag = "InMonth";
                menu.Items.Add(itemInYear);

                DXMenuItem itemInTime = new DXMenuItem(typeCodeFind__InTime, new EventHandler(btnCodeFind_Click));
                itemInTime.Tag = "InTime";
                menu.Items.Add(itemInTime);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FormatDtIntructionDate()
        {
            try
            {
                layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcibtnPreviousInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcibtnNextInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtInHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtInHospital.Properties.EditMask = "MM/yyyy";
                    dtInHospital.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InYear)
                {
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "yyyy";
                    dtInHospital.Properties.EditMask = "yyyy";
                    dtInHospital.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lcibtnPreviousInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lcibtnNextInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtInHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtInHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtInHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtInDateCome.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtInDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInDateCome.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtInDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtInDateCome.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtInDateCome.Properties.EditMask = "dd/MM/yyyy";
                    dtInDateCome.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtInHospital.EditValue = DateTime.Now;
                    dtInDateCome.EditValue = DateTime.Now;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboOutHopital()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemOutDateCode = new DXMenuItem(typeCodeFind__KeyWork_OutDate, new EventHandler(cboOutOfHospital_Click));
                itemOutDateCode.Tag = "OutDate";
                menu.Items.Add(itemOutDateCode);

                DXMenuItem itemOutMonth = new DXMenuItem(typeCodeFind__OutMonth, new EventHandler(cboOutOfHospital_Click));
                itemOutMonth.Tag = "OutMonth";
                menu.Items.Add(itemOutMonth);
                DXMenuItem itemOutYear = new DXMenuItem(typeCodeFind__OutYear, new EventHandler(cboOutOfHospital_Click));
                itemOutYear.Tag = "OutYear";
                menu.Items.Add(itemOutYear);

                DXMenuItem itemOutTime = new DXMenuItem(typeCodeFind__OutTime, new EventHandler(cboOutOfHospital_Click));
                itemOutTime.Tag = "OutTime";
                menu.Items.Add(itemOutTime);


                cboOutOfHospital.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboOutOfHospital_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboOutOfHospital.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_OutDate = btnMenuCodeFind.Caption;

                FormatDtIntructionOutDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FormatDtIntructionOutDate()
        {
            try
            {
                layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lciPreviousOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lciNextOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtOutHospital.Properties.EditMask = "MM/yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__InYear)
                {
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "yyyy";
                    dtOutHospital.Properties.EditMask = "yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPreviousOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciNextOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtOutHospital.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtOutHospital.Properties.EditMask = "dd/MM/yyyy";
                    dtOutHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtOutDateCome.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtOutDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutDateCome.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtOutDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtOutDateCome.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtOutDateCome.Properties.EditMask = "dd/MM/yyyy";
                    dtOutDateCome.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtOutHospital.EditValue = DateTime.Now;
                    dtOutDateCome.EditValue = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                btnCodeFind.Text = typeCodeFind_InDate;
                cboOutOfHospital.Text = typeCodeFind_OutDate;
                this.typeCodeFind_InDate = "Trong ngày";
                this.typeCodeFind__KeyWork_InDate = this.typeCodeFind_InDate;
                FormatDtIntructionDate();
                FormatDtIntructionOutDate();
                dtInHospital.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtOutHospital.Text = null;

                layoutInDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutOutDateCome.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                emptySpaceInHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                emptySpaceOutHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;


                // Set defaul hoan thanh cho cboSignatureStatus
                GridCheckMarksSelection gridCheckMark = cboSignatureStatus.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboSignatureStatus.Properties.View);
                    cboSignatureStatus.EditValue = null;
                    cboSignatureStatus.Focus();
                    List<SignatureStatusADO> defaultStatus = listSignatureStatus.Where(o => o.ID == 1).ToList();
                    if (defaultStatus != null)
                    {
                        this.signatureStatusSelecteds.AddRange(defaultStatus);
                        gridCheckMark.SelectAll(this.signatureStatusSelecteds);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 300;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__EmrDocumentType(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                emrDocumentTypeSelecteds = new List<EMR_DOCUMENT_TYPE>();
                foreach (EMR_DOCUMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.DOCUMENT_TYPE_NAME);
                    emrDocumentTypeSelecteds.Add(rv);
                }
                cboEmrDocumentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__SingnatureStatus(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                signatureStatusSelecteds = new List<SignatureStatusADO>();
                foreach (SignatureStatusADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.StatusName);
                    signatureStatusSelecteds.Add(rv);
                }
                cboSignatureStatus.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__DepartmentFinish(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                departmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.DEPARTMENT_NAME);
                    departmentSelecteds.Add(rv);
                }

                Inventec.Common.Logging.LogSystem.Debug("departmentSelecteds: " + departmentSelecteds.Count());
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("departmentSelecteds___", departmentSelecteds));

                cboDepartmentFinish.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrDocumentType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string textDisplay = "";
                if (emrDocumentTypeSelecteds != null && emrDocumentTypeSelecteds.Count > 0 && this.emrDocumentTypeSelecteds.Count < this.listEmrDocumentType.Count)
                {
                    foreach (var item in emrDocumentTypeSelecteds)
                    {
                        textDisplay += item.DOCUMENT_TYPE_NAME;

                        if (!(item == emrDocumentTypeSelecteds.Last()))
                        {
                            textDisplay += ", ";
                        }
                    }
                }
                else if (this.emrDocumentTypeSelecteds.Count == this.listEmrDocumentType.Count)
                {
                    textDisplay = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.GridLookupEdit.Column.SelectionAll.Caption",
                    Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                }

                e.DisplayText = textDisplay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmrDocumentType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboEmrDocumentType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboEmrDocumentType.Properties.View);
                    }
                    cboEmrDocumentType.EditValue = null;
                    cboEmrDocumentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSignatureStatus_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string textDisplay = "";
                if (signatureStatusSelecteds != null && signatureStatusSelecteds.Count > 0 && this.signatureStatusSelecteds.Count < this.listSignatureStatus.Count)
                {
                    foreach (var item in signatureStatusSelecteds)
                    {
                        textDisplay += item.StatusName;

                        if (!(item == signatureStatusSelecteds.Last()))
                        {
                            textDisplay += ", ";
                        }
                    }
                }
                else if (this.signatureStatusSelecteds.Count == this.listSignatureStatus.Count)
                {
                    textDisplay = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.GridLookupEdit.Column.SelectionAll.Caption",
                    Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                }

                e.DisplayText = textDisplay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSignatureStatus_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboSignatureStatus.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboSignatureStatus.Properties.View);
                    }
                    cboSignatureStatus.EditValue = null;
                    cboSignatureStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (V_EMR_DOCUMENT)treeEmrDocument.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.HasChildren)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_PATIENT_ISSUED == 1)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        //e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var data = (EmrDocumentADO)treeEmrDocument.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "IMG")
                    {
                        try
                        {
                            if (!data.IS_PARENT)
                            {
                                if (String.IsNullOrEmpty(data.SIGNERS))
                                {
                                    e.Value = imageList1.Images[1]; //Chưa ký
                                }
                                else if (data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE)) //Đã ký
                                {
                                    e.Value = imageList1.Images[0];
                                }
                                else //Đang ký
                                {
                                    e.Value = imageList1.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai ky", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var data = (V_EMR_DOCUMENT)treeEmrDocument.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.HasChildren)
                    {
                        e.NodeImageIndex = 1;
                    }
                    else
                    {
                        e.NodeImageIndex = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_StateImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = (V_EMR_DOCUMENT)treeEmrDocument.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    WaitingManager.Show();
                    frmViewEmrDocument frmErrorForm = new frmViewEmrDocument(data, this.currentModule);
                    frmErrorForm.ShowDialog();
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPreDayInHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtInHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtInHospital.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                        dtInHospital.EditValue = currentdate.AddMonths(-1);
                    else
                        dtInHospital.EditValue = currentdate.AddYears(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextDayInHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtInHospital.EditValue != null && dtInHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtInHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtInHospital.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                        dtInHospital.EditValue = currentdate.AddMonths(1);
                    else
                        dtInHospital.EditValue = currentdate.AddYears(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreDayOutHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboOutOfHospital.Text))
                {
                    var currentdate = dtOutHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtOutHospital.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dtOutHospital.EditValue = currentdate.AddMonths(-1);
                    else
                        dtOutHospital.EditValue = currentdate.AddYears(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextDayOutHospital_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtOutHospital.EditValue != null && dtOutHospital.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboOutOfHospital.Text))
                {
                    var currentdate = dtOutHospital.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtOutHospital.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dtOutHospital.EditValue = currentdate.AddMonths(1);
                    else
                        dtOutHospital.EditValue = currentdate.AddYears(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintAllCheck_Click(object sender, EventArgs e)
        {
            try
            {
                listEmrDocument = new List<V_EMR_DOCUMENT>();
                var rowHandles = treeEmrDocument.GetAllCheckedNodes();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_EMR_DOCUMENT)treeEmrDocument.GetDataRecordByNode(i);
                        if (row != null)
                        {
                            if (row.ID != null && row.ID > 0)
                            {
                                listEmrDocument.Add(row);
                            }
                        }
                    }
                    LogSystem.Debug("11111:::::" + listEmrDocument.Where(o => o.ID != null && o.ID > 0).ToList().Count.ToString());
                    LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => listEmrDocument.Where(o => o.ID != null && o.ID > 0).ToList()), listEmrDocument.Where(o => o.ID != null && o.ID > 0).ToList()));
                    if (listEmrDocument != null && listEmrDocument.Count > 0)
                    {
                        loadDictionary(listEmrDocument);

                        CommonParam paramCommon = new CommonParam();
                        EmrDocumentDownloadFileSDO filter = new EmrDocumentDownloadFileSDO();
                        filter.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                        filter.EmrDocumentViewFilter.IDs = listEmrDocument.Select(o => o.ID).ToList();
                        filter.IsMerge = true;
                        filter.IsShowPatientSign = true;
                        filter.IsShowWatermark = false;
                        V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        if (room != null)
                        {
                            filter.RoomCode = room.ROOM_CODE;
                            filter.DepartmentCode = room.DEPARTMENT_CODE;
                        }
                        filter.IsView = null;

                        List<EmrDocumentFileSDO> apiResult = new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, filter, paramCommon);

                        if (apiResult != null && apiResult.Count > 0)
                        {
                            string output = Utils.GenerateTempFileWithin();
                            List<string> joinStreams = new List<string>();

                            List<MemoryStream> documentData = new List<MemoryStream>();
                            foreach (var item in apiResult)
                            {
                                if (item.Extension.ToLower().Equals("pdf"))
                                {
                                    string pdfAddFile = Utils.GenerateTempFileWithin();
                                    Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                    joinStreams.Add(pdfAddFile);
                                }
                            }

                            Stream currentStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                            var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                            var pages = new List<int>();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                            foreach (var file in joinStreams)
                            {
                                iTextSharp.text.pdf.PdfReader pdfReader = null;
                                pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                                pages = new List<int>();
                                for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                                {
                                    pages.Add(i);
                                }
                                pdfReader.SelectPages(pages);
                                pdfConcat.AddPages(pdfReader);
                                pdfReader.Close();
                            }
                            try
                            {
                                pdfConcat.Close();
                            }
                            catch { }

                            foreach (var file in joinStreams)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch { }
                            }

                            Inventec.Common.Logging.LogSystem.Warn("output: " + output);
                            Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output, UpdateListIsPatientIsssued);

                            DocumentView.Text = "In";
                            DocumentView.ShowDialog();

                        }
                        else
                        {
                            #region Hien thi message thong bao
                            MessageManager.Show(paramCommon, false);
                            #endregion
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn văn bản", "Thông báo");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateListIsPatientIsssued()
        {
            try
            {
                WaitingManager.Show();
                foreach (var item in listEmrDocument)
                {
                    CommonParam param = new CommonParam();
                    long idEmrDpcument = item.ID;
                    var apiResult = new BackendAdapter(param).Post<EMR_DOCUMENT>(RequestUriStore.EMR_DOCUMENT_ISSUED_UPDATE, ApiConsumers.EmrConsumer, idEmrDpcument, param);
                }
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void loadDictionary(List<V_EMR_DOCUMENT> listEmrDocument)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => outPdfFile), outPdfFile));
                DicoutPdfFile = new Dictionary<long, string>();
                foreach (var item in listEmrDocument)
                {
                    outPdfFile = "";
                    if (!String.IsNullOrEmpty(item.MERGE_CODE))
                    {
                        if (String.IsNullOrEmpty(outPdfFile))
                        {
                            string strDTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                            DocumentManager documentManager = new DocumentManager(strDTI);
                            var uc = documentManager.GetUcDocumentMerge((item.ORIGINAL_HIGH ?? 0), item.TREATMENT_CODE, item.MERGE_CODE, ref outPdfFile);
                        }

                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                    else
                    {
                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void InsertPage1(Stream sourceStream, List<EmrDocumentFileSDO> fileBase64Data, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();
            if (fileBase64Data != null && fileBase64Data.Count > 0)
            {
                iTextSharp.text.pdf.PdfReader reader1 = null;
                if (sourceStream != null)
                {
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceStream);
                }
                int pageCount = reader1.NumberOfPages;
                iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
                iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

                foreach (var item in fileBase64Data)
                {
                    if (item.Extension != "pdf")
                    {
                        var stream = new MemoryStream(Convert.FromBase64String(item.Base64Data));
                        stream.Position = 0;
                        string convertTpPdf = Utils.GenerateTempFileWithin();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu convertTpPdf: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => convertTpPdf), convertTpPdf));
                        Stream streamConvert = new FileStream(convertTpPdf, FileMode.Create, FileAccess.Write);
                        iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        iTextdocument.Open();
                        writer.Open();

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(stream);
                        if (img.Height > img.Width)
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Height / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Width / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        iTextdocument.Add(img);
                        iTextdocument.Close();
                        writer.Close();

                        joinStreams.Add(convertTpPdf);
                    }
                    else
                    {
                        var stream = new MemoryStream(Convert.FromBase64String(item.Base64Data));

                        if (stream != null && stream.Length > 0)
                        {
                            stream.Position = 0;
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(stream), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram____item=" + item);
                        }
                    }
                }

                Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                var pages = new List<int>();
                for (int i = 0; i <= reader1.NumberOfPages; i++)
                {
                    pages.Add(i);
                }
                reader1.SelectPages(pages);
                pdfConcat.AddPages(reader1);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                foreach (var file in joinStreams)
                {
                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    pages = new List<int>();
                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                    {
                        pages.Add(i);
                    }
                    pdfReader.SelectPages(pages);
                    pdfConcat.AddPages(pdfReader);
                    pdfReader.Close();
                }

                try
                {
                    reader1.Close();
                }
                catch { }

                try
                {
                    if (sourceStream != null)
                        sourceStream.Close();
                }
                catch { }

                try
                {
                    pdfConcat.Close();
                }
                catch { }

                foreach (var file in joinStreams)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        internal static void InsertPageOne(Stream sourceFile, string streamSourceStr, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();
            iTextSharp.text.pdf.PdfReader reader1 = null;
            if (sourceFile != null)
            {
                reader1 = new PdfReader(sourceFile);
            }
            if (!string.IsNullOrEmpty(streamSourceStr))
            {
                reader1 = new PdfReader(streamSourceStr);
            }

            int pageCount = reader1.NumberOfPages;
            iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
            iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

            Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

            var pages = new List<int>();
            for (int i = 0; i <= reader1.NumberOfPages; i++)
            {
                pages.Add(i);
            }
            reader1.SelectPages(pages);
            pdfConcat.AddPages(reader1);

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

            foreach (var file in joinStreams)
            {
                iTextSharp.text.pdf.PdfReader pdfReader = null;
                pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                pages = new List<int>();
                for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                {
                    pages.Add(i);
                }
                pdfReader.SelectPages(pages);
                pdfConcat.AddPages(pdfReader);
                pdfReader.Close();
            }

            try
            {
                reader1.Close();
            }
            catch { }

            try
            {
                pdfConcat.Close();
            }
            catch { }

            foreach (var file in joinStreams)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        private void treeEmrDocument_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                var data = (V_EMR_DOCUMENT)treeEmrDocument.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.HasChildren)
                    {
                        e.NodeImageIndex = 1;
                    }
                    else
                    {
                        e.NodeImageIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);

                if (e.State == CheckState.Checked)
                {
                    numEmrDocumentSelecteds += (sender as DevExpress.XtraTreeList.TreeList).Selection.Count;
                }
                else
                {
                    numEmrDocumentSelecteds -= (sender as DevExpress.XtraTreeList.TreeList).Selection.Count;
                }
                if (numEmrDocumentSelecteds > 0)
                {
                    btnPrintAllCheck.Enabled = true;
                }
                else
                {
                    btnPrintAllCheck.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void rdoPrinted_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoPrinted.Checked)
                {
                    rdoNotPrinted.Checked = false;
                }
                else
                {
                    rdoNotPrinted.Checked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void rdoNotPrinted_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoNotPrinted.Checked)
                {
                    rdoPrinted.Checked = false;
                }
                else
                {
                    rdoPrinted.Checked = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboDepartmentFinish_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string textDisplay = "";
                //Inventec.Common.Logging.LogSystem.Info("textDisplay___" + textDisplay);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("departmentSelecteds___", departmentSelecteds));
                if (departmentSelecteds != null && departmentSelecteds.Count > 0 && this.departmentSelecteds.Count < this.listDepartment.Count)
                {
                    foreach (var item in departmentSelecteds)
                    {
                        textDisplay += item.DEPARTMENT_NAME;

                        if (!(item == departmentSelecteds.Last()))
                        {
                            textDisplay += ", ";
                        }
                    }
                }
                else if (this.departmentSelecteds.Count == this.listDepartment.Count)
                {
                    textDisplay = Inventec.Common.Resource.Get.Value("UCPatientDocumentIssued.GridLookupEdit.Column.SelectionAll.Caption",
                    Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                }

                Inventec.Common.Logging.LogSystem.Info("textDisplay___" + textDisplay);
                e.DisplayText = textDisplay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartmentFinish_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboDepartmentFinish.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDepartmentFinish.Properties.View);
                    }
                    cboDepartmentFinish.EditValue = null;
                    cboDepartmentFinish.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeEmrDocument_CustomDrawColumnHeader(object sender, DevExpress.XtraTreeList.CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                TreeListColumn nextColumn = treeListColumn1;
                if (nextColumn == null) return;
                if (e.Column == nextColumn) { e.Handled = true; return; }
                if (e.Column != treeEmpmtyColumn) return;
                System.Drawing.Rectangle r = e.ObjectArgs.Bounds;
                r.Width = r.Width + nextColumn.VisibleWidth;
                e.ObjectArgs.Bounds = r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.Utils.ToolTipControlInfo info = null;
                TreeListHitInfo hi = treeEmrDocument.CalcHitInfo(e.ControlMousePosition);
                var o = hi.Node;
                var data = (EmrDocumentADO)treeEmrDocument.GetDataRecordByNode(o);
                if (data != null && hi.Column.FieldName == "IMG")
                {
                    string text = "";
                    if (!data.IS_PARENT)
                    {
                        if (String.IsNullOrEmpty(data.SIGNERS))
                        {
                            text = "Văn bản chưa được ký";
                        }
                        else if (data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE)) //Đã ký
                        {
                            text = "Bệnh nhân/người nhà đã ký";
                        }
                        else //Đang ký
                        {
                            text = "Văn bản đang được ký và Bệnh nhân/người nhà chưa ký";
                        }
                    }
                    info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                    e.Info = info;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
