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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmHisExpMestTemplateCreate : HIS.Desktop.Utility.FormBase
    {
        public List<MediMatyTypeADO> mediMatyTypeADOs;
        public RefeshReference refeshData;
        int positionHandle = -1;

        const int THUOC = 1;
        const int VATTU = 2;
        const int THUOC_DM = 3;
        const int VATTU_DM = 4;
        const int THUOC_TUTUC = 5;

        MOS.SDO.HisExpMestTemplateSDO HisExpMestTemplateSDO { get; set; }
        MOS.SDO.HisExpMestTemplateSDO HisExpMestTemplateResultSDO { get; set; }
        public Inventec.Desktop.Common.Modules.Module _currentModule;

        public frmHisExpMestTemplateCreate(List<MediMatyTypeADO> mediMatyTypeADOs, RefeshReference refeshData, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();

            this.mediMatyTypeADOs = mediMatyTypeADOs;
            this.refeshData = refeshData;
            this._currentModule = module;
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandle == -1)
                {
                    this.positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandle > edit.TabIndex)
                {
                    this.positionHandle = edit.TabIndex;
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

        private void ValidControls()
        {
            try
            {
                this.ValidationSingleControl(this.txtExpMestTemplateCode, 10, true);
                this.ValidationSingleControl(this.txtExpMestTemplateName, 100, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, int? maxLength, [Optional] bool IsRequest)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave__ExpMestTemplate_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            WaitingManager.Show();
            try
            {
                this.positionHandle = -1;
                if (!this.dxValidationProvider.Validate())
                {
                    WaitingManager.Hide();
                    return;
                }
                if (this.mediMatyTypeADOs == null || this.mediMatyTypeADOs.Count == 0)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
                    WaitingManager.Hide();
                    return;
                }
                else
                {
                    this.ProcessDataInputExpMestMedicineTemplate();
                    this.HisExpMestTemplateSDO.ExpMestTemplate.IS_PUBLIC = (short)(this.chkPublic.Checked ? 1 : 0);
                    this.HisExpMestTemplateResultSDO = new BackendAdapter(param).Post<MOS.SDO.HisExpMestTemplateSDO>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_TEMPLATE_CREATE, ApiConsumers.MosConsumer, this.HisExpMestTemplateSDO, ProcessLostToken, param);
                    WaitingManager.Hide();
                    if (this.HisExpMestTemplateResultSDO != null)
                    {
                        success = true;
                        if (this.refeshData != null)
                            this.refeshData();

                        this.Close();
                    }

                    MessageManager.Show(this, param, success);
                }

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        internal double GetValueSpin(string strValue)
        {
            double value = 0;
            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    string vl = strValue;
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            value = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        value = Convert.ToDouble(vl);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        private void ProcessDataInputExpMestMedicineTemplate()
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    this.HisExpMestTemplateSDO.EmteMedicineTypes.Clear();
                    this.HisExpMestTemplateSDO.EmteMaterialTypes.Clear();

                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        var service = BackendDataWorker.Get<V_HIS_SERVICE>().SingleOrDefault(o => o.ID == item.SERVICE_ID);

                        if (item.DataType == THUOC)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE emteMedicineType = new MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE();

                            double sang, trua, chieu, toi = 0;
                            sang = this.GetValueSpin(item.Sang);
                            trua = this.GetValueSpin(item.Trua);
                            chieu = this.GetValueSpin(item.Chieu);
                            toi = this.GetValueSpin(item.Toi);
                            double tongCong = (double)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((sang + trua + chieu + toi) * (double)item.UseDays));

                            if (
                                (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (item.IS_SPLIT_COMPENSATION.HasValue && item.IS_SPLIT_COMPENSATION == 1))
                                && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                                && (item.IsAllowOdd == null || item.IsAllowOdd.Value == false)
                                && (int)tongCong != tongCong
                                )
                            {
                                emteMedicineType.AMOUNT = (item.CONVERT_RATIO ?? 0) > 0 ? (decimal)tongCong / (item.CONVERT_RATIO ?? 1) : (decimal)tongCong;
                            }
                            else
                                emteMedicineType.AMOUNT = (item.CONVERT_RATIO ?? 0) > 0 ? ((item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1)) : (item.AMOUNT ?? 0);

                            emteMedicineType.MEDICINE_TYPE_ID = item.ID;
                            if (service != null)
                                emteMedicineType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMedicineType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMedicineType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMedicineType.TUTORIAL = item.TUTORIAL;
                            if (!String.IsNullOrEmpty(item.Sang))
                                emteMedicineType.MORNING = item.Sang;
                            if (!String.IsNullOrEmpty(item.Trua))
                                emteMedicineType.NOON = item.Trua;
                            if (!String.IsNullOrEmpty(item.Chieu))
                                emteMedicineType.AFTERNOON = item.Chieu;
                            if (!String.IsNullOrEmpty(item.Toi))
                                emteMedicineType.EVENING = item.Toi;
                            if (item.UseDays.HasValue)
                                emteMedicineType.DAY_COUNT = (long?)item.UseDays;
                            emteMedicineType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);

                            if (item.IS_SUB_PRES == 1)
                            {
                                emteMedicineType.IS_OUT_MEDI_STOCK = null;
                            }

                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                        else if (item.DataType == VATTU)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE emteMaterialType = new MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE();

                            emteMaterialType.AMOUNT = (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1);

                            emteMaterialType.MATERIAL_TYPE_ID = item.ID;
                            if (service != null)
                                emteMaterialType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMaterialType.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMaterialType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMaterialType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);

                            if (item.IS_SUB_PRES == 1)
                            {
                                emteMaterialType.IS_OUT_MEDI_STOCK = null;
                            }

                            this.HisExpMestTemplateSDO.EmteMaterialTypes.Add(emteMaterialType);
                        }
                        else if (item.DataType == THUOC_DM)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE emteMedicineType = new MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE();
                            emteMedicineType.MEDICINE_TYPE_ID = item.ID;
                            emteMedicineType.AMOUNT = (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1);
                            emteMedicineType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMedicineType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            if (service != null)
                                emteMedicineType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMedicineType.IS_OUT_MEDI_STOCK = 1;
                            emteMedicineType.TUTORIAL = item.TUTORIAL;
                            emteMedicineType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);

                            if (item.IS_SUB_PRES == 1)
                            {
                                emteMedicineType.IS_OUT_MEDI_STOCK = null;
                            }

                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                        else if (item.DataType == VATTU_DM)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE emteMaterialType = new MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE();
                            emteMaterialType.AMOUNT = (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 1);
                            emteMaterialType.MATERIAL_TYPE_ID = item.ID;
                            emteMaterialType.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            if (service != null)
                                emteMaterialType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMaterialType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMaterialType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);
                            emteMaterialType.IS_OUT_MEDI_STOCK = 1;

                            if (item.IS_SUB_PRES == 1)
                            {
                                emteMaterialType.IS_OUT_MEDI_STOCK = null;
                            }

                            this.HisExpMestTemplateSDO.EmteMaterialTypes.Add(emteMaterialType);
                        }
                        else if (item.DataType == THUOC_TUTUC)//thuoc tu tuc
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE emteMedicineType = new MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE();
                            emteMedicineType.AMOUNT = item.AMOUNT ?? 0;
                            emteMedicineType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMedicineType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMedicineType.IS_OUT_MEDI_STOCK = 1;
                            emteMedicineType.TUTORIAL = item.TUTORIAL;
                            emteMedicineType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);

                            if (item.IS_SUB_PRES == 1)
                            {
                                emteMedicineType.IS_OUT_MEDI_STOCK = null;
                            }

                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                    }
                }

                this.HisExpMestTemplateSDO.ExpMestTemplate.EXP_MEST_TEMPLATE_CODE = this.txtExpMestTemplateCode.Text;
                this.HisExpMestTemplateSDO.ExpMestTemplate.EXP_MEST_TEMPLATE_NAME = this.txtExpMestTemplateName.Text;
                this.HisExpMestTemplateSDO.ExpMestTemplate.DESCRIPTION = this.txtExpMestTemplateDescription.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmHisExpMestTemplateCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.btnSave__ExpMestTemplate.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.btnSave__ExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription.frmHisExpMestTemplateCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.btnSave__ExpMestTemplate.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.btnSave__ExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplateCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisExpMestTemplateCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKeyNew();
                this.ValidControls();

                this.HisExpMestTemplateSDO = new MOS.SDO.HisExpMestTemplateSDO();
                this.HisExpMestTemplateSDO.EmteMedicineTypes = new List<MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE>();
                this.HisExpMestTemplateSDO.EmteMaterialTypes = new List<MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE>();
                this.HisExpMestTemplateSDO.ExpMestTemplate = new MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.btnSave__ExpMestTemplate_Click(null, null);
        }

        private void txtExpMestTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtExpMestTemplateName.Focus();
                    this.txtExpMestTemplateName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExpMestTemplateName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtExpMestTemplateDescription.Focus();
                    this.txtExpMestTemplateDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExpMestTemplateDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkPublic.Properties.FullFocusRect = true;
                    this.chkPublic.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPublic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave__ExpMestTemplate.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
