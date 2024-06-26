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
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EnterKskInfomantion.Resources.Lang", typeof(HIS.Desktop.Plugins.EnterKskInfomantion.frmEnterKskInfomantion).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCodeForSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.txtServiceReqCodeForSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCodeForSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.txtTreatmentCodeForSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsFinish.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.chkIsFinish.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnfinish.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnUnfinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnfinish.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnUnfinish.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlTab1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFinish.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bbtnFinish.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnUnfinish.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bbtnUnfinish.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnF2.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bbtnF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExamTraditional.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.txtExamTraditional.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExamOccupationalTherapy.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.txtExamOccupationalTherapy.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.label3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label2.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.label2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.label1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChooseResult.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnChooseResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblBmiDisplayTextTab1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lblBmiDisplayTextTab1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMentalRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboMentalRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamENTRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamENTRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSurgeryRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboSurgeryRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamDermatologyRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamDermatologyRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamNeurologicalRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamNeurologicalRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamOENDRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamOENDRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamRepiratoryRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamRepiratoryRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamStomatologyRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamStomatologyRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEyeRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboEyeRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamKidneyUrologyRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamKidneyUrologyRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamMuscleBoneRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamMuscleBoneRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamDigestionRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamDigestionRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamCirculationRankTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamCirculationRankTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblKetQuaKhamLamSang.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lblKetQuaKhamLamSang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPLSucKhoeTab1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboPLSucKhoeTab1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamObsteticRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamObsteticRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamTraditionalRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamTraditionalRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamOccupationalTherapyRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamOccupationalTherapyRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamNutrionRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamNutrionRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBSketluan.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboBSketluan.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem40.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem43.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem45.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem47.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem48.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem48.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem69.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem69.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem69.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem69.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem70.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem70.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem71.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem71.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem139.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem139.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem141.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem141.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem141.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem141.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem65.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem65.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem65.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem65.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem67.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem67.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem67.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem67.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem143.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem143.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem145.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem145.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem146.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem146.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem146.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem146.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlTab2.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlTab2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem72.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem72.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem72.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem72.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem73.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem73.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem73.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem73.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem74.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem74.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem74.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem74.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem75.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem75.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem76.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem76.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem76.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem76.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem77.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem77.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem78.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem78.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlTab3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblBmiDisplayTextTab3.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lblBmiDisplayTextTab3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl11.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamLymphNodesRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamLymphNodesRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamCapillaryRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamCapillaryRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamCardiovascularRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamCardiovascularRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamHematopoieticRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamHematopoieticRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamNailRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamNailRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamMotionRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamMotionRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamMucosaRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamMucosaRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamCirculationRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamCirculationRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamDigestionRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamDigestionRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamMuscleBoneRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamMuscleBoneRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamKidneyUrologyRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamKidneyUrologyRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEyeRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboEyeRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamStomatologyRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamStomatologyRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamRepiratoryRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamRepiratoryRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamOENDRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamOENDRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamNeurologicalRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamNeurologicalRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamDermatologyRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamDermatologyRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSurgeryRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboSurgeryRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamENTRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamENTRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMentalRankTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboMentalRankTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl19.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl17.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.labelControl17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPLSucKhoeTab3.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboPLSucKhoeTab3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup8.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlGroup8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem79.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem80.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem80.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem81.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem81.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem14.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem15.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem82.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem82.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem85.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem85.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem86.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem86.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem89.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem89.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem90.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem90.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem93.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem93.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem94.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem94.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem95.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem95.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem96.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem96.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem97.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem97.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem97.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem97.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem98.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem98.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem99.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem99.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem101.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem101.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem103.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem103.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem104.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem104.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem104.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem104.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem106.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem106.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem107.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem107.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem108.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem108.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem110.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem110.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem112.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem112.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem113.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem113.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem114.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem114.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem115.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem115.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem117.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem117.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem118.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem118.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem119.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem119.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem126.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem126.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem127.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem127.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem127.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem127.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem16.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem19.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem20.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem22.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.emptySpaceItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem128.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem128.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem129.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem129.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem120.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem120.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem120.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem120.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem121.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem121.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem122.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem122.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem123.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem123.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem124.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem124.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem125.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem125.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem125.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem125.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem131.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem131.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem100.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem100.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem100.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem100.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem109.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem109.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem105.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem105.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem116.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem116.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem102.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem102.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem102.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem102.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem111.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem111.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem130.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem130.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem83.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem83.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem84.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem84.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem88.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem88.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem87.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem87.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem91.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem91.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem92.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem92.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem140.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem140.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxPatientInfo.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.groupBoxPatientInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlPatientInfo.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlPatientInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAddress.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciAddress.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem38.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem147.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem147.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKSKContract.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboKSKContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCodeForSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.txtPatientCodeForSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewServiceReq.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.gridViewServiceReq.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientName.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColExecuteDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomName.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColExecuteRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColInstrctionTime.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColInstrctionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBirthDate.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColBirthDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColGender.Caption = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.grdColGender.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceReqStt.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboServiceReqStt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartmentToSearch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboDepartmentToSearch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamRoomToSearch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.cboExamRoomToSearch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIntructionDate.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciIntructionDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKSKContract.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciKSKContract.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKSKContract.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.lciKSKContract.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem148.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem148.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem149.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.layoutControlItem149.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
