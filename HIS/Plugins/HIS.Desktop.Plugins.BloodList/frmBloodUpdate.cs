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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BloodList.Validate;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Adapter;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Win;
using System.Text.RegularExpressions;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.BloodList
{
    public partial class frmBloodUpdate : Form
    {
        V_HIS_BLOOD currentBlood;
        HIS_BLOOD_GIVER currentBloodGiver;
        int positionHandleControl = -1;

        public frmBloodUpdate(V_HIS_BLOOD _blood)
        {
            InitializeComponent();
            try
            {
                this.currentBlood = _blood;
                SetIcon();
                ValidationControls();

                if (_blood.BLOOD_GIVE_ID > 0)
                {
                    lcgBloodGiver.Expanded = true;
                    lcgBloodGiver.ExpandOnDoubleClick = true;
                    lcgBloodGiver.ExpandButtonVisible = true;
                    lciGiverName.AppearanceItemCaption.ForeColor = lciGiverCode.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateSingleControl(txtGiveCode);
                    ValidateSingleControl(txtGiveName);
                    this.Size = new Size(860, 720);
                }
                else
                {
                    lcgBloodGiver.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LcgBloodGriverCheck();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LcgBloodGriverCheck()
        {
            lcgBloodGiver.BeginUpdate();

            try
            {
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcgBloodGiver.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        dxValidationProvider1.SetValidationRule(fomatFrm, null);
                    }
                }
                dxValidationProvider1.SetValidationRule(txtDob, null);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BloodList.Resources.Lang", typeof(HIS.Desktop.Plugins.BloodList.frmBloodUpdate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodUpdate.cboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkInfect.Properties.Caption = Inventec.Common.Resource.Get.Value("frmBloodUpdate.chkInfect.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpSource.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodUpdate.cboImpSource.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodUpdate.cboBloodRh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodAbo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodUpdate.cboBloodAbo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBloodUpdate.cboBloodType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGiverCode.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGiverName.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmBloodUpdate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmBloodUpdate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAtUnit.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.chkAtUnit.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAtPermanentAddress.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.chkAtPermanentAddress.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmBloodUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombos()
        {
            try
            {
                InitComboDefault("ID", "BLOOD_TYPE_NAME", "BLOOD_TYPE_CODE", BackendDataWorker.Get<HIS_BLOOD_TYPE>(), cboBloodType);
                InitComboDefault("ID", "IMP_SOURCE_NAME", "IMP_SOURCE_CODE", BackendDataWorker.Get<HIS_IMP_SOURCE>(), cboImpSource);
                InitComboDefault("ID", "SUPPLIER_NAME", "SUPPLIER_CODE", BackendDataWorker.Get<HIS_SUPPLIER>(), cboSupplier);
                InitComboDefault("ID", "WORK_PLACE_NAME", "WORK_PLACE_CODE", BackendDataWorker.Get<HIS_WORK_PLACE>(), cboWorkPlace);
                InitComboDefault("ID", "CAREER_NAME", "CAREER_CODE", BackendDataWorker.Get<HIS_CAREER>(), cboCareer);
                InitComboDefault("NATIONAL_CODE", "NATIONAL_NAME", "NATIONAL_CODE", BackendDataWorker.Get<SDA_NATIONAL>(), cboNational);
                InitComboDefault("PROVINCE_CODE", "PROVINCE_NAME", "PROVINCE_CODE", BackendDataWorker.Get<SDA_PROVINCE>(), cboProvinceBlood);
                InitComboDefault("PROVINCE_CODE", "PROVINCE_NAME", "PROVINCE_CODE", BackendDataWorker.Get<SDA_PROVINCE>(), cboProvince);
                InitComboDefault("LOGINNAME", "USERNAME", "LOGINNAME", BackendDataWorker.Get<ACS_USER>(), cboExamName);
                InitComboExecuteName();
                InitComboPermanentAddressAddress();


                InitCombo("ID", "BLOOD_ABO_CODE", BackendDataWorker.Get<HIS_BLOOD_ABO>(), cboBloodAbo);
                InitCombo("ID", "BLOOD_RH_CODE", BackendDataWorker.Get<HIS_BLOOD_RH>(), cboBloodRh);
                InitCombo("ID", "GENDER_NAME", BackendDataWorker.Get<HIS_GENDER>(), cboGender);
                InitCombo("ID", "VOLUME", BackendDataWorker.Get<HIS_BLOOD_VOLUME>().Where(o => o.IS_DONATION == 1 && o.IS_ACTIVE == 1).ToList(), cboBloodVolumn);
                InitCombo("ID", "BLOOD_ABO_CODE", BackendDataWorker.Get<HIS_BLOOD_ABO>(), cboBloodAboAfter);
                InitCombo("ID", "BLOOD_RH_CODE", BackendDataWorker.Get<HIS_BLOOD_RH>(), cboBloodRhAfter);

                InitComboGiveType();
                InitComboBeforeHbv();
                InitComboTestAfter(cboTestAfterHbv);
                InitComboTestAfter(cboTestAfterHcv);
                InitComboTestAfter(cboTestAfterHiv);
                InitComboTestAfter(cboTestAfterGm);
                InitComboTestAfter(cboTestAfterKtbt);
                InitComboTestAfterNat(cboTestAfterNatHbv);
                InitComboTestAfterNat(cboTestAfterNatHcv);
                InitComboTestAfterNat(cboTestAfterNatHiv);
                InitComboTestAfterNat(cboTestAfterSlktb);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboPermanentAddressAddress()
        {
            try
            {
                List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                if (communeADOs != null)
                {
                    this.InitComboCommonUtil(this.cboPermanentAddress, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 650, "SEARCH_CODE_COMMUNE", 150, "RENDERER_PDC_NAME_UNSIGNED", 5, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommonUtil(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth, string displayMember1, int displayMember1Width, int displayMember1VisibleIndex)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 150), 1, true));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 150);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 550), 2, true));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 550);
                }
                if (!String.IsNullOrEmpty(displayMember1))
                {
                    columnInfos.Add(new ColumnInfo(displayMember1, "", (displayMember1Width > 0 ? displayMember1Width : 250), (displayMember1VisibleIndex > 0 ? displayMember1VisibleIndex : -1), true));
                    popupWidth += (displayMember1Width > 0 ? displayMember1Width : (displayMember1VisibleIndex > 0 ? 250 : 0));
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteName()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboExecuteName, BackendDataWorker.Get<ACS_USER>(), controlEditorADO);
                string logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                cboExecuteName.EditValue = logginname;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestAfterNat(GridLookUpEdit grd)
        {
            try
            {
                List<TypeADO> data = new List<TypeADO>();
                data.Add(new TypeADO(0, "Không phản ứng"));
                data.Add(new TypeADO(1, "Có phản ứng"));
                data.Add(new TypeADO(2, "Không xét nghiệm"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(grd, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestAfter(GridLookUpEdit grd)
        {
            try
            {
                List<TypeADO> data = new List<TypeADO>();
                data.Add(new TypeADO(0, "Không phản ứng"));
                data.Add(new TypeADO(1, "Có phản ứng"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(grd, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBeforeHbv()
        {
            try
            {
                List<TypeADO> data = new List<TypeADO>();
                data.Add(new TypeADO(0, "Âm tính"));
                data.Add(new TypeADO(1, "Dương tính"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboTestBeforeHbv, data, controlEditorADO);
                cboTestBeforeHbv.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGiveType()
        {
            try
            {
                List<TypeADO> data = new List<TypeADO>();
                data.Add(new TypeADO(1, "Tình nguyện"));
                data.Add(new TypeADO(2, "Chuyên nghiệp"));
                data.Add(new TypeADO(3, "Người nhà"));
                data.Add(new TypeADO(4, "Người thân"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboGiveType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDefault(string valueMember, string displayMember, string columnName, object data, object control)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(columnName, "", 100, 1));
                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCombo(string valueMember, string displayMember, object data, object control)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                columnInfos.Add(new ColumnInfo(displayMember, "", 350, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillCurrentBlood(V_HIS_BLOOD blood)
        {
            try
            {
                if (blood != null)
                {
                    this.txtBloodCode.Text = blood.BLOOD_CODE;
                    this.txtGiveCode.Text = blood.GIVE_CODE;
                    this.txtGiveName.Text = blood.GIVE_NAME;
                    this.txtPackingNumber.Text = blood.PACKAGE_NUMBER;

                    this.cboBloodAbo.EditValue = blood.BLOOD_ABO_ID;
                    this.cboBloodRh.EditValue = blood.BLOOD_RH_ID;
                    this.cboBloodType.EditValue = blood.BLOOD_TYPE_ID;
                    this.cboImpSource.EditValue = blood.IMP_SOURCE_ID;
                    this.cboSupplier.EditValue = blood.SUPPLIER_ID;

                    this.dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(blood.EXPIRED_DATE ?? 0);
                    this.dtPackingTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(blood.PACKING_TIME ?? 0);

                    chkInfect.Checked = blood.IS_INFECT == 1 ? true : false;

                    spImpPrice.EditValue = blood.IMP_PRICE;
                    spImpRatioVat.EditValue = blood.IMP_VAT_RATIO * 100;
                    spInternalPrice.EditValue = blood.INTERNAL_PRICE;
                    if (blood.BLOOD_GIVE_ID > 0)
                    {
                        HisBloodGiverFilter bloodGiverFilter = new HisBloodGiverFilter();
                        bloodGiverFilter.ID = blood.BLOOD_GIVE_ID;
                        currentBloodGiver = new BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_GIVER>>("api/HisBloodGiver/Get", ApiConsumers.MosConsumer, bloodGiverFilter, null).FirstOrDefault();
                        if (currentBloodGiver != null)
                        {
                            FillCurrentBloodGiver(currentBloodGiver);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillCurrentBloodGiver(HIS_BLOOD_GIVER bloodGiver)
        {
            try
            {
                var dob = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.DOB);
                txtDob.Text = dob.ToString("dd/MM/yyyy");
                dtDob.DateTime = dob;
                cboGender.EditValue = bloodGiver.GENDER_ID;
                cboWorkPlace.EditValue = bloodGiver.WORK_PLACE_ID;
                cboGiveType.EditValue = bloodGiver.GIVE_TYPE;
                cboCareer.EditValue = bloodGiver.CAREER_ID;
                txtWorkPlace.Text = bloodGiver.WORK_PLACE;
                cboNational.EditValue = bloodGiver.NATIONAL_CODE;
                cboProvinceBlood.EditValue = bloodGiver.PROVINCE_CODE_BLOOD;
                cboDistrictBlood.EditValue = bloodGiver.DISTRICT_CODE_BLOOD;
                cboProvince.EditValue = bloodGiver.PROVINCE_CODE;
                cboDistrict.EditValue = bloodGiver.DISTRICT_CODE;
                cboCommune.EditValue = bloodGiver.COMMUNE_CODE;
                txtAddress.Text = bloodGiver.ADDRESS;
                txtCardNumber.Text = bloodGiver.GIVE_CARD;
                txtPhone.Text = bloodGiver.PHONE;
                if (!string.IsNullOrEmpty(bloodGiver.PASSPORT_NUMBER))
                {
                    txtCmndCccdHc.Text = bloodGiver.PASSPORT_NUMBER;
                    dtDateRange.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.PASSPORT_DATE ?? 0) ?? DateTime.MinValue;
                    txtAddressProvide.Text = bloodGiver.PASSPORT_PLACE;
                }
                else if (!string.IsNullOrEmpty(bloodGiver.CMND_NUMBER))
                {
                    txtCmndCccdHc.Text = bloodGiver.CMND_NUMBER;
                    dtDateRange.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.CMND_DATE ?? 0) ?? DateTime.MinValue;
                    txtAddressProvide.Text = bloodGiver.CMND_PLACE;
                }
                else if (!string.IsNullOrEmpty(bloodGiver.CCCD_NUMBER))
                {
                    txtCmndCccdHc.Text = bloodGiver.CCCD_NUMBER;
                    dtDateRange.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.CCCD_DATE ?? 0) ?? DateTime.MinValue;
                    txtAddressProvide.Text = bloodGiver.CCCD_PLACE;
                }
                //KLX
                dtExamTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.EXAM_TIME ?? 0) ?? DateTime.MinValue;
                spnTurn.EditValue = bloodGiver.TURN;
                spnWeight.EditValue = bloodGiver.WEIGHT;
                spnPluse.EditValue = bloodGiver.PULSE;
                spnBloodPressureMax.EditValue = bloodGiver.BLOOD_PRESSURE_MAX;
                spnBloodPressureMin.EditValue = bloodGiver.BLOOD_PRESSURE_MIN;
                txtNoteSubclinical.Text = bloodGiver.NOTE_SUBCLINICAL;
                txtTestBeforeHb.Text = bloodGiver.TEST_BEFORE_HB;
                cboTestBeforeHbv.EditValue = bloodGiver.TEST_BEFORE_HBV;
                cboBloodVolumn.EditValue = bloodGiver.BLOOD_VOLUME_ID;
                cboExamName.EditValue = bloodGiver.EXAM_LOGINNAME;
                dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiver.EXECUTE_TIME ?? 0) ?? DateTime.MinValue;
                spnAmount.EditValue = bloodGiver.AMOUNT;
                cboExecuteName.EditValue = bloodGiver.EXECUTE_LOGINNAME;
                txtExecute.Text = bloodGiver.EXECUTE;
                // XN sau lay mau
                cboBloodAboAfter.EditValue = bloodGiver.BLOOD_ABO_ID;
                cboBloodRhAfter.EditValue = bloodGiver.BLOOD_RH_ID;
                cboTestAfterHbv.EditValue = bloodGiver.TEST_AFTER_HBV;
                cboTestAfterHcv.EditValue = bloodGiver.TEST_AFTER_HCV;
                cboTestAfterHiv.EditValue = bloodGiver.TEST_AFTER_HIV;
                cboTestAfterGm.EditValue = bloodGiver.TEST_AFTER_GM;
                cboTestAfterKtbt.EditValue = bloodGiver.TEST_AFTER_KTBT;
                cboTestAfterNatHbv.EditValue = bloodGiver.TEST_AFTER_NAT_HBV;
                cboTestAfterNatHcv.EditValue = bloodGiver.TEST_AFTER_NAT_HCV;
                cboTestAfterNatHiv.EditValue = bloodGiver.TEST_AFTER_NAT_HIV;
                cboTestAfterSlktb.EditValue = bloodGiver.TEST_AFTER_SLKTB;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetHisBlood(long id, ref HIS_BLOOD bloodRef)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodFilter bloodFilter = new HisBloodFilter();
                bloodFilter.ID = id;

                var listRs = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, param);
                if (listRs != null && listRs.Count > 0)
                {
                    bloodRef = listRs.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UpdateDataSave(ref HIS_BLOOD hisBlood)
        {
            try
            {
                hisBlood.BLOOD_CODE = txtBloodCode.Text;
                hisBlood.GIVE_CODE = txtGiveCode.Text;
                hisBlood.GIVE_NAME = txtGiveName.Text;
                hisBlood.PACKAGE_NUMBER = txtPackingNumber.Text;
                if (cboBloodType.EditValue != null)
                {
                    hisBlood.BLOOD_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodType.EditValue.ToString());
                }

                if (spImpPrice.EditValue != null)
                    hisBlood.IMP_PRICE = spImpPrice.Value;
                if (spImpRatioVat.EditValue != null)
                    hisBlood.IMP_VAT_RATIO = spImpRatioVat.Value / 100;
                if (spInternalPrice.EditValue != null)
                    hisBlood.INTERNAL_PRICE = spInternalPrice.Value;
                else
                    hisBlood.INTERNAL_PRICE = null;
                if (cboImpSource.EditValue != null)
                    hisBlood.IMP_SOURCE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpSource.EditValue.ToString());
                else
                    hisBlood.IMP_SOURCE_ID = null;
                if (cboSupplier.EditValue != null)
                    hisBlood.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                else
                    hisBlood.SUPPLIER_ID = null;

                if (dtExpiredDate.DateTime != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    hisBlood.EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    hisBlood.EXPIRED_DATE = null;
                }
                if (dtPackingTime.DateTime != null && dtPackingTime.DateTime != DateTime.MinValue)
                {
                    hisBlood.PACKING_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtPackingTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    hisBlood.PACKING_TIME = null;
                }

                if (cboBloodAbo.EditValue != null)
                    hisBlood.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodAbo.EditValue.ToString());
                if (cboBloodRh.EditValue != null)
                    hisBlood.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodRh.EditValue.ToString());

                hisBlood.IS_INFECT = chkInfect.Checked ? (short)1 : (short)0;
                hisBlood.GIVE_CODE = txtGiveCode.Text.Trim();
                hisBlood.GIVE_NAME = txtGiveName.Text.Trim();
                // 55329
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(pText);
        }

        private bool IsValid(string txtCMND)
        {
            bool valid = false;
            var txt = txtCMND;
            int countNumber = 0;
            int total = txt.Length;
            for (int i = 0; i < txt.Length; i++)
            {
                if (IsNumber(txt[i].ToString()))
                {
                    countNumber++;
                }
            }
            if (countNumber == 0)
            {
                valid = false;
            }
            else if (countNumber != 0 && countNumber < total)
            {
                valid = true;
            }
            return valid;
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl1.IsInitialized) return;
                layoutControl1.BeginUpdate();

                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            dxValidationProvider1.RemoveControlError(fomatFrm);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl1.EndUpdate();
                    chkAtUnit.Text = "Tại đơn vị";
                    chkAtPermanentAddress.Text = "Tại địa chỉ thường trú";
                }
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
                ResetFormData();
                txtBloodCode.Focus();
                txtBloodCode.SelectAll();
                txtWorkPlace.Text = "Tự do";
                chkAtPermanentAddress.Checked = false;
                chkAtUnit.Checked = false;
                dtExamTime.DateTime = DateTime.Now;
                txtTestBeforeHb.Text = "Đạt";
                dtExecuteTime.DateTime = dtExamTime.DateTime.AddMinutes(5);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Event
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmBloodUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultValue();
                InitCombos();
                FillCurrentBlood(this.currentBlood);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;

            try
            {
                this.positionHandleControl = -1;

                if (!dxValidationProvider1.Validate())
                    return;
                if (!chkAtUnit.Checked && cboWorkPlace.EditValue == null && lcgBloodGiver.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn đơn vị", Resources.ResourceMessage.ThongBao);
                    return;
                }
                else if (!chkAtPermanentAddress.Checked && (cboDistrictBlood.EditValue == null || cboProvinceBlood.EditValue == null) && lcgBloodGiver.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn Tỉnh/Huyện", Resources.ResourceMessage.ThongBao);
                    return;
                }
                WaitingManager.Show();

                HIS_BLOOD hisBlood = new HIS_BLOOD();

                GetHisBlood(this.currentBlood.ID, ref hisBlood);

                UpdateDataSave(ref hisBlood);

                var rs = new BackendAdapter(param).Post<HIS_BLOOD>("api/HisBlood/Update", ApiConsumer.ApiConsumers.MosConsumer, hisBlood, param);

                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    if (this.currentBloodGiver != null)
                    {
                        UpdateDataGiverBlood(ref this.currentBloodGiver);
                        var rsBloodGiver = new BackendAdapter(param).Post<HIS_BLOOD_GIVER>("api/HisBloodGiver/Update", ApiConsumer.ApiConsumers.MosConsumer, this.currentBloodGiver, param);
                        success = rsBloodGiver != null;
                    }
                }

                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataGiverBlood(ref HIS_BLOOD_GIVER bloodGiver)
        {
            try
            {

                bloodGiver.GIVE_CODE = txtGiveCode.Text.Trim();
                bloodGiver.GIVE_NAME = txtGiveName.Text.Trim();
                bloodGiver.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDob.DateTime) ?? 0;
                bloodGiver.GENDER_ID = cboGender.EditValue != null ? (long?)Convert.ToInt64(cboGender.EditValue) : null;
                bloodGiver.WORK_PLACE_ID = cboWorkPlace.EditValue != null ? (long?)Convert.ToInt64(cboWorkPlace.EditValue) : null;
                bloodGiver.GIVE_TYPE = cboGiveType.EditValue != null ? (short)Convert.ToInt32(cboGiveType.EditValue) : (short)0;


                if (cboCareer.EditValue != null)
                {
                    var career = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboCareer.EditValue));
                    bloodGiver.CAREER_ID = career != null ? (long?)career.ID : null;
                    bloodGiver.CAREER_NAME = career != null ? career.CAREER_NAME : null;
                }
                if (chkAtUnit.Checked)
                {
                    var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboWorkPlace.EditValue));
                    bloodGiver.WORK_PLACE = workPlace != null ? workPlace.WORK_PLACE_NAME : null;
                }
                else
                {
                    bloodGiver.WORK_PLACE = txtWorkPlace.Text.Trim();
                }
                if (cboNational.EditValue != null)
                {
                    var national = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == cboNational.EditValue.ToString());
                    bloodGiver.NATIONAL_CODE = national != null ? national.NATIONAL_CODE : null;
                    bloodGiver.NATIONAL_NAME = national != null ? national.NATIONAL_NAME : null;
                }
                if (chkAtPermanentAddress.Checked)
                {
                    if (cboPermanentAddress.EditValue != null)
                    {
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboPermanentAddress.EditValue ?? "").ToString());

                        bloodGiver.PROVINCE_CODE_BLOOD = commune != null ? commune.PROVINCE_CODE : null;
                        bloodGiver.PROVINCE_NAME_BLOOD = commune != null ? commune.PROVINCE_NAME : null;
                        bloodGiver.DISTRICT_CODE_BLOOD = commune != null ? commune.DISTRICT_CODE : null;
                        bloodGiver.DISTRICT_NAME_BLOOD = commune != null ? commune.DISTRICT_NAME : null;
                    }
                }
                else
                {
                    var province = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == cboProvinceBlood.EditValue.ToString());
                    var district = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == cboDistrictBlood.EditValue.ToString());

                    bloodGiver.PROVINCE_CODE_BLOOD = province != null ? province.PROVINCE_CODE : null;
                    bloodGiver.PROVINCE_NAME_BLOOD = province != null ? province.PROVINCE_NAME : null;
                    bloodGiver.DISTRICT_CODE_BLOOD = district != null ? district.DISTRICT_CODE : null;
                    bloodGiver.DISTRICT_NAME_BLOOD = district != null ? district.DISTRICT_NAME : null;
                }
                if (cboProvince.EditValue != null)
                {
                    var province = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                    bloodGiver.PROVINCE_CODE = province != null ? province.PROVINCE_CODE : null;
                    bloodGiver.PROVINCE_NAME = province != null ? province.PROVINCE_NAME : null;
                }
                else
                {
                    bloodGiver.PROVINCE_CODE = null;
                    bloodGiver.PROVINCE_NAME = null;
                }
                if (cboDistrict.EditValue != null)
                {
                    var district = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == cboDistrict.EditValue.ToString());
                    bloodGiver.DISTRICT_CODE = district != null ? district.DISTRICT_CODE : null;
                    bloodGiver.DISTRICT_NAME = district != null ? district.DISTRICT_NAME : null;
                }
                else
                {
                    bloodGiver.DISTRICT_CODE = null;
                    bloodGiver.DISTRICT_NAME = null;
                }
                if (cboCommune.EditValue != null)
                {
                    var commune = BackendDataWorker.Get<SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == cboCommune.EditValue.ToString());
                    bloodGiver.COMMUNE_CODE = commune != null ? commune.COMMUNE_CODE : null;
                    bloodGiver.COMMUNE_NAME = commune != null ? commune.COMMUNE_NAME : null;
                }
                else
                {
                    bloodGiver.COMMUNE_CODE = null;
                    bloodGiver.COMMUNE_NAME = null;
                }
                bloodGiver.ADDRESS = txtAddress.Text.Trim();
                bloodGiver.GIVE_CARD = txtCardNumber.Text.Trim();
                bloodGiver.PHONE = txtPhone.Text.Trim();

                if (!string.IsNullOrEmpty(txtCmndCccdHc.Text.Trim()))
                {
                    if (IsNumber(txtCmndCccdHc.Text.Trim()) && txtCmndCccdHc.Text.Trim().Length == 9)
                    {
                        //La CMND
                        bloodGiver.CMND_NUMBER = txtCmndCccdHc.Text.Trim();
                        bloodGiver.PASSPORT_NUMBER = null;
                        bloodGiver.CCCD_NUMBER = null;
                        if (dtDateRange.EditValue != null && dtDateRange.DateTime != DateTime.MinValue)
                        {
                            bloodGiver.CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDateRange.DateTime);
                            bloodGiver.CCCD_DATE = null;
                            bloodGiver.PASSPORT_DATE = null;
                        }
                        if (!string.IsNullOrEmpty(txtAddressProvide.Text.Trim()))
                        {
                            bloodGiver.CMND_PLACE = txtAddressProvide.Text.Trim();
                            bloodGiver.PASSPORT_PLACE = null;
                            bloodGiver.CCCD_PLACE = null;
                        }
                    }
                    else if (IsNumber(txtCmndCccdHc.Text.Trim()) && txtCmndCccdHc.Text.Trim().Length == 12)
                    {
                        //La CCCD
                        bloodGiver.CMND_NUMBER = null;
                        bloodGiver.PASSPORT_NUMBER = null;
                        bloodGiver.CCCD_NUMBER = txtCmndCccdHc.Text.Trim();
                        if (dtDateRange.EditValue != null && dtDateRange.DateTime != DateTime.MinValue)
                        {
                            bloodGiver.CCCD_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDateRange.DateTime);
                            bloodGiver.CMND_DATE = null;
                            bloodGiver.PASSPORT_DATE = null;
                        }
                        if (!string.IsNullOrEmpty(txtAddressProvide.Text.Trim()))
                        {
                            bloodGiver.PASSPORT_PLACE = null;
                            bloodGiver.CMND_PLACE = null;
                            bloodGiver.CCCD_PLACE = txtAddressProvide.Text.Trim();
                        }
                    }
                    else if (IsValid(txtCmndCccdHc.Text.Trim()) && txtCmndCccdHc.Text.Trim().Length < 10)
                    {
                        bloodGiver.CMND_NUMBER = null;
                        bloodGiver.CCCD_NUMBER = null;
                        bloodGiver.PASSPORT_NUMBER = txtCmndCccdHc.Text.Trim();

                        if (dtDateRange.EditValue != null && dtDateRange.DateTime != DateTime.MinValue)
                        {
                            bloodGiver.PASSPORT_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDateRange.DateTime);
                            bloodGiver.CMND_DATE = null;
                            bloodGiver.CCCD_DATE = null;

                        }
                        if (!string.IsNullOrEmpty(txtAddressProvide.Text.Trim()))
                        {
                            bloodGiver.PASSPORT_PLACE = txtAddressProvide.Text.Trim();
                            bloodGiver.CMND_PLACE = null;
                            bloodGiver.CCCD_PLACE = null;
                        }
                    }
                }
                bloodGiver.EXAM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExamTime.DateTime);
                bloodGiver.TURN = spnTurn.EditValue != null ? (long?)Convert.ToInt64(spnTurn.EditValue) : null;
                bloodGiver.WEIGHT = spnWeight.EditValue != null ? (long?)Convert.ToInt64(spnWeight.EditValue) : null;
                bloodGiver.PULSE = spnPluse.EditValue != null ? (long?)Convert.ToInt64(spnPluse.EditValue) : null;
                bloodGiver.BLOOD_PRESSURE_MAX = spnBloodPressureMax.EditValue != null ? (long?)Convert.ToInt64(spnBloodPressureMax.EditValue) : null;
                bloodGiver.BLOOD_PRESSURE_MIN = spnBloodPressureMin.EditValue != null ? (long?)Convert.ToInt64(spnBloodPressureMin.EditValue) : null;
                bloodGiver.NOTE_SUBCLINICAL = !string.IsNullOrEmpty(txtNoteSubclinical.Text.Trim()) ? txtNoteSubclinical.Text.Trim() : null;
                bloodGiver.TEST_BEFORE_HB = txtTestBeforeHb.Text.Trim();
                bloodGiver.TEST_BEFORE_HBV = cboTestBeforeHbv.EditValue != null ? (short?)Convert.ToInt32(cboTestBeforeHbv.EditValue) : null;
                bloodGiver.BLOOD_VOLUME_ID = cboBloodVolumn.EditValue != null ? (long?)Convert.ToInt32(cboBloodVolumn.EditValue) : null;
                if (cboExamName.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboExamName.EditValue.ToString());
                    bloodGiver.EXAM_LOGINNAME = user != null ? user.LOGINNAME : null;
                    bloodGiver.EXAM_USERNAME = user != null ? user.USERNAME : null;
                }
                bloodGiver.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                bloodGiver.AMOUNT = spnAmount.EditValue != null ? (decimal?)Convert.ToDecimal(spnAmount.EditValue) : null;
                if (cboExecuteName.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboExecuteName.EditValue.ToString());
                    bloodGiver.EXECUTE_LOGINNAME = user != null ? user.LOGINNAME : null;
                    bloodGiver.EXECUTE_USERNAME = user != null ? user.USERNAME : null;
                }
                bloodGiver.EXECUTE = !string.IsNullOrEmpty(txtExecute.Text.Trim()) ? txtExecute.Text.Trim() : null;
                bloodGiver.BLOOD_ABO_ID = cboBloodAboAfter.EditValue != null ? (long?)Convert.ToInt64(cboBloodAboAfter.EditValue) : null;
                bloodGiver.BLOOD_RH_ID = cboBloodRhAfter.EditValue != null ? (long?)Convert.ToInt64(cboBloodRhAfter.EditValue) : null;
                bloodGiver.TEST_AFTER_HBV = cboTestAfterHbv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterHbv.EditValue) : null;
                bloodGiver.TEST_AFTER_HCV = cboTestAfterHcv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterHcv.EditValue) : null;
                bloodGiver.TEST_AFTER_HIV = cboTestAfterHiv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterHiv.EditValue) : null;
                bloodGiver.TEST_AFTER_GM = cboTestAfterGm.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterGm.EditValue) : null;
                bloodGiver.TEST_AFTER_KTBT = cboTestAfterKtbt.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterKtbt.EditValue) : null;
                bloodGiver.TEST_AFTER_NAT_HBV = cboTestAfterNatHbv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterNatHbv.EditValue) : null;
                bloodGiver.TEST_AFTER_NAT_HCV = cboTestAfterNatHcv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterNatHcv.EditValue) : null;
                bloodGiver.TEST_AFTER_NAT_HIV = cboTestAfterNatHiv.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterNatHiv.EditValue) : null;
                bloodGiver.TEST_AFTER_SLKTB = cboTestAfterSlktb.EditValue != null ? (short?)Convert.ToInt64(cboTestAfterSlktb.EditValue) : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Key Up

        private void txtBloodCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBloodType.Focus();
                    cboBloodType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBloodType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBloodAbo.Focus();
                    cboBloodAbo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodAbo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBloodRh.Focus();
                    cboBloodRh.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpSource.Focus();
                    cboImpSource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBid_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboImpSource.Focus();
                    cboImpSource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpSource_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPackingTime.Focus();
                    dtPackingTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Focus();
                    dtExpiredDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpiredDate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGiveCode.Focus();
                    txtGiveCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGiveCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGiveName.Focus();
                    txtGiveName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGiveName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackingNumber.Focus();
                    txtPackingNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackingNumber_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spInternalPrice.Focus();
                    spInternalPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spInternalPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spImpPrice.Focus();
                    spImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spImpPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spImpRatioVat.Focus();
                    spImpRatioVat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spImpRatioVat_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkInfect.Focus();
                    chkInfect.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPackingTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSupplier.Focus();
                    cboSupplier.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInfect_KeyUp(object sender, KeyEventArgs e)
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

        private void gridLookUpEdit5_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpSource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridLookUpEdit6_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSupplier.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private void ValidationControls()
        {
            try
            {
                ValidationControlSpinNotVatAndBlack(spInternalPrice);
                ValidationControlSpinNotVatAndRed(spImpPrice);
                ValidationControlSpinVatRed(spImpRatioVat);
                ValidateSingleControl(txtBloodCode);
                ValidateSingleControl(cboBloodAbo);
                ValidateSingleControl(cboBloodType);
                ValidControlCmndCccdHc(txtCmndCccdHc);
                ValidateSingleControl(txtDob);
                ValidateSingleControl(cboWorkPlace);
                ValidateSingleControl(cboGiveType);
                ValidateSingleControl(cboNational);
                ValidateSingleControl(dtExamTime);
                ValidateSingleControl(txtTestBeforeHb);
                ValidateSingleControl(cboBloodVolumn);
                ValidateSingleControl(cboExamName);
                ValidateSingleControl(dtExecuteTime);
                ValidateSingleControl(cboExecuteName);
                ValidateSingleControl(cboBloodAboAfter);
                ValidateSingleControl(cboBloodRhAfter);
                ValidateSingleControl(spnTurn);
                ValidateSingleControl(spnAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidControlCmndCccdHc(TextEdit control)
        {
            try
            {
                ValidCmndCccdHc validRule = new ValidCmndCccdHc();
                validRule.txt = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControl(Control control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidationControlSpinNotVatAndBlack(SpinEdit control)
        {
            try
            {
                SpinNotVatAndBlack validRule = new SpinNotVatAndBlack();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinNotVatAndRed(SpinEdit control)
        {
            try
            {
                SpinNotVatAndRed validRule = new SpinNotVatAndRed();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinVatBlack(SpinEdit control)
        {
            try
            {
                SpinVatBlack validRule = new SpinVatBlack();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinVatRed(SpinEdit control)
        {
            try
            {
                SpinVatRed validRule = new SpinVatRed();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        #endregion

        private void cboBloodRh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBloodRh.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtDob.Text)) return;

                var dt = this.txtDob.Text;
                if (this.txtDob.Text.Contains("/"))
                {
                    dt = dt.Replace(":", "");
                    dt = dt.Replace("/", "");
                    dt = dt.Replace(" ", "").Trim();
                }
                if (!String.IsNullOrEmpty(dt))
                {
                    this.txtDob.Text = dt;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDob_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    if (!string.IsNullOrEmpty(txtDob.Text.Trim()))
                    {
                        DateTime? dt = null;
                        var txt = txtDob.Text.Trim();
                        if (txt.Contains("/")) txt = txt.Replace("/", "");
                        if (txt.Length == 4)
                        {
                            dt = DateTimeHelper.ConvertDateStringToSystemDate(txt);
                        }
                        else
                        {
                            int day = Int16.Parse(txt.Substring(0, 2));
                            int month = Int16.Parse(txt.Substring(2, 2));
                            int year = Int16.Parse(txt.Substring(4, 4));
                            dt = new DateTime(year, month, day);
                        }

                        if (dt != null && dt.Value != DateTime.MinValue)
                        {
                            this.dtDob.EditValue = dt;
                            this.dtDob.Update();
                        }
                    }
                    this.dtDob.Visible = true;
                    this.dtDob.ShowPopup();
                    this.dtDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtDob.Visible = true;
                    this.dtDob.Update();
                    this.txtDob.Text = this.dtDob.DateTime.ToString("dd/MM/yyyy");

                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtDob.Visible = false;
                    this.txtDob.Text = dtDob.DateTime.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDob_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtDob.Text.Trim()))
                {
                    DateTime dt;
                    string dobDisplay = "";
                    var txt = txtDob.Text.Trim();
                    if (txt.Contains("/")) txt = txt.Replace("/", "");
                    if (txt.Length == 4)
                    {
                        dt = (DateTime)DateTimeHelper.ConvertDateStringToSystemDate(txt);
                        dobDisplay = dt.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        int day = Int16.Parse(txt.Substring(0, 2));
                        int month = Int16.Parse(txt.Substring(2, 2));
                        int year = Int16.Parse(txt.Substring(4, 4));
                        dobDisplay = string.Format("{0}/{1}/{2}", txt.Substring(0, 2), txt.Substring(2, 2), txt.Substring(4, 4));
                        dt = new DateTime(year, month, day);
                    }

                    if (dt != null)
                    {
                        this.dtDob.EditValue = dt;
                        this.dtDob.Update();
                        this.txtDob.Text = dobDisplay;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAtUnit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem22.AppearanceItemCaption.ForeColor = chkAtUnit.Checked ? Color.Black : Color.Maroon;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDistrictsBloodCombo(string provinceCode)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();

                if (!string.IsNullOrEmpty(provinceCode))
                {
                    listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.PROVINCE_CODE == provinceCode).ToList();
                }


                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DISTRICT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DISTRICT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDistrictBlood, listResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAtPermanentAddress_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                layoutControlItem29.AppearanceItemCaption.ForeColor = layoutControlItem28.AppearanceItemCaption.ForeColor = chkAtPermanentAddress.Checked ? Color.Black : Color.Maroon;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDistrictsCombo(string provinceCode)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();

                if (!string.IsNullOrEmpty(provinceCode))
                {
                    listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.PROVINCE_CODE == provinceCode).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DISTRICT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DISTRICT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDistrict, listResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneCombo(string districtCode)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();

                if (!string.IsNullOrEmpty(districtCode))
                {
                    listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.DISTRICT_CODE == districtCode).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("COMMUNE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("COMMUNE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("COMMUNE_NAME", "COMMUNE_CODE", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboCommune, listResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPermanentAddress_Popup(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit != null)
                {
                    PopupGridLookUpEditForm f = (edit as IPopupControl).PopupWindow as PopupGridLookUpEditForm;
                    if (f != null)
                    {
                        int newPopupFormWidth = cboPermanentAddress.Width;
                        f.Width = newPopupFormWidth;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPermanentAddress_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboPermanentAddress.EditValue != null)
                {
                    HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboPermanentAddress.EditValue ?? "").ToString());
                    if (commune != null)
                    {
                        this.txtPermanentAddress.Text = commune.SEARCH_CODE_COMMUNE;

                        var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                        if (district != null)
                        {
                            var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == district.PROVINCE_ID);
                            cboProvince.EditValue = province != null ? province.PROVINCE_CODE : null;
                            cboDistrict.EditValue = district.DISTRICT_CODE;
                            cboCommune.EditValue = commune.COMMUNE_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPermanentAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        InitComboPermanentAddressAddress();
                        return;
                    }
                    InitComboPermanentAddressAddress();
                    this.cboPermanentAddress.EditValue = null;
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>()
                                                                                            .Where(o => (o.SEARCH_CODE_COMMUNE != null
                                                                                                    && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    if (listResult != null && listResult.Count >= 1)
                    {
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            InitComboPermanentAddressAddress(listResult);
                        }
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboPermanentAddress.EditValue = dataNoCommunes[0].ID_RAW;
                            this.txtPermanentAddress.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;
                        }
                        else if (listResult.Count == 1)
                        {
                            InitComboPermanentAddressAddress();
                            this.cboPermanentAddress.EditValue = listResult[0].ID_RAW;
                            this.txtPermanentAddress.Text = listResult[0].SEARCH_CODE_COMMUNE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPermanentAddressAddress(List<CommuneADO> communeADOs)
        {
            try
            {
                if (communeADOs != null)
                {
                    this.InitComboCommonUtil(this.cboPermanentAddress, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 650, "SEARCH_CODE_COMMUNE", 150, "RENDERER_PDC_NAME_UNSIGNED", 5, 0);
                }
                this.cboPermanentAddress.EditValue = null;
                this.FocusShowpopupExt(this.cboPermanentAddress, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopupExt(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    Inventec.Common.Controls.PopupLoader.PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceBlood_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboProvinceBlood.EditValue != null) LoadDistrictsBloodCombo(cboProvinceBlood.EditValue.ToString());
                cboDistrictBlood.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboProvince.EditValue != null) LoadDistrictsCombo(cboProvince.EditValue.ToString());
                cboDistrict.Focus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDistrict.EditValue != null) LoadCommuneCombo(cboDistrict.EditValue.ToString());
                cboCommune.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
