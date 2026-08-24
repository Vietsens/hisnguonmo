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
using System.Resources;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Multilingual captions. Every literal shown on screen must come from Resources/Lang.*.resx.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        /// <summary>Key prefix of every entry belonging to this form inside Lang.*.resx.</summary>
        private const string LANG_PREFIX = "FormHisTreatmentRecordChecking.";

        /// <summary>
        /// Reads one localised value. Accepts the key WITHOUT the form prefix.
        /// Returns an empty string when the key is missing so the UI never shows an exception.
        /// </summary>
        private string GetLangValue(string keyWithoutPrefix)
        {
            try
            {
                if (Resources.ResourceLanguageManager.LanguageResource == null)
                {
                    Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                        "HIS.Desktop.Plugins.HisTreatmentRecordChecking.Resources.Lang",
                        typeof(FormHisTreatmentRecordChecking).Assembly);
                }

                return Inventec.Common.Resource.Get.Value(
                    LANG_PREFIX + keyWithoutPrefix,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Applies the current language to every caption of the form.
        /// MUST be called from the Load event, before FillDataToGrid().
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisTreatmentRecordChecking.Resources.Lang",
                    typeof(FormHisTreatmentRecordChecking).Assembly);

                // ----- Buttons -----
                this.BtnSearch.Text = GetLangValue("BtnSearch.Text");
                this.btnKhongDat.Text = GetLangValue("btnKhongDat.Text");
                this.btnKhongDat.ToolTip = GetLangValue("btnKhongDat.ToolTip");
                this.btnDat.Text = GetLangValue("btnDat.Text");
                this.btnDat.ToolTip = GetLangValue("btnDat.ToolTip");
                this.btnDuyet.Text = GetLangValue("btnDuyet.Text");
                this.btnDuyet.ToolTip = GetLangValue("btnDuyet.ToolTip");
                this.btnHuyDuyet.Text = GetLangValue("btnHuyDuyet.Text");

                // ----- Bar items (keyboard shortcuts) -----
                this.barButtonItem1.Caption = GetLangValue("barButtonItem1.Caption");
                this.bbtnKhongDat.Caption = GetLangValue("bbtnKhongDat.Caption");
                this.bbtnDat.Caption = GetLangValue("bbtnDat.Caption");
                this.bbtnHuyDuyet.Caption = GetLangValue("bbtnHuyDuyet.Caption");

                // ----- Filter check boxes -----
                this.chkUuTien.Properties.Caption = GetLangValue("chkUuTien.Text");
                this.chkUuTien.ToolTip = GetLangValue("chkUuTien.ToolTip");
                this.chkToiTao.Properties.Caption = GetLangValue("chkToiTao.Text");
                this.chkToiTao.ToolTip = GetLangValue("chkToiTao.ToolTip");
                this.chkIncludeCancelDoc.Properties.Caption = GetLangValue("chkIncludeCancelDoc.Text");

                // ----- Filter row added by task 53180 -----
                this.lciRequestDoctor.Text = GetLangValue("lciRequestDoctor.Text");
                this.lciFromDate.Text = GetLangValue("lciFromDate.Text");
                this.lciToDate.Text = GetLangValue("lciToDate.Text");
                this.lciTreatmentStatus.Text = GetLangValue("lciTreatmentStatus.Text");
                this.chkNoDocument.Properties.Caption = GetLangValue("chkNoDocument.Text");
                this.chkNoDocument.ToolTip = GetLangValue("chkNoDocument.ToolTip");
                this.chkNotFullySigned.Properties.Caption = GetLangValue("chkNotFullySigned.Text");
                this.chkNotFullySigned.ToolTip = GetLangValue("chkNotFullySigned.ToolTip");

                // ----- Patient information group -----
                this.LcgPatientInfo.Text = GetLangValue("LcgPatientInfo.Text");
                this.LciPatientCode.Text = GetLangValue("LciPatientCode.Text");
                this.LciPatientName.Text = GetLangValue("LciPatientName.Text");
                this.LciDob.Text = GetLangValue("LciDob.Text");
                this.LciGender.Text = GetLangValue("LciGender.Text");
                this.LciPatientType.Text = GetLangValue("LciPatientType.Text");
                this.LciHeinNumber.Text = GetLangValue("LciHeinNumber.Text");
                this.LciHeinTime.Text = GetLangValue("LciHeinTime.Text");
                this.LciMediOrg.Text = GetLangValue("LciMediOrg.Text");
                this.LciAddress.Text = GetLangValue("LciAddress.Text");
                this.LciMainIcd.Text = GetLangValue("LciMainIcd.Text");
                this.LciSubIcd.Text = GetLangValue("LciSubIcd.Text");
                this.LciIcdYhct.Text = GetLangValue("LciIcdYhct.Text");
                this.LciSubIcdYhct.Text = GetLangValue("LciSubIcdYhct.Text");
                this.LciNote.Text = GetLangValue("LciNote.Text");
                this.layoutControlItem8.Text = GetLangValue("layoutControlItem8.Text");

                // ----- Document type grid (left) -----
                this.Gr_Gc_Name.Caption = GetLangValue("Gr_Gc_Name.Caption");

                // ----- Treatment grid (only visible when opened from another module) -----
                this.Gv_Treatment_Gc_STT.Caption = GetLangValue("Gv_Treatment_Gc_STT.Caption");
                this.Gv_Treatment_Gc_PatientName.Caption = GetLangValue("Gv_Treatment_Gc_PatientName.Caption");
                this.Gv_Treatment_Gc_PatientDOB.Caption = GetLangValue("Gv_Treatment_Gc_PatientDOB.Caption");
                this.Gv_Treatment_Gc_TreatmentCode.Caption = GetLangValue("Gv_Treatment_Gc_TreatmentCode.Caption");

                // ----- Document grid (right) -----
                this.Gv_ED_Gc_STT.Caption = GetLangValue("Gv_ED_Gc_STT.Caption");
                this.Gv_ED_Gc_View.Caption = GetLangValue("Gv_ED_Gc_View.Caption");
                this.Gv_ED_Gc_Name.Caption = GetLangValue("Gv_ED_Gc_Name.Caption");
                this.Gv_ED_Gc_HisCode.Caption = GetLangValue("Gv_ED_Gc_HisCode.Caption");
                this.Gv_ED_Gc_Creator.Caption = GetLangValue("Gv_ED_Gc_Creator.Caption");
                this.Gv_ED_Gc_Signers.Caption = GetLangValue("Gv_ED_Gc_Signers.Caption");
                this.Gv_ED_Gc_UnSigners.Caption = GetLangValue("Gv_ED_Gc_UnSigners.Caption");
                this.Gv_ED_Gc_CreateTime.Caption = GetLangValue("Gv_ED_Gc_CreateTime.Caption");
                this.Gv_ED_Gc_DocumentTime.Caption = GetLangValue("Gv_ED_Gc_DocumentTime.Caption");

                // ----- Order grid (middle). The other captions are language driven
                //       inside ProcessCaptionGridInfoRecord() because they depend on the document type.
                this.Gv_IR_STT.Caption = GetLangValue("Gv_IR_STT.Caption");
                this.Gv_IR_Creator.ToolTip = GetLangValue("Gv_IR_Creator.ToolTip");
                this.Gv_IR_CreateTimeReal.Caption = GetLangValue("Gv_IR_CreateTimeReal.Caption");
                this.Gv_IR_DocStatus.Caption = GetLangValue("Gv_IR_DocStatus.Caption");
                this.Gv_IR_PatientCode.Caption = GetLangValue("Gv_IR_PatientCode.Caption");
                this.Gv_IR_PatientName.Caption = GetLangValue("Gv_IR_PatientName.Caption");
                this.Gv_IR_TreatmentCode.Caption = GetLangValue("Gv_IR_TreatmentCode.Caption");
                this.Gv_IR_CreateDoc.Caption = GetLangValue("Gv_IR_CreateDoc.Caption");
                this.Gv_IR_CreateDoc.ToolTip = GetLangValue("Gv_IR_CreateDoc.ToolTip");

                // Nhan tren nut trong tung dong luoi. Buttons duoc nap o Designer nen luon co phan tu 0,
                // van kiem tra Count de khong lap lai loi truy cap collection rong.
                if (this.repositoryItemButtonCreateDoc.Buttons.Count > 0)
                {
                    this.repositoryItemButtonCreateDoc.Buttons[0].Caption =
                        GetLangValue("repositoryItemButtonCreateDoc.Caption");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
