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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using System.Configuration;
using System.Globalization;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using Inventec.Core;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using LIS.Desktop.Plugins.LisSample.ADO;
using DevExpress.XtraGrid.Views.Base;
using AutoMapper;
using DevExpress.Data;
using System.Collections;
using HIS.Desktop.Utility;
using LIS.Desktop.Plugins;

namespace LIS.Desktop.Plugins.LisSample.LisSample
{
    public partial class frmLisSample : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int positionHandleControlLisSampleInfo = -1;

        long? LisSampleID = null;

        V_HIS_ROOM room = null;
        LIS_SAMPLE resultData;
        private List<HisServiceADO> listServiceAdo;
        private List<HisServiceADO> listServiceChoices = new List<HisServiceADO>();

        LisSampleResearchSDO sample = null;
        private int theRequiredWidth = 600, theRequiredHeight = 100;
        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        bool isShow = true;
        bool isDobTextEditKeyEnter = false;
        internal bool isNotPatientDayDob = false;
        #endregion

        Inventec.Desktop.Common.Modules.Module moduleData;
        public frmLisSample(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            Config.LisConfigCFG.LoadConfig();
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);

        }
        private void frmLisSample_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show(this);
                LoadCombo();
                LoadService();
                this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);

                Validates();
                //SetControlvalue();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadService()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.RebuildAssignServiceContainer(); }));
                }
                else
                {
                    this.RebuildAssignServiceContainer();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlvalue()
        {
            try
            {
                txtTenbenhnhan.Text = this.resultData.LAST_NAME;
                cboGioitinh.EditValue = this.resultData.GENDER_CODE;
                txtNgaysinh.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.resultData.DOB ?? 0);
                txtDiachi.Text = this.resultData.ADDRESS;
                txtBarcode.Text = this.resultData.BARCODE;
                cboLoaimau.EditValue = this.resultData.SAMPLE_TYPE_ID;
                btnPBarcode.Enabled = !String.IsNullOrWhiteSpace(this.resultData.BARCODE);
                //cboDichVu.EditValue = this.resultData;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessSave()
        {
            Inventec.Core.CommonParam param = new CommonParam();


            try
            {
                bool sucsses = false;
                positionHandleControlLisSampleInfo = -1;
                if (!btnSaves.Enabled) return;
                if (!dxValidationProvider1.Validate()) return;

                if (listServiceChoices == null || listServiceChoices.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn dịch vụ xét nghiệm.", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                
                LisSampleResearchSDO sdo = new LisSampleResearchSDO();
                int idx = txtTenbenhnhan.Text.Trim().LastIndexOf(" ");
                sdo.FirstName = (idx > -1 ? txtTenbenhnhan.Text.Trim().Substring(idx).Trim() : txtTenbenhnhan.Text.Trim());
                sdo.LastName = (idx > -1 ? txtTenbenhnhan.Text.Trim().Substring(0, idx).Trim() : " ");

                if (cboGioitinh.EditValue != null)
                {
                    sdo.GenderCode = cboGioitinh.EditValue.ToString();
                }

                if (cboLoaimau.EditValue != null)
                {
                    sdo.SampleTypeId = System.Convert.ToInt64(cboLoaimau.EditValue);

                }

              //  LIS_SAMPLE currentDob=new LIS_SAMPLE();
                
                if (this.dtPatientDob.EditValue != null)
                    sdo.Dob = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtPatientDob.DateTime) ?? 0;
                else
                {
                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtNgaysinh.Text);
                    if (dateValidObject != null && dateValidObject.HasNotDayDob)
                    {
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }
                if (this.isNotPatientDayDob)
                {
                    sdo.IsHasNotDayDob = 1;
                }
                else
                {
                    sdo.IsHasNotDayDob = null;
                }
                


                sdo.Address = txtDiachi.Text;
                sdo.Barcode = txtBarcode.Text;

                sdo.ServiceCodes = listServiceChoices.Select(s => s.SERVICE_CODE).ToList();

                sdo.WorkingRoomCode = this.room.ROOM_CODE;
                sdo.WorkingRoomName = this.room.ROOM_NAME;

                var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/CreateResearch", ApiConsumers.LisConsumer, sdo, param);
                if (rs != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("##########____________: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    btnPBarcode.Enabled = true;

                    sucsses = true;
                }

                WaitingManager.Hide();


                #region Hien thi message thong bao
                MessageManager.Show(this, param, sucsses);
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
        

        #region LoadCombo
        private void LoadCombo()
        {
            loadComboLoaimau();
            LoadComboGioitinh();
        }

        private void LoadComboGioitinh()
        {
            try
            {
                // Inventec.Core.CommonParam param = new CommonParam();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "GENDER_CODE", columnInfos, false, 150);
                ControlEditorLoader.Load(cboGioitinh, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), controlEditorADO);


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void loadComboLoaimau()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLoaimau, BackendDataWorker.Get<LIS_SAMPLE_TYPE>(), controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAssignServiceAdo()
        {
            try
            {
                if (this.listServiceAdo == null || listServiceAdo.Count <= 0)
                {
                    this.listServiceAdo = new List<HisServiceADO>();
                    List<V_HIS_SERVICE> datas = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        this.listServiceAdo = (from r in datas select new HisServiceADO(r)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task RebuildAssignServiceContainer()
        {
            try
            {
                this.LoadAssignServiceAdo();
                gridViewService.BeginUpdate();
                gridViewService.Columns.Clear();
                popupControlContainerService.Width = theRequiredWidth;
                popupControlContainerService.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "SERVICE_CODE";
                col1.Caption = "Mã";
                col1.Width = 80;
                col1.VisibleIndex = 1;
                gridViewService.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "SERVICE_NAME";
                col2.Caption = "Dịch vụ";
                col2.Width = 250;
                col2.VisibleIndex = 2;
                gridViewService.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "SERVICE_UNIT_NAME";
                col5.Caption = "Đơn vị tính";
                col5.Width = 60;
                col5.VisibleIndex = 5;
                gridViewService.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "SERVICE_CODE_FOR_SEARCH";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewService.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "SERVICE_NAME_FOR_SEARCH";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewService.Columns.Add(col14);

                gridViewService.GridControl.DataSource = this.listServiceAdo;
                gridViewService.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewService.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region --clickDELETEcombo
        private void cboGioitinh_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGioitinh.Properties.Buttons[1].Visible = true;
                    cboGioitinh.EditValue = null;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaimau_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaimau.Properties.Buttons[1].Visible = true;
                    cboLoaimau.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region --KeyDown
        private void txtTenbenhnhan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGioitinh.Focus();
                    cboGioitinh.ShowPopup();


                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void cboGioitinh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtNgaysinh.Focus();
                    //txtNgaysinh.ShowPopup();

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtNgaysinh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiachi.Focus();
                    txtDiachi.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiachi_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.KeyCode == Keys.Enter)
                {
                    txtBarcode.Focus();
                    txtBarcode.SelectAll();

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaimau_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtAsignService.Focus();
                    txtAsignService.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    cboLoaimau.Focus();
                    cboLoaimau.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region validates
        private void Validates()
        {
            Validate_loaimau();
            ValidateBar();
            validateDiachi();
            validateTenbenhnhan();

        }

        private void Validate_loaimau()
        {
            try
            {
                LIS.Desktop.Plugins.LisSample.Validates.Validatebatbuoc vadi = new Validates.Validatebatbuoc();
                vadi.txtControl = cboLoaimau;
                vadi.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboLoaimau, vadi);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateBar()
        {
            try
            {
                LIS.Desktop.Plugins.LisSample.Validates.Barcode valimax = new Validates.Barcode();
                valimax.textEdit = txtBarcode;
                valimax.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                valimax.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtBarcode, valimax);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void validateTenbenhnhan()
        {
            try
            {
                LIS.Desktop.Plugins.LisSample.Validates.maxLength max = new Validates.maxLength();
                max.Maxlangth = 50;
                max.ErrorType = ErrorType.Warning;
                max.txtControl = txtTenbenhnhan;
                dxValidationProvider1.SetValidationRule(txtTenbenhnhan, max);


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void validateDiachi()
        {
            try
            {
                LIS.Desktop.Plugins.LisSample.Validates.maxLength max = new Validates.maxLength();
                max.Maxlangth = 500;
                max.ErrorType = ErrorType.Warning;
                max.txtControl = txtDiachi;
                dxValidationProvider1.SetValidationRule(txtDiachi, max);


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        private void btnSaves_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSave();
            }
            catch (Exception ec)
            {
                Inventec.Common.Logging.LogSystem.Error(ec);
            }
        }

        private void btnPBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPBarcode.Enabled || this.resultData == null) return;
                this.onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region Print
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                string a = txtBarcode.Text.Trim();
                resultData = BackendDataWorker.Get<LIS_SAMPLE>().FirstOrDefault(o => o.BARCODE == a);

                LisSampleViewFilter samleFilter = new LisSampleViewFilter();
                samleFilter.ID = this.resultData.ID;
                List<V_LIS_SAMPLE> samples = new BackendAdapter(new CommonParam()).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, samleFilter, null);
                V_LIS_SAMPLE print = samples != null ? samples.FirstOrDefault() : null;
                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           print,
                           null
                           );
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void onClickBtnPrintBarCode()
        {
            try
            {
                if (Config.LisConfigCFG.PRINT_BARCODE_BY_BARTENDER == "1")
                {
                    this.PrintBarcodeByBartender();
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void PrintBarcodeByBartender()
        {
            try
            {
                if (HIS.Desktop.Utility.StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {
                    Bartender.PrintClient.ClientPrintADO ado = new Bartender.PrintClient.ClientPrintADO();
                    ado.Barcode = this.resultData.BARCODE;
                    if (this.resultData.DOB.HasValue)
                    {
                        ado.DobYear = this.resultData.DOB.Value.ToString().Substring(0, 4);
                        ado.DobAge = MPS.AgeUtil.CalculateFullAge(this.resultData.DOB.Value);
                    }
                    ado.ExecuteRoomCode = this.resultData.EXECUTE_ROOM_CODE;
                    ado.ExecuteRoomName = this.resultData.EXECUTE_ROOM_NAME ?? "";
                    ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(this.resultData.EXECUTE_ROOM_NAME ?? "");
                    ado.GenderName = (!String.IsNullOrWhiteSpace(this.resultData.GENDER_CODE)) ? (this.resultData.GENDER_CODE == "01" ? "Nữ" : "Nam") : "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.PatientCode = this.resultData.PATIENT_CODE ?? "";
                    List<string> name = new List<string>();
                    if (!String.IsNullOrWhiteSpace(this.resultData.LAST_NAME))
                    {
                        name.Add(this.resultData.LAST_NAME);
                    }
                    if (!String.IsNullOrWhiteSpace(this.resultData.FIRST_NAME))
                    {
                        name.Add(this.resultData.FIRST_NAME);
                    }
                    ado.PatientName = String.Join(" ", name);
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.RequestDepartmentCode = this.resultData.REQUEST_DEPARTMENT_CODE ?? "";
                    ado.RequestDepartmentName = this.resultData.REQUEST_DEPARTMENT_NAME ?? "";
                    ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    ado.ServiceReqCode = this.resultData.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = this.resultData.TREATMENT_CODE;
                    Bartender.PrintClient.BartenderPrintClientManager client = new Bartender.PrintClient.BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        Inventec.Common.Logging.LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSave();
                System.Threading.Thread.Sleep(1000);
                this.onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGioitinh_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtNgaysinh.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region Ctrl+key
        private void bbtnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaves_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnLuuin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnbarcode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPBarcode_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboGioitinh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGioitinh.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtNgaysinh_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtNgaysinh.Properties.Buttons[1].Visible = true;
                    txtNgaysinh.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAsignService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        Rectangle buttonBounds = new Rectangle(txtAsignService.Bounds.X + this.DesktopLocation.X, txtAsignService.Bounds.Y + this.DesktopLocation.Y, txtAsignService.Bounds.Width, txtAsignService.Bounds.Height);
                        popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAsignService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var selectADO = (HisServiceADO)this.gridViewService.GetFocusedRow();
                    if (selectADO != null && !isShow)
                    {
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        popupControlContainerService.HidePopup();
                        AssignService_RowClick(selectADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewService.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtAsignService.Bounds.X + this.DesktopLocation.X, txtAsignService.Bounds.Y + this.DesktopLocation.Y, txtAsignService.Bounds.Width, txtAsignService.Bounds.Height);
                    popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));

                    gridViewService.Focus();
                    gridViewService.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtAsignService.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAsignService_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtAsignService.Text))
                {
                    txtAsignService.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewService.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        //Filter data
                        gridViewService.ActiveFilterString = String.Format("[SERVICE_CODE] Like '%{0}%' OR [SERVICE_CODE_FOR_SEARCH] Like '%{0}%' OR [SERVICE_NAME] Like '%{0}%' OR [SERVICE_NAME_FOR_SEARCH] Like '%{0}%'", txtAsignService.Text);

                        gridViewService.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewService.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewService.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewService.FocusedRowHandle = 0;
                        gridViewService.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewService.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtAsignService.Bounds.X + this.DesktopLocation.X, txtAsignService.Bounds.Y + this.DesktopLocation.Y, txtAsignService.Bounds.Width, txtAsignService.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerService.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                            isShow = false;
                        }

                        txtAsignService.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewService.ActiveFilter.Clear();
                    if (!isShowContainer)
                    {
                        popupControlContainerService.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HisServiceADO selectedADO = (HisServiceADO)this.gridViewService.GetFocusedRow();
                    if (selectedADO != null)
                    {
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        popupControlContainerService.HidePopup();
                        AssignService_RowClick(selectedADO);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewService.Focus();
                    this.gridViewService.FocusedRowHandle = this.gridViewService.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewService_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                HisServiceADO selectedADO = (HisServiceADO)this.gridViewService.GetFocusedRow();
                if (selectedADO != null)
                {
                    popupControlContainerService.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    AssignService_RowClick(selectedADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AssignService_RowClick(HisServiceADO selectADO)
        {
            try
            {
                HisServiceADO exist = listServiceChoices != null ? listServiceChoices.FirstOrDefault(o => o.ID == selectADO.ID) : null;
                if (exist == null)
                {
                    Mapper.CreateMap<HisServiceADO, HisServiceADO>();
                    exist = Mapper.Map<HisServiceADO>(selectADO);
                    exist.AMOUNT = 1;
                    if (listServiceChoices == null) listServiceChoices = new List<HisServiceADO>();
                    listServiceChoices.Add(exist);
                }
                gridControlAsignService.BeginUpdate();
                gridControlAsignService.DataSource = listServiceChoices;
                gridControlAsignService.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerService_CloseUp(object sender, EventArgs e)
        {
            try
            {
                this.isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerService_Popup(object sender, EventArgs e)
        {
            try
            {
                this.isShow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAsignService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisServiceADO data = (HisServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisServiceADO row = (HisServiceADO)gridViewAsignService.GetFocusedRow();
                if (row != null)
                {
                    listServiceChoices.Remove(row);
                    gridControlAsignService.BeginUpdate();
                    gridControlAsignService.DataSource = listServiceChoices;
                    gridControlAsignService.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonIFocusAssignService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtAsignService.Focus();
                txtAsignService.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtNgaysinh.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtPatientDob.EditValue = dt;
                        this.dtPatientDob.Update();
                    }
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.ShowPopup();
                    this.dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtNgaysinh.Text)) return;

                string dob = "";
                if (this.txtNgaysinh.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtNgaysinh.Text);

                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtNgaysinh.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_KeyDown_1(object sender, KeyEventArgs e)
        {
           
        }

        private void txtNgaysinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.isDobTextEditKeyEnter = true;

                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtNgaysinh.Text);
                    if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        //this.txtAge.Text = this.txtPatientDob.Text;
                        //this.cboAge.EditValue = 1;
                        this.txtNgaysinh.Text = dateValidObject.Age.ToString();
                    }
                    else if (String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        if (!dateValidObject.HasNotDayDob)
                        {
                            this.txtNgaysinh.Text = dateValidObject.OutDate;
                            this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                            this.dtPatientDob.Update();
                        }
                    }


                    this.txtDiachi.Focus();


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtNgaysinh.Text.Trim()))
                    return;
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtNgaysinh.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    //this.txtAge.Text = this.txtPatientDob.Text;
                    //this.cboAge.EditValue = 1;
                    this.txtNgaysinh.Text = dateValidObject.Age.ToString();
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtNgaysinh.Text = dateValidObject.OutDate;
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtNgaysinh.EditValue ?? "").ToString() != (this.txtNgaysinh.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.dxValidationProvider1.RemoveControlError(this.txtNgaysinh);
                    this.txtNgaysinh.ErrorText = "";
                    this.CalulatePatientAge(dateValidObject.OutDate);
                    //this.SetValueCareerComboByCondition();
                    this.SearchPatientByFilterCombo();
                }
                //if (this.isDobTextEditKeyEnter && this.txtAge.Enabled)
                //{
                //    this.txtAge.Focus();
                //    this.txtAge.SelectAll();
                //    this.ValidateTextAge();
                //}
                //else
                //{
                //    this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                //}
                this.isDobTextEditKeyEnter = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CalulatePatientAge(string strDob)
        {
            try
            {
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {

                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        //this.txtAge.EditValue = "";
                        //this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;
 

                    if (nam >= 7)
                    {

                    }
                
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SearchPatientByFilterCombo()
        {
            try
            {
                string strDob = "";
                if (this.txtNgaysinh.Text.Length == 4)
                    strDob = "01/01/" + this.txtNgaysinh.Text;
                else if (this.txtNgaysinh.Text.Length == 8)
                {
                    strDob = this.txtNgaysinh.Text.Substring(0, 2) + "/" + this.txtNgaysinh.Text.Substring(2, 2) + "/" + this.txtNgaysinh.Text.Substring(4, 4);
                }
                else
                    strDob = this.txtNgaysinh.Text;
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                this.dtPatientDob.Update();

                if ((this.dtPatientDob.EditValue == null
                    || this.dtPatientDob.DateTime == DateTime.MinValue)

                    || String.IsNullOrEmpty(this.txtNgaysinh.Text.Trim()))
                {
                    return;
                }

                string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
                string timeDob = "00";
           
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtNgaysinh.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.Update();
                    this.txtNgaysinh.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    this.CalulatePatientAge(this.txtNgaysinh.Text);
              
                    this.SearchPatientByFilterCombo();

                    System.Threading.Thread.Sleep(100);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    this.txtNgaysinh.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    string strDob = this.txtNgaysinh.Text;
                    this.CalulatePatientAge(strDob);
                 
                    this.SearchPatientByFilterCombo();
               
                      if (dtPatientDob.Text.Length == 4 && Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                    {
                        isNotPatientDayDob = true;

                    }
                    else if (dtPatientDob.Text.Length == 8 || dtPatientDob.Text.Length == 10)
                    {
                        if (dtPatientDob.DateTime <= DateTime.Now.Date)
                            isNotPatientDayDob = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaysinh_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

      
        

      
    }
}
