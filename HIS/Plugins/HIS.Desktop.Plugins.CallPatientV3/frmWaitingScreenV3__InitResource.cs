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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.CallPatientV3
{
    public partial class frmWaitingScreen5 : FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientV3.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatientV3.frmWaitingScreen5).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLastName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnLastName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFirstName.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnFirstName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAge.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnAge.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqStt.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnServiceReqStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionTime.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnInstructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqType.Caption = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.gridColumnServiceReqType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen_QY5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
