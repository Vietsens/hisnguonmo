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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
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

        Inventec.Desktop.Common.Modules.Module _currentModule;

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
                this.ValidationSingleControl(this.txtExpMestTemplateCode, this.dxValidationProvider);
                this.ValidationSingleControl(this.txtExpMestTemplateName, this.dxValidationProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
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
                            emteMedicineType.AMOUNT = item.AMOUNT ?? 0;
                            emteMedicineType.MEDICINE_TYPE_ID = item.ID;
                            if (service != null)
                                emteMedicineType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMedicineType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMedicineType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMedicineType.TUTORIAL = item.TUTORIAL;
                            emteMedicineType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);
                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                        else if (item.DataType == VATTU)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE emteMaterialType = new MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE();
                            emteMaterialType.AMOUNT = item.AMOUNT ?? 0;
                            emteMaterialType.MATERIAL_TYPE_ID = item.ID;
                            if (service != null)
                                emteMaterialType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMaterialType.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMaterialType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMaterialType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);
                            this.HisExpMestTemplateSDO.EmteMaterialTypes.Add(emteMaterialType);
                        }
                        else if (item.DataType == THUOC_DM)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE emteMedicineType = new MOS.EFMODEL.DataModels.HIS_EMTE_MEDICINE_TYPE();
                            emteMedicineType.MEDICINE_TYPE_ID = item.ID;
                            emteMedicineType.AMOUNT = item.AMOUNT ?? 0;
                            emteMedicineType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            emteMedicineType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            if (service != null)
                                emteMedicineType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMedicineType.IS_OUT_MEDI_STOCK = 1;
                            emteMedicineType.TUTORIAL = item.TUTORIAL;
                            emteMedicineType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);
                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                        else if (item.DataType == VATTU_DM)
                        {
                            MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE emteMaterialType = new MOS.EFMODEL.DataModels.HIS_EMTE_MATERIAL_TYPE();
                            emteMaterialType.AMOUNT = item.AMOUNT ?? 0;
                            emteMaterialType.MATERIAL_TYPE_ID = item.ID;
                            emteMaterialType.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            if (service != null)
                                emteMaterialType.SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
                            emteMaterialType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            emteMaterialType.IS_EXPEND = (item.IsExpend ? (short)1 : (short)0);
                            emteMaterialType.IS_OUT_MEDI_STOCK = 1;
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
                            this.HisExpMestTemplateSDO.EmteMedicineTypes.Add(emteMedicineType);
                        }
                    }
                }
                MediMatyTypeADO mediMatyTypeADO = this.mediMatyTypeADOs.FirstOrDefault(o => o.RemedyCount > 0);
                if (mediMatyTypeADO != null)
                {
                    this.HisExpMestTemplateSDO.ExpMestTemplate.REMEDY_COUNT = mediMatyTypeADO.RemedyCount;
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources.ResourceLanguageManager.LanguagefrmHisExpMestTemplateCreate = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription.frmHisExpMestTemplateCreate).Assembly);

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
                this.SetCaptionByLanguageKey();
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
