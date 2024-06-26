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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
namespace TYT.Desktop.Plugins.TYTTuberculosis
{
    public partial class frm : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("TYT.Desktop.Plugins.TYTTuberculosis.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTTuberculosis.frm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frm.layoutControl1.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frm.btnRefresh.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frm.btnUpdate.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnSTT.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnLock.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnDelete.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientCode.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnPatientCode.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnVirPersonName.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnVirPersonName.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDob.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnDob.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnGenderName.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnGenderName.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCareerName.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnCareerName.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInTimeStr.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnInTimeStr.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDtcksCode.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnDtcksCode.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDiseaseType.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnDiseaseType.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTreatmentResult.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnTreatmentResult.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnNote.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnNote.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnCreateTime.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnCreator.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifierTime.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnModifierTime.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frm.gridColumnModifier.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frm.bar1.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frm.barButtonItem__Save.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonI__Refesh.Caption = Inventec.Common.Resource.Get.Value("frm.barButtonI__Refesh.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frm.bbtnEdit.Caption", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboResult.Properties.NullText = Inventec.Common.Resource.Get.Value("frm.cboResult.Properties.NullText", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frm.btnDelete.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frm.btnSave.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frm.layoutControlItem1.OptionsToolTip.ToolTip", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem1.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem9.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layout.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frm.layout.OptionsToolTip.ToolTip", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layout.Text = Inventec.Common.Resource.Get.Value("frm.layout.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frm.layoutControlItem5.OptionsToolTip.ToolTip", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem5.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frm.layoutControlItem2.OptionsToolTip.ToolTip", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frm.layoutControlItem2.Text", TYT.Desktop.Plugins.TYTKhh.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = this.currentModule != null ? this.currentModule.text : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
