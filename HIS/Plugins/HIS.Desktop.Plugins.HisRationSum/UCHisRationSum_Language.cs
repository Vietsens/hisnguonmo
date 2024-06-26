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
using HIS.Desktop.Plugins.HisRationSum.Base;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisRationSum
{
    public partial class UCHisRationSum : HIS.Desktop.Utility.UserControlBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.LanguageUCHisRationSum = new ResourceManager("HIS.Desktop.Plugins.HisRationSum.Resources.Lang", typeof(HIS.Desktop.Plugins.HisRationSum.UCHisRationSum).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.layoutControl1.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridSereServColumnSTT.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridSereServColumnSTT.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridSereServColumnServiceName.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridSereServColumnServiceName.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridSereServColumnPrice.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridSereServColumnPrice.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridSereServColumnAmount.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridSereServColumnAmount.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewColumnSereServIntructionDate.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewColumnSereServIntructionDate.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn2.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn3.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn4.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn5.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn6.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.btnSearch.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.cboRequestDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisRationSum.cboRequestDepartment.Properties.NullText", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.RadioHasSum.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.RadioHasSum.Properties.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.radioHasNotSum.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.radioHasNotSum.Properties.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisRationSum.txtKeyword.Properties.NullValuePrompt", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.btnSum.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.btnSum.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_STT.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_STT.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_DELETE.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_DELETE.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_RoomName.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_RoomName.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_IntructionDate.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_IntructionDate.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_RationTimeName.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_RationTimeName.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_CreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_CreateTime.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewRationSumColumn_Creator.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewRationSumColumn_Creator.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_STT.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_STT.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_ServiceReqStt.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_ServiceReqStt.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn8.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn1.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumn1.ToolTip", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridColumnServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridColumnServiceReqCode.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_PatientName.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_PatientName.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_PatientDob.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_PatientDob.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_PatientGenDerName.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_PatientGenDerName.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_RequestDepartment.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_RequestDepartment.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_TreatmentCode.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_PatientCode.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_PatientCode.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.gridViewServiceReq_CreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.gridViewServiceReq_CreateTime.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.GcCreator.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.GcModifyTime.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisRationSum.GcModifier.Caption", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.lciKeyword.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.lciKeyword.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.lciRadioSumStatus.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.lciRadioSumStatus.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.lciRequestDepartmentCode.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.lciRequestDepartmentCode.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                this.lciRooms.Text = Inventec.Common.Resource.Get.Value("UCHisRationSum.lciRooms.Text", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
