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
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Controls.PopupLoader;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;

namespace HIS.Desktop.Plugins.TranPatiDocumentInfo
{
    public partial class UCTranPatiDocumentInfo : UserControlBase
    {
        #region
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int positionHandle = -1;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<HIS_TREATMENT> ListHisTreatment = new List<HIS_TREATMENT>();
        HIS_TREATMENT HisTreatment = new HIS_TREATMENT();


        #endregion

        public UCTranPatiDocumentInfo(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void UCTranPatiDocumentInfo_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                InitComboUse();
                SetDefaultControl();
                ValidateForm();
                FillDataToGrid();


                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControl1);
               
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>> apiResult = null;
                HisTreatmentFilter filter = new HisTreatmentFilter();
                SetFilterNavBar(ref filter);
                this.gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_TREATMENT>)apiResult.Data;
                    if (data != null)
                    {
                        this.gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                this.gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex); 
            }
        }

        private void SetFilterNavBar(ref HisTreatmentFilter filter)
        {
            try
            {
                filter.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN;

                if (dtOutDateFrom.EditValue != null && dtOutDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }

                if (dtOutDateTo.EditValue != null && dtOutDateTo.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }

                if (chkChuaNhap.Checked)
                {
                    filter.HAS_TRAN_PATI_BOOK_NUMBER = false;
                }
                else if (chkDaNhap.Checked)
                {
                    filter.HAS_TRAN_PATI_BOOK_NUMBER = true;
                }
                else if (chkTatCa.Checked)
                {
                    filter.HAS_TRAN_PATI_BOOK_NUMBER = null;
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
                ValidationSingleControl(dtVaoSo,dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboTenBacSy, txtMaBacSy, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboTenLDKhoa, txtMaLDKhoa, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboTenLDBV, txtMaLDBV, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void SetDefaultControl()
        {
            try
            {
                this.dtChuyenVien.Enabled = false;
                this.txtChanDoan.Enabled = false;
                this.txtBVChuyenDi.Enabled = false;
                this.txtSoLuuTru.Enabled = false;
                this.chkChuaNhap.Checked = true;

                this.chkChuaNhap.CheckState = CheckState.Checked;
                this.dtOutDateFrom.EditValue = DateTime.Now;
                this.dtOutDateTo.EditValue = DateTime.Now;

                this.lblPatientCode.Text = "";
                this.lblPatientName.Text = "";
                this.lblGender.Text = "";
                this.lblDOB.Text = "";
                this.lblAdress.Text = "";
                this.lblHeinCardNumber.Text = "";

                this.dtChuyenVien.EditValue = null;
                this.txtChanDoan.Text = "";
                this.txtBVChuyenDi.Text = "";
                this.dtVaoSo.EditValue = null;
                this.txtSoLuuTru.Text = "";
                this.txtMaBacSy.Text = "";
                this.cboTenBacSy.EditValue = null;
                this.txtMaLDKhoa.Text = "";
                this.cboTenLDKhoa.EditValue = null;
                this.txtMaLDBV.Text = "";
                this.cboTenLDBV.EditValue = null;
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboUse()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                if (data == null) return;

                data = data.Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTenBacSy, data, controlEditorADO);
                ControlEditorLoader.Load(cboTenLDKhoa, data, controlEditorADO);
                ControlEditorLoader.Load(cboTenLDBV, data, controlEditorADO);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TranPatiDocumentInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.TranPatiDocumentInfo.UCTranPatiDocumentInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                this.chkChuaNhap.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.chkChuaNhap.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDaNhap.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.chkDaNhap.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTatCa.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.chkTatCa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("UCTranPatiDocumentInfo.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
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
                    HIS_TREATMENT dataRow = (HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "Statust") 
                    {
                        //Hiển thị màu xanh: TRAN_PATI_BOOK_NUMBER có giá trị
                        //Hiển thị màu trắng: TRAN_PATI_BOOK_NUMBER không có giá trị

                        if (dataRow.TRAN_PATI_BOOK_NUMBER != null)
                        {
                            e.Value = imageCollection1.Images[1];
                        }
                        else 
                        {
                            e.Value = imageCollection1.Images[0];
                        }
                    }

                    else if (e.Column.FieldName == "OUT_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.OUT_DATE ?? 0);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay chuyển OUT_DATE", ex);
                        }
                    }
                }
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
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_TREATMENT)this.gridView1.GetFocusedRow();

                if (rowData != null)
                {
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                    (dxValidationProvider1, dxErrorProvider1);

                    HisTreatment = new HIS_TREATMENT();
                    HisTreatment = rowData;
                    this.lblPatientCode.Text = rowData.TDL_PATIENT_CODE;
                    this.lblPatientName.Text = rowData.TDL_PATIENT_NAME;
                    this.lblGender.Text = rowData.TDL_PATIENT_GENDER_NAME;

                    if (rowData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        this.lblDOB.Text = rowData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        this.lblDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowData.TDL_PATIENT_DOB);
                    }

                    this.lblAdress.Text = rowData.TDL_PATIENT_ADDRESS;
                    this.lblHeinCardNumber.Text = HeinCardHelper.SetHeinCardNumberDisplayByNumber(rowData.TDL_HEIN_CARD_NUMBER);

                    this.dtChuyenVien.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowData.OUT_DATE ?? 0);
                    this.txtChanDoan.Text = rowData.ICD_CODE + " - " + rowData.ICD_NAME;
                    this.txtBVChuyenDi.Text = rowData.MEDI_ORG_NAME;

                    if (rowData.TRAN_PATI_BOOK_TIME != null)
                    {
                        this.dtVaoSo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowData.TRAN_PATI_BOOK_TIME ?? 0);
                    }
                    else
                    {
                        this.dtVaoSo.EditValue = DateTime.Now;
                    }

                    this.txtSoLuuTru.Text = rowData.TRAN_PATI_BOOK_NUMBER.ToString();

                    if (!String.IsNullOrEmpty(rowData.TRAN_PATI_DOCTOR_LOGINNAME))
                    {
                        this.txtMaBacSy.Text = rowData.TRAN_PATI_DOCTOR_LOGINNAME;
                        this.cboTenBacSy.EditValue = rowData.TRAN_PATI_DOCTOR_LOGINNAME;
                    }
                    else
                    {
                        this.txtMaBacSy.Text = rowData.DOCTOR_LOGINNAME;
                        this.cboTenBacSy.EditValue = rowData.DOCTOR_LOGINNAME;
                    }

                    if (!String.IsNullOrEmpty(rowData.TRAN_PATI_DEPARTMENT_LOGINNAME))
                    {
                        this.txtMaLDKhoa.Text = rowData.TRAN_PATI_DEPARTMENT_LOGINNAME;
                        this.cboTenLDKhoa.EditValue = rowData.TRAN_PATI_DEPARTMENT_LOGINNAME;
                    }

                    if (!String.IsNullOrEmpty(rowData.TRAN_PATI_HOSPITAL_LOGINNAME))
                    {
                        this.txtMaLDBV.Text = rowData.TRAN_PATI_HOSPITAL_LOGINNAME;
                        this.cboTenLDBV.EditValue = rowData.TRAN_PATI_HOSPITAL_LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusWhileSelectedUser(TextEdit txtEdit)
        {
            try
            {
                txtEdit.Focus();
                txtEdit.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Bác sỹ ký
        private void txtMaBacSy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboTenBacSy.EditValue = null;
                        this.FocusShowpopup(cboTenBacSy, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboTenBacSy.EditValue = searchResult[0].LOGINNAME;
                            this.txtMaBacSy.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser(this.txtMaLDKhoa);
                        }
                        else
                        {
                            this.cboTenBacSy.EditValue = null;
                            this.FocusShowpopup(cboTenBacSy, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenBacSy_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTenBacSy.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenBacSy.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtMaBacSy.Text = data.LOGINNAME;
                        }
                    }

                    this.FocusWhileSelectedUser(this.txtMaLDKhoa);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenBacSy_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTenBacSy.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenBacSy.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser(this.txtMaLDKhoa);
                            this.txtMaBacSy.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboTenBacSy.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Lãnh đạo khoa phòng ký
        private void txtMaLDKhoa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboTenLDKhoa.EditValue = null;
                        this.FocusShowpopup(cboTenLDKhoa, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboTenLDKhoa.EditValue = searchResult[0].LOGINNAME;
                            this.txtMaLDKhoa.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser(this.txtMaLDBV);
                        }
                        else
                        {
                            this.cboTenLDKhoa.EditValue = null;
                            this.FocusShowpopup(cboTenLDKhoa, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenLDKhoa_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTenLDKhoa.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenLDKhoa.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtMaLDKhoa.Text = data.LOGINNAME;
                        }
                    }

                    this.FocusWhileSelectedUser(this.txtMaLDBV);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenLDKhoa_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTenLDKhoa.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenLDKhoa.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser(this.txtMaLDBV);
                            this.txtMaLDKhoa.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboTenLDKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Lãnh đạo bệnh viện ký
        private void txtMaLDBV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboTenLDBV.EditValue = null;
                        this.FocusShowpopup(cboTenLDBV, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboTenLDBV.EditValue = searchResult[0].LOGINNAME;
                            this.txtMaLDBV.Text = searchResult[0].LOGINNAME;
                            
                        }
                        else
                        {
                            this.cboTenLDBV.EditValue = null;
                            this.FocusShowpopup(cboTenLDBV, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenLDBV_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTenLDBV.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenLDBV.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtMaLDBV.Text = data.LOGINNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTenLDBV_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTenLDBV.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboTenLDBV.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtMaLDBV.Text = data.LOGINNAME;
                            this.btnSave.Focus();
                            e.Handled = true;
                        }
                    }
                }
                else
                {
                    this.cboTenLDBV.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void cboTenBacSy_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtMaBacSy.Text = "";
                    cboTenBacSy.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void cboTenLDKhoa_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtMaLDKhoa.Text = "";
                    cboTenLDKhoa.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTenLDBV_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtMaLDBV.Text = "";
                    cboTenLDBV.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();

                var resultData = new BackendAdapter(param).Post<bool>("api/HisTreatment/ClearTranPatiBookInfo", ApiConsumers.MosConsumer, this.HisTreatment.ID, param);
                if (resultData)
                {
                    success = true;
                    FillDataToGrid();
                    this.txtSoLuuTru.Text = "";
                    this.dtVaoSo.EditValue = null;
                    this.txtMaBacSy.Text = "";
                    this.cboTenBacSy.EditValue = null;

                    this.txtMaLDKhoa.Text = "";
                    this.cboTenLDKhoa.EditValue = null;

                    this.txtMaLDBV.Text = "";
                    this.cboTenLDBV.EditValue = null;
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    MessageBox.Show("Không cho phép lưu do thiếu trường dữ liệu bắt buộc.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HisTreatmentSetTranPatiBookSDO TranPatiBookSDO = new HisTreatmentSetTranPatiBookSDO();

                TranPatiBookSDO.TranPatiBookTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtVaoSo.EditValue).ToString("yyyyMMdd") + "000000");
                TranPatiBookSDO.TranPatiDepartmentLoginname = txtMaLDKhoa.Text;
                TranPatiBookSDO.TranPatiDepartmentUsername = cboTenLDKhoa.Text;
                TranPatiBookSDO.TranPatiDoctorLoginname = txtMaBacSy.Text;
                TranPatiBookSDO.TranPatiDoctorUsername = cboTenBacSy.Text;
                TranPatiBookSDO.TranPatiHospitalLoginname = txtMaLDBV.Text;
                TranPatiBookSDO.TranPatiHospitalUsername = cboTenLDBV.Text;
                TranPatiBookSDO.TreatmentId = this.HisTreatment.ID;

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/SetTranPatiBookInfo", ApiConsumers.MosConsumer, TranPatiBookSDO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGrid();
                    txtSoLuuTru.Text = resultData.TRAN_PATI_BOOK_NUMBER.ToString();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
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

    }
}
