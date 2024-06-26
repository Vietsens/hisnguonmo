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
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Advise
{
    public partial class frmAdvise : Form
    {
        Action<AdviseFormADO> actSave;
        AdviseFormADO currentAdviseFormADO;
        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> ExpMestTypeDatas;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> MedicineUseFormDatas;
        List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> medicineUseFormSelecteds;
        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> expMestTypeSelecteds;

        public frmAdvise(AdviseFormADO _currentAdviseFormADO, Action<AdviseFormADO> _actSave)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.actSave = _actSave;
                this.currentAdviseFormADO = _currentAdviseFormADO;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmAdvise_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKeyNew();
                InitMedicineUseFormCheck();
                InitComboMedicineUseForm();
                InitExpMestTypeCheck();
                InitComboExpMestType();

                if (this.currentAdviseFormADO != null)
                {
                    chkIncludeMaterial.Checked = (this.currentAdviseFormADO.IncludeMaterial != null && this.currentAdviseFormADO.IncludeMaterial.Value) ? true : false;
                    if (this.currentAdviseFormADO.MedicineUseFormIds != null && this.currentAdviseFormADO.MedicineUseFormIds.Count > 0 && MedicineUseFormDatas != null && MedicineUseFormDatas.Count > 0)
                    {
                        medicineUseFormSelecteds = MedicineUseFormDatas.Where(o => this.currentAdviseFormADO.MedicineUseFormIds.Contains(o.ID)).ToList();
                        GridCheckMarksSelection gridCheckMark = cboMedicineUseForm.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.SelectAll(medicineUseFormSelecteds);
                        }
                    }
                    if (this.currentAdviseFormADO.ExpMestTypeIds != null && this.currentAdviseFormADO.ExpMestTypeIds.Count > 0 && ExpMestTypeDatas != null && ExpMestTypeDatas.Count > 0)
                    {
                        expMestTypeSelecteds = ExpMestTypeDatas.Where(o => this.currentAdviseFormADO.ExpMestTypeIds.Contains(o.ID)).ToList();
                        GridCheckMarksSelection gridCheckMark = cboPresType.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.SelectAll(expMestTypeSelecteds);
                        }
                    }
                    chkAutoGetHomePres.Checked = (this.currentAdviseFormADO.AutoGetHomePres != null && this.currentAdviseFormADO.AutoGetHomePres.Value) ? true : false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAdvise
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmAdvise = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmAdvise).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAdvise.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAdvise.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.cboPresType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAdvise.cboPresType.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.cboMedicineUseForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAdvise.cboMedicineUseForm.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.chkAutoGetHomePres.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAdvise.chkAutoGetHomePres.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.chkIncludeMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAdvise.chkIncludeMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmAdvise.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmAdvise.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmAdvise.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmAdvise.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmAdvise.bar2.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmAdvise.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAdvise.Text", Resources.ResourceLanguageManager.LanguagefrmAdvise, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineUseFormCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMedicineUseForm.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__MedicineUseForm);
                cboMedicineUseForm.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboMedicineUseForm_CustomDisplayText);
                cboMedicineUseForm.Properties.Tag = gridCheck;
                cboMedicineUseForm.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMedicineUseForm.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMedicineUseForm.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDICINE_USE_FORM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__MedicineUseForm(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                medicineUseFormSelecteds = new List<HIS_MEDICINE_USE_FORM>();
                foreach (MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        medicineUseFormSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.MEDICINE_USE_FORM_NAME.ToString());
                    }
                }
                this.cboMedicineUseForm.Text = sb.ToString();
                cboMedicineUseForm.Properties.Buttons[1].Visible = (medicineUseFormSelecteds != null && medicineUseFormSelecteds.Count > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitExpMestTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPresType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ExpMestType);
                cboPresType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboExpMestType_CustomDisplayText);
                cboPresType.Properties.Tag = gridCheck;
                cboPresType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPresType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPresType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ExpMestType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                expMestTypeSelecteds = new List<HIS_EXP_MEST_TYPE>();
                foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        expMestTypeSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                    }
                }
                cboPresType.Properties.Buttons[1].Visible = (expMestTypeSelecteds != null && expMestTypeSelecteds.Count > 0);
                this.cboPresType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //Đường dùng thuốc
        private void InitComboMedicineUseForm(List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> data = null)
        {
            try
            {
                MedicineUseFormDatas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                MedicineUseFormDatas = MedicineUseFormDatas != null ? MedicineUseFormDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                if (data != null)
                {
                    MedicineUseFormDatas = data.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                    MedicineUseFormDatas = MedicineUseFormDatas.OrderBy(o => o.NUM_ORDER).ToList();

                cboMedicineUseForm.Properties.DataSource = MedicineUseFormDatas;
                cboMedicineUseForm.Properties.DisplayMember = "MEDICINE_USE_FORM_NAME";
                cboMedicineUseForm.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboMedicineUseForm.Properties.View.Columns.AddField("MEDICINE_USE_FORM_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboMedicineUseForm.Properties.PopupFormWidth = 200;
                cboMedicineUseForm.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboMedicineUseForm.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboMedicineUseForm.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboMedicineUseForm.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExpMestType(List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> data = null)
        {
            try
            {
                ExpMestTypeDatas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
                ExpMestTypeDatas = ExpMestTypeDatas != null ? ExpMestTypeDatas.Where(o => o.IS_ACTIVE == 1 && (o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK || o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)).ToList() : null;

                cboPresType.Properties.DataSource = ExpMestTypeDatas;
                cboPresType.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                cboPresType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboPresType.Properties.View.Columns.AddField("EXP_MEST_TYPE_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "";
                cboPresType.Properties.PopupFormWidth = 200;
                cboPresType.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboPresType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPresType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPresType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actSave != null)
                {
                    if (currentAdviseFormADO == null)
                        currentAdviseFormADO = new AdviseFormADO();
                    currentAdviseFormADO.IncludeMaterial = chkIncludeMaterial.Checked;
                    currentAdviseFormADO.AutoGetHomePres = chkAutoGetHomePres.Checked;
                    if (medicineUseFormSelecteds != null && medicineUseFormSelecteds.Count > 0)
                        currentAdviseFormADO.MedicineUseFormIds = medicineUseFormSelecteds.Select(o => o.ID).ToList();
                    else
                        currentAdviseFormADO.MedicineUseFormIds = null;
                    if (expMestTypeSelecteds != null && expMestTypeSelecteds.Count > 0)
                        currentAdviseFormADO.ExpMestTypeIds = expMestTypeSelecteds.Select(o => o.ID).ToList();
                    else
                        currentAdviseFormADO.ExpMestTypeIds = null;

                    this.actSave(currentAdviseFormADO);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
                
        private void cboPresType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPresType.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboPresType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPresType.Properties.View);
                    }
                    expMestTypeSelecteds = null;
                    cboPresType.Properties.Buttons[1].Visible = (expMestTypeSelecteds != null && expMestTypeSelecteds.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMedicineUseForm.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboMedicineUseForm.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboMedicineUseForm.Properties.View);
                    }
                    medicineUseFormSelecteds = null;
                    cboMedicineUseForm.Properties.Buttons[1].Visible = (medicineUseFormSelecteds != null && medicineUseFormSelecteds.Count > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
