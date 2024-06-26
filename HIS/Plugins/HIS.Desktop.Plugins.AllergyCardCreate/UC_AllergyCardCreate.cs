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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using MPS.Processor.Mps000253.PDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.AllergyCardCreate
{
    public partial class UC_AllergyCardCreate : UserControl
    {
        long treatmentID;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TREATMENT currentTreatment;
        V_HIS_ALLERGY_CARD currentAllergyCard;
        string loginName;
        List<AllergenicADO> listAllergenic;
        int positionHandleAdd = -1;
        int positionHandleSave = -1;
        HisAllergyCardResultSDO resultAllergyCardSdo;
        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        V_HIS_MEDICINE_TYPE currentMedi { get; set; }
        bool isShow = true;
        int theRequiredWidth = 900, theRequiredHeight = 130;

        public UC_AllergyCardCreate()
        {
            InitializeComponent();
        }

        public UC_AllergyCardCreate(Inventec.Desktop.Common.Modules.Module module, long treatmentId)
        {
            InitializeComponent();
            try
            {
                this.treatmentID = treatmentId;
                this.currentModule = module;
                this.currentTreatment = GetTreatment(treatmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public UC_AllergyCardCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_ALLERGY_CARD data)
        {
            InitializeComponent();
            try
            {
                this.currentAllergyCard = data;
                this.currentModule = module;
                this.treatmentID = data.TREATMENT_ID;
                this.currentTreatment = GetTreatment(data.TREATMENT_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UC_AllergyCardCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                SetCaptionByLanguageKey();
                RebuildControlContainerThuoc();
                ValidateControlForm();
                LoadDefaultInfo();
                SetDefaultValue();
                LoadCombo();
                SetControl(true);
                LoadTreatmentInfoForm(this.currentTreatment);
                LoadAllergyCardInfoForm(this.currentAllergyCard);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AllergyCardCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.AllergyCardCreate.UC_AllergyCardCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnLamLai.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.btnLamLai.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChacChan.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.chkChacChan.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNghiNgo.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.chkNghiNgo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboThuoc.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.cboThuoc.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboThuoc.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.cboThuoc.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBacSi.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.cboBacSi.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UC_AllergyCardCreate.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_TREATMENT GetTreatment(long _treatmentID)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = _treatmentID;
                var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (treatment != null && treatment.Count > 0)
                {
                    result = treatment.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void LoadTreatmentInfoForm(V_HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    lblGioiTinh.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblHoTen.Text = data.TDL_PATIENT_NAME;
                    lblMaDieuTri.Text = data.TREATMENT_CODE;
                    lblNgaySinh.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    txtBacSi.Text = loginName;
                    cboBacSi.EditValue = loginName;
                    dtNgayCap.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadAllergyCardInfoForm(V_HIS_ALLERGY_CARD data)
        {
            try
            {
                if (data != null)
                {
                    dtNgayCap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.ISSUE_TIME);
                    txtBacSi.Text = data.DIAGNOSE_LOGINNAME;
                    cboBacSi.EditValue = data.DIAGNOSE_LOGINNAME;
                    txtSoDienThoai.Text = data.DIAGNOSE_PHONE;
                    LoadEditGrid(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadEditGrid(V_HIS_ALLERGY_CARD data)
        {
            try
            {
                listAllergenic = new List<AllergenicADO>();
                CommonParam param = new CommonParam();
                HisAllergenicFilter filter = new HisAllergenicFilter();
                filter.ALLERGY_CARD_ID = data.ID;

                var rsAllergenic = new BackendAdapter(param).Get<List<HIS_ALLERGENIC>>(HisRequestUriStore.HIS_ALLERGEIC_GET, ApiConsumers.MosConsumer, filter, param);

                if (rsAllergenic != null && rsAllergenic.Count > 0)
                {
                    foreach (var item in rsAllergenic)
                    {
                        var ado = new AllergenicADO(item);
                        listAllergenic.Add(ado);
                    }
                }
                SetDataSourceGrid(listAllergenic);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadCombo()
        {
            try
            {
                LoadComboBacSi();
                LoadComboThuoc();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboBacSi()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBacSi, BackendDataWorker.Get<ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboThuoc()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã loại thuốc", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên loại thuốc", 200, 2));
                columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "Quốc gia", 200, 2));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "Nhà sản xuất", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, true, 1000);
                controlEditorADO.PopupWidth = 1000;
                //ControlEditorLoader.Load(cboThuoc, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private HIS_ALLERGY_CARD UpdateAllergyCardByForm()
        {
            HIS_ALLERGY_CARD updateData = null;
            try
            {
                updateData = new HIS_ALLERGY_CARD();

                if (this.currentAllergyCard != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ALLERGY_CARD>(updateData, this.currentAllergyCard);
                }

                if (cboBacSi.EditValue != null)
                {
                    var bacSi = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboBacSi.EditValue.ToString());
                    if (bacSi != null)
                    {
                        updateData.DIAGNOSE_LOGINNAME = bacSi.LOGINNAME;
                        updateData.DIAGNOSE_USERNAME = bacSi.USERNAME;
                    }
                }

                updateData.DIAGNOSE_PHONE = txtSoDienThoai.Text;

                if (dtNgayCap.EditValue != null)
                {
                    updateData.ISSUE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayCap.DateTime) ?? 0;
                }
                updateData.TREATMENT_ID = this.treatmentID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                updateData = null;
            }
            return updateData;
        }

        private void AddAllergenicByForm()
        {
            try
            {
                AllergenicADO allergenic = new AllergenicADO();

                if (this.currentMedi != null)
                {
                    allergenic.MEDICINE_TYPE_ID = currentMedi.ID;
                }
                allergenic.ALLERGENIC_NAME = txtDiNguyenThuoc.Text;
                allergenic.IS_DOUBT = chkNghiNgo.Checked ? (short?)1 : null;
                allergenic.IS_SURE = chkChacChan.Checked ? (short?)1 : null;
                allergenic.ChacChan = chkChacChan.Checked;
                allergenic.NghiNgo = chkNghiNgo.Checked;
                allergenic.CLINICAL_EXPRESSION = txtBieuHienLamSang.Text;
                listAllergenic.Add(allergenic);

                SetDataSourceGrid(listAllergenic);
                SetDefaultValueAdd();
                CheckEnableToSave();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSourceGrid(List<AllergenicADO> data)
        {
            try
            {
                gridControlAllergenic.DataSource = null;
                gridViewAllergenic.BeginUpdate();
                gridViewAllergenic.GridControl.DataSource = data;
                gridViewAllergenic.RefreshData();
                gridViewAllergenic.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultValue()
        {
            try
            {
                dtNgayCap.DateTime = DateTime.Now;
                txtBacSi.Text = "";
                cboBacSi.EditValue = null;
                txtSoDienThoai.Text = "";
                //cboThuoc.EditValue = null;
                txtDiNguyenThuoc.Text = "";
                chkNghiNgo.Checked = true;
                chkChacChan.Checked = false;
                txtBieuHienLamSang.Text = "";
                SetVisibleDiNguyenThuoc(true);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProviderAdd);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderSave, dxErrorProviderSave);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultValueAdd()
        {
            try
            {
                chkNghiNgo.Checked = true;
                chkChacChan.Checked = false;
                txtBieuHienLamSang.Text = "";
                txtDiNguyenThuoc.Text = "";

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProviderAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDefaultInfo()
        {
            try
            {
                listAllergenic = new List<AllergenicADO>();
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                dtNgayCap.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetControl(bool set)
        {
            try
            {
                btnSave.Enabled = set;
                btnPrint.Enabled = !set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckEnableToSave()
        {
            try
            {
                if (listAllergenic == null && listAllergenic.Count == 0)
                    btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetVisibleDiNguyenThuoc(bool set)
        {
            try
            {
                txtDiNguyenThuoc.Visible = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GridThuoc_RowClick(object data)
        {
            try
            {
                V_HIS_MEDICINE_TYPE medi = data as V_HIS_MEDICINE_TYPE;
                if (medi != null)
                {
                    currentMedi = medi;
                    txtDiNguyenThuoc.Text = currentMedi.MEDICINE_TYPE_NAME;
                }
                txtDiNguyenThuoc.Focus();
                txtDiNguyenThuoc.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildControlContainerThuoc()
        {
            try
            {
                gridViewThuoc.BeginUpdate();
                gridViewThuoc.Columns.Clear();
                popupControlContainer.Width = theRequiredWidth;
                popupControlContainer.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = "Tên thuốc vật tư";
                col1.Width = 150;
                col1.VisibleIndex = 1;
                gridViewThuoc.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = "Mã thuốc/vật tư";
                col2.Width = 80;
                col2.VisibleIndex = 2;
                gridViewThuoc.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = "Hoạt chất";
                col4.Width = 200;
                col4.VisibleIndex = 4;
                gridViewThuoc.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewThuoc.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 80;
                col7.VisibleIndex = 7;
                gridViewThuoc.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà cung cấp";
                col8.Width = 150;
                col8.VisibleIndex = 8;
                gridViewThuoc.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Tên nước";
                col9.Width = 80;
                col9.VisibleIndex = 9;
                gridViewThuoc.Columns.Add(col9);

                gridViewThuoc.GridControl.DataSource = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                gridViewThuoc.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Event form

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void btnLamLai_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnLamLai.Enabled)
                {
                    listAllergenic = new List<AllergenicADO>();
                    SetControl(true);
                    SetDefaultValue();
                    SetDataSourceGrid(null);
                    LoadTreatmentInfoForm(this.currentTreatment);
                    LoadAllergyCardInfoForm(this.currentAllergyCard);
                    CheckEnableToSave();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtNgayCap_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBacSi.Focus();
                    txtBacSi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtBacSi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtBacSi.Text))
                    {
                        cboBacSi.EditValue = null;
                        cboBacSi.Focus();
                        cboBacSi.ShowPopup();
                    }
                    else
                    {
                        List<ACS_USER> searchs = null;
                        var listData1 = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Contains(txtBacSi.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.LOGINNAME.ToUpper() == txtBacSi.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtBacSi.Text = searchs[0].LOGINNAME;
                            cboBacSi.EditValue = searchs[0].LOGINNAME;
                            txtSoDienThoai.Focus();
                            txtSoDienThoai.SelectAll();
                        }
                        else
                        {
                            cboBacSi.Focus();
                            cboBacSi.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBacSi_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBacSi.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboBacSi.EditValue.ToString());
                        if (data != null)
                        {
                            txtBacSi.Text = data.LOGINNAME;
                            txtSoDienThoai.Focus();
                            txtSoDienThoai.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoDienThoai_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiNguyenThuoc.Focus();
                    txtDiNguyenThuoc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkNghiNgo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChacChan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkChacChan_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBieuHienLamSang.Focus();
                    txtBieuHienLamSang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNghiNgo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNghiNgo.Checked)
                {
                    chkChacChan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkChacChan_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChacChan.Checked)
                {
                    chkNghiNgo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBacSi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBacSi.EditValue != null)
                {
                    var data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboBacSi.EditValue.ToString());
                    if (data != null)
                    {
                        txtBacSi.Text = data.LOGINNAME;
                    }
                }
                else
                {
                    txtBacSi.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridControlThuoc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    V_HIS_MEDICINE_TYPE medi = gridViewThuoc.GetFocusedRow() as V_HIS_MEDICINE_TYPE;
                    if (medi != null)
                    {
                        popupControlContainer.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridThuoc_RowClick(medi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtDiNguyenThuoc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    popupControlContainer.HidePopup();
                    chkNghiNgo.Focus();
                    //D_HIS_MEDI_STOCK_1 matyChePham = gridViewThuoc.GetFocusedRow() as D_HIS_MEDI_STOCK_1;
                    //if (matyChePham != null)
                    //{
                    //    popupControlContainer.HidePopup();
                    //    isShowContainer = false;
                    //    isShowContainerForChoose = true;
                    //    GridThuoc_RowClick(matyChePham);
                    //}
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewThuoc.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtDiNguyenThuoc.Bounds.X + this.ParentForm.Bounds.X + 108, txtDiNguyenThuoc.Bounds.Y + this.ParentForm.Bounds.Y + 60, txtDiNguyenThuoc.Bounds.Width - 90, txtDiNguyenThuoc.Bounds.Height);
                    popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 28));
                    gridViewThuoc.Focus();
                    gridViewThuoc.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtDiNguyenThuoc.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiNguyenThuoc_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        Rectangle buttonBounds = new Rectangle(txtDiNguyenThuoc.Bounds.X + this.ParentForm.Bounds.X + 108, txtDiNguyenThuoc.Bounds.Y + this.ParentForm.Bounds.Y + 60, txtDiNguyenThuoc.Bounds.Width - 90, txtDiNguyenThuoc.Bounds.Height);
                        popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 28));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiNguyenThuoc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtDiNguyenThuoc.Text))
                {
                    txtDiNguyenThuoc.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewThuoc.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        //Filter data
                        gridViewThuoc.ActiveFilterString = "[MEDICINE_TYPE_NAME] Like '%" + txtDiNguyenThuoc.Text
                            + "%' OR [MEDICINE_TYPE_CODE] Like '%" + txtDiNguyenThuoc.Text + "%'"
                        + " OR [ACTIVE_INGR_BHYT_NAME] Like '%" + txtDiNguyenThuoc.Text + "%'";
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewThuoc.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewThuoc.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewThuoc.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewThuoc.FocusedRowHandle = 0;
                        gridViewThuoc.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewThuoc.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtDiNguyenThuoc.Bounds.X + this.ParentForm.Bounds.X + 108, txtDiNguyenThuoc.Bounds.Y + this.ParentForm.Bounds.Y + 60, txtDiNguyenThuoc.Bounds.Width - 90, txtDiNguyenThuoc.Bounds.Height);

                        if (isShow)
                        {
                            popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 28));
                            isShow = false;
                        }

                        txtDiNguyenThuoc.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewThuoc.ActiveFilter.Clear();
                    if (!isShowContainer)
                    {
                        popupControlContainer.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
                txtDiNguyenThuoc.Focus();
                txtDiNguyenThuoc.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewThuoc_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                V_HIS_MEDICINE_TYPE medi = gridViewThuoc.GetFocusedRow() as V_HIS_MEDICINE_TYPE;
                if (medi != null)
                {
                    popupControlContainer.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridThuoc_RowClick(medi);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event grid

        private void gridViewAllergenic_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_ALLERGENIC pData = (MOS.EFMODEL.DataModels.HIS_ALLERGENIC)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "NghiNgo")
                    {
                        e.Value = pData.IS_DOUBT == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "ChacChan")
                    {
                        e.Value = pData.IS_SURE == 1 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAllergenic_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewAllergenic_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

        }

        private void Btn_Xoa_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (AllergenicADO)gridViewAllergenic.GetFocusedRow();
                if (row != null)
                {
                    listAllergenic.Remove(row);

                    gridControlAllergenic.BeginUpdate();
                    gridControlAllergenic.DataSource = listAllergenic;
                    gridControlAllergenic.EndUpdate();
                    CheckEnableToSave();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewAllergenic_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "NghiNgo")
                {
                    var val = (bool)e.Value;
                    gridViewAllergenic.SetRowCellValue(e.RowHandle, "ChacChan", !val);
                    gridViewAllergenic.RefreshRow(e.RowHandle);
                }

                if (e.Column.FieldName == "ChacChan")
                {
                    var val = (bool)e.Value;
                    gridViewAllergenic.SetRowCellValue(e.RowHandle, "NghiNgo", !val);
                    gridViewAllergenic.RefreshRow(e.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method

        public void Add()
        {
            try
            {
                if (btnAdd.Enabled)
                {
                    positionHandleAdd = -1;
                    if (!dxValidationProviderAdd.Validate() || ValidateAdd())
                    {
                        return;
                    }

                    AddAllergenicByForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Save()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    gridViewAllergenic.PostEditor();
                    bool success = false;
                    positionHandleSave = -1;
                    if (!dxValidationProviderSave.Validate() || ValidateSave())
                    {
                        return;
                    }

                    var listDataSource = (List<AllergenicADO>)gridControlAllergenic.DataSource;
                    if (listDataSource != null && listDataSource.Count > 0)
                    {

                        List<HIS_ALLERGENIC> allergenics = new List<HIS_ALLERGENIC>();
                        foreach (var item in listDataSource)
                        {
                            HIS_ALLERGENIC aller = new HIS_ALLERGENIC();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ALLERGENIC>(aller, item);
                            aller.IS_DOUBT = item.NghiNgo ? (short?)1 : null;
                            aller.IS_SURE = item.ChacChan ? (short?)1 : null;
                            allergenics.Add(aller);
                        }

                        CommonParam param = new CommonParam();
                        HisAllergyCardSDO sdo = new HisAllergyCardSDO();
                        sdo.AllergyCard = UpdateAllergyCardByForm();
                        sdo.Allergenics = allergenics;

                        WaitingManager.Show();
                        if (this.currentAllergyCard != null)
                        {
                            var result = new BackendAdapter(param).Post<HisAllergyCardResultSDO>(HisRequestUriStore.HIS_ALLERGY_CARD_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            if (result != null)
                            {
                                resultAllergyCardSdo = result;
                                success = true;
                                SetControl(false);
                            }
                        }
                        else
                        {
                            var result = new BackendAdapter(param).Post<HisAllergyCardResultSDO>(HisRequestUriStore.HIS_ALLERGY_CARD_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            if (result != null)
                            {
                                resultAllergyCardSdo = result;
                                success = true;
                                SetControl(false);
                            }
                        }
                        WaitingManager.Hide();

                        #region Hien thi message thong bao
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        public void Print()
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    PrintTypeCare type = new PrintTypeCare();
                    type = PrintTypeCare.IN_THE_DI_UNG;
                    PrintProcess(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RefreshForm()
        {
            try
            {
                btnLamLai_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private void dxValidationProviderAdd_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleAdd == -1)
                {
                    positionHandleAdd = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandleAdd > edit.TabIndex)
                {
                    positionHandleAdd = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderSave_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleSave == -1)
                {
                    positionHandleSave = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandleSave > edit.TabIndex)
                {
                    positionHandleSave = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControlForm()
        {
            try
            {
                ValidateSingleControl(dxValidationProviderSave, dtNgayCap);
                ValidateSingleControl(dxValidationProviderAdd, txtDiNguyenThuoc);
                ValidateTextEditWithGridLookUp(dxValidationProviderSave, txtBacSi, cboBacSi);
                ValidateMaxLength(dxValidationProviderSave, txtSoDienThoai, 12);
                ValidateMaxLength(dxValidationProviderAdd, txtBieuHienLamSang, 500);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateSingleControl(DXValidationProvider dx, BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule controlValid = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                controlValid.editor = control;
                controlValid.ErrorType = ErrorType.Warning;
                controlValid.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                dx.SetValidationRule(control, controlValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateTextEditWithGridLookUp(DXValidationProvider dx, TextEdit txt, GridLookUpEdit gr)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.GridLookupEditWithTextEditValidationRule controlValid = new Inventec.Desktop.Common.Controls.ValidationRule.GridLookupEditWithTextEditValidationRule();
                controlValid.cbo = gr;
                controlValid.txtTextEdit = txt;
                controlValid.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                controlValid.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(txt, controlValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateMaxLength(DXValidationProvider dx, BaseEdit txt, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule controlValid = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                controlValid.editor = txt;
                controlValid.maxLength = maxLength;
                controlValid.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                controlValid.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(txt, controlValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool ValidateAdd()
        {
            bool result = false;
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return result;
        }

        private bool ValidateSave()
        {
            bool result = false;
            try
            {
                if (listAllergenic == null || listAllergenic.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập đầy đủ thông tin", "Thông báo");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return result;
        }

        #endregion

        #region Print

        internal enum PrintTypeCare
        {
            IN_THE_DI_UNG,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_THE_DI_UNG:
                        richEditorMain.RunPrintTemplate("Mps000253", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000253":
                        LoadInTheDiUng(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadInTheDiUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (resultAllergyCardSdo != null)
                {
                    V_HIS_PATIENT patientPrint = new V_HIS_PATIENT();
                    V_HIS_TREATMENT treatmentPrint = new V_HIS_TREATMENT();
                    Mps000253ADO adoPrint = new Mps000253ADO();
                    V_HIS_ALLERGY_CARD allergenyCardPrint = new V_HIS_ALLERGY_CARD();
                    List<V_HIS_ALLERGENIC> listAllergenicPrint = new List<V_HIS_ALLERGENIC>();

                    allergenyCardPrint = resultAllergyCardSdo.HisAllergyCard;

                    if (resultAllergyCardSdo.HisAllergenics != null && resultAllergyCardSdo.HisAllergenics.Count > 0)
                    {
                        listAllergenicPrint = GetAllergenicPrint(resultAllergyCardSdo.HisAllergenics);
                    }

                    if (allergenyCardPrint != null)
                    {
                        treatmentPrint = GetTreatment(allergenyCardPrint.TREATMENT_ID);
                    }
                    adoPrint.DEPARTMENT_NAME = WorkPlace.GetDepartmentName();

                    if (treatmentPrint != null)
                    {
                        patientPrint = GetPatientPrint(treatmentPrint.PATIENT_ID);
                    }

                    MPS.Processor.Mps000253.PDO.Mps000253PDO mps000253PDO = new MPS.Processor.Mps000253.PDO.Mps000253PDO(treatmentPrint, patientPrint, allergenyCardPrint, adoPrint, listAllergenicPrint);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentPrint != null ? treatmentPrint.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]) { EmrInputADO = inputADO };
                        }
                    }
                    else
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000253PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_ALLERGENIC> GetAllergenicPrint(List<HIS_ALLERGENIC> data)
        {
            List<V_HIS_ALLERGENIC> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisAllergenicViewFilter filter = new HisAllergenicViewFilter();
                filter.IDs = data.Select(o => o.ID).ToList();

                result = new BackendAdapter(param).Get<List<V_HIS_ALLERGENIC>>(HisRequestUriStore.HIS_ALLERGEIC_GETVIEW, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }

        private V_HIS_PATIENT GetPatientPrint(long patientID)
        {
            V_HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = patientID;

                var getResult = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (getResult != null && getResult.Count > 0)
                {
                    result = getResult.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return result;

        }

        #endregion

    }
}
