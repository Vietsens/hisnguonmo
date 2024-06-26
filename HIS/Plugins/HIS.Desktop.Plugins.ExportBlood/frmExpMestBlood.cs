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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExportBlood.ADO;
using HIS.Desktop.Plugins.ExportBlood.Validation;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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
    public partial class frmExpMestBlood : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        List<V_HIS_EXP_MEST_BLTY> listExpMestBlty;

        List<V_HIS_BLOOD> listBlood = new List<V_HIS_BLOOD>();
        Dictionary<string, V_HIS_BLOOD> dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();

        Dictionary<long, V_HIS_BLOOD> dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
        Dictionary<long, V_HIS_BLOOD> dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();

        Dictionary<long, VHisBloodADO> dicBloodAdo = new Dictionary<long, VHisBloodADO>();

        V_HIS_EXP_MEST_BLTY currentBlty = null;

        List<BloodVolumeADO> bloodVolume;

        bool checkBtnRefresh = true;

        //List<V_HIS_PATIENT_TYPE_ALLOW> listPatyAllow = new List<V_HIS_PATIENT_TYPE_ALLOW>();
        //HIS_PATIENT_TYPE currentPatientType = null;

        V_HIS_EXP_MEST ExpMest = null;
        long expMestId;

        HIS_EXP_MEST resultExpMest = null;

        int positionHandleControl = -1;

        public frmExpMestBlood(Inventec.Desktop.Common.Modules.Module module, long data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                this.expMestId = data;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmExpMestBlood(Inventec.Desktop.Common.Modules.Module module, V_HIS_EXP_MEST data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                this.ExpMest = data;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpMestBlood_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadExpMestById();
                if (this.ExpMest != null)
                {
                    this.ValidControl();
                    this.FillDataToGridExpMestBlty();
                    this.LoadDataBloodAndPatyMediStockId();
                    this.ProcessDataBlood();
                    this.FillDataToGridBlood();
                    this.FillDataToGridExpMestBlood();
                    this.SetControlByExpMestBlty();
                    this.btnSave.Enabled = true;
                    this.btnPrint.Enabled = false;
                    frmExpMestBlood_Plus_GridLookup();
                }
                else
                {
                    this.btnSave.Enabled = false;
                    this.btnPrint.Enabled = false;
                }
                this.btnAssignService.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestById()
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpMestBlty()
        {
            try
            {
                listExpMestBlty = new List<V_HIS_EXP_MEST_BLTY>();
                HisExpMestBltyViewFilter expMestBltyFilter = new HisExpMestBltyViewFilter();
                expMestBltyFilter.EXP_MEST_ID = this.ExpMest.ID;
                listExpMestBlty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY>>("api/HisExpMestBlty/GetView", ApiConsumers.MosConsumer, expMestBltyFilter, null);
                gridControlExpMestBlty.BeginUpdate();
                gridControlExpMestBlty.DataSource = listExpMestBlty;
                gridControlExpMestBlty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientType()
        {
            try
            {
                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodAndPatyMediStockId()
        {
            try
            {
                listBlood = new List<V_HIS_BLOOD>();
                HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                bloodFilter.MEDI_STOCK_ID = this.ExpMest.MEDI_STOCK_ID;
                bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataBlood()
        {
            try
            {
                dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();
                if (listBlood != null && listBlood.Count > 0)
                {
                    foreach (var item in listBlood)
                    {
                        dicBloodCode[item.BLOOD_CODE] = item;
                        dicCurrentBlood[item.ID] = item;
                        if (!dicBloodAdo.ContainsKey(item.ID))
                            dicShowBlood[item.ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBlood()
        {
            try
            {
                gridControlBlood.BeginUpdate();
                gridControlBlood.DataSource = dicShowBlood.Select(s => s.Value).OrderBy(o =>
                  o.BLOOD_TYPE_NAME).ThenBy(o => o.VOLUME).ThenBy(o => o.BLOOD_CODE).ToList();
                gridControlBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpMestBlood()
        {
            try
            {
                gridControlExpMestBlood.BeginUpdate();
                gridControlExpMestBlood.DataSource = dicBloodAdo.Select(s => s.Value).ToList();
                gridControlExpMestBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlByExpMestBlty()
        {
            try
            {
                if (this.currentBlty != null)
                {
                    lblBloodTypeInfo.Text = this.currentBlty.BLOOD_TYPE_CODE + " - " + this.currentBlty.BLOOD_TYPE_NAME;
                }
                else
                {
                    lblBloodTypeInfo.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlBloodCode();
                ValidControlExpiredDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBloodCode()
        {
            try
            {
                BarCodeValidationRule bloodCodeRule = new BarCodeValidationRule();
                bloodCodeRule.txtBarCode = txtBloodCode;
                dxValidationProvider2.SetValidationRule(txtBloodCode, bloodCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpiredDate()
        {
            try
            {
                ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
                expiredDateRule.dtExpiredDate = dtExpiredDate;
                dxValidationProvider1.SetValidationRule(txtExpiredDate, expiredDateRule);
                dxValidationProvider2.SetValidationRule(txtExpiredDate, expiredDateRule);
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
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__BTN_SAVE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__BTN_PRINT", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnAssignService.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__BTN_ASSIGN_SERVICE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Layout
                this.layoutBloodCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__LAYOUT_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodTypeInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__LAYOUT_BLOOD_TYPE_INFO", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutExpiredDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__LAYOUT_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl Blood
                this.gridColumn_Blood_BloodAbo.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_BLOOD_ABO", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodRh.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_BLOOD_RH", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_STT", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Blood_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_BLOOD__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


                //GridControl ExpMestBlood
                this.gridColumn_ExpMestBlood_BloodAbo.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_ABO", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_CODE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodRhCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_RH", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_TYPE_CODE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_PATIENT_TYPE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_STT", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlood_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLOOD__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControlExpMestBlty
                this.gridColumn_ExpMestBlty_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodAbo.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_ABO", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodRh.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_RH", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_TYPE_CODE", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_STT", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExpMestBlty_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__GRID_EXP_MEST_BLTY__COLUMN_VOLUME", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemBtnDeleteBlood.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_BLOOD__REPOSITORY__BTN_DELETE_BLOOD", Base.ResourceLangManager.LanguageFrmExpMestBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodType.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpVolume.Focus();
                        gridLookUpVolume.SelectAll();
                    }
                    else
                    {
                        cboBloodType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBloodType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                gridLookUpVolume.Focus();
                gridLookUpVolume.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


    }
}
