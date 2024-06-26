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

namespace HIS.Desktop.Plugins.HisPosition.HisPosition
{
    partial class frmHisPosition
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPosition.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPosition.HisPosition.frmHisPosition).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lciCode.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.lciCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciName.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.lciName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.STT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPositionCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnPositionCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPositionCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnPositionCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPositionName.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnPositionName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPositionName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnPositionName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDescription.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDescription.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnDescription.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsActive.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnIsActive.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPosition.gridColumnModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisPosition.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
