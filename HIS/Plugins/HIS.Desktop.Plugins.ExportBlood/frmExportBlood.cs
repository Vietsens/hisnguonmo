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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExportBlood.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportBlood
{
    public partial class frmExportBlood : Form
    {
        V_HIS_EXP_MEST ExpMest = null;
        HIS_EXP_MEST resultExpMest = null;
        List<V_HIS_EXP_MEST_BLTY> ListExpMestBlty;
        List<VHisBloodADO> ListBlood = new List<VHisBloodADO>();
        V_HIS_EXP_MEST_BLTY currentExpMestBlty = null;
        Dictionary<long, List<VHisBloodADO>> dicBlood = new Dictionary<long, List<VHisBloodADO>>();
        List<V_HIS_EXP_MEST_BLOOD> ListExpMestBlood = new List<V_HIS_EXP_MEST_BLOOD>();

        long expMestId;

        int positionHandleControl = -1;

        public frmExportBlood(long data)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.expMestId = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmExportBlood(V_HIS_EXP_MEST data)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                ExpMest = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmExportBlood_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                SetControlValue();
                LoadDataToGridExpMestBlty();
                SetPrintTypeToMps();
                ValidControl();
                txtBarCode.Focus();
                txtBarCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlValue()
        {
            try
            {
                if (this.expMestId > 0)
                {
                    HisExpMestViewFilter expFilter = new HisExpMestViewFilter();
                    expFilter.ID = this.expMestId;
                    var listExp = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expFilter, null);
                    if (listExp != null && listExp.Count == 1)
                    {
                        this.ExpMest = listExp.First();
                    }
                }
                btnPrint.Enabled = false;
                dtExpiredDate.EditValue = null;
                txtBloodTypeCode.Text = "";
                lblBloodTypeName.Text = "";
                if (ExpMest != null)
                {
                    lblExpMestCode.Text = ExpMest.EXP_MEST_CODE;
                    lblMediStockName.Text = ExpMest.MEDI_STOCK_NAME;
                    lblPatientCode.Text = ExpMest.PATIENT_CODE;
                    lblVirPatientName.Text = ExpMest.VIR_PATIENT_NAME;
                }
                else
                {
                    btnAdd.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridExpMestBlty()
        {
            try
            {
                if (ExpMest != null)
                {
                    HisExpMestBltyViewFilter bltyFilter = new HisExpMestBltyViewFilter();
                    bltyFilter.EXP_MEST_ID = ExpMest.ID;
                    ListExpMestBlty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY>>(HisRequestUriStore.HIS_EXP_MEST_BLTY_GETVIEW, ApiConsumers.MosConsumer, bltyFilter, null);
                    gridControlExpMestBlty.BeginUpdate();
                    gridControlExpMestBlty.DataSource = ListExpMestBlty;
                    gridControlExpMestBlty.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Caption Frm
                this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__CAPTION", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__BTN_ADD", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__BTN_REFRESH", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__BTN_SAVE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__BTN_PRINT", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutBarCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodTypeCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_BLOOD_TYPE_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodTypeName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpiredDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpMestCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_EXP_MEST_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutMediStockName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_MEDI_STOCK_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPatientCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_PATIENT_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutVirPatientName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__LAYOUT_VIR_PATIENT_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl Blood
                this.gridColumn_Blood_BloodAboCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_ABO_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodRhCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_RH_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_ExperidDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_MediStockName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_MEDI_STOCK_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_PATIENT_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_SupplierName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_SUPPLIER_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_BLOOD__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl ExpMestBlood
                this.gridColumn_ExpMestBlood_BloodAboCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_ABO_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodRhCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_RH_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_ExpMestCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_EXP_MEST_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_ExpTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_EXP_TIME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_PRICE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_VatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_PATIENT_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControlExpMestBlty
                this.gridColumn_ExpMestBlty_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodAboCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_ABO_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodRhCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_RH_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_TYPE_CODE", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemBtnDeleteBlood.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXPORT_BLOOD__REPOSITORY__BTN_DELETE_BLOOD", Base.ResourceLangManager.LanguageFrmExportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
