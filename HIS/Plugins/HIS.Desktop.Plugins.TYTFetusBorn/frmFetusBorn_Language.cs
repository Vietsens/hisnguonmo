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

namespace HIS.Desktop.Plugins.TYTFetusBorn
{
    public partial class frmFetusBorn : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TYTFetusBorn.Resources.Lang", typeof(HIS.Desktop.Plugins.TYTFetusBorn.frmFetusBorn).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar8.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.bar8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem13.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.barButtonItem13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem14.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.barButtonItem14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxConSongCanNang.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxConSongCanNang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl16.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoFemale.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.rdoFemale.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoMale.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.rdoMale.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChildGender__Female.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildGender__Female.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChildGender__Male.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildGender__Male.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChildWeight.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildWeight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsK1.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkIsK1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBreastFeedingFirstHour.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkIsBreastFeedingFirstHour.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxChamSocSauSinh.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxChamSocSauSinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFirstWeekExam.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciFirstWeekExam.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExam742.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciExam742.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxTaiBienSanKhoa.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxTaiBienSanKhoa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsDeath.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkIsDeath.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsDeath.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsDeath.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciObstetricComplication.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciObstetricComplication.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxXetNghiemHIV.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxXetNghiemHIV.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHIVBefore.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkHIVBefore.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHIVBorn.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkHIVBorn.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHIVBefore.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciHIVBefore.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHIVBorn.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciHIVBorn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsChildDeath.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsChildDeath.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsFetusManage.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkIsFetusManage.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxSoLanKiemTra.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxSoLanKiemTra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheckCase33.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkCheckCase33.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheckCase43.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkCheckCase43.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheckCaseHaveNotYet.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkCheckCaseHaveNotYet.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckCase33.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCase33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckCase43.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCase43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckCaseHaveNotYet.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciCheckCaseHaveNotYet.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxTienSuThaiSan.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBoxTienSuThaiSan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonI__Refesh.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.barButtonI__Refesh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParaNormalCount.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciParaNormalCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParaPrematurelyCount.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciParaPrematurelyCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParaChildCount.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciParaChildCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParaMiscarriageCount.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciParaMiscarriageCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsUVFull.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.chkIsUVFull.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsFetusManage.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsFetusManage.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsUVFull.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsUVFull.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBornMethod.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciBornMethod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMidWifeName.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciMidWifeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChildStatus.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciChildStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsBreastFeedingFirstHour.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsBreastFeedingFirstHour.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsK1.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciIsK1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBornPlace.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.lciBornPlace.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton1.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.simpleButton1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton2.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.simpleButton2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.memoEdit1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmFetusBorn.memoEdit1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox3.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBox3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox4.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBox4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit2.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.checkEdit2.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.checkEdit3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit4.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.checkEdit4.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit7.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.checkEdit7.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton3.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.simpleButton3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton4.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.simpleButton4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.memoEdit2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmFetusBorn.memoEdit2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox5.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBox5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox6.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.groupBox6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit8.Properties.Caption = Inventec.Common.Resource.Get.Value("frmFetusBorn.checkEdit8.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("frmFetusBorn.layoutControlItem59.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
